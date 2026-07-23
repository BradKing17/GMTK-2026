using Godot;
using System;

public partial class HealthController : Node
{
    [Export(PropertyHint.Range, "0,1")] public float fillValue = 0;
    private Material material;
    private RichTextLabel textLabel;
    private float time;
    public override void _Ready()
    {
        material = GetChild<Sprite2D>(0).Material;
        (material as ShaderMaterial).SetShaderParameter("fill_value", fillValue);
        SlerpToFillLevel(fillValue, 1, 10f);
    }

    public void SetFillLevel(float fV)
    {
        (material as ShaderMaterial).SetShaderParameter("fill_value", fV);
        GetChild<RichTextLabel>(1).Text = $"{fV * 100} ml";
    }

    public void SlerpToFillLevel(float fromLevel, float destinationLevel, float t)
    {
        Tween tween = GetTree().CreateTween();
        tween.SetParallel(true);
        tween.TweenMethod(Callable.From<int>(SetLabelText), fromLevel * 100, destinationLevel * 100, t);
        tween.TweenProperty(material,"shader_parameter/fill_value", destinationLevel, t);
        tween.Chain();
    }
    private void SetLabelText(int value)
    {
        GetChild<RichTextLabel>(1).Text = $"{value} ml";
    }
}