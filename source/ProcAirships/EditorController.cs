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
            
            //EZValueChangedDelegate

            
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

        }

    }
}
