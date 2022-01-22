using System.Collections;
using System.Collections.Generic;
using System;

namespace TestBattle
{
    public class BattleUnit
    {
        public int UnitID;
        public int TableID => this.UnitData.UnitTID;
        public Type_BattleCamp Camp;
        public int SlotID;
        public UnitType UnitType =>this.UnitData.UnitType;

        public TeamRowType RowType;
        public TeamColumnType ColumnType;

        public TeamRowType Orientation => this.UnitData.BattleTeamRow;
        public Type_BattleAttribution BattleAttribution => this.UnitData.BattleAttribution;
        public bool LeaderSkillActive => IsLeader;
        public bool PassiveSkillActive => true;

        public int CurrentHp { get; private set; }
        public int MaxHp {
            get
            {
                return this._attributes[(int)Type_Attribution.HP];
            }
        } 
        public int CurrentShield { get; private set; }
        public int MaxShield { get; private set; }

        public int AwakenGageMaxPoint = 6;
        public int AwakenGagePoint { get; private set; }

        public int RoundDamage { get; private set; }


        public bool IsLeader = false;

        private Dictionary<int, int> _origin_attributes = new Dictionary<int, int>();
        private Dictionary<int, int> _attributes = new Dictionary<int, int>();
       
        public UnitBuffManager BuffManager;


        public readonly string UnitLogInfo = "";

        public IBattleUnitData UnitData;//remove unity later
        public CardData BaseSkill = null;

        public bool IsDead { 
            get {
                return this.CurrentHp <= 0;
            }
        }

        public bool Selectable
        {
            get
            {
                return !this.HasCondition(Type_Condition.sneak) && !this.HasCondition(Type_Condition.freezing);
            }
        }

        public float CurrentHpRate { get { return (float)CurrentHp / (float)MaxHp; } }

        protected BattleLogic _battle;
        public BattleUnit(BattleLogic battle, int uid, Type_BattleCamp camp, int slot_id, IBattleUnitData data ,bool is_leader)
        {
            this._battle = battle;
            this.UnitID = uid;
            this.Camp = camp;
            this.SlotID = slot_id;
            this.UnitData = data;
            this.IsLeader = is_leader;
            this.AwakenGagePoint = 0;
            this.BaseSkill = BattleClassCache.Instance.GetInstance<CardData>();
            this.BaseSkill.Init(this.UnitID, this.UnitData.BaseSkillData, 1);
            this.RowType = Data_Team.GetSlotRowType(this.SlotID);
            this.ColumnType = Data_Team.GetSlotColumnType(this.SlotID);
            this.BuffManager = new UnitBuffManager(this);
            this.UnitLogInfo = string.Format("[uid:{0}[{5}],camp:{1},slot:{2},tid:{3},type:{4}]", this.UnitID, this.Camp, this.SlotID, this.UnitData.UnitTID, this.UnitData.UnitType,this.UnitData.Name);
            this.ResetAttris(data.Attris);
            this.InitTriggerSkill();
        }

        public void UpdateUnitData(/*属性，可用技能，当前buff*/) { //暂定战斗中技能基本数据是不变的，这里不更新技能数据
        }

        //添加技能
        public bool AddSkill(ISkillData skill) {
            return true;
        }

        public void CastSkill(int skill_id,int skill_rank_level) { 
        }

        public void ResetAttris(Dictionary<Type_Attribution, int> attris) {
            this._origin_attributes.Clear();
            this._attributes.Clear();
            for (int i = (int)Type_Attribution.StatStart; i < (int)Type_Attribution.StatEnd; i++)
            {
                this._origin_attributes.Add(i, attris[(Type_Attribution)i]);
                this._attributes.Add(i, attris[(Type_Attribution)i]);
            }
            this.CurrentHp = this.MaxHp;
        }

        public void UpdateAttributes() {
            for (int i = (int)Type_Attribution.StatStart; i < (int)Type_Attribution.StatEnd; i++)
            {
                this._attributes[i] = this._origin_attributes[i];
                int rate = this.BuffManager.GetModifier<BuffAttributeModifier>().GetAttributeRate(0, (Type_Attribution)i);
                this._attributes[i] = (int)(this._attributes[i] * (1 + GameUtil.ToRate(rate)));
            }
        }
        public int GetAttribute(Type_Attribution attri_type)
        {
            return this._attributes[(int)attri_type];
        }

        public void AddAttribute(Type_Attribution attri_type ,int value) {
            if (this._attributes.ContainsKey((int)attri_type)) {
                this._attributes[(int)attri_type] += value;
            }
        }

        public void BeginTeamTurn() { 
        }

        public void EndTeamTurn() {
            this.BuffManager.ProcessBuffTurn();
            this.RoundDamage = 0;
        }

        public void BeginSelfTurn() {

        }

        public void EndSelfTurn() {
        }

        public void Release()
        {
            this.BaseSkill.Release();
        }

        public void AddShield(BattleUnit source, int value,int max) {
            this.MaxShield += max;
            this.CurrentShield += value;
            this._battle.GetManager<BattleEventManager>().SendMessage(BattleEvent.BattleUnitShieldChanged, this, BattleUnitShieldEventData.CreateEventData(source, this, value));

        }

        public void AddHp(BattleUnit source, int change,Type_Damage damage_type = Type_Damage.Skill) {
            if (change > 0) {
                if (this.BuffManager.HasBuff(Type_Condition.block_recovery_value))
                    change = 0;
                else {
                    change = this.BuffManager.GetModifier<BuffDeclineRecoveryModifier>().ModifyRecovery(change);
                }
            }
            if (change > 0)
            {
                BattleLog.Log(string.Format("{0} heal {1} with {2} point by {3}", source !=null? source.UnitLogInfo:"None", this.UnitLogInfo, change,damage_type));
            }
            else if(change < 0){
                this.RoundDamage -= change;
                BattleLog.Log(string.Format("{0} hit {1} with {2} point by {3}", source != null ? source.UnitLogInfo : "None", this.UnitLogInfo, change, damage_type));

            }


            int max_change = this.MaxHp - this.CurrentHp;
            change = Math.Min(max_change, change);
            this.CurrentHp += change;
            this._battle.GetManager<BattleEventManager>().SendMessage(BattleEvent.BattleUnitDamage, this, BattleUnitDamageEventData.CreateEventData(source,this, damage_type,change));
            if (this.CurrentHp <= 0) {
                this._battle.GetManager<BattleEventManager>().SendMessage(BattleEvent.BattleUnitDead, this, BattleUnitDeadEventData.CreateEventData(source, this, damage_type));
            }
        }
        //1: not full to full , -1 :full to not full
        public int ChangeAwakenGage(int point) {
            int pre_gage = this.AwakenGagePoint;
            this.AwakenGagePoint += point;
            if (this.AwakenGagePoint < 0) {
                this.AwakenGagePoint = 0;
            }
            if (this.AwakenGagePoint > this.AwakenGageMaxPoint) {
                this.AwakenGagePoint = this.AwakenGageMaxPoint;
            }
            if (pre_gage < this.AwakenGageMaxPoint && this.AwakenGagePoint == this.AwakenGageMaxPoint) {
                return 1;
            }
            if(pre_gage == this.AwakenGageMaxPoint && this.AwakenGagePoint < this.AwakenGageMaxPoint)
            {
                return -1;
            }
            return 0;
        }

        public bool IsFullGage() {
            return this.AwakenGagePoint == this.AwakenGageMaxPoint;
        }
        #region conditions 
        //todo 

        public bool AddBuff(BattleUnit caster, SkillBuffInfo buff_info) {
            BaseBattleBuff buff = this.CreateBuff(caster,buff_info);//to create
            bool added = false;
            if (buff != null)
            {
                buff.BuffUID = (caster.UnitID << 54) + (buff_info.BuffID << 22);
                added = this.BuffManager.CheckAddBuff(buff);
                if (added && buff is ChangeAttributeBuff)
                {
                    this.UpdateAttributes();
                }
            }
            return added;
        }
        public bool HasCondition(Type_Condition condition)
        {
            return this.BuffManager.HasBuff(condition);
        }

        public bool HasConditionKind(Type_ConditionKind conditionKind)
        {
            return this.BuffManager.HasKindBuff(conditionKind);
        }

        public void RemoveBuff(long buff_id)
        {
            this.BuffManager.RemoveBuff(buff_id);
        }

        public List<BaseBattleBuff> GetConditions(Type_Condition type)
        {
            return this.BuffManager.GetBuffByType(type);
        }

        public BaseBattleBuff CreateBuff(BattleUnit caster, SkillBuffInfo buff_info) {
            BaseBattleBuff buff = null;//to create
            switch ((Type_Condition)buff_info.BuffType) {
                case Type_Condition.attack_up:
                case Type_Condition.critical_up:
                case Type_Condition.critical_value_up:
                case Type_Condition.defence_physical_up:
                case Type_Condition.defence_magic_up:
                case Type_Condition.defence_critical_up:
                case Type_Condition.attack_down:
                case Type_Condition.critical_down:
                case Type_Condition.critical_value_down:
                case Type_Condition.defence_physical_down:
                case Type_Condition.defence_magic_down:
                case Type_Condition.defence_critical_down:
                    buff = new ChangeAttributeBuff(this._battle, this, caster, buff_info);
                    break;
                case Type_Condition.true_damage:
                    buff = new TrueDamageBuff(this._battle, this, caster, buff_info);
                    break;
                case Type_Condition.shield_damage:
                    buff = new ShieldBuff(this._battle, this, caster, buff_info);
                    break;
                case Type_Condition.blood_shield:
                    buff = new BloodShieldBuff(this._battle, this, caster, buff_info);
                    break;
                case Type_Condition.poison_dot:
                case Type_Condition.burn_dot:
                case Type_Condition.freezing_dot:
                case Type_Condition.pvp_dot:
                case Type_Condition.wound_dot:
                case Type_Condition.recovery_dot:
                    buff = new DotBuff(this._battle,this, caster, buff_info);
                    break;
                case Type_Condition.damage_up:
                case Type_Condition.damage_down:
      
                    buff = new TargetChangeDamageBuff(this._battle, this, caster, buff_info);
                    break;
                case Type_Condition.glamorous:
                    buff = new GlamorousBuff(this._battle, this, caster, buff_info); 
                    break;
                case Type_Condition.hit_damage_up:
                case Type_Condition.pvp_damage_up:
                case Type_Condition.hp_damage_up:
                    buff = new AttackerChangeDamageBuff(this._battle, this, caster, buff_info);
                    break;
                case Type_Condition.provocation:
                case Type_Condition.stun:
                case Type_Condition.freezing:
                case Type_Condition.sneak:
                case Type_Condition.hax:
                case Type_Condition.block_recovery_value:
                    buff = new NoneValueBuff(this._battle, this, caster, buff_info);
                    break;
                case Type_Condition.clear_condition:
                    buff = new ClearBuff(this._battle, this, caster, buff_info);
                    break;
                case Type_Condition.immun_condition:
                    buff = new ImmuneBuff(this._battle, this, caster, buff_info);
                    break;
                case Type_Condition.decline_recovery:
                    buff = new DeclineRecoveryBuff(this._battle, this, caster, buff_info);
                    break;
                case Type_Condition.atk_suck_blood:
                case Type_Condition.suck_blood:
                    buff = new SuckBloodBuff(this._battle, this, caster, buff_info);
                    break;
                case Type_Condition.shield_devine:
                case Type_Condition.shield_devine_once:
                    buff = new DevineShieldBuff(this._battle, this, caster, buff_info);
                    break;
                case Type_Condition.reflect:
                    buff = new ReflectBuff(this._battle, this, caster, buff_info);
                    break;
                case Type_Condition.row_extra_damage:
                    buff = new BackRowDamageBuff(this._battle, this, caster, buff_info);
                    break;
               
                case Type_Condition.debuff_num_extra_damage:
                case Type_Condition.debuff_type_extra_damage:
                    buff = new DebuffDamageBuff(this._battle, this, caster, buff_info);
                    break;
                case Type_Condition.enchanting:
                    buff = new ChangeAttackTypeBuff(this._battle, this, caster, buff_info);
                    break;
                case Type_Condition.inspire:
                    buff = new InspireBuff(this._battle, this, caster, buff_info);
                    break;
                case Type_Condition.paralysis:
                    buff = new ParalysisBuff(this._battle, this, caster, buff_info);
                    break;
                case Type_Condition.block_skill:
                    buff = new LockSkillBuff(this._battle, this, caster, buff_info);
                    break;
                case Type_Condition.restoration:
                    buff = new RestorationBuff(this._battle, this, caster, buff_info);
                    break;
                case Type_Condition.electric:
                case Type_Condition.damage_addition:
                    buff = new AdditionalDamageBuff(this._battle, this, caster, buff_info);
                    break;
                case Type_Condition.awaken_gage_up:
                case Type_Condition.awaken_gage_down:
                    buff = new AwakenGageChangeBuff(this._battle, this, caster, buff_info);
                    break;
                case Type_Condition.action_turn_change:
                    buff = new TeamActionCountChangeBuff(this._battle, this, caster, buff_info);
                    break;
                default:
                    BattleLog.LogError(string.Format("{0} not implement", (Type_Condition)buff_info.BuffType));
                    break;
            }

            return buff;
        }
        #endregion

        #region test skills

        public List<SkillData> OneSkillDatas = new List<SkillData>();

        public IBattleUnitSkillData GetSkillData(int skill_id)
        {
            return this.UnitData.SkillDatas.Find(s => s.SkillID == skill_id);
        }
        public List<IBattleUnitSkillData> GetSkillDatas()
        {
            return this.UnitData.SkillDatas;
        }
        public bool IsSkillLock(int skill_id,int rank_level) {
            IBattleUnitSkillData skill_data = this.GetSkillData(skill_id);
            bool locked = this.BuffManager.GetModifier<BuffLockSkillCheckModifier>().CheckSkillLock(skill_data, rank_level);
            return locked;
        }

        public bool CanCastSkill() {
            bool cast_succ = this.BuffManager.GetModifier<BuffCastSkillRateCheckModifier>().CanCastSkill();
            return cast_succ;
        }

        public SkillCalculateInfo GetSkillCastInfo(CardData skill_card, int target_uid = 0)
        {
            IBattleUnitSkillData skill_data = skill_card.SkillData;
            int skill_id = skill_data.SkillID;
            SkillCalculateInfo info = new SkillCalculateInfo();
            info.AttackerID = this.UnitID;
            info.SkillID = skill_id;
            info.AttackType = skill_data.Data.attack_type;
            info.SkillLevel = skill_data.SkillLevel;
            info.SkillType = (int)skill_data.SkillType;
            info.SkillRank = skill_card.Rank;
            info.HitCount = skill_data.Data.skill_hit_count;
            info.TargetType1 = skill_data.Data.damage_select;
            info.SkillGeneralValue1 = skill_data.SkillValueData1 != null ? skill_data.SkillValueData1.GetValue(skill_data.SkillType, skill_data.SkillLevel, skill_card.Rank):0;
            info.SkillStableValue1 = skill_data.SkillValueData1 != null ? skill_data.SkillValueData1.GetStableValue(skill_data.SkillLevel) : 0;
            info.TargetType2 = (int)skill_data.Data.recover_select;
            info.SkillGeneralValue2 = skill_data.SkillValueData2 != null ? skill_data.SkillValueData2.GetValue(skill_data.SkillType, skill_data.SkillLevel, skill_card.Rank):0;
            info.SkillStableValue2 = skill_data.SkillValueData2 != null? skill_data.SkillValueData2.GetStableValue(skill_data.SkillLevel):0;
            info.PreBuffs = new List<SkillBuffInfo>();
            info.SelectTargetID = target_uid;
            for (int i = 0; i < skill_data.SkillSummaryData.pre_buff_ids.Length; i++)
            {
                SkillBuffInfo buff_info = BattleCalculator.CreateSkillBuffInfo(skill_data.SkillSummaryData.pre_buff_ids[i], info);
                info.PreBuffs.Add(buff_info);
            }
            info.PostBuffs = new List<SkillBuffInfo>();
            for (int i = 0; i < skill_data.SkillSummaryData.post_buff_ids.Length; i++)
            {
                SkillBuffInfo buff_info = BattleCalculator.CreateSkillBuffInfo(skill_data.SkillSummaryData.post_buff_ids[i], info);
                info.PostBuffs.Add(buff_info);
            }
            return info;
        }

        protected Dictionary<int, List<CardData>> _TriggerSkill = new Dictionary<int, List<CardData>>();

        public void InitTriggerSkill() {
            this._TriggerSkill.Clear();
            for (int i = 0; i < this.UnitData.SkillDatas.Count; i++)
            {
                IBattleUnitSkillData skill = this.UnitData.SkillDatas[i];
                if (skill.TriggerType != Type_SkillTriggerTime.None)
                {
                    if ((skill.SkillType == Type_Skill.Leader && this.IsLeader && this.UnitData.LeaderSkillActive) ||
                        (skill.SkillType == Type_Skill.Passive && this.UnitData.PassiveSkillActive))
                    {
                        int trigger_type = (int)skill.TriggerType;
                        List<CardData> skills = null;
                        if (!this._TriggerSkill.TryGetValue(trigger_type, out skills))
                        {
                            skills = new List<CardData>();
                            this._TriggerSkill.Add(trigger_type, skills);
                        }
                        CardData card_data = BattleClassCache.Instance.GetInstance<CardData>();
                        card_data.Init(this.UnitID, skill, 1);
                        skills.Add(card_data);
                    }
                }
            }
        }

        public List<CardData> GetTriggerSkills(Type_SkillTriggerTime trigger_type) {
            List<CardData> skills = null;
           
            this._TriggerSkill.TryGetValue((int)trigger_type, out skills);
            return skills;
        }
        public void OnTriggerSkill(Type_SkillTriggerTime trigger_type,object param) {
            List<CardData> skills = null;
            if (this._TriggerSkill.TryGetValue((int)trigger_type, out skills)) {
                for (int i = 0; i < skills.Count; i++) {
                    SkillCalculateInfo skill_info = this.GetSkillCastInfo(skills[i]);
                    skill_info.SelectTargetID = ((BattleUnit)param).UnitID;
                    this._battle.GetManager<BattleCalculator>().CalculateSkill(skill_info);
                }
            }
        }

        #endregion

        protected void ErroMsg(string msg) {
            BattleLog.LogError(string.Format("{0}-------",msg,this.UnitLogInfo));
        }

    }
}
