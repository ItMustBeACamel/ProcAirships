using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using UnityEngine;

namespace ProcAirships
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    class GUI: MonoBehaviour
    {

        bool LockedKSC = false;
        private static ApplicationLauncherButton LauncherButton = null;

        private static bool showOptions = false;
        

        private SortedDictionary<LogLevel, String> LogLevelNames = null;

        GUI()
        {
            LogLevelNames = new SortedDictionary<LogLevel, String>();

            
            LogLevelNames.Add(LogLevel.LOG_ERROR, "Error");
            LogLevelNames.Add(LogLevel.LOG_WARNING, "Warning");
            LogLevelNames.Add(LogLevel.LOG_INFORMATION, "Information");
            LogLevelNames.Add(LogLevel.LOG_DEBUG, "Debug");
            LogLevelNames.Add(LogLevel.LOG_ALL, "All");     
        }
        
        void Awake()
        {
            Debug.Log("awake");
           
            GameEvents.onGUIApplicationLauncherReady.Add(OnAppLauncherReady);
            GameEvents.onGUIApplicationLauncherDestroyed.Add(OnAppLauncherDestroyed);       
        }

        void Destroy()
        {
            Debug.Log("destroy");

            GameEvents.onGUIApplicationLauncherReady.Remove(OnAppLauncherReady);
            GameEvents.onGUIApplicationLauncherDestroyed.Remove(OnAppLauncherDestroyed);  
            
        }


        public void OnAppLauncherReady()
        {
            if (ApplicationLauncher.Ready && !LauncherButton)
            {
                Debug.Log("adding button");
                LauncherButton = ApplicationLauncher.Instance.AddModApplication(ShowOptions, HideOptions, doNothing, doNothing, doNothing, doNothing,
                    ApplicationLauncher.AppScenes.SPACECENTER | ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.SPH,
                    (Texture)GameDatabase.Instance.GetTexture("ProcAirships/Textures/launcher", false));

            }
        }

        public void OnAppLauncherDestroyed()
        {
            Debug.Log("deleting button");
            LauncherButton = null;
        }


        public static void ShowOptions()
        {
            if(util.editorActive())
            {
                EditorGUI.showEditorGUI = true;
            }
            else
                showOptions = true;
        }

        public static void HideOptions()
        {
            if (util.editorActive())
            {
                EditorGUI.showEditorGUI = false;
            }
            else
                showOptions = false;
        }

        public static void doNothing()
        {

        }

        Rect optionsRect = new Rect(300, 300, 550, 300);
      

        void OnGUI()
        {  
            
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER && showOptions)
            {
                UnityEngine.GUI.skin = HighLogic.Skin;
                optionsRect = GUILayout.Window(4345670, optionsRect, optionsFunc, "Procedural Airships Options");
            }

            


        }

        void Update()
        {

            if (HighLogic.LoadedScene == GameScenes.SPACECENTER && showOptions)
            {
                Vector2 mousePos = Input.mousePosition;
                mousePos.y = Screen.height - mousePos.y;

                if (optionsRect.Contains(mousePos))
                {
                    if (!LockedKSC)
                    {
                        InputLockManager.SetControlLock(ControlTypes.KSC_ALL, "ProcAirshipsLock");
                        LockedKSC = true;
                    }
                }
                else
                {
                    if (LockedKSC)
                    {
                        InputLockManager.RemoveControlLock("ProcAirshipsLock");
                        LockedKSC = false;
                    }
                }
            }

        }

        public void optionsFunc(int id)
        {
            GUILayout.Space(3);
            GUILayout.Label(" --- Save Game Preferences --- ", GUILayout.ExpandWidth(true));
            GUILayout.Space(3);

            GUILayout.BeginHorizontal();
            GUILayout.Label(String.Format("Buoyancy: {0:N} x", ProcAirships.Instance.buoyancyMultiplicator));
            ProcAirships.Instance.buoyancyMultiplicator = (float)Math.Truncate((Double)GUILayout.HorizontalSlider(ProcAirships.Instance.buoyancyMultiplicator, 1.0f, 15.0f, GUILayout.ExpandWidth(true)));
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            ProcAirships.Instance.pressureDestruction = GUILayout.Toggle(ProcAirships.Instance.pressureDestruction, "Pressure Damage");
            ProcAirships.Instance.alwaysControllable = GUILayout.Toggle(ProcAirships.Instance.alwaysControllable, "Always Controllable");
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            ProcAirships.Instance.showTemperatureInFlight = GUILayout.Toggle(ProcAirships.Instance.showTemperatureInFlight, "Show temperature in flight");
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();



            GUILayout.Space(3);
            GUILayout.Label(" --- Debug Preferences --- ", GUILayout.ExpandWidth(true));
            GUILayout.Space(3);

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            Preferences.showVolumeInfoInEditor = GUILayout.Toggle(Preferences.showVolumeInfoInEditor, "Show volume in editor");
            Preferences.showVolumeInfoInFlight = GUILayout.Toggle(Preferences.showVolumeInfoInFlight, "Show volume in flight");
            Preferences.showBuoyancyInEditor = GUILayout.Toggle(Preferences.showBuoyancyInEditor, "Show buoyancy in editor");
            Preferences.showBuoyancyInFlight = GUILayout.Toggle(Preferences.showBuoyancyInFlight, "Show buoyancy in flight");
            Preferences.showVesselMassInEditor = GUILayout.Toggle(Preferences.showVesselMassInEditor, "Show vessel mass in editor");
            Preferences.showVesselMassInFlight = GUILayout.Toggle(Preferences.showVesselMassInFlight, "Show vessel mass in flight");
            Preferences.showTemperatureInEditor = GUILayout.Toggle(Preferences.showTemperatureInEditor, "Show temperature in editor");
            GUILayout.EndVertical();


            GUILayout.BeginVertical();
            Preferences.showAbsPressureInEditor = GUILayout.Toggle(Preferences.showAbsPressureInEditor, "Show abs. pressure in editor");
            Preferences.showAbsPressureInFlight = GUILayout.Toggle(Preferences.showAbsPressureInFlight, "Show abs. pressure in flight");
            Preferences.showVesselBuoyancyInEditor = GUILayout.Toggle(Preferences.showVesselBuoyancyInEditor, "Show vessel buoyancy in editor");
            Preferences.showVesselBuoyancyInFlight = GUILayout.Toggle(Preferences.showVesselBuoyancyInFlight, "Show vessel buoyancy in flight");
            Preferences.showGravPullInEditor = GUILayout.Toggle(Preferences.showGravPullInEditor, "Show grav. pull in editor");
            Preferences.showGravPullInFlight = GUILayout.Toggle(Preferences.showGravPullInFlight, "Show grav. pull in flight");
            /*Preferences.debugLevel = */
            //GUILayout.Label(((LogLevel)Preferences.debugLevel).ToString());
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.Label(String.Format("Logging Level: {0}", LogLevelNames[(LogLevel)Preferences.debugLevel]));
            
            GUILayout.BeginHorizontal();
            foreach(LogLevel ll in LogLevelNames.Keys)
            {
                if(GUILayout.Button(LogLevelNames[ll]))
                {
                    Preferences.debugLevel = (uint)ll;
                }
            } 
            GUILayout.EndHorizontal();
            
            // Do stuff before windows dragging
            UnityEngine.GUI.DragWindow();

        }

    }
}
