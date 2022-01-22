using System.Collections;
using System.Collections.Generic;
using System;
using table;

namespace TestBattle
{
    public class SkillCalculateInfo {
        public int AttackerID;
        public int SelectTargetID;
        public int SkillID;
        public int SkillLevel;
        public int SkillType;
        public int SkillRank;
        public int HitCount;
        public int AttackType;
        public int TargetType1;
        public float SkillGeneralValue1;
        public int SkillStableValue1;
        public int TargetType2;
        public float SkillGeneralValue2;
        public int SkillStableValue2;
        public List<SkillBuffInfo> PreBuffs;
        public List<SkillBuffInfo> PostBuffs;
    }

    public class SkillBuffInfo
    {
        public int BuffID;
        public int CasterID;
        public int BuffType;//Type_Condition
        public int TargetType;
        public int BuffKind;//none:0 ,buff 1, debuff 2
        public string CheckValue;
        public int Probability;
        public int Value;
        public int Turn;
        public int ExistType;
        public string ExistValue;
    }


    public class BattleCalculator : BattleBaseManager
    {
        public System.Random Random = new System.Random(1);
        public override void OnInit()
        {
            
        }

        public override void OnRelease()
        {

        }

        private List<BattleUnit> _targets = new List<BattleUnit>();

        //
        public int GetRandom(int min,int max,Type_BattleRandom random = Type_BattleRandom.None) {
            int r = this.Random.Next(min, max);
            //BattleLog.Log("random:" + r);
            return r;
        }
        #region select targets
        public List<BattleUnit> GetTargets(BattleUnit attacker,Type_Target target_type) {
            List<BattleUnit> friendly_targets = this.GetManager<BattleUnitManager>().GetRelationUnits(attacker.Camp,Type_BattleRelationship.Friendly);
            List<BattleUnit> hostile_targets = this.GetManager<BattleUnitManager>().GetRelationUnits(attacker.Camp, Type_BattleRelationship.Hostile);
            hostile_targets.RemoveAll(t => !t.Selectable);

            List<BattleUnit> targets = new List<BattleUnit>();
            switch (target_type)
            {
                case Type_Target.EnemyNearest:
                    {//nearest enemy
                        targets.Add(this.GetNereastTarget(attacker, hostile_targets));
                    }
                    break;
                case Type_Target.EnemyAll:
                    {
                        targets = hostile_targets;
                    }
                    break;
                case Type_Target.EnemyOne:
                    {
                        int index = this.GetRandom(0, hostile_targets.Count) ;
                        targets.Add(hostile_targets[index]);
                    }
                    break;
                case Type_Target.AllyOne:
                    {
                        int index = this.GetRandom(0, friendly_targets.Count);
                        targets.Add(friendly_targets[index]);
                    }
                    break;
                case Type_Target.AllySelf:
                    targets.Add(attacker);
                    break;
                case Type_Target.AllyAll:
                    {
                        targets = friendly_targets;
                    }
                    break;
                case Type_Target.EnemyFrontRow:
                    {
                        int offset = 0;
                        if (hostile_targets.Count == 0) {//no selectable targets
                            hostile_targets = this.GetManager<BattleUnitManager>().GetRelationUnits(attacker.Camp, Type_BattleRelationship.Hostile);
                        }
                        do
                        {
                            TeamRowType target_row = (TeamRowType)(((int)(TeamRowType.Team_Front_Row + offset)) % ((int)TeamRowType.Team_Row_Count));
                            for (int i = 0; i < hostile_targets.Count; i++)
                            {
                                if (hostile_targets[i].RowType == target_row)
                                {
                                    targets.Add(hostile_targets[i]);
                                }
                            }
                            offset += 1;
                        } while (offset < (int)TeamRowType.Team_Row_Count && targets.Count == 0);
                    }
                    break;
                case Type_Target.EnemyBackRow:
                    {
                        int offset = 0;
                        if (hostile_targets.Count == 0)
                        {//no selectable targets
                            hostile_targets = this.GetManager<BattleUnitManager>().GetRelationUnits(attacker.Camp, Type_BattleRelationship.Hostile);
                        }
                        do
                        {
                            TeamRowType target_row = (TeamRowType)(((int)(TeamRowType.Team_Back_Row - offset) % ((int)TeamRowType.Team_Row_Count)));
                            for (int i = 0; i < hostile_targets.Count; i++)
                            {
                                if (hostile_targets[i].RowType == target_row)
                                {
                                    targets.Add(hostile_targets[i]);
                                }
                            }
                            offset += 1;
                        } while (offset < (int)TeamRowType.Team_Row_Count && targets.Count == 0);
                    }
                    break;
                case Type_Target.AllyLowestHp:
                    {
                        BattleUnit t = friendly_targets[0];
                        for (int i = 0; i < friendly_targets.Count; i++)
                        {
                            if (friendly_targets[i] != t && friendly_targets[i].CurrentHpRate < t.CurrentHpRate)
                            {
                                t = friendly_targets[i];
                            }
                        }
                        targets.Add(t);
                    }
                    break;
                case Type_Target.EnemyLowestHp:
                    {
                        if (hostile_targets.Count == 0) {
                            hostile_targets = this.GetManager<BattleUnitManager>().GetRelationUnits(attacker.Camp, Type_BattleRelationship.Hostile);
                        }
                        BattleUnit t = hostile_targets[0];
                        for (int i = 0; i < hostile_targets.Count; i++)
                        {
                            if (hostile_targets[i] != t && hostile_targets[i].CurrentHpRate < t.CurrentHpRate)
                            {
                                t = hostile_targets[i];
                            }
                        }
                        targets.Add(t);
                    }
                    break;
                case Type_Target.EnemySameColumn:
                    {
                        if (hostile_targets.Count == 0)
                        {//no selectable targets
                            hostile_targets = this.GetManager<BattleUnitManager>().GetRelationUnits(attacker.Camp, Type_BattleRelationship.Hostile);
                        }
                        int offset = 0;
                        int dir = attacker.ColumnType != TeamColumnType.Team_Far_Column ? -1 : 1;
                        do
                        {
                            TeamColumnType target_column = (TeamColumnType)(((int)(attacker.ColumnType + offset * dir + (int)TeamColumnType.Team_Column_Count)) % ((int)TeamColumnType.Team_Column_Count));
                            for (int i = 0; i < hostile_targets.Count; i++)
                            {
                                if (hostile_targets[i].ColumnType == target_column)
                                {
                                    targets.Add(hostile_targets[i]);
                                }
                            }
                            offset += 1;
                        } while (offset < (int)TeamColumnType.Team_Column_Count && targets.Count == 0);
                    }
                    break;
                case Type_Target.AllyOrientationFront:
                    {
                        for (int i = 0; i < friendly_targets.Count; i++) {
                            if (friendly_targets[i].Orientation == TeamRowType.Team_Front_Row) {
                                targets.Add(friendly_targets[i]);
                            }
                        }
                    }
                    break;
                case Type_Target.AllyOrientationMiddle:
                    {
                        for (int i = 0; i < friendly_targets.Count; i++)
                        {
                            if (friendly_targets[i].Orientation == TeamRowType.Team_Middle_Row)
                            {
                                targets.Add(friendly_targets[i]);
                            }
                        }
                    }
                    break;
                case Type_Target.AllyOrientationBack:
                    {
                        for (int i = 0; i < friendly_targets.Count; i++)
                        {
                            if (friendly_targets[i].Orientation == TeamRowType.Team_Back_Row)
                            {
                                targets.Add(friendly_targets[i]);
                            }
                        }
                    }
                    break;
                case Type_Target.AllyAttriEngineering:
                    for (int i = 0; i < friendly_targets.Count; i++)
                    {
                        if (friendly_targets[i].BattleAttribution == Type_BattleAttribution.Engineering)
                        {
                            targets.Add(friendly_targets[i]);
                        }
                    }
                    break;
                case Type_Target.AllyAttriNature:
                    for (int i = 0; i < friendly_targets.Count; i++)
                    {
                        if (friendly_targets[i].BattleAttribution == Type_BattleAttribution.Nature)
                        {
                            targets.Add(friendly_targets[i]);
                        }
                    }
                    break;
                case Type_Target.AllyAttriMagic:
                    for (int i = 0; i < friendly_targets.Count; i++)
                    {
                        if (friendly_targets[i].BattleAttribution == Type_BattleAttribution.Magic)
                        {
                            targets.Add(friendly_targets[i]);
                        }
                    }
                    break;
                case Type_Target.AllyFrontRow:
                    {
                        int offset = 0;
                        if (friendly_targets.Count == 0)
                        {//no selectable targets
                            friendly_targets = this.GetManager<BattleUnitManager>().GetRelationUnits(attacker.Camp, Type_BattleRelationship.Friendly);
                        }
                        do
                        {
                            TeamRowType target_row = (TeamRowType)(((int)(TeamRowType.Team_Front_Row + offset)) % ((int)TeamRowType.Team_Row_Count));
                            for (int i = 0; i < friendly_targets.Count; i++)
                            {
                                if (friendly_targets[i].RowType == target_row)
                                {
                                    targets.Add(friendly_targets[i]);
                                }
                            }
                            offset += 1;
                        } while (offset < (int)TeamRowType.Team_Row_Count && targets.Count == 0);
                    }
                    break;

                default:
                    BattleLog.LogError(string.Format("{0} ---------- no target type", attacker.UnitLogInfo));
                    targets.Add(GetNereastTarget(attacker,hostile_targets));
                    break;
            }
            return targets;
        }

        //for hostile
        public BattleUnit GetNereastTarget(BattleUnit attacker, List<BattleUnit> targets) {
            if (targets.Count == 0) {
                targets = this.GetManager<BattleUnitManager>().GetRelationUnits(attacker.Camp, Type_BattleRelationship.Hostile);
            }
            List<BattleUnit> provacations = targets.FindAll(t => t.HasCondition(Type_Condition.provocation));
            if (provacations != null && provacations.Count > 0) {
                targets = provacations;
            }
            if (targets.Count > 0)
            {
                BattleUnit nearest = targets[0];
                int dis = ((int)attacker.RowType + (int)nearest.RowType) * 100 + Math.Abs(attacker.ColumnType - nearest.ColumnType);//smaller means nearer
                for (int i = 0; i < targets.Count; i++)
                {
                    BattleUnit target = targets[i];
                    int d = ((int)attacker.RowType + (int)target.RowType) * 100 + Math.Abs(attacker.ColumnType - target.ColumnType);
                    if (d < dis)
                    {
                        dis = d;
                        nearest = target;
                    }
                    else if (d == dis && target.ColumnType < nearest.ColumnType)
                    {
                        dis = d;
                        nearest = target;
                    }
                }
                return nearest;
            }
            LogManager.LogError("cannot reach here!");
            return null;
        }

        private List<BattleUnit> _ValidateTargets(Type_Target tt, BattleUnit attacker, BattleUnit select_unit)
        {
            List<BattleUnit> hit_targets = new List<BattleUnit>();
            if(select_unit != null) {
                if (tt == Type_Target.CustomType)
                {
                    hit_targets.Add(select_unit);
                    return hit_targets;
                }
                if (tt == Type_Target.EnemyOne || tt == Type_Target.AllyOne)
                {
                    if (select_unit.HasCondition(Type_Condition.provocation))
                    {
                        hit_targets.Add(select_unit);
                    }
                    else
                    {
                        List<BattleUnit> target_set = null;
                        if (tt == Type_Target.EnemyOne)
                        {
                            target_set = this.GetManager<BattleUnitManager>().GetRelationUnits(attacker.Camp, Type_BattleRelationship.Hostile);
                        }
                        else
                        {
                            target_set = this.GetManager<BattleUnitManager>().GetRelationUnits(attacker.Camp, Type_BattleRelationship.Friendly);
                        }
                        List<BattleUnit> provacations = target_set.FindAll(t => t.HasCondition(Type_Condition.provocation));

                        if (provacations != null && provacations.Count > 0)
                        {
                            hit_targets.Add(this.GetNereastTarget(attacker, provacations));
                        }
                        else
                        {
                            hit_targets.Add(select_unit);
                        }
                    }
                }
                else
                {
                    hit_targets = this.GetTargets(attacker, tt);
                }
            }
            else
            {
                hit_targets = this.GetTargets(attacker, tt);
            }
            return hit_targets;
        }

        #endregion

        #region calculate skill

        public SkillData CalculateSkill(SkillCalculateInfo info)
        {
            if (info == null) {
                BattleLog.LogError("null info");
                return null;
            }

            if (info.SkillType == (int)Type_Skill.Active || info.SkillType == (int)Type_Skill.Awaken) {
                this.Battle.GetManager<BattleEventManager>().SendMessage(BattleEvent.BattleUnitUseCards,
                    this, BattleUnitUseCardEventData.CreateEventData(info.AttackerID, info.SkillID, info.SkillLevel, (int)info.SkillType, info.SkillRank));

            }

            BattleUnit attacker = this.GetManager<BattleUnitManager>().GetUnit(info.AttackerID);
            SkillData skill_data = BattleClassCache.Instance.GetInstance<SkillData>();
            skill_data.CasterID = attacker.UnitID;
            BattleUnit select_unit = info.SelectTargetID > 0 ? this.GetManager<BattleUnitManager>().GetUnit(info.SelectTargetID) : null;//should validate selection 

            for (int i = 0; i < info.PreBuffs.Count; i++)
            {
                this.CheckAddBuff(attacker, attacker, info.PreBuffs[i]);
            }


            int attack_value = attacker.GetAttribute(Type_Attribution.Attack);
            float damage_value = info.SkillGeneralValue1 * attack_value + info.SkillStableValue1;
            float recovery_value = info.SkillGeneralValue2 * attack_value + info.SkillStableValue2;
            if (damage_value != 0 || info.PostBuffs.Count > 0)
            {
                List<BattleUnit> hit_targets = this._ValidateTargets((Type_Target)info.TargetType1, attacker, select_unit);

                if (hit_targets.Count > 0)
                {
                    if (damage_value != 0)
                    {
                        for (int j = 0; j < info.HitCount; j++)
                        {
                            HitData hit_data = BattleClassCache.Instance.GetInstance<HitData>();
                            for (int i = 0; i < hit_targets.Count; i++)
                            {
                                BattleUnit t = hit_targets[i];
                                HitInfo hit_info = hit_data.GetTargetHitInfo(t.UnitID);
                                if (hit_info == null)
                                {
                                    hit_info = BattleClassCache.Instance.GetInstance<HitInfo>();
                                    hit_data.AddTargetHitInfo(t.UnitID, hit_info);
                                }

                                HitItem target_hit_item = BattleClassCache.Instance.GetInstance<HitItem>();
                                hit_info.AddTargetHitItem(target_hit_item);

                                Type_Attack attack_type = (Type_Attack)info.AttackType;
                                attack_type = attacker.BuffManager.GetModifier<BuffAttackTypeModifer>().ChangeAttackType(attack_type);
                                float raw_damage = GetSkillRawDamage(attacker, damage_value, t, attack_type);
                                raw_damage /= info.HitCount;


                                int crit_src = attacker.GetAttribute(Type_Attribution.Critical);
                                int crit_def = t.GetAttribute(Type_Attribution.Critical_Defence);
                                int crit_random = this.GetRandom(0, 10000);
                                //UnityEngine.Debug.LogError("crit_random:" + crit_random);
                                bool crit = crit_random < (crit_src - crit_def);
                                float damage = raw_damage;
                                if (crit)
                                {
                                    damage *= (1 + attacker.GetAttribute(Type_Attribution.Critical_Value) * 0.0001f);
                                }

                                int defend_value = 0;
                                int int_damage = 0;
                               
                                if (t.BuffManager.GetModifier<BuffDevineShieldCheckModifier>().CheckDevineShield(Type_Damage.Skill))
                                {
                                    int_damage = 0;
                                    crit = false;
                                    target_hit_item.Invincible = true;
                                }
                                else {
                                    DamageChangeMessage msg = BattleClassCache.Instance.GetInstance<DamageChangeMessage>();
                                    msg.Init(attacker, t, Type_Damage.Skill, hit_targets.Count);
                                    damage = attacker.BuffManager.GetModifier<BuffAttackerDamageModifier>().ModifyDamage(damage, msg);
                                    int_damage = this.CalculateGeneralDamage(attacker, t, Type_Damage.Skill, damage, out defend_value);
                                }

                                if (int_damage > 0)
                                {//for attacker
                                    bool reflect = t.BuffManager.GetModifier<BuffReflectModifier>().CheckReflect(attacker, int_damage,(Type_HitType)info.AttackType ,ref hit_info.CasterHit);

                                    bool suck_blood = attacker.BuffManager.GetModifier<BuffSuckBloodCheckModifier>().CheckSuckBlood(int_damage, (Type_HitType)info.AttackType,ref hit_info.CasterHit);
                                }

                                t.AddHp(attacker, -int_damage);
                                target_hit_item.AttackType = (Type_HitType)(Type_Attack)info.AttackType;
                                target_hit_item.DamageType = Type_Damage.Skill;
                                target_hit_item.Critical = crit;
                                target_hit_item.DefendValue = defend_value;
                                target_hit_item.Value = int_damage;
                                if (crit)
                                {
                                    skill_data.AddCriticalTarget(t.UnitID);
                                }

                                if (int_damage > 0)
                                {//for target addition damage
                                    bool target_addition = t.BuffManager.GetModifier<BuffAdditionalDamageCheckModifier>().CheckAdditionalDamage(attacker, t,int_damage, (Type_HitType)info.AttackType, ref hit_info.TargetHit);
                                    bool attacker_addition = attacker.BuffManager.GetModifier<BuffAdditionalDamageCheckModifier>().CheckAdditionalDamage(attacker, t, int_damage, (Type_HitType)info.AttackType, ref hit_info.CasterHit);
                                }
                            }
                            skill_data.AddHitData(hit_data);
                            if (attacker.IsDead) //攻击过程中死了
                                break;
                        }
                       
                    }

                }

                for (int i = 0; i < info.PostBuffs.Count; i++)
                {
                    this.CheckAddBuff(attacker, hit_targets, info.PostBuffs[i]);
                }

            }


            if (recovery_value > 0)
            {
                Type_Target recovery_target_type = (Type_Target)info.TargetType2;
                List<BattleUnit> recovery_targets;
                if (recovery_target_type == Type_Target.EnemyOne || recovery_target_type == Type_Target.AllyOne)
                {
                    recovery_targets = new List<BattleUnit>();
                    recovery_targets.Add(select_unit);
                }
                else
                {
                    recovery_targets = this.GetTargets(attacker, recovery_target_type);
                }
                HitData recovery_data = BattleClassCache.Instance.GetInstance<HitData>();

                for (int i = 0; i < recovery_targets.Count; i++)
                {
                    HitInfo hit_info = BattleClassCache.Instance.GetInstance<HitInfo>();

                    HitItem item = BattleClassCache.Instance.GetInstance<HitItem>();
                    item.DamageType = Type_Damage.Skill;
                    item.AttackType = Type_HitType.Recovery;
                    item.Critical = false;
                    item.Value = (int)Math.Floor(recovery_value);

                    hit_info.AddTargetHitItem(item);
                    recovery_data.AddTargetHitInfo(recovery_targets[i].UnitID, hit_info);

                    recovery_targets[i].AddHp(attacker, item.Value);
                }

                skill_data.Recovery = recovery_data;

            }

            return skill_data;
        }

        public int CalculateGeneralDamage(BattleUnit attacker,BattleUnit target, Type_Damage damage_type, float damage, out int defend_value) {
            float restraint_rate = BattleAttributeDamageDeclineRate(attacker.BattleAttribution, target.BattleAttribution);
            damage = damage * (1 + restraint_rate);

            DamageChangeMessage msg = BattleClassCache.Instance.GetInstance<DamageChangeMessage>();
            msg.Init(attacker, target, Type_Damage.Skill, 1);
            damage = target.BuffManager.GetModifier<BuffTargetDamageModifier>().ModifyDamage(damage, msg);

            int int_damage = (int)Math.Floor(damage);
            bool ignore_shield = attacker != null && attacker.BuffManager.GetModifier<BuffIgnoreShieldCheckModifier>().IgnoreShield();
            defend_value = 0;
            if (!ignore_shield)
            {
                defend_value = target.BuffManager.GetModifier<BuffShieldModifier>().ShieldDefendValue(attacker,int_damage);
                int_damage -= defend_value;
            }
            return int_damage;
        }

        public static float GetSkillRawDamage(BattleUnit attacker, float attack, BattleUnit target, Type_Attack attack_type)
        {
            float attack_value = attack;// this._character.Data.AttackAbility_Current;
            int defense = 0;
            if (attack_type == Type_Attack.Physical)
            {
                defense = target.GetAttribute(Type_Attribution.Physics_Defence);
            }
            else if (attack_type == Type_Attack.Magical)
            {
                defense = target.GetAttribute(Type_Attribution.Magic_Defence);
            }
            defense = attacker.BuffManager.GetModifier<BuffIgnoreDefenceModifier>().GetFinalDefence(defense);
            float raw_damage = GetDamageWithDefense(attack_value, defense);
            //raw_damage *= skill_coe;
            return raw_damage;
        }

        public static float GetDamageWithDefense(float attack, int defense)
        {
            float damage_decline_rate = defense * 1.0f / (400 + defense);
            float raw_damage = attack * (1 - damage_decline_rate);
            return raw_damage;
        }

        public static float BattleAttributeDamageDeclineRate(Type_BattleAttribution attackAttribution, Type_BattleAttribution defenceAttribution) {
            return GameFormula.GetAttackAttributionRate(attackAttribution, defenceAttribution);
        }
        #endregion
        #region test add buff
        public static SkillBuffInfo CreateSkillBuffInfo(int buff_id, SkillCalculateInfo skill_info) {
            Table_Skill_buff_data buff_data = Table_Manager.Instance.GetData<Table_Skill_buff_data>(buff_id);
            if (buff_data.GetTurn(skill_info.SkillRank) == 0)
                return null;
            SkillBuffInfo buff_info = new SkillBuffInfo();
            buff_info.BuffID = buff_id;
            buff_info.BuffType = (int)GameUtil.StringToConditionType(buff_data.buff_type);
            buff_info.CasterID = skill_info.AttackerID;
            buff_info.TargetType = buff_data.target_type;
            buff_info.CheckValue = buff_data.check_value;
            
            RepeatedField<Table_Condition_info> condition_infos = Table_Manager.Instance.GetDataList<Table_Condition_info>();
            Table_Condition_info info = condition_infos.Find(e => e.condition_enum_name == buff_data.buff_type);
            buff_info.BuffKind = info.condition_type;
            buff_info.Probability = buff_data.GetProbability(skill_info.SkillRank);
            buff_info.Value = buff_data.GetValue((Type_Skill)skill_info.SkillType, skill_info.SkillLevel, skill_info.SkillRank);
            buff_info.Turn = buff_data.GetTurn(skill_info.SkillRank);
            buff_info.ExistType = buff_data.coexist;
            buff_info.ExistValue = buff_data.start_string_key;
            return buff_info;
        }

        public Dictionary<int,List<BuffInfo>> CheckAddBuff(BattleUnit attacker, List<BattleUnit> skill_targets , SkillBuffInfo buff_info) {
            if (buff_info == null)
                return null;
            List<BattleUnit> buff_targets = skill_targets;
            if (buff_info.TargetType != 0 && buff_info.TargetType != (int)Type_Target.CustomType) {
                buff_targets = this.GetTargets(attacker,(Type_Target)buff_info.TargetType);
            }
           
            for (int i = 0; i < buff_targets.Count; i++)
            {
                this.CheckAddBuff(attacker, buff_targets[i], buff_info);
            }

            return null;
        }

        public BuffInfo CheckAddBuff(BattleUnit attacker, BattleUnit target, SkillBuffInfo buff_info)
        {
            if (this.GetRandom(0, 10000) < buff_info.Probability)
            {
                target.AddBuff(attacker, buff_info);
                List<long> clear_buffs = target.BuffManager.GetModifier<BuffClearCheckModifier>().CheckClearBuffs();
                for (int j = 0; j < clear_buffs.Count; j++)
                {
                    target.RemoveBuff(clear_buffs[j]);
                }
            }
            return null;//todo
        }

        #endregion
    }
}
