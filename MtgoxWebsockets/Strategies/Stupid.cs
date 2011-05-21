using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MtgoxWebsockets.Exchanges;

namespace MtgoxWebsockets.Strategies
{
    public class Stupid
        :TradeStrategy
    {
        Dictionary<decimal, decimal> wallet = new Dictionary<decimal, decimal>();

        protected override IEnumerable<IDisposable> Subscribe(TradeManager tradeManager, IObservable<Ticker> ticker, IObservable<DepthTable.Entry> depth, IObservable<Trade> trades)
        {
            wallet[0] = tradeManager.Exchange.GetBalance().Btc;

            IObservable<decimal> slidingVolumeWeightedAveragePrice = trades
                .SlidingWindow(5)
                .Select(a => a.Sum(t => t.Price * t.Volume) / a.Sum(t => t.Volume));

            yield return trades.Subscribe(OnTrade(slidingVolumeWeightedAveragePrice));
        }

        private Action<Trade> OnTrade(IObservable<decimal> slidingVolumeWeightedAveragePrice)
        {
            bool up = false;
            bool down = false;
            decimal vwap = 0;
            slidingVolumeWeightedAveragePrice.Subscribe(a => 
                {
                    up = a > vwap;
                    down = a < vwap;
                    vwap = a;
                });

            return a =>
                {
                    IExchange exchange = Manager.Exchange;
                    Balance balance = exchange.GetBalance();

                    if (up && balance.Currency > 0)
                    {
                        decimal btcAmount = balance.Currency / a.Price;
                        exchange.Buy(btcAmount, a.Price);

                        if (wallet.ContainsKey(a.Price))
                            wallet[a.Price] += btcAmount;
                        else
                            wallet[a.Price] = btcAmount;

                        Console.WriteLine(string.Format("Buy {0} for {1}", btcAmount, a.Price));
                    }
                    else if (down)
                    {
                        foreach (var key in wallet.Keys.Where(k => k < a.Price).ToArray())
                        {
                            decimal btc = wallet[key];
                            wallet.Remove(key);
                            exchange.Sell(btc, a.Price);

                            Console.WriteLine(string.Format("Sell {0} for {1}", btc, a.Price));
                        }
                    }
                };
        }
    }
}
