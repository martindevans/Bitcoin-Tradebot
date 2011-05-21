using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace MtgoxWebsockets
{
    public struct Ticker
    {
        decimal Buy;
        decimal High;
        decimal Low;
        decimal Sell;
        decimal Volume;

        public Ticker(decimal buy, decimal high, decimal low, decimal sell, decimal volume)
        {
            Buy = buy;
            High = high;
            Low = low;
            Sell = sell;
            Volume = volume;
        }

        public Ticker(JObject o)
            :this(decimal.Parse(o["buy"].ToString()),
                decimal.Parse(o["high"].ToString()),
                decimal.Parse(o["low"].ToString()),
                decimal.Parse(o["sell"].ToString()),
                decimal.Parse(o["vol"].ToString()))
        {
        }

        public override string ToString()
        {
            return String.Format("Buy {0}, Sell {1}, High {2}, Low {3}, Volume {4}", Buy, Sell, High, Low, Volume);
        }
    }
}
