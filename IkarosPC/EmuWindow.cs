using ImGuiNET;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;

namespace IkarosPC;

internal class EmuWindow
{
    private PC _pc;

    private Sdl2Window _window;
    private GraphicsDevice _graphicsDevice;
    private ImGuiRenderer _imgui;

    private CommandList _commandList;
    private DeviceBuffer _vertexBuffer;
    private DeviceBuffer _indexBuffer;
    private Shader[] _shaders;
    private Pipeline _pipeline;

    private Stopwatch _frameTimer;

    private const int FPS = 60;
    private const int CLOCKS_PER_FRAME = CPU.CLOCKS_PER_SECOND / FPS;
    private const float FRAME_TIME = 1.0f / FPS;
    private const float FRAME_TIME_MS = FRAME_TIME * 1000;

    public EmuWindow(PC pc)
    {
        _pc = pc;
        _frameTimer = new Stopwatch();
    }

    public void Start()
    {
        SetupWindow();
        CreateGraphicsResources();

        _frameTimer.Start();

        bool hasSimulatedFrame = false;

        while (_window.Exists)
        {
            if (!hasSimulatedFrame)
            {
                for (int i = 0; i < CLOCKS_PER_FRAME; i++)
                {
                    _pc.Step();
                }
                hasSimulatedFrame = true;
            }

            if (hasSimulatedFrame && _frameTimer.ElapsedMilliseconds > FRAME_TIME_MS)
            {
                hasSimulatedFrame = false;

                var snapshot = _window.PumpEvents();
                _imgui.Update(FRAME_TIME, snapshot);

                if (ImGui.Begin("Window"))
                {
                    ImGui.Text("Hello");
                    if (ImGui.Button("Quit"))
                    {
                        _window.Close();
                    }
                }
                ImGui.End();

                Draw();

                _frameTimer.Restart();
            }
        }

        DisposeResources();
    }

    private void Window_KeyUp(KeyEvent key)
    {

    }

    private void Window_KeyDown(KeyEvent key)
    {
        if (key.Key == Key.Escape)
        {
            _window.Close();
        }
    }

    private void SetupWindow()
    {
        var windowCI = new WindowCreateInfo()
        {
            X = 100,
            Y = 100,
            WindowWidth = 1920,
            WindowHeight = 1080,
            WindowTitle = "IkarosPC",
        };

        var options = new GraphicsDeviceOptions
        {
            PreferStandardClipSpaceYDirection = true,
            PreferDepthRangeZeroToOne = true,
        };

        VeldridStartup.CreateWindowAndGraphicsDevice(windowCI, options, out _window, out _graphicsDevice);

        _imgui = new ImGuiRenderer(
            _graphicsDevice,
            _graphicsDevice.SwapchainFramebuffer.OutputDescription,
            _window.Width,
            _window.Height);

        _window.Resized += () =>
        {
            _imgui.WindowResized(_window.Width, _window.Height);
        };

        _window.KeyDown += Window_KeyDown;
        _window.KeyUp += Window_KeyUp;
    }

    private void DisposeResources()
    {
        _pipeline.Dispose();
        
        for (int i = 0; i < _shaders.Length; i++)
        {
            _shaders[i].Dispose();
        }

        _commandList.Dispose();
        _vertexBuffer.Dispose();
        _indexBuffer.Dispose();
        _graphicsDevice.Dispose();
        _imgui.Dispose();
    }

    private void Draw()
    {
        _commandList.Begin();

        _commandList.SetFramebuffer(_graphicsDevice.SwapchainFramebuffer);

        _commandList.ClearColorTarget(0, RgbaFloat.CornflowerBlue);

        _commandList.SetVertexBuffer(0, _vertexBuffer);
        _commandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
        _commandList.SetPipeline(_pipeline);
        _commandList.DrawIndexed(
            indexCount: 4,
            instanceCount: 1,
            indexStart: 0,
            vertexOffset: 0,
            instanceStart: 0);

        _imgui.Render(_graphicsDevice, _commandList);

        _commandList.End();

        _graphicsDevice.SubmitCommands(_commandList);
        _graphicsDevice.SwapBuffers();
    }

    private void CreateGraphicsResources()
    {
        var factory = _graphicsDevice.ResourceFactory;

        VertexPositionColor[] vertices =
        {
            new (new Vector2(-0.75f, 0.75f), RgbaFloat.Red),
            new (new Vector2(0.75f, 0.75f), RgbaFloat.Green),
            new (new Vector2(-0.75f, -0.75f), RgbaFloat.Blue),
            new (new Vector2(0.75f, -0.75f), RgbaFloat.Yellow),
        };

        ushort[] indices = { 0, 1, 2, 3, };

        _vertexBuffer = factory.CreateBuffer(new BufferDescription(4 * VertexPositionColor.SizeInBytes, BufferUsage.VertexBuffer));
        _indexBuffer = factory.CreateBuffer(new BufferDescription(4 * sizeof(ushort), BufferUsage.IndexBuffer));

        _graphicsDevice.UpdateBuffer(_vertexBuffer, 0, vertices);
        _graphicsDevice.UpdateBuffer(_indexBuffer, 0, indices);

        var vertexLayout = new VertexLayoutDescription(
            new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
            new VertexElementDescription("Colour", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4));

        var vertexShaderDesc = new ShaderDescription(
            ShaderStages.Vertex,
            Encoding.UTF8.GetBytes(VertexCode),
            "main");
        var fragmentShaderDesc = new ShaderDescription(
            ShaderStages.Fragment,
            Encoding.UTF8.GetBytes(FragmentCode),
            "main");

        _shaders = factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

        var pipelineDescription = new GraphicsPipelineDescription();
        pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;

        pipelineDescription.DepthStencilState = new DepthStencilStateDescription(
            depthTestEnabled: true,
            depthWriteEnabled: true,
            comparisonKind: ComparisonKind.LessEqual);

        pipelineDescription.RasterizerState = new RasterizerStateDescription(
            cullMode: FaceCullMode.Back,
            fillMode: PolygonFillMode.Solid,
            frontFace: FrontFace.Clockwise,
            depthClipEnabled: true,
            scissorTestEnabled: false);

        pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
        pipelineDescription.ResourceLayouts = Array.Empty<ResourceLayout>();
        pipelineDescription.ShaderSet = new ShaderSetDescription(
            vertexLayouts: new VertexLayoutDescription[] { vertexLayout },
            shaders: _shaders);

        pipelineDescription.Outputs = _graphicsDevice.SwapchainFramebuffer.OutputDescription;

        _pipeline = factory.CreateGraphicsPipeline(pipelineDescription);

        _commandList = factory.CreateCommandList();
    }

    struct VertexPositionColor
    {
        public Vector2 Position;
        public RgbaFloat Colour;

        public VertexPositionColor(Vector2 position, RgbaFloat colour)
        {
            Position = position;
            Colour = colour;
        }

        public const uint SizeInBytes = 24;
    }

    private const string VertexCode = @"
#version 450

layout(location = 0) in vec2 Position;
layout(location = 1) in vec4 Colour;

layout(location = 0) out vec4 fsin_Colour;

void main()
{
    gl_Position = vec4(Position, 0, 1);
    fsin_Colour = Colour;
}";

    private const string FragmentCode = @"
#version 450

layout(location = 0) in vec4 fsin_Colour;
layout(location = 0) out vec4 fsout_Colour;

void main()
{
    fsout_Colour = fsin_Colour;
}";
}
