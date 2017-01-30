using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureProcessingWUniversal.Models
{
    public class EventLST
    {
        public List<Event> Events { get; set; }
        public String Find { get; set; }

        public EventLST(string aFind, List<Event> aEvents)
        {
            Events = aEvents;
            Find = aFind;

        }

    }
}
