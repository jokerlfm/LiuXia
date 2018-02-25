using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    public class Skill
    {
        public Skill(int pmID)
        {
            skillID = pmID;
            damageArray = new int[10];
            moveArray = new int[10];
            affectArray = new int[10];
            addPowerArray = new int[10];
            removePowerArray = new int[10];
        }

        public Skill(int pmID, string pmName)
        {
            skillID = pmID;
            skillName = pmName;
            damageArray = new int[10];
            moveArray = new int[10];
            affectArray = new int[10];
            addPowerArray = new int[10];
            removePowerArray = new int[10];
        }

        #region declaration
        public int skillID = 0;
        public string skillName = "";
        public int soundID = 0;
        public int skillType = 0;
        public int skillAnimeNumber = 0;
        public int attackType = 0;
        public int attackRange = 0;
        public int powerCost = 0;
        public int tox = 0;
        public int[] damageArray;
        public int[] moveArray;
        public int[] affectArray;
        public int[] addPowerArray;
        public int[] removePowerArray;
        #endregion

        #region business

        #endregion
    }
}
