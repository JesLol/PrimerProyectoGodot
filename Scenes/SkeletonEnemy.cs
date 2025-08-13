using Godot;
using System;

public partial class SkeletonEnemy : CharacterBody2D
{
	[Export] public float Speed = 50.0f;
	private int direction = -1;
	private RayCast2D floorDetector;
	private AnimatedSprite2D anim;
	public override void _Ready()
	{
		floorDetector = GetNode<RayCast2D>("FloorDetector");
		anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		velocity.Y += 980 * (float)delta; //Gravedad
										  // Add the gravity.
		velocity.X = Speed * direction; //Esto  lo mueve siempre dependiendo la direccion
		if (floorDetector != null)
		{
			floorDetector.TargetPosition = new Vector2(20 * direction, 30); //Esto mueve el rayo de deteccion a la direccion 25, 25 0 -25, 25 con respecto al personaje
			floorDetector.ForceRaycastUpdate(); // Actualiza el RayCast2D
			if (!floorDetector.IsColliding())
			{
				direction *= -1; //Cambia la direccion del personaje multiplicando por -1
			}
			if (IsOnWall())
			{
				direction *= -1;
				velocity.X = 0;
			}
		}
		


		Velocity = velocity;
		ActualizarAnimacion(velocity);
		MoveAndSlide();
	}
	public void ActualizarAnimacion(Vector2 velocidad)
	{
		bool isMoving = Math.Abs(velocidad.X) > 0.1;
		if (velocidad.X != 0)
		{
			anim.FlipH = velocidad.X < 0;
		}
		if (isMoving)
		{
			anim.Play("walk");
		}
	}
	public void _on_damage_area_body_entered(Node2D body)
	{
		if (body is FumikoPlayer player)
		{
			player.Die();
		}
	}
}
