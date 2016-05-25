using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkChangeReactor;

namespace ReactorConsoleRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Debug("Starting Reactor from Console Runner");
            Reactor.Start();
            Logger.Debug("Press any key to exit");
            Console.ReadLine();
        }
    }
}
