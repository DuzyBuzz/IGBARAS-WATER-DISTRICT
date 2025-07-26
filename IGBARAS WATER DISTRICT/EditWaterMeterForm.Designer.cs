namespace IGBARAS_WATER_DISTRICT
{
    partial class EditWaterMeterForm : Form
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.Text = "Edit Concessionaire Info";
            this.ClientSize = new System.Drawing.Size(700, 700);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Font = new System.Drawing.Font("Segoe UI", 10F);

            // --- TableLayoutPanel for grid layout ---
            var table = new TableLayoutPanel
            {
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(680, 600),
                ColumnCount = 2,
                RowCount = 27,
                AutoScroll = true,
                Dock = DockStyle.Top,
                Padding = new Padding(20, 20, 20, 20),
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            int row = 0;

            // Helper to add a row
            void AddRow(string label, Control control)
            {
                table.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
                table.Controls.Add(new Label
                {
                    Text = label,
                    TextAlign = System.Drawing.ContentAlignment.MiddleRight,
                    Dock = DockStyle.Fill,
                    Font = new System.Drawing.Font("Segoe UI", 10F)
                }, 0, row);
                control.Dock = DockStyle.Fill;
                table.Controls.Add(control, 1, row);
                row++;
            }

            // Controls
            accountnoTextBox = new TextBox();
            concessionairenoTextBox = new TextBox();
            districtnoTextBox = new TextBox();
            concessionairecodeTextBox = new TextBox();
            zonecodeTextBox = new TextBox();
            zoneTextBox = new TextBox();
            servicecodeTextBox = new TextBox();
            servicetypeTextBox = new TextBox();
            pipesizeTextBox = new TextBox();
            servicerateTextBox = new TextBox();
            connectionnoTextBox = new TextBox();
            dateinstalledPicker = new DateTimePicker { Format = DateTimePickerFormat.Short };
            lastnameTextBox = new TextBox();
            firstnameTextBox = new TextBox();
            miTextBox = new TextBox();
            businessnameTextBox = new TextBox();
            contactnoTextBox = new TextBox();
            barangayTextBox = new TextBox();
            barangaycodeTextBox = new TextBox();
            addressTextBox = new TextBox();
            routenoTextBox = new TextBox();
            statusTextBox = new TextBox();
            applyBillCheckBox = new CheckBox { Text = "Apply Bill" };
            meternoTextBox = new TextBox();
            brandTextBox = new TextBox();
            watermetercodeTextBox = new TextBox();
            zonetempTextBox = new TextBox();

            // Add all fields to the grid
            AddRow("Account No:", accountnoTextBox);
            AddRow("Concessionaire No:", concessionairenoTextBox);
            AddRow("District No:", districtnoTextBox);
            AddRow("Concessionaire Code:", concessionairecodeTextBox);
            AddRow("Zone Code:", zonecodeTextBox);
            AddRow("Zone:", zoneTextBox);
            AddRow("Service Code:", servicecodeTextBox);
            AddRow("Service Type:", servicetypeTextBox);
            AddRow("Pipe Size:", pipesizeTextBox);
            AddRow("Service Rate:", servicerateTextBox);
            AddRow("Connection No:", connectionnoTextBox);
            AddRow("Date Installed:", dateinstalledPicker);
            AddRow("Last Name:", lastnameTextBox);
            AddRow("First Name:", firstnameTextBox);
            AddRow("MI:", miTextBox);
            AddRow("Business Name:", businessnameTextBox);
            AddRow("Contact No:", contactnoTextBox);
            AddRow("Barangay:", barangayTextBox);
            AddRow("Barangay Code:", barangaycodeTextBox);
            AddRow("Address:", addressTextBox);
            AddRow("Route No:", routenoTextBox);
            AddRow("Status:", statusTextBox);
            AddRow("Apply Bill:", applyBillCheckBox);
            AddRow("Meter No:", meternoTextBox);
            AddRow("Brand:", brandTextBox);
            AddRow("Water Meter Code:", watermetercodeTextBox);
            AddRow("Zone Temp:", zonetempTextBox);

            // --- Buttons ---
            saveButton = new Button
            {
                Text = "Save",
                Width = 110,
                Height = 36,
                Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold),
                Anchor = AnchorStyles.Right
            };
            saveButton.Click += new System.EventHandler(this.saveButton_Click);

            cancelButton = new Button
            {
                Text = "Cancel",
                Width = 110,
                Height = 36,
                Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold),
                Anchor = AnchorStyles.Left
            };
            cancelButton.Click += (s, e) => this.Close();

            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(0, 10, 20, 10)
            };
            buttonPanel.Controls.Add(saveButton);
            buttonPanel.Controls.Add(cancelButton);

            // --- Add to Form ---
            this.Controls.Add(table);
            this.Controls.Add(buttonPanel);
        }

        #endregion

        #region Fields

        public TextBox accountnoTextBox, concessionairenoTextBox, districtnoTextBox, concessionairecodeTextBox, zonecodeTextBox, zoneTextBox,
            servicecodeTextBox, servicetypeTextBox, pipesizeTextBox, servicerateTextBox, connectionnoTextBox,
            lastnameTextBox, firstnameTextBox, miTextBox, businessnameTextBox, contactnoTextBox, barangayTextBox,
            barangaycodeTextBox, addressTextBox, routenoTextBox, statusTextBox, meternoTextBox, brandTextBox,
            watermetercodeTextBox, zonetempTextBox;
        public DateTimePicker dateinstalledPicker;
        public CheckBox applyBillCheckBox;
        public Button saveButton, cancelButton;

        #endregion
    }
}

