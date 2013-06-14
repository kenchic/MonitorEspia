namespace DesktopCapture
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.propGridControl = new System.Windows.Forms.PropertyGrid();
            this.captureTimerControl = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.mailSendTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // propGridControl
            // 
            this.propGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propGridControl.Location = new System.Drawing.Point(0, 0);
            this.propGridControl.Name = "propGridControl";
            this.propGridControl.Size = new System.Drawing.Size(292, 266);
            this.propGridControl.TabIndex = 0;
            this.propGridControl.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propGridControl_PropertyValueChanged);
            // 
            // captureTimerControl
            // 
            this.captureTimerControl.Enabled = true;
            this.captureTimerControl.Tick += new System.EventHandler(this.captureTimerControl_Tick);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Mobile Device Connected";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseUp);
            this.notifyIcon1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDown);
            // 
            // mailSendTimer
            // 
            this.mailSendTimer.Enabled = true;
            this.mailSendTimer.Interval = 300000;
            this.mailSendTimer.Tick += new System.EventHandler(this.mailSendTimer_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.propGridControl);
            this.Name = "MainForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DesktopCapture";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid propGridControl;
        private System.Windows.Forms.Timer captureTimerControl;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Timer mailSendTimer;
    }
}

