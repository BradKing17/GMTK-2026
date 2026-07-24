using Godot;
using System;
public partial class HealthController : Node
{
    [Export(PropertyHint.Range, "0,1")] public float fillValue = 0;
    [Export] private bool testing = false;
    [Export] private Sprite2D bloodShader;
    private ShaderMaterial material;
    [Export] private RichTextLabel textLabel;
    [Export] private Area2D drainArea;
    [Export] private Node2D leakParent;
    [Export] private PackedScene packedParticle;
    private Godot.Collections.Array<BloodParticle> leaks;

    HealthController()
    {
        leaks = [];
    }

    public override void _Ready()
    {
        bloodShader ??= GetChild<Sprite2D>(0);
        material = bloodShader.Material as ShaderMaterial;
        textLabel ??= GetChild<RichTextLabel>(1);
        drainArea ??= GetChild<Area2D>(2);
        material.SetShaderParameter("fill_value", fillValue);
        SlerpToFillLevel(fillValue, 1, 10f);
    }

    public void SetFillLevel(float fV)
    {
        material.SetShaderParameter("fill_value", fV);
        textLabel.Text = $"{fV * 100} ml";
    }

    public void SlerpToFillLevel(float fromLevel, float destinationLevel, float t)
    {
        Tween tween = GetTree().CreateTween();
        tween.SetParallel(true);
        tween.TweenMethod(Callable.From<int>(SetLabelText), fromLevel * 100, destinationLevel * 100, t);
        tween.TweenProperty(material,"shader_parameter/fill_value", destinationLevel, t);
        tween.Chain();
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if(!testing) { return; }
        if (@event.IsActionReleased("attack", true))
        {
            SpringLeak();
        }
        else if (@event.IsActionReleased("dodge", true))
        {
            PlugLeak();
        }
    }

    private void SetLabelText(int value)
    {
        textLabel.Text = $"{value} ml";
    }

    public void SpringLeak()
    {
        int randomSide = new RandomNumberGenerator().RandiRange(0,1);        
        BloodParticle leak = packedParticle.Instantiate<BloodParticle>();
        leak.flipX = randomSide == 0;
        leakParent.AddChild(leak);
        leaks.Add(leak);
        leak.Position = PositionInRegion(drainArea.GetChildren()[randomSide] as CollisionShape2D);
    }

    private Vector2 PositionInRegion(CollisionShape2D spawnRegion)
    {
        RectangleShape2D shape = spawnRegion.Shape as RectangleShape2D;
        Vector2 origin = spawnRegion.Position - shape.Size/2; //unsure as to anchor/origin of shape but this should work from (0,0)
        Vector2 bounds = new(new RandomNumberGenerator().RandfRange(0,shape.Size.X), new RandomNumberGenerator().RandfRange(0,shape.Size.Y));
        return new(origin.X + bounds.X, origin.Y + bounds.Y);
    }
    public void PlugLeak()
    {
        if (leaks.Count <= 0 ) { return; }
        var r =  leaks.Count - 1 == 0 ? 0 : new RandomNumberGenerator().RandiRange(0, leaks.Count - 1);
        BloodParticle randomLeak = leaks[r];
        leaks.Remove(randomLeak);
        randomLeak.StopFlow();
    }
}