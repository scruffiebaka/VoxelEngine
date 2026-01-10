using OpenTK.Mathematics;

namespace VoxelEngine.World.Blocks;

public struct Chunk
{
    public const int size = 16;
    public Block[] blocks;
    public Vector3 world_position;

    public Chunk()
    {
        blocks = new Block[size * size * size];
    }

    public ref Block Get(int x, int y, int z)
    {
        return ref blocks[x + size * (y + size * z)];
    }
}
