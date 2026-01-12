using OpenTK.Mathematics;

namespace VoxelEngine.World.Blocks;

public static class BlockModels
{
    public static readonly Vector3[][] FaceVertices =
    {
        //Front
        new[]
        {
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 1),
            new Vector3(1, 1, 1),
            new Vector3(0, 1, 1),
        },
        //Back
        new[]
        {
            new Vector3(1, 0, 0),
            new Vector3(0, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),
        },
        //Left
        new[]
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(0, 1, 1),
            new Vector3(0, 1, 0),
        },
        //Right
        new[]
        {
            new Vector3(1, 0, 1),
            new Vector3(1, 0, 0),
            new Vector3(1, 1, 0),
            new Vector3(1, 1, 1),
        },
        //Top
        new[]
        {
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1),
            new Vector3(1, 1, 0),
            new Vector3(0, 1, 0),
        },
        //Bottom
        new[]
        {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 1),
            new Vector3(0, 0, 1),
        },
    };

    public static readonly Vector3[] FaceNormals =
    {
        new( 0,  0,  1), // Front
        new( 0,  0, -1), // Back
        new(-1,  0,  0), // Left
        new( 1,  0,  0), // Right
        new( 0,  1,  0), // Top
        new( 0, -1,  0), // Bottom
    };

    public static readonly Vector2[] FaceUVs =
    {
        new(0, 0),
        new(1, 0),
        new(1, 1),
        new(0, 1),
    };
}

public enum BlockId : byte
{
    Air = 0,
    Dirt = 1,
    Stone = 2,
    Grass = 3,
    Bedrock = 4
}
