using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MtgoxWebsockets.Exchanges
{
    public class FakeExchange
        : BehaviorSubject<Balance>, IExchange
    {
        public decimal Currency
        {
            get { return this.Latest().First().Currency; }
            set { OnNext(new Balance(value, Btc)); }
        }

        public decimal Btc
        {
            get { return this.Latest().First().Btc; }
            set { OnNext(new Balance(Currency, value)); }
        }

        #region constructors
        public FakeExchange(decimal initialCurrency, decimal initialBtc)
            :base(new Balance(initialCurrency, initialBtc))
        {
        }
        #endregion

        public IEnumerable<MarketOrder> Buy(decimal btc, decimal currency)
        {
            Btc += btc;
            Currency -= currency * btc;

            return PendingOrders();
        }

        public IEnumerable<MarketOrder> Sell(decimal btc, decimal currency)
        {
            Btc -= btc;
            Currency += currency * btc;

            return PendingOrders();
        }

        public IEnumerable<MarketOrder> PendingOrders()
        {
            return new MarketOrder[0];
        }

        public void CancelOrder(long orderId, bool sell)
        {
        }

        public Balance GetBalance()
        {
            return this.Latest().First();
        }
    }
}
