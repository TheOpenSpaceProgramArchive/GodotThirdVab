using Godot;
using System;

public class Planet : RigidBody
{

    SurfaceTool st;
    ArrayMesh am;
    public MeshInstance mesh = new MeshInstance();
    float radius = 1;

    public override void _Ready()
    {
        this.AddChild(mesh);
    }


    public override void _Process(float delta)
    {
        CameraNode camera = (CameraNode)GetNode("/root/Node/CameraNode");
        draw(camera.Translation.Normalized());
        //draw(new Vector3(0, 1, 0));
    }


    public void draw_triangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        st.AddColor(new Color(255, 0, 0));
        st.AddUv(new Vector2(255, 0));

        st.AddVertex(p3* radius);
        st.AddVertex(p2* radius);
        st.AddVertex(p1* radius);
    }

    public void draw_recursive(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 center, float size = 1)
    {
        float ratio = 1f; //gui.screen[0].slider["lod.ratio"].val; // default : 1
        float minsize = 0.01f; //gui.screen[0].slider["detail"].val;  // default : 0.01

        double dot = (double)(((p1 + p2 + p3) / 3).Dot(center));
        double dist = Math.Acos(Clamp(dot, -1, 1)) / Math.PI;


        if (dist > 0.5)
            return;//culling

        if (dist > ratio * size || size < minsize)
        {
            draw_triangle(p1, p2, p3);
            return;
        }

        // Recurse
        Vector3[] p = new Vector3[] { p1, p2, p3, (p1 + p2) / 2, (p2 + p3) / 2, (p3 + p1) / 2 };
        int[] idx = new int[] { 0, 3, 5, 5, 3, 4, 3, 1, 4, 5, 4, 2 };

        for (int i = 0; i < 4; i++)
        {
            draw_recursive(
                p[idx[3 * i + 0]].Normalized(),
                p[idx[3 * i + 1]].Normalized(),
                p[idx[3 * i + 2]].Normalized(),
                center, size / 2);
        }
    }

    public void draw(Vector3 center)
    {
        am = new ArrayMesh();
        st = new SurfaceTool();
        
        st.Begin(Mesh.PrimitiveType.Triangles);

        // create icosahedron
        float t = (float)((1.0 + Math.Sqrt(5)) / 2.0);

        Vector3[] p = new[]
        {
            new Vector3(-1, t, 0), new Vector3(1, t, 0), new Vector3(-1, -t, 0), new Vector3(1, -t, 0), new Vector3(0, -1, t), new Vector3(0, 1, t),
            new Vector3(0, -1, -t),new Vector3(0, 1, -t),new Vector3(t, 0, -1), new Vector3(t, 0, 1), new Vector3(-t, 0, -1), new Vector3(-t, 0, 1)
        };

        int[] idx = new int[]
        {
            0, 11, 5, 0, 5, 1, 0, 1, 7, 0, 7, 10, 0, 10, 11,
            1, 5, 9, 5, 11, 4, 11, 10, 2, 10, 7, 6, 10, 7, 6, 7, 1, 8,
            3, 9, 4, 3, 4, 2, 3, 2, 6, 3, 6, 8, 3, 8, 9,
            4, 9, 5, 2, 4, 11, 6, 2, 10, 8, 6, 7, 9, 8, 1
        };

        for (int i = 0; i < idx.Length / 3; i++)
        {
            draw_recursive(
                p[idx[i * 3 + 0]].Normalized(), // triangle point 1
                p[idx[i * 3 + 1]].Normalized(), // triangle point 2
                p[idx[i * 3 + 2]].Normalized(), // triangle point 3
                center);

        }
        am = st.Commit(am);
        this.mesh.SetMesh(am);
        this.UpdateCollisionShape();
    }
    public double Clamp(double val, double min, double max)
    {
        if (val.CompareTo(min) < 0)
            return min;

        else if (val.CompareTo(max) > 0)
            return max;

        else
            return val;
    }
    public void UpdateCollisionShape()
    {
        RemoveShapeOwner(0);
        CreateShapeOwner(this);
        this.ShapeOwnerAddShape(0, mesh.Mesh.CreateTrimeshShape());
    }
}
