using Godot;
using System;

public class PPFuelEditor : WindowDialog
{
    public Part PartBeingEdited;

    float topRadius=1;
    float bottomradius=1;
    float height=1;
    int stepcount = 100;

    HSlider topRadiusSlider;
    HSlider bottomRadiusSlider;
    HSlider heightSlider;

    public override void _Ready()
    {
        topRadiusSlider = (HSlider)GetNode("/root/Root/CanvasLayer/PPFuelEditor/TopRadiusPanel/HSlider");
        topRadiusSlider.Connect("value_changed", this, "OnSliderChanged");
        topRadiusSlider.SetStep(0.001f);
        topRadiusSlider.SetMax(1);
        topRadiusSlider.SetMin(0);

        bottomRadiusSlider = (HSlider)GetNode("/root/Root/CanvasLayer/PPFuelEditor/BottomRadiusPanel/HSlider");
        bottomRadiusSlider.Connect("value_changed", this, "OnSliderChanged");
        bottomRadiusSlider.SetStep(0.001f);
        bottomRadiusSlider.SetMax(1);
        bottomRadiusSlider.SetMin(0);

        heightSlider = (HSlider)GetNode("/root/Root/CanvasLayer/PPFuelEditor/HeightPanel/HSlider");
        heightSlider.Connect("value_changed", this, "OnSliderChanged");
        heightSlider.SetStep(0.001f);
        heightSlider.SetMax(1);
        heightSlider.SetMin(0);
    }
    private void OnSliderChanged(float value)
    {
        topRadius = topRadiusSlider.Value;
        bottomradius = bottomRadiusSlider.Value;
        height = heightSlider.Value;
        SurfaceTool st = new SurfaceTool();
        ArrayMesh am;
        
        //top surface
        st.Begin(Mesh.PRIMITIVE_TRIANGLES);
        st.AddColor(new Color(1, 0, 0));
        st.AddUv(new Vector2(0, 0));
        for (int step = 0; step < stepcount; step++)
        {
            st.AddVertex(new Vector3(0, height/2, 0));//center vertex
            st.AddVertex(StepToVector3(step, topRadius, height / 2));
            st.AddVertex(StepToVector3(step+1, topRadius, height / 2));
        }
        am = st.Commit();

        //bottom surface
        st.Begin(Mesh.PRIMITIVE_TRIANGLES);
        st.AddColor(new Color(1, 0, 0));
        st.AddUv(new Vector2(0, 0));
        for (int step = 0; step < stepcount; step++)
        {
            st.AddNormal(new Vector3(0, -1, 0));
            st.AddVertex(new Vector3(0, -height / 2, 0));//center vertex
            st.AddVertex(StepToVector3(step, bottomradius, -height / 2));
            st.AddVertex(StepToVector3(step - 1, bottomradius, -height / 2));
        }
        st.GenerateNormals();
        am = st.Commit(am);

        //side surface
        st.Begin(Mesh.PRIMITIVE_TRIANGLES);
        st.AddColor(new Color(1, 0, 0));
        st.AddUv(new Vector2(0, 0));
        for (int step = 0; step < stepcount; step++)
        {
            st.AddVertex(StepToVector3(step, topRadius, height / 2));
            st.AddVertex(StepToVector3(step, bottomradius, -height / 2));
            st.AddVertex(StepToVector3(step+1, bottomradius, -height / 2));

            st.AddVertex(StepToVector3(step, topRadius, height / 2));
            st.AddVertex(StepToVector3(step + 1, bottomradius, -height / 2));
            st.AddVertex(StepToVector3(step + 1, topRadius, height / 2));
        }
        am = st.Commit(am);



            PartBeingEdited.mesh.SetMesh(am);
        Console.WriteLine("Mesh changed");
    }

    private Vector3 StepToVector3(int step,float radius,float h)
    {
        float stepRadiaans = (float)(Math.PI * 2) / stepcount;
        Vector3 vector = new Vector3(0, h, 0);
        vector.x = (float)Math.Cos(stepRadiaans*step) * radius;
        vector.z = (float)Math.Sin(stepRadiaans * step) * radius;
        return vector;
    }

    public void setup()
    {

    }
    private void _on_Close_pressed()
    {
        this.Hide();
    }
}
