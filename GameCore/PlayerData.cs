using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    public class PlayerData
    {
        public PlayerData()
        {            
            partyMembersArray = new int[8];
            itemsIDArray = new int[1000];
            itemsCountArray = new int[1000];
        }

        #region declaration
        public int boating = 0, playerFaceDirection = 0, mmapCoordinateX = 0, mmapCoordinateY = 0, smapCoordinateX = 0, smapCoordinateY = 0, boatCoordinateX1 = 0, boatCoordinateY1 = 0, boatCoordinateX2 = 0, boatCoordinateY2 = 0, boatFaceDirection = 0;
        public int[] partyMembersArray;
        public int[] itemsIDArray;
        public int[] itemsCountArray;

        #endregion
    }
}
