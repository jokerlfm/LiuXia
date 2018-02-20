using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GameForm
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //GameCore.GameService targetGame = GameCore.GameService.CreateGame("JYQXZ");
            //targetGame.Run();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }
    }
}