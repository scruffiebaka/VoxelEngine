using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

using VoxelEngine.Core;

namespace VoxelEngine
{
    public class Window : GameWindow
    {
        private Game? game;

        public Window(int width, int height, string Title) : base(GameWindowSettings.Default,
            new NativeWindowSettings() { Title = Title, ClientSize = new Vector2i(width, height), MaximumClientSize = new Vector2i(width, height) })
        {
            CenterWindow();
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.LoadBindings(new GLFWBindingsContext());
            GL.ClearColor(new Color4(0.53f, 0.81f, 0.92f, 1.0f));
            GL.Enable(EnableCap.DepthTest);

            game = new Game(Size.X, Size.Y, this);
            game.Load();
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (game != null)
                game.Render(args);

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (game != null)
                game.Update(args, KeyboardState, MouseState);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }
    }
}