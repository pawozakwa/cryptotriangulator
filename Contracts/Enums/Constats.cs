using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Enums
{
    public class Constants
    {
        // When enter will be USDT, 
        // BTC needs to be added as constant next first step in chain
        public const Currency EnterCurrency = Currency.BTC;

        public const Currency ArbitraryCurrency = Currency.BTC;

        public const int SearchDepth = 5;

        public const decimal Commision = 0.1m / 100m;

        //Tweaks

        public const bool LiveProgressBar = false;

        public const bool SaveWhatIsUnparsableToFile = false;
    }
}
