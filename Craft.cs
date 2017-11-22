using Godot;
using System;

public class Craft : Spatial
{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here
        
    }

    //    public override void _Process(float delta)
    //    {
    //        // Called every frame. Delta is time since last frame.
    //        // Update game logic here.
    //        
    //    }

    public void Save()
    {
        LineEdit shipnamenode = (LineEdit)GetNode("/root/Root/CanvasLayer/TopPanel/CraftName");
        string shipname = shipnamenode.Text;
        SetOwnership(this.GetChild(0));

        PackedScene craftScene = new PackedScene();
        craftScene.Pack(this);
        ResourceSaver.Save("res://ships/" + shipname + ".tscn", craftScene);
    }
    public void SetOwnership(Node node)
    {
        node.SetOwner(this);
        foreach(Node child in node.GetChildren())
        {
            SetOwnership(child);
        }
    }
}
