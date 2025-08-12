using Godot;
using System;

public partial class ScoreCanvasLayer : CanvasLayer
{
	public GameManager gameManager;
	private Label scoreLabel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gameManager = GetNode<GameManager>("/root/GameManager"); //Obtiene el nodo GameManager que esta en la escena principal
		scoreLabel = GetNode<Label>("Panel/Label");
		if (gameManager != null) {
			gameManager.ScoreChanged += ActualizarScore; //Conecta la señal ScoreChanged del GameManager al metodo ActualizarScore
			ActualizarScore(gameManager.Score); //Actualiza el score al iniciar
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
	public void ActualizarScore(int newScore)
	{
		scoreLabel.Text = $": {newScore}";
	}
}
