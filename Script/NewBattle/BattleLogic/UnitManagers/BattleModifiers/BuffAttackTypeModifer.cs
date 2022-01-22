using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public interface IBuffAttackTypeHandler {
        Type_Attack GetAttackType(Type_Attack origin_attack);
    }
    public class BuffAttackTypeModifer : BaseBuffModifier<IBuffAttackTypeHandler>
    {
        public BuffAttackTypeModifer(BattleUnit owner) : base(owner)
        {
            
        }
        public Type_Attack ChangeAttackType(Type_Attack origin_attack) {
            if (this.Owner.HasCondition(Type_Condition.true_damage))
                return origin_attack;
            if (this._handlers.Count > 0) {
                return Type_Attack.Magical;
            }
            return origin_attack;
        }
    }
}
