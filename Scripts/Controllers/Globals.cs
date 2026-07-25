using Godot;
using System;
using static Godot.GD;

public partial class Globals : Node2D
{
	[Export]
	CharacterBody2D player;
	float score = 0.0f;

	PackedScene enemyScene = GD.Load<PackedScene>("res://Scenes/Enemy.tscn");

	float spawnRate = 4.0f;

	// Called when the node enters the scene tree for the first time.
	public override void _EnterTree()
	{
	
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		spawnRate -= (float)delta;
		if(spawnRate <= 0.0f)
		{
			Print("Spawning enemy");
			SpawnEnemy();
			spawnRate = 4.0f;
		}
	}

	public void SpawnEnemy()
	{
		var enemy = enemyScene.Instantiate<CharacterBody2D>();
		enemy.Position = new Vector2(player.Position.X + GD.RandRange(-500, 500), player.Position.Y + GD.RandRange(-500, 500));
		GetTree().CurrentScene.AddChild(enemy);
	}
}
