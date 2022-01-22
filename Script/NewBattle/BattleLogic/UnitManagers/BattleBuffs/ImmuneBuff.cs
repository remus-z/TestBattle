using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public class ImmuneBuff : BaseBattleBuff, IBuffImmuneCheckHandler
    {
        private int _immune_type = -1;// 0:all ---- 1,buff ---- 2,debuff ---- 3,buffnames
        private List<int> _immune_types = new List<int>();

        public ImmuneBuff(BattleLogic battle, BattleUnit target, BattleUnit caster, SkillBuffInfo buff_data) : base(battle, target, caster, buff_data)
        {
        }

        public override void ParseData(SkillBuffInfo data)
        {
            if (string.IsNullOrEmpty(data.CheckValue))
            {
                this._immune_type = 0;
            }
            else
            {
                string[] p = data.CheckValue.Split('|');
                this._immune_type = int.Parse(p[0]);
                if (p.Length > 1)
                {
                    for (int i = 1; i < p.Length; i++)
                    {
                        Type_Condition c = (Type_Condition)System.Enum.Parse(typeof(Type_Condition), p[i]);
                        this._immune_types.Add((int)c);
                    }
                }

            }
        }

        protected override void OnAdd()
        {
            this.Owner.BuffManager.AddModifierHandler<BuffImmuneCheckModifier, IBuffImmuneCheckHandler>(this);
        }

        public bool IsImmune(SkillBuffInfo buff_info)
        {
            if (this._immune_type == 0)
                return true;

            if (this._immune_type == 1)
            {
                return buff_info.BuffKind == (int)Type_ConditionKind.Buff;
            }
            else if (this._immune_type == 2)
            {
                return buff_info.BuffKind == (int)Type_ConditionKind.Debuff;
            }
            else { 
                return _immune_types.Contains(buff_info.BuffType);
            }
        }

        protected override void OnRelease() {
            this.Owner.BuffManager.RemoveModifierHandler<BuffImmuneCheckModifier, IBuffImmuneCheckHandler>(this);
            this._immune_type = -1;
            this._immune_types.Clear();
        }
#if UNITY_EDITOR
        public override void OnGUI()
        {
            UnityEditor.EditorGUILayout.LabelField("Immune type", this._immune_type.ToString());
            for (int i = 0; i < this._immune_types.Count; i++) {
                UnityEditor.EditorGUILayout.LabelField("Immune type", ((Type_Condition) this._immune_types[i]).ToString());
            }
        }

#endif 
    }
}
