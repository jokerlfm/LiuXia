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
        public static Dictionary<int, Character> characterDictionary;
        public static Dictionary<int, Item> itemDictionary;
        public static Dictionary<int, Skill> skillDictionary;
        public static Dictionary<int, Shop> shopDictionary;

        public static PlayerData mainPlayerData;
        public static Dictionary<MenuType, Menu> menuDictionary;

        private static int selfIncreaseCount = 0;
        private static int SelfIncreaseCount
        {
            set
            {
                selfIncreaseCount = value;
            }
            get
            {
                selfIncreaseCount++;
                return selfIncreaseCount - 1;
            }
        }
        #endregion

        #region business
        public static void LoadResource()
        {
            musicStore = new Dictionary<int, byte[]>();

            LoadNP();
            GenerateMenus();            
            LoadDynamicEvents();

            GameService game = GameService.GetGame();
            game.mainCamera = new CameraUnit();
            game.mainPlayer = new ActivePlayer();
            game.activeMenu = menuDictionary[MenuType.MenuType_None];
        }

        private static void LoadDynamicEvents()
        {

        }
        
        private static void DataAdjustion()
        {
            foreach (Character eachC in characterDictionary.Values)
            {
                int checkWeapon = eachC.weapon;
                int checkArmor = eachC.armor;
                eachC.weapon = -1;
                eachC.armor = -1;
                if (checkWeapon >= 0)
                {
                    eachC.EquipItem(checkWeapon);
                }
                if (checkArmor >= 0)
                {
                    eachC.EquipItem(checkArmor);
                }
            }
        }

        private static void LoadNP()
        {
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
        }

        private static void GenerateMenus()
        {
            menuDictionary = new Dictionary<MenuType, Menu>();
            Menu noneM = new Menu(MenuType.MenuType_None, "無", null);
            Menu mainM = new Menu(MenuType.MenuType_Main, "總", noneM);

            Menu medicalM = new Menu(MenuType.MenuType_Medical, "醫療", mainM, true);
            Menu detoxM = new Menu(MenuType.MenuType_Detox, "解毒", mainM, true);
            Menu itemM = new Menu(MenuType.MenuType_Item, "物品", mainM, true);
            Menu statusM = new Menu(MenuType.MenuType_Status, "狀態", mainM, true);
            Menu leaveM = new Menu(MenuType.MenuType_Leave, "離隊", mainM, true);
            Menu systemM = new Menu(MenuType.MenuType_System, "系統", mainM, true);

            Menu saveM = new Menu(MenuType.MenuType_Save, "存檔", systemM, true);
            Menu loadM = new Menu(MenuType.MenuType_Load, "讀檔", systemM, true);
            Menu fullM = new Menu(MenuType.MenuType_Full, "全屏", systemM, true);
            Menu quitM = new Menu(MenuType.MenuType_Quit, "離開", systemM, true);

            Menu quitYM = new Menu(MenuType.MenuType_Quit_Yes, "確定", systemM, true);
            Menu quitNM = new Menu(MenuType.MenuType_Quit_No, "取消", systemM, true);

            noneM.subMenuList.Add(mainM);
            mainM.subMenuList.Add(medicalM);
            mainM.subMenuList.Add(detoxM);
            mainM.subMenuList.Add(itemM);
            mainM.subMenuList.Add(statusM);
            mainM.subMenuList.Add(leaveM);
            mainM.subMenuList.Add(systemM);

            systemM.subMenuList.Add(saveM);
            systemM.subMenuList.Add(loadM);
            systemM.subMenuList.Add(fullM);
            systemM.subMenuList.Add(quitM);

            quitM.subMenuList.Add(quitYM);
            quitM.subMenuList.Add(quitNM);

            menuDictionary.Add(MenuType.MenuType_None, noneM);
            menuDictionary.Add(MenuType.MenuType_Main, mainM);
            menuDictionary.Add(MenuType.MenuType_Medical, medicalM);
            menuDictionary.Add(MenuType.MenuType_Detox, detoxM);
            menuDictionary.Add(MenuType.MenuType_Item, itemM);
            menuDictionary.Add(MenuType.MenuType_Status, statusM);
            menuDictionary.Add(MenuType.MenuType_Leave, leaveM);
            menuDictionary.Add(MenuType.MenuType_System, systemM);
            menuDictionary.Add(MenuType.MenuType_Save, saveM);
            menuDictionary.Add(MenuType.MenuType_Load, loadM);
            menuDictionary.Add(MenuType.MenuType_Full, fullM);
            menuDictionary.Add(MenuType.MenuType_Quit, quitM);
            menuDictionary.Add(MenuType.MenuType_Quit_Yes, quitYM);
            menuDictionary.Add(MenuType.MenuType_Quit_No, quitNM);
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
                NTexture newNT = new NTexture(bodyBytes, width, height, gapX, gapY);
                result.Add(checkCount, newNT);
                basePos += length;
                checkCount++;
            }

            return result;
        }

        public static bool LoadGameFile(int pmFileIndex)
        {
            try
            {
                string allRangerPath = string.Format(AppDomain.CurrentDomain.BaseDirectory + "resource\\save\\ranger{0}.np", pmFileIndex);
                if (!System.IO.File.Exists(allRangerPath))
                {
                    throw new Exception();
                }
                byte[] allRangerBytes = System.IO.File.ReadAllBytes(allRangerPath);
                characterDictionary = new Dictionary<int, Character>();
                itemDictionary = new Dictionary<int, Item>();
                itemDictionary.Add(-1, new Item(-1, "無"));
                skillDictionary = new Dictionary<int, Skill>();
                skillDictionary.Add(-1, new Skill(-1, "無"));
                shopDictionary = new Dictionary<int, Shop>();
                int playerLength = BitConverter.ToInt32(allRangerBytes, 0);
                int characterLength = BitConverter.ToInt32(allRangerBytes, playerLength);
                int eachCharacterSize = 0x96;
                int totalCharacterCount = (characterLength - 4) / eachCharacterSize;
                int itemLength = BitConverter.ToInt32(allRangerBytes, playerLength + characterLength);
                int eachItemSize = 0xb8;
                int totalItemCount = (itemLength - 4) / eachItemSize;
                int skillLength = BitConverter.ToInt32(allRangerBytes, playerLength + characterLength + itemLength);
                int eachSkillSize = 0x7e;
                int totalSkillCount = (skillLength - 4) / eachSkillSize;
                int shopLength = BitConverter.ToInt32(allRangerBytes, playerLength + characterLength + itemLength + skillLength);
                int eachShopSize = 0x1e;
                int totalShopCount = (shopLength - 4) / eachShopSize;
                mainPlayerData = new PlayerData();
                mainPlayerData.boating = BitConverter.ToInt16(allRangerBytes, 4);
                mainPlayerData.mmapCoordinateX = BitConverter.ToInt16(allRangerBytes, 4 + 2 * 1);
                mainPlayerData.mmapCoordinateY = BitConverter.ToInt16(allRangerBytes, 4 + 2 * 2);
                mainPlayerData.smapCoordinateX = BitConverter.ToInt16(allRangerBytes, 4 + 2 * 3);
                mainPlayerData.smapCoordinateY = BitConverter.ToInt16(allRangerBytes, 4 + 2 * 4);
                mainPlayerData.playerFaceDirection = BitConverter.ToInt16(allRangerBytes, 4 + 2 * 5);
                mainPlayerData.boatCoordinateX1 = BitConverter.ToInt16(allRangerBytes, 4 + 2 * 6);
                mainPlayerData.boatCoordinateY1 = BitConverter.ToInt16(allRangerBytes, 4 + 2 * 7);
                mainPlayerData.boatCoordinateX2 = BitConverter.ToInt16(allRangerBytes, 4 + 2 * 8);
                mainPlayerData.boatCoordinateY2 = BitConverter.ToInt16(allRangerBytes, 4 + 2 * 9);
                mainPlayerData.boatFaceDirection = BitConverter.ToInt16(allRangerBytes, 4 + 2 * 10);
                int checkPartyMemberCount = 0;
                while (checkPartyMemberCount < 8)
                {
                    int partyMemberID = BitConverter.ToInt16(allRangerBytes, 4 + 2 * 11 + 2 * checkPartyMemberCount);
                    if (partyMemberID >= 0)
                    {
                        mainPlayerData.partyMembersIDList.Add(partyMemberID);
                    }
                    checkPartyMemberCount++;
                }
                for (int itemCount = 0; itemCount < 1000; itemCount++)
                {
                    int checkID = BitConverter.ToInt16(allRangerBytes, 4 + 2 * 19 + itemCount * 4);
                    if (checkID > 0)
                    {
                        mainPlayerData.itemsIDList.Add(checkID);
                        mainPlayerData.itemsCountList.Add(BitConverter.ToInt16(allRangerBytes, 4 + 2 * 20 + itemCount * 4));
                    }
                }
                for (int entryCount = 0; entryCount < totalCharacterCount; entryCount++)
                {
                    Character eachC = new Character(entryCount);
                    eachC.characterPortrait = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 1);
                    eachC.increaseRate = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 2);
                    eachC.characterName = Encoding.GetEncoding("BIG5").GetString(allRangerBytes.Skip(playerLength + 4 + eachCharacterSize * entryCount + 2 * 3).Take(10).ToArray()).Trim('\0');
                    eachC.characterNickName = Encoding.GetEncoding("BIG5").GetString(allRangerBytes.Skip(playerLength + 4 + eachCharacterSize * entryCount + 2 * 8).Take(10).ToArray()).Trim('\0');
                    eachC.gender = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 13);
                    eachC.level = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 14);
                    eachC.exp = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 15);
                    eachC.activeLife = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 16);
                    eachC.maxLife = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 17);
                    eachC.injury = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 18);
                    eachC.poisoned = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 19);
                    eachC.physic = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 20);
                    eachC.itemMakingEXP = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 21);
                    eachC.weapon = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 22);
                    eachC.armor = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 23);
                    eachC.powerType = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 24);
                    eachC.activePower = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 25);
                    eachC.maxPower = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 26);
                    eachC.attack = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 27);
                    eachC.move = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 28);
                    eachC.defence = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 29);
                    eachC.medical = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 30);
                    eachC.tox = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 31);
                    eachC.detox = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 32);
                    eachC.antiTox = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 33);
                    eachC.fist = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 34);
                    eachC.sword = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 35);
                    eachC.blade = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 36);
                    eachC.special = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 37);
                    eachC.hidden = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 38);
                    eachC.knowledge = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 39);
                    eachC.moral = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 40);
                    eachC.attackWithTox = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 41);
                    eachC.leftRight = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 42);
                    eachC.reputation = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 43);
                    eachC.intelligence = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 44);
                    eachC.trainingItem = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 45);
                    eachC.trainingEXP = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 46);
                    for (int skillCount = 0; skillCount < eachC.skillsIDArray.Length; skillCount++)
                    {
                        eachC.skillsIDArray[skillCount] = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 47 + skillCount * 2);
                        if (eachC.skillsIDArray[skillCount] == 0)
                        {
                            eachC.skillsIDArray[skillCount] = -1;
                        }
                    }
                    for (int skillCount = 0; skillCount < eachC.skillsIDArray.Length; skillCount++)
                    {
                        eachC.skillsLevelArray[skillCount] = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 57 + skillCount * 2);
                    }
                    for (int itemCount = 0; itemCount < eachC.itemsIDList.Length; itemCount++)
                    {
                        eachC.itemsIDList[itemCount] = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 67 + itemCount * 2);
                    }
                    for (int itemCount = 0; itemCount < eachC.itemsCountArray.Length; itemCount++)
                    {
                        eachC.itemsCountArray[itemCount] = BitConverter.ToInt16(allRangerBytes, playerLength + 4 + eachCharacterSize * entryCount + 2 * 71 + itemCount * 2);
                    }
                    characterDictionary.Add(entryCount, eachC);
                }
                for (int entryCount = 0; entryCount < totalItemCount; entryCount++)
                {
                    Item eachI = new Item(entryCount);
                    eachI.itemName = Encoding.GetEncoding("BIG5").GetString(allRangerBytes.Skip(playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * 1).Take(20).ToArray()).Trim('\0');
                    eachI.itemAlias = Encoding.GetEncoding("BIG5").GetString(allRangerBytes.Skip(playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * 11).Take(20).ToArray()).Trim('\0');
                    eachI.itemDescription = Encoding.GetEncoding("BIG5").GetString(allRangerBytes.Skip(playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * 21).Take(30).ToArray()).Trim('\0');
                    selfIncreaseCount = 36;
                    eachI.itemSkill = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.hiddenAnimeNumber = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.user = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.equipType = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.showDescription = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.type = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.addActiveLife = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.addMaxLife = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.addPoisened = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.addPhysical = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.changePowerType = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.addActivePower = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.addMaxPower = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.addAttack = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.addMove = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.addDefence = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.addMedical = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.addTox = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.addDetox = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.addAntiTox = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.addFist = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.addSword = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.addBlade = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.addSpecial = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.addHidden = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.addKnowledge = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.addMoral = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.addLeftRight = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.addAttackWithTox = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.forCharacter = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.needPowerType = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.needActivePower = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.needAttack = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.needMove = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.needTox = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.needMedical = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.needDetox = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.needFist = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.needSword = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.needBlade = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.needSpecial = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.needHidden = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.needIntelligence = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.needEXP = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.itemMakingNeedEXP = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    eachI.needMaterial = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * SelfIncreaseCount);
                    for (int itemCount = 0; itemCount < eachI.itemsIDList.Length; itemCount++)
                    {
                        eachI.itemsIDList[itemCount] = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * 82 + itemCount * 2);
                    }
                    for (int itemCount = 0; itemCount < eachI.itemsCountArray.Length; itemCount++)
                    {
                        eachI.itemsCountArray[itemCount] = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + 4 + eachItemSize * entryCount + 2 * 87 + itemCount * 2);
                    }
                    itemDictionary.Add(entryCount, eachI);
                }
                for (int entryCount = 0; entryCount < totalSkillCount; entryCount++)
                {
                    Skill eachS = new Skill(entryCount);
                    eachS.skillName = Encoding.GetEncoding("BIG5").GetString(allRangerBytes.Skip(playerLength + characterLength + itemLength + 4 + eachSkillSize * entryCount + 2 * 1).Take(10).ToArray()).Trim('\0');
                    selfIncreaseCount = 6;
                    eachS.soundID = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + itemLength + 4 + eachSkillSize * entryCount + 2 * SelfIncreaseCount);
                    eachS.skillType = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + itemLength + 4 + eachSkillSize * entryCount + 2 * SelfIncreaseCount);
                    eachS.skillAnimeNumber = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + itemLength + 4 + eachSkillSize * entryCount + 2 * SelfIncreaseCount);
                    eachS.attackType = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + itemLength + 4 + eachSkillSize * entryCount + 2 * SelfIncreaseCount);
                    eachS.attackRange = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + itemLength + 4 + eachSkillSize * entryCount + 2 * SelfIncreaseCount);
                    eachS.powerCost = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + itemLength + 4 + eachSkillSize * entryCount + 2 * SelfIncreaseCount);
                    eachS.tox = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + itemLength + 4 + eachSkillSize * entryCount + 2 * SelfIncreaseCount);
                    for (int skillCount = 0; skillCount < eachS.damageArray.Length; skillCount++)
                    {
                        eachS.damageArray[skillCount] = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + itemLength + 4 + eachSkillSize * entryCount + 2 * 13 + skillCount * 2);
                    }
                    for (int skillCount = 0; skillCount < eachS.moveArray.Length; skillCount++)
                    {
                        eachS.moveArray[skillCount] = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + itemLength + 4 + eachSkillSize * entryCount + 2 * 23 + skillCount * 2);
                    }
                    for (int skillCount = 0; skillCount < eachS.affectArray.Length; skillCount++)
                    {
                        eachS.affectArray[skillCount] = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + itemLength + 4 + eachSkillSize * entryCount + 2 * 33 + skillCount * 2);
                    }
                    for (int skillCount = 0; skillCount < eachS.addPowerArray.Length; skillCount++)
                    {
                        eachS.addPowerArray[skillCount] = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + itemLength + 4 + eachSkillSize * entryCount + 2 * 43 + skillCount * 2);
                    }
                    for (int skillCount = 0; skillCount < eachS.removePowerArray.Length; skillCount++)
                    {
                        eachS.removePowerArray[skillCount] = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + itemLength + 4 + eachSkillSize * entryCount + 2 * 53 + skillCount * 2);
                    }
                    skillDictionary.Add(entryCount, eachS);
                }
                for (int entryCount = 0; entryCount < totalShopCount; entryCount++)
                {
                    Shop eachS = new Shop(entryCount);
                    for (int checkItemCount = 0; checkItemCount < eachS.itemArray.Length; checkItemCount++)
                    {
                        eachS.itemArray[checkItemCount] = new SellingItem();
                        eachS.itemArray[checkItemCount].itemID = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + itemLength + skillLength + 4 + eachShopSize * entryCount + 2 * 0 + checkItemCount * 2);
                    }
                    for (int checkItemCount = 0; checkItemCount < eachS.itemArray.Length; checkItemCount++)
                    {
                        eachS.itemArray[checkItemCount].itemCount = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + itemLength + skillLength + 4 + eachShopSize * entryCount + 2 * 5 + checkItemCount * 2);
                    }
                    for (int checkItemCount = 0; checkItemCount < eachS.itemArray.Length; checkItemCount++)
                    {
                        eachS.itemArray[checkItemCount].RequireItemAmount = BitConverter.ToInt16(allRangerBytes, playerLength + characterLength + itemLength + skillLength + 4 + eachShopSize * entryCount + 2 * 10 + checkItemCount * 2);
                    }
                    shopDictionary.Add(entryCount, eachS);
                }

                string allScenePath = string.Format(AppDomain.CurrentDomain.BaseDirectory + "resource\\save\\scene{0}.np", pmFileIndex);
                if (!System.IO.File.Exists(allScenePath))
                {
                    throw new Exception();
                }
                sceneMapDictionary = new Dictionary<int, SceneMap>();
                byte[] allSceneBytes = System.IO.File.ReadAllBytes(allScenePath);
                int totalCount = BitConverter.ToInt32(allSceneBytes, 0);
                int declarationSize = 0x8000;
                int declarationStartPos = 4;
                int eventStartPos = declarationStartPos + totalCount * 0x8000;
                int eventSize = 0x1130;
                int eachEventSize = 0x16;
                int checkCount = 0;
                int declarationMarkCount = 0;
                int entranceStartPos = declarationStartPos + totalCount * declarationSize + totalCount * eventSize;
                int entranceSize = 0x2a;
                while (checkCount < totalCount)
                {
                    declarationMarkCount = 0;
                    SceneMap eachSM = new SceneMap(checkCount);
                    eachSM.exitMusicID = BitConverter.ToInt16(allSceneBytes, entranceStartPos + checkCount * entranceSize + 2 * 1);
                    eachSM.enterMusicID = BitConverter.ToInt16(allSceneBytes, entranceStartPos + checkCount * entranceSize + 2 * 2);
                    int subSceneID = BitConverter.ToInt16(allSceneBytes, entranceStartPos + checkCount * entranceSize + 2 * 3);
                    eachSM.opened = BitConverter.ToInt16(allSceneBytes, entranceStartPos + checkCount * entranceSize + 2 * 4);
                    int mmapCoordinateX1 = BitConverter.ToInt16(allSceneBytes, entranceStartPos + checkCount * entranceSize + 2 * 5);
                    int mmapCoordinateY1 = BitConverter.ToInt16(allSceneBytes, entranceStartPos + checkCount * entranceSize + 2 * 6);
                    int mmapCoordinateX2 = BitConverter.ToInt16(allSceneBytes, entranceStartPos + checkCount * entranceSize + 2 * 7);
                    int mmapCoordinateY2 = BitConverter.ToInt16(allSceneBytes, entranceStartPos + checkCount * entranceSize + 2 * 8);
                    if (mmapCoordinateX1 > 0 || mmapCoordinateY1 > 0)
                    {
                        mainMap.entranceMatrix[mmapCoordinateX1, mmapCoordinateY1] = new Entrance();
                        mainMap.entranceMatrix[mmapCoordinateX1, mmapCoordinateY1].destMapType = MapType.Scene;
                        mainMap.entranceMatrix[mmapCoordinateX1, mmapCoordinateY1].sceneID = checkCount;
                    }
                    if (mmapCoordinateX2 > 0 || mmapCoordinateY2 > 0)
                    {
                        mainMap.entranceMatrix[mmapCoordinateX2, mmapCoordinateY2] = new Entrance();
                        mainMap.entranceMatrix[mmapCoordinateX2, mmapCoordinateY2].destMapType = MapType.Scene;
                        mainMap.entranceMatrix[mmapCoordinateX2, mmapCoordinateY2].sceneID = checkCount;
                    }
                    eachSM.playerEnterCoordinateX = BitConverter.ToInt16(allSceneBytes, entranceStartPos + checkCount * entranceSize + 2 * 9);
                    eachSM.playerEnterCoordinateY = BitConverter.ToInt16(allSceneBytes, entranceStartPos + checkCount * entranceSize + 2 * 10);
                    int exitCoordinateX1 = BitConverter.ToInt16(allSceneBytes, entranceStartPos + checkCount * entranceSize + 2 * 11);
                    int exitCoordinateX2 = BitConverter.ToInt16(allSceneBytes, entranceStartPos + checkCount * entranceSize + 2 * 12);
                    int exitCoordinateX3 = BitConverter.ToInt16(allSceneBytes, entranceStartPos + checkCount * entranceSize + 2 * 13);
                    int exitCoordinateY1 = BitConverter.ToInt16(allSceneBytes, entranceStartPos + checkCount * entranceSize + 2 * 14);
                    int exitCoordinateY2 = BitConverter.ToInt16(allSceneBytes, entranceStartPos + checkCount * entranceSize + 2 * 15);
                    int exitCoordinateY3 = BitConverter.ToInt16(allSceneBytes, entranceStartPos + checkCount * entranceSize + 2 * 16);
                    if (exitCoordinateX1 > 0 || exitCoordinateY1 > 0)
                    {
                        eachSM.entranceMatrix[exitCoordinateX1, exitCoordinateY1] = new Entrance();
                        eachSM.entranceMatrix[exitCoordinateX1, exitCoordinateY1].destMapType = MapType.World;
                        eachSM.entranceMatrix[exitCoordinateX1, exitCoordinateY1].sceneID = -1;
                    }
                    if (exitCoordinateX2 > 0 || exitCoordinateY2 > 0)
                    {
                        eachSM.entranceMatrix[exitCoordinateX2, exitCoordinateY2] = new Entrance();
                        eachSM.entranceMatrix[exitCoordinateX2, exitCoordinateY2].destMapType = MapType.World;
                        eachSM.entranceMatrix[exitCoordinateX2, exitCoordinateY2].sceneID = -1;
                    }
                    if (exitCoordinateX3 > 0 || exitCoordinateY3 > 0)
                    {
                        eachSM.entranceMatrix[exitCoordinateX3, exitCoordinateY3] = new Entrance();
                        eachSM.entranceMatrix[exitCoordinateX3, exitCoordinateY3].destMapType = MapType.World;
                        eachSM.entranceMatrix[exitCoordinateX3, exitCoordinateY3].sceneID = -1;
                    }
                    int enterSubSceneCoordinateX1 = BitConverter.ToInt16(allSceneBytes, entranceStartPos + checkCount * entranceSize + 2 * 17);
                    int enterSubSceneCoordinateY1 = BitConverter.ToInt16(allSceneBytes, entranceStartPos + checkCount * entranceSize + 2 * 18);
                    int enterSubSceneCoordinateX2 = BitConverter.ToInt16(allSceneBytes, entranceStartPos + checkCount * entranceSize + 2 * 19);
                    int enterSubSceneCoordinateY2 = BitConverter.ToInt16(allSceneBytes, entranceStartPos + checkCount * entranceSize + 2 * 20);
                    if (enterSubSceneCoordinateX1 > 0 || enterSubSceneCoordinateY1 > 0)
                    {
                        eachSM.entranceMatrix[enterSubSceneCoordinateX1, enterSubSceneCoordinateY1] = new Entrance();
                        eachSM.entranceMatrix[enterSubSceneCoordinateX1, enterSubSceneCoordinateY1].destMapType = MapType.Scene;
                        eachSM.entranceMatrix[enterSubSceneCoordinateX1, enterSubSceneCoordinateY1].sceneID = subSceneID;
                    }
                    if (enterSubSceneCoordinateX2 > 0 || enterSubSceneCoordinateY2 > 0)
                    {
                        eachSM.entranceMatrix[enterSubSceneCoordinateX2, enterSubSceneCoordinateY2] = new Entrance();
                        eachSM.entranceMatrix[enterSubSceneCoordinateX2, enterSubSceneCoordinateY2].destMapType = MapType.Scene;
                        eachSM.entranceMatrix[enterSubSceneCoordinateX2, enterSubSceneCoordinateY2].sceneID = subSceneID;
                    }
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

                DataAdjustion();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        #endregion        
    }
}