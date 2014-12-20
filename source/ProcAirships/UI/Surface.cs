using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProcAirships.UI
{
    class Surface: CompositionControl, IComposition
    {
        
        public Surface(Window window)
            : base(null)
        {
            this.window = window;
        }


        public override void OnDraw()
        {
            foreach (Control c in Children)
                c.Draw();     
        }


        private Window window;
        
    }
}
