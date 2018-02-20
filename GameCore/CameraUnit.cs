using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    public class CameraUnit : AnchorUnit
    {
        public CameraUnit() : base()
        {

        }

        #region declaration
        public bool bondToPlayer = false;
        #endregion

        #region business
        public void BindToPlayer()
        {
            bondToPlayer = true;
        }

        public void UnbindFromPlayer()
        {

        }

        public void ResetCamera(int pmPosX, int pmPosY)
        {
            positionX = pmPosX;
            positionY = pmPosY;
        }
        #endregion        
    }
}
