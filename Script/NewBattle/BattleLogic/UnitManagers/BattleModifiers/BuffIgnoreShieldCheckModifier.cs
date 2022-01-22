using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public interface IIgnoreShieldHandler {
        bool IgnoreShield();
    }
    public class BuffIgnoreShieldCheckModifier : BaseBuffModifier<IIgnoreShieldHandler>
    {
        public BuffIgnoreShieldCheckModifier(BattleUnit owner) : base(owner)
        {
        }

        public bool IgnoreShield() {
            for (int i = 0; i < this._handlers.Count; i++) {
                if (this._handlers[i].IgnoreShield())
                    return true;
            }
            return false;
        }
    }
}
