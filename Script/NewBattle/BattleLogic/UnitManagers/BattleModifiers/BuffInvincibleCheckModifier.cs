using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public interface IBuffInvincibaleCheckHandler{
        bool IsInvincible();
    }

    public class BuffInvincibleCheckModifier : BaseBuffModifier<IBuffInvincibaleCheckHandler>
    {
        public BuffInvincibleCheckModifier(BattleUnit owner) : base(owner)
        {
        }

        public bool IsInvincible() {
            for (int i = 0; i < this._handlers.Count; i++) {
                if (this._handlers[i].IsInvincible())
                    return true;
            }
            return false;
        }
    }
}
