using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GameCore
{
    public class GameService
    {
        private GameService(string pmGameName, System.Windows.Forms.Form pmTargetForm = null)
        {
            this.gameName = pmGameName;
            displayTargetForm = pmTargetForm;
        }

        #region declaration
        private static GameService singletonGame;

        public string gameName = "";
        public System.Windows.Forms.Form displayTargetForm;
        public RuntimeState runtimeState = RuntimeState.RuntimeState_World;
        public GameRunningState state = GameRunningState.GameRunningState_Stopped;

        public MediaCore.DrawOperator pen;
        public MediaCore.InputOperator controller;

        public CameraUnit mainCamera;
        public Player player;
        #endregion

        #region business
        public static GameService CreateGame(string pmGameName, System.Windows.Forms.Form pmTargetForm = null)
        {
            if (singletonGame == null)
            {
                singletonGame = new GameService(pmGameName, pmTargetForm);
            }
            return singletonGame;
        }

        public static GameService GetGame()
        {
            return singletonGame;
        }

        public void Run()
        {
            if (state == GameRunningState.GameRunningState_Stopped)
            {
                if (!GameOperator.InitGame())
                {
                    state = GameRunningState.GameRunningState_Stopped;
                    return;
                }

                if (displayTargetForm == null)
                {
                    displayTargetForm = new System.Windows.Forms.Form();
                }
                displayTargetForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                displayTargetForm.Width = ConfigHandler.GetConfigValue_int("windowwidth");
                displayTargetForm.Height = ConfigHandler.GetConfigValue_int("windowheight");
                displayTargetForm.Show();

                pen = new MediaCore.DrawOperator(displayTargetForm.Handle, displayTargetForm.Width, displayTargetForm.Height, ConfigHandler.GetConfigValue_float("zoom"));
                controller = new MediaCore.InputOperator(displayTargetForm);

                //GameRunning();

                WaitCallback wcbGameRunning = new WaitCallback(this.GameRunning);
                ThreadPool.QueueUserWorkItem(wcbGameRunning, null);
            }
        }

        public void ForceShutdown()
        {
            state = GameRunningState.GameRunningState_Stopped;
        }

        private void GameRunning(object pmMain = null)
        {
            state = GameRunningState.GameRunningState_Running;

            //ProcedureHandler.ShowWelcome();
            GameOperator.LoadGame(0);

            DateTime dtPrevUpdateTime = DateTime.Now;
            while (state == GameRunningState.GameRunningState_Running)
            {
                int timeElapsed = (int)(DateTime.Now - dtPrevUpdateTime).TotalMilliseconds;

                this.HandleInput();

                this.UpdatePlayer(timeElapsed);
                this.UpdateCamera(timeElapsed);

                HandleDrawing();

                dtPrevUpdateTime = DateTime.Now;
                Thread.Sleep(10);
            }

            state = GameRunningState.GameRunningState_Finished;
        }

        private void HandleInput()
        {
            string inputName = controller.GetInputName();
            if (inputName != "")
            {
                if (inputName == "Escape")
                {
                    ForceShutdown();
                    return;
                }
                switch (runtimeState)
                {
                    case RuntimeState.RuntimeState_World:
                        {
                            switch (inputName)
                            {
                                case "E":
                                    {
                                        mainCamera.MoveTo(mainCamera.GetValidPositionX(), mainCamera.GetValidPositionY() - 100, 100);
                                        break;
                                    }
                                case "D":
                                    {
                                        mainCamera.MoveTo(mainCamera.GetValidPositionX(), mainCamera.GetValidPositionY() + 100, 100);
                                        break;
                                    }
                                case "S":
                                    {
                                        mainCamera.MoveTo(mainCamera.GetValidPositionX() - 100, mainCamera.GetValidPositionY(), 100);
                                        break;
                                    }
                                case "F":
                                    {
                                        mainCamera.MoveTo(mainCamera.GetValidPositionX() + 100, mainCamera.GetValidPositionY(), 100);
                                        break;
                                    }
                                case "UpArrow":
                                    {
                                        if (ResourceManager.mainMap.Movable(player.playerWorldUnit.coordinateX, player.playerWorldUnit.coordinateY - 1))
                                        {
                                            player.playerWorldUnit.MoveUp(ConfigHandler.GetConfigValue_int("movespeed"), ConfigHandler.GetConfigValue_int("actspeed"));
                                        }
                                        else
                                        {
                                            player.playerWorldUnit.FaceUp();
                                        }
                                        break;
                                    }
                                case "DownArrow":
                                    {
                                        if (ResourceManager.mainMap.Movable(player.playerWorldUnit.coordinateX, player.playerWorldUnit.coordinateY + 1))
                                        {
                                            player.playerWorldUnit.MoveDown(ConfigHandler.GetConfigValue_int("movespeed"), ConfigHandler.GetConfigValue_int("actspeed"));
                                        }
                                        else
                                        {
                                            player.playerWorldUnit.FaceDown();
                                        }
                                        break;
                                    }
                                case "LeftArrow":
                                    {
                                        if (ResourceManager.mainMap.Movable(player.playerWorldUnit.coordinateX - 1, player.playerWorldUnit.coordinateY))
                                        {
                                            player.playerWorldUnit.MoveLeft(ConfigHandler.GetConfigValue_int("movespeed"), ConfigHandler.GetConfigValue_int("actspeed"));
                                        }
                                        else
                                        {
                                            player.playerWorldUnit.FaceLeft();
                                        }
                                        break;
                                    }
                                case "RightArrow":
                                    {
                                        if (ResourceManager.mainMap.Movable(player.playerWorldUnit.coordinateX + 1, player.playerWorldUnit.coordinateY))
                                        {
                                            player.playerWorldUnit.MoveRight(ConfigHandler.GetConfigValue_int("movespeed"), ConfigHandler.GetConfigValue_int("actspeed"));
                                        }
                                        else
                                        {
                                            player.playerWorldUnit.FaceRight();
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        break;
                                    }
                            }
                            break;
                        }
                    case RuntimeState.RuntimeState_Scene:
                        {
                            switch (inputName)
                            {
                                case "E":
                                    {
                                        mainCamera.MoveTo(mainCamera.GetValidPositionX(), mainCamera.GetValidPositionY() - 100, 100);
                                        break;
                                    }
                                case "D":
                                    {
                                        mainCamera.MoveTo(mainCamera.GetValidPositionX(), mainCamera.GetValidPositionY() + 100, 100);
                                        break;
                                    }
                                case "S":
                                    {
                                        mainCamera.MoveTo(mainCamera.GetValidPositionX() - 100, mainCamera.GetValidPositionY(), 100);
                                        break;
                                    }
                                case "F":
                                    {
                                        mainCamera.MoveTo(mainCamera.GetValidPositionX() + 100, mainCamera.GetValidPositionY(), 100);
                                        break;
                                    }
                                case "UpArrow":
                                    {
                                        if (ResourceManager.sceneMapDictionary[player.smapID].Movable(player.playerSceneUnit.coordinateX, player.playerSceneUnit.coordinateY - 1))
                                        {
                                            player.playerSceneUnit.MoveUp(ConfigHandler.GetConfigValue_int("movespeed"), ConfigHandler.GetConfigValue_int("actspeed"));
                                        }
                                        else
                                        {
                                            player.playerSceneUnit.FaceUp();
                                        }
                                        break;
                                    }
                                case "DownArrow":
                                    {
                                        if (ResourceManager.sceneMapDictionary[player.smapID].Movable(player.playerSceneUnit.coordinateX, player.playerSceneUnit.coordinateY + 1))
                                        {
                                            player.playerSceneUnit.MoveDown(ConfigHandler.GetConfigValue_int("movespeed"), ConfigHandler.GetConfigValue_int("actspeed"));
                                        }
                                        else
                                        {
                                            player.playerSceneUnit.FaceDown();
                                        }
                                        break;
                                    }
                                case "LeftArrow":
                                    {
                                        if (ResourceManager.sceneMapDictionary[player.smapID].Movable(player.playerSceneUnit.coordinateX - 1, player.playerSceneUnit.coordinateY))
                                        {
                                            player.playerSceneUnit.MoveLeft(ConfigHandler.GetConfigValue_int("movespeed"), ConfigHandler.GetConfigValue_int("actspeed"));
                                        }
                                        else
                                        {
                                            player.playerSceneUnit.FaceLeft();
                                        }
                                        break;
                                    }
                                case "RightArrow":
                                    {
                                        if (ResourceManager.sceneMapDictionary[player.smapID].Movable(player.playerSceneUnit.coordinateX + 1, player.playerSceneUnit.coordinateY))
                                        {
                                            player.playerSceneUnit.MoveRight(ConfigHandler.GetConfigValue_int("movespeed"), ConfigHandler.GetConfigValue_int("actspeed"));
                                        }
                                        else
                                        {
                                            player.playerSceneUnit.FaceRight();
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        break;
                                    }
                            }
                            break;
                        }
                    case RuntimeState.RuntimeState_Battle:
                        {
                            switch (inputName)
                            {
                                case "E":
                                    {
                                        mainCamera.MoveTo(mainCamera.GetValidPositionX(), mainCamera.GetValidPositionY() - 100, 100);
                                        break;
                                    }
                                case "D":
                                    {
                                        mainCamera.MoveTo(mainCamera.GetValidPositionX(), mainCamera.GetValidPositionY() + 100, 100);
                                        break;
                                    }
                                case "S":
                                    {
                                        mainCamera.MoveTo(mainCamera.GetValidPositionX() - 100, mainCamera.GetValidPositionY(), 100);
                                        break;
                                    }
                                case "F":
                                    {
                                        mainCamera.MoveTo(mainCamera.GetValidPositionX() + 100, mainCamera.GetValidPositionY(), 100);
                                        break;
                                    }
                                case "UpArrow":
                                    {
                                        if (ResourceManager.battleMapDictionary[player.smapID].Movable(player.playerBattleUnit.coordinateX, player.playerBattleUnit.coordinateY - 1))
                                        {
                                            player.playerBattleUnit.MoveUp(ConfigHandler.GetConfigValue_int("movespeed"));
                                        }
                                        else
                                        {
                                            player.playerBattleUnit.FaceUp();
                                        }
                                        break;
                                    }
                                case "DownArrow":
                                    {
                                        if (ResourceManager.battleMapDictionary[player.smapID].Movable(player.playerBattleUnit.coordinateX, player.playerBattleUnit.coordinateY + 1))
                                        {
                                            player.playerBattleUnit.MoveDown(ConfigHandler.GetConfigValue_int("movespeed"));
                                        }
                                        else
                                        {
                                            player.playerBattleUnit.FaceDown();
                                        }
                                        break;
                                    }
                                case "LeftArrow":
                                    {
                                        if (ResourceManager.battleMapDictionary[player.smapID].Movable(player.playerBattleUnit.coordinateX - 1, player.playerBattleUnit.coordinateY))
                                        {
                                            player.playerBattleUnit.MoveLeft(ConfigHandler.GetConfigValue_int("movespeed"));
                                        }
                                        else
                                        {
                                            player.playerBattleUnit.FaceLeft();
                                        }
                                        break;
                                    }
                                case "RightArrow":
                                    {
                                        if (ResourceManager.battleMapDictionary[player.smapID].Movable(player.playerBattleUnit.coordinateX + 1, player.playerBattleUnit.coordinateY))
                                        {
                                            player.playerBattleUnit.MoveRight(ConfigHandler.GetConfigValue_int("movespeed"));
                                        }
                                        else
                                        {
                                            player.playerBattleUnit.FaceRight();
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        break;
                                    }
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
        }

        private void UpdatePlayer(int pmTimeElapsed)
        {
            switch (player.state)
            {
                case PlayerState.PlayerState_World:
                    {
                        if (player.playerWorldUnit.moving)
                        {
                            player.playerWorldUnit.UpdateMoving(pmTimeElapsed);
                            if (mainCamera.bondToPlayer)
                            {
                                mainCamera.ResetCamera((int)(player.playerWorldUnit.screenBasePositionX - displayTargetForm.Width / ConfigHandler.GetConfigValue_float("zoom") / 2), (int)(player.playerWorldUnit.screenBasePositionY - displayTargetForm.Height / ConfigHandler.GetConfigValue_float("zoom") / 2));
                            }
                        }
                        break;
                    }
                case PlayerState.PlayerState_Scene:
                    {
                        if (player.playerSceneUnit.moving)
                        {
                            player.playerSceneUnit.UpdateMoving(pmTimeElapsed);
                            if (mainCamera.bondToPlayer)
                            {
                                mainCamera.ResetCamera((int)(player.playerSceneUnit.screenBasePositionX - displayTargetForm.Width / ConfigHandler.GetConfigValue_float("zoom") / 2), (int)(player.playerSceneUnit.screenBasePositionY - displayTargetForm.Height / ConfigHandler.GetConfigValue_float("zoom") / 2));
                            }
                        }
                        break;
                    }
                case PlayerState.PlayerState_Battle:
                    {
                        if (player.playerBattleUnit.moving)
                        {
                            player.playerBattleUnit.UpdateMoving(pmTimeElapsed);
                            mainCamera.ResetCamera((int)(player.playerBattleUnit.screenBasePositionX - displayTargetForm.Width / ConfigHandler.GetConfigValue_float("zoom") / 2), (int)(player.playerBattleUnit.screenBasePositionY - displayTargetForm.Height / ConfigHandler.GetConfigValue_float("zoom") / 2));
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private void UpdateCamera(int pmTimeElapsed)
        {
            if (mainCamera.moving)
            {
                mainCamera.UpdateMoving(pmTimeElapsed);
            }
        }

        private void HandleDrawing()
        {
            pen.BeginDraw();
            switch (runtimeState)
            {
                case RuntimeState.RuntimeState_World:
                    {
                        DrawMmap();
                        break;
                    }
                case RuntimeState.RuntimeState_Scene:
                    {
                        DrawSmap();
                        break;
                    }
                case RuntimeState.RuntimeState_Battle:
                    {
                        DrawWmap();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            pen.EndDraw();
        }

        private void DrawMmap()
        {
            int minX = player.playerWorldUnit.coordinateX - 30, maxX = player.playerWorldUnit.coordinateX + 30;
            int minY = player.playerWorldUnit.coordinateY - 30, maxY = player.playerWorldUnit.coordinateY + 30;
            if (minX < 0)
            {
                minX = 0;
            }
            if (maxX > 479)
            {
                maxX = 479;
            }
            if (minY < 0)
            {
                minY = 0;
            }
            if (maxY > 479)
            {
                maxY = 479;
            }
            for (int yCount = minY; yCount <= maxY; yCount++)
            {
                for (int xCount = minX; xCount <= maxX; xCount++)
                {
                    if (ResourceManager.mainMap.earthLayerMatrix[xCount, yCount] != null)
                    {
                        if (ResourceManager.mmapTextureStore[ResourceManager.mainMap.earthLayerMatrix[xCount, yCount].textureID].textureD3D9 == null)
                        {
                            ResourceManager.mmapTextureStore[ResourceManager.mainMap.earthLayerMatrix[xCount, yCount].textureID].textureD3D9 = pen.CreateTexture(
                                ResourceManager.mmapTextureStore[ResourceManager.mainMap.earthLayerMatrix[xCount, yCount].textureID].textureBytes,
                                ResourceManager.mmapTextureStore[ResourceManager.mainMap.earthLayerMatrix[xCount, yCount].textureID].textureWidth,
                                ResourceManager.mmapTextureStore[ResourceManager.mainMap.earthLayerMatrix[xCount, yCount].textureID].textureHeight);
                        }
                        pen.DrawTexture(ResourceManager.mmapTextureStore[ResourceManager.mainMap.earthLayerMatrix[xCount, yCount].textureID].textureD3D9,
                            this.mainCamera.GetValidPositionX(), this.mainCamera.GetValidPositionY(),
                            ResourceManager.mainMap.earthLayerMatrix[xCount, yCount].screenBasePositionX - ResourceManager.mmapTextureStore[ResourceManager.mainMap.earthLayerMatrix[xCount, yCount].textureID].textureGapX,
                            ResourceManager.mainMap.earthLayerMatrix[xCount, yCount].screenBasePositionY - ResourceManager.mmapTextureStore[ResourceManager.mainMap.earthLayerMatrix[xCount, yCount].textureID].textureGapY);
                    }

                    if (ResourceManager.mainMap.surfaceLayerMatrix[xCount, yCount] != null)
                    {
                        if (ResourceManager.mmapTextureStore[ResourceManager.mainMap.surfaceLayerMatrix[xCount, yCount].textureID].textureD3D9 == null)
                        {
                            ResourceManager.mmapTextureStore[ResourceManager.mainMap.surfaceLayerMatrix[xCount, yCount].textureID].textureD3D9 = pen.CreateTexture(
                                ResourceManager.mmapTextureStore[ResourceManager.mainMap.surfaceLayerMatrix[xCount, yCount].textureID].textureBytes,
                                ResourceManager.mmapTextureStore[ResourceManager.mainMap.surfaceLayerMatrix[xCount, yCount].textureID].textureWidth,
                                ResourceManager.mmapTextureStore[ResourceManager.mainMap.surfaceLayerMatrix[xCount, yCount].textureID].textureHeight);
                        }
                        pen.DrawTexture(ResourceManager.mmapTextureStore[ResourceManager.mainMap.surfaceLayerMatrix[xCount, yCount].textureID].textureD3D9,
                            this.mainCamera.GetValidPositionX(), this.mainCamera.GetValidPositionY(),
                            ResourceManager.mainMap.surfaceLayerMatrix[xCount, yCount].screenBasePositionX - ResourceManager.mmapTextureStore[ResourceManager.mainMap.surfaceLayerMatrix[xCount, yCount].textureID].textureGapX,
                            ResourceManager.mainMap.surfaceLayerMatrix[xCount, yCount].screenBasePositionY - ResourceManager.mmapTextureStore[ResourceManager.mainMap.surfaceLayerMatrix[xCount, yCount].textureID].textureGapY);
                    }
                }
            }
            // clear drawed
            for (int xCount = minX; xCount <= maxX; xCount++)
            {
                for (int yCount = minY; yCount <= maxY; yCount++)
                {
                    if (ResourceManager.mainMap.buildingLayerMatrix[xCount, yCount] != null)
                    {
                        ResourceManager.mainMap.buildingLayerMatrix[xCount, yCount].drawed = false;
                    }
                }
            }

            for (int xCount = minX; xCount <= maxX; xCount++)
            {
                for (int yCount = minY; yCount <= maxY; yCount++)
                {
                    if (ResourceManager.mainMap.buildXYLayerMatrix[xCount, yCount] != null)
                    {
                        if (ResourceManager.mainMap.buildingLayerMatrix[ResourceManager.mainMap.buildXYLayerMatrix[xCount, yCount].buildX, ResourceManager.mainMap.buildXYLayerMatrix[xCount, yCount].buildY] != null)
                        {
                            if (!ResourceManager.mainMap.buildingLayerMatrix[ResourceManager.mainMap.buildXYLayerMatrix[xCount, yCount].buildX, ResourceManager.mainMap.buildXYLayerMatrix[xCount, yCount].buildY].drawed)
                            {
                                if (ResourceManager.mmapTextureStore[ResourceManager.mainMap.buildingLayerMatrix[ResourceManager.mainMap.buildXYLayerMatrix[xCount, yCount].buildX, ResourceManager.mainMap.buildXYLayerMatrix[xCount, yCount].buildY].textureID].textureD3D9 == null)
                                {
                                    ResourceManager.mmapTextureStore[ResourceManager.mainMap.buildingLayerMatrix[ResourceManager.mainMap.buildXYLayerMatrix[xCount, yCount].buildX, ResourceManager.mainMap.buildXYLayerMatrix[xCount, yCount].buildY].textureID].textureD3D9 = pen.CreateTexture(
                                        ResourceManager.mmapTextureStore[ResourceManager.mainMap.buildingLayerMatrix[ResourceManager.mainMap.buildXYLayerMatrix[xCount, yCount].buildX, ResourceManager.mainMap.buildXYLayerMatrix[xCount, yCount].buildY].textureID].textureBytes,
                                        ResourceManager.mmapTextureStore[ResourceManager.mainMap.buildingLayerMatrix[ResourceManager.mainMap.buildXYLayerMatrix[xCount, yCount].buildX, ResourceManager.mainMap.buildXYLayerMatrix[xCount, yCount].buildY].textureID].textureWidth,
                                        ResourceManager.mmapTextureStore[ResourceManager.mainMap.buildingLayerMatrix[ResourceManager.mainMap.buildXYLayerMatrix[xCount, yCount].buildX, ResourceManager.mainMap.buildXYLayerMatrix[xCount, yCount].buildY].textureID].textureHeight);
                                }
                                pen.DrawTexture(ResourceManager.mmapTextureStore[ResourceManager.mainMap.buildingLayerMatrix[ResourceManager.mainMap.buildXYLayerMatrix[xCount, yCount].buildX, ResourceManager.mainMap.buildXYLayerMatrix[xCount, yCount].buildY].textureID].textureD3D9,
                                    this.mainCamera.GetValidPositionX(), this.mainCamera.GetValidPositionY(),
                                    ResourceManager.mainMap.buildingLayerMatrix[ResourceManager.mainMap.buildXYLayerMatrix[xCount, yCount].buildX, ResourceManager.mainMap.buildXYLayerMatrix[xCount, yCount].buildY].screenBasePositionX - ResourceManager.mmapTextureStore[ResourceManager.mainMap.buildingLayerMatrix[ResourceManager.mainMap.buildXYLayerMatrix[xCount, yCount].buildX, ResourceManager.mainMap.buildXYLayerMatrix[xCount, yCount].buildY].textureID].textureGapX,
                                    ResourceManager.mainMap.buildingLayerMatrix[ResourceManager.mainMap.buildXYLayerMatrix[xCount, yCount].buildX, ResourceManager.mainMap.buildXYLayerMatrix[xCount, yCount].buildY].screenBasePositionY - ResourceManager.mmapTextureStore[ResourceManager.mainMap.buildingLayerMatrix[ResourceManager.mainMap.buildXYLayerMatrix[xCount, yCount].buildX, ResourceManager.mainMap.buildXYLayerMatrix[xCount, yCount].buildY].textureID].textureGapY);
                                ResourceManager.mainMap.buildingLayerMatrix[ResourceManager.mainMap.buildXYLayerMatrix[xCount, yCount].buildX, ResourceManager.mainMap.buildXYLayerMatrix[xCount, yCount].buildY].drawed = true;
                            }
                        }
                    }

                    if (xCount == (int)player.playerWorldUnit.movingCoordinateX && yCount == (int)player.playerWorldUnit.movingCoordinateY)
                    {
                        if (ResourceManager.mmapTextureStore[player.playerWorldUnit.textureIDDictionary[player.playerWorldUnit.actGroupIndex][player.playerWorldUnit.actFrameIndex]].textureD3D9 == null)
                        {
                            ResourceManager.mmapTextureStore[player.playerWorldUnit.textureIDDictionary[player.playerWorldUnit.actGroupIndex][player.playerWorldUnit.actFrameIndex]].textureD3D9 = pen.CreateTexture(
                                ResourceManager.mmapTextureStore[player.playerWorldUnit.textureIDDictionary[player.playerWorldUnit.actGroupIndex][player.playerWorldUnit.actFrameIndex]].textureBytes,
                                ResourceManager.mmapTextureStore[player.playerWorldUnit.textureIDDictionary[player.playerWorldUnit.actGroupIndex][player.playerWorldUnit.actFrameIndex]].textureWidth,
                                ResourceManager.mmapTextureStore[player.playerWorldUnit.textureIDDictionary[player.playerWorldUnit.actGroupIndex][player.playerWorldUnit.actFrameIndex]].textureHeight);
                        }
                        pen.DrawTexture(ResourceManager.mmapTextureStore[player.playerWorldUnit.textureIDDictionary[player.playerWorldUnit.actGroupIndex][player.playerWorldUnit.actFrameIndex]].textureD3D9,
                                this.mainCamera.GetValidPositionX(), this.mainCamera.GetValidPositionY(),
                                player.playerWorldUnit.screenBasePositionX - ResourceManager.mmapTextureStore[player.playerWorldUnit.textureIDDictionary[player.playerWorldUnit.actGroupIndex][player.playerWorldUnit.actFrameIndex]].textureGapX,
                                player.playerWorldUnit.screenBasePositionY - ResourceManager.mmapTextureStore[player.playerWorldUnit.textureIDDictionary[player.playerWorldUnit.actGroupIndex][player.playerWorldUnit.actFrameIndex]].textureGapY);
                    }

                    //if (ResourceManager.mainMap.buildingLayerMatrix[xCount, yCount] != null)
                    //{
                    //    if (ResourceManager.mmapTextureStore[ResourceManager.mainMap.buildingLayerMatrix[xCount, yCount].textureID].textureD3D9 == null)
                    //    {
                    //        ResourceManager.mmapTextureStore[ResourceManager.mainMap.buildingLayerMatrix[xCount, yCount].textureID].textureD3D9 = pen.CreateTexture(
                    //            ResourceManager.mmapTextureStore[ResourceManager.mainMap.buildingLayerMatrix[xCount, yCount].textureID].textureBytes,
                    //            ResourceManager.mmapTextureStore[ResourceManager.mainMap.buildingLayerMatrix[xCount, yCount].textureID].textureWidth,
                    //            ResourceManager.mmapTextureStore[ResourceManager.mainMap.buildingLayerMatrix[xCount, yCount].textureID].textureHeight);
                    //    }
                    //    pen.DrawTexture(ResourceManager.mmapTextureStore[ResourceManager.mainMap.buildingLayerMatrix[xCount, yCount].textureID].textureD3D9,
                    //        this.mainCamera.GetValidPositionX(), this.mainCamera.GetValidPositionY(),
                    //        ResourceManager.mainMap.buildingLayerMatrix[xCount, yCount].screenBasePositionX - ResourceManager.mmapTextureStore[ResourceManager.mainMap.buildingLayerMatrix[xCount, yCount].textureID].textureGapX,
                    //        ResourceManager.mainMap.buildingLayerMatrix[xCount, yCount].screenBasePositionY - ResourceManager.mmapTextureStore[ResourceManager.mainMap.buildingLayerMatrix[xCount, yCount].textureID].textureGapY);
                    //}
                }
            }
        }

        private void DrawSmap()
        {
            for (int yCount = 0; yCount <= 63; yCount++)
            {
                for (int xCount = 0; xCount <= 63; xCount++)
                {
                    if (ResourceManager.sceneMapDictionary[0].floorLayerMatrix[xCount, yCount] != null)
                    {

                        if (ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].floorLayerMatrix[xCount, yCount].textureID].textureD3D9 == null)
                        {
                            ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].floorLayerMatrix[xCount, yCount].textureID].textureD3D9 = pen.CreateTexture(
                                ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].floorLayerMatrix[xCount, yCount].textureID].textureBytes,
                                ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].floorLayerMatrix[xCount, yCount].textureID].textureWidth,
                                ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].floorLayerMatrix[xCount, yCount].textureID].textureHeight);
                        }
                        pen.DrawTexture(ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].floorLayerMatrix[xCount, yCount].textureID].textureD3D9,
                            this.mainCamera.GetValidPositionX(), this.mainCamera.GetValidPositionY(),
                            ResourceManager.sceneMapDictionary[0].floorLayerMatrix[xCount, yCount].screenBasePositionX - ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].floorLayerMatrix[xCount, yCount].textureID].textureGapX,
                            ResourceManager.sceneMapDictionary[0].floorLayerMatrix[xCount, yCount].screenBasePositionY - ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].floorLayerMatrix[xCount, yCount].textureID].textureGapY);
                    }
                }
            }

            for (int yCount = 0; yCount <= 63; yCount++)
            {
                for (int xCount = 0; xCount <= 63; xCount++)
                {
                    if (xCount == (int)player.playerSceneUnit.movingCoordinateX && yCount == (int)player.playerSceneUnit.movingCoordinateY)
                    {
                        if (ResourceManager.smapTextureStore[player.playerSceneUnit.textureIDDictionary[player.playerSceneUnit.actGroupIndex][player.playerSceneUnit.actFrameIndex]].textureD3D9 == null)
                        {
                            ResourceManager.smapTextureStore[player.playerSceneUnit.textureIDDictionary[player.playerSceneUnit.actGroupIndex][player.playerSceneUnit.actFrameIndex]].textureD3D9 = pen.CreateTexture(
                                ResourceManager.smapTextureStore[player.playerSceneUnit.textureIDDictionary[player.playerSceneUnit.actGroupIndex][player.playerSceneUnit.actFrameIndex]].textureBytes,
                                ResourceManager.smapTextureStore[player.playerSceneUnit.textureIDDictionary[player.playerSceneUnit.actGroupIndex][player.playerSceneUnit.actFrameIndex]].textureWidth,
                                ResourceManager.smapTextureStore[player.playerSceneUnit.textureIDDictionary[player.playerSceneUnit.actGroupIndex][player.playerSceneUnit.actFrameIndex]].textureHeight);
                        }
                        pen.DrawTexture(ResourceManager.smapTextureStore[player.playerSceneUnit.textureIDDictionary[player.playerSceneUnit.actGroupIndex][player.playerSceneUnit.actFrameIndex]].textureD3D9,
                                this.mainCamera.GetValidPositionX(), this.mainCamera.GetValidPositionY(),
                                player.playerSceneUnit.screenBasePositionX - ResourceManager.smapTextureStore[player.playerSceneUnit.textureIDDictionary[player.playerSceneUnit.actGroupIndex][player.playerSceneUnit.actFrameIndex]].textureGapX,
                                player.playerSceneUnit.screenBasePositionY - ResourceManager.smapTextureStore[player.playerSceneUnit.textureIDDictionary[player.playerSceneUnit.actGroupIndex][player.playerSceneUnit.actFrameIndex]].textureGapY);
                    }
                    if (ResourceManager.sceneMapDictionary[0].buildingLayerMatrix[xCount, yCount] != null)
                    {

                        if (ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].buildingLayerMatrix[xCount, yCount].textureID].textureD3D9 == null)
                        {
                            ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].buildingLayerMatrix[xCount, yCount].textureID].textureD3D9 = pen.CreateTexture(
                                ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].buildingLayerMatrix[xCount, yCount].textureID].textureBytes,
                                ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].buildingLayerMatrix[xCount, yCount].textureID].textureWidth,
                                ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].buildingLayerMatrix[xCount, yCount].textureID].textureHeight);
                        }
                        pen.DrawTexture(ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].buildingLayerMatrix[xCount, yCount].textureID].textureD3D9,
                            this.mainCamera.GetValidPositionX(), this.mainCamera.GetValidPositionY(),
                            ResourceManager.sceneMapDictionary[0].buildingLayerMatrix[xCount, yCount].screenBasePositionX - ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].buildingLayerMatrix[xCount, yCount].textureID].textureGapX,
                            ResourceManager.sceneMapDictionary[0].buildingLayerMatrix[xCount, yCount].screenBasePositionY - ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].buildingLayerMatrix[xCount, yCount].textureID].textureGapY);
                    }

                    if (ResourceManager.sceneMapDictionary[0].hangLayerMatrix[xCount, yCount] != null)
                    {
                        if (ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].hangLayerMatrix[xCount, yCount].textureID].textureD3D9 == null)
                        {
                            ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].hangLayerMatrix[xCount, yCount].textureID].textureD3D9 = pen.CreateTexture(
                                ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].hangLayerMatrix[xCount, yCount].textureID].textureBytes,
                                ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].hangLayerMatrix[xCount, yCount].textureID].textureWidth,
                                ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].hangLayerMatrix[xCount, yCount].textureID].textureHeight);
                        }
                        pen.DrawTexture(ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].hangLayerMatrix[xCount, yCount].textureID].textureD3D9,
                            this.mainCamera.GetValidPositionX(), this.mainCamera.GetValidPositionY(),
                            ResourceManager.sceneMapDictionary[0].hangLayerMatrix[xCount, yCount].screenBasePositionX - ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].hangLayerMatrix[xCount, yCount].textureID].textureGapX,
                            ResourceManager.sceneMapDictionary[0].hangLayerMatrix[xCount, yCount].screenBasePositionY - ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].hangLayerMatrix[xCount, yCount].textureID].textureGapY);
                    }
                    if (ResourceManager.sceneMapDictionary[0].eventLayerMatrix[xCount, yCount] != null)
                    {
                        if (ResourceManager.sceneMapDictionary[0].eventLayerMatrix[xCount, yCount].startTextureID > 0)
                        {
                            if (ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].eventLayerMatrix[xCount, yCount].startTextureID].textureD3D9 == null)
                            {
                                ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].eventLayerMatrix[xCount, yCount].startTextureID].textureD3D9 = pen.CreateTexture(
                                    ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].eventLayerMatrix[xCount, yCount].startTextureID].textureBytes,
                                    ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].eventLayerMatrix[xCount, yCount].startTextureID].textureWidth,
                                    ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].eventLayerMatrix[xCount, yCount].startTextureID].textureHeight);
                            }
                            pen.DrawTexture(ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].eventLayerMatrix[xCount, yCount].startTextureID].textureD3D9,
                                this.mainCamera.GetValidPositionX(), this.mainCamera.GetValidPositionY(),
                                ResourceManager.sceneMapDictionary[0].eventLayerMatrix[xCount, yCount].screenBasePositionX - ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].eventLayerMatrix[xCount, yCount].startTextureID].textureGapX,
                                ResourceManager.sceneMapDictionary[0].eventLayerMatrix[xCount, yCount].screenBasePositionY - ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].eventLayerMatrix[xCount, yCount].startTextureID].textureGapY);
                        }
                    }
                }
            }
        }

        private void DrawWmap()
        {
            for (int yCount = 0; yCount <= 63; yCount++)
            {
                for (int xCount = 0; xCount <= 63; xCount++)
                {
                    if (ResourceManager.battleMapDictionary[0].floorLayerMatrix[xCount, yCount] != null)
                    {

                        if (ResourceManager.wmapTextureStore[ResourceManager.battleMapDictionary[0].floorLayerMatrix[xCount, yCount].textureID].textureD3D9 == null)
                        {
                            ResourceManager.wmapTextureStore[ResourceManager.battleMapDictionary[0].floorLayerMatrix[xCount, yCount].textureID].textureD3D9 = pen.CreateTexture(
                                ResourceManager.wmapTextureStore[ResourceManager.battleMapDictionary[0].floorLayerMatrix[xCount, yCount].textureID].textureBytes,
                                ResourceManager.wmapTextureStore[ResourceManager.battleMapDictionary[0].floorLayerMatrix[xCount, yCount].textureID].textureWidth,
                                ResourceManager.wmapTextureStore[ResourceManager.battleMapDictionary[0].floorLayerMatrix[xCount, yCount].textureID].textureHeight);
                        }
                        pen.DrawTexture(ResourceManager.wmapTextureStore[ResourceManager.battleMapDictionary[0].floorLayerMatrix[xCount, yCount].textureID].textureD3D9,
                            this.mainCamera.GetValidPositionX(), this.mainCamera.GetValidPositionY(),
                            ResourceManager.battleMapDictionary[0].floorLayerMatrix[xCount, yCount].screenBasePositionX - ResourceManager.wmapTextureStore[ResourceManager.battleMapDictionary[0].floorLayerMatrix[xCount, yCount].textureID].textureGapX,
                            ResourceManager.battleMapDictionary[0].floorLayerMatrix[xCount, yCount].screenBasePositionY - ResourceManager.wmapTextureStore[ResourceManager.battleMapDictionary[0].floorLayerMatrix[xCount, yCount].textureID].textureGapY);
                    }
                }
            }

            for (int yCount = 0; yCount <= 63; yCount++)
            {
                for (int xCount = 0; xCount <= 63; xCount++)
                {
                    if (xCount == (int)player.playerBattleUnit.movingCoordinateX && yCount == (int)player.playerBattleUnit.movingCoordinateY)
                    {
                        if (ResourceManager.wmapTextureStore[player.playerBattleUnit.textureIDDictionary[player.playerBattleUnit.actGroupIndex][player.playerBattleUnit.actFrameIndex]].textureD3D9 == null)
                        {
                            ResourceManager.wmapTextureStore[player.playerBattleUnit.textureIDDictionary[player.playerBattleUnit.actGroupIndex][player.playerBattleUnit.actFrameIndex]].textureD3D9 = pen.CreateTexture(
                                ResourceManager.wmapTextureStore[player.playerBattleUnit.textureIDDictionary[player.playerBattleUnit.actGroupIndex][player.playerBattleUnit.actFrameIndex]].textureBytes,
                                ResourceManager.wmapTextureStore[player.playerBattleUnit.textureIDDictionary[player.playerBattleUnit.actGroupIndex][player.playerBattleUnit.actFrameIndex]].textureWidth,
                                ResourceManager.wmapTextureStore[player.playerBattleUnit.textureIDDictionary[player.playerBattleUnit.actGroupIndex][player.playerBattleUnit.actFrameIndex]].textureHeight);
                        }
                        pen.DrawTexture(ResourceManager.wmapTextureStore[player.playerBattleUnit.textureIDDictionary[player.playerBattleUnit.actGroupIndex][player.playerBattleUnit.actFrameIndex]].textureD3D9,
                                this.mainCamera.GetValidPositionX(), this.mainCamera.GetValidPositionY(),
                                player.playerBattleUnit.screenBasePositionX - ResourceManager.wmapTextureStore[player.playerBattleUnit.textureIDDictionary[player.playerBattleUnit.actGroupIndex][player.playerBattleUnit.actFrameIndex]].textureGapX,
                                player.playerBattleUnit.screenBasePositionY - ResourceManager.wmapTextureStore[player.playerBattleUnit.textureIDDictionary[player.playerBattleUnit.actGroupIndex][player.playerBattleUnit.actFrameIndex]].textureGapY);
                    }
                    if (ResourceManager.battleMapDictionary[0].buildingLayerMatrix[xCount, yCount] != null)
                    {

                        if (ResourceManager.wmapTextureStore[ResourceManager.battleMapDictionary[0].buildingLayerMatrix[xCount, yCount].textureID].textureD3D9 == null)
                        {
                            ResourceManager.wmapTextureStore[ResourceManager.battleMapDictionary[0].buildingLayerMatrix[xCount, yCount].textureID].textureD3D9 = pen.CreateTexture(
                                ResourceManager.wmapTextureStore[ResourceManager.battleMapDictionary[0].buildingLayerMatrix[xCount, yCount].textureID].textureBytes,
                                ResourceManager.wmapTextureStore[ResourceManager.battleMapDictionary[0].buildingLayerMatrix[xCount, yCount].textureID].textureWidth,
                                ResourceManager.wmapTextureStore[ResourceManager.battleMapDictionary[0].buildingLayerMatrix[xCount, yCount].textureID].textureHeight);
                        }
                        pen.DrawTexture(ResourceManager.wmapTextureStore[ResourceManager.battleMapDictionary[0].buildingLayerMatrix[xCount, yCount].textureID].textureD3D9,
                            this.mainCamera.GetValidPositionX(), this.mainCamera.GetValidPositionY(),
                            ResourceManager.battleMapDictionary[0].buildingLayerMatrix[xCount, yCount].screenBasePositionX - ResourceManager.wmapTextureStore[ResourceManager.battleMapDictionary[0].buildingLayerMatrix[xCount, yCount].textureID].textureGapX,
                            ResourceManager.battleMapDictionary[0].buildingLayerMatrix[xCount, yCount].screenBasePositionY - ResourceManager.wmapTextureStore[ResourceManager.battleMapDictionary[0].buildingLayerMatrix[xCount, yCount].textureID].textureGapY);
                    }
                }
            }
        }
        #endregion
    }

    public enum GameRunningState
    {
        GameRunningState_Stopped,
        GameRunningState_Running,
        GameRunningState_Finished,
    }

    public enum RuntimeState
    {
        RuntimeState_Welcome,
        RuntimeState_WelcomeMenu,
        RuntimeState_WelcomeHint,
        RuntimeState_World,
        RuntimeState_WorldMenu,
        RuntimeState_WorldActing,
        RuntimeState_WorldHint,
        RuntimeState_Scene,
        RuntimeState_SceneMenu,
        RuntimeState_SceneActing,
        RuntimeState_SceneHint,
        RuntimeState_Battle,
        RuntimeState_BattleMenu,
        RuntimeState_BattleActing,
        RuntimeState_BattleHint,
    }
}