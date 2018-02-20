using SlimDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SlimDXTestForm
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            Preparations();
        }

        #region declaration
        Device dd;
        Sprite sp;
        #endregion

        #region event
        private void FormMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Application.Exit();
            }
        }
        #endregion

        #region business
        private void Preparations()
        {
            this.Paint += FormMain_Paint;
            Direct3D d3d = new Direct3D();
            PresentParameters newpp = new PresentParameters();
            newpp.Windowed = true;
            dd = new Device(new Direct3D(), 0, DeviceType.Hardware, this.Handle, CreateFlags.HardwareVertexProcessing, new PresentParameters()
            {
                BackBufferWidth = this.ClientSize.Width,
                BackBufferHeight = this.ClientSize.Height
            });

            //Device dd = new Device(d3d, 0, DeviceType.Hardware, this.Handle, CreateFlags.PureDevice, newpp);
            sp = new Sprite(dd);           
        }

        private void FormMain_Paint(object sender, PaintEventArgs e)
        {
            dd.BeginScene();
            dd.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            string tFilePath = @"D:\Dev\JYQXZ\LiuXia\GameForm\bin\x86\Debug\resource\temp\0.png";
            Texture t = Texture.FromFile(dd, tFilePath);
            SlimDX.Color4 cl = new SlimDX.Color4(Color.White);
            sp.Begin(SpriteFlags.AlphaBlend);
            sp.Draw(t, null, cl);
            sp.End();
            dd.EndScene();
            dd.Present();
        }
        #endregion
    }
}
