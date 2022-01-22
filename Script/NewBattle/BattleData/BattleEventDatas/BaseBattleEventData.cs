using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public abstract class BaseBattleEventData : BattleCacheClass
    {
        public virtual BattleEvent BattleEvent => BattleEvent.None;
    }
}
