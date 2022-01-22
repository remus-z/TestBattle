using System.Collections;
using System.Collections.Generic;
using System;

namespace TestBattle
{
    public class TargetChangeDamageBuff : BaseBattleBuff, IDamageModifyHandler
    {
        private int change_type = 0;
        private Type_BattleRelationship _check_camp = Type_BattleRelationship.None;
        private int hp_change_rate = 0;

        private int coe = 0;
        public TargetChangeDamageBuff(BattleLogic battle, BattleUnit target, BattleUnit caster, SkillBuffInfo buff_data) : base(battle,target, caster, buff_data)
        {
            coe = 1;
            if (buff_data.BuffType == (int)Type_Condition.damage_down) {
                coe = -1;
            }
        }


        public override void ParseData(SkillBuffInfo data)
        {
            this.change_type = 0;
            if (!string.IsNullOrEmpty(data.CheckValue))
            {
                string[] p = data.CheckValue.Split('|');
                if (p.Length > 1)
                {
                    if (!int.TryParse(p[0], out this.change_type) || !int.TryParse(p[1], out this.hp_change_rate))
                    {
                        this.change_type = 0;
                        this._BuffCheckValueError();
                    }

                }
            }
        }

        protected override void OnAdd()
        {
            this.Owner.BuffManager.AddModifierHandler<BuffTargetDamageModifier, IDamageModifyHandler>(this);
        }

        protected override void OnRelease()
        {
            this.Owner.BuffManager.RemoveModifierHandler<BuffTargetDamageModifier, IDamageModifyHandler>(this);
            this._check_camp = Type_BattleRelationship.None;
        }


        public int GetDamageRate(DamageChangeMessage msg)
        {
            switch (this.change_type) {
                case 1:
                    {
                        int hp_rate = (int)((1 - this.Owner.CurrentHpRate) * 10000);
                        return hp_rate / hp_change_rate * this.Value * coe;
                    }
            }
            return 10000 + this.Value * coe;
        }
#if UNITY_EDITOR
        public override void OnGUI()
        {
            base.OnGUI();
            int rate = this.Value;
            if (this.change_type == 1) {
                int hp_rate = (int)((1 - this.Owner.CurrentHpRate) * 10000);
                rate = hp_rate / hp_change_rate * this.Value;
            }
            UnityEditor.EditorGUILayout.LabelField("rate", rate.ToString());
        }
#endif 

    }
}