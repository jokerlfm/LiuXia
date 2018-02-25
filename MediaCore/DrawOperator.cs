using SlimDX;
using SlimDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;

namespace MediaCore
{
    public class DrawOperator
    {
        public DrawOperator(IntPtr pmTargetHandle, int pmWidth, int pmHeight, float pmUnitTextureScaling, float pmPortraitTextureScaling, float pmItemTextureScaling)
        {
            this.clientWidth = pmWidth;
            this.clientHeight = pmHeight;
            this.targetHandle = pmTargetHandle;
            this.white = new Color4(Color.White);
            this.black = new Color4(Color.Black);
            Direct3D d3d = new Direct3D();
            this.mainDevice = new Device(new Direct3D(), 0, DeviceType.Hardware, targetHandle, CreateFlags.HardwareVertexProcessing, new PresentParameters()
            {
                BackBufferWidth = pmWidth,
                BackBufferHeight = pmHeight,
                Windowed = true
            });
            menuTextSprite = new Sprite(mainDevice);
            this.unitTextureSprite = new Sprite(mainDevice);
            portraitTextureSprite = new Sprite(mainDevice);
            itemTextureSprite = new Sprite(mainDevice);
            Matrix unitZoomMatrix = new Matrix();
            Matrix.Scaling(pmUnitTextureScaling, pmUnitTextureScaling, 1, out unitZoomMatrix);
            this.unitTextureSprite.Transform = unitZoomMatrix;
            Matrix portraitZoomMatrix = new Matrix();
            Matrix.Scaling(pmPortraitTextureScaling, pmPortraitTextureScaling, 1, out portraitZoomMatrix);
            portraitTextureSprite.Transform = portraitZoomMatrix;
            Matrix itemZoomMatrix = new Matrix();
            Matrix.Scaling(pmItemTextureScaling, pmItemTextureScaling, 1, out itemZoomMatrix);
            itemTextureSprite.Transform = itemZoomMatrix;
            string fontPath = AppDomain.CurrentDomain.BaseDirectory + "resource\\font.ttc";
            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddFontFile(fontPath);
            System.Drawing.Font menuFileFont = new System.Drawing.Font(pfc.Families[0], pmWidth * 0.02f);
            this.menuTCFont = new SlimDX.Direct3D9.Font(mainDevice, menuFileFont);
            System.Drawing.Font detailsFileFont = new System.Drawing.Font(pfc.Families[0], pmWidth * 0.02f);
            this.detailsTCFont = new SlimDX.Direct3D9.Font(mainDevice, detailsFileFont);
        }

        #region declaratoin
        public SlimDX.Direct2D.Factory factory2D;
        public SlimDX.Direct2D.WindowRenderTarget rt2D;
        public SlimDX.Direct2D.Brush brush2D;
        public Device mainDevice;
        public Sprite unitTextureSprite;
        public Sprite menuTextSprite;
        public Sprite portraitTextureSprite;
        public Sprite itemTextureSprite;
        Color4 white;
        Color4 black;
        SlimDX.Direct3D9.Font menuTCFont;
        SlimDX.Direct3D9.Font detailsTCFont;
        public int clientWidth = 0, clientHeight = 0;
        public IntPtr targetHandle;
        #endregion

        #region business
        public void ResizeDevice(int pmWidth, int pmHeight)
        {
            this.clientWidth = pmWidth;
            this.clientHeight = pmHeight;
            this.mainDevice.Reset(new PresentParameters()
            {
                BackBufferWidth = pmWidth,
                BackBufferHeight = pmHeight,
                Windowed = true
            });

            string fontPath = AppDomain.CurrentDomain.BaseDirectory + "resource\\font.ttc";
            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddFontFile(fontPath);
            System.Drawing.Font menuFileFont = new System.Drawing.Font(pfc.Families[0], pmWidth * 0.02f);
            this.menuTCFont = new SlimDX.Direct3D9.Font(mainDevice, menuFileFont);
            System.Drawing.Font detailsFileFont = new System.Drawing.Font(pfc.Families[0], pmWidth * 0.02f);
            this.detailsTCFont = new SlimDX.Direct3D9.Font(mainDevice, detailsFileFont);
        }

        public void BeginDrawing()
        {
            mainDevice.BeginScene();
            mainDevice.Clear(ClearFlags.Target, black, 0f, 0);
        }

        public void EndDrawing()
        {
            mainDevice.EndScene();
            mainDevice.Present();
        }

        public void BeginUnitDrawing()
        {
            unitTextureSprite.Begin(SpriteFlags.AlphaBlend);
        }

        public void EndUnitDrawing()
        {
            unitTextureSprite.End();
        }

        public void BeginPortraitDrawing()
        {
            portraitTextureSprite.Begin(SpriteFlags.AlphaBlend);
        }

        public void BeginItemDrawing()
        {
            itemTextureSprite.Begin(SpriteFlags.AlphaBlend);
        }

        public void BeginMenuDrawing()
        {
            menuTextSprite.Begin(SpriteFlags.AlphaBlend);

        }

        public void EndMenuDrawing()
        {
            menuTextSprite.End();
        }

        public void EndPortraitDrawing()
        {
            portraitTextureSprite.End();
        }

        public void EndItemDrawing()
        {
            itemTextureSprite.End();
        }

        public void DrawMenuText(string pmText, float pmScreenPosRateX, float pmScreenPosRateY, Color pmColor)
        {
            int screenPosX = (int)(pmScreenPosRateX * clientWidth);
            int screenPosY = (int)(pmScreenPosRateY * clientHeight);

            this.menuTCFont.DrawString(menuTextSprite, pmText, screenPosX, screenPosY, pmColor);
        }

        public void DrawDetailsText(string pmText, float pmScreenPosRateX, float pmScreenPosRateY, Color pmColor)
        {
            int screenPosX = (int)(pmScreenPosRateX * clientWidth);
            int screenPosY = (int)(pmScreenPosRateY * clientHeight);

            this.detailsTCFont.DrawString(menuTextSprite, pmText, screenPosX, screenPosY, pmColor);
        }

        public void DrawBytes(byte[] pmBytes, int pmPosX, int pmPosY, int pmGapX, int pmGapY, int pmWidth, int pmHeight)
        {
            Texture t = Texture.FromMemory(mainDevice, pmBytes);
            Rectangle r = new Rectangle(pmPosX + pmGapX, pmPosY + pmGapY, pmWidth, pmHeight);
            unitTextureSprite.Draw(t, r, white);
            t.Dispose();
        }

        public void DrawUnitTexture(Texture pmTexture, float pmCameraPosX, float pmCameraPosY, float pmScreenPosX, float pmScreenPosY)
        {
            unitTextureSprite.Draw(pmTexture, null, new Vector3(pmCameraPosX, pmCameraPosY, 0f), new Vector3(pmScreenPosX, pmScreenPosY, 0f), white);
        }

        public void DrawPortraitTexture(Texture pmTexture, float pmScreenPosX, float pmScreenPosY)
        {
            portraitTextureSprite.Draw(pmTexture, null, new Vector3(0, 0, 0f), new Vector3(pmScreenPosX, pmScreenPosY, 0f), white);
        }

        public void DrawItemTexture(Texture pmTexture, float pmScreenPosX, float pmScreenPosY)
        {
            itemTextureSprite.Draw(pmTexture, null, new Vector3(0, 0, 0f), new Vector3(pmScreenPosX, pmScreenPosY, 0f), white);
        }

        public void DrawRateLine(float pmStartPosRateX, float pmStartPosRateY, float pmEndPosRateX, float pmEndPosRateY, Color pmColor)
        {
            float startScreenPosX = clientWidth * pmStartPosRateX;
            float startScreenPosY = clientHeight * pmStartPosRateY;
            float endScreenPosX = clientWidth * pmEndPosRateX;
            float endScreenPosY = clientHeight * pmEndPosRateY;

            var vertexElems = new[] {
                new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.PositionTransformed, 0),
                new VertexElement(0, 16, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
                VertexElement.VertexDeclarationEnd
            };

            var vertices = new VertexBuffer(mainDevice, 2 * 20, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
            vertices.Lock(0, 0, LockFlags.None).WriteRange(new[] {
                new Vertex() { vertexColor = pmColor.ToArgb(), vertexPosition = new Vector4(startScreenPosX, startScreenPosY, 0f, 1f) },
                new Vertex() { vertexColor = pmColor.ToArgb(), vertexPosition = new Vector4(endScreenPosX, endScreenPosY, 0f,  1f) },
                });
            vertices.Unlock();

            var insideVertexDecl = new VertexDeclaration(mainDevice, vertexElems);

            mainDevice.SetStreamSource(0, vertices, 0, 20);
            mainDevice.VertexDeclaration = insideVertexDecl;
            mainDevice.DrawPrimitives(PrimitiveType.LineList, 0, 1);
        }

        public void DrawRateLineRectangle(float pmStartPosRateX, float pmStartPosRateY, float pmEndPosRateX, float pmEndPosRateY, Color pmBorderColor, Color pmInnerColor)
        {
            float startScreenPosX = clientWidth * pmStartPosRateX;
            float startScreenPosY = clientHeight * pmStartPosRateY;
            float endScreenPosX = clientWidth * pmEndPosRateX;
            float endScreenPosY = clientHeight * pmEndPosRateY;

            var vertexElems = new[] {
                new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.PositionTransformed, 0),
                new VertexElement(0, 16, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
                VertexElement.VertexDeclarationEnd
            };

            if (pmInnerColor.A > 0)
            {
                var insideVertices = new VertexBuffer(mainDevice, 4 * 20, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
                insideVertices.Lock(0, 0, LockFlags.None).WriteRange(new[] {
                new Vertex() { vertexColor = pmInnerColor.ToArgb(), vertexPosition = new Vector4(startScreenPosX, startScreenPosY, 0f, 1f) },
                new Vertex() { vertexColor = pmInnerColor.ToArgb(), vertexPosition = new Vector4(endScreenPosX, startScreenPosY, 0f,  1f) },

                new Vertex() { vertexColor = pmInnerColor.ToArgb(), vertexPosition = new Vector4(startScreenPosX, endScreenPosY, 0f,  1f) },
                new Vertex() { vertexColor = pmInnerColor.ToArgb(), vertexPosition = new Vector4(endScreenPosX, endScreenPosY, 0f,  1f) },
                });
                insideVertices.Unlock();

                var insideVertexDecl = new VertexDeclaration(mainDevice, vertexElems);

                mainDevice.SetStreamSource(0, insideVertices, 0, 20);
                mainDevice.VertexDeclaration = insideVertexDecl;
                mainDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            }

            var borderVertices = new VertexBuffer(mainDevice, 8 * 20, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
            borderVertices.Lock(0, 0, LockFlags.None).WriteRange(new[] {
                new Vertex() { vertexColor = pmBorderColor.ToArgb(), vertexPosition = new Vector4(startScreenPosX, startScreenPosY, 0f, 1) },
                new Vertex() { vertexColor = pmBorderColor.ToArgb(), vertexPosition = new Vector4(endScreenPosX, startScreenPosY, 0f, 1) },

                new Vertex() { vertexColor = pmBorderColor.ToArgb(), vertexPosition = new Vector4(startScreenPosX, startScreenPosY, 0f, 1) },
                new Vertex() { vertexColor = pmBorderColor.ToArgb(), vertexPosition = new Vector4(startScreenPosX, endScreenPosY, 0f, 1) },

                new Vertex() { vertexColor = pmBorderColor.ToArgb(), vertexPosition = new Vector4(endScreenPosX, startScreenPosY, 0f, 1) },
                new Vertex() { vertexColor = pmBorderColor.ToArgb(), vertexPosition = new Vector4(endScreenPosX, endScreenPosY, 0f, 1) },

                new Vertex() { vertexColor = pmBorderColor.ToArgb(), vertexPosition = new Vector4(startScreenPosX, endScreenPosY, 0f, 1) },
                new Vertex() { vertexColor = pmBorderColor.ToArgb(), vertexPosition = new Vector4(endScreenPosX, endScreenPosY, 0f, 1) },
            });
            borderVertices.Unlock();

            var borderVertexDecl = new VertexDeclaration(mainDevice, vertexElems);

            mainDevice.SetStreamSource(0, borderVertices, 0, 20);
            mainDevice.VertexDeclaration = borderVertexDecl;
            mainDevice.DrawPrimitives(PrimitiveType.LineList, 0, 4);
        }

        public Texture CreateTexture(byte[] pmBytes, int pmWidth, int pmHeight)
        {
            return Texture.FromMemory(mainDevice, pmBytes, pmWidth, pmHeight, 1, Usage.None, Format.A8R8G8B8, Pool.Managed, Filter.Linear, Filter.None, 0);
        }
        #endregion
    }

    struct Vertex
    {
        public Vertex(Vector4 pmPosition, int pmColor)
        {
            vertexPosition = pmPosition;
            vertexColor = pmColor;
        }

        public Vector4 vertexPosition;
        public int vertexColor;

        public byte[] GetAllBytes()
        {
            byte[] result = new byte[20];
            Buffer.BlockCopy(BitConverter.GetBytes(vertexPosition.X), 0, result, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(vertexPosition.Y), 0, result, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(vertexPosition.Z), 0, result, 8, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(vertexPosition.W), 0, result, 12, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(vertexColor), 0, result, 16, 4);
            return result;
        }
    }
}