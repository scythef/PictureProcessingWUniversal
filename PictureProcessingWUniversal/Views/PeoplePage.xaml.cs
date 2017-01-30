using PictureProcessingWUniversal.Services;
using PictureProcessingWUniversal.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.FaceAnalysis;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System.Collections.ObjectModel;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureProcessingWUniversal.Views
{
        /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PeoplePage : Page
    {
        public PeopleViewModelLST PeopleVMLST { get; set; }
        ObservableCollection<CropViewModel> CropPictures = new ObservableCollection<CropViewModel>();

        public PeoplePage()
        {
            this.InitializeComponent();
            PeopleSVC.CheckCreatePersonGroup();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PeopleVMLST = App.varPeopleVMLST;
        }



        private void PeopleListView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private async void FindPictureButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // Application now has read/write access to the picked file 

                // Open a stream for the selected file. 
                // The 'using' block ensures the stream is disposed 
                // after the image is loaded. 
                using (Windows.Storage.Streams.IRandomAccessStream fileStream =
                    await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    // Set the image source to the selected bitmap. 
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(fileStream);
                    CreateFaceCrops(fileStream);

                    AdaptiveFacesGridView.ItemsSource = null;

                }
            }

        }

        private async void CreatePictureButton_Click(object sender, RoutedEventArgs e)
        {
            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Png;
            StorageFile photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);
            if (photo == null)
            {
                // User cancelled photo capture
                return;
            }

            IRandomAccessStream stream = await photo.OpenAsync(FileAccessMode.Read);

            CreateFaceCrops(stream);

            AdaptiveFacesGridView.ItemsSource = null;
        }

        private async void CreateFaceCrops(IRandomAccessStream aRAStream)   
        {
            try
            {
                ProgressRingControl.IsActive = true;

                WriteableBitmap bitmapImageEx = await new WriteableBitmap(1, 1).FromStream(aRAStream);
                List<BitmapBounds> lDetectedFaces = await GetFace(bitmapImageEx);
                PeoplePicture.Source = bitmapImageEx;

                CropPictures.Clear();
                if (lDetectedFaces.Count > 0)
                {
                    foreach (var lFace in lDetectedFaces)
                    {
                        Models.Crop lCrop = new Models.Crop();
                        lCrop.ImageSource = bitmapImageEx.Crop((int)lFace.X, (int)lFace.Y, (int)lFace.Width, (int)lFace.Height);
                        CropPictures.Add(new CropViewModel(lCrop));
                    }
                }
                AdaptiveCropGridView.ItemsSource = CropPictures;
                CropGrid.Visibility = Visibility.Visible;
            }
            finally
            {
                ProgressRingControl.IsActive = false;
            }
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


        private void PeopleListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PeopleVMLST.SelectedPeople != null)
            {
                AdaptiveFacesGridView.ItemsSource = PeopleVMLST.SelectedPeople.Faces;
            }
            else
            {
                AdaptiveFacesGridView.ItemsSource = null;
            }
        }

        private async void NewButton_Click(object sender, RoutedEventArgs e)
        {
            await PeopleVMLST.AddUser();
        }

        private async void AdaptiveCropGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                CropGrid.Visibility = Visibility.Collapsed;
                AdaptiveCropGridView.ItemsSource = null;
                PeoplePicture.Source = null;
                ProgressRingControl.IsActive = true;
                Guid Lfg = await PeopleSVC.AddFace(PeopleVMLST.SelectedPeople.GUID, PeopleVMLST.SelectedPeople.Name, ((e.ClickedItem as CropViewModel).ImageSource as WriteableBitmap));
                PeopleVMLST.SelectedPeople.AddCrop(PeopleVMLST.SelectedPeople.GUID, Lfg);
                AdaptiveFacesGridView.ItemsSource = PeopleVMLST.SelectedPeople.Faces;
            }
            finally
            {
                ProgressRingControl.IsActive = false;
            }
        }

        private void AdaptiveFacesGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                ProgressRingControl.IsActive = true;
                PeopleSVC.DeleteFace(PeopleVMLST.SelectedPeople.GUID, Guid.Parse((e.ClickedItem as CropViewModel).FaceID));
                PeopleVMLST.SelectedPeople.DeleteFace(e.ClickedItem as CropViewModel);
            }
            finally
            {
                ProgressRingControl.IsActive = false;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProgressRingControl.IsActive = true;
                PeopleVMLST.DeleteUser();
            }
            finally
            {
                ProgressRingControl.IsActive = false;
            }
        }
    }
}
