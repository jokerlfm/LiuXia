using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    public class DynamicUnit : MapUnit
    {
        public DynamicUnit() : base(UnitType.UnitType_Dynamic)
        {
            this.textureIDDictionary = new Dictionary<int, List<int>>();
        }

        #region declaration
        public Dictionary<int, List<int>> textureIDDictionary;
        public int actGroupIndex = 0;
        public int actFrameIndex = 0;
        public bool actRepeating = false;
        public int actFrameElapsed = 0;
        public int frameSpeed = 0;

        public float movingCoordinateX = 0;
        public float movingCoordinateY = 0;

        public float moveCoordinateSpeed = 0;

        public bool moving = false;

        public int targetCoordinateX = 0;
        public int targetCoordinateY = 0;

        public bool acting = false;
        #endregion

        #region business
        /// <summary>
        /// 
        /// </summary>        
        /// <param name="pmMoveCoordinateSpeed">coordinate gap per second</param>
        /// <param name="pmMoveFrameSpeed">frame stay time (ms)</param>
        public void MoveUp(float pmMoveCoordinateSpeed, int pmFrameSpeed)
        {
            this.actGroupIndex = 0;
            this.movingCoordinateX = this.coordinateX;
            this.movingCoordinateY = this.coordinateY;
            this.targetCoordinateX = coordinateX;
            this.targetCoordinateY = coordinateY - 1;
            moveCoordinateSpeed = pmMoveCoordinateSpeed;
            frameSpeed = pmFrameSpeed;
            actRepeating = true;
            this.moving = true;
        }

        /// <summary>
        /// 
        /// </summary>        
        /// <param name="pmMoveCoordinateSpeed">coordinate gap per second</param>
        /// <param name="pmMoveFrameSpeed">frame stay time (ms)</param>
        public void MoveRight(float pmMoveCoordinateSpeed, int pmFrameSpeed)
        {
            this.actGroupIndex = 1;
            this.movingCoordinateX = this.coordinateX;
            this.movingCoordinateY = this.coordinateY;
            this.targetCoordinateX = coordinateX + 1;
            this.targetCoordinateY = coordinateY;
            moveCoordinateSpeed = pmMoveCoordinateSpeed;
            frameSpeed = pmFrameSpeed;
            actRepeating = true;
            this.moving = true;
        }

        /// <summary>
        /// 
        /// </summary>        
        /// <param name="pmMoveCoordinateSpeed">coordinate gap per second</param>
        /// <param name="pmMoveFrameSpeed">frame stay time (ms)</param>
        public void MoveLeft(float pmMoveCoordinateSpeed, int pmFrameSpeed)
        {
            this.actGroupIndex = 2;
            this.movingCoordinateX = this.coordinateX;
            this.movingCoordinateY = this.coordinateY;
            this.targetCoordinateX = coordinateX - 1;
            this.targetCoordinateY = coordinateY;
            moveCoordinateSpeed = pmMoveCoordinateSpeed;
            frameSpeed = pmFrameSpeed;
            actRepeating = true;
            this.moving = true;
        }

        /// <summary>
        /// 
        /// </summary>        
        /// <param name="pmMoveCoordinateSpeed">coordinate gap per second</param>
        /// <param name="pmMoveFrameSpeed">frame stay time (ms)</param>
        public void MoveDown(float pmMoveCoordinateSpeed, int pmFrameSpeed)
        {
            this.actGroupIndex = 3;
            this.movingCoordinateX = this.coordinateX;
            this.movingCoordinateY = this.coordinateY;
            this.targetCoordinateX = coordinateX;
            this.targetCoordinateY = coordinateY + 1;
            moveCoordinateSpeed = pmMoveCoordinateSpeed;
            frameSpeed = pmFrameSpeed;
            actRepeating = true;
            this.moving = true;
        }

        public void FaceUp()
        {
            this.actFrameIndex = 0;
            this.actGroupIndex = 4;
        }

        public void FaceRight()
        {
            this.actFrameIndex = 0;
            this.actGroupIndex = 5;
        }

        public void FaceLeft()
        {
            this.actFrameIndex = 0;
            this.actGroupIndex = 6;
        }

        public void FaceDown()
        {
            this.actFrameIndex = 0;
            this.actGroupIndex = 7;
        }

        public void UpdateMoving(int pmElapsed)
        {
            actFrameElapsed += pmElapsed;
            if (actFrameElapsed >= frameSpeed)
            {
                if (actFrameIndex + 1 >= textureIDDictionary[actGroupIndex].Count)
                {
                    if (actRepeating)
                    {
                        actFrameIndex = 0;
                    }
                }
                else
                {
                    actFrameIndex += 1;
                }
                actFrameElapsed = 0;
            }
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

        public void SetMovingCoordinate(float pmX, float pmY)
        {
            this.movingCoordinateX = pmX;
            this.movingCoordinateY = pmY;

            //this.screenBasePositionX = this.coordinateX * 40;
            //this.screenBasePositionY = this.coordinateY * 40;

            this.screenBasePositionX = this.movingCoordinateX + (this.movingCoordinateX * ConstManager.BaseMapCoordinateSize) - (this.movingCoordinateY * ConstManager.BaseMapCoordinateSize);
            this.screenBasePositionY = this.movingCoordinateY + (this.movingCoordinateX * ConstManager.BaseMapCoordinateSize / 2) + (this.movingCoordinateY * ConstManager.BaseMapCoordinateSize / 2);
        }
        #endregion
    }
}