using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public class ParalysisBuff : BaseBattleBuff, IBuffCastSkillRateCheckHandler
    {
        public ParalysisBuff(BattleLogic battle, BattleUnit target, BattleUnit caster, SkillBuffInfo buff_data) : base(battle, target, caster, buff_data)
        {
        }

        public override void ParseData(SkillBuffInfo data)
        {
            
        }

        protected override void OnAdd()
        {
            this.Owner.BuffManager.AddModifierHandler<BuffCastSkillRateCheckModifier, IBuffCastSkillRateCheckHandler>(this);

        }

        protected override void OnRemove()
        {
            this.Owner.BuffManager.RemoveModifierHandler<BuffCastSkillRateCheckModifier, IBuffCastSkillRateCheckHandler>(this);
        }

        public bool CheckCastSkillSucc()
        {
            int prob = this._battle.GetManager<BattleCalculator>().GetRandom(0, 10000);
            return prob < this.Value;
        }
    }
}