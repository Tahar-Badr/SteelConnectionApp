namespace Front_end
{
    partial class hm
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
            panelMenu = new Panel();
            button3 = new Button();
            button2 = new Button();
            button1 = new Button();
            login = new Panel();
            label2 = new Label();
            lblTitle = new Panel();
            label1 = new Label();
            panelDesktopPane = new Panel();
            panelMenu.SuspendLayout();
            login.SuspendLayout();
            lblTitle.SuspendLayout();
            SuspendLayout();
            // 
            // panelMenu
            // 
            panelMenu.BackColor = Color.FromArgb(24, 30, 54);
            panelMenu.Controls.Add(button3);
            panelMenu.Controls.Add(button2);
            panelMenu.Controls.Add(button1);
            panelMenu.Controls.Add(login);
            panelMenu.Dock = DockStyle.Left;
            panelMenu.Location = new Point(0, 0);
            panelMenu.Name = "panelMenu";
            panelMenu.Size = new Size(220, 577);
            panelMenu.TabIndex = 0;
            panelMenu.Paint += panel1_Paint;
            // 
            // button3
            // 
            button3.Dock = DockStyle.Bottom;
            button3.FlatAppearance.BorderSize = 0;
            button3.FlatStyle = FlatStyle.Flat;
            button3.Font = new Font("Segoe UI", 10F);
            button3.ForeColor = Color.Gainsboro;
            button3.Image = Properties.Resources.setting_white_1;
            button3.ImageAlign = ContentAlignment.MiddleLeft;
            button3.Location = new Point(0, 517);
            button3.Name = "button3";
            button3.Size = new Size(220, 60);
            button3.TabIndex = 3;
            button3.Text = "setting";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button2
            // 
            button2.Dock = DockStyle.Top;
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Segoe UI", 10F);
            button2.ForeColor = Color.Gainsboro;
            button2.Image = Properties.Resources.home_white_;
            button2.ImageAlign = ContentAlignment.MiddleLeft;
            button2.Location = new Point(0, 140);
            button2.Name = "button2";
            button2.Size = new Size(220, 60);
            button2.TabIndex = 2;
            button2.Text = "history";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button1
            // 
            button1.Dock = DockStyle.Top;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Segoe UI", 10F);
            button1.ForeColor = Color.Gainsboro;
            button1.Image = Properties.Resources.home_white_;
            button1.ImageAlign = ContentAlignment.MiddleLeft;
            button1.Location = new Point(0, 80);
            button1.Name = "button1";
            button1.Size = new Size(220, 60);
            button1.TabIndex = 1;
            button1.Text = "home";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // login
            // 
            login.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            login.Controls.Add(label2);
            login.Dock = DockStyle.Top;
            login.Location = new Point(0, 0);
            login.Name = "login";
            login.Size = new Size(220, 80);
            login.TabIndex = 0;
            login.Paint += login_Paint;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.ForeColor = Color.LightGray;
            label2.Location = new Point(75, 34);
            label2.Name = "label2";
            label2.Size = new Size(99, 31);
            label2.TabIndex = 0;
            label2.Text = "pr name";
            label2.Click += label2_Click;
            // 
            // lblTitle
            // 
            lblTitle.BackColor = Color.FromArgb(30, 30, 45);
            lblTitle.Controls.Add(label1);
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Location = new Point(220, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(731, 80);
            lblTitle.TabIndex = 1;
            lblTitle.Paint += lblTitle_Paint;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.None;
            label1.AutoSize = true;
            label1.Font = new Font("Montserrat", 16F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(313, 19);
            label1.Name = "label1";
            label1.Size = new Size(118, 47);
            label1.TabIndex = 0;
            label1.Text = "HOME";
            label1.Click += label1_Click;
            // 
            // panelDesktopPane
            // 
            panelDesktopPane.Dock = DockStyle.Fill;
            panelDesktopPane.Location = new Point(220, 80);
            panelDesktopPane.Name = "panelDesktopPane";
            panelDesktopPane.Size = new Size(731, 497);
            panelDesktopPane.TabIndex = 2;
            panelDesktopPane.Paint += paneldesktoppane_Paint;
            // 
            // hm
            // 
            AutoScaleDimensions = new SizeF(9F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(46, 51, 73);
            ClientSize = new Size(951, 577);
            Controls.Add(panelDesktopPane);
            Controls.Add(lblTitle);
            Controls.Add(panelMenu);
            Name = "hm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            Load += Form1_Load;
            panelMenu.ResumeLayout(false);
            login.ResumeLayout(false);
            login.PerformLayout();
            lblTitle.ResumeLayout(false);
            lblTitle.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelMenu;
        private Panel login;
        private Button button1;
        private Button button3;
        private Button button2;
        private Panel lblTitle;
        private Label label1;
        private Label label2;
        private Panel panelDesktopPane;
    }
}
