using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public class LockSkillBuff : BaseBattleBuff, IBuffBlockSkillCheckHandler
    {
        protected List<int> _lock_skill_type = new List<int>();
        public LockSkillBuff(BattleLogic battle, BattleUnit target, BattleUnit caster, SkillBuffInfo buff_data) : base(battle, target, caster, buff_data)
        {
        }

        public override void ParseData(SkillBuffInfo data)
        {
            string[] lock_type = data.CheckValue.Split('|');
            for (int i = 0; i < lock_type.Length; i++) {
                int t = int.Parse(lock_type[i]);
                this._lock_skill_type.Add(t);
            }

        }

        protected override void OnAdd()
        {
            this.Owner.BuffManager.AddModifierHandler<BuffLockSkillCheckModifier, IBuffBlockSkillCheckHandler>(this);

        }

        protected override void OnRemove()
        {
            this.Owner.BuffManager.RemoveModifierHandler<BuffLockSkillCheckModifier, IBuffBlockSkillCheckHandler>(this);
            this._lock_skill_type.Clear();
        }

        public bool IsSkillBlock(IBattleUnitSkillData skill_data,int rank_level)
        {
            return this._lock_skill_type.Contains((int)skill_data.GetSkillAttribution(rank_level));
        }
    }
}
