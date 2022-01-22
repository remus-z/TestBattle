using System.Collections;
using System.Collections.Generic;
using System;
namespace TestBattle
{

    public enum Type_BattleCheckState
    {
        Continue = 0,
        Win = 1,
        Lose = 2,
        Error = 3,
    }

    public class AutoBattleFlowController
    {
        public readonly BattleLogic Battle;
        BattleFlowManager _battle_flow;
        public AutoBattleFlowController(BattleData data) {
            this.Battle = BattleManager.Instance.CreateBattle(data);
            this._battle_flow = this.Battle.GetManager<BattleFlowManager>();
        }
        public void DoFlow()
        {
            try
            {
                this.Battle.Start();
                this._battle_flow.StartPhase(0);
                //while (!this._battle_flow.Finish)
                //{
                //    BattleTeam curr_team = this.Battle.GetManager<BattleUnitManager>().GetCurrentTeam(this._battle_flow.CurrentActiveCamp);
                //    CardData card = this.GetACard(curr_team);
                //    if (card != null)
                //    {
                //        if (this._battle_flow.HandleSkillInput(card))
                //        {
                //            this._battle_flow.OnSkillDone(card);
                //        }
                //    }
                //    else
                //    {
                //        BattleLog.LogError("error");
                //        break;
                //    }
                //}
            }
            catch (Exception e) {
                BattleLog.LogError(e.ToString());
            }
            //BattleFlowManager flow = this.Battle.GetManager<BattleFlowManager>();
            //int phase_index = 0;
            //do
            //{
            //    if (!ProcessPhase(phase_index))
            //    {
            //        BattleLog.LogError("！！！");
            //        return;
            //    }
            //    phase_index++;
            //}
            //while (!flow.IsBattleFinish());
        }

        public void ProcessFlow() {
            try
            {
                if (!this._battle_flow.Finish)
                {
                    BattleTeam curr_team = this.Battle.GetManager<BattleUnitManager>().GetCurrentTeam(this._battle_flow.CurrentActiveCamp);
                    CardData card = this.GetACard(curr_team);
                    if (card != null)
                    {
                        if (this._battle_flow.BeginUnitTurn(card))
                        {
                            Type_BattleTurnEndState state = this._battle_flow.CheckUnitEndTurn(card.BattleUnitID);
                            if (state == Type_BattleTurnEndState.None)
                                return;
                            if (state == Type_BattleTurnEndState.TeamTurnEnd)
                            {
                                this._battle_flow.TerminateTeamTurn(curr_team.Camp);
                                this._battle_flow.StartTeamTurn(this._battle_flow.CurrentActiveCamp);
                            }
                            else if (state == Type_BattleTurnEndState.PhaseEnd)
                            {
                                int phase = this._battle_flow.Phase+1;
                                this._battle_flow.StartPhase(phase);
                                //todo  change phase
                                //BattleLog.LogError("todo  change phase");
                            }
                            else {
                                BattleLog.LogError(state.ToString());
                            }

                        }
                    }
                    else
                    {
                        this._battle_flow.TerminateTeamTurn(curr_team.Camp);

                        BattleLog.LogError("error");
                    }
                }
            }
            catch (Exception e)
            {
                BattleLog.LogError(e.ToString());
            }
        }

        //public bool ProcessPhase(int phase_index) {
        //    BattleFlowManager flow = this.Battle.GetManager<BattleFlowManager>();
        //    flow.StartPhase(phase_index);
        //    do
        //    {
        //        if (!ProcessTurn(flow.Turn++))
        //            return false;
        //    } while (!flow.IsPhaseFinish());
        //    return true;
        //}
        //public bool ProcessTurn(int turn_index) {
        //    BattleFlowManager flow = this.Battle.GetManager<BattleFlowManager>();

        //    Type_BattleCamp camp = flow.CurrentActiveCamp;
        //    bool succ = this.ProcessTeam(camp);
        //    flow.SwitchTurn(camp);

        //    camp = flow.CurrentActiveCamp;
        //    succ = this.ProcessTeam(camp);
        //    flow.SwitchTurn(camp);
        //    return succ;
        //}

        //public Type_BattleCheckState ProcessTeam(Type_BattleCamp camp)
        //{
        //    BattleFlowManager flow = this.Battle.GetManager<BattleFlowManager>();
        //    BattleCalculator calc = this.Battle.GetManager<BattleCalculator>();
        //    BattleTeam team = this.Battle.GetManager<BattleUnitManager>().GetCurrentTeam(camp);
        //    List<BattleUnit> units = team.GetUnits();
        //    for (int i = 0; i < units.Count; i++)
        //    {
        //        //units[i].DeckCards
        //        CardData card = this.GetACard(team);
        //        if (card == null)
        //        {
        //            BattleLog.LogError("no card");
        //            return false;
        //        }
        //        flow.HandleSkillInput(card, 0);
        //        //units[i].TryCastSkill
        //    }
        //    return true;
        //}

        public CardData GetACard(BattleTeam team)
        {
            Dictionary<int, CardData> awaken_cards = team.CardManager.DeckAwakenCards;
            if (awaken_cards.Count > 0) {
                foreach (var kvp in awaken_cards) {
                    return kvp.Value;
                }
            }
            Dictionary<int, List<CardData>> cards = team.CardManager.DeckActiveCards;
            foreach (var kvp in cards)
            {
                for (int i = 0; i < kvp.Value.Count; i++) {
                    CardData c = kvp.Value[i];
                    if (!team.GetUnit(c.BattleUnitID).IsSkillLock(c.SkillID, c.Rank)) {
                        return c;
                    }
                }
            }
            return null;
        }

        public void RunBattle() {
            BattleFlowManager flow = this.Battle.GetManager<BattleFlowManager>();
            Type_BattleTurnEndState bs = Type_BattleTurnEndState.None;
            while (bs == Type_BattleTurnEndState.None || bs == Type_BattleTurnEndState.TeamTurnEnd)
            {
                BattleTeam curr_team = this.Battle.GetManager<BattleUnitManager>().GetCurrentTeam(flow.CurrentActiveCamp);
                CardData card = GetACard(curr_team);
                if (card == null)
                {
                    BattleLog.LogError("no card to play :" + flow.CurrentActiveCamp);
                    flow.TerminateTeamTurn(curr_team.Camp);
                    flow.StartTeamTurn(flow.CurrentActiveCamp);
                }
                else
                {
                    if (flow.BeginUnitTurn(card))
                    {
                        bs = flow.CheckUnitEndTurn(card.BattleUnitID);
                        if (bs == Type_BattleTurnEndState.TeamTurnEnd)
                        {
                            flow.TerminateTeamTurn(curr_team.Camp);
                            flow.StartTeamTurn(flow.CurrentActiveCamp);
                        }
                        else if (bs == Type_BattleTurnEndState.PhaseEnd)
                        {
                            int phase = this._battle_flow.Phase + 1;
                            this._battle_flow.StartPhase(phase);
                            bs = Type_BattleTurnEndState.None;
                            //todo  change phase
                            //BattleLog.LogError("todo  change phase");
                        }
                    }
                    else
                    {
                        BattleLog.LogError("unit play card failed :" + card.BattleUnitID);
                        flow.TerminateTeamTurn(curr_team.Camp);
                        flow.StartTeamTurn(flow.CurrentActiveCamp);
                    }
                }
            }
            BattleLog.LogError("battle result :" + bs);
            //this.Battle.FinishBattle();
        }
    }

}
