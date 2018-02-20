using SlimDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    public class ResourceManager
    {
        #region declaration
        public static Dictionary<int, byte[]> musicStore;

        public static Dictionary<int, NTexture> titleTextureStore;
        public static Dictionary<int, NTexture> portraitTextureStore;
        public static Dictionary<int, NTexture> iconTextureStore;
        public static Dictionary<int, NTexture> mmapTextureStore;
        public static Dictionary<int, NTexture> smapTextureStore;
        public static Dictionary<int, NTexture> wmapTextureStore;
        public static WorldMap mainMap;
        public static Dictionary<int, SceneMap> sceneMapDictionary;
        public static Dictionary<int, BattleMap> battleMapDictionary;
        #endregion

        #region business
        public static void LoadResource()
        {
            musicStore = new Dictionary<int, byte[]>();

            string titlePath = AppDomain.CurrentDomain.BaseDirectory + "resource\\title.np";
            if (System.IO.File.Exists(titlePath))
            {
                titleTextureStore = GenerateNTextureDictionary(System.IO.File.ReadAllBytes(titlePath));
            }
            string portraitPath = AppDomain.CurrentDomain.BaseDirectory + "resource\\portrait.np";
            if (System.IO.File.Exists(portraitPath))
            {
                portraitTextureStore = GenerateNTextureDictionary(System.IO.File.ReadAllBytes(portraitPath));
            }
            string iconPath = AppDomain.CurrentDomain.BaseDirectory + "resource\\icon.np";
            if (System.IO.File.Exists(iconPath))
            {
                iconTextureStore = GenerateNTextureDictionary(System.IO.File.ReadAllBytes(iconPath));
            }
            string mmapPath = AppDomain.CurrentDomain.BaseDirectory + "resource\\mmap.np";
            if (System.IO.File.Exists(mmapPath))
            {
                mmapTextureStore = GenerateNTextureDictionary(System.IO.File.ReadAllBytes(mmapPath));
            }
            string smapPath = AppDomain.CurrentDomain.BaseDirectory + "resource\\smap.np";
            if (System.IO.File.Exists(smapPath))
            {
                smapTextureStore = GenerateNTextureDictionary(System.IO.File.ReadAllBytes(smapPath));
            }
            string wmapPath = AppDomain.CurrentDomain.BaseDirectory + "resource\\wmap.np";
            if (System.IO.File.Exists(wmapPath))
            {
                wmapTextureStore = GenerateNTextureDictionary(System.IO.File.ReadAllBytes(wmapPath));
            }

            mainMap = new WorldMap();
            string worldNPPath = AppDomain.CurrentDomain.BaseDirectory + "resource\\world.np";
            if (System.IO.File.Exists(worldNPPath))
            {
                byte[] worldBytes = System.IO.File.ReadAllBytes(worldNPPath);
                int checkPosition = 0;
                for (int yCount = 0; yCount <= 479; yCount++)
                {
                    for (int xCount = 0; xCount <= 479; xCount++)
                    {
                        int imgID = BitConverter.ToInt16(worldBytes, checkPosition) / 2;
                        if (imgID > 0)
                        {
                            mainMap.earthLayerMatrix[xCount, yCount] = new FixedUnit();
                            mainMap.earthLayerMatrix[xCount, yCount].SetFixedCoordinate(xCount, yCount);
                            mainMap.earthLayerMatrix[xCount, yCount].textureID = imgID;
                        }
                        checkPosition += 2;
                    }
                }
                for (int yCount = 0; yCount <= 479; yCount++)
                {
                    for (int xCount = 0; xCount <= 479; xCount++)
                    {
                        int imgID = BitConverter.ToInt16(worldBytes, checkPosition) / 2;
                        if (imgID > 0)
                        {
                            mainMap.surfaceLayerMatrix[xCount, yCount] = new FixedUnit();
                            mainMap.surfaceLayerMatrix[xCount, yCount].SetFixedCoordinate(xCount, yCount);
                            mainMap.surfaceLayerMatrix[xCount, yCount].textureID = imgID;
                        }
                        checkPosition += 2;
                    }
                }
                for (int yCount = 0; yCount <= 479; yCount++)
                {
                    for (int xCount = 0; xCount <= 479; xCount++)
                    {
                        int imgID = BitConverter.ToInt16(worldBytes, checkPosition) / 2;
                        if (imgID > 0)
                        {
                            mainMap.buildingLayerMatrix[xCount, yCount] = new WorldBuildingUnit();
                            mainMap.buildingLayerMatrix[xCount, yCount].SetFixedCoordinate(xCount, yCount);
                            mainMap.buildingLayerMatrix[xCount, yCount].textureID = imgID;
                        }
                        checkPosition += 2;
                    }
                }
                for (int yCount = 0; yCount <= 479; yCount++)
                {
                    for (int xCount = 0; xCount <= 479; xCount++)
                    {
                        int xMark = BitConverter.ToInt16(worldBytes, checkPosition);
                        if (xMark > 0)
                        {
                            if (xCount == 0 && yCount == 14)
                            {
                                bool breakPoint = true;
                            }
                            if (mainMap.buildXYLayerMatrix[xCount, yCount] == null)
                            {
                                mainMap.buildXYLayerMatrix[xCount, yCount] = new BuildXY();
                            }
                            mainMap.buildXYLayerMatrix[xCount, yCount].buildX = xMark;
                        }
                        checkPosition += 2;
                    }
                }
                for (int yCount = 0; yCount <= 479; yCount++)
                {
                    for (int xCount = 0; xCount <= 479; xCount++)
                    {
                        int yMark = BitConverter.ToInt16(worldBytes, checkPosition);
                        if (yMark > 0)
                        {
                            if (xCount == 0 && yCount == 14)
                            {
                                bool breakPoint = true;
                            }
                            if (mainMap.buildXYLayerMatrix[xCount, yCount] == null)
                            {
                                mainMap.buildXYLayerMatrix[xCount, yCount] = new BuildXY();
                            }
                            mainMap.buildXYLayerMatrix[xCount, yCount].buildY = yMark;
                        }
                        checkPosition += 2;
                    }
                }
            }

            battleMapDictionary = new Dictionary<int, BattleMap>();
            string allBattlePath = AppDomain.CurrentDomain.BaseDirectory + "resource\\battle.np";
            if (System.IO.File.Exists(allBattlePath))
            {
                byte[] allBattleBytes = System.IO.File.ReadAllBytes(allBattlePath);
                int totalCount = BitConverter.ToInt32(allBattleBytes, 0);
                int startPos = 4;
                int checkCount = 0;
                int markCount = 0;
                while (checkCount < totalCount)
                {
                    markCount = 0;
                    BattleMap eachBM = new BattleMap(checkCount);
                    for (int yCount = 0; yCount < 64; yCount++)
                    {
                        for (int xCount = 0; xCount < 64; xCount++)
                        {
                            int textureID = BitConverter.ToInt16(allBattleBytes, startPos + markCount) / 2;
                            if (textureID > 0)
                            {
                                eachBM.floorLayerMatrix[xCount, yCount] = new FixedUnit();
                                eachBM.floorLayerMatrix[xCount, yCount].SetFixedCoordinate(xCount, yCount);
                                eachBM.floorLayerMatrix[xCount, yCount].textureID = textureID;
                            }
                            markCount += 2;
                        }
                    }
                    for (int yCount = 0; yCount < 64; yCount++)
                    {
                        for (int xCount = 0; xCount < 64; xCount++)
                        {
                            int textureID = BitConverter.ToInt16(allBattleBytes, startPos + markCount) / 2;
                            if (textureID > 0)
                            {
                                eachBM.buildingLayerMatrix[xCount, yCount] = new FixedUnit();
                                eachBM.buildingLayerMatrix[xCount, yCount].SetFixedCoordinate(xCount, yCount);
                                eachBM.buildingLayerMatrix[xCount, yCount].textureID = textureID;
                            }
                            markCount += 2;
                        }
                    }
                    battleMapDictionary.Add(checkCount, eachBM);
                    checkCount++;
                }
            }

            GameService game = GameService.GetGame();
            game.mainCamera = new CameraUnit();
            game.player = new Player();

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
            game.player.playerSceneUnit.textureIDDictionary.Add(0, goUpTextures);
            game.player.playerSceneUnit.textureIDDictionary.Add(1, goRightTextures);
            game.player.playerSceneUnit.textureIDDictionary.Add(2, goLeftTextures);
            game.player.playerSceneUnit.textureIDDictionary.Add(3, goDownTextures);
            game.player.playerSceneUnit.textureIDDictionary.Add(4, faceUpTextures);
            game.player.playerSceneUnit.textureIDDictionary.Add(5, faceRightTextures);
            game.player.playerSceneUnit.textureIDDictionary.Add(6, faceLeftTextures);
            game.player.playerSceneUnit.textureIDDictionary.Add(7, faceDownTextures);
            game.player.playerWorldUnit.textureIDDictionary.Add(0, goUpTextures);
            game.player.playerWorldUnit.textureIDDictionary.Add(1, goRightTextures);
            game.player.playerWorldUnit.textureIDDictionary.Add(2, goLeftTextures);
            game.player.playerWorldUnit.textureIDDictionary.Add(3, goDownTextures);
            game.player.playerWorldUnit.textureIDDictionary.Add(4, faceUpTextures);
            game.player.playerWorldUnit.textureIDDictionary.Add(5, faceRightTextures);
            game.player.playerWorldUnit.textureIDDictionary.Add(6, faceLeftTextures);
            game.player.playerWorldUnit.textureIDDictionary.Add(7, faceDownTextures);

            List<int> battleUpTextures = new List<int>();
            battleUpTextures.Add(2553);
            List<int> battleRightTextures = new List<int>();
            battleRightTextures.Add(2554);
            List<int> battleLeftTextures = new List<int>();
            battleLeftTextures.Add(2555);
            List<int> battleDownTextures = new List<int>();
            battleDownTextures.Add(2556);
            game.player.playerBattleUnit.textureIDDictionary.Add(0, battleUpTextures);
            game.player.playerBattleUnit.textureIDDictionary.Add(1, battleRightTextures);
            game.player.playerBattleUnit.textureIDDictionary.Add(2, battleLeftTextures);
            game.player.playerBattleUnit.textureIDDictionary.Add(3, battleDownTextures);
        }

        private static Dictionary<int, NTexture> GenerateNTextureDictionary(byte[] pmNPBytes)
        {
            Dictionary<int, NTexture> result = new Dictionary<int, NTexture>();

            int maxCount = BitConverter.ToInt32(pmNPBytes, 0);
            int checkCount = 0;
            int basePos = 4;
            int length = 0;
            short width = 0, height = 0, gapX = 0, gapY = 0;
            while (checkCount < maxCount)
            {
                length = BitConverter.ToInt32(pmNPBytes, basePos);
                width = BitConverter.ToInt16(pmNPBytes, basePos + 4);
                height = BitConverter.ToInt16(pmNPBytes, basePos + 4 + 2);
                gapX = BitConverter.ToInt16(pmNPBytes, basePos + 4 + 2 + 2);
                gapY = BitConverter.ToInt16(pmNPBytes, basePos + 4 + 2 + 2 + 2);
                byte[] bodyBytes = new byte[length - 12];
                Buffer.BlockCopy(pmNPBytes, basePos + 4 + 2 + 2 + 2 + 2, bodyBytes, 0, length - 12);
                NTexture newNT = new NTexture(bodyBytes, 0, 0, gapX, gapY);
                result.Add(checkCount, newNT);
                basePos += length;
                checkCount++;
            }

            return result;
        }

        public static void LoadGameFile(int pmFileIndex)
        {
            string allScenePath = string.Format(AppDomain.CurrentDomain.BaseDirectory + "resource\\save\\scene{0}.np", pmFileIndex);
            sceneMapDictionary = new Dictionary<int, SceneMap>();
            if (System.IO.File.Exists(allScenePath))
            {
                byte[] allSceneBytes = System.IO.File.ReadAllBytes(allScenePath);
                int declarationTotalCount = BitConverter.ToInt32(allSceneBytes, 0);
                int declarationSize = 0x8000;
                int declarationStartPos = 4;
                int eventStartPos = declarationStartPos + declarationTotalCount * 0x8000;
                int eventTotalCount = BitConverter.ToInt32(allSceneBytes, eventStartPos);
                eventStartPos += 4;
                int eventSize = 0x1130;
                int eachEventSize = 0x16;
                int checkCount = 0;
                int declarationMarkCount = 0;
                while (checkCount < declarationTotalCount)
                {
                    declarationMarkCount = 0;
                    SceneMap eachSM = new SceneMap(checkCount);
                    for (int yCount = 0; yCount < 64; yCount++)
                    {
                        for (int xCount = 0; xCount < 64; xCount++)
                        {
                            int textureID = BitConverter.ToInt16(allSceneBytes, declarationStartPos + checkCount * declarationSize + declarationMarkCount * 2) / 2;
                            if (textureID > 0)
                            {
                                eachSM.floorLayerMatrix[xCount, yCount] = new FixedUnit();
                                eachSM.floorLayerMatrix[xCount, yCount].SetFixedCoordinate(xCount, yCount);
                                eachSM.floorLayerMatrix[xCount, yCount].textureID = textureID;
                            }
                            declarationMarkCount++;
                        }
                    }
                    for (int yCount = 0; yCount < 64; yCount++)
                    {
                        for (int xCount = 0; xCount < 64; xCount++)
                        {
                            int textureID = BitConverter.ToInt16(allSceneBytes, declarationStartPos + checkCount * declarationSize + declarationMarkCount * 2) / 2;
                            if (textureID > 0)
                            {
                                eachSM.buildingLayerMatrix[xCount, yCount] = new FixedUnit();
                                eachSM.buildingLayerMatrix[xCount, yCount].SetFixedCoordinate(xCount, yCount);
                                eachSM.buildingLayerMatrix[xCount, yCount].textureID = textureID;
                            }
                            declarationMarkCount++;
                        }
                    }
                    for (int yCount = 0; yCount < 64; yCount++)
                    {
                        for (int xCount = 0; xCount < 64; xCount++)
                        {
                            int textureID = BitConverter.ToInt16(allSceneBytes, declarationStartPos + checkCount * declarationSize + declarationMarkCount * 2) / 2;
                            if (textureID > 0)
                            {
                                eachSM.hangLayerMatrix[xCount, yCount] = new FixedUnit();
                                eachSM.hangLayerMatrix[xCount, yCount].SetFixedCoordinate(xCount, yCount);
                                eachSM.hangLayerMatrix[xCount, yCount].textureID = textureID;
                            }
                            declarationMarkCount++;
                        }
                    }
                    for (int yCount = 0; yCount < 64; yCount++)
                    {
                        for (int xCount = 0; xCount < 64; xCount++)
                        {
                            short eventNumber = BitConverter.ToInt16(allSceneBytes, declarationStartPos + checkCount * declarationSize + declarationMarkCount * 2);
                            if (eventNumber >= 0)
                            {
                                eachSM.eventLayerMatrix[xCount, yCount] = new EventUnit();
                                eachSM.eventLayerMatrix[xCount, yCount].SetFixedCoordinate(xCount, yCount);
                                eachSM.eventLayerMatrix[xCount, yCount].goThrough = BitConverter.ToInt16(allSceneBytes, eventStartPos + checkCount * eventSize + eventNumber * eachEventSize);
                                eachSM.eventLayerMatrix[xCount, yCount].unitNumber = BitConverter.ToInt16(allSceneBytes, eventStartPos + checkCount * eventSize + eventNumber * eachEventSize + 2);
                                eachSM.eventLayerMatrix[xCount, yCount].eventIDList.Add(BitConverter.ToInt16(allSceneBytes, eventStartPos + checkCount * eventSize + eventNumber * eachEventSize + 2 + 2));
                                eachSM.eventLayerMatrix[xCount, yCount].eventIDList.Add(BitConverter.ToInt16(allSceneBytes, eventStartPos + checkCount * eventSize + eventNumber * eachEventSize + 2 + 2 + 2));
                                eachSM.eventLayerMatrix[xCount, yCount].eventIDList.Add(BitConverter.ToInt16(allSceneBytes, eventStartPos + checkCount * eventSize + eventNumber * eachEventSize + 2 + 2 + 2 + 2));
                                eachSM.eventLayerMatrix[xCount, yCount].startTextureID = BitConverter.ToInt16(allSceneBytes, eventStartPos + checkCount * eventSize + eventNumber * eachEventSize + 2 + 2 + 2 + 2 + 2) / 2;
                                eachSM.eventLayerMatrix[xCount, yCount].endTextureID = BitConverter.ToInt16(allSceneBytes, eventStartPos + checkCount * eventSize + eventNumber * eachEventSize + 2 + 2 + 2 + 2 + 2 + 2) / 2;
                            }
                            declarationMarkCount++;
                        }
                    }
                    sceneMapDictionary.Add(checkCount, eachSM);
                    checkCount++;
                }
            }

            // Debug
            GameService game = GameService.GetGame();

            game.mainCamera.ResetCamera(-400, 200);
            game.mainCamera.BindToPlayer();

            //game.player.EnterScene(0, 0);
            //game.player.playerSceneUnit.SetFixedCoordinate(25, 39);
            //game.player.EnterWorld();
            game.player.EnterBattle(0);
            game.player.playerBattleUnit.SetFixedCoordinate(25, 39);

            game.runtimeState = RuntimeState.RuntimeState_Battle;
        }
        #endregion        
    }
}