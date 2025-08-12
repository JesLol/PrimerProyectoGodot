using Godot;
using System;

public partial class Door : Area2D
{
	private string nextScene = "res://Scenes/Level2.tscn";
	private void OnBodyEntered(Node2D body)
	{
		if (body is FumikoPlayer player)
		{
			GetTree().ChangeSceneToFile(nextScene); //Cambia la escena al siguiente nivel
		}
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
