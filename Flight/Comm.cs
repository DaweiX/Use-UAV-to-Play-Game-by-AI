/* ------------------------------------------------
 *      The Codes for Communicating with Tello
 *                   By DaweiX
 *              Based on Tello SDK:
 *  https://www.ryzerobotics.com/cn/tello/downloads
 * -----------------------------------------------*/

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Storage.Streams;

namespace UAV_with_AI.Flight
{
    public static class Comm
    {
        public const string CommandPortNumber = "8889";

        public const string StatusPortNumber = "8890";

        public const string VideoPortNumber = "11111";

        public const string TelloIpAddress = "192.168.10.1";

        //public const string PCIpAddress = "0.0.0.0";

        public static bool IsCommandReady = false;

        public static string CommandResponse { get; set; }

        public static string StatusResponse { get; set; }

        static Windows.Networking.Sockets.DatagramSocket client;

        static Windows.Networking.Sockets.DatagramSocket status;

        // Communicate with UI by delegate
        public delegate void ReportStatus(string status);
        public static event ReportStatus ReportMsg;


        public static async Task StartAll()
        {
            await StartCommand();
            await StartStatus();
        }

        public static async Task StartStatus()
        {
            while (!IsCommandReady)
            {
                await Task.Delay(50);
            }
            // WORK AS SERVER
            try
            {
                // Initialize the UDP connect
                status = new Windows.Networking.Sockets.DatagramSocket();

                await status.BindServiceNameAsync(StatusPortNumber);
                // Called when new message arrived
                status.MessageReceived += ServerDatagramSocket_MessageReceived;
            }
            catch (Exception ex)
            {
                Windows.Networking.Sockets.SocketErrorStatus webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
                Debug.WriteLine(webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message);
            }
        }

        public static void ServerDatagramSocket_MessageReceived(
            Windows.Networking.Sockets.DatagramSocket sender,
            Windows.Networking.Sockets.DatagramSocketMessageReceivedEventArgs args)
        {
            string respond;
            using (DataReader dataReader = args.GetDataReader())
            {
                respond = dataReader.ReadString(dataReader.UnconsumedBufferLength).Trim();
            }

            StatusResponse = respond;
        }

        public static async Task StartCommand()
        {
            // WORK AS CLIENT
            try
            {
                client = new Windows.Networking.Sockets.DatagramSocket();
                client.MessageReceived += ClientDatagramSocket_MessageReceived;
                //var hostName = new HostName(TelloIpAddress);
                await client.BindServiceNameAsync(CommandPortNumber);
                // await client.ConnectAsync(new HostName(TelloIpAddress), CommandPortNumber);
                var a = client.Information;
                // Init, and set the speed to the minimum
                await SendCommand("command");
                await SendCommand("speed 10");
                // await SendCommand("streamon");
                IsCommandReady = true;
            }
            catch (Exception ex)
            {
                Windows.Networking.Sockets.SocketErrorStatus webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
                Debug.WriteLine(webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message);
            }
        }

        public async static Task SendMessage(Windows.Networking.Sockets.DatagramSocket socket, HostName hostName,string port, string request)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            var temp = utf8.GetBytes(request);
            string request8 = utf8.GetString(temp);
            using (Stream outputStream = (await socket.GetOutputStreamAsync(hostName, port)).AsStreamForWrite())
            {
                using (var streamWriter = new StreamWriter(outputStream))
                {
                    await streamWriter.WriteAsync(request8);
                    await streamWriter.FlushAsync();
                }
            }
        }
    

    // Send Command to Tello
    public async static Task SendCommand(string command)
        {
            ReportMsg(command);
            await SendMessage(client, new HostName(TelloIpAddress), CommandPortNumber, command);
            //while (iserror)
            //{
            //    await Task.Delay(500);
            //    await SendMessage(client, new HostName(TelloIpAddress), CommandPortNumber, command);
            //}
        }

        // Ask for status of Tello
        public async static Task AskForStatus(string command)
        {
            await SendMessage(client, new HostName(TelloIpAddress), StatusPortNumber, command);
        }

        public static void ClientDatagramSocket_MessageReceived(
            Windows.Networking.Sockets.DatagramSocket sender, 
            Windows.Networking.Sockets.DatagramSocketMessageReceivedEventArgs args)
        {
            string response;
            using (DataReader dataReader = args.GetDataReader())
            {
                byte[] buffer = new byte[dataReader.UnconsumedBufferLength];
                dataReader.ReadBytes(buffer);
                StringBuilder builder = new StringBuilder();
                foreach (var item in buffer)
                {
                    builder.Append((char)item);
                }
                response = builder.ToString();
            }

            Debug.WriteLine("COMMAND RESPONSE:" + response);
            ReportMsg("COMMAND RESPONSE:" + response);
            CommandResponse = response;
            //iserror = (response != "ok");
            //sender.Dispose();
        }

        //static bool iserror = false;
    }
}
