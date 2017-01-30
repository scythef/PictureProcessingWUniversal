using DoorPhoneIoTUIAPI;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.FaceAnalysis;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DoorPhoneIoTUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private IoTHubCommunicator _communicator;
        private bool varRunnning = false;
        private List<String> varStringList = new List<string>();
        private static readonly IFaceServiceClient faceServiceClient = new FaceServiceClient(Connections.ConnectionDict["FaceAPISubscriptionKey"]);
        private static string PersonGroupID = Connections.DeploymentDict["PersonGroupID"];
        private static string DeviceID = Connections.DeploymentDict["DeviceID"];
        private static DateTime LastSentHeartBeat = DateTime.MinValue;
        private static DateTime LastTime = DateTime.Now;
        private static DateTime NowTime = LastTime;
        WriteableBitmap bitmapImageEx;
        DoorPhoneIoTUIAPIClient lPPAPI = new DoorPhoneIoTUIAPIClient();
        int varCounter = 0;
        string varLastStr = "";

        public MainPage()
        {
            this.InitializeComponent();
            TBCameraUrl.Text = Connections.DeploymentDict["CameraUrl"];
            TBUnlockUrl.Text = Connections.DeploymentDict["UnlockUrl"];

            _communicator = new IoTHubCommunicator();
            _communicator.MessageReceivedEvent += _communicator_MessageReceivedEvent;
            _communicator.ReceiveDataFromAzure(); //start listening

            StartStopSwitch();
        }

        private void _communicator_MessageReceivedEvent(object sender, string e)
        {
            //update UI
            LogString(System.DateTime.Now.ToString() + " " + e);

            switch (e)
            {
                case "unlock": 
                    HttpClient client = new HttpClient(); // Create HttpClient
                    Unlock(client);
                    break;
            }

            //start listening again
            _communicator.ReceiveDataFromAzure();
        }

        private void Unlock(HttpClient aClient)
        {
            aClient.GetAsync(TBUnlockUrl.Text);
            _communicator.SendDataToAzure(JsonConvert.SerializeObject("unlocked"));
        }

        private static async Task<DeviceInformation> FindCameraDeviceByPanelAsync(Windows.Devices.Enumeration.Panel desiredPanel)
        {
            // Get available devices for capturing pictures
            var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            // Get the desired camera by panel
            DeviceInformation desiredDevice = allVideoDevices.FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == desiredPanel);
            // If there is no device mounted on the desired panel, return the first device found
            return desiredDevice ?? allVideoDevices.FirstOrDefault();
        }


        private async void RunningCycle()
        {
            try
            {
                TBCameraUrl.IsEnabled = false;

                CloudStorageAccount LCloudStorageAccount = CloudStorageAccount.Parse(Connections.ConnectionDict["BlobStorage"]);
                CloudBlobClient LBlobClient = LCloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer LBlobContainer = LBlobClient.GetContainerReference("facedetected");
                await LBlobContainer.CreateIfNotExistsAsync();

                CloudBlobContainer LBlobContainerFull = LBlobClient.GetContainerReference("pictures");
                await LBlobContainerFull.CreateIfNotExistsAsync();
                CloudBlockBlob LBlockBlobFull;

                CloudTableClient LTableClient = LCloudStorageAccount.CreateCloudTableClient();
                CloudTable LTable = LTableClient.GetTableReference("events");
                await LTable.CreateIfNotExistsAsync();

                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.Credentials = new NetworkCredential(TBName.Text, TBPassword.Text);
                HttpClient client = new HttpClient(clientHandler); // Create HttpClient

                BitmapImage b = new BitmapImage();

                HttpResponseMessage response;
                List<BitmapBounds> lDetectedFaces;
                string lStr;
                WriteableBitmap bmCrop;
                InMemoryRandomAccessStream lstream;
//                InMemoryRandomAccessStream lstreamFull;
//                Face[] xFaces;
//                Person lPerson;
                byte[] img;
                string lImgFileName;
                string lyyyyMMddHHmmssffff;
                string lUrl;

                string EventID = "";
                
                MediaCapture lmediaCapture = new MediaCapture();
                if (TBCameraUrl.Text == "")
                {
                    var lcameraDevice = await FindCameraDeviceByPanelAsync(Windows.Devices.Enumeration.Panel.Front);
                    var lsettings = new MediaCaptureInitializationSettings { VideoDeviceId = lcameraDevice.Id };
                    await lmediaCapture.InitializeAsync(lsettings);
                }

                while (varRunnning)
                {
                    lStr = System.DateTime.Now.ToString();
                    try
                    {
                        InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
                        DataWriter writer = new DataWriter(randomAccessStream.GetOutputStreamAt(0));
                        if (TBCameraUrl.Text == "")
                        {
                            InMemoryRandomAccessStream randomAccessStreamLocal = new InMemoryRandomAccessStream();
                            await lmediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), randomAccessStreamLocal);
                            var reader = new DataReader(randomAccessStreamLocal.GetInputStreamAt(0));
                            img = new byte[randomAccessStreamLocal.Size];
                            await reader.LoadAsync((uint)randomAccessStreamLocal.Size);
                            reader.ReadBytes(img);
                            writer.WriteBytes(img);
                            await writer.StoreAsync();
                        }
                        else
                        {
                            response = await client.GetAsync(TBCameraUrl.Text);
                            img = await response.Content.ReadAsByteArrayAsync();
                            writer.WriteBytes(img);
                            await writer.StoreAsync();
                        }

                        b.SetSource(randomAccessStream);

                        bitmapImageEx = await new WriteableBitmap(b.PixelWidth, b.PixelHeight).FromStream(randomAccessStream);

                        lyyyyMMddHHmmssffff = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_ffff");
                        lImgFileName = PersonGroupID + "/" + DeviceID + "/" + lyyyyMMddHHmmssffff + ".jpg";
                        LBlockBlobFull = LBlobContainerFull.GetBlockBlobReference(lImgFileName);
                        lUrl = LBlockBlobFull.Uri.ToString();

                        if (ChBHourlySnapshot.IsChecked == true)
                        {
                            LastTime = NowTime;
                            NowTime = DateTime.Now;
                            if (LastTime.Minute > NowTime.Minute)
                            {
                                randomAccessStream.Seek(0);
                                LBlockBlobFull.UploadFromStreamAsync(randomAccessStream.AsStream());

                                LogEventAndPicture(System.DateTime.Now.ToString() + " Hourly snapshot", Guid.NewGuid().ToString(), lyyyyMMddHHmmssffff, lImgFileName, lUrl, client, 1 /*Snapshot*/);
                            }
                        }


                        lDetectedFaces = await GetFace(bitmapImageEx);
                        if (lDetectedFaces.Count > 0)
                        {
                            lStr = System.DateTime.Now.ToString() + " Face detected";
                            _communicator.SendDataToAzure(JsonConvert.SerializeObject("facedetected"));

                            randomAccessStream.Seek(0);
                            LBlockBlobFull.UploadFromStreamAsync(randomAccessStream.AsStream());
                            foreach (var xFace in lDetectedFaces)
                            {
                                bmCrop = bitmapImageEx.Crop((int)xFace.X, (int)xFace.Y, (int)xFace.Width, (int)xFace.Height);
                                lstream = new InMemoryRandomAccessStream();
                                await bmCrop.ToStreamAsJpeg(lstream);
                                if (EventID == "")
                                {
                                    EventID = Guid.NewGuid().ToString();
                                }
                                ValidateFace(lStr, lstream, EventID, lyyyyMMddHHmmssffff, LBlobContainer, lImgFileName, lUrl, client);
                            }
                        }
                        else
                        {
                            //nothing happend
                            LogString(lStr);
                            EventID = "";
                        }
//                        randomAccessStream.Dispose();
                        if (TBCameraUrl.Text != "")
                        {
                            writer.Dispose();
                        }
                    }
                    catch (Exception e)
                    {
                        lStr = System.DateTime.Now.ToString() + " " + e.Message;
                        if (e is FaceAPIException)
                        {
                            lStr = lStr + "; " + (e as FaceAPIException).ErrorMessage;
                        }
                        LogString(lStr);
                    }
                    finally
                    {
                        if (LastSentHeartBeat.AddSeconds(Int32.Parse(TBHeartBeat.Text)) < DateTime.Now)
                        {
                            LastSentHeartBeat = DateTime.Now;
                            var telemetryDataPoint = new
                            {
                                deviceId = DeviceID,
                                datatype = 4, //heartbeat
                                datetime = LastSentHeartBeat.ToString("yyyy_MM_dd_HH_mm_ss_ffff"),
                                notice = "heart beat"
                            };
                            _communicator.SendDataToAzure(JsonConvert.SerializeObject(telemetryDataPoint));
                            LogString(LastSentHeartBeat.ToString() + " Heart beat");
                        }
                        GC.Collect();
                    }
                }
            }
            finally
            {
                TBCameraUrl.IsEnabled = true;
            }
        }

        private async void LogString(string aStr)
        {
            if (ChBLog.IsChecked == true)
            {
                if (varLastStr == aStr)
                {
                    varCounter++;
                    varStringList[0] = aStr + " " + varCounter.ToString() + "x";
                }
                else
                {
                    varCounter = 1;
                    varStringList.Insert(0, aStr + " " + varCounter.ToString() + "x");
                }
                varLastStr = aStr;
                if (varStringList.Count() > 100)
                {
                    varStringList.RemoveRange(100, varStringList.Count() - 100);
                }
                TBLog.Text = String.Join(((char)13).ToString(), varStringList.ToArray());
            }
        }

        private async void ValidateFace(string aStr, InMemoryRandomAccessStream aStream, string aEventID, string ayyyyMMddHHmmssffff, CloudBlobContainer aBlobContainer, string aCapturedImgFileName, string aCapturedImgUrl, HttpClient aClient)
        {
            CloudBlockBlob lBlockBlob = aBlobContainer.GetBlockBlobReference(DeviceID + aEventID + ayyyyMMddHHmmssffff + ".jpg");
            await lBlockBlob.UploadFromStreamAsync(aStream.AsStream());
            LogEventAndPicture(aStr, aEventID, ayyyyMMddHHmmssffff, aCapturedImgFileName, aCapturedImgUrl, aClient, 0 /*Face Recognized*/);
        }

        private async void LogEventAndPicture(string aStr, string aEventID, string ayyyyMMddHHmmssffff, string aCapturedImgFileName, string aCapturedImgUrl, HttpClient aClient, int aEventType)
        {
            string lStr = aStr;
            var x = await lPPAPI.CreateEventWithOperationResponseAsync(aEventID, DeviceID, PersonGroupID, ayyyyMMddHHmmssffff, aEventType, new System.Threading.CancellationToken());

            switch (aEventType)
            {
                case 0: //facerecognized
                    _communicator.SendDataToAzure(JsonConvert.SerializeObject("facerecognized: " + x.Body.RecognizedPersonName));
                    if ((x.Response.StatusCode == HttpStatusCode.OK) && (x.Body.Confidence > 0))
                    {
                        lStr = System.DateTime.Now.ToString() + " " + x.Body.Message + "; " + x.Body.ExceptionMessage;
                        Unlock(aClient);
                    }
                    break;
                case 1: //snapshot
                    _communicator.SendDataToAzure(JsonConvert.SerializeObject("snapshot"));
                    break;
            }
            lPPAPI.CreatePictureWithOperationResponseAsync(aEventID, DeviceID, ayyyyMMddHHmmssffff, aCapturedImgFileName, aCapturedImgUrl, new System.Threading.CancellationToken());

            LogString(lStr);
        }


        private void StartStop_Click(object sender, RoutedEventArgs e)
        {
            StartStopSwitch();
        }

        private void StartStopSwitch()
        {
            varRunnning = !varRunnning;
            if (varRunnning) { StartStop.Content = "Stop"; } else { StartStop.Content = "Start"; }
            RunningCycle();
        }

        private async Task<List<BitmapBounds>> GetFace(WriteableBitmap aWBitMapEx)
        {
            FaceDetector faceDetector;
            var lstream = new InMemoryRandomAccessStream();
            await aWBitMapEx.ToStreamAsJpeg(lstream);

            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(lstream);
            SoftwareBitmap image = await decoder.GetSoftwareBitmapAsync(decoder.BitmapPixelFormat, BitmapAlphaMode.Premultiplied);

            const BitmapPixelFormat faceDetectionPixelFormat = BitmapPixelFormat.Gray8;
            if (image.BitmapPixelFormat != faceDetectionPixelFormat)
            {
                image = SoftwareBitmap.Convert(image, faceDetectionPixelFormat);
            }

            faceDetector = await FaceDetector.CreateAsync();
            IEnumerable<DetectedFace> detectedFaces = await faceDetector.DetectFacesAsync(image);

            List<BitmapBounds> lDetectedFaces = new List<BitmapBounds>();
            foreach (DetectedFace face in detectedFaces)
            {
                BitmapBounds Lbmb = new BitmapBounds();
                Lbmb = EnlargeFaceBoxSize(face, image);
                lDetectedFaces.Add(Lbmb);
            }

            return lDetectedFaces;
        }

        private static BitmapBounds EnlargeFaceBoxSize(DetectedFace face, SoftwareBitmap image)
        {
            BitmapBounds Lbmb = new BitmapBounds();

            int FaceImageBoxPaddingPercentage = 50;
            Lbmb.Width = face.FaceBox.Width;
            Lbmb.Height = face.FaceBox.Height;
            uint paddingWidth = (uint)(face.FaceBox.Width * FaceImageBoxPaddingPercentage / 100);
            uint paddingHeight = (uint)(face.FaceBox.Height * FaceImageBoxPaddingPercentage / 100);
            Lbmb.X = face.FaceBox.X;
            Lbmb.Y = face.FaceBox.Y;

            if (Lbmb.X >= paddingWidth)
            {
                Lbmb.X = (Lbmb.X - paddingWidth);
                Lbmb.Width = (Lbmb.Width + paddingWidth);
            }
            else
            {
                Lbmb.Width = Lbmb.Width + Lbmb.X;
                Lbmb.X = 0;
            }

            if (Lbmb.Y >= paddingHeight)
            {
                Lbmb.Y = (Lbmb.Y - paddingHeight);
                Lbmb.Height = (Lbmb.Height + paddingHeight);
            }
            else
            {
                Lbmb.Height = (Lbmb.Height + paddingHeight);
                Lbmb.Y = 0;
            }

            if (image.PixelWidth >= Lbmb.X + Lbmb.Width + paddingWidth)
            {
                Lbmb.Width = (Lbmb.Width + paddingWidth);
            }
            else
            {
                Lbmb.Width = ((uint)image.PixelWidth - Lbmb.X);
            }

            if (image.PixelHeight >= Lbmb.Y + Lbmb.Height + paddingHeight)
            {
                Lbmb.Height = (Lbmb.Height + paddingHeight);
            }
            else
            {
                Lbmb.Height = ((uint)image.PixelHeight - Lbmb.Y);
            }
            return Lbmb;
        }

        private void Snapshot_Click(object sender, RoutedEventArgs e)
        {
            Camera.Source = bitmapImageEx;
        }

        private void TBHeartBeat_TextChanged(object sender, TextChangedEventArgs e)
        {
            int lTry;
            if (!Int32.TryParse(TBHeartBeat.Text, out lTry))
            {
                TBHeartBeat.Text = "60";
            };
        }
    }
}
