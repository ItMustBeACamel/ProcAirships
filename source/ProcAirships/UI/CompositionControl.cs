using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProcAirships.UI
{
    abstract class CompositionControl : Control, IComposition
    {

        public CompositionControl(IComposition parent)
            : base(parent)
        { }
        
        public Control[] Children
        {
            get { return children.ToArray(); }
        }

        protected List<Control> children = new List<Control>();


        public void AddChild(Control control)
        {
            if(null != control)
                children.Add(control);
        }
    }
}
