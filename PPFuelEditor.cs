using Godot;
using System;

public class PPFuelEditor : WindowDialog
{
    public Part PartBeingEdited;

    float topRadius = 1;
    float bottomradius = 1;
    float height = 1;
    int stepcount = 100;

    float massMultiplier = 0.001f;//mass = volume*this

    HSlider topRadiusSlider;
    HSlider bottomRadiusSlider;
    HSlider heightSlider;

    public override void _Ready()
    {
        topRadiusSlider = (HSlider)GetNode("/root/VAB/CanvasLayer/PPFuelEditor/TopRadiusPanel/HSlider");
        topRadiusSlider.Connect("value_changed", this, "OnSliderChanged");
        topRadiusSlider.SetStep(0.001f);
        topRadiusSlider.SetMax(1);
        topRadiusSlider.SetMin(0);

        bottomRadiusSlider = (HSlider)GetNode("/root/VAB/CanvasLayer/PPFuelEditor/BottomRadiusPanel/HSlider");
        bottomRadiusSlider.Connect("value_changed", this, "OnSliderChanged");
        bottomRadiusSlider.SetStep(0.001f);
        bottomRadiusSlider.SetMax(1);
        bottomRadiusSlider.SetMin(0);

        heightSlider = (HSlider)GetNode("/root/VAB/CanvasLayer/PPFuelEditor/HeightPanel/HSlider");
        heightSlider.Connect("value_changed", this, "OnSliderChanged");
        heightSlider.SetStep(0.001f);
        heightSlider.SetMax(1);
        heightSlider.SetMin(0);
    }
    private void OnSliderChanged(float value)
    {
        #region updatelabels
        Label trcur = (Label)GetNode("/root/VAB/CanvasLayer/PPFuelEditor/TopRadiusPanel/ValueCurr");
        Label trmin = (Label)GetNode("/root/VAB/CanvasLayer/PPFuelEditor/TopRadiusPanel/ValueMin");
        Label trmax = (Label)GetNode("/root/VAB/CanvasLayer/PPFuelEditor/TopRadiusPanel/ValueMax");

        Label brcur = (Label)GetNode("/root/VAB/CanvasLayer/PPFuelEditor/BottomRadiusPanel/ValueCurr");
        Label brmin = (Label)GetNode("/root/VAB/CanvasLayer/PPFuelEditor/BottomRadiusPanel/ValueMin");
        Label brmax = (Label)GetNode("/root/VAB/CanvasLayer/PPFuelEditor/BottomRadiusPanel/ValueMax");

        Label hcur = (Label)GetNode("/root/VAB/CanvasLayer/PPFuelEditor/HeightPanel/ValueCurr");
        Label hmin = (Label)GetNode("/root/VAB/CanvasLayer/PPFuelEditor/HeightPanel/ValueMin");
        Label hmax = (Label)GetNode("/root/VAB/CanvasLayer/PPFuelEditor/HeightPanel/ValueMax");

        trcur.Text = topRadiusSlider.Value.ToString() + " m";
        brcur.Text = bottomRadiusSlider.Value.ToString() + " m";
        hcur.Text = heightSlider.Value.ToString() + " m";

        trmin.Text = topRadiusSlider.MinValue.ToString() + " m";
        brmin.Text = bottomRadiusSlider.MinValue.ToString() + " m";
        hmin.Text = heightSlider.MinValue.ToString() + " m";

        trmax.Text = topRadiusSlider.MaxValue.ToString() + " m";
        brmax.Text = bottomRadiusSlider.MaxValue.ToString() + " m";
        hmax.Text = heightSlider.MaxValue.ToString() + " m";
        #endregion
        #region updatemesh
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
            st.AddVertex(new Vector3(0, height / 2, 0));//center vertex
            st.AddVertex(StepToVector3(step, topRadius, height / 2));
            st.AddVertex(StepToVector3(step + 1, topRadius, height / 2));
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
            st.AddVertex(StepToVector3(step + 1, bottomradius, -height / 2));

            st.AddVertex(StepToVector3(step, topRadius, height / 2));
            st.AddVertex(StepToVector3(step + 1, bottomradius, -height / 2));
            st.AddVertex(StepToVector3(step + 1, topRadius, height / 2));
        }
        am = st.Commit(am);
        PartBeingEdited.mesh.SetMesh(am);
        #endregion

        int ymul = 1;
        foreach (Node child in PartBeingEdited.GetChildren())
        {
            if(child.IsClass("Area"))
            {
                Area area = (Area)child;
                Vector3 pos = new Vector3(0, (height/2)*ymul, 0);
                area.SetTranslation(pos);
                ymul *= -1;
            }
        }

        Label volumelabel = (Label)GetNode("/root/VAB/CanvasLayer/PPFuelEditor/StatPanel/StatGrid/VolumeLabel");
        Label drymasslabel = (Label)GetNode("/root/VAB/CanvasLayer/PPFuelEditor/StatPanel/StatGrid/DryMassLabel");

        PartBeingEdited.volume = (float)(Math.PI / 3) * (bottomradius * bottomradius + bottomradius * topRadius + topRadius * topRadius) * height;// V = 1/3 * PI * (r1^2+r1*r2+r2^2) * h
        volumelabel.Text = "Volume: " + PartBeingEdited.volume+ " m\xB3";

        PartBeingEdited.mass = PartBeingEdited.volume * massMultiplier;
        drymasslabel.Text = ("Dry mass: " + PartBeingEdited.mass + " Kg");

        #region fuel
        //find all engine parts


        #endregion
    }

    private Vector3 StepToVector3(int step, float radius, float h)
    {
        float stepRadiaans = (float)(Math.PI * 2) / stepcount;
        Vector3 vector = new Vector3(0, h, 0);
        vector.x = (float)Math.Cos(stepRadiaans * step) * radius;
        vector.z = (float)Math.Sin(stepRadiaans * step) * radius;
        return vector;
    }


    #region changeslidervalues

    private void _on_Smaller0Top_pressed()
    {
        ChangeSlider(topRadiusSlider, -0.1f);
    }


    private void _on_Smaller1Top_pressed()
    {
        ChangeSlider(topRadiusSlider, -1f);
    }


    private void _on_Larger0Top_pressed()
    {
        ChangeSlider(topRadiusSlider, 0.1f);
    }


    private void _on_Larger1Top_pressed()
    {
        ChangeSlider(topRadiusSlider, 1f);
    }


    private void _on_Smaller0Bottom_pressed()
    {
        ChangeSlider(bottomRadiusSlider, -0.1f);
    }


    private void _on_Smaller1Bottom_pressed()
    {
        ChangeSlider(bottomRadiusSlider, -1f);
    }


    private void _on_Larger0Bottom_pressed()
    {
        ChangeSlider(bottomRadiusSlider, 0.1f);
    }


    private void _on_Larger1Bottom_pressed()
    {
        ChangeSlider(bottomRadiusSlider, 1f);
    }


    private void _on_Smaller0Height_pressed()
    {
        ChangeSlider(heightSlider, -0.1f);
    }


    private void _on_Smaller1Height_pressed()
    {
        ChangeSlider(heightSlider, -1f);
    }


    private void _on_Larger0Height_pressed()
    {
        ChangeSlider(heightSlider, 0.1f);
    }


    private void _on_Larger1Height_pressed()
    {
        ChangeSlider(heightSlider, 1f);
    }

    private void ChangeSlider(HSlider slider, float value)
    {
        float slidervalue = slider.Value;
        float slidermax = slider.MaxValue;
        float slidermin = slider.MinValue;
        slider.Value = (float)Math.Round(slider.Value * 10) / 10;//rounding


        if (slider.Value + value < slidermax & slider.Value + value > slidermin)//slider + value between min and max
        {
            slider.Value += value;
        }
        else
        {
            slider.MaxValue += value;
            slider.MinValue += value;
            slider.Value += value;
        }
        OnSliderChanged(0);
    }

    #endregion

    public void setup()
    {
        OnSliderChanged(0);
    }
}



