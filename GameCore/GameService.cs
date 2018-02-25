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
        public bool isFullScreen = false;
        public float unitTextureScaling = 0;
        public float portraitTextureScaling = 0;
        public float itemTextureScaling = 0;
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

                pen.BeginDrawing();
                DrawUnits();
                DrawMenus();
                pen.EndDrawing();

                dtPrevUpdateTime = DateTime.Now;
                Thread.Sleep(10);
            }

            state = GameRunningState.GameRunningState_Finished;
        }

        private void HandleInput()
        {
            string pressingKeyName = "";
            string releasedKeyName = "";
            if (controller.GetKeyName(out pressingKeyName, out releasedKeyName))
            {
                if (runtimeState == RuntimeState.RuntimeState_World)
                {
                    HandleWorldFreeInput(pressingKeyName, releasedKeyName);
                }
                else if (runtimeState == RuntimeState.RuntimeState_Scene)
                {
                    HandleSceneFreeInput(pressingKeyName, releasedKeyName);
                }
                else if (runtimeState == RuntimeState.RuntimeState_Battle_Menu)
                {
                    HandleBattleMenuInput(releasedKeyName);
                }
                else if (runtimeState == RuntimeState.RuntimeState_Welcome_Menu || runtimeState == RuntimeState.RuntimeState_World_Menu ||
                    runtimeState == RuntimeState.RuntimeState_Scene_Menu || runtimeState == RuntimeState.RuntimeState_Battle_Menu)
                {
                    HandleMenuInput(releasedKeyName);
                }
            }
        }

        private void HandleWorldFreeInput(string pmPressingKeyName, string pmReleasedKeyName)
        {
            if (pmReleasedKeyName == "Escape")
            {
                activeMenu = ResourceManager.menuDictionary[MenuType.MenuType_Main];
                activeMenu.subSelectedIndex = 0;
                runtimeState = RuntimeState.RuntimeState_World_Menu;
                return;
            }
            else
            {
                switch (pmPressingKeyName)
                {
                    case "UpArrow":
                        {
                            if (!mainPlayer.playerWorldUnit.moving)
                            {
                                if (ResourceManager.mainMap.Movable(mainPlayer.playerWorldUnit.coordinateX, mainPlayer.playerWorldUnit.coordinateY - 1))
                                {
                                    mainPlayer.playerWorldUnit.MoveUp(ConfigHandler.GetConfigValue_int("movespeed"), ConfigHandler.GetConfigValue_int("actspeed"));
                                }
                                else
                                {
                                    mainPlayer.playerWorldUnit.FaceUp();
                                }
                            }
                            return;
                        }
                    case "DownArrow":
                        {
                            if (!mainPlayer.playerWorldUnit.moving)
                            {
                                if (ResourceManager.mainMap.Movable(mainPlayer.playerWorldUnit.coordinateX, mainPlayer.playerWorldUnit.coordinateY + 1))
                                {
                                    mainPlayer.playerWorldUnit.MoveDown(ConfigHandler.GetConfigValue_int("movespeed"), ConfigHandler.GetConfigValue_int("actspeed"));
                                }
                                else
                                {
                                    mainPlayer.playerWorldUnit.FaceDown();
                                }
                            }
                            return;
                        }
                    case "LeftArrow":
                        {
                            if (!mainPlayer.playerWorldUnit.moving)
                            {
                                if (ResourceManager.mainMap.Movable(mainPlayer.playerWorldUnit.coordinateX - 1, mainPlayer.playerWorldUnit.coordinateY))
                                {
                                    mainPlayer.playerWorldUnit.MoveLeft(ConfigHandler.GetConfigValue_int("movespeed"), ConfigHandler.GetConfigValue_int("actspeed"));
                                }
                                else
                                {
                                    mainPlayer.playerWorldUnit.FaceLeft();
                                }
                            }
                            return;
                        }
                    case "RightArrow":
                        {
                            if (!mainPlayer.playerWorldUnit.moving)
                            {
                                if (ResourceManager.mainMap.Movable(mainPlayer.playerWorldUnit.coordinateX + 1, mainPlayer.playerWorldUnit.coordinateY))
                                {
                                    mainPlayer.playerWorldUnit.MoveRight(ConfigHandler.GetConfigValue_int("movespeed"), ConfigHandler.GetConfigValue_int("actspeed"));
                                }
                                else
                                {
                                    mainPlayer.playerWorldUnit.FaceRight();
                                }
                            }
                            return;
                        }
                    default:
                        {
                            return;
                        }
                }
            }
        }

        private void HandleSceneFreeInput(string pmPressingKeyName, string pmReleasedKeyName)
        {
            if (pmReleasedKeyName == "Escape")
            {
                activeMenu = ResourceManager.menuDictionary[MenuType.MenuType_Main];
                activeMenu.subSelectedIndex = 0;
                runtimeState = RuntimeState.RuntimeState_Scene_Menu;
                return;
            }
            else
            {
                switch (pmPressingKeyName)
                {
                    case "UpArrow":
                        {
                            if (!mainPlayer.playerSceneUnit.moving)
                            {
                                if (ResourceManager.sceneMapDictionary[currentSmapID].Movable(mainPlayer.playerSceneUnit.coordinateX, mainPlayer.playerSceneUnit.coordinateY - 1))
                                {
                                    mainPlayer.playerSceneUnit.MoveUp(ConfigHandler.GetConfigValue_int("movespeed"), ConfigHandler.GetConfigValue_int("actspeed"));
                                }
                                else
                                {
                                    mainPlayer.playerSceneUnit.FaceUp();
                                }
                            }
                            return;
                        }
                    case "DownArrow":
                        {
                            if (!mainPlayer.playerSceneUnit.moving)
                            {
                                if (ResourceManager.sceneMapDictionary[currentSmapID].Movable(mainPlayer.playerSceneUnit.coordinateX, mainPlayer.playerSceneUnit.coordinateY + 1))
                                {
                                    mainPlayer.playerSceneUnit.MoveDown(ConfigHandler.GetConfigValue_int("movespeed"), ConfigHandler.GetConfigValue_int("actspeed"));
                                }
                                else
                                {
                                    mainPlayer.playerSceneUnit.FaceDown();
                                }
                            }
                            return;
                        }
                    case "LeftArrow":
                        {
                            if (!mainPlayer.playerSceneUnit.moving)
                            {
                                if (ResourceManager.sceneMapDictionary[currentSmapID].Movable(mainPlayer.playerSceneUnit.coordinateX - 1, mainPlayer.playerSceneUnit.coordinateY))
                                {
                                    mainPlayer.playerSceneUnit.MoveLeft(ConfigHandler.GetConfigValue_int("movespeed"), ConfigHandler.GetConfigValue_int("actspeed"));
                                }
                                else
                                {
                                    mainPlayer.playerSceneUnit.FaceLeft();
                                }
                            }
                            return;
                        }
                    case "RightArrow":
                        {
                            if (!mainPlayer.playerSceneUnit.moving)
                            {
                                if (ResourceManager.sceneMapDictionary[currentSmapID].Movable(mainPlayer.playerSceneUnit.coordinateX + 1, mainPlayer.playerSceneUnit.coordinateY))
                                {
                                    mainPlayer.playerSceneUnit.MoveRight(ConfigHandler.GetConfigValue_int("movespeed"), ConfigHandler.GetConfigValue_int("actspeed"));
                                }
                                else
                                {
                                    mainPlayer.playerSceneUnit.FaceRight();
                                }
                            }
                            return;
                        }
                    default:
                        {
                            return;
                        }
                }
            }
        }

        private void HandleBattleMenuInput(string pmReleasedKeyName)
        {

        }

        private void HandleMenuInput(string pmReleasedKeyName)
        {
            switch (pmReleasedKeyName)
            {
                case "Escape":
                    {
                        activeMenu = activeMenu.parentMenu;
                        if (activeMenu.type == MenuType.MenuType_None)
                        {
                            if (runtimeState == RuntimeState.RuntimeState_World_Menu)
                            {
                                runtimeState = RuntimeState.RuntimeState_World;
                            }
                            else if (runtimeState == RuntimeState.RuntimeState_Scene_Menu)
                            {
                                runtimeState = RuntimeState.RuntimeState_Scene;
                            }
                        }
                        return;
                    }
                case "Space":
                    {
                        switch (activeMenu.type)
                        {
                            case MenuType.MenuType_Main:
                                {
                                    switch (activeMenu.subMenuList[activeMenu.subSelectedIndex].type)
                                    {
                                        case MenuType.MenuType_Item:
                                            {
                                                activeMenu = activeMenu.subMenuList[activeMenu.subSelectedIndex];
                                                if (ResourceManager.mainPlayerData.itemsIDList.Count < activeMenu.subSelectedIndex - 1)
                                                {
                                                    activeMenu.subSelectedIndex = 0;
                                                }
                                                return;
                                            }
                                        case MenuType.MenuType_System:
                                            {
                                                activeMenu = activeMenu.subMenuList[activeMenu.subSelectedIndex];
                                                activeMenu.subSelectedIndex = 0;
                                                return;
                                            }
                                        case MenuType.MenuType_Status:
                                            {
                                                // update party list
                                                activeMenu.subMenuList[activeMenu.subSelectedIndex].subMenuList.Clear();
                                                foreach (int characterID in ResourceManager.mainPlayerData.partyMembersArray)
                                                {
                                                    if (characterID >= 0)
                                                    {
                                                        activeMenu.subMenuList[activeMenu.subSelectedIndex].subMenuList.Add(new Menu(MenuType.MenuType_Status_Each, ResourceManager.characterDictionary[characterID].characterName, characterID, activeMenu.subMenuList[activeMenu.subSelectedIndex], true));
                                                    }
                                                }
                                                activeMenu = activeMenu.subMenuList[activeMenu.subSelectedIndex];
                                                activeMenu.subSelectedIndex = 0;
                                                return;
                                            }
                                        default:
                                            {
                                                return;
                                            }
                                    }
                                }
                            case MenuType.MenuType_System:
                                {
                                    switch (activeMenu.subMenuList[activeMenu.subSelectedIndex].type)
                                    {
                                        case MenuType.MenuType_Quit:
                                            {
                                                activeMenu = activeMenu.subMenuList[activeMenu.subSelectedIndex];
                                                activeMenu.subSelectedIndex = 1;
                                                return;
                                            }
                                        default:
                                            {
                                                return;
                                            }
                                    }
                                }
                            case MenuType.MenuType_Quit:
                                {
                                    switch (activeMenu.subMenuList[activeMenu.subSelectedIndex].type)
                                    {
                                        case MenuType.MenuType_Quit_Yes:
                                            {
                                                this.ForceShutdown();
                                                return;
                                            }
                                        case MenuType.MenuType_Quit_No:
                                            {
                                                this.activeMenu = this.activeMenu.parentMenu;
                                                return;
                                            }
                                        default:
                                            {
                                                return;
                                            }
                                    }
                                }
                            default:
                                {
                                    return;
                                }
                        }
                    }
                case "UpArrow":
                    {
                        if (activeMenu.subMenuList.Count > 0)
                        {
                            activeMenu.subSelectedIndex--;
                            if (activeMenu.subSelectedIndex < 0)
                            {
                                activeMenu.subSelectedIndex = activeMenu.subMenuList.Count - 1;
                            }
                        }
                        else
                        {
                            if (activeMenu.type == MenuType.MenuType_Item)
                            {
                                activeMenu.subSelectedIndex -= 7;
                            }
                        }
                        return;
                    }
                case "DownArrow":
                    {
                        if (activeMenu.subMenuList.Count > 0)
                        {
                            activeMenu.subSelectedIndex++;
                            if (activeMenu.subSelectedIndex >= activeMenu.subMenuList.Count)
                            {
                                activeMenu.subSelectedIndex = 0;
                            }
                        }
                        else
                        {
                            if (activeMenu.type == MenuType.MenuType_Item)
                            {
                                activeMenu.subSelectedIndex += 7;
                            }
                        }
                        return;
                    }
                case "LeftArrow":
                    {
                        if (activeMenu.subMenuList.Count > 0)
                        {
                            activeMenu.subSelectedIndex--;
                            if (activeMenu.subSelectedIndex < 0)
                            {
                                activeMenu.subSelectedIndex = activeMenu.subMenuList.Count - 1;
                            }
                        }
                        else
                        {
                            if (activeMenu.type == MenuType.MenuType_Item)
                            {
                                activeMenu.subSelectedIndex -= 1;
                            }
                        }
                        return;
                    }
                case "RightArrow":
                    {
                        if (activeMenu.subMenuList.Count > 0)
                        {
                            activeMenu.subSelectedIndex++;
                            if (activeMenu.subSelectedIndex >= activeMenu.subMenuList.Count)
                            {
                                activeMenu.subSelectedIndex = 0;
                            }
                        }
                        else
                        {
                            if (activeMenu.type == MenuType.MenuType_Item)
                            {
                                activeMenu.subSelectedIndex += 1;
                            }
                        }
                        return;
                    }
                case "PageUp":
                    {
                        if (activeMenu.type == MenuType.MenuType_Item)
                        {
                            activeMenu.subSelectedIndex -= 35;
                        }
                        return;
                    }
                case "PageDown":
                    {
                        if (activeMenu.type == MenuType.MenuType_Item)
                        {
                            activeMenu.subSelectedIndex += 35;
                        }
                        return;
                    }
                default:
                    {
                        return;
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
                                mainCamera.ResetCamera(mainPlayer.playerWorldUnit.screenBasePositionX - displayTargetForm.Width / unitTextureScaling / 2, mainPlayer.playerWorldUnit.screenBasePositionY - displayTargetForm.Height / unitTextureScaling / 2);
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
                                mainCamera.ResetCamera(mainPlayer.playerSceneUnit.screenBasePositionX - displayTargetForm.Width / unitTextureScaling / 2, mainPlayer.playerSceneUnit.screenBasePositionY - displayTargetForm.Height / unitTextureScaling / 2);
                            }
                        }
                        break;
                    }
                case PlayerState.PlayerState_Battle:
                    {
                        if (mainPlayer.playerBattleUnit.moving)
                        {
                            mainPlayer.playerBattleUnit.UpdateMoving(pmTimeElapsed);
                            mainCamera.ResetCamera(mainPlayer.playerBattleUnit.screenBasePositionX - displayTargetForm.Width / unitTextureScaling / 2, mainPlayer.playerBattleUnit.screenBasePositionY - displayTargetForm.Height / unitTextureScaling / 2);
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

        private void DrawUnits()
        {
            pen.BeginUnitDrawing();
            if (runtimeState == RuntimeState.RuntimeState_World || runtimeState == RuntimeState.RuntimeState_WorldActing || runtimeState == RuntimeState.RuntimeState_WorldHint || runtimeState == RuntimeState.RuntimeState_World_Menu)
            {
                DrawMmap();
            }
            else if (runtimeState == RuntimeState.RuntimeState_Scene || runtimeState == RuntimeState.RuntimeState_SceneActing || runtimeState == RuntimeState.RuntimeState_SceneHint || runtimeState == RuntimeState.RuntimeState_Scene_Menu)
            {
                DrawSmap();
            }
            else if (runtimeState == RuntimeState.RuntimeState_Battle || runtimeState == RuntimeState.RuntimeState_BattleActing || runtimeState == RuntimeState.RuntimeState_BattleHint || runtimeState == RuntimeState.RuntimeState_Battle_Menu)
            {
                DrawWmap();
            }
            pen.EndUnitDrawing();
        }

        private void DrawMmap()
        {
            int minX = mainPlayer.playerWorldUnit.coordinateX - 25, maxX = mainPlayer.playerWorldUnit.coordinateX + 25;
            int minY = mainPlayer.playerWorldUnit.coordinateY - 25, maxY = mainPlayer.playerWorldUnit.coordinateY + 25;
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
                        pen.DrawUnitTexture(ResourceManager.mmapTextureStore[ResourceManager.mainMap.earthLayerMatrix[xCount, yCount].textureID].textureD3D9,
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
                        pen.DrawUnitTexture(ResourceManager.mmapTextureStore[ResourceManager.mainMap.surfaceLayerMatrix[xCount, yCount].textureID].textureD3D9,
                            this.mainCamera.GetValidPositionX(), this.mainCamera.GetValidPositionY(),
                            ResourceManager.mainMap.surfaceLayerMatrix[xCount, yCount].screenBasePositionX - ResourceManager.mmapTextureStore[ResourceManager.mainMap.surfaceLayerMatrix[xCount, yCount].textureID].textureGapX,
                            ResourceManager.mainMap.surfaceLayerMatrix[xCount, yCount].screenBasePositionY - ResourceManager.mmapTextureStore[ResourceManager.mainMap.surfaceLayerMatrix[xCount, yCount].textureID].textureGapY);
                    }
                }
            }

            for (int xCount = minX; xCount <= maxX; xCount++)
            {
                for (int yCount = minY; yCount <= maxY; yCount++)
                {
                    if (ResourceManager.mainMap.buildingLayerMatrix[xCount, yCount] != null)
                    {
                        if (ResourceManager.mmapTextureStore[ResourceManager.mainMap.buildingLayerMatrix[xCount, yCount].textureID].textureD3D9 == null)
                        {
                            ResourceManager.mmapTextureStore[ResourceManager.mainMap.buildingLayerMatrix[xCount, yCount].textureID].textureD3D9 = pen.CreateTexture(
                                ResourceManager.mmapTextureStore[ResourceManager.mainMap.buildingLayerMatrix[xCount, yCount].textureID].textureBytes,
                                ResourceManager.mmapTextureStore[ResourceManager.mainMap.buildingLayerMatrix[xCount, yCount].textureID].textureWidth,
                                ResourceManager.mmapTextureStore[ResourceManager.mainMap.buildingLayerMatrix[xCount, yCount].textureID].textureHeight);
                        }
                        pen.DrawUnitTexture(ResourceManager.mmapTextureStore[ResourceManager.mainMap.buildingLayerMatrix[xCount, yCount].textureID].textureD3D9,
                            this.mainCamera.GetValidPositionX(), this.mainCamera.GetValidPositionY(),
                            ResourceManager.mainMap.buildingLayerMatrix[xCount, yCount].screenBasePositionX - ResourceManager.mmapTextureStore[ResourceManager.mainMap.buildingLayerMatrix[xCount, yCount].textureID].textureGapX,
                            ResourceManager.mainMap.buildingLayerMatrix[xCount, yCount].screenBasePositionY - ResourceManager.mmapTextureStore[ResourceManager.mainMap.buildingLayerMatrix[xCount, yCount].textureID].textureGapY);
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
                        pen.DrawUnitTexture(ResourceManager.mmapTextureStore[mainPlayer.playerWorldUnit.textureIDDictionary[mainPlayer.playerWorldUnit.actGroupIndex][mainPlayer.playerWorldUnit.actFrameIndex]].textureD3D9,
                                this.mainCamera.GetValidPositionX(), this.mainCamera.GetValidPositionY(),
                                mainPlayer.playerWorldUnit.screenBasePositionX - ResourceManager.mmapTextureStore[mainPlayer.playerWorldUnit.textureIDDictionary[mainPlayer.playerWorldUnit.actGroupIndex][mainPlayer.playerWorldUnit.actFrameIndex]].textureGapX,
                                mainPlayer.playerWorldUnit.screenBasePositionY - ResourceManager.mmapTextureStore[mainPlayer.playerWorldUnit.textureIDDictionary[mainPlayer.playerWorldUnit.actGroupIndex][mainPlayer.playerWorldUnit.actFrameIndex]].textureGapY);
                    }
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
                        pen.DrawUnitTexture(ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].floorLayerMatrix[xCount, yCount].textureID].textureD3D9,
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
                        pen.DrawUnitTexture(ResourceManager.smapTextureStore[mainPlayer.playerSceneUnit.textureIDDictionary[mainPlayer.playerSceneUnit.actGroupIndex][mainPlayer.playerSceneUnit.actFrameIndex]].textureD3D9,
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
                        pen.DrawUnitTexture(ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].buildingLayerMatrix[xCount, yCount].textureID].textureD3D9,
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
                        pen.DrawUnitTexture(ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].hangLayerMatrix[xCount, yCount].textureID].textureD3D9,
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
                            pen.DrawUnitTexture(ResourceManager.smapTextureStore[ResourceManager.sceneMapDictionary[0].eventLayerMatrix[xCount, yCount].startTextureID].textureD3D9,
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
                        pen.DrawUnitTexture(ResourceManager.wmapTextureStore[ResourceManager.battleMapDictionary[0].floorLayerMatrix[xCount, yCount].textureID].textureD3D9,
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
                        pen.DrawUnitTexture(ResourceManager.wmapTextureStore[mainPlayer.playerBattleUnit.textureIDDictionary[mainPlayer.playerBattleUnit.actGroupIndex][mainPlayer.playerBattleUnit.actFrameIndex]].textureD3D9,
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
                        pen.DrawUnitTexture(ResourceManager.wmapTextureStore[ResourceManager.battleMapDictionary[0].buildingLayerMatrix[xCount, yCount].textureID].textureD3D9,
                            this.mainCamera.GetValidPositionX(), this.mainCamera.GetValidPositionY(),
                            ResourceManager.battleMapDictionary[0].buildingLayerMatrix[xCount, yCount].screenBasePositionX - ResourceManager.wmapTextureStore[ResourceManager.battleMapDictionary[0].buildingLayerMatrix[xCount, yCount].textureID].textureGapX,
                            ResourceManager.battleMapDictionary[0].buildingLayerMatrix[xCount, yCount].screenBasePositionY - ResourceManager.wmapTextureStore[ResourceManager.battleMapDictionary[0].buildingLayerMatrix[xCount, yCount].textureID].textureGapY);
                    }
                }
            }
        }

        private void DrawMenus()
        {
            pen.BeginMenuDrawing();
            pen.BeginPortraitDrawing();
            pen.BeginItemDrawing();
            DrawMenu(activeMenu);
            pen.EndItemDrawing();
            pen.EndPortraitDrawing();
            pen.EndMenuDrawing();
        }

        private void DrawMenu(Menu pmTargetMenu)
        {
            switch (pmTargetMenu.type)
            {
                case MenuType.MenuType_None:
                    {
                        return;
                    }
                case MenuType.MenuType_Main:
                    {
                        pen.DrawRateLineRectangle(0.01f, 0.02f, 0.08f, 0.32f, System.Drawing.Color.White, System.Drawing.Color.FromArgb(200, System.Drawing.Color.Black));
                        for (int subCount = 0; subCount < pmTargetMenu.subMenuList.Count; subCount++)
                        {
                            System.Drawing.Color menuItemColor = System.Drawing.Color.DarkOrange;
                            if (subCount == pmTargetMenu.subSelectedIndex)
                            {
                                menuItemColor = System.Drawing.Color.White;
                            }
                            pen.DrawMenuText(pmTargetMenu.subMenuList[subCount].menuName, 0.015f, 0.025f + 0.05f * subCount, menuItemColor);
                        }
                        if (pmTargetMenu.drawParents)
                        {
                            DrawMenu(pmTargetMenu.parentMenu);
                        }
                        return;
                    }
                case MenuType.MenuType_Item:
                    {
                        pen.DrawRateLineRectangle(0.09f, 0.02f, 0.99f, 0.99f, System.Drawing.Color.White, System.Drawing.Color.FromArgb(200, System.Drawing.Color.Black));
                        pen.DrawRateLine(0.1f, 0.18f, 0.98f, 0.18f, System.Drawing.Color.White);
                        if (pmTargetMenu.subSelectedIndex < 0)
                        {
                            pmTargetMenu.subSelectedIndex = 0;
                        }
                        if (pmTargetMenu.subSelectedIndex >= ResourceManager.mainPlayerData.itemsIDList.Count)
                        {
                            pmTargetMenu.subSelectedIndex = ResourceManager.mainPlayerData.itemsIDList.Count - 1;
                        }
                        if (ResourceManager.mainPlayerData.itemsIDList.Count > pmTargetMenu.subSelectedIndex)
                        {
                            if (ResourceManager.mmapTextureStore[ConstManager.itemTextureStartIndex + ResourceManager.mainPlayerData.itemsIDList[pmTargetMenu.subSelectedIndex]].textureD3D9 == null)
                            {
                                ResourceManager.mmapTextureStore[ConstManager.itemTextureStartIndex + ResourceManager.mainPlayerData.itemsIDList[pmTargetMenu.subSelectedIndex]].textureD3D9 = pen.CreateTexture(ResourceManager.mmapTextureStore[ConstManager.itemTextureStartIndex + ResourceManager.mainPlayerData.itemsIDList[pmTargetMenu.subSelectedIndex]].textureBytes, ResourceManager.mmapTextureStore[ConstManager.itemTextureStartIndex + ResourceManager.mainPlayerData.itemsIDList[pmTargetMenu.subSelectedIndex]].textureWidth, ResourceManager.mmapTextureStore[ConstManager.itemTextureStartIndex + ResourceManager.mainPlayerData.itemsIDList[pmTargetMenu.subSelectedIndex]].textureHeight);
                            }
                            pen.DrawItemTexture(ResourceManager.mmapTextureStore[ConstManager.itemTextureStartIndex + ResourceManager.mainPlayerData.itemsIDList[pmTargetMenu.subSelectedIndex]].textureD3D9, (float)displayTargetForm.Width / itemTextureScaling * 0.13f - ResourceManager.mmapTextureStore[ConstManager.itemTextureStartIndex + ResourceManager.mainPlayerData.itemsIDList[pmTargetMenu.subSelectedIndex]].textureWidth / 2 / itemTextureScaling, (float)displayTargetForm.Height / itemTextureScaling * 0.07f - ResourceManager.mmapTextureStore[ConstManager.itemTextureStartIndex + ResourceManager.mainPlayerData.itemsIDList[pmTargetMenu.subSelectedIndex]].textureHeight / 2 / itemTextureScaling);
                            pen.DrawMenuText(ResourceManager.itemDictionary[ResourceManager.mainPlayerData.itemsIDList[pmTargetMenu.subSelectedIndex]].itemName, 0.22f, 0.05f, System.Drawing.Color.White);
                            pen.DrawMenuText(ResourceManager.itemDictionary[ResourceManager.mainPlayerData.itemsIDList[pmTargetMenu.subSelectedIndex]].itemDescription, 0.22f, 0.11f, System.Drawing.Color.White);

                            int totalCountPerPage = ConstManager.itemDisplayCountPerRow * ConstManager.itemDisplayColumnCountPerPage;
                            int startIndex = pmTargetMenu.subSelectedIndex / totalCountPerPage;
                            startIndex = startIndex * totalCountPerPage;
                            float gapX = 0, gapY = 0;
                            int checkCount = startIndex;
                            int endIndex = startIndex + totalCountPerPage;
                            if (endIndex > ResourceManager.mainPlayerData.itemsIDList.Count)
                            {
                                endIndex = ResourceManager.mainPlayerData.itemsIDList.Count;
                            }
                            while (checkCount < endIndex)
                            {
                                if (ResourceManager.mmapTextureStore[ConstManager.itemTextureStartIndex + ResourceManager.mainPlayerData.itemsIDList[checkCount]].textureD3D9 == null)
                                {
                                    ResourceManager.mmapTextureStore[ConstManager.itemTextureStartIndex + ResourceManager.mainPlayerData.itemsIDList[checkCount]].textureD3D9 = pen.CreateTexture(ResourceManager.mmapTextureStore[ConstManager.itemTextureStartIndex + ResourceManager.mainPlayerData.itemsIDList[checkCount]].textureBytes, ResourceManager.mmapTextureStore[ConstManager.itemTextureStartIndex + ResourceManager.mainPlayerData.itemsIDList[checkCount]].textureWidth, ResourceManager.mmapTextureStore[ConstManager.itemTextureStartIndex + ResourceManager.mainPlayerData.itemsIDList[checkCount]].textureHeight);
                                }
                                pen.DrawItemTexture(ResourceManager.mmapTextureStore[ConstManager.itemTextureStartIndex + ResourceManager.mainPlayerData.itemsIDList[checkCount]].textureD3D9,
                                (float)displayTargetForm.Width / itemTextureScaling * (0.15f + gapX) - ResourceManager.mmapTextureStore[ConstManager.itemTextureStartIndex + ResourceManager.mainPlayerData.itemsIDList[pmTargetMenu.subSelectedIndex]].textureWidth / 2 / itemTextureScaling, (float)displayTargetForm.Height / itemTextureScaling * (0.25f + gapY) - ResourceManager.mmapTextureStore[ConstManager.itemTextureStartIndex + ResourceManager.mainPlayerData.itemsIDList[pmTargetMenu.subSelectedIndex]].textureHeight / 2 / itemTextureScaling);

                                if (checkCount == pmTargetMenu.subSelectedIndex)
                                {
                                    pen.DrawRateLineRectangle(0.12f + gapX, 0.21f + gapY, 0.12f + gapX + 0.11f, 0.21f + gapY + 0.145f, System.Drawing.Color.White, System.Drawing.Color.FromArgb(0, System.Drawing.Color.White));
                                }

                                gapX += 0.12f;
                                checkCount++;
                                if (checkCount % ConstManager.itemDisplayCountPerRow == 0)
                                {
                                    gapX = 0;
                                    gapY += 0.15f;
                                }
                            }
                        }

                        if (pmTargetMenu.drawParents)
                        {
                            DrawMenu(pmTargetMenu.parentMenu);
                        }
                        return;
                    }
                case MenuType.MenuType_Status:
                    {
                        pen.DrawRateLineRectangle(0.09f, 0.02f, 0.224f, 0.02f + 0.05f * pmTargetMenu.subMenuList.Count, System.Drawing.Color.White, System.Drawing.Color.FromArgb(200, System.Drawing.Color.Black));
                        pen.DrawRateLineRectangle(0.234f, 0.02f, 0.99f, 0.99f, System.Drawing.Color.White, System.Drawing.Color.FromArgb(200, System.Drawing.Color.Black));
                        for (int subCount = 0; subCount < pmTargetMenu.subMenuList.Count; subCount++)
                        {
                            System.Drawing.Color menuItemColor = System.Drawing.Color.DarkOrange;
                            if (subCount == pmTargetMenu.subSelectedIndex)
                            {
                                menuItemColor = System.Drawing.Color.White;
                            }
                            pen.DrawMenuText(pmTargetMenu.subMenuList[subCount].menuName, 0.095f, 0.025f + 0.05f * subCount, menuItemColor);
                        }
                        Character targetCharacter = ResourceManager.characterDictionary[pmTargetMenu.subMenuList[pmTargetMenu.subSelectedIndex].contexID];
                        if (ResourceManager.portraitTextureStore[targetCharacter.characterPortrait].textureD3D9 == null)
                        {
                            ResourceManager.portraitTextureStore[targetCharacter.characterPortrait].textureD3D9 = pen.CreateTexture(ResourceManager.portraitTextureStore[targetCharacter.characterPortrait].textureBytes, ResourceManager.portraitTextureStore[targetCharacter.characterPortrait].textureWidth, ResourceManager.portraitTextureStore[targetCharacter.characterPortrait].textureHeight);
                        }
                        pen.DrawPortraitTexture(ResourceManager.portraitTextureStore[targetCharacter.characterPortrait].textureD3D9,
                            (float)displayTargetForm.Width / portraitTextureScaling * 0.35f - ResourceManager.portraitTextureStore[targetCharacter.characterPortrait].textureWidth / 2 / portraitTextureScaling, (float)displayTargetForm.Height / portraitTextureScaling * 0.15f - ResourceManager.portraitTextureStore[targetCharacter.characterPortrait].textureHeight / 2 / portraitTextureScaling);

                        pen.DrawMenuText("等級", 0.25f, 0.3f, System.Drawing.Color.Orange); pen.DrawMenuText(targetCharacter.level.ToString(), 0.35f, 0.3f, System.Drawing.Color.Yellow);
                        pen.DrawMenuText("經驗", 0.25f, 0.35f, System.Drawing.Color.Orange); pen.DrawMenuText(targetCharacter.exp.ToString(), 0.35f, 0.35f, System.Drawing.Color.Yellow);
                        pen.DrawMenuText("生命", 0.25f, 0.45f, System.Drawing.Color.Orange); pen.DrawMenuText(targetCharacter.activeLife.ToString() + "/" + targetCharacter.maxLife.ToString(), 0.35f, 0.45f, System.Drawing.Color.Yellow);
                        pen.DrawMenuText("内力", 0.25f, 0.5f, System.Drawing.Color.Orange); pen.DrawMenuText(targetCharacter.activePower.ToString() + "/" + targetCharacter.maxPower.ToString(), 0.35f, 0.5f, targetCharacter.powerType == 2 ? System.Drawing.Color.White : (targetCharacter.powerType == 0 ? System.Drawing.Color.Yellow : System.Drawing.Color.Purple));
                        pen.DrawMenuText("體力", 0.25f, 0.55f, System.Drawing.Color.Orange); pen.DrawMenuText(targetCharacter.physic.ToString(), 0.35f, 0.55f, System.Drawing.Color.Yellow);
                        pen.DrawMenuText("中毒", 0.25f, 0.65f, System.Drawing.Color.Green); pen.DrawMenuText(targetCharacter.poisoned.ToString(), 0.35f, 0.65f, System.Drawing.Color.Green);
                        pen.DrawMenuText("内傷", 0.25f, 0.7f, System.Drawing.Color.Red); pen.DrawMenuText(targetCharacter.injury.ToString(), 0.35f, 0.7f, System.Drawing.Color.Red);
                        pen.DrawMenuText("武器", 0.25f, 0.8f, System.Drawing.Color.Orange); pen.DrawMenuText(ResourceManager.itemDictionary[targetCharacter.weapon].itemName, 0.35f, 0.8f, System.Drawing.Color.Yellow);
                        pen.DrawMenuText("防具", 0.25f, 0.85f, System.Drawing.Color.Orange); pen.DrawMenuText(ResourceManager.itemDictionary[targetCharacter.armor].itemName, 0.35f, 0.85f, System.Drawing.Color.Yellow);

                        pen.DrawMenuText("攻擊", 0.5f, 0.1f, System.Drawing.Color.Orange); pen.DrawMenuText(targetCharacter.attack.ToString(), 0.6f, 0.1f, System.Drawing.Color.Yellow);
                        pen.DrawMenuText("防禦", 0.5f, 0.15f, System.Drawing.Color.Orange); pen.DrawMenuText(targetCharacter.defence.ToString(), 0.6f, 0.15f, System.Drawing.Color.Yellow);
                        pen.DrawMenuText("輕功", 0.5f, 0.2f, System.Drawing.Color.Orange); pen.DrawMenuText(targetCharacter.move.ToString(), 0.6f, 0.2f, System.Drawing.Color.Yellow);
                        pen.DrawMenuText("拳脚", 0.5f, 0.3f, System.Drawing.Color.Orange); pen.DrawMenuText(targetCharacter.fist.ToString(), 0.6f, 0.3f, System.Drawing.Color.Yellow);
                        pen.DrawMenuText("御劍", 0.5f, 0.35f, System.Drawing.Color.Orange); pen.DrawMenuText(targetCharacter.sword.ToString(), 0.6f, 0.35f, System.Drawing.Color.Yellow);
                        pen.DrawMenuText("耍刀", 0.5f, 0.4f, System.Drawing.Color.Orange); pen.DrawMenuText(targetCharacter.blade.ToString(), 0.6f, 0.4f, System.Drawing.Color.Yellow);
                        pen.DrawMenuText("奇門", 0.5f, 0.45f, System.Drawing.Color.Orange); pen.DrawMenuText(targetCharacter.special.ToString(), 0.6f, 0.45f, System.Drawing.Color.Yellow);
                        pen.DrawMenuText("暗器", 0.5f, 0.55f, System.Drawing.Color.Orange); pen.DrawMenuText(targetCharacter.hidden.ToString(), 0.6f, 0.55f, System.Drawing.Color.Yellow);
                        pen.DrawMenuText("用毒", 0.5f, 0.6f, System.Drawing.Color.Orange); pen.DrawMenuText(targetCharacter.tox.ToString(), 0.6f, 0.6f, System.Drawing.Color.Yellow);
                        pen.DrawMenuText("醫療", 0.5f, 0.7f, System.Drawing.Color.Orange); pen.DrawMenuText(targetCharacter.medical.ToString(), 0.6f, 0.7f, System.Drawing.Color.Yellow);
                        pen.DrawMenuText("解毒", 0.5f, 0.75f, System.Drawing.Color.Orange); pen.DrawMenuText(targetCharacter.detox.ToString(), 0.6f, 0.75f, System.Drawing.Color.Yellow);
                        pen.DrawMenuText("抗毒", 0.5f, 0.8f, System.Drawing.Color.Orange); pen.DrawMenuText(targetCharacter.antiTox.ToString(), 0.6f, 0.8f, System.Drawing.Color.Yellow);

                        pen.DrawMenuText("所學武功", 0.75f, 0.1f, System.Drawing.Color.Orange);
                        pen.DrawMenuText(ResourceManager.skillDictionary[targetCharacter.skillsIDArray[0]].skillName, 0.7f, 0.15f, System.Drawing.Color.Yellow); pen.DrawMenuText((targetCharacter.skillsLevelArray[0] / 100 + 1).ToString(), 0.9f, 0.15f, System.Drawing.Color.Yellow);
                        pen.DrawMenuText(ResourceManager.skillDictionary[targetCharacter.skillsIDArray[1]].skillName, 0.7f, 0.2f, System.Drawing.Color.Yellow); pen.DrawMenuText((targetCharacter.skillsLevelArray[1] / 100 + 1).ToString(), 0.9f, 0.2f, System.Drawing.Color.Yellow);
                        pen.DrawMenuText(ResourceManager.skillDictionary[targetCharacter.skillsIDArray[2]].skillName, 0.7f, 0.25f, System.Drawing.Color.Yellow); pen.DrawMenuText((targetCharacter.skillsLevelArray[2] / 100 + 1).ToString(), 0.9f, 0.25f, System.Drawing.Color.Yellow);
                        pen.DrawMenuText(ResourceManager.skillDictionary[targetCharacter.skillsIDArray[3]].skillName, 0.7f, 0.3f, System.Drawing.Color.Yellow); pen.DrawMenuText((targetCharacter.skillsLevelArray[3] / 100 + 1).ToString(), 0.9f, 0.3f, System.Drawing.Color.Yellow);
                        pen.DrawMenuText(ResourceManager.skillDictionary[targetCharacter.skillsIDArray[4]].skillName, 0.7f, 0.35f, System.Drawing.Color.Yellow); pen.DrawMenuText((targetCharacter.skillsLevelArray[4] / 100 + 1).ToString(), 0.9f, 0.35f, System.Drawing.Color.Yellow);
                        pen.DrawMenuText(ResourceManager.skillDictionary[targetCharacter.skillsIDArray[5]].skillName, 0.7f, 0.4f, System.Drawing.Color.Yellow); pen.DrawMenuText((targetCharacter.skillsLevelArray[5] / 100 + 1).ToString(), 0.9f, 0.4f, System.Drawing.Color.Yellow);
                        pen.DrawMenuText(ResourceManager.skillDictionary[targetCharacter.skillsIDArray[6]].skillName, 0.7f, 0.45f, System.Drawing.Color.Yellow); pen.DrawMenuText((targetCharacter.skillsLevelArray[6] / 100 + 1).ToString(), 0.9f, 0.45f, System.Drawing.Color.Yellow);
                        pen.DrawMenuText(ResourceManager.skillDictionary[targetCharacter.skillsIDArray[7]].skillName, 0.7f, 0.5f, System.Drawing.Color.Yellow); pen.DrawMenuText((targetCharacter.skillsLevelArray[7] / 100 + 1).ToString(), 0.9f, 0.5f, System.Drawing.Color.Yellow);
                        pen.DrawMenuText(ResourceManager.skillDictionary[targetCharacter.skillsIDArray[8]].skillName, 0.7f, 0.55f, System.Drawing.Color.Yellow); pen.DrawMenuText((targetCharacter.skillsLevelArray[8] / 100 + 1).ToString(), 0.9f, 0.55f, System.Drawing.Color.Yellow);
                        pen.DrawMenuText(ResourceManager.skillDictionary[targetCharacter.skillsIDArray[9]].skillName, 0.7f, 0.6f, System.Drawing.Color.Yellow); pen.DrawMenuText((targetCharacter.skillsLevelArray[9] / 100 + 1).ToString(), 0.9f, 0.6f, System.Drawing.Color.Yellow);
                        pen.DrawMenuText("修煉物品", 0.75f, 0.7f, System.Drawing.Color.Orange);
                        pen.DrawMenuText(ResourceManager.itemDictionary[targetCharacter.trainingItem].itemName, 0.75f, 0.75f, System.Drawing.Color.Yellow);
                        pen.DrawMenuText("修煉經驗", 0.75f, 0.8f, System.Drawing.Color.Orange);
                        pen.DrawMenuText(targetCharacter.trainingEXP.ToString(), 0.75f, 0.85f, System.Drawing.Color.Yellow);

                        if (pmTargetMenu.drawParents)
                        {
                            DrawMenu(pmTargetMenu.parentMenu);
                        }
                        return;
                    }
                case MenuType.MenuType_System:
                    {
                        pen.DrawRateLineRectangle(0.09f, 0.02f, 0.16f, 0.22f, System.Drawing.Color.White, System.Drawing.Color.FromArgb(200, System.Drawing.Color.Black));
                        for (int subCount = 0; subCount < pmTargetMenu.subMenuList.Count; subCount++)
                        {
                            System.Drawing.Color menuItemColor = System.Drawing.Color.DarkOrange;
                            if (subCount == pmTargetMenu.subSelectedIndex)
                            {
                                menuItemColor = System.Drawing.Color.White;
                            }
                            pen.DrawMenuText(pmTargetMenu.subMenuList[subCount].menuName, 0.095f, 0.025f + 0.05f * subCount, menuItemColor);
                        }
                        if (pmTargetMenu.drawParents)
                        {
                            DrawMenu(pmTargetMenu.parentMenu);
                        }
                        return;
                    }
                case MenuType.MenuType_Quit:
                    {
                        pen.DrawRateLineRectangle(0.17f, 0.02f, 0.24f, 0.12f, System.Drawing.Color.White, System.Drawing.Color.FromArgb(200, System.Drawing.Color.Black));
                        for (int subCount = 0; subCount < pmTargetMenu.subMenuList.Count; subCount++)
                        {
                            System.Drawing.Color menuItemColor = System.Drawing.Color.DarkOrange;
                            if (subCount == pmTargetMenu.subSelectedIndex)
                            {
                                menuItemColor = System.Drawing.Color.White;
                            }
                            pen.DrawMenuText(pmTargetMenu.subMenuList[subCount].menuName, 0.175f, 0.025f + 0.05f * subCount, menuItemColor);
                        }
                        if (pmTargetMenu.drawParents)
                        {
                            DrawMenu(pmTargetMenu.parentMenu);
                        }
                        return;
                    }
                default:
                    {
                        return;
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

                if (displayTargetForm == null)
                {
                    displayTargetForm = new System.Windows.Forms.Form();
                }
                displayTargetForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                displayTargetForm.Width = ConfigHandler.GetConfigValue_int("windowwidth");
                displayTargetForm.Height = ConfigHandler.GetConfigValue_int("windowheight");
                displayTargetForm.Show();

                ResourceManager.LoadResource();

                this.unitTextureScaling = (float)displayTargetForm.Width / (float)ConstManager.baseScreenSizeX;
                this.portraitTextureScaling = (float)ConstManager.basePortraitSize / (float)ResourceManager.portraitTextureStore[0].textureWidth;
                this.itemTextureScaling = (float)ConstManager.baseItemSize / (float)ResourceManager.mmapTextureStore[3501].textureWidth;

                pen = new MediaCore.DrawOperator(displayTargetForm.Handle, displayTargetForm.Width, displayTargetForm.Height, unitTextureScaling, portraitTextureScaling, itemTextureScaling);
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
            ResourceManager.mainPlayerData.partyMembersArray[1] = 3;
            ResourceManager.mainPlayerData.partyMembersArray[2] = 13;
            ResourceManager.mainPlayerData.partyMembersArray[3] = 23;
            ResourceManager.mainPlayerData.partyMembersArray[4] = 62;
            ResourceManager.mainPlayerData.partyMembersArray[5] = 43;
            ResourceManager.mainPlayerData.partyMembersArray[6] = 291;
            ResourceManager.mainPlayerData.partyMembersArray[7] = 27;
            Random rdTest = new Random();
            for (int checkID = 0; checkID < 200; checkID++)
            {
                ResourceManager.mainPlayerData.itemsIDList.Add(checkID);
                ResourceManager.mainPlayerData.itemsCountList.Add(rdTest.Next(1, 1000));
            }

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
            //this.mainPlayer.EnterScene(0, 0);
            //this.mainPlayer.playerSceneUnit.SetFixedCoordinate(25, 39);
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
        RuntimeState_Welcome,
        RuntimeState_Welcome_Menu,
        RuntimeState_WelcomeHint,
        RuntimeState_World,
        RuntimeState_World_Menu,
        RuntimeState_WorldActing,
        RuntimeState_WorldHint,
        RuntimeState_Scene,
        RuntimeState_Scene_Menu,
        RuntimeState_SceneActing,
        RuntimeState_SceneHint,
        RuntimeState_Battle,
        RuntimeState_Battle_Menu,
        RuntimeState_BattleActing,
        RuntimeState_BattleHint,
    }
}