﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperWebSocket.Client;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using MtgoxWebsockets.Data;
using MtgoxWebsockets.Exchanges;
using MtgoxWebsockets.Strategies;
using System.IO;

namespace MtgoxWebsockets
{
    class Program
    {
        static void Main(string[] args)
        {
            //ListenToMtgox();
            RunFakeTests();
        }

        private static void RunFakeTests()
        {
            var data = new string[]
            {
                "Training/0.t"
            }.Select(a => File.ReadAllLines(a));

            StrategyTester t = new StrategyTester(data, new Func<TradeStrategy>[]
            {
                //Add new type of strategy to this array as a factory method to construct the strategy
                () => new Stupid()
            });

            var result = t.Run(0, 10).ToArray();

            Console.Clear();

            foreach (var item in result)
            {
                Console.WriteLine(item.Value.Name + " with $" + item.Key);
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }

        private static void ListenToMtgox()
        {
            var ws = new WebSocket("ws://" + Dns.GetHostAddresses("websocket.mtgox.com").First().ToString() + ":80/mtgox", "*");
            var observableWs = new ObservableWebsocket(ws);

            if (!ws.Connect())
            {
                Console.WriteLine("Not connected");
                Console.ReadLine();
            }
            else
            {
                Console.Clear();
                Connected(observableWs);
                ws.Close();
            }
        }

        private static void Connected(IObservable<string> data)
        {
            ISubject<Trade> fakeTrades = new Subject<Trade>();
            ISubject<DepthTable.Entry> fakeDepth = new Subject<DepthTable.Entry>();
            ISubject<Ticker> fakeTicker = new Subject<Ticker>();

            FakeExchange exchange = new FakeExchange(10, 0);
            exchange.Subscribe(a => Console.Title = a.ToString());

            IObservable<Ticker> ticker =
                //fakeTicker;
                data.AsJson().AsMtgoxTicker();
            IObservable<DepthTable.Entry> depth =
                //fakeDepth;
                data.AsJson().AsMtgoxDepth();
            IObservable<Trade> trade = 
                fakeTrades;
                //data.AsJson().AsMtgoxTrades();

            TradeManager manager = new TradeManager(exchange, ticker, depth, trade, new Stupid());

            //var tradelog = new StreamWriter(File.Open("trades.t", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read));
            //manager.Trades.Subscribe(a => tradelog.WriteLine(String.Format("tr {0} {1}", a.Volume, a.Price)));

            manager.Trades.Subscribe(a => Console.WriteLine(String.Format("Mtgox Traded {0} BTC for {1} USD", a.Volume, a.Price)));
            //manager.Ticker.Subscribe(a => Console.Title = a.ToString());

            var trFile = new StreamReader("Training/0.t");

            while (true)
            {
                Console.WriteLine("Enter a line to inject fake data");
                Console.WriteLine("tr {volume} {price}");

                var line = Console.ReadLine();
                line = trFile.ReadLine();

                var tokens = line.Split(' ').Select(a => a.ToLower()).ToArray();
                if (tokens[0] == "tr" && tokens.Length == 3)
                {
                    fakeTrades.OnNext(new Trade(decimal.Parse(tokens[1]), decimal.Parse(tokens[2]), DateTime.Now.ToUniversalTime().Ticks));
                }
            }

            Console.WriteLine("Press any key to exit");
            Console.Read();

            //tradelog.Flush();
            //tradelog.Close();
        }
    }
}
