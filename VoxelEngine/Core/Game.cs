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

    Player player;

    public Game(int width, int height, Window window)
    {
        camera = new Camera(Vector3.UnitZ * 3, width / (float)height);
        this.window = window;
    }

    public void Load()
    {
        window.CursorState = CursorState.Grabbed;
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
                    else if (y <= 8)
                        block.id = (byte)BlockId.Dirt;
                    else if (y <= 9)
                        block.id = (byte)BlockId.Grass;
                    else
                        block.id = (byte)BlockId.Air;
                }
            }
        }

        Texture baseTex = new Texture(Texture.LoadFromFile("Resources/atlas.png"));
        Texture.Use(baseTex);

        TextureRegistry.RegisterBlocksTextures();

        chunkMesh = ChunkMeshRenderer.GenerateChunkMesh(testchunk, shader);

        testchunk.OnBlockChanged += () =>
        {
            chunkMesh = ChunkMeshRenderer.GenerateChunkMesh(testchunk, shader);
        };

        player = new Player(camera);
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
        player.chunk = testchunk;
        player.Update(args, keyboard, mouse);

        if (keyboard.IsKeyDown(Keys.Escape))
        {
            window.Close();
        }
    }
}
