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
        public DrawOperator(IntPtr pmTargetHandle, int pmWidth, int pmHeight, float pmZoom)
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
            this.unitSprite = new Sprite(mainDevice);
            //Matrix zoomMatrix = new SlimDX.Matrix();
            //Matrix.Scaling(pmZoom, pmZoom, 1, out zoomMatrix);            
            //this.textureSprite.Transform = zoomMatrix;

            menuSprite = new Sprite(mainDevice);
            string fontPath = AppDomain.CurrentDomain.BaseDirectory + "resource\\font.ttc";
            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddFontFile(fontPath);
            System.Drawing.Font fileFont = new System.Drawing.Font(pfc.Families[0], pmWidth * 0.025f);
            this.menuTCFont = new SlimDX.Direct3D9.Font(mainDevice, fileFont);
        }

        #region declaratoin
        public SlimDX.Direct2D.Factory factory2D;
        public SlimDX.Direct2D.WindowRenderTarget rt2D;
        public SlimDX.Direct2D.Brush brush2D;
        public Device mainDevice;
        public Sprite unitSprite;
        public Sprite menuSprite;
        Color4 white;
        Color4 black;
        SlimDX.Direct3D9.Font menuTCFont;
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
            System.Drawing.Font fileFont = new System.Drawing.Font(pfc.Families[0], pmWidth * 0.025f);
            this.menuTCFont = new SlimDX.Direct3D9.Font(mainDevice, fileFont);
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
            unitSprite.Begin(SpriteFlags.AlphaBlend);
        }

        public void EndUnitDrawing()
        {
            unitSprite.End();
        }

        public void BeginMenuDrawing()
        {
            menuSprite.Begin(SpriteFlags.AlphaBlend);
        }

        public void EndMenuDrawing()
        {
            menuSprite.End();
        }

        public void DrawText(string pmText, float pmScreenPosRateX, float pmScreenPosRateY, Color pmColor)
        {
            int screenPosX = (int)(pmScreenPosRateX * clientWidth);
            int screenPosY = (int)(pmScreenPosRateY * clientHeight);

            this.menuTCFont.DrawString(menuSprite, pmText, screenPosX, screenPosY, pmColor);
        }

        public void DrawBytes(byte[] pmBytes, int pmPosX, int pmPosY, int pmGapX, int pmGapY, int pmWidth, int pmHeight)
        {
            Texture t = Texture.FromMemory(mainDevice, pmBytes);
            Rectangle r = new Rectangle(pmPosX + pmGapX, pmPosY + pmGapY, pmWidth, pmHeight);
            unitSprite.Draw(t, r, white);
            t.Dispose();
        }

        public void DrawTexture(Texture pmTexture, float pmCameraPosX, float pmCameraPosY, float pmScreenPosX, float pmScreenPosY)
        {
            unitSprite.Draw(pmTexture, null, new SlimDX.Vector3(pmCameraPosX, pmCameraPosY, 0f), new SlimDX.Vector3(pmScreenPosX, pmScreenPosY, 0f), white);
        }

        public void DrawRoundedCornerFrame(float pmPosRateX, float pmPosRateY, float pmWidthRate, float pmHeightRate, float pmThickRate, Color pmColor, float pmSolid)
        {
            float thickSize = clientWidth * pmThickRate;
            float startScreenPosX = clientWidth * pmPosRateX;
            float startScreenPosY = clientHeight * pmPosRateY;
            float screenWidth = clientWidth * pmWidthRate;
            float screenHeight = clientHeight * pmHeightRate;

            DrawPixelRectangle(startScreenPosX + thickSize, startScreenPosY, startScreenPosX + screenWidth - thickSize, startScreenPosY + thickSize, pmColor, pmSolid);
            DrawPixelRectangle(startScreenPosX, startScreenPosY + thickSize, startScreenPosX + thickSize, startScreenPosY + screenHeight - thickSize, pmColor, pmSolid);
            DrawPixelRectangle(startScreenPosX + screenWidth - thickSize, startScreenPosY + thickSize, startScreenPosX + screenWidth, startScreenPosY + screenHeight - thickSize, pmColor, pmSolid);
            DrawPixelRectangle(startScreenPosX + thickSize, startScreenPosY + screenHeight - thickSize, startScreenPosX + screenWidth - thickSize, startScreenPosY + screenHeight, pmColor, pmSolid);

        }

        public void DrawFrame(float pmPosRateX, float pmPosRateY, float pmWidthRate, float pmHeightRate, float pmThickRate, Color pmColor, float pmSolid)
        {
            float thickSize = clientWidth * pmThickRate;
            float startScreenPosX = clientWidth * pmPosRateX;
            float startScreenPosY = clientHeight * pmPosRateY;
            float screenWidth = clientWidth * pmWidthRate;
            float screenHeight = clientHeight * pmHeightRate;

            DrawPixelRectangle(startScreenPosX, startScreenPosY, startScreenPosX + screenWidth, startScreenPosY + thickSize, pmColor, pmSolid);
            DrawPixelRectangle(startScreenPosX, startScreenPosY + thickSize, startScreenPosX + thickSize, startScreenPosY + screenHeight - thickSize, pmColor, pmSolid);
            DrawPixelRectangle(startScreenPosX + screenWidth - thickSize, startScreenPosY + thickSize, startScreenPosX + screenWidth, startScreenPosY + screenHeight - thickSize, pmColor, pmSolid);
            DrawPixelRectangle(startScreenPosX, startScreenPosY + screenHeight - thickSize, startScreenPosX + screenWidth, startScreenPosY + screenHeight, pmColor, pmSolid);

        }

        private void DrawPixelRectangle(float pmStartScreenPosX, float pmStartScreenPosY, float pmEndScreenPosX, float pmEndScreenPosY, Color pmColor, float pmSolid)
        {
            var vertices = new VertexBuffer(mainDevice, 4 * 20, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
            vertices.Lock(0, 0, LockFlags.None).WriteRange(new[] {
                new Vertex() { vertexColor = pmColor.ToArgb(), vertexPosition = new Vector4(pmStartScreenPosX, pmStartScreenPosY, 0f, pmSolid) },
                new Vertex() { vertexColor = pmColor.ToArgb(), vertexPosition = new Vector4(pmEndScreenPosX, pmStartScreenPosY, 0f, pmSolid) },
                new Vertex() { vertexColor = pmColor.ToArgb(), vertexPosition = new Vector4(pmStartScreenPosX, pmEndScreenPosY, 0f, pmSolid) },
                new Vertex() { vertexColor = pmColor.ToArgb(), vertexPosition = new Vector4(pmEndScreenPosX, pmEndScreenPosY, 0f, pmSolid) }
            });
            vertices.Unlock();

            var vertexElems = new[] {
                new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.PositionTransformed, 0),
                new VertexElement(0, 16, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
                VertexElement.VertexDeclarationEnd
            };

            var vertexDecl = new VertexDeclaration(mainDevice, vertexElems);

            mainDevice.SetStreamSource(0, vertices, 0, 20);
            mainDevice.VertexDeclaration = vertexDecl;
            mainDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
        }

        private void DrawPixelCircle(float pmCenterScreenPosX, float pmCenterScreenPosY, float pmRadius, float pmStartAngle, float pmEndAngle, Color pmColor, float pmSolid)
        {
            float radius2 = pmRadius * pmRadius;
            List<Vertex> allCircleVertexes = new List<Vertex>();
            Vertex centerVertex = new Vertex(new Vector4(pmCenterScreenPosX, pmCenterScreenPosY, 0f, pmSolid), pmColor.ToArgb());
            allCircleVertexes.Add(centerVertex);
            for (float checkAngle = pmStartAngle; checkAngle <= pmEndAngle; checkAngle += 0.01f)
            {
                float checkRelativeX = (float)(pmRadius * Math.Cos(checkAngle));
                float checkRelativeY = (float)(pmRadius * Math.Sin(checkAngle));
                Vertex eachVertex = new Vertex(new Vector4(pmCenterScreenPosX + checkRelativeX, pmCenterScreenPosY + checkRelativeY, 0f, pmSolid), pmColor.ToArgb());
                allCircleVertexes.Add(eachVertex);
            }
            var vertices = new VertexBuffer(mainDevice, allCircleVertexes.Count * 20, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
            DataStream dsVertices = vertices.Lock(0, 0, LockFlags.None);
            foreach (Vertex eachV in allCircleVertexes)
            {
                dsVertices.Write(eachV.GetAllBytes(), 0, 20);
            }
            vertices.Unlock();

            var vertexElems = new[] {
                new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.PositionTransformed, 0),
                new VertexElement(0, 16, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
                VertexElement.VertexDeclarationEnd
            };

            var vertexDecl = new VertexDeclaration(mainDevice, vertexElems);

            mainDevice.SetStreamSource(0, vertices, 0, 20);
            mainDevice.VertexDeclaration = vertexDecl;
            mainDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, allCircleVertexes.Count);
        }

        public void DrawRateRectangle(float pmStartPosRateX, float pmStartPosRateY, float pmEndPosRateX, float pmEndPosRateY, float pmThickRate, Color pmColor, float pmSolid)
        {
            float thickSize = clientWidth * pmThickRate;
            float startScreenPosX = clientWidth * pmStartPosRateX;
            float startScreenPosY = clientHeight * pmStartPosRateY;
            float endScreenPosX = clientWidth * pmEndPosRateX;
            float endScreenPosY = clientHeight * pmEndPosRateY + thickSize;

            var vertices = new VertexBuffer(mainDevice, 4 * 20, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
            vertices.Lock(0, 0, LockFlags.None).WriteRange(new[] {
                new Vertex() { vertexColor = pmColor.ToArgb(), vertexPosition = new Vector4(startScreenPosX, startScreenPosY, 0f, pmSolid) },
                new Vertex() { vertexColor = pmColor.ToArgb(), vertexPosition = new Vector4(endScreenPosX, startScreenPosY, 0f, pmSolid) },
                new Vertex() { vertexColor = pmColor.ToArgb(), vertexPosition = new Vector4(startScreenPosX, endScreenPosY, 0f, pmSolid) },
                new Vertex() { vertexColor = pmColor.ToArgb(), vertexPosition = new Vector4(endScreenPosX, endScreenPosY, 0f, pmSolid) }
            });
            vertices.Unlock();

            var vertexElems = new[] {
                new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.PositionTransformed, 0),
                new VertexElement(0, 16, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
                VertexElement.VertexDeclarationEnd
            };

            var vertexDecl = new VertexDeclaration(mainDevice, vertexElems);

            mainDevice.SetStreamSource(0, vertices, 0, 20);
            mainDevice.VertexDeclaration = vertexDecl;
            mainDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
        }

        public Texture CreateTexture(byte[] pmBytes, int pmWidth, int pmHeight)
        {
            return Texture.FromMemory(mainDevice, pmBytes, pmWidth, pmHeight, 1, Usage.None, Format.A8R8G8B8, Pool.Default, Filter.None, Filter.None, 0);
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