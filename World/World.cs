using Godot;
using System;

public partial class World : Node2D
{
    [Export]
    public float FadeSpeed = 2f;

    private ColorRect fadeRect;
    private bool fadingIn = true;

    public override void _Ready()
    {
        // Busca el ColorRect
        fadeRect = GetNode<ColorRect>("ColorRect");
        fadeRect.Visible = true;

        // Asegurarse que empieza negro
        var c = fadeRect.Modulate;
        c.A = 1f;
        fadeRect.Modulate = c;
    }

    public override void _Process(double delta)
    {
        if (fadingIn)
        {
            var color = fadeRect.Modulate;
            color.A -= FadeSpeed * (float)delta;
            if (color.A <= 0f)
            {
                color.A = 0f;
                fadingIn = false;
                fadeRect.Visible = false; // Ocultar para no bloquear clicks
            }
            fadeRect.Modulate = color;
        }
    }
}
