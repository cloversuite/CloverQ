using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueSystem
{
    class CallOrderStrategyDefault : ICallOrderStrategy
    {
        List<Call> calls = null;

        public CallOrderStrategyDefault(List<Call> calls)
        {
            SetCallList(calls);
        }

        public CallOrderStrategyDefault()
        {

        }

        public void SetCallList(List<Call> calls)
        {
            this.calls = calls;
        }

        public Call GetNext()
        {
            Call next = null;
            if (calls != null && calls.Count > 0)
            {
                for (int i = 0; i < calls.Count; i++)
                {
                    if ( !calls[i].IsDispatching && !calls[i].Connected)
                    {
                        next = calls[i];
                        next.IsDispatching = true;
                        break;
                    }
                }
            }
            //Si retorno null, no hay mas llamadas para despachar
            return next;
        }
    }
}
