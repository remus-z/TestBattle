using System.Collections;
using System.Collections.Generic;
namespace TestBattle
{
    public abstract class BattleBaseManager : IBattleManager
    {
        protected IManagerHandler _manager_handler;
        public BattleLogic Battle => (BattleLogic)this._manager_handler;
        public void SetHandler(IManagerHandler handler)
        {
            this._manager_handler = handler;
        }

        public void Init()
        {
            this.OnInit();
        }

        public void Release()
        {
            this.OnRelease();
        }

        public abstract void OnInit();
        public abstract void OnRelease();

        public T GetManager<T>() where T : IBattleManager
        {
            return this._manager_handler.GetManager<T>();
        }

        public virtual void Start()
        {
        }
    }
}
