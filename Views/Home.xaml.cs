/* ------------------------
 *  The Logic of Page 'Home'
 *       By DaweiX
 * ------------------------*/

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


namespace UAV_with_AI.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Home : Page
    {
        // Store the current step 
        static AI.Step current;
        static bool IsUseLastStep = true;

        public Home()
        {
            this.InitializeComponent();
            fpvFrame.Navigate(typeof(Camera));
        }

        async Task WaitForInit()
        {
            while (!AI.Nums.IsReady)
            {
                await Task.Delay(500);
            }
            Btn_Start.IsEnabled = true;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Flight.Comm.ReportMsg += Comm_ReportMsg;
            //this.Invoke(() =>
            //{
            //    AI.Test.GoSim();
            //});
            Thread th = new Thread((objParam) =>
            {
                AI.Test.GoSim();
            })
            {
                IsBackground = true
            };
            th.Start();
            // Although MCTS is easy to run, we put
            // it in another thread.
            await WaitForInit();

            await Flight.Comm.StartAll();

            while (!Flight.Comm.IsCommandReady)
            {
                // We must make Tello be ready for
                // next operations
                await Task.Delay(100);
            }

            // Update the real-time infos of Tello
            // Flight.GetPere.UpdateAll();
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                while (true)
                {
                    // 0       1   2   3   4          5       6      7     8     9     10    11       12       13     14 15      16          17
                    // mid:257;x:0;y:0;z:0;mpry:0,0,0;pitch:0;roll:0;yaw:0;vgx:0;vgy:0;vgz:0;templ:67;temph:69;tof:10;h:0;bat:77;baro:152.52;time:0;agx:4.00;agy:-16.00;agz:-1000.00;
                    var res = Flight.Comm.StatusResponse;
                    if (!string.IsNullOrEmpty(res)) 
                    {
                        TB_height.Text = res.Split(';')[14].Split(':')[1];
                        TB_battery.Text = res.Split(';')[15].Split(':')[1];
                        TB_yaw.Text = res.Split(';')[7].Split(':')[1];
                        TB_time.Text = res.Split(';')[17].Split(':')[1];
                    }
                    await Task.Delay(100);
                }
            });
        }

        private async void Comm_ReportMsg(string status)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                TB_AI_log.Text += status + Environment.NewLine;
            });
        }

        public async void Invoke(Action action,Windows.UI.Core.CoreDispatcherPriority priority = Windows.UI.Core.CoreDispatcherPriority.Normal)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.
                CoreWindow.Dispatcher.RunAsync(priority, () => { action(); });
        }

        private async void Btn_Start_Click(object sender, RoutedEventArgs e)
        {
            // Start the workflow
            await Flight.Comm.SendCommand("takeoff");
            await Task.Delay(8000);
            var TrainingSteps = AI.Nums.Steps;
            for (int i = 0; i < TrainingSteps.Count; i++)
            {
                var step = TrainingSteps[i];
                if (i == 0)
                {
                    current = new AI.Step { Move = new AI.GameState.Move(1, 1, 1) };
                }
                else
                {
                    // If a game is not finished, the 'current' step
                    // is given by AI. If a game is just finished,
                    // the 'current' location is the center point of the board.
                    if (IsUseLastStep)
                        current = TrainingSteps[i - 1];
                }
                if(step.Type == AI.StepType.OUTCOME)
                {
                    ClearBoard();
                    string text;
                    switch (step.Result)
                    {
                        case 1: text = "O WINs"; break;
                        case -1: text = "X WINs"; break;
                        case 0: text = "DRAW"; break;
                        default: text = "NONE"; break;
                    }
                    TB_result.Text = text;

                    // After a game, move the UAV back to the start grid (center)
                    await Flight.Move.MoveBetweenTwoPoints(current, new AI.Step { Move = new AI.GameState.Move(1, 1, 1) });
                    current = new AI.Step { Move = new AI.GameState.Move(1, 1, 1) };
                    IsUseLastStep = false;
                }
                else
                {
                    // Update UI
                    TB_result.Text = "UNFINISHED";
                    TB_status.Text = step.Type.ToString();
                    Image image = new Image();
                    BitmapSource source = new BitmapImage(new Uri(string.Format("ms-appx:///Assets//Icons//{0}.png", step.Move.Side)));
                    image.Source = source;
                    image.Margin = new Thickness(8);
                    board.Children.Add(image);
                    Grid.SetRow(image, step.Move.X);
                    Grid.SetColumn(image, step.Move.Y);

                    // Update AI logs
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.AppendLine(step.Type.ToString());
                        builder.AppendLine(AI.Log.GetMatString(step.CurrentBoard));
                        TB_AI_log.Text += builder.ToString();
                    });

                    // Move Tello to the next point (node)
                    await Flight.Move.MoveBetweenTwoPoints(current, step);
                    IsUseLastStep = true;
                }
            }
        }

        private void ClearBoard()
        {
            while (board.Children.Count > 9)
            {
                board.Children.RemoveAt(9);
            }
        }

        // Used for emergency or stopping the current training
        private async void Land_Click(object sender, RoutedEventArgs e)
        {
            if (!Flight.Comm.IsCommandReady) return;
            await Flight.Comm.SendCommand("land");
        }

        // The functions below are only used for testing and recording 
        // the durance of certain flight action
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Flight.Comm.SendCommand("takeoff");

        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await Flight.Move.Left(45);
        }

        private async void R_Click(object sender, RoutedEventArgs e)
        {
            await Flight.Move.Right(45);
        }

        private async void F_Click(object sender, RoutedEventArgs e)
        {
            await Flight.Move.Forward(45);
        }

        private async void Back_Click(object sender, RoutedEventArgs e)
        {
            await Flight.Move.Back(45);
        }
    }
}
