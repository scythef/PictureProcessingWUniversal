using PictureProcessingWUniversal.Services;
using PictureProcessingWUniversal.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureProcessingWUniversal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EventsPage : Page
    {
        public PlaceViewModel PlaceVM { get; set; }
        public DeviceViewModelLST DeviceVMLST { get; set; }
        public EventsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e != null)
            {
                PlaceVM = e.Parameter as PlaceViewModel;
                DeviceVMLST = new DeviceViewModelLST(PlaceVM.DeviceLST.Devices);

                RefreshEvents();
                AdaptiveGridView.ItemsSource = PlaceVM.Events;

            }
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            LivePicture.DataContext = (e.ClickedItem as DeviceViewModel);
            if ((e.ClickedItem as DeviceViewModel).DeviceType == 1 /*Relay*/)
            {
                DeviceSVC.SendCloudToDeviceMessageAsync((e.ClickedItem as DeviceViewModel).GUID.ToString(), "unlock");
            }

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((LivePicture.DataContext as DeviceViewModel).DeviceType == 0)
            {
                await Launcher.LaunchUriAsync(new Uri((LivePicture.DataContext as DeviceViewModel).DeviceButtonClickUrl));
            }
            else
            {
                HttpClient httpClient = new HttpClient();
                string responseBodyAsText = "";
                try
                {
                    responseBodyAsText = await httpClient.GetStringAsync((LivePicture.DataContext as DeviceViewModel).DeviceButtonClickUrl);
                }
                catch
                {

                }
            }


        }

        private void AdaptiveGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            AdaptiveGridViewPictures.ItemsSource = (e.ClickedItem as EventViewModel).Pictures;
            
        }

        private async void AdaptiveGridViewPictures_ItemClick(object sender, ItemClickEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(((e.ClickedItem as CamPictureViewModel).ImageSource as BitmapImage).UriSource.AbsoluteUri));
        }

        private void ButtonPrevious_Click(object sender, RoutedEventArgs e)
        {
            App.varActualDate = App.varActualDate.AddDays(-1);
            RefreshEvents();
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            App.varActualDate = App.varActualDate.AddDays(1);
            RefreshEvents();
        }

        private void RefreshEvents()
        {
            SetDateFormatByWindowSize();
            PlaceVM.RefreshEvents();
        }

        private void SetDateFormatByWindowSize()
        {
            if (this.ActualWidth > 720)
            {
                ActualDate.Text = App.varActualDate.Date.ToString("D");
            }
            else
            {
                ActualDate.Text = App.varActualDate.Date.ToString("d");
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetDateFormatByWindowSize();
        }
    }
}
