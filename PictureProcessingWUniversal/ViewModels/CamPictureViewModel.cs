using PictureProcessingWUniversal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace PictureProcessingWUniversal.ViewModels
{
    public class CamPictureViewModel : NotificationBase<CamPicture>
    {
        public CamPictureViewModel(CamPicture aPicture = null) : base(aPicture)
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
        public String Time
        {
            get { return This.Time; }
            set
            {
                SetProperty(This.Time, value, () => This.Time = value);
            }
        }

        public String Attributes
        {
            get { return This.Attributes; }
            set
            {
                SetProperty(This.Attributes, value, () => This.Attributes = value);
            }
        }

    }

}
