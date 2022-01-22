using System.Collections;
using System.Collections.Generic;
using System;

namespace TestBattle
{
    public abstract class BaseBuffModifier<T> where T:class
    {
        public BattleUnit Owner;
        public BaseBuffModifier(BattleUnit owner) {
            this.Owner = owner;
        }
        protected List<T> _handlers = new List<T>();
        public void AddModifyHandler(T handler) {
            this._handlers.Add(handler);
        }
        public void RemoveModifyHandler(T handler) {
            this._handlers.Remove(handler);
        }
    }

    public class UnitBuffManager
    {
        public BattleUnit Owner;

        #region modifier
        private Dictionary<Type, object> _modifiers = new Dictionary<Type, object>();
        public UnitBuffManager(BattleUnit owner)
        {
            this.Owner = owner;
            this._modifiers.Add(typeof(BuffAttackerDamageModifier), new BuffAttackerDamageModifier(this.Owner));
            this._modifiers.Add(typeof(BuffTargetDamageModifier), new BuffTargetDamageModifier(this.Owner));
            this._modifiers.Add(typeof(BuffAttributeModifier), new BuffAttributeModifier(this.Owner));
            this._modifiers.Add(typeof(BuffImmuneCheckModifier), new BuffImmuneCheckModifier(this.Owner));
            this._modifiers.Add(typeof(BuffClearCheckModifier), new BuffClearCheckModifier(this.Owner));
            this._modifiers.Add(typeof(BuffIgnoreDefenceModifier), new BuffIgnoreDefenceModifier(this.Owner));
            this._modifiers.Add(typeof(BuffIgnoreShieldCheckModifier), new BuffIgnoreShieldCheckModifier(this.Owner));
            this._modifiers.Add(typeof(BuffInvincibleCheckModifier), new BuffInvincibleCheckModifier(this.Owner));
            this._modifiers.Add(typeof(BuffShieldModifier), new BuffShieldModifier(this.Owner));
            this._modifiers.Add(typeof(BuffDotModifier), new BuffDotModifier(this.Owner));
            this._modifiers.Add(typeof(BuffDeclineRecoveryModifier), new BuffDeclineRecoveryModifier(this.Owner));
            this._modifiers.Add(typeof(BuffAfterSkillCheckRemoveModifier), new BuffAfterSkillCheckRemoveModifier(this.Owner));
            this._modifiers.Add(typeof(BuffDevineShieldCheckModifier), new BuffDevineShieldCheckModifier(this.Owner));
            this._modifiers.Add(typeof(BuffSuckBloodCheckModifier), new BuffSuckBloodCheckModifier(this.Owner));
            this._modifiers.Add(typeof(BuffReflectModifier), new BuffReflectModifier(this.Owner)); 
            this._modifiers.Add(typeof(BuffAttackTypeModifer), new BuffAttackTypeModifer(this.Owner));
            this._modifiers.Add(typeof(BuffLockSkillCheckModifier), new BuffLockSkillCheckModifier(this.Owner)); 
            this._modifiers.Add(typeof(BuffCastSkillRateCheckModifier), new BuffCastSkillRateCheckModifier(this.Owner));
            this._modifiers.Add(typeof(BuffSkillInspireCheckModifier), new BuffSkillInspireCheckModifier(this.Owner));
            this._modifiers.Add(typeof(BuffAdditionalDamageCheckModifier), new BuffAdditionalDamageCheckModifier(this.Owner));

            
        }

        public void Release()
        {
            this._modifiers.Remove(typeof(BuffAttackerDamageModifier));
            this._modifiers.Remove(typeof(BuffTargetDamageModifier));
            this._modifiers.Remove(typeof(BuffAttributeModifier));
            this._modifiers.Remove(typeof(BuffImmuneCheckModifier));
            this._modifiers.Remove(typeof(BuffClearCheckModifier));
            this._modifiers.Remove(typeof(BuffIgnoreDefenceModifier));
            this._modifiers.Remove(typeof(BuffIgnoreShieldCheckModifier));
            this._modifiers.Remove(typeof(BuffInvincibleCheckModifier));
            this._modifiers.Remove(typeof(BuffShieldModifier));
            this._modifiers.Remove(typeof(BuffDotModifier));
            this._modifiers.Remove(typeof(BuffDeclineRecoveryModifier));
            this._modifiers.Remove(typeof(BuffSuckBloodCheckModifier));
            this._modifiers.Remove(typeof(BuffReflectModifier));
            this._modifiers.Remove(typeof(BuffAttackTypeModifer));
            this._modifiers.Remove(typeof(BuffLockSkillCheckModifier));
            this._modifiers.Remove(typeof(BuffCastSkillRateCheckModifier));
            this._modifiers.Remove(typeof(BuffSkillInspireCheckModifier));
            this._modifiers.Remove(typeof(BuffAdditionalDamageCheckModifier));

        }

        public void AddModifierHandler<T, H>(H handler) where T : BaseBuffModifier<H> where H : class
        {
            T modifier = this.GetModifier<T>();
            modifier.AddModifyHandler(handler);
        }

        public void RemoveModifierHandler<T, H>(H handler) where T : BaseBuffModifier<H> where H : class
        {
            T modifier = this.GetModifier<T>();
            modifier.RemoveModifyHandler(handler);
        }

        public T GetModifier<T>() where T : class
        {
            object modifer = null;
            this._modifiers.TryGetValue(typeof(T), out modifer);
            return modifer as T;
        }
        #endregion

        public Dictionary<long, BaseBattleBuff> _all_buffs = new Dictionary<long, BaseBattleBuff>();
        public List<BaseBattleBuff> _buffs = new List<BaseBattleBuff>();
        public List<BaseBattleBuff> _debuffs = new List<BaseBattleBuff>();
        public Dictionary<int, List<BaseBattleBuff>> _type_buffs = new Dictionary<int, List<BaseBattleBuff>>();
        public List<BaseBattleBuff> _ordereed_buff = new List<BaseBattleBuff>();

        public bool CheckAddBuff(BaseBattleBuff buff) {
            if (buff == null)
                return false;
            bool immune = this.GetModifier<BuffImmuneCheckModifier>().IsImmune(buff.BuffData);
            if (immune)
            {
                buff.Release();
                return false;
            }
            BaseBattleBuff exist_buff = null;
            this._all_buffs.TryGetValue(buff.BuffUID,out exist_buff);
            if (exist_buff != null)
            {
                Type_BuffExist exist_type = (Type_BuffExist)buff.BuffData.ExistType;
                if (exist_type == Type_BuffExist.Coexist)
                {
                    long final_id = buff.BuffUID;
                    final_id++;
                    do
                    {
                        this._all_buffs.TryGetValue(final_id,out exist_buff);
                        if (exist_buff != null)
                        {
                            if (exist_buff.CasterID != buff.CasterID && exist_buff.BuffID != buff.BuffID)
                            {
                                BattleLog.LogError("cannot get here!!");
                                buff.Release();
                                return false;
                            }
                            final_id++;
                        }
                        else
                        {
                            buff.BuffUID = final_id;
                            break;
                        }
                    } while (true);
                    this._DoAddBuff(buff);
                }
                else if (exist_type == Type_BuffExist.Substitute)
                {
                    if (exist_buff.Turn < buff.Turn || (exist_buff.Turn != -1 && buff.Turn == -1))
                    {
                        exist_buff.Value = buff.Value;
                        exist_buff.Turn = exist_buff.MaxTurn = buff.Turn;
                    }

                    buff.Release();
                }
                else if (exist_type == Type_BuffExist.Overlap)
                {
                    exist_buff.Overlap(buff);

                    buff.Release();
                }
                else
                {
                    BattleLog.LogError("should not get here with buff exist type:" + exist_type);
                }
            }
            else
            {
                this._DoAddBuff(buff);
            }
            return true;
        }
        private void _DoAddBuff(BaseBattleBuff buff) {
            buff.Add();
            this._all_buffs.Add(buff.BuffUID, buff);
            List<BaseBattleBuff> buff_list = null;
            if (!this._type_buffs.TryGetValue((int)buff.BuffType, out buff_list)) {
                buff_list = new List<BaseBattleBuff>();
                this._type_buffs.Add((int)buff.BuffType, buff_list);
            }
            buff_list.Add(buff);
            if (buff.BuffKind == Type_ConditionKind.Buff) {
                this._buffs.Add(buff);
            }
            if (buff.BuffKind == Type_ConditionKind.Debuff) {
                this._debuffs.Add(buff);
            }
            this._ordereed_buff.Add(buff);

        }

        public void RemoveBuff(BaseBattleBuff buff) {
            if (buff == null)
                return;
            buff.Remove();
            this._all_buffs.Remove(buff.BuffUID);
            List<BaseBattleBuff> buff_list = null;
            if (this._type_buffs.TryGetValue((int)buff.BuffType, out buff_list))
            {
                buff_list.Remove(buff);
            }
            if (buff.BuffKind == Type_ConditionKind.Buff)
            {
                this._buffs.Remove(buff);
            }
            if (buff.BuffKind == Type_ConditionKind.Debuff)
            {
                this._debuffs.Remove(buff);
            }
            this._ordereed_buff.Remove(buff);
            if (buff is ChangeAttributeBuff)
            {
                this.Owner.UpdateAttributes();
            }
            buff.Release();
        }

        public void RemoveBuff(long buff_id)
        {
            BaseBattleBuff buff = null;
            this._all_buffs.TryGetValue(buff_id, out buff);
            this.RemoveBuff(buff);
        }

        protected List<BaseBattleBuff> _removeable_buffs = new List<BaseBattleBuff>();
        public void ProcessBuffTurn() {
            //this.
            
            int defend_damage = 0;
            int dot_damage = this.GetModifier<BuffDotModifier>().GetDotDamage(out defend_damage);
            if (dot_damage != 0)
            {
                this.Owner.AddHp(this.Owner, dot_damage, Type_Damage.Dot);
            }

            this._removeable_buffs.Clear();
            foreach (KeyValuePair<long, BaseBattleBuff> kvp in this._all_buffs) {
                kvp.Value.ProcessTurn();
                if (kvp.Value.Finished) {
                    this._removeable_buffs.Add(kvp.Value);
                }
            }
            bool has_attri_buff = false;
            for (int i = 0; i < this._removeable_buffs.Count; i++) {
                if (this._removeable_buffs[i] is ChangeAttributeBuff) {
                    has_attri_buff = true;
                }
                this.RemoveBuff(this._removeable_buffs[i]);
            }
            if (has_attri_buff) {
                this.Owner.UpdateAttributes();
            }
        }

        public bool HasBuff(Type_Condition condition) {
            List<BaseBattleBuff> buffs = this.GetBuffByType(condition);
            return buffs != null && buffs.Count > 0;
        }

        public List<BaseBattleBuff> GetBuffByType(Type_Condition condition) {
            List<BaseBattleBuff> buffs = null;
            this._type_buffs.TryGetValue((int)condition, out buffs);
            return buffs;
        }

        public bool HasKindBuff(Type_ConditionKind kind)
        {
            List<BaseBattleBuff> buffs = this.GetBuffByKind(kind);
            return buffs != null && buffs.Count > 0;
        }
        public List<BaseBattleBuff> GetBuffByKind(Type_ConditionKind kind)
        {
            if (kind == Type_ConditionKind.Buff)
                return this._buffs;
            if (kind == Type_ConditionKind.Debuff)
                return this._debuffs;
            return null;
        }

        public Dictionary<long, BaseBattleBuff> GetAllBuffs() {
            return this._all_buffs;
        }

        public List<BaseBattleBuff> GetAllOrderedBuffs() {
            return this._ordereed_buff;
        }
    }
}