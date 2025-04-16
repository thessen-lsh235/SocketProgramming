using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace echo_client
{
    public partial class MainWindow : Window // WPF의 Window 클래스를 상속
    {
        private TcpClient client; // 클라이언트 소켓 객체
        private NetworkStream stream; // client로부터 얻는 네트워크 스트림
        private CancellationTokenSource cts; // 비동기 작업을 강제로 종료시키기 위한 토큰

        private string userName; // 클라이언트의 사용자 정보
        private string userId;

        public MainWindow()
        {
            InitializeComponent(); // XAML로 만든 UI 요소들을 메모리에 생성하고 화면에 표시
            SetDisconnectedUI(); // 초기 상태를 “서버에 연결되지 않음”으로 표시
        }

        // RoutedEventArgs는 이벤트에 대한 상세 정보 객체,이벤트가 어느 컨트롤에서 발생했고 어떻게 전달되고 있는지 확인하기 위해
        // 서버 연결 버튼 누르면 실행
        private async void Connect_Click(object sender, RoutedEventArgs e)
        {
            userName = txtUserName.Text.Trim(); // 유저 이름을 입력창에서 받아옴
            userId = txtUserId.Text.Trim();

            // 이름이나 ID 둘 중 하나라도 비어 있으면 경고 메시지를 띄우고 연결을 시도하지 않음
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userId))
            {
                MessageBox.Show("이름과 ID를 모두 입력하세요.");
                return;
            }

            try
            {
                client = new TcpClient(); // TCPClient 객체 새로 생성
                await client.ConnectAsync("127.0.0.1", 5000); // 로컬 서버의 5000번 포트에 접속 시도
                stream = client.GetStream(); // 연결이 성공되면 데이터 송수신을 위한 NetworkStream 가져옴

                // 서버에게 보낼 로그인 메시지를 구성
                var loginMsg = new ChatMessage
                {
                    Sender = userName,
                    UserId = userId,
                    Message = "",
                    Timestamp = DateTime.Now,
                    MessageType = "Login"
                };

                string loginJson = JsonSerializer.Serialize(loginMsg); // ChatMessage 객체를 JSON 문자열로 직렬화 (전송에 쉽도록)
                byte[] loginData = Encoding.UTF8.GetBytes(loginJson); // 직렬화된 문자열을 UTF-8 바이트 배열로 변환 (네트워크 전송은 바이트 단위)
                await stream.WriteAsync(loginData, 0, loginData.Length); // 서버로 로그인 메시지를 비동기로 전송

                lstMessages.Items.Add("서버에 로그인 정보를 보냈습니다.");
                SetConnectedUI(); // 연결 상태 "연결됨"으로 업데이트

                cts = new CancellationTokenSource(); // 서버로부터 메시지를 계속 받을 수신 쓰레드를 중단할 수 있는 토큰 생성
                _ = Task.Run(() => ReceiveLoop(cts.Token)); // 백그라운드 쓰레드에서 ReceiveLoop() 실행 (비동기로 계속 수신)
            }
            // 연결 실패
            catch (Exception ex)
            {
                lstMessages.Items.Add($"서버 연결 실패: {ex.Message}");
                SetDisconnectedUI();
            }
        }

        // 메인 UI 쓰레드를 막지 않기 위해 별도 쓰레드 실행
        // CancellationToken을 받아서 외부에서 중단 가능
        private async Task ReceiveLoop(CancellationToken token)
        {
            byte[] buffer = new byte[1024]; // 서버에서 수신할 데이터를 담을 임시 저장소
            try
            {
                while (!token.IsCancellationRequested) // 취소 요청 들어올때까지
                {
                    // bytesRead에 실제 수신된 바이트 수 저장
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token); // 서버에서 실제 메시지를 읽어오는 부분
                    if (bytesRead == 0) break; // 서버 연결 끊김

                    string json = Encoding.UTF8.GetString(buffer, 0, bytesRead); // 받은 바이트 데이터를 UTF-8 문자열로 복원
                    ChatMessage msg = JsonSerializer.Deserialize<ChatMessage>(json); // JSON 문자열을 ChatMessage 객체로 역직렬화

                    Dispatcher.Invoke(() =>
                    {
                        lstMessages.Items.Add($"[{msg.Timestamp:HH:mm:ss}] {msg.Sender}: {msg.Message}");
                    });
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                    lstMessages.Items.Add($"[수신 오류] {ex.Message}")
                );
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

            var msg = new ChatMessage // ChatMessage class 객체
            {
                Sender = userName,
                UserId = userId,
                Message = message,
                Timestamp = DateTime.Now,
                MessageType = "Chat"
            };
            //json을 쓰는 이유?
            // 직관적이고 범용적(파이썬, javascript 등), 확장성 좋음, 구조적으로 전송 가능
            //직렬화란?
            //객체를 저장하거나 전송하기 위해 문자열 또는 바이트로 바꾸는 것
            string json = JsonSerializer.Serialize(msg); // Json 문자열로 직렬화
            byte[] data = Encoding.UTF8.GetBytes(json); // UTF-8로 변환
            await stream.WriteAsync(data, 0, data.Length); // 서버로 전송

            lstMessages.Items.Add($"[나]: {message}");
            txtMessage.Clear();
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            cts?.Cancel(); // 수신 중인 ReceiveLoop 쓰레드를 중단시키는 명령
            stream?.Close(); // ?.는 null이 아닐때 호출
            client?.Close();
            client = null; // 객체 메모리 해제
            stream = null;
            lstMessages.Items.Add("서버 연결 종료됨.");
            SetDisconnectedUI();
        }

        private void SetConnectedUI()
        {
            lblStatus.Content = "연결됨";
            lblStatus.Foreground = Brushes.Green;
        }

        private void SetDisconnectedUI()
        {
            lblStatus.Content = "연결되지 않음";
            lblStatus.Foreground = Brushes.Red;
        }
    }

    // 이 클래스는 채팅 메시지 하나를 구성하는 데이터를 담는 그릇(객체)
    public class ChatMessage
    {
        public string Sender { get; set; }
        public string UserId { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; } //  이 메시지가 언제 생성되었는지
        public string MessageType { get; set; }
    }
}
