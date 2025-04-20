using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ClassServerManager
{
    #region PV
    public bool isRunning = false;

    private Thread serverListenerThread;

    private TcpListener server;
    private int serverPort = 9000;
    Dictionary<string, TcpClient> manageClients = new Dictionary<string, TcpClient>();    

    private ClassClientManager clientManager;
    #endregion

    #region PE
    public event Action<string> eventLog;
    public event Action eventConnected;
    public event Action eventDisconnected;
    #endregion

    public ClassServerManager(ClassClientManager clientManager)
    {
        this.clientManager = clientManager;
    }

    #region manage Server State

    public async Task startServer(int port)
    {
        if (isRunning)
        {
            return;
        }

        serverPort = port;
        isRunning = true;

        serverListenerThread = new Thread(() => runServer()) { IsBackground = true };
        serverListenerThread.Start();

        eventConnected?.Invoke();        
        eventLog?.Invoke($"서버 시작됨 (포트: {port})");
    }

    private async Task runServer()
    {
        try
        {
            server = new TcpListener(IPAddress.Any, serverPort);
            server.Start();
            eventLog?.Invoke("클라이언트 다 드르와;");

            while (isRunning)
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                string clientKey = ((IPEndPoint)client.Client.RemoteEndPoint).ToString();

                manageClients[clientKey] = client;
                clientManager.addClient(clientKey);

                Task.Run(() => receiveLoop(client, clientKey));

                eventLog?.Invoke($"연결 됐수다: {clientKey}");
                eventConnected?.Invoke();
            }
        }

        catch (SocketException ex)
        {
            eventLog?.Invoke($"서버 ㅂㅂ, {ex.Message}");
        }
    }    

    public void stopServer()
    {
        isRunning = false;
        try
        {
            server?.Stop();
            foreach (var client in manageClients.Values)
            {
                client?.Close();
            }

            manageClients.Clear();
            eventLog?.Invoke("-- 서버 the end--");
            eventDisconnected?.Invoke();
        }
        catch (Exception ex)
        {
            eventLog?.Invoke($"[stopServer catch문] {ex.Message}");
        }
    }
    #endregion

    #region Send/Receive Loop
    private async Task receiveLoop(TcpClient client, string clientKey)
    {
        try
        {
            var stream = client.GetStream();
            var buffer = new byte[1024];

            while (isRunning)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    break;
                }

                string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                eventLog.Invoke($"[수신] {clientKey} -> {receivedData}");
            }
        }
        catch (Exception ex)
        {
            eventLog?.Invoke($"클라이언트 에러 ;;, {ex.Message}");
        }
        finally
        {
            eventLog?.Invoke("클라이언트 ㅂㅂ");
            clientManager.removeClient(clientKey);
            manageClients.Remove(clientKey);
            closeSocket(client);
            eventDisconnected?.Invoke();
        }
    }

    public async Task sendLoop(string clientKey, string message)
    {
        if (manageClients.TryGetValue(clientKey, out TcpClient client))
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                await client.GetStream().WriteAsync(data, 0, data.Length);
                eventLog?.Invoke($"[송신] {message} ->  {clientKey}");
            }

            catch (Exception ex)
            {
                eventLog?.Invoke($"[sendLoop catch문] {clientKey}: {ex.Message}");
            }
        }
        else
        {
            eventLog?.Invoke("해당 클라이언트가 연결ㄴㄴ임");
        }
    }
    #endregion

    private void closeSocket(TcpClient client)
    {
        if (client == null) return;

        try
        {
            client.Client.Shutdown(SocketShutdown.Both);
        }
        catch { }

        try
        {
            client.GetStream().Close();
        }
        catch { }

        try
        {
            client.Close();
        }
        catch { }
    }

    #region manageThread
    private void startThread(ref Thread thread, ThreadStart targetMethod)
    {
        thread = new Thread(targetMethod);
        thread.IsBackground = true;
        thread.Start();
    }

    private void stopThread(Thread thread)
    {
        if (thread != null && thread.IsAlive)
        {
            thread.Join(1000);
            thread = null;
        }
    }
    #endregion
}
