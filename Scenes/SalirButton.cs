using Godot;
using System;

public partial class SalirButton : Button
{
	[Export]
	public string MainMenuPath = "res://main_menu.tscn";

	// Método que se activa cuando el botón es presionado
	private void _on_pressed()
	{
		GD.Print("Button pressed, returning to main menu.");
		GetTree().ChangeSceneToFile(MainMenuPath);
	}
}

