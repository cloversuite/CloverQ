using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using ProtocolMessages;
using DataAccess;
using Models;

namespace AkkaActorSystem
{
    class ActorDataAccess : ReceiveActor
    {
        DataAccessService das;

        public ActorDataAccess()
        {
            das = new DataAccessService(); //aca deberia pasarle el string de conexion y tal vez el tipo de almacenamiento, por el momento harcodeado

            Receive<DAGetQueues>(cf =>
            {

                Sender.Tell(new DAQueues() { Queues = das.GetQueues() });

            });

            Receive<DAGetMembers>(cf =>
            {
                Sender.Tell(new DAMembers() { Members = das.GetMembers() });

            });

        }
    }
}
