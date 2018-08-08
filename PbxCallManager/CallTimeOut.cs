namespace CallManager
{
    public class CallTimeOut
    {
        int timeOut = -1;

        public CallTimeOut()
        {

        }


        /// <summary>
        /// Call identifier
        /// </summary>
        public string CallHandlerId { get; set; }
        /// <summary>
        /// Queue TimeOut for this call
        /// </summary>
        public int TimeOut
        {
            get
            {
                return timeOut;
            }
            set
            {
                timeOut = value;
                OriginalTimeOutValue = value;
            }
        }

        public int OriginalTimeOutValue { get; private set; }
    }
}