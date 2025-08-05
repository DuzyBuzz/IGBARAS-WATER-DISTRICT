namespace IGBARAS_WATER_DISTRICT
{
    partial class BillSettingsControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            tableLayoutPanel10 = new TableLayoutPanel();
            settingsApplyButton = new Button();
            settingsUndoButton = new Button();
            label4 = new Label();
            tableLayoutPanel9 = new TableLayoutPanel();
            settingsDataGidView = new DataGridView();
            SettingID = new DataGridViewTextBoxColumn();
            serviceDataGridView = new DataGridView();
            ServiceiD = new DataGridViewTextBoxColumn();
            tableLayoutPanel3 = new TableLayoutPanel();
            serviceApplyButton = new Button();
            serviceUndoButton = new Button();
            label1 = new Label();
            tableLayoutPanel4 = new TableLayoutPanel();
            tableLayoutPanel5 = new TableLayoutPanel();
            tableLayoutPanel6 = new TableLayoutPanel();
            discountApplyButton = new Button();
            discountUndoButton = new Button();
            label2 = new Label();
            discountDataGridView = new DataGridView();
            DiscountID = new DataGridViewTextBoxColumn();
            tableLayoutPanel7 = new TableLayoutPanel();
            tableLayoutPanel8 = new TableLayoutPanel();
            zoneApplyButton = new Button();
            zoneUndoButton = new Button();
            label3 = new Label();
            zoneDataGridView = new DataGridView();
            ZoneID = new DataGridViewTextBoxColumn();
            panel1 = new Panel();
            label5 = new Label();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel10.SuspendLayout();
            tableLayoutPanel9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)settingsDataGidView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)serviceDataGridView).BeginInit();
            tableLayoutPanel3.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            tableLayoutPanel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)discountDataGridView).BeginInit();
            tableLayoutPanel7.SuspendLayout();
            tableLayoutPanel8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)zoneDataGridView).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoScroll = true;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 1);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel4, 0, 2);
            tableLayoutPanel1.Controls.Add(label5, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25.568182F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 74.4318161F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 412F));
            tableLayoutPanel1.Size = new Size(1374, 927);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(tableLayoutPanel10, 1, 0);
            tableLayoutPanel2.Controls.Add(tableLayoutPanel9, 1, 1);
            tableLayoutPanel2.Controls.Add(serviceDataGridView, 0, 1);
            tableLayoutPanel2.Controls.Add(tableLayoutPanel3, 0, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 134);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 11.8863049F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 88.11369F));
            tableLayoutPanel2.Size = new Size(1368, 377);
            tableLayoutPanel2.TabIndex = 1;
            // 
            // tableLayoutPanel10
            // 
            tableLayoutPanel10.ColumnCount = 3;
            tableLayoutPanel10.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel10.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 155F));
            tableLayoutPanel10.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 173F));
            tableLayoutPanel10.Controls.Add(settingsApplyButton, 2, 0);
            tableLayoutPanel10.Controls.Add(settingsUndoButton, 1, 0);
            tableLayoutPanel10.Controls.Add(label4, 0, 0);
            tableLayoutPanel10.Dock = DockStyle.Fill;
            tableLayoutPanel10.Location = new Point(687, 3);
            tableLayoutPanel10.Name = "tableLayoutPanel10";
            tableLayoutPanel10.RowCount = 1;
            tableLayoutPanel10.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel10.Size = new Size(678, 38);
            tableLayoutPanel10.TabIndex = 3;
            // 
            // settingsApplyButton
            // 
            settingsApplyButton.BackColor = Color.SteelBlue;
            settingsApplyButton.Dock = DockStyle.Fill;
            settingsApplyButton.ForeColor = Color.White;
            settingsApplyButton.Location = new Point(508, 3);
            settingsApplyButton.Name = "settingsApplyButton";
            settingsApplyButton.Size = new Size(167, 32);
            settingsApplyButton.TabIndex = 17;
            settingsApplyButton.Text = "✏️ Apply Changes";
            settingsApplyButton.UseVisualStyleBackColor = false;
            settingsApplyButton.Click += settingsApplyButton_Click;
            // 
            // settingsUndoButton
            // 
            settingsUndoButton.BackColor = Color.Brown;
            settingsUndoButton.Dock = DockStyle.Fill;
            settingsUndoButton.ForeColor = Color.White;
            settingsUndoButton.Location = new Point(353, 3);
            settingsUndoButton.Name = "settingsUndoButton";
            settingsUndoButton.Size = new Size(149, 32);
            settingsUndoButton.TabIndex = 16;
            settingsUndoButton.Text = "↩ Undo Changes";
            settingsUndoButton.UseVisualStyleBackColor = false;
            settingsUndoButton.Click += settingsUndoButton_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.Transparent;
            label4.Dock = DockStyle.Left;
            label4.Font = new Font("Arial", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.Location = new Point(3, 0);
            label4.Name = "label4";
            label4.Size = new Size(158, 38);
            label4.TabIndex = 0;
            label4.Text = "Tax and Penalty";
            // 
            // tableLayoutPanel9
            // 
            tableLayoutPanel9.ColumnCount = 1;
            tableLayoutPanel9.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel9.Controls.Add(settingsDataGidView, 0, 0);
            tableLayoutPanel9.Dock = DockStyle.Fill;
            tableLayoutPanel9.Location = new Point(687, 47);
            tableLayoutPanel9.Name = "tableLayoutPanel9";
            tableLayoutPanel9.RowCount = 1;
            tableLayoutPanel9.RowStyles.Add(new RowStyle(SizeType.Percent, 10.2244387F));
            tableLayoutPanel9.Size = new Size(678, 327);
            tableLayoutPanel9.TabIndex = 2;
            // 
            // settingsDataGidView
            // 
            settingsDataGidView.AllowUserToAddRows = false;
            settingsDataGidView.AllowUserToDeleteRows = false;
            settingsDataGidView.BackgroundColor = SystemColors.ButtonFace;
            settingsDataGidView.BorderStyle = BorderStyle.Fixed3D;
            settingsDataGidView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            settingsDataGidView.Columns.AddRange(new DataGridViewColumn[] { SettingID });
            settingsDataGidView.Dock = DockStyle.Fill;
            settingsDataGidView.Location = new Point(3, 3);
            settingsDataGidView.Name = "settingsDataGidView";
            settingsDataGidView.Size = new Size(672, 321);
            settingsDataGidView.TabIndex = 2;
            // 
            // SettingID
            // 
            SettingID.DataPropertyName = "SettingID";
            SettingID.HeaderText = "SettingID";
            SettingID.Name = "SettingID";
            SettingID.Visible = false;
            // 
            // serviceDataGridView
            // 
            serviceDataGridView.AllowUserToOrderColumns = true;
            serviceDataGridView.BackgroundColor = SystemColors.ButtonFace;
            serviceDataGridView.BorderStyle = BorderStyle.Fixed3D;
            serviceDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            serviceDataGridView.Columns.AddRange(new DataGridViewColumn[] { ServiceiD });
            serviceDataGridView.Dock = DockStyle.Fill;
            serviceDataGridView.Location = new Point(3, 47);
            serviceDataGridView.Name = "serviceDataGridView";
            serviceDataGridView.RowHeadersVisible = false;
            serviceDataGridView.Size = new Size(678, 327);
            serviceDataGridView.TabIndex = 0;
            // 
            // ServiceiD
            // 
            ServiceiD.DataPropertyName = "ServiceiD";
            ServiceiD.HeaderText = "ServiceiD";
            ServiceiD.Name = "ServiceiD";
            ServiceiD.Visible = false;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 3;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 203F));
            tableLayoutPanel3.Controls.Add(serviceApplyButton, 2, 0);
            tableLayoutPanel3.Controls.Add(serviceUndoButton, 1, 0);
            tableLayoutPanel3.Controls.Add(label1, 0, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(3, 3);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.Size = new Size(678, 38);
            tableLayoutPanel3.TabIndex = 1;
            // 
            // serviceApplyButton
            // 
            serviceApplyButton.BackColor = Color.SteelBlue;
            serviceApplyButton.Dock = DockStyle.Fill;
            serviceApplyButton.ForeColor = Color.White;
            serviceApplyButton.Location = new Point(478, 3);
            serviceApplyButton.Name = "serviceApplyButton";
            serviceApplyButton.Size = new Size(197, 32);
            serviceApplyButton.TabIndex = 17;
            serviceApplyButton.Text = "✏️ Apply Changes";
            serviceApplyButton.UseVisualStyleBackColor = false;
            serviceApplyButton.Click += applyServiceButton_Click;
            // 
            // serviceUndoButton
            // 
            serviceUndoButton.BackColor = Color.Brown;
            serviceUndoButton.Dock = DockStyle.Fill;
            serviceUndoButton.ForeColor = Color.White;
            serviceUndoButton.Location = new Point(288, 3);
            serviceUndoButton.Name = "serviceUndoButton";
            serviceUndoButton.Size = new Size(184, 32);
            serviceUndoButton.TabIndex = 16;
            serviceUndoButton.Text = "↩ Undo Changes";
            serviceUndoButton.UseVisualStyleBackColor = false;
            serviceUndoButton.Click += undoServiceButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Dock = DockStyle.Left;
            label1.Font = new Font("Arial", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(82, 38);
            label1.TabIndex = 0;
            label1.Text = "Service";
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 2;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.Controls.Add(tableLayoutPanel5, 0, 0);
            tableLayoutPanel4.Controls.Add(tableLayoutPanel7, 1, 0);
            tableLayoutPanel4.Location = new Point(3, 517);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 1;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.Size = new Size(1368, 407);
            tableLayoutPanel4.TabIndex = 2;
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.ColumnCount = 1;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel5.Controls.Add(tableLayoutPanel6, 0, 0);
            tableLayoutPanel5.Controls.Add(discountDataGridView, 0, 1);
            tableLayoutPanel5.Dock = DockStyle.Fill;
            tableLayoutPanel5.Location = new Point(3, 3);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 2;
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 10.2244387F));
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 89.77556F));
            tableLayoutPanel5.Size = new Size(678, 401);
            tableLayoutPanel5.TabIndex = 2;
            // 
            // tableLayoutPanel6
            // 
            tableLayoutPanel6.ColumnCount = 3;
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 155F));
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 173F));
            tableLayoutPanel6.Controls.Add(discountApplyButton, 2, 0);
            tableLayoutPanel6.Controls.Add(discountUndoButton, 1, 0);
            tableLayoutPanel6.Controls.Add(label2, 0, 0);
            tableLayoutPanel6.Dock = DockStyle.Fill;
            tableLayoutPanel6.Location = new Point(3, 3);
            tableLayoutPanel6.Name = "tableLayoutPanel6";
            tableLayoutPanel6.RowCount = 1;
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel6.Size = new Size(672, 35);
            tableLayoutPanel6.TabIndex = 2;
            // 
            // discountApplyButton
            // 
            discountApplyButton.BackColor = Color.SteelBlue;
            discountApplyButton.Dock = DockStyle.Fill;
            discountApplyButton.ForeColor = Color.White;
            discountApplyButton.Location = new Point(502, 3);
            discountApplyButton.Name = "discountApplyButton";
            discountApplyButton.Size = new Size(167, 29);
            discountApplyButton.TabIndex = 17;
            discountApplyButton.Text = "✏️ Apply Changes";
            discountApplyButton.UseVisualStyleBackColor = false;
            discountApplyButton.Click += discountApplyButton_Click;
            // 
            // discountUndoButton
            // 
            discountUndoButton.BackColor = Color.Brown;
            discountUndoButton.Dock = DockStyle.Fill;
            discountUndoButton.ForeColor = Color.White;
            discountUndoButton.Location = new Point(347, 3);
            discountUndoButton.Name = "discountUndoButton";
            discountUndoButton.Size = new Size(149, 29);
            discountUndoButton.TabIndex = 16;
            discountUndoButton.Text = "↩ Undo Changes";
            discountUndoButton.UseVisualStyleBackColor = false;
            discountUndoButton.Click += discountUndoButton_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Dock = DockStyle.Left;
            label2.Font = new Font("Arial", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(3, 0);
            label2.Name = "label2";
            label2.Size = new Size(92, 35);
            label2.TabIndex = 0;
            label2.Text = "Discount";
            // 
            // discountDataGridView
            // 
            discountDataGridView.AllowUserToOrderColumns = true;
            discountDataGridView.BackgroundColor = SystemColors.ButtonFace;
            discountDataGridView.BorderStyle = BorderStyle.Fixed3D;
            discountDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            discountDataGridView.Columns.AddRange(new DataGridViewColumn[] { DiscountID });
            discountDataGridView.Dock = DockStyle.Fill;
            discountDataGridView.Location = new Point(3, 44);
            discountDataGridView.Name = "discountDataGridView";
            discountDataGridView.RowHeadersVisible = false;
            discountDataGridView.Size = new Size(672, 354);
            discountDataGridView.TabIndex = 1;
            // 
            // DiscountID
            // 
            DiscountID.DataPropertyName = "DiscountID";
            DiscountID.HeaderText = "DiscountID";
            DiscountID.Name = "DiscountID";
            DiscountID.Visible = false;
            // 
            // tableLayoutPanel7
            // 
            tableLayoutPanel7.ColumnCount = 1;
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel7.Controls.Add(tableLayoutPanel8, 0, 0);
            tableLayoutPanel7.Controls.Add(zoneDataGridView, 0, 1);
            tableLayoutPanel7.Dock = DockStyle.Fill;
            tableLayoutPanel7.Location = new Point(687, 3);
            tableLayoutPanel7.Name = "tableLayoutPanel7";
            tableLayoutPanel7.RowCount = 2;
            tableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 10.2244387F));
            tableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 89.77556F));
            tableLayoutPanel7.Size = new Size(678, 401);
            tableLayoutPanel7.TabIndex = 1;
            // 
            // tableLayoutPanel8
            // 
            tableLayoutPanel8.ColumnCount = 3;
            tableLayoutPanel8.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel8.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 155F));
            tableLayoutPanel8.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 173F));
            tableLayoutPanel8.Controls.Add(zoneApplyButton, 2, 0);
            tableLayoutPanel8.Controls.Add(zoneUndoButton, 1, 0);
            tableLayoutPanel8.Controls.Add(label3, 0, 0);
            tableLayoutPanel8.Dock = DockStyle.Fill;
            tableLayoutPanel8.Location = new Point(3, 3);
            tableLayoutPanel8.Name = "tableLayoutPanel8";
            tableLayoutPanel8.RowCount = 1;
            tableLayoutPanel8.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel8.Size = new Size(672, 35);
            tableLayoutPanel8.TabIndex = 2;
            // 
            // zoneApplyButton
            // 
            zoneApplyButton.BackColor = Color.SteelBlue;
            zoneApplyButton.Dock = DockStyle.Fill;
            zoneApplyButton.ForeColor = Color.White;
            zoneApplyButton.Location = new Point(502, 3);
            zoneApplyButton.Name = "zoneApplyButton";
            zoneApplyButton.Size = new Size(167, 29);
            zoneApplyButton.TabIndex = 17;
            zoneApplyButton.Text = "✏️ Apply Changes";
            zoneApplyButton.UseVisualStyleBackColor = false;
            zoneApplyButton.Click += zoneApplyButton_Click;
            // 
            // zoneUndoButton
            // 
            zoneUndoButton.BackColor = Color.Brown;
            zoneUndoButton.Dock = DockStyle.Fill;
            zoneUndoButton.ForeColor = Color.White;
            zoneUndoButton.Location = new Point(347, 3);
            zoneUndoButton.Name = "zoneUndoButton";
            zoneUndoButton.Size = new Size(149, 29);
            zoneUndoButton.TabIndex = 16;
            zoneUndoButton.Text = "↩ Undo Changes";
            zoneUndoButton.UseVisualStyleBackColor = false;
            zoneUndoButton.Click += zoneUndoButton_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.Dock = DockStyle.Left;
            label3.Font = new Font("Arial", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(3, 0);
            label3.Name = "label3";
            label3.Size = new Size(58, 35);
            label3.TabIndex = 0;
            label3.Text = "Zone";
            // 
            // zoneDataGridView
            // 
            zoneDataGridView.AllowUserToOrderColumns = true;
            zoneDataGridView.BackgroundColor = SystemColors.ButtonFace;
            zoneDataGridView.BorderStyle = BorderStyle.Fixed3D;
            zoneDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            zoneDataGridView.Columns.AddRange(new DataGridViewColumn[] { ZoneID });
            zoneDataGridView.Dock = DockStyle.Fill;
            zoneDataGridView.Location = new Point(3, 44);
            zoneDataGridView.Name = "zoneDataGridView";
            zoneDataGridView.RowHeadersVisible = false;
            zoneDataGridView.Size = new Size(672, 354);
            zoneDataGridView.TabIndex = 1;
            // 
            // ZoneID
            // 
            ZoneID.DataPropertyName = "ZoneID";
            ZoneID.HeaderText = "ZoneID";
            ZoneID.Name = "ZoneID";
            ZoneID.Visible = false;
            // 
            // panel1
            // 
            panel1.BorderStyle = BorderStyle.Fixed3D;
            panel1.Controls.Add(tableLayoutPanel1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1378, 931);
            panel1.TabIndex = 1;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Dock = DockStyle.Fill;
            label5.Font = new Font("Arial Narrow", 48F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.Location = new Point(3, 0);
            label5.Name = "label5";
            label5.Size = new Size(1368, 131);
            label5.TabIndex = 3;
            label5.Text = "BILL SETTINGS";
            label5.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // BillSettingsControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(panel1);
            Name = "BillSettingsControl";
            Size = new Size(1378, 931);
            Load += BillSettingsControl_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel10.ResumeLayout(false);
            tableLayoutPanel10.PerformLayout();
            tableLayoutPanel9.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)settingsDataGidView).EndInit();
            ((System.ComponentModel.ISupportInitialize)serviceDataGridView).EndInit();
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel5.ResumeLayout(false);
            tableLayoutPanel6.ResumeLayout(false);
            tableLayoutPanel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)discountDataGridView).EndInit();
            tableLayoutPanel7.ResumeLayout(false);
            tableLayoutPanel8.ResumeLayout(false);
            tableLayoutPanel8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)zoneDataGridView).EndInit();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private DataGridView serviceDataGridView;
        private TableLayoutPanel tableLayoutPanel3;
        private Label label1;
        private Button serviceApplyButton;
        private Button serviceUndoButton;
        private DataGridViewTextBoxColumn ServiceiD;
        private Panel panel1;
        private TableLayoutPanel tableLayoutPanel4;
        private TableLayoutPanel tableLayoutPanel10;
        private Button settingsApplyButton;
        private Button settingsUndoButton;
        private Label label4;
        private TableLayoutPanel tableLayoutPanel9;
        private DataGridView settingsDataGidView;
        private TableLayoutPanel tableLayoutPanel5;
        private TableLayoutPanel tableLayoutPanel6;
        private Button discountApplyButton;
        private Button discountUndoButton;
        private Label label2;
        private DataGridView discountDataGridView;
        private TableLayoutPanel tableLayoutPanel7;
        private TableLayoutPanel tableLayoutPanel8;
        private Button zoneApplyButton;
        private Button zoneUndoButton;
        private Label label3;
        private DataGridView zoneDataGridView;
        private DataGridViewTextBoxColumn SettingID;
        private DataGridViewTextBoxColumn DiscountID;
        private DataGridViewTextBoxColumn ZoneID;
        private Label label5;
    }
}
