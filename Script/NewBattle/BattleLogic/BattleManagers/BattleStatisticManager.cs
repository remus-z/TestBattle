using System.Collections;
using System.Collections.Generic;
using System;

namespace TestBattle
{
    public class StatisticData{
        public int Total = 0;
        public Dictionary<int, int> Values = new Dictionary<int, int>();
        public Dictionary<int, Dictionary<int, int>> RoundTotalValue = new Dictionary<int, Dictionary<int, int>>();//key: phase*100000 + round
        public void AddItem(int uid,int value,int round_index) {
            Total += value;

            Dictionary<int, int> values = null;
            if (!RoundTotalValue.TryGetValue(round_index, out values)) {
                values = new Dictionary<int, int>();
                RoundTotalValue.Add(round_index, values);
            }
            if (values.ContainsKey(uid))
            {
                values[uid] += value;
            }
            else
            {
                values.Add(uid, value);
            }

        }
    }

    public class CardStatisticData 
    {
        public int SkillID;
        public int SkillLevel;
        public int SkillType;
        public int CardRank;

        public CardStatisticData(int skill_id, int skill_level, int skill_type, int rank)
        {
            this.SkillID = skill_id;
            this.SkillLevel = skill_level;
            this.SkillType = skill_type;
            this.CardRank = rank;
        }
    }
    public class UnitStatisticData {
        private int _unit_uid = 0;
        private int _camp;
        private int _table_id = 0;
        private int _slot_id = 0;
        private int _unit_type = 0;

        public StatisticData CauseDamages = new StatisticData();
        public StatisticData CauseRecoverys = new StatisticData();
        public StatisticData BearDamages = new StatisticData();
        public StatisticData BearRecoverys = new StatisticData();

        public Dictionary<int, List<int>> Killed = new Dictionary<int, List<int>>();//key :round_index, value:killed unit list
        public int KilledBy = 0;

        public StatisticData CauseShields = new StatisticData();
        public StatisticData BearShields = new StatisticData();

        public Dictionary<int,List<CardStatisticData>> DrewCards = new Dictionary<int, List<CardStatisticData>>();
        public Dictionary<int, List<CardStatisticData>> UsedCards = new Dictionary<int, List<CardStatisticData>>();
        public Dictionary<int, List<CardStatisticData>> RemainCards = new Dictionary<int, List<CardStatisticData>>();


        public UnitStatisticData(int uid,int camp,int tid,int slot,int unit_type) {
            this._unit_uid = uid;
            this._camp = camp;
            this._table_id = tid;
            this._slot_id = slot;
            this._unit_type = unit_type;
        }

        public void CauseDamage(int target_id, Type_Damage dt,int value,int round_index) {
            if (value < 0) {
                CauseDamages.AddItem(target_id, -value, round_index);
            }
            if (value > 0) {
                CauseRecoverys.AddItem(target_id, value, round_index);
            }
        }

        public void BearDamage(int source_id, Type_Damage dt, int value, int round_index)
        {
            if (value < 0)
            {
                BearDamages.AddItem(source_id, -value, round_index);
            }
            if (value > 0)
            {
                BearRecoverys.AddItem(source_id, value, round_index);
            }
        }

        public void AddKill(int id,int round_index) {
            List<int> kills = null;
            if (!this.Killed.TryGetValue(round_index, out kills)) {
                kills = new List<int>();
                this.Killed.Add(round_index , kills);
            }
            kills.Add(id);
        }
        public void CauseShield(int target_id, int value, int round_index)
        {
            if (value > 0)
            {
                CauseShields.AddItem(target_id, value,round_index);
            }
        }
        public void BearShield(int source_id, int value, int round_index)
        {
            if (value > 0)
            {
                BearShields.AddItem(source_id, value, round_index);
            }
        }
        public void AddDrewCard(int round_index, BattleUnitCardEventData card_data) {
            List<CardStatisticData> datas = null;
            if (!this.DrewCards.TryGetValue(round_index, out datas))
            {
                datas = new List<CardStatisticData>();
                this.DrewCards.Add(round_index, datas);
            }
            datas.Add(new CardStatisticData(card_data.SkillID,card_data.SkillLevel,card_data.SkillType,card_data.CardRank ));
        }

        public void AddUsedCard(int round_index, BattleUnitUseCardEventData card_data)
        {
            List<CardStatisticData> datas = null;
            if (!this.UsedCards.TryGetValue(round_index, out datas))
            {
                datas = new List<CardStatisticData>();
                this.UsedCards.Add(round_index, datas);
            }
            datas.Add(new CardStatisticData(card_data.SkillID, card_data.SkillLevel, card_data.SkillType, card_data.CardRank));
        }
    }

    public class BattleStatisticManager : BattleBaseManager
    {
        private Dictionary<int, UnitStatisticData> _statistic_datas = new Dictionary<int, UnitStatisticData>();
        public override void OnInit()
        {
            this.GetManager<BattleEventManager>().AddListener(BattleEvent.BattleUnitCreate, this._OnBattleUnitCreate);
            this.GetManager<BattleEventManager>().AddListener(BattleEvent.BattleUnitDamage, this._OnBattleUnitDamage);
            this.GetManager<BattleEventManager>().AddListener(BattleEvent.BattleUnitDead, this._OnBattleUnitDead); 
            this.GetManager<BattleEventManager>().AddListener(BattleEvent.BattleUnitRemove, this._OnBattleUnitRemove);
            this.GetManager<BattleEventManager>().AddListener(BattleEvent.BattleUnitShieldChanged, this._OnBattleUnitShieldChange);
            this.GetManager<BattleEventManager>().AddListener(BattleEvent.BattleUnitDrewCards, this._OnBattleUnitDrewCards);
            this.GetManager<BattleEventManager>().AddListener(BattleEvent.BattleUnitUseCards, this._OnBattleUnitUseCards);

            //this.GetManager<BattleEventManager>().AddListener(BattleEvent.BattleRoundEnd, this._OnBattleRoundEnd);
        }

        public override void OnRelease()
        {
            this.GetManager<BattleEventManager>().RemoveListener(BattleEvent.BattleUnitCreate, this._OnBattleUnitCreate);
            this.GetManager<BattleEventManager>().RemoveListener(BattleEvent.BattleUnitDamage, this._OnBattleUnitDamage);
            this.GetManager<BattleEventManager>().RemoveListener(BattleEvent.BattleUnitDead, this._OnBattleUnitDead);
            this.GetManager<BattleEventManager>().RemoveListener(BattleEvent.BattleUnitRemove, this._OnBattleUnitRemove);
            this.GetManager<BattleEventManager>().RemoveListener(BattleEvent.BattleUnitShieldChanged, this._OnBattleUnitShieldChange);
            this.GetManager<BattleEventManager>().AddListener(BattleEvent.BattleUnitDrewCards, this._OnBattleUnitDrewCards);
            this.GetManager<BattleEventManager>().AddListener(BattleEvent.BattleUnitUseCards, this._OnBattleUnitUseCards);
            //this.GetManager<BattleEventManager>().RemoveListener(BattleEvent.BattleRoundEnd, this._OnBattleRoundEnd);
        }

        public UnitStatisticData GetStatisticData(int uid) {
            UnitStatisticData data = null;
            this._statistic_datas.TryGetValue(uid, out data);
            return data;
        }

        protected void _OnBattleUnitCreate(object sender, object data) {
            BattleUnit unit = (BattleUnit)data;
            UnitStatisticData sd = this.GetStatisticData(unit.UnitID);
            if (sd == null) {
                sd = new UnitStatisticData(unit.UnitID,(int)unit.Camp,unit.TableID,unit.SlotID,(int)unit.UnitType);
                this._statistic_datas.Add(unit.UnitID, sd);
            }
        }

        protected void _OnBattleUnitDamage(object sender, object data)
        {
            BattleUnitDamageEventData damage_data = (BattleUnitDamageEventData)data;
            int source_id = damage_data.Source != null ? damage_data.Source.UnitID : 0; ;
            int target_id = damage_data.Target.UnitID;
            UnitStatisticData source_sd = this.GetStatisticData(source_id);
            if (source_sd != null)
            {
                source_sd.CauseDamage(target_id, damage_data.Type_Damage, damage_data.Value, this._GetRoundIndex());
            }
            else
            {
                //BattleLog.LogError(string.Format("damage source {0} not in statistic", damage_data.Source.UnitID));
            }
            
            UnitStatisticData target_sd = this.GetStatisticData(target_id);
            if (target_sd != null)
            {
                target_sd.BearDamage(source_id, damage_data.Type_Damage, damage_data.Value, this._GetRoundIndex());
            }
            else
            {
                BattleLog.LogError(string.Format("damage target {0} not in statistic", damage_data.Target.UnitID));
            }
        }

        protected void _OnBattleUnitDead(object sender, object data) {
            BattleUnitDeadEventData dead_data = (BattleUnitDeadEventData)data;
            int source_id = dead_data.Source != null ? dead_data.Source.UnitID : 0; ;
            int target_id = dead_data.Target.UnitID;
            UnitStatisticData source_sd = this.GetStatisticData(source_id);
            UnitStatisticData target_sd = this.GetStatisticData(target_id);
            target_sd.KilledBy = source_id;
            if (source_sd != null) {
                source_sd.AddKill(target_id,this._GetRoundIndex());
            }
        }

        protected void _OnBattleUnitShieldChange(object sender, object data)
        {
            BattleUnitShieldEventData msg = (BattleUnitShieldEventData)data;
            int source_id = msg.Source != null ? msg.Source.UnitID : 0; ;
            int target_id = msg.Target.UnitID;
            UnitStatisticData source_sd = this.GetStatisticData(source_id);
            UnitStatisticData target_sd = this.GetStatisticData(target_id);
            if (source_sd != null)
            {
                source_sd.CauseShield(target_id,msg.ShieldValue,this._GetRoundIndex());
            }
            target_sd.BearShield(source_id, msg.ShieldValue, this._GetRoundIndex());
        }


        protected void _OnBattleRoundEnd(object sender, object data) {
            BattleRoundEndEventData msg = (BattleRoundEndEventData)data;

        }

        protected void _OnBattleUnitRemove(object sender, object data) {
            BattleUnit unit = (BattleUnit)data;
            UnitStatisticData sd = this.GetStatisticData(unit.UnitID);
            if (this._statistic_datas.ContainsKey(unit.UnitID)) {
                this._statistic_datas.Remove(unit.UnitID);
            }
        }

        protected void _OnBattleUnitDrewCards(object sender, object data)
        {
            BattleUnitCardEventData msg = (BattleUnitCardEventData)data;
            UnitStatisticData sd = this.GetStatisticData(msg.UnitID);
            sd.AddDrewCard(this._GetRoundIndex(), msg);
        }

        protected void _OnBattleUnitUseCards(object sender, object data)
        {
            BattleUnitUseCardEventData msg = (BattleUnitUseCardEventData)data;
            UnitStatisticData sd = this.GetStatisticData(msg.UnitID);
            sd.AddUsedCard(this._GetRoundIndex(), msg);
        }


        public static int RoundIndex(int phase, int round)
        {
            return phase * 100000 + round + 1;
        }

        private int _GetRoundIndex() {
            int phase = this.Battle.GetManager<BattleFlowManager>().Phase;
            int round = this.Battle.GetManager<BattleFlowManager>().Round;
            return RoundIndex(phase, round);
        }
    }
}