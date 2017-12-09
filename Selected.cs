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
    bool editPart = false;
    
    public override void _Ready()
    {
        SetPhysicsProcess(true);
        GetTree().SetDebugCollisionsHint(true);
    }

    public void _physics_process(float delta)
    {
        Camera camera = (Camera)GetNode("/root/VAB/CameraVAB");
        Vector3 from = camera.ProjectRayOrigin(mousepos);
        Vector3 to = from + (camera.ProjectRayNormal(mousepos) * partdistance);

        //Ray hit
        MouseRay ray = (MouseRay)GetNode("/root/VAB/MouseRay");
        object collided = ray.collider;
        Part hit = null;
        if (collided != null)
        {
            if (collided.ToString() == "Part")
            {
                hit = (Part)collided;
            }
        }

        //part connection
        if (!connected)
        {
            this.SetTranslation(to);
            if (hit != null)
            {
                if (this.GetChildCount() != 0)
                {
                    Part selectedPart = (Part)this.GetChild(0);
                    foreach (Node connection in selectedPart.GetChildren())
                    {
                        if (!connected)
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
            }
        }

        else//connected == true
        {
            if (hit == null)
            {
                connected = false;
            }
        }

        if (grabPart)
        {
            grabPart = false;

            if (hit != null)
            {
                hit.GetParent().RemoveChild(hit);
                this.AddChild(hit);
                hit.SetOwner(this);
                hit.SetTranslation(new Vector3(0, 0, 0));
            }
        }
        if (editPart)
        {
            editPart = false;

            if (hit != null)
            {
                if (hit.type== "proceduraltank")
                {
                    PPFuelEditor window = (PPFuelEditor)GetNode("/root/VAB/CanvasLayer/PPFuelEditor");
                    window.PartBeingEdited = hit;
                    window.setup();
                    window.PopupCentered();


                    WindowDialog windowdial = (WindowDialog)GetNode("/root/VAB/CanvasLayer/WindowDialog");
                    windowdial.PopupCentered();
                }


            }
        }
    }

    public void _input(InputEventMouse mouse)
    {
        Node Craft = GetNode("/root/VAB/Craft");
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
        else if (mouse.IsAction("RightClick"))
        {
            if (!mouse.IsPressed())
            {
                editPart = true;
            }
        }
    }
}
