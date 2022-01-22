using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public interface IBuffReflectHandler {
        int GetReflectRate();
    }
    public class BuffReflectModifier : BaseBuffModifier<IBuffReflectHandler>
    {
        public BuffReflectModifier(BattleUnit owner) : base(owner)
        {
            
        }

        public bool CheckReflect(BattleUnit attacker,float damage, Type_HitType hit_type, ref List<HitItem> reflect_items) {
            bool can_reflect = this._handlers.Count > 0;
            int reflect = 0;
            if (can_reflect)
            {
                for (int i = 0; i < this._handlers.Count; i++)
                {
                    int rate = this._handlers[i].GetReflectRate();
                    reflect = (int)(damage * (1 + rate));
                    if (reflect > 0)
                    {
                        attacker.AddHp(this.Owner, -reflect);
                    }
                    HitItem reflect_item = BattleClassCache.Instance.GetInstance<HitItem>();
                    reflect_item.AttackType = hit_type;
                    reflect_item.DamageType = Type_Damage.Refection;
                    reflect_item.Critical = false;
                    reflect_item.DefendValue = 0;
                    reflect_item.Value = reflect;
                    reflect_items.Add(reflect_item);
                }
            }
            return can_reflect;
        }
    }
}
