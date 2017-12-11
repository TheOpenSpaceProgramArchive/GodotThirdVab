using Godot;
using System;

public class MouseRay : RayCast
{
    public Part hit;
    Vector2 mousepos = new Vector2(0, 0);

    float rayLength = 1000;

    public override void _Ready()
    {
        this.Enabled = true;
    }

    public override void _Process(float delta)
    {

        Camera camera = (Camera)GetNode("/root/VAB/CameraVAB");
        Vector3 from = camera.ProjectRayOrigin(mousepos);
        Vector3 to = (camera.ProjectRayNormal(mousepos) * rayLength);

        Translation = from;
        CastTo = to;

        //prevent ray from colliding with selected part
        if (GetName() == "MouseRay2")
        {
            ClearExceptions();
            AddExeptions(GetNode("/root/VAB/Selected"));
        }

        hit = null;
        if (IsColliding())
        {
            object collider = GetCollider();
            if (collider.ToString() == "Part")
            {
                hit = (Part)collider;
                //Console.WriteLine("Hit");
            }
        }
    }
    public void _input(InputEventMouse mouse)
    {
        if (mouse.IsClass("InputEventMouseMotion"))
        {
            mousepos = mouse.Position;
        }
    }
    public void AddExeptions(Node node)
    {
        AddException(node);
        foreach(Node n in node.GetChildren())
        {
            AddExeptions(n);
        }
    }
}
