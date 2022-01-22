using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TestBattle
{
    public class SkillData : BattleCacheClass
    {
        public int CasterID = 0;
        public int SkillIndex = 0;
        public List<HitData> HitDatas = new List<HitData>();//hits in skill
        public HitData Recovery;

        public List<BuffInfo> PreBuffInfos = new List<BuffInfo>();
        public List<BuffInfo> PostBuffInfos = new List<BuffInfo>();

        private Dictionary<int, int> _target_hit_count = new Dictionary<int, int>();
        private List<int> _hit_targets = new List<int>();

        private List<int> _critical_targets = new List<int>();
        public List<int> CriticalTargets => this._critical_targets;

        public void AddHitData(HitData hit_data)
        {
            HitDatas.Add( hit_data);
            Dictionary<int, HitInfo> hit_info = hit_data.HitInfos;
            foreach (KeyValuePair<int, HitInfo> kvp in hit_info) {
                if (this._target_hit_count.ContainsKey(kvp.Key))
                {
                    this._target_hit_count[kvp.Key]++;
                }
                else {
                    this._target_hit_count[kvp.Key] = 1;
                    this._hit_targets.Add(kvp.Key);
                }
            }
        }

        public HitData GetHitData(int index)
        {
            if(index >= 0 && index < this.HitDatas.Count)
                return this.HitDatas[index];
            Debug.LogError("out of range");
            return null;
        }

        public int HitCount { get { return HitDatas.Count; } }

        public int GetTargetHitCount(int target_id) {
            if (this._target_hit_count.ContainsKey(target_id))
            {
                return this._target_hit_count[target_id];
            }
            return 0;
        }
        public List<int> GetTargetsIDs() {
            return this._hit_targets;
        }

        public void AddCriticalTarget(int uid) {
            if (!this._critical_targets.Contains(uid))
                this._critical_targets.Add(uid);
        }
        public override void Reset()
        {
            this._critical_targets.Clear();
            for (int i = 0; i < this.HitDatas.Count; i++) {
                this.HitDatas[i].Release();
            }
            this.HitDatas.Clear();
            this.Recovery.Release();
            this.Recovery = null;
            this._target_hit_count.Clear();
            this._hit_targets.Clear();

            for (int i = 0; i < this.PreBuffInfos.Count; i++) {
                this.PreBuffInfos[i].Release();
            }
            for (int i = 0; i < this.PostBuffInfos.Count; i++)
            {
                this.PostBuffInfos[i].Release();
            }
            this.PreBuffInfos.Clear();
            this.PostBuffInfos.Clear();
        }
    }

    public class HitData : BattleCacheClass
    {
        public Dictionary<int,HitInfo> HitInfos = new Dictionary<int, HitInfo>();//key:target id 

        public void AddTargetHitInfo(int target_id , HitInfo hit_info) {
            this.HitInfos[target_id] = hit_info;
        }

        public HitInfo GetTargetHitInfo(int target_id) {
            HitInfo info = null;
            this.HitInfos.TryGetValue(target_id, out info);
            return info;
        }

        public override void Reset()
        {
            foreach (var hit_info in HitInfos)
            {
                hit_info.Value.Release();
            }
            this.HitInfos.Clear();
        }
    }

    public class HitInfo : BattleCacheClass
    {
        public List<HitItem> TargetHit = new List<HitItem>();//for target
        public List<HitItem> CasterHit = new List<HitItem>();//for caster:reflect/suck blood

        public void AddTargetHitItem(HitItem item) {
            this.TargetHit.Add(item);
        }

        public override void Reset()
        {
            for (int i = 0; i < this.TargetHit.Count; i++)
            {
                this.TargetHit[i].Release();
            }
            this.TargetHit.Clear();
            for (int i = 0; i < this.CasterHit.Count; i++)
            {
                this.CasterHit[i].Release();
            }
            this.CasterHit.Clear();
        }
    }

    public class HitItem : BattleCacheClass
    {
        public Type_HitType AttackType;
        public Type_Damage DamageType;
        public int Value;
        public int DefendValue;
        public bool Critical;
        public bool Invincible = false;

        public override void Reset()
        {
            this.DamageType = Type_Damage.None;
            this.AttackType = Type_HitType.None;
            this.Value = 0;
            this.Critical = false;
            this.Invincible = false;
        }
    }

    public class BuffInfo : BattleCacheClass
    {
        public int TargetID;
        public long BuffUID;
        public Type_BuffState BuffState;
        public override void Reset()
        {
            this.BuffUID = 0;
            this.BuffState = Type_BuffState.None;
        }
    }
}
