using System;
using System.Windows.Forms;

namespace EchoClient
{
    public partial class FormMainClient : Form
    {
        #region PV
        private ClassLogManager logManager;
        private ClassClientManager clientManager;
        #endregion

        public FormMainClient()
        {
            InitializeComponent();
        }

        private void FormMainClient_Load(object sender, EventArgs e)
        {
            logManager = new ClassLogManager(LV_CLIENT);
            clientManager = new ClassClientManager();

            clientManager.eventLog += (log) =>
            {
                Invoke(new Action(() => logManager.addLog(log)));
            };

            clientManager.eventConnected += () =>
            {
                Invoke(new Action(() => setButtonStates(true)));
            };

            clientManager.eventDisconnected += () =>
            {
                Invoke(new Action(() => setButtonStates(false)));
            };
        }

        #region manage Buttons
        //
        //  버튼 상태 관리
        //
        private void setButtonStates(bool isConnected)
        {
            BTN_CONNECT.Enabled = !isConnected;
            BTN_DISCONNECT.Enabled = isConnected;
            BTN_SEND.Enabled = isConnected;
        }
        
        //
        //  버튼 이벤트 처리
        //  BTN_SEND_Click, BTN_CONNECT_Click, BTN_DISCONNECT_Click
        //
        private void BTN_SEND_Click(object sender, EventArgs e)
        {
            if (clientManager.isConnected)
            {
                try
                {
                    string message = TB_DATA.Text.Trim();
                    if (!string.IsNullOrEmpty(message))
                    {
                        clientManager.sendData(message);
                        TB_DATA.Clear();
                    }
                }

                catch (Exception ex)
                {
                    logManager.addLog("BTN_SEND_Click 오류 " + ex.Message);
                }
            }
        }

        private void BTN_CONNECT_Click(object sender, EventArgs e)
        {
            clientManager.connectClient(TB_IP.Text, int.Parse(TB_PORT.Text));
        }

        private void BTN_DISCONNECT_Click(object sender, EventArgs e)
        {
            clientManager.disconnectClient();
        }
        #endregion
    }
}