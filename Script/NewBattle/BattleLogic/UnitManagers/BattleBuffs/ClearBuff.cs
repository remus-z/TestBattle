using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public class ClearBuff : BaseBattleBuff,IBuffAfterSkillCheckRemoveHandler
    {
        private int _clear_type = -1;// 0:all ---- 1,buff ---- 2,debuff ---- 3,number ----- 4,buffnames
        private int _clear_nums = 0;
        private List<int> _clear_types = new List<int>();
        List<BaseBattleBuff> _clear_buffs = new List<BaseBattleBuff>();

        public bool AfterSkillRemovable => true;

        public ClearBuff(BattleLogic battle, BattleUnit target, BattleUnit caster, SkillBuffInfo buff_data) : base(battle, target, caster, buff_data)
        {
        }

        public override void ParseData(SkillBuffInfo data)
        {
            if (string.IsNullOrEmpty(data.CheckValue))
            {
                this._clear_type = 0;
            }
            else {
                string[] p = data.CheckValue.Split('|');
                this._clear_type = int.Parse(p[0]);
                if (p.Length > 1)
                {
                    if (this._clear_type == 3)
                    {
                        _clear_nums = this.Value;
                    }
                    else
                    {
                        for (int i = 1; i < p.Length; i++)
                        {
                            Type_Condition c = (Type_Condition)System.Enum.Parse(typeof(Type_Condition), p[i]);

                            this._clear_types.Add((int)c);
                        }
                    }
                }

            }
        }


        protected override void OnAdd()
        {
            this.CheckClearBuffs();
            this.Owner.BuffManager.AddModifierHandler<BuffAfterSkillCheckRemoveModifier, IBuffAfterSkillCheckRemoveHandler>(this);
        }

        public void CheckClearBuffs()
        {
            this._clear_buffs.Clear();
            switch (this._clear_type) {
                case 0://all
                    this._clear_buffs = this.Owner.BuffManager.GetAllOrderedBuffs();
                    break;
                case 1://buff
                    this._clear_buffs = this.Owner.BuffManager.GetBuffByKind(Type_ConditionKind.Buff);
                    break;
                case 2://debuff
                    this._clear_buffs = this.Owner.BuffManager.GetBuffByKind(Type_ConditionKind.Debuff);
                    break;
                case 3://nums
                    this._clear_buffs = this.Owner.BuffManager.GetAllOrderedBuffs().GetRange(0, this._clear_nums);
                    break;
                case 4://buff names
                    for (int i = 0; i < this._clear_types.Count; i++)
                    {
                        this._clear_buffs.AddRange(this.Owner.BuffManager.GetBuffByType((Type_Condition)this._clear_types[i]));
                    }
                    break;
            }
            for (int i = 0; i < this._clear_buffs.Count; i++) {
                BaseBattleBuff buff = this._clear_buffs[i];
                this.Owner.RemoveBuff(buff.BuffUID);
            }
            
        }

        protected override void OnRelease()
        {
            this.Owner.BuffManager.RemoveModifierHandler<BuffAfterSkillCheckRemoveModifier, IBuffAfterSkillCheckRemoveHandler>(this);
            this._clear_type = -1;
            this._clear_types.Clear();
        }
    }
}
