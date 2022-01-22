using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public class TrueDamageBuff : BaseBattleBuff,IIgnoreShieldHandler,IFinalDefenceHandler,IDamageModifyHandler
    {
        public TrueDamageBuff(BattleLogic battle, BattleUnit target, BattleUnit caster, SkillBuffInfo buff_data) : base(battle, target, caster, buff_data)
        {
        }


        protected override void OnAdd()
        {
            this.Owner.BuffManager.AddModifierHandler<BuffIgnoreDefenceModifier, IFinalDefenceHandler>(this);
            this.Owner.BuffManager.AddModifierHandler<BuffIgnoreShieldCheckModifier, IIgnoreShieldHandler>(this);
            this.Owner.BuffManager.AddModifierHandler<BuffAttackerDamageModifier, IDamageModifyHandler>(this);
        }

        protected override void OnRelease()
        {
            this.Owner.BuffManager.RemoveModifierHandler<BuffIgnoreDefenceModifier, IFinalDefenceHandler>(this);
            this.Owner.BuffManager.RemoveModifierHandler<BuffIgnoreShieldCheckModifier, IIgnoreShieldHandler>(this);
            this.Owner.BuffManager.RemoveModifierHandler<BuffAttackerDamageModifier, IDamageModifyHandler>(this);

        }

        public int GetFinalDefence(int origin)
        {
            return 0;
        }


        public bool IgnoreShield()
        {
            return true;
        }

        public override void ParseData(SkillBuffInfo data)
        {
            
        }

        public int GetDamageRate(DamageChangeMessage msg)
        {
            return this.Value;
        }
    }
}
