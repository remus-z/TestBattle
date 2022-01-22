using System.Collections;
using System.Collections.Generic;
namespace TestBattle
{
    public class BattleUnitDamageEventData : BaseBattleEventData
    {
        public override BattleEvent BattleEvent => BattleEvent.BattleUnitDamage;
        public BattleUnit Source;
        public BattleUnit Target;
        public Type_Damage Type_Damage;
        public int Value;
        public void Init(BattleUnit source,BattleUnit target,Type_Damage dt,int value)
        {
            this.Source = source;
            this.Target = target;
            this.Type_Damage = dt;
            this.Value = value;
        }

        public override void Reset()
        {
            this.Source = null;
            this.Target = null;
            this.Type_Damage = Type_Damage.None;
            this.Value = 0;
        }

        public static BattleUnitDamageEventData CreateEventData(BattleUnit souorce, BattleUnit target, Type_Damage dt, int value) {
            BattleUnitDamageEventData data = BattleClassCache.Instance.GetInstance<BattleUnitDamageEventData>();
            data.Init(souorce, target, dt, value);
            return data;
        }
    }
}
