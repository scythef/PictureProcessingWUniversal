using PictureProcessingWUniversal.Models;
using PictureProcessingWUniversal.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureProcessingWUniversal.ViewModels
{
    public class PlaceViewModelLST : NotificationBase
    {
        PlaceLST PlaceLST;

        public PlaceViewModelLST(List<Place> aPlaces)
        {
            PlaceLST = new PlaceLST(aPlaces);
            _SelectedIndex = -1;
            // Load the database 
            foreach (var lPlace in PlaceLST.Places)
            {
                var np = new PlaceViewModel(lPlace);
                np.PropertyChanged += Place_OnNotifyPropertyChanged;
                _Places.Add(np);
            }
            if (PlaceLST.Places.Count > 0)
            {
                _SelectedIndex = 0;
            }
        }

        ObservableCollection<PlaceViewModel> _Places = new ObservableCollection<PlaceViewModel>();
        public ObservableCollection<PlaceViewModel> Places
        {
            get { return _Places; }
            set { SetProperty(ref _Places, value); }
        }

        int _SelectedIndex;
        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set
            {
                if (SetProperty(ref _SelectedIndex, value))
                { RaisePropertyChanged(nameof(SelectedPlace)); }
            }
        }

        public PlaceViewModel SelectedPlace
        {
            get { return (_SelectedIndex >= 0) ? _Places[_SelectedIndex] : null; }
        }

        //to implement in case of need
        public async void Add()
        {
            Place lPlace = new Place();
            lPlace.Name = "New place";
            lPlace.Devices = new DeviceLST(new List<Device>());
            PlaceViewModel lPlaceVM = new PlaceViewModel(lPlace);
            Guid lPlaceGuid = await PlaceSVC.CreatePlace(lPlaceVM.Name);
            lPlaceVM.GUID = lPlaceGuid;
            lPlaceVM.CustomerGUID = Guid.Parse(App.varCustomerGUID);
            lPlaceVM.PropertyChanged += Place_OnNotifyPropertyChanged;
            Places.Add(lPlaceVM);
            //PlaceLST.Add(lPlaceVM);
            SelectedIndex = Places.IndexOf(lPlaceVM);
        }


        public void Delete()
        {
            if (SelectedIndex != -1)
            {
                var lPlaceVM = Places[SelectedIndex];
                Places.RemoveAt(SelectedIndex);
                //PlaceLST.Delete(lPlaceVM);
            }
        }


        void Place_OnNotifyPropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            PlaceSVC.UpdatePlace(sender as PlaceViewModel);
        }



    }

}
