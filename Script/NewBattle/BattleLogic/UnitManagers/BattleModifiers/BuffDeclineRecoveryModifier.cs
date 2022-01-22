using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public interface IDeclineRecoveryHandler {
        int GetDeclineRate();
    }
    public class BuffDeclineRecoveryModifier : BaseBuffModifier<IDeclineRecoveryHandler>
    {
        public BuffDeclineRecoveryModifier(BattleUnit owner) : base(owner)
        {
        }

        public int ModifyRecovery(float recover_value)
        {
            float rate = 0;
            for (int i = 0; i < this._handlers.Count; i++)
            {
                rate += GameUtil.ToRate(this._handlers[i].GetDeclineRate());
            }
            if (rate > 1)
                rate = 1;
            return (int)(recover_value * (1 - rate));
        }
    }
}
