using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace devoctomy.funk.client.test
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {

        #region private objects

        private static FunkClient cFCtClient;

        #endregion

        #region public properties

        public static FunkClient Client
        {
            get { return (cFCtClient); }
        }

        #endregion

        private async Task InitClient()
        {
            //This uri needs to be registered with the facebook application
            Uri pUriAppCallback = FunkClient.ApplicationCallbackURI;

            //We need a config file, "Windows.Storage.ApplicationData.Current.LocalFolder\funk\config.json"
            //this is temporary until i've moved it to a more sensible location
            StorageFolder pDLyDocsLib = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFolder pSFrFunk = await pDLyDocsLib.GetFolderAsync("funk");
            StorageFile pSFeConfig = await pSFrFunk.GetFileAsync("config.json");
            Stream pStmConfig = await pSFeConfig.OpenStreamForReadAsync();
            String pStrClientID = String.Empty;
            String pStrAzureFunctionRootURL = String.Empty;

            using (StreamReader pSRrConfig = new StreamReader(pStmConfig))
            {
                using (var pJTRConfig = new JsonTextReader(pSRrConfig))
                {
                    JObject pJOtConfig = (JObject)JObject.ReadFrom(pJTRConfig);
                    pStrClientID = pJOtConfig["ClientID"].Value<String>();
                    pStrAzureFunctionRootURL = pJOtConfig["AzureFunctionRootURL"].Value<String>();
                }
            }

            cFCtClient = new FunkClient(pStrClientID,
                pStrAzureFunctionRootURL);
        }


        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            Frame rootFrame = Window.Current.Content as Frame;

            await InitClient();

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
