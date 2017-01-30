using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureProcessingWUniversal.Models
{
    class PlaceLST
    {
        public List<Place> Places { get; set; }

        public PlaceLST(List<Place> aPlaces)
        {
            Places = aPlaces;
        }
    }
}
