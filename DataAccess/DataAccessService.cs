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
        #region Métodos públicos
        public List<DTOQueue> GetQueues() {
            return null;
        }

        public List<DTOMember> GetMembers() {
            return null;
        }
        public List<string> GetMemberQueues(string memberId) {
            return null;
        }
        #endregion
    }
}
