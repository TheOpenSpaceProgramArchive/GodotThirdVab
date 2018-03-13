using Godot;
using System;

public class CameraNode : Camera
{
    Vector3 pos;
    Vector3 rotation;

    float yaw = 0;
    float pitch = 0;

    float rotationspeed = 0.1f;
    float movespeed = 0.1f;

    public override void _Ready()
    {
        pos = this.Translation;
        rotation = this.Rotation;
    }

    public void _input(InputEvent input)
    {
        Vector2 screencenter = GetViewport().GetVisibleRect().Size/2;

        if (input.IsAction("MoveForward"))
        {
            pos += ProjectRayNormal(screencenter) * movespeed;
        }
        if (input.IsAction("MoveBackwards"))
        {
            pos -= ProjectRayNormal(screencenter) * movespeed;
        }
        if (input.IsAction("RotateLeft"))
        {
            rotation.y += rotationspeed;
        }
        if (input.IsAction("RotateRight"))
        {
            rotation.y -= rotationspeed;
        }

        if (input.IsAction("RotateUp"))
        {
            rotation.x += rotationspeed;
        }
        if (input.IsAction("RotateDown"))
        {
            rotation.x -= rotationspeed;
        }

        //if (pos.Length() > 10)
        //{
        //    Planet planet = (Planet)GetNode("/root/Node/Planet");
        //    planet.Translation -= pos;
        //    pos = new Vector3(0, 0, 0);
        //}

        SetTranslation(pos);
        SetRotation(rotation);

    }
}
