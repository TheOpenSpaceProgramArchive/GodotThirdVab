using Godot;
using System;

public class CameraVAB : Camera
{
    bool isZooming = false;

    float distance = 10;
    float height = 0;
    float yaw = 0;
    float pitch = 0;

    float zoomspeed = 0.1f;
    float movespeed = 0.1f;

    public override void _Ready()
    {
        Move();
    }


    public void _input(InputEvent input)
    {
        if(input.IsActionPressed("vabZoom"))
        {
            isZooming = true;
        }
        if (input.IsActionReleased("vabZoom"))
        {
            isZooming = false;
        }
        if (input.IsAction("ScrollUp"))
        {
            if(isZooming)
            {
                distance -= zoomspeed;
            }
            else
            {
                height += zoomspeed;
            }
        }
        if (input.IsAction("ScrollDown"))
        {
            if (isZooming)
            {
                distance += zoomspeed;
            }
            else
            {
                height -= zoomspeed;
            }
        }
        if (input.IsAction("vabLeft"))
        {
            yaw += movespeed;
        }
        if (input.IsAction("vabRight"))
        {
            yaw -= movespeed;
        }
        Move();
    }

    public void Move()
    {
        Vector3 pos = new Vector3(0, height, 0);
        pos.x += (float)(distance * Math.Cos(yaw));
        pos.z += (float)(distance * Math.Sin(yaw));
        this.SetTranslation(pos);

        Vector3 dir = new Vector3(0, 0, 0);
        dir.y = -yaw;
        dir.y += (float)(Math.PI/2);
        this.SetRotation(dir);
    }
}
