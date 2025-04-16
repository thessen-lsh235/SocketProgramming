using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace echo_server
{
    // MainWindow.xaml.cs - WPF 메인 윈도우 클래스
    public partial class MainWindow : Window
    {
        private TcpListener listener;   // TCP 연결을 수락하는 서버 객체
        private bool isRunning = false; // 서버 실행 여부 플래그

        public MainWindow()
        {
            InitializeComponent(); // XAML과 연결된 UI 초기화
        }

        // [서버 시작] 버튼 클릭 시 호출되는 메서드
        private async void StartServer_Click(object sender, RoutedEventArgs e)
        {
            // 모든 네트워크 인터페이스(IPAddress.Any)의 5000번 포트에서 수신 대기
            listener = new TcpListener(IPAddress.Any, 5000);
            listener.Start(); // 서버 시작
            isRunning = true;
            lstMessages.Items.Add("서버 시작됨...");

            // 비동기적으로 백그라운드에서 클라이언트 연결을 계속 수신 UI 멈춤 방지
            await Task.Run(async () => // Task.Run
            {
                while (isRunning)
                {
                    // 클라이언트가 연결되기를 기다리고 접속하면 TCPClient 객체 반환
                    TcpClient client = await listener.AcceptTcpClientAsync();

                    // 클라이언트 별로 통신을 비동기 Task로 처리 (병렬로 동작 가능)
                    _ = HandleClientAsync(client);
                }
            });
        }

        // 클라이언트와의 통신을 처리하는 비동기 메서드
        private async Task HandleClientAsync(TcpClient client)
        {
            // 클라이언트의 IP:Port 주소를 가져옴
            var endpoint = client.Client.RemoteEndPoint.ToString();

            // UI 쓰레드에서 메시지를 리스트박스에 추가
            Dispatcher.Invoke(() => lstMessages.Items.Add($"클라이언트 연결됨: {endpoint}"));// UI 접근 위해 Dispatcher 사용

            try
            {
                // 클라이언트로부터 데이터를 받을 수 있는 스트림 생성
                using var stream = client.GetStream();
                byte[] buffer = new byte[1024]; // 버퍼 크기 1KB

                while (true)
                {
                    // 클라이언트가 보낸 데이터를 비동기적으로 읽음
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                    // 클라이언트가 연결을 끊으면 0 바이트 읽힘 → 종료 조건
                    if (bytesRead == 0) break;

                    // 바이트 배열 → 문자열로 디코딩
                    string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    // UI에 수신 메시지 출력
                    Dispatcher.Invoke(() =>
                        lstMessages.Items.Add($"[{endpoint}]: {msg}")
                    );
                }
            }
            catch (Exception ex)
            {
                // 통신 중 예외 발생 시 메시지 출력
                Dispatcher.Invoke(() =>
                    lstMessages.Items.Add($"[오류] {ex.Message}")
                );
            }
            finally
            {
                // 클라이언트 종료 처리
                Dispatcher.Invoke(() =>
                    lstMessages.Items.Add($"클라이언트 종료됨: {endpoint}")
                );

                // 연결된 클라이언트 소켓 종료
                client.Close();
            }
        }
    }
}
