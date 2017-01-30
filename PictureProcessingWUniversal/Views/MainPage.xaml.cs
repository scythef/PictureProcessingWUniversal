using PictureProcessingWUniversal.Models;
using PictureProcessingWUniversal.Services;
using PictureProcessingWUniversal.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PictureProcessingWUniversal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();

            App.varMainPage = this;
            App.varActualDate = DateTime.Now;
            HamburgerMenu.OptionsItemsSource = OptionMenuItem.GetOptionsItems( null/* varPlaceVMLST*/);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (App.varCustomerGUID == null)
            {
                contentFrame.Navigate(typeof(AccountPage), null);
            }
            else
            {
                LoadPlaces();
                LoadPeople();
            }
        }


        private void OnMenuItemClick(object sender, ItemClickEventArgs e)
        {
            var menuItem = e.ClickedItem as MenuItem;
            contentFrame.Navigate(menuItem.PageType, menuItem.PlaceVM);
        }
        private async void OnOptionMenuItemClick(object sender, ItemClickEventArgs e)
        {
            var menuItem = e.ClickedItem as OptionMenuItem;
            contentFrame.Navigate(menuItem.PageType);
        }
    
        public async void LoadPlaces()
        {
            App.varPlaceVMLST = new PlaceViewModelLST(await PlaceSVC.GetPlaces(App.varCustomerGUID));
            if (App.varPlaceVMLST == null)
            {
                List<Place> lPlaces = new List<Place>();
                App.varPlaceVMLST = new PlaceViewModelLST(lPlaces);
            }
            HamburgerMenu.ItemsSource = MenuItem.GetMainItems(App.varPlaceVMLST);
            if (App.varPlaceVMLST.Places.Count() == 0)
            {
                contentFrame.Navigate(typeof(SettingsPage));
            }
            else
            {
                App.varPlaceVMLST.SelectedIndex = 0;
                contentFrame.Navigate(typeof(EventsPage), App.varPlaceVMLST.SelectedPlace);
            }
        }

        public async void LoadPeople()
        {
            App.varPeopleVMLST = new PeopleViewModelLST(await PeopleSVC.GetPeople(App.varCustomerGUID));
            if (App.varPeopleVMLST == null)
            {
                List<People> lPeoples = new List<People>();
                PeopleViewModelLST varPeopleVMLST = new PeopleViewModelLST(lPeoples);
            }
        }
    }

    public class MenuItem
    {
        public Symbol Icon { get; set; }
        public string Name { get; set; }
        public Type PageType { get; set; }
        public PlaceViewModel PlaceVM { get; set; }

        public static List<MenuItem> GetMainItems(PlaceViewModelLST aPlaceVMLST)
        {

            var items = new List<MenuItem>();
            foreach(PlaceViewModel xPlaceVM in aPlaceVMLST.Places)
            {
                items.Add(new MenuItem() { Icon = Symbol.Refresh, Name = xPlaceVM.Name, PageType = typeof(Views.EventsPage), PlaceVM = xPlaceVM });
            }

            return items;
        }
    }

    public class OptionMenuItem
    {
        public Symbol Icon { get; set; }
        public string Name { get; set; }
        public Type PageType { get; set; }

        public static List<OptionMenuItem> GetOptionsItems(PlaceViewModelLST aPlaceVMLST)
        {
            var items = new List<OptionMenuItem>();
            items.Add(new OptionMenuItem() { Icon = Symbol.People, Name = "People", PageType = typeof(Views.PeoplePage) });//, PlaceVMLST = aPlaceVMLST });
            items.Add(new OptionMenuItem() { Icon = Symbol.Setting, Name = "Settings", PageType = typeof(Views.SettingsPage) });//, PlaceVMLST = aPlaceVMLST});
            items.Add(new OptionMenuItem() { Icon = Symbol.Account, Name = "Account", PageType = typeof(Views.AccountPage) });//, PlaceVMLST = aPlaceVMLST });
            return items;
        }

    }

}
