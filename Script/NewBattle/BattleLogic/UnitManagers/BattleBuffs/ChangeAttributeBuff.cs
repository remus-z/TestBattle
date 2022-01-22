using System.Collections;
using System.Collections.Generic;
using System;

namespace TestBattle
{
    public class ChangeAttributeBuff : BaseBattleBuff, IAttributeModifyHandler
    {
        int add = 1;
        private Type_Attribution _attri_type;

        private int _max_value = -1;
        public ChangeAttributeBuff(BattleLogic battle, BattleUnit target, BattleUnit caster, SkillBuffInfo buff_data) : base(battle, target, caster, buff_data)
        {
            switch ((Type_Condition)buff_data.BuffType)
            {
                case Type_Condition.attack_up:
                    _attri_type = Type_Attribution.Attack;
                    break;
                case Type_Condition.critical_up:
                    _attri_type = Type_Attribution.Critical;
                    break;
                case Type_Condition.critical_value_up:
                    _attri_type = Type_Attribution.Critical_Value;
                    break;
                case Type_Condition.defence_physical_up:
                    _attri_type = Type_Attribution.Physics_Defence;
                    break;
                case Type_Condition.defence_magic_up:
                    _attri_type = Type_Attribution.Magic_Defence;
                    break;
                case Type_Condition.defence_critical_up:
                    _attri_type = Type_Attribution.Critical_Defence;
                    break;
                case Type_Condition.attack_down:
                    add = -1;
                    _attri_type = Type_Attribution.Attack;
                    break;
                case Type_Condition.critical_down:
                    _attri_type = Type_Attribution.Critical;
                    add = -1;
                    break;
                case Type_Condition.critical_value_down:
                    add = -1;
                    _attri_type = Type_Attribution.Critical_Value;
                    break;
                case Type_Condition.defence_physical_down:
                    add = -1;
                    _attri_type = Type_Attribution.Physics_Defence;
                    break;
                case Type_Condition.defence_magic_down:
                    add = -1;
                    _attri_type = Type_Attribution.Magic_Defence;
                    break;
                case Type_Condition.defence_critical_down:
                    add = -1;
                    _attri_type = Type_Attribution.Critical_Defence;
                    break;
            }
        }

        public int GetAttributeAddition(Type_Attribution attri_type)
        {
            if (this._attri_type == attri_type) {
                return this.Value;
            }
            return 0;
        }

        public override void ParseData(SkillBuffInfo data)
        {

            //throw new System.NotImplementedException();
            if (data.ExistType == (int)Type_BuffExist.Overlap && !string.IsNullOrEmpty(data.ExistValue)) {
                this._max_value = int.Parse(data.ExistValue);
            }
        }

        public override void Overlap(BaseBattleBuff buff)
        {
            base.Overlap(buff);
            if (buff.BuffType != this.BuffType) {
                BattleLog.LogError("canot reach herr: base :" + buff.BuffType + ",new :" + this._attri_type);
                return;
            }
            this.Value += buff.Value;
            if (this._max_value >= 0) {
                this.Value = Math.Min(this.Value, this._max_value);
            }
        }

        protected override void OnAdd()
        {
            this.Owner.BuffManager.AddModifierHandler<BuffAttributeModifier, IAttributeModifyHandler>(this);
        }

        protected override void OnRelease()
        {
            this.Owner.BuffManager.RemoveModifierHandler<BuffAttributeModifier, IAttributeModifyHandler>(this);
            this._max_value = -1;
        }
    }
}
