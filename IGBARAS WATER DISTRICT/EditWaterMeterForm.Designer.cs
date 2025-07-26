namespace IGBARAS_WATER_DISTRICT
{
    public partial class EditWaterMeterForm : Form
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
            // === Form Setup ===
            this.Text = "Edit Concessionaire Info";
            this.ClientSize = new System.Drawing.Size(900, 750);
            this.Font = new Font("Segoe UI", 10F);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;

            // === Scrollable Panel ===
            var scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(20),
                BackColor = SystemColors.Control
            };

            // === Grid-style TableLayoutPanel ===
            var grid = new TableLayoutPanel
            {
                ColumnCount = 4,
                AutoSize = true,
                Dock = DockStyle.Top,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                Padding = new Padding(10),
            };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F)); // Label 1
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));   // Control 1
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F)); // Label 2
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));   // Control 2

            int currentColumn = 0;
            int currentRow = 0;

            // === Helper to add controls in grid pattern ===
            void AddGridField(string label, Control control)
            {
                // Add a new row every 2 fields (i.e., when column reaches 4)
                if (currentColumn >= 4)
                {
                    currentColumn = 0;
                    currentRow++;
                    grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
                }

                // Add label
                var lbl = new Label
                {
                    Text = label,
                    TextAlign = ContentAlignment.MiddleRight,
                    Dock = DockStyle.Fill,
                    Padding = new Padding(0, 0, 5, 0)
                };
                grid.Controls.Add(lbl, currentColumn, currentRow);
                currentColumn++;

                // Add control
                control.Dock = DockStyle.Fill;
                control.Margin = new Padding(0, 3, 10, 3);
                grid.Controls.Add(control, currentColumn, currentRow);
                currentColumn++;
            }

            // === Controls ===
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
            applyBillCheckBox = new CheckBox { Text = "Apply", AutoSize = true };
            meternoTextBox = new TextBox();
            brandTextBox = new TextBox();
            watermetercodeTextBox = new TextBox();
            zonetempTextBox = new TextBox();

            // === Add Fields in Grid ===
            AddGridField("Account No:", accountnoTextBox);
            AddGridField("Concessionaire No:", concessionairenoTextBox);
            AddGridField("District No:", districtnoTextBox);
            AddGridField("Concessionaire Code:", concessionairecodeTextBox);
            AddGridField("Zone Code:", zonecodeTextBox);
            AddGridField("Zone:", zoneTextBox);
            AddGridField("Service Code:", servicecodeTextBox);
            AddGridField("Service Type:", servicetypeTextBox);
            AddGridField("Pipe Size:", pipesizeTextBox);
            AddGridField("Service Rate:", servicerateTextBox);
            AddGridField("Connection No:", connectionnoTextBox);
            AddGridField("Date Installed:", dateinstalledPicker);
            AddGridField("Last Name:", lastnameTextBox);
            AddGridField("First Name:", firstnameTextBox);
            AddGridField("MI:", miTextBox);
            AddGridField("Business Name:", businessnameTextBox);
            AddGridField("Contact No:", contactnoTextBox);
            AddGridField("Barangay:", barangayTextBox);
            AddGridField("Barangay Code:", barangaycodeTextBox);
            AddGridField("Address:", addressTextBox);
            AddGridField("Route No:", routenoTextBox);
            AddGridField("Status:", statusTextBox);
            AddGridField("Apply Bill:", applyBillCheckBox);
            AddGridField("Meter No:", meternoTextBox);
            AddGridField("Brand:", brandTextBox);
            AddGridField("Water Meter Code:", watermetercodeTextBox);
            AddGridField("Zone Temp:", zonetempTextBox);

            scrollPanel.Controls.Add(grid);

            // === Buttons Panel ===
            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(20, 10, 20, 10),
                BackColor = SystemColors.ControlLight
            };

            saveButton = new Button
            {
                Text = "Save",
                Width = 120,
                Height = 36,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            saveButton.Click += new EventHandler(this.saveButton_Click);

            cancelButton = new Button
            {
                Text = "Cancel",
                Width = 120,
                Height = 36,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            cancelButton.Click += (s, e) => this.Close();

            buttonPanel.Controls.Add(saveButton);
            buttonPanel.Controls.Add(cancelButton);

            // === Add to Form ===
            this.Controls.Add(scrollPanel);
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
