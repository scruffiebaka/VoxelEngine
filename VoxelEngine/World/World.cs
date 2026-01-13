using OpenTK.Mathematics;

using VoxelEngine.Graphics.Rendering;
using VoxelEngine.World.Blocks;

namespace VoxelEngine.World;

public class World
{
    public Vector2 generationPoint;
    public Dictionary<Vector2i, Chunk> chunks = new Dictionary<Vector2i, Chunk>();

    private const float generationPointShiftThreshold = 5.0f;

    public void UpdateGenerationPoint(Vector2 playerPosition)
    {
        if (Vector2.Distance(generationPoint, playerPosition) >= generationPointShiftThreshold)
        {
            generationPoint = playerPosition;
        }
    }
    public static Vector2i ToChunkCoordinate(Vector2 pos)
    {
        Vector2i chunkPos = new(
            (int)MathF.Floor(pos.X / Chunk.sizeXZ),
            (int)MathF.Floor(pos.Y / Chunk.sizeXZ)
        );

        return chunkPos;
    }

    public static Chunk? GetChunk(Vector3i pos, World world)
    {
        Vector3i chunkPos = new(
            (int)MathF.Floor(pos.X / Chunk.sizeXZ),
            0,
            (int)MathF.Floor(pos.Z / Chunk.sizeXZ)
        );

        return world.chunks.TryGetValue(chunkPos.Xz, out var chunk) ? chunk : null;
    }

    public static Vector3i WorldToLocalChunk(Vector3i worldPos, Chunk chunk)
    {
        return new Vector3i(
            worldPos.X - chunk.chunkPosition.X * Chunk.sizeXZ,
            worldPos.Y,
            worldPos.Z - chunk.chunkPosition.Y * Chunk.sizeXZ
        );
    }

    public static bool isSolid(Vector3i blockPosition, World world)
    {
        Chunk? chunk = GetChunk(blockPosition, world);
        if (chunk == null)
            return false;

        int localX = blockPosition.X - chunk.chunkPosition.X * Chunk.sizeXZ;
        int localY = blockPosition.Y;
        int localZ = blockPosition.Z - chunk.chunkPosition.Y * Chunk.sizeXZ;

        return !ChunkMeshRenderer.IsAir(chunk, localX, localY, localZ);
    }
}
