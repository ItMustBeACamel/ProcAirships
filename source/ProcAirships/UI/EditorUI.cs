using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProcAirships.UI
{
    class EditorUI : Window
    {

        #region event handler

        public void OnBodyGridChange(object sender, ChangeEventArgs<int> e)
        {
            RefreshPresets();
            altSlider.RightValue = 0f;
            altSlider.LeftValue = (float)availableBodies[e.Value].atmosphereScaleHeight * 1000;
            UpdateAltitudeDisplay();

            UpdateAtmosphere();
        }

        public void OnAddPresetClick(object sender, EventArgs e)
        {
            InputBoxCallback okCallback = (input) =>
            {
                if (input.Length <= 0)
                    return false;

                EditorPreset newPreset = new EditorPreset(input, bodyGrid.Selection, altSlider.Value);
                if (!ProcAirships.Instance.Presets.Contains(newPreset))
                {
                    ProcAirships.Instance.Presets.Add(newPreset);
                    RefreshPresets();
                    return true;
                }
                else
                    return false;
            };
            
            InputBoxCallback cancelCallback = (input) => { return true; };

            InputBox inputBox =
            InputBox("Celestial Body: " + availableBodies[bodyGrid.Selection].theName +
            "\nAltitude: " + altSlider.Value + "m", "Preset generation",
                new InputBoxOption("OK", okCallback),
                new InputBoxOption("Cancel", cancelCallback));

            inputBox.Position = new Vector2(Rectangle.xMax, Rectangle.yMin);
            inputBox.Height = 200;           
        }

        public void OnPresetSelect(object sender, SelectEventArgs<int> e)
        {
            if(null != altSlider)
                altSlider.Value = Presets[e.Index].altitude;

            UpdateAltitudeDisplay();
            UpdateAtmosphere();
            EditorController.altitude = Presets[e.Index].altitude;
        }

        public void OnPresetRemove(object sender, CancelSelectEventArgs<int> e)
        {
            DialogOption[] options = new DialogOption[2];
            options[0] = new DialogOption("Yes", () => { ProcAirships.Instance.Presets.Remove(Presets[e.Index]); RefreshPresets(); });
            options[1] = new DialogOption("No", () => { });

            MultiOptionDialog diag = new MultiOptionDialog("Really delete this preset?", windowTitle: "Are you sure?", options: options);
            PopupDialog.SpawnPopupDialog(diag, false, HighLogic.Skin);
        }

        public void OnAltitudeSliderChange(object sender, ChangeEventArgs<float> e)
        {
            altSlider.Value = (float)Math.Truncate(e.Value);

            UpdateAltitudeDisplay();
            UpdateAtmosphere();
            EditorController.altitude = e.Value;
        }

        public void OnAutoFillClick(object sender, EventArgs e)
        {
            EditorController.AutoFill = true;
        }

        protected override void OnPreDraw()
        {       
            labNetBuoyancy.Caption = String.Format("net. Buoyancy: {0:N}kN", EditorController.NetBuoyancy);        
        }

        protected override void OnPostDraw()
        {
            int autoFillCount = EditorController.AutoFillCounter;
            int overpressureCount = EditorController.OverpressureCounter;
            int pControlCount = EditorController.PControlCounter;

            if (autoFillCount > 0)
                GUILayout.Label(" WARNING: Autofill on " + autoFillCount + " parts.");

            if (overpressureCount > 0)
                GUILayout.Label(" PRESSURE WARNING on " + overpressureCount + " parts.");

            if(pControlCount > 0)
                GUILayout.Label(" Pressure Control on " + pControlCount + " parts.");
            
        }

#endregion

        #region init

        public override void OnCreate()
        {
            Caption = "Airships";

            foreach (CelestialBody b in FlightGlobals.Bodies)
            {
                if (b.atmosphere)
                    availableBodies.Add(b);
            }

            bodyGrid = new SelectionGrid(Surface, 3);
            foreach (CelestialBody b in availableBodies)
                bodyGrid.AddItem(b.bodyName);
            bodyGrid.OnChange += OnBodyGridChange;

            LayoutGroup layoutGroupHorizontal = new LayoutGroup(Surface, LayoutGroup.LayoutOrientation.Horizontal);

            altSlider = new Slider(layoutGroupHorizontal, Slider.SliderOrientation.Vertical);
            altSlider.OnChange += OnAltitudeSliderChange;

            LayoutGroup layoutGroupPresets = new LayoutGroup(layoutGroupHorizontal, LayoutGroup.LayoutOrientation.Vertical);

            btAddPreset = new Button(layoutGroupPresets, "Add Preset");
            btAddPreset.OnClick += OnAddPresetClick;

            presetsList = new PresetList(layoutGroupPresets);
            presetsList.OnSelect += OnPresetSelect;
            presetsList.OnRemove += OnPresetRemove;
            

            labAltitude = new Label(layoutGroupPresets);

            btAutoFill = new Button(Surface, "Auto Fill");
            btAutoFill.OnClick += OnAutoFillClick;

            labNetBuoyancy = new Label(Surface);


            // init everything
            
            RefreshPresets();

            OnBodyGridChange(this, new ChangeEventArgs<int>(bodyGrid.Selection));

          
            altSlider.Value = EditorController.altitude;

            UpdateAltitudeDisplay();
        }

        #endregion

        #region functions
        private void UpdateAltitudeDisplay()
        {
            if(null != labAltitude)
                labAltitude.Caption = String.Format("Altitude: {0:N}m", altSlider.Value);
        }

        private void RefreshPresets()
        {
            Presets = ProcAirships.Instance.Presets.Where((preset) => { return preset.bodyID == bodyGrid.Selection; }).ToList();
            Presets.Sort(new PresetComparer());

            if (null != presetsList)
            {
                IEnumerable<PresetList.Item> items = Presets.Select<EditorPreset, PresetList.Item>((p) => { return new PresetList.Item(p.name + " (" + p.altitude + "m)"); });
                presetsList.ClearItems();
                presetsList.AddItems(items);
            }
        }

        private void UpdateAtmosphere()
        {
            Athmosphere.fetch().ForceBody = availableBodies[bodyGrid.Selection];
            //Athmosphere.fetch().EditorAltitude = altSlider.Value;
        }

        #endregion

        #region fields
        Button btAddPreset;
        PresetList presetsList;
        Label labAltitude;
        Slider altSlider;
        SelectionGrid bodyGrid;
        Label labNetBuoyancy;
        Button btAutoFill;
        

        List<CelestialBody> availableBodies = new List<CelestialBody>();
        List<EditorPreset> Presets;
        #endregion

    }
}
