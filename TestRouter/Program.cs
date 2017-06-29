using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace TestRouter
{
    class Program
    {
        static void Main(string[] args)
        {
            //Router r = new Router();
            //r.Start();

            CallManager callManager = new CallManager();
            callManager.Connect("192.168.56.101", 8088, "asterisk", "pelo2dos");
            Console.WriteLine("Presione una tecla para terminar la aplicación...");
            Console.ReadLine();
            callManager.Disconnect();

        }
    }
}
