using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeGBounce
{
    class EventWiring
    {
        public delegate void HDataCompleted_EventHandler(object sender, HDataCompletedEventArgs e);
        public class HDataCompletedEventArgs : EventArgs
        {
            public int RequestId { get; set; }
        }
    }
}
