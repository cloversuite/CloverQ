using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueSystem
{
    /// <summary>
    /// Clase que representa un miembro
    /// </summary>
    public class Member
    {
        private DateTime loginTime;
        private DateTime pauseTime;
        private int loginElapsedTime = 0;
        private int pauseElapsedTime = 0;
        

        public Member()
        {
            IsAvailable = true;
        }

        #region Properties
        public string Id { get; set; }
        public string Name { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Contact { get; set; }
        public bool IsLogedIn { get; set; }
        public bool IsPaused { get; set; }
        public string PauseCode { get; set; }
        public string PauseReason { get; set; }
        public bool IsAvailable { get; set; }
        public string DeviceId { get; set; }
        public bool DeviceIsInUse { get; set; }
        public bool EndpointIsOfline { get; set; }
        public int LoginElapsedTime
        {
            get
            {
                return loginElapsedTime;
            }

            set
            {
                loginElapsedTime = value;
            }
        }
        public int PauseElapsedTime
        {
            get
            {
                return pauseElapsedTime;
            }

            set
            {
                pauseElapsedTime = value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Set the member login date time 
        /// </summary>
        public void SetLoginTime() {
            this.loginTime = DateTime.Now;
        }
        /// <summary>
        /// Set the member last pause datetime
        /// </summary>
        public void SetPauseTime() {
            this.pauseTime = DateTime.Now;
        }
        /// <summary>
        /// Set the member logoff time and returns elapsed time from login
        /// </summary>
        /// <returns>elapsed time from login</returns>
        public int SetLogoffTime() {
            int elapsed = 0;
            if (IsLogedIn)
            {
                elapsed = (int)(DateTime.Now - loginTime).TotalSeconds;
                LoginElapsedTime = elapsed;
            }
            else
                throw new InvalidOperationException("The member is not loggedin");

            return elapsed;
        }
        /// <summary>
        /// Set the member unpause time and returns elapsed time from last pause
        /// </summary>
        /// <returns>elapsed time from last pause</returns>
        public int SetUnpauseTime() {
            int elapsed = 0;
            if (IsLogedIn)
            {
                elapsed = (int)(DateTime.Now - pauseTime).TotalSeconds;
                LoginElapsedTime = elapsed;
            }
            else
                throw new InvalidOperationException("The member is not paused");

            return elapsed;
        }

        #endregion
    }

}
