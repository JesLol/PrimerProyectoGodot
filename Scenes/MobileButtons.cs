using Godot;
using System;

public partial class MobileButtons : CanvasLayer
{
	[Export] public bool AlwaysVisible = true;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		string osName = OS.GetName();

		// Oculta el CanvasLayer si el juego no se ejecuta en un dispositivo móvil.
		if (osName != "Android" && osName != "iOS" && !AlwaysVisible)
		{
			Visible = false;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
