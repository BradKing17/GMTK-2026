using Godot;
using System;

public partial class Cubicle : Node2D
{
    public enum ItemType {
            normal, clean, papers, computerless, mice, secondChair
        };
    [Export] ItemType cubicleStyle = ItemType.normal;

}
