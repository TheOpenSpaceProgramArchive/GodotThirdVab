using Godot;
using System;
using System.Collections.Generic;

public class Selected : Spatial
{
    int partdistance = 10;//distance from screen to part being moved
    Vector2 mousepos = new Vector2(0, 0);
    bool connected = false;
    Part connectedPart;
    Area connectedspheresel;
    Area connectedspherecra;
    bool grabPart = false;

    public override void _Ready()
    {
        SetPhysicsProcess(true);
        GetTree().SetDebugCollisionsHint(true);
    }

    public void _physics_process(float delta)
    {
        Camera camera = (Camera)GetNode("/root/Root/CameraVAB");
        Vector3 from = camera.ProjectRayOrigin(mousepos);
        Vector3 to = from + (camera.ProjectRayNormal(mousepos) * partdistance);

        //Ray hit
        Part hit = raycast(from, to,true);
        if (hit != null)
        {
            //todo: surface attachment
        }

        //part connection
        if (!connected)
        {
            this.SetTranslation(to);
            if (this.GetChildCount() != 0)
            {
                Part selectedPart = (Part)this.GetChild(0);
                foreach (Node connection in selectedPart.GetChildren())
                {
                    if (connection.IsClass("Area"))
                    {
                        Area area = (Area)connection;
                        object[] overlaps = area.GetOverlappingAreas();
                        if (overlaps.Length != 0)
                        {
                            Node overlap = (Node)overlaps[0];
                            if (overlap.IsClass("Area") & !this.GetChild(0).IsAParentOf(overlap))//dont overlap with other connections of same part
                            {
                                Part overlapPart = (Part)overlap.GetParent();
                                selectedPart.SetRotation(overlapPart.GetRotation());

                                Vector3 pos = ((Area)overlap).GetGlobalTransform().origin;
                                pos -= ((Area)connection).GetTranslation();
                                
                                this.SetTranslation(pos);
                                connected = true;
                                connectedPart = overlapPart;
                                connectedspherecra = (Area)overlap;
                                connectedspheresel = (Area)connection;
                            }
                        }
                    }
                }
            }
        }

        else//connected == true
        {
            hit = raycast(from, to, false);
            if (hit == null)
            {
                connected = false;
                this.SetTranslation(to);
            }
        }

        if(grabPart)
        {
            grabPart = false;

            hit = raycast(from, to, false);
            if (hit != null)
            {
                hit.GetParent().RemoveChild(hit);
                this.AddChild(hit);
                hit.SetOwner(this);
                hit.SetTranslation(new Vector3(0, 0, 0));
            }
        }
    }

    public void _input(InputEventMouse mouse)
    {
        Node Craft = GetNode("/root/Root/Craft");
        if (mouse.IsClass("InputEventMouseMotion"))
        {
            mousepos = mouse.Position;
        }
        if (mouse.IsAction("leftClick"))
        {
            if (!mouse.IsPressed())
            {
                if (this.GetChildCount() != 0)
                {
                    Part selectedPart = (Part)this.GetChild(0);

                    //connect part
                    if (connected)
                    {
                        Vector3 pos = connectedspherecra.GetTranslation();
                        pos -= connectedspheresel.GetTranslation();

                        this.RemoveChild(selectedPart);
                        connectedPart.AddChild(selectedPart);
                        selectedPart.SetOwner(Craft);
                        selectedPart.SetTranslation(pos);

                        //Todo: joints
                    }
                    //Drop part without connecting
                    else // connected == false
                    {
                        Vector3 pos = this.GetGlobalTransform().origin;
                        this.RemoveChild(selectedPart);
                        this.GetParent().AddChild(selectedPart);//become child of Root node
                        selectedPart.SetOwner(this.GetParent());
                        selectedPart.SetTranslation(pos);
                    }
                }
                else// this.GetChildCount() == 0
                {
                    grabPart = true;
                }
            }
        }
    }

    public Part raycast(Vector3 from, Vector3 to,bool passThroughItself)
    {
        try
        {
            PhysicsDirectSpaceState spaceState = GetWorld().GetDirectSpaceState();
            if(spaceState==null)
            {
                Console.WriteLine("Why does this happen?????");
            }
            Dictionary<object, object> result;

            if (passThroughItself&this.GetChildCount()!=0)
            {
                object[] obj = new object[this.GetChildCount()];// pass through own rigidbody
                obj = this.GetChildren();
                result = spaceState.IntersectRay(from, to, obj);
            }
            else
            {
                result = spaceState.IntersectRay(from, to);
            }

            if (result.ContainsKey("collider"))
            {
                return (Part)result["collider"];
            }
            else
            {
                return null;
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
    }
}
