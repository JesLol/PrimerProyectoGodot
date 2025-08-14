using Godot;
using System;

public partial class Door : Area2D
{
    private string nextScene = "res://Scenes/creditos.tscn";

    private void OnBodyEntered(Node2D body)
    {
        if (body is FumikoPlayer player)
    {
        // Encuentra el nodo del mundo
        var world = GetTree().Root.GetNode<World>("World");
        if (world != null)
        {
            // Llama a FadeOut y pasa la ruta de la siguiente escena
            world.FadeOut(nextScene);
        }
    }
    }

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }
}