using Godot;
using System;
using System.Collections.Generic;

public class PartSelection : Panel
{
    const string path = "res://GameData";
    List<string> partlist = new List<string>();//list of partconfig paths
    List<string> fuellist = new List<string>();
    
    public override void _Ready()
    {
        VAB vab = (VAB)GetNode("/root/VAB");
        FindAllFiles(path);
    }

    public void FindAllFiles(string path)//find all cfg files inside GameData/modname/
    {
        Directory dir = new Directory();
        dir.Open(path);
        dir.ListDirBegin(true, true);
        string dirname = dir.GetNext();
        while (dirname != "")
        {
            if (dir.CurrentIsDir())
            {
                //Parts
                Directory partdir = new Directory();
                partdir.Open(path + "/" + dirname + "/Parts");
                partdir.ListDirBegin();
                string filename = partdir.GetNext();
                while (filename != "")
                {
                    if (!partdir.CurrentIsDir())
                    {
                        partlist.Add(path + "/" + dirname + "/Parts/" + filename);
                    }
                    filename = partdir.GetNext();
                }
                //Fuels
                Directory fueldir = new Directory();
                fueldir.Open(path + "/" + dirname + "/fuels");
                fueldir.ListDirBegin();
                filename = fueldir.GetNext();
                while (filename != "")
                {
                    if (!fueldir.CurrentIsDir())
                    {
                        fuellist.Add(path + "/" + dirname + "/Fuels/" + filename);
                    }
                    filename = fueldir.GetNext();
                }
            }
            dirname = dir.GetNext();
        }

        //
        foreach(string fuelpath in fuellist)
        {
            ConfigFile fuelconfig = new ConfigFile();
            fuelconfig.Load(fuelpath);
            VAB vab = (VAB)GetNode("/root/VAB");
            Fuel fuel = new Fuel();
            fuel.name = (string)fuelconfig.GetValue("fuel", "name");
            fuel.density = float.Parse((string)fuelconfig.GetValue("fuel", "density"));
            //fuel.density = (int)fuelconfig.GetValue("fuel", "density");//why no work???
            vab.fuels.Add(fuel);
        }
    }

    //show only parts from the chosen category
    private void UpdatePartGrid(string category)
    {
        Node container = GetNode("/root/VAB/CanvasLayer/LeftPanel/PartSelection/PartGrid");
        foreach (Node child in container.GetChildren())
        {
            child.QueueFree();//remove old parts
        }
        foreach (string path in partlist)
        {
            ConfigFile part = new ConfigFile();
            part.Load(path);

            if ((string)part.GetValue("part", "category") == category)
            {
                TextureButton partnode = new TextureButton();
                partnode.SetName(path);
                partnode.SetNormalTexture((Texture)ResourceLoader.Load((string)part.GetValue("part", "iconlocation")));
                partnode.SetExpand(true);
                partnode.SetCustomMinimumSize(new Vector2(25, 25));

                object[] obj = new object[1];
                obj[0] = path;
                partnode.Connect("pressed", this, "OnPartPressed", obj);
                container.AddChild(partnode);
            }
        }
    }

    private void OnPartPressed(object obj)
    {
        Part part = new Part();
        part.CreatePart((string)obj);

        Node Craft = GetNode("/root/VAB/Craft");
        Node Selected = GetNode("/root/VAB/Selected");


        if (part.type == "proceduraltank")
        {
            PPFuelEditor window = (PPFuelEditor)GetNode("/root/VAB/CanvasLayer/PPFuelEditor");
            window.PartBeingEdited = part;
            window.setup();
            //window.PopupCentered();
        }

        if (Craft.GetChildren().Length == 0)
        {
            Craft.AddChild(part);
            part.SetOwner(Craft);
        }
        else
        {
            Selected.AddChild(part);
        }
    }

    private void _on_Command_pressed()
    {
        UpdatePartGrid("command");
    }


    private void _on_FuelTanks_pressed()
    {
        UpdatePartGrid("fueltank");
    }


    private void _on_Engine_pressed()
    {
        UpdatePartGrid("engine");
    }


    private void _on_Controll_pressed()
    {
        UpdatePartGrid("controll");
    }


    private void _on_Separator_pressed()
    {
        UpdatePartGrid("separator");
    }


    private void _on_Structure_pressed()
    {
        UpdatePartGrid("structure");
    }


    private void _on_Landing_pressed()
    {
        UpdatePartGrid("landing");
    }


    private void _on_Aerodynamics_pressed()
    {
        UpdatePartGrid("aerodynamics");
    }


    private void _on_Electrical_pressed()
    {
        UpdatePartGrid("electrical");
    }


    private void _on_Communications_pressed()
    {
        UpdatePartGrid("communication");
    }


    private void _on_Science_pressed()
    {
        UpdatePartGrid("science");
    }
}