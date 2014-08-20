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

        

        public static void post(string message, LogLevel level = LogLevel.LOG_DEBUG)
        {
            if (level <= logLevel)
            {
                string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
                string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                switch (level)
                {
                    case LogLevel.LOG_ERROR:
                        Debug.LogError("[" + assemblyName + "|" + assemblyVersion +"] " + message);
                        break;
                    case LogLevel.LOG_WARNING:
                        Debug.LogWarning("[" + assemblyName + "|" + assemblyVersion + "] " + message);
                        break;
                    default:
                        Debug.Log("[" + assemblyName + "|" + assemblyVersion + "] " + message);
                        break;
                }

            }

        } // post

        public static void postException(Exception e)
        {
            Debug.LogException(e);
        }




    } // class Log
} // namespace
