using System;

namespace Helpers
{
    public static class Helpers
    {
        
        public static void PrintInColor(string text, ConsoleColor color)
        {
            var lastColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = lastColor;
        }
        
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
