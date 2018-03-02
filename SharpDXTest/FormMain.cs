using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using Device11 = SharpDX.Direct3D11.Device;
using Resource11 = SharpDX.Direct3D11.Resource;
using Buffer11 = SharpDX.Direct3D11.Buffer;
using SharpDX.Windows;

namespace SharpDXTest
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            var desc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription =
                                   new ModeDescription(this.Width, this.Height,
                                                       new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = this.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            // Create Device and SwapChain
            Device11 device11;
            SwapChain swMain;
            Device11.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, desc, out device11, out swMain);

            // Ignore all windows events
            var factory = swMain.GetParent<Factory>();
            factory.MakeWindowAssociation(this.Handle, WindowAssociationFlags.IgnoreAll);

            // New RenderTargetView from the backbuffer
            var backBuffer = Texture2D.FromSwapChain<Texture2D>(swMain, 0);
            var renderView = new RenderTargetView(device11, backBuffer);

            device11.ImmediateContext.Rasterizer.State = new RasterizerState(device11, new RasterizerStateDescription() { CullMode = CullMode.None, FillMode = FillMode.Solid });
            device11.ImmediateContext.Rasterizer.SetViewport(new Viewport(0, 0, this.Width, this.Height, 0.0f, 1.0f));
            device11.ImmediateContext.OutputMerger.SetTargets(renderView);

            // Compile Vertex and Pixel shaders
            var vertexShaderByteCode = ShaderBytecode.CompileFromFile("Color.hlsl", "VS", "vs_5_0", ShaderFlags.None, EffectFlags.None);
            var vertexShader = new VertexShader(device11, vertexShaderByteCode);

            var pixelShaderByteCode = ShaderBytecode.CompileFromFile("Color.hlsl", "PS", "ps_4_0", ShaderFlags.None, EffectFlags.None);
            var pixelShader = new PixelShader(device11, pixelShaderByteCode);

            // Layout from VertexShader input signature
            var layout = new InputLayout(
                device11,
                ShaderSignature.GetInputSignature(vertexShaderByteCode),
                new[]
                    {
                        new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
                    });

            // Instantiate Vertex buiffer from vertex data
            var vertices1 = Buffer11.Create(device11, BindFlags.VertexBuffer, new[]
                                  {
                                      new Vector4(-0.5f, 0.5f, 0f, 1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
                                      new Vector4(-0.1f, 0.5f, 0f, 1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
                                      new Vector4(-0.5f, 0.1f, 0f, 1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f),

                                      new Vector4(-0.5f, 0.1f, 0f, 1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
                                      new Vector4(-0.1f, 0.5f, 0f, 1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
                                      new Vector4(-0.1f, 0.1f, 0f, 1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f),

                                      new Vector4(0.1f, 0.5f, 0f, 1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
                                      new Vector4(0.5f, 0.5f, 0f, 1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
                                      new Vector4(0.1f, 0.1f, 0f, 1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f),

                                      new Vector4(0.1f, 0.1f, 0f, 1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
                                      new Vector4(0.5f, 0.5f, 0f, 1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
                                      new Vector4(0.5f, 0.1f, 0f, 1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f),                                      
                                  });

            var vertices2 = Buffer11.Create(device11, BindFlags.VertexBuffer, new[]
                                 {
                                      new Vector4(-0.5f, -0.1f, 0f, 1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
                                      new Vector4(-0.1f, -0.1f, 0f, 1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
                                      new Vector4(-0.5f, -0.5f, 0f, 1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f),

                                      new Vector4(-0.5f, -0.5f, 0f, 1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
                                      new Vector4(-0.1f, -0.1f, 0f, 1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
                                      new Vector4(-0.1f, -0.5f, 0f, 1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f),

                                      new Vector4(0.1f, -0.1f, 0f, 1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
                                      new Vector4(0.5f, -0.1f, 0f, 1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
                                      new Vector4(0.1f, -0.5f, 0f, 1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f),

                                      new Vector4(0.1f, -0.5f, 0f, 1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
                                      new Vector4(0.5f, -0.1f, 0f, 1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
                                      new Vector4(0.5f, -0.5f, 0f, 1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
                                  });

            // Prepare All the stages
            device11.ImmediateContext.InputAssembler.InputLayout = layout;
            device11.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            device11.ImmediateContext.VertexShader.Set(vertexShader);            
            device11.ImmediateContext.PixelShader.Set(pixelShader);
            Buffer11 wvpBuffer = new Buffer11(device11, Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);            

            // Main loop
            RenderLoop.Run(this, () =>
            {

                device11.ImmediateContext.ClearRenderTargetView(renderView, Color.Aquamarine);
                device11.ImmediateContext.VertexShader.SetConstantBuffer(0, wvpBuffer);

                Matrix projection = Matrix.OrthoLH(1f, 1f, 0, 2f);
                Matrix view = Matrix.LookAtLH(new Vector3(0, 0, -1f), Vector3.Zero, Vector3.Up);
                Matrix world = Matrix.RotationZ(2);
                Matrix WVP = world * view * projection;
                device11.ImmediateContext.UpdateSubresource<Matrix>(ref WVP, wvpBuffer);

                device11.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertices1, 32, 0));
                device11.ImmediateContext.Draw(12, 0);
                
                device11.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertices2, 32, 0));
                device11.ImmediateContext.Draw(12, 0);
                swMain.Present(0, PresentFlags.None);
            });

            // Release all resources
            vertexShaderByteCode.Dispose();
            vertexShader.Dispose();
            pixelShaderByteCode.Dispose();
            pixelShader.Dispose();
            vertices1.Dispose();
            vertices2.Dispose();
            layout.Dispose();
            renderView.Dispose();
            backBuffer.Dispose();
            device11.ImmediateContext.ClearState();
            device11.ImmediateContext.Flush();
            device11.Dispose();
            device11.ImmediateContext.Dispose();
            swMain.Dispose();
            factory.Dispose();
        }
    }
}