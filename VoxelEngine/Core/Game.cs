using System;

using OpenTK.Graphics.OpenGL4;
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
    Shader crosshairShader = new Shader("./Resources/Shaders/Crosshair/VertexShader.glsl", "./Resources/Shaders/Crosshair/FragmentShader.glsl");
    Shader selectionShader = new Shader("./Resources/Shaders/Selection/VertexShader.glsl", "./Resources/Shaders/Selection/FragmentShader.glsl");

    Window window;

    World.World? world;

    Chunk testchunk = new Chunk();
    Chunk testchunk2 = new Chunk();

    Player? player;

    public Game(int width, int height, Window window)
    {
        camera = new Camera(Vector3.UnitZ * 3, width / (float)height);
        this.window = window;
    }

    public void Load()
    {
        window.CursorState = CursorState.Grabbed;

        shader.Use();
        shader.SetVector3("lightDir", new Vector3(0.5f, -1.0f, 0.3f));
        ChunkMeshRenderer.shader = shader;

        crosshairShader.Use();
        CrosshairGenerate();

        selectionShader.Use();
        SelectionGenerate();

        Texture baseTex = new Texture(Texture.LoadFromFile("Resources/atlas.png"));
        Texture.Use(baseTex);
        TextureRegistry.RegisterBlocksTextures();

        #region TEMPORARY

        world = new World.World();
        world.generationPoint = new Vector2(0, 0);

        testchunk.chunkPosition = new Vector2i(0, 0);
        testchunk2.chunkPosition = new Vector2i(1, 0);

        for (int x = 0; x < Chunk.sizeXZ; x++)
        {
            for (int z = 0; z < Chunk.sizeXZ; z++)
            {
                for (int y = 0; y < Chunk.sizeY; y++)
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
        for (int x = 0; x < Chunk.sizeXZ; x++)
        {
            for (int z = 0; z < Chunk.sizeXZ; z++)
            {
                for (int y = 0; y < Chunk.sizeY; y++)
                {
                    ref Block block = ref testchunk2.Get(x, y, z);

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

        testchunk.mesh = ChunkMeshRenderer.GenerateChunkMesh(testchunk);
        testchunk2.mesh = ChunkMeshRenderer.GenerateChunkMesh(testchunk2);

        testchunk.OnBlockChanged += () =>
        {
            testchunk.mesh = ChunkMeshRenderer.GenerateChunkMesh(testchunk);
        };
        testchunk2.OnBlockChanged += () =>
        {
            testchunk2.mesh = ChunkMeshRenderer.GenerateChunkMesh(testchunk2);
        };

        world.chunks.Add(testchunk.chunkPosition, testchunk);
        world.chunks.Add(testchunk2.chunkPosition, testchunk2);

        #endregion

        player = new Player(camera);

        player.OnPlayerMove += () =>
        {
            world.UpdateGenerationPoint(player.position.Xz);
        };

        player.world = world;
    }

    public void Render(FrameEventArgs args)
    {
        shader.Use();
        shader.SetMatrix4("view", camera.GetViewMatrix());
        shader.SetMatrix4("projection", camera.GetProjectionMatrix());

        if (world != null)
        {
            foreach (Chunk chunk in world.chunks.Values)
            {
                ChunkMeshRenderer.DrawChunk(chunk);
            }
        }

        if(player.raycastBlockPosition != null)
        {
            selectionShader.Use();
            selectionShader.SetVector3("color", Vector3.One);
            SelectionRender((Vector3i)player.raycastBlockPosition);
        }

        crosshairShader.Use();
        CrosshairRender();
    }

    public void Update(FrameEventArgs args, KeyboardState keyboard, MouseState mouse)
    {
        if (player != null)
        {
            player.Update(args, keyboard, mouse);
        }

        if (keyboard.IsKeyDown(Keys.Escape))
        {
            window.Close();
        }
    }

    int crosshairVAO;
    private void CrosshairGenerate()
    {
        float[] crosshairVertices =
        {
            -0.02f,  0.0f,
            0.02f,  0.0f,

            0.0f, -0.02f,
            0.0f,  0.02f
        };

        crosshairVAO = GL.GenVertexArray();
        int crosshairVBO = GL.GenBuffer();

        GL.BindVertexArray(crosshairVAO);

        GL.BindBuffer(BufferTarget.ArrayBuffer, crosshairVBO);
        GL.BufferData(BufferTarget.ArrayBuffer, crosshairVertices.Length * sizeof(float), crosshairVertices, BufferUsageHint.StaticDraw);

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

        GL.BindVertexArray(0);
    }

    private void CrosshairRender()
    {
        GL.Disable(EnableCap.DepthTest);
        GL.BindVertexArray(crosshairVAO);
        GL.DrawArrays(PrimitiveType.Lines, 0, 4);
        GL.Enable(EnableCap.DepthTest);
    }

    int selectionVAO;

    private void SelectionGenerate()
    {
        float[] vertices =
        {
            0,0,0,  1,0,0,  1,1,0,  0,1,0,
            0,0,1,  1,0,1,  1,1,1,  0,1,1
        };

        uint[] indices =
        {
            0,1, 1,2, 2,3, 3,0,
            4,5, 5,6, 6,7, 7,4,
            0,4, 1,5, 2,6, 3,7
        };

        selectionVAO = GL.GenVertexArray();
        int outlineCubeVbo = GL.GenBuffer();
        int outlineCubeEbo = GL.GenBuffer();

        GL.BindVertexArray(selectionVAO);

        GL.BindBuffer(BufferTarget.ArrayBuffer, outlineCubeVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, outlineCubeEbo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

        GL.BindVertexArray(0);
    }

    private void SelectionRender(Vector3 selectionBlockPos)
    {
        GL.Enable(EnableCap.DepthTest);
        GL.DepthMask(false);
        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

        selectionShader.Use();
        selectionShader.SetMatrix4("model", Matrix4.CreateScale(1.01f) *
                        Matrix4.CreateTranslation(selectionBlockPos));
        selectionShader.SetMatrix4("view", camera.GetViewMatrix());
        selectionShader.SetMatrix4("projection", camera.GetProjectionMatrix());

        GL.BindVertexArray(selectionVAO);
        GL.DrawElements(PrimitiveType.Lines, 24, DrawElementsType.UnsignedInt, 0);

        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        GL.DepthMask(true);
    }

}
