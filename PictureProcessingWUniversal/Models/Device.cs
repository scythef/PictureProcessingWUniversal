using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureProcessingWUniversal.Models
{
    public class Device
    {
        public Guid GUID { get; set; }
        public Guid PlaceGUID { get; set; }
        public string Name { get; set; }
        public TimeSpan Timestamp { get; set; }
        public int DeviceType { get; set; } //0 - Camera, 1 - Relay
        public string Url { get; set; }


        //Camera features
        //       public string LastPictureUrl { get; set; }

        //       public EventLST Events { get; set; }

        //       public string LiveCamUrl { get; set; }

        //Job analyzed camera
        //public int DiffLimit { get; set; } //0-100
        //public int PixelSquareSize { get; set; }

        //Relay features

        //public string UnlockUrl { get; set; }


    }

}
