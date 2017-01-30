using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace PictureProcessingWUniversal.Models
{
    public class Event
    {
        public CamPictureLST Pictures { get; set; }

        public ImageSource ImageSource { get; set; }

        public String Icon { get; set; }

        public string Name { get; set; }

        public string Time { get; set; }

//        public int PicCount { get; set; }

    }
}
