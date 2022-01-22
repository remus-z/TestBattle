using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestBattle {
    public class BattleUnitViewManager : BattleBaseManager
    {
        public override void OnInit()
        {
            
        }

        public override void OnRelease()
        {
            
        }

        //private Dictionary<int, BattleCharacter> _battle_unit_views = new Dictionary<int, BattleCharacter>();

        //public void BindUnitView(BattleUnit unit_data,BattleCharacter battle_unit_view) {
        //    this._battle_unit_views[unit_data.UnitID] = battle_unit_view;
        //    //battle_unit_view.UnitData = unit_data;
        //    //todo ..
        //}

        //public BattleCharacter GetUnitView(int unit_id) {
        //    BattleCharacter view = null;
        //    this._battle_unit_views.TryGetValue(unit_id, out view);
        //    return view;
        //}
    }
}
