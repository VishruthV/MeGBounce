using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MeGBounce
{
    static class Parameters
    {
        #region Private Variables
        private static ConfigCFO cfo = new ConfigCFO();
        private static string _workingDirectory = cfo.WorkingDirectory;

        private static string _systemDir = cfo.SystemDirectory;
        private static string _symbolUniverseFile = cfo.SymbolsUniverse;
        private static string _persistantDataFile = cfo.PersistantDataFile;
        private static string _SecType = cfo.SecurityType;
        #endregion

        #region System Parameters
        public static string WorkingDirectory
        {
            get { return Parameters._workingDirectory; }
            set { Parameters._workingDirectory = value; }
        }

        public static string SystemDirectory
        {
            get { return Path.Combine(_workingDirectory, _systemDir); }
            set { _systemDir = value; }
        }

        public static string SymbolUniverseFile
        {
            get { return Path.Combine(SystemDirectory, _symbolUniverseFile); }
            set { _symbolUniverseFile = value; }
        }

        public static Krs.Ats.IBNet.SecurityType SecurityType
        {
            get { return Parameters._SecType.ConvertToSecType(); }
            set { Parameters._SecType = value.ToString(); System.Diagnostics.Debugger.Launch(); } //TODO: Will lead to error. When will need to set Parameters.SecType?
        }

        public static string PersistantDataFile
        {
            get { return Path.Combine(SystemDirectory, _persistantDataFile); }
            set { _persistantDataFile = value; }
        }

        public static string Currency = cfo.Currency;

        public static string Exchange = cfo.Exchange;

        public static string ExpiryDate = cfo.ExpiryDate;

        public static string IPAddress = cfo.IPAddress;

        public static string Port = cfo.Port;

        public static string ClientId = cfo.ClientId;

        public static DateTime FetchFirstCandleAt
        {
            get
            {
                string strDateTime = DateTime.Today.Date.ToString("dd-MM-yyyy");
                DateTime ret = DateTime.Parse(string.Format("{0} {1}", strDateTime, cfo.FetchFirstCandleAt));
                if (DateTime.Now.TimeOfDay > Parameters.MarketEndTime)
                {
                    return ret.AddDays(1.0);
                }
                return ret;
            }
            set 
            { 

            }
        }
        //public static TimeSpan FetchFirstCandleAt = TimeSpan.Parse(cfo.FetchFirstCandleAt);

        //public static DateTime MarketStartTime = DateTime.Parse(DateTime.Today.Date.ToString("dd-MM-yyyy ") + cfo.MarketStartTime);
        //public static DateTime MarketEndTime = DateTime.Parse(DateTime.Today.Date.ToString("dd-MM-yyyy ") + cfo.MarketEndTime); 

        public static TimeSpan MarketStartTime = TimeSpan.Parse(cfo.MarketStartTime);
        public static TimeSpan MarketEndTime = TimeSpan.Parse(cfo.MarketEndTime);
        #endregion

        #region Strategy Parameters

        public static decimal PctMinLC
        {
            get { return cfo.PctMinLC / 100; }
            set { cfo.PctMinLC = value; }
        }

        public static decimal PctMaxLC
        {
            get { return cfo.PctMaxLC / 100; }
            set { cfo.PctMaxLC = value; }
        }

        public static decimal PctMinLT
        {
            get { return cfo.PctMinLT / 100; }
            set { cfo.PctMinLT = value; }
        }

        public static decimal PctMaxLB
        {
            get { return cfo.PctMaxLB / 100; }
            set { cfo.PctMaxLB = value; }
        }

        public static decimal PctMinLTBeyondBollingerBand
        {
            get { return cfo.PctMinLTBeyondBollingerBand / 100; }
            set { cfo.PctMinLTBeyondBollingerBand = value; }
        }

        #endregion

        //private static string allFilesDateformat = "yyyyMMdd";
        //private static string allFilesTimeformat = "HH:mm:SS"; //todo: check

        public static bool AreParametersNull()
        {
            return false;
        }
    }
}