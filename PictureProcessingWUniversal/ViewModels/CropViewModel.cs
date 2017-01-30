using PictureProcessingWUniversal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace PictureProcessingWUniversal.ViewModels
{
    public class CropViewModel : NotificationBase<Crop>
    {
        public CropViewModel(Crop aCrop = null) : base(aCrop)
        {

        }
        public ImageSource ImageSource
        {
            get { return This.ImageSource; }
            set
            {
                SetProperty(This.ImageSource, value, () => This.ImageSource = value);
            }
        }
        public string FaceID
        {
            get { return This.FaceID; }
            set
            {
                SetProperty(This.FaceID, value, () => This.FaceID = value);
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
