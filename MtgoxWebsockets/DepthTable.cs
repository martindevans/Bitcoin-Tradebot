using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using HandyCollections.Heap;
using System.IO;
using System.Security.AccessControl;

namespace MtgoxWebsockets
{
    public class DepthTable
    {
        private MinMaxHeap<Entry> buys = new MinMaxHeap<Entry>();
        private MinMaxHeap<Entry> sells = new MinMaxHeap<Entry>();

        public Entry? MaxBuy
        {
            get
            {
                if (buys.Count == 0)
                    return null;
                return buys.Maximum;
            }
        }
        public Entry? MinSell
        {
            get
            {
                if (sells.Count == 0)
                    return null;
                return sells.Minimum;
            }
        }

        public IEnumerable<Entry> Buys
        {
            get
            {
                return buys;
            }
        }
        public IEnumerable<Entry> Sells
        {
            get
            {
                return sells;
            }
        }

        public int BuyCount
        {
            get
            {
                return buys.Count;
            }
        }
        public int SellCount
        {
            get
            {
                return sells.Count;
            }
        }

        public bool AutoSaveCsv = false;
        public bool AutoIncrementLog = false;
        int log = 0;

        public DepthTable(IObservable<Entry> dataStream)
        {
            dataStream.Subscribe(DataArrived, Error);
        }

        private void DataArrived(Entry a)
        {
            if (a.Volume < 0)
                RemoveEntry(new Entry(a.Buy, a.Price, -a.Volume));
            else
                AddEntry(a);

            if (AutoSaveCsv)
            {
                using (var file = new StreamWriter(File.Create("Depth" + (AutoIncrementLog ? (++log).ToString() : "") + ".csv", 1 + buys.Count * 32 + sells.Count * 32, FileOptions.SequentialScan)))
                    SaveAsCsv(file);
            }
        }

        private void Error(Exception e)
        {
            Console.WriteLine("Depth Exception");
            Console.WriteLine(e);
        }

        private void SaveAsCsv(StreamWriter file)
        {
            file.WriteLine("Buy Volume,Buy Price,Sell Volume,Sell Price");

            var b = (buys as IEnumerable<Entry>).GetEnumerator();
            var s = (sells as IEnumerable<Entry>).GetEnumerator();

            StringBuilder line = new StringBuilder();


            bool moreBuys;
            bool moreSells;
            do
            {
                moreBuys = b.MoveNext();
                moreSells = s.MoveNext();
                line.Clear();

                if (moreBuys) line.Append(b.Current.Volume);
                line.Append(",");
                if (moreBuys) line.Append(b.Current.Price);
                line.Append(",");

                if (moreSells) line.Append(s.Current.Volume);
                line.Append(",");
                if (moreSells) line.Append(s.Current.Price);
                line.Append(",");

                file.WriteLine(line.ToString());
            } while (moreBuys || moreSells);
        }

        private bool RemoveEntry(Entry a)
        {
            MinMaxHeap<Entry> entries = a.Buy ? buys : sells;

            return entries.Remove(a);
        }

        private void AddEntry(Entry a)
        {
            if (a.Buy)
                buys.Add(a);
            else
                sells.Add(a);
        }

        public struct Entry
            :IComparable<Entry>
        {
            public readonly bool Buy;
            public readonly decimal Price;
            public readonly decimal Volume;

            public Entry(JObject data)
            {
                Price = decimal.Parse(data["price"].ToString());
                Volume = decimal.Parse(data["volume"].ToString());

                byte type = byte.Parse(data["type"].ToString());
                if (type < 1 || type > 2)
                    throw new ArgumentOutOfRangeException("Type must be 1 or 2");

                Buy = type == 2;
            }

            public Entry(bool buy, decimal price, decimal volume)
            {
                Buy = buy;
                Price = price;
                Volume = volume;
            }

            public override bool Equals(object obj)
            {
                if (obj is Entry)
                    return Equals((Entry)obj);

                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return Buy.GetHashCode() ^ Price.GetHashCode() ^ Volume.GetHashCode();
            }

            public bool Equals(Entry other)
            {
                return other.Buy == Buy && other.Volume == Volume && other.Price == Price;
            }

            public int CompareTo(Entry other)
            {
                return Price.CompareTo(other.Price);
            }

            public override string ToString()
            {
                return "{ " + (Buy ? "buying " : "selling ") + Volume + "btc@$" + Price + " }";
            }
        }
    }
}
