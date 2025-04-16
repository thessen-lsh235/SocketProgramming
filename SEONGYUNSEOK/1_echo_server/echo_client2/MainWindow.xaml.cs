using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace echo_client2
{
    public partial class MainWindow : Window
    {
        private TcpClient client;
        private NetworkStream stream;

        public MainWindow()
        {
            InitializeComponent();
            //ConnectToServer();
        }

        private async void ConnectToServer()
        {
            try
            {
                client = new TcpClient();
                await client.ConnectAsync("127.0.0.1", 5000);
                stream = client.GetStream();
                lstMessages.Items.Add("서버에 연결되었습니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"서버 연결 실패: {ex.Message}");
            }
        }
        private async void Connect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (client != null && client.Connected)
                {
                    lstMessages.Items.Add("이미 서버에 연결되어 있습니다.");
                    return;
                }

                client = new TcpClient();
                await client.ConnectAsync("127.0.0.1", 5000);
                stream = client.GetStream();
                lstMessages.Items.Add("서버에 연결되었습니다.");
            }
            catch (Exception ex)
            {
                lstMessages.Items.Add($"서버 연결 실패: {ex.Message}");
            }
        }

        private async void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            if (client == null || stream == null || !client.Connected)
            {
                lstMessages.Items.Add("[오류] 서버에 연결되어 있지 않습니다.");
                return;
            }

            string message = txtMessage.Text.Trim();
            if (string.IsNullOrEmpty(message)) return;

            byte[] data = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(data, 0, data.Length);

            lstMessages.Items.Add($"[나]: {message}");
            txtMessage.Clear();
        }


        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            if (client != null)
            {
                stream?.Close();
                client.Close();
                lstMessages.Items.Add("서버 연결 종료됨.");
            }
        }
    }
}
