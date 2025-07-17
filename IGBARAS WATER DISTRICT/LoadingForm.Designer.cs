namespace IGBARAS_WATER_DISTRICT
{
    partial class LoadingForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            loadingLabel = new Label();
            animationTimer = new System.Windows.Forms.Timer(components);
            tableLayoutPanel1 = new TableLayoutPanel();
            statusLabel = new Label();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // loadingLabel
            // 
            loadingLabel.AutoSize = true;
            loadingLabel.BackColor = SystemColors.ControlLightLight;
            loadingLabel.Dock = DockStyle.Fill;
            loadingLabel.Font = new Font("Arial", 99.75F, FontStyle.Italic, GraphicsUnit.Point, 0);
            loadingLabel.Location = new Point(3, 3);
            loadingLabel.Margin = new Padding(3);
            loadingLabel.Name = "loadingLabel";
            loadingLabel.Size = new Size(965, 172);
            loadingLabel.TabIndex = 0;
            loadingLabel.Text = "Loading...";
            loadingLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(loadingLabel, 0, 0);
            tableLayoutPanel1.Controls.Add(statusLabel, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 80.18018F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 19.81982F));
            tableLayoutPanel1.Size = new Size(971, 222);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // statusLabel
            // 
            statusLabel.AutoSize = true;
            statusLabel.Dock = DockStyle.Fill;
            statusLabel.Location = new Point(3, 181);
            statusLabel.Margin = new Padding(3);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(965, 38);
            statusLabel.TabIndex = 1;
            statusLabel.Text = "Fetching Data";
            statusLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LoadingForm
            // 
            AutoScaleDimensions = new SizeF(17F, 34F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            BackgroundImageLayout = ImageLayout.Zoom;
            ClientSize = new Size(971, 222);
            ControlBox = false;
            Controls.Add(tableLayoutPanel1);
            Font = new Font("Arial", 21.75F, FontStyle.Italic, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(7);
            Name = "LoadingForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "LoadingForm";
            Load += LoadingForm_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label loadingLabel;
        private System.Windows.Forms.Timer animationTimer;
        private TableLayoutPanel tableLayoutPanel1;
        public Label statusLabel;
    }
}