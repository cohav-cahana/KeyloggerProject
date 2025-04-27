namespace KeyloggerProject
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnToggleLogging = new Button();
            labelLogStatus = new Label();
            welcome = new Label();
            textBoxLiveLog = new TextBox();
            labelHackingMode = new Label();
            textBoxMatrix = new TextBox();
            labelAboutText = new Label();
            SuspendLayout();
            // 
            // btnToggleLogging
            // 
            btnToggleLogging.BackColor = Color.Black;
            btnToggleLogging.ForeColor = Color.Lime;
            btnToggleLogging.Location = new Point(11, 80);
            btnToggleLogging.Margin = new Padding(2, 3, 2, 3);
            btnToggleLogging.Name = "btnToggleLogging";
            btnToggleLogging.Size = new Size(77, 25);
            btnToggleLogging.TabIndex = 1;
            btnToggleLogging.Text = "Start Logging";
            btnToggleLogging.UseVisualStyleBackColor = false;
            btnToggleLogging.Click += btnToggleLogging_Click;
            // 
            // labelLogStatus
            // 
            labelLogStatus.AutoSize = true;
            labelLogStatus.Location = new Point(11, 108);
            labelLogStatus.Name = "labelLogStatus";
            labelLogStatus.Size = new Size(117, 20);
            labelLogStatus.TabIndex = 2;
            labelLogStatus.Text = "Logging: OFF\n";
            // 
            // welcome
            // 
            welcome.AutoSize = true;
            welcome.Font = new Font("Consolas", 19.8000011F, FontStyle.Bold, GraphicsUnit.Point, 0);
            welcome.ForeColor = SystemColors.HighlightText;
            welcome.Location = new Point(179, 30);
            welcome.Name = "welcome";
            welcome.Size = new Size(644, 40);
            welcome.TabIndex = 3;
            welcome.Text = "Welcome to our KEYLOGGER PROJECT!";
            // 
            // textBoxLiveLog
            // 
            textBoxLiveLog.BackColor = SystemColors.ActiveCaptionText;
            textBoxLiveLog.BorderStyle = BorderStyle.None;
            textBoxLiveLog.ForeColor = Color.Lime;
            textBoxLiveLog.Location = new Point(12, 140);
            textBoxLiveLog.Multiline = true;
            textBoxLiveLog.Name = "textBoxLiveLog";
            textBoxLiveLog.ReadOnly = true;
            textBoxLiveLog.ScrollBars = ScrollBars.Vertical;
            textBoxLiveLog.Size = new Size(531, 233);
            textBoxLiveLog.TabIndex = 4;
            // 
            // labelHackingMode
            // 
            labelHackingMode.Location = new Point(0, 0);
            labelHackingMode.Name = "labelHackingMode";
            labelHackingMode.Size = new Size(100, 23);
            labelHackingMode.TabIndex = 7;
            // 
            // textBoxMatrix
            // 
            textBoxMatrix.BackColor = SystemColors.ActiveCaptionText;
            textBoxMatrix.BorderStyle = BorderStyle.None;
            textBoxMatrix.ForeColor = Color.Lime;
            textBoxMatrix.Location = new Point(580, 80);
            textBoxMatrix.Multiline = true;
            textBoxMatrix.Name = "textBoxMatrix";
            textBoxMatrix.ReadOnly = true;
            textBoxMatrix.Size = new Size(390, 358);
            textBoxMatrix.TabIndex = 6;
            // 
            // labelAboutText
            // 
            labelAboutText.AutoSize = true;
            labelAboutText.ForeColor = Color.White;
            labelAboutText.Location = new Point(-2, 421);
            labelAboutText.Name = "labelAboutText";
            labelAboutText.Size = new Size(585, 20);
            labelAboutText.TabIndex = 8;
            labelAboutText.Text = "This keylogger was developed by Cohav Cahana and Maria Badarne. ";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = SystemColors.ActiveCaptionText;
            ClientSize = new Size(982, 450);
            Controls.Add(labelAboutText);
            Controls.Add(textBoxMatrix);
            Controls.Add(labelHackingMode);
            Controls.Add(textBoxLiveLog);
            Controls.Add(welcome);
            Controls.Add(labelLogStatus);
            Controls.Add(btnToggleLogging);
            Font = new Font("Consolas", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ForeColor = Color.Red;
            Location = new Point(10, 10);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btnToggleLogging;
        private Label labelLogStatus;
        private Label welcome;
        private TextBox textBoxLiveLog;
        private Label labelHackingMode;
        private TextBox textBoxMatrix;
        private Label labelAboutText;
    }
}
