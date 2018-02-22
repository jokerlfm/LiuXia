using SlimDX;
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
            this.clientWidth = pmWidth;
            this.clientHeight = pmHeight;
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

            //factory2D = new SlimDX.Direct2D.Factory();
            //rt2D = new SlimDX.Direct2D.WindowRenderTarget(factory2D, new SlimDX.Direct2D.WindowRenderTargetProperties
            //{
            //    Handle = pmTargetHandle,
            //    PixelSize = new Size(pmWidth, pmHeight)
            //});
            //brush2D = new SlimDX.Direct2D.SolidColorBrush(rt2D, new Color4(0.93f, 0.40f, 0.08f));
        }

        #region declaratoin
        public SlimDX.Direct2D.Factory factory2D;
        public SlimDX.Direct2D.WindowRenderTarget rt2D;
        public SlimDX.Direct2D.Brush brush2D;
        public Device mainDevice;
        public Sprite mainSprite;
        SlimDX.Color4 white;
        SlimDX.Color4 black;
        public int clientWidth = 0, clientHeight = 0;
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
        }

        public void BeginDrawing()
        {
            mainDevice.BeginScene();
            mainDevice.Clear(ClearFlags.Target, black, 0f, 0);
            //rt2D.BeginDraw();
        }

        public void BeginTextureDrawing()
        {
            mainSprite.Begin(SpriteFlags.AlphaBlend);
        }

        public void BeginHintDrawing()
        {

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

        public void DrawFrame(float pmPosRateX, float pmPosRateY, float pmWidthRate, float pmHeightRate, float pmThickRate, Color pmColor, float pmSolid)
        {
            DrawPixelCircle(400, 400, 200, (float)Math.PI / 4, (float)Math.PI / 2, Color.Yellow, 1f);
            //float thickSize = clientWidth * pmThickRate;
            //float startScreenPosX = clientWidth * pmPosRateX;
            //float startScreenPosY = clientHeight * pmPosRateY;
            //float screenWidth = clientWidth * pmWidthRate;
            //float screenHeight = clientHeight * pmHeightRate;

            //DrawPixelRectangle(startScreenPosX + thickSize, startScreenPosY, startScreenPosX + screenWidth - thickSize, startScreenPosY + thickSize, pmColor, pmSolid);
            //DrawPixelRectangle(startScreenPosX, startScreenPosY + thickSize, startScreenPosX + thickSize, startScreenPosY + screenHeight - thickSize, pmColor, pmSolid);
            //DrawPixelRectangle(startScreenPosX + screenWidth - thickSize, startScreenPosY + thickSize, startScreenPosX + screenWidth, startScreenPosY + screenHeight - thickSize, pmColor, pmSolid);
            //DrawPixelRectangle(startScreenPosX + thickSize, startScreenPosY + screenHeight - thickSize, startScreenPosX + screenWidth - thickSize, startScreenPosY + screenHeight, pmColor, pmSolid);

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
            //Vertex testVertex1 = new Vertex(new Vector4(pmCenterScreenPosX, pmCenterScreenPosY - 100, 0f, pmSolid), pmColor.ToArgb());
            //allCircleVertexes.Add(testVertex1);
            //Vertex testVertex2 = new Vertex(new Vector4(pmCenterScreenPosX + 100, pmCenterScreenPosY - 100, 0f, pmSolid), pmColor.ToArgb());
            //allCircleVertexes.Add(testVertex2);
            //Vertex testVertex3 = new Vertex(new Vector4(pmCenterScreenPosX + 100, pmCenterScreenPosY, 0f, pmSolid), pmColor.ToArgb());
            //allCircleVertexes.Add(testVertex3);
            //var vertices = new VertexBuffer(mainDevice, allCircleVertexes.Count * 20, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
            //DataStream dsVertices = vertices.Lock(0, 0, LockFlags.None);
            //foreach (Vertex eachV in allCircleVertexes)
            //{
            //    dsVertices.Write(eachV.GetAllBytes(), 0, 20);
            //}
            //vertices.Unlock();

            //var vertexElems = new[] {
            //    new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.PositionTransformed, 0),
            //    new VertexElement(0, 16, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
            //    VertexElement.VertexDeclarationEnd
            //};

            //var vertexDecl = new VertexDeclaration(mainDevice, vertexElems);

            //mainDevice.SetStreamSource(0, vertices, 0, 20);
            //mainDevice.VertexDeclaration = vertexDecl;
            //mainDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, allCircleVertexes.Count - 2);

            for (float checkRelativeY = 0; checkRelativeY < pmRadius; checkRelativeY += 1)
            {
                float checkAngle = checkRelativeY / pmRadius;
                if (checkAngle >= pmStartAngle && checkAngle < pmEndAngle)
                {
                    float checkRelativeX = (float)Math.Sqrt(radius2 - checkRelativeY * checkRelativeY);
                    Vertex eachVertex = new Vertex(new Vector4(pmCenterScreenPosX + checkRelativeX, pmCenterScreenPosY + checkRelativeY, 0f, pmSolid), pmColor.ToArgb());
                    allCircleVertexes.Add(eachVertex);
                }
            }
            for (float checkRelativeY = pmRadius; checkRelativeY > 0; checkRelativeY -= 1)
            {
                float checkAngle = (float)(checkRelativeY / pmRadius + Math.PI / 2);
                if (checkAngle >= pmStartAngle && checkAngle < pmEndAngle)
                {
                    float checkRelativeX = (float)Math.Sqrt(radius2 - checkRelativeY * checkRelativeY);
                    Vertex eachVertex = new Vertex(new Vector4(pmCenterScreenPosX - checkRelativeX, pmCenterScreenPosY + checkRelativeY, 0f, pmSolid), pmColor.ToArgb());
                    allCircleVertexes.Add(eachVertex);
                }
            }
            for (float checkRelativeY = 0; checkRelativeY < pmRadius; checkRelativeY += 1)
            {
                float checkAngle = (float)(checkRelativeY / pmRadius + Math.PI);
                if (checkAngle >= pmStartAngle && checkAngle < pmEndAngle)
                {
                    float checkRelativeX = (float)Math.Sqrt(radius2 - checkRelativeY * checkRelativeY);
                    Vertex eachVertex = new Vertex(new Vector4(pmCenterScreenPosX - checkRelativeX, pmCenterScreenPosY - checkRelativeY, 0f, pmSolid), pmColor.ToArgb());
                    allCircleVertexes.Add(eachVertex);
                }
            }
            for (float checkRelativeY = pmRadius; checkRelativeY > 0; checkRelativeY -= 1)
            {
                float checkAngle = (float)(checkRelativeY / pmRadius + Math.PI * 3 / 2);
                if (checkAngle >= pmStartAngle && checkAngle < pmEndAngle)
                {
                    float checkRelativeX = (float)Math.Sqrt(radius2 - checkRelativeY * checkRelativeY);
                    Vertex eachVertex = new Vertex(new Vector4(pmCenterScreenPosX + checkRelativeX, pmCenterScreenPosY - checkRelativeY, 0f, pmSolid), pmColor.ToArgb());
                    allCircleVertexes.Add(eachVertex);
                }
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
            mainDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, allCircleVertexes.Count-2);
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

        public void EndTextureDrawing()
        {
            mainSprite.End();
        }

        public void EndHintDrawing()
        {
            mainDevice.EndScene();
            mainDevice.Present();
        }

        public void EndDrawing()
        {

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