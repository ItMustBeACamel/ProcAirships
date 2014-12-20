using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProcAirships.UI
{
    interface IComposition
    {    

        Control[] Children
        { get; }

        void AddChild(Control control);

    }
}
