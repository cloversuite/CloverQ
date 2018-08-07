using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CallManager
{
    //TODO: Refactorizar esta clase, hay que mejorar el tema del diccionario y la lista, mucha vuelta
    public class CallTimeoutHandler
    {
        Timer toTimer = new Timer();
        public delegate void DelCallTimeout(CallTimeOut callTimeOut);
        public event DelCallTimeout CallTimeOutEvent;

        object locker = new object();
        Dictionary<string, CallTimeOut> callTimeOutList = new Dictionary<string, CallTimeOut>();
        List<string> elapsed = new List<string>();

        public CallTimeoutHandler()
        {
            toTimer.Elapsed += ToTimer_Elapsed;
        }

        private void ToTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.toTimer.Enabled = false;
            lock (locker)
            {
                foreach (KeyValuePair<string,CallTimeOut> entry in callTimeOutList) {
                    entry.Value.TimeOut--;
                    if(entry.Value.TimeOut <=0)
                    {
                        OnCallTimeOut(entry.Value);
                        elapsed.Add(entry.Key);
                    }
                }
            }
            foreach(string id in elapsed)
            {
                callTimeOutList.Remove(id);
            }
            elapsed.Clear();
            this.toTimer.Enabled = true;
        }

        protected void OnCallTimeOut(CallTimeOut callTimeOut) {
            if (callTimeOut != null) {
                CallTimeOutEvent(callTimeOut);
            }
        }

        public void AddCallTimeOut(CallTimeOut callTimeOut)
        {
            lock (locker)
            {
                callTimeOutList.Add(callTimeOut.CallHandlerId, callTimeOut);
            }
        }

        public void CancelCallTimOut(string callHandlerId) {
            callTimeOutList.Remove(callHandlerId);
        }

        public void Start() {
            toTimer.Interval = 1000;
            toTimer.Start();
        }

        public void Stop() {
            toTimer.Stop();
            toTimer.Dispose();
        }

    }
}
