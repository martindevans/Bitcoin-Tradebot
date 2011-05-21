using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace MtgoxWebsockets
{
    public struct Trade
    {
        public decimal Volume;
        public decimal Price;
        public long Date;

        public Trade(decimal volume, decimal price, long date)
        {
            Volume = volume;
            Price = price;
            Date = date;
        }

        public Trade(JObject o)
            :this(decimal.Parse(o["amount"].ToString()),
                decimal.Parse(o["price"].ToString()),
                long.Parse(o["date"].ToString()))
        {
        }
    }
}
