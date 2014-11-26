using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProcAirships.UI
{
    class Label : Control
    {

        // Properties
        public String Caption
        {
            get { return caption; }
            set { caption = null != value ? value : ""; }
        }


        // Fields
        protected String caption = "";

        // Events
        //protected Event<EventArgs> SendClick = new Event<EventArgs>();      
        
        public override void OnDraw()
        {
            GUILayout.Label(caption);
            //if(GUILayout.Button(Caption))
            //{
            //    SendOnClick();
            //}        
        }

        public Label(IComposition parent, String caption = "label")
            : base(parent)
        {
            Caption = caption;
        }
    }
}
