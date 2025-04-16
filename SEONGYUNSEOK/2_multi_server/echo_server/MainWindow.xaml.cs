using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace echo_server
{
    public partial class MainWindow : Window
    {
        private TcpListener listener; // TcpListener는 수신 전용 소켓 (클라이언트의 접속을 기다리고 받아들이는 객체)
        private bool isRunning = false; // 서버가 현재 실행 중인지 여부를 나타내는 플래그
        private Dictionary<string, ClientInfo> clients = new(); // 현재 연결된 클라이언트들을 관리하는 자료구조
        // string은 클라이언트 식별자, ClientInfo는 해당 클라이언트 정보 


        public MainWindow()
        {
            InitializeComponent();
            UpdateClientCount();
        }

        private async void StartServer_Click(object sender, RoutedEventArgs e)
        {
            listener = new TcpListener(IPAddress.Any, 5000);
            listener.Start(); // 위에서 설정한 포트로 실제 리스닝(수신)을 시작
            isRunning = true; // 서버가 현재 실행중이므로 flag를 true로
            lstMessages.Items.Add("서버 시작됨...");

            await Task.Run(async () =>
            {
                while (isRunning)
                {
                    // 클라이언트가 서버에 접속해 올 때까지 기다렸다가, 연결되면 그 클라이언트를 나타내는 TcpClient 객체를 반환
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    _ = HandleClientAsync(client); // HandleClientAsync으로 client 넘김
                }
            });
        }

        private void UpdateClientCount()
        {
            lblClientCount.Content = $"클라이언트 수: {clients.Count}명";
        }

        // 클라이언트에서 메세지 수신
        private async Task HandleClientAsync(TcpClient client)
        {
            var endpoint = client.Client.RemoteEndPoint.ToString(); // 접속한 클라이언트의 IP와 포트 정보(IP:PORT)를 문자열로 저장
            var stream = client.GetStream(); // 클라이언트와 데이터를 주고받기 위한 NetworkStream 객체
            byte[] buffer = new byte[1024]; // 수신된 데이터를 담을 임시저장소(버퍼)

            var clientInfo = new ClientInfo // 클라이언트의 기본 정보를 담은 ClientInfo 객체
            {
                Client = client,
                Stream = stream,
                EndPoint = endpoint // endpoint에 클라이언트의 IP와 포트
            };

            string displayName = ""; // 나중에 Client에서 이름,ID가 들어오면 저장

            try
            {
                // 클라이언트가 보내는 첫 메세지 수신(로그인 정보)
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) return;

                // 로그인 정보 json으로 변환하고 역직렬화
                string loginJson = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                ChatMessage loginMsg = JsonSerializer.Deserialize<ChatMessage>(loginJson);

                if (loginMsg.MessageType == "Login") // 객체 안의 메세지 타입이 Login이면
                {
                    // Clientinfo 객체에 유저 정보 저장
                    clientInfo.UserId = loginMsg.UserId;
                    clientInfo.UserName = loginMsg.Sender;
                    displayName = clientInfo.DisplayName;

                    clients[endpoint] = clientInfo; // 딕셔너리에 클라이언트 등록

                    Dispatcher.Invoke(() =>
                    {
                        comboClients.Items.Add(displayName); // 이름(ID)으로 표시
                        lstMessages.Items.Add($"[알림] {loginMsg.Sender} 접속");
                        UpdateClientCount();
                    });
                }

                // 메세지 수신 루프
                while (client.Connected)
                {
                    bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length); // 비동기로 데이터 수신
                    if (bytesRead == 0) break; //0바이트 수신은 클라이언트가 연결을 끊은것

                    string json = Encoding.UTF8.GetString(buffer, 0, bytesRead); // json 문자열로 복원
                    ChatMessage msg = JsonSerializer.Deserialize<ChatMessage>(json); // json을 메시지 객체로 복원(역직렬화)

                    Dispatcher.Invoke(() =>
                    {
                        lstMessages.Items.Add($"[{clientInfo.UserName}]: {msg.Message}");
                    });
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                    lstMessages.Items.Add($"[오류] {ex.Message}")
                );
            }
            finally
            {
                Dispatcher.Invoke(() =>
                {
                    lstMessages.Items.Add($"클라이언트 종료됨: {clientInfo.UserName}");

                    //DisplayName 기준으로 콤보박스에서 삭제
                    comboClients.Items.Remove(displayName);

                    //EndPoint 기준으로 딕셔너리에서 제거
                    clients.Remove(endpoint);

                    // 콤보박스에 남은 항목이 있다면 첫 번째를 자동 선택
                    if (comboClients.Items.Count > 0)
                        comboClients.SelectedIndex = 0;
                    else
                        txtClientInfo.Text = "";

                    UpdateClientCount();
                });

                client.Close(); // 소켓 닫기
            }
        }


        private void comboClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ComboBox에서 현재 선택된 항목(=DisplayName)을 문자열로 꺼냄
            string selectedName = comboClients.SelectedItem as string; //as string은 형변환

            if (selectedName != null)
            {
                foreach (var client in clients.Values) // 딕셔너리의 value들(Clientinfo들)을 꺼냄
                {
                    if (client.DisplayName == selectedName)
                    {
                        //텍스트박스에 출력
                        txtClientInfo.Text = $"이름: {client.UserName}\nID: {client.UserId}\n주소: {client.EndPoint}";
                        break;
                    }
                }
            }
        }

        //전송 버튼 누르면 실행
        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            // ComboBox에서 선택된 클라이언트 이름(=DisplayName)을 문자열로
            string selectedName = comboClients.SelectedItem as string; 

            if (string.IsNullOrEmpty(selectedName)) return; // 선택된 클라이언트가 없거나 비어있다면 함수를 종료

            ClientInfo targetClient = null; // 전송할 대상 클라이언트를 담을 변수(아직 찾기 전이므로 null)
            foreach (var client in clients.Values) // 현재 연결된 모든 클라이언트(ClientInfo)를 순회
            {
                if (client.DisplayName == selectedName) // ComboBox에서 선택한 이름과 ClientInfo에 저장된 이름이 같은지 비교
                {
                    targetClient = client; // 일치하는 클라이언트 찾았으면 targetClient에 저장
                    break;
                }
            }

            if (targetClient == null) return; // 못찾았으면 종료

            string message = txtSendMessage.Text.Trim(); // 전송할 메세지를 입력창에서 받아옴
            if (string.IsNullOrEmpty(message)) return;

            var msg = new ChatMessage // 전송할 메세지를 ChatMessage 객체에 저장
            {
                Sender = "Server",
                UserId = "server",
                Message = message,
                Timestamp = DateTime.Now,
                MessageType = "Chat"
            };

            string json = JsonSerializer.Serialize(msg); // json 문자열로 직렬화
            byte[] data = Encoding.UTF8.GetBytes(json); // 문자열을 바이트 배열로 변환
            await targetClient.Stream.WriteAsync(data, 0, data.Length); // 해당 클라이언트의 NetworkStream을 통해 비동기로 전송

            lstMessages.Items.Add($"[서버 → {targetClient.DisplayName}]: {message}");
            txtSendMessage.Clear(); // 전송 후 입력창 비우기
        }

    }

    public class ClientInfo
    {
        public TcpClient Client { get; set; }
        public NetworkStream Stream { get; set; }
        public string EndPoint { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }

        //사용자 보기용 문자열
        public string DisplayName => $"{UserName} ({UserId})";
    }


    public class ChatMessage
    {
        public string Sender { get; set; }
        public string UserId { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public string MessageType { get; set; } // "Login", "Chat"
    }
}
