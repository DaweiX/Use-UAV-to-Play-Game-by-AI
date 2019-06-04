/* ----------------------------
 *  The Logic of Page 'MainPage'
 *          By DaweiX
 * ---------------------------*/

using DJI.WindowsSDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace UAV_with_AI
{

    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public string APIKEY = "0281645fb34b5dd61a0ce508";


        public List<PaneItem> PaneListItems => new List<PaneItem>
        {
            new PaneItem { Title = "Home", Glyph = "ms-appx:///Assets//Icons//star.png" , Index = 0 },
            new PaneItem { Title = "FPV", Glyph = "ms-appx:///Assets//Icons//play.png" , Index = 1 },
        };

        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
            DJISDKManager.Instance.SDKRegistrationStateChanged += Instance_SDKRegistrationStateChanged;
            if (DJISDKManager.Instance.appActivationState != AppActivationState.ACTIVATED)
            {
                // Active the APP with ApiKey
                DJISDKManager.Instance.RegisterApp(APIKEY);
                TB_active.Text = "Activiting...";
            }
            Mainframe.Navigate(typeof(Views.Home));
        }

        private async void Instance_SDKRegistrationStateChanged(SDKRegistrationState state, SDKError errorCode)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
             {
                 TB_active.Text = state == SDKRegistrationState.Succeeded
                 ? "Api Key Activated"
                 : "Api Key Not Activated";
                 if (errorCode == SDKError.NO_ERROR)
                     return;
                 else
                     Debug.WriteLine(errorCode);
             });
        }


        private void MainList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (ham.DisplayMode == SplitViewDisplayMode.Overlay) ham.IsPaneOpen = false;
            if (e.ClickedItem is PaneItem)
            {
                var item = e.ClickedItem as PaneItem;
                switch (item.Index)
                {
                    case 0: Mainframe.Navigate(typeof(Views.Home));break;
                    case 1: Mainframe.Navigate(typeof(Views.Camera)); break;
                }
            }
        }

        private void Mainframe_Navigated(object sender, NavigationEventArgs e)
        {

        }
    }

    public class PaneItem
    {
        public string Title { get; set; }
        public string Glyph { get; set; }
        public int Index { get; set; }
    }
}