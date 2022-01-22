using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public class BattleUnitDeadEventData : BaseBattleEventData
    {
        public override BattleEvent BattleEvent => BattleEvent.BattleUnitDead;

        public BattleUnit Source;
        public BattleUnit Target;
        public Type_Damage Type_Damage;

        public void Init(BattleUnit source, BattleUnit target, Type_Damage dt)
        {
            this.Source = source;
            this.Target = target;
            this.Type_Damage = dt;
        }

        public override void Reset()
        {
            this.Source = null;
            this.Target = null;
            this.Type_Damage = Type_Damage.None;
        }

        public static BattleUnitDeadEventData CreateEventData(BattleUnit souorce, BattleUnit target, Type_Damage dt)
        {
            BattleUnitDeadEventData data = BattleClassCache.Instance.GetInstance<BattleUnitDeadEventData>();
            data.Init(souorce, target, dt);
            return data;
        }
    }
}
