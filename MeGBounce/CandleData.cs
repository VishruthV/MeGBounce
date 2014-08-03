using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeGBounce
{
    public struct CandleData
    {
        public string Symbol;
        public DateTime CandleStartDateTime;
        public decimal Open;
        public decimal High;
        public decimal Low;
        public decimal Close;
        public int Volume;
    }
}
