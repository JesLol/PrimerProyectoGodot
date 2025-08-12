using Godot;
using System;

public partial class GameManager : Node
{
	public int Score { get; set; } = 0;
	[Signal] public delegate void ScoreChangedEventHandler(int newScore);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void AddCoin(int numCoins)
	{
		Score += numCoins;
		EmitSignal(SignalName.ScoreChanged, Score);
	}
}
