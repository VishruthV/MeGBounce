using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Krs.Ats.IBNet;

namespace MeGBounce
{
    static class DinosaurExtender
    {
        public static SecurityType ConvertToSecType(this string strSecType)
        {
            if (strSecType == "STK")
                return SecurityType.Stock;

            else if (strSecType == "FUT")
                return SecurityType.Future;

            else
                return SecurityType.Undefined;
        }

        public static string FullyQualifiedSymbol(this Krs.Ats.IBNet.Contract c)
        {
            string ret = c.Symbol + "_" + c.Exchange + "_" + c.SecurityType.ToString(); //todo: Add/remove if needed
            return ret;
        }

        internal static TimeInForce ConvertTIF(string strTif)
        {
            switch (strTif.ToUpper())
            {
                case "GTC":
                    return TimeInForce.GoodTillCancel;

                case "GTD":
                    return TimeInForce.GoodTillDate;

                case "DAY":
                    return TimeInForce.Day;

                default:
                    return TimeInForce.Undefined;
            }
        }

        internal static OrderType ConvertOrderType(string orderType_in)
        {
            switch (orderType_in.ToUpper())
            {
                case "MKT":
                    return Krs.Ats.IBNet.OrderType.Market;

                case "LMT":
                    return Krs.Ats.IBNet.OrderType.Limit;

                case "MOC":
                    return Krs.Ats.IBNet.OrderType.MarketOnClose;

                case "LOC":
                    return Krs.Ats.IBNet.OrderType.LimitOnClose;

                default:
                    throw new Exception(string.Format("Unsupported Order type: {0}", orderType_in));
            }
        }
    }
}
