using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    public class EventUnit : MapUnit
    {
        public EventUnit() : base(UnitType.UnitType_Event)
        {
            this.eventIDList = new List<int>();
        }

        #region declaration
        public int unitNumber = 0;
        public int goThrough = 0;
        public int startTextureID = 0;
        public int endTextureID = 0;
        public List<int> eventIDList;
        public EventUnitState state = EventUnitState.EventUnitState_Wait;
        #endregion

        #region business

        #endregion
    }
}