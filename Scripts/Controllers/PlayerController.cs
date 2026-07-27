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

	// Lunge variables
	[Export] private float MaxLungeRadius = 500.0f;
	[Export] private float lungeCD = 1.0f;
	[Export] private float timeSinceLastLunge = 0.0f;
	[Export] private bool canAttack = true;
	private bool isAttacking = false;
	private Vector2 lungeTarget;

	// Dodge variables
	[Export] private float MaxDodgeRadius = 300.0f;
	[Export] private float dodgeCD = 1.0f;
	[Export] private float timeSinceLastDodge = 0.0f;
	[Export] private bool canDodge = true;

	private bool isDodging = false;
	private Vector2 dodgeTarget;

	[Export] private AnimatedSprite2D playerSprite;
	[Export] private HealthController healthController;

	[Export] private AudioStreamPlayer2D audioPlayer;
	[Export]  Godot.Collections.Array<AudioStream> hitSounds = new Godot.Collections.Array<AudioStream>();

	[Export]private AnimatedSprite2D attackIcon;

	[Export]private AnimatedSprite2D dodgeIcon;

	[Export]private RichTextLabel timeText;
	[Export]private RichTextLabel enemiesText;
	[Export]private RichTextLabel holesText;

	[Export]private Control canvas;

	[Export]private PackedScene mainMenu;

	[Export]private Button quitButton;

	private bool canMove = true;

	//Score stuff
	private float time;

	private int enemiesKilled = 0;

	private int holesPlugged = 0;

	public override void _Ready()
	{
		audioPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
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
		time += (float)delta;
		//Attacking and moving logic
		if (isAttacking)
		{
			canAttack = false;
			attackIcon.Frame = 1;
			timeSinceLastLunge = lungeCD;
			Velocity = Position.DirectionTo(lungeTarget) * moveSpeed * 3;
			if (Position.DistanceTo(lungeTarget) > 20.0f)
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
		else if (isDodging)
		{
			canDodge = false;
			dodgeIcon.Frame = 1;
			this.SetCollisionMaskValue(1, false);
			this.SetCollisionMaskValue(4, false);
			this.SetCollisionLayerValue(1, false);

			timeSinceLastDodge = dodgeCD;
			Velocity = Position.DirectionTo(dodgeTarget) * moveSpeed * 2;
			if (Position.DistanceTo(dodgeTarget) > 20.0f)
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
						isDodging = false;
						playerSprite.Frame = 0;
						this.SetCollisionMaskValue(1, true);
						this.SetCollisionMaskValue(4, true);

						this.SetCollisionLayerValue(1, true);
						
					}
				}
			}
			else
			{
			
				isDodging = false;
				playerSprite.Frame = 0;

				this.SetCollisionMaskValue(1, true);
				this.SetCollisionMaskValue(4, true);

				this.SetCollisionLayerValue(1, true);

				
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
			attackIcon.Frame = 0;
		}
	
		if(!canDodge)
		{
			timeSinceLastDodge -= (float)delta;
		}

		if(timeSinceLastDodge <= 0.0f)
		{
			Print("Dodge ready");
			canDodge = true;
			timeSinceLastDodge = dodgeCD;
			dodgeIcon.Frame = 0;
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
			canvas.Visible = true;
			playerSprite.Frame = 2;
			canMove = false;
			canAttack = false;
			canDodge = false;
			enemiesText.Text = "Enemies Killed: " + enemiesKilled;
		
			holesText.Text = "Holes Plugged: " + holesPlugged;
		
			timeText.Text = "Time Survived: " + time;

			quitButton.Pressed += changeToMainScene;
	


			
		}
	}

	private void changeToMainScene()
	{   
		var scene = ResourceLoader.Load<PackedScene>(mainMenu.ResourcePath);
		GetTree().ChangeSceneToPacked(scene);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("attack"))
		{
			if(canAttack && !isDodging)
			{
				isAttacking = true;
				Vector2 clickPosition = GetGlobalMousePosition();
				Vector2 offset = clickPosition - GlobalPosition;
				lungeTarget = GlobalPosition + (offset.Normalized() * MaxLungeRadius);
				playerSprite.Frame = 1;
			}
			else
			{
				Print("Lunge on cooldown");
			}
		}

		if (@event.IsActionPressed("dodge"))
		{
			if(canDodge && !isAttacking)
			{
				isDodging = true;
				Vector2 clickPosition = GetGlobalMousePosition();
				Vector2 offset = clickPosition - GlobalPosition;
				dodgeTarget = GlobalPosition + (offset.Normalized() * MaxDodgeRadius);
				playerSprite.Frame = 2;
			}
			else
			{
				Print("Dodge on cooldown");
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
				body.QueueFree();
				enemiesKilled++;
			}
		}
		else if (body is Projectile)
		{
			if(!isDodging)
			{
				holes++;
				healthController.SpringLeak();
				body.QueueFree();
				Print($"hit {holes}");

				AudioStream audioEffect = hitSounds.PickRandom();
				audioPlayer.Stream = audioEffect;
				audioPlayer.Play();
			}
		}
		else if (body is Cork)
		{
			if(holes > 0)
			{
				holes--;
				healthController.PlugLeak();
				holesPlugged++;
			}
			body.QueueFree();
		}
	}
}
