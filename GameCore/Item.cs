using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    public class Item
    {
        public Item(int pmID)
        {
            itemID = pmID;
            itemsIDArray = new int[5];
            itemsCountArray = new int[5];
        }

        #region declaration
        public int itemID = 0;
        public string itemName = "";
        public string itemAlias = "";
        public string itemDescription = "";
        public int itemSkill = 0;
        public int hiddenAnimeNumber = 0;
        public int user = 0;
        public int equipType = 0;
        public int showDescription = 0;
        public int type = 0;
        public int addActiveLife = 0;
        public int addMaxLife = 0;
        public int addPoisened = 0;
        public int addPhysical = 0;
        public int changePowerType = 0;
        public int addActivePower = 0;
        public int addMaxPower = 0;
        public int addAttack = 0;
        public int addMove = 0;
        public int addDefence = 0;
        public int addMedical = 0;
        public int addTox = 0;
        public int addDetox = 0;
        public int addAntiTox = 0;
        public int addFist = 0;
        public int addSword = 0;
        public int addBlade = 0;
        public int addSpecial = 0;
        public int addHidden = 0;
        public int addKnowledge = 0;
        public int addMoral = 0;
        public int addLeftRight = 0;
        public int addAttackWithTox = 0;
        public int forCharacter = 0;
        public int needPowerType = 0;
        public int needActivePower = 0;
        public int needMaxPower = 0;
        public int needAttack = 0;
        public int needMove = 0;
        public int needDefence = 0;
        public int needMedical = 0;
        public int needTox = 0;
        public int needDetox = 0;
        public int needAntiTox = 0;
        public int needFist = 0;
        public int needSword = 0;
        public int needBlade = 0;
        public int needSpecial = 0;
        public int needHidden = 0;
        public int needIntelligence = 0;
        public int needEXP = 0;
        public int itemMakingNeedEXP = 0;
        public int needMaterial = 0;
        public int[] itemsIDArray;
        public int[] itemsCountArray;
        #endregion

        #region business

        #endregion
    }
}
