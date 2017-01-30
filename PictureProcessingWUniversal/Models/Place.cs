using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureProcessingWUniversal.Models
{
    public class Place
    {
        public Guid GUID { get; set; }
        public Guid CustomerGUID { get; set; }
        public string Name { get; set; }
        public TimeSpan Timestamp { get; set; }
        public string LastPictureUrl { get; set; }
        public EventLST Events { get; set; }
        public DeviceLST Devices { get; set; }
        public int TimeZone { get; set; }
    }

}
