using System.Collections;
using System.Collections.Generic;
namespace TestBattle
{
    public class InspireBuff : BaseBattleBuff, IBuffSkillInspireCheckHandler
    {
        public InspireBuff(BattleLogic battle, BattleUnit target, BattleUnit caster, SkillBuffInfo buff_data) : base(battle, target, caster, buff_data)
        {
        }

        public override void ParseData(SkillBuffInfo data)
        {
        }


        protected override void OnAdd()
        {
            this.Owner.BuffManager.AddModifierHandler<BuffSkillInspireCheckModifier, IBuffSkillInspireCheckHandler>(this);

        }

        protected override void OnRemove()
        {
            this.Owner.BuffManager.RemoveModifierHandler<BuffSkillInspireCheckModifier, IBuffSkillInspireCheckHandler>(this);
        }

        public bool InspireSucc()
        {
            int prob = this._battle.GetManager<BattleCalculator>().GetRandom(0, 10000);
            return prob < this.Value;
        }
    }
}