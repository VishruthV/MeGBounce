using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krs.Ats.IBNet;
using System.Threading.Tasks;

namespace MeGBounce
{
    class TwsConnector
    {
        public event EventHandler<OrderStatusEventArgs> MySharedOrderStatusEvent;

        #region Private Members
        private IBClient ibclient = new IBClient();
        private static TwsConnector myTws = null;
        private DataAccessLayer dataAccess = null;
        private object myOrderIdLock = new object();
        private object myOrderStatusLock = new object();

        private bool myOrderIdReceived = false;

        //Temp int used for OrderID ref
        int myOrderIdRef = -1;

        //Order ID Field
        private int _orderId;

        //Order ID Property
        public int OrderId
        {
            get
            {
                System.Threading.Interlocked.Increment(ref myOrderIdRef);
                return myOrderIdRef;
            }
            set
            {
                _orderId = value;
            }
        }

        private Dictionary<string, bool> AccountDownloadEndData = new Dictionary<string, bool>();
        //Account, Symbol, Quantity
        private Dictionary<Tuple<string, string>, int> OPData = new Dictionary<Tuple<string, string>, int>();
        private Tuple<string, string> myTuple = null;

        //List<OpenPositionDataStr> OPData = new List<OpenPositionDataStr>();

        private const string twsError = "TWS Error";
        #endregion

        #region Private Constructor
        private TwsConnector()
        {
            dataAccess = DataAccessLayer.GetMySingletonDataAccessLayer();

            ibclient.ThrowExceptions = true;

            ibclient.NextValidId += ibclient_NextValidId;
            ibclient.Error += ibclient_Error;
            ibclient.OrderStatus += ibclient_OrderStatus;
            ibclient.UpdatePortfolio += ibclient_UpdatePortfolio;
            ibclient.AccountDownloadEnd += ibclient_AccountDownloadEnd;
            ibclient.HistoricalData += ibclient_HistoricalData;

            if (!this.ibclient.Connected)
            {
                this.ibclient.Connect(Parameters.IPAddress, Convert.ToInt32(Parameters.Port), Convert.ToInt32(Parameters.ClientId));
                this.ibclient.RequestIds(1);
                while (!myOrderIdReceived)
                {
                    System.Threading.Thread.Sleep(10);
                }

                
            }
        }
        #endregion

        #region Singleton Manager
        public static TwsConnector GetMySingletonTwsConnector()
        {
            if (myTws == null)
            {
                myTws = new TwsConnector();
            }
            return myTws;
        }
        #endregion

        #region IB Events
        void ibclient_OrderStatus(object sender, OrderStatusEventArgs e)
        {
            if (this.MySharedOrderStatusEvent != null)  //Todo: Test this event firing and catching in OrderManager
            {
                MySharedOrderStatusEvent(this, e);
            }
        }

        void ibclient_AccountDownloadEnd(object sender, AccountDownloadEndEventArgs e)
        {
            if (AccountDownloadEndData.ContainsKey(e.AccountName))
            {
                AccountDownloadEndData[e.AccountName] = true;
            }
        }

        void ibclient_UpdatePortfolio(object sender, UpdatePortfolioEventArgs e)
        {
            //Log.Debug(string.Format("Portfolio Update: {0} {1} {2}", e.AccountName, e.Contract.Symbol, e.Position));
            //OpenPositionDataStr op = new OpenPositionDataStr { Account = e.AccountName, Symbol = e.Contract.Symbol, Quantity = e.Position };

            //myTuple = Tuple.Create(e.AccountName, e.Contract.Symbol);
            myTuple = Tuple.Create(e.AccountName, e.Contract.LocalSymbol);

            if (!OPData.ContainsKey(myTuple))
                OPData.Add(myTuple, e.Position);
        }

        void ibclient_Error(object sender, ErrorEventArgs e)
        {
            Log.Error(string.Format("{0}: {1} - {2}, ID: {3}", twsError, e.ErrorCode, e.ErrorMsg, e.TickerId));

            MegBounceDataSet.HistoricalDataRequestsRow drHRow = dataAccess.GetHistoricalRequestById(e.TickerId);

            if (drHRow != null)
            {
                Log.Error(string.Format("Historical Data request Error: {0}: {1} - {2}, ID: {3}", twsError, e.ErrorCode, e.ErrorMsg, e.TickerId));
                dataAccess.WriteHDataError(e.TickerId, string.Format("Msg: {0}. Error Code: {1}", e.ErrorMsg, e.ErrorCode));
            }
        }

        void ibclient_NextValidId(object sender, NextValidIdEventArgs e)
        {
            OrderId = e.OrderId;
            myOrderIdReceived = true;
        }

        void ibclient_HistoricalData(object sender, HistoricalDataEventArgs e)
        {
            //e.Date is the Candle Start DateTime

            if ((e.Date.TimeOfDay < Parameters.MarketStartTime) || (e.Date.TimeOfDay > Parameters.MarketEndTime))
                return;

            dataAccess.WriteHistoricalData(e.RequestId, e.Date, e.Open, e.High, e.Low, e.Close, e.Volume);

            if (e.RecordNumber == e.RecordTotal - 1)
            {
                dataAccess.WriteHRequestCompleted(e.RequestId);
            }
        }
        #endregion

        #region Public Methods

        public async Task FillHistoricalData(int requestId, Contract c, DateTime endDatetime, string duration, BarSize b, HistoricalDataType type, int useRth)
        {
            ibclient.RequestHistoricalData(requestId, c, endDatetime, duration, b, type, useRth);
            MegBounceDataSet.HistoricalDataRequestsRow drR = dataAccess.GetHistoricalRequestById(requestId);

            while (drR.Completed == false && drR.HasError == false)
            {
                await Task.Delay(10);
            }
        }

        internal void PlaceOrder(int orderId, Contract contract, Order o)
        {
            //this.ibclient.PlaceOrder(orderId, contract, o);
        }

        #endregion

        #region Helper Methods

        #endregion
    }
}
