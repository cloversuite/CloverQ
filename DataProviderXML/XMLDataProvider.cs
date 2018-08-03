using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DataAccess;

namespace DataProviderXML
{
    public class XMLDataProvider : IDataProvider
    {
        string fileName = "";
        XMLDataSerializer ser = new XMLDataSerializer();

        public void Connect(string cnnString)
        {
            fileName = cnnString;
        }

        public DTOMember GetMemberById(string memberId)
        {
            throw new NotImplementedException();
        }

        public List<DTOMember> GetMembers()
        {
            return ser.DeSerializeObject<List<DTOMember>>(fileName + "members.xml");
        }

        public DTOQueue GetQueueById(string queueId)
        {
            throw new NotImplementedException();
        }

        public List<DTOQueue> GetQueues()
        {
            return ser.DeSerializeObject<List<DTOQueue>>(fileName + "queues.xml");
        }

        public void SaveMembers(List<DTOMember> members)
        {
            ser.SerializeObject<List<DTOMember>>(members, fileName + "members.xml");
        }

        public void SaveQueues(List<DTOQueue> queues)
        {
            ser.SerializeObject<List<DTOQueue>>(queues, fileName + "queues.xml");
        }
    }
}
