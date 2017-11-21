using Godot;
using System;

public class TopPanel : Panel
{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here
        
    }

    //    public override void _Process(float delta)
    //    {
    //        // Called every frame. Delta is time since last frame.
    //        // Update game logic here.
    //        
    //    }

    private void _on_Save_pressed()
    {
        Craft craft = (Craft)GetNode("/root/Root/Craft");
        craft.Save();
    }


    private void _on_Load_pressed()
    {
        PopupPanel loadpopup = (PopupPanel)GetNode("/root/Root/CanvasLayer/TopPanel/LoadPopup");

        string path = "res://Ships";
        Directory dir = new Directory();
        dir.Open(path);
        dir.ListDirBegin(true, true);
        string craftname = dir.GetNext();
        while (craftname != "")
        {
            Console.WriteLine(craftname);
            Button craftbutton = new Button();
            craftbutton.Text = craftname;
            loadpopup.AddChild(craftbutton);
            craftname = dir.GetNext();
        }
            //TODO:add crafts
            loadpopup.PopupCentered();
    }

    private void _on_CloseLoadPopup_pressed()
    {
        PopupPanel loadpopup = (PopupPanel)GetNode("/root/Root/CanvasLayer/TopPanel/LoadPopup");
        loadpopup.Hide();
    }


    private void _on_Fly_pressed()
    {
        Console.WriteLine("Not implemented yet");
    }


    private void _on_Exit_pressed()
    {
        Console.WriteLine("Not implemented yet");
    }
}


