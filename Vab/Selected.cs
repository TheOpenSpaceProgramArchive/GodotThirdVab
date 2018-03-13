using Godot;
using System;
using System.Collections.Generic;

public class Selected : Spatial
{
    int partdistance = 10;//distance from screen to part being moved
    Vector2 mousepos = new Vector2(0, 0);
    bool connected = false;
    bool connectedByNode = false;
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
        MouseRay ray = (MouseRay)GetNode("/root/VAB/MouseRay");
        MouseRay surfaceray = (MouseRay)GetNode("/root/VAB/MouseRay2");


        #region surface attachment
        //Ray hit
        if (surfaceray.hit != null)
        {
            if (GetChildCount() != 0)
            {
                this.SetTranslation(surfaceray.GetCollisionPoint());
                connected = true;
                connectedByNode = false;
                connectedPart = surfaceray.hit;
            }
        }
        #endregion

        #region node attachment
        else//surface attachment did not collide
        {
            //part connection
            if (!connected)
            {
                this.SetTranslation(to);
                if (ray.hit != null)
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
                                            connectedByNode = true;
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
            #endregion
            else//connected == true
            {
                if (ray.hit == null)
                {
                    connected = false;
                }
            }
        }

        if (grabPart)
        {
            grabPart = false;
            if (ray.hit != null)
            {
                //TODO FIX THIS FOR IMPORTED MESHES
                ray.hit.GetParent().RemoveChild(ray.hit);
                this.AddChild(ray.hit);
                ray.hit.SetOwner(this);
                ray.hit.SetTranslation(new Vector3(0, 0, 0));
            }
        }
        if (editPart)
        {
            editPart = false;

            if (ray.hit != null)
            {
                if (ray.hit.type == "proceduraltank")
                {
                    PPFuelEditor window = (PPFuelEditor)GetNode("/root/VAB/CanvasLayer/PPFuelEditor");
                    window.PartBeingEdited = ray.hit;
                    window.setup();
                    window.PopupCentered();
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
                        Vector3 pos;
                        if (connectedByNode)
                        {
                            pos = connectedspherecra.GetTranslation();
                            pos -= connectedspheresel.GetTranslation();
                        }
                        else
                        {
                            pos = GetTranslation();
                            pos -= connectedPart.GetGlobalTransform().origin;
                        }

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
