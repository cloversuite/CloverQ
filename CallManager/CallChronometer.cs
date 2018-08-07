using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallManager
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
        public int HoldingTime
        {
            get
            {
                return totalCallHold;
            }
        }

        /// <summary>
        /// Return wait time between enter queue and memeber answer
        /// </summary>
        public int WaitingTime
        {
            get
            {
                //si es un abandono nunca hay un connect
                if((int)(timeConnect - timeStart).TotalSeconds < 0)
                    return (int)(DateTime.Now - timeStart).TotalSeconds;
                else
                    return (int)(timeConnect - timeStart).TotalSeconds;
            }
        }

        /// <summary>
        /// Return de second that the caller was connected with the member
        /// </summary>
        public int ConnectedTime {
            get
            {
                return (int)(timeEnd - timeConnect).TotalSeconds;
            }
        }

        /// <summary>
        /// Retun the seconds that the caller was connected to the member not in onhold 
        /// </summary>
        public int TalkingTime
        {
            get
            {
                return ConnectedTime - HoldingTime;
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
            int elapsed = (int)(timeStopRing - timeStartRing).TotalSeconds;
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
            int elapsed = (int)(timeConnect - timeStart).TotalSeconds;
            return elapsed;
        }

        /// <summary>
        /// Mark call onhold start time
        /// </summary>
        public void CallHoldStart()
        {
            timeStartHold = DateTime.Now;
        }

        /// <summary>
        /// Mark call unhold time and accumulate the elapsed time between OnHold and UnHold
        /// </summary>
        /// <returns>elapsed time from las onhold</returns>
        public int CallHoldStop()
        {
            timeStopHold = DateTime.Now;
            int elapsed = (int) (timeStopHold - timeStartHold).TotalSeconds;
            totalCallHold += elapsed;
            return elapsed;
        }
        /// <summary>
        /// Mark the end of call with a member
        /// </summary>
        /// <returns></returns>
        public int CallEnd()
        {
            timeEnd = DateTime.Now;
            int elapsed = (int)(timeEnd - timeStart).TotalSeconds;
            return elapsed;
        }
    }
}
