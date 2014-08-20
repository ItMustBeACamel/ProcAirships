using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace ProcAirships
{
    public class AthmosphereFAR : Athmosphere
    {

        public override double getAirDensity()
        {
            try
            {
                return ferram4.FARAPI.GetActiveControlSys_AirDensity();
            }
            catch (Exception e)
            {
                Log.postException(e);
                return 0;
            }
        }// getAirDensity

    } // class

} // namespace
