using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public abstract class BaseBattleBuff : BattleCacheClass
    {
        public long BuffUID { get; set; }
        public int BuffID { get { return this.BuffData.BuffID; } }
        public Type_Condition BuffType { get { return (Type_Condition)this.BuffData.BuffType; } }
        public Type_ConditionKind BuffKind { get { return (Type_ConditionKind)this.BuffData.BuffKind; } }
        public int Turn;
        public int MaxTurn;
        public int Value;
        public int CasterID => this.Caster != null ? this.Caster.UnitID : -1;
        public int OwnerID =>  this.Owner.UnitID;

        public BattleUnit Caster;
        public BattleUnit Owner;

        public SkillBuffInfo BuffData { get; private set; }
        public virtual bool Finished { get { return this.MaxTurn != -1 && this.Turn <= 0; } }
        protected BattleLogic _battle;
        public BaseBattleBuff(BattleLogic battle, BattleUnit target, BattleUnit caster, SkillBuffInfo buff_data) {
            this._battle = battle;
            this.Owner = target;
            this.Caster = caster;
            this.BuffData = buff_data;
            this.Value = buff_data.Value;
            this.Turn = this.MaxTurn = buff_data.Turn;
            this.ParseData(buff_data);
        }
        public void Add() { this.OnAdd(); }
        public void Remove() { this.OnRemove(); }
        protected virtual void OnAdd() { }
        protected virtual void OnRemove() { }
        public abstract void ParseData(SkillBuffInfo data);

        public virtual void Overlap(BaseBattleBuff buff) { 

        }

        public virtual void ProcessTurn() {
            this.Turn--;
        }

        public override void Reset() {
            this.OnRelease();
        }

        protected virtual void OnRelease() { }

        protected void _BuffCheckValueError() {
            BattleLog.LogError(string.Format("buff id {0} has wrong check_value:{1}", this.BuffID, this.BuffData.CheckValue));
        }
#if UNITY_EDITOR
        public virtual void OnGUI() {
        }
#endif 
    }
}