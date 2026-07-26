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

	[Export]float enemySpawnRate = 4.0f;
	float timeSinceEnemySpawn;

	[Export]float corkSpawnRate = 15.0f;
	float timeSinceCorkSpawn;


	[Export]  Godot.Collections.Array<PackedScene> enemies = new Godot.Collections.Array<PackedScene>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		timeSinceEnemySpawn -= (float)delta;
		if(timeSinceEnemySpawn <= 0.0f)
		{
			Print("Spawning enemy");
			SpawnEnemy();
			timeSinceEnemySpawn = enemySpawnRate;
		}

		timeSinceCorkSpawn -= (float)delta;
		if(timeSinceCorkSpawn <= 0.0f)
		{
			Print("Spawning cork");
			SpawnCork();
			timeSinceCorkSpawn = corkSpawnRate;
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
