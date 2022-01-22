using System.Collections;
using System.Collections.Generic;
namespace TestBattle
{
    public class AdditionalDamageBuff : BaseBattleBuff, IBuffAdditionalDamageCheckHandler, IBuffAfterSkillCheckRemoveHandler
    {
        public AdditionalDamageBuff(BattleLogic battle, BattleUnit target, BattleUnit caster, SkillBuffInfo buff_data) : base(battle, target, caster, buff_data)
        {
        }

        public bool IsAttackerBuff => this.BuffData.BuffType == (int)Type_Condition.damage_addition;

        public bool AfterSkillRemovable => this.BuffData.BuffType == (int)Type_Condition.damage_addition;

        public int GetAdditionalDamageRate()
        {
            return this.Value;
        }

        protected override void OnAdd()
        {
            this.Owner.BuffManager.AddModifierHandler<BuffAdditionalDamageCheckModifier, IBuffAdditionalDamageCheckHandler>(this);
            this.Owner.BuffManager.AddModifierHandler<BuffAfterSkillCheckRemoveModifier, IBuffAfterSkillCheckRemoveHandler>(this);
        }

        protected override void OnRemove()
        {
            this.Owner.BuffManager.RemoveModifierHandler<BuffAdditionalDamageCheckModifier, IBuffAdditionalDamageCheckHandler>(this);
            this.Owner.BuffManager.RemoveModifierHandler<BuffAfterSkillCheckRemoveModifier, IBuffAfterSkillCheckRemoveHandler>(this);
        }

        public override void ParseData(SkillBuffInfo data)
        {
            
        }
    }
}