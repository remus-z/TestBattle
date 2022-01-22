using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestBattle {
    public enum Type_BattleCamp {
        None = -1,
        Ally,
        Enemy,
        Count,
    }

    public enum Type_BattleRelationship {
        None = -1,
        Friendly,
        Hostile,
    }

    public enum Type_HitType {
        None = -1,
        Physical,
        Magical,
        Recovery,
    }
    public enum Type_BuffExist {
        Substitute = 0,
        Coexist, 
        Overlap,
    }

    public enum Type_BuffState
    {
        None = 0,
        Add,
        Remove,
        Substitute,
        Overlap,
    }

    public enum Type_Damage {
        None = 0,
        Skill = 1,
        Dot = 2,
        Refection ,
        Addition,
        SuckBlood,
    }

    public enum Type_SkillTriggerTime
    {
        None = 0,//active skill
        BattleStart = 1,
        PhaseStart = 2,//not implement yet
        SelfTurnBegin = 3,
        SelfTurnEnd = 4,
        TeamTurnBegin = 5,
        TeamTurnEnd = 6,//not implement yet
        SkillHitTarget = 7,
        HitCritical = 8,
        GetHit = 9,
        GetHitCritical = 10,
        KillTarget = 11,
        CastSkill = 12,
    }

    public enum Type_BattleRandom {
        None,
    }

    public enum Type_BattleTurnEndState {
        None = 0,
        Win,
        Lose,
        PhaseEnd,
        TeamTurnEnd,
    }
}