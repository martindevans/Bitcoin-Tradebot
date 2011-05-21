using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MtgoxWebsockets
{
    public abstract class TradeStrategy
    {
        public TradeManager Manager
        {
            get;
            private set;
        }

        ISet<IDisposable> subscriptions = new HashSet<IDisposable>();

        internal void SubscribeFeeds(TradeManager tradeManager, IObservable<Ticker> ticker, IObservable<DepthTable.Entry> depth, IObservable<Trade> trades)
        {
            if (Manager != null)
                throw new InvalidOperationException("Cannot subscribe to two managers");
            Manager = tradeManager;

            subscriptions.UnionWith(Subscribe(tradeManager, ticker, depth, trades));
        }

        protected abstract IEnumerable<IDisposable> Subscribe(TradeManager tradeManager, IObservable<Ticker> ticker, IObservable<DepthTable.Entry> depth, IObservable<Trade> trades);

        internal void Unsubscribe()
        {
            foreach (var s in subscriptions)
                s.Dispose();
            subscriptions.Clear();

            Manager = null;
        }
    }
}
