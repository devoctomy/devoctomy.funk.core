using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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

namespace devoctomy.funk.client.test
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        #region constructor / destructor

        public MainPage()
        {
            this.InitializeComponent();
        }

        #endregion

        #region object events

        private async void butAuthenticate_Click(object sender, RoutedEventArgs e)
        {
            if (await App.Client.AuthenticateAsync())
            {

            }
        }

        private async void butRegister_Click(object sender, RoutedEventArgs e)
        {
            FunkManagementRegisterResponse pRReRegisterResponse = await App.Client.RegisterAsync();
            if (pRReRegisterResponse.Success)
            {

            }
            else
            {

            }
        }

        private async void butServiceInfo_Click(object sender, RoutedEventArgs e)
        {
            HttpResponseMessage resp = await App.Client.AuthenticatedRequestAsync("GET", App.Client.GetFunctionURI("Test"));
            if (resp.IsSuccessStatusCode)
            {
                //success
                String respMessage = await resp.Content.ReadAsStringAsync();
                respMessage = "";
            }
            else
            {
                //fail
            }


            FunkManagementServiceInfoResponse pSIRServiceInfoResponse = await App.Client.ServiceInfoAsync();
            if (pSIRServiceInfoResponse.Success)
            {

            }
            else
            {

            }
        }

        #endregion

    }

}
