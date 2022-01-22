using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public interface IBuffSkillInspireCheckHandler {
        bool InspireSucc();
    }
    public class BuffSkillInspireCheckModifier : BaseBuffModifier<IBuffSkillInspireCheckHandler>
    {
        public BuffSkillInspireCheckModifier(BattleUnit owner) : base(owner)
        {
        }
        public bool CheckInspireSucc() {
            for (int i = 0; i < this._handlers.Count; i++) {
                if (this._handlers[i].InspireSucc()) {
                    return true;
                }
            }
            return false;
        }
    }
}