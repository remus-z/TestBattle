using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public interface IBuffSuckBloodHandler {
        int GetSuckBloodRate();
    }
    public class BuffSuckBloodCheckModifier : BaseBuffModifier<IBuffSuckBloodHandler>
    {
        public BuffSuckBloodCheckModifier(BattleUnit owner) : base(owner)
        {
        }

        public bool CheckSuckBlood(int damage,Type_HitType hit_type, ref List<HitItem> caster_hits) {
            bool can_suck = this._handlers.Count > 0;
            if (can_suck && !Owner.IsDead) {
                for(int i = 0; i < this._handlers.Count; i++)
                {
                    int suck_rate = this._handlers[i].GetSuckBloodRate();
                    int suck_value = (int)(damage * GameUtil.ToRate(suck_rate));
                    this.Owner.AddHp(this.Owner, suck_value, Type_Damage.SuckBlood);
                    HitItem suck_blood_item = BattleClassCache.Instance.GetInstance<HitItem>();
                    suck_blood_item.AttackType = hit_type;
                    suck_blood_item.DamageType = Type_Damage.SuckBlood;
                    suck_blood_item.Critical = false;
                    suck_blood_item.DefendValue = 0;
                    suck_blood_item.Value = suck_value;
                    caster_hits.Add(suck_blood_item);
                }
               
                return true;
            }
            return false;
        }
    }
}
