using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

namespace EchoServer
{
    public partial class Form1 : Form
    {
        private TcpListener tcpListener;
        private TcpClient connectedClient;
        private bool isRunning = false;
        private Thread listenerThread;
        private const int Port = 9000;
        

        public Form1()
        {
            InitializeComponent();

            LVInit();
        }

        private void LVInit()
        {
            LV_SERVER.View = View.Details;
            LV_SERVER.Columns.Add("시간", 100, HorizontalAlignment.Left);
            LV_SERVER.Columns.Add("데이터", 300, HorizontalAlignment.Left);
        }

        private void StartServer()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, Port);
                tcpListener.Start();

                while (isRunning)
                {
                    TcpClient client = tcpListener.AcceptTcpClient();
                    connectedClient = client;

                    Invoke(new Action(() =>
                    {
                        string remoteEP = client.Client.RemoteEndPoint.ToString();
                        AddListViewItem(DateTime.Now.ToString("HH:mm:ss"), $"연결 됐수다: {remoteEP}");
                    }));

                    Thread clientThread = new Thread(() => ControlClient(client));
                    clientThread.IsBackground = true;
                    clientThread.Start();
                }
            }

            catch (SocketException ex)
            {
                Invoke(new Action(() =>
                {
                    AddListViewItem("서버 ㅂㅂ", ex.Message);
                }));
            }
        }

        private void ControlClient(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];

                // Running flag, client 연결 확인 하고 ㄱㄱ
                while (isRunning)
                {
                    int bytesRead = 0;
                    try
                    {
                        bytesRead = stream.Read(buffer, 0, buffer.Length);
                    }
                    catch
                    {
                        break;
                    }

                    if (bytesRead == 0)
                    {
                        break;
                    }

                    string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    Invoke(new Action(() =>
                    {
                        AddListViewItem(DateTime.Now.ToString("HH:mm:ss"), receivedData);
                    }));
                }

                Invoke(new Action(() =>
                {
                    AddListViewItem(DateTime.Now.ToString("HH:mm:ss"), "클라이언트 ㅂㅂ");
                }));

            }
            catch (Exception ex)
            {
                Invoke(new Action(() =>
                {
                    AddListViewItem("클라이언트 에러 ;;", ex.Message);
                }));
            }

            finally
            {
                Socket_Close(client);
            }
        }

        private void AddListViewItem(string time, string data)
        {
            ListViewItem item = new ListViewItem(time);
            item.SubItems.Add(data);
            LV_SERVER.Items.Add(item);
        }

        private void BTN_OPEN_Click(object sender, EventArgs e)
        {            
            if (!isRunning)
            {
                Btn_Status_Open();

                listenerThread = new Thread(new ThreadStart(StartServer));
                listenerThread.IsBackground = true;
                listenerThread.Start();
                AddListViewItem("서버 ㄱㄱ", $"포트 {Port}");
            }
        }

        private void BTN_STOP_Click(object sender, EventArgs e)
        {
            Btn_Status_Close();
            tcpListener?.Stop();
        }

        private void BTN_EXIT_Click(object sender, EventArgs e)
        {
            Socket_Close();
            Application.Exit();
        }

        private void Btn_Status_Open()
        {
            BTN_OPEN.Enabled = false;
            BTN_STOP.Enabled = true;
            isRunning = true;
        }

        private void Btn_Status_Close()
        {
            BTN_OPEN.Enabled = true;
            BTN_STOP.Enabled = false;
            isRunning = false;
        }

        private void Socket_Close()
        {
            connectedClient?.GetStream().Close();
            connectedClient?.Close();
            connectedClient = null;
        }

        private void Socket_Close(TcpClient client)
        {
            if (connectedClient == client)
            {
                connectedClient = null;
            }

            client?.GetStream().Close();
            client?.Close();
            client = null;
        }
    }
}