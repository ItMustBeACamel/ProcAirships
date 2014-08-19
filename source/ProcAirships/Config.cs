using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using KSP;

namespace ProcAirships
{
    [Serializable]
    class Config : IConfigNode
    {
        [SerializeField]
        public string name;

        [SerializeField]
        public float buoyancyMultiplicator = 5.0f;


        public void Load(ConfigNode node)
        {
            name = node.GetValue("name");
            float.TryParse(node.GetValue("buoyancyMultiplicator"), out buoyancyMultiplicator);
        }
        public void Save(ConfigNode node)
        {
            node.SetValue("name", name);
            node.SetValue("buoyancyMultiplicator", buoyancyMultiplicator.ToString());
        }

    }
}
