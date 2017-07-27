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

        public Device UpdateDeviceContact(string id, string contact)
        {
            Device device = null;
            if (deviceCache.ContainsKey(id))
            {
                device = deviceCache[id];
                device.Contact = contact;
            }
            return device;
        }

        public Device UpdateDeviceMember(string id, string memberId)
        {
            Device device = null;
            if (deviceCache.ContainsKey(id))
            {
                device = deviceCache[id];
                device.MemberId = memberId;
            }
            return device;
        }

        public Device UpdateDeviceState(string id, string state)
        {
            Device device = null;
            if (deviceCache.ContainsKey(id))
            {
                device = deviceCache[id];
                device.DeviceState = state;
            }
            return device;
        }

        public Device UpdateEndpointState(string id, string state)
        {
            Device device = null;
            if (deviceCache.ContainsKey(id))
            {
                device = deviceCache[id];
                device.EndpointState = state;
            }
            return device;
        }

        public Device AttachMemberToDevice(string deviceId, string memberId) {
            Device device = null;
            if (deviceCache.ContainsKey(deviceId))
            {
                device = deviceCache[deviceId];
                device.MemberId = memberId;
            }
            return device;
        }

        public Device DetachMemberFromDevice(string deviceId, string memberId)
        {
            Device device = null;
            if (deviceCache.ContainsKey(deviceId))
            {
                device = deviceCache[deviceId];
                //debería controlar que sea el mismo memberId que esta actualmente en el device?
                device.MemberId = "";
            }
            return device;
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
