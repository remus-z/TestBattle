using System.Collections;
using System.Collections.Generic;
using System;
namespace TestBattle
{

    public abstract class BattleCacheClass
    {
        public BattleCacheClass() {
            this._reference = 0;
        }
        private int _reference = 0;
        public abstract void Reset();
        public void Retain() {
            this._reference++;
        }
        public void Release()
        {
            this._OnRelease();
            this._reference--;
            if (this._reference == 0)
            {
                BattleClassCache.Instance.Return(this);
            }
        }

        protected virtual void _OnRelease() {
        }
        public static void DestroyInstance(BattleCacheClass instance)
        {
            instance.Release();
            instance = null;
        }
    }

    public class BattleClassCache
    {
        private static BattleClassCache _instance;
        public static BattleClassCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BattleClassCache();
                }
                return _instance;
            }
        }

        private Dictionary<Type, Queue<BattleCacheClass>> _cache = new Dictionary<Type, Queue<BattleCacheClass>>();

        public T GetInstance<T>() where T : BattleCacheClass, new()
        {
            Queue<BattleCacheClass> queue = null;
            BattleCacheClass data;
            if (!this._cache.TryGetValue(typeof(T), out queue))
            {
                queue = new Queue<BattleCacheClass>();
            }
            if (queue.Count == 0)
            {
                data = new T();
            }
            else
            {
                data = queue.Dequeue();
            }
            data.Retain();
            return data as T;
        }

        public void Return<T>(T data) where T : BattleCacheClass
        {
            Queue<BattleCacheClass> queue = null;
            if (!this._cache.TryGetValue(data.GetType(), out queue))
            {
                queue = new Queue<BattleCacheClass>();
            }
            data.Reset();
            queue.Enqueue(data);
        }

        public void Clear() {
            foreach (var q in this._cache) {
                q.Value.Clear();
            }
            //this._cache.Clear();
        }
    }
}
