using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ProcAirships.UI
{

    

    class SelectEventArgs<T> : EventArgs
    {
        public SelectEventArgs()
        {

        }

        public SelectEventArgs(T index)
            : base()
        {
            this.Index = index;
        }

        public T Index
        { get; set; }
    }

    class ChangeEventArgs<T> : EventArgs
    {
        public ChangeEventArgs()
        {

        }

        public ChangeEventArgs(T value)
            : base()
        {
            Value = value;
        }

        public T Value
        { get; set; }
    }
    
    class CancelSelectEventArgs<T> : CancelEventArgs
    {
        public CancelSelectEventArgs()
        {

        }

        public CancelSelectEventArgs(T index)
            : base()
        {
            this.Index = index;
            
        }

        public T Index
        { get; set; }

    }

    public class Event<T> where T : EventArgs
    {
        //public delegate void EventHandler(object sender, T e);

        //System.EventHandler<T> 
      
        private event EventHandler<T> FireEvent;
        
        public void Fire(object sender, T e)
        {
            if(null != FireEvent)
                FireEvent(sender, e);
        }
        public void Subscribe(EventHandler<T> handler)
        {
            FireEvent += handler;
        }
        public void Unsubscribe(EventHandler<T> handler)
        {
            FireEvent -= handler;
        }
    }

    class EventArgs<T> : EventArgs
    {
        public EventArgs()
        {
            
        }

        public EventArgs(T value)
        {
            Value = value;
        }

        public T Value
        { get; set; }
    }

    


    
}
