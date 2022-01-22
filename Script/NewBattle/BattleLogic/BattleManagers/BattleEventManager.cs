using System.Collections;
using System.Collections.Generic;
namespace TestBattle
{
    public enum BattleEvent
    {
        None,
        BattleUnitCreate,
        BattleUnitRemove,
        BattleUnitJoinBattle,
        BattleUnitQuitBattle,
        BattleUnitDead,
        BattleUnitDamage,
        BattleUnitShieldChanged,
        BattleUnitDrewCards,
        BattleUnitUseCards,
        BattleRoundEnd,
    }

    public class BattleEventManager : BattleBaseManager
    {
        public delegate void BattleEventHandler(object sender, object data);

        private Dictionary<int, BattleEventHandler> _handlers = new Dictionary<int, BattleEventHandler>();

        public override void OnInit()
        {

        }

        public override void OnRelease()
        {
            this._handlers.Clear();
        }

        public void AddListener(BattleEvent event_type, BattleEventHandler handler)
        {
            int id = (int)event_type;
            BattleEventHandler h = null;
            if (!this._handlers.TryGetValue(id, out h))
            {
                h = handler;
                this._handlers.Add(id, handler);
            }
            else
            {
                h += handler;
            }
        }

        public void RemoveListener(BattleEvent event_type, BattleEventHandler handler)
        {
            int id = (int)event_type;
            BattleEventHandler h = null;
            if (this._handlers.TryGetValue(id, out h))
            {
                h -= handler;
            }
        }

        /// <summary>
        /// never cache a BaseBattleEventData in callback
        /// </summary>
        public void SendMessage(BattleEvent event_type, object sender, object data)
        {
            int id = (int)event_type;
            BattleEventHandler h = null;
            if (this._handlers.TryGetValue(id, out h))
            {
                h.Invoke(sender, data);
                if (data is BaseBattleEventData) {
                    BattleClassCache.Instance.Return((BattleCacheClass)data);
                }
            }
        }
    }
}