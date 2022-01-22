using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public interface IBuffDevineShieldHandler {
        void GetDevineShieldHit(Type_Damage damage_type);
    }
    public class BuffDevineShieldCheckModifier : BaseBuffModifier<IBuffDevineShieldHandler>
    {
        public BuffDevineShieldCheckModifier(BattleUnit owner) : base(owner)
        {
        }

        public bool CheckDevineShield(Type_Damage damage_type) {
            bool has_devine_shield = this._handlers.Count > 0;
            if (has_devine_shield) {
                this._handlers[0].GetDevineShieldHit(damage_type);
            }
            return has_devine_shield;
        }
    }
}
