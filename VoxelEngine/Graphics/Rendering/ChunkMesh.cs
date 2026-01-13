using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace VoxelEngine.Graphics.Rendering;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Vertex
{
    public Vector3 Position;
    public Vector3 Normal;
    public Vector2 TexCoord;
}

public class ChunkMesh
{
    public Vertex[] vertices;
    public uint[] indices;

    internal int VertexArrayObject, VertexBufferObject, ElementBufferObject;
    Shader shader;

    public ChunkMesh(Vertex[] vertices, uint[] indices, Shader shader)
    {
        this.vertices = vertices;
        this.indices = indices;
        this.shader = shader;

        SetupChunkMesh();
    }

    private void SetupChunkMesh()
    {
        VertexArrayObject = GL.GenVertexArray();
        VertexBufferObject = GL.GenBuffer();
        ElementBufferObject = GL.GenBuffer();

        GL.BindVertexArray(VertexArrayObject);

        GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Marshal.SizeOf<Vertex>(), vertices, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        var positionLocation = shader.GetAttribLocation("aPos");
        GL.EnableVertexAttribArray(positionLocation);
        GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

        var normalLocation = shader.GetAttribLocation("aNormal");
        GL.EnableVertexAttribArray(normalLocation);
        GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

        var texCoordLocation = shader.GetAttribLocation("aTexCoords");
        GL.EnableVertexAttribArray(texCoordLocation);
        GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));

        GL.BindVertexArray(0);
    }
}
