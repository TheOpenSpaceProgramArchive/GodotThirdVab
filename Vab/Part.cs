using Godot;
using System;
using System.Collections.Generic;

public class Part : RigidBody
{
    #region general
    [Export]
    public float mass = 1;

    [Export]
    public string category = "science";
    [Export]
    public string type;
    [Export]
    public string icontexturelocation = "Parts/125_coolie/125_coolie.png";
    [Export]
    public Texture iconTexture;
    [Export]
    public Vector3[] connections;
    [Export]
    public Vector3 connection0;
    [Export]
    public Vector3 connection1;

    List<Vector3> connectionlist =new List<Vector3>();
    #endregion


    //tank
    [Export]
    public float volume;

    //proceduraltank
    public MeshInstance mesh;

    #region engine
    [Export]
    public float seaIsp;
    [Export]
    public float VacIsp;
    [Export]
    public float minthrust;
    [Export]
    public float maxthrust;
    public float currentthrust;

    [Export]
    public string fuel0;
    [Export]
    public float fuelportion0;

    [Export]
    public string fuel1;
    [Export]
    public float fuelportion1;

    public List<string> fuels = new List<string>();
    public List<float> fuelportion = new List<float>();
    #endregion

    public override void _Ready()
    {
    }

    //create part in vab
    public void CreatePart(string cfgPath)
    {
        ConfigFile cfg = new ConfigFile();
        cfg.Load(cfgPath);

        SetGravityScale(0);

        mass = (int)cfg.GetValue("part", "mass");
        SetMass(mass);

        type = (string)cfg.GetValue("part", "type");

        for(int i=0;i< (int)cfg.GetValue("part", "connections"); i++)
        {
            Console.WriteLine("Connection: " + i);
            float x = (int)cfg.GetValue("part", "connectionX" + i);
            float y = (int)cfg.GetValue("part", "connectionY" + i);
            float z = (int)cfg.GetValue("part", "connectionZ" + i);
            connections.Add(new Vector3(x/100, y/100, z/100));
        }
        SetMode(ModeEnum.Static);//StaticBody

        
        mesh = new MeshInstance()
        {
            Mesh = new CylinderMesh()//TODO: change to actual mesh of the part
        };
        SpatialMaterial material = new SpatialMaterial();
        material.AlbedoTexture = (Texture)ResourceLoader.Load((string)cfg.GetValue("part", "texturelocation"));
        mesh.SetMaterialOverride(material);

        AddChild(mesh);

        UpdateCollisionShape();

        //create connection spheres
        foreach (Vector3 pos in connections)
        {
            ConnectionSphere connectionsphere = new ConnectionSphere();
            CollisionShape collshape = new CollisionShape();
            SphereShape sphere = new SphereShape();
            collshape.Shape = sphere;
            connectionsphere.AddChild(collshape);
            connectionsphere.SetTranslation(pos);
            AddChild(connectionsphere);
        }

        //proceduraltank
        if (type == "proceduraltank")
        {

        }
        Console.WriteLine(type);
        //engines
        if (type== "engine")
        {
            seaIsp= (int)cfg.GetValue("part", "seaisp");
            VacIsp = (int)cfg.GetValue("part", "vacisp");
            minthrust = (int)cfg.GetValue("part", "minthrust");
            maxthrust = (int)cfg.GetValue("part", "maxthrust");
            for(int i = 0;i < (int)cfg.GetValue("part", "fuels"); i++)
            {
                fuels.Add((string)cfg.GetValue("part", "fuel" + i));
                fuelportion.Add(float.Parse((string)cfg.GetValue("part", "fuelportion" + i)));
            }
        }
    }
    public void UpdateCollisionShape()
    {
        RemoveShapeOwner(0);
        CreateShapeOwner(this);
        this.ShapeOwnerAddShape(0, mesh.Mesh.CreateTrimeshShape());
    }
}
