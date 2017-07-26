using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateProvider
{
    public class Device
    {
        private string id ;
        private string  memberId;
        private string  host;
        private string port;
        private string contact;
        private string deviceState;
        private string endpointState;

        public Device()
        {

        }

        public string Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }
        public string MemberId
        {
            get
            {
                return memberId;
            }

            set
            {
                memberId = value;
            }
        }
        public string Host
        {
            get
            {
                return host;
            }

            set
            {
                host = value;
            }
        }
        public string Port
        {
            get
            {
                return port;
            }

            set
            {
                port = value;
            }
        }
        public string Contact
        {
            get
            {
                return contact;
            }

            set
            {
                contact = value;
            }
        }
        public string DeviceState
        {
            get
            {
                return deviceState;
            }

            set
            {
                deviceState = value;
            }
        }
        public string EndpointState
        {
            get
            {
                return endpointState;
            }

            set
            {
                endpointState = value;
            }
        }
    }
}
