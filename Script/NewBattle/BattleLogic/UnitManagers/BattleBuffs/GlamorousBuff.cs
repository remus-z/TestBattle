using System.Collections;
using System.Collections.Generic;
namespace TestBattle
{
    public class GlamorousBuff : BaseBattleBuff, IDamageModifyHandler, IBuffCastSkillRateCheckHandler
    {
        public GlamorousBuff(BattleLogic battle, BattleUnit target, BattleUnit caster, SkillBuffInfo buff_data) : base(battle, target, caster, buff_data)
        {
        }

        public bool CheckCastSkillSucc()
        {
            return false;
        }

        protected override void OnAdd()
        {
            this.Owner.BuffManager.AddModifierHandler<BuffTargetDamageModifier, IDamageModifyHandler>(this);
            this.Owner.BuffManager.AddModifierHandler<BuffCastSkillRateCheckModifier, IBuffCastSkillRateCheckHandler>(this);
        }

        protected override void OnRemove()
        {
            this.Owner.BuffManager.RemoveModifierHandler<BuffTargetDamageModifier, IDamageModifyHandler>(this);
            this.Owner.BuffManager.RemoveModifierHandler<BuffCastSkillRateCheckModifier, IBuffCastSkillRateCheckHandler>(this);
        }

        public int GetDamageRate(DamageChangeMessage msg)
        {
            return 10000 + this.Value;
        }

        public override void ParseData(SkillBuffInfo data)
        {
        }
    }
}