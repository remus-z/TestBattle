using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public interface IBuffAdditionalDamageCheckHandler {
        bool IsAttackerBuff { get; }
        int GetAdditionalDamageRate();
    }
    public class BuffAdditionalDamageCheckModifier : BaseBuffModifier<IBuffAdditionalDamageCheckHandler>
    {
        public BuffAdditionalDamageCheckModifier(BattleUnit owner) : base(owner)
        {
        }

        public bool CheckAdditionalDamage(BattleUnit attacker,BattleUnit target, int damage,Type_HitType hit_type,ref List<HitItem> target_hits ) {
            bool has_additional_damage = this._handlers.Count > 0;
            for (int i = 0; i < this._handlers.Count; i++) {
                bool valid = this._handlers[i].IsAttackerBuff && attacker == this.Owner || !this._handlers[i].IsAttackerBuff;
                if (valid) {
                    int rate = this._handlers[i].GetAdditionalDamageRate();
                    int addition = (int)(damage * (1 + rate));

                    HitItem reflect_item = BattleClassCache.Instance.GetInstance<HitItem>();
                    reflect_item.AttackType = hit_type;
                    reflect_item.DamageType = Type_Damage.Addition;
                    reflect_item.Critical = false;
                    reflect_item.DefendValue = 0;
                    reflect_item.Value = addition;
                    target_hits.Add(reflect_item);
                    target.AddHp(attacker, -addition);
                }
            }
            return has_additional_damage;
        }
    }
}
