using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;

namespace ProcAirships
{

    public enum LogLevel : uint
    {
        LOG_INFORMATION = 1,
        LOG_ERROR = 2,
        LOG_WARNING = 3,
        LOG_DEBUG = 4,
        LOG_ALL = uint.MaxValue
    }

    public class Log
    {
        public static LogLevel logLevel = LogLevel.LOG_ALL; // the highest debug level to show in logs

        

        public static void post(object message, LogLevel level = LogLevel.LOG_DEBUG, UnityEngine.Object context = null)
        {
            if (level <= logLevel)
            {
                string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
                string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                switch (level)
                {
                    case LogLevel.LOG_ERROR:

                        if(context == null)
                            Debug.LogError("[" + assemblyName + "|" + assemblyVersion +"] " + message);
                        else
                            Debug.LogError("[" + assemblyName + "|" + assemblyVersion + "] " + message, context);
                        break;

                    case LogLevel.LOG_WARNING:

                        if (context == null)
                            Debug.LogWarning("[" + assemblyName + "|" + assemblyVersion + "] " + message);
                        else
                            Debug.LogWarning("[" + assemblyName + "|" + assemblyVersion + "] " + message, context);
                        break;

                    default:
                        if (context == null)
                            Debug.Log("[" + assemblyName + "|" + assemblyVersion + "] " + message);
                        else
                            Debug.Log("[" + assemblyName + "|" + assemblyVersion + "] " + message, context);
                        break;
                }

            }

        } // post

        public static void postException(Exception e)
        {
            Debug.LogException(e);
            //Debug.Log()
        }






    } // class Log
} // namespace
