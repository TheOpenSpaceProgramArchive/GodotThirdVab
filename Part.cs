using Godot;
using System;
using System.Collections.Generic;

public class Part : RigidBody
{
    public float mass = 1;
    public string type;
    public MeshInstance mesh;
    List<Vector3> connections =new List<Vector3>();

    //tank
    public float volume;

    //engine
    public float seaIsp;
    public float VacIsp;
    public float minthrust;
    public float maxthrust;
    public float currentthrust;
    public List<string> fuels = new List<string>();
    public List<float> fuelportion = new List<float>();

    //create part in vab
    public void CreatePart(string cfgPath)
    {
        Node Craft = GetNode("/root/VAB/Craft");
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
        SetMode(1);//StaticBody

        mesh = new MeshInstance()
        {
            Mesh = new CylinderMesh()//TODO: change to actual mesh of the part
        };
        mesh.SetOwner(Craft);
        SpatialMaterial material = new SpatialMaterial();
        material.AlbedoTexture = (Texture)ResourceLoader.Load((string)cfg.GetValue("part", "texturelocation"));
        mesh.SetMaterialOverride(material);

        AddChild(mesh);

        CreateShapeOwner(this);
        this.ShapeOwnerAddShape(0, mesh.Mesh.CreateTrimeshShape());


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
            connectionsphere.SetOwner(Craft);
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
}
