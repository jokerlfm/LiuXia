using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    public class Character
    {
        public Character(int pmID)
        {
            characterID = pmID;
            skillsIDArray = new int[10];
            skillsLevelArray = new int[10];
            itemsIDList = new int[4];
            itemsCountArray = new int[4];
        }

        #region declaration
        public int characterID = 0;
        public int characterPortrait = 0;
        public int increaseRate = 0;
        public string characterName = "";
        public string characterNickName = "";
        public int gender = 0;
        public int level = 0;
        public int exp = 0;
        public int activeLife = 0;
        public int maxLife = 0;
        public int injury = 0;
        public int poisoned = 0;
        public int physic = 0;
        public int itemMakingEXP = 0;
        public int weapon = 0;
        public int armor = 0;
        public int powerType = 0;
        public int activePower = 0;
        public int maxPower = 0;
        public int attack = 0;
        public int move = 0;
        public int defence = 0;
        public int medical = 0;
        public int tox = 0;
        public int detox = 0;
        public int antiTox = 0;
        public int fist = 0;
        public int sword = 0;
        public int blade = 0;
        public int special = 0;
        public int hidden = 0;
        public int knowledge = 0;
        public int moral = 0;
        public int attackWithTox = 0;
        public int leftRight = 0;
        public int reputation = 0;
        public int intelligence = 0;
        public int trainingItem = 0;
        public int trainingEXP = 0;
        public int[] skillsIDArray;
        public int[] skillsLevelArray;
        public int[] itemsIDList;
        public int[] itemsCountArray;

        public int maxLifeExtra = 0;
        public int maxPowerExtra = 0;
        public int attackExtra = 0;
        public int moveExtra = 0;
        public int defenceExtra = 0;
        public int antiToxExtra = 0;
        public int fistExtra = 0;
        public int swordExtra = 0;
        public int bladeExtra = 0;
        public int specialExtra = 0;
        public int hiddenExtra = 0;
        public int attackWithToxExtra = 0;
        #endregion

        #region business
        public bool EquipItem(int pmItemID)
        {
            // todo check requirement

            Item targetItem = ResourceManager.itemDictionary[pmItemID];
            if (targetItem.equipType == 0)
            {
                if (weapon >= 0)
                {
                    return false;
                }
                weapon = pmItemID;
            }
            else if (targetItem.equipType == 1)
            {
                if (armor >= 0)
                {
                    return false;
                }
                armor = pmItemID;
            }
            if (targetItem.addAntiTox != 0)
            {
                antiToxExtra += targetItem.addAntiTox;
            }
            if (targetItem.addAttack != 0)
            {
                attackExtra += targetItem.addAttack;
            }
            if (targetItem.addAttackWithTox != 0)
            {
                attackWithToxExtra += targetItem.addAttackWithTox;
            }
            if (targetItem.addBlade != 0)
            {
                bladeExtra += targetItem.addBlade;
            }
            if (targetItem.addDefence != 0)
            {
                defenceExtra += targetItem.addDefence;
            }
            if (targetItem.addMaxPower != 0)
            {
                fistExtra += targetItem.addMaxPower;
            }
            if (targetItem.addHidden != 0)
            {
                hiddenExtra += targetItem.addHidden;
            }
            if (targetItem.addMaxLife != 0)
            {
                maxLifeExtra += targetItem.addMaxLife;
            }
            if (targetItem.addMaxPower != 0)
            {
                maxPowerExtra += targetItem.addMaxPower;
            }
            if (targetItem.addMove != 0)
            {
                moveExtra += targetItem.addMove;
            }
            if (targetItem.addSpecial != 0)
            {
                specialExtra += targetItem.addSpecial;
            }
            if (targetItem.addSword != 0)
            {
                swordExtra += targetItem.addSword;
            }

            return true;
        }

        public void UnequipItem(int pmItemID)
        {
            Item targetItem = ResourceManager.itemDictionary[pmItemID];
            if (targetItem.equipType == 0)
            {
                weapon = -1;
            }
            else if (targetItem.equipType == 1)
            {
                armor = -1;
            }
            if (targetItem.addAntiTox != 0)
            {
                antiToxExtra -= targetItem.addAntiTox;
            }
            if (targetItem.addAttack != 0)
            {
                attackExtra -= targetItem.addAttack;
            }
            if (targetItem.addAttackWithTox != 0)
            {
                attackWithToxExtra -= targetItem.addAttackWithTox;
            }
            if (targetItem.addBlade != 0)
            {
                bladeExtra -= targetItem.addBlade;
            }
            if (targetItem.addDefence != 0)
            {
                defenceExtra -= targetItem.addDefence;
            }
            if (targetItem.addMaxPower != 0)
            {
                fistExtra -= targetItem.addMaxPower;
            }
            if (targetItem.addHidden != 0)
            {
                hiddenExtra -= targetItem.addHidden;
            }
            if (targetItem.addMaxLife != 0)
            {
                maxLifeExtra -= targetItem.addMaxLife;
            }
            if (targetItem.addMaxPower != 0)
            {
                maxPowerExtra -= targetItem.addMaxPower;
            }
            if (targetItem.addMove != 0)
            {
                moveExtra -= targetItem.addMove;
            }
            if (targetItem.addSpecial != 0)
            {
                specialExtra -= targetItem.addSpecial;
            }
            if (targetItem.addSword != 0)
            {
                swordExtra -= targetItem.addSword;
            }
        }

        public bool CanEquip(int itemID)
        {
            return false;
        }
        #endregion
    }
}