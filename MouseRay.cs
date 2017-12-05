using Godot;
using System;

public class MouseRay : RayCast
{
    public object collider;
    Vector2 mousepos = new Vector2(0, 0);

    float rayLength = 20;

    public override void _Ready()
    {
        this.Enabled = true;
    }

    public override void _Process(float delta)
    {

        Camera camera = (Camera)GetNode("/root/Root/CameraVAB");
        Vector3 from = camera.ProjectRayOrigin(mousepos);
        Vector3 to = (camera.ProjectRayNormal(mousepos) * rayLength);

        Translation = from;
        CastTo = to;

        if (IsColliding())
        {
            collider = GetCollider();
            //Console.WriteLine("Hit");
        }
        else
        {
            collider = null;
            //Console.WriteLine("Miss");
        }
    }
    public void _input(InputEventMouse mouse)
    {
        if (mouse.IsClass("InputEventMouseMotion"))
        {
            mousepos = mouse.Position;
        }
    }
}
