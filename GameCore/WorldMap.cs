﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    public class WorldMap
    {
        public WorldMap()
        {
            earthLayerMatrix = new FixedUnit[480, 480];
            surfaceLayerMatrix = new FixedUnit[480, 480];
            buildingLayerMatrix = new WorldBuildingUnit[480, 480];
            buildXYLayerMatrix = new BuildXY[480, 480];
            entranceMatrix = new Entrance[480, 480];
        }

        #region declaration
        public FixedUnit[,] earthLayerMatrix;
        public FixedUnit[,] surfaceLayerMatrix;
        public WorldBuildingUnit[,] buildingLayerMatrix;
        public BuildXY[,] buildXYLayerMatrix;
        public Entrance[,] entranceMatrix;
        #endregion

        #region business
        public bool Movable(int pmTargetCoordinateX, int pmTargetCoordinateY)
        {
            if (pmTargetCoordinateX < 0 || pmTargetCoordinateX >= 480 || pmTargetCoordinateY < 0 || pmTargetCoordinateY >= 480)
            {
                return false;
            }
            else if (buildingLayerMatrix[pmTargetCoordinateX, pmTargetCoordinateY] != null)
            {
                return false;
            }
            else if (buildXYLayerMatrix[pmTargetCoordinateX, pmTargetCoordinateY] != null)
            {
                return false;
            }
            return true;
        }

        public int Enterable(int pmTargetCoordinateX, int pmTargetCoordinateY)
        {
            if (pmTargetCoordinateX < 0 || pmTargetCoordinateX >= 480 || pmTargetCoordinateY < 0 || pmTargetCoordinateY >= 480)
            {
                return -1;
            }
            else if (entranceMatrix[pmTargetCoordinateX, pmTargetCoordinateY] != null)
            {
                return entranceMatrix[pmTargetCoordinateX, pmTargetCoordinateY].sceneID;
            }

            return -1;
        }
        #endregion
    }

    public class BuildXY
    {
        #region declaration
        public int buildX = 0;
        public int buildY = 0;
        #endregion        
    }
}
