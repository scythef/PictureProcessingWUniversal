using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using PictureProcessingAPI.Models;
using PictureProcessingWUniversal.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace PictureProcessingWUniversal.Services
{
    class PictureSVC
    {
        private static string connectionstring = Connections.ConnectionDict["BlobStorage"];

        public static async Task<CamPicture> GetPicture(PictureAPI aPicture)
        {
            CamPicture LPicture = new CamPicture();

            LPicture.ImageSource = new BitmapImage(new Uri(aPicture.Url));
            LPicture.Time = DateTime.ParseExact(aPicture.YyyyMMddHHmmssffff, "yyyy_MM_dd_HH_mm_ss_ffff", CultureInfo.InvariantCulture).TimeOfDay.ToString();
            LPicture.Attributes = "";
            return LPicture;
        }
    }
}
