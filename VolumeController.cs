using Godot;
using System;

public partial class VolumeController : HSlider
{
    private int musicBusIndex = AudioServer.GetBusIndex("Music");

    // Este método se activa cuando el nodo tiene el foco
    public override void _UnhandledInput(InputEvent @event)
    {
        // Solo procesamos los eventos de teclado si el slider tiene el foco
        if (HasFocus())
        {
            float step = 0.05f;

            if (@event.IsActionPressed("ui_left"))
            {
                Value -= step;
                // Consumir el evento para que no se propague a otros nodos
                GetViewport().SetInputAsHandled();
            }
            else if (@event.IsActionPressed("ui_right"))
            {
                Value += step;
                GetViewport().SetInputAsHandled();
            }
        }
    }

    public override void _Ready()
    {
        MinValue = 0;
        MaxValue = 1;

        ValueChanged += OnValueChanged;

        Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(musicBusIndex));
    }

    private void OnValueChanged(double value)
    {
        AudioServer.SetBusVolumeDb(musicBusIndex, Mathf.LinearToDb((float)value));
    }
}