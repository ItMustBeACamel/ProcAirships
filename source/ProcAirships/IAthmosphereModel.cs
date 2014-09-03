using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProcAirships
{
    interface  IAthmosphereModel
    {

        bool init();

        double getAirDensity(double altitude, CelestialBody body);
        double getAirDensity(Vector3 worldPosition, CelestialBody body);

        double getAirPressure(double altitude, CelestialBody body);
        double getAirPressure(Vector3 worldPosition, CelestialBody body);

    }
}
