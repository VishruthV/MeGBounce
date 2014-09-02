using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Krs.Ats.IBNet;

namespace MeGBounce
{
    class Trader
    {
        private List<MeGSymbol> mySymbols = new List<MeGSymbol>();
        private HistoricalDataManager hdMgr = new HistoricalDataManager();
        private DataAccessLayer dataAccess = DataAccessLayer.GetMySingletonDataAccessLayer(); //Initializing only to have dezerialized data, rather than doing it at use-time! (runtime)
        private OrderManager orderMgr = null;
        private TwsConnector twsc = TwsConnector.GetMySingletonTwsConnector();

        public bool _PlcaeOrders = false;

        internal async Task Start()
        {
            this.mySymbols = ContractsManager.GetContractsFromSymbols(); //Invalid symbols need to be either removed or marked //TODO

            WaitTill(Parameters.FetchFirstCandleAt);

            //Todo: Should start the program BEFOR start time.. else there will be a phase shift.
            /*
             * If the pgm is started at 9:38, it will check the latest candle at 9:38 and then the next candle at 10:08.
             * To avoid that you should do a mod kinda operation. But should try to avoid relying on the Barsize, 
             * so that logic of cutting the phase shift works for all barsizes.
            //int nextMinute = -1;
            //int currentMinute = DateTime.Now.Minute;

            //if (currentMinute >= 30)
            //    nextMinute = 00;

            //else if (currentMinute < 30)
            // 
             * nextMinute = 30;
             */

            Log.Debug("Scanning for the first time");
            await Scan(); //Scan after the first candle is completed. (Manually called cuz Timer does the task after its interval)
            //todo test candles first and following

            System.Timers.Timer t = new System.Timers.Timer();
            t.Interval = 1000 * 60 * 30; //TODO: Parameter
            t.Enabled = true;
            t.Elapsed += t_Elapsed;
            t.Start();
        }

        void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Log.Debug("In the timer tick now");
            Scan();
        }

        private void WaitTill(DateTime time)
        {
            TimeSpan diff = time - DateTime.Now;
            Log.Debug(string.Format("Will wait {0} seconds", diff.TotalSeconds));

            if (diff.TotalMilliseconds > 0)
                System.Threading.Thread.Sleep(diff.Add(new TimeSpan(0,0,(int)twsc.TimeDiff)));
        }

        private async Task Scan()
        {
            List<Task> allRets = new List<Task>();
            foreach (MeGSymbol sym in mySymbols)
            {
                Log.Debug(string.Format("Calling DoScanAsync({0})", sym.Contract.Symbol));
                allRets.Add(DoScanAsync(sym));
            }

            Task.WaitAll(allRets.ToArray());
            Log.Debug("All Tasks returned in Trader.Scan");
        }

        private async Task DoScanAsync(MeGSymbol sym)
        {
            Log.Info(string.Format("Fetching Historical data for {0} - (TWSTime: {1})", sym.Contract.Symbol,twsc.TWSDateTime));
            await hdMgr.SyncHistoricalData(sym.Contract);
            Log.Info(string.Format("Finished fetching Historical data for {0}", sym.Contract.Symbol));

            bool isDataEnough = dataAccess.TryGetLatestNCandles(sym.Contract.Symbol, 15); //TODO: Parameter?

            if (isDataEnough)
            {
                Log.Info(string.Format("Checking Signal for {0}", sym.Contract.Symbol));
                Signal s = new Strategy().CheckSignal(sym.Contract);
                Log.Info(string.Format("Finished Checking Signal for {0}", sym.Contract.Symbol));
                Log.Info(string.Format("Signal: {0} - Symbol: {1}", s.SignalType.ToString(), sym.Contract.Symbol));

                if (s.SignalType != SignalType.NoSignal)
                {
                    if (orderMgr == null) orderMgr = new OrderManager();

                    if(_PlcaeOrders)
                    orderMgr.PlaceOrder(s);
                }
            }
            else
            {
                Log.Info(string.Format("Not enough data for {0} to continue Scanning ", sym.Contract.Symbol));
            }
        }
    }
}
