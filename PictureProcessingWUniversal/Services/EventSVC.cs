using PictureProcessingAPI;
using PictureProcessingAPI.Models;
using PictureProcessingWUniversal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace PictureProcessingWUniversal.Services
{
    class EventSVC
    {
        public static async Task<List<Event>> GetEvents(DateTime aDateTime)
        {
            PictureProcessingAPIClient lPPAPI = new PictureProcessingAPIClient();
            var x = await lPPAPI.GetEventsByDayWithOperationResponseAsync(aDateTime.ToString("yyyy_MM_dd"), new System.Threading.CancellationToken());

            IList<EventAPI> Lx = (x as Microsoft.Rest.HttpOperationResponse<IList<EventAPI>>).Body;

            List<Event> LEvList = new List<Event>();

            foreach(var xEvent in Lx)
            {
                Event lEvent = new Event();
                lEvent.Time = xEvent.EventTime.Value.TimeOfDay.ToString();
                List<CamPicture> lPicList = new List<CamPicture>();
                foreach(var xPic in xEvent.Pictures)
                {
                    CamPicture lCamPic = await PictureSVC.GetPicture(xPic);
                    lPicList.Add(lCamPic);
                }
                lEvent.Pictures = new CamPictureLST(lPicList);
                if (lEvent.Pictures.Pictures.Count() > 0)
                {
                    lEvent.ImageSource = lEvent.Pictures.Pictures[0].ImageSource;
                }
                lEvent.Name = xEvent.Notice;
                switch (xEvent.EventType)
                {
                    case 0:
                        lEvent.Icon = "ms-appx:///Assets/Icons/user64.png";
                        break;
                    case 1:
                        lEvent.Icon = "ms-appx:///Assets/Icons/timer64.png";
                        break;
                    case 2:
                        lEvent.Icon = "ms-appx:///Assets/Icons/checkeduser64.png";
                        break;
                    case 3:
                        lEvent.Icon = "ms-appx:///Assets/Icons/unlock64.png";
                        break;
                }

                LEvList.Add(lEvent);
            }

            return LEvList;
        }

    }
}
