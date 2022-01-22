using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public class BloodShieldBuff : ShieldBuff
    {

        private int _decilne_hp_rate;
        public BloodShieldBuff(BattleLogic battle, BattleUnit target, BattleUnit caster, SkillBuffInfo buff_data) : base(battle, target, caster, buff_data)
        {

        }
        public override void ParseData(SkillBuffInfo data)
        {
            this._decilne_hp_rate  = int.Parse(data.CheckValue);
        }

        protected override void OnAdd()
        {
            base.OnAdd();
            int decline_hp = (int)(this.Owner.CurrentHp * GameUtil.ToRate(_decilne_hp_rate));
            this._shield_max_hp = this._shield_hp = (int)(decline_hp * GameUtil.ToRate(this.Value));
            this.Owner.AddHp(this.Owner, -decline_hp);
        }


        public void OnSelfAdd()
        {
            
        }
#if UNITY_EDITOR
        public override void OnGUI()
        {
            UnityEditor.EditorGUILayout.LabelField("ShieldHP", string.Format("{0}/{1}", this._shield_hp, this._shield_max_hp));
            UnityEditor.EditorGUILayout.LabelField("HpRate", this._decilne_hp_rate.ToString());
        }
#endif
    }

}