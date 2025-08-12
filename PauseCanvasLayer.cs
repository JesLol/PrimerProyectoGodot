using Godot;
using System;

public partial class PauseCanvasLayer : CanvasLayer
{
	private bool isPaused = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = false; //Oculta el ui de la pausa al iniciar
	}
    public override void _Input(InputEvent @event)
    {
		if (@event.IsActionPressed("pause")) //Ejecutar si el boton de pausa esta presionado
		{
			TogglePause();
		}
    }
	public void TogglePause()
	{
		isPaused = !isPaused;
		Visible = isPaused;
		GetTree().Paused = isPaused; //GetTree().Paused le dice al arbol de la escena que pause o reanude el juego (pausa como tal el motor)
	}
	public void ContinuePressedBtn()
	{
		TogglePause();
	}
	public void ExitPressedButton()
	{
		GetTree().Quit(); //Cierra el juego
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
