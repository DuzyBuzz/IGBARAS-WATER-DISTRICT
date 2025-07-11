// Project: IGBARAS Water District Billing System
// Type: WinForms (.NET Framework 4.8)
// File: MainForm.Designer.cs

namespace IGBARAS_WATER_DISTRICT
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private Panel sidebarPanel;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            sidebarPanel = new Panel();
            button2 = new Button();
            button1 = new Button();
            transactionsButton = new Button();
            accountsButton = new Button();
            billingButton = new Button();
            dashboardButton = new Button();
            panel1 = new Panel();
            panel7 = new Panel();
            pictureBox1 = new PictureBox();
            panel6 = new Panel();
            usernameLabel = new Label();
            panel5 = new Panel();
            panel4 = new Panel();
            panel3 = new Panel();
            panel2 = new Panel();
            logoutButton = new Button();
            mainPanel = new Panel();
            sidebarPanel.SuspendLayout();
            panel1.SuspendLayout();
            panel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel6.SuspendLayout();
            SuspendLayout();
            // 
            // sidebarPanel
            // 
            sidebarPanel.AutoScroll = true;
            sidebarPanel.BackColor = SystemColors.ActiveCaption;
            sidebarPanel.Controls.Add(button2);
            sidebarPanel.Controls.Add(button1);
            sidebarPanel.Controls.Add(transactionsButton);
            sidebarPanel.Controls.Add(accountsButton);
            sidebarPanel.Controls.Add(billingButton);
            sidebarPanel.Controls.Add(dashboardButton);
            sidebarPanel.Controls.Add(panel1);
            sidebarPanel.Controls.Add(logoutButton);
            sidebarPanel.Dock = DockStyle.Left;
            sidebarPanel.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            sidebarPanel.Location = new Point(0, 0);
            sidebarPanel.Name = "sidebarPanel";
            sidebarPanel.Size = new Size(206, 710);
            sidebarPanel.TabIndex = 1;
            // 
            // button2
            // 
            button2.Dock = DockStyle.Bottom;
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Arial", 12F);
            button2.ForeColor = Color.Black;
            button2.Location = new Point(0, 575);
            button2.Name = "button2";
            button2.Size = new Size(206, 45);
            button2.TabIndex = 12;
            button2.Text = "ℹ️ System Info";
            button2.TextAlign = ContentAlignment.MiddleLeft;
            button2.Click += systemInformationButton_Click;
            // 
            // button1
            // 
            button1.Dock = DockStyle.Bottom;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Arial", 12F);
            button1.ForeColor = Color.Black;
            button1.Location = new Point(0, 620);
            button1.Name = "button1";
            button1.Size = new Size(206, 45);
            button1.TabIndex = 11;
            button1.Text = "⚙️ Settings";
            button1.TextAlign = ContentAlignment.MiddleLeft;
            button1.Click += settingsButton_Click;
            // 
            // transactionsButton
            // 
            transactionsButton.Dock = DockStyle.Top;
            transactionsButton.FlatAppearance.BorderSize = 0;
            transactionsButton.FlatStyle = FlatStyle.Flat;
            transactionsButton.Font = new Font("Arial", 12F);
            transactionsButton.Location = new Point(0, 299);
            transactionsButton.Name = "transactionsButton";
            transactionsButton.Size = new Size(206, 45);
            transactionsButton.TabIndex = 9;
            transactionsButton.Text = "\U0001f91d Transactions";
            transactionsButton.TextAlign = ContentAlignment.MiddleLeft;
            transactionsButton.Click += transactionsButton_Click;
            // 
            // accountsButton
            // 
            accountsButton.Dock = DockStyle.Top;
            accountsButton.FlatAppearance.BorderSize = 0;
            accountsButton.FlatStyle = FlatStyle.Flat;
            accountsButton.Font = new Font("Arial", 12F);
            accountsButton.Location = new Point(0, 254);
            accountsButton.Name = "accountsButton";
            accountsButton.Size = new Size(206, 45);
            accountsButton.TabIndex = 7;
            accountsButton.Text = "👥 Accounts";
            accountsButton.TextAlign = ContentAlignment.MiddleLeft;
            accountsButton.Click += accountsButton_Click;
            // 
            // billingButton
            // 
            billingButton.BackgroundImageLayout = ImageLayout.Zoom;
            billingButton.Dock = DockStyle.Top;
            billingButton.FlatAppearance.BorderSize = 0;
            billingButton.FlatStyle = FlatStyle.Flat;
            billingButton.Font = new Font("Arial", 12F);
            billingButton.ImageAlign = ContentAlignment.MiddleLeft;
            billingButton.Location = new Point(0, 209);
            billingButton.Name = "billingButton";
            billingButton.Size = new Size(206, 45);
            billingButton.TabIndex = 8;
            billingButton.Text = "₱ Billing";
            billingButton.TextAlign = ContentAlignment.MiddleLeft;
            billingButton.Click += billingButton_Click;
            // 
            // dashboardButton
            // 
            dashboardButton.Dock = DockStyle.Top;
            dashboardButton.FlatAppearance.BorderSize = 0;
            dashboardButton.FlatStyle = FlatStyle.Flat;
            dashboardButton.Font = new Font("Arial", 12F);
            dashboardButton.Location = new Point(0, 164);
            dashboardButton.Name = "dashboardButton";
            dashboardButton.Size = new Size(206, 45);
            dashboardButton.TabIndex = 10;
            dashboardButton.Text = "📊 Dashboard";
            dashboardButton.TextAlign = ContentAlignment.MiddleLeft;
            dashboardButton.Click += dashboardButton_Click;
            // 
            // panel1
            // 
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(panel7);
            panel1.Controls.Add(panel6);
            panel1.Controls.Add(panel5);
            panel1.Controls.Add(panel4);
            panel1.Controls.Add(panel3);
            panel1.Controls.Add(panel2);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(206, 164);
            panel1.TabIndex = 6;
            // 
            // panel7
            // 
            panel7.Controls.Add(pictureBox1);
            panel7.Dock = DockStyle.Fill;
            panel7.Location = new Point(35, 15);
            panel7.Name = "panel7";
            panel7.Size = new Size(134, 94);
            panel7.TabIndex = 8;
            // 
            // pictureBox1
            // 
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Margin = new Padding(0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(134, 94);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // panel6
            // 
            panel6.Controls.Add(usernameLabel);
            panel6.Dock = DockStyle.Bottom;
            panel6.Location = new Point(35, 109);
            panel6.Name = "panel6";
            panel6.Size = new Size(134, 33);
            panel6.TabIndex = 7;
            // 
            // usernameLabel
            // 
            usernameLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            usernameLabel.AutoSize = true;
            usernameLabel.BackColor = Color.Transparent;
            usernameLabel.Font = new Font("Arial Narrow", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            usernameLabel.Location = new Point(44, 3);
            usernameLabel.Name = "usernameLabel";
            usernameLabel.Size = new Size(52, 23);
            usernameLabel.TabIndex = 0;
            usernameLabel.Text = "label1";
            usernameLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel5
            // 
            panel5.Dock = DockStyle.Left;
            panel5.Location = new Point(0, 15);
            panel5.Name = "panel5";
            panel5.Size = new Size(35, 127);
            panel5.TabIndex = 5;
            // 
            // panel4
            // 
            panel4.Dock = DockStyle.Right;
            panel4.Location = new Point(169, 15);
            panel4.Name = "panel4";
            panel4.Size = new Size(35, 127);
            panel4.TabIndex = 4;
            // 
            // panel3
            // 
            panel3.Dock = DockStyle.Bottom;
            panel3.Location = new Point(0, 142);
            panel3.Name = "panel3";
            panel3.Size = new Size(204, 20);
            panel3.TabIndex = 3;
            // 
            // panel2
            // 
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(204, 15);
            panel2.TabIndex = 2;
            // 
            // logoutButton
            // 
            logoutButton.Dock = DockStyle.Bottom;
            logoutButton.FlatAppearance.BorderSize = 0;
            logoutButton.FlatStyle = FlatStyle.Flat;
            logoutButton.Font = new Font("Arial", 12F);
            logoutButton.ForeColor = Color.Red;
            logoutButton.Location = new Point(0, 665);
            logoutButton.Name = "logoutButton";
            logoutButton.Size = new Size(206, 45);
            logoutButton.TabIndex = 5;
            logoutButton.Text = "⍈ Logout";
            logoutButton.TextAlign = ContentAlignment.MiddleLeft;
            logoutButton.Click += logoutButton_Click;
            // 
            // mainPanel
            // 
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(206, 0);
            mainPanel.Name = "mainPanel";
            mainPanel.Size = new Size(1226, 710);
            mainPanel.TabIndex = 3;
            // 
            // MainForm
            // 
            AutoScroll = true;
            ClientSize = new Size(1432, 710);
            Controls.Add(mainPanel);
            Controls.Add(sidebarPanel);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "IGBARAS Water District Billing";
            WindowState = FormWindowState.Maximized;
            Load += MainForm_Load;
            sidebarPanel.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel6.ResumeLayout(false);
            panel6.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Button logoutButton;
        private Panel mainPanel;
        private Button transactionsButton;
        private Button accountsButton;
        private Button billingButton;
        private Button dashboardButton;
        private Panel panel1;
        private Panel panel5;
        private Panel panel4;
        private Panel panel3;
        private Panel panel2;
        private Button button2;
        private Button button1;
        private Panel panel7;
        private PictureBox pictureBox1;
        private Panel panel6;
        private Label usernameLabel;
    }
}
