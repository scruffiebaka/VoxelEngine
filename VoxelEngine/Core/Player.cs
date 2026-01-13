using System;

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

using VoxelEngine.Graphics;
using VoxelEngine.World.Blocks;

namespace VoxelEngine.Core;

public class Player
{
    Camera camera;
    public World.World world;

    public Vector3 position;
    Vector3 velocity;

    float gravity = -9.8f;

    bool _firstMoveMouse = true;
    Vector2 _lastPosMouse;
    Vector3 lastPosition;

    float sensitivity = 0.2f;
    float speed = 3.0f;

    float jumpSpeed = 5f;
    bool isGrounded = false;

    //Events
    public event Action OnPlayerMove;

    public Player(Camera camera)
    {
        this.camera = camera;
        position = new Vector3(8.0f, 16.0f, 8.0f);
    }

    public void Update(FrameEventArgs args, KeyboardState keyboard, MouseState mouse)
    {
        Vector3 currentPosition = position;

        if (!currentPosition.Equals(lastPosition))
        {
            OnPlayerMove?.Invoke();
            lastPosition = currentPosition;
        }

        camera.Position = position + (Vector3.UnitY * 1.8f);

        PlayerMovement(keyboard);
        CameraMovement(mouse);

        velocity.Y += gravity * (float)args.Time;
        MoveAndCollide((float)args.Time);

        Test_BlockPlaceAndDestroy(keyboard, mouse);
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
        if (_firstMoveMouse)
        {
            _lastPosMouse = new Vector2(mouse.X, mouse.Y);
            _firstMoveMouse = false;
        }
        else
        {
            var deltaX = mouse.X - _lastPosMouse.X;
            var deltaY = mouse.Y - _lastPosMouse.Y;
            _lastPosMouse = new Vector2(mouse.X, mouse.Y);

            camera.Yaw += deltaX * sensitivity;
            camera.Pitch -= deltaY * sensitivity;
        }
    }

    void MoveAndCollide(float dt)
    {
        position.X += velocity.X * dt;
        if (PlayerPhysics.Collides(position, world))
            position.X -= velocity.X * dt;

        position.Y += velocity.Y * dt;
        if (PlayerPhysics.Collides(position, world))
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
        if (PlayerPhysics.Collides(position, world))
            position.Z -= velocity.Z * dt;
    }

    public Vector3i? raycastBlockPosition;
    BlockId inventoryBlock = BlockId.Dirt;
    void Test_BlockPlaceAndDestroy(KeyboardState input, MouseState mouse)
    {
        if (input.IsKeyPressed(Keys.D1))
        {
            inventoryBlock = BlockId.Dirt;
        }
        if (input.IsKeyPressed(Keys.D2))
        {
            inventoryBlock = BlockId.Grass;
        }
        if (input.IsKeyPressed(Keys.D3))
        {
            inventoryBlock = BlockId.Stone;
        }
        if (input.IsKeyPressed(Keys.D4))
        {
            inventoryBlock = BlockId.Bedrock;
        }

        PlayerPhysics.BlockRaycastHit? block = PlayerPhysics.RaycastBlocks(camera.Position, camera.Front, 8, world);
        if (block != null)
        {
            raycastBlockPosition = block.Value.Position;
            if (mouse.IsButtonPressed(MouseButton.Left))
            {
                Chunk? chunk = World.World.GetChunk(block.Value.Position, world);
                if (chunk == null)
                    return;

                Vector3i localPosition = World.World.WorldToLocalChunk(block.Value.Position, chunk);
                chunk.DestroyBlock(localPosition.X, localPosition.Y, localPosition.Z);
            }
            if (mouse.IsButtonPressed(MouseButton.Right))
            {
                Vector3i placePos = block.Value.Position + block.Value.Normal;

                Chunk? chunk = World.World.GetChunk(placePos, world);
                if (chunk == null)
                    return;

                Vector3i localPosition = World.World.WorldToLocalChunk(placePos, chunk);
                chunk.CreateBlock(localPosition.X, localPosition.Y, localPosition.Z, inventoryBlock);
            }
        }
        else
        {
            raycastBlockPosition = null;
        }
    }
}
