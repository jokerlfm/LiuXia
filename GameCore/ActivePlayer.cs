using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    public class ActivePlayer
    {
        public ActivePlayer()
        {
            this.playerWorldUnit = new DynamicUnit();
            this.playerSceneUnit = new DynamicUnit();
            this.playerBattleUnit = new BattleUnit();
        }

        #region declaration      
        public DynamicUnit playerWorldUnit;
        public DynamicUnit playerSceneUnit;
        public BattleUnit playerBattleUnit;
        #endregion

        #region business

        #endregion
    }
}
