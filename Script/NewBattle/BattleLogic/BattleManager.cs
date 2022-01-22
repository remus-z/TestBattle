using System.Collections;
using System.Collections.Generic;
namespace TestBattle
{
    public class BattleManager
    {
        private static BattleManager _instance = null;
        public static BattleManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BattleManager();
                }
                return _instance;
            }
        }

        private int battle_id = 1;

        private Dictionary<int, BattleLogic> _battles = new Dictionary<int, BattleLogic>();
        public BattleLogic CreateBattle(BattleData data) {
            BattleLogic battle = new BattleLogic(battle_id++, data);
            this._battles.Add(battle.BattleID, battle);
            return battle;
        }

        public BattleLogic GetBattle(int id) {
            BattleLogic battle = null;
            this._battles.TryGetValue(id, out battle);
            return battle;
        }

        public void RemoveBattle(int id)
        {
            BattleLogic battle = null;
            if (this._battles.TryGetValue(id, out battle)) {
                battle.FinishBattle();
            }
        }
        //for test
        public BattleLogic Default { get; set; }

        public static BattleLogic DefaultBattle => _instance.Default;
    }
}
