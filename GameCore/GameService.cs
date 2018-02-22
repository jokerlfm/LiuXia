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
        public ActivePlayer mainPlayer;
        public Menu activeMenu;
        public int currentSmapID = 0;
        public int currentWmapID = 0;
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

        #region Core operatrion

        #endregion
        public void Run()
        {
            if (state == GameRunningState.GameRunningState_Stopped)
            {
                if (!this.InitGame())
                {
                    state = GameRunningState.GameRunningState_Stopped;
                    return;
                }

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
            if (!this.LoadGame(0))
            {
                ForceShutdown();
                return;
            }

            // Debug
            this.DebugAdjusting();

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
                switch (runtimeState)
                {
                    case RuntimeState.RuntimeState_World:
                        {
                            HandleWorldInput(inputName);
                            break;
                        }
                    case RuntimeState.RuntimeState_Scene:
                        {
                            HandleSceneInput(inputName);
                            break;
                        }
                    case RuntimeState.RuntimeState_Battle:
                        {
                            HandleBattleInput(inputName);
                            break;
                        }
                    case RuntimeState.RuntimeState_Menu:
                        {
                            HandleMenuInput(inputName);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
        }

        private void HandleWorldInput(string pmInputName)
        {
            switch (pmInputName)
            {
                case "Escape":
                    {
                        switch (activeMenu.type)
                        {
                            case MenuType.MenuType_None:
                                {
                                    activeMenu = ResourceManager.menuDictionary[MenuType.MenuType_Main];
                                    activeMenu.selectedIndex = 0;
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                        break;
                    }
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
                        if (ResourceManager.mainMap.Movable(mainPlayer.playerWorldUnit.coordinateX, mainPlayer.playerWorldUnit.coordinateY - 1))
                        {
                            mainPlayer.playerWorldUnit.MoveUp(ConfigHandler.GetConfigValue_int("movespeed"), ConfigHandler.GetConfigValue_int("actspeed"));
                        }
                        else
                        {
                            mainPlayer.playerWorldUnit.FaceUp();
                        }
                        break;
                    }
                case "DownArrow":
                    {
                        if (ResourceManager.mainMap.Movable(mainPlayer.playerWorldUnit.coordinateX, mainPlayer.playerWorldUnit.coordinateY + 1))
                        {
                            mainPlayer.playerWorldUnit.MoveDown(ConfigHandler.GetConfigValue_int("movespeed"), ConfigHandler.GetConfigValue_int("actspeed"));
                        }
                        else
                        {
                            mainPlayer.playerWorldUnit.FaceDown();
                        }
                        break;
                    }
                case "LeftArrow":
                    {
                        if (ResourceManager.mainMap.Movable(mainPlayer.playerWorldUnit.coordinateX - 1, mainPlayer.playerWorldUnit.coordinateY))
                        {
                            mainPlayer.playerWorldUnit.MoveLeft(ConfigHandler.GetConfigValue_int("movespeed"), ConfigHandler.GetConfigValue_int("actspeed"));
                        }
                        else
                        {
                            mainPlayer.playerWorldUnit.FaceLeft();
                        }
                        break;
                    }
                case "RightArrow":
                    {
                        if (ResourceManager.mainMap.Movable(mainPlayer.playerWorldUnit.coordinateX + 1, mainPlayer.playerWorldUnit.coordinateY))
                        {
                            mainPlayer.playerWorldUnit.MoveRight(ConfigHandler.GetConfigValue_int("movespeed"), ConfigHandler.GetConfigValue_int("actspeed"));
                        }
                        else
                        {
                            mainPlayer.playerWorldUnit.FaceRight();
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private void HandleSceneInput(string pmInputName)
        {
            switch (pmInputName)
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
                        if (ResourceManager.sceneMapDictionary[currentSmapID].Movable(mainPlayer.playerSceneUnit.coordinateX, mainPlayer.playerSceneUnit.coordinateY - 1))
                        {
                            mainPlayer.playerSceneUnit.MoveUp(ConfigHandler.GetConfigValue_int("movespeed"), ConfigHandler.GetConfigValue_int("actspeed"));
                        }
                        else
                        {
                            mainPlayer.playerSceneUnit.FaceUp();
                        }
                        break;
                    }
                case "DownArrow":
                    {
                        if (ResourceManager.sceneMapDictionary[currentSmapID].Movable(mainPlayer.playerSceneUnit.coordinateX, mainPlayer.playerSceneUnit.coordinateY + 1))
                        {
                            mainPlayer.playerSceneUnit.MoveDown(ConfigHandler.GetConfigValue_int("movespeed"), ConfigHandler.GetConfigValue_int("actspeed"));
                        }
                        else
                        {
                            mainPlayer.playerSceneUnit.FaceDown();
                        }
                        break;
                    }
                case "LeftArrow":
                    {
                        if (ResourceManager.sceneMapDictionary[currentSmapID].Movable(mainPlayer.playerSceneUnit.coordinateX - 1, mainPlayer.playerSceneUnit.coordinateY))
                        {
                            mainPlayer.playerSceneUnit.MoveLeft(ConfigHandler.GetConfigValue_int("movespeed"), ConfigHandler.GetConfigValue_int("actspeed"));
                        }
                        else
                        {
                            mainPlayer.playerSceneUnit.FaceLeft();
                        }
                        break;
                    }
                case "RightArrow":
                    {
                        if (ResourceManager.sceneMapDictionary[currentSmapID].Movable(mainPlayer.playerSceneUnit.coordinateX + 1, mainPlayer.playerSceneUnit.coordinateY))
                        {
                            mainPlayer.playerSceneUnit.MoveRight(ConfigHandler.GetConfigValue_int("movespeed"), ConfigHandler.GetConfigValue_int("actspeed"));
                        }
                        else
                        {
                            mainPlayer.playerSceneUnit.FaceRight();
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private void HandleBattleInput(string pmInputName)
        {
            switch (pmInputName)
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
                        if (ResourceManager.battleMapDictionary[currentWmapID].Movable(mainPlayer.playerBattleUnit.coordinateX, mainPlayer.playerBattleUnit.coordinateY - 1))
                        {
                            mainPlayer.playerBattleUnit.MoveUp(ConfigHandler.GetConfigValue_int("movespeed"));
                        }
                        else
                        {
                            mainPlayer.playerBattleUnit.FaceUp();
                        }
                        break;
                    }
                case "DownArrow":
                    {
                        if (ResourceManager.battleMapDictionary[currentWmapID].Movable(mainPlayer.playerBattleUnit.coordinateX, mainPlayer.playerBattleUnit.coordinateY + 1))
                        {
                            mainPlayer.playerBattleUnit.MoveDown(ConfigHandler.GetConfigValue_int("movespeed"));
                        }
                        else
                        {
                            mainPlayer.playerBattleUnit.FaceDown();
                        }
                        break;
                    }
                case "LeftArrow":
                    {
                        if (ResourceManager.battleMapDictionary[currentWmapID].Movable(mainPlayer.playerBattleUnit.coordinateX - 1, mainPlayer.playerBattleUnit.coordinateY))
                        {
                            mainPlayer.playerBattleUnit.MoveLeft(ConfigHandler.GetConfigValue_int("movespeed"));
                        }
                        else
                        {
                            mainPlayer.playerBattleUnit.FaceLeft();
                        }
                        break;
                    }
                case "RightArrow":
                    {
                        if (ResourceManager.battleMapDictionary[currentWmapID].Movable(mainPlayer.playerBattleUnit.coordinateX + 1, mainPlayer.playerBattleUnit.coordinateY))
                        {
                            mainPlayer.playerBattleUnit.MoveRight(ConfigHandler.GetConfigValue_int("movespeed"));
                        }
                        else
                        {
                            mainPlayer.playerBattleUnit.FaceRight();
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private void HandleMenuInput(string pmInputName)
        {
            switch (pmInputName)
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
                        if (ResourceManager.battleMapDictionary[currentWmapID].Movable(mainPlayer.playerBattleUnit.coordinateX, mainPlayer.playerBattleUnit.coordinateY - 1))
                        {
                            mainPlayer.playerBattleUnit.MoveUp(ConfigHandler.GetConfigValue_int("movespeed"));
                        }
                        else
                        {
                            mainPlayer.playerBattleUnit.FaceUp();
                        }
                        break;
                    }
                case "DownArrow":
                    {
                        if (ResourceManager.battleMapDictionary[currentWmapID].Movable(mainPlayer.playerBattleUnit.coordinateX, mainPlayer.playerBattleUnit.coordinateY + 1))
                        {
                            mainPlayer.playerBattleUnit.MoveDown(ConfigHandler.GetConfigValue_int("movespeed"));
                        }
                        else
                        {
                            mainPlayer.playerBattleUnit.FaceDown();
                        }
                        break;
                    }
                case "LeftArrow":
                    {
                        if (ResourceManager.battleMapDictionary[currentWmapID].Movable(mainPlayer.playerBattleUnit.coordinateX - 1, mainPlayer.playerBattleUnit.coordinateY))
                        {
                            mainPlayer.playerBattleUnit.MoveLeft(ConfigHandler.GetConfigValue_int("movespeed"));
                        }
                        else
                        {
                            mainPlayer.playerBattleUnit.FaceLeft();
                        }
                        break;
                    }
                case "RightArrow":
                    {
                        if (ResourceManager.battleMapDictionary[currentWmapID].Movable(mainPlayer.playerBattleUnit.coordinateX + 1, mainPlayer.playerBattleUnit.coordinateY))
                        {
                            mainPlayer.playerBattleUnit.MoveRight(ConfigHandler.GetConfigValue_int("movespeed"));
                        }
                        else
                        {
                            mainPlayer.playerBattleUnit.FaceRight();
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private void UpdatePlayer(int pmTimeElapsed)
        {
            switch (mainPlayer.state)
            {
                case PlayerState.PlayerState_World:
                    {
                        if (mainPlayer.playerWorldUnit.moving)
                        {
                            mainPlayer.playerWorldUnit.UpdateMoving(pmTimeElapsed);
                            if (mainCamera.bondToPlayer)
                            {
                                mainCamera.ResetCamera((int)(mainPlayer.playerWorldUnit.screenBasePositionX - displayTargetForm.Width / ConfigHandler.GetConfigValue_float("zoom") / 2), (int)(mainPlayer.playerWorldUnit.screenBasePositionY - displayTargetForm.Height / ConfigHandler.GetConfigValue_float("zoom") / 2));
                            }
                        }
                        break;
                    }
                case PlayerState.PlayerState_Scene:
                    {
                        if (mainPlayer.playerSceneUnit.moving)
                        {
                            mainPlayer.playerSceneUnit.UpdateMoving(pmTimeElapsed);
                            if (mainCamera.bondToPlayer)
                            {
                                mainCamera.ResetCamera((int)(mainPlayer.playerSceneUnit.screenBasePositionX - displayTargetForm.Width / ConfigHandler.GetConfigValue_float("zoom") / 2), (int)(mainPlayer.playerSceneUnit.screenBasePositionY - displayTargetForm.Height / ConfigHandler.GetConfigValue_float("zoom") / 2));
                            }
                        }
                        break;
                    }
                case PlayerState.PlayerState_Battle:
                    {
                        if (mainPlayer.playerBattleUnit.moving)
                        {
                            mainPlayer.playerBattleUnit.UpdateMoving(pmTimeElapsed);
                            mainCamera.ResetCamera((int)(mainPlayer.playerBattleUnit.screenBasePositionX - displayTargetForm.Width / ConfigHandler.GetConfigValue_float("zoom") / 2), (int)(mainPlayer.playerBattleUnit.screenBasePositionY - displayTargetForm.Height / ConfigHandler.GetConfigValue_float("zoom") / 2));
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
            pen.BeginDrawing();
            pen.BeginTextureDrawing();
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
            pen.EndTextureDrawing();
            pen.BeginHintDrawing();
            DrawMenu(activeMenu);
            pen.EndHintDrawing();
            pen.EndDrawing();
        }

        private void DrawMmap()
        {
            int minX = mainPlayer.playerWorldUnit.coordinateX - 30, maxX = mainPlayer.playerWorldUnit.coordinateX + 30;
            int minY = mainPlayer.playerWorldUnit.coordinateY - 30, maxY = mainPlayer.playerWorldUnit.coordinateY + 30;
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

                    if (xCount == (int)mainPlayer.playerWorldUnit.movingCoordinateX && yCount == (int)mainPlayer.playerWorldUnit.movingCoordinateY)
                    {
                        if (ResourceManager.mmapTextureStore[mainPlayer.playerWorldUnit.textureIDDictionary[mainPlayer.playerWorldUnit.actGroupIndex][mainPlayer.playerWorldUnit.actFrameIndex]].textureD3D9 == null)
                        {
                            ResourceManager.mmapTextureStore[mainPlayer.playerWorldUnit.textureIDDictionary[mainPlayer.playerWorldUnit.actGroupIndex][mainPlayer.playerWorldUnit.actFrameIndex]].textureD3D9 = pen.CreateTexture(
                                ResourceManager.mmapTextureStore[mainPlayer.playerWorldUnit.textureIDDictionary[mainPlayer.playerWorldUnit.actGroupIndex][mainPlayer.playerWorldUnit.actFrameIndex]].textureBytes,
                                ResourceManager.mmapTextureStore[mainPlayer.playerWorldUnit.textureIDDictionary[mainPlayer.playerWorldUnit.actGroupIndex][mainPlayer.playerWorldUnit.actFrameIndex]].textureWidth,
                                ResourceManager.mmapTextureStore[mainPlayer.playerWorldUnit.textureIDDictionary[mainPlayer.playerWorldUnit.actGroupIndex][mainPlayer.playerWorldUnit.actFrameIndex]].textureHeight);
                        }
                        pen.DrawTexture(ResourceManager.mmapTextureStore[mainPlayer.playerWorldUnit.textureIDDictionary[mainPlayer.playerWorldUnit.actGroupIndex][mainPlayer.playerWorldUnit.actFrameIndex]].textureD3D9,
                                this.mainCamera.GetValidPositionX(), this.mainCamera.GetValidPositionY(),
                                mainPlayer.playerWorldUnit.screenBasePositionX - ResourceManager.mmapTextureStore[mainPlayer.playerWorldUnit.textureIDDictionary[mainPlayer.playerWorldUnit.actGroupIndex][mainPlayer.playerWorldUnit.actFrameIndex]].textureGapX,
                                mainPlayer.playerWorldUnit.screenBasePositionY - ResourceManager.mmapTextureStore[mainPlayer.playerWorldUnit.textureIDDictionary[mainPlayer.playerWorldUnit.actGroupIndex][mainPlayer.playerWorldUnit.actFrameIndex]].textureGapY);
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
                    if (xCount == (int)mainPlayer.playerSceneUnit.movingCoordinateX && yCount == (int)mainPlayer.playerSceneUnit.movingCoordinateY)
                    {
                        if (ResourceManager.smapTextureStore[mainPlayer.playerSceneUnit.textureIDDictionary[mainPlayer.playerSceneUnit.actGroupIndex][mainPlayer.playerSceneUnit.actFrameIndex]].textureD3D9 == null)
                        {
                            ResourceManager.smapTextureStore[mainPlayer.playerSceneUnit.textureIDDictionary[mainPlayer.playerSceneUnit.actGroupIndex][mainPlayer.playerSceneUnit.actFrameIndex]].textureD3D9 = pen.CreateTexture(
                                ResourceManager.smapTextureStore[mainPlayer.playerSceneUnit.textureIDDictionary[mainPlayer.playerSceneUnit.actGroupIndex][mainPlayer.playerSceneUnit.actFrameIndex]].textureBytes,
                                ResourceManager.smapTextureStore[mainPlayer.playerSceneUnit.textureIDDictionary[mainPlayer.playerSceneUnit.actGroupIndex][mainPlayer.playerSceneUnit.actFrameIndex]].textureWidth,
                                ResourceManager.smapTextureStore[mainPlayer.playerSceneUnit.textureIDDictionary[mainPlayer.playerSceneUnit.actGroupIndex][mainPlayer.playerSceneUnit.actFrameIndex]].textureHeight);
                        }
                        pen.DrawTexture(ResourceManager.smapTextureStore[mainPlayer.playerSceneUnit.textureIDDictionary[mainPlayer.playerSceneUnit.actGroupIndex][mainPlayer.playerSceneUnit.actFrameIndex]].textureD3D9,
                                this.mainCamera.GetValidPositionX(), this.mainCamera.GetValidPositionY(),
                                mainPlayer.playerSceneUnit.screenBasePositionX - ResourceManager.smapTextureStore[mainPlayer.playerSceneUnit.textureIDDictionary[mainPlayer.playerSceneUnit.actGroupIndex][mainPlayer.playerSceneUnit.actFrameIndex]].textureGapX,
                                mainPlayer.playerSceneUnit.screenBasePositionY - ResourceManager.smapTextureStore[mainPlayer.playerSceneUnit.textureIDDictionary[mainPlayer.playerSceneUnit.actGroupIndex][mainPlayer.playerSceneUnit.actFrameIndex]].textureGapY);
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
                    if (xCount == (int)mainPlayer.playerBattleUnit.movingCoordinateX && yCount == (int)mainPlayer.playerBattleUnit.movingCoordinateY)
                    {
                        if (ResourceManager.wmapTextureStore[mainPlayer.playerBattleUnit.textureIDDictionary[mainPlayer.playerBattleUnit.actGroupIndex][mainPlayer.playerBattleUnit.actFrameIndex]].textureD3D9 == null)
                        {
                            ResourceManager.wmapTextureStore[mainPlayer.playerBattleUnit.textureIDDictionary[mainPlayer.playerBattleUnit.actGroupIndex][mainPlayer.playerBattleUnit.actFrameIndex]].textureD3D9 = pen.CreateTexture(
                                ResourceManager.wmapTextureStore[mainPlayer.playerBattleUnit.textureIDDictionary[mainPlayer.playerBattleUnit.actGroupIndex][mainPlayer.playerBattleUnit.actFrameIndex]].textureBytes,
                                ResourceManager.wmapTextureStore[mainPlayer.playerBattleUnit.textureIDDictionary[mainPlayer.playerBattleUnit.actGroupIndex][mainPlayer.playerBattleUnit.actFrameIndex]].textureWidth,
                                ResourceManager.wmapTextureStore[mainPlayer.playerBattleUnit.textureIDDictionary[mainPlayer.playerBattleUnit.actGroupIndex][mainPlayer.playerBattleUnit.actFrameIndex]].textureHeight);
                        }
                        pen.DrawTexture(ResourceManager.wmapTextureStore[mainPlayer.playerBattleUnit.textureIDDictionary[mainPlayer.playerBattleUnit.actGroupIndex][mainPlayer.playerBattleUnit.actFrameIndex]].textureD3D9,
                                this.mainCamera.GetValidPositionX(), this.mainCamera.GetValidPositionY(),
                                mainPlayer.playerBattleUnit.screenBasePositionX - ResourceManager.wmapTextureStore[mainPlayer.playerBattleUnit.textureIDDictionary[mainPlayer.playerBattleUnit.actGroupIndex][mainPlayer.playerBattleUnit.actFrameIndex]].textureGapX,
                                mainPlayer.playerBattleUnit.screenBasePositionY - ResourceManager.wmapTextureStore[mainPlayer.playerBattleUnit.textureIDDictionary[mainPlayer.playerBattleUnit.actGroupIndex][mainPlayer.playerBattleUnit.actFrameIndex]].textureGapY);
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

        private void DrawMenu(Menu pmTargetMenu)
        {
            switch (pmTargetMenu.type)
            {
                case MenuType.MenuType_None:
                    {
                        break;
                    }
                case MenuType.MenuType_Main:
                    {                        
                        pen.DrawFrame(0.01f, 0.02f, 0.1f, 0.6f, 0.004f, System.Drawing.Color.Yellow, 1f);
                        DrawMenu(pmTargetMenu.parentMenu);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
        #endregion

        #region Command
        public bool InitGame()
        {
            try
            {
                ConfigHandler.LoadConfig();
                ResourceManager.LoadResource();

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

                ResizeWindow();
            }
            catch (Exception exp)
            {
                return false;
            }
            return true;
        }

        public void ResizeWindow()
        {
            this.displayTargetForm.Width = ConfigHandler.GetConfigValue_int("WindowWidth");
            this.displayTargetForm.Height = ConfigHandler.GetConfigValue_int("WindowHeight");
            this.pen.ResizeDevice(this.displayTargetForm.Width, this.displayTargetForm.Height);
        }

        public void ShowWelcome()
        {
            GameService destGame = GameService.GetGame();

            this.runtimeState = RuntimeState.RuntimeState_Welcome;
        }

        public bool LoadGame(int pmFileIndex)
        {
            if (!ResourceManager.LoadGameFile(pmFileIndex))
            {
                return false;
            }

            return true;
        }
        #endregion

        #region debug
        private void DebugAdjusting()
        {

            // player img ids in mmap and smap are the same
            List<int> goUpTextures = new List<int>();
            //goUpTextures.Add(2501);
            goUpTextures.Add(2502);
            goUpTextures.Add(2503);
            goUpTextures.Add(2504);
            goUpTextures.Add(2505);
            goUpTextures.Add(2506);
            goUpTextures.Add(2507);
            List<int> goRightTextures = new List<int>();
            //goRightTextures.Add(2508);
            goRightTextures.Add(2509);
            goRightTextures.Add(2510);
            goRightTextures.Add(2511);
            goRightTextures.Add(2512);
            goRightTextures.Add(2513);
            goRightTextures.Add(2514);
            List<int> goLeftTextures = new List<int>();
            //goLeftTextures.Add(2515);
            goLeftTextures.Add(2516);
            goLeftTextures.Add(2517);
            goLeftTextures.Add(2518);
            goLeftTextures.Add(2519);
            goLeftTextures.Add(2520);
            goLeftTextures.Add(2521);
            List<int> goDownTextures = new List<int>();
            //goDownTextures.Add(2522);
            goDownTextures.Add(2523);
            goDownTextures.Add(2524);
            goDownTextures.Add(2525);
            goDownTextures.Add(2526);
            goDownTextures.Add(2527);
            goDownTextures.Add(2528);
            List<int> faceUpTextures = new List<int>();
            faceUpTextures.Add(2501);
            List<int> faceRightTextures = new List<int>();
            faceRightTextures.Add(2508);
            List<int> faceLeftTextures = new List<int>();
            faceLeftTextures.Add(2515);
            List<int> faceDownTextures = new List<int>();
            faceDownTextures.Add(2522);
            this.mainPlayer.playerSceneUnit.textureIDDictionary.Add(0, goUpTextures);
            this.mainPlayer.playerSceneUnit.textureIDDictionary.Add(1, goRightTextures);
            this.mainPlayer.playerSceneUnit.textureIDDictionary.Add(2, goLeftTextures);
            this.mainPlayer.playerSceneUnit.textureIDDictionary.Add(3, goDownTextures);
            this.mainPlayer.playerSceneUnit.textureIDDictionary.Add(4, faceUpTextures);
            this.mainPlayer.playerSceneUnit.textureIDDictionary.Add(5, faceRightTextures);
            this.mainPlayer.playerSceneUnit.textureIDDictionary.Add(6, faceLeftTextures);
            this.mainPlayer.playerSceneUnit.textureIDDictionary.Add(7, faceDownTextures);
            this.mainPlayer.playerWorldUnit.textureIDDictionary.Add(0, goUpTextures);
            this.mainPlayer.playerWorldUnit.textureIDDictionary.Add(1, goRightTextures);
            this.mainPlayer.playerWorldUnit.textureIDDictionary.Add(2, goLeftTextures);
            this.mainPlayer.playerWorldUnit.textureIDDictionary.Add(3, goDownTextures);
            this.mainPlayer.playerWorldUnit.textureIDDictionary.Add(4, faceUpTextures);
            this.mainPlayer.playerWorldUnit.textureIDDictionary.Add(5, faceRightTextures);
            this.mainPlayer.playerWorldUnit.textureIDDictionary.Add(6, faceLeftTextures);
            this.mainPlayer.playerWorldUnit.textureIDDictionary.Add(7, faceDownTextures);

            List<int> battleUpTextures = new List<int>();
            battleUpTextures.Add(2553);
            List<int> battleRightTextures = new List<int>();
            battleRightTextures.Add(2554);
            List<int> battleLeftTextures = new List<int>();
            battleLeftTextures.Add(2555);
            List<int> battleDownTextures = new List<int>();
            battleDownTextures.Add(2556);
            this.mainPlayer.playerBattleUnit.textureIDDictionary.Add(0, battleUpTextures);
            this.mainPlayer.playerBattleUnit.textureIDDictionary.Add(1, battleRightTextures);
            this.mainPlayer.playerBattleUnit.textureIDDictionary.Add(2, battleLeftTextures);
            this.mainPlayer.playerBattleUnit.textureIDDictionary.Add(3, battleDownTextures);

            this.mainCamera.ResetCamera(-400, 200);
            this.mainCamera.BindToPlayer();

            this.mainPlayer.EnterWorld();
            //this.player.EnterScene(0, 0);
            //this.player.playerSceneUnit.SetFixedCoordinate(25, 39);            
            //this.mainPlayer.EnterBattle(0);
            //this.mainPlayer.playerBattleUnit.SetFixedCoordinate(25, 39);

            this.runtimeState = RuntimeState.RuntimeState_World;
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
        RuntimeState_Menu,
        RuntimeState_Welcome,
        RuntimeState_WelcomeHint,
        RuntimeState_World,
        RuntimeState_WorldActing,
        RuntimeState_WorldHint,
        RuntimeState_Scene,
        RuntimeState_SceneActing,
        RuntimeState_SceneHint,
        RuntimeState_Battle,
        RuntimeState_BattleActing,
        RuntimeState_BattleHint,
    }
}