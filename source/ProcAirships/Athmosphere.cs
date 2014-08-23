using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using KSP;


namespace ProcAirships
{
    abstract public class  Athmosphere
    {
        public abstract double getAirDensity();
        public abstract double getAirPressure();  
    }
}
