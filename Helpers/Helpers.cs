using System;
using System.IO;
using System.Threading.Tasks;

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
            //var oldColor = Console.ForegroundColor;
            //Console.ForegroundColor = ConsoleColor.DarkGray;
            //Console.WriteLine(text);
            //Console.ForegroundColor = oldColor;
#endif
        }
        
        private static string DesktopPath => Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private static string LogFileName = "Triangulation results.txt";
        private static string LogFilePath => Path.Combine(DesktopPath, LogFileName);

        public static void AddToFileOnDesktop(params string[] texts)
        {
            using (var sw = File.AppendText(LogFilePath))
                foreach (var text in texts)
                    sw.WriteLineAsync(text);
        }
    }
}
