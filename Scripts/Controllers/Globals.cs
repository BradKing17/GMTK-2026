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

	[Export] Godot.Collections.Array<CollisionShape2D> spawnAreas = new Godot.Collections.Array<CollisionShape2D>();


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
		enemy.Position = PositionInRegion();
		GetTree().CurrentScene.AddChild(enemy);
	}

		public void SpawnCork()
	{
		var cork = corkScene.Instantiate<StaticBody2D>();
		cork.Position = PositionInRegion();
		GetTree().CurrentScene.AddChild(cork);
	}

	    private Vector2 PositionInRegion()
    {
		var spawnRegion = spawnAreas.PickRandom();
        RectangleShape2D shape = spawnRegion.Shape as RectangleShape2D;
        Vector2 origin = spawnRegion.Position - shape.Size/2; //unsure as to anchor/origin of shape but this should work from (0,0)
        Vector2 bounds = new(new RandomNumberGenerator().RandfRange(0,shape.Size.X), new RandomNumberGenerator().RandfRange(0,shape.Size.Y));
        return new(origin.X + bounds.X, origin.Y + bounds.Y);
    }
}
