using Godot;
using System;
using System.Collections.Generic;

public class PartSelection : Panel
{
    const string path = "res://GameData";
    List<string> partlist = new List<string>();//list of partconfig paths

    public override void _Ready()
    {
        partlist = FindAllFiles(path);
    }

    public List<string> FindAllFiles(string path)//find all cfg files inside GameData/modname/Parts/
    {
        List<string> filelist = new List<string>();

        Directory dir = new Directory();
        dir.Open(path);
        dir.ListDirBegin(true, true);
        string dirname = dir.GetNext();
        while (dirname != "")
        {
            if(dir.CurrentIsDir())
            {
                Directory subdir = new Directory();
                subdir.Open(path + "/" + dirname);
                subdir.ListDirBegin();
                string subdirname = subdir.GetNext();
                while (subdirname != "")
                {
                    if(subdirname=="Parts")
                    {
                        Directory subsubdir = new Directory();
                        subsubdir.Open(path + "/" + dirname + "/" + subdirname);
                        subsubdir.ListDirBegin();
                        string filename = subsubdir.GetNext();
                        while(filename!="")
                        {
                            if (!subsubdir.CurrentIsDir())
                            {
                                filelist.Add(path + "/" + dirname + "/" + subdirname + "/" + filename);
                            }
                            filename = subsubdir.GetNext();
                        }
                    }
                    subdirname = subdir.GetNext();
                }
            }
            dirname = dir.GetNext();
        }
        return filelist;
    }

    //show only parts from the chosen category
    private void UpdatePartGrid(string category)
    {
        foreach(string str in partlist)
        {
            Console.WriteLine(str);
        }

        Node container = GetNode("/root/Root/CanvasLayer/LeftPanel/PartSelection/PartGrid");
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
        
        Node Craft = GetNode("/root/Root/Craft");
        Node Selected = GetNode("/root/Root/Selected");
        
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