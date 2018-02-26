using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    public class Event
    {
        public Event(int pmID)
        {
            eventID = pmID;
            instructionDictionary = new Dictionary<int, Instruction>();
        }

        #region declaration
        public int eventID = 0;
        public EventType type = EventType.EventType_ConstEvent;
        public Dictionary<int, Instruction> instructionDictionary;
        #endregion

        #region business

        #endregion
    }

    public enum EventType
    {
        EventType_ConstEvent,
        EventType_DynamicEvent
    }
}