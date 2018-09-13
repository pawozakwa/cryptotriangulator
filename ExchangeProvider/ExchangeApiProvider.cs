using System;
using System.Collections.Generic;
using System.IO;
using ExchangeSharp;

namespace ExchangeProvider
{
    public class ExchangeApiProvider
    {
        public Dictionary<Exchange, ExchangeAPI> Apis;

        public ExchangeApiProvider()
        {
            Console.Write("Initialization of apis...");
            Apis = new Dictionary<Exchange, ExchangeAPI>()
            {
                { Exchange.Abucoins, new ExchangeAbucoinsAPI() },
                { Exchange.Binance, new ExchangeBinanceAPI() },
                { Exchange.Bitfinex, new ExchangeBitfinexAPI() },
                { Exchange.Bithumb, new ExchangeBithumbAPI() },
                { Exchange.Bitmex, new ExchangeBitMEXAPI() },
                { Exchange.Bitstamp, new ExchangeBitstampAPI() },
                { Exchange.Bittrex, new ExchangeBittrexAPI() },
                { Exchange.Bleutrade, new ExchangeBleutradeAPI() },
                { Exchange.Coinbase, new ExchangeCoinbaseAPI() },
                { Exchange.Cryptopia, new ExchangeCryptopiaAPI() },
                { Exchange.Gemini, new ExchangeGeminiAPI() },
                { Exchange.Hitbtc, new ExchangeHitbtcAPI() },
                { Exchange.Huobi, new ExchangeHuobiAPI() },
                { Exchange.Kraken, new ExchangeKrakenAPI() },
                { Exchange.Kucoin, new ExchangeKucoinAPI() },
                { Exchange.Livecoin, new ExchangeLivecoinAPI() },
                { Exchange.Okex, new ExchangeOkexAPI() },
                { Exchange.Poloniex, new ExchangePoloniexAPI() },
                { Exchange.TuxExchange, new ExchangeTuxExchangeAPI() },
                { Exchange.Yobit, new ExchangeYobitAPI() }
            };
            Console.WriteLine("   => Done!");
        }

        public ExchangeAPI GetApi(Exchange exchange) => Apis[exchange];

        public IEnumerable<ExchangeAPI> GetAllApis => Apis.Values;        
    }
}
