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

        PlayerPhysics.BlockRaycastHit? block = PlayerPhysics.RaycastBlocks(camera.Position, camera.Front, 8, chunk);
        if (block != null)
        {
            if (mouse.IsButtonPressed(MouseButton.Left))
            {
                chunk.DestroyBlock(block.Value.Position.X, block.Value.Position.Y, block.Value.Position.Z);
            }
            if (mouse.IsButtonPressed(MouseButton.Right))
            {
                Vector3i placePos = block.Value.Position + block.Value.Normal;
                chunk.CreateBlock(placePos.X, placePos.Y, placePos.Z, BlockId.Dirt);
            }
        }
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

    void MoveAndCollide(float dt)
    {
        position.X += velocity.X * dt;
        if (PlayerPhysics.Collides(position, chunk))
            position.X -= velocity.X * dt;

        position.Y += velocity.Y * dt;
        if (PlayerPhysics.Collides(position, chunk))
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
        if (PlayerPhysics.Collides(position, chunk))
            position.Z -= velocity.Z * dt;
    }
}
