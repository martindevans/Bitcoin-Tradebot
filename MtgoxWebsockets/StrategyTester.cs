using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MtgoxWebsockets.Exchanges;

namespace MtgoxWebsockets
{
    public class StrategyTester
    {
        public readonly IEnumerable<IEnumerable<string>> TestData;
        public readonly Func<TradeStrategy>[] Strategies;

        public StrategyTester(IEnumerable<IEnumerable<string>> testDataSets, IEnumerable<Func<TradeStrategy>> strategies)
        {
            TestData = testDataSets;
            Strategies = strategies.ToArray();
        }

        public IEnumerable<KeyValuePair<decimal, Type>> Run(decimal startingBtc, decimal startingCurrency)
        {
            return Strategies
                .SelectMany(s => TestData.Select(d => new { Strategy = s(), Data = d.Select(a => ParseString(a)), Exchange = new FakeExchange(startingCurrency, startingBtc) }))
                .Select(a => new { Params = a, Manager = new TradeManager(a.Exchange, new Ticker[0].ToObservable(), new DepthTable.Entry[0].ToObservable(), a.Data.ToObservable(), new[] { a.Strategy }) })
                .Select(a => new { Currency = a.Params.Exchange.Btc * a.Params.Data.Last().Price + a.Params.Exchange.Currency, Strategy = a.Params.Strategy.GetType() })
                .OrderBy(a => a.Currency)
                .Select(a => new KeyValuePair<decimal, Type>(a.Currency, a.Strategy));
        }

        private Trade ParseString(string line)
        {
            var tokens = line.Split(' ').Select(a => a.ToLower()).ToArray();
            if (tokens[0] == "tr" && tokens.Length == 3)
                return new Trade(decimal.Parse(tokens[1]), decimal.Parse(tokens[2]), DateTime.Now.ToUniversalTime().Ticks);

            throw new ArgumentException("Invalid string");
        }
    }
}
