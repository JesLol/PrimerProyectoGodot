using Godot;
using System;

public partial class Creditos : Node2D
{
    private Button salirButton;

	public override void _Ready()
	{
		// Obtiene la referencia al botón de salir
		salirButton = GetNode<Button>("SalirButton");

		// Le da el foco al botón al iniciar la escena
		salirButton.GrabFocus();
		GD.Print("Credits scene is ready, focus set to SalirButton.");
    }

    public override void _Input(InputEvent @event)
    {
        // Si hay una entrada de navegación o de acción,
        // nos aseguramos de que el botón tenga el foco
        if (@event.IsActionPressed("ui-up") || @event.IsActionPressed("ui-down") || @event.IsActionPressed("ui-accept"))
        {
            if (@event.IsActionPressed("ui-accept"))
            {
                // Emite la señal 'pressed' del botón.
                // Esto es lo mismo que si el usuario hiciera clic en él.
                salirButton.EmitSignal(Godot.Button.SignalName.Pressed);
            }
            // if (!salirButton.HasFocus())
            // {
            //     salirButton.GrabFocus();
            // }
        }
    }
}