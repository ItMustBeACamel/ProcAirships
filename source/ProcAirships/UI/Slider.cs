using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProcAirships.UI
{
    class Slider : Control
    {

        public Slider(IComposition parent, SliderOrientation orientation, float value=0f, float leftValue=0f, float rightValue=10f)
            : base(parent)
        {
            Orientation = orientation;
            LeftValue = leftValue;
            RightValue = rightValue;
            Value = value;
        }

        public enum SliderOrientation
        {
            Horizontal,
            Vertical
        }

        public override void OnDraw()
        {
            float newValue = value;
            switch(Orientation)
            {
                case SliderOrientation.Horizontal:
                    newValue = GUILayout.HorizontalSlider(value, LeftValue, RightValue);
                    break;
                case SliderOrientation.Vertical:
                    newValue = GUILayout.VerticalSlider(value, LeftValue, RightValue);
                    break;
            }

            if(newValue != value)
            {
                value = newValue;
                SendOnChange(value);
            }
            
        }

        public event EventHandler<ChangeEventArgs<float>> OnChange;
        protected void SendOnChange(float value)
        {
            if (null != OnChange)
            {
                foreach (EventHandler<ChangeEventArgs<float>> eh in OnChange.GetInvocationList())
                {
                    try
                    {
                        ChangeEventArgs<float> args = new ChangeEventArgs<float>(value);
                        OnChange(this, args);
                    }
                    catch (Exception e)
                    {
                        Log.postException(e);
                    }
                }
            }
        }       


        


        public SliderOrientation Orientation
        {
            get; set;
        }

        private float value;
        public float Value
        {
            get { return value; }
            set { this.value = Mathf.Clamp(value, Math.Min(LeftValue, RightValue), Math.Max(LeftValue, RightValue)); }
        }

        private float rightValue;
        public float RightValue
        {
            get { return rightValue; }
            set
            {
                rightValue = value;
                Value = this.value;
            }
        }

        private float leftValue;
        public float LeftValue
        {
            get { return leftValue; }
            set
            {
                leftValue = value;
                Value = this.value;
            }
        }
    }
}
