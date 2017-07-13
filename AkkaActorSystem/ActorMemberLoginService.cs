using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using ProtocolMessages;

namespace AkkaActorSystem
{
    //Este actor debe manejar los logins, le llega un login, lo valida, y si es correcto consulta las colas del miembro y 
    //le envía al call distributor los MessageQMemberAdd necesarios.
    //IDEA: deberia poder crear actores hijos del tipo proxy que implementen un ivr de login basado en FastAGI ó AsyncAGI, 
    //tambien podria crear algun tipo de actor que ofrezca un endponint para login via rest, exixte buena integracions para usar 
    //asp.net con akka por lo que podría probarse con .net core web api 
    class ActorMemberLoginService : ReceiveActor
    {
        IActorRef callDistributor;

        public ActorMemberLoginService(IActorRef callDistributor, IActorRef actorDataAccess)
        {
            this.callDistributor = callDistributor;

            Receive<MessageMemberLogin>(mlin =>
            {
                //solicito de forma asíncron las colas a las que pertenece el meimbro
                //Que pasa si el miembro no posee registro de a que colas pertenece?, debería tener una prop en el mensaje que indique eso
                //para no ir a buscar al dataacces??
                actorDataAccess.Tell(new DAGetMemberQueues() { MemberId = mlin.MemberId });

            });

            Receive<MessageMemberLogoff>(mlof => {
                //le paso el queuesid list en null para desloguearlo de todas las colas
                callDistributor.Tell(new MessageQMemberRemove() { MemberId = mlof.MemberId, QueuesId = null });
            });

            Receive<DAMemberQueues>(mqs => {
                //Cuando el actor access service me responde le indico al call distrbutor que agregue el miembro a las colas
                callDistributor.Tell(new MessageQMemberAdd() { MemberId = mqs.MemberId, QueuesId = mqs.MemberQueues });
            });

        }

        protected override void Unhandled(object message)
        {
            base.Unhandled(message);
            Console.WriteLine("MemberLoginService mensaje no manejado");
        }
    }
}
