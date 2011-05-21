using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace MtgoxWebsockets.Exchanges
{
    public interface IExchange
    {
        IEnumerable<MarketOrder> Buy(decimal btc, decimal currency);

        IEnumerable<MarketOrder> Sell(decimal btc, decimal currency);

        IEnumerable<MarketOrder> PendingOrders();

        void CancelOrder(long orderId, bool sell);

        Balance GetBalance();
    }

    public struct Balance
    {
        public readonly decimal Btc;
        public readonly decimal Currency;

        public Balance(decimal currency, decimal btc)
        {
            Btc = btc;
            Currency = currency;
        }

        public override string ToString()
        {
            return String.Format("Balance Btc={0} Cur={1}", Btc, Currency);
        }

        public override bool Equals(object obj)
        {
            if (obj is Balance)
                return Equals((Balance)obj);
            return base.Equals(obj);
        }

        public bool Equals(Balance other)
        {
            return other.Btc.Equals(Btc) && other.Currency.Equals(Currency);
        }

        public override int GetHashCode()
        {
            return Btc.GetHashCode() ^ Currency.GetHashCode();
        }
    }
}
