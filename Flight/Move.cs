/* -----------------------------------------------
 *  The Logic to implement flight control of Tello
 *                   By DaweiX
 *              Based on Tello SDK:
 *  https://www.ryzerobotics.com/cn/tello/downloads
 * -----------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace UAV_with_AI.Flight

{
    public class Move
    {

        public static int C = 80;

        public static async Task<int> MoveBetweenTwoPoints(AI.Step current, AI.Step next)
        {
            int delay_waitfor1move = C * 150;

            int delay_upanddown = 8000;

            int delay_betweenaction = 600;

            var actions = GetActionSeq(current, next);

            int delay = 0;
            foreach (var item in actions)
            {
                int movedelay = delay_waitfor1move * (item[1] - '0');
                delay += movedelay;
                switch ((char)item[0])
                {
                    case 'B': await Back((int)(item[1] - '0') * C); break;
                    case 'F': await Forward((int)(item[1] - '0') * C); break;
                    case 'R': await Right((int)(item[1] - '0') * C); break;
                    case 'L': await Left((int)(item[1] - '0') * C); break;
                }
                await Task.Delay(movedelay);

                delay += delay_betweenaction;
                await Task.Delay(delay_betweenaction);
            }

            // After take the step, land
            await Comm.SendCommand("land");
            await Task.Delay(delay_upanddown);

            await Comm.SendCommand("takeoff");
            await Task.Delay(delay_upanddown);
            delay += delay_upanddown * 2;

            return delay;
        }

        public static async Task MoveToStart(AI.Step current)
        {
            await MoveBetweenTwoPoints(current, new AI.Step { Move = new AI.GameState.Move(1, 1, 1) });
        }

        // In this method, no yaw will be contained.
        public static List<string> GetActionSeq(AI.Step current, AI.Step next)
        {
            List<string> actions = new List<string>();
            int delta_x = next.Move.X - current.Move.X;
            int delta_y = next.Move.Y - current.Move.Y;
            if (delta_x != 0)
            {
                string action = (delta_x > 0 ? "B" : "F") + Math.Abs(delta_x).ToString();
                Debug.WriteLine(action);
                actions.Add(action);
            }
            if (delta_y != 0)
            {
                string action = (delta_y > 0 ? "R" : "L") + Math.Abs(delta_y).ToString();
                Debug.WriteLine(action);
                actions.Add(action);
            }
            return actions;
        }

        static public async Task Up(int cm)
        {
            await Comm.SendCommand($"up {cm}");
        }

        static public async Task Down(int cm)
        {
            await Comm.SendCommand($"down {cm}");
        }

        static public async Task Left(int cm)
        {
            await Comm.SendCommand($"left {cm}");
        }

        static public async Task Right(int cm)
        {
            await Comm.SendCommand($"right {cm}");
        }

        static public async Task Forward(int cm)
        {
            await Comm.SendCommand($"forward {cm}");
        }

        static public async Task Back(int cm)
        {
            await Comm.SendCommand($"back {cm}");
        }
    }
}
