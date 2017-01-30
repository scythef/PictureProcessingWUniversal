using PictureProcessingWUniversal.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace PictureProcessingWUniversal.ViewModels
{
    public class EventViewModel : NotificationBase<Event>
    {
        CamPictureLST PictureLST;

        public EventViewModel(Event aEvent = null) : base(aEvent)
        {
            PictureLST = new CamPictureLST(aEvent.Pictures.Pictures);
            // Load pictures 
            foreach (var lPicture in PictureLST.Pictures)
            {
                var np = new CamPictureViewModel(lPicture);
                _Pictures.Add(np);
            }

        }

        ObservableCollection<CamPictureViewModel> _Pictures = new ObservableCollection<CamPictureViewModel>();
        public ObservableCollection<CamPictureViewModel> Pictures
        {
            get { return _Pictures; }
            set { SetProperty(ref _Pictures, value); }
        }
        public String Icon
        {
            get { return This.Icon; }
            set
            {
                SetProperty(This.Icon, value, () => This.Icon = value);
                //                RaisePropertyChanged("RCZK");
            }
        }
        public ImageSource ImageSource
        {
            get { return This.ImageSource; }
            set
            {
                SetProperty(This.ImageSource, value, () => This.ImageSource = value);
            }
        }
        public int PicCount
        {
            get { return This.Pictures.Pictures.Count; }
        }

        public String Time
        {
            get { return This.Time; }
            set
            {
                SetProperty(This.Time, value, () => This.Time = value);
            }
        }
        public String Name
        {
            get { return This.Name; }
            set
            {
                SetProperty(This.Name, value, () => This.Name = value);
            }
        }

        public String Text
        {
            get { return This.Time; }
        }

        public String Detail
        {
            get
            {
                String lstr = This.Pictures.Pictures.Count.ToString() + "x";// + This.Name;
                return lstr;
            }
        }
        public override string ToString()
        {
            String lstr = This.Time + ", " + This.Pictures.Pictures.Count.ToString() + "x";// + This.Name;
            return lstr;
        }
        public String AsString
        {
            get
            {
                String lstr = This.Time + ", " + This.Pictures.Pictures.Count.ToString() + "x";// + This.Name;
                return lstr;
            }
        }
    }

}
