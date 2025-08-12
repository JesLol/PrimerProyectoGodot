using Godot;
using System;

public partial class Checkpoint : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var animatedFlag = GetNode<AnimatedSprite2D>("AnimatedSprite2D"); //Obtiene el Sprite animado y lo convierte a variable
		animatedFlag.Play("animated-flag");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
