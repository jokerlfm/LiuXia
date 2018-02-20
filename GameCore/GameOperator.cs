using SlimDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace GameCore
{
    class GameOperator
    {
        public static bool InitGame()
        {
            try
            {
                ConfigHandler.LoadConfig();
                ResourceManager.LoadResource();

                ResizeWindow();
            }
            catch (Exception exp)
            {
                return false;
            }
            return true;
        }

        public static void ResizeWindow()
        {
            GameService destGame = GameService.GetGame();
            destGame.displayTargetForm.Width = ConfigHandler.GetConfigValue_int("WindowWidth");
            destGame.displayTargetForm.Height = ConfigHandler.GetConfigValue_int("WindowHeight");
            //destGame.pen.ResizeDevice(destGame.displayTargetForm.Width, destGame.displayTargetForm.Height);
        }

        public static void ShowWelcome()
        {
            GameService destGame = GameService.GetGame();

            destGame.runtimeState = RuntimeState.RuntimeState_Welcome;
        }

        public static void LoadGame(int pmFileIndex)
        {
            ResourceManager.LoadGameFile(pmFileIndex);
        }
    }
}