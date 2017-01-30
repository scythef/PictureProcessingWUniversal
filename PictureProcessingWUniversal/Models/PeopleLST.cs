using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureProcessingWUniversal.Models
{
    class PeopleLST
    {
        public List<People> People { get; set; }

        public PeopleLST(List<People> aPeople)
        {
            People = aPeople;
        }

    }
}
