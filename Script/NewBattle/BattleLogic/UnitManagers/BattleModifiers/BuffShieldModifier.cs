using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public interface IShieldBuffHandler {
        int DefendValue(BattleUnit attacker, int origin_damage);
    }
    public class BuffShieldModifier : BaseBuffModifier<IShieldBuffHandler>
    {
        public BuffShieldModifier(BattleUnit owner) : base(owner)
        {
        }

        public int ShieldDefendValue(BattleUnit attacker, int origin_damage) {
            int defend_value = 0;
            int rest_damage = origin_damage;
            for (int i = 0; i < this._handlers.Count; i++) {
                defend_value += this._handlers[i].DefendValue(attacker,rest_damage);
                rest_damage -= defend_value;
            }
            return defend_value;
        }
    }
}
