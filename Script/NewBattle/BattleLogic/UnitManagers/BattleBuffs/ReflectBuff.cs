using System.Collections;
using System.Collections.Generic;
namespace TestBattle
{
    public class ReflectBuff : BaseBattleBuff, IBuffReflectHandler
    {
        public ReflectBuff(BattleLogic battle, BattleUnit target, BattleUnit caster, SkillBuffInfo buff_data) : base(battle, target, caster, buff_data)
        {
        }
        public override void ParseData(SkillBuffInfo data)
        {
        }

        protected override void OnAdd()
        {
            this.Owner.BuffManager.AddModifierHandler<BuffReflectModifier, IBuffReflectHandler>(this);

        }

        protected override void OnRemove()
        {
            this.Owner.BuffManager.RemoveModifierHandler<BuffReflectModifier, IBuffReflectHandler>(this);
        }

        public int GetReflectRate()
        {
            return this.Value;
        }
    }
}
