using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    public class AnchorUnit : Unit
    {
        public AnchorUnit() : base(UnitType.UnitType_Anchor)
        {

        }

        #region declaration
        public int positionX = 0;
        public int positionY = 0;

        public int movingPositionX = 0;
        public int movingPositionY = 0;

        public int targetPostionX = 0;
        public int targetPostionY = 0;
        public int moveSpeed = 0;

        public bool moving = false;
        #endregion

        #region business
        public void MoveTo(int pmTargetPositionX, int pmTargetPositionY, int pmSpeed)
        {
            if (moving)
            {
                return;
            }
            movingPositionX = positionX;
            movingPositionY = positionY;
            this.targetPostionX = pmTargetPositionX;
            this.targetPostionY = pmTargetPositionY;
            moveSpeed = pmSpeed;
            this.moving = true;
        }

        public void UpdateMoving(int pmElapsed)
        {
            int movedDistance = pmElapsed * moveSpeed / 1000;
            bool xReady = false, yReady = false;
            if (!xReady)
            {
                if (targetPostionX > positionX)
                {
                    movingPositionX += movedDistance;
                    if (movingPositionX >= targetPostionX)
                    {
                        xReady = true;
                        movingPositionX = targetPostionX;
                    }
                }
                else
                {
                    movingPositionX -= movedDistance;
                    if (movingPositionX <= targetPostionX)
                    {
                        xReady = true;
                        movingPositionX = targetPostionX;
                    }
                }
            }
            if (!yReady)
            {
                if (targetPostionY > positionY)
                {
                    movingPositionY += movedDistance;
                    if (movingPositionY >= targetPostionY)
                    {
                        yReady = true;
                        movingPositionY = targetPostionY;
                    }
                }
                else
                {
                    movingPositionY -= movedDistance;
                    if (movingPositionY <= targetPostionY)
                    {
                        yReady = true;
                        movingPositionY = targetPostionY;
                    }
                }
            }
            if (xReady && yReady)
            {
                positionX = movingPositionX;
                positionY = movingPositionY;
                moving = false;
            }
        }

        public int GetValidPositionX()
        {
            if (moving)
            {
                return movingPositionX;
            }
            else
            {
                return positionX;
            }
        }

        public int GetValidPositionY()
        {
            if (moving)
            {
                return movingPositionY;
            }
            else
            {
                return positionY;
            }
        }
        #endregion
    }
}