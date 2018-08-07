using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginProvider
{
    /// <summary>
    /// Esta clase mapea el device al member para poder obtener el memberId teniendo el deviceId
    /// </summary>
    public class DeviceMemberMap
    {
        Dictionary<string, string> deviceMember;

        public DeviceMemberMap()
        {
            deviceMember = new Dictionary<string, string>();
        }

        public void TrackMemberDeviceId(string deviceId, string memberId)
        {
            if (!deviceMember.ContainsKey(deviceId))
            {
                deviceMember.Add(deviceId, memberId);
            }
            else
            {
                Console.WriteLine("DeviceMemberMap: Error device: " + deviceId + " ya esta asociado a: " + deviceMember[deviceId]);
            }
        }

        public string GetMemberIdFromDeviceId(string deviceId)
        {
            string memberId = "";
            if (deviceMember.ContainsKey(deviceId))
                memberId = deviceMember[deviceId];
            else
                Console.WriteLine("DeviceMemberMap: Warning no se pudo determinar el memberid para el device: " + deviceId);

            return memberId;
        }

        public void UnTrackMemberDeviceId(string deviceId)
        {
            if (deviceMember.ContainsKey(deviceId))
                deviceMember.Remove(deviceId);
        }

    }
}
