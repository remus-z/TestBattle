using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public class BattleUnitActiveSkill : IActiveSkill
    {
        public int RankLevel => throw new System.NotImplementedException();

        public int ID => throw new System.NotImplementedException();

        public int Level => throw new System.NotImplementedException();

        public Type_Skill SkillType => throw new System.NotImplementedException();

        public Type_Target SkillSelectTargetType => throw new System.NotImplementedException();

        public ISkillValue GetSkillValue(int index)
        {
            throw new System.NotImplementedException();
        }
    }
}
