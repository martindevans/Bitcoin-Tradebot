using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace MtgoxWebsockets
{
    public struct MarketOrder
    {
        public readonly long OrderId;
        public readonly bool IsSell;
        public readonly decimal Volume;
        public readonly decimal Price;
        public readonly long Date;
        public readonly bool IsActive;

        #region constructors
        public MarketOrder(long oid, bool sell, decimal volume, decimal price, bool active, long date)
        {
            OrderId = oid;
            IsSell = sell;
            Volume = volume;
            Price = price;
            IsActive = active;
            Date = date;
        }

        public MarketOrder(JObject o)
            :this(long.Parse(o["oid"].ToString()),
                ParseSell(o),
                decimal.Parse(o["amount"].ToString()),
                decimal.Parse(o["price"].ToString()),
                ParseStatus(o),
                long.Parse(o["date"].ToString())
            )
        {
        }
        #endregion

        private static bool ParseSell(JObject o)
        {
            int sellValue = int.Parse(o["type"].ToString());
            if (sellValue < 1 || sellValue > 2)
                throw new ArgumentOutOfRangeException("type must be 1 or 2");
            return sellValue == 1;
        }

        private static bool ParseStatus(JObject o)
        {
            int statusValue = int.Parse(o["status"].ToString());
            if (statusValue < 1 || statusValue > 2)
                throw new ArgumentOutOfRangeException("status must be 1 or 2");
            return statusValue == 1;
        }
    }
}
