using PictureProcessingWUniversal.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureProcessingWUniversal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public PlaceViewModelLST PlaceVMLST { get; set; } //linked to XAML

        public SettingsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PlaceVMLST = App.varPlaceVMLST;
            DetailFrame.IsEnabled = PlaceVMLST.Places.Count() > 0;
        }


        private void PlaceListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            foreach (PlaceViewModel xPVM in PlaceVMLST.Places)
            {
                if (xPVM.GUID == (e.ClickedItem as PlaceViewModel).GUID)
                {
                    if (xPVM.DeviceVMLST.Devices.Count > 0)
                    {
                        xPVM.DeviceVMLST.SelectedIndex = 0;
                    }
                    else
                    {
                        xPVM.DeviceVMLST.SelectedIndex = -1;
                    }
                }
                else
                {
                    xPVM.DeviceVMLST.SelectedIndex = -1;
                }
            }
        }

        private void DeviceListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            foreach (PlaceViewModel xPVM in PlaceVMLST.Places)
            {
                if (xPVM.GUID == (e.ClickedItem as DeviceViewModel).PlaceGUID)
                {
                    PlaceVMLST.SelectedIndex = PlaceVMLST.Places.IndexOf(xPVM);
                }
                else
                {
                    xPVM.DeviceVMLST.SelectedIndex = -1;
                }
            }
        }

        private void PlaceButton_Click(object sender, RoutedEventArgs e)
        {
            PlaceVMLST.Add();
        }

        private void CameraButton_Click(object sender, RoutedEventArgs e)
        {
            PlaceVMLST.SelectedPlace.DeviceVMLST.Add(PlaceVMLST.SelectedPlace.GUID, 0);
        }

        private void RelayButton_Click(object sender, RoutedEventArgs e)
        {
            PlaceVMLST.SelectedPlace.DeviceVMLST.Add(PlaceVMLST.SelectedPlace.GUID, 1);
        }

    }
}
