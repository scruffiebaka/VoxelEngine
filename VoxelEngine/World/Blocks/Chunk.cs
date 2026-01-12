using OpenTK.Mathematics;

namespace VoxelEngine.World.Blocks;

public struct Chunk
{
    public const int size = 16;
    public Block[] blocks;
    public Vector3 world_position;

    public event Action OnBlockChanged;

    public Chunk()
    {
        blocks = new Block[size * size * size];
    }

    public ref Block Get(int x, int y, int z)
    {
        return ref blocks[x + size * (y + size * z)];
    }

    public void CreateBlock(int x, int y, int z, BlockId blockId)
    {
        blocks[x + size * (y + size * z)].id = (byte)blockId;
        OnBlockChanged?.Invoke();
    }

    public void DestroyBlock(int x, int y, int z)
    {
        blocks[x + size * (y + size * z)].id = (byte)BlockId.Air;
        OnBlockChanged?.Invoke();
    }
}
