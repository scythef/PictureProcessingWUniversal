using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web.Core;
using Windows.Security.Credentials;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureProcessingWUniversal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AccountPage : Page
    {
        public string lCustomerGUID { get; set; }
        public AccountPage()
        {
            this.InitializeComponent();
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            lCustomerGUID = Connections.DeploymentDict["PersonGroupID"];

        }


        private async void SignIn()
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            App.varCustomerGUID = lCustomerGUID;
            App.varMainPage.LoadPlaces();
            App.varMainPage.LoadPeople();
            
        }
    }
}
