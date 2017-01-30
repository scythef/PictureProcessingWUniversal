using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace PictureProcessingWUniversal.Models
{
    public class Crop
    {
        public ImageSource ImageSource { get; set; }
        public string FaceID { get; set; }
        public string Url { get; set; }
    }
}
