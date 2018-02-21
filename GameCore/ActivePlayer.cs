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
            state = PlayerState.PlayerState_World;
            this.playerWorldUnit = new DynamicUnit();
            this.playerSceneUnit = new DynamicUnit();
            this.playerBattleUnit = new BattleUnit();
        }

        #region declaration                
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
            GameService.GetGame().currentSmapID = pmSmapID;
            state = PlayerState.PlayerState_Scene;
        }

        public void EnterBattle(int pmWmapID)
        {
            GameService.GetGame().currentWmapID = pmWmapID;
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
