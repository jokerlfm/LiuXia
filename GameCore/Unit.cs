using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    public class Unit
    {
        public Unit(UnitType pmType)
        {
            this.type = pmType;
        }

        #region declaration
        UnitType type = UnitType.UnitType_Fixed;
        #endregion

        #region business

        #endregion
    }

    public enum UnitType
    {
        UnitType_Fixed,
        UnitType_Anchor,
        UnitType_Dynamic,        
        UnitType_Event
    }

    public enum DynamicUnitState
    {
        DynamicUnitState_Stopped,
        DynamicUnitState_Moving,
        DynamicUnitState_Acting
    }

    public enum EventUnitState
    {
        EventUnitState_Wait,
        EventUnitState_Finished
    }
}
