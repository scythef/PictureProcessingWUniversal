using PictureProcessingWUniversal.Models;
using PictureProcessingWUniversal.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureProcessingWUniversal.ViewModels
{
    public class PeopleViewModelLST : NotificationBase
    {
        PeopleLST PeopleLST;

        public PeopleViewModelLST(List<People> aPeople)
        {
            PeopleLST = new PeopleLST(aPeople);
            _SelectedIndex = -1;
            // Load the database 
            foreach (var lPeople in PeopleLST.People)
            {
                var np = new PeopleViewModel(lPeople);
                np.PropertyChanged += People_OnNotifyPropertyChanged;
                _People.Add(np);
            }
            if (PeopleLST.People.Count > 0)
            {
                _SelectedIndex = 0;
            }
        }

        ObservableCollection<PeopleViewModel> _People = new ObservableCollection<PeopleViewModel>();
        public ObservableCollection<PeopleViewModel> People
        {
            get { return _People; }
            set { SetProperty(ref _People, value); }
        }

        int _SelectedIndex;
        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set
            {
                if (SetProperty(ref _SelectedIndex, value))
                { RaisePropertyChanged(nameof(SelectedPeople)); }
            }
        }

        public PeopleViewModel SelectedPeople
        {
            get { return (_SelectedIndex >= 0) ? _People[_SelectedIndex] : null; }
        }

        //to implement in case of need
        public async Task<PeopleViewModel> AddUser()
        {
            PeopleViewModel lPeopleVM = new PeopleViewModel(new People());
            lPeopleVM.Name = "New user";
            Guid lPersonGuid = await PeopleSVC.CreateUpdatePerson(lPeopleVM.GUID, lPeopleVM.Name);
            lPeopleVM.GUID = lPersonGuid;
            lPeopleVM.CustomerGUID = Guid.Parse(App.varCustomerGUID);

            lPeopleVM.PropertyChanged += People_OnNotifyPropertyChanged;
            People.Add(lPeopleVM);
            //PlaceLST.Add(lPlaceVM);
            SelectedIndex = People.IndexOf(lPeopleVM);
            return lPeopleVM;
        }


        public async void DeleteUser()
        {
            if (SelectedIndex != -1)
            {
                PeopleSVC.DeletePerson(SelectedPeople.GUID);
                People.RemoveAt(SelectedIndex);
                if (this.People.Count() > 0)
                {
                    SelectedIndex = 0;
                }
                else
                {
                    SelectedIndex = -1;
                }
            }
        }


        void People_OnNotifyPropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            PeopleSVC.CreateUpdatePerson((sender as PeopleViewModel).GUID, (sender as PeopleViewModel).Name);
        }

    }
}
