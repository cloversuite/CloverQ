using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRouter.Proxy
{
    public class ComProxy
    {
        public event EventHandler Receive;
        public void Tell() { }

        protected void onReceive() {

        }
    }

    
    
}
