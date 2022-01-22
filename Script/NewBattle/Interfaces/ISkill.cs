using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public interface ISkillData
    {
        int ID { get; }
        int Level { get; }
        Type_Skill SkillType { get; }

        /// <summary>
        /// 用作技能的目标选择标记
        /// </summary>
        Type_Target SkillSelectTargetType { get; }

        /// <summary>
        /// 获取某次技能数值信息
        /// </summary>
        /// <param name="index">[0,?)</param>
        /// <returns></returns>
        ISkillValue GetSkillValue(int index);
    }

    public interface ISkillValue
    {
        /// <summary>
        /// 技能值
        /// </summary>
        Type_SkillValue ValueType { get; }
        /// <summary>
        /// 数值作用目标
        /// </summary>
        Type_Target TargetType { get; }
        /// <summary>
        /// 技能值
        /// </summary>
        int SkillValue { get; }
    }

    public interface IActiveSkill: ISkillData
    {
        int RankLevel { get; }
    }

    public interface IPassiveSkill : ISkillData
    {
        Type_SkillTriggerTime TriggerType { get; }
    }
}