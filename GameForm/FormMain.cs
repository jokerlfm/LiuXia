using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GameForm
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            Preparations();
        }

        #region declaration
        GameCore.GameService targetGame;
        #endregion

        #region business
        private void Preparations()
        {
            targetGame = GameCore.GameService.CreateGame("JYQXZ", this);
            targetGame.Run();

            WaitCallback wcbGlobalCheck = new WaitCallback(this.GlobalCheck);
            ThreadPool.QueueUserWorkItem(wcbGlobalCheck, null);
        }

        private void GlobalCheck(object pmMain = null)
        {
            while (targetGame.state != GameCore.GameRunningState.GameRunningState_Finished)
            {
                Thread.Sleep(200);
            }
            Application.Exit();
        }
        #endregion
    }
}