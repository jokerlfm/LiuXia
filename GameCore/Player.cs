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

            partyMembersArray = new int[8];
            itemsDictionary = new Dictionary<int, int>(1000);
        }

        #region declaration        
        public int boating = 0, playerFaceDirection = 0, mmapCoordX = 0, mmapCoordY = 0, smapCoordX = 0, smapCoordY = 0, boatCoordX1 = 0, boatCoordY1 = 0, boatCoordX2 = 0, boatCoordY2 = 0, boatFaceDirection = 0;
        public int[] partyMembersArray;
        Dictionary<int, int> itemsDictionary;

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
