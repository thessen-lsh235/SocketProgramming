using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace EchoClient
{
    public partial class Form1 : Form
    {
        private TcpClient tcpClient;
        private NetworkStream stream;
        private bool isConnected = false;
        

        public Form1()
        {
            InitializeComponent();
            LVInit();
        }

        private void LVInit()
        {
            LV_CLIENT.View = View.Details;
            LV_CLIENT.Columns.Add("시간", 100, HorizontalAlignment.Left);
            LV_CLIENT.Columns.Add("보낸 데이터", 200, HorizontalAlignment.Left);
        }

        private void AddLog(string time, string sent)
        {
            ListViewItem item = new ListViewItem(time);
            item.SubItems.Add(sent);
            LV_CLIENT.Items.Add(item);
        }

        private void BTN_SEND_Click(object sender, EventArgs e)
        {
            if (isConnected && stream != null)
            {
                try
                {
                    BTN_SEND.Enabled = true;
                    string message = TB_DATA.Text.Trim();
                    if (!string.IsNullOrEmpty(message))
                    {
                        byte[] data = Encoding.UTF8.GetBytes(message);
                        stream.Write(data, 0, data.Length);

                        AddLog(DateTime.Now.ToString("HH:mm:ss"), message);
                        TB_DATA.Clear();
                    }
                }
                catch (Exception ex)
                {
                    AddLog(DateTime.Now.ToString("HH:mm:ss"), "BTN_SEND_Click 오류 " + ex.Message);
                }
            }
        }        

        private void CloseClient()
        {
            isConnected = false;

            if (stream != null)
            {
                stream.Close();
                stream = null;
            }
            if (tcpClient != null)
            {
                tcpClient.Close();
                tcpClient = null;
            }
        }

        private void BTN_CONNECT_Click(object sender, EventArgs e)
        {
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(TB_IP.Text, int.Parse(TB_PORT.Text));
                stream = tcpClient.GetStream();
                isConnected = true;

                BTN_CONNECT.Enabled = false;
                BTN_DISCONNECT.Enabled = true;
                BTN_SEND.Enabled = true;

                AddLog(DateTime.Now.ToString("HH:mm:ss"), "나이스");
            }
            catch (Exception ex)
            {
                AddLog(DateTime.Now.ToString("HH:mm:ss"), "BTN_CONNECT_Click 오류 " + ex.Message);
            }
        }

        private void BTN_DISCONNECT_Click(object sender, EventArgs e)
        {
            CloseClient();

            BTN_CONNECT.Enabled = true;
            BTN_DISCONNECT.Enabled = false;
            BTN_SEND.Enabled = false;

            AddLog(DateTime.Now.ToString("HH:mm:ss"), "ㅂㅂ");
        }

        private void BTN_EXIT_Click(object sender, EventArgs e)
        {
            CloseClient();

            Application.Exit();
        }
    }
}