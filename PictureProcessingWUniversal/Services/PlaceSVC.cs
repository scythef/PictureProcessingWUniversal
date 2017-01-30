using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using PictureProcessingAPI;
using PictureProcessingAPI.Models;
using PictureProcessingWUniversal.Models;
using PictureProcessingWUniversal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureProcessingWUniversal.Services
{
    class PlaceSVC
    {

        public static async void UpdatePlace(PlaceViewModel aPlaceVM)
        {
            PictureProcessingAPIClient lPPAPI = new PictureProcessingAPIClient();
        }

        public static async Task<Guid> CreatePlace(string aName)
        {
            Guid lOutputPlaceGuid = Guid.NewGuid();

            PictureProcessingAPIClient lPPAPI = new PictureProcessingAPIClient();
            var x = await lPPAPI.CreatePlaceWithOperationResponseAsync(aName, App.varCustomerGUID.ToString(), lOutputPlaceGuid.ToString(), new System.Threading.CancellationToken());

            return lOutputPlaceGuid;
        }

        public static async Task<List<Place>> GetPlaces(string aCustomerGUID)
        {
            PictureProcessingAPIClient lPPAPI = new PictureProcessingAPIClient();
            var x = await lPPAPI.GetPlacesByCustomerWithOperationResponseAsync(aCustomerGUID, new System.Threading.CancellationToken());

            IList<PlaceAPI> Lx = (x as Microsoft.Rest.HttpOperationResponse<IList<PlaceAPI>>).Body;

            List<Place> LPlaList = new List<Place>();
            foreach (PlaceAPI xPlace in Lx)
            {
                LPlaList.Add(GetPlace(xPlace));
            }

            return LPlaList;

        }

        public static Place GetPlace(PlaceAPI aPlace)
        {
            Place LPlace = new Place();
            LPlace.Name = aPlace.Name;
            LPlace.CustomerGUID = Guid.Parse(aPlace.CustomerGUID);
            LPlace.GUID = Guid.Parse(aPlace.GUID);
            LPlace.TimeZone = aPlace.TimeZone.GetValueOrDefault(0);
            List<Device> LDevices = new List<Device>();
            foreach (DeviceAPI xDevice in aPlace.Devices)
            {
                Device LDevice = new Device();
                LDevice.DeviceType = xDevice.DeviceType.GetValueOrDefault(0);
                LDevice.GUID = Guid.Parse(xDevice.GUID);
                LDevice.Url = xDevice.Url;
                LDevice.Name = xDevice.Name;
                LDevice.PlaceGUID = LPlace.GUID;
                LDevices.Add(LDevice);
            }
            LPlace.Devices = new DeviceLST(LDevices);

            return LPlace;
        }
    }
}
