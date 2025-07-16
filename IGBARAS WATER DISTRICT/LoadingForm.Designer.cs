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
            SuspendLayout();
            // 
            // loadingLabel
            // 
            loadingLabel.AutoSize = true;
            loadingLabel.BackColor = SystemColors.ControlLightLight;
            loadingLabel.Dock = DockStyle.Fill;
            loadingLabel.Font = new Font("Arial", 150F, FontStyle.Italic, GraphicsUnit.Point, 0);
            loadingLabel.Location = new Point(0, 0);
            loadingLabel.Name = "loadingLabel";
            loadingLabel.Size = new Size(971, 222);
            loadingLabel.TabIndex = 0;
            loadingLabel.Text = "Loading...";
            loadingLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LoadingForm
            // 
            AutoScaleDimensions = new SizeF(17F, 34F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            BackgroundImageLayout = ImageLayout.Zoom;
            ClientSize = new Size(971, 222);
            ControlBox = false;
            Controls.Add(loadingLabel);
            Font = new Font("Arial", 21.75F, FontStyle.Italic, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(7);
            Name = "LoadingForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "LoadingForm";
            Load += LoadingForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label loadingLabel;
        private System.Windows.Forms.Timer animationTimer;
    }
}