using PictureProcessingWUniversal.Models;
using PictureProcessingWUniversal.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace PictureProcessingWUniversal.ViewModels
{

    public class DeviceViewModel : NotificationBase<Device>
    {
        EventLST EventLST;

        public DeviceViewModel(Device aDevice = null) : base(aDevice)
        {

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

        public Guid GUID
        {
            get { return This.GUID; }
            set
            {
                SetProperty(This.GUID, value, () => This.GUID = value);
            }
        }
        public Guid PlaceGUID
        {
            get { return This.PlaceGUID; }
            set
            {
                SetProperty(This.PlaceGUID, value, () => This.PlaceGUID = value);
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
        public int DeviceType //0 - Camera, 1 - Relay
        {
            get { return This.DeviceType; }
            set
            {
                SetProperty(This.DeviceType, value, () => This.DeviceType = value);
            }
        }

        public Stretch PictureStretch
        {
            get
            {
                if (this.DeviceType == 0)
                {
                    return Stretch.UniformToFill;
                }
                else
                {
                    return Stretch.None;
                }
            }
        }

        public String DeviceButtonPictureUrl
        {
            get
            {
                if (this.DeviceType == 0)
                {
                    if (this.Url != null)
                    {
                        return this.Url;
                    }
                    else
                    {
                        return "ms-appx:///Assets/Icons/camera64.png";
                    }
                }
                else
                {
                    if (this.Url != null)
                    {
                        return "ms-appx:///Assets/Icons/unlock64.png";
                    }
                    else
                    {
                        return "ms-appx:///Assets/Icons/door64.png";
                    }
                }
            }
        }
        public String DeviceButtonClickUrl
        {
            get
            {
                if (this.DeviceType == 0)
                {
                    if (this.Url != null)
                    {
                        return this.Url;
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    if (this.Url != null)
                    {
                        return this.Url;
                    }
                    else
                    {
                        return "";
                    }
                }
            }
        }

        public String Icon
        {
            get
            {
                if (this.DeviceType == 0)
                {
                    return "ms-appx:///Assets/Icons/camera64.png";
                }
                else
                {
                    return "ms-appx:///Assets/Icons/door64.png";
                }

            } 
        }
        public string Url
        {
            get { return This.Url; }
            set
            {
                SetProperty(This.Url, value, () => This.Url = value);
            }
        }

    }

}
