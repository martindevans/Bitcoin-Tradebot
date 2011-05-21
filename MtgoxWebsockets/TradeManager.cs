using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MtgoxWebsockets.Exchanges;

namespace MtgoxWebsockets
{
    public class TradeManager
        :IEnumerable<TradeStrategy>
    {
        public readonly IExchange Exchange;

        HashSet<TradeStrategy> strategies = new HashSet<TradeStrategy>();

        public readonly IObservable<Ticker> Ticker;
        public readonly IObservable<DepthTable.Entry> Depth;
        public readonly IObservable<Trade> Trades;

        public readonly DepthTable DepthTable;

        #region constructors
        public TradeManager(IExchange exchange, IObservable<Ticker> ticker, IObservable<DepthTable.Entry> depth, IObservable<Trade> trades, params TradeStrategy[] strategies)
            :this(exchange, ticker, depth, trades, strategies as IEnumerable<TradeStrategy>)
        {
        }

        public TradeManager(IExchange exchange, IObservable<Ticker> ticker, IObservable<DepthTable.Entry> depth, IObservable<Trade> trades, IEnumerable<TradeStrategy> strategies)
        {
            this.Ticker = ticker;
            this.Depth = depth;
            this.Trades = trades;
            this.Exchange = exchange;

            this.DepthTable = new DepthTable(depth);

            foreach (var item in strategies)
                Add(item);
        }
        #endregion

        bool Add(TradeStrategy strategy)
        {
            strategy.SubscribeFeeds(this, Ticker, Depth, Trades);

            return strategies.Add(strategy);
        }

        bool Remove(TradeStrategy strategy)
        {
            if (strategies.Remove(strategy))
            {
                strategy.Unsubscribe();
                return true;
            }

            return false;
        }

        #region IEnumerable
        public IEnumerator<TradeStrategy> GetEnumerator()
        {
            return strategies.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return strategies.GetEnumerator();
        }
        #endregion
    }
}
