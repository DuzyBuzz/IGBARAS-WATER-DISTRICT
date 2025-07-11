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
        private Panel mainPanel;
        private Button dashboardButton;
        private Button billingButton;
        private Button paymentsButton;
        private Button toggleSidebarButton;
        private Button customersButton;
        private Button agingAccountsButton;
        private Button settingsButton;

        private System.Windows.Forms.Timer sidebarTimer;

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
            components = new System.ComponentModel.Container();
            sidebarPanel = new Panel();
            logoutButton = new Button();
            systemInformationButton = new Button();
            settingsButton = new Button();
            agingAccountsButton = new Button();
            customersButton = new Button();
            paymentsButton = new Button();
            billingButton = new Button();
            dashboardButton = new Button();
            toggleSidebarButton = new Button();
            mainPanel = new Panel();
            sidebarTimer = new System.Windows.Forms.Timer(components);
            sidebarPanel.SuspendLayout();
            SuspendLayout();
            // 
            // sidebarPanel
            // 
            sidebarPanel.BackColor = SystemColors.ActiveCaption;
            sidebarPanel.Controls.Add(logoutButton);
            sidebarPanel.Controls.Add(systemInformationButton);
            sidebarPanel.Controls.Add(settingsButton);
            sidebarPanel.Controls.Add(agingAccountsButton);
            sidebarPanel.Controls.Add(customersButton);
            sidebarPanel.Controls.Add(paymentsButton);
            sidebarPanel.Controls.Add(billingButton);
            sidebarPanel.Controls.Add(dashboardButton);
            sidebarPanel.Controls.Add(toggleSidebarButton);
            sidebarPanel.Dock = DockStyle.Left;
            sidebarPanel.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            sidebarPanel.Location = new Point(0, 0);
            sidebarPanel.Name = "sidebarPanel";
            sidebarPanel.Size = new Size(206, 710);
            sidebarPanel.TabIndex = 1;
            // 
            // logoutButton
            // 
            logoutButton.Dock = DockStyle.Bottom;
            logoutButton.FlatAppearance.BorderSize = 0;
            logoutButton.FlatStyle = FlatStyle.Flat;
            logoutButton.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold);
            logoutButton.ForeColor = Color.Red;
            logoutButton.Location = new Point(0, 665);
            logoutButton.Name = "logoutButton";
            logoutButton.Size = new Size(206, 45);
            logoutButton.TabIndex = 5;
            logoutButton.Text = "⍈ Logout";
            logoutButton.TextAlign = ContentAlignment.MiddleLeft;
            logoutButton.Click += logoutButton_Click;
            // 
            // systemInformationButton
            // 
            systemInformationButton.Dock = DockStyle.Top;
            systemInformationButton.FlatAppearance.BorderSize = 0;
            systemInformationButton.FlatStyle = FlatStyle.Flat;
            systemInformationButton.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold);
            systemInformationButton.Location = new Point(0, 310);
            systemInformationButton.Name = "systemInformationButton";
            systemInformationButton.Size = new Size(206, 45);
            systemInformationButton.TabIndex = 4;
            systemInformationButton.Text = "ⓘ System Information";
            systemInformationButton.TextAlign = ContentAlignment.MiddleLeft;
            systemInformationButton.Click += systemInformationButton_Click;
            // 
            // settingsButton
            // 
            settingsButton.Dock = DockStyle.Top;
            settingsButton.FlatAppearance.BorderSize = 0;
            settingsButton.FlatStyle = FlatStyle.Flat;
            settingsButton.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold);
            settingsButton.Location = new Point(0, 265);
            settingsButton.Name = "settingsButton";
            settingsButton.Size = new Size(206, 45);
            settingsButton.TabIndex = 0;
            settingsButton.Text = "⚙️ Settings";
            settingsButton.TextAlign = ContentAlignment.MiddleLeft;
            settingsButton.Click += settingsButton_Click;
            // 
            // agingAccountsButton
            // 
            agingAccountsButton.Dock = DockStyle.Top;
            agingAccountsButton.FlatAppearance.BorderSize = 0;
            agingAccountsButton.FlatStyle = FlatStyle.Flat;
            agingAccountsButton.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold);
            agingAccountsButton.Location = new Point(0, 220);
            agingAccountsButton.Name = "agingAccountsButton";
            agingAccountsButton.Size = new Size(206, 45);
            agingAccountsButton.TabIndex = 1;
            agingAccountsButton.Text = "📅 Aging";
            agingAccountsButton.TextAlign = ContentAlignment.MiddleLeft;
            agingAccountsButton.Click += agingAccountsButton_Click;
            // 
            // customersButton
            // 
            customersButton.Dock = DockStyle.Top;
            customersButton.FlatAppearance.BorderSize = 0;
            customersButton.FlatStyle = FlatStyle.Flat;
            customersButton.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold);
            customersButton.Location = new Point(0, 175);
            customersButton.Name = "customersButton";
            customersButton.Size = new Size(206, 45);
            customersButton.TabIndex = 2;
            customersButton.Text = "👥 Customers";
            customersButton.TextAlign = ContentAlignment.MiddleLeft;
            customersButton.Click += customersButton_Click;
            // 
            // paymentsButton
            // 
            paymentsButton.Dock = DockStyle.Top;
            paymentsButton.FlatAppearance.BorderSize = 0;
            paymentsButton.FlatStyle = FlatStyle.Flat;
            paymentsButton.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold);
            paymentsButton.Location = new Point(0, 130);
            paymentsButton.Name = "paymentsButton";
            paymentsButton.Size = new Size(206, 45);
            paymentsButton.TabIndex = 0;
            paymentsButton.Text = "💳 Payments";
            paymentsButton.TextAlign = ContentAlignment.MiddleLeft;
            paymentsButton.Click += paymentsButton_Click;
            // 
            // billingButton
            // 
            billingButton.Dock = DockStyle.Top;
            billingButton.FlatAppearance.BorderSize = 0;
            billingButton.FlatStyle = FlatStyle.Flat;
            billingButton.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold);
            billingButton.Location = new Point(0, 85);
            billingButton.Name = "billingButton";
            billingButton.Size = new Size(206, 45);
            billingButton.TabIndex = 1;
            billingButton.Text = "📤 Billing";
            billingButton.TextAlign = ContentAlignment.MiddleLeft;
            billingButton.Click += billingButton_Click;
            // 
            // dashboardButton
            // 
            dashboardButton.Dock = DockStyle.Top;
            dashboardButton.FlatAppearance.BorderSize = 0;
            dashboardButton.FlatStyle = FlatStyle.Flat;
            dashboardButton.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold);
            dashboardButton.Location = new Point(0, 40);
            dashboardButton.Name = "dashboardButton";
            dashboardButton.Size = new Size(206, 45);
            dashboardButton.TabIndex = 2;
            dashboardButton.Text = "🏠 Dashboard";
            dashboardButton.TextAlign = ContentAlignment.MiddleLeft;
            dashboardButton.Click += dashboardButton_Click;
            // 
            // toggleSidebarButton
            // 
            toggleSidebarButton.Dock = DockStyle.Top;
            toggleSidebarButton.FlatAppearance.BorderSize = 0;
            toggleSidebarButton.FlatStyle = FlatStyle.Flat;
            toggleSidebarButton.Font = new Font("Segoe UI Black", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            toggleSidebarButton.Location = new Point(0, 0);
            toggleSidebarButton.Name = "toggleSidebarButton";
            toggleSidebarButton.Size = new Size(206, 40);
            toggleSidebarButton.TabIndex = 3;
            toggleSidebarButton.Text = "☰";
            toggleSidebarButton.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // mainPanel
            // 
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(206, 0);
            mainPanel.Name = "mainPanel";
            mainPanel.Size = new Size(1226, 710);
            mainPanel.TabIndex = 0;
            // 
            // sidebarTimer
            // 
            sidebarTimer.Interval = 10;
            sidebarTimer.Tick += sidebarTimer_Tick;
            // 
            // MainForm
            // 
            ClientSize = new Size(1432, 710);
            Controls.Add(mainPanel);
            Controls.Add(sidebarPanel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "MainForm";
            Text = "IGBARAS Water District Billing";
            WindowState = FormWindowState.Maximized;
            Load += MainForm_Load;
            sidebarPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button logoutButton;
        private Button systemInformationButton;
    }
}
