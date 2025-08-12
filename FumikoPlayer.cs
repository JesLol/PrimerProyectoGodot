using Godot;
using System;

public partial class FumikoPlayer : CharacterBody2D
{
	public GameManager gameManager;
	[Export] public float Speed = 100.0f;
	[Export] public float RunSpeed = 250.0f;
	public float currentSpeed;
	[Export] public float JumpVelocity = -400.0f;
	// public int score { get; private set; } = 0;
	private Vector2 checkpointPosition = new Vector2(100, 100); //Guarda el valor de respawn del jugador
	private bool _firstFlagIsChecked = false;
	private AnimatedSprite2D anim;
	private bool wasOnFloorLastFrame = false;
	private bool isDead = false;
	public override void _Ready()
	{
		anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		gameManager = GetNode<GameManager>("/root/GameManager"); //Obtiene el nodo GameManager que esta en la escena principal
	}


	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor()) //Si no esta en el piso agrega la gravedad
		{
			velocity += GetGravity() * (float)delta;
		}
		if (isDead)
		{
			Velocity += GetGravity() * (float)delta;
			MoveAndSlide();
			return;
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
		bool isRunning = Math.Abs(velocity.X) > 249.0f;
		bool isJumping = velocity.Y < -10f;
		bool isFalling = velocity.Y > 10f;
		bool isOnFloor = IsOnFloor();
		bool isDancing = false;
		if (!isDancing && Input.IsActionPressed("dance")){isDancing = true;}
		if (isDancing && Input.IsActionJustReleased("dance")){isDancing = false;}

		// Actualizar la animación del sprite
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
		else if (isDancing && isOnFloor && !isMoving && !isJumping && !isFalling){anim.Play("dancing");}
		else if (Input.IsActionJustPressed("jump")) { anim.Play("jump_start"); }
		else if (isMoving)
		{
			if (isRunning) { anim.Play("run"); }
			else { anim.Play("walking"); }
		}
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
					isDead = true;
					Velocity = Vector2.Zero; //Detiene el movimiento
					anim.Play("death");
					GetTree().CreateTimer(1.5f).Timeout += RespawnPlayer;
				}
			}
		}
	}
	public void RespawnPlayer()
	{
		isDead = false;
		Position = checkpointPosition;
		anim.Play("idle");
		Velocity = Vector2.Zero;
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
	public void Die(){
		if (isDead) { return; }
		isDead = true;
		Velocity = Vector2.Zero;
		anim.Play("death");
		GetTree().CreateTimer(1.5f).Timeout += RespawnPlayer;
	}

}
