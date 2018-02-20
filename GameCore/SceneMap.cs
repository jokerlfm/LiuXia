using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    public class SceneMap
    {
        public SceneMap(int pmMapID)
        {
            this.mapID = pmMapID;
            this.floorLayerMatrix = new FixedUnit[64, 64];
            this.buildingLayerMatrix = new FixedUnit[64, 64];
            this.hangLayerMatrix = new FixedUnit[64, 64];
            this.eventLayerMatrix = new EventUnit[64, 64];
        }

        #region declaration
        public int mapID = 0;
        public FixedUnit[,] floorLayerMatrix;
        public FixedUnit[,] buildingLayerMatrix;
        public FixedUnit[,] hangLayerMatrix;
        public EventUnit[,] eventLayerMatrix;
        #endregion

        #region business
        public bool Movable(int pmTargetCoordinateX, int pmTargetCoordinateY)
        {
            if (pmTargetCoordinateX < 0 || pmTargetCoordinateX >= 64 || pmTargetCoordinateY < 0 || pmTargetCoordinateY >= 64)
            {
                return false;
            }
            else if (buildingLayerMatrix[pmTargetCoordinateX, pmTargetCoordinateY] != null)
            {
                return false;
            }
            else
            {
                if (eventLayerMatrix[pmTargetCoordinateX, pmTargetCoordinateY] != null)
                {
                    if (eventLayerMatrix[pmTargetCoordinateX, pmTargetCoordinateY].goThrough != 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion
    }
}