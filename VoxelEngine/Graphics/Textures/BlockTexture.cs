using System;
using System.ComponentModel.DataAnnotations;
using OpenTK.Mathematics;
using VoxelEngine.World.Blocks;

namespace VoxelEngine.Graphics.Textures;

public struct BlockTexture
{
    public BlockAtlasRegion[] faces;
    public BlockTexture(
        BlockAtlasRegion top,
        BlockAtlasRegion bottom,
        BlockAtlasRegion front,
        BlockAtlasRegion back,
        BlockAtlasRegion right,
        BlockAtlasRegion left)
    {
        faces = new BlockAtlasRegion[6]
        {
            front,
            back,
            left,
            right,
            top,
            bottom
        };
    }
}

public struct BlockAtlasRegion
{
    public Vector2[] uvs;
    public BlockAtlasRegion(Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector2 uv3)
    {
        uvs = new Vector2[4]
        {
            uv0, uv1, uv2, uv3
        };
    }
}