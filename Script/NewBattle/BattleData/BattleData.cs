using System.Collections;
using System.Collections.Generic;

using table;

namespace TestBattle
{
    public enum UnitType { 
        None,
        Hero,
        Monster,
    }
    public interface IBattleUnitData{
        int UnitTID { get; }
        string Name { get; }
        UnitType UnitType { get; }
        int UnitLevel { get; }
        int UnitPower { get; }

        IBattleUnitSkillData BaseSkillData { get; }
        TeamRowType BattleTeamRow { get; }
        Type_BattleAttribution BattleAttribution { get; } 

        Dictionary<Type_Attribution, int> Attris { get; }
        List<IBattleUnitSkillData> SkillDatas { get; }

        bool LeaderSkillActive { get; }
        bool PassiveSkillActive { get; }
    }

    public interface IBattleUnitSkillData
    {
        int SkillID { get; }
        int SkillLevel { get; }
        Type_Skill SkillType { get; }
        Type_SkillAttribution GetSkillAttribution(int rank_lvl);

        Table_Skill_info Data { get; }

        Table_Skill_data_summary SkillSummaryData { get; }
        Table_Skill_value_data SkillValueData1 { get; }
        Table_Skill_value_data SkillValueData2 { get; }

        Type_SkillTriggerTime TriggerType { get; }
    }
    public class BattleData
    {
        public Type_Battle BattleType;
        public List<IBattleUnitData> AllyTeamData = new List<IBattleUnitData>();
        public List<List<IBattleUnitData>> EnemyTeamsData = new List<List<IBattleUnitData>>();
        public IBattleUnitData AllyLeader = null;
        public List<IBattleUnitData> EnemyLeaders = new List<IBattleUnitData>() { null, null };

        public int PhaseCount => this.EnemyTeamsData.Count;
        public List<IBattleUnitData> GetPhaseEnemy(int phase_index) {
            if (phase_index >= 0 && phase_index < PhaseCount)
                return EnemyTeamsData[phase_index];
            return null;
        }

        public void AddAlly(IBattleUnitData data, int slot)
        {
            for (int i = AllyTeamData.Count; i <= slot; i++)
            {
                AllyTeamData.Add(null);
            }
            AllyTeamData[slot] = data;
        }

        public void AddEnemy(IBattleUnitData data, int phase, int slot)
        {

            for (int i = EnemyTeamsData.Count; i <= phase; i++)
            {
                EnemyTeamsData.Add(new List<IBattleUnitData>());
            }
            List<IBattleUnitData> team = EnemyTeamsData[phase];
            for (int i = team.Count; i <= slot; i++)
            {
                team.Add(null);
            }
            team[slot] = data;
        }
    }
}