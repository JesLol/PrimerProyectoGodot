using Godot;
using System;

public partial class CharacterBody2d : CharacterBody2D
{
    [Export] public float Speed = 100.0f;
    [Export] public float RunSpeed = 250.0f;
    public float currentSpeed;
    [Export] public float JumpVelocity = -400.0f;
    private Vector2 checkpointPosition = new Vector2(100, 100); //Guarda el valor de respawn del jugador
    private bool _firstFlagIsChecked = false;
    private AnimatedSprite2D anim;
    private bool wasOnFloorLastFrame = false;
    public override void _Ready()
    {
        anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }


    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        // Add the gravity.
        if (!IsOnFloor()) //Si no esta en el piso agrega la gravedad
        {
            velocity += GetGravity() * (float)delta;
        }
        float currentSpeed = Speed;
        if (Input.IsActionPressed("run"))
        {
            currentSpeed = RunSpeed;
        }
        // Handle Jump.
        if (Input.IsActionJustPressed("jump") && IsOnFloor())
        {
            velocity.Y = JumpVelocity;
        }
        velocity.X = 0;
        if (Input.IsActionPressed("move-left"))
        {
            velocity.X -= currentSpeed;
        }
            
        if (Input.IsActionPressed("move-right"))
            velocity.X += currentSpeed;


        Velocity = velocity;
        MoveAndSlide();
        UpdateAnimation(velocity);
        ColisionDeMuerte();
    }
    private void UpdateAnimation(Vector2 velocity)
    {
        bool isMoving = Math.Abs(velocity.X) > 0.1f;
        bool isJumping = velocity.Y < -10f;
        bool isFalling = velocity.Y > 10f;
        bool isOnFloor = IsOnFloor();

        // Mirar hacia la dirección de movimiento
        if (velocity.X != 0)
            anim.FlipH = velocity.X < 0;

        if (!wasOnFloorLastFrame && isOnFloor) { anim.Play("landing"); }
        else if (!isOnFloor)
        {
            if (isJumping)
                anim.Play("jump-up");
            else if (isFalling)
                anim.Play("falling");
        }
        else if (Input.IsActionJustPressed("jump")) { anim.Play("jump_start"); }
        else if (isMoving) { anim.Play("run"); }
        else { anim.Play("idle"); }

        wasOnFloorLastFrame = isOnFloor;
    }
    public void ColisionDeMuerte()
    {
        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            var colision = GetSlideCollision(i);
            // GD.Print(colision);
            if (colision.GetCollider() is TileMapLayer tileMapLayer)
            {
                var tileSet = tileMapLayer.TileSet;
                Vector2I tileCoords = tileMapLayer.LocalToMap(colision.GetPosition());
                TileData tileData = tileMapLayer.GetCellTileData(tileCoords);
                int customData = tileSet.GetCustomDataLayerByName("deadly"); // Obtiene el Id del dataLayer "deadly", si no lo encuentra devuelve -1
                if (tileData != null && customData == 0)
                {
                    Position = checkpointPosition;
                    // Position = new Vector2(100, 100);
                }
            }
        }
    }
    public void _first_flag_checked(Node2D body)
    {
        // Verifica si el cuerpo que entró es tu personaje
        if (body == this && !_firstFlagIsChecked)
        {
            // Guarda la posición actual como el nuevo checkpoint
            checkpointPosition = Position;
            _firstFlagIsChecked = true;
        }
    }
}
