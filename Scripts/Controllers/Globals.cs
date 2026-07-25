using Godot;
using System;
using System.Collections.Generic;
using static Godot.GD;

public partial class Globals : Node2D
{
	[Export]
	CharacterBody2D player;
	float score = 0.0f;

	[Export]PackedScene corkScene = GD.Load<PackedScene>("res://Scenes/Cork.tscn");

	float enemySpawnRate = 4.0f;

	float corkSpawnRate = 15.0f;


	[Export]  Godot.Collections.Array<PackedScene> enemies = new Godot.Collections.Array<PackedScene>();

	// Called when the node enters the scene tree for the first time.
	public override void _EnterTree()
	{
	
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		enemySpawnRate -= (float)delta;
		if(enemySpawnRate <= 0.0f)
		{
			Print("Spawning enemy");
			SpawnEnemy();
			enemySpawnRate = 4.0f;
		}

		corkSpawnRate -= (float)delta;
		if(corkSpawnRate <= 0.0f)
		{
			Print("Spawning cork");
			SpawnCork();
			corkSpawnRate = 15.0f;
		}
	}

	public void SpawnEnemy()
	{
		var enemyScene = enemies.PickRandom();
		var enemy = enemyScene.Instantiate<CharacterBody2D>();
		enemy.Position = new Vector2(player.Position.X + GD.RandRange(-500, 500), player.Position.Y + GD.RandRange(-500, 500));
		GetTree().CurrentScene.AddChild(enemy);
	}

		public void SpawnCork()
	{
		var cork = corkScene.Instantiate<StaticBody2D>();
		cork.Position = new Vector2(player.Position.X + GD.RandRange(-500, 500), player.Position.Y + GD.RandRange(-500, 500));
		GetTree().CurrentScene.AddChild(cork);
	}
}
