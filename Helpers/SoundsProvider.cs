using System;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers
{
    public static class SoundsProvider
    {
        public static void PlayMario()
        {
            new Task(() =>
            {
                Console.Beep(659, 125);
                Console.Beep(659, 125); Thread.Sleep(125);
                Console.Beep(659, 125); Thread.Sleep(167);
                Console.Beep(523, 125);
                Console.Beep(659, 125); Thread.Sleep(125);
                Console.Beep(784, 125); Thread.Sleep(375);
                Console.Beep(392, 125); Thread.Sleep(375);
                Console.Beep(523, 125); Thread.Sleep(250);
                Console.Beep(392, 125); Thread.Sleep(250);
                Console.Beep(330, 125); Thread.Sleep(250);
                Console.Beep(440, 125); Thread.Sleep(125);
                Console.Beep(494, 125); Thread.Sleep(125);
                Console.Beep(466, 125); Thread.Sleep(42);
                Console.Beep(440, 125); Thread.Sleep(250); 
                Console.Beep(659, 125); Thread.Sleep(125);
                Console.Beep(784, 125); Thread.Sleep(125);
                Console.Beep(880, 125); Thread.Sleep(125);
                Console.Beep(698, 125);
                Console.Beep(784, 125); Thread.Sleep(125);
                Console.Beep(659, 125); Thread.Sleep(125);
                Console.Beep(523, 125); Thread.Sleep(125);
                Console.Beep(587, 125);
                Console.Beep(494, 125);
            }).Start();            
        }

        public static void PlayWinnerMusic()
        {
            new Task(() =>
            {
                Console.Beep(659, 125);
                Console.Beep(659, 125); Thread.Sleep(125);
                Console.Beep(659, 125); Thread.Sleep(167);
                Console.Beep(523, 125);
                Console.Beep(659, 125); Thread.Sleep(125);
                Console.Beep(784, 125); Thread.Sleep(375);
                Console.Beep(392, 125); 
            }).Start();
        }
    }
}
