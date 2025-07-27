namespace IGBARAS_WATER_DISTRICT
{
    partial class ReportsControl
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            tableLayoutPanel1 = new TableLayoutPanel();
            label1 = new Label();
            tableLayoutPanel10 = new TableLayoutPanel();
            tableLayoutPanel9 = new TableLayoutPanel();
            refreshReportsButton = new Button();
            exportAllToExcelButton = new Button();
            tableLayoutPanel11 = new TableLayoutPanel();
            dailyTab = new TabControl();
            tabPage1 = new TabPage();
            tableLayoutPanel12 = new TableLayoutPanel();
            dailyBillingDGV = new DataGridView();
            tableLayoutPanel13 = new TableLayoutPanel();
            dailyBillExportButton = new Button();
            label3 = new Label();
            dailyBillDateTimePicker = new DateTimePicker();
            dailyBillPrintButton = new Button();
            tabPage2 = new TabPage();
            tableLayoutPanel14 = new TableLayoutPanel();
            monthlyBillingDGV = new DataGridView();
            tableLayoutPanel15 = new TableLayoutPanel();
            monthlyBillExportButton = new Button();
            label4 = new Label();
            monthBillDateTimePicker = new DateTimePicker();
            monthlyBillPrintButton = new Button();
            mont = new TabControl();
            tabPage3 = new TabPage();
            tableLayoutPanel18 = new TableLayoutPanel();
            dailyCollectionDGV = new DataGridView();
            tableLayoutPanel19 = new TableLayoutPanel();
            dailyCollectionExportButton = new Button();
            label6 = new Label();
            dailyCollectionDateTimePicker = new DateTimePicker();
            dailyCollectionPrintButton = new Button();
            tabPage4 = new TabPage();
            tableLayoutPanel16 = new TableLayoutPanel();
            monthlyCollectionDGV = new DataGridView();
            tableLayoutPanel17 = new TableLayoutPanel();
            monthlyCollectionExportButton = new Button();
            label5 = new Label();
            monthlyCollectionDateTimePicker = new DateTimePicker();
            monthlyCollectionPrintButton = new Button();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel10.SuspendLayout();
            tableLayoutPanel9.SuspendLayout();
            tableLayoutPanel11.SuspendLayout();
            dailyTab.SuspendLayout();
            tabPage1.SuspendLayout();
            tableLayoutPanel12.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dailyBillingDGV).BeginInit();
            tableLayoutPanel13.SuspendLayout();
            tabPage2.SuspendLayout();
            tableLayoutPanel14.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)monthlyBillingDGV).BeginInit();
            tableLayoutPanel15.SuspendLayout();
            mont.SuspendLayout();
            tabPage3.SuspendLayout();
            tableLayoutPanel18.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dailyCollectionDGV).BeginInit();
            tableLayoutPanel19.SuspendLayout();
            tabPage4.SuspendLayout();
            tableLayoutPanel16.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)monthlyCollectionDGV).BeginInit();
            tableLayoutPanel17.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel10, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100.000008F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 875F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new Size(1378, 931);
            tableLayoutPanel1.TabIndex = 0;
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
            label1.Text = "IGBARAS WATER DISTRICT REPORTS";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel10
            // 
            tableLayoutPanel10.ColumnCount = 1;
            tableLayoutPanel10.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel10.Controls.Add(tableLayoutPanel9, 0, 0);
            tableLayoutPanel10.Controls.Add(tableLayoutPanel11, 0, 1);
            tableLayoutPanel10.Dock = DockStyle.Fill;
            tableLayoutPanel10.Location = new Point(3, 59);
            tableLayoutPanel10.Name = "tableLayoutPanel10";
            tableLayoutPanel10.RowCount = 3;
            tableLayoutPanel10.RowStyles.Add(new RowStyle(SizeType.Percent, 4.36320734F));
            tableLayoutPanel10.RowStyles.Add(new RowStyle(SizeType.Percent, 95.636795F));
            tableLayoutPanel10.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel10.Size = new Size(1372, 869);
            tableLayoutPanel10.TabIndex = 2;
            // 
            // tableLayoutPanel9
            // 
            tableLayoutPanel9.ColumnCount = 7;
            tableLayoutPanel9.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel9.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 717F));
            tableLayoutPanel9.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 26F));
            tableLayoutPanel9.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 262F));
            tableLayoutPanel9.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 207F));
            tableLayoutPanel9.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F));
            tableLayoutPanel9.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 84F));
            tableLayoutPanel9.Controls.Add(refreshReportsButton, 6, 0);
            tableLayoutPanel9.Controls.Add(exportAllToExcelButton, 4, 0);
            tableLayoutPanel9.Dock = DockStyle.Fill;
            tableLayoutPanel9.Location = new Point(3, 3);
            tableLayoutPanel9.Name = "tableLayoutPanel9";
            tableLayoutPanel9.RowCount = 1;
            tableLayoutPanel9.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel9.Size = new Size(1366, 31);
            tableLayoutPanel9.TabIndex = 2;
            // 
            // refreshReportsButton
            // 
            refreshReportsButton.BackColor = Color.CadetBlue;
            refreshReportsButton.ForeColor = Color.White;
            refreshReportsButton.Location = new Point(1285, 3);
            refreshReportsButton.Name = "refreshReportsButton";
            refreshReportsButton.Size = new Size(78, 25);
            refreshReportsButton.TabIndex = 12;
            refreshReportsButton.Text = "🔁 Refresh";
            refreshReportsButton.UseVisualStyleBackColor = false;
            refreshReportsButton.Click += refreshReportsButton_Click;
            // 
            // exportAllToExcelButton
            // 
            exportAllToExcelButton.BackColor = Color.Green;
            exportAllToExcelButton.Dock = DockStyle.Fill;
            exportAllToExcelButton.ForeColor = Color.White;
            exportAllToExcelButton.Location = new Point(1068, 3);
            exportAllToExcelButton.Name = "exportAllToExcelButton";
            exportAllToExcelButton.Size = new Size(201, 25);
            exportAllToExcelButton.TabIndex = 19;
            exportAllToExcelButton.Text = "📊 EXPORT ALL REPORT TO EXCEL";
            exportAllToExcelButton.UseVisualStyleBackColor = false;
            exportAllToExcelButton.Click += exportAllToExcelButton_Click;
            // 
            // tableLayoutPanel11
            // 
            tableLayoutPanel11.ColumnCount = 2;
            tableLayoutPanel11.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel11.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel11.Controls.Add(dailyTab, 0, 0);
            tableLayoutPanel11.Controls.Add(mont, 1, 0);
            tableLayoutPanel11.Dock = DockStyle.Fill;
            tableLayoutPanel11.Location = new Point(3, 40);
            tableLayoutPanel11.Name = "tableLayoutPanel11";
            tableLayoutPanel11.RowCount = 1;
            tableLayoutPanel11.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel11.Size = new Size(1366, 805);
            tableLayoutPanel11.TabIndex = 3;
            // 
            // dailyTab
            // 
            dailyTab.Controls.Add(tabPage1);
            dailyTab.Controls.Add(tabPage2);
            dailyTab.Dock = DockStyle.Fill;
            dailyTab.Location = new Point(3, 3);
            dailyTab.Name = "dailyTab";
            dailyTab.SelectedIndex = 0;
            dailyTab.Size = new Size(677, 799);
            dailyTab.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(tableLayoutPanel12);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(669, 771);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Billing Report (Daily)";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel12
            // 
            tableLayoutPanel12.ColumnCount = 1;
            tableLayoutPanel12.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel12.Controls.Add(dailyBillingDGV, 0, 1);
            tableLayoutPanel12.Controls.Add(tableLayoutPanel13, 0, 0);
            tableLayoutPanel12.Dock = DockStyle.Fill;
            tableLayoutPanel12.Location = new Point(3, 3);
            tableLayoutPanel12.Name = "tableLayoutPanel12";
            tableLayoutPanel12.RowCount = 2;
            tableLayoutPanel12.RowStyles.Add(new RowStyle(SizeType.Percent, 4.82573748F));
            tableLayoutPanel12.RowStyles.Add(new RowStyle(SizeType.Percent, 95.17426F));
            tableLayoutPanel12.Size = new Size(663, 765);
            tableLayoutPanel12.TabIndex = 1;
            // 
            // dailyBillingDGV
            // 
            dailyBillingDGV.AllowUserToAddRows = false;
            dailyBillingDGV.AllowUserToDeleteRows = false;
            dailyBillingDGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dailyBillingDGV.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dailyBillingDGV.BackgroundColor = SystemColors.ControlLightLight;
            dailyBillingDGV.BorderStyle = BorderStyle.Fixed3D;
            dailyBillingDGV.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Window;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle1.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dailyBillingDGV.DefaultCellStyle = dataGridViewCellStyle1;
            dailyBillingDGV.Dock = DockStyle.Fill;
            dailyBillingDGV.Location = new Point(3, 39);
            dailyBillingDGV.Name = "dailyBillingDGV";
            dailyBillingDGV.ReadOnly = true;
            dailyBillingDGV.RowHeadersVisible = false;
            dailyBillingDGV.Size = new Size(657, 723);
            dailyBillingDGV.TabIndex = 0;
            // 
            // tableLayoutPanel13
            // 
            tableLayoutPanel13.ColumnCount = 4;
            tableLayoutPanel13.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60.09615F));
            tableLayoutPanel13.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 39.9038467F));
            tableLayoutPanel13.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 137F));
            tableLayoutPanel13.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 135F));
            tableLayoutPanel13.Controls.Add(dailyBillExportButton, 3, 0);
            tableLayoutPanel13.Controls.Add(label3, 0, 0);
            tableLayoutPanel13.Controls.Add(dailyBillDateTimePicker, 1, 0);
            tableLayoutPanel13.Controls.Add(dailyBillPrintButton, 2, 0);
            tableLayoutPanel13.Dock = DockStyle.Fill;
            tableLayoutPanel13.Location = new Point(3, 3);
            tableLayoutPanel13.Name = "tableLayoutPanel13";
            tableLayoutPanel13.RowCount = 1;
            tableLayoutPanel13.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel13.Size = new Size(657, 30);
            tableLayoutPanel13.TabIndex = 1;
            // 
            // dailyBillExportButton
            // 
            dailyBillExportButton.BackColor = Color.Green;
            dailyBillExportButton.Dock = DockStyle.Fill;
            dailyBillExportButton.ForeColor = Color.White;
            dailyBillExportButton.Location = new Point(524, 3);
            dailyBillExportButton.Name = "dailyBillExportButton";
            dailyBillExportButton.Size = new Size(130, 24);
            dailyBillExportButton.TabIndex = 17;
            dailyBillExportButton.Text = "📊 EXPORT TO EXCEL";
            dailyBillExportButton.UseVisualStyleBackColor = false;
            dailyBillExportButton.Click += dailyBillExportButton_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = DockStyle.Fill;
            label3.Font = new Font("Arial", 18F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label3.Location = new Point(3, 0);
            label3.Name = "label3";
            label3.Size = new Size(225, 30);
            label3.TabIndex = 16;
            label3.Text = "Daily Billing";
            label3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // dailyBillDateTimePicker
            // 
            dailyBillDateTimePicker.Dock = DockStyle.Fill;
            dailyBillDateTimePicker.Location = new Point(234, 3);
            dailyBillDateTimePicker.Name = "dailyBillDateTimePicker";
            dailyBillDateTimePicker.Size = new Size(147, 23);
            dailyBillDateTimePicker.TabIndex = 1;
            dailyBillDateTimePicker.ValueChanged += dailyBillDateTimePicker_ValueChanged;
            // 
            // dailyBillPrintButton
            // 
            dailyBillPrintButton.BackColor = Color.SteelBlue;
            dailyBillPrintButton.Dock = DockStyle.Fill;
            dailyBillPrintButton.ForeColor = Color.White;
            dailyBillPrintButton.Location = new Point(387, 3);
            dailyBillPrintButton.Name = "dailyBillPrintButton";
            dailyBillPrintButton.Size = new Size(131, 24);
            dailyBillPrintButton.TabIndex = 15;
            dailyBillPrintButton.Text = "🖨️ PRINT";
            dailyBillPrintButton.UseVisualStyleBackColor = false;
            dailyBillPrintButton.Click += dailyBillPrintButton_Click;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(tableLayoutPanel14);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(669, 771);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Billing Report (Monthly)";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel14
            // 
            tableLayoutPanel14.ColumnCount = 1;
            tableLayoutPanel14.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel14.Controls.Add(monthlyBillingDGV, 0, 1);
            tableLayoutPanel14.Controls.Add(tableLayoutPanel15, 0, 0);
            tableLayoutPanel14.Dock = DockStyle.Fill;
            tableLayoutPanel14.Location = new Point(3, 3);
            tableLayoutPanel14.Name = "tableLayoutPanel14";
            tableLayoutPanel14.RowCount = 2;
            tableLayoutPanel14.RowStyles.Add(new RowStyle(SizeType.Percent, 4.82573748F));
            tableLayoutPanel14.RowStyles.Add(new RowStyle(SizeType.Percent, 95.17426F));
            tableLayoutPanel14.Size = new Size(663, 765);
            tableLayoutPanel14.TabIndex = 2;
            // 
            // monthlyBillingDGV
            // 
            monthlyBillingDGV.AllowUserToAddRows = false;
            monthlyBillingDGV.AllowUserToDeleteRows = false;
            monthlyBillingDGV.BackgroundColor = SystemColors.ControlLightLight;
            monthlyBillingDGV.BorderStyle = BorderStyle.Fixed3D;
            monthlyBillingDGV.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            monthlyBillingDGV.Dock = DockStyle.Fill;
            monthlyBillingDGV.Location = new Point(3, 39);
            monthlyBillingDGV.Name = "monthlyBillingDGV";
            monthlyBillingDGV.ReadOnly = true;
            monthlyBillingDGV.RowHeadersVisible = false;
            monthlyBillingDGV.Size = new Size(657, 723);
            monthlyBillingDGV.TabIndex = 0;
            // 
            // tableLayoutPanel15
            // 
            tableLayoutPanel15.ColumnCount = 4;
            tableLayoutPanel15.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60.1F));
            tableLayoutPanel15.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 39.9F));
            tableLayoutPanel15.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 137F));
            tableLayoutPanel15.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 135F));
            tableLayoutPanel15.Controls.Add(monthlyBillExportButton, 3, 0);
            tableLayoutPanel15.Controls.Add(label4, 0, 0);
            tableLayoutPanel15.Controls.Add(monthBillDateTimePicker, 1, 0);
            tableLayoutPanel15.Controls.Add(monthlyBillPrintButton, 2, 0);
            tableLayoutPanel15.Dock = DockStyle.Fill;
            tableLayoutPanel15.Location = new Point(3, 3);
            tableLayoutPanel15.Name = "tableLayoutPanel15";
            tableLayoutPanel15.RowCount = 1;
            tableLayoutPanel15.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel15.Size = new Size(657, 30);
            tableLayoutPanel15.TabIndex = 1;
            // 
            // monthlyBillExportButton
            // 
            monthlyBillExportButton.BackColor = Color.Green;
            monthlyBillExportButton.Dock = DockStyle.Fill;
            monthlyBillExportButton.ForeColor = Color.White;
            monthlyBillExportButton.Location = new Point(524, 3);
            monthlyBillExportButton.Name = "monthlyBillExportButton";
            monthlyBillExportButton.Size = new Size(130, 24);
            monthlyBillExportButton.TabIndex = 18;
            monthlyBillExportButton.Text = "📊 EXPORT TO EXCEL";
            monthlyBillExportButton.UseVisualStyleBackColor = false;
            monthlyBillExportButton.Click += monthlyBillExportButton_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Dock = DockStyle.Fill;
            label4.Font = new Font("Arial", 18F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label4.Location = new Point(3, 0);
            label4.Name = "label4";
            label4.Size = new Size(225, 30);
            label4.TabIndex = 16;
            label4.Text = "Monthly Billing Report";
            label4.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // monthBillDateTimePicker
            // 
            monthBillDateTimePicker.Dock = DockStyle.Fill;
            monthBillDateTimePicker.Location = new Point(234, 3);
            monthBillDateTimePicker.Name = "monthBillDateTimePicker";
            monthBillDateTimePicker.Size = new Size(147, 23);
            monthBillDateTimePicker.TabIndex = 1;
            monthBillDateTimePicker.ValueChanged += monthBillDateTimePicker_ValueChanged;
            // 
            // monthlyBillPrintButton
            // 
            monthlyBillPrintButton.BackColor = Color.SteelBlue;
            monthlyBillPrintButton.Dock = DockStyle.Fill;
            monthlyBillPrintButton.ForeColor = Color.White;
            monthlyBillPrintButton.Location = new Point(387, 3);
            monthlyBillPrintButton.Name = "monthlyBillPrintButton";
            monthlyBillPrintButton.Size = new Size(131, 24);
            monthlyBillPrintButton.TabIndex = 15;
            monthlyBillPrintButton.Text = "🖨️ PRINT";
            monthlyBillPrintButton.UseVisualStyleBackColor = false;
            monthlyBillPrintButton.Click += monthlyBillPrintButton_Click_1;
            // 
            // mont
            // 
            mont.Controls.Add(tabPage3);
            mont.Controls.Add(tabPage4);
            mont.Dock = DockStyle.Fill;
            mont.Location = new Point(686, 3);
            mont.Name = "mont";
            mont.SelectedIndex = 0;
            mont.Size = new Size(677, 799);
            mont.TabIndex = 1;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(tableLayoutPanel18);
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(669, 771);
            tabPage3.TabIndex = 0;
            tabPage3.Text = "Collection Summary (Daily)";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel18
            // 
            tableLayoutPanel18.ColumnCount = 1;
            tableLayoutPanel18.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel18.Controls.Add(dailyCollectionDGV, 0, 1);
            tableLayoutPanel18.Controls.Add(tableLayoutPanel19, 0, 0);
            tableLayoutPanel18.Dock = DockStyle.Fill;
            tableLayoutPanel18.Location = new Point(3, 3);
            tableLayoutPanel18.Name = "tableLayoutPanel18";
            tableLayoutPanel18.RowCount = 2;
            tableLayoutPanel18.RowStyles.Add(new RowStyle(SizeType.Percent, 4.82573748F));
            tableLayoutPanel18.RowStyles.Add(new RowStyle(SizeType.Percent, 95.17426F));
            tableLayoutPanel18.Size = new Size(663, 765);
            tableLayoutPanel18.TabIndex = 2;
            // 
            // dailyCollectionDGV
            // 
            dailyCollectionDGV.AllowUserToAddRows = false;
            dailyCollectionDGV.AllowUserToDeleteRows = false;
            dailyCollectionDGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dailyCollectionDGV.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dailyCollectionDGV.BackgroundColor = SystemColors.ControlLightLight;
            dailyCollectionDGV.BorderStyle = BorderStyle.Fixed3D;
            dailyCollectionDGV.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dailyCollectionDGV.Dock = DockStyle.Fill;
            dailyCollectionDGV.Location = new Point(3, 39);
            dailyCollectionDGV.Name = "dailyCollectionDGV";
            dailyCollectionDGV.ReadOnly = true;
            dailyCollectionDGV.RowHeadersVisible = false;
            dailyCollectionDGV.Size = new Size(657, 723);
            dailyCollectionDGV.TabIndex = 0;
            // 
            // tableLayoutPanel19
            // 
            tableLayoutPanel19.ColumnCount = 4;
            tableLayoutPanel19.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60.1F));
            tableLayoutPanel19.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 39.9F));
            tableLayoutPanel19.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 137F));
            tableLayoutPanel19.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 135F));
            tableLayoutPanel19.Controls.Add(dailyCollectionExportButton, 3, 0);
            tableLayoutPanel19.Controls.Add(label6, 0, 0);
            tableLayoutPanel19.Controls.Add(dailyCollectionDateTimePicker, 1, 0);
            tableLayoutPanel19.Controls.Add(dailyCollectionPrintButton, 2, 0);
            tableLayoutPanel19.Dock = DockStyle.Fill;
            tableLayoutPanel19.Location = new Point(3, 3);
            tableLayoutPanel19.Name = "tableLayoutPanel19";
            tableLayoutPanel19.RowCount = 1;
            tableLayoutPanel19.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel19.Size = new Size(657, 30);
            tableLayoutPanel19.TabIndex = 1;
            // 
            // dailyCollectionExportButton
            // 
            dailyCollectionExportButton.BackColor = Color.Green;
            dailyCollectionExportButton.Dock = DockStyle.Fill;
            dailyCollectionExportButton.ForeColor = Color.White;
            dailyCollectionExportButton.Location = new Point(524, 3);
            dailyCollectionExportButton.Name = "dailyCollectionExportButton";
            dailyCollectionExportButton.Size = new Size(130, 24);
            dailyCollectionExportButton.TabIndex = 18;
            dailyCollectionExportButton.Text = "📊 EXPORT TO EXCEL";
            dailyCollectionExportButton.UseVisualStyleBackColor = false;
            dailyCollectionExportButton.Click += dailyCollectionExportButton_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Dock = DockStyle.Fill;
            label6.Font = new Font("Arial", 18F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label6.Location = new Point(3, 0);
            label6.Name = "label6";
            label6.Size = new Size(225, 30);
            label6.TabIndex = 16;
            label6.Text = "Daily Collection Report";
            label6.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // dailyCollectionDateTimePicker
            // 
            dailyCollectionDateTimePicker.Dock = DockStyle.Fill;
            dailyCollectionDateTimePicker.Location = new Point(234, 3);
            dailyCollectionDateTimePicker.Name = "dailyCollectionDateTimePicker";
            dailyCollectionDateTimePicker.Size = new Size(147, 23);
            dailyCollectionDateTimePicker.TabIndex = 1;
            dailyCollectionDateTimePicker.ValueChanged += dailyCollectionDateTimePicker_ValueChanged;
            // 
            // dailyCollectionPrintButton
            // 
            dailyCollectionPrintButton.BackColor = Color.SteelBlue;
            dailyCollectionPrintButton.Dock = DockStyle.Fill;
            dailyCollectionPrintButton.ForeColor = Color.White;
            dailyCollectionPrintButton.Location = new Point(387, 3);
            dailyCollectionPrintButton.Name = "dailyCollectionPrintButton";
            dailyCollectionPrintButton.Size = new Size(131, 24);
            dailyCollectionPrintButton.TabIndex = 15;
            dailyCollectionPrintButton.Text = "🖨️ PRINT";
            dailyCollectionPrintButton.UseVisualStyleBackColor = false;
            dailyCollectionPrintButton.Click += dailyCollectionPrintButton_Click_1;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(tableLayoutPanel16);
            tabPage4.Location = new Point(4, 24);
            tabPage4.Name = "tabPage4";
            tabPage4.Padding = new Padding(3);
            tabPage4.Size = new Size(669, 771);
            tabPage4.TabIndex = 1;
            tabPage4.Text = "Collection Summary (Monthly)";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel16
            // 
            tableLayoutPanel16.ColumnCount = 1;
            tableLayoutPanel16.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel16.Controls.Add(monthlyCollectionDGV, 0, 1);
            tableLayoutPanel16.Controls.Add(tableLayoutPanel17, 0, 0);
            tableLayoutPanel16.Dock = DockStyle.Fill;
            tableLayoutPanel16.Location = new Point(3, 3);
            tableLayoutPanel16.Name = "tableLayoutPanel16";
            tableLayoutPanel16.RowCount = 2;
            tableLayoutPanel16.RowStyles.Add(new RowStyle(SizeType.Percent, 4.82573748F));
            tableLayoutPanel16.RowStyles.Add(new RowStyle(SizeType.Percent, 95.17426F));
            tableLayoutPanel16.Size = new Size(663, 765);
            tableLayoutPanel16.TabIndex = 2;
            // 
            // monthlyCollectionDGV
            // 
            monthlyCollectionDGV.AllowUserToAddRows = false;
            monthlyCollectionDGV.AllowUserToDeleteRows = false;
            monthlyCollectionDGV.BackgroundColor = SystemColors.ControlLightLight;
            monthlyCollectionDGV.BorderStyle = BorderStyle.Fixed3D;
            monthlyCollectionDGV.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            monthlyCollectionDGV.Dock = DockStyle.Fill;
            monthlyCollectionDGV.Location = new Point(3, 39);
            monthlyCollectionDGV.Name = "monthlyCollectionDGV";
            monthlyCollectionDGV.ReadOnly = true;
            monthlyCollectionDGV.RowHeadersVisible = false;
            monthlyCollectionDGV.Size = new Size(657, 723);
            monthlyCollectionDGV.TabIndex = 0;
            // 
            // tableLayoutPanel17
            // 
            tableLayoutPanel17.ColumnCount = 4;
            tableLayoutPanel17.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60.1F));
            tableLayoutPanel17.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 39.9F));
            tableLayoutPanel17.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 137F));
            tableLayoutPanel17.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 135F));
            tableLayoutPanel17.Controls.Add(monthlyCollectionExportButton, 3, 0);
            tableLayoutPanel17.Controls.Add(label5, 0, 0);
            tableLayoutPanel17.Controls.Add(monthlyCollectionDateTimePicker, 1, 0);
            tableLayoutPanel17.Controls.Add(monthlyCollectionPrintButton, 2, 0);
            tableLayoutPanel17.Dock = DockStyle.Fill;
            tableLayoutPanel17.Location = new Point(3, 3);
            tableLayoutPanel17.Name = "tableLayoutPanel17";
            tableLayoutPanel17.RowCount = 1;
            tableLayoutPanel17.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel17.Size = new Size(657, 30);
            tableLayoutPanel17.TabIndex = 1;
            // 
            // monthlyCollectionExportButton
            // 
            monthlyCollectionExportButton.BackColor = Color.Green;
            monthlyCollectionExportButton.Dock = DockStyle.Fill;
            monthlyCollectionExportButton.ForeColor = Color.White;
            monthlyCollectionExportButton.Location = new Point(524, 3);
            monthlyCollectionExportButton.Name = "monthlyCollectionExportButton";
            monthlyCollectionExportButton.Size = new Size(130, 24);
            monthlyCollectionExportButton.TabIndex = 18;
            monthlyCollectionExportButton.Text = "📊 EXPORT TO EXCEL";
            monthlyCollectionExportButton.UseVisualStyleBackColor = false;
            monthlyCollectionExportButton.Click += monthlyCollectionExportButton_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Dock = DockStyle.Fill;
            label5.Font = new Font("Arial", 18F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label5.Location = new Point(3, 0);
            label5.Name = "label5";
            label5.Size = new Size(225, 30);
            label5.TabIndex = 16;
            label5.Text = "Daily Collection Report";
            label5.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // monthlyCollectionDateTimePicker
            // 
            monthlyCollectionDateTimePicker.Dock = DockStyle.Fill;
            monthlyCollectionDateTimePicker.Location = new Point(234, 3);
            monthlyCollectionDateTimePicker.Name = "monthlyCollectionDateTimePicker";
            monthlyCollectionDateTimePicker.Size = new Size(147, 23);
            monthlyCollectionDateTimePicker.TabIndex = 1;
            monthlyCollectionDateTimePicker.ValueChanged += monthlyCollectionDateTimePicker_ValueChanged;
            // 
            // monthlyCollectionPrintButton
            // 
            monthlyCollectionPrintButton.BackColor = Color.SteelBlue;
            monthlyCollectionPrintButton.Dock = DockStyle.Fill;
            monthlyCollectionPrintButton.ForeColor = Color.White;
            monthlyCollectionPrintButton.Location = new Point(387, 3);
            monthlyCollectionPrintButton.Name = "monthlyCollectionPrintButton";
            monthlyCollectionPrintButton.Size = new Size(131, 24);
            monthlyCollectionPrintButton.TabIndex = 15;
            monthlyCollectionPrintButton.Text = "🖨️ PRINT";
            monthlyCollectionPrintButton.UseVisualStyleBackColor = false;
            monthlyCollectionPrintButton.Click += monthlyCollectionPrintButton_Click_1;
            // 
            // ReportsControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            Controls.Add(tableLayoutPanel1);
            Name = "ReportsControl";
            Size = new Size(1378, 931);
            Load += ReportsControl_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel10.ResumeLayout(false);
            tableLayoutPanel9.ResumeLayout(false);
            tableLayoutPanel11.ResumeLayout(false);
            dailyTab.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tableLayoutPanel12.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dailyBillingDGV).EndInit();
            tableLayoutPanel13.ResumeLayout(false);
            tableLayoutPanel13.PerformLayout();
            tabPage2.ResumeLayout(false);
            tableLayoutPanel14.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)monthlyBillingDGV).EndInit();
            tableLayoutPanel15.ResumeLayout(false);
            tableLayoutPanel15.PerformLayout();
            mont.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            tableLayoutPanel18.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dailyCollectionDGV).EndInit();
            tableLayoutPanel19.ResumeLayout(false);
            tableLayoutPanel19.PerformLayout();
            tabPage4.ResumeLayout(false);
            tableLayoutPanel16.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)monthlyCollectionDGV).EndInit();
            tableLayoutPanel17.ResumeLayout(false);
            tableLayoutPanel17.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Label label1;
        private Button refreshReportsButton;
        private TableLayoutPanel tableLayoutPanel9;
        private TableLayoutPanel tableLayoutPanel10;
        private TableLayoutPanel tableLayoutPanel11;
        private TabControl mont;
        private TabPage tabPage3;
        private TabPage tabPage4;
        private TabControl dailyTab;
        private TabPage tabPage1;
        private TableLayoutPanel tableLayoutPanel12;
        private DataGridView dailyBillingDGV;
        private TableLayoutPanel tableLayoutPanel13;
        private Label label3;
        private DateTimePicker dailyBillDateTimePicker;
        private Button dailyBillPrintButton;
        private TabPage tabPage2;
        private TableLayoutPanel tableLayoutPanel14;
        private DataGridView monthlyBillingDGV;
        private TableLayoutPanel tableLayoutPanel15;
        private Label label4;
        private DateTimePicker monthBillDateTimePicker;
        private Button monthlyBillPrintButton;
        private TableLayoutPanel tableLayoutPanel18;
        private DataGridView dailyCollectionDGV;
        private TableLayoutPanel tableLayoutPanel19;
        private Label label6;
        private DateTimePicker dailyCollectionDateTimePicker;
        private Button dailyCollectionPrintButton;
        private TableLayoutPanel tableLayoutPanel16;
        private DataGridView monthlyCollectionDGV;
        private TableLayoutPanel tableLayoutPanel17;
        private Label label5;
        private DateTimePicker monthlyCollectionDateTimePicker;
        private Button monthlyCollectionPrintButton;
        private Button dailyBillExportButton;
        private Button monthlyBillExportButton;
        private Button dailyCollectionExportButton;
        private Button monthlyCollectionExportButton;
        private Button exportAllToExcelButton;
    }
}
