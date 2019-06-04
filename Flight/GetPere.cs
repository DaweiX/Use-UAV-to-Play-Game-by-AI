/* -----------------------------------------------
 *        The Logic to get Status of Tello
 *                   By DaweiX
 *              Based on Tello SDK:
 *  https://www.ryzerobotics.com/cn/tello/downloads
 * -----------------------------------------------*/

using System;
using System.Threading;
using System.Threading.Tasks;

namespace UAV_with_AI.Flight
{
    public static class GetPere
    {
        static string _speed;
        static string _battery;
        static string _height;

        // When try to get the properties,
        // check them if is in the valid range
        public static string Speed
        {
            get { return _speed; }
            set
            {
                if (int.TryParse(value, out int x) && x >= 10 && x <= 100)
                {
                    _speed = x.ToString();
                }
                else
                {
                    _speed = "N/A";
                }
            }
        }

        public static string Battery
        {
            get { return _battery; }
            set
            {
                if (int.TryParse(value, out int x) && x >= 0 && x <= 100)
                {
                    _battery = x.ToString();
                }
                else
                {
                    _battery = "N/A";
                }
            }
        }

        public static string Height
        {
            get { return _height; }
            set
            {
                if (int.TryParse(value, out int x) && x >= 10 && x <= 3000) 
                {
                    _height = x.ToString();
                }
                else
                {
                    _height = "N/A";
                }
            }
        }

        public static string SNR { get; set; }

        public async static Task UpdateSpeed()
        {
            await Comm.AskForStatus("speed?");
            Speed =  Comm.StatusResponse;
        }

        public async static Task UpdateBattery()
        {
            await Comm.AskForStatus("battery?");
            Battery = Comm.StatusResponse;
        }

        public async static Task UpdateHeight()
        {
            await Comm.AskForStatus("baro?");
            Height = Comm.StatusResponse;
        }

        public async static Task UpdateSNR()
        {
            await Comm.AskForStatus("wifi?");
            SNR = Comm.StatusResponse;
        }

        public static void UpdateAll()
        {
            // A dead cycle, so must put it
            // in a new background thread

            new Thread(async () =>
            {
                while (Comm.IsCommandReady)
                {
                    await UpdateBattery();
                    await UpdateHeight();
                    await UpdateSNR();
                    await UpdateSpeed();
                    await Task.Delay(100);
                }
            })
            {
                IsBackground = true
            }.Start();
        }
    }
}
