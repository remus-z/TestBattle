using System.Collections;
using System.Collections.Generic;

namespace TestBattle {
    public interface IBuffAfterSkillCheckRemoveHandler {
        bool AfterSkillRemovable { get; }
    }

    public class BuffAfterSkillCheckRemoveModifier : BaseBuffModifier<IBuffAfterSkillCheckRemoveHandler>
    {
        private List<long> _removable_buffs = new List<long>();

        public BuffAfterSkillCheckRemoveModifier(BattleUnit owner) : base(owner)
        {
        }

        public void CheckAfterSkillBuffRemove() {
            this._removable_buffs.Clear();
            for (int i = 0; i < this._handlers.Count; i++)
            {
                if (this._handlers[i].AfterSkillRemovable) {
                    //if (this._handlers[i].IsAttackerBuff && attacker == this.Owner ||  !this._handlers[i].IsAttackerBuff) {
                        this._removable_buffs.Add(((BaseBattleBuff)this._handlers[i]).BuffUID);
                    //}
                }
            }
            for (int i = 0; i < this._removable_buffs.Count; i++) {
                this.Owner.RemoveBuff(this._removable_buffs[i]);
            }
            this._removable_buffs.Clear();
        }
    }
}