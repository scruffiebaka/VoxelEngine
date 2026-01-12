using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using VoxelEngine.Graphics;
using VoxelEngine.Graphics.Rendering;
using VoxelEngine.World.Blocks;

namespace VoxelEngine.Core;

public class Player
{
    public Chunk chunk;
    Camera camera;

    Vector3 position;
    Vector3 velocity;

    static readonly Vector3 PlayerSize = new Vector3(0.6f, 1.8f, 0.6f);
    Vector3 Min(Vector3 pos) => pos - new Vector3(PlayerSize.X / 2f, 0f, PlayerSize.Z / 2f);
    Vector3 Max(Vector3 pos) => pos + new Vector3(PlayerSize.X / 2f, PlayerSize.Y, PlayerSize.Z / 2f);

    float gravity = -9.8f;

    bool _firstMove = true;
    Vector2 _lastPos;

    float sensitivity = 0.2f;
    float speed = 3.0f;

    float jumpSpeed = 5f;
    bool isGrounded = false;

    public Player(Camera camera)
    {
        this.camera = camera;
        position = new Vector3(8.0f, 16.0f, 8.0f);
    }

    public void Update(FrameEventArgs args, KeyboardState keyboard, MouseState mouse)
    {
        camera.Position = position + (Vector3.UnitY * 1.8f);

        PlayerMovement(keyboard);
        CameraMovement(mouse);

        velocity.Y += gravity * (float)args.Time;
        MoveAndCollide((float)args.Time);
    }

    private void PlayerMovement(KeyboardState Input)
    {
        Vector3 moveDir = Vector3.Zero;

        Vector3 forward = new Vector3(camera.Front.X, 0f, camera.Front.Z).Normalized();
        Vector3 right = new Vector3(camera.Right.X, 0f, camera.Right.Z).Normalized();

        if (Input.IsKeyDown(Keys.W)) moveDir += forward;
        if (Input.IsKeyDown(Keys.S)) moveDir -= forward;
        if (Input.IsKeyDown(Keys.A)) moveDir -= right;
        if (Input.IsKeyDown(Keys.D)) moveDir += right;

        if (moveDir.LengthSquared > 0)
            moveDir = moveDir.Normalized();

        velocity.X = moveDir.X * speed;
        velocity.Z = moveDir.Z * speed;

        if (Input.IsKeyDown(Keys.Space) && isGrounded)
        {
            velocity.Y = jumpSpeed;
            isGrounded = false;
        }
    }

    private void CameraMovement(MouseState mouse)
    {
        if (_firstMove)
        {
            _lastPos = new Vector2(mouse.X, mouse.Y);
            _firstMove = false;
        }
        else
        {
            var deltaX = mouse.X - _lastPos.X;
            var deltaY = mouse.Y - _lastPos.Y;
            _lastPos = new Vector2(mouse.X, mouse.Y);

            camera.Yaw += deltaX * sensitivity;
            camera.Pitch -= deltaY * sensitivity;
        }
    }

    bool Intersects(Vector3 minA, Vector3 maxA, Vector3 minB, Vector3 maxB)
    {
        return minA.X < maxB.X && maxA.X > minB.X &&
               minA.Y < maxB.Y && maxA.Y > minB.Y &&
               minA.Z < maxB.Z && maxA.Z > minB.Z;
    }
    void MoveAndCollide(float dt)
    {
        position.X += velocity.X * dt;
        if (Collides())
            position.X -= velocity.X * dt;

        position.Y += velocity.Y * dt;
        if (Collides())
        {
            if (velocity.Y < 0)
                isGrounded = true;

            position.Y -= velocity.Y * dt;
            velocity.Y = 0f;
        }
        else
        {
            isGrounded = false;
        }

        position.Z += velocity.Z * dt;
        if (Collides())
            position.Z -= velocity.Z * dt;
    }

    bool Collides()
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
}
