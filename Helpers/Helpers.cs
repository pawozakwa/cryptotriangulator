using System;

namespace Helpers
{
    public static class Helpers
    {
        public static void Debug(string text)
        {
    #if DEBUG
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(text);
            Console.ForegroundColor = oldColor;
    #endif
        }
    }
}
