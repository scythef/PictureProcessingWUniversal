using PictureProcessingWUniversal.Models;
using PictureProcessingWUniversal.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureProcessingWUniversal.ViewModels
{
    public class PlaceViewModel : NotificationBase<Place>
    {
        public DeviceViewModelLST DeviceVMLST;
        public DeviceLST DeviceLST;
        EventLST EventLST;


        public PlaceViewModel(Place aPlace = null) : base(aPlace)
        {
            if (aPlace.Devices != null)
            {
                if (aPlace.Devices.Devices != null)
                {
                    DeviceVMLST = new DeviceViewModelLST(aPlace.Devices.Devices);
                    DeviceLST = DeviceVMLST.DeviceLST;
                    // Load Devices 
                    foreach (var lDevice in DeviceLST.Devices)
                    {
                        var np = new DeviceViewModel(lDevice);
                        _Devices.Add(np);
                    }
                }
            }

            if (aPlace.Events != null)
            {
                if (aPlace.Events.Events != null)
                {
                    EventLST = new EventLST("", aPlace.Events.Events);
                    // Load Events 
                    foreach (var lEvent in EventLST.Events)
                    {
                        var np = new EventViewModel(lEvent);
                        _Events.Add(np);
                    }
                }
            }

        }

        public async void RefreshEvents()
        {
            List<Event> lEvents = await EventSVC.GetEvents(App.varActualDate, App.varPlaceVMLST.SelectedPlace.GUID);
            _Events.Clear();
            // Load Events 
            foreach (var xEvent in lEvents)
            {
                var np = new EventViewModel(xEvent);
                _Events.Add(np);
            }
        }

        ObservableCollection<EventViewModel> _Events = new ObservableCollection<EventViewModel>();
        public ObservableCollection<EventViewModel> Events
        {
            get { return _Events; }
            set { SetProperty(ref _Events, value); }
        }

        ObservableCollection<DeviceViewModel> _Devices = new ObservableCollection<DeviceViewModel>();
        public ObservableCollection<DeviceViewModel> Devices
        {
            get { return _Devices; }
            set { SetProperty(ref _Devices, value); }
        }
        
        public Guid GUID
        {
            get { return This.GUID; }
            set
            {
                SetProperty(This.GUID, value, () => This.GUID = value);
            }
        }
        public Guid CustomerGUID
        {
            get { return This.CustomerGUID; }
            set
            {
                SetProperty(This.CustomerGUID, value, () => This.CustomerGUID = value);
            }
        }
        public string Name
        {
            get { return This.Name; }
            set
            {
                SetProperty(This.Name, value, () => This.Name = value);
            }
        }
        public TimeSpan Timestamp
        {
            get { return This.Timestamp; }
            set
            {
                SetProperty(This.Timestamp, value, () => This.Timestamp = value);
            }
        }
        public string LastPictureUrl
        {
            get { return This.LastPictureUrl; } //to do read from children
        }
        public int TimeZone 
        {
            get { return This.TimeZone; }
            set
            {
                SetProperty(This.TimeZone, value, () => This.TimeZone = value);
            }
        }

    }

}
