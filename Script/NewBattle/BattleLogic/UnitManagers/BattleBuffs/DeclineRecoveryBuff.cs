using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public class DeclineRecoveryBuff : BaseBattleBuff, IDeclineRecoveryHandler
    {
        public DeclineRecoveryBuff(BattleLogic battle, BattleUnit target, BattleUnit caster, SkillBuffInfo buff_data) : base(battle, target, caster, buff_data)
        {
        }

        public override void ParseData(SkillBuffInfo data)
        {
            
        }

        protected override void OnAdd()
        {
            this.Owner.BuffManager.AddModifierHandler<BuffDeclineRecoveryModifier, IDeclineRecoveryHandler>(this);
        }

        protected override void OnRemove()
        {
            this.Owner.BuffManager.RemoveModifierHandler<BuffDeclineRecoveryModifier, IDeclineRecoveryHandler>(this);
        }

        public int GetDeclineRate()
        {
            return this.Value;
        }
    }
}
