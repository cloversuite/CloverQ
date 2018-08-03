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
            throw new NotImplementedException();
        }

        public DTOQueue GetQueueById(string queueId)
        {
            throw new NotImplementedException();
        }

        public List<DTOQueue> GetQueues()
        {
            throw new NotImplementedException();
        }

        public void SaveMembers(List<DTOMember> members)
        {
            ser.SerializeObject<List<DTOMember>>(members, fileName);
        }

        public void SaveQueues(List<DTOQueue> queues)
        {
            ser.SerializeObject<List<DTOQueue>>(queues, fileName);
        }
    }
}
