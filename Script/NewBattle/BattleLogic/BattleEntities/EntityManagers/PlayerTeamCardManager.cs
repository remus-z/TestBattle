using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public class CardData : BattleCacheClass
    {
        public int BattleUnitID;
        public IBattleUnitSkillData SkillData;
        public int Rank = 1;
        public int SkillID => this.SkillData.SkillID;
        public void Init(int unit_id, IBattleUnitSkillData skill_data,int rank) {
            this.BattleUnitID = unit_id;
            this.SkillData = skill_data;
            this.Rank = rank;
        }
        public override void Reset()
        {
            
        }
        public CardData Copy() {
            CardData new_data = BattleClassCache.Instance.GetInstance<CardData>();
            new_data.Init(this.BattleUnitID, this.SkillData, this.Rank);
            new_data.BattleUnitID = this.BattleUnitID;
            new_data.SkillData = this.SkillData;
            new_data.Rank = this.Rank;
            return new_data;
        }

    }

    public class PlayerTeamCardManager
    {
        private BattleTeam _team;
        public PlayerTeamCardManager(BattleTeam team) {
            this._team = team;
        }
        public void Release() {
        }

        protected List<CardData> _cards_in_tomb = new List<CardData>();
        protected Dictionary<int, List<CardData>> _deck_cards = new Dictionary<int, List<CardData>>();//active cards
        public Dictionary<int, List<CardData>> DeckActiveCards => this._deck_cards;
        public int DeckAvtiveCardsCount {
            get {
                int count = 0;
                foreach (var kvp in this._deck_cards) {
                    count += kvp.Value.Count;
                }
                return count;
            }
        }

        protected Dictionary<int, CardData> _tomb_awaken_cards = new Dictionary<int, CardData>();
        protected Dictionary<int, CardData> _deck_awaken_cards = new Dictionary<int, CardData>();
        public Dictionary<int, CardData> DeckAwakenCards => this._deck_awaken_cards;
        public int DeckAwakenCardsCount
        {
            get
            {
                return this._deck_awaken_cards.Count;
            }
        }
        public void InitCardTomb()
        {
            this._cards_in_tomb.Clear();
            this._tomb_awaken_cards.Clear();
            List<BattleUnit> survive_units = this._team.GetSurviveUnits();
            for (int i = 0; i < survive_units.Count; i++)
            {
                List<IBattleUnitSkillData> skills = survive_units[i].GetSkillDatas();
                for (int j = 0; j < skills.Count; j++)
                {
                    if (skills[j].SkillType == Type_Skill.Active) {
                        for (int k = 0; k < 3; k++)
                        {
                            CardData card = BattleClassCache.Instance.GetInstance<CardData>();
                            card.Init(survive_units[i].UnitID, skills[j], 1);
                            this._cards_in_tomb.Add(card);
                        }
                    } else if (skills[j].SkillType == Type_Skill.Awaken) {
                        CardData card = BattleClassCache.Instance.GetInstance<CardData>();
                        card.Init(survive_units[i].UnitID, skills[j], 1);
                        this._tomb_awaken_cards.Add(survive_units[i].UnitID, card);
                    }
                }
            }
            //shuffle
            for (int i = 0; i < this._cards_in_tomb.Count; i++)
            {
                int last = this._cards_in_tomb.Count - 1 - i;
                int index = this._team.Battle.GetManager<BattleCalculator>().GetRandom(0, last);
                CardData tmp = this._cards_in_tomb[index];
                this._cards_in_tomb[index] = this._cards_in_tomb[last];
                this._cards_in_tomb[last] = tmp;
            }
            this.ClearDeckCards();
        }

        private List<CardData> _switch_list = new List<CardData>();
        private List<CardData> _need_cards_list = new List<CardData>();
        protected void _ShuffleTombWithNeed() {
            this._switch_list.Clear();
            this._need_cards_list.Clear();
            List<BattleUnit> units = this._team.GetSurviveUnits();
            for (int i = 0;i< units.Count; i++)
            {
                BattleUnit unit = units[i];
                List<CardData> deck_cards = null;
                this._deck_cards.TryGetValue(unit.UnitID, out deck_cards);
                for (int j = 0; j < unit.UnitData.SkillDatas.Count; j++)
                {
                    IBattleUnitSkillData skill = unit.UnitData.SkillDatas[j];
                    if (skill.SkillType == Type_Skill.Active) {
                        if (deck_cards == null || deck_cards.Find(c => c.SkillID == skill.SkillID) == null) {
                            int card_index = this._cards_in_tomb.FindIndex(c => c.SkillID == skill.SkillID);
                            if (card_index == -1) {
                                throw new System.Exception(string.Format("unit :{0},skill:{1} not in tomb,impossible!", unit.UnitID, skill.SkillID));
                            }
                            else {
                                this._switch_list.Add(this._cards_in_tomb[card_index]);
                                this._cards_in_tomb.RemoveAt(card_index);
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < this._cards_in_tomb.Count; i++) {
                this._switch_list.Add(this._cards_in_tomb[i]);
            }
            //switch tomb list
            List<CardData> tmp = this._cards_in_tomb;
            this._cards_in_tomb.Clear();
            this._cards_in_tomb = this._switch_list;
            this._switch_list = tmp;
            //this._cards_in_tomb.Reverse();
        }

        protected void _ShuffleTombRandom()
        {
            //shuffle
            for (int i = 0; i < this._cards_in_tomb.Count; i++)
            {
                int last = this._cards_in_tomb.Count - 1 - i;
                int index = this._team.Battle.GetManager<BattleCalculator>().GetRandom(0, last);
                CardData tmp = this._cards_in_tomb[index];
                this._cards_in_tomb[index] = this._cards_in_tomb[last];
                this._cards_in_tomb[last] = tmp;
            }
        }

        public void DrawCards() {
            //this._ShuffleTombRandom();
            this._ShuffleTombWithNeed();
            int deck_card_max_rank = this._team.SurviveUnitCount() * 3;
            int deck_current_cards_rank = this.GetDeckCardsRank();
            int card_count_to_draw = deck_card_max_rank - deck_current_cards_rank;
            BattleLog.Log(string.Format("team {3} begin draw card, tomb:{0}, deck_max:{1}, deck_curr :{2}", this._cards_in_tomb.Count, deck_card_max_rank, deck_current_cards_rank, this._team.Camp));
            int count = card_count_to_draw;
            while (card_count_to_draw > 0) {
                //int card_index = this._cards_in_tomb.Count - 1;
                CardData data = this._cards_in_tomb[count- card_count_to_draw];
                this._AddActiveDeckCard(data);
                //this._cards_in_tomb.RemoveAt(0);
                data.Release();
                card_count_to_draw--;
                
            }
            this._cards_in_tomb.RemoveRange(0, count);
            BattleLog.Log(string.Format("team {0} draw cards :tomb {1}/ deck {2}", this._team.Camp,this._cards_in_tomb.Count, this.GetDeckCardsRank()));
        }

        public void DrawAwakenCard(int unit_id) {
            CardData tomb_awake_card = this._tomb_awaken_cards[unit_id];
            this._tomb_awaken_cards.Remove(unit_id);
            CardData draw_card = tomb_awake_card.Copy();
            tomb_awake_card.Release();
            this._deck_awaken_cards.Add(unit_id, draw_card);
            this._team.Battle.GetManager<BattleEventManager>().SendMessage(BattleEvent.BattleUnitDrewCards, this, BattleUnitCardEventData.CreateEventData(draw_card.BattleUnitID, draw_card.SkillID, draw_card.SkillData.SkillLevel, (int)draw_card.SkillData.SkillType, draw_card.Rank));

        }

        public void RemoveUnitCards(int unit_id) {
            this.ReclaimUnitCard(unit_id);
            for (int i = this._cards_in_tomb.Count -1; i >=0 ; i--) {
                if (this._cards_in_tomb[i].BattleUnitID == unit_id) {
                    this._cards_in_tomb[i].Release();
                    this._cards_in_tomb.RemoveAt(i);
                }
            }
            this._tomb_awaken_cards[unit_id].Release();
            this._tomb_awaken_cards.Remove(unit_id);
        }

        public void ReclaimUnitCards() {
            List<BattleUnit> units = this._team.GetSurviveUnits();
            for (int i = 0; i < units.Count; i++) {
                this.ReclaimUnitCard(units[i].UnitID);
            }
            this.ClearDeckCards();
        }

        private List<CardData> _card_to_reclaim = new List<CardData>();
        public void ReclaimUnitCard(int unit_id)
        {
            this._card_to_reclaim.Clear();
            List<CardData> cards = this.GetDeckActiveCards(unit_id);
            if (cards != null && cards.Count > 0)
            {
                this._card_to_reclaim.AddRange(cards);
                for (int i = 0; i < _card_to_reclaim.Count; i++)
                {
                    this.ReclaimCard(_card_to_reclaim[i]);
                }
            }
            this._card_to_reclaim.Clear();
            CardData awake_card = null;
            this._deck_awaken_cards.TryGetValue(unit_id, out awake_card);
            this.ReclaimCard(awake_card);
        }


        public void ReclaimAwakenCard(int unit_id)
        {
            CardData deck_awake_card = null;
            this._deck_awaken_cards.TryGetValue(unit_id, out deck_awake_card);
            if (deck_awake_card != null) {
                this.ReclaimCard(deck_awake_card);
            }
        }


        public void ReclaimCard(CardData deck_card) {
            if (deck_card == null)
                return;
            if (deck_card.SkillData.SkillType == Type_Skill.Active)
            {
                for (int j = 0; j < deck_card.Rank; j++)
                {
                    CardData data = deck_card.Copy();
                    data.Rank = 1;
                    this._cards_in_tomb.Add(data);
                }
                List<CardData> cards = GetDeckActiveCards(deck_card.BattleUnitID);
                cards.Remove(deck_card);
            }
            else if(deck_card.SkillData.SkillType == Type_Skill.Awaken) {
                CardData data = deck_card.Copy();
                this._tomb_awaken_cards.Add(data.BattleUnitID, data);
                this._deck_awaken_cards.Remove(deck_card.BattleUnitID);
            }
            deck_card.Release();
        }

        public List<CardData> GetDeckActiveCards(int unit_id) {
            List<CardData> cards = null;
            if (!this._deck_cards.TryGetValue(unit_id, out cards)) {
                cards = new List<CardData>();
                this._deck_cards.Add(unit_id,cards);
            }
            return cards;
        }

        protected void _AddActiveDeckCard(CardData card) {
            List<CardData> cards = GetDeckActiveCards(card.BattleUnitID);
            CardData deck_card = cards.Find(c => c.SkillData.SkillID == card.SkillData.SkillID);
            if (deck_card == null)
            {
                cards.Add(card.Copy());
            }
            else
            {
                if (deck_card.Rank + 1 > 3)
                {
                    throw new System.Exception("cannot get here,more than 3 cards for one skill:" + deck_card.SkillData.SkillID);
                }
                else
                {
                    deck_card.Rank++;
                }
            }
            this._team.Battle.GetManager<BattleEventManager>().SendMessage(BattleEvent.BattleUnitDrewCards,this,BattleUnitCardEventData.CreateEventData(card.BattleUnitID,card.SkillID,card.SkillData.SkillLevel,(int)card.SkillData.SkillType,card.Rank));
        }
        //protected void _RemoveDeckCard(CardData card) {
        //    if (card.SkillData.SkillType == Type_Skill.Active)
        //    {
        //        List<CardData> cards = GetDeckActiveCards(card.BattleUnitID);
        //        cards.Remove(card);
        //    }
        //    else if(card.SkillData.SkillType == Type_Skill.Awaken)
        //    {
        //        this._deck_awaken_cards.Remove(card.BattleUnitID);
        //    }
        //    card.Release();
        //}
        public CardData GetDeckCard(int unit_id,int skill_id)
        {
            CardData card = null;
            List<CardData> cards = this.GetDeckActiveCards(unit_id);
            if (cards != null) {
                card = cards.Find(c => c.SkillID == skill_id);
            }
            if (card == null) {
                if (this._deck_awaken_cards.ContainsKey(unit_id) && this._deck_awaken_cards[unit_id].SkillID == skill_id) {
                    card = this._deck_awaken_cards[unit_id];
                }
            }
            return card;
        }

        public int GetDeckCardsRank() {
            int rank = 0;
            foreach (var kvp in this._deck_cards) {
                for (int i = 0; i < kvp.Value.Count; i++) {
                    rank += kvp.Value[i].Rank;
                }
            }
            return rank;
        }
        public void ClearDeckCards() {
            foreach (var kvp in this._deck_cards) {
                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    kvp.Value[i].Release();
                }
                kvp.Value.Clear();
            }
            this._deck_cards.Clear();
            foreach (var kvp in this._deck_awaken_cards) {
                kvp.Value.Release();
            }
            this._deck_awaken_cards.Clear();
        }
    }
}
