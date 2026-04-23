using Godot;
using System;

public partial class FumikoPlayer : CharacterBody2D
{
	public GameManager gameManager;
	[Export] public float Speed = 100.0f;
	[Export] public float RunSpeed = 250.0f;
	public float currentSpeed;
	[Export] public float JumpVelocity = -400.0f;
	
	private Vector2 checkpointPosition = new Vector2(100, 100);
	private bool _firstFlagIsChecked = false;
	private AnimatedSprite2D anim;
	private bool wasOnFloorLastFrame = false;
	public bool isDead = false;
	private Vector2 _initialPosition;
	
	private RayCast2D _rayFront;
	private RayCast2D _rayDiagonal;
	private RayCast2D _rayFloor;

	public float[] AiActions = new float[2]; // [0] eje X, [1] Salto
	public bool IsAiControlled = false;

	public float Reward = 0.0f;

	public override void _Ready()
	{
		anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		gameManager = GetNode<GameManager>("/root/GameManager");
		_initialPosition = GlobalPosition;
		checkpointPosition = GlobalPosition;
		
		_rayFront = GetNode<RayCast2D>("Sensors/RayFront");
		_rayDiagonal = GetNode<RayCast2D>("Sensors/RayDiagonal");
		_rayFloor = GetNode<RayCast2D>("Sensors/RayFloor");

		IsAiControlled = true;
	}

		public override void _PhysicsProcess(double delta)
		{
			ApplyGravity(delta);

			if (isDead)
			{
				MoveAndSlide();
				return;
			}

			Vector2 velocity = Velocity;
			
			float inputX = 0;
			if (IsAiControlled) 
			{
				inputX = AiActions[0]; 
			} 
			else 
			{
				// Control manual
				inputX = Input.GetAxis("move-left", "move-right");
			}

			velocity.X = inputX * (Input.IsActionPressed("run") ? RunSpeed : Speed);

			bool wantsToJump = IsAiControlled ? (AiActions[1] > 0.5f) : Input.IsActionJustPressed("jump");
			
			if (wantsToJump && IsOnFloor())
			{
				velocity.Y = JumpVelocity;
			}
			
			Velocity = velocity;
			MoveAndSlide();
			
			UpdateAnimation(velocity);
			CheckDeathCollision();
			
			if (IsAiControlled && !isDead) 
			{
				Reward -= 0.0001f; 
			}
		}
	
	public float[] GetObservations()
	{
		return new float[] 
		{
			_rayFront.IsColliding() ? 1.0f : 0.0f,    // ¿Pared enfrente?
			_rayDiagonal.IsColliding() ? 1.0f : 0.0f, // ¿Hay suelo adelante?
			_rayFloor.IsColliding() ? 1.0f : 0.0f,    // ¿Estoy tocando el piso?
			Velocity.X / RunSpeed,                   // Velocidad normalizada X
			Velocity.Y / 400.0f                      // Velocidad normalizada Y
		};
	}
	
	private void ApplyGravity(double delta)
	{
		if (!IsOnFloor())
		{
			Velocity += GetGravity() * (float)delta;
		}
	}

	private void HandleMovement(ref Vector2 velocity, double delta)
	{
		float inputX = 0;
		
		if (IsAiControlled) {
			inputX = AiActions[0]; // La IA enviará un valor entre -1 y 1
		} else {
			inputX = Input.GetAxis("move-left", "move-right");
		}

		velocity.X = inputX * (Input.IsActionPressed("run") ? RunSpeed : Speed);
	}

	private void HandleJumping(ref Vector2 velocity)
	{
		bool wantsToJump = IsAiControlled ? AiActions[1] > 0.5f : Input.IsActionJustPressed("jump");

		if (wantsToJump && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}
	}

	private void UpdateAnimation(Vector2 velocity)
	{
		bool isMoving = Mathf.Abs(velocity.X) > 0.1f;
		bool isRunning = Mathf.Abs(velocity.X) > 249.0f;
		bool isJumping = velocity.Y < -10f;
		bool isFalling = velocity.Y > 10f;
		bool isOnFloor = IsOnFloor();
		bool isDancing = false;

		if (Input.IsActionPressed("dance")) { isDancing = true; }
		if (Input.IsActionJustReleased("dance")) { isDancing = false; }

		if (velocity.X != 0)
		{
			anim.FlipH = velocity.X < 0;
		}

		if (!wasOnFloorLastFrame && isOnFloor)
		{
			anim.Play("landing");
		}
		else if (!isOnFloor)
		{
			if (isJumping) { anim.Play("jump-up"); }
			else if (isFalling) { anim.Play("falling"); }
		}
		else if (isDancing && isOnFloor && !isMoving && !isJumping && !isFalling)
		{
			anim.Play("dancing");
		}
		else if (Input.IsActionJustPressed("jump"))
		{
			anim.Play("jump_start");
		}
		else if (isMoving)
		{
			if (isRunning) { anim.Play("run"); }
			else { anim.Play("walking"); }
		}
		else
		{
			anim.Play("idle");
		}

		wasOnFloorLastFrame = isOnFloor;
	}

	private void CheckDeathCollision()
	{
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			var colision = GetSlideCollision(i);
			if (colision.GetCollider() is TileMapLayer tileMapLayer)
			{
				var tileSet = tileMapLayer.TileSet;
				Vector2I tileCoords = tileMapLayer.LocalToMap(colision.GetPosition());
				TileData tileData = tileMapLayer.GetCellTileData(tileCoords);
				int customData = tileSet.GetCustomDataLayerByName("deadly");
				if (tileData != null && customData == 0)
				{
					Die();
				}
			}
		}
	}

	public void RespawnPlayer()
	{
		isDead = false;
		Reward = 0.0f;
		GD.Print("Jugador reiniciado");
		GD.Print(Reward);
		GlobalPosition = _initialPosition; 
		anim.Play("idle");
		Velocity = Vector2.Zero;
		GD.Print("Fumiko reseteada al inicio.");
	}
	
	public void _first_flag_checked(Node2D body)
	{
		if (body == this && !_firstFlagIsChecked)
		{
			checkpointPosition = Position;
			_firstFlagIsChecked = true;
		}
	}

	public void Die()
	{
		if (isDead) { return; }
		Reward -= 1.0f;
		isDead = true;
		Velocity = Vector2.Zero;
		anim.Play("death");
		GetTree().CreateTimer(0f).Timeout += RespawnPlayer;
	}
}
