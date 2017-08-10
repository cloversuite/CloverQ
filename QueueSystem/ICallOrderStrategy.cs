using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueSystem
{
    public interface ICallOrderStrategy
    {
        void SetCallList(List<Call> call);
        Call GetNext();
    }
}
