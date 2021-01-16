using System;

namespace MMORPG_Server {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Hello World!");
            Server.Start(50, 26950);
            Console.ReadKey();
        }
    }
}