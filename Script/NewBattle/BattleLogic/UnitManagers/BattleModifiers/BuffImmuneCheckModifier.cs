using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public interface IBuffImmuneCheckHandler {
        bool IsImmune(SkillBuffInfo buff_info); 
    }

    public class BuffImmuneCheckModifier : BaseBuffModifier<IBuffImmuneCheckHandler>
    {
        public BuffImmuneCheckModifier(BattleUnit owner) : base(owner)
        {
        }

        public bool IsImmune(SkillBuffInfo buff_info) {
            for (int i = 0; i < this._handlers.Count; i++) {
                if (this._handlers[i].IsImmune(buff_info)) {
                    return true;
                }
            }
            return false;
        }
    }
}
