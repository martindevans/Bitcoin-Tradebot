using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MtgoxWebsockets.Exchanges;

namespace MtgoxWebsockets
{
    public class StrategyTester
    {
        public readonly IEnumerable<Func<KeyValuePair<TradeStrategy, string>>> Strategies;

        public StrategyTester(IEnumerable<Func<KeyValuePair<TradeStrategy, string>>> strategies)
        {
            Strategies = strategies.ToArray();
        }

        public IEnumerable<Tuple<decimal, Type, String>> Run(IEnumerable<Trade> trades, decimal startingBtc, decimal startingCurrency)
        {
            return Strategies
                .Select(s => new { Strategy = s(), Data = trades, Exchange = new FakeExchange(startingCurrency, startingBtc) })
                .Select(a => new { Params = a, Manager = new TradeManager(a.Exchange, new Ticker[0].ToObservable(), new DepthTable.Entry[0].ToObservable(), a.Data.ToObservable(), new[] { a.Strategy.Key }) })
                .Select(a => new { Currency = a.Params.Exchange.Btc * a.Params.Data.Last().Price + a.Params.Exchange.Currency, Strategy = a.Params.Strategy.Key.GetType(), Comment = a.Params.Strategy.Value })
                .OrderBy(a => a.Currency)
                .Select(a => new Tuple<decimal, Type, String>(a.Currency, a.Strategy, a.Comment))
                .Reverse();
        }
    }
}
