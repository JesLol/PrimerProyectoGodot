using Godot;
using System.Collections.Generic;

public partial class PauseCanvasLayer2 : CanvasLayer
{
    private Dictionary<string, Control> panels = new();
    
    // Cambiamos el tipo de esta variable para que sea una lista de botones
    private List<Button> activeButtonsList = new(); 
    
    private int focusedIndex = 0;
    private bool isPaused = false;
    private bool focusChangedThisFrame = false;
	// Cambia la lista de "Button" a "Control"
	private List<Control> activeControlsList = new();

    public override void _Ready()
	{
		Visible = false;

		panels["MainPause"] = GetNode<Control>("MainPause");
		panels["Settings"] = GetNode<Control>("Settings");

		foreach (var panel in panels.Values)
		{
			panel.Visible = false;
			panel.MouseFilter = Control.MouseFilterEnum.Ignore;
		}

		ShowPanel("MainPause");
	}

    public override void _Process(double delta)
    {
        // Puedes dejar esto para depurar si es necesario
    }

   // Dentro de PauseCanvasLayer2.cs
public override void _Input(InputEvent @event)
{
    // Lógica para pausar/despausar el juego
    if (@event.IsActionPressed("pause"))
    {
        TogglePause();
        GetViewport().SetInputAsHandled(); 
        return;
    }

    if (!isPaused || activeControlsList.Count == 0) 
        return;
    
    var currentFocusedControl = activeControlsList[focusedIndex];

    // Lógica para navegar entre controles con las flechas arriba/abajo
    if (@event.IsActionPressed("ui-down"))
    {
        MoveFocus(1);
        GetViewport().SetInputAsHandled(); 
    }
    else if (@event.IsActionPressed("ui-up"))
    {
        MoveFocus(-1);
        GetViewport().SetInputAsHandled(); 
    }

    // Lógica para aceptar (presionar) un botón con la tecla Enter o el gamepad
    if (@event.IsActionPressed("ui-accept"))
    {
        if (currentFocusedControl is Button button)
        {
            button.EmitSignal(Button.SignalName.Pressed);
            GetViewport().SetInputAsHandled(); 
        }
    }
}

    private void MoveFocus(int direction)
	{
		if (activeControlsList.Count == 0)
			return;

		focusedIndex = (focusedIndex + direction);

		if (focusedIndex >= activeControlsList.Count)
		{
			focusedIndex = 0;
		}
		else if (focusedIndex < 0)
		{
			focusedIndex = activeControlsList.Count - 1;
		}
		
		// Ahora le damos el foco a un Control genérico
		activeControlsList[focusedIndex]?.GrabFocus();
	}

	private Button GetFocusedButton()
	{
		// Este método ya no es necesario si solo usas el Accept para los botones.
		// Si quieres que el botón se presione con Enter, puedes verificarlo aquí.
		if (activeControlsList.Count > 0 && focusedIndex >= 0 && focusedIndex < activeControlsList.Count)
		{
			// Si el control actual es un botón, lo devolvemos
			if (activeControlsList[focusedIndex] is Button button)
			{
				return button;
			}
		}
		return null;
	}

    public void ShowPanel(string panelName)
	{
		foreach (var kv in panels)
		{
			bool isActive = kv.Key == panelName;
			kv.Value.Visible = isActive;
			kv.Value.MouseFilter = isActive ? Control.MouseFilterEnum.Stop : Control.MouseFilterEnum.Ignore;

			if (isActive)
			{
				activeControlsList.Clear();
				var activeControlsContainer = kv.Value.GetNodeOrNull<VBoxContainer>("VBoxContainer");

				if (activeControlsContainer != null)
				{
					foreach (var child in activeControlsContainer.GetChildren())
					{
						// ¡Aquí está el cambio clave!
						// Solo añade los nodos que son de tipo Button o HSlider
						if (child is Button button)
						{
							activeControlsList.Add(button);
						}
						else if (child is HSlider hslider)
						{
							activeControlsList.Add(hslider);
						}
					}
				}

				focusedIndex = 0;
				GetViewport().GuiReleaseFocus();

				if (activeControlsList.Count > 0)
				{
					activeControlsList[0].GrabFocus();
				}
			}
		}
	}

    private void TogglePause()
    {
        isPaused = !isPaused;
        Visible = isPaused;
        GetTree().Paused = isPaused;

        if (isPaused)
            ShowPanel("MainPause");
    }

    // Métodos para botones
    public void ContinuePressedBtn1() => TogglePause();
    public void ExitPressedButton1() => GetTree().Quit();
    public void OpenConfigButton() => ShowPanel("Settings");
    public void VolverMenuBtn() => ShowPanel("MainPause");
}