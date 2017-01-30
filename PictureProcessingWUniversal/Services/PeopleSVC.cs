using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using PictureProcessingWUniversal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace PictureProcessingWUniversal.Services
{
    class PeopleSVC
    {
        private static readonly IFaceServiceClient faceServiceClient = new FaceServiceClient(Connections.ConnectionDict["FaceAPISubscriptionKey"]);
        private static string connectionstring = Connections.ConnectionDict["BlobStorage"];


        public static async void CheckCreatePersonGroup()
        {
            try
            {
                PersonGroup lPersonG = await faceServiceClient.GetPersonGroupAsync(App.varCustomerGUID.ToLower());
            }
            catch (FaceAPIException e)
            {
                if (e.ErrorCode == "PersonGroupNotFound")
                {
                    await faceServiceClient.CreatePersonGroupAsync(App.varCustomerGUID.ToLower(), App.varCustomerGUID.ToLower());
                }
                else
                {
                    throw;
                }
            }
        }

        public static async void DeletePerson(Guid aPersonGuid)
        {
            await faceServiceClient.DeletePersonAsync(App.varCustomerGUID.ToLower(), aPersonGuid);
        }

        public static async Task<Guid> CreateUpdatePerson(Guid aPersonGuid, string aName)
        {
            Guid lOutputPersonGuid; //null == 0000000000
            if (aPersonGuid == lOutputPersonGuid)
            {
                CreatePersonResult lResult = await faceServiceClient.CreatePersonAsync(App.varCustomerGUID.ToLower(), aName);
                lOutputPersonGuid = lResult.PersonId;
            }
            else
            {
                Person lPerson = await faceServiceClient.GetPersonAsync(App.varCustomerGUID.ToLower(), aPersonGuid);
                await faceServiceClient.UpdatePersonAsync(App.varCustomerGUID, aPersonGuid, aName);
                lOutputPersonGuid = aPersonGuid;
            }

            return lOutputPersonGuid;
        }

        public static string GetCropFileName (Guid aPersonGuid, Guid aFaceGuid)
        {
            return App.varCustomerGUID + "_" + aPersonGuid.ToString() + "_" + aFaceGuid.ToString() + ".jpg";
        }

        public static string GetCropFileUri(Guid aPersonGuid, Guid aFaceGuid)
        {
            CloudStorageAccount LCloudStorageAccount = CloudStorageAccount.Parse(connectionstring);
            CloudBlobClient LBlobClient = LCloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer LBlobContainer = LBlobClient.GetContainerReference("people");
            CloudBlockBlob LBlockBlob = LBlobContainer.GetBlockBlobReference(GetCropFileName(aPersonGuid, aFaceGuid));
            return LBlockBlob.Uri.ToString();
        }

        public static void DeleteFace(Guid aPersonGuid, Guid aFaceGuid)
        {
            CloudStorageAccount LCloudStorageAccount = CloudStorageAccount.Parse(connectionstring);
            CloudBlobClient LBlobClient = LCloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer LBlobContainer = LBlobClient.GetContainerReference("people");
            CloudBlockBlob LBlockBlob = LBlobContainer.GetBlockBlobReference(GetCropFileName(aPersonGuid, aFaceGuid));
            faceServiceClient.DeletePersonFaceAsync(App.varCustomerGUID, aPersonGuid, aFaceGuid);
            faceServiceClient.TrainPersonGroupAsync(App.varCustomerGUID);
            LBlockBlob.DeleteIfExistsAsync();
        }
        public static async Task<Guid> AddFace(Guid aGuid, string aName, WriteableBitmap abitmap)
        {
            try
            {
                CloudStorageAccount LCloudStorageAccount = CloudStorageAccount.Parse(connectionstring);
                CloudBlobClient LBlobClient = LCloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer LBlobContainer = LBlobClient.GetContainerReference("people");
                await LBlobContainer.CreateIfNotExistsAsync();
                CloudBlockBlob LBlockBlob = LBlobContainer.GetBlockBlobReference(App.varCustomerGUID+"_"+aGuid.ToString()+".jpg");
                var lstream = new InMemoryRandomAccessStream();
                await abitmap.ToStreamAsJpeg(lstream);
                await LBlockBlob.UploadFromStreamAsync(lstream.AsStream());
                AddPersistedFaceResult x = await faceServiceClient.AddPersonFaceAsync(App.varCustomerGUID, aGuid, LBlockBlob.Uri.ToString());
                CloudBlockBlob LBlockBlobFinal = LBlobContainer.GetBlockBlobReference(GetCropFileName(aGuid, x.PersistedFaceId));
                await LBlockBlobFinal.StartCopyAsync(LBlockBlob);
                LBlockBlob.DeleteAsync();
                faceServiceClient.TrainPersonGroupAsync(App.varCustomerGUID);
                return x.PersistedFaceId;
            }
            catch (FaceAPIException e)
            {
                var Dialog = new MessageDialog(e.ErrorMessage);
                await Dialog.ShowAsync();
                throw;
            }
        }


        public static async Task<List<People>> GetPeople(string aCustomerGUID)
        {

            CheckCreatePersonGroup();
            List<People> LPeoList = new List<People>();
            try
            {
                Person[] lPersons = await faceServiceClient.GetPersonsAsync(App.varCustomerGUID.ToLower());
                foreach (Person xPeople in lPersons)
                {
                    People Lpx = GetPeople(xPeople);
                    LPeoList.Add(Lpx);
                }
            }
            catch (FaceAPIException e)
            {
                throw;
            }
            return LPeoList;
        }

        public static People GetPeople(Person aPeople)
        {
            People LPeople = new People();
            LPeople.Name = aPeople.Name;
            LPeople.CustomerGUID = Guid.Parse(App.varCustomerGUID);
            LPeople.GUID = aPeople.PersonId;
            List<Crop> LCropList = new List<Crop>();
            foreach (Guid xPersistedFaceGUID in aPeople.PersistedFaceIds)
            {
                Crop Lcx = GetCrop(aPeople.PersonId, xPersistedFaceGUID);
                LCropList.Add(Lcx);
            }
            LPeople.Crops = new CropLST(LCropList);
            return LPeople;
        }

        public static Crop GetCrop(Guid aPersonGuid, Guid aPersistedFaceGuid)
        {
            CloudStorageAccount LCloudStorageAccount = CloudStorageAccount.Parse(connectionstring);
            CloudBlobClient LBlobClient = LCloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer LBlobContainer = LBlobClient.GetContainerReference("people");
            CloudBlockBlob LBlockBlob = LBlobContainer.GetBlockBlobReference(GetCropFileName(aPersonGuid, aPersistedFaceGuid));

            Crop LCrop = new Crop();
            LCrop.FaceID = aPersistedFaceGuid.ToString();
            LCrop.Url = LBlockBlob.Uri.ToString();
            return LCrop;
        }

    }
}
