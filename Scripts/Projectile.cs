using Godot;
using System;

public partial class Projectile : StaticBody2D
{
	private PlayerController player;
	private Vector2 target;
	private const float Speed = 1000.0f;
	public override void _Ready()
	{
		player = GetTree().GetFirstNodeInGroup("Player") as PlayerController;
		target = (GlobalPosition - player.GlobalPosition).Normalized();
	}
	public override void _PhysicsProcess(double delta)
	{
		GlobalPosition -= target * Speed * (float)delta;
	}
}
