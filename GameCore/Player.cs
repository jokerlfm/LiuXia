using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    public class Player
    {
        public Player()
        {
            state = PlayerState.PlayerState_World;
            this.playerWorldUnit = new DynamicUnit();
            this.playerSceneUnit = new DynamicUnit();
            this.playerBattleUnit = new BattleUnit();
        }

        #region declaration
        public int smapID = 0, wmapID = 0;
        public int mmapCoordX = 0, mmapCoordY = 0, smapCoordX = 0, smapCoordY = 0, wmapCoordX = 0, wmapCoordY = 0;
        public PlayerState state;

        public DynamicUnit playerWorldUnit;
        public DynamicUnit playerSceneUnit;
        public BattleUnit playerBattleUnit;
        #endregion

        #region business
        public void EnterWorld()
        {
            state = PlayerState.PlayerState_World;
        }

        public void EnterScene(int pmSmapID, int pmEntranceNumber)
        {
            this.smapID = pmSmapID;
            state = PlayerState.PlayerState_Scene;
        }

        public void EnterBattle(int pmWmapID)
        {
            this.wmapID = pmWmapID;
            state = PlayerState.PlayerState_Battle;
        }
        #endregion
    }

    public enum PlayerState
    {
        PlayerState_World,
        PlayerState_Scene,
        PlayerState_Battle
    }
}
