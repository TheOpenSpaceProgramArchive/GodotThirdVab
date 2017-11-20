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

        if (input.IsAction("vabPitchUp"))
        {
            pitch += movespeed;
        }
        if (input.IsAction("vabPitchDown"))
        {
            pitch -= movespeed;
        }

        Move();
    }

    public void Move()
    {
        if(pitch>Math.PI/2)
        {
            pitch = (float)Math.PI / 2;
        }
        if(pitch<-Math.PI/2)
        {
            pitch = (float)-Math.PI / 2;
        }

        //Yaw
        Vector3 pos = new Vector3(0, height, 0);
        pos.x = (float)Math.Cos(yaw);
        pos.z = (float)Math.Sin(yaw);

        //Pitch
        pos.y += (float)Math.Sin(pitch);
        pos.x *= (float)Math.Cos(pitch);
        pos.z *= (float)Math.Cos(pitch);

        pos.x *= distance;
        pos.y *= distance;
        pos.z *= distance;


        this.SetTranslation(pos);

        Vector3 dir = new Vector3(0, 0, 0);
        dir.y = -yaw;
        dir.y += (float)(Math.PI/2);
        dir.x -= pitch;
        this.SetRotation(dir);
    }
}
