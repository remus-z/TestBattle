using System.Collections;
using System.Collections.Generic;
using System;

namespace TestBattle
{
    public class ShieldBuff : BaseBattleBuff,IShieldBuffHandler,IBuffAfterSkillCheckRemoveHandler
    {
        protected int _shield_max_hp;
        protected int _shield_hp;

        public bool AfterSkillRemovable => this._shield_hp <= 0;

        public ShieldBuff(BattleLogic battle, BattleUnit target, BattleUnit caster, SkillBuffInfo buff_data) : base(battle, target, caster, buff_data)
        {

        }

        public override void ParseData(SkillBuffInfo data)
        {
        }

        protected override void OnAdd() {
            this._shield_max_hp = this._shield_hp = (int) GameUtil.ToRate(this.Value) * this.Owner.GetAttribute(Type_Attribution.Attack);
            this.Owner.AddShield(this.Caster, this._shield_hp, this._shield_max_hp);
            this.Owner.BuffManager.AddModifierHandler<BuffAfterSkillCheckRemoveModifier, IBuffAfterSkillCheckRemoveHandler>(this);
            this.Owner.BuffManager.AddModifierHandler<BuffShieldModifier, IShieldBuffHandler>(this);
        }


        protected override void OnRelease()
        {
            this.Owner.AddShield(this.Owner, -this._shield_hp, -this._shield_max_hp);
            this.Owner.BuffManager.RemoveModifierHandler<BuffAfterSkillCheckRemoveModifier, IBuffAfterSkillCheckRemoveHandler>(this);
            this.Owner.BuffManager.RemoveModifierHandler<BuffShieldModifier, IShieldBuffHandler>(this);
        }

        public int DefendValue(BattleUnit attacker,int origin_damage)
        {
            int defend_value = 0;
            if (_shield_hp >= origin_damage) {
                defend_value = origin_damage;
                _shield_hp -= origin_damage;
            }
            else {
                defend_value = _shield_hp;
                _shield_hp = 0;
            }
            this.Owner.AddShield(attacker ,- defend_value, 0);
            return defend_value;
        }

#if UNITY_EDITOR
        public override void OnGUI()
        {
            UnityEditor.EditorGUILayout.LabelField("ShieldHP", string.Format("{0}/{1}", this._shield_hp,this._shield_max_hp) );
        }
#endif 
    }
}
