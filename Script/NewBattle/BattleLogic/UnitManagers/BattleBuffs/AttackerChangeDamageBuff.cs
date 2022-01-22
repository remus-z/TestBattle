using System.Collections;
using System.Collections.Generic;
using System;

namespace TestBattle
{
    public class AttackerChangeDamageBuff : BaseBattleBuff, IDamageModifyHandler
    {
        private int change_type = 0;
        private Type_BattleRelationship _check1camp = Type_BattleRelationship.None;

        private int hp2min = 0;

        private int hp4rate = 0;
        private int max4value = 0;

        public AttackerChangeDamageBuff(BattleLogic battle, BattleUnit target, BattleUnit caster, SkillBuffInfo buff_data) : base(battle,target, caster, buff_data)
        {
        }


        public override void ParseData(SkillBuffInfo data)
        {
            this.change_type = 0;
            if (!string.IsNullOrEmpty(data.CheckValue))
            {
                string[] p = data.CheckValue.Split('|');
                if (p.Length > 0 && int.TryParse(p[0], out this.change_type))
                {
                    if (change_type == 1)
                    {
                        int rel = 0;
                        if (p.Length != 2 || !int.TryParse(p[1], out rel))
                        {
                            this._BuffCheckValueError();
                        }
                        else
                        {
                            this._check1camp = (Type_BattleRelationship)rel;
                        }
                    }
                    else if (change_type == 2)
                    {
                        if (p.Length != 2 || !int.TryParse(p[1], out this.hp2min))
                        {
                            this._BuffCheckValueError();

                        }
                    }
                    else if (change_type == 4)
                    {
                        if (p.Length != 3 || !int.TryParse(p[1], out this.hp4rate) || !int.TryParse(p[2], out this.max4value))
                        {
                            this._BuffCheckValueError();
                        }
                    }
                }
                else {
                    this._BuffCheckValueError();
                }
            }
        }

        protected override void OnAdd()
        {
            this.Owner.BuffManager.AddModifierHandler<BuffAttackerDamageModifier, IDamageModifyHandler>(this);
        }

        protected override void OnRelease()
        {
            this.Owner.BuffManager.RemoveModifierHandler<BuffAttackerDamageModifier, IDamageModifyHandler>(this);
            this._check1camp = Type_BattleRelationship.None;
        }


        public int GetDamageRate(DamageChangeMessage msg)
        {
            switch (this.change_type) {
                case 1:
                    {
                        int count = this._battle.GetManager<BattleUnitManager>().GetRelationTeam(msg.Attacker.Camp, this._check1camp).SurviveUnitCount();
                        return this.Value * count;
                    }
                case 2:
                    {
                        if (msg.Target.CurrentHpRate > GameUtil.ToRate(hp2min)) {
                            return this.Value;
                        }
                        return 0;
                    }
                case 3:
                    return msg.TotalTargetCount * this.Value;
                case 4:
                    {
                        int hp_rate = (int)((1 - this.Owner.CurrentHpRate) * 10000);
                        return Math.Min( hp_rate / hp4rate * this.Value,max4value);
                    }
            }
            return 10000 + this.Value;
        }
#if UNITY_EDITOR
        public override void OnGUI()
        {
            base.OnGUI();
            int rate = this.Value;
            if (this.change_type == 1) {
                UnityEditor.EditorGUILayout.LabelField(string.Format("{0} count *", _check1camp), this.Value.ToString());
            } else if (this.change_type == 2) {
                UnityEditor.EditorGUILayout.LabelField(string.Format("target hp rate>{0}", hp2min), this.Value.ToString());
            }
            else if (this.change_type == 3)
            {
                UnityEditor.EditorGUILayout.LabelField(string.Format("target count "), this.Value.ToString());
            }
            else if (this.change_type == 4)
            {
                int hp_rate = (int)((1 - this.Owner.CurrentHpRate) * 10000);
                rate = Math.Min(hp_rate / hp4rate * this.Value, max4value);
                UnityEditor.EditorGUILayout.LabelField("hp rate:"+ hp_rate, rate.ToString());
            }
        }
#endif 


    }
}