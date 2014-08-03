using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeGBounce
{
    class Strategy
    {
        //todo:
        /* Min no of hanging cdls to be added.
         * 3. Calculate boll for prev ones
         */
        DataAccessLayer dataAccess = DataAccessLayer.GetMySingletonDataAccessLayer();

        public Signal CheckSignal(Krs.Ats.IBNet.Contract c)
        {
            List<CandleData> reqdDataForCaln = dataAccess.GetLatestNCandlesLatestFirst(c.Symbol, 15); //TODO: Parameter?
            MeGBounceIndicators ind = CalulateIndicators(c, reqdDataForCaln);
            CandleData latestCandle = reqdDataForCaln[0];

            Log.Debug("CheckSignal Debug data");
            string logMsg = string.Format("Symbol: {0}. CandleStartTime: {1}. BL: {2} BU: {3}", c.Symbol, reqdDataForCaln[0].CandleStartDateTime.ToString(), ind.BollingerLower, ind.BollingerUpper);
            Log.Debug(logMsg);

            Signal ret = new Signal();

            decimal lengthOfTheCandle = latestCandle.High - latestCandle.Low;
            decimal lengthOfTheBody = Math.Abs((latestCandle.Open - latestCandle.Close));
            decimal lengthOfTheTail = (latestCandle.Open < latestCandle.Close ? latestCandle.Open : latestCandle.Close) - latestCandle.Low;
            decimal lengthOfTheHead = latestCandle.High - (latestCandle.Open > latestCandle.Close ? latestCandle.Open : latestCandle.Close);

            /*
             * Condition 1: Close is above Bol Lower
             * Condition 2: Low is below Bol Lower
             * Condition 3: LC should be greater than or equal to Close * x% of close AND LC Less than or equal to Close * y%
             * Condition 4: LT >= a % of LC
             * Condition 5: LB <= a % of LC
             * Condition 6: Lt of tail below BLow >= a % of LT
             */

            bool condition3Common = (lengthOfTheCandle >= (latestCandle.Close * Parameters.PctMinLC)) && (lengthOfTheCandle <= (latestCandle.Close * Parameters.PctMaxLC));
            bool condition4Common = (lengthOfTheTail >= (lengthOfTheCandle * Parameters.PctMinLT));
            bool condition5Common = (lengthOfTheBody <= (lengthOfTheCandle * Parameters.PctMaxLB));



            #region Long Signal
            decimal lengthOfTailBelowBLow = ind.BollingerLower - latestCandle.Low;

            bool condition1Long = latestCandle.Close > ind.BollingerLower;
            bool condition2Long = latestCandle.Low < ind.BollingerLower;
            bool condition6Long = (lengthOfTailBelowBLow >= (lengthOfTheTail * Parameters.PctMinLTBeyondBollingerBand));

            if (condition1Long && condition2Long && condition3Common && condition4Common && condition5Common && condition6Long)
            {
                ret = new Signal { Contract = c, SignalDateTime = DateTime.Now, SignalType = SignalType.Long, ProfitTarget = ind.MovingAverage, StopLoss = latestCandle.Low };
            }

            Log.Debug(string.Format("Long: {0} 1: {1} 2: {2} 3: {3} 4: {4} 5: {5} 6: {6} - Symbol:{7}", Environment.NewLine, condition1Long, condition2Long, condition3Common, condition4Common, condition5Common, condition6Long,c.Symbol));
            #endregion

            #region Short Signal
            decimal lengthOfHeadAboveBUpper = ind.BollingerLower - latestCandle.Low;

            bool condition1Short = latestCandle.Close < ind.BollingerUpper;
            bool condition2Short = latestCandle.High > ind.BollingerUpper;
            bool condition6Short = (lengthOfHeadAboveBUpper >= (lengthOfTheTail * Parameters.PctMinLTBeyondBollingerBand));

            if (condition1Short && condition2Short && condition3Common && condition4Common && condition5Common && condition6Short)
            {
                ret = new Signal { Contract = c, SignalDateTime = DateTime.Now, SignalType = SignalType.Short, ProfitTarget = ind.MovingAverage, StopLoss = latestCandle.High };
            }

            Log.Debug(string.Format("Short: {0} 1: {1} 2: {2} 3: {3} 4: {4} 5: {5} 6: {6} - Symbol: {7}", Environment.NewLine, condition1Short, condition2Short, condition3Common, condition4Common, condition5Common, condition6Short, c.Symbol));
            #endregion

            #region No Signal
            ret = new Signal { SignalType = SignalType.NoSignal };
            #endregion

            return ret;
        }

        private MeGBounceIndicators CalulateIndicators(Krs.Ats.IBNet.Contract c, List<CandleData> reqdDataForCaln)
        {
            decimal[] closeValues = (from a in reqdDataForCaln
                                     orderby a.CandleStartDateTime ascending
                                     select a.Close).ToArray<decimal>();

            CalcIndicators.Bollinger boll = new CalcIndicators.Bollinger();
            boll.CalculateBollinger(closeValues, 15, 2); //todo Parameter?
            decimal ma = Enumerable.Average(closeValues);
            //TODO: why are boll and MA values not matching?

            return new MeGBounceIndicators { BollingerLower = decimal.Round(boll.BbLower, 3), BollingerUpper = decimal.Round(boll.BbUpper, 3), MovingAverage = decimal.Round(ma, 3) };
        }
    }
}