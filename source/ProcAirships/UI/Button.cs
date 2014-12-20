using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProcAirships.UI
{
    //public delegate void OnClickHandler(object sender, EventArgs e);

    class Button : Control
    {
        // Properties
        public String Caption
        {
            get { return caption; }
            set { caption = null != value ? value : ""; }
        }

        //public delegate void OnClickHandler(object sender, EventArgs e);
        
        public event EventHandler OnClick;
        protected void SendOnClick()
        {
            if(null != OnClick)
            {
                foreach (EventHandler eh in OnClick.GetInvocationList())
                {
                    try
                    {
                        OnClick(this, EventArgs.Empty);
                    }
                    catch(Exception e)
                    {
                        Log.postException(e);
                    }
                }
            }
        }       

        //public Event<EventArgs> OnClick
        //{ get { return SendClick; } }


        // Fields
        protected String caption = "";

        // Events
        //protected Event<EventArgs> SendClick = new Event<EventArgs>();      
        
        public override void OnDraw()
        {    
            if(GUILayout.Button(Caption))
            {
                SendOnClick();
            }        
        }

        public Button(IComposition parent, String caption = "button")
            : base(parent)
        {
            Caption = caption;
        }


    }
}
