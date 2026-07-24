using Godot;
using System;

public partial class MainMenu : Node
{
    [Export] AnimationPlayer animationPlayer;
    [Export] PackedScene mainGame;
    [Export] Button playButton;

    public override void _EnterTree()
    {
        base._EnterTree();
        playButton.Pressed += changeToMainScene;
    }

    private void changeToMainScene()
    {   
		var scene = ResourceLoader.Load<PackedScene>(mainGame.ResourcePath);
		GetTree().ChangeSceneToPacked(scene);
    }
}
