using Godot;
using System;
using static Godot.GD;

public partial class BaseEnemy : CharacterBody2D
{
	public const float Speed = 100.0f;
	
	[Export] public float minDistanceToPlayer = 100.0f;
	[Export] public float maxDistanceToPlayer = 500.0f;
	
	[Export] public float fireRate = 1.0f;
	[Export] public float projectileSpeed = 300.0f;
	private float timeSinceLastShot = 0.0f;

	[Export] public PlayerController player;

	[Export] public float experienceValue = 10.0f;

	public override void _Ready()
	{
		player = GetTree().GetFirstNodeInGroup("Player") as PlayerController;

		timeSinceLastShot = fireRate;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Position.DistanceTo(player.Position) < minDistanceToPlayer)
		{
			Velocity = Position.DirectionTo(player.Position) * -Speed;
		}
		else if (Position.DistanceTo(player.Position) > maxDistanceToPlayer)
		{
			Velocity = Position.DirectionTo(player.Position) * Speed;
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
			projectile.Speed = 300.0f;
			timeSinceLastShot = fireRate; // Reset fire rate
		}
	}
}
