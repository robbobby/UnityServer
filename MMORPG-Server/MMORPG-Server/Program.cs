using System;
using System.Threading;

namespace MMORPG_Server {
    class Program {
        private static bool _isRunning;
        static void Main(string[] args) {
            Console.Title = "MMORPG Server";
            _isRunning = true;

            Thread mainTread = new Thread(new ThreadStart(MainThread));
            mainTread.Start();
            Console.WriteLine("Starting the server");
            Server.Start(50, 26950);
        }

        private static void MainThread() {
            Console.WriteLine($"Main thread has started. Running at {Constants.TICKS_PER_SECOND} ticks per second.");
            DateTime nextLoop = DateTime.Now;

            while (_isRunning) {
                while (nextLoop < DateTime.Now) {
                    GameLogic.Update();

                    nextLoop = nextLoop.AddMilliseconds(Constants.MS_PER_TICK);

                    if (nextLoop > DateTime.Now) {
                        Thread.Sleep(nextLoop - DateTime.Now);
                    }
                }
            }
        }
    }
}