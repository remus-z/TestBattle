using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public class BackRowDamageBuff : BaseBattleBuff, IDamageModifyHandler
    {
        
        public BackRowDamageBuff(BattleLogic battle, BattleUnit target, BattleUnit caster, SkillBuffInfo buff_data) : base(battle, target, caster, buff_data)
        {
        }

        public override void ParseData(SkillBuffInfo data)
        {
            
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
            if (msg.DamageType == Type_Damage.Skill)
            {
                if (msg.Target.RowType == TeamRowType.Team_Back_Row)
                {
                    return 10000 + this.Value;
                }
            }
            return 0;
        }


    }
}
