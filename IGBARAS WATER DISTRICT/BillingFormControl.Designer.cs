namespace IGBARAS_WATER_DISTRICT
{
    partial class BillingFormControl
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
            labelInvoiceNo = new Label();
            labelBillingDate = new Label();
            labelDueDate = new Label();
            labelCustomerInfo = new Label();
            labelMeterReading = new Label();
            labelWaterConsumption = new Label();
            labelMinimumCharge = new Label();
            labelBreakdown = new Label();
            labelDiscount = new Label();
            labelFranchiseTax = new Label();
            labelWithholdingTax = new Label();
            labelAddArrears = new Label();
            labelTotalDue = new Label();
            labelPenalty = new Label();
            labelFinalAmountDue = new Label();
            labelFooter = new Label();
            labelSignature = new Label();
            labelIDNo = new Label();
            labelCopyNote = new Label();
            SuspendLayout();
            // 
            // labelInvoiceNo
            // 
            labelInvoiceNo.BackColor = Color.Transparent;
            labelInvoiceNo.Font = new Font("Arial", 10F, FontStyle.Bold);
            labelInvoiceNo.ForeColor = Color.Red;
            labelInvoiceNo.Location = new Point(270, 95);
            labelInvoiceNo.Name = "labelInvoiceNo";
            labelInvoiceNo.Size = new Size(120, 20);
            labelInvoiceNo.TabIndex = 0;
            labelInvoiceNo.Text = "0000000001";
            // 
            // labelBillingDate
            // 
            labelBillingDate.BackColor = Color.Transparent;
            labelBillingDate.Font = new Font("Arial", 9F);
            labelBillingDate.Location = new Point(270, 115);
            labelBillingDate.Name = "labelBillingDate";
            labelBillingDate.Size = new Size(100, 20);
            labelBillingDate.TabIndex = 1;
            labelBillingDate.Text = "June 2, 2025";
            // 
            // labelDueDate
            // 
            labelDueDate.BackColor = Color.Transparent;
            labelDueDate.Font = new Font("Arial", 9F);
            labelDueDate.Location = new Point(735, 33);
            labelDueDate.Name = "labelDueDate";
            labelDueDate.Size = new Size(100, 20);
            labelDueDate.TabIndex = 2;
            labelDueDate.Text = "June 16, 2025";
            // 
            // labelCustomerInfo
            // 
            labelCustomerInfo.BackColor = Color.Transparent;
            labelCustomerInfo.Font = new Font("Arial", 9F);
            labelCustomerInfo.Location = new Point(110, 155);
            labelCustomerInfo.Name = "labelCustomerInfo";
            labelCustomerInfo.Size = new Size(280, 60);
            labelCustomerInfo.TabIndex = 3;
            labelCustomerInfo.Text = "JUAN DE LA CRUZ\n000-000-000\nM. EZPELETA ST., IGBARAS, ILOILO\n00-0-00-000";
            // 
            // labelMeterReading
            // 
            labelMeterReading.BackColor = Color.Transparent;
            labelMeterReading.Font = new Font("Arial", 8F);
            labelMeterReading.Location = new Point(140, 245);
            labelMeterReading.Name = "labelMeterReading";
            labelMeterReading.Size = new Size(300, 20);
            labelMeterReading.TabIndex = 4;
            labelMeterReading.Text = "Previous: 30  Present: 3070  Cu. M: 3040";
            // 
            // labelWaterConsumption
            // 
            labelWaterConsumption.BackColor = Color.Transparent;
            labelWaterConsumption.Font = new Font("Arial", 8F);
            labelWaterConsumption.Location = new Point(570, 100);
            labelWaterConsumption.Name = "labelWaterConsumption";
            labelWaterConsumption.Size = new Size(150, 20);
            labelWaterConsumption.TabIndex = 5;
            labelWaterConsumption.Text = "WATER CONSUMPTION";
            // 
            // labelMinimumCharge
            // 
            labelMinimumCharge.BackColor = Color.Transparent;
            labelMinimumCharge.Font = new Font("Arial", 8F);
            labelMinimumCharge.Location = new Point(570, 120);
            labelMinimumCharge.Name = "labelMinimumCharge";
            labelMinimumCharge.Size = new Size(150, 30);
            labelMinimumCharge.TabIndex = 6;
            labelMinimumCharge.Text = "Minimum Charge\n352.00";
            // 
            // labelBreakdown
            // 
            labelBreakdown.BackColor = Color.Transparent;
            labelBreakdown.Font = new Font("Arial", 7.5F);
            labelBreakdown.Location = new Point(570, 160);
            labelBreakdown.Name = "labelBreakdown";
            labelBreakdown.Size = new Size(280, 100);
            labelBreakdown.TabIndex = 7;
            labelBreakdown.Text = "Qty  Unit Price  Amount\n0–10 m³  10  35.20  352.00\n...";
            // 
            // labelDiscount
            // 
            labelDiscount.BackColor = Color.Transparent;
            labelDiscount.Font = new Font("Arial", 8F);
            labelDiscount.Location = new Point(570, 265);
            labelDiscount.Name = "labelDiscount";
            labelDiscount.Size = new Size(250, 20);
            labelDiscount.TabIndex = 8;
            labelDiscount.Text = "Less: Discount 7%  -9,168.36";
            // 
            // labelFranchiseTax
            // 
            labelFranchiseTax.BackColor = Color.Transparent;
            labelFranchiseTax.Font = new Font("Arial", 8F);
            labelFranchiseTax.Location = new Point(570, 285);
            labelFranchiseTax.Name = "labelFranchiseTax";
            labelFranchiseTax.Size = new Size(250, 20);
            labelFranchiseTax.TabIndex = 9;
            labelFranchiseTax.Text = "Add: Franchise Tax 2%  2,802.99";
            // 
            // labelWithholdingTax
            // 
            labelWithholdingTax.BackColor = Color.Transparent;
            labelWithholdingTax.Font = new Font("Arial", 8F);
            labelWithholdingTax.Location = new Point(570, 305);
            labelWithholdingTax.Name = "labelWithholdingTax";
            labelWithholdingTax.Size = new Size(250, 20);
            labelWithholdingTax.TabIndex = 10;
            labelWithholdingTax.Text = "Less: Withholding Tax";
            // 
            // labelAddArrears
            // 
            labelAddArrears.BackColor = Color.Transparent;
            labelAddArrears.Font = new Font("Arial", 8F);
            labelAddArrears.Location = new Point(570, 325);
            labelAddArrears.Name = "labelAddArrears";
            labelAddArrears.Size = new Size(250, 30);
            labelAddArrears.TabIndex = 11;
            labelAddArrears.Text = "Add: Arrears\n110,000.00";
            labelAddArrears.Click += labelAddArrears_Click;
            // 
            // labelTotalDue
            // 
            labelTotalDue.BackColor = Color.Transparent;
            labelTotalDue.Font = new Font("Arial", 9.5F, FontStyle.Bold);
            labelTotalDue.Location = new Point(570, 360);
            labelTotalDue.Name = "labelTotalDue";
            labelTotalDue.Size = new Size(250, 30);
            labelTotalDue.TabIndex = 12;
            labelTotalDue.Text = "TOTAL AMOUNT DUE\n234,611.04";
            // 
            // labelPenalty
            // 
            labelPenalty.BackColor = Color.Transparent;
            labelPenalty.Font = new Font("Arial", 8F);
            labelPenalty.Location = new Point(570, 390);
            labelPenalty.Name = "labelPenalty";
            labelPenalty.Size = new Size(250, 30);
            labelPenalty.TabIndex = 13;
            labelPenalty.Text = "Penalty 10%\n12,180.81";
            // 
            // labelFinalAmountDue
            // 
            labelFinalAmountDue.BackColor = Color.Transparent;
            labelFinalAmountDue.Font = new Font("Arial", 9.5F, FontStyle.Bold);
            labelFinalAmountDue.Location = new Point(570, 415);
            labelFinalAmountDue.Name = "labelFinalAmountDue";
            labelFinalAmountDue.Size = new Size(250, 30);
            labelFinalAmountDue.TabIndex = 14;
            labelFinalAmountDue.Text = "TOTAL AMOUNT DUE\n246,791.86";
            // 
            // labelFooter
            // 
            labelFooter.BackColor = Color.Transparent;
            labelFooter.Font = new Font("Arial", 9F, FontStyle.Bold);
            labelFooter.ForeColor = Color.Red;
            labelFooter.Location = new Point(30, 225);
            labelFooter.Name = "labelFooter";
            labelFooter.Size = new Size(300, 30);
            labelFooter.TabIndex = 15;
            labelFooter.Text = "THIS DOCUMENT IS NOT VALID FOR CLAIM OF INPUT TAX";
            // 
            // labelSignature
            // 
            labelSignature.BackColor = Color.Transparent;
            labelSignature.Font = new Font("Arial", 8F);
            labelSignature.Location = new Point(140, 370);
            labelSignature.Name = "labelSignature";
            labelSignature.Size = new Size(100, 20);
            labelSignature.TabIndex = 16;
            labelSignature.Text = "Signature:";
            // 
            // labelIDNo
            // 
            labelIDNo.BackColor = Color.Transparent;
            labelIDNo.Font = new Font("Arial", 8F);
            labelIDNo.Location = new Point(140, 350);
            labelIDNo.Name = "labelIDNo";
            labelIDNo.Size = new Size(200, 20);
            labelIDNo.TabIndex = 17;
            labelIDNo.Text = "ID No.: 000000000";
            // 
            // labelCopyNote
            // 
            labelCopyNote.BackColor = Color.Transparent;
            labelCopyNote.Font = new Font("Arial", 9F);
            labelCopyNote.Location = new Point(735, 15);
            labelCopyNote.Name = "labelCopyNote";
            labelCopyNote.Size = new Size(150, 20);
            labelCopyNote.TabIndex = 18;
            labelCopyNote.Text = "Concessionaire's Copy";
            // 
            // BillingFormControl
            // 
            BackgroundImageLayout = ImageLayout.Stretch;
            Controls.Add(labelInvoiceNo);
            Controls.Add(labelBillingDate);
            Controls.Add(labelDueDate);
            Controls.Add(labelCustomerInfo);
            Controls.Add(labelMeterReading);
            Controls.Add(labelWaterConsumption);
            Controls.Add(labelMinimumCharge);
            Controls.Add(labelBreakdown);
            Controls.Add(labelDiscount);
            Controls.Add(labelFranchiseTax);
            Controls.Add(labelWithholdingTax);
            Controls.Add(labelAddArrears);
            Controls.Add(labelTotalDue);
            Controls.Add(labelPenalty);
            Controls.Add(labelFinalAmountDue);
            Controls.Add(labelFooter);
            Controls.Add(labelSignature);
            Controls.Add(labelIDNo);
            Controls.Add(labelCopyNote);
            Name = "BillingFormControl";
            Size = new Size(873, 459);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label labelInvoiceNo;
        private System.Windows.Forms.Label labelBillingDate;
        private System.Windows.Forms.Label labelDueDate;
        private System.Windows.Forms.Label labelCustomerInfo;
        private System.Windows.Forms.Label labelMeterReading;
        private System.Windows.Forms.Label labelWaterConsumption;
        private System.Windows.Forms.Label labelMinimumCharge;
        private System.Windows.Forms.Label labelBreakdown;
        private System.Windows.Forms.Label labelDiscount;
        private System.Windows.Forms.Label labelFranchiseTax;
        private System.Windows.Forms.Label labelWithholdingTax;
        private System.Windows.Forms.Label labelAddArrears;
        private System.Windows.Forms.Label labelTotalDue;
        private System.Windows.Forms.Label labelPenalty;
        private System.Windows.Forms.Label labelFinalAmountDue;
        private System.Windows.Forms.Label labelFooter;
        private System.Windows.Forms.Label labelSignature;
        private System.Windows.Forms.Label labelIDNo;
        private System.Windows.Forms.Label labelCopyNote;
    }
}