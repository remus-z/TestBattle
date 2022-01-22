using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type_Attribution
{
    StatStart = 0,

    None = 0,
    Attack,
    Physics_Defence,
    Magic_Defence,
    HP,
    Critical,
    Critical_Value,
    Critical_Defence,
    Legacy_Resist, 
    Legacy_Focus,
    Legacy_Luck,    

    StatEnd,

    Add_Exp = 12,
    Add_Gold = 13,

    SkillLevel = 100,
    FormationLevel,
    SkillLevel1 = 301,
    SkillLevel2 = 302,
    SkillLevel3 = 303,
}
