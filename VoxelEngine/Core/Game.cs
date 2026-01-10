using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using VoxelEngine.Graphics;
using VoxelEngine.Graphics.Rendering;
using VoxelEngine.World.Blocks;

namespace VoxelEngine.Core;

public class Game
{
    Chunk coolchunk;
    ChunkMesh chunkMesh;
    Camera camera;
    Shader shader;

    public Game(int width, int height)
    {
        this.camera = new Camera(Vector3.UnitZ * 3, width / (float)height);
    }

    public void Load()
    {
        shader = new Shader("./Resources/Shaders/VertexShader.glsl", "./Resources/Shaders/FragmentShader.glsl");
        shader.SetVector3("lightDir", new Vector3(1f, -1f, 0.5f));
        shader.SetVector3("color", new Vector3(0.6f, 0.45f, 0.3f));
        shader.Use();

        coolchunk = new Chunk();
        coolchunk.world_position = new Vector3(0, 0, 0);

        for (int i = 0; i < coolchunk.blocks.Length; i++)
        {
            Block coolblock = new Block();
            coolblock.id = (byte)BlockId.Dirt;
            coolchunk.blocks[i] = coolblock;
        }

        chunkMesh = ChunkMeshRenderer.GenerateChunkMesh(coolchunk, shader);
    }

    public void Render(FrameEventArgs args)
    {
        shader.Use();
        shader.SetMatrix4("view", camera.GetViewMatrix());
        shader.SetMatrix4("projection", camera.GetProjectionMatrix());

        ChunkMeshRenderer.Draw(coolchunk.world_position, chunkMesh, shader);
    }

    public void Update(FrameEventArgs args, KeyboardState keyboard, MouseState mouse)
    {
        TestCamera_Movement(args, keyboard, mouse);
    }

    private bool _firstMove = true;
    private Vector2 _lastPos;
    private void TestCamera_Movement(FrameEventArgs updateArguements, KeyboardState Input, MouseState mouse)
    {
        const float cameraSpeed = 1.5f;
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
