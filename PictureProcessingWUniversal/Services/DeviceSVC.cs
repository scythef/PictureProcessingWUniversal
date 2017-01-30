using PictureProcessingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureProcessingWUniversal.Services
{
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
