using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace MeGBounce
{
    public static class Log
    {
        private static ICollection<string> category;
        private static bool setupDone = false;

        static LogWriter writer = null;

        private static void SetUp()
        {
            if (!setupDone)
            {
                writer = EnterpriseLibraryContainer.Current.GetInstance<LogWriter>();
                category = new List<string> { "Debug" };
                setupDone = true;
            }
        }

        public static void Info(string message)
        {
            SetUp();

            LogEntry logEntry = new LogEntry();
            logEntry.Message = message;
            logEntry.Severity = System.Diagnostics.TraceEventType.Information;
            logEntry.Priority = (int)Priority.Low;
            writer.Write(logEntry);
        }

        public static void Error(string message)
        {
            SetUp();

            LogEntry logEntry = new LogEntry();
            logEntry.Message = message;
            logEntry.Severity = System.Diagnostics.TraceEventType.Error;
            logEntry.Priority = (int)Priority.High;
            writer.Write(logEntry);
        }

        public static void Warning(string message)
        {
            SetUp();

            LogEntry logEntry = new LogEntry();
            logEntry.Message = message;
            logEntry.Severity = System.Diagnostics.TraceEventType.Warning;
            logEntry.Priority = (int)Priority.High;
            writer.Write(logEntry);
        }

        public static void WriteExceptionLog(Exception exc)
        {
            SetUp();

            if (!(exc is System.Configuration.ConfigurationErrorsException))
            {
                LogEntry logEntry = new LogEntry();
                logEntry.Message = exc.ToString();
                logEntry.Severity = System.Diagnostics.TraceEventType.Error;
                logEntry.Priority = (int)Priority.Highest;
                writer.Write(logEntry);
            }
        }

        internal static void PrintAndLogInfo(string message)
        {
            Console.WriteLine(message);
            Info(message);
        }

        internal static void Debug(string message)
        {
            SetUp();
            LogEntry logEntry = new LogEntry();
            logEntry.Message = message;
            logEntry.Severity = System.Diagnostics.TraceEventType.Information;
            logEntry.Priority = (int)Priority.Normal;
            logEntry.Categories = category;
            writer.Write(logEntry);
        }
    }

    public enum Priority
    {
        Lowest = 0,
        Low = 1,
        Normal = 2,
        High = 3,
        Highest = 4
    }
}