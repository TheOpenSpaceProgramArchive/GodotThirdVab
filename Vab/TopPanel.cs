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
		Craft craft = (Craft)GetNode("/root/VAB/Craft");
		craft.Save();
	}


	private void _on_Load_pressed()
	{
		PopupPanel loadpopup = (PopupPanel)GetNode("/root/VAB/CanvasLayer/TopPanel/LoadPopup");

		string path = "res://Ships";
		Directory dir = new Directory();
		dir.Open(path);
		dir.ListDirBegin(true, true);
		string craftname = dir.GetNext();
		while (craftname != "")
		{
			Button craftbutton = new Button();
			craftbutton.Text = craftname;

			object []obj = new object[1];
			obj[0] = craftname;
			craftbutton.Connect("pressed", this, "OnLoadCraftPressed", obj);

			loadpopup.AddChild(craftbutton);
			craftname = dir.GetNext();
		}
			loadpopup.PopupCentered();
	}

	private void _on_CloseLoadPopup_pressed()
	{
		PopupPanel loadpopup = (PopupPanel)GetNode("/root/VAB/CanvasLayer/TopPanel/LoadPopup");
		loadpopup.Hide();
	}

	private void OnLoadCraftPressed(object obj)
	{
		string craftname = (string)obj;
		Node Craft = GetNode("/root/VAB/Craft");

		//clear craft
		foreach(Node child in Craft.GetChildren())
		{
			child.QueueFree();
		}

		//add ship to craft node
		PackedScene packedScene = (PackedScene)ResourceLoader.Load("res://Ships/" + craftname);
		Node craftInstance = packedScene.Instance();
		Craft.AddChild(craftInstance);
		Node oldcraft = Craft.GetChild(0);
		Node firstpart = oldcraft.GetChild(0);
		oldcraft.RemoveChild(firstpart);
		Craft.AddChild(firstpart);
		oldcraft.QueueFree();

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


