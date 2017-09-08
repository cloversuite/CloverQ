using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRouter
{
    public class CallChronometer
    {
        DateTime timeStart;
        DateTime timeStartRing;
        DateTime timeStopRing;
        DateTime timeConnect;
        DateTime timeStartHold;
        DateTime timeStopHold;
        DateTime timeEnd;
        int totalCallHold;

        /// <summary>
        /// Total amount of seconds that the call was on Hold
        /// </summary>
        public int TotalCallHoldSeconds
        {
            get
            {
                return totalCallHold;
            }
        }

        /// <summary>
        /// Mark the call start time form callhandler perspective
        /// </summary>
        public void CallStart()
        {
            timeStart = DateTime.Now;
        }

        /// <summary>
        /// Mark the call to start time
        /// </summary>
        public void CallToStart()
        {
            timeStartRing = DateTime.Now;
        }

        /// <summary>
        /// Mark call to failed time and return elapsed seconds from calltostart
        /// </summary>
        /// <returns>elapsed seconds from calltostart</returns>
        public int CallToEndFailed()
        {
            timeStopRing = DateTime.Now;
            int elapsed = (timeStopRing - timeStartRing).Seconds;
            return elapsed;
        }
        /// <summary>
        /// Mark call to success (connect) and return elapsed seconds from CallStart
        /// </summary>
        /// <returns>elapsed seconds from CallStart</returns>
        public int CallToSuccess()
        {
            timeStopRing = DateTime.Now;
            timeConnect = timeStopRing;
            int elapsed = (timeConnect - timeStart).Seconds;
            return elapsed;
        }

        /// <summary>
        /// Mark call onhold start time
        /// </summary>
        public void CallHoldStart() {
            timeStartHold = DateTime.Now;
        }
        
        /// <summary>
        /// Mark call unhold time and accumulate the elapsed time between OnHold and UnHold
        /// </summary>
        /// <returns>elapsed time from las onhold</returns>
        public int CallHoldStop()
        {
            timeStopHold = DateTime.Now;
            int elapsed = (timeStopHold - timeStartHold).Seconds;
            totalCallHold += elapsed;
            return elapsed;
        }
    }
}
