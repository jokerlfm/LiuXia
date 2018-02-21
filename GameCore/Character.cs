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
            itemsIDArray = new int[4];
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
        public int[] itemsIDArray;
        public int[] itemsCountArray;
        #endregion

        #region business

        #endregion
    }
}