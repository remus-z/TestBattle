using System.Collections;
using System.Collections.Generic;
using System;
namespace TestBattle
{
    public class BattleUnitManager : BattleBaseManager, IBattleFlow
    {
        private int unit_id = 1;
        public Dictionary<int, BattleTeam> Teams { get; } = new Dictionary<int, BattleTeam>();//current phase team

        private BattleTeam _ally_team;

        private List<BattleTeam> _enemy_teams = new List<BattleTeam>();

        public override void OnInit()
        {
            this.unit_id = 1;
            this.Battle.GetManager<BattleFlowManager>().AddBattleFlow(this);

            this.GetManager<BattleEventManager>().AddListener(BattleEvent.BattleUnitDead, this._OnBattleUnitDead);
        }

        public override void Start() {
            this._ally_team = this._CreateTeam(Type_BattleCamp.Ally, this.Battle.BattleData.AllyTeamData, this.Battle.BattleData.AllyLeader);
            Teams.Add((int)Type_BattleCamp.Ally, _ally_team);

            for (int i = 0; i < this.Battle.BattleData.EnemyTeamsData.Count; i++) {
                BattleTeam enemy_team = this._CreateTeam(Type_BattleCamp.Enemy, this.Battle.BattleData.EnemyTeamsData[i], this.Battle.BattleData.EnemyLeaders[i]);
                this._enemy_teams.Add(enemy_team);
            }

            List<BattleUnit> units = this._ally_team.GetSurviveUnits();
            for (int i = 0; i < units.Count; i++)
            {
                this.Battle.GetManager<BattleFlowManager>().TriggerSkill(Type_SkillTriggerTime.BattleStart, units[i]);
            }


            foreach (var team in _enemy_teams)
            {
                units = team.GetSurviveUnits();
                for (int i = 0; i < units.Count; i++) {
                    this.Battle.GetManager<BattleFlowManager>().TriggerSkill(Type_SkillTriggerTime.BattleStart, units[i]);
                }
            }
        }
        public override void OnRelease()
        {
            foreach (var team in Teams)
            {
                team.Value.Release();
            }
            this.Teams.Clear();
            this.Battle.GetManager<BattleFlowManager>().RemoveBattleFlow(this);
            this.GetManager<BattleEventManager>().RemoveListener(BattleEvent.BattleUnitDead, this._OnBattleUnitDead);
        }

        protected BattleTeam _CreateTeam(Type_BattleCamp camp, List<IBattleUnitData> unit_datas, IBattleUnitData leader) {
            BattleTeam team = new BattleTeam(this.Battle, camp);

            if (unit_datas != null)
            {
                for (int i = 0; i < unit_datas.Count; i++)
                {
                    IBattleUnitData data = unit_datas[i];
                    if (data != null)
                    {
                        BattleUnit unit = this.CreateUnit(camp, i, data, leader == data);
                        if (!team.HasUnit(unit.UnitID))
                        {
                            team.AddUnit(unit);
                        }
                    }
                }
            }
            team.Init();
            return team;
        }

        public void SwitchPhaseTeam(int phase_index) {
            if (this.Teams.ContainsKey((int)Type_BattleCamp.Enemy))
            {
                this.Teams.Remove((int)Type_BattleCamp.Enemy);
            }

            for (int i = this._enemy_teams.Count; i <= phase_index; i++) {
                this._enemy_teams.Add(this._CreateTeam(Type_BattleCamp.Enemy, null, null));
            }
            Teams.Add((int)Type_BattleCamp.Enemy, this._enemy_teams[phase_index]);
        }

        //todo
        public BattleUnit CreateUnit(Type_BattleCamp camp, int slot_id, IBattleUnitData data, bool is_leader = false)
        {
            BattleUnit unit = null;
            unit = new BattleUnit((BattleLogic)this._manager_handler, unit_id++, camp, slot_id, data, is_leader);
            this.GetManager<BattleEventManager>().SendMessage(BattleEvent.BattleUnitCreate, this, unit);
            return unit;
        }

        public BattleUnit GetUnit(Type_BattleCamp camp, int uid)
        {
            if (this.Teams.ContainsKey((int)camp))
                return this.Teams[(int)camp].GetUnit(uid);
            return null;
        }

        public BattleUnit GetUnit(int uid)
        {
            BattleUnit unit = null;
            foreach (var team in Teams)
            {
                unit = team.Value.GetUnit(uid);
                if (unit != null)
                    return unit;
            }
            BattleLog.LogError("no such unit ,uid:" + uid);
            return unit;
        }

        public BattleUnit GetSurviveUnit(int uid)
        {
            BattleUnit unit = null;
            foreach (var team in Teams)
            {
                unit = team.Value.GetSurviveUnit(uid);
                if (unit != null)
                    return unit;
            }
            BattleLog.LogError("no such survive unit ,uid:" + uid);
            return unit;
        }

        public BattleUnit GetUnitBySlot(Type_BattleCamp camp, int slot_id)
        {
            return this.Teams[(int)camp].GetSlotUnit(slot_id);
        }

        public List<BattleUnit> GetTeamUnits(Type_BattleCamp camp)
        {
            return this.Teams[(int)camp].GetSurviveUnits();
        }

        public List<BattleUnit> GetAllUnits()
        {
            List<BattleUnit> list = new List<BattleUnit>();
            list.AddRange(this.GetTeamUnits(Type_BattleCamp.Ally));
            list.AddRange(this.GetTeamUnits(Type_BattleCamp.Enemy));
            return list;
        }

        public BattleTeam GetCurrentTeam(Type_BattleCamp camp)
        {
            BattleTeam team = null;
            this.Teams.TryGetValue((int)camp, out team);
            return team;
        }

        public BattleTeam GetHostileTeamByPhase(int phase_index)
        {
            if (phase_index >= 0 && phase_index < this._enemy_teams.Count) {
                return this._enemy_teams[phase_index];
            }
            return null;
        }

        public BattleTeam GetRelationTeam(Type_BattleCamp camp, Type_BattleRelationship relation)
        {
            bool same_camp = relation == Type_BattleRelationship.Friendly;
            foreach (var team in Teams)
            {
                if (same_camp)
                {
                    if (team.Key == (int)camp)
                    {
                        return team.Value;
                    }
                }
                else
                {
                    if (team.Key != (int)camp)
                    {
                        return team.Value;
                    }
                }
            }
            //should not get here
            return null;
        }

        public List<BattleUnit> GetRelationUnits(Type_BattleCamp camp, Type_BattleRelationship relation) {
            bool same_camp = relation == Type_BattleRelationship.Friendly;
            foreach (var team in Teams)
            {
                if (same_camp)
                {
                    if (team.Key == (int)camp)
                    {
                        return new List<BattleUnit>(team.Value.GetSurviveUnits());
                    }
                }
                else {
                    if (team.Key != (int)camp)
                    {
                        return new List<BattleUnit>(team.Value.GetSurviveUnits());
                    }
                }
            }
            //should not get here
            return null;
        }

        public void SetUnitDead(BattleUnit unit) {
            BattleTeam team = this.GetCurrentTeam(unit.Camp);
            team.SetUnitDead(unit);
        }

        //never remove a unit during a battle,remove them after battle finish
        public void RemoveUnit(BattleUnit unit)
        {
            if (!this.Teams.ContainsKey((int)unit.Camp))
                return;
            BattleTeam team = this.Teams[(int)unit.Camp];
            if (team != null) {
                this.GetManager<BattleEventManager>().SendMessage(BattleEvent.BattleUnitRemove, this, unit);
                team.RemoveUnit(unit);
            }
        }


        public int Priority => 0;


        public void StartFlow()
        {

        }

        public void StartPhase(int phase_id)
        {

            this.SwitchPhaseTeam(phase_id);


            foreach (var team in Teams)
            {
                team.Value.CurrentTurn = -1;
                team.Value.CardManager.DrawCards();
            }
        }

        public void StartTurn(Type_BattleCamp camp, int turn)
        {
            this.Teams[(int)camp].StartTurn(turn);
        }

        public void EndTurn(Type_BattleCamp camp, int turn)
        {
            this.Teams[(int)camp].EndTurn(turn);
        }

        public void EndPhase(int phase_id)
        {
        }

        public void EndFlow()
        {

        }

        public void EndSkill()
        {
            //foreach (var team in Teams)
            //{
            //    team.Value.UpdateTeamUnit();
            //}
        }

        protected void _OnBattleUnitDead(object sender, object data) { 
            BattleUnitDeadEventData msg = (BattleUnitDeadEventData)data;
            BattleTeam team = null;
            this.Teams.TryGetValue((int) msg.Target.Camp, out team);
            if (team != null) {
                team.SetUnitDead(msg.Target);
            }
        }
    }
}