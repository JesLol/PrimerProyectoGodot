using Godot;
using System;

public partial class MainMenu : Control
{
	private VBoxContainer menuButtons;
	private int focusedIndex = 0;
	public float FadeSpeed = 2f;
	public ColorRect fadeRect;
	private bool fadingOut = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		menuButtons = GetNode<VBoxContainer>("VBoxContainer"); // Tu lista de botones
		menuButtons.GetChild<Button>(0).GrabFocus(); // Elige el primer botón al iniciar
													 // Inicializa el rectángulo de color para la pantalla de inicio
		fadeRect = GetNode<ColorRect>("ColorRect");
		var c = fadeRect.Modulate;
		c.A = 0f;
		fadeRect.Modulate = c;
		// Reproduce la música de fondo
		var music = GetNode<AudioStreamPlayer>("AudioStreamPlayer");

		if (music.Stream is AudioStreamOggVorbis oggStream)
		{
			oggStream.Loop = true;
		}
		else if (music.Stream is AudioStreamWav wavStream)
		{
			wavStream.LoopMode = AudioStreamWav.LoopModeEnum.Forward;
		}

		music.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (fadingOut)
		{
			var color = fadeRect.Modulate;
			float.TryParse(delta.ToString(), out float deltaFloat);
			color.A += FadeSpeed * deltaFloat;
			if (color.A >= 1f)
			{
				color.A = 1f;
				fadingOut = false;
				GetTree().ChangeSceneToFile("res://World/world.tscn");
			}
			fadeRect.Modulate = color;
		}
	}
	public void _start_button_pressed()
	{
		fadingOut = true;
	}
	public void _on_exitButton_pressed()
	{
		GetTree().Quit(); // Cierra el juego
	}
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui-down"))
		{
			focusedIndex = (focusedIndex + 1) % menuButtons.GetChildCount();
			menuButtons.GetChild<Button>(focusedIndex).GrabFocus();
		}
		else if (@event.IsActionPressed("ui-up"))
		{
			focusedIndex = (focusedIndex - 1 + menuButtons.GetChildCount()) % menuButtons.GetChildCount();
			menuButtons.GetChild<Button>(focusedIndex).GrabFocus();
		}
		else if (@event.IsActionPressed("ui-accept"))
		{
			// Llama al método del botón enfocado, por ejemplo, para reanudar el juego
			menuButtons.GetChild<Button>(focusedIndex).EmitSignal(Button.SignalName.Pressed);
		}
	}
}
