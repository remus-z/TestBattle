using System.Collections;
using System.Collections.Generic;
namespace TestBattle {

    public interface IBuffCastSkillRateCheckHandler {
        bool CheckCastSkillSucc();
    }

    public class BuffCastSkillRateCheckModifier : BaseBuffModifier<IBuffCastSkillRateCheckHandler>
    {
        public BuffCastSkillRateCheckModifier(BattleUnit owner) : base(owner)
        {
        }
        public bool CanCastSkill() {
            for (int i = 0; i < this._handlers.Count; i++) {

                if (!this._handlers[i].CheckCastSkillSucc())
                    return false;
            }
            return true;
        }

    }

}

