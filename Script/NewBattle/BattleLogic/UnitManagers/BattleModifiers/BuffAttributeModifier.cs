using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public interface IAttributeModifyHandler
    {
        int GetAttributeAddition(Type_Attribution attri_type);
    }
    public class BuffAttributeModifier : BaseBuffModifier<IAttributeModifyHandler>
    {
        public BuffAttributeModifier(BattleUnit owner) : base(owner)
        {
        }
        public int GetAttributeRate(int base_rate, Type_Attribution attri_type) {
            for (int i = 0; i < this._handlers.Count; i++) {
                base_rate += this._handlers[i].GetAttributeAddition( attri_type);
            }
            return base_rate;
        }
    }
}