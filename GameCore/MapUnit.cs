using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    public class MapUnit : Unit
    {
        public MapUnit(UnitType pmUT) : base(pmUT)
        {

        }

        #region declaration
        public int coordinateX = 0;
        public int coordinateY = 0;

        public float screenBasePositionX = 0;
        public float screenBasePositionY = 0;
        #endregion

        #region business
        public void SetFixedCoordinate(int pmX, int pmY)
        {
            this.coordinateX = pmX;
            this.coordinateY = pmY;

            //this.screenBasePositionX = this.coordinateX * 40;
            //this.screenBasePositionY = this.coordinateY * 40;

            this.screenBasePositionX = this.coordinateX + (this.coordinateX * ConstManager.BaseMapCoordinateSize) - (this.coordinateY * ConstManager.BaseMapCoordinateSize);
            this.screenBasePositionY = this.coordinateY + (this.coordinateX * ConstManager.BaseMapCoordinateSize / 2) + (this.coordinateY * ConstManager.BaseMapCoordinateSize / 2);
        }
        #endregion
    }
}