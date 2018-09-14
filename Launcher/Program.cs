using ExchangeProvider;
using TraceMapper;
using ExchangeSharp;
using System;

namespace Launcher
{
    class Program
    {
        static void Main(string[] args)
        {
            ShowLogo();

            var apiProvider = new ExchangeApiProvider();

            ExchangeAPI[] exchangeAPIs = new[] {
                apiProvider.GetApi(Exchange.Bitfinex)
            };

            var crawler = new NetworkCrawler(exchangeAPIs);

            crawler.InitializeNetwork().GetAwaiter().GetResult();
            
            crawler.IterativeFindTraceWithProfit(3);
            crawler.ShowBestFoundedTrace();

            Console.ReadLine();
        }
        
        private static void ShowLogo()
        {

            Console.ForegroundColor = ConsoleColor.DarkMagenta;

            Console.WriteLine(@"**********************************************************************************************************");
            Console.WriteLine(@" ________          __                                          __             __                          ");           
            Console.WriteLine(@"|        \        |  \                                        |  \           |  \                         ");
            Console.WriteLine(@" \$$$$$$$$______   \$$  ______   _______    ______   __    __ | $$  ______  _| $$_     ______    ______   ");
            Console.WriteLine(@"   | $$  /      \ |  \ |      \ |       \  /      \ |  \  |  \| $$ |      \|   $$ \   /      \  /      \  ");
            Console.WriteLine(@"   | $$ |  $$$$$$\| $$  \$$$$$$\| $$$$$$$\|  $$$$$$\| $$  | $$| $$  \$$$$$$\\$$$$$$  |  $$$$$$\|  $$$$$$\ ");
            Console.WriteLine(@"   | $$ | $$   \$$| $$ /      $$| $$  | $$| $$  | $$| $$  | $$| $$ /      $$ | $$ __ | $$  | $$| $$   \$$ ");
            Console.WriteLine(@"   | $$ | $$      | $$|  $$$$$$$| $$  | $$| $$__| $$| $$__/ $$| $$|  $$$$$$$ | $$|  \| $$__/ $$| $$       ");
            Console.WriteLine(@"   | $$ | $$      | $$ \$$    $$| $$  | $$ \$$    $$ \$$    $$| $$ \$$    $$  \$$  $$ \$$    $$| $$       ");
            Console.WriteLine(@"    \$$  \$$       \$$  \$$$$$$$ \$$   \$$ _\$$$$$$$  \$$$$$$  \$$  \$$$$$$$   \$$$$   \$$$$$$  \$$       ");
            Console.WriteLine(@"                                          |  \__| $$                                                      ");
            Console.WriteLine(@"                                           \$$    $$          by Xv0                                      ");
            Console.WriteLine(@"                                            \$$$$$$                                                       ");
            Console.WriteLine(@"**********************************************************************************************************");

            Console.ForegroundColor = ConsoleColor.DarkYellow;
        }
    }
}
