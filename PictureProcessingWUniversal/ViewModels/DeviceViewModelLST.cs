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

    public class DeviceViewModelLST : NotificationBase
    {
        public DeviceLST DeviceLST;

        public DeviceViewModelLST(List<Device> aDevices)
        {
            DeviceLST = new DeviceLST(aDevices);
            _SelectedIndex = -1;
            // Load the database 
            foreach (var lDevice in DeviceLST.Devices)
            {
                var np = new DeviceViewModel(lDevice);
                np.PropertyChanged += Device_OnNotifyPropertyChanged;
                _Devices.Add(np);
            }
        }

        ObservableCollection<DeviceViewModel> _Devices = new ObservableCollection<DeviceViewModel>();
        public ObservableCollection<DeviceViewModel> Devices
        {
            get { return _Devices; }
            set { SetProperty(ref _Devices, value); }
        }

        int _SelectedIndex;
        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set
            {
                if (SetProperty(ref _SelectedIndex, value))
                { RaisePropertyChanged(nameof(SelectedDevice)); }
            }
        }

        public DeviceViewModel SelectedDevice
        {
            get { return (_SelectedIndex >= 0) ? _Devices[_SelectedIndex] : null; }
        }

        //to implement in case of need
        public async void Add(Guid aPlaceGuid, int aDeviceType)
        {
            Device lDevice = new Device();
            lDevice.DeviceType = aDeviceType;
            if (aDeviceType == 0)
            {
                lDevice.Name = "New camera";
            }
            else
            {
                lDevice.Name = "New relay";
            }
            DeviceViewModel lDeviceVM = new DeviceViewModel(lDevice);
            Guid lDeviceGuid = await DeviceSVC.CreateDevice(lDeviceVM.Name, aPlaceGuid, aDeviceType);
            lDeviceVM.GUID = lDeviceGuid;
            lDeviceVM.PlaceGUID = aPlaceGuid;
            lDeviceVM.PropertyChanged += Device_OnNotifyPropertyChanged;
            Devices.Add(lDeviceVM);
            SelectedIndex = Devices.IndexOf(lDeviceVM);
        }


        public void Delete()
        {
            if (SelectedIndex != -1)
            {
                var lDeviceVM = Devices[SelectedIndex];
                Devices.RemoveAt(SelectedIndex);
            }
        }


        void Device_OnNotifyPropertyChanged(Object sender, PropertyChangedEventArgs e)
        {

        }



    }

}
