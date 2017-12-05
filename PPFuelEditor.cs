using Godot;
using System;

public class PPFuelEditor : PopupPanel
{
    public Part PartBeingEdited;

    //float topRadius=1;
    //float bottomradius=1;
    //float height=1;

    //HSlider topRadiusSlider;
    //HSlider bottomRadiusSlider;
    //HSlider heightSlider;

    public override void _Ready()
    {
        //topRadiusSlider = (HSlider)GetNode("/root/Root/CanvasLayer/PPFuelEditor/TopRadiusPanel/HSlider");
        //topRadiusSlider.Connect("changed", this, "OnSliderChanged");
        //topRadiusSlider.SetMax(1);
        //topRadiusSlider.SetMin(0);

        //bottomRadiusSlider = (HSlider)GetNode("/root/Root/CanvasLayer/PPFuelEditor/BottomRadiusPanel/HSlider");
        //bottomRadiusSlider.Connect("changed", this, "OnSliderChanged");
        //bottomRadiusSlider.SetMax(1);
        //bottomRadiusSlider.SetMin(0);

        //heightSlider = (HSlider)GetNode("/root/Root/CanvasLayer/PPFuelEditor/HeightPanel/HSlider");
        //heightSlider.Connect("changed", this, "OnSliderChanged");
        //heightSlider.SetMax(1);
        //heightSlider.SetMin(0);
    }
    private void OnSliderChanged()
    {
    //    topRadius = topRadiusSlider.Value;
    //    bottomradius = bottomRadiusSlider.Value;
    //    height = heightSlider.Value;

    //    SurfaceTool st = new SurfaceTool();
        //st.Begin(Mesh.)
    }

    public void setup()
    {

    }
    private void _on_Close_pressed()
    {
        this.Hide();
    }
}


