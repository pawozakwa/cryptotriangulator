using System;
using System.Collections.Generic;
using System.IO;
using ExchangeSharp;

namespace ExchangeProvider
{
    public static class Sandbox
    {
        public async static void Play()
        {
            var provider = new ExchangeApiProvider();
            
            var s1 = "DASH";
            var s1m = "dash";
            var s2 = "BTC";
            var s2m = "btc";

            var tickers = new List<ExchangeTicker>();

            var file = File.CreateText("ticks.txt");
            
            foreach(var KV in provider.Apis)
            {
                try
                {
                    Console.WriteLine("Exchange: " + KV.Value.Name);

                    foreach (var item in await KV.Value.GetMarketSymbolsAsync())
                    {
                        file.WriteLine(item);
                        var separator = KV.Value.MarketSymbolSeparator;
                        if ((item.StartsWith(s1) && item.EndsWith(s2)) ||
                            (item.StartsWith(s1m) && item.EndsWith(s2m)))
                        {
                            var name = item;
                            if (separator != "")
                                name = item.Replace(KV.Value.MarketSymbolSeparator, "");

                            Console.WriteLine(name);

                            var ticker = await KV.Value.GetTickerAsync(item);
                            tickers.Add(ticker);

                            Console.WriteLine(ticker);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                Console.WriteLine();
            }
            file.Flush();

            Console.WriteLine();
            Console.WriteLine("Options to hit:");

            var diffs = new List<decimal>();

            foreach(ExchangeTicker ticker1 in tickers)
            {
                foreach (ExchangeTicker ticker2 in tickers)
                {
                    if (ticker1.Equals(ticker2)) continue;

                    if (ticker1.Bid > ticker2.Ask && ticker1.Ask != 0 && ticker2.Bid != 0)
                    {
                        Console.WriteLine($"Ask:{ticker1.Ask}  Bid:{ticker2.Bid}");
                        diffs.Add(ticker1.Bid - ticker2.Ask);
                    } 
                }
            }

            diffs.Sort();

            foreach (var diff in diffs)
            {
                Console.WriteLine(diff);
            }
        }
    }
}
