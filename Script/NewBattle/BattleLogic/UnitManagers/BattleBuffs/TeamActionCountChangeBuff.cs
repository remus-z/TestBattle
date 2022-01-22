using System.Collections;
using System.Collections.Generic;
namespace TestBattle
{
    public class TeamActionCountChangeBuff : BaseBattleBuff, IBuffAfterSkillCheckRemoveHandler
    {
        public TeamActionCountChangeBuff(BattleLogic battle, BattleUnit target, BattleUnit caster, SkillBuffInfo buff_data) : base(battle, target, caster, buff_data)
        {
        }

        public bool AfterSkillRemovable => true;

        public override void ParseData(SkillBuffInfo data)
        {
        }

        protected override void OnAdd()
        {
            this.Owner.BuffManager.AddModifierHandler<BuffAfterSkillCheckRemoveModifier, IBuffAfterSkillCheckRemoveHandler>(this);
            BattleTeam team = this._battle.GetManager<BattleUnitManager>().GetCurrentTeam(this.Owner.Camp);
            team.ChangePlayCount(this.Value);
        }

        protected override void OnRelease()
        {
            this.Owner.BuffManager.RemoveModifierHandler<BuffAfterSkillCheckRemoveModifier, IBuffAfterSkillCheckRemoveHandler>(this);
        }
    }
}
