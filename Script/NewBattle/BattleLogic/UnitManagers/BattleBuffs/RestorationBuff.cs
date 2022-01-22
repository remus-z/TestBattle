using System.Collections;
using System.Collections.Generic;
namespace TestBattle
{
    public class RestorationBuff : BaseBattleBuff,IBuffAfterSkillCheckRemoveHandler
    {
        private int _restoration_type = 0;
        public RestorationBuff(BattleLogic battle, BattleUnit target, BattleUnit caster, SkillBuffInfo buff_data) : base(battle, target, caster, buff_data)
        {
        }

        protected override void OnAdd()
        {
            base.OnAdd();
            this.Owner.BuffManager.AddModifierHandler<BuffAfterSkillCheckRemoveModifier, IBuffAfterSkillCheckRemoveHandler>(this);
            switch (this._restoration_type) {
                case 1:
                    {
                        int recover = (int)(this.Owner.RoundDamage * GameUtil.ToRate(this.Value));
                        this.Owner.AddHp(this.Caster, recover);
                    }
                    break;
                case 2:
                    {
                        int recover = (int)(this.Caster.GetAttribute(Type_Attribution.Attack) * GameUtil.ToRate(this.Value));
                        this.Owner.AddHp(this.Caster, recover);
                    }
                    break;
                default:
                    {
                        this.Owner.AddHp(this.Caster, this.Value);
                    }
                    break;
            }
            
        }

        protected override void OnRemove()
        {
            this._restoration_type = 0;
            this.Owner.BuffManager.RemoveModifierHandler<BuffAfterSkillCheckRemoveModifier, IBuffAfterSkillCheckRemoveHandler>(this);
        }

        public bool IsAttackerBuff => true;

        public bool AfterSkillRemovable => true;

        public override void ParseData(SkillBuffInfo data)
        {
            if (!string.IsNullOrEmpty(data.CheckValue))
            {
                this._restoration_type = int.Parse(data.CheckValue);
            }
        }
    }
}