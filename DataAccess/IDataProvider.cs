using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace DataAccess
{
    /// <summary>
    /// Simple Interfaz que implementan los ditintos proveedores de datos
    /// 
    /// </summary>
    public interface IDataProvider
    {
        void Connect(string cnnString);

        List<DTOQueue> GetQueues();
        void SaveQueues(List<DTOQueue> queues);
        DTOQueue GetQueueById(string queueId);

        List<DTOMember> GetMembers();
        void SaveMembers(List<DTOMember> members);
        DTOMember GetMemberById(string memberId);
    }
}
