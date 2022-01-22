using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public interface IFinalDefenceHandler {
        int GetFinalDefence(int origin);
    }

    public class BuffIgnoreDefenceModifier : BaseBuffModifier<IFinalDefenceHandler>
    {
        public BuffIgnoreDefenceModifier(BattleUnit owner) : base(owner)
        {
        }

        public int GetFinalDefence(int origin) {
            for (int i = 0; i < this._handlers.Count; i++) {
                origin = this._handlers[i].GetFinalDefence(origin);
            }
            return origin;
        }
    }
}