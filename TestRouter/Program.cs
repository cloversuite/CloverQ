using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StateProvider;
using AkkaActorSystem;
using LoginProvider;

namespace TestRouter
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO: mover la configuracion al sistema de actores

            //SystemConfiguration config = new SystemConfiguration("cloverq-conf.json");
            //config.QueueLog = new ConfQueueLog() {LogFilePrefix = "logcito" };
            //config.CallManagers.Add(new ConfHost() { Ip = "192.168.56.102", Port = 8088, User = "asterisk", Password = "pelo2dos" });
            //config.StateProviders.Add(new ConfHost() { Ip = "192.168.56.102", Port = 8088, User = "asterisk", Password = "pelo2dos" });
            //config.LoginProviders.Add(new ConfHost() { Ip = "192.168.56.102", Port = 8088, User = "asterisk", Password = "pelo2dos" });

            //config.SaveConf();

            //SystemConfiguration conf = SystemConfiguration.GetConf("cloverq-conf.json");


            QActorSystem qActorSystem = new QActorSystem();


            CallManager callManager = new CallManager(qActorSystem.GetActorPbxProxy());
            callManager.Connect("192.168.56.102", 8088, "asterisk", "pelo2dos"); //192.168.56.102
            Console.WriteLine("CallManager iniciado...");



            DeviceStateManager dsm = new DeviceStateManager(qActorSystem.GetActorStateProxy());
            dsm.Connect("192.168.56.90", 8088, "asterisk", "pelo2dos"); //192.168.56.90
            Console.WriteLine("StateManager iniciado...");

            PbxLoginProvider plp = new PbxLoginProvider(qActorSystem.GetActorLoginProxy());
            plp.Connect("192.168.56.90", 8088, "asterisk", "pelo2dos"); //192.168.56.90
            Console.WriteLine("PbxLoginProvider iniciado...");

            Console.WriteLine("Presione una tecla para terminar la aplicación...");
            Console.ReadLine();

            callManager.Disconnect();
            dsm.Disconnect();
            plp.Disconnect();
            qActorSystem.Stop();

        }
    }
}
