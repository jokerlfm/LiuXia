using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.Windows;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Device11 = SlimDX.Direct3D11.Device;
using Buffer11 = SlimDX.Direct3D11.Buffer;
using BufferSys = System.Buffer;
using System.Collections.Generic;
using System.Threading;

namespace SlimDXTestForm
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        #region declaration
        Device11 mainD11 = null;
        SwapChain mainSC = null;

        float screenPixelRateX = 0;
        float screenPixelRateY = 0;

        float moveX = 0f;
        float moveY = 0f;

        bool running = false;
        #endregion

        #region event
        private void FormMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Application.Exit();
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            running = false;
            Thread.Sleep(1000);
        }

        private void FormMain_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'e')
            {
                moveY -= 0.02f;
            }
            else if (e.KeyChar == 'd')
            {
                moveY += 0.02f;
            }
            else if (e.KeyChar == 's')
            {
                moveX += 0.02f;
            }
            else if (e.KeyChar == 'f')
            {
                moveX -= 0.02f;
            }
        }

        private void FormMain_Shown(object sender, System.EventArgs e)
        {
            SwapChainDescription desc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(this.Width, this.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = this.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };
            screenPixelRateX = 2f / (float)this.Width;
            screenPixelRateY = 2f / (float)this.Height;
            Device11.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.Debug, desc, out mainD11, out mainSC);
            mainD11.Factory.SetWindowAssociation(this.Handle, WindowAssociationFlags.IgnoreAll);

            WaitCallback wcb = new WaitCallback(this.DoRenderLoop);
            ThreadPool.QueueUserWorkItem(wcb, null);

            running = true;
        }
        #endregion

        #region business
        private void DoRenderLoop(object pmMain = null)
        {
            RasterizerStateDescription rsd = new RasterizerStateDescription();
            rsd.CullMode = CullMode.None;
            rsd.FillMode = FillMode.Solid;
            mainD11.ImmediateContext.Rasterizer.State = RasterizerState.FromDescription(mainD11, rsd);
            SamplerDescription sd = new SamplerDescription();
            sd.Filter = Filter.MinLinearMagPointMipLinear;
            sd.AddressU = TextureAddressMode.Wrap;
            sd.AddressV = TextureAddressMode.Wrap;
            sd.AddressW = TextureAddressMode.Wrap;
            sd.BorderColor = new Color4(Color.White);
            SamplerState ssMain = SamplerState.FromDescription(mainD11, sd);
            Texture2D backBuffer = Texture2D.FromSwapChain<Texture2D>(mainSC, 0);
            var renderView = new RenderTargetView(mainD11, backBuffer);

            BlendStateDescription bsd = new BlendStateDescription();
            bsd.RenderTargets[0] = new RenderTargetBlendDescription();
            bsd.RenderTargets[0].BlendEnable = true;
            bsd.RenderTargets[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;
            bsd.RenderTargets[0].BlendOperation = BlendOperation.Add;
            bsd.RenderTargets[0].SourceBlend = BlendOption.SourceColor;
            bsd.RenderTargets[0].DestinationBlend = BlendOption.DestinationAlpha;
            bsd.RenderTargets[0].SourceBlendAlpha = BlendOption.One;
            bsd.RenderTargets[0].BlendOperationAlpha = BlendOperation.Add;
            bsd.RenderTargets[0].DestinationBlendAlpha = BlendOption.One;
            mainD11.ImmediateContext.OutputMerger.BlendState = BlendState.FromDescription(mainD11, bsd);

            Dictionary<int, MmapUnit> mmapIndexDictionaryFloor = null;
            Dictionary<int, MmapUnit> mmapIndexDictionarySurface = null;
            ShaderResourceView resourceViewFloor = GenerateMmapSRV(mainD11, 0, 512, out mmapIndexDictionaryFloor);
            ShaderResourceView resourceViewSurface = GenerateMmapSRV(mainD11, 512, 1024, out mmapIndexDictionarySurface);
            MapRange mmapRangeFloor = GenerateMmapRange(mainD11, 1, 0, 80, 0, 210, mmapIndexDictionaryFloor, screenPixelRateX, screenPixelRateY);
            MapRange mmapRangeSurface = GenerateMmapRange(mainD11, 2, 0, 80, 0, 210, mmapIndexDictionarySurface, screenPixelRateX, screenPixelRateY);

            var colorBytecodeV = ShaderBytecode.CompileFromFile("Color.hlsl", "VS", "vs_5_0", ShaderFlags.None, EffectFlags.None);
            VertexShader vsColor = new VertexShader(mainD11, colorBytecodeV);
            var colorBytecodeP = ShaderBytecode.CompileFromFile("Color.hlsl", "PS", "ps_5_0", ShaderFlags.None, EffectFlags.None);
            PixelShader psColor = new PixelShader(mainD11, colorBytecodeP);
            var signatureColor = ShaderSignature.GetInputSignature(colorBytecodeV);
            var layoutColor = new InputLayout(mainD11, signatureColor, new[] {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
            });

            var textureBytecodeV = ShaderBytecode.CompileFromFile("Texture.hlsl", "VS", "vs_5_0", ShaderFlags.None, EffectFlags.None);
            VertexShader vsTexture = new VertexShader(mainD11, textureBytecodeV);
            var textureBytecodeP = ShaderBytecode.CompileFromFile("Texture.hlsl", "PS", "ps_5_0", ShaderFlags.None, EffectFlags.None);
            PixelShader psTexture = new PixelShader(mainD11, textureBytecodeP);
            var signatureTexture = ShaderSignature.GetInputSignature(textureBytecodeV);
            var layoutTexture = new InputLayout(mainD11, signatureTexture, new[] {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32B32A32_Float, 16, 0)
            });

            mainD11.ImmediateContext.OutputMerger.SetTargets(renderView);
            mainD11.ImmediateContext.Rasterizer.SetViewports(new Viewport(0, 0, this.Width, this.Height, 0.0f, 1.0f));
            mainD11.ImmediateContext.PixelShader.SetSampler(ssMain, 0);
            mainD11.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            mainD11.ImmediateContext.InputAssembler.InputLayout = layoutTexture;
            mainD11.ImmediateContext.VertexShader.Set(vsTexture);
            mainD11.ImmediateContext.PixelShader.Set(psTexture);

            Matrix projection = Matrix.OrthoLH(2f, 2f, 0, 2f);
            Matrix view = Matrix.LookAtLH(new Vector3(0, 0, -1f), Vector3.Zero, Vector3.UnitY);
            Matrix world = Matrix.Translation(Vector3.Zero);
            Matrix WVP = world * view * projection;
            Buffer11 constantBufferCB = new Buffer11(mainD11, new BufferDescription
            {
                Usage = ResourceUsage.Default,
                SizeInBytes = Marshal.SizeOf(WVP),
                BindFlags = BindFlags.ConstantBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                StructureByteStride = 0,
            });
            mainD11.ImmediateContext.VertexShader.SetConstantBuffer(constantBufferCB, 0);

            while (running)
            {
                mainD11.ImmediateContext.ClearRenderTargetView(renderView, Color.Black);

                world = Matrix.Translation(new Vector3(moveX, moveY, 0));
                
                WVP = world * view * projection;
                DataStream dsCamera = new DataStream(Marshal.SizeOf(WVP), true, true);
                dsCamera.Write(WVP);
                dsCamera.Position = 0;
                mainD11.ImmediateContext.UpdateSubresource(new DataBox(0, 0, dsCamera), constantBufferCB, 0);

                mainD11.ImmediateContext.PixelShader.SetShaderResource(resourceViewFloor, 0);
                mainD11.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(mmapRangeFloor.mapVertexBuffer, 32, 0));
                mainD11.ImmediateContext.Draw(mmapRangeFloor.vector4Count, 0);

                mainD11.ImmediateContext.PixelShader.SetShaderResource(resourceViewSurface, 0);
                if (mmapRangeSurface != null)
                {
                    mainD11.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(mmapRangeSurface.mapVertexBuffer, 32, 0));
                    mainD11.ImmediateContext.Draw(mmapRangeSurface.vector4Count, 0);
                }

                mainSC.Present(0, PresentFlags.None);
                Thread.Sleep(10);
            }

            colorBytecodeP.Dispose();
            colorBytecodeV.Dispose();
            textureBytecodeP.Dispose();
            textureBytecodeV.Dispose();
            mmapRangeFloor.mapVertexBuffer.Dispose();
            if (mmapRangeSurface != null)
            {
                mmapRangeSurface.mapVertexBuffer.Dispose();
            }
            layoutColor.Dispose();
            layoutTexture.Dispose();
            constantBufferCB.Dispose();
            renderView.Dispose();
            backBuffer.Dispose();
            mainD11.Dispose();
            mainSC.Dispose();
        }

        private ShaderResourceView GenerateMmapSRV(Device11 pmDevice11, int pmPicStartIndex, int pmPicEndIndex, out Dictionary<int, MmapUnit> pmMmapIndexDictionary)
        {
            pmMmapIndexDictionary = new Dictionary<int, MmapUnit>();
            byte[] mmapNPBytes = File.ReadAllBytes("mmap.np");

            int checkCount = pmPicStartIndex;
            int basePos = 4;
            int length = 0;
            short width = 0, height = 0, gapX = 0, gapY = 0;
            int unitWidth = 36, unitHeight = 18;
            Bitmap resultPic = new Bitmap(unitWidth * 16, unitHeight * 32);
            Color emptyColor = Color.FromArgb(0, 0, 0, 0);
            while (checkCount < pmPicEndIndex)
            {
                length = BitConverter.ToInt32(mmapNPBytes, basePos);
                width = BitConverter.ToInt16(mmapNPBytes, basePos + 4);
                height = BitConverter.ToInt16(mmapNPBytes, basePos + 4 + 2);
                gapX = BitConverter.ToInt16(mmapNPBytes, basePos + 4 + 2 + 2);
                gapY = BitConverter.ToInt16(mmapNPBytes, basePos + 4 + 2 + 2 + 2);
                byte[] bodyBytes = new byte[length - 12];
                BufferSys.BlockCopy(mmapNPBytes, basePos + 4 + 2 + 2 + 2 + 2, bodyBytes, 0, length - 12);
                using (MemoryStream ms = new MemoryStream(bodyBytes))
                {
                    Image eachIMG = Image.FromStream(ms);
                    Bitmap eachPic = new Bitmap(eachIMG);
                    MmapUnit eachMU = new MmapUnit(checkCount, eachPic.Width, eachPic.Height, gapX, gapY);
                    for (int yCount = 0; yCount < unitHeight; yCount++)
                    {
                        for (int xCount = 0; xCount < unitWidth; xCount++)
                        {
                            Color pixelColor = emptyColor;
                            if (xCount < width && yCount < height)
                            {
                                pixelColor = eachPic.GetPixel(xCount, yCount);
                            }
                            resultPic.SetPixel((checkCount - pmPicStartIndex) % 16 * unitWidth + xCount, unitHeight * ((checkCount - pmPicStartIndex) / 16) + yCount, pixelColor);
                        }
                    }
                    pmMmapIndexDictionary.Add(checkCount, eachMU);
                }

                basePos += length;
                checkCount++;
            }

            // debug
            resultPic.Save("result.png");

            MemoryStream msResult = new MemoryStream();
            resultPic.Save(msResult, System.Drawing.Imaging.ImageFormat.Bmp);
            byte[] resultBytes = msResult.GetBuffer();
            Texture2D t2d = Texture2D.FromMemory(pmDevice11, resultBytes);
            ShaderResourceView resourceView = new ShaderResourceView(pmDevice11, t2d);
            t2d.Dispose();


            return resourceView;
        }

        private MapRange GenerateMmapRange(Device11 pmDevice11, int pmLayer, int pmStartCoordinateX, int pmEndCoordinateX, int pmStartCoordinateY, int pmEndCoordinateY, Dictionary<int, MmapUnit> pmMmapIndexDictionary, float pmScreenPixelRateX, float pmScreenPixelRateY)
        {
            if (pmStartCoordinateX < 0 || pmStartCoordinateX >= 480 || pmEndCoordinateX < 0 || pmEndCoordinateX >= 480 || pmStartCoordinateY < 0 || pmStartCoordinateY >= 480 || pmEndCoordinateY < 0 || pmEndCoordinateY >= 480)
            {
                return null;
            }
            if (pmStartCoordinateX > pmEndCoordinateX || pmStartCoordinateY > pmEndCoordinateY)
            {
                return null;
            }

            List<Vector4> allVector4List = new List<Vector4>();
            if (pmLayer == 1)
            {
                byte[] check002Bytes = File.ReadAllBytes("Earth.002");
                for (int coordinateYCount = pmStartCoordinateY; coordinateYCount <= pmEndCoordinateY; coordinateYCount++)
                {
                    for (int coordinateXCount = pmStartCoordinateX; coordinateXCount <= pmEndCoordinateX; coordinateXCount++)
                    {
                        int checkBytesPos = (coordinateYCount * 480 + coordinateXCount) * 2;
                        int picIndex = BitConverter.ToInt16(check002Bytes, checkBytesPos) / 2;
                        float unitSizeX = pmMmapIndexDictionary[picIndex].width * pmScreenPixelRateX;
                        float unitSizeY = pmMmapIndexDictionary[picIndex].height * pmScreenPixelRateY;
                        float drawStartPosX = (coordinateXCount * 18 - coordinateYCount * 18) * pmScreenPixelRateX - 18 * pmScreenPixelRateX;
                        float drawEndPosX = drawStartPosX + 36 * pmScreenPixelRateX;
                        float drawStartPosY = (-coordinateXCount * 9 - coordinateYCount * 9) * pmScreenPixelRateY + 9 * pmScreenPixelRateY;
                        float drawEndPosY = drawStartPosY - 18 * pmScreenPixelRateY;
                        float uStart = (float)(picIndex % 16) / 16f;
                        float uEnd = (float)((picIndex + 1) % 16) / 16f;
                        float vStart = (float)(picIndex / 16) / 32f;
                        float vEnd = (float)((picIndex / 16) + 1) / 32f;
                        allVector4List.Add(new Vector4(drawStartPosX, drawStartPosY, 0.0f, 1.0f));
                        allVector4List.Add(new Vector4(uStart, vStart, 0.0f, 0.0f));
                        allVector4List.Add(new Vector4(drawEndPosX, drawStartPosY, 0.0f, 1.0f));
                        allVector4List.Add(new Vector4(uEnd, vStart, 0.0f, 0.0f));
                        allVector4List.Add(new Vector4(drawStartPosX, drawEndPosY, 0.0f, 1.0f));
                        allVector4List.Add(new Vector4(uStart, vEnd, 0.0f, 0.0f));
                        allVector4List.Add(new Vector4(drawStartPosX, drawEndPosY, 0.0f, 1.0f));
                        allVector4List.Add(new Vector4(uStart, vEnd, 0.0f, 0.0f));
                        allVector4List.Add(new Vector4(drawEndPosX, drawStartPosY, 0.0f, 1.0f));
                        allVector4List.Add(new Vector4(uEnd, vStart, 0.0f, 0.0f));
                        allVector4List.Add(new Vector4(drawEndPosX, drawEndPosY, 0.0f, 1.0f));
                        allVector4List.Add(new Vector4(uEnd, vEnd, 0.0f, 0.0f));
                    }
                }
            }
            else if (pmLayer == 2)
            {
                byte[] check002Bytes = File.ReadAllBytes("Surface.002");
                for (int coordinateYCount = pmStartCoordinateY; coordinateYCount <= pmEndCoordinateY; coordinateYCount++)
                {
                    for (int coordinateXCount = pmStartCoordinateX; coordinateXCount <= pmEndCoordinateX; coordinateXCount++)
                    {
                        int checkBytesPos = (coordinateYCount * 480 + coordinateXCount) * 2;
                        int picIndex = BitConverter.ToInt16(check002Bytes, checkBytesPos) / 2;
                        if (picIndex == 0)
                        {
                            continue;
                        }
                        float unitSizeX = pmMmapIndexDictionary[picIndex].width * pmScreenPixelRateX;
                        float unitSizeY = pmMmapIndexDictionary[picIndex].height * pmScreenPixelRateY;
                        float drawStartPosX = (coordinateXCount * 18 - coordinateYCount * 18) * pmScreenPixelRateX - 18 * pmScreenPixelRateX;
                        float drawEndPosX = drawStartPosX + 36 * pmScreenPixelRateX;
                        float drawStartPosY = (-coordinateXCount * 9 - coordinateYCount * 9) * pmScreenPixelRateY + 9 * pmScreenPixelRateY;
                        float drawEndPosY = drawStartPosY - 18 * pmScreenPixelRateY;
                        float uStart = (float)(picIndex % 16) / 16f;
                        float uEnd = (float)((picIndex + 1) % 16) / 16f;
                        float vStart = (float)(picIndex / 16) / 32f;
                        float vEnd = (float)((picIndex / 16) + 1) / 32f;
                        allVector4List.Add(new Vector4(drawStartPosX, drawStartPosY, 0.0f, 1.0f));
                        allVector4List.Add(new Vector4(uStart, vStart, 0.0f, 0.0f));
                        allVector4List.Add(new Vector4(drawEndPosX, drawStartPosY, 0.0f, 1.0f));
                        allVector4List.Add(new Vector4(uEnd, vStart, 0.0f, 0.0f));
                        allVector4List.Add(new Vector4(drawStartPosX, drawEndPosY, 0.0f, 1.0f));
                        allVector4List.Add(new Vector4(uStart, vEnd, 0.0f, 0.0f));
                        allVector4List.Add(new Vector4(drawStartPosX, drawEndPosY, 0.0f, 1.0f));
                        allVector4List.Add(new Vector4(uStart, vEnd, 0.0f, 0.0f));
                        allVector4List.Add(new Vector4(drawEndPosX, drawStartPosY, 0.0f, 1.0f));
                        allVector4List.Add(new Vector4(uEnd, vStart, 0.0f, 0.0f));
                        allVector4List.Add(new Vector4(drawEndPosX, drawEndPosY, 0.0f, 1.0f));
                        allVector4List.Add(new Vector4(uEnd, vEnd, 0.0f, 0.0f));
                    }
                }
            }

            if (allVector4List.Count > 0)
            {
                DataStream ds = new DataStream(allVector4List.Count * 32, true, true);
                for (int checkVector4Count = 0; checkVector4Count < allVector4List.Count; checkVector4Count++)
                {
                    ds.Write(allVector4List[checkVector4Count]);
                }
                ds.Position = 0;

                Buffer11 resultBuffer = new Buffer11(pmDevice11, ds, new BufferDescription()
                {
                    BindFlags = BindFlags.VertexBuffer,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None,
                    SizeInBytes = allVector4List.Count * 32,
                    Usage = ResourceUsage.Default
                });
                ds.Dispose();
                return new MapRange(resultBuffer, allVector4List.Count);
            }
            else
            {
                return null;
            }
        }
        #endregion
    }

    public struct TexturedVertex
    {
        /// <summary>
        /// Position
        /// </summary>
        public Vector4 POSITION;

        /// <summary>
        /// Texture coordinate
        /// </summary>
        public Vector4 TEXCOORD;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">Position</param>
        /// <param name="textureCoordinate">Texture Coordinate</param>
        public TexturedVertex(Vector4 position, Vector4 textureCoordinate)
        {
            POSITION = position;
            TEXCOORD = textureCoordinate;
        }
    }

    public class MmapUnit
    {
        public MmapUnit(int pmID, int pmWidth, int pmHeight, int pmGapX, int pmGapY)
        {
            id = pmID;
            width = pmWidth;
            height = pmHeight;
            gapX = pmGapX;
            gapY = pmGapY;
        }

        #region declaration
        public int id = 0;
        public int width = 0;
        public int height = 0;
        public int gapX = 0;
        public int gapY = 0;
        #endregion
    }

    public class MapRange
    {
        public MapRange(Buffer11 pmMapVertexBuffer, int pmVector4Count)
        {
            mapVertexBuffer = pmMapVertexBuffer;
            vector4Count = pmVector4Count;
        }

        #region declaration
        public Buffer11 mapVertexBuffer = null;
        public int vector4Count = 0;
        #endregion        
    }
}