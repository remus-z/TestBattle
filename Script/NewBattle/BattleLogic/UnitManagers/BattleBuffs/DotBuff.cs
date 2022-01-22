using System.Collections;
using System.Collections.Generic;


namespace TestBattle
{
    public class DotBuff : BaseBattleBuff,IBuffDotHandler
    {
        public DotBuff(BattleLogic battle,BattleUnit target, BattleUnit caster, SkillBuffInfo buff_data) : base(battle,target, caster, buff_data)
        {
        }

        public override void ParseData(SkillBuffInfo data)
        {
            this.Value = (int)(GameUtil.ToRate(data.Value) * this.Caster.GetAttribute(Type_Attribution.Attack));
        }


        protected override void OnAdd()
        {
            this.Owner.BuffManager.AddModifierHandler<BuffDotModifier, IBuffDotHandler>(this);
        }

        protected override void OnRelease()
        {
            this.Owner.BuffManager.RemoveModifierHandler<BuffDotModifier, IBuffDotHandler>(this);
        }

        public bool IsDebuffDOT => this.BuffKind == Type_ConditionKind.Debuff;

        public int GetDotDamage(out int defend_damage)
        {
            defend_damage = 0;
            if ((Type_Condition)this.BuffData.BuffType == Type_Condition.recovery_dot) {
                return this.Value;
            }
            this._battle.GetManager<BattleCalculator>().CalculateGeneralDamage( this.Caster,this.Owner, Type_Damage.Dot, (int)this.Value,out defend_damage);
            return -this.Value;
        }
#if UNITY_EDITOR
        public override void OnGUI()
        {
            base.OnGUI();
            UnityEditor.EditorGUILayout.LabelField("DotValue", this.Value.ToString());
        }
#endif
    }
}
