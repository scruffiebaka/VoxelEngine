using OpenTK.Mathematics;
using VoxelEngine.Graphics.Rendering;

namespace VoxelEngine.World.Blocks;

public class Chunk
{
    public const int sizeXZ = 16;
    public const int sizeY = 321;

    public Block[] blocks;
    public Vector2i chunkPosition;
    public ChunkMesh? mesh;

    public event Action OnBlockChanged;

    public Chunk()
    {
        blocks = new Block[sizeXZ * sizeY * sizeXZ];
    }

    public ref Block Get(int x, int y, int z)
    {
        return ref blocks[x + sizeXZ * (y + sizeY * z)];
    }

    public void CreateBlock(int x, int y, int z, BlockId blockId)
    {
        blocks[x + sizeXZ * (y + sizeY * z)].id = (byte)blockId;
        OnBlockChanged?.Invoke();
    }

    public void DestroyBlock(int x, int y, int z)
    {
        blocks[x + sizeXZ * (y + sizeY * z)].id = (byte)BlockId.Air;
        OnBlockChanged?.Invoke();
    }
}
