using PictureProcessingWUniversal.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureProcessingWUniversal.ViewModels
{
    public class PeopleViewModel : NotificationBase<People>
    {
        public DeviceViewModelLST DeviceVMLST;
        public DeviceLST DeviceLST;
        public CropLST FaceLST;
        public People People;

        public PeopleViewModel(People aPeople = null) : base(aPeople)
        {
            People = aPeople;
            if (aPeople.Devices != null)
            {
                if (aPeople.Devices.Devices != null)
                {
                    DeviceVMLST = new DeviceViewModelLST(aPeople.Devices.Devices);
                    DeviceLST = DeviceVMLST.DeviceLST;
                    // Load Devices 
                    foreach (var lDevice in DeviceLST.Devices)
                    {
                        var np = new DeviceViewModel(lDevice);
                        _Devices.Add(np);
                    }
                }
            }

            if (aPeople.Crops == null)
            {
                aPeople.Crops = new CropLST(new List<Crop>());
            } 

            FaceLST = new CropLST(aPeople.Crops.Crops);
            // Load faces
            foreach (var lFace in FaceLST.Crops)
            {
                var np = new CropViewModel(lFace);
                _Faces.Add(np);
            }
        }

        public void AddCrop(Guid aPersonGuid, Guid aFaceGuid)
        {
            Crop LCrop = new Crop();
            LCrop.FaceID = aFaceGuid.ToString();
            LCrop.Url = Services.PeopleSVC.GetCropFileUri(aPersonGuid,aFaceGuid);
            FaceLST.Crops.Add(LCrop);
            Faces.Add(new CropViewModel(LCrop));
        }

        public void DeleteFace(CropViewModel aCropVM)
        {
            FaceLST.Crops.RemoveAt(Faces.IndexOf(aCropVM));
            Faces.Remove(aCropVM);
        }

        ObservableCollection<CropViewModel> _Faces = new ObservableCollection<CropViewModel>();
        public ObservableCollection<CropViewModel> Faces
        {
            get { return _Faces; }
            set { SetProperty(ref _Faces, value); }
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
    }
}
