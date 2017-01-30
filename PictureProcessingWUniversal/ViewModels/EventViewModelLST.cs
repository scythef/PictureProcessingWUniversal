using PictureProcessingWUniversal.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureProcessingWUniversal.ViewModels
{

    public class EventViewModelLST : NotificationBase
    {
        EventLST EventLST;

        public EventViewModelLST(string aFind, List<Event> aEvents)
        {
            EventLST = new EventLST(aFind, aEvents);
            _SelectedIndex = -1;
            // Load the database 
            foreach (var lEvent in EventLST.Events)
            {
                var np = new EventViewModel(lEvent);
                np.PropertyChanged += Event_OnNotifyPropertyChanged;
                _Events.Add(np);
            }
        }

        ObservableCollection<EventViewModel> _Events = new ObservableCollection<EventViewModel>();
        public ObservableCollection<EventViewModel> Events
        {
            get { return _Events; }
            set { SetProperty(ref _Events, value); }
        }

        int _SelectedIndex;
        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set
            {
                if (SetProperty(ref _SelectedIndex, value))
                { RaisePropertyChanged(nameof(SelectedEvent)); }
            }
        }

        public EventViewModel SelectedEvent
        {
            get { return (_SelectedIndex >= 0) ? _Events[_SelectedIndex] : null; }
        }

        public String Find
        {
            get { return EventLST.Find; }
            set { SetProperty(EventLST.Find, value, () => EventLST.Find = value); }
        }

        //to implement in case of need
        public void Add()
        {
            var lEventVM = new EventViewModel();
            lEventVM.PropertyChanged += Event_OnNotifyPropertyChanged;
            Events.Add(lEventVM);
            //EventLST.Add(lEventVM);
            SelectedIndex = Events.IndexOf(lEventVM);
        }


        public void Delete()
        {
            if (SelectedIndex != -1)
            {
                var lEventVM = Events[SelectedIndex];
                Events.RemoveAt(SelectedIndex);
            }
        }


        void Event_OnNotifyPropertyChanged(Object sender, PropertyChangedEventArgs e)
        {

        }



    }

}
