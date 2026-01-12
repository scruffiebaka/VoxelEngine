using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using VoxelEngine.Graphics;
using VoxelEngine.Graphics.Rendering;
using VoxelEngine.Graphics.Textures;
using VoxelEngine.World.Blocks;

namespace VoxelEngine.Core;

public class Game
{
    Camera camera;
    Shader shader = new Shader("./Resources/Shaders/VertexShader.glsl", "./Resources/Shaders/FragmentShader.glsl");
    Window window;

    Chunk testchunk = new Chunk();
    ChunkMesh? chunkMesh;

    public Game(int width, int height, Window window)
    {
        camera = new Camera(Vector3.UnitZ * 3, width / (float)height);
        this.window = window;
    }

    public void Load()
    {
        window.CursorState = CursorState.Grabbed;
        //camera.Position += Vector3.UnitY * 16f;
        shader.SetVector3("lightDir", new Vector3(1f, -1f, 0.5f));

        testchunk = new Chunk();
        testchunk.world_position = new Vector3(0, 0, 0);

        for (int x = 0; x < Chunk.size; x++)
        {
            for (int z = 0; z < Chunk.size; z++)
            {
                for (int y = 0; y < Chunk.size; y++)
                {
                    ref Block block = ref testchunk.Get(x, y, z);

                    if (y == 0)
                        block.id = (byte)BlockId.Bedrock;
                    else if (y <= 3)
                        block.id = (byte)BlockId.Stone;
                    else if (y <= 14)
                        block.id = (byte)BlockId.Dirt;
                    else
                        block.id = (byte)BlockId.Grass;
                }
            }
        }

        Texture baseTex = new Texture(Texture.LoadFromFile("Resources/atlas.png"));
        Texture.Use(baseTex);

        TextureRegistry.RegisterBlocksTextures();

        chunkMesh = ChunkMeshRenderer.GenerateChunkMesh(testchunk, shader);
    }

    public void Render(FrameEventArgs args)
    {
        shader.Use();
        shader.SetMatrix4("view", camera.GetViewMatrix());
        shader.SetMatrix4("projection", camera.GetProjectionMatrix());

        if (chunkMesh != null)
            ChunkMeshRenderer.Draw(testchunk.world_position, chunkMesh, shader);
    }

    public void Update(FrameEventArgs args, KeyboardState keyboard, MouseState mouse)
    {
        CameraMovement(args, keyboard, mouse);

        if (keyboard.IsKeyDown(Keys.Escape))
        {
            window.Close();
        }
    }

    private bool _firstMove = true;
    private Vector2 _lastPos;
    private void CameraMovement(FrameEventArgs updateArguements, KeyboardState Input, MouseState mouse)
    {
        const float cameraSpeed = 2.5f;
        const float sensitivity = 0.2f;

        if (Input.IsKeyDown(Keys.W))
        {
            camera.Position += camera.Front * cameraSpeed * (float)updateArguements.Time; // Forward
        }

        if (Input.IsKeyDown(Keys.S))
        {
            camera.Position -= camera.Front * cameraSpeed * (float)updateArguements.Time; // Backwards
        }
        if (Input.IsKeyDown(Keys.A))
        {
            camera.Position -= camera.Right * cameraSpeed * (float)updateArguements.Time; // Left
        }
        if (Input.IsKeyDown(Keys.D))
        {
            camera.Position += camera.Right * cameraSpeed * (float)updateArguements.Time; // Right
        }
        if (Input.IsKeyDown(Keys.Space))
        {
            camera.Position += camera.Up * cameraSpeed * (float)updateArguements.Time; // Up
        }
        if (Input.IsKeyDown(Keys.LeftShift))
        {
            camera.Position -= camera.Up * cameraSpeed * (float)updateArguements.Time; // Down
        }

        if (_firstMove)
        {
            _lastPos = new Vector2(mouse.X, mouse.Y);
            _firstMove = false;
        }
        else
        {
            var deltaX = mouse.X - _lastPos.X;
            var deltaY = mouse.Y - _lastPos.Y;
            _lastPos = new Vector2(mouse.X, mouse.Y);

            camera.Yaw += deltaX * sensitivity;
            camera.Pitch -= deltaY * sensitivity;
        }
    }

}
