using SlimDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MediaCore
{
    public class DrawOperator
    {
        public DrawOperator(IntPtr pmTargetHandle, int pmWidth, int pmHeight, float pmZoom)
        {
            this.white = new SlimDX.Color4(Color.White);
            this.black = new SlimDX.Color4(Color.Black);
            Direct3D d3d = new Direct3D();
            this.mainDevice = new Device(new Direct3D(), 0, DeviceType.Hardware, pmTargetHandle, CreateFlags.HardwareVertexProcessing, new PresentParameters()
            {
                BackBufferWidth = pmWidth,
                BackBufferHeight = pmHeight,
                Windowed = true
            });
            this.mainSprite = new Sprite(mainDevice);
            SlimDX.Matrix zoomMatrix = new SlimDX.Matrix();
            SlimDX.Matrix.Scaling(pmZoom, pmZoom, 1, out zoomMatrix);
            this.mainSprite.Transform = zoomMatrix;
        }

        #region declaratoin
        public Device mainDevice;
        public Sprite mainSprite;
        SlimDX.Color4 white;
        SlimDX.Color4 black;
        #endregion

        #region business
        public void ResizeDevice(int pmWidth, int pmHeight)
        {
            this.mainDevice.Reset(new PresentParameters()
            {
                BackBufferWidth = pmWidth,
                BackBufferHeight = pmHeight,
                Windowed = true
            });
        }

        public void BeginDraw()
        {
            mainDevice.BeginScene();
            mainDevice.Clear(ClearFlags.Target, black, 0f, 0);
            mainSprite.Begin(SpriteFlags.AlphaBlend);
        }

        public void DrawBytes(byte[] pmBytes, int pmPosX, int pmPosY, int pmGapX, int pmGapY, int pmWidth, int pmHeight)
        {
            Texture t = Texture.FromMemory(mainDevice, pmBytes);
            Rectangle r = new Rectangle(pmPosX + pmGapX, pmPosY + pmGapY, pmWidth, pmHeight);
            mainSprite.Draw(t, r, white);
            t.Dispose();
        }

        public void DrawTexture(Texture pmTexture, float pmCameraPosX, float pmCameraPosY, float pmScreenPosX, float pmScreenPosY)
        {
            mainSprite.Draw(pmTexture, null, new SlimDX.Vector3(pmCameraPosX, pmCameraPosY, 0f), new SlimDX.Vector3(pmScreenPosX, pmScreenPosY, 0f), white);
        }

        public void EndDraw()
        {
            mainSprite.End();
            mainDevice.EndScene();
            mainDevice.Present();
        }

        public Texture CreateTexture(byte[] pmBytes, int pmWidth, int pmHeight)
        {
            return Texture.FromMemory(mainDevice, pmBytes, pmWidth, pmHeight, 1, Usage.None, Format.A8R8G8B8, Pool.Default, Filter.None, Filter.None, 0);
        }
        #endregion
    }
}