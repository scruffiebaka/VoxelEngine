using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using VoxelEngine.World.Blocks;

namespace VoxelEngine.Graphics.Rendering;

public static class ChunkMeshRenderer
{
    public static void Draw(Vector3 position, ChunkMesh mesh, Shader shader)
    {
        shader.Use();

        shader.SetMatrix4("model",
                Matrix4.CreateScale(Vector3.One)
                * Matrix4.CreateFromQuaternion(Quaternion.Identity)
                * Matrix4.CreateTranslation(position));

        GL.BindVertexArray(mesh.VertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, mesh.indices.Length, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);
    }

    static readonly Vector3i[] Directions =
    {
        new( 0,  0,  1), //Front
        new( 0,  0, -1), //Back
        new(-1,  0,  0), //Left
        new( 1,  0,  0), //Right
        new( 0,  1,  0), //Top
        new( 0, -1,  0), //Bottom
    };

    static bool IsAir(Chunk chunk, int x, int y, int z)
    {
        if (x < 0 || y < 0 || z < 0 ||
            x >= Chunk.size || y >= Chunk.size || z >= Chunk.size)
            return true; // outside chunk = air

        int index = x + Chunk.size * (y + Chunk.size * z);
        return chunk.blocks[index].id == (byte)BlockId.Air;
    }

    public static ChunkMesh GenerateChunkMesh(Chunk chunk, Shader shader)
    {
        List<Vertex> vertices = new();
        List<uint> indices = new();
        uint indexOffset = 0;

        for (int i = 0; i < chunk.blocks.Length; i++)
        {
            if (chunk.blocks[i].id == (byte)BlockId.Air)
                continue;

            int x = i % Chunk.size;
            int y = (i / Chunk.size) % Chunk.size;
            int z = i / (Chunk.size * Chunk.size);

            for (int face = 0; face < 6; face++)
            {
                Vector3i d = Directions[face];

                if (!IsAir(chunk, x + d.X, y + d.Y, z + d.Z))
                    continue;

                for (int v = 0; v < 4; v++)
                {
                    vertices.Add(new Vertex
                    {
                        Position = new Vector3(x, y, z) + BlockModels.FaceVertices[face][v],
                        Normal = BlockModels.FaceNormals[face],
                        TexCoord = BlockModels.FaceUVs[v]
                    });
                }

                indices.Add(indexOffset + 0);
                indices.Add(indexOffset + 1);
                indices.Add(indexOffset + 2);
                indices.Add(indexOffset + 0);
                indices.Add(indexOffset + 2);
                indices.Add(indexOffset + 3);

                indexOffset += 4;

            }
        }
        return new ChunkMesh(vertices.ToArray(), indices.ToArray(), shader);
    }
}
