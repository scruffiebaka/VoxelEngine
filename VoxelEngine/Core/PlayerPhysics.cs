using System;
using OpenTK.Mathematics;
using VoxelEngine.Graphics.Rendering;
using VoxelEngine.World.Blocks;

namespace VoxelEngine.Core;

public static class PlayerPhysics
{
    static readonly Vector3 PlayerSize = new Vector3(0.6f, 1.8f, 0.6f);
    static Vector3 Min(Vector3 pos) => pos - new Vector3(PlayerSize.X / 2f, 0f, PlayerSize.Z / 2f);
    static Vector3 Max(Vector3 pos) => pos + new Vector3(PlayerSize.X / 2f, PlayerSize.Y, PlayerSize.Z / 2f);

    static bool Intersects(Vector3 minA, Vector3 maxA, Vector3 minB, Vector3 maxB)
    {
        return minA.X < maxB.X && maxA.X > minB.X &&
               minA.Y < maxB.Y && maxA.Y > minB.Y &&
               minA.Z < maxB.Z && maxA.Z > minB.Z;
    }

    public static bool Collides(Vector3 position, Chunk chunk)
    {
        Vector3 min = Min(position);
        Vector3 max = Max(position);

        for (int x = (int)MathF.Floor(min.X); x <= MathF.Floor(max.X); x++)
            for (int y = (int)MathF.Floor(min.Y); y <= MathF.Floor(max.Y); y++)
                for (int z = (int)MathF.Floor(min.Z); z <= MathF.Floor(max.Z); z++)
                {
                    if (!ChunkMeshRenderer.IsAir(chunk, x, y, z))
                    {
                        Vector3 bMin = new Vector3(x, y, z);
                        Vector3 bMax = bMin + Vector3.One;

                        if (Intersects(min, max, bMin, bMax))
                            return true;
                    }
                }

        return false;
    }

    public struct BlockRaycastHit
    {
        public Vector3i Position;
        public Vector3i Normal;
    }

    public static BlockRaycastHit? RaycastBlocks(
        Vector3 origin,
        Vector3 direction,
        float maxDistance,
        Chunk chunk
    )
    {
        direction = direction.Normalized();

        int x = (int)MathF.Floor(origin.X);
        int y = (int)MathF.Floor(origin.Y);
        int z = (int)MathF.Floor(origin.Z);

        int stepX = direction.X > 0 ? 1 : -1;
        int stepY = direction.Y > 0 ? 1 : -1;
        int stepZ = direction.Z > 0 ? 1 : -1;

        float tDeltaX = direction.X != 0 ? MathF.Abs(1f / direction.X) : float.MaxValue;
        float tDeltaY = direction.Y != 0 ? MathF.Abs(1f / direction.Y) : float.MaxValue;
        float tDeltaZ = direction.Z != 0 ? MathF.Abs(1f / direction.Z) : float.MaxValue;

        float tMaxX = direction.X != 0
            ? ((stepX > 0 ? x + 1 : x) - origin.X) / direction.X
            : float.MaxValue;

        float tMaxY = direction.Y != 0
            ? ((stepY > 0 ? y + 1 : y) - origin.Y) / direction.Y
            : float.MaxValue;

        float tMaxZ = direction.Z != 0
            ? ((stepZ > 0 ? z + 1 : z) - origin.Z) / direction.Z
            : float.MaxValue;

        Vector3i normal = Vector3i.Zero;
        float t = 0f;

        while (t <= maxDistance)
        {
            if (!ChunkMeshRenderer.IsAir(chunk, x, y, z))
            {
                return new BlockRaycastHit
                {
                    Position = new Vector3i(x, y, z),
                    Normal = normal
                };
            }

            if (tMaxX < tMaxY)
            {
                if (tMaxX < tMaxZ)
                {
                    x += stepX;
                    t = tMaxX;
                    tMaxX += tDeltaX;
                    normal = new Vector3i(-stepX, 0, 0);
                }
                else
                {
                    z += stepZ;
                    t = tMaxZ;
                    tMaxZ += tDeltaZ;
                    normal = new Vector3i(0, 0, -stepZ);
                }
            }
            else
            {
                if (tMaxY < tMaxZ)
                {
                    y += stepY;
                    t = tMaxY;
                    tMaxY += tDeltaY;
                    normal = new Vector3i(0, -stepY, 0);
                }
                else
                {
                    z += stepZ;
                    t = tMaxZ;
                    tMaxZ += tDeltaZ;
                    normal = new Vector3i(0, 0, -stepZ);
                }
            }
        }

        return null;
    }

}
