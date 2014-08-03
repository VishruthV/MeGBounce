using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeGBounce
{
    class ConfigCFO : ApplicationSettingsBase
    {
        #region Program
        [UserScopedSettingAttribute()]
        [DefaultSettingValueAttribute(@"D:\Work\MegBounce")]
        public string WorkingDirectory
        {
            get { return this["WorkingDirectory"] as string; }
            set { this["WorkingDirectory"] = value; }
        }

        [UserScopedSettingAttribute()]
        [DefaultSettingValueAttribute(@"System")]
        public string SystemDirectory
        {
            get { return this["SystemDirectory"] as string; }
            set { this["SystemDirectory"] = value; }
        }

        [UserScopedSettingAttribute()]
        [DefaultSettingValueAttribute(@"SymbolUniverse.txt")]
        public string SymbolsUniverse
        {
            get { return this["SymbolsUniverse"] as string; }
            set { this["SymbolsUniverse"] = value; }
        }

        [UserScopedSettingAttribute()]
        [DefaultSettingValueAttribute(@"PersistantData.xml")]
        public string PersistantDataFile
        {
            get { return this["PersistantDataFile"] as string; }
            set { this["PersistantDataFile"] = value; }
        }
        #endregion

        #region TWS
        [UserScopedSettingAttribute()]
        [DefaultSettingValueAttribute("127.0.0.1")]
        public string IPAddress
        {
            get { return this["IPAddress"] as string; }
            set { this["IPAddress"] = value; }
        }

        [UserScopedSettingAttribute()]
        [DefaultSettingValueAttribute("7496")]
        public string Port
        {
            get { return this["Port"] as string; }
            set { this["Port"] = value; }
        }

        [UserScopedSettingAttribute()]
        [DefaultSettingValueAttribute("0")]
        public string ClientId
        {
            get { return this["ClientId"] as string; }
            set { this["ClientId"] = value; }
        } 
        #endregion

        #region Market
        [UserScopedSettingAttribute()]
        [DefaultSettingValueAttribute("SMART")]
        public string Exchange
        {
            get { return this["Exchange"] as string; }
            set { this["Exchange"] = value; }
        }

        [UserScopedSettingAttribute()]
        [DefaultSettingValueAttribute("27022014")]
        public string ExpiryDate
        {
            get { return this["ExpiryDate"] as string; }
            set { this["ExpiryDate"] = value; }
        }

        [UserScopedSettingAttribute()]
        [DefaultSettingValueAttribute("STK")]
        public string SecurityType
        {
            get { return this["SecurityType"] as string; }
            set { this["SecurityType"] = value; }
        }

        [UserScopedSettingAttribute()]
        [DefaultSettingValueAttribute("USD")]
        public string Currency
        {
            get { return this["Currency"] as string; }
            set { this["Currency"] = value; }
        }

        [UserScopedSettingAttribute()]
        [DefaultSettingValueAttribute("09:30:00")]
        public string MarketStartTime
        {
            get { return this["MarketStartTime"] as string; }
            set { this["MarketStartTime"] = value; }
        }

        [UserScopedSettingAttribute()]
        [DefaultSettingValueAttribute("15:30:00")]
        public string MarketEndTime
        {
            get { return this["MarketEndTime"] as string; }
            set { this["MarketEndTime"] = value; }
        }

        [UserScopedSettingAttribute()]
        [DefaultSettingValueAttribute("10:00:00")]
        public string FetchFirstCandleAt
        {
            get { return this["FetchFirstCandleAt"] as string; }
            set { this["FetchFirstCandleAt"] = value; }
        }
        #endregion

        #region Strategy
        [UserScopedSettingAttribute()]
        [DefaultSettingValueAttribute("0.5")]
        public decimal PctMinLC
        {
            get { return (decimal)this["PctMinLC"]; }
            set { this["PctMinLC"] = value; }
        }

        [UserScopedSettingAttribute()]
        [DefaultSettingValueAttribute("5.0")]
        public decimal PctMaxLC
        {
            get { return (decimal)this["PctMaxLC"]; }
            set { this["PctMaxLC"] = value; }
        }

        [UserScopedSettingAttribute()]
        [DefaultSettingValueAttribute("0.1")]
        public decimal PctMinLT
        {
            get { return (decimal)this["PctMinLT"]; }
            set { this["PctMinLT"] = value; }
        }

        [UserScopedSettingAttribute()]
        [DefaultSettingValueAttribute("50.0")]
        public decimal PctMaxLB
        {
            get { return (decimal)this["PctMaxLB"]; }
            set { this["PctMaxLB"] = value; }
        }

        [UserScopedSettingAttribute()]
        [DefaultSettingValueAttribute("1.0")]
        public decimal PctMinLTBeyondBollingerBand
        {
            get { return (decimal)this["PctMinLTBeyondBollingerBand"]; }
            set { this["PctMinLTBeyondBollingerBand"] = value; }
        } 
        #endregion
    }
}