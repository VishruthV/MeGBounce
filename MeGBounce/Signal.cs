using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MeGBounce
{
    class Signal
    {
        private static int MySignalId = 0;

        public int SignalId;
        public Krs.Ats.IBNet.Contract Contract;
        public DateTime SignalDateTime;
        public SignalType SignalType;
        public decimal ProfitTarget;
        public decimal StopLoss;

        public Signal()
        {
            MySignalId++;
            this.SignalId = MySignalId;
        }
    }

    public enum SignalType
    {
        Long,
        Short,
        NoSignal
    }
}
