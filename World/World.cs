using Godot;
using System;

public partial class World : Node2D
{
    [Export]
    public float FadeSpeed = 3f;

    private ColorRect fadeRect;
    private ColorRect fadeRect2;
    private bool fadingIn = true;
    private bool fadingOut = false;
    private string nextScenePath; // Nueva variable para guardar la ruta de la siguiente escena

    public override void _Ready()
    {
        fadeRect = GetNode<ColorRect>("ColorRect");
        fadeRect2 = GetNode<ColorRect>("ColorRect2");
        fadeRect2.Visible = false; // Asegúrate de que el segundo ColorRect esté oculto al inicio
        fadeRect.Visible = true;
        var d = fadeRect2.Modulate;
        d.A = 0f;
        fadeRect2.Modulate = d;
        var c = fadeRect.Modulate;
        c.A = 1f;
        fadeRect.Modulate = c;
    }

    public void FadeOut(string scenePath)
    {
        fadingOut = true;
        fadingIn = false;
        fadeRect2.Visible = true;
        nextScenePath = scenePath; // Guarda la ruta de la escena
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
                fadeRect.Visible = false;
            }
            fadeRect.Modulate = color;
        }
        else if (fadingOut)
        {
            var color = fadeRect2.Modulate;
            color.A += FadeSpeed * (float)delta;
            if (color.A >= 1f)
            {
                color.A = 1f;
                fadingOut = false;

                // ¡Aquí es donde cambias la escena!
                GetTree().ChangeSceneToFile(nextScenePath);
            }
            fadeRect2.Modulate = color;
        }
        
    }
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_fullscreen"))
        {
            ToggleFullscreen();
        }
    }
    private void ToggleFullscreen()
    {
        // Obtiene el objeto Window raíz
        var window = GetTree().Root.GetWindow();

        // Alterna entre el modo de pantalla completa y de ventana
        if (window.Mode == Window.ModeEnum.Fullscreen)
        {
            window.Mode = Window.ModeEnum.Windowed;
        }
        else
        {
            window.Mode = Window.ModeEnum.Fullscreen;
        }
    }
}