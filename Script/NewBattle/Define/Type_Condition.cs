using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type_Condition
{
    none,
    stun,
    hax,
    freezing,
    block_skill,
    block_attack,//xxx  use block_skill
    block_buff,//xxx    use block_skill
    block_debuff,//xxx  use block_skill
    block_recovery,//xxx    use block_skill
    block_awaken,//xxx  use block_skill
    block_general_skills,//xxx  use block_skill
    block_recovery_value,
    attack_up,
    critical_up,
    critical_value_up,
    defence_physical_up,
    defence_magic_up,
    defence_critical_up,
    focus_up,//xxx
    resist_up,  //xxx  
    attack_down,
    critical_down,
    critical_value_down,
    defence_physical_down,
    defence_magic_down,
    defence_critical_down,
    focus_down,//xxx
    resist_down,//xxx
    shield_damage,
    shield_devine,
    immun_condition,
    immun_condition_specific,//xxx 可以不用了
    clear_debuff,//xxx
    clear_buff,//xxx
    clear_condition, // replace clear_condition_specific
    clear_condition_specific,// xxx
    recovery,//xxx
    recovery_hp_max, //vvxx 留着 还没实现
    awaken_gage_up,
    awaken_gage_down,
    skill_rank_up,//???
    skill_rank_down,//???
    suck_blood,
    damage_up,
    damage_down,
    hit_damage_up,
    damage_addition,
    electric,
    reflect,
    provocation,
    sneak,
    recovery_dot,
    poison_dot,
    freezing_dot,
    wound_dot,
    burn_dot,
    atk_crtical_value, //???
    atk_suck_blood,
    clear_stun,//xxx
    clear_hax,//xxx
    clear_freezing,//xxx
    mix_stat_atk_up,//xxx
    mix_stat_def_up,//xxx
    mix_stat_cri_up,//xxx
    mix_stat_atk_down,//xxx
    mix_stat_def_down,//xxx
    mix_stat_cri_down,//xxx
    berserk, //???
    pvp_dot,
    card_select_add,//???
    card_carry_add,//???
    card_get_add,//???
    awaken_gage_max_add,//???
    furify,//xxx
    private_shiningnatsuha_attack_up,//xxx
    rebirth, //???
    passive_buff, //???
    passive_debuff, //???
    #region new buff
    paralysis,
    decline_recovery,
    glamorous,
    true_damage,
    inspire,
    //backrow_extra_damage,
    row_extra_damage,
    clear_num_debuff,// 
    action_turn_change,
    shield_devine_once,
    enchanting,
    restoration,
    blood_shield,
    debuff_num_extra_damage,
    debuff_type_extra_damage,
    //confusion,
    #endregion
    #region 2012812 new
    pvp_damage_up,
    hp_damage_up,
    damage_share,
    burn_damage_up,
    burn_explode_trigger,
    hax_damage_up,
    full_hp_damage_up,
    slain,
    steal_gage_point,
    electric_dot,
    random_add_gage,//random one ally,one gage
    random_clear_buffs,//clear n debuffs from m allys
    steal_buff,//steal n buffs from target
    counter_attack,
    assist_attack,
    #endregion
    #region 2012826 new
    sputtering,//  溅射
    sign,// 标记
    drain,//  吸魂
    resist,//  抵抗
    protection,// 保护
    absolute_defence_physical_up,
    absolute_defence_magic_up,
    absolute_defence_physical_down,
    absolute_defence_magic_down,
    avoid_death,//免死
    single_damage_up,//增加受到的单体技能伤害
    single_damage_down,//减少受到的单体技能伤害
    decline_hp_rate,
    symbiosis,//共生 9999999
    summon,//召唤
    clear_num_buff,//驱散n个buff
    dot_change,//dot 伤害加深
    #endregion
    attack_down_noclear,
}
