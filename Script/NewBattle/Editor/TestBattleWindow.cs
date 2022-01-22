﻿

using table;

using Sirenix.OdinInspector.Editor;
using System.Linq;
using UnityEngine;
using Sirenix.Utilities.Editor;
using Sirenix.Serialization;
using UnityEditor;
using Sirenix.Utilities;
using Sirenix.OdinInspector;
using TestBattle;
using System.Collections.Generic;
//using UnityEngine;
using System;
using Network;

using Google.Protobuf;
using Google.Protobuf.Collections;

namespace TestBattle.BattleEditor
{
    public class BattleEditorLogHandler : IBattleLog
    {
        public void Log(string message)
        {
            Debug.Log(message);
        }

        public void LogError(string message)
        {
            Debug.LogError(message);
        }
    }

    [LabelText("Battle Test")]
    public class TestBattleWindow : EditorWindow,IBattleFlow
    {
        public static TestBattleWindow Instance;
        public static bool FlowMode = false;
        [MenuItem("GameTools/TestBattle")]
        private static void OpenWindow()
        {
            var window = GetWindow<TestBattleWindow>();
            Instance = window;
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
        }

        private Rect _function_area = new Rect(10, 10, 1000, 150);

        private Rect _ally_slot_area = new Rect(10, 40, 200, 150);

        private Rect _enemy_slot_area = new Rect(650, 40, 200, 150);

        private Rect _slot_detail = new Rect(10, 520, 1200, 400);

        private Rect _battle_field = new Rect(1300, 40, 300, 300);

        private FunctionView _function_view;

        private int[] _ally_map = { 7, 4, 1, 6, 3, 0, 8, 5, 2 };

        private int[] _enemy_map = { 1, 4, 7, 0, 3, 6, 2, 5, 8 };
        //[SerializeField]
        private List<BattleSlotInfo> _ally_slot = new List<BattleSlotInfo>();
        //[SerializeField]
        private List<BattleSlotInfo> _enemy_slot = new List<BattleSlotInfo>();

        private List<List<BattleSlotInfo>> _enemy_slots = new List<List<BattleSlotInfo>>();


        private SlotDetailInfo _selected_slot_detail;

        public AutoBattleFlowController AutoBattle;

        public static int EditorBattleID = 0;
        public static int RunningBattleID = 0;
        public static BattleLogic Battle
        {
            get {
                BattleLogic battle = BattleManager.Instance.GetBattle(RunningBattleID);
                if (battle == null) {
                    battle = BattleManager.Instance.GetBattle(EditorBattleID);
                }
                return battle;
            }
        }

        public int Priority => 99;

        private void OnEnable()
        {
            this.Init();
        }

        public void Init() {
            TestBattleWindow.FlowMode = false;
            EditorBattleID = 0;
            RunningBattleID = 0;
            Table_Manager.Instance.Init(true);
            BattleLog.LogHandler = new BattleEditorLogHandler();

            EquipmentModule.Instance.Init();

            this._function_view = new FunctionView(this);
            this._ally_slot.Clear();
            for (int i = 0; i < 9; i++)
            {
                BattleSlotInfo slot = new BattleSlotInfo(Type_BattleCamp.Ally, this._ally_map[i]);
                int row = i / 3;
                int column = i % 3;
                slot.SetArea(new Rect(_ally_slot_area.xMin + column * _ally_slot_area.width, _ally_slot_area.yMin + row * _ally_slot_area.height, _ally_slot_area.width, _ally_slot_area.height));
                this._ally_slot.Add(slot);
            }

            this._enemy_slots.Clear();
            //this._enemy_slot.Clear();
            //for (int i = 0; i < 9; i++)
            //{
            //    BattleSlotInfo slot = new BattleSlotInfo(Type_BattleCamp.Enemy, this._enemy_map[i]);
            //    int row = i / 3;
            //    int column = i % 3;
            //    slot.SetArea(new Rect(_enemy_slot_area.xMin + column * _enemy_slot_area.width, _enemy_slot_area.yMin + row * _enemy_slot_area.height, _enemy_slot_area.width, _enemy_slot_area.height));
            //    this._enemy_slot.Add(slot);
            //}


            Dictionary<Type_Attribution, int> attris = new Dictionary<Type_Attribution, int>();
            for (int i = (int)Type_Attribution.StatStart; i < (int)Type_Attribution.StatEnd; i++)
            {
                attris.Add((Type_Attribution)i, 100);
            }
            this._selected_slot_detail = new SlotDetailInfo(this);

            this.SetPhase(1);
            BattleLogic battle = BattleManager.Instance.CreateBattle(new BattleData());
            EditorBattleID = battle.BattleID;
            battle.Start();
            this.SetSelectPhase(0);
        }

        private Vector2 _scroll_pos;
        private void OnGUI()
        {
            this._scroll_pos = GUI.BeginScrollView(new Rect(0,0,this.position.width, this.position.height), this._scroll_pos, new Rect(0,0,1600,1300));

          
            this._selected_slot_detail.OnGUI(this._slot_detail);
            for (int i = 0; i < this._ally_slot.Count; i++)
            {

                this._ally_slot[i].OnGUI();
            }
            for (int i = 0; i < this._enemy_slot.Count; i++)
            {
                this._enemy_slot[i].OnGUI();
            }

            if (Event.current.type == EventType.MouseDown)
            {
                Vector2 mouse_pos = Event.current.mousePosition;

                for (int i = 0; i < this._ally_slot.Count; i++)
                {
                    if (this._ally_slot[i].SlotArea.Contains(mouse_pos))
                    {
                        this._selected_slot_detail.SetSlot(this._ally_slot[i]);
                        Event.current.Use();
                        break;
                    }
                }
                for (int i = 0; i < this._enemy_slot.Count; i++)
                {
                    if (this._enemy_slot[i].SlotArea.Contains(mouse_pos))
                    {
                        this._selected_slot_detail.SetSlot(this._enemy_slot[i]);
                        Event.current.Use();
                        break;
                    }
                }
            }
            this._function_view.OnGUI(this._function_area);
            this._DrawBattleInfo(this._battle_field);
            GUI.EndScrollView();
            //this.Repaint();
        }
        void _DrawBattleInfo(Rect rect)
        {
            if (TestBattleWindow.FlowMode)
            {
                BattleFlowManager flow = TestBattleWindow.Battle.GetManager<BattleFlowManager>();
                Rect battle = new Rect(rect.x, rect.y, rect.width, 200);
                GUILayout.BeginArea(battle, "battle", GUI.skin.box);
                GUILayout.BeginVertical();
                GUILayout.Space(20);
                EditorGUILayout.LabelField("Phase", string.Format("{0}/{1}", flow.Phase, TestBattleWindow.Battle.BattleData.PhaseCount));
                EditorGUILayout.LabelField("Round", flow.Round.ToString());
                EditorGUILayout.LabelField("ActiveCamp", flow.CurrentActiveCamp.ToString());
                GUILayout.EndVertical();
                GUILayout.EndArea();

                Rect ally = new Rect(rect.x, rect.y + 200, rect.width / 2 - 5, 200);
                GUILayout.BeginArea(ally, "ally", GUI.skin.box);
                GUILayout.BeginVertical();
                GUILayout.Space(20);
                BattleTeam ally_team = Battle.GetManager<BattleUnitManager>().GetCurrentTeam(Type_BattleCamp.Ally);
                EditorGUILayout.LabelField("act", string.Format("{0}/{1}", ally_team.CardToPlayCount, ally_team.CardToPlayMaxCount));
                GUILayout.EndVertical();
                GUILayout.EndArea();

                Rect enemy = new Rect(rect.max.x - rect.width / 2 + 5, rect.y+200, rect.width / 2 - 5, 200);
                GUILayout.BeginArea(enemy, "enemy", GUI.skin.box);
                GUILayout.BeginVertical();
                GUILayout.Space(20);
                BattleTeam enemy_team = Battle.GetManager<BattleUnitManager>().GetCurrentTeam(Type_BattleCamp.Enemy);
                EditorGUILayout.LabelField("act", string.Format("{0}/{1}", enemy_team.CardToPlayCount, enemy_team.CardToPlayMaxCount));
                GUILayout.EndVertical();
                GUILayout.EndArea();
            }

        }


        private void OnDestroy()
        {
            Instance = null;
        }

        public BattleData CreateBattleData() {
            BattleData data = new BattleData();
           
            for (int i = 0; i < 9; i++) {
                data.AllyTeamData.Add(null);
            }
            for (int i = 0; i < this._ally_slot.Count; i++)
            {
                int slot_id = this._ally_map[i];
             
                if (this._ally_slot[i].Unit != null)
                {
                    data.AllyTeamData[slot_id] = this._ally_slot[i].Unit.UnitData;
                    if (this._ally_slot[i].Unit.IsLeader) {
                        data.AllyLeader = this._ally_slot[i].Unit.UnitData;
                    }
                }

            }
            for (int j = 0; j < this._enemy_slots.Count; j++)
            {
                List<IBattleUnitData> enemys = new List<IBattleUnitData>();
                for (int i = 0; i < 9; i++)
                {
                    enemys.Add(null);
                }
                List<BattleSlotInfo> slot = this._enemy_slots[j];
                for (int i = 0; i < slot.Count; i++)
                {
                    int slot_id = this._enemy_map[i];
                    if (slot[i].Unit != null)
                    {
                        enemys[slot_id] = slot[i].Unit.UnitData;
                        if (slot[i].Unit.IsLeader)
                        {
                            data.EnemyLeaders[0] = slot[i].Unit.UnitData;
                        }
                    }
                }
                data.EnemyTeamsData.Add(enemys);
            }

            //for (int i = 0; i < this._enemy_slot.Count; i++)
            //{
            //    int slot_id = this._enemy_map[i];
            //    if (this._enemy_slot[i].Unit != null)
            //    {
            //        enemys[slot_id] = this._enemy_slot[i].Unit.UnitData;
            //        if (this._enemy_slot[i].Unit.IsLeader)
            //        {
            //            data.EnemyLeaders[0] = this._enemy_slot[i].Unit.UnitData;
            //        }
            //    }

            //}
            //data.EnemyTeamsData.Add(enemys);
            return data;
        }


        public void StartAutoBattle() {
            
            BattleData data = CreateBattleData();
            AutoBattle = new AutoBattleFlowController(data);
            AutoBattle.DoFlow();
            RunningBattleID = AutoBattle.Battle.BattleID;
            TestBattleWindow.FlowMode = true;

            BattleUnitManager um = AutoBattle.Battle.GetManager<BattleUnitManager>();
            for (int i = 0; i < this._ally_slot.Count; i++)
            {
                this._ally_slot[i].ClearSlot();
                int slot_id = this._ally_map[i];
                BattleUnit unit = um.GetUnitBySlot(Type_BattleCamp.Ally, slot_id);
                if (unit != null) {
                    this._ally_slot[i].SetUnit(unit);
                }
            }

            for (int j = 0; j < this._enemy_slots.Count; j++) {
                List<BattleSlotInfo> slots = this._enemy_slots[j];
                BattleTeam team = um.GetHostileTeamByPhase(j);
                for (int i = 0; i < slots.Count; i++)
                {
                    slots[i].ClearSlot();
                    int slot_id = this._enemy_map[i];
                    BattleUnit unit = team.GetSlotUnit(slot_id);
                    if (unit != null)
                    {
                        slots[i].SetUnit(unit);
                    }
                }
            }

            this._enemy_slot = this._enemy_slots[0];
            AutoBattle.Battle.GetManager<BattleFlowManager>().AddBattleFlow(this);


            //for (int i = 0; i < this._enemy_slot.Count; i++)
            //{
            //    this._enemy_slot[i].ClearSlot();
            //    int slot_id = this._enemy_map[i];
            //    BattleUnit unit = um.GetUnitBySlot(Type_BattleCamp.Enemy, slot_id);
            //    if (unit != null)
            //    {
            //        this._enemy_slot[i].SetUnit(unit);
            //    }
            //}
        }
        public void CancelAutoBattle()
        {
            BattleManager.Instance.RemoveBattle(TestBattleWindow.RunningBattleID);
            RunningBattleID = 0;
            BattleLogic editor_battle = BattleManager.Instance.GetBattle(EditorBattleID);
            BattleUnitManager um = editor_battle.GetManager<BattleUnitManager>();
            for (int i = 0; i < this._ally_slot.Count; i++)
            {
                this._ally_slot[i].ClearSlot();
                int slot_id = this._ally_map[i];
                BattleUnit unit = um.GetUnitBySlot(Type_BattleCamp.Ally, slot_id);
                if (unit != null)
                {
                    this._ally_slot[i].SetUnit(unit);
                }
            }

            for (int j = 0; j < this._enemy_slots.Count; j++)
            {
                List<BattleSlotInfo> slots = this._enemy_slots[j];
                BattleTeam team = um.GetHostileTeamByPhase(j);
                for (int i = 0; i < slots.Count; i++)
                {
                    slots[i].ClearSlot();
                    int slot_id = this._enemy_map[i];
                    BattleUnit unit = team.GetSlotUnit(slot_id);
                    if (unit != null)
                    {
                        slots[i].SetUnit(unit);
                    }
                }
            }
        }

        public void RefreshEditorBattle() {
            BattleUnitManager unit_manager = BattleManager.Instance.GetBattle(EditorBattleID).GetManager<BattleUnitManager>();
            BattleTeam ally = unit_manager.GetCurrentTeam(Type_BattleCamp.Ally);
            if (ally != null)
            {
                for (int i = 0; i < this._ally_slot.Count; i++)
                {
                    if (this._ally_slot[i].Unit != null)
                    {
                        ally.RemoveUnit(this._ally_slot[i].Unit);
                        ally.AddUnit(this._ally_slot[i].Unit);
                    }
                }
            }
            BattleTeam enemy = unit_manager.GetHostileTeamByPhase(this.SelectPhase);
            if (enemy != null)
            {
                for (int i = 0; i < this._enemy_slot.Count; i++)
                {
                    if (this._enemy_slot[i].Unit != null)
                    {
                        enemy.RemoveUnit(this._enemy_slot[i].Unit);
                        enemy.AddUnit(this._enemy_slot[i].Unit);
                    }
                }
            }
        }

        public int PhaseCount = 1;
        public int SelectPhase = 0;
        public void SetPhase(int phase_count) {
            this.PhaseCount = phase_count;
            for (int i = this._enemy_slots.Count; i < this.PhaseCount; i++) {
                List<BattleSlotInfo> enemy_slot = new List<BattleSlotInfo>();
                this._enemy_slots.Add(enemy_slot);

                for (int j = 0; j < 9; j++)
                {
                    BattleSlotInfo slot = new BattleSlotInfo(Type_BattleCamp.Enemy, this._enemy_map[j]);
                    int row = j / 3;
                    int column = j % 3;
                    slot.SetArea(new Rect(_enemy_slot_area.xMin + column * _enemy_slot_area.width, _enemy_slot_area.yMin + row * _enemy_slot_area.height, _enemy_slot_area.width, _enemy_slot_area.height));
                    enemy_slot.Add(slot);
                }
            }
            this.SetSelectPhase(Mathf.Min(this.SelectPhase, this.PhaseCount - 1));
        }
        public void SetSelectPhase(int phase_index)
        {
            this.SelectPhase = phase_index;
            this._enemy_slot = this._enemy_slots[this.SelectPhase];
            BattleLogic battle = BattleManager.Instance.GetBattle(EditorBattleID);
            if (battle != null) {
                battle.GetManager<BattleUnitManager>().SwitchPhaseTeam(this.SelectPhase );
            }
        }

        public void StartPhase(int phase_id)
        {
            this._enemy_slot = this._enemy_slots[phase_id];
        }

        public void StartTurn(Type_BattleCamp camp, int turn)
        {
            
        }

        public void EndTurn(Type_BattleCamp camp, int turn)
        {
            
        }

        public void EndPhase(int phase_id)
        {
            
        }

        public void EndSkill()
        {
           
        }
    }

    public class FunctionView{

        public TestBattleWindow Window;

        public FunctionView(TestBattleWindow window) {
            Window = window;
        }
        public void OnGUI(Rect area)
        {
            GUILayout.BeginArea(area);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear", GUILayout.ExpandWidth(false)))
            {
                Window.Init();
            }


            if (GUILayout.Button("start battle flow", GUILayout.ExpandWidth(false)))
            {
                Window.StartAutoBattle();
                //BattleManager.DefaultBattle.GetManager<BattleFlowManager>().StartFlow();
                //BattleManager.DefaultBattle.GetManager<BattleFlowManager>().StartPhase(0);
            }

            if (GUILayout.Button("Step", GUILayout.ExpandWidth(false)))
            {
                if (this.Window.AutoBattle != null)
                {
                    this.Window.AutoBattle.ProcessFlow();
                }
                else
                {
                    List<BattleUnit> units = TestBattleWindow.Battle.GetManager<BattleUnitManager>().GetAllUnits();
                    for (int i = 0; i < units.Count; i++)
                    {
                        units[i].BuffManager.ProcessBuffTurn();
                    }
                }

            }
            if (this.Window.AutoBattle != null)
            {
                if (GUILayout.Button("OneKeyFinish", GUILayout.ExpandWidth(false)))
                {
                    if (this.Window.AutoBattle != null)
                    {
                        this.Window.AutoBattle.RunBattle();
                    }
                }
            }



            EditorGUI.BeginChangeCheck();
            TestBattleWindow.FlowMode = GUILayout.Toggle(TestBattleWindow.FlowMode,"FlowMode");
            if (EditorGUI.EndChangeCheck()) {
                if (TestBattleWindow.FlowMode == false) {
                    
                    Window.CancelAutoBattle();
                }
            }

            EditorGUI.BeginDisabledGroup(TestBattleWindow.FlowMode);
            EditorGUI.BeginChangeCheck();
            int phase_count = Window.PhaseCount;
            phase_count = EditorGUILayout.IntSlider("PhaseCount", phase_count, 1, 4);
            if (EditorGUI.EndChangeCheck())
            {
                Window.SetPhase(phase_count);
            }

            EditorGUI.BeginChangeCheck();
            int select_phase = Window.SelectPhase;
            List<string> displays = new List<string>();
            List<int> options = new List<int>();
            for (int i = 0; i < phase_count; i++) {
                displays.Add(i.ToString());
                options.Add(i);
            }
            select_phase = EditorGUILayout.IntPopup("SelectPhase", select_phase, displays.ToArray(),options.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                Window.SetSelectPhase(select_phase);
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.EndHorizontal();
            GUILayout.EndArea();

        }
    }

    public class BattleSlotInfo
    {
        public Type_BattleCamp Camp;
        public int Slot;
        public Rect SlotArea;

        private static Texture2D _empty_slot;

        public BattleUnit Unit;

        public List<int> SkillRank = new List<int>();

        public SkillBuffInfo ConfigBuff = null;
        public bool PreBuff = false;
        public static Texture2D EmptySlot {
            get {
                if (_empty_slot == null) {
                    _empty_slot = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/_Texture/UI/Renewal/Btn_Cancle.png");
                }
                return _empty_slot;
            }
        }
        public BattleSlotInfo(Type_BattleCamp camp, int slot)
        {
            this.Camp = camp;
            this.Slot = slot;
        }
        public void SetArea(Rect rect)
        {
            this.SlotArea = rect;
        }

        public void SetUnit(BattleUnit unit) {
            this.ClearSlot();
            this.Unit = unit;
            this.SkillRank.Clear();
            for (int i = 0; i < this.Unit.UnitData.SkillDatas.Count; i++)
            {
                Data_Skill skill = (Data_Skill)this.Unit.UnitData.SkillDatas[i];
                this.SkillRank.Add(1);
            }
        }

        public void ClearSlot() {
            this.Unit = null;
        }

        public void OnGUI()
        {
            GUILayout.BeginArea(this.SlotArea);
            GUILayout.Label(this.Slot.ToString());
            GUILayout.EndArea();
            Rect unit_area = new Rect(this.SlotArea.x, this.SlotArea.y + 15, this.SlotArea.width - 10, this.SlotArea.height - 15);
            if (this.Unit != null)
            {
                this._DrawUnit(unit_area);
            }
            else {
                EditorGUI.DrawPreviewTexture(unit_area, EmptySlot);
            }

        }

        private void _DrawUnit(Rect area) {
            BattleLogic battle = TestBattleWindow.Battle;
            GUILayout.BeginArea(area);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUIUtility.labelWidth = 50;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("UID", this.Unit.UnitID.ToString());
            EditorGUILayout.LabelField(this.Unit.UnitData.Name.ToString());
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("HP", string.Format("{0}/{1}", this.Unit.CurrentHp, this.Unit.MaxHp));
            EditorGUILayout.LabelField("SP", string.Format("{0}/{1}", this.Unit.AwakenGagePoint, this.Unit.AwakenGageMaxPoint));
            EditorGUILayout.EndHorizontal();
            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginDisabledGroup(TestBattleWindow.FlowMode);
            bool is_leader = this.Unit.IsLeader;
            is_leader = EditorGUILayout.ToggleLeft("Leader", is_leader);
            if (EditorGUI.EndChangeCheck()) {
                BattleTeam team = battle.GetManager<BattleUnitManager>().GetCurrentTeam(this.Unit.Camp);
                if (is_leader && team.GetTeamLeader() != this.Unit)
                {
                    team.SetLeader(this.Unit);
                    this.Unit.InitTriggerSkill();
                }
            }
            EditorGUI.EndDisabledGroup();

            for (int i = 0; i < this.Unit.UnitData.SkillDatas.Count; i++) {
                Data_Skill skill = (Data_Skill)this.Unit.UnitData.SkillDatas[i];
                if (skill.Type == Type_Skill.Active || skill.Type == Type_Skill.Awaken || skill.Type == Type_Skill.Leader)
                {
                    int rank_level = 1;
                    bool has_skill = true;

                    if (TestBattleWindow.FlowMode && battle != null)
                    {
                        BattleTeam team = battle.GetManager<BattleUnitManager>().GetCurrentTeam(this.Unit.Camp);
                        CardData card = team.CardManager.GetDeckCard(this.Unit.UnitID, skill.ID);
                        has_skill = card != null;
                        if (card != null && skill.Type == Type_Skill.Active) {
                            SkillRank[i] = rank_level = card.Rank;
                        }
                    }
                    if (TestBattleWindow.FlowMode && !has_skill)
                        continue;
                    EditorGUILayout.BeginHorizontal();
                   
                    if (skill.Type == Type_Skill.Active) {
                        EditorGUI.BeginDisabledGroup(TestBattleWindow.FlowMode);
                        SkillRank[i] = EditorGUILayout.IntSlider( SkillRank[i],1,3);
                        rank_level = SkillRank[i];
                        EditorGUI.EndDisabledGroup();
                    }
                   
                    if (has_skill)
                    {
                        EditorGUI.BeginDisabledGroup(TestBattleWindow.FlowMode && battle.GetManager<BattleFlowManager>().CurrentActiveCamp != this.Unit.Camp || Unit.IsSkillLock(skill.ID, rank_level) || !Unit.CanCastSkill());
                        if (GUILayout.Button(skill.ID.ToString()))
                        {
                            BattleTeam team = battle.GetManager<BattleUnitManager>().GetCurrentTeam(this.Unit.Camp);
                            CardData card = null;
                            if (TestBattleWindow.FlowMode)
                            {
                                BattleFlowManager flow = battle.GetManager<BattleFlowManager>();
                                card = team.CardManager.GetDeckCard(this.Unit.UnitID, skill.ID);
                                flow.BeginUnitTurn(card);
                                Type_BattleTurnEndState state = flow.CheckUnitEndTurn(card.BattleUnitID);
                                if (state == Type_BattleTurnEndState.TeamTurnEnd) {
                                    flow.TerminateTeamTurn(this.Unit.Camp);
                                    flow.StartTeamTurn(flow.CurrentActiveCamp);
                                }
                            }
                            else {
                                battle.GetManager<BattleFlowManager>().CurrentActiveCamp = this.Unit.Camp;
                                card = BattleClassCache.Instance.GetInstance<CardData>();
                                card.Init(this.Unit.UnitID, skill, SkillRank[i]);
                                SkillCalculateInfo info = this.Unit.GetSkillCastInfo(card, 0);
                                if (this.ConfigBuff != null) {
                                    if (this.PreBuff) {
                                        info.PreBuffs.Add(this.ConfigBuff);
                                    }
                                    else{
                                        info.PostBuffs.Add(this.ConfigBuff);
                                    }
                                }
                                battle.GetManager<BattleCalculator>().CalculateSkill(info);
                                //battle.GetManager<BattleFlowManager>().BeginUnitTurn(card, 0);

                            }
                            //Debug.LogError("cast skill:" + skill.ID);
                        }
                        EditorGUI.EndDisabledGroup();
                    }
                   
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }


    public class SlotDetailInfo{

        private string[] _hero_ids = null;
        private string[] _buff_types = null;
        private int selected_id = 0;
        private int _config_buff_index = 0;
        Table_Condition_info buff_info = null;

        private string[] _monster_ids = null;

        private bool _select_hero = true;

        TestBattleWindow _window;
        Vector2 _buff_scroll_vec = Vector2.zero;

        public SlotDetailInfo(TestBattleWindow window) {
            _window = window;
            IList<Table_Character> heroes = Table_Manager.Instance.GetDataList<Table_Character>();
            _hero_ids = new string[heroes.Count+1];
            _hero_ids[0] = "none";
            for (int i = 1; i <= heroes.Count; i++) {
                _hero_ids[i] = string.Format("{0}_{1}", heroes[i-1].id, heroes[i - 1].Name.ToString());
            }

            IList<Table_Condition_info> buffs = Table_Manager.Instance.GetDataList<Table_Condition_info>();
            _buff_types = new string[buffs.Count + 1];
            _buff_types[0] = "none";
            for (int i = 1; i <= buffs.Count; i++)
            {
                _buff_types[i] = buffs[i-1].condition_enum_name;
            }

            IList<Table_Enemy> enemies = Table_Manager.Instance.GetDataList<Table_Enemy>();
            _monster_ids = new string[enemies.Count + 1];
            _monster_ids[0] = "none";
            for (int i = 1; i <= enemies.Count; i++)
            {
                _monster_ids[i] = string.Format("{0}_{1}", enemies[i - 1].id, TextManager.Instance.GetText(enemies[i - 1].name).ToString());
            }

        }

        public BattleSlotInfo Slot;

        public void SetSlot(BattleSlotInfo slot) {
            this.Slot = slot;
            _buff_scroll_vec = Vector2.zero;
            if (this.Slot.ConfigBuff == null)
            {
                this._config_buff_index = 0;
            }
            else {
                RepeatedField<Table_Condition_info> buffs = Table_Manager.Instance.GetDataList<Table_Condition_info>();
                this.buff_info = buffs.Find(c => c.condition_enum_name == this.Slot.ConfigBuff.BuffType.ToString());
                this._config_buff_index = _buff_types.ToList().FindIndex(c => c == ((Type_Condition)this.Slot.ConfigBuff.BuffType).ToString());
            }
        }

        public void OnGUI(Rect area)
        {

            if (this.Slot != null)
            {
                EditorGUI.DrawPreviewTexture(this.Slot.SlotArea, Texture2D.whiteTexture);
            }
           
            if (this.Slot != null)
            {
                EditorGUI.BeginDisabledGroup(TestBattleWindow.FlowMode);
                this._draw_empty_slot(area);
                EditorGUI.EndDisabledGroup();
                if (this.Slot.Unit != null)
                {
                    area = new Rect(area.x, area.y + 20, area.width, area.height);
                    this._draw_slot(area);
                }
            }
           
        }
        private void _draw_empty_slot(Rect area) {
            GUILayout.BeginArea(area);

            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            this._select_hero = EditorGUILayout.ToggleLeft("Heroes", this._select_hero,GUILayout.ExpandWidth(false));
            if (EditorGUI.EndChangeCheck())
            {
                this.selected_id = 0;
            }

            EditorGUI.BeginChangeCheck();
            if (this._select_hero)
            {
                this.selected_id = EditorGUILayout.Popup(this.selected_id, this._hero_ids);
                if (EditorGUI.EndChangeCheck())
                {

                    if (this._hero_ids[this.selected_id] == "none")
                    {
                        this.Slot.ClearSlot();
                    }
                    else
                    {
                        string hero = this._hero_ids[selected_id];
                        string s_id = hero.Substring(0, hero.IndexOf('_'));

                        int hero_id = int.Parse(s_id);
                        Table_Character table = Table_Manager.Instance.GetData<Table_Character>(hero_id);
                        Data_Character_Valkyrie hero_data = new Data_Character_Valkyrie(table, 1, 60, 5, 0, 1, 1, 1, 1, 1, 1, 0, 0, false, 0, 0);
                        BattleUnit unit = TestBattleWindow.Battle.GetManager<BattleUnitManager>().CreateUnit(this.Slot.Camp, this.Slot.Slot, hero_data);
                        this.Slot.SetUnit(unit);
                        _window.RefreshEditorBattle();
                    }
                }
            }
            else {
                this.selected_id = EditorGUILayout.Popup(this.selected_id, this._monster_ids);
                if (EditorGUI.EndChangeCheck())
                {
                    if (this._monster_ids[this.selected_id] == "none")
                    {
                        this.Slot.ClearSlot();
                    }
                    else
                    {
                        string monster = this._monster_ids[selected_id];
                        string s_id = monster.Substring(0, monster.IndexOf('_'));

                        int monster_id = int.Parse(s_id);
                        Table_Enemy table = Table_Manager.Instance.GetData<Table_Enemy>(monster_id);
                        Data_Character_Monster monster_data = new Data_Character_Monster(table, 1, 1, "");
                        BattleUnit unit = TestBattleWindow.Battle.GetManager<BattleUnitManager>().CreateUnit(this.Slot.Camp, this.Slot.Slot, monster_data);
                        this.Slot.SetUnit(unit);
                        _window.RefreshEditorBattle();
                    }
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void _draw_slot(Rect area)
        {
            EditorGUI.BeginDisabledGroup(TestBattleWindow.FlowMode);
            EditorGUI.BeginChangeCheck();
            Rect config_rect = new Rect(area.x, area.y, 300, area.height);
            this._draw_config(config_rect);
            Rect attri_rect = new Rect(config_rect.x + config_rect.width + 10, area.y, 250, area.height);
            this._draw_attri(attri_rect);
            if (EditorGUI.EndChangeCheck()) {
                this.Slot.Unit.ResetAttris(this.Slot.Unit.UnitData.Attris);
            }
            
            Rect statistic_rect = new Rect(attri_rect.x + attri_rect.width + 10, area.y, 200, area.height);
            this._draw_statistics(statistic_rect);

            Rect buff_config = new Rect(statistic_rect.x + statistic_rect.width + 10, area.y, 300, area.height);
            this._draw_config_buff(buff_config);
            EditorGUI.EndDisabledGroup();
            Rect buff_rect = new Rect(buff_config.x + buff_config.width + 10, area.y, 250, area.height);
            this._draw_buffs(buff_rect);
        }

        private void _draw_config(Rect config_rect)
        {
            GUILayout.BeginArea(config_rect);
            EditorGUIUtility.labelWidth = 70;
            Data_Character data = (Data_Character)Slot.Unit.UnitData;

            EditorGUILayout.LabelField("Power", data.Power.ToString(), GUILayout.MinWidth(30/*area.width*/));

            int level = data.Level;
            level = EditorGUILayout.IntSlider("Level", level,1,150);
            if (data.Level != level)
            {
                data.SetLevel(level);
            }

            for (int i = 0; i < this.Slot.Unit.UnitData.SkillDatas.Count; i++)
            {
                Data_Skill skill = (Data_Skill)this.Slot.Unit.UnitData.SkillDatas[i];
                int skill_level = skill.Level;
                EditorGUI.BeginChangeCheck();
                if (skill.SkillType != Type_Skill.Leader)
                {
                    skill_level = EditorGUILayout.IntSlider(string.Format("{0}:Lv", skill.ID), skill_level, 1, 90);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    skill.SetLevel(skill_level);
                }
            }

            if (data is Data_Character_Valkyrie)
            {
                Data_Character_Valkyrie hero_data = (Data_Character_Valkyrie)data;
                int rebirth = hero_data.Rebirth;
                rebirth = EditorGUILayout.IntSlider("Rebirth", rebirth, 1, 5);
                if (hero_data.Rebirth != rebirth)
                {
                    hero_data.SetRebirth(rebirth);
                    this.Slot.Unit.InitTriggerSkill();
                }

                int rank_level = hero_data.RankLevel;
                rank_level = EditorGUILayout.IntField("Rank", rank_level);
                if (hero_data.RankLevel != rank_level)
                {
                    hero_data.SetRankLevel(rank_level);
                    hero_data.ClearEquipment();
                }

                for (int i = 0; i < 6; i++)
                {
                    int equip_id = EquipmentModule.Instance.GetHeroSlotEquipment(hero_data.ID, hero_data.RankLevel, i);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("slot1", equip_id.ToString());
                    EquippedItem equip = hero_data.GetRankEquippedItem(i);
                    bool equiped = equip != null;
                    EditorGUI.BeginChangeCheck();
                    equiped = EditorGUILayout.Toggle(equiped);
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (equiped)
                        {
                            equip = new EquippedItem() { slotid = i, enhancedot = 0, level = 0, equipid = equip_id };
                            hero_data.AddRankEquipment(equip);
                        }
                        else
                        {
                            hero_data.RemoveRankEquipment(i);
                        }
                    }
                    if (equiped)
                    {
                        int enhance_max = EquipmentModule.Instance.GetEquipmentReinforceMaxLevel(equip.equipid);
                        if (enhance_max > 0)
                        {
                            EditorGUI.BeginChangeCheck();
                            int equip_level = equip.level;
                            equip_level = EditorGUILayout.IntSlider(equip_level, 0, enhance_max);
                            if (EditorGUI.EndChangeCheck())
                            {
                                hero_data.RemoveRankEquipment(i);
                                equip = new EquippedItem() { slotid = i, enhancedot = 0, level = equip_level, equipid = equip_id };
                                hero_data.AddRankEquipment(equip);
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }

            }
            else if (data is Data_Character_Monster) {
                Data_Character_Monster monster_data = (Data_Character_Monster)data;
                int attri_type = monster_data.GetAttriType();
                EditorGUI.BeginChangeCheck();
                attri_type = EditorGUILayout.IntSlider("AttriType", attri_type, 1, 7);
                if (EditorGUI.EndChangeCheck()) {
                    monster_data.SetAttriType(attri_type);
                }
            }
            GUILayout.EndArea();
        }
        private void _draw_attri(Rect attri_rect)
        {
            EditorGUIUtility.labelWidth = 100;
            Data_Character data = (Data_Character)Slot.Unit.UnitData;
            GUILayout.BeginArea(attri_rect);
            for (int i = (int)Type_Attribution.StatStart; i < (int)Type_Attribution.StatEnd; i++)
            {
                if (i == (int)Type_Attribution.StatStart || i == (int)Type_Attribution.StatEnd)
                    continue;
                Type_Attribution attri = (Type_Attribution)i;
                EditorGUILayout.LabelField(attri.ToString(), string.Format("ui:{0}/curr:{1}", data.GetUIStat(attri), Slot.Unit.GetAttribute(attri)));
            }
            GUILayout.EndArea();
        }
        private void _draw_buffs(Rect buff_rect) {

            GUILayout.BeginArea(buff_rect);
            EditorGUIUtility.labelWidth = 100;
            BattleUnit unit = Slot.Unit;
            List<BaseBattleBuff> ordered_buffs = unit.BuffManager.GetAllOrderedBuffs();

            this._buff_scroll_vec = EditorGUILayout.BeginScrollView(this._buff_scroll_vec);
            for (int i = 0; i < ordered_buffs.Count; i++){
                BaseBattleBuff buff = ordered_buffs[i];
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.LabelField("buff type", ((Type_Condition)buff.BuffType).ToString());
                EditorGUILayout.LabelField("buff_uid", buff.BuffUID.ToString());
                EditorGUILayout.LabelField("buff_id", buff.BuffID.ToString());
                EditorGUILayout.LabelField("turn", string.Format("{0}/{1}",buff.Turn,buff.MaxTurn));
                EditorGUILayout.LabelField("value", buff.Value.ToString());
                EditorGUILayout.LabelField("caster", buff.Caster!=null?buff.Caster.UnitID.ToString():"null");
                EditorGUILayout.LabelField("check value", buff.BuffData.CheckValue);
                buff.OnGUI();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void _draw_statistics(Rect statistic_rect)
        {
            UnitStatisticData data = TestBattleWindow.Battle.GetManager<BattleStatisticManager>().GetStatisticData(this.Slot.Unit.UnitID);
            if (data != null)
            {
                GUILayout.BeginArea(statistic_rect);
                EditorGUIUtility.labelWidth = 150;
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.LabelField("cause damage", data.CauseDamages.Total.ToString());
                EditorGUILayout.LabelField("bear damage", data.BearDamages.Total.ToString());
                EditorGUILayout.LabelField("cause heal", data.CauseRecoverys.Total.ToString());
                EditorGUILayout.LabelField("bear heal", data.BearRecoverys.Total.ToString());
                EditorGUILayout.LabelField("cause shield", data.CauseShields.Total.ToString());
                EditorGUILayout.LabelField("bear shield", data.BearShields.Total.ToString());
                EditorGUILayout.EndVertical();
                GUILayout.EndArea();
            }
        }

        private void _draw_config_buff(Rect config_buff_rect) {
            if (!TestBattleWindow.FlowMode) {
                GUILayout.BeginArea(config_buff_rect);
                EditorGUI.BeginChangeCheck();
                this._config_buff_index = EditorGUILayout.Popup(this._config_buff_index, this._buff_types);

                if (EditorGUI.EndChangeCheck())
                {

                    if (this._buff_types[this._config_buff_index] == "none")
                    {
                        this.Slot.ConfigBuff = null;
                    }
                    else
                    {
                        this.Slot.ConfigBuff = new SkillBuffInfo();
                        string buff = this._buff_types[_config_buff_index];
                        Type_Condition buff_type = (Type_Condition)System.Enum.Parse(typeof(Type_Condition), buff);
                        this.Slot.ConfigBuff.BuffType = (int)buff_type;
                    }
                }
                
                if (this.Slot.ConfigBuff != null) {
                    this.Slot.ConfigBuff.BuffID = EditorGUILayout.IntField("BuffID", this.Slot.ConfigBuff.BuffID);
                    RepeatedField<Table_Condition_info> buffs = Table_Manager.Instance.GetDataList<Table_Condition_info>();
                    Table_Condition_info info = buffs.Find(c => c.condition_enum_name == this._buff_types[_config_buff_index]);
                    EditorGUILayout.LabelField("caster", this.Slot.Unit.UnitID.ToString());
                    EditorGUILayout.LabelField("BuffKind", info.condition_type.ToString());
                    this.Slot.ConfigBuff.Value = EditorGUILayout.IntField("Value", this.Slot.ConfigBuff.Value);
                    this.Slot.ConfigBuff.Turn = EditorGUILayout.IntField("Turn", this.Slot.ConfigBuff.Turn);
                    this.Slot.ConfigBuff.Probability = EditorGUILayout.IntSlider("Probability", this.Slot.ConfigBuff.Probability, 0,10000);
                    this.Slot.ConfigBuff.TargetType = EditorGUILayout.IntField("TargetType", this.Slot.ConfigBuff.TargetType);
                    this.Slot.ConfigBuff.ExistType = EditorGUILayout.IntField("ExistType", this.Slot.ConfigBuff.ExistType);
                    this.Slot.ConfigBuff.CheckValue = EditorGUILayout.TextField("check value",this.Slot.ConfigBuff.CheckValue);
                    this.Slot.ConfigBuff.ExistValue = EditorGUILayout.TextField("exist value", this.Slot.ConfigBuff.ExistValue);
                    this.Slot.PreBuff = EditorGUILayout.Toggle("PreBuff",this.Slot.PreBuff);
                }
                GUILayout.EndArea();
            }
        }
    }
}

