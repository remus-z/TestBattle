using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public class ChangeAttackTypeBuff : BaseBattleBuff, IBuffAttackTypeHandler
    {
        public ChangeAttackTypeBuff(BattleLogic battle, BattleUnit target, BattleUnit caster, SkillBuffInfo buff_data) : base(battle, target, caster, buff_data)
        {
        }


        public override void ParseData(SkillBuffInfo data)
        {
        }

        protected override void OnAdd()
        {
            this.Owner.BuffManager.AddModifierHandler<BuffAttackTypeModifer, IBuffAttackTypeHandler>(this);
        }

        protected override void OnRelease()
        {
            this.Owner.BuffManager.RemoveModifierHandler<BuffAttackTypeModifer, IBuffAttackTypeHandler>(this);
        }

        public Type_Attack GetAttackType(Type_Attack origin_attack)
        {
            return Type_Attack.Magical;
        }
    }
}
