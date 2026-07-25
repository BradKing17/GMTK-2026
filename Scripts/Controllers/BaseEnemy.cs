using Godot;
using System;
using static Godot.GD;
using static Godot.GD;

public partial class BaseEnemy : CharacterBody2D
{
	public const float Speed = 100.0f;
	
	public float maxDistanceToPlayer = 100.0f;
	
	[Export] public float fireRate = 1.0f;
	private float timeSinceLastShot = 0.0f;

	[Export] public PlayerController player;

	public override void _Ready()
	{
		player = GetTree().GetFirstNodeInGroup("Player") as PlayerController;

		timeSinceLastShot = fireRate;
		timeSinceLastShot = fireRate;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (maxDistanceToPlayer < Position.DistanceTo(player.Position))
		{
			Velocity = Position.DirectionTo(player.Position) * -Speed;
		}
		
		MoveAndSlide();

		timeSinceLastShot -= (float)delta;
		if(timeSinceLastShot <= 0.0f)
        {
            Print("Firing projectile");
			// Fire projectile
			var projectile = GD.Load<PackedScene>("res://Scenes/Projectile.tscn").Instantiate<Projectile>();
			projectile.GlobalPosition = GlobalPosition;
			GetTree().CurrentScene.AddChild(projectile);
			timeSinceLastShot = fireRate; // Reset fire rate
		}
	}
}
