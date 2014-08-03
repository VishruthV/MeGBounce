using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeGBounce
{
    //There must only be a single object. Or if instanciated at multiple places, use Singleton
    class HistoricalDataManager
    {
        int RequestId = 10000;//Order ID will be IB controlled. So ctrl this ID
        DataAccessLayer dataAccess = null;
        object lockForPacingViolation = new object();

        public HistoricalDataManager()
        {
            dataAccess = DataAccessLayer.GetMySingletonDataAccessLayer();
        }

        //Completed? Sd return from TWS only after Complete or error.. How to do it?
        //Has Errors? How t ihandle it?
        //Has data gaps or no data? How to get it?

        public async Task SyncHistoricalData(Krs.Ats.IBNet.Contract c)
        {
            Task ret = null;
            UInt16 test = 0;

            lock (lockForPacingViolation)
            {
                TwsConnector twsc = TwsConnector.GetMySingletonTwsConnector();

                Krs.Ats.IBNet.BarSize bSize = Krs.Ats.IBNet.BarSize.ThirtyMinutes; //TODO: Parameter?
                TimeSpan barSize = new TimeSpan(0, 30, 0); //TODO: Parameter?

                string duration = "3 D";
                int reqId = GetRequestID();
                DateTime reqCandleEndDateTime = DateTime.Now;

                dataAccess.WriteHDataRequest(reqId, c.Symbol, barSize);
                ret = twsc.FillHistoricalData(reqId, c, reqCandleEndDateTime, duration, bSize, Krs.Ats.IBNet.HistoricalDataType.Trades, 0);
                test++;
                System.Threading.Thread.Sleep(1000);//To avoid pacing violation error, 2s wait
            }
            System.Diagnostics.Debug.Assert(test != 0, "Test = 0. Has reached this point without going into lock!");
            await ret; 
        }

        private int GetRequestID()
        {
            this.RequestId = System.Threading.Interlocked.Increment(ref RequestId);
            return this.RequestId;
        }
    }
}
