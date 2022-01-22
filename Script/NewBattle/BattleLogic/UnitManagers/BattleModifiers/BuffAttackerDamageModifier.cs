using System.Collections;
using System.Collections.Generic;
using System;

namespace TestBattle
{
    public class DamageChangeMessage : BattleCacheClass
    {
        public BattleUnit Attacker;
        public BattleUnit Target;
        public Type_Damage DamageType;
        public int TotalTargetCount = 0;
        public void Init(BattleUnit attacker, BattleUnit target, Type_Damage damage_type,int total_target) {
            this.Attacker = attacker;
            this.Target = target;
            this.DamageType = damage_type;
            this.TotalTargetCount = total_target;
        }
        public override void Reset()
        {
            this.Attacker = null;
            this.Target = null;
            this.DamageType = Type_Damage.None;
            this.TotalTargetCount = 0;
        }
    }
    public interface IDamageModifyHandler {
        int GetDamageRate(DamageChangeMessage msg);
    }
    public class BuffAttackerDamageModifier : BaseBuffModifier<IDamageModifyHandler>
    {
        public BuffAttackerDamageModifier(BattleUnit owner) : base(owner)
        {
        }
        public float ModifyDamage(float origin, DamageChangeMessage msg) {
            float rate = 0;
            for (int i = 0; i < this._handlers.Count; i++) {
                rate = GameUtil.ToRate(this._handlers[i].GetDamageRate(msg));
                origin *= rate;
            }
            //if (rate < -1)
            //    rate = -1;
            msg.Release();
            return origin/* * (1+rate)*/;
        }

    }
    public class BuffTargetDamageModifier : BaseBuffModifier<IDamageModifyHandler>
    {
        public BuffTargetDamageModifier(BattleUnit owner) : base(owner)
        {
        }
        public float ModifyDamage(float origin, DamageChangeMessage msg)
        {
            float rate = 0;
            for (int i = 0; i < this._handlers.Count; i++)
            {
                rate = GameUtil.ToRate(this._handlers[i].GetDamageRate(msg));
                origin *= (rate);
            }
            //if (rate < -1)
            //    rate = -1;
            msg.Release();
            return origin/* * (1+rate)*/;
        }

    }
    
}
