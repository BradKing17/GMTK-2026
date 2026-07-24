using Godot;
using System;
using static Godot.GD;

public partial class Projectile : Area2D
{
	private PlayerController player;

	private Vector2 target;
    private float dot;

	private const float Speed = 1000.0f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetTree().GetFirstNodeInGroup("Player") as PlayerController;
		target = (GlobalPosition - player.GlobalPosition).Normalized();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		GlobalPosition -= target * Speed * (float)delta;
	}
}
