using Godot;
using System;
using static Godot.GD;

public partial class PlayerController : CharacterBody2D
{
		[Export] private float moveSpeed = 5.0f;
		[Export] private float maxHealth = 5500.0f;
		[Export] private float health = 5500.0f;
		[Export] private float rateOfBloodLoss = 10.0f;
		[Export] private int holes = 0;

		[Export] private float MaxRadius = 500.0f;

		[Export] private AnimatedSprite2D playerSprite;

		private Vector2 target;
		private bool canMove = true;
		private bool isAttacking = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AddToGroup("Player");
		GameStart();
	}

	public void GameStart()
	{
		Print("Game Started");
		health = maxHealth;
		holes = 1;
		canMove = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		//Attacking and moving logic
		if(isAttacking)
		{
			Velocity = Position.DirectionTo(target) * moveSpeed * 2;
			if (Position.DistanceTo(target) > 20.0f)
			{
				bool collided =MoveAndSlide();
				if(collided)
				{
					Velocity = Vector2.Zero;
					isAttacking = false;
					playerSprite.Frame = 0;
				}
			}
			else
			{	

				isAttacking = false;
				playerSprite.Frame = 0;
			}
		}
		else
		{
			GetInput();
			MoveAndSlide();
		}
	
	
		
		
		//Health drain logic
		if(holes > 0)
		{
			health -= (float)delta * holes * rateOfBloodLoss;
		}

		if(health <= 0)
		{
			Print("You Died");
			GetTree().Quit();
		}

	}

 public override void _Input(InputEvent @event)
	{
		
		if (@event.IsActionPressed("attack"))
		{
			isAttacking = true;
			Vector2 clickPosition = GetGlobalMousePosition();
			 Vector2 offset = clickPosition - GlobalPosition;
		   // float distance = offset.Length();

			target = GlobalPosition + (offset.Normalized() * MaxRadius);

			playerSprite.Frame = 1;
		}
	}

	public void GetInput()
	{
		if(!isAttacking)
		{
			Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");
			Velocity = inputDirection * moveSpeed;
		}
	}

	public void OnOverlap(Node2D body)
	{
		if(body is BaseEnemy)
		{
			Print(health);
			health += 1000.0f;
			if(health > maxHealth)
			{
				health = maxHealth;
			}
			Print(health);
			body.QueueFree();
		}

		if(body is Projectile)
		{
			Print("Hit by projectile");
			holes++;
			body.QueueFree();
			Print(holes);
		}
	}

}
