using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureProcessingWUniversal.Models
{
    public class CamPictureLST
    {
        public List<CamPicture> Pictures { get; set; }

        public CamPictureLST(List<CamPicture> aPictures)
        {
            Pictures = aPictures;
        }

    }
}
