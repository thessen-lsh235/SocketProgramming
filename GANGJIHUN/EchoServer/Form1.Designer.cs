namespace EchoServer
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
            this.LV_SERVER = new System.Windows.Forms.ListView();
            this.BTN_STOP = new System.Windows.Forms.Button();
            this.BTN_OPEN = new System.Windows.Forms.Button();
            this.BTN_EXIT = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LV_SERVER
            // 
            this.LV_SERVER.HideSelection = false;
            this.LV_SERVER.Location = new System.Drawing.Point(17, 18);
            this.LV_SERVER.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.LV_SERVER.Name = "LV_SERVER";
            this.LV_SERVER.Size = new System.Drawing.Size(561, 637);
            this.LV_SERVER.TabIndex = 0;
            this.LV_SERVER.UseCompatibleStateImageBehavior = false;
            // 
            // BTN_STOP
            // 
            this.BTN_STOP.Location = new System.Drawing.Point(589, 88);
            this.BTN_STOP.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BTN_STOP.Name = "BTN_STOP";
            this.BTN_STOP.Size = new System.Drawing.Size(139, 62);
            this.BTN_STOP.TabIndex = 1;
            this.BTN_STOP.Text = "Stop";
            this.BTN_STOP.UseVisualStyleBackColor = true;
            this.BTN_STOP.Click += new System.EventHandler(this.BTN_STOP_Click);
            // 
            // BTN_OPEN
            // 
            this.BTN_OPEN.Location = new System.Drawing.Point(589, 18);
            this.BTN_OPEN.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BTN_OPEN.Name = "BTN_OPEN";
            this.BTN_OPEN.Size = new System.Drawing.Size(139, 62);
            this.BTN_OPEN.TabIndex = 2;
            this.BTN_OPEN.Text = "Open";
            this.BTN_OPEN.UseVisualStyleBackColor = true;
            this.BTN_OPEN.Click += new System.EventHandler(this.BTN_OPEN_Click);
            // 
            // BTN_EXIT
            // 
            this.BTN_EXIT.Location = new System.Drawing.Point(589, 159);
            this.BTN_EXIT.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BTN_EXIT.Name = "BTN_EXIT";
            this.BTN_EXIT.Size = new System.Drawing.Size(139, 62);
            this.BTN_EXIT.TabIndex = 3;
            this.BTN_EXIT.Text = "Exit";
            this.BTN_EXIT.UseVisualStyleBackColor = true;
            this.BTN_EXIT.Click += new System.EventHandler(this.BTN_EXIT_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 675);
            this.Controls.Add(this.BTN_EXIT);
            this.Controls.Add(this.BTN_OPEN);
            this.Controls.Add(this.BTN_STOP);
            this.Controls.Add(this.LV_SERVER);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "EchoServer";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView LV_SERVER;
        private System.Windows.Forms.Button BTN_STOP;
        private System.Windows.Forms.Button BTN_OPEN;
        private System.Windows.Forms.Button BTN_EXIT;
    }
}

