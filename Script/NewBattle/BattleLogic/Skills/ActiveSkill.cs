using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestBattle
{
    public class ActiveSkill : ISkillData
    {
        public readonly int RankLevel;
        public ActiveSkill(int rank_level) { 
            //for init skill entity
        }

        
        public int ID => throw new System.NotImplementedException();

        public int Level => throw new System.NotImplementedException();

        public Type_Skill SkillType => throw new System.NotImplementedException();

        public ISkillTarget GetTarget(int index)
        {
            throw new System.NotImplementedException();
        }
    }
}
