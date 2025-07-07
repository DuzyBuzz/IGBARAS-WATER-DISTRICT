namespace IGBARAS_WATER_DISTRICT
{
    partial class AgingControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private Label label;

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
            components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;


            this.label = new Label();
            this.SuspendLayout();
            // 
            // label
            // 
            this.label.Text = "📅 Aging of Accounts";
            this.label.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.label.Dock = DockStyle.Fill;
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AgingControl
            // 
            this.Controls.Add(this.label);
            this.Name = "AgingControl";
            this.Size = new System.Drawing.Size(800, 600);
            this.ResumeLayout(false);
        }

        #endregion
    }
}
