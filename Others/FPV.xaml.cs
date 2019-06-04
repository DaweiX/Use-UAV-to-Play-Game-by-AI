/* ------------------------------------------------------------------------------
 *              The Codes for Get the Video Stream of DJI Mavic
 *                   By DaweiX          Based on Mavic SDK:                     
 *  https://developer.dji.com/cn/windows-sdk/documentation/quick-start/index.html
 * -----------------------------------------------------------------------------*/

using DJI.WindowsSDK;
using DJIVideoParser;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

#if OUTDOOR
namespace UAV_with_AI.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class FPV : Page
    {
        Parser videoParser;

        public FPV()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                base.OnNavigatedTo(e);
                InitVideoFeed();
                await DJISDKManager.Instance.ComponentManager.
                    GetCameraHandler(0, 0).SetCameraWorkModeAsync(new CameraWorkModeMsg
                    {
                        value = CameraWorkMode.SHOOT_PHOTO
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            UninitVideoFeed();
        }


        public void UninitVideoFeed()
        {
            if (DJISDKManager.Instance.SDKRegistrationResultCode == SDKError.NO_ERROR)
            {
                if (videoParser == null) return;
                videoParser.SetSurfaceAndVideoCallback(0, 0, null, null);
                DJISDKManager.Instance.VideoFeeder.GetPrimaryVideoFeed(0).VideoDataUpdated -= FPV_VideoDataUpdated;
            }
        }

        private async void InitVideoFeed()
        {
            try
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    if (videoParser == null)
                    {
                        videoParser = new Parser();
                        videoParser.Initialize(delegate (byte[] data)
                        {
                            return DJISDKManager.Instance.
                            VideoFeeder.ParseAssitantDecodingInfo(0, data);
                        });
                        videoParser.SetSurfaceAndVideoCallback(0, 0, panel, ReceiveDecodeData);
                        DJISDKManager.Instance.VideoFeeder.GetPrimaryVideoFeed(0).VideoDataUpdated
                        += FPV_VideoDataUpdated;
                    }
                    DJISDKManager.Instance.ComponentManager.GetCameraHandler(0, 0).CameraTypeChanged += FPV_CameraTypeChanged;
                    var type = await DJISDKManager.Instance.
                    ComponentManager.GetCameraHandler(0, 0).GetCameraTypeAsync();
                    FPV_CameraTypeChanged(this, type.value);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        async void ReceiveDecodeData(byte[] data, int width, int height)
        {
            // Return the row array of the image. In RGBA format.
        }

        private void FPV_CameraTypeChanged(object sender, CameraTypeMsg? value)
        {
            if (value != null)
            {
                videoParser.SetCameraSensor(AircraftCameraType.Others);
            }
        }

        private void FPV_VideoDataUpdated(VideoFeed sender, byte[] bytes)
        {
            videoParser.PushVideoData(0, 0, bytes, bytes.Length);
        }
    }
}

#endif