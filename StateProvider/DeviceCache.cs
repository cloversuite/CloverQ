using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateProvider
{
    public class DeviceCache
    {
        //TODO: cambiar por un concurrent dictionary, cambian los metodos add y remove
        Dictionary<string, Device> deviceCache;

        public DeviceCache()
        {
            deviceCache = new Dictionary<string, Device>();
        }

        public void AddDevice(Device device)
        {
            deviceCache.Add(device.Id, device);
        }

        public void RemoveDevice(string id)
        {
            if (deviceCache.ContainsKey(id))
            {
                deviceCache.Remove(id);
            }
        }

        public void UpdateDeviceContact(string id, string contact)
        {
            if (deviceCache.ContainsKey(id))
            {
                deviceCache[id].Contact = contact;
            }
        }

        public void UpdateDeviceMember(string id, string memberId)
        {
            if (deviceCache.ContainsKey(id))
            {
                deviceCache[id].MemberId = memberId;
            }
        }

        public void UpdateDeviceState(string id, string state)
        {
            if (deviceCache.ContainsKey(id))
            {
                deviceCache[id].DeviceState = state;
            }
        }

        public void UpdateEndpointState(string id, string state)
        {
            if (deviceCache.ContainsKey(id))
            {
                deviceCache[id].EndpointState = state;
            }
        }

        public void AttachMemberToDevice(string deviceId, string memberId) {
            if (deviceCache.ContainsKey(deviceId))
            {
                deviceCache[deviceId].MemberId = memberId;
            }
        }

        public void DetachMemberFromDevice(string deviceId, string memberId)
        {
            if (deviceCache.ContainsKey(deviceId))
            {
                //debo controlar que el member que tiene el device sea el mismo que quiero quitar?
                deviceCache[deviceId].MemberId = "";
            }
        }

        public Device GetDeviceById(string id)
        {
            if (deviceCache.ContainsKey(id))
            {
                return deviceCache[id];
            }
            else
            {
                return null;
            }
        }

    }
}
