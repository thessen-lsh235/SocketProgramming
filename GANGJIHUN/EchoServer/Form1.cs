using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
namespace EchoServer
{
    public partial class Form1 : Form
    {
        private TcpListener tcpListener;
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
                    if (tcpListener.Pending())
                    {
                        TcpClient client = tcpListener.AcceptTcpClient();

                        Invoke(new Action(() =>
                        {
                            string remoteEP = client.Client.RemoteEndPoint.ToString();
                            AddListViewItem(DateTime.Now.ToString("HH:mm:ss"), $"연결 됐수다: {remoteEP}");
                        }));

                        Thread clientThread = new Thread(() => ControlClient(client));
                        clientThread.IsBackground = true;
                        clientThread.Start();
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
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

                    byte[] sendData = Encoding.UTF8.GetBytes(receivedData);
                    stream.Write(sendData, 0, sendData.Length);
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
                client.Close();
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
            BTN_OPEN.Enabled = false;
            BTN_CLOSE.Enabled = true;
            if (!isRunning)
            {
                isRunning = true;
                listenerThread = new Thread(new ThreadStart(StartServer));
                listenerThread.IsBackground = true;
                listenerThread.Start();
                AddListViewItem("서버 ㄱㄱ", $"포트 {Port}");
            }
        }

        private void BTN_CLOSE_Click(object sender, EventArgs e)
        {
            BTN_OPEN.Enabled = true;
            BTN_CLOSE.Enabled = false;
            isRunning = false;
            if (tcpListener != null)
            {
                tcpListener.Stop();
            }
        }

        private void BTN_EXIT_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
