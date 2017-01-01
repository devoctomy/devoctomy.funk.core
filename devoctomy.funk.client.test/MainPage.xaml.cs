using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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


        #region private objects

        private String cStrEmail = String.Empty;

        #endregion

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
                butUserInfo.IsEnabled = true;
                butServiceInfo.IsEnabled = true;
            }
        }

        private async void butUserInfo_Click(object sender, RoutedEventArgs e)
        {
            FunkManagementGetUserInfoResponse pSIRServiceInfoResponse = await App.Client.GetUserInfoAsync();
            if (pSIRServiceInfoResponse.Success)
            {
                cStrEmail = pSIRServiceInfoResponse.Response.Email;
                butRegister.IsEnabled = !pSIRServiceInfoResponse.Response.Registered;
            }
            else
            {
                butRegister.IsEnabled = false;
            }
        }

        private async void butRegister_Click(object sender, RoutedEventArgs e)
        {
            FunkManagementGetRegisterResponse pRReRegisterResponse = await App.Client.GetRegisterAsync();
            if (pRReRegisterResponse.Success)
            {
                butRegister.IsEnabled = false;

                MessageDialog pMDgDialog = new MessageDialog($"Please check your email @ '{cStrEmail}' for your activation code!");
                await pMDgDialog.ShowAsync();
            }
            else
            {
                butRegister.IsEnabled = true;
            }
        }

        private async void butActivate_Click(object sender, RoutedEventArgs e)
        {
            FunkManagementGetActivateResponse pGARGetActivateResponse = await App.Client.GetActivateAsync(tbxActivationCode.Text,
                tbxUserName.Text);
            if(pGARGetActivateResponse.Success)
            {
                MessageDialog pMDgDialog = new MessageDialog("Successfully activated account!");
                await pMDgDialog.ShowAsync();
            }
            else
            {
                MessageDialog pMDgDialog = new MessageDialog("Failed to activate account!");
                await pMDgDialog.ShowAsync();
            }
        }

        private async void butServiceInfo_Click(object sender, RoutedEventArgs e)
        {
            FunkManagementServiceInfoResponse pSIRServiceInfoResponse = await App.Client.GetServiceInfoAsync();
            if (pSIRServiceInfoResponse.Success)
            {
                //success
            }
            else
            {
                //fail
            }
        }

        #endregion

    }

}
