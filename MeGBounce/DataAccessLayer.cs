using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeGBounce
{
    public class DataAccessLayer
    {
        private static DataAccessLayer _myDataAccessLayer = null;
        private MegBounceDataSet myData = null;
        private object DataSetWriteLock = new object(); //TODO: SD I have a dataset level lock or data table level ?
        private object DataSetCountQueryLock = new object();

        private DataAccessLayer()
        {
            if (myData == null)
            {
                myData = new MegBounceDataSet();

                //Deserializing the data [RK: Only one xml data file is maintained.. Data files will not be stored for everyday]//RK:Requirement Key, sorta
                string filename = Parameters.PersistantDataFile;
                if (System.IO.File.Exists(filename))
                {
                    myData.ReadXml(filename);
                }

                //todo: WriteXML

                //Data CleanUp
                //TODO
                /*
                 * Select *
                 * GroupBy Symbol
                 * OrderBy Date First and then by start time
                 * if no of rows in a group is > x
                 * delete the top most row of every group
                 * -----------------------
                 * Reset the ID Column
                 */
            }
        }

        public static DataAccessLayer GetMySingletonDataAccessLayer()
        {
            if (_myDataAccessLayer == null)
            {
                _myDataAccessLayer = new DataAccessLayer();
            }

            return _myDataAccessLayer;
        }


        internal MegBounceDataSet.HistoricalDataRequestsRow GetHistoricalRequestById(int requestId)
        {
            return myData.HistoricalDataRequests.FindByRequestId(requestId);
        }

        internal List<CandleData> GetLatestNCandlesLatestFirst(string _symbol, int _noOfCandles)
        {
            var query = ((from candles in myData.Candles
                          where candles.Symbol == _symbol
                          orderby candles.CandleStartDateTime descending
                          select candles).Take(_noOfCandles)).ToList();

            List<CandleData> ret = new List<CandleData>();

            foreach (var q in query)
            {
                CandleData cdl = new CandleData
                {
                    Symbol = q.Symbol,
                    CandleStartDateTime = q.CandleStartDateTime,
                    Open = q.Open,
                    High = q.High,
                    Low = q.Low,
                    Close = q.Close,
                    Volume = q.Volume
                };
                ret.Add(cdl);
            }

            return ret;
        }

        internal bool TryGetLatestNCandles(string _symbol, int _noOfCandles)
        {
            lock (DataSetCountQueryLock)
            {
                int count = ((from candles in myData.Candles
                              where candles.Symbol == _symbol
                              orderby candles.CandleStartDateTime descending
                              select candles).Take(_noOfCandles)).Count();

                if (count >= _noOfCandles)
                    return true;
                else
                    return false;
            }
        }


        internal void WriteHDataError(int reqId, string message)
        {
            lock (DataSetWriteLock)
            {
                MegBounceDataSet.HistoricalDataRequestsRow drRow = myData.HistoricalDataRequests.FindByRequestId(reqId);
                if (drRow != null)
                {
                    drRow.HasError = true;
                    drRow.Message = message;
                }
            }
        }

        internal void WriteNewOrderEntry(int orderId, string fullyQualifiedSymbol)
        {
            lock (DataSetWriteLock)
            {
                MegBounceDataSet.OrdersRow drORow = myData.Orders.NewOrdersRow();
                drORow.OrderId = orderId;
                drORow.FQSymbol = fullyQualifiedSymbol;
                myData.Orders.AddOrdersRow(drORow);
            }
        }

        internal void WriteHRequestCompleted(int reqId)
        {
            lock (DataSetWriteLock)
            {
                GetHistoricalRequestById(reqId).Completed = true; // if null exception, something sd be wrong.
            }
        }

        internal void WriteHDataRequest(int reqId, string symbol, TimeSpan barSzie)
        {
            lock (DataSetWriteLock)
            {
                MegBounceDataSet.HistoricalDataRequestsRow drHReqRow = myData.HistoricalDataRequests.NewHistoricalDataRequestsRow();
                drHReqRow.RequestId = reqId;
                drHReqRow.Symbol = symbol;
                drHReqRow.BarSize = barSzie;
                myData.HistoricalDataRequests.AddHistoricalDataRequestsRow(drHReqRow);
            }
        }

        internal void WriteHistoricalData(int _reqId, DateTime _cdlStartDateTime, decimal _open, decimal _high, decimal _low, decimal _close, int _volume)
        {
            lock (DataSetWriteLock)
            {
                MegBounceDataSet.HistoricalDataRequestsRow dr = myData.HistoricalDataRequests.FindByRequestId(_reqId);
                string _symbol = dr.Symbol;
                TimeSpan _barsize = dr.BarSize;

                if (myData.Candles.FindByCandleStartDateTimeSymbol(_cdlStartDateTime, _symbol) == null)
                {
                    MegBounceDataSet.CandlesRow drCdl = myData.Candles.NewCandlesRow();
                    drCdl.Symbol = _symbol;
                    drCdl.CandleStartDateTime = _cdlStartDateTime;
                    drCdl.Open = _open;
                    drCdl.High = _high;
                    drCdl.Low = _low;
                    drCdl.Close = _close;
                    drCdl.Volume = _volume;
                    myData.Candles.AddCandlesRow(drCdl);
                }
            }
        }

        public void DumpData(string directory)
        {
            myData.HistoricalDataRequests.WriteXml(directory + "HistoricalRequests.xml");
            myData.Candles.WriteXml(directory + "CandleData.xml");
        }
    }
}