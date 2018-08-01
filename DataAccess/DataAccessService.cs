using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace DataAccess
{
    //Esta clase levanta del medio de persistnecia que sea la siguiente info
    //Las propiedades de las colas y dentro de cada una sus queuemember con sus propiedades
    //una lista de members con sus propiedades
    //De aca se cargan las queue sus queuemember y la lista de members para el queue system
    public class DataAccessService
    {
        //esto es para desarrollo, pero tal vez podría mantener un cache intermendio con un timepo de expiracion para no 
        //consultar continuamente al storage (tal vez Hazelcast)
        Dictionary<string, DTOMember> members;
        Dictionary<string, DTOQueue> queues;
        Dictionary<string, DTOQueue> memberQueues;

        public DataAccessService()
        {
            members = new Dictionary<string, DTOMember>();
            queues = new Dictionary<string, DTOQueue>();
            memberQueues = new Dictionary<string, DTOQueue>();
            InitTestData();
        }

        private void InitTestData()
        {
            //creo members
            //DTOMember dtom1 = new DTOMember() { Id = "3333", Password = "1234", Name = "Carlos" };
            //DTOMember dtom2 = new DTOMember() { Id = "3344", Password = "1234", Name = "Juana" };

            DTOMember dtom1 = new DTOMember() { Id = "3333", Password = "1234", Name = "Juana" };
            DTOMember dtom2 = new DTOMember() { Id = "3334", Password = "1234", Name = "Juana" };
            DTOMember dtom3 = new DTOMember() { Id = "3335", Password = "1234", Name = "Juana" };
            DTOMember dtom4 = new DTOMember() { Id = "3336", Password = "1234", Name = "Juana" };
            DTOMember dtom5 = new DTOMember() { Id = "3337", Password = "1234", Name = "Juana" };
            DTOMember dtom6 = new DTOMember() { Id = "3338", Password = "1234", Name = "Juana" };
            DTOMember dtom7 = new DTOMember() { Id = "3339", Password = "1234", Name = "Juana" };
            DTOMember dtom8 = new DTOMember() { Id = "3340", Password = "1234", Name = "Juana" };
            DTOMember dtom9 = new DTOMember() { Id = "3341", Password = "1234", Name = "Juana" };
            DTOMember dtom10 = new DTOMember() { Id = "3342", Password = "1234", Name = "Juana" };
            DTOMember dtom11 = new DTOMember() { Id = "3343", Password = "1234", Name = "Juana" };
            DTOMember dtom12 = new DTOMember() { Id = "3344", Password = "1234", Name = "Juana" };
            DTOMember dtom13 = new DTOMember() { Id = "3345", Password = "1234", Name = "Juana" };
            DTOMember dtom14 = new DTOMember() { Id = "3346", Password = "1234", Name = "Juana" };
            DTOMember dtom15 = new DTOMember() { Id = "3347", Password = "1234", Name = "Juana" };
            DTOMember dtom16 = new DTOMember() { Id = "3348", Password = "1234", Name = "Juana" };
            DTOMember dtom17 = new DTOMember() { Id = "3349", Password = "1234", Name = "Juana" };
            DTOMember dtom18 = new DTOMember() { Id = "3350", Password = "1234", Name = "Juana" };
            DTOMember dtom19 = new DTOMember() { Id = "3351", Password = "1234", Name = "Juana" };
            DTOMember dtom20 = new DTOMember() { Id = "3352", Password = "1234", Name = "Juana" };
            DTOMember dtom21 = new DTOMember() { Id = "3353", Password = "1234", Name = "Juana" };
            DTOMember dtom22 = new DTOMember() { Id = "3354", Password = "1234", Name = "Juana" };
            DTOMember dtom23 = new DTOMember() { Id = "3355", Password = "1234", Name = "Juana" };
            DTOMember dtom24 = new DTOMember() { Id = "3356", Password = "1234", Name = "Juana" };
            DTOMember dtom25 = new DTOMember() { Id = "3357", Password = "1234", Name = "Juana" };
            DTOMember dtom26 = new DTOMember() { Id = "3358", Password = "1234", Name = "Juana" };
            DTOMember dtom27 = new DTOMember() { Id = "3359", Password = "1234", Name = "Juana" };
            DTOMember dtom28 = new DTOMember() { Id = "3360", Password = "1234", Name = "Juana" };
            DTOMember dtom29 = new DTOMember() { Id = "3361", Password = "1234", Name = "Juana" };
            DTOMember dtom30 = new DTOMember() { Id = "3362", Password = "1234", Name = "Juana" };


            //members.Add(dtom1.Id, dtom1);
            //members.Add(dtom2.Id, dtom2);


            members.Add(dtom1.Id, dtom1);
            members.Add(dtom2.Id, dtom2);
            members.Add(dtom3.Id, dtom3);
            members.Add(dtom4.Id, dtom4);
            members.Add(dtom5.Id, dtom5);
            members.Add(dtom6.Id, dtom6);
            members.Add(dtom7.Id, dtom7);
            members.Add(dtom8.Id, dtom8);
            members.Add(dtom9.Id, dtom9);
            members.Add(dtom10.Id, dtom10);
            members.Add(dtom11.Id, dtom11);
            members.Add(dtom12.Id, dtom12);
            members.Add(dtom13.Id, dtom13);
            members.Add(dtom14.Id, dtom14);
            members.Add(dtom15.Id, dtom15);
            members.Add(dtom16.Id, dtom16);
            members.Add(dtom17.Id, dtom17);
            members.Add(dtom18.Id, dtom18);
            members.Add(dtom19.Id, dtom19);
            members.Add(dtom20.Id, dtom20);
            members.Add(dtom21.Id, dtom21);
            members.Add(dtom22.Id, dtom22);
            members.Add(dtom23.Id, dtom23);
            members.Add(dtom24.Id, dtom24);
            members.Add(dtom25.Id, dtom25);
            members.Add(dtom26.Id, dtom26);
            members.Add(dtom27.Id, dtom27);
            members.Add(dtom28.Id, dtom28);
            members.Add(dtom29.Id, dtom29);
            members.Add(dtom30.Id, dtom30);


            //creo colas
            DTOQueue dtoq1 = new DTOQueue() { Id = "5000", Media = "default", MediaType = "MoH", QueueMembers = new List<DTOQueueMember>(), Weight = 1, WrapupTime = 3, MemberStrategy = "rrmemory", CallOrderStrategy = "default" };
            DTOQueue dtoq2 = new DTOQueue() { Id = "6000", Media = "default", MediaType = "MoH", QueueMembers = new List<DTOQueueMember>(), Weight = 2, WrapupTime = 3, MemberStrategy = "rrmemory", CallOrderStrategy = "default" };
            queues.Add(dtoq1.Id, dtoq1);
            queues.Add(dtoq2.Id, dtoq2);

            //creo queue members
            //DTOQueueMember dtoqm1 = new DTOQueueMember() { MemberId = dtom1.Id, Priority = 0 };
            //DTOQueueMember dtoqm2 = new DTOQueueMember() { MemberId = dtom2.Id, Priority = 1 };

            DTOQueueMember dtoqm1 = new DTOQueueMember() { MemberId = dtom1.Id, Priority = 0 };
            DTOQueueMember dtoqm2 = new DTOQueueMember() { MemberId = dtom2.Id, Priority = 0 };
            DTOQueueMember dtoqm3 = new DTOQueueMember() { MemberId = dtom3.Id, Priority = 0 };
            DTOQueueMember dtoqm4 = new DTOQueueMember() { MemberId = dtom4.Id, Priority = 0 };
            DTOQueueMember dtoqm5 = new DTOQueueMember() { MemberId = dtom5.Id, Priority = 0 };
            DTOQueueMember dtoqm6 = new DTOQueueMember() { MemberId = dtom6.Id, Priority = 0 };
            DTOQueueMember dtoqm7 = new DTOQueueMember() { MemberId = dtom7.Id, Priority = 0 };
            DTOQueueMember dtoqm8 = new DTOQueueMember() { MemberId = dtom8.Id, Priority = 0 };
            DTOQueueMember dtoqm9 = new DTOQueueMember() { MemberId = dtom9.Id, Priority = 0 };
            DTOQueueMember dtoqm10 = new DTOQueueMember() { MemberId = dtom10.Id, Priority = 0 };
            DTOQueueMember dtoqm11 = new DTOQueueMember() { MemberId = dtom11.Id, Priority = 0 };
            DTOQueueMember dtoqm12 = new DTOQueueMember() { MemberId = dtom12.Id, Priority = 0 };
            DTOQueueMember dtoqm13 = new DTOQueueMember() { MemberId = dtom13.Id, Priority = 0 };
            DTOQueueMember dtoqm14 = new DTOQueueMember() { MemberId = dtom14.Id, Priority = 0 };
            DTOQueueMember dtoqm15 = new DTOQueueMember() { MemberId = dtom15.Id, Priority = 0 };
            DTOQueueMember dtoqm16 = new DTOQueueMember() { MemberId = dtom16.Id, Priority = 0 };
            DTOQueueMember dtoqm17 = new DTOQueueMember() { MemberId = dtom17.Id, Priority = 0 };
            DTOQueueMember dtoqm18 = new DTOQueueMember() { MemberId = dtom18.Id, Priority = 0 };
            DTOQueueMember dtoqm19 = new DTOQueueMember() { MemberId = dtom19.Id, Priority = 0 };
            DTOQueueMember dtoqm20 = new DTOQueueMember() { MemberId = dtom20.Id, Priority = 0 };
            DTOQueueMember dtoqm21 = new DTOQueueMember() { MemberId = dtom21.Id, Priority = 0 };
            DTOQueueMember dtoqm22 = new DTOQueueMember() { MemberId = dtom22.Id, Priority = 0 };
            DTOQueueMember dtoqm23 = new DTOQueueMember() { MemberId = dtom23.Id, Priority = 0 };
            DTOQueueMember dtoqm24 = new DTOQueueMember() { MemberId = dtom24.Id, Priority = 0 };
            DTOQueueMember dtoqm25 = new DTOQueueMember() { MemberId = dtom25.Id, Priority = 0 };
            DTOQueueMember dtoqm26 = new DTOQueueMember() { MemberId = dtom26.Id, Priority = 0 };
            DTOQueueMember dtoqm27 = new DTOQueueMember() { MemberId = dtom27.Id, Priority = 0 };
            DTOQueueMember dtoqm28 = new DTOQueueMember() { MemberId = dtom28.Id, Priority = 0 };
            DTOQueueMember dtoqm29 = new DTOQueueMember() { MemberId = dtom29.Id, Priority = 0 };
            DTOQueueMember dtoqm30 = new DTOQueueMember() { MemberId = dtom30.Id, Priority = 0 };


            //agrego los queue members a las colas
            //dtoq1.QueueMembers.Add(dtoqm1);
            //dtoq1.QueueMembers.Add(dtoqm2);
            //dtoq2.QueueMembers.Add(dtoqm1);

            dtoq1.QueueMembers.Add(dtoqm1);
            dtoq1.QueueMembers.Add(dtoqm3);
            dtoq1.QueueMembers.Add(dtoqm5);
            dtoq1.QueueMembers.Add(dtoqm7);
            dtoq1.QueueMembers.Add(dtoqm9);
            dtoq1.QueueMembers.Add(dtoqm11);
            dtoq1.QueueMembers.Add(dtoqm13);
            dtoq1.QueueMembers.Add(dtoqm15);
            dtoq1.QueueMembers.Add(dtoqm17);
            dtoq1.QueueMembers.Add(dtoqm19);
            dtoq1.QueueMembers.Add(dtoqm21);
            dtoq1.QueueMembers.Add(dtoqm23);
            dtoq1.QueueMembers.Add(dtoqm25);
            dtoq1.QueueMembers.Add(dtoqm27);
            dtoq1.QueueMembers.Add(dtoqm29);

            dtoq2.QueueMembers.Add(dtoqm2);
            dtoq2.QueueMembers.Add(dtoqm4);
            dtoq2.QueueMembers.Add(dtoqm6);
            dtoq2.QueueMembers.Add(dtoqm8);
            dtoq2.QueueMembers.Add(dtoqm10);
            dtoq2.QueueMembers.Add(dtoqm12);
            dtoq2.QueueMembers.Add(dtoqm14);
            dtoq2.QueueMembers.Add(dtoqm16);
            dtoq2.QueueMembers.Add(dtoqm18);
            dtoq2.QueueMembers.Add(dtoqm20);
            dtoq2.QueueMembers.Add(dtoqm22);
            dtoq2.QueueMembers.Add(dtoqm24);
            dtoq2.QueueMembers.Add(dtoqm26);
            dtoq2.QueueMembers.Add(dtoqm28);
            dtoq2.QueueMembers.Add(dtoqm30);
            //dtoq2.QueueMembers.Add(dtoqm1);
            //dtoq2.QueueMembers.Add(dtoqm5);
            //dtoq2.QueueMembers.Add(dtoqm7);



        }

        #region Métodos públicos
        public List<DTOQueue> GetQueues()
        {
            return queues.Values.ToList<DTOQueue>();
        }

        public List<DTOMember> GetMembers()
        {
            return null;
        }

        public DTOMember GetMemberById(string memberId)
        {
            if (members != null && members.ContainsKey(memberId))
            {
                return members[memberId];
            }
            else
            {
                return null;
            }
        }

        //TODO: mejorar esto, está medio chancho
        public List<string> GetMemberQueues(string memberId)
        {
            //if (memberQueues != null && memberQueues.ContainsKey(memberId))
            //{
                List<string> colas = new List<string>();
                foreach (DTOQueue q in queues.Values) {
                    foreach(DTOQueueMember m in q.QueueMembers){
                        if (memberId == m.MemberId) {
                            colas.Add(q.Id);
                        }
                    }
                }
                return colas;
            //}
            //else
            //{
            //    return null;
            //}
        }
        #endregion
    }
}
