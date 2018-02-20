using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    public class BattleUnit : DynamicUnit
    {
        public BattleUnit() : base()
        {

        }

        #region declaration

        #endregion

        #region business
        /// <summary>
        /// 
        /// </summary>        
        /// <param name="pmMoveCoordinateSpeed">coordinate gap per second</param>        
        public void MoveUp(float pmMoveCoordinateSpeed)
        {
            if (moving)
            {
                return;
            }
            this.actGroupIndex = 0;
            this.movingCoordinateX = this.coordinateX;
            this.movingCoordinateY = this.coordinateY;
            this.targetCoordinateX = coordinateX;
            this.targetCoordinateY = coordinateY - 1;
            moveCoordinateSpeed = pmMoveCoordinateSpeed;            
            this.moving = true;
        }

        /// <summary>
        /// 
        /// </summary>        
        /// <param name="pmMoveCoordinateSpeed">coordinate gap per second</param>        
        public void MoveRight(float pmMoveCoordinateSpeed)
        {
            if (moving)
            {
                return;
            }
            this.actGroupIndex = 1;
            this.movingCoordinateX = this.coordinateX;
            this.movingCoordinateY = this.coordinateY;
            this.targetCoordinateX = coordinateX + 1;
            this.targetCoordinateY = coordinateY;
            moveCoordinateSpeed = pmMoveCoordinateSpeed;
            this.moving = true;
        }

        /// <summary>
        /// 
        /// </summary>        
        /// <param name="pmMoveCoordinateSpeed">coordinate gap per second</param>        
        public void MoveLeft(float pmMoveCoordinateSpeed)
        {
            if (moving)
            {
                return;
            }
            this.actGroupIndex = 2;
            this.movingCoordinateX = this.coordinateX;
            this.movingCoordinateY = this.coordinateY;
            this.targetCoordinateX = coordinateX - 1;
            this.targetCoordinateY = coordinateY;
            moveCoordinateSpeed = pmMoveCoordinateSpeed;
            this.moving = true;
        }

        /// <summary>
        /// 
        /// </summary>        
        /// <param name="pmMoveCoordinateSpeed">coordinate gap per second</param>        
        public void MoveDown(float pmMoveCoordinateSpeed)
        {
            if (moving)
            {
                return;
            }
            this.actGroupIndex = 3;
            this.movingCoordinateX = this.coordinateX;
            this.movingCoordinateY = this.coordinateY;
            this.targetCoordinateX = coordinateX;
            this.targetCoordinateY = coordinateY + 1;
            moveCoordinateSpeed = pmMoveCoordinateSpeed;
            this.moving = true;
        }

        public new void FaceUp()
        {
            this.actFrameIndex = 0;
            this.actGroupIndex = 0;
        }

        public new void FaceRight()
        {
            this.actFrameIndex = 0;
            this.actGroupIndex = 1;
        }

        public new void FaceLeft()
        {
            this.actFrameIndex = 0;
            this.actGroupIndex = 2;
        }

        public new void FaceDown()
        {
            this.actFrameIndex = 0;
            this.actGroupIndex = 3;
        }

        public new void UpdateMoving(int pmElapsed)
        {            
            if (!moving)
            {
                return;
            }
            float movedCoordinateDistance = moveCoordinateSpeed * pmElapsed / 1000;
            bool xReady = false, yReady = false;
            float newMovingCoordinateX = movingCoordinateX, newMovingCoordinateY = movingCoordinateY;
            if (!xReady)
            {
                if (targetCoordinateX < coordinateX)
                {
                    if (movingCoordinateX <= targetCoordinateX)
                    {
                        xReady = true;
                    }
                    else
                    {
                        newMovingCoordinateX = movingCoordinateX - movedCoordinateDistance;
                    }
                }
                else
                {
                    if (movingCoordinateX >= targetCoordinateX)
                    {
                        xReady = true;
                    }
                    else
                    {
                        newMovingCoordinateX = movingCoordinateX + movedCoordinateDistance;
                    }
                }
            }
            if (!yReady)
            {
                if (targetCoordinateY < coordinateY)
                {
                    if (movingCoordinateY <= targetCoordinateY)
                    {
                        yReady = true;
                    }
                    else
                    {
                        newMovingCoordinateY = movingCoordinateY - movedCoordinateDistance;
                    }
                }
                else
                {
                    if (movingCoordinateY >= targetCoordinateY)
                    {
                        yReady = true;
                    }
                    else
                    {
                        newMovingCoordinateY = movingCoordinateY + movedCoordinateDistance;
                    }
                }
            }
            SetMovingCoordinate(newMovingCoordinateX, newMovingCoordinateY);
            if (xReady && yReady)
            {
                SetMovingCoordinate(targetCoordinateX, targetCoordinateY);
                SetFixedCoordinate(targetCoordinateX, targetCoordinateY);
                moving = false;
            }
        }
        #endregion
    }
}