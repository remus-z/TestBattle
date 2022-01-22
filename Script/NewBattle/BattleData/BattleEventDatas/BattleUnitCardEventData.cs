using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public class BattleUnitCardEventData : BaseBattleEventData
    {
        public override BattleEvent BattleEvent => BattleEvent.BattleUnitDrewCards;

        public int UnitID;
        public int SkillID;
        public int SkillLevel;
        public int SkillType;
        public int CardRank;

        public void Init(int uid,int skill_id,int skill_level,int skill_type ,int rank) {
            this.UnitID = uid;
            this.SkillID = skill_id;
            this.SkillLevel = skill_level;
            this.SkillType = skill_type;
            this.CardRank = rank;
        }

        public override void Reset()
        {
            this.UnitID = 0;
            this.SkillID = 0;
            this.SkillLevel = 0;
            this.SkillType = 0;
            this.CardRank = 0;
        }

        public static BattleUnitCardEventData CreateEventData(int uid, int skill_id, int skill_level, int skill_type, int rank)
        {
            BattleUnitCardEventData data = BattleClassCache.Instance.GetInstance<BattleUnitCardEventData>();
            data.Init(uid, skill_id, skill_level, skill_type, rank);
            return data;
        }
    }

    public class BattleUnitUseCardEventData : BattleUnitCardEventData
    {
        public override BattleEvent BattleEvent => BattleEvent.BattleUnitUseCards;
        public static new BattleUnitUseCardEventData CreateEventData(int uid, int skill_id, int skill_level, int skill_type, int rank)
        {
            BattleUnitUseCardEventData data = BattleClassCache.Instance.GetInstance<BattleUnitUseCardEventData>();
            data.Init(uid, skill_id, skill_level, skill_type, rank);
            return data;
        }
    }
}
