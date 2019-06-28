/* ------------------------------------------------
 *      The Codes for Communicating with Tello
 *                   By DaweiX
 *              Based on Tello SDK:
 *  https://www.ryzerobotics.com/cn/tello/downloads
 * -----------------------------------------------*/

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UAV_WPF.Flight
{
    public static class Comm
    {
        public struct UdpState
        {
            public UdpClient u;
            public IPEndPoint e;
        }

        public const int CommandPortNumber = 8889;

        public const int StatusPortNumber = 8890;

        public const string TelloIpAddress = "192.168.10.1";

        //public const string PCIpAddress = "0.0.0.0";

        public static bool IsCommandReady = false;

        public static string CommandResponse { get; set; }

        public static string StatusResponse { get; set; }


        private static UdpClient client_command;

        private static UdpClient client_status;

        // Communicate with UI by delegate
        public delegate void ReportStatus(string status);
        public static event ReportStatus ReportMsg;


        public static async Task StartAll()
        {
            await StartCommand();
            await StartStatus();
            //Vision.Init();
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
                client_status = new UdpClient(StatusPortNumber);
                UdpState s = new UdpState
                {
                    e = new IPEndPoint(IPAddress.None, StatusPortNumber),
                    u = client_status
                };
                // Called when new message arrived
                client_status.BeginReceive(new AsyncCallback(ReceiveCallback_Status), s);
            }
            catch (Exception ex)
            {
                Console.WriteLine("STATUS CLIENT INIT ERROR: " + ex.Message);
            }
        }

        public static async Task StartCommand()
        {
            // WORK AS CLIENT
            try
            {
                client_command = new UdpClient(CommandPortNumber);
                UdpState s = new UdpState
                {
                    e = new IPEndPoint(IPAddress.None, CommandPortNumber),
                    u = client_command
                };
                //client_command.Connect(new IPEndPoint(IPAddress.Parse(TelloIpAddress), CommandPortNumber));             
                client_command.BeginReceive(new AsyncCallback(ReceiveCallback_Command), s);
                // Init, and set the speed to the minimum
                await SendCommand("command");
                await SendCommand("speed 10");
                await SendCommand("streamon");
                IsCommandReady = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("COMMAND CLIENT INIT ERROR: " + ex.Message);
            }
        }

        public static void ReceiveCallback_Command(IAsyncResult ar)
        {
            UdpClient u = ((UdpState)(ar.AsyncState)).u;
            IPEndPoint e = ((UdpState)(ar.AsyncState)).e;

            byte[] receiveBytes = u.EndReceive(ar, ref e);
            string receiveString = Encoding.ASCII.GetString(receiveBytes);

            ReportMsg("COMMAND RESPONSE:" + receiveString);
        }

        public static void ReceiveCallback_Status(IAsyncResult ar)
        {
            UdpClient u = ((UdpState)(ar.AsyncState)).u;
            IPEndPoint e = ((UdpState)(ar.AsyncState)).e;

            byte[] receiveBytes = u.EndReceive(ar, ref e);
            StatusResponse = Encoding.ASCII.GetString(receiveBytes);
        }

        public static async Task SendCommand(string command)
        {
            try
            {
                byte[] bytes = Encoding.Unicode.GetBytes(command);
                await client_command.SendAsync(bytes, bytes.Length, new IPEndPoint(IPAddress.Parse(TelloIpAddress), CommandPortNumber));
                ReportMsg(command);
            }
            catch (Exception e)
            {
                Console.WriteLine("COMMAND SEND ERROR" + e.Message);
            }

        }
    }
}
