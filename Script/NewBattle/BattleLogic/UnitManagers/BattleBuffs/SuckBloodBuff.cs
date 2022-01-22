using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public class SuckBloodBuff : BaseBattleBuff, IBuffSuckBloodHandler, IBuffAfterSkillCheckRemoveHandler
    {
        public SuckBloodBuff(BattleLogic battle, BattleUnit target, BattleUnit caster, SkillBuffInfo buff_data) : base(battle, target, caster, buff_data)
        {
        }

        public bool IsAttackerBuff => true;

        public bool AfterSkillRemovable => this.BuffType == Type_Condition.atk_suck_blood;

        public int GetSuckBloodRate()
        {
            return this.Value;
        }

        public override void ParseData(SkillBuffInfo data)
        {
            
        }

        protected override void OnAdd()
        {
            this.Owner.BuffManager.AddModifierHandler<BuffSuckBloodCheckModifier, IBuffSuckBloodHandler>(this);
            this.Owner.BuffManager.AddModifierHandler<BuffAfterSkillCheckRemoveModifier, IBuffAfterSkillCheckRemoveHandler>(this);

        }

        protected override void OnRemove()
        {
            this.Owner.BuffManager.RemoveModifierHandler<BuffSuckBloodCheckModifier, IBuffSuckBloodHandler>(this);
            this.Owner.BuffManager.RemoveModifierHandler<BuffAfterSkillCheckRemoveModifier, IBuffAfterSkillCheckRemoveHandler>(this);

        }
    }
}
