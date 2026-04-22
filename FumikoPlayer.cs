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
	private bool isDead = false;
	private Vector2 _initialPosition;

	public override void _Ready()
	{
		anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		gameManager = GetNode<GameManager>("/root/GameManager");
		_initialPosition = GlobalPosition;
		checkpointPosition = GlobalPosition;
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
		
		HandleMovement(ref velocity, delta);
		HandleJumping(ref velocity);
		
		Velocity = velocity;
		MoveAndSlide();
		UpdateAnimation(velocity);
		CheckDeathCollision();
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
		currentSpeed = Speed;
		if (Input.IsActionPressed("run"))
		{
			currentSpeed = RunSpeed;
		}

		velocity.X = 0;
		if (Input.IsActionPressed("move-left"))
		{
			velocity.X -= currentSpeed;
		}
		if (Input.IsActionPressed("move-right"))
		{
			velocity.X += currentSpeed;
		}
	}

	private void HandleJumping(ref Vector2 velocity)
	{
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
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
		isDead = true;
		Velocity = Vector2.Zero;
		anim.Play("death");
		GetTree().CreateTimer(1.5f).Timeout += RespawnPlayer;
	}
}
