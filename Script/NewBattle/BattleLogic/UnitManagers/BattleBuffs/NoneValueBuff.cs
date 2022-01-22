using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public class NoneValueBuff : BaseBattleBuff
    {
        public NoneValueBuff(BattleLogic battle, BattleUnit target, BattleUnit caster, SkillBuffInfo buff_data) : base(battle, target, caster, buff_data)
        {
        }

        public override void ParseData(SkillBuffInfo data)
        {
           
        }

    }
}
