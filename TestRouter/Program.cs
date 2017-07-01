﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StateProvider;
using AkkaActorSystem;

namespace TestRouter
{
    class Program
    {
        static void Main(string[] args)
        {

            QActorSystem qActorSystem = new QActorSystem();


            CallManager callManager = new CallManager(qActorSystem.GetActorPbxProxy());
            callManager.Connect("192.168.56.101", 8088, "asterisk", "pelo2dos");
            Console.WriteLine("CallManager iniciado...");

            

            DeviceStateManager dsm = new DeviceStateManager(qActorSystem.GetActorStateProxy());
            dsm.Connect("192.168.56.101", 8088, "asterisk", "pelo2dos");
            Console.WriteLine("StateManager iniciado...");
            
            Console.WriteLine("Presione una tecla para terminar la aplicación...");
            Console.ReadLine();

            callManager.Disconnect();
            dsm.Disconnect();
            qActorSystem.Stop();

        }
    }
}