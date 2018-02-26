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
            entranceMatrix = new Entrance[64, 64];
        }

        #region declaration
        public int mapID = 0;
        public int enterMusicID = 0;
        public int exitMusicID = 0;
        public int opened = 0;
        public int playerEnterCoordinateX = 0, playerEnterCoordinateY = 0;
        public FixedUnit[,] floorLayerMatrix;
        public FixedUnit[,] buildingLayerMatrix;
        public FixedUnit[,] hangLayerMatrix;
        public EventUnit[,] eventLayerMatrix;
        public Entrance[,] entranceMatrix;
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

        public int Enterable(int pmTargetCoordinateX, int pmTargetCoordinateY)
        {
            if (pmTargetCoordinateX < 0 || pmTargetCoordinateX >= 64 || pmTargetCoordinateY < 0 || pmTargetCoordinateY >= 64)
            {
                return -2;
            }
            else if (entranceMatrix[pmTargetCoordinateX, pmTargetCoordinateY] != null)
            {
                if (entranceMatrix[pmTargetCoordinateX, pmTargetCoordinateY].destMapType == MapType.Scene)
                {
                    return entranceMatrix[pmTargetCoordinateX, pmTargetCoordinateY].sceneID;
                }
                else if (entranceMatrix[pmTargetCoordinateX, pmTargetCoordinateY].destMapType == MapType.World)
                {
                    return -1;
                }
            }

            return -2;
        }
        #endregion
    }
}