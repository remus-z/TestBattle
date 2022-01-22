using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type_Target
{
    None = 0,

    AllyOne,
    AllySelf,
    AllyAll,
    EnemyOne,
    EnemyAll,
    EnemyNearest,
    EnemyFrontRow,
    EnemyBackRow,
    AllyLowestHp,
    EnemyLowestHp,
    EnemySameColumn,
    AllyOrientationFront,
    AllyOrientationMiddle,
    AllyOrientationBack,
    AllyAttriEngineering,
    AllyAttriNature,
    AllyAttriMagic,
    EnemyColumn,
    EnemyRow,
    EnemyFrontOne,
    EnemyPhysical,
    EnemyMagical,
    AllyFrontRow,
    CustomType = 999,
}
