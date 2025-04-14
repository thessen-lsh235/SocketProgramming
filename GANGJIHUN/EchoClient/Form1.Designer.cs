namespace EchoClient
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.LV_CLIENT = new System.Windows.Forms.ListView();
            this.TB_DATA = new System.Windows.Forms.TextBox();
            this.BTN_SEND = new System.Windows.Forms.Button();
            this.BTN_CONNECT = new System.Windows.Forms.Button();
            this.BTN_DISCONNECT = new System.Windows.Forms.Button();
            this.BTN_EXIT = new System.Windows.Forms.Button();
            this.TB_PORT = new System.Windows.Forms.TextBox();
            this.LB_PORT = new System.Windows.Forms.Label();
            this.LB_IP = new System.Windows.Forms.Label();
            this.TB_IP = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // LV_CLIENT
            // 
            this.LV_CLIENT.HideSelection = false;
            this.LV_CLIENT.Location = new System.Drawing.Point(12, 12);
            this.LV_CLIENT.Name = "LV_CLIENT";
            this.LV_CLIENT.Size = new System.Drawing.Size(532, 399);
            this.LV_CLIENT.TabIndex = 0;
            this.LV_CLIENT.UseCompatibleStateImageBehavior = false;
            // 
            // TB_DATA
            // 
            this.TB_DATA.Location = new System.Drawing.Point(12, 417);
            this.TB_DATA.Name = "TB_DATA";
            this.TB_DATA.Size = new System.Drawing.Size(451, 21);
            this.TB_DATA.TabIndex = 1;
            // 
            // BTN_SEND
            // 
            this.BTN_SEND.Location = new System.Drawing.Point(469, 416);
            this.BTN_SEND.Name = "BTN_SEND";
            this.BTN_SEND.Size = new System.Drawing.Size(75, 21);
            this.BTN_SEND.TabIndex = 2;
            this.BTN_SEND.Text = "Send";
            this.BTN_SEND.UseVisualStyleBackColor = true;
            this.BTN_SEND.Click += new System.EventHandler(this.BTN_SEND_Click);
            // 
            // BTN_CONNECT
            // 
            this.BTN_CONNECT.Location = new System.Drawing.Point(550, 93);
            this.BTN_CONNECT.Name = "BTN_CONNECT";
            this.BTN_CONNECT.Size = new System.Drawing.Size(90, 30);
            this.BTN_CONNECT.TabIndex = 3;
            this.BTN_CONNECT.Text = "Connect";
            this.BTN_CONNECT.UseVisualStyleBackColor = true;
            this.BTN_CONNECT.Click += new System.EventHandler(this.BTN_CONNECT_Click);
            // 
            // BTN_DISCONNECT
            // 
            this.BTN_DISCONNECT.Location = new System.Drawing.Point(550, 129);
            this.BTN_DISCONNECT.Name = "BTN_DISCONNECT";
            this.BTN_DISCONNECT.Size = new System.Drawing.Size(90, 30);
            this.BTN_DISCONNECT.TabIndex = 4;
            this.BTN_DISCONNECT.Text = "Disconnect";
            this.BTN_DISCONNECT.UseVisualStyleBackColor = true;
            this.BTN_DISCONNECT.Click += new System.EventHandler(this.BTN_DISCONNECT_Click);
            // 
            // BTN_EXIT
            // 
            this.BTN_EXIT.Location = new System.Drawing.Point(550, 165);
            this.BTN_EXIT.Name = "BTN_EXIT";
            this.BTN_EXIT.Size = new System.Drawing.Size(90, 30);
            this.BTN_EXIT.TabIndex = 5;
            this.BTN_EXIT.Text = "Exit";
            this.BTN_EXIT.UseVisualStyleBackColor = true;
            this.BTN_EXIT.Click += new System.EventHandler(this.BTN_EXIT_Click);
            // 
            // TB_PORT
            // 
            this.TB_PORT.Location = new System.Drawing.Point(550, 66);
            this.TB_PORT.Name = "TB_PORT";
            this.TB_PORT.Size = new System.Drawing.Size(90, 21);
            this.TB_PORT.TabIndex = 6;
            this.TB_PORT.Text = "9000";
            // 
            // LB_PORT
            // 
            this.LB_PORT.AutoSize = true;
            this.LB_PORT.Location = new System.Drawing.Point(550, 51);
            this.LB_PORT.Name = "LB_PORT";
            this.LB_PORT.Size = new System.Drawing.Size(27, 12);
            this.LB_PORT.TabIndex = 7;
            this.LB_PORT.Text = "Port";
            // 
            // LB_IP
            // 
            this.LB_IP.AutoSize = true;
            this.LB_IP.Location = new System.Drawing.Point(550, 12);
            this.LB_IP.Name = "LB_IP";
            this.LB_IP.Size = new System.Drawing.Size(16, 12);
            this.LB_IP.TabIndex = 9;
            this.LB_IP.Text = "IP";
            // 
            // TB_IP
            // 
            this.TB_IP.Location = new System.Drawing.Point(550, 27);
            this.TB_IP.Name = "TB_IP";
            this.TB_IP.Size = new System.Drawing.Size(90, 21);
            this.TB_IP.TabIndex = 8;
            this.TB_IP.Text = "127.0.0.1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(651, 450);
            this.Controls.Add(this.LB_IP);
            this.Controls.Add(this.TB_IP);
            this.Controls.Add(this.LB_PORT);
            this.Controls.Add(this.TB_PORT);
            this.Controls.Add(this.BTN_EXIT);
            this.Controls.Add(this.BTN_DISCONNECT);
            this.Controls.Add(this.BTN_CONNECT);
            this.Controls.Add(this.BTN_SEND);
            this.Controls.Add(this.TB_DATA);
            this.Controls.Add(this.LV_CLIENT);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "Form1";
            this.Text = "EchoClient";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView LV_CLIENT;
        private System.Windows.Forms.TextBox TB_DATA;
        private System.Windows.Forms.Button BTN_SEND;
        private System.Windows.Forms.Button BTN_CONNECT;
        private System.Windows.Forms.Button BTN_DISCONNECT;
        private System.Windows.Forms.Button BTN_EXIT;
        private System.Windows.Forms.TextBox TB_PORT;
        private System.Windows.Forms.Label LB_PORT;
        private System.Windows.Forms.Label LB_IP;
        private System.Windows.Forms.TextBox TB_IP;
    }
}

