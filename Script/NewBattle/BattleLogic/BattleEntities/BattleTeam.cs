using System.Collections;
using System.Collections.Generic;
using System;

namespace TestBattle
{
    public class BattleTeam
    {
        public Type_BattleCamp Camp;
        public int CurrentTurn = -1;
        protected Dictionary<int, BattleUnit> _units = new Dictionary<int, BattleUnit>();//all units

        //survive units
        protected List<BattleUnit> _unit_list = new List<BattleUnit>();
        protected Dictionary<int, List<BattleUnit>> _row_units = new Dictionary<int, List<BattleUnit>>();
        protected Dictionary<int, List<BattleUnit>> _column_units = new Dictionary<int, List<BattleUnit>>();
        protected BattleUnit[] _slot_units = new BattleUnit[9];

        protected List<BattleUnit> _dead_units = new List<BattleUnit>();//dead unit during battle,

        public PlayerTeamCardManager CardManager;

        public int CardToPlayMaxCount;
        public int CardToPlayCount;
        public bool ShouldFinishTurn => this.CardToPlayCount == 0 || (this.CardManager.DeckAvtiveCardsCount == 0 && this.CardManager.DeckAwakenCards.Count == 0);
        public void ChangePlayCount(int change) {
            CardToPlayCount += change;
            CardToPlayCount = Math.Min(CardToPlayCount, CardToPlayMaxCount);
            CardToPlayCount = Math.Max(CardToPlayCount, 0);
        }

        public readonly BattleLogic Battle;
        public BattleTeam(BattleLogic battle, Type_BattleCamp camp) {
            this.Battle = battle;
            this.Camp = camp;
        }
        public bool AddUnit(BattleUnit unit)
        {
            if (this._units.ContainsKey(unit.UnitID))
            {
                BattleLog.LogError(string.Format("can not add same unit {0} to same team {1}", unit.UnitLogInfo, this.Camp));
                return false;
            }
            this._units.Add(unit.UnitID, unit);
            this._unit_list.Add(unit);

            int row = (int)unit.RowType;
            List<BattleUnit> row_list = null;
            if (!this._row_units.TryGetValue(row, out row_list))
            {
                row_list = new List<BattleUnit>();
                this._row_units.Add(row, row_list);
            }
            row_list.Add(unit);

            int column = (int)unit.ColumnType;
            List<BattleUnit> column_list = null;
            if (!this._column_units.TryGetValue(column, out column_list))
            {
                column_list = new List<BattleUnit>();
                this._column_units.Add(column, column_list);
            }
            column_list.Add(unit);

            this._slot_units[unit.SlotID] = unit;
            this.Battle.GetManager<BattleEventManager>().SendMessage(BattleEvent.BattleUnitJoinBattle, this, unit);
            return true;
        }

        public void RemoveUnit(BattleUnit unit)
        {
            if (!this._units.ContainsKey(unit.UnitID))
                return;
            this._units.Remove(unit.UnitID);
            this._unit_list.Remove(unit);
            this._row_units[(int)unit.RowType].Remove(unit);
            this._column_units[(int)unit.ColumnType].Remove(unit);
            this._slot_units[unit.SlotID] = null;
            this.Battle.GetManager<BattleEventManager>().SendMessage(BattleEvent.BattleUnitJoinBattle, this, unit);

            unit.Release();
            

        }


        public BattleUnit GetUnit(int uid)
        {
            if (this._units.ContainsKey(uid))
            {
                return this._units[uid];
            }
            return null;
        }

        public bool HasUnit(int uid)
        {
            return this._units.ContainsKey(uid);
        }

        public List<BattleUnit> GetSurviveUnits()
        {
            return this._unit_list;
        }

        public BattleUnit GetSurviveUnit(int uid) {
            return this._unit_list.Find(u => u.UnitID == uid);
        }

        public int SurviveUnitCount() {
            return this._unit_list.Count;
        }

        public BattleUnit GetTeamLeader()
        {
            return this._unit_list.Find(e => e.IsLeader == true);
        }

        public BattleUnit SetLeader(BattleUnit leader) {
            if (leader == null)
                return null; ;
            BattleUnit pre = this.GetTeamLeader();
            if (pre != null) {
                pre.IsLeader = false;
            }
            leader.IsLeader = true;
            return leader;
        }

        public List<BattleUnit> GetRowUnits(TeamRowType row)
        {
            return this._row_units[(int)row];
        }

        public List<BattleUnit> GetColumnUnits(TeamColumnType column)
        {
            return this._column_units[(int)column];
        }

        public BattleUnit GetSlotUnit(int slot_id)
        {
            return this._slot_units[slot_id];
        }

        public void SetUnitDead(BattleUnit unit) {
            if (this._unit_list.Contains(unit)) {
                BattleLog.Log(string.Format("unit dead------ {0}", unit.UnitLogInfo));
                this._unit_list.Remove(unit);
                this._row_units[(int)unit.RowType].Remove(unit);
                this._column_units[(int)unit.ColumnType].Remove(unit);
                this._slot_units[unit.SlotID] = null;
                this._dead_units.Add(unit);
                this.CardManager.RemoveUnitCards(unit.UnitID);
            }
        }


        public void Init() {
            this.CardToPlayMaxCount = this.SurviveUnitCount(); 
            this.CardManager = new PlayerTeamCardManager(this);
            this.CardManager.InitCardTomb();
            this.CardManager.DrawCards();
            this.SetLeader(this._unit_list.Find(u => u.IsLeader));
        }

        public void StartTurn(int turn)
        {
            this.CurrentTurn = turn;
            for (int i = 0; i < this._unit_list.Count; i++)
            {
                this._unit_list[i].BeginTeamTurn();
            }
            for (int i = 0; i < this._unit_list.Count; i++)
            {
                this.Battle.GetManager<BattleFlowManager>().TriggerSkill(Type_SkillTriggerTime.TeamTurnBegin, this._unit_list[i]);
            }
            this.CardToPlayMaxCount = this.SurviveUnitCount();
            this.CardToPlayCount = this.CardToPlayMaxCount;
        }

        public void EndTurn(int turn)
        {
            for (int i = 0; i < this._unit_list.Count; i++)
            {
                this._unit_list[i].EndTeamTurn();
            }
            //this.UpdateTeamUnit();
           
            this.CardManager.DrawCards();
        }


        public void Release()
        {
            this.CardManager.Release();
            foreach (var kvp in this._units)
            {
                kvp.Value.Release();
            }
            this._units.Clear();
            this._unit_list.Clear();
        }

    }
}