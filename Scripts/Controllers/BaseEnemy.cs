using Godot;
using System;

public partial class BaseEnemy : CharacterBody2D
{
	public const float Speed = 100.0f;
	
	public float maxDistanceToPlayer = 100.0f;
	
	[Export]
	public PlayerController player;

	public override void _Ready()
	{
		player = GetTree().GetFirstNodeInGroup("Player") as PlayerController;
	}

	public override void _PhysicsProcess(double delta)
	{

		if(maxDistanceToPlayer < Position.DistanceTo(player.Position))
		{
			Velocity = Position.DirectionTo(player.Position) * -Speed;
		}
		
		MoveAndSlide();
	}
}
