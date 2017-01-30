using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureProcessingWUniversal.Models
{
    public class DeviceLST
    {
        public List<Device> Devices { get; set; }

        public DeviceLST(List<Device> aDevices)
        {
            Devices = aDevices;
        }

    }
}
