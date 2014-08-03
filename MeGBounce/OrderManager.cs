using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeGBounce
{
    public class OrderManager
    {
        private TwsConnector twsc = TwsConnector.GetMySingletonTwsConnector();
        private DataAccessLayer dataAccess = DataAccessLayer.GetMySingletonDataAccessLayer();

        public OrderManager()
        {
            twsc.MySharedOrderStatusEvent += twsc_MySharedOrderStatusEvent;
        }

        internal void PlaceOrder(Signal signal)
        {
            int noOfContractsToTrade = GetNumberOfContractsToTrade(signal.Contract);
            Krs.Ats.IBNet.Order order = new Krs.Ats.IBNet.Order();
            int orderId = twsc.OrderId;
            order.OrderId = orderId;
            order.Action = Krs.Ats.IBNet.ActionSide.Buy;
            order.OrderRef = "MegBounce Opening Order";
            order.OrderType = Krs.Ats.IBNet.OrderType.Market;
            order.TotalQuantity = noOfContractsToTrade;

            dataAccess.WriteNewOrderEntry(orderId, signal.Contract.FullyQualifiedSymbol());

            //twsc.PlaceOrder(orderId, signal.Contract, order);
            
            /*
             * GetNumberOfContractsToTrade
             * Unique OrderID Logic
             * Place one opening market order of the contract C and with a OCA type/group place 1 SL order and 1 PT order
             * Update Orders table
             * Subscribe to TWS orders status event
             * Logic to update UI
             */
        }

        private int GetNumberOfContractsToTrade(Krs.Ats.IBNet.Contract contract)
        {
            return 1; //todo: Add Margin and NLV logic later
        }
        
        void twsc_MySharedOrderStatusEvent(object sender, Krs.Ats.IBNet.OrderStatusEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
