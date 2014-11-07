using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProcAirships
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    class EditorGUI : MonoBehaviour
    {
        public static bool showEditorGUI = false;

        Rect editorGuiRect = new Rect(300, 300, 300, 300);

        float altitude = 0.0f;

        int currentBody = -1;

        List<CelestialBody> availableBodies = new List<CelestialBody>();

        String[] bodyNames;


        void Start()
        {

            foreach(CelestialBody b in FlightGlobals.Bodies)
            {
                if(b.atmosphere)
                    availableBodies.Add(b);
            }

            if (availableBodies.Count > 0)
                currentBody = 0;
            else
                Log.post("Could not find any celestial body with an atmosphere", LogLevel.LOG_ERROR);

            bodyNames = availableBodies.Select<CelestialBody, String>((CelestialBody b) => { return b.bodyName; }).ToArray();

            if (util.vabActive())
                altitude = (float)Athmosphere.vabAltitude;

            if (util.sphActive())
                altitude = (float)Athmosphere.sphAltitude;

            Athmosphere.fetch().EditorAltitude = altitude;

        }

        void OnGUI()
        {
            if (util.editorActive() && showEditorGUI)
            {
                UnityEngine.GUI.skin = HighLogic.Skin;
                editorGuiRect = GUILayout.Window(102342345, editorGuiRect, WinFunc, "Airships");
            }

        }


        private void WinFunc(int id)
        {
            currentBody = GUILayout.SelectionGrid(currentBody, bodyNames, 3);
            
            GUILayout.BeginHorizontal();
            altitude = (float)Math.Truncate(GUILayout.VerticalSlider(altitude, 10000, 0, GUILayout.ExpandHeight(true)));

            GUILayout.BeginVertical();
            if (GUILayout.Button("Launch Pad"))
                altitude = (float)Athmosphere.vabAltitude;

            if (GUILayout.Button("Runway"))
                altitude = (float)Athmosphere.sphAltitude;

            GUILayout.FlexibleSpace();
            GUILayout.Label(String.Format("altitude: {0:N}m", altitude));

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            if (UnityEngine.GUI.changed)
            {
                Athmosphere.fetch().ForceBody = availableBodies[currentBody];
                Athmosphere.fetch().EditorAltitude = altitude;
            }


            UnityEngine.GUI.DragWindow();


        }

    }
}
