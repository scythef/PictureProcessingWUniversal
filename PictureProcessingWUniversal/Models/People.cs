using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureProcessingWUniversal.Models
{
    public class People
    {
        public CropLST Crops { get; set; }

        public Guid GUID { get; set; }
        public Guid CustomerGUID { get; set; }
        public string Name { get; set; }
        //public TimeSpan Timestamp { get; set; }
        public DeviceLST Devices { get; set; }
        //public byte[] PictureArray { get; set; }
        //public string PictureUrl { get; set; }
    }
}
