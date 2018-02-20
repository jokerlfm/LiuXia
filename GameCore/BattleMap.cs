using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    public class BattleMap
    {
        public BattleMap(int pmID)
        {
            this.battleMapID = pmID;
            this.floorLayerMatrix = new FixedUnit[64, 64];
            this.buildingLayerMatrix = new FixedUnit[64, 64];
        }

        #region declaration
        public int battleMapID = 0;
        public FixedUnit[,] floorLayerMatrix;
        public FixedUnit[,] buildingLayerMatrix;
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

            }
            return true;
        }
        #endregion
    }
}
