using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProcAirships
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    class EditorController : MonoBehaviour
    {

        public static float altitude;

        private GameObject CoL;
        private GameObject CoB;
        private GameObject CoBdir;

        COBMarker cobMarker;
        EditorVesselOverlays vesselOverlays;

        bool markerVisibilityChanged = false;
        
        void Start()
        {
            
            vesselOverlays = (EditorVesselOverlays)GameObject.FindObjectOfType(
               typeof(EditorVesselOverlays));

            if (vesselOverlays == null)
                Log.post("Error. No vesselOverlays found", LogLevel.LOG_ERROR);

            CoL = vesselOverlays.CoLmarker.gameObject;

            if(CoL == null)
                Log.post("CoL == null", LogLevel.LOG_ERROR);

            if (CoL.transform.parent != null)
                Log.post("CoL parent: " + CoL.transform.parent);

            CoB = (GameObject)UnityEngine.Object.Instantiate(vesselOverlays.CoLmarker.gameObject);
            //CoBdir = (GameObject)UnityEngine.Object.Instantiate(vesselOverlays.CoLmarker.dirMarkerObject);

            try
            {
                CoBdir = CoB.transform.GetChild(0).gameObject;       
            }
            catch (Exception e)
            {
                Log.postException("Could not find CoBdir game object", e);
            }

            try
            {
                Destroy(CoB.GetComponent<EditorMarker_CoL>());
            }
            catch (Exception e)
            {
                Log.postException("Could destroy stock col marker", e);
            }
           

            CoB.name = "CoB marker";

            cobMarker = CoB.AddComponent<COBMarker>();

            //CoB.SetActive(true);
            //CoBdir.SetActive(true);
            
            if(cobMarker == null)
                Log.post("cobMarker == null", LogLevel.LOG_ERROR);

            cobMarker.enabled = true;
            
            cobMarker.posMarkerObject = CoB;

            cobMarker.dirMarkerObject = CoBdir;

            //cobMarker.referencePitch = vesselOverlays.referencePitch;
            //cobMarker.referenceSpeed = vesselOverlays.referenceAirSpeed;
            

            //CoB.renderer.enabled = true;
            //CoB.renderer.material.color = Color.gray;

            foreach(Material m in CoB.renderer.materials)
            {
                m.color = Color.gray;
            }

            
            try
            {
                vesselOverlays.toggleCoLbtn.AddValueChangedDelegate(this.CoLButtonClick);
            }
            catch (Exception e)
            {
                Log.postException("Could not add Change delegate", e);
            }

            if (util.vabActive())
                altitude = (float)Athmosphere.vabAltitude;

            if (util.sphActive())
                altitude = (float)Athmosphere.sphAltitude;

            
        }

        public void CoLButtonClick(IUIObject obj)
        {

            markerVisibilityChanged = true;
            
       
        }

        void LateUpdate()
        {
            if(markerVisibilityChanged)
            {
                CoB.SetActive(CoL.activeSelf);
                CoBdir.SetActive(CoL.activeSelf);
                markerVisibilityChanged = false;
            }

            autofillCounter = 0;
            overpressureCounter = 0;
            pControlCounter = 0;
            autoFill = false;

            if (null != EditorLogic.fetch.ship.Parts)
            {

                foreach(Part p in EditorLogic.fetch.ship.Parts)
                {
                    AirshipEnvelope env = p.GetComponent<AirshipEnvelope>();
                    if (null != env)
                    {
                        if (env.AutoFill)
                            ++autofillCounter;

                        if (env.Overpressure > 0.0f)
                            ++overpressureCounter;

                        if (env.PressureControl)
                            ++pControlCounter;
                    }
                }

            
                foreach (Part p in EditorLogic.fetch.ship.Parts)
                {
                    if (null == p)
                        continue;
                    BuoyancyStats stats = p.GetComponent<BuoyancyStats>();
                    if (null != stats)
                    {
                        netBuoyancy = stats.vesselNetBuoyancy;
                        break;
                    }
                }
            }
            else
                Log.post("null == EditorLogic.fetch.ship.Parts", LogLevel.LOG_ERROR);

        }


        private static int autofillCounter = 0;
        public static int AutoFillCounter
        {
            get
            { return autofillCounter; }
        }

        private static int pControlCounter = 0;
        public static int PControlCounter
        {
            get
            { return pControlCounter; }
        }

        private static int overpressureCounter = 0;
        public static int OverpressureCounter
        {
            get
            { return overpressureCounter; }
        }

        private static float netBuoyancy;
        public static float NetBuoyancy
        {
            get{ return netBuoyancy; }
        }

        private static bool autoFill = false;
        public static bool AutoFill
        {
            get { return autoFill; }
            set { autoFill = value; }
        }

    }
}
