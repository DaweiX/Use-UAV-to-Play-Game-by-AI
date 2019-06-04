///* -------------------------------------------
// *      Code For implement UWB and APS
// *              By DaweiX
// *-------------------------------------------*/


//// Note that this code is a background logic for another
//// page, where point1-4 represents the locations of 4 UWB modules in the room
//// and they are showned on a small map. And 'distance' is a TextBlock.

//using System;
//using System.Diagnostics;
//using System.Numerics;
//using System.Threading.Tasks;
//using Windows.Foundation;
//using Windows.Networking;
//using Windows.Networking.Sockets;
//using Windows.Storage.Streams;

//public class UwbReceive
//{
//    const string PORT_RESULT_1 = "9996";
//    const string PORT_RESULT_2 = "9997";
//    const string PORT_RESULT_3 = "9998";
//    const string PORT_SEND = "9999";
//    const string HOSTIP_PC = "192.168.0.90";

//    System.Random a = new System.Random();
//    System.Random b = new System.Random();
//    System.Random c = new System.Random();

//    System.Random a_60 = new System.Random();
//    System.Random a_50 = new System.Random();
//    System.Random a_40 = new System.Random();
//    System.Random a40 = new System.Random();

//    private int d12 = 0, d13 = 0, d14 = 0, d23 = 0, d24 = 0, d34 = 0;
//    private Vector2 p1, p2, p3, p4;

//#if WINDOWS_UWP
//    // 网络组件
//    StreamSocketListener listener_uwb_1 = new StreamSocketListener();
//    StreamSocketListener listener_uwb_2 = new StreamSocketListener();
//    StreamSocketListener listener_uwb_3 = new StreamSocketListener();
//    StreamSocket socket;
//    DataWriter writer;

//    async void Start()
//    {
//        Debug.WriteLine("UWB 入口");

//#if TEST
//        string path = "Main Camera/GameObject/Canvas_MAIN/Canvas_MAP/";
//        distance = GameObject.Find(path + "distance").GetComponent<Text>();
//        point1 = GameObject.Find(path + "1").GetComponent<SpriteRenderer>();
//        point2 = GameObject.Find(path + "2").GetComponent<SpriteRenderer>();
//        point3 = GameObject.Find(path + "3").GetComponent<SpriteRenderer>();
//        point4 = GameObject.Find(path + "4").GetComponent<SpriteRenderer>();

//        ShowMsg.ChangeColor(ShowMsg.MyIcons.uwb);
//        ShowMsg.ChangeColor(ShowMsg.MyIcons.gps);


//#else

//        //point1 = GameObject.Find(path_points + "1").GetComponent<SpriteRenderer>();
//        //point2 = GameObject.Find(path_points + "2").GetComponent<SpriteRenderer>();
//        //point3 = GameObject.Find(path_points + "3").GetComponent<SpriteRenderer>();
//        //point4 = GameObject.Find(path_points + "4").GetComponent<SpriteRenderer>();

//        if (point3 != null)
//        {
//            point1.enabled = false;
//            point2.enabled = false;
//            point3.enabled = false;
//            point4.enabled = false;
//        }
//        else
//        {
//            System.Diagnostics.Debug.WriteLine("点为空");
//        }

//        // distance = GameObject.Find(path_texts + "distance").GetComponent<Text>();
//        var destance = TB_distance;
//        if (distance != null)
//        {
//            distance.text = "N/A";
//        }
//        else
//        {
//            System.Diagnostics.Debug.WriteLine("-------------t1-------------");
//        }

//        await InitializeNetworkAsync();
//        await InitSendSocket();
//#endif
//    }
//#if TEST
//    void Update()
//    {
//        InvokeRepeating("MyUpdate", 1f, 1f);
//    }
//#else
//    void Update()
//    {
//        // 确认数据全部有效
//        if (d12 != 0 || d13 != 0 || d14 != 0)
//        {
//            // 显示 distance
//            ShowDistance();
//            // 执行 APS 算法定点
//            ApsLocation();
//        }
//    }
//#endif
//    async Task InitSendSocket()
//    {
//        HostName serverHost = new HostName(HOSTIP_PC);

//        socket = new StreamSocket();
//        try
//        {
//            await socket.ConnectAsync(serverHost, PORT_SEND);
//            writer = new DataWriter(socket.OutputStream);
//            Debug.WriteLine("准备发送定位数据");
//        }
//        catch (Exception e)
//        {
//            Debug.WriteLine(e.Message);
//            await InitSendSocket();
//        }
//    }

//    private void UpdatePoint()
//    {
//        //System.Diagnostics.Debug.WriteLine("UPDATE POINT");

//        point1.enabled = true;
//        point2.enabled = true;
//        point3.enabled = true;
//        point4.enabled = true;
//        float y = 40;
//        float scale = y / d12;
//        float thred = 100;
//        point1.transform.localPosition = new Vector3(0, 0, 0);
//        point2.transform.localPosition = new Vector3(0, y, 0);

//        point3.transform.localPosition = new Vector3(
//    p3.X * scale > thred ? thred : p3.X * scale,
//    p3.Y * scale > thred ? thred : p3.Y * scale, 0);
//        point4.transform.localPosition = new Vector3(
//    p4.X * scale > thred ? thred : p4.X * scale,
//    p4.Y * scale > thred ? thred : p4.Y * scale, 0);
//    }

//    private void ApsLocation()
//    {
//        try
//        {
//            // 完整的 4 点 APS 算法
//            // 对于锚定点来说，只需3，4两坐标及 d12 即可
//            p1.X = 0;
//            p2.X = 0;
//            p1.Y = -0.5f * d12;
//            p2.Y = 0.5f * d12;
//            float dd = 0.5f * d12;
//            p3.Y = (d13 * d13 - d23 * d23) / (4 * dd);
//            float cc = Math.Abs(d13 * d13 - (p3.Y + dd) * (p3.Y + dd));
//            p3.X = (float)Math.Sqrt(cc);
//            float a = p2.X - p1.X;
//            float b = p2.Y - p1.Y;
//            float c = p3.X - p1.X;
//            float d = p3.Y - p1.Y;
//            if (a * d == b * c)
//            {
//                System.Diagnostics.Debug.WriteLine("矩阵无法求逆");
//            }
//            else
//            {
//                float e = 1 / (a * d - b * c);
//                float m = p2.X * p2.X + p2.Y * p2.Y - p1.X * p1.X - p1.Y * p1.Y + d14 * d14 - d24 * d24;
//                float n = p3.X * p3.X + p3.Y * p3.Y - p1.X * p1.X - p1.Y * p1.Y + d14 * d14 - d34 * d34;
//                p4.X = e * (d * m - b * n);
//                p4.Y = e * (a * n - c * m);
//            }

//            //System.Diagnostics.Debug.WriteLine($"\nd12:{d12}\nx3:{p3.X}\ny3:{p3.Y}\nx4:{p4.X}\ny4:{p4.Y}");

//            if (d12 != 0)
//            {
//                UpdatePoint();
//            }
//        }
//        catch
//        {
//            System.Diagnostics.Debug.WriteLine("APS 算法执行异常");
//            //ShowMsg.ShowMessage("APS 算法执行异常");
//        }
//    }

//    private void ShowDistance()
//    {
//        distance.text = (d12 / 1000f).ToString("0.0") + "m\n"
//            + (d13 / 1000f).ToString("0.0") + "m\n" +
//            (d14 / 1000f).ToString("0.0") + "m\n";
//    }

//    private async Task InitializeNetworkAsync()
//    {
//        System.Diagnostics.Debug.WriteLine("监听UWB初始化");
//        //if (ShowMsg.block == null)
//        //{
//        //    string path = "Main Camera/GameObject/Canvas_MAIN/Canvas_Popup/msg";
//        //    ShowMsg.block = GameObject.Find(path).GetComponent<Text>();
//        //}

//        listener_uwb_1.Control.KeepAlive = false;
//        listener_uwb_1.Control.QualityOfService = SocketQualityOfService.Normal;
//        listener_uwb_1.ConnectionReceived += Listener_ConnectionReceived;
//        await listener_uwb_1.BindServiceNameAsync(PORT_RESULT_1);

//        listener_uwb_2.Control.KeepAlive = false;
//        listener_uwb_2.Control.QualityOfService = SocketQualityOfService.Normal;
//        listener_uwb_2.ConnectionReceived += Listener_ConnectionReceived_2;
//        await listener_uwb_2.BindServiceNameAsync(PORT_RESULT_2);

//        listener_uwb_3.Control.KeepAlive = false;
//        listener_uwb_3.Control.QualityOfService = SocketQualityOfService.Normal;
//        listener_uwb_3.ConnectionReceived += Listener_ConnectionReceived_3;
//        await listener_uwb_3.BindServiceNameAsync(PORT_RESULT_3);

//        System.Diagnostics.Debug.WriteLine("UWB监听设置完成");
//    }

//    async Task DealRev(StreamSocket socket)
//    {
//        using (socket)
//        {
//            using (DataReader reader = new DataReader(socket.InputStream))
//            {
//                try
//                {
//                    //ShowMsg.ChangeColor(ShowMsg.MyIcons.uwb);
//                    while (true)
//                    {
//                        await reader.LoadAsync(11);
//                        string str = reader.ReadString(11);
//                        System.Diagnostics.Debug.WriteLine(str);
//                        int num = int.Parse(str.Split(':')[1]);
//                        switch (str.Substring(0, 3))
//                        {
//                            case "1 2": d12 = num; break;
//                            case "1 3": d13 = num; break;
//                            case "1 4": d14 = num; break;
//                            case "2 3": d23 = num; break;
//                            case "2 4": d24 = num; break;
//                            case "3 4": d34 = num; break;
//                        }
//                        if (writer != null)
//                        {
//                            writer.WriteString(str);
//                            DataWriterStoreOperation operation = writer.StoreAsync();
//                            operation.Completed = new AsyncOperationCompletedHandler<uint>(DataSendHandler);
//                        }
//                    }
//                }
//                catch (Exception e)
//                {
//                    System.Diagnostics.Debug.WriteLine("ERROR---------" + e.Message);
//                    //ShowMsg.ShowMessage("UWB数据接收错误" + e.Message);
//                }
//            }
//        }
//    }

//    private void DataSendHandler(IAsyncOperation<uint> asyncInfo, AsyncStatus asyncStatus)
//    {
//        if (asyncStatus == AsyncStatus.Error)
//        {
//            Debug.WriteLine("发送定位数据时发生错误" + asyncInfo.ErrorCode);
//        }
//        else if (asyncStatus == AsyncStatus.Completed)
//        {
//            Debug.WriteLine("定位数据发送");
//        }
//    }

//    private void Listener_ConnectionReceived_3(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
//    {
//        System.Diagnostics.Debug.WriteLine("----------3----------");
//        DealRev(args.Socket);
//    }

//    private void Listener_ConnectionReceived_2(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
//    {
//        System.Diagnostics.Debug.WriteLine("----------2----------");
//        DealRev(args.Socket);
//    }

//    private void Listener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
//    {
//        System.Diagnostics.Debug.WriteLine("----------1----------");
//        DealRev(args.Socket);
//    }
//#else
//    void Start()
//    {
//        //distance = GameObject.Find(path_texts + "distance").GetComponent<Text>();
//        point1 = GameObject.Find(path_points + "1").GetComponent<SpriteRenderer>();
//        point2 = GameObject.Find(path_points + "2").GetComponent<SpriteRenderer>();
//        point3 = GameObject.Find(path_points + "3").GetComponent<SpriteRenderer>();
//        point4 = GameObject.Find(path_points + "4").GetComponent<SpriteRenderer>();

//        point1.transform.localPosition = new Vector3(0, 0, 0);
//        point2.transform.localPosition = new Vector3(0, 80, 0); //1
//        point3.transform.localPosition = new Vector3(40, 0, 0); //2
//        point4.transform.localPosition = new Vector3(0, 40, 0); //3

//        ShowMsg.ChangeColor(ShowMsg.MyIcons.uwb);
//        ShowMsg.ChangeColor(ShowMsg.MyIcons.gps);
//    }

//    void Update() 
//    {
//        //InvokeRepeating("MyUpdate", 1f, 1f);
//    }
//#endif

//    void MyUpdate()
//    {
//        speed += 0.0f;
//        //var d1 = (8.6 - speed * 0.0005f) * 0.6f;
//        //var d2 = Math.Abs(7.6 - speed * 0.005f) * 0.6f;
//        //var d3 = Math.Abs(7.3 + speed * 0.001f) * 0.6f;

//        //distance.text = d1.ToString("0.0") + "m\n" + d2.ToString("0.0") + "m\n" + d3.ToString("0.0") + "m\n";
//        //distance.text = d1.ToString("0.0") + "m\n";


//        // POINT
//        point1.enabled = true;
//        point2.enabled = true;
//        point3.enabled = true;
//        point4.enabled = true;

//        point1.transform.localPosition = new Vector3(0, 0, 0);
//        point2.transform.localPosition = new Vector3(0, 80, 0); //1
//        point3.transform.localPosition = new Vector3(40, 0, 0); //2
//        point4.transform.localPosition = new Vector3(0, 40, 0); //3


//        //point3.transform.localPosition = new Vector3(a40.Next(40, 41) - speed * 0.025f, a_60.Next(60, 62) - speed * 0.04f, 0);
//        //point4.transform.localPosition = new Vector3(a_40.Next(-42, -40) + speed * .01f, a_50.Next(-51, -50) - speed * .01f, -1);

//    }
//    float speed = 0f;

//}