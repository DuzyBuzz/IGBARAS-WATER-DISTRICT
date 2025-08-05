namespace IGBARAS_WATER_DISTRICT
{
    partial class AccountsControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        public System.ComponentModel.IContainer components = null;

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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            miniToolStrip = new ToolStrip();
            tableLayoutPanel1 = new TableLayoutPanel();
            label1 = new Label();
            tableLayoutPanel2 = new TableLayoutPanel();
            tableLayoutPanel7 = new TableLayoutPanel();
            tableLayoutPanel8 = new TableLayoutPanel();
            accountApplyButton = new Button();
            accountUndoButton = new Button();
            tableLayoutPanel3 = new TableLayoutPanel();
            clearButton = new Button();
            searchAccountNumberTextBox = new TextBox();
            zoneComboBox = new ComboBox();
            accountsDataGridView = new DataGridView();
            ConcessionaireID = new DataGridViewTextBoxColumn();
            AccountNo = new DataGridViewTextBoxColumn();
            ConcessionaireName = new DataGridViewTextBoxColumn();
            Address = new DataGridViewTextBoxColumn();
            ZoneCode = new DataGridViewTextBoxColumn();
            ServiceID = new DataGridViewTextBoxColumn();
            MeterNo = new DataGridViewTextBoxColumn();
            FirstReadingDate = new DataGridViewTextBoxColumn();
            SeniorCitizen = new DataGridViewCheckBoxColumn();
            TaxExempt = new DataGridViewCheckBoxColumn();
            DueExempt = new DataGridViewCheckBoxColumn();
            Status = new DataGridViewTextBoxColumn();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel7.SuspendLayout();
            tableLayoutPanel8.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)accountsDataGridView).BeginInit();
            SuspendLayout();
            // 
            // miniToolStrip
            // 
            miniToolStrip.AccessibleName = "New item selection";
            miniToolStrip.AccessibleRole = AccessibleRole.ButtonDropDown;
            miniToolStrip.AutoSize = false;
            miniToolStrip.CanOverflow = false;
            miniToolStrip.Dock = DockStyle.None;
            miniToolStrip.GripStyle = ToolStripGripStyle.Hidden;
            miniToolStrip.Location = new Point(0, 0);
            miniToolStrip.Name = "miniToolStrip";
            miniToolStrip.Size = new Size(518, 20);
            miniToolStrip.TabIndex = 24;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100.000008F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 875F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new Size(1378, 931);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Arial", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(1372, 56);
            label1.TabIndex = 1;
            label1.Text = "IGBARAS WATER DISTRICT CONCESSIONAIRE";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(tableLayoutPanel7, 0, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 59);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 6.329114F));
            tableLayoutPanel2.Size = new Size(1372, 869);
            tableLayoutPanel2.TabIndex = 2;
            // 
            // tableLayoutPanel7
            // 
            tableLayoutPanel7.ColumnCount = 1;
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel7.Controls.Add(tableLayoutPanel8, 0, 0);
            tableLayoutPanel7.Controls.Add(accountsDataGridView, 0, 1);
            tableLayoutPanel7.Dock = DockStyle.Fill;
            tableLayoutPanel7.Location = new Point(3, 3);
            tableLayoutPanel7.Name = "tableLayoutPanel7";
            tableLayoutPanel7.RowCount = 2;
            tableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 5.561993F));
            tableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 94.438F));
            tableLayoutPanel7.Size = new Size(1366, 863);
            tableLayoutPanel7.TabIndex = 2;
            // 
            // tableLayoutPanel8
            // 
            tableLayoutPanel8.ColumnCount = 3;
            tableLayoutPanel8.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel8.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 155F));
            tableLayoutPanel8.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 173F));
            tableLayoutPanel8.Controls.Add(accountApplyButton, 2, 0);
            tableLayoutPanel8.Controls.Add(accountUndoButton, 1, 0);
            tableLayoutPanel8.Controls.Add(tableLayoutPanel3, 0, 0);
            tableLayoutPanel8.Dock = DockStyle.Fill;
            tableLayoutPanel8.Location = new Point(3, 3);
            tableLayoutPanel8.Name = "tableLayoutPanel8";
            tableLayoutPanel8.RowCount = 1;
            tableLayoutPanel8.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel8.Size = new Size(1360, 42);
            tableLayoutPanel8.TabIndex = 2;
            // 
            // accountApplyButton
            // 
            accountApplyButton.BackColor = Color.SteelBlue;
            accountApplyButton.Dock = DockStyle.Fill;
            accountApplyButton.ForeColor = Color.White;
            accountApplyButton.Location = new Point(1190, 3);
            accountApplyButton.Name = "accountApplyButton";
            accountApplyButton.Size = new Size(167, 36);
            accountApplyButton.TabIndex = 17;
            accountApplyButton.Text = "✏️ Apply Changes";
            accountApplyButton.UseVisualStyleBackColor = false;
            accountApplyButton.Click += accountApplyButton_Click;
            // 
            // accountUndoButton
            // 
            accountUndoButton.BackColor = Color.Brown;
            accountUndoButton.Dock = DockStyle.Fill;
            accountUndoButton.ForeColor = Color.White;
            accountUndoButton.Location = new Point(1035, 3);
            accountUndoButton.Name = "accountUndoButton";
            accountUndoButton.Size = new Size(149, 36);
            accountUndoButton.TabIndex = 16;
            accountUndoButton.Text = "↩ Undo Changes";
            accountUndoButton.UseVisualStyleBackColor = false;
            accountUndoButton.Click += accountUndoButton_Click;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 5;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 90.0862045F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 9.913794F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 124F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 81F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 376F));
            tableLayoutPanel3.Controls.Add(clearButton, 1, 0);
            tableLayoutPanel3.Controls.Add(searchAccountNumberTextBox, 0, 0);
            tableLayoutPanel3.Controls.Add(zoneComboBox, 2, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(3, 3);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.Size = new Size(1026, 36);
            tableLayoutPanel3.TabIndex = 18;
            // 
            // clearButton
            // 
            clearButton.BackColor = Color.White;
            clearButton.Dock = DockStyle.Fill;
            clearButton.FlatStyle = FlatStyle.Popup;
            clearButton.Font = new Font("Arial Narrow", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            clearButton.ForeColor = Color.Crimson;
            clearButton.Location = new Point(403, 3);
            clearButton.Name = "clearButton";
            clearButton.Size = new Size(38, 30);
            clearButton.TabIndex = 25;
            clearButton.Text = "❌";
            clearButton.UseVisualStyleBackColor = false;
            clearButton.Click += clearButton_Click;
            // 
            // searchAccountNumberTextBox
            // 
            searchAccountNumberTextBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            searchAccountNumberTextBox.Dock = DockStyle.Left;
            searchAccountNumberTextBox.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            searchAccountNumberTextBox.Location = new Point(3, 5);
            searchAccountNumberTextBox.Margin = new Padding(3, 5, 3, 3);
            searchAccountNumberTextBox.Name = "searchAccountNumberTextBox";
            searchAccountNumberTextBox.Size = new Size(394, 26);
            searchAccountNumberTextBox.TabIndex = 7;
            searchAccountNumberTextBox.KeyDown += searchAccountNumberTextBox_KeyDown;
            // 
            // zoneComboBox
            // 
            zoneComboBox.Dock = DockStyle.Fill;
            zoneComboBox.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            zoneComboBox.FormattingEnabled = true;
            zoneComboBox.Location = new Point(447, 3);
            zoneComboBox.Name = "zoneComboBox";
            zoneComboBox.Size = new Size(118, 26);
            zoneComboBox.TabIndex = 8;
            zoneComboBox.SelectedIndexChanged += zoneComboBox_SelectedIndexChanged;
            // 
            // accountsDataGridView
            // 
            accountsDataGridView.AllowUserToOrderColumns = true;
            accountsDataGridView.BackgroundColor = SystemColors.ButtonFace;
            accountsDataGridView.BorderStyle = BorderStyle.Fixed3D;
            accountsDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            accountsDataGridView.Columns.AddRange(new DataGridViewColumn[] { ConcessionaireID, AccountNo, ConcessionaireName, Address, ZoneCode, ServiceID, MeterNo, FirstReadingDate, SeniorCitizen, TaxExempt, DueExempt, Status });
            accountsDataGridView.Dock = DockStyle.Fill;
            accountsDataGridView.Location = new Point(3, 51);
            accountsDataGridView.Name = "accountsDataGridView";
            accountsDataGridView.RowHeadersVisible = false;
            accountsDataGridView.Size = new Size(1360, 809);
            accountsDataGridView.TabIndex = 1;
            accountsDataGridView.CellFormatting += accountsDataGridView_CellFormatting;
            // 
            // ConcessionaireID
            // 
            ConcessionaireID.DataPropertyName = "ConcessionaireID";
            ConcessionaireID.HeaderText = "ConcessionaireID";
            ConcessionaireID.Name = "ConcessionaireID";
            ConcessionaireID.Visible = false;
            // 
            // AccountNo
            // 
            AccountNo.DataPropertyName = "AccountNo";
            AccountNo.HeaderText = "AccountNo";
            AccountNo.Name = "AccountNo";
            AccountNo.Width = 123;
            // 
            // ConcessionaireName
            // 
            ConcessionaireName.DataPropertyName = "ConcessionaireName";
            ConcessionaireName.HeaderText = "ConcessionaireName";
            ConcessionaireName.Name = "ConcessionaireName";
            ConcessionaireName.Width = 124;
            // 
            // Address
            // 
            Address.DataPropertyName = "Address";
            Address.HeaderText = "Address";
            Address.Name = "Address";
            Address.Width = 123;
            // 
            // ZoneCode
            // 
            ZoneCode.DataPropertyName = "ZoneCode";
            ZoneCode.HeaderText = "ZoneCode";
            ZoneCode.Name = "ZoneCode";
            ZoneCode.Width = 123;
            // 
            // ServiceID
            // 
            ServiceID.DataPropertyName = "ServiceID";
            ServiceID.HeaderText = "ServiceID";
            ServiceID.Name = "ServiceID";
            ServiceID.Width = 124;
            // 
            // MeterNo
            // 
            MeterNo.DataPropertyName = "MeterNo";
            MeterNo.HeaderText = "MeterNo";
            MeterNo.Name = "MeterNo";
            MeterNo.Width = 123;
            // 
            // FirstReadingDate
            // 
            FirstReadingDate.DataPropertyName = "FirstReadingDate";
            FirstReadingDate.HeaderText = "FirstReadingDate";
            FirstReadingDate.Name = "FirstReadingDate";
            FirstReadingDate.Width = 124;
            // 
            // SeniorCitizen
            // 
            SeniorCitizen.DataPropertyName = "SeniorCitizen";
            SeniorCitizen.HeaderText = "SeniorCitizen";
            SeniorCitizen.Name = "SeniorCitizen";
            SeniorCitizen.Resizable = DataGridViewTriState.True;
            SeniorCitizen.SortMode = DataGridViewColumnSortMode.Automatic;
            SeniorCitizen.Width = 123;
            // 
            // TaxExempt
            // 
            TaxExempt.DataPropertyName = "TaxExempt";
            TaxExempt.HeaderText = "TaxExempt";
            TaxExempt.Name = "TaxExempt";
            TaxExempt.Resizable = DataGridViewTriState.True;
            TaxExempt.SortMode = DataGridViewColumnSortMode.Automatic;
            TaxExempt.Width = 123;
            // 
            // DueExempt
            // 
            DueExempt.DataPropertyName = "DueExempt";
            DueExempt.HeaderText = "DueExempt";
            DueExempt.Name = "DueExempt";
            DueExempt.Resizable = DataGridViewTriState.True;
            DueExempt.SortMode = DataGridViewColumnSortMode.Automatic;
            DueExempt.Width = 124;
            // 
            // Status
            // 
            Status.DataPropertyName = "Status";
            dataGridViewCellStyle1.BackColor = Color.Transparent;
            Status.DefaultCellStyle = dataGridViewCellStyle1;
            Status.HeaderText = "Status";
            Status.Name = "Status";
            Status.Resizable = DataGridViewTriState.True;
            Status.Width = 123;
            // 
            // AccountsControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Name = "AccountsControl";
            Size = new Size(1378, 931);
            Load += AccountsControl_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel7.ResumeLayout(false);
            tableLayoutPanel8.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)accountsDataGridView).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private ToolStrip miniToolStrip;
        private TableLayoutPanel tableLayoutPanel1;
        private Label label1;
        private TableLayoutPanel tableLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel7;
        private TableLayoutPanel tableLayoutPanel8;
        private Button accountApplyButton;
        private Button accountUndoButton;
        private DataGridView accountsDataGridView;
        private TableLayoutPanel tableLayoutPanel3;
        private TextBox searchAccountNumberTextBox;
        private ComboBox zoneComboBox;
        private Button clearButton;
        private DataGridViewTextBoxColumn ConcessionaireID;
        private DataGridViewTextBoxColumn AccountNo;
        private DataGridViewTextBoxColumn ConcessionaireName;
        private DataGridViewTextBoxColumn Address;
        private DataGridViewTextBoxColumn ZoneCode;
        private DataGridViewTextBoxColumn ServiceID;
        private DataGridViewTextBoxColumn MeterNo;
        private DataGridViewTextBoxColumn FirstReadingDate;
        private DataGridViewCheckBoxColumn SeniorCitizen;
        private DataGridViewCheckBoxColumn TaxExempt;
        private DataGridViewCheckBoxColumn DueExempt;
        private DataGridViewTextBoxColumn Status;
    }
}
