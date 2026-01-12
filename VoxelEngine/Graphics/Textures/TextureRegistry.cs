using System;
using OpenTK.Mathematics;
using VoxelEngine.World.Blocks;

namespace VoxelEngine.Graphics.Textures;

public static class TextureRegistry
{
    public static Dictionary<BlockId, BlockTexture> blocktextures = new Dictionary<BlockId, BlockTexture>();

    public static void RegisterBlocksTextures()
    {
        blocktextures.Add(BlockId.Dirt, new BlockTexture(
            GetBlockAtlasRegion(0),
            GetBlockAtlasRegion(0),
            GetBlockAtlasRegion(0),
            GetBlockAtlasRegion(0),
            GetBlockAtlasRegion(0),
            GetBlockAtlasRegion(0)
        ));
        blocktextures.Add(BlockId.Grass, new BlockTexture(
            GetBlockAtlasRegion(3),
            GetBlockAtlasRegion(0),
            GetBlockAtlasRegion(4),
            GetBlockAtlasRegion(4),
            GetBlockAtlasRegion(4),
            GetBlockAtlasRegion(4)
        ));
        blocktextures.Add(BlockId.Stone, new BlockTexture(
            GetBlockAtlasRegion(1),
            GetBlockAtlasRegion(1),
            GetBlockAtlasRegion(1),
            GetBlockAtlasRegion(1),
            GetBlockAtlasRegion(1),
            GetBlockAtlasRegion(1)
        ));
        blocktextures.Add(BlockId.Bedrock, new BlockTexture(
            GetBlockAtlasRegion(2),
            GetBlockAtlasRegion(2),
            GetBlockAtlasRegion(2),
            GetBlockAtlasRegion(2),
            GetBlockAtlasRegion(2),
            GetBlockAtlasRegion(2)
        ));
    }

    public static BlockAtlasRegion GetBlockAtlasRegion(int index, int atlasSize = 128, int tileSize = 16)
    {
        int tilesPerRow = atlasSize / tileSize;
        int x = index % tilesPerRow;
        int y = index / tilesPerRow;

        float tileUnit = tileSize / (float)atlasSize;

        float flippedY = 1.0f - (y + 1) * tileUnit;

        Vector2 bottomLeft = new Vector2(x * tileUnit, flippedY);
        Vector2 bottomRight = new Vector2((x + 1) * tileUnit, flippedY);
        Vector2 topRight = new Vector2((x + 1) * tileUnit, flippedY + tileUnit);
        Vector2 topLeft = new Vector2(x * tileUnit, flippedY + tileUnit);

        return new BlockAtlasRegion(bottomLeft, bottomRight, topRight, topLeft);
    }
}
