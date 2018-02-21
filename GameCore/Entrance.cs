using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    public class Entrance
    {
        public Entrance()
        {

        }

        #region declaration
        public MapType destMapType = MapType.Scene;
        public int sceneID = 0;
        #endregion
    }

    public enum MapType
    {
        World,
        Scene
    }
}