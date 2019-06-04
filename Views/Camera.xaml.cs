/* -------------------------------------------------
 *  The Logic for fetching the video stream of Tello
 *                   By DaweiX
 * -------------------------------------------------*/


using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace UAV_with_AI.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Camera : Page
    {
        public Camera()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await StartVideo();
        }

        // The Util Funtion for bytes2buffer
        public static IBuffer BytesToBuffer(byte[] bytes)
        {
            using (var dataWriter = new DataWriter())
            {
                dataWriter.WriteBytes(bytes);
                return dataWriter.DetachBuffer();
            }
        }

        // The Util Funtion for stream2buffer
        public static async Task<IBuffer> StreamToBuffer(IRandomAccessStream stream)
        {
            var s = stream.AsStreamForRead();
            if (stream != null)
            {
                s = stream.AsStreamForRead();
                int len = (int)s.Length;
                byte[] b = new byte[(uint)s.Length];
                await s.ReadAsync(b, 0, len);
                IBuffer buffer = WindowsRuntimeBufferExtensions.AsBuffer(b, 0, len);
                return buffer;
            }
            return null;
        }


        public async Task StartVideo()
        {
            while (!Flight.Comm.IsCommandReady)
            {
                await Task.Delay(50);
            }
            // WORK AS SERVER, work as the same way of port 8890 (Status)
            try
            {
                var videoDatagramSocket = new Windows.Networking.Sockets.DatagramSocket();

                await videoDatagramSocket.BindServiceNameAsync(Flight.Comm.VideoPortNumber);

                videoDatagramSocket.MessageReceived += VideoDatagramSocket_MessageReceived;

                img.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                Windows.Networking.Sockets.SocketErrorStatus webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
                Debug.WriteLine(webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message);
            }
        }

        // The callback when getting image buffer from remote device (Tello)
        private async void VideoDatagramSocket_MessageReceived(Windows.Networking.Sockets.DatagramSocket sender, Windows.Networking.Sockets.DatagramSocketMessageReceivedEventArgs args)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                IBuffer buffer;
                using (DataReader dataReader = args.GetDataReader())
                {
                    buffer = dataReader.ReadBuffer(dataReader.UnconsumedBufferLength);
                }
                Debug.WriteLine(buffer.Length);
                BitmapImage bitmap = new BitmapImage();

                // Here, buffer2stream is implemented by C# funtions
                // So the 2 Util functions are not used
                await bitmap.SetSourceAsync(buffer.AsStream().AsRandomAccessStream());
                image.Source = bitmap;
            });
        }
    }
}
