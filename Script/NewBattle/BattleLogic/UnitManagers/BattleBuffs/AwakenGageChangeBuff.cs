using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public class AwakenGageChangeBuff : BaseBattleBuff,IBuffAfterSkillCheckRemoveHandler
    {
        public AwakenGageChangeBuff(BattleLogic battle, BattleUnit target, BattleUnit caster, SkillBuffInfo buff_data) : base(battle, target, caster, buff_data)
        {
        }

        public bool AfterSkillRemovable => true;

        public override void ParseData(SkillBuffInfo data)
        {
        }

        protected override void OnAdd()
        {
            this.Owner.BuffManager.AddModifierHandler<BuffAfterSkillCheckRemoveModifier, IBuffAfterSkillCheckRemoveHandler>(this);
            int gage_value = this.Value;
            if (this.BuffType == Type_Condition.awaken_gage_down) {
                gage_value = -this.Value;
            }
            int stat = this.Owner.ChangeAwakenGage(gage_value);
            if (stat == 1)
            {
                this._battle.GetManager<BattleUnitManager>().GetCurrentTeam(this.Owner.Camp).CardManager.DrawAwakenCard(this.Owner.UnitID);
            }
            else {
                this._battle.GetManager<BattleUnitManager>().GetCurrentTeam(this.Owner.Camp).CardManager.ReclaimAwakenCard(this.Owner.UnitID);
            }
        }

        protected override void OnRemove()
        {
            this.Owner.BuffManager.RemoveModifierHandler<BuffAfterSkillCheckRemoveModifier, IBuffAfterSkillCheckRemoveHandler>(this);
        }
    }
}
