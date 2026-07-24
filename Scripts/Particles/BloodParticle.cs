using Godot;
using System;
public partial class BloodParticle : GpuParticles2D
{
    public ParticleProcessMaterial pm;
    
    [Export] public bool flipX;
    
    private float RandomVariance;

    public GpuParticles2D burstEmitter;
    public bool stopping = false;
    public BloodParticle()
    {
        pm = ProcessMaterial as ParticleProcessMaterial;
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        pm.ResourceLocalToScene = true;
        burstEmitter = this.GetChild<GpuParticles2D>(0);
        // (burstEmitter.Material as ParticleProcessMaterial).ResourceLocalToScene = true;
    }
    public override void _Ready()
    {
        base._Ready();
        // burstEmitter.Material.ResourceLocalToScene = true;
        if (flipX)
        {
            Scale = new(Scale.X *-1, Scale.Y);
            // Vector3 newDirection = new(pm.Direction.X * -1,pm.Direction.Y,pm.Direction.Z);
            // pm.Direction = newDirection;
            // ParticleProcessMaterial bE = burstEmitter.ProcessMaterial as ParticleProcessMaterial;
            // bE.Direction =  new(bE.Direction.X * -1,bE.Direction.Y,bE.Direction.Z);
        }

        RandomNumberGenerator rng = new();
        RandomVariance = rng.RandfRange(-5,20);
        
        BurstSequence();
        VaryColour();
    }
    public void BurstSequence()
    {
        burstEmitter.Emitting = true;
        Tween tween2 = GetTree().CreateTween();
        tween2.SetParallel(true);
        tween2.TweenProperty(pm, "initial_velocity_min", 35f + RandomVariance, 0.25f).From(300)
                .SetTrans(Tween.TransitionType.Bounce)
                .SetEase(Tween.EaseType.InOut);
        tween2.TweenProperty(pm, "initial_velocity_max", 52f + RandomVariance, 0.25f).From(500)
        .SetTrans(Tween.TransitionType.Bounce)
        .SetEase(Tween.EaseType.InOut);
        tween2.Chain();
    }
    public void RampFlow()
    {
        Tween tween = GetTree().CreateTween();
        tween.SetParallel(true);
        tween.TweenProperty(pm, "initial_velocity_min", 35f, 3.25f)
                .SetTrans(Tween.TransitionType.Elastic)
                .SetEase(Tween.EaseType.InOut);
        tween.TweenProperty(pm, "initial_velocity_max", 52f, 3.25f)
        .SetTrans(Tween.TransitionType.Elastic)
        .SetEase(Tween.EaseType.InOut);
        tween.Chain();
    }
    private void VaryFlow()
    {
        Tween tween = GetTree().CreateTween();
        tween.SetParallel(true);
        tween.TweenProperty(pm, "initial_velocity_min", 45f, 1.25f).From(35f)
                .SetTrans(Tween.TransitionType.Elastic)
                .SetEase(Tween.EaseType.InOut);
        tween.TweenProperty(pm, "initial_velocity_max", 68f, 1.25f).From(52f)
        .SetTrans(Tween.TransitionType.Elastic)
        .SetEase(Tween.EaseType.InOut);
        tween.Chain();
        tween.TweenProperty(pm, "initial_velocity_min", 35f, 1.25f).From(45f)
                .SetTrans(Tween.TransitionType.Elastic)
                .SetEase(Tween.EaseType.InOut);
        tween.TweenProperty(pm, "initial_velocity_max", 52f, 1.25f).From(68f)
        .SetTrans(Tween.TransitionType.Elastic)
        .SetEase(Tween.EaseType.InOut);
        tween.Chain();
        tween.SetLoops();
    }
    private void VaryColour()
    {
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(pm, "color:r8", 234f, 3.25f)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.Out);
            tween.TweenProperty(pm, "color:r8", 244f, 3.25f)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.In);
            tween.SetLoops();
    }
    
    public void StopFlow()
    {
        if (stopping) { return; }
        stopping = true;
        this.Emitting = false;
        SignalAwaiter timer = ToSignal(GetTree().CreateTimer(1f), "timeout");
        timer.OnCompleted(()=> { QueueFree(); });
    }
}
