using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace MtgoxWebsockets.Data
{
    public static class MtgoxJSON
    {
        public const string TRADES_CHANNEL = "dbf1dee9-4f2e-4a08-8cb7-748919a71b21";
        public const string TICKER_CHANNEL = "d5f06780-30a8-4a48-a2f8-7ed181b4a13f";
        public const string DEPTH_CHANNEL = "24e67e0d-1cad-4cc0-9e7a-f8523ef460fe";

        public static IObservable<DepthTable.Entry> AsMtgoxDepth(this IObservable<JObject> jsonFeed)
        {
            return jsonFeed.FilterByKeyValue("channel", DEPTH_CHANNEL).FilterByKeyValue("op", "private").Select(a => new DepthTable.Entry((JObject)a["depth"]));
        }

        public static IObservable<Trade> AsMtgoxTrades(this IObservable<JObject> jsonFeed)
        {
            return jsonFeed.FilterByKeyValue("channel", TRADES_CHANNEL).FilterByKeyValue("op", "private").Select(a => new Trade((JObject)a["trade"]));
        }

        public static IObservable<Ticker> AsMtgoxTicker(this IObservable<JObject> jsonFeed)
        {
            return jsonFeed.FilterByKeyValue("channel", TICKER_CHANNEL).FilterByKeyValue("op", "private").Select(a => new Ticker((JObject)a["ticker"]));
        }
    }
}
