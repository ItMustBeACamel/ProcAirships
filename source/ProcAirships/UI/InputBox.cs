using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProcAirships.UI
{
     

    delegate bool InputBoxCallback(String input);

    class InputBoxOption
    {

        public String Caption
        {
            get
            { return caption; }
            set
            { caption = value; }
        }

        public InputBoxCallback Callback
        {
            get
            { return callback; }
            set
            { callback = value; }
        }
        

        public InputBoxOption()
        {
            caption = "OK";
            callback = (i) => { return true; };
        }

        public InputBoxOption(String caption, InputBoxCallback callback)
        {
            this.caption = caption;
            this.callback = callback;
        }


        String caption;
        InputBoxCallback callback;
    }

    class InputBox : Window
    {
        public InputBox(String text, String title, params InputBoxOption[] options)
            : base()
        {
            Caption = title;
            this.text = text;
            this.options = options;
            
        }

        protected override void OnDraw(int id)
        {
            GUILayout.Label(text, GUILayout.ExpandHeight(true));
            input = GUILayout.TextField(input);

            GUILayout.BeginHorizontal();
            foreach(InputBoxOption option in options)
            {
                if(GUILayout.Button(option.Caption))
                {
                    if (option.Callback == null)
                        Close();
                    else
                    {
                        if (option.Callback(input))
                            Close();
                    }
                }

            }
            GUILayout.EndHorizontal();
           
            
        }


        String text = "";
        String input = "";
        InputBoxOption[] options;
        
    }
}
