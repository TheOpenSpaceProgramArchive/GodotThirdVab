using Godot;
using System;
using System.Collections.Generic;

public class PartSelection : Panel
{
	/*
	const string path = "res://GameData";
	List<string> partlist = new List<string>();//list of partconfig paths
	List<string> fuellist = new List<string>();
	*/
	public override void _Ready()
	{
		VAB vab = (VAB)GetNode("/root/VAB");
		Node parts = (Node)GetNode("/root/VAB/Parts");

		foreach(Part part in parts.GetChildren())
		{
			part.Translation += new Vector3(0,1000,0);//get it out of sight
		}
	}
	/*
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
	*/

	//show only parts from the chosen category
	private void UpdatePartGrid(string category)
	{
		Node container = GetNode("/root/VAB/CanvasLayer/LeftPanel/PartSelection/PartGrid");
		foreach (Node child in container.GetChildren())
		{
			child.QueueFree();//remove old parts from list
		}
		Node parts = (Node)GetNode("/root/VAB/Parts");
		foreach (Part part in parts.GetChildren())
		{
			if (part.category == category)
			{
				TextureButton partnode = new TextureButton();
				partnode.SetNormalTexture(part.iconTexture);
				partnode.SetExpand(true);
				partnode.SetCustomMinimumSize(new Vector2(50, 50));
				Godot.Collections.Array partarray = new Godot.Collections.Array() ;
				partarray[0] = part;
				
				partnode.Connect("pressed", this, "OnPartPressed", partarray);
				container.AddChild(partnode);
			}
		}
	}

	private void OnPartPressed(object obj)
	{
		Part referencepart = (Part)obj;
		//Part part = (Part)referencepart.Duplicate();
		Part part = (Part)ClassDB.Instance((String)obj);

		#region copyVariables
		part.category = referencepart.category;
		part.type = referencepart.type;
		part.volume = referencepart.volume;
		part.seaIsp = referencepart.seaIsp;
		part.VacIsp = referencepart.VacIsp;
		part.minthrust = referencepart.minthrust;
		part.maxthrust = referencepart.maxthrust;
		//part.fuel0 = referencepart.fuel0;
		//part.fuelportion0 = referencepart.fuelportion0;
		part.fuel1 = referencepart.fuel1;
		part.fuelportion1 = referencepart.fuelportion1;
		#endregion


		/*
		foreach (Node child in part.GetChildren())
		{
			if (child.GetName() == "Scene Root")//imported mesh
			{
				part.CreateShapeOwner(part);
				part.ShapeOwnerAddShape(0, ((MeshInstance)child.GetChild(1)).Mesh.CreateTrimeshShape());
			}
		}
		*/

		#region connections

		foreach (Vector3 connection in referencepart.connections)
		{
			part.connectionList.Add(connection);
		}

		/*
		if (part.connection0!=new Vector3(0,0,0))
		{
			ConnectionSphere connectionsphere = new ConnectionSphere();
			CollisionShape collshape = new CollisionShape();
			SphereShape sphere = new SphereShape();
			collshape.Shape = sphere;
			connectionsphere.AddChild(collshape);
			connectionsphere.SetTranslation(part.connection0);
			part.AddChild(connectionsphere);
		}
		if (part.connection1 != new Vector3(0, 0, 0))
		{
			ConnectionSphere connectionsphere = new ConnectionSphere();
			CollisionShape collshape = new CollisionShape();
			SphereShape sphere = new SphereShape();
			collshape.Shape = sphere;
			connectionsphere.AddChild(collshape);
			connectionsphere.SetTranslation(part.connection1);
			part.AddChild(connectionsphere);
		}
		*/
		#endregion
	
		#region proceduraltank
		if (part.type == "proceduraltank")
		{
			part.mesh = new MeshInstance()
			{
				Mesh = new CylinderMesh()//TODO: change to actual mesh of the part
			};
			SpatialMaterial material = new SpatialMaterial();
			//material.AlbedoTexture = (Texture)ResourceLoader.Load("");
			//mesh.SetMaterialOverride(material);
			part.AddChild(part.mesh);

			int i =(int) part.CreateShapeOwner(part);

			PPFuelEditor window = (PPFuelEditor)GetNode("/root/VAB/CanvasLayer/PPFuelEditor");
			window.PartBeingEdited = part;
			window.setup();
			window.PopupCentered();
		}
		#endregion

		Node Craft = GetNode("/root/VAB/Craft");
		Node Selected = GetNode("/root/VAB/Selected");
		if (Craft.GetChildCount() == 0)
		{
			Craft.AddChild(part);
			part.SetOwner(Craft);
			part.Translation = new Vector3(0, 0, 0);
		}
		else
		{
			Selected.AddChild(part);
			part.Translation = new Vector3(0, 0, 0);
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
