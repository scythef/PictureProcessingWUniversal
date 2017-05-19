using PictureProcessingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureProcessingWUniversal.Services
{
    //duplicity in PictureProcessingWebJob, DoorPhoneIoTUI a WUniversal
    public class InOutMessage
    {
        public string DeviceID { get; set; }
        public int Type { get; set; }
        /*
         0 - facedetected
         1 - in-forcedsnapshot/out-snapshot
         2 - facerecognized
         3 - in-unlock/out-unlocked
         4 - heartbeat
         5 - in-forcedcall/out-call
         */
        public string DateTime { get; set; }
        public string Parameter { get; set; }
    }

    class DeviceSVC
    {
        public static async Task<Guid> CreateDevice(string aName, Guid aPlaceGuid, int aDevicetype)
        {
            Guid lOutputDeviceGuid = Guid.NewGuid();

            PictureProcessingAPIClient lPPAPI = new PictureProcessingAPIClient();
            var x = await lPPAPI.CreateDeviceWithOperationResponseAsync(aName, lOutputDeviceGuid.ToString(), aPlaceGuid.ToString(), aDevicetype, new System.Threading.CancellationToken());

            return lOutputDeviceGuid;
        }

        public async static Task SendCloudToDeviceMessageAsync(string aDeviceID, string aMessage)
        {
                PictureProcessingAPIClient lPPAPI = new PictureProcessingAPIClient();
                var x = await lPPAPI.IoT.PostWithOperationResponseAsync(aDeviceID, aMessage);
        }

    }
}
