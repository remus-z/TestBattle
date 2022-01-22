using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TestBattle
{
    public class BattleActionData
    {
        public List<PhaseActionData> PhaseDatas = new List<PhaseActionData>();

        //public 
    }

    public class PhaseActionData
    {
        public int PhaseID;
        public List<RoundActionData> RoundDatas = new List<RoundActionData>();
    }

    public class RoundActionData
    {
        public int RoundID;
        public List<TeamActionData> TeamDatas = new List<TeamActionData>();
    }

    public class TeamActionData
    {
        public Type_BattleCamp Camp;
        public List<SkillData> SkillDatas = new List<SkillData>();
    }
}