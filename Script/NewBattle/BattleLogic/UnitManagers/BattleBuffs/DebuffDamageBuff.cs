using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public class DebuffDamageBuff : BaseBattleBuff, IDamageModifyHandler
    {

        private List<int> debuff_types = new List<int>();

        public DebuffDamageBuff(BattleLogic battle, BattleUnit target, BattleUnit caster, SkillBuffInfo buff_data) : base(battle, target, caster, buff_data)
        {
        }

        public override void ParseData(SkillBuffInfo data)
        {
            if (!string.IsNullOrEmpty(data.CheckValue)) {
                string[] debuffs = ((string)data.CheckValue).Split('|');
                for (int j = 0; j < debuffs.Length; j++)
                {
                    Type_Condition ct = (Type_Condition)System.Enum.Parse(typeof(Type_Condition), debuffs[j]);
                    debuff_types.Add((int)ct);
                }
            }
        }

        protected override void OnAdd()
        {
            this.Owner.BuffManager.AddModifierHandler<BuffAttackerDamageModifier, IDamageModifyHandler>(this);
        }

        protected override void OnRelease()
        {
            this.Owner.BuffManager.RemoveModifierHandler<BuffAttackerDamageModifier, IDamageModifyHandler>(this);
        }

        public int GetDamageRate(DamageChangeMessage msg)
        {
            int rate = 0;
            if (msg.Target != null) {
                if (this.BuffType == Type_Condition.debuff_type_extra_damage)
                {
                    if (debuff_types.Count == 0)
                    {
                        rate = this.Value;
                    }
                    else
                    {
                        for (int i = 0; i < this.debuff_types.Count; i++)
                        {
                            if (msg.Target.HasCondition((Type_Condition)this.debuff_types[i]))
                            {
                                rate += this.Value;
                            }
                        }
                    }
                }
                else if (this.BuffType == Type_Condition.debuff_num_extra_damage) {
                    int num = msg.Target.BuffManager.GetBuffByKind(Type_ConditionKind.Debuff).Count;
                    rate = this.Value * num;
                }
            }
            return 10000 + rate;
        }
    }
}
