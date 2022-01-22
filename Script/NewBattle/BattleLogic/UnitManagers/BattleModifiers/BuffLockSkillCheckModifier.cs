using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public interface IBuffBlockSkillCheckHandler {
        bool IsSkillBlock(IBattleUnitSkillData skill_data, int rank_level);
    }
    public class BuffLockSkillCheckModifier : BaseBuffModifier<IBuffBlockSkillCheckHandler>
    {
        public BuffLockSkillCheckModifier(BattleUnit owner) : base(owner)
        {
        }

        public bool CheckSkillLock(IBattleUnitSkillData skill_data,int rank_level) {
            bool block = false;
            for (int i = 0; i < this._handlers.Count; i++) {
                if (this._handlers[i].IsSkillBlock(skill_data, rank_level)) {
                    block = true;
                    break;
                }
            }
            return block;
        }
    }
}
