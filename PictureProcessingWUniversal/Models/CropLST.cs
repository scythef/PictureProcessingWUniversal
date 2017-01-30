using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureProcessingWUniversal.Models
{
    public class CropLST
    {
        public List<Crop> Crops { get; set; }

        public CropLST(List<Crop> aCrops)
        {
            if (aCrops == null)
            {
                Crops = new List<Crop>();
            }
            else
            {
                Crops = aCrops;
            }
        }

    }
}
