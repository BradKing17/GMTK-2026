using Godot;
using System;
using static Godot.GD;

public partial class PlayerController : CharacterBody2D
{
		[Export] private float moveSpeed = 5.0f;
		[Export] private float health = 5500.0f;
		[Export] private float rateOfBloodLoss = 10.0f;
		[Export] private int holes = 0;
		private bool canMove = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GameStart();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		GetInput();
		MoveAndSlide();

		if(holes > 0)
		{
			health -= (float)delta * holes * rateOfBloodLoss;
		}

		if(health <= 0)
		{
			Print("You Died");
			GetTree().Quit();
		}

		Print(health);
	}
	public void GetInput()
    {
        Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        Velocity = inputDirection * moveSpeed;
    }

	public void GameStart()
	{
		Print("Game Started");
		holes = 1;
		canMove = true;
	}
}
