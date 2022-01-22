using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public interface IBuffClearHandler {
        List<long> CheckClearBuffs();
    }
    public class BuffClearCheckModifier : BaseBuffModifier<IBuffClearHandler>
    {
        public BuffClearCheckModifier(BattleUnit owner) : base(owner)
        {
        }

        public List<long> CheckClearBuffs() {
            List<long> clears = new List<long>();
            for (int i = 0; i<this._handlers.Count; i++) {
                List<long> buffs = this._handlers[i].CheckClearBuffs();
                for (int j = 0; j < buffs.Count; j++) {
                    if (!clears.Contains(buffs[j])) {
                        clears.Add(buffs[j]);
                    }
                }
            }
            for (int j = 0; j < clears.Count; j++)
            {
                this.Owner.RemoveBuff(clears[j]);
            }
            return clears;
        }
    }
}
