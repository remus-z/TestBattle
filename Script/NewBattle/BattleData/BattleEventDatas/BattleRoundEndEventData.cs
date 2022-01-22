using System.Collections;
using System.Collections.Generic;
namespace TestBattle
{
    public class BattleRoundEndEventData : BaseBattleEventData
    {
        public override BattleEvent BattleEvent => BattleEvent.BattleRoundEnd;
        public Type_BattleTurnEndState BattleState = Type_BattleTurnEndState.None;
        public int Phase;
        public int Round;
        public void Init(Type_BattleTurnEndState state,int phase,int round)
        {
            this.BattleState = state;
            this.Phase = phase;
            this.Round = round;
        }

        public override void Reset()
        {
            this.BattleState = Type_BattleTurnEndState.None;
            this.Phase = 0;
            this.Round = 0;
        }

        public static BattleRoundEndEventData CreateEventData(Type_BattleTurnEndState state, int phase, int round)
        {
            BattleRoundEndEventData data = BattleClassCache.Instance.GetInstance<BattleRoundEndEventData>();
            data.Init(state, phase, round);
            return data;
        }
    }
}
