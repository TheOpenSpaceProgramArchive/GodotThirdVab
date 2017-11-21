using Godot;
using System;
using System.Collections.Generic;

public class Part : RigidBody
{
    int mass = 1;
    List<Vector3> connections =new List<Vector3>();

    //create part in vab
    public void CreatePart(string cfgPath)
    {
        ConfigFile cfg = new ConfigFile();
        cfg.Load(cfgPath);

        SetGravityScale(0);

        mass = (int)cfg.GetValue("part", "mass");
        SetMass(mass);

        for(int i=0;i< (int)cfg.GetValue("part", "connections"); i++)
        {
            Console.WriteLine("Connection: " + i);
            float x = (int)cfg.GetValue("part", "connectionX" + i);
            float y = (int)cfg.GetValue("part", "connectionY" + i);
            float z = (int)cfg.GetValue("part", "connectionZ" + i);
            connections.Add(new Vector3(x/100, y/100, z/100));
        }
        SetMode(1);//StaticBody

        MeshInstance mesh = new MeshInstance()
        {
            Mesh = new CylinderMesh()//TODO: change to actual mesh of the part
        };
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
            connectionsphere.SetOwner(this);
        }
    }
}
