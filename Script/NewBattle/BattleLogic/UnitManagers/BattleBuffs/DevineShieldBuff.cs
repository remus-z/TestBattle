using System.Collections;
using System.Collections.Generic;
namespace TestBattle
{
    public class DevineShieldBuff : BaseBattleBuff, IBuffDevineShieldHandler, IBuffAfterSkillCheckRemoveHandler
    {
        private bool _get_hit = false;
        public DevineShieldBuff(BattleLogic battle, BattleUnit target, BattleUnit caster, SkillBuffInfo buff_data) : base(battle, target, caster, buff_data)
        {
        }

        public override void ParseData(SkillBuffInfo data)
        {
            
        }

        public bool IsAttackerBuff => false;

        public bool AfterSkillRemovable => this.BuffType == Type_Condition.shield_devine_once && this._get_hit;

        public void GetDevineShieldHit(Type_Damage damage_type)
        {
            if (damage_type == Type_Damage.Skill)
            {
                this._get_hit = true;
            }
        }

        protected override void OnAdd()
        {
            this.Owner.BuffManager.AddModifierHandler<BuffDevineShieldCheckModifier,IBuffDevineShieldHandler>(this);
            this.Owner.BuffManager.AddModifierHandler<BuffAfterSkillCheckRemoveModifier,IBuffAfterSkillCheckRemoveHandler>(this);
        }

        protected override void OnRemove()
        {
            this.Owner.BuffManager.RemoveModifierHandler<BuffDevineShieldCheckModifier, IBuffDevineShieldHandler>(this);
            this.Owner.BuffManager.RemoveModifierHandler<BuffAfterSkillCheckRemoveModifier, IBuffAfterSkillCheckRemoveHandler>(this);
        }
        protected override void OnRelease()
        {
            base.OnRelease();
            this._get_hit = false;
        }
    }
}
