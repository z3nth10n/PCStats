namespace PCStats
{
    partial class frmCredentials
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabLogin = new System.Windows.Forms.TabPage();
            this.chkRemember = new System.Windows.Forms.CheckBox();
            this.lblLogin = new PCStats.NotifyLabel();
            this.button1 = new System.Windows.Forms.Button();
            this.txtPassword = new PCStats.ExTextBox();
            this.txtUsername = new PCStats.ExTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabRegister = new System.Windows.Forms.TabPage();
            this.label11 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.pictureBox1 = new PCStats.Captcha();
            this.lblRegister = new PCStats.NotifyLabel();
            this.txtSolution = new PCStats.ExTextBox();
            this.txtRepeatEmail = new PCStats.ExTextBox();
            this.txtEmail = new PCStats.ExTextBox();
            this.txtRepeatPassword = new PCStats.ExTextBox();
            this.txtRegisterPassword = new PCStats.ExTextBox();
            this.txtRegisterUsername = new PCStats.ExTextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.languageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.espanolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.englishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1.SuspendLayout();
            this.tabLogin.SuspendLayout();
            this.lblLogin.SuspendLayout();
            this.tabRegister.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.lblRegister.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabLogin);
            this.tabControl1.Controls.Add(this.tabRegister);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(344, 397);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabLogin
            // 
            this.tabLogin.Controls.Add(this.chkRemember);
            this.tabLogin.Controls.Add(this.lblLogin);
            this.tabLogin.Controls.Add(this.button1);
            this.tabLogin.Controls.Add(this.txtPassword);
            this.tabLogin.Controls.Add(this.txtUsername);
            this.tabLogin.Controls.Add(this.label2);
            this.tabLogin.Controls.Add(this.label1);
            this.tabLogin.Location = new System.Drawing.Point(4, 22);
            this.tabLogin.Name = "tabLogin";
            this.tabLogin.Padding = new System.Windows.Forms.Padding(3);
            this.tabLogin.Size = new System.Drawing.Size(336, 371);
            this.tabLogin.TabIndex = 0;
            this.tabLogin.Text = "Login";
            this.tabLogin.UseVisualStyleBackColor = true;
            // 
            // chkRemember
            // 
            this.chkRemember.AutoSize = true;
            this.chkRemember.Location = new System.Drawing.Point(6, 107);
            this.chkRemember.Name = "chkRemember";
            this.chkRemember.Size = new System.Drawing.Size(137, 17);
            this.chkRemember.TabIndex = 6;
            this.chkRemember.Text = "Remember credentials?";
            this.chkRemember.UseVisualStyleBackColor = true;
            this.chkRemember.CheckedChanged += new System.EventHandler(this.chkRemember_CheckedChanged);
            // 
            // lblLogin
            // 
            this.lblLogin.Location = new System.Drawing.Point(8, 3);
            this.lblLogin.MyOrder = 0;
            this.lblLogin.Name = "lblLogin";
            this.lblLogin.Size = new System.Drawing.Size(210, 23);
            this.lblLogin.TabIndex = 5;
            this.lblLogin.Text = "Log in into Lerp2Dev website:";
            this.lblLogin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(80, 130);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Login";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.BackColor = System.Drawing.SystemColors.ControlDark;
            this.txtPassword.DefaultBorderColor = System.Drawing.SystemColors.ControlDark;
            this.txtPassword.ErrorBorderColor = System.Drawing.Color.Red;
            this.txtPassword.FocusedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(190)))), ((int)(((byte)(247)))));
            this.txtPassword.Location = new System.Drawing.Point(6, 81);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Padding = new System.Windows.Forms.Padding(1);
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(212, 20);
            this.txtPassword.TabIndex = 3;
            this.txtPassword.ValidBorderColor = System.Drawing.Color.Green;
            this.txtPassword.Leave += new System.EventHandler(this.txtPassword_Leave);
            // 
            // txtUsername
            // 
            this.txtUsername.BackColor = System.Drawing.SystemColors.ControlDark;
            this.txtUsername.DefaultBorderColor = System.Drawing.SystemColors.ControlDark;
            this.txtUsername.ErrorBorderColor = System.Drawing.Color.Red;
            this.txtUsername.FocusedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(190)))), ((int)(((byte)(247)))));
            this.txtUsername.Location = new System.Drawing.Point(6, 42);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Padding = new System.Windows.Forms.Padding(1);
            this.txtUsername.PasswordChar = '\0';
            this.txtUsername.Size = new System.Drawing.Size(212, 20);
            this.txtUsername.TabIndex = 2;
            this.txtUsername.ValidBorderColor = System.Drawing.Color.Green;
            this.txtUsername.Leave += new System.EventHandler(this.txtUsername_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Password:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Username:";
            // 
            // tabRegister
            // 
            this.tabRegister.Controls.Add(this.label11);
            this.tabRegister.Controls.Add(this.label9);
            this.tabRegister.Controls.Add(this.button2);
            this.tabRegister.Controls.Add(this.label10);
            this.tabRegister.Controls.Add(this.label7);
            this.tabRegister.Controls.Add(this.label8);
            this.tabRegister.Controls.Add(this.label5);
            this.tabRegister.Controls.Add(this.label6);
            this.tabRegister.Controls.Add(this.pictureBox1);
            this.tabRegister.Controls.Add(this.lblRegister);
            this.tabRegister.Controls.Add(this.txtSolution);
            this.tabRegister.Controls.Add(this.txtRepeatEmail);
            this.tabRegister.Controls.Add(this.txtEmail);
            this.tabRegister.Controls.Add(this.txtRepeatPassword);
            this.tabRegister.Controls.Add(this.txtRegisterPassword);
            this.tabRegister.Controls.Add(this.txtRegisterUsername);
            this.tabRegister.Location = new System.Drawing.Point(4, 22);
            this.tabRegister.Name = "tabRegister";
            this.tabRegister.Padding = new System.Windows.Forms.Padding(3);
            this.tabRegister.Size = new System.Drawing.Size(336, 371);
            this.tabRegister.TabIndex = 1;
            this.tabRegister.Text = "Register";
            this.tabRegister.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 316);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(48, 13);
            this.label11.TabIndex = 22;
            this.label11.Text = "Solution:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 221);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(50, 13);
            this.label9.TabIndex = 20;
            this.label9.Text = "Captcha:";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(122, 343);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 19;
            this.button2.Text = "Register";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 182);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(72, 13);
            this.label10.TabIndex = 15;
            this.label10.Text = "Repeat email:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 143);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Email:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 104);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(93, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "Repeat password:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Password:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 26);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Username:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(6, 237);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(322, 71);
            this.pictureBox1.TabIndex = 19;
            this.pictureBox1.TabStop = false;
            // 
            // lblRegister
            // 
            this.lblRegister.Location = new System.Drawing.Point(8, 3);
            this.lblRegister.MyOrder = 0;
            this.lblRegister.Name = "lblRegister";
            this.lblRegister.Size = new System.Drawing.Size(320, 23);
            this.lblRegister.TabIndex = 10;
            this.lblRegister.Text = "Create a new account into Lerp2Dev website:";
            this.lblRegister.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtSolution
            // 
            this.txtSolution.BackColor = System.Drawing.SystemColors.ControlDark;
            this.txtSolution.DefaultBorderColor = System.Drawing.SystemColors.ControlDark;
            this.txtSolution.ErrorBorderColor = System.Drawing.Color.Red;
            this.txtSolution.FocusedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(190)))), ((int)(((byte)(247)))));
            this.txtSolution.Location = new System.Drawing.Point(60, 314);
            this.txtSolution.Name = "txtSolution";
            this.txtSolution.Padding = new System.Windows.Forms.Padding(1);
            this.txtSolution.PasswordChar = '\0';
            this.txtSolution.Size = new System.Drawing.Size(268, 20);
            this.txtSolution.TabIndex = 18;
            this.txtSolution.ValidBorderColor = System.Drawing.Color.Green;
            // 
            // txtRepeatEmail
            // 
            this.txtRepeatEmail.BackColor = System.Drawing.SystemColors.ControlDark;
            this.txtRepeatEmail.DefaultBorderColor = System.Drawing.SystemColors.ControlDark;
            this.txtRepeatEmail.ErrorBorderColor = System.Drawing.Color.Red;
            this.txtRepeatEmail.FocusedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(190)))), ((int)(((byte)(247)))));
            this.txtRepeatEmail.Location = new System.Drawing.Point(6, 198);
            this.txtRepeatEmail.Name = "txtRepeatEmail";
            this.txtRepeatEmail.Padding = new System.Windows.Forms.Padding(1);
            this.txtRepeatEmail.PasswordChar = '\0';
            this.txtRepeatEmail.Size = new System.Drawing.Size(322, 20);
            this.txtRepeatEmail.TabIndex = 17;
            this.txtRepeatEmail.ValidBorderColor = System.Drawing.Color.Green;
            // 
            // txtEmail
            // 
            this.txtEmail.BackColor = System.Drawing.SystemColors.ControlDark;
            this.txtEmail.DefaultBorderColor = System.Drawing.SystemColors.ControlDark;
            this.txtEmail.ErrorBorderColor = System.Drawing.Color.Red;
            this.txtEmail.FocusedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(190)))), ((int)(((byte)(247)))));
            this.txtEmail.Location = new System.Drawing.Point(6, 159);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Padding = new System.Windows.Forms.Padding(1);
            this.txtEmail.PasswordChar = '\0';
            this.txtEmail.Size = new System.Drawing.Size(322, 20);
            this.txtEmail.TabIndex = 14;
            this.txtEmail.ValidBorderColor = System.Drawing.Color.Green;
            // 
            // txtRepeatPassword
            // 
            this.txtRepeatPassword.BackColor = System.Drawing.SystemColors.ControlDark;
            this.txtRepeatPassword.DefaultBorderColor = System.Drawing.SystemColors.ControlDark;
            this.txtRepeatPassword.ErrorBorderColor = System.Drawing.Color.Red;
            this.txtRepeatPassword.FocusedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(190)))), ((int)(((byte)(247)))));
            this.txtRepeatPassword.Location = new System.Drawing.Point(6, 120);
            this.txtRepeatPassword.Name = "txtRepeatPassword";
            this.txtRepeatPassword.Padding = new System.Windows.Forms.Padding(1);
            this.txtRepeatPassword.PasswordChar = '*';
            this.txtRepeatPassword.Size = new System.Drawing.Size(322, 20);
            this.txtRepeatPassword.TabIndex = 13;
            this.txtRepeatPassword.ValidBorderColor = System.Drawing.Color.Green;
            // 
            // txtRegisterPassword
            // 
            this.txtRegisterPassword.BackColor = System.Drawing.SystemColors.ControlDark;
            this.txtRegisterPassword.DefaultBorderColor = System.Drawing.SystemColors.ControlDark;
            this.txtRegisterPassword.ErrorBorderColor = System.Drawing.Color.Red;
            this.txtRegisterPassword.FocusedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(190)))), ((int)(((byte)(247)))));
            this.txtRegisterPassword.Location = new System.Drawing.Point(6, 81);
            this.txtRegisterPassword.Name = "txtRegisterPassword";
            this.txtRegisterPassword.Padding = new System.Windows.Forms.Padding(1);
            this.txtRegisterPassword.PasswordChar = '*';
            this.txtRegisterPassword.Size = new System.Drawing.Size(322, 20);
            this.txtRegisterPassword.TabIndex = 9;
            this.txtRegisterPassword.ValidBorderColor = System.Drawing.Color.Green;
            // 
            // txtRegisterUsername
            // 
            this.txtRegisterUsername.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(190)))), ((int)(((byte)(247)))));
            this.txtRegisterUsername.DefaultBorderColor = System.Drawing.SystemColors.ControlDark;
            this.txtRegisterUsername.ErrorBorderColor = System.Drawing.Color.Red;
            this.txtRegisterUsername.FocusedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(190)))), ((int)(((byte)(247)))));
            this.txtRegisterUsername.Location = new System.Drawing.Point(6, 42);
            this.txtRegisterUsername.Name = "txtRegisterUsername";
            this.txtRegisterUsername.Padding = new System.Windows.Forms.Padding(1);
            this.txtRegisterUsername.PasswordChar = '\0';
            this.txtRegisterUsername.Size = new System.Drawing.Size(322, 20);
            this.txtRegisterUsername.TabIndex = 8;
            this.txtRegisterUsername.ValidBorderColor = System.Drawing.Color.Green;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.languageToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(344, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // languageToolStripMenuItem
            // 
            this.languageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.espanolToolStripMenuItem,
            this.englishToolStripMenuItem});
            this.languageToolStripMenuItem.Name = "languageToolStripMenuItem";
            this.languageToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.languageToolStripMenuItem.Text = "Language";
            // 
            // espanolToolStripMenuItem
            // 
            this.espanolToolStripMenuItem.Name = "espanolToolStripMenuItem";
            this.espanolToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.espanolToolStripMenuItem.Tag = "ES";
            this.espanolToolStripMenuItem.Text = "Español";
            this.espanolToolStripMenuItem.Click += new System.EventHandler(this.españolToolStripMenuItem_Click);
            // 
            // englishToolStripMenuItem
            // 
            this.englishToolStripMenuItem.Name = "englishToolStripMenuItem";
            this.englishToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.englishToolStripMenuItem.Tag = "EN";
            this.englishToolStripMenuItem.Text = "English";
            this.englishToolStripMenuItem.Click += new System.EventHandler(this.englishToolStripMenuItem_Click);
            // 
            // frmCredentials
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 421);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCredentials";
            this.Text = "Lerp2Dev Auth";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmCredentials_FormClosing);
            this.Load += new System.EventHandler(this.LerpedCredentials_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabLogin.ResumeLayout(false);
            this.tabLogin.PerformLayout();
            this.lblLogin.ResumeLayout(false);
            this.tabRegister.ResumeLayout(false);
            this.tabRegister.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.lblRegister.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabLogin;
        private NotifyLabel lblLogin;
        private System.Windows.Forms.Button button1;
        private ExTextBox txtPassword;
        private ExTextBox txtUsername;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabRegister;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem languageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem espanolToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem englishToolStripMenuItem;
        private System.Windows.Forms.Button button2;
        private ExTextBox txtRepeatEmail;
        private System.Windows.Forms.Label label10;
        private ExTextBox txtEmail;
        private ExTextBox txtRepeatPassword;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private NotifyLabel lblRegister;
        private ExTextBox txtRegisterPassword;
        private ExTextBox txtRegisterUsername;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private ExTextBox txtSolution;
        private System.Windows.Forms.Label label9;
        private Captcha pictureBox1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox chkRemember;
    }
}