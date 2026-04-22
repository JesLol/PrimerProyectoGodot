using Godot;
using System;

public partial class AiGoal : Area2D
{
	[Signal]
	public delegate void PlayerReachedGoalEventHandler();

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
	{
		// Usamos el nombre de tu clase de jugador
		if (body is FumikoPlayer player)
		{
			GD.Print("¡Meta alcanzada!");
			// Si SignalName te sigue dando error tras compilar, usa la cadena de texto:
			EmitSignal(SignalName.PlayerReachedGoal); 
			player.RespawnPlayer();
		}
	}
}
