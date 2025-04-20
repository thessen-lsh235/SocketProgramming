namespace EchoServer
{
    partial class FormMainServer
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
            this.LV_SERVER = new System.Windows.Forms.ListView();
            this.BTN_STOP = new System.Windows.Forms.Button();
            this.BTN_OPEN = new System.Windows.Forms.Button();
            this.TB_SEND = new System.Windows.Forms.TextBox();
            this.BTN_SEND = new System.Windows.Forms.Button();
            this.LV_MANAGE_CLIENT = new System.Windows.Forms.ListView();
            this.LB_CLIENT_MANAGEMENT = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LV_SERVER
            // 
            this.LV_SERVER.HideSelection = false;
            this.LV_SERVER.Location = new System.Drawing.Point(12, 12);
            this.LV_SERVER.Name = "LV_SERVER";
            this.LV_SERVER.Size = new System.Drawing.Size(394, 426);
            this.LV_SERVER.TabIndex = 0;
            this.LV_SERVER.UseCompatibleStateImageBehavior = false;
            // 
            // BTN_STOP
            // 
            this.BTN_STOP.Location = new System.Drawing.Point(412, 59);
            this.BTN_STOP.Name = "BTN_STOP";
            this.BTN_STOP.Size = new System.Drawing.Size(115, 41);
            this.BTN_STOP.TabIndex = 1;
            this.BTN_STOP.Text = "Stop";
            this.BTN_STOP.UseVisualStyleBackColor = true;
            this.BTN_STOP.Click += new System.EventHandler(this.BTN_STOP_Click);
            // 
            // BTN_OPEN
            // 
            this.BTN_OPEN.Location = new System.Drawing.Point(412, 12);
            this.BTN_OPEN.Name = "BTN_OPEN";
            this.BTN_OPEN.Size = new System.Drawing.Size(115, 41);
            this.BTN_OPEN.TabIndex = 2;
            this.BTN_OPEN.Text = "Open";
            this.BTN_OPEN.UseVisualStyleBackColor = true;
            this.BTN_OPEN.Click += new System.EventHandler(this.BTN_OPEN_Click);
            // 
            // TB_SEND
            // 
            this.TB_SEND.Location = new System.Drawing.Point(12, 444);
            this.TB_SEND.Name = "TB_SEND";
            this.TB_SEND.Size = new System.Drawing.Size(394, 21);
            this.TB_SEND.TabIndex = 4;
            // 
            // BTN_SEND
            // 
            this.BTN_SEND.Location = new System.Drawing.Point(412, 444);
            this.BTN_SEND.Name = "BTN_SEND";
            this.BTN_SEND.Size = new System.Drawing.Size(115, 21);
            this.BTN_SEND.TabIndex = 5;
            this.BTN_SEND.Text = "Send";
            this.BTN_SEND.UseVisualStyleBackColor = true;
            this.BTN_SEND.Click += new System.EventHandler(this.BTN_SEND_Click);
            // 
            // LV_MANAGE_CLIENT
            // 
            this.LV_MANAGE_CLIENT.HideSelection = false;
            this.LV_MANAGE_CLIENT.Location = new System.Drawing.Point(412, 206);
            this.LV_MANAGE_CLIENT.Name = "LV_MANAGE_CLIENT";
            this.LV_MANAGE_CLIENT.Size = new System.Drawing.Size(115, 232);
            this.LV_MANAGE_CLIENT.TabIndex = 6;
            this.LV_MANAGE_CLIENT.UseCompatibleStateImageBehavior = false;
            // 
            // LB_CLIENT_MANAGEMENT
            // 
            this.LB_CLIENT_MANAGEMENT.AutoSize = true;
            this.LB_CLIENT_MANAGEMENT.Location = new System.Drawing.Point(412, 191);
            this.LB_CLIENT_MANAGEMENT.Name = "LB_CLIENT_MANAGEMENT";
            this.LB_CLIENT_MANAGEMENT.Size = new System.Drawing.Size(115, 12);
            this.LB_CLIENT_MANAGEMENT.TabIndex = 7;
            this.LB_CLIENT_MANAGEMENT.Text = "Client Management";
            // 
            // FormMainServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(539, 475);
            this.Controls.Add(this.LB_CLIENT_MANAGEMENT);
            this.Controls.Add(this.LV_MANAGE_CLIENT);
            this.Controls.Add(this.BTN_SEND);
            this.Controls.Add(this.TB_SEND);
            this.Controls.Add(this.BTN_OPEN);
            this.Controls.Add(this.BTN_STOP);
            this.Controls.Add(this.LV_SERVER);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "FormMainServer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "EchoServer";
            this.Load += new System.EventHandler(this.FormMainServer_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView LV_SERVER;
        private System.Windows.Forms.Button BTN_STOP;
        private System.Windows.Forms.Button BTN_OPEN;
        private System.Windows.Forms.TextBox TB_SEND;
        private System.Windows.Forms.Button BTN_SEND;
        private System.Windows.Forms.ListView LV_MANAGE_CLIENT;
        private System.Windows.Forms.Label LB_CLIENT_MANAGEMENT;
    }
}

