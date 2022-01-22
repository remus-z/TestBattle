using System.Collections;
using System.Collections.Generic;
namespace TestBattle
{

    public class BattleUnitShieldEventData : BaseBattleEventData
    {
        public override BattleEvent BattleEvent => BattleEvent.BattleUnitShieldChanged;

        public BattleUnit Source;
        public BattleUnit Target;
        public int ShieldValue;

        public void Init(BattleUnit source, BattleUnit target, int shield_value)
        {
            this.Source = source;
            this.Target = target;
            this.ShieldValue = shield_value;
        }

        public override void Reset()
        {
            this.Source = null;
            this.Target = null;
            this.ShieldValue = 0;
        }

        public static BattleUnitShieldEventData CreateEventData(BattleUnit souorce, BattleUnit target, int shield_value)
        {
            BattleUnitShieldEventData data = BattleClassCache.Instance.GetInstance<BattleUnitShieldEventData>();
            data.Init(souorce, target, shield_value);
            return data;
        }
    }
}
