using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace echo_client
{
    // WPF의 MainWindow.xaml에 연결된 코드 비하인드 클래스
    public partial class MainWindow : Window
    {
        private TcpClient client;           // 서버에 연결할 클라이언트 객체
        private NetworkStream stream;       // 서버와의 데이터 송수신용 스트림

        public MainWindow()
        {
            InitializeComponent(); // UI 컴포넌트 초기화
            // ConnectToServer(); ← 자동 연결 제거됨 (Connect 버튼 사용)
        }

        // Connect 버튼 대신 사용할 수 있는 함수 (현재는 사용 안 함)
        private async void ConnectToServer()
        {
            try
            {
                client = new TcpClient(); // 클라이언트 객체 생성
                await client.ConnectAsync("127.0.0.1", 5000); // 서버 연결 시도
                stream = client.GetStream(); // 데이터 스트림 확보
                lstMessages.Items.Add("서버에 연결되었습니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"서버 연결 실패: {ex.Message}");
            }
        }

        // [Connect] 버튼 클릭 시 서버 연결
        private async void Connect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 이미 연결된 경우 중복 연결 방지
                if (client != null && client.Connected)
                {
                    lstMessages.Items.Add("이미 서버에 연결되어 있습니다.");
                    return;
                }

                client = new TcpClient(); // 새 TcpClient 생성
                //await는 작업이 진행되는 동안 UI가 멈추지 않도록 해줌 async 함수 안에서만 사용 가능
                await client.ConnectAsync("127.0.0.1", 5000); // 서버 연결 ConnectAsync는 서버의 IP와 포트 번호로 TCP 연결 시도
                stream = client.GetStream(); //서버와 클라이언트가 데이터를 주고받는 스트림 생성
                lstMessages.Items.Add("서버에 연결되었습니다.");
            }
            catch (Exception ex) // ConnectAsync가 실패하면 catch 부분 실행
            {
                lstMessages.Items.Add($"서버 연결 실패: {ex.Message}");
            }
        }

        // [Send] 버튼 클릭 시 실행되는 메시지 전송 함수
        private async void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            // 연결 확인
            if (client == null || stream == null || !client.Connected)
            {
                lstMessages.Items.Add("[오류] 서버에 연결되어 있지 않습니다.");
                return;
            }

            string message = txtMessage.Text.Trim(); // 입력 메시지 가져오기 Trim은 공백제거
            if (string.IsNullOrEmpty(message)) return; // 공백이거나 빈 문자열이면 전송하지 않고 함수종료

            byte[] data = Encoding.UTF8.GetBytes(message); // UTF-8 인코딩 (바이트 데이터)
            await stream.WriteAsync(data, 0, data.Length); // 서버로 전송 WriteAsync는 바이트 데이터를 네트워크를 통해 전송

            lstMessages.Items.Add($"[나]: {message}"); // 클라이언트 자신도 메시지 표시
            txtMessage.Clear(); // 입력창 초기화
        }

        // [Disconnect] 버튼 클릭 시 서버 연결 종료
        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            if (client != null)// 클라이언트 객체가 존재할때 연결종료 실행
            {
                stream?.Close();       // 스트림 닫기  ?.은 null이 아니면 실행
                client.Close();        // 클라이언트 종료
                lstMessages.Items.Add("서버 연결 종료됨.");
                client = null;         // 객체 null 처리
                stream = null;
            }
        }
    }
}
