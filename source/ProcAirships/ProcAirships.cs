using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProcAirships
{
    [KSPScenario(ScenarioCreationOptions.AddToAllGames, GameScenes.FLIGHT, GameScenes.EDITOR, GameScenes.SPH, GameScenes.SPACECENTER)]
    class ProcAirships : ScenarioModule
    {
        private static ProcAirships instance = null;

        [KSPField(isPersistant=true)]
        public float buoyancyMultiplicator = 5.0f;

        [KSPField(isPersistant = true)]
        public bool pressureDestruction = true;

        [KSPField(isPersistant = true)]
        public bool alwaysControllable = false;

        [KSPField(isPersistant = true)]
        public bool showTemperatureInFlight = true;

        public List<EditorPreset> Presets = new List<EditorPreset>();


        public static ProcAirships Instance
        {
            get
            {
                return instance;
            }
        }

        ProcAirships()
            : base()
        {
            instance = this;            
        }


        public override void OnAwake()
        {
            Log.post("ProcAirships Scenario onAwake", LogLevel.LOG_DEBUG);
            base.OnAwake();
        }


        public override void OnLoad(ConfigNode node)
        {
            Log.post("loading presets", LogLevel.LOG_DEBUG);
            ConfigNode presetsNode = node.GetNode("PRESETS");

            if(null != presetsNode)
            {
                foreach (ConfigNode n in presetsNode.GetNodes("PRESET"))
                {
                    try
                    {
                        EditorPreset newPreset = new EditorPreset();
                        newPreset.Load(n);

                        Log.post("loaded preset: " + newPreset.name);
                        Presets.Add(newPreset);
                    }
                    catch(Exception e)
                    {
                        Log.postException("Exception while loading preset", e);
                    }
                }
            }
            else
            {
                Log.post("Could not find config node Presets", LogLevel.LOG_WARNING);
                Presets.Add(new EditorPreset("Launch Pad", 0, (float)Athmosphere.vabAltitude));
                Presets.Add(new EditorPreset("Runway", 0, (float)Athmosphere.sphAltitude));
            }


            Log.post("loaded presets", LogLevel.LOG_DEBUG);
            base.OnLoad(node);
        }

        public override void OnSave(ConfigNode node)
        {   
            ConfigNode presetsNode = new ConfigNode("PRESETS");
          
            foreach(EditorPreset p in Presets)
            {
                ConfigNode newNode = new ConfigNode("PRESET");

                if (newNode != null)
                {
                    p.Save(newNode);
                    presetsNode.AddNode(newNode);
                    Log.post("saved preset: " + p.name);
                }
            }

            node.AddNode(presetsNode);
            Log.post("saved presets", LogLevel.LOG_DEBUG);
            base.OnSave(node);
        }      
      
    }


    class PresetComparer : IComparer<EditorPreset>
    {

        public int Compare(EditorPreset x, EditorPreset y)
        {
            return -x.altitude.CompareTo(y.altitude);
        }
    }


    class EditorPreset : IConfigNode, IEquatable<EditorPreset>
    {
        public string name;
        public int bodyID;
        public float altitude;

        public EditorPreset()
        {
            
        }

        public EditorPreset(string name, int body, float alt)
        {
            this.name = name;
            this.bodyID = body;
            this.altitude = alt;
        }

        public void Load(ConfigNode node)
        {
            name = node.GetValue("name");
            bodyID = int.Parse(node.GetValue("body"));
            altitude = float.Parse(node.GetValue("alt"));
            
        }

        public void Save(ConfigNode node)
        {
            node.AddValue("name", name);
            node.AddValue("body", bodyID.ToString());
            node.AddValue("alt", altitude.ToString());
        }
        
        public bool Equals(EditorPreset other)
        {
            return name == other.name && bodyID == other.bodyID ? true : false;
        }
        
    }
}
