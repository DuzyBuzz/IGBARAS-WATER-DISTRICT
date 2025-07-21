namespace IGBARAS_WATER_DISTRICT
{
    partial class CollectionReceiptForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private Panel receiptPanel;

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
            this.receiptPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();


            this.receiptPanel.BackColor = System.Drawing.Color.White;
            this.receiptPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.receiptPanel.Location = new System.Drawing.Point(10, 10);
            this.receiptPanel.Name = "receiptPanel";
            this.receiptPanel.Size = new System.Drawing.Size(408, 624); // A4 1/4 (4.25" x 6.5")
            this.Controls.Add(this.receiptPanel);

            // Title
            Label title = new Label();
            title.Text = "IGBARAS WATER DISTRICT (ILOILO)";
            title.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            title.TextAlign = ContentAlignment.MiddleCenter;
            title.Size = new Size(400, 30);
            title.Location = new Point(4, 10);
            receiptPanel.Controls.Add(title);

            // Receipt No.
            Label receiptNo = new Label();
            receiptNo.Text = "No.: 0000001";
            receiptNo.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            receiptNo.ForeColor = Color.Maroon;
            receiptNo.Location = new Point(300, 40);
            receiptNo.AutoSize = true;
            receiptPanel.Controls.Add(receiptNo);

            // Payment Date
            Label paymentDate = new Label();
            paymentDate.Text = "Payment Date: _______________";
            paymentDate.Location = new Point(200, 70);
            paymentDate.Size = new Size(200, 20);
            receiptPanel.Controls.Add(paymentDate);

            // Received From
            Label receivedFrom = new Label();
            receivedFrom.Text = "RECEIVED FROM:";
            receivedFrom.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            receivedFrom.Location = new Point(10, 70);
            receivedFrom.Size = new Size(150, 20);
            receiptPanel.Controls.Add(receivedFrom);

            // Registered Name
            Label regName = new Label();
            regName.Text = "Registered Name:";
            regName.Location = new Point(10, 100);
            regName.Size = new Size(120, 20);
            receiptPanel.Controls.Add(regName);

            TextBox regNameBox = new TextBox();
            regNameBox.Location = new Point(130, 100);
            regNameBox.Size = new Size(260, 20);
            receiptPanel.Controls.Add(regNameBox);

            // TIN
            Label tin = new Label();
            tin.Text = "TIN:";
            tin.Location = new Point(10, 130);
            tin.Size = new Size(100, 20);
            receiptPanel.Controls.Add(tin);

            TextBox tinBox = new TextBox();
            tinBox.Location = new Point(130, 130);
            tinBox.Size = new Size(260, 20);
            receiptPanel.Controls.Add(tinBox);

            // Business Address
            Label address = new Label();
            address.Text = "Business Address:";
            address.Location = new Point(10, 160);
            address.Size = new Size(120, 20);
            receiptPanel.Controls.Add(address);

            TextBox addressBox = new TextBox();
            addressBox.Location = new Point(130, 160);
            addressBox.Size = new Size(260, 20);
            receiptPanel.Controls.Add(addressBox);

            // Table: Payment Fields
            string[] descLabels = {
        "Payment for Metered Current Billing",
        "Payment for Metered Arrears Billing",
        "Payment for Penalty",
        "Payment for Franchise Tax",
        "Payment for SCF",
        "Payment for Others:"
    };

            int startY = 200;
            for (int i = 0; i < descLabels.Length; i++)
            {
                Label lbl = new Label();
                lbl.Text = descLabels[i];
                lbl.Location = new Point(10, startY + i * 25);
                lbl.Size = new Size(250, 20);
                receiptPanel.Controls.Add(lbl);

                TextBox amountBox = new TextBox();
                amountBox.Location = new Point(270, startY + i * 25);
                amountBox.Size = new Size(120, 20);
                receiptPanel.Controls.Add(amountBox);
            }

            // Total + Invoice
            Label totalPaid = new Label();
            totalPaid.Text = "TOTAL AMOUNT PAID:";
            totalPaid.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            totalPaid.Location = new Point(10, 370);
            totalPaid.Size = new Size(200, 20);
            receiptPanel.Controls.Add(totalPaid);

            TextBox totalBox = new TextBox();
            totalBox.Location = new Point(220, 370);
            totalBox.Size = new Size(170, 20);
            receiptPanel.Controls.Add(totalBox);

            Label billingInvoice = new Label();
            billingInvoice.Text = "BILLING INVOICE NO.:";
            billingInvoice.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            billingInvoice.Location = new Point(10, 400);
            billingInvoice.Size = new Size(200, 20);
            receiptPanel.Controls.Add(billingInvoice);

            TextBox invoiceBox = new TextBox();
            invoiceBox.Location = new Point(220, 400);
            invoiceBox.Size = new Size(170, 20);
            receiptPanel.Controls.Add(invoiceBox);

            // Mode of Payment
            Label modePayment = new Label();
            modePayment.Text = "MODE OF PAYMENT:";
            modePayment.Location = new Point(10, 440);
            modePayment.Size = new Size(200, 20);
            receiptPanel.Controls.Add(modePayment);

            CheckBox cashBox = new CheckBox();
            cashBox.Text = "CASH";
            cashBox.Location = new Point(30, 460);
            receiptPanel.Controls.Add(cashBox);

            CheckBox checkBox = new CheckBox();
            checkBox.Text = "CHECK";
            checkBox.Location = new Point(100, 460);
            receiptPanel.Controls.Add(checkBox);

            Label bankLabel = new Label();
            bankLabel.Text = "BANK/No.:";
            bankLabel.Location = new Point(180, 460);
            receiptPanel.Controls.Add(bankLabel);

            TextBox bankBox = new TextBox();
            bankBox.Location = new Point(250, 460);
            bankBox.Size = new Size(140, 20);
            receiptPanel.Controls.Add(bankBox);

            // Collecting Officer
            Label officer = new Label();
            officer.Text = "COLLECTING OFFICER:";
            officer.Location = new Point(10, 500);
            receiptPanel.Controls.Add(officer);

            TextBox officerBox = new TextBox();
            officerBox.Location = new Point(150, 500);
            officerBox.Size = new Size(240, 20);
            receiptPanel.Controls.Add(officerBox);

            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 650); // Make sure fits the panel
            this.Name = "CollectionReceiptForm";
            this.Text = "Collection Receipt";

            this.ResumeLayout(false);
        }


        #endregion
    }
}