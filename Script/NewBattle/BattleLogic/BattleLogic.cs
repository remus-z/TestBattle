using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{

    public interface IBattleManager
    {
        void SetHandler(IManagerHandler handler);
        void Init();
        void Start();

        void Release();
        T GetManager<T>() where T : IBattleManager;
    }

    public interface IManagerHandler
    {
        T GetManager<T>() where T : IBattleManager;
    }
    public class BattleLogic : IManagerHandler
    {
        //private static BattleLogic _instance = null;
        //public static BattleLogic Instance
        //{
        //    get
        //    {
        //        if (_instance == null)
        //        {
        //            _instance = new BattleLogic();
        //        }
        //        return _instance;
        //    }
        //}

        public readonly int BattleID;
        public readonly BattleData BattleData;
        public readonly int BattleType;
        public BattleLogic(int battle_id, BattleData data) {
            this.BattleID = battle_id;
            this.BattleData = data;
            this._CreateManagers();
        }
        public bool IsInBattle { get; private set; } = false;

        private void _CreateManagers()
        {
            this._managers = new Dictionary<object, IBattleManager>();
            //unit manager
            BattleUnitManager unit_manager = new BattleUnitManager();
            this._AddManager<BattleUnitManager>(unit_manager);
            //event manager
            BattleEventManager event_manager = new BattleEventManager();
            this._AddManager<BattleEventManager>(event_manager);
            //calculator
            BattleCalculator calculator = new BattleCalculator();
            this._AddManager<BattleCalculator>(calculator);

            //view manager,only for client
            BattleUnitViewManager view_manager = new BattleUnitViewManager();
            this._AddManager<BattleUnitViewManager>(view_manager);

            BattleFlowManager flow_manager = new BattleFlowManager();
            this._AddManager<BattleFlowManager>(flow_manager);

            BattleStatisticManager statistic_manager = new BattleStatisticManager();
            this._AddManager<BattleStatisticManager>(statistic_manager);

            this._InitManagers();
        }
        private void _InitManagers()
        {
            foreach (var kvp in this._managers)
            {
                kvp.Value.Init();
            }
        }

        public void Start() {
            foreach (var kvp in this._managers)
            {
                kvp.Value.Start();
            }
        }

        public void FinishBattle()
        {
            this._Release();
            this.IsInBattle = false;
        }

        //public bool IsBattleFinish() {

        //}

        private void _Release()
        {
            foreach (var kvp in this._managers)
            {
                kvp.Value.Release();
            }
            this._managers.Clear();
        }

        private Dictionary<object, IBattleManager> _managers = new Dictionary<object, IBattleManager>();

        private void _AddManager<T>(IBattleManager manager) where T : IBattleManager
        {
            manager.SetHandler(this);
            this._managers.Add(typeof(T), manager);
        }

        public T GetManager<T>() where T : IBattleManager
        {
            IBattleManager manager = null;
            if (!this._managers.TryGetValue(typeof(T), out manager))
            {
                BattleLog.LogError("no such manager:" + typeof(T));
            }
            return (T)manager;
        }

    }
}