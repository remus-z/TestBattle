using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public interface IBuffDotHandler {
        bool IsDebuffDOT { get; }
        int GetDotDamage(out int defend_damage);
    }
    public class BuffDotModifier : BaseBuffModifier<IBuffDotHandler>
    {
        public BuffDotModifier(BattleUnit owner) : base(owner)
        {
        }

        public int GetDotDamage(out int defend_damage) {
            defend_damage = 0;
            int dot_value = 0;
            for (int i = 0; i < this._handlers.Count; i++) {
                int def = 0;
                if (this._handlers[i].IsDebuffDOT && !this.Owner.BuffManager.GetModifier<BuffDevineShieldCheckModifier>().CheckDevineShield(Type_Damage.Dot))
                {
                    dot_value += this._handlers[i].GetDotDamage(out def);
                }
                defend_damage += def;
            }
            return dot_value;
        }
    }
}
