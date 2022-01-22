using System.Collections;
using System.Collections.Generic;
using System;

namespace TestBattle
{
    public interface IBattleFlow
    {
        int Priority { get; }
        void StartPhase(int phase_id);
        void StartTurn(Type_BattleCamp camp, int turn);
        void EndTurn(Type_BattleCamp camp, int turn);
        void EndPhase(int phase_id);
        void EndSkill();
    }

    public class BattleFlowManager : BattleBaseManager
    {
        public int Phase = 0;
        public int Round = 0;
        public Type_BattleCamp CurrentActiveCamp = Type_BattleCamp.None;

        private List<int> _flow_priority = new List<int>();
        private Dictionary<int, List<IBattleFlow>> _priority_flows = new Dictionary<int, List<IBattleFlow>>(); 
        public override void OnInit()
        {
        }

        public override void Start()
        {
            base.Start();
            this.Finish = false;
        }

        public override void OnRelease()
        {
        }

        public void AddBattleFlow(IBattleFlow flow)
        {
            int priority = flow.Priority;
            if (this._flow_priority.Count == 0)
            {
                this._flow_priority.Add(flow.Priority);
            }
            else
            {
                for (int i = 0; i < this._flow_priority.Count; i++)
                {
                    if (priority == this._flow_priority[i])
                    {
                        break;
                    }
                    if (priority > this._flow_priority[i])
                    {
                        this._flow_priority.Insert(i, priority);
                        break;
                    }
                }
            }
            List<IBattleFlow> flows = null;
            if (!this._priority_flows.TryGetValue(priority, out flows)) {
                flows = new List<IBattleFlow>();
                this._priority_flows.Add(priority, flows);
            }
            flows.Add(flow);
        }

        public void RemoveBattleFlow(IBattleFlow flow)
        {
            List<IBattleFlow> flows = null;
            if (this._priority_flows.TryGetValue(flow.Priority, out flows))
            {
                flows.Remove(flow);
            }
        }

        public void StartPhase(int phase) {
            this.Phase = phase;
            this.Round = 0;
            this.CurrentActiveCamp = Type_BattleCamp.Ally;
            for (int i = 0; i < this._flow_priority.Count; i++) {
                int priority = this._flow_priority[i];
                List<IBattleFlow> flows = null;
                if (this._priority_flows.TryGetValue(priority, out flows))
                {
                    for (int j = 0; j < flows.Count; j++) {
                        flows[j].StartPhase(phase);
                    }
                }
            }
            this.StartTurn(this.CurrentActiveCamp);
        }

        protected void FinishPhase()
        {
            for (int i = 0; i < this._flow_priority.Count; i++)
            {
                int priority = this._flow_priority[i];
                List<IBattleFlow> flows = null;
                if (this._priority_flows.TryGetValue(priority, out flows))
                {
                    for (int j = 0; j < flows.Count; j++)
                    {
                        flows[j].EndPhase(this.Phase);
                    }
                }
            }
               
        }

        protected void StartTurn(Type_BattleCamp current_camp) {
            for (int i = 0; i < this._flow_priority.Count; i++)
            {
                int priority = this._flow_priority[i];
                List<IBattleFlow> flows = null;
                if (this._priority_flows.TryGetValue(priority, out flows))
                {
                    for (int j = 0; j < flows.Count; j++)
                    {
                        flows[j].StartTurn(this.CurrentActiveCamp, this.Round);
                    }
                }
            }

        }

        protected void FinishTurn(Type_BattleCamp current_camp)
        {
            for (int i = 0; i < this._flow_priority.Count; i++)
            {
                int priority = this._flow_priority[i];
                List<IBattleFlow> flows = null;
                if (this._priority_flows.TryGetValue(priority, out flows))
                {
                    for (int j = 0; j < flows.Count; j++)
                    {
                        flows[j].EndTurn(current_camp, this.Round);
                    }
                }
            }
        }

        protected void FinishSkill()
        {
            for (int i = 0; i < this._flow_priority.Count; i++)
            {
                int priority = this._flow_priority[i];
                List<IBattleFlow> flows = null;
                if (this._priority_flows.TryGetValue(priority, out flows))
                {
                    for (int j = 0; j < flows.Count; j++)
                    {
                        flows[j].EndSkill();
                    }
                }
            }
        }

        public bool BeginUnitTurn(CardData card, int target_uid = 0) {
            if (card == null)
                return false;
            int unit_id = card.BattleUnitID;
            int skill_id = card.SkillID;
            BattleUnit unit = this.GetManager<BattleUnitManager>().GetUnit(unit_id);
            if (unit == null) {
                throw new Exception("null unit");
            }
            if (unit.Camp != this.CurrentActiveCamp)
            {
                throw new Exception("not in turn:" + unit.Camp + ",uid:" + unit_id + ",skill_id:" + skill_id);
            }
            if (unit.IsDead)
            {
                throw new Exception("dead man cannot cast skill,uid:" + unit_id + ",skill_id:" + skill_id);
            }
            unit.BeginSelfTurn();
            this.TriggerSkill(Type_SkillTriggerTime.SelfTurnBegin, unit);
            unit.OneSkillDatas.Clear();
            BattleTeam team = this.Battle.GetManager<BattleUnitManager>().GetCurrentTeam(unit.Camp);
            if (unit.CanCastSkill() && !unit.IsSkillLock(card.SkillID,card.Rank))
            {
                SkillData sd = null;
                SkillCalculateInfo info = unit.GetSkillCastInfo(card, target_uid);
                BattleLog.Log(string.Format("{0} cast skill : {1} with rank {2} succ", unit.UnitLogInfo, card.SkillID, card.Rank));
                sd = this.GetManager<BattleCalculator>().CalculateSkill(info);

                this.TriggerSkill(Type_SkillTriggerTime.CastSkill, unit);

                if (unit.ChangeAwakenGage(card.Rank) == 1)
                {
                    this.Battle.GetManager<BattleUnitManager>().GetCurrentTeam(unit.Camp).CardManager.DrawAwakenCard(unit.UnitID);
                }
                unit.OneSkillDatas.Add(sd);
                if (card.SkillData.SkillType == Type_Skill.Awaken) {
                    unit.ChangeAwakenGage(-unit.AwakenGageMaxPoint);
                }
                team.CardManager.ReclaimCard(card);
                team.ChangePlayCount(-1);
                this.AfterSkill(sd);

                BuffSkillInspireCheckModifier inspire_check = unit.BuffManager.GetModifier<BuffSkillInspireCheckModifier>();
                if (inspire_check.CheckInspireSucc())
                {
                    info = unit.GetSkillCastInfo(unit.BaseSkill, target_uid);
                    sd = this.GetManager<BattleCalculator>().CalculateSkill(info);
                    this.TriggerSkill(Type_SkillTriggerTime.CastSkill, unit);
                    unit.OneSkillDatas.Add(sd);
                    this.AfterSkill(sd);
                }
                
            }
            else
            {
                team.CardManager.ReclaimCard(card);
                team.ChangePlayCount(-1);
                BattleLog.Log(string.Format("{0} cast skill : {1} with rank {2} failed", unit.UnitLogInfo, card.SkillID, card.Rank));
            }
            
            this.TriggerSkill(Type_SkillTriggerTime.SelfTurnEnd, unit);
            
            
            return true;
        }

        public void AfterSkill(SkillData skill_data) {
            this.FinishSkill();
            BattleUnit unit = this.GetManager<BattleUnitManager>().GetUnit(skill_data.CasterID);
            List<int> hit_targets = skill_data.GetTargetsIDs();
            if (hit_targets.Count > 0)
            {
                for (int i = 0; i < hit_targets.Count; i++)
                {
                    this.TriggerSkill(Type_SkillTriggerTime.SkillHitTarget, unit, hit_targets[i]);

                    BattleUnit target = this.GetManager<BattleUnitManager>().GetUnit(hit_targets[i]);
                    if (target != null)
                    {
                        this.TriggerSkill(Type_SkillTriggerTime.GetHit, target, unit.UnitID);
                    }
                    if (target.IsDead) {
                        this.TriggerSkill(Type_SkillTriggerTime.KillTarget, unit, target.UnitID);
                    }
                }
            }

            List<int> critical_targets = skill_data.CriticalTargets;
            if (critical_targets.Count > 0)
            {
                this.TriggerSkill(Type_SkillTriggerTime.HitCritical, unit, unit.UnitID);
            }
            for (int i = 0; i < critical_targets.Count; i++)
            {
                int target_id = critical_targets[i];
                BattleUnit target = this.GetManager<BattleUnitManager>().GetUnit(hit_targets[i]);
                if (target != null)
                {
                    this.TriggerSkill(Type_SkillTriggerTime.HitCritical, target, unit.UnitID);
                }
            }

            unit.BuffManager.GetModifier<BuffAfterSkillCheckRemoveModifier>().CheckAfterSkillBuffRemove();
            for (int i = 0; i < hit_targets.Count; i++)
            {
                BattleUnit target = this.GetManager<BattleUnitManager>().GetUnit(hit_targets[i]);
                target.BuffManager.GetModifier<BuffAfterSkillCheckRemoveModifier>().CheckAfterSkillBuffRemove();
            }
        }
        public void TriggerSkill(Type_SkillTriggerTime trigger_type,BattleUnit caster, int target_id = 0)
        {
            if (caster == null)
                return;
            List<CardData> skills = caster.GetTriggerSkills(trigger_type);
            if (skills != null)
            {
                for (int i = 0; i < skills.Count; i++)
                {
                    SkillCalculateInfo info = caster.GetSkillCastInfo(skills[i], target_id);
                    BattleLog.Log(string.Format("{0}-----trigger skill:{1},target : {2}", caster.UnitLogInfo, trigger_type,target_id));
                    SkillData sd = this.GetManager<BattleCalculator>().CalculateSkill(info);
                    caster.OneSkillDatas.Add(sd);
                    this.AfterSkill(sd);
                }
            }
        }

        //
        public Type_BattleTurnEndState CheckUnitEndTurn(int unit_id) {
            BattleUnit unit = this.GetManager<BattleUnitManager>().GetUnit(unit_id);
            BattleTeam team = this.Battle.GetManager<BattleUnitManager>().GetCurrentTeam(unit.Camp);
            unit.EndSelfTurn();
            Type_BattleTurnEndState state = Type_BattleTurnEndState.None;
            state = this.CheckBattleState();
            if (state == Type_BattleTurnEndState.None)
            {
                if (team.ShouldFinishTurn)
                {
                    state = Type_BattleTurnEndState.TeamTurnEnd;
                }
            }
            else
            {
                if (state == Type_BattleTurnEndState.Lose || state == Type_BattleTurnEndState.Win || state == Type_BattleTurnEndState.PhaseEnd)
                {
                    Battle.GetManager<BattleEventManager>().SendMessage(BattleEvent.BattleRoundEnd, this, BattleRoundEndEventData.CreateEventData(state, this.Phase, this.Round));

                }
            }
            return state;
        }

        public Type_BattleTurnEndState CheckBattleState() {
            BattleUnitManager um = this.Battle.GetManager<BattleUnitManager>();
            BattleTeam ally = um.GetCurrentTeam(Type_BattleCamp.Ally);
            if (ally.SurviveUnitCount() == 0)
                return Type_BattleTurnEndState.Lose;
            BattleTeam enemy = um.GetCurrentTeam(Type_BattleCamp.Enemy);
            if (enemy.SurviveUnitCount() == 0)
            {
                if (this.Phase == this.Battle.BattleData.PhaseCount - 1)
                    return Type_BattleTurnEndState.Win;
                return Type_BattleTurnEndState.PhaseEnd;
            }
            return Type_BattleTurnEndState.None;
        }

        public void StartTeamTurn(Type_BattleCamp camp)
        {
            this.StartTurn(camp);
        }
        public void TerminateTeamTurn(Type_BattleCamp camp) {
            this.FinishTurn(camp);
            this.CurrentActiveCamp = (Type_BattleCamp)(((int)camp + 1) % 2);
            if (this.CheckEndRound()) {
                Battle.GetManager<BattleEventManager>().SendMessage(BattleEvent.BattleRoundEnd, this, BattleRoundEndEventData.CreateEventData(Type_BattleTurnEndState.None, this.Phase, this.Round));
                this.Round++;
            }
        }

        public bool CheckEndRound() {
            bool all_team_acted = true;
            foreach (var kvp in this.GetManager<BattleUnitManager>().Teams)
            {
                if (kvp.Value.CurrentTurn != this.Round)
                {
                    all_team_acted = false;
                }
            }
            return all_team_acted;
        }

        public bool IsPhaseFinish() {
            foreach (var kvp in this.GetManager<BattleUnitManager>().Teams) {
                if (kvp.Value.SurviveUnitCount() == 0)
                    return true;
            }
            return false;
        }

        public bool IsBattleFinish() {
            return this.IsPhaseFinish() && this.Phase == this.Battle.BattleData.PhaseCount;
        }

        public bool Finish = false;

    }
}
