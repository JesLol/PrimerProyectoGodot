using Godot;
using System;

public partial class PauseCanvasLayer : CanvasLayer
{
	private VBoxContainer menuButtons;
    private int focusedIndex = 0;
	private bool isPaused = false;
	private int musicBusIndex = AudioServer.GetBusIndex("Music");
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = false; //Oculta el ui de la pausa al iniciar
		menuButtons = GetNode<VBoxContainer>("Panel/VBoxContainer"); // Tu lista de botones
		menuButtons.GetChild<Button>(0).GrabFocus(); // Elige el primer botón al iniciar
	}
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("pause")) //Ejecutar si el boton de pausa esta presionado
		{
			TogglePause();
		}
		if (isPaused)
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
	public void TogglePause()
	{
		isPaused = !isPaused;
		Visible = isPaused;
		GetTree().Paused = isPaused; //GetTree().Paused le dice al arbol de la escena que pause o reanude el juego (pausa como tal el motor)
		if (isPaused)
        {
            TweenVolumeDown();
        }
        else
        {
            TweenVolumeUp();
        }
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
    private void TweenVolumeDown()
	{
		// ...
		// Le decimos a Godot que busque la propiedad en el script
		CreateTween().TweenProperty(this, new NodePath("."), -20f, 0.5f);
	}

	private void TweenVolumeUp()
	{
		// ...
		// Le decimos a Godot que busque la propiedad en el script
		CreateTween().TweenProperty(this, new NodePath("."), 0f, 0.5f);
	}

    public float MusicVolumeDb
    {
        get => AudioServer.GetBusVolumeDb(musicBusIndex);
        set => AudioServer.SetBusVolumeDb(musicBusIndex, value);
    }
}
