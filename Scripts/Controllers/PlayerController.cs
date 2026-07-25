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
	[Export] private float lungeCD = 1.0f;
	[Export] private float timeSinceLastLunge = 0.0f;
	[Export] private bool canAttack = true;
	[Export] private AnimatedSprite2D playerSprite;
	[Export] private HealthController healthController;
	private Vector2 target;
	private bool canMove = true;
	private bool isAttacking = false;
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
		healthController.SpringLeak();
		canMove = true;
	}

	public override void _PhysicsProcess(double delta)
	{
		//Attacking and moving logic
		if (isAttacking)
		{
			canAttack = false;
			timeSinceLastLunge = lungeCD;
			Velocity = Position.DirectionTo(target) * moveSpeed * 2;
			if (Position.DistanceTo(target) > 20.0f)
			{
				bool collided = MoveAndSlide();
				if (collided)
				{
					if(collided && GetSlideCollision(0).GetCollider() is Projectile)
					{
						Print("Collided with projectile");
					}
					else
					{
						Velocity = Vector2.Zero;
						isAttacking = false;
						playerSprite.Frame = 0;
					}
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

		//Reset lunge cooldown
		if(!canAttack)
		{
			timeSinceLastLunge -= (float)delta;
		}

		if(timeSinceLastLunge <= 0.0f)
		{
			Print("Lunge ready");
			canAttack = true;
			timeSinceLastLunge = lungeCD;
		}
	
		//Health drain logic
		if (holes > 0)
		{
			health -= (float)delta * holes * rateOfBloodLoss;
			healthController.SetFillLevel(health);
		}
		if (health <= 0)
		{
			Print("You Died");
			GetTree().Quit();
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("attack"))
		{
			if(canAttack)
			{
				isAttacking = true;
				Vector2 clickPosition = GetGlobalMousePosition();
				Vector2 offset = clickPosition - GlobalPosition;
				target = GlobalPosition + (offset.Normalized() * MaxRadius);
				playerSprite.Frame = 1;
			}
			else
			{
				Print("Lunge on cooldown");
			}
			
		}
	}

	public void GetInput()
	{
		if (!isAttacking)
		{
			Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");
			Velocity = inputDirection * moveSpeed;
		}
	}

	public void OnOverlap(Node2D body)
	{
		if (body is BaseEnemy)
		{
			if(isAttacking)
			{
				Print(health);
				health += 1000.0f;
				if (health > maxHealth)
				{
					health = maxHealth;
				}

				healthController.SetFillLevel(health);
				Print(health);
				healthController.SpringLeak();
				body.QueueFree();
			}
		}
		else if (body is Projectile)
		{
			holes++;
			healthController.SpringLeak();
			body.QueueFree();
			Print($"hit {holes}");
		}
	}
}
