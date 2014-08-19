using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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
                switch (level)
                {
                    case LogLevel.LOG_ERROR:
                        Debug.LogError("[ProcAirships]" + message);
                        break;
                    case LogLevel.LOG_WARNING:
                        Debug.LogWarning("[ProcAirships]" + message);
                        break;
                    default:
                        Debug.Log("[ProcAirships]" + message);
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
