using Godot;
using System;

public partial class Coin : Area2D
{
	public GameManager gameManager;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered; //Esto conecta la señal al metodo OnBodyEntered. Equivalente a hacerlo desde el editor
		AnimatedSprite2D anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		anim.Play("default");
		gameManager = GetNode<GameManager>("/root/GameManager"); //Obtiene el nodo GameManager desde la raiz del arbol de escenas
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void OnBodyEntered(Node2D body)
	{
		if (body is FumikoPlayer player)
		{
			gameManager.AddCoin(1); 
			QueueFree();
		}
	}
}
