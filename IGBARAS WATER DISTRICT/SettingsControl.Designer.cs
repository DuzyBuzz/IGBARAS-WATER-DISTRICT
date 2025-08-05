namespace IGBARAS_WATER_DISTRICT
{
    partial class SettingsControl
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
            confirmPasswordTextBox = new TextBox();
            newPasswordTextBox = new TextBox();
            currentPasswordTextBox = new TextBox();
            userNameTextBox = new TextBox();
            fullnameTextBox = new TextBox();
            label1 = new Label();
            accountApplyButton = new Button();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 1, 1);
            tableLayoutPanel1.Controls.Add(label1, 1, 0);
            tableLayoutPanel1.Controls.Add(accountApplyButton, 1, 2);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 24.06015F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 34.3716431F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 41.5682068F));
            tableLayoutPanel1.Size = new Size(1378, 931);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Controls.Add(confirmPasswordTextBox, 0, 5);
            tableLayoutPanel2.Controls.Add(newPasswordTextBox, 0, 4);
            tableLayoutPanel2.Controls.Add(currentPasswordTextBox, 0, 3);
            tableLayoutPanel2.Controls.Add(userNameTextBox, 0, 2);
            tableLayoutPanel2.Controls.Add(fullnameTextBox, 0, 1);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(462, 227);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 6;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 11.7647057F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 17.6470585F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 17.6470585F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 17.6470585F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 17.6470585F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 17.6470585F));
            tableLayoutPanel2.Size = new Size(453, 314);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // confirmPasswordTextBox
            // 
            confirmPasswordTextBox.Dock = DockStyle.Fill;
            confirmPasswordTextBox.Font = new Font("Arial", 12F);
            confirmPasswordTextBox.Location = new Point(3, 259);
            confirmPasswordTextBox.Name = "confirmPasswordTextBox";
            confirmPasswordTextBox.Size = new Size(447, 26);
            confirmPasswordTextBox.TabIndex = 5;
            // 
            // newPasswordTextBox
            // 
            newPasswordTextBox.Dock = DockStyle.Fill;
            newPasswordTextBox.Font = new Font("Arial", 12F);
            newPasswordTextBox.Location = new Point(3, 204);
            newPasswordTextBox.Name = "newPasswordTextBox";
            newPasswordTextBox.Size = new Size(447, 26);
            newPasswordTextBox.TabIndex = 4;
            // 
            // currentPasswordTextBox
            // 
            currentPasswordTextBox.Dock = DockStyle.Fill;
            currentPasswordTextBox.Font = new Font("Arial", 12F);
            currentPasswordTextBox.Location = new Point(3, 149);
            currentPasswordTextBox.Name = "currentPasswordTextBox";
            currentPasswordTextBox.Size = new Size(447, 26);
            currentPasswordTextBox.TabIndex = 3;
            // 
            // userNameTextBox
            // 
            userNameTextBox.Dock = DockStyle.Fill;
            userNameTextBox.Font = new Font("Arial", 12F);
            userNameTextBox.Location = new Point(3, 94);
            userNameTextBox.Name = "userNameTextBox";
            userNameTextBox.Size = new Size(447, 26);
            userNameTextBox.TabIndex = 2;
            // 
            // fullnameTextBox
            // 
            fullnameTextBox.Dock = DockStyle.Fill;
            fullnameTextBox.Font = new Font("Arial", 12F);
            fullnameTextBox.Location = new Point(3, 39);
            fullnameTextBox.Name = "fullnameTextBox";
            fullnameTextBox.Size = new Size(447, 26);
            fullnameTextBox.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Bottom;
            label1.Font = new Font("Arial Narrow", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(462, 195);
            label1.Name = "label1";
            label1.Size = new Size(453, 29);
            label1.TabIndex = 1;
            label1.Text = "Account Settings";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // accountApplyButton
            // 
            accountApplyButton.Anchor = AnchorStyles.Top;
            accountApplyButton.BackColor = Color.SteelBlue;
            accountApplyButton.ForeColor = Color.White;
            accountApplyButton.Location = new Point(605, 547);
            accountApplyButton.Name = "accountApplyButton";
            accountApplyButton.Size = new Size(167, 36);
            accountApplyButton.TabIndex = 18;
            accountApplyButton.Text = "✏️ Apply Changes";
            accountApplyButton.UseVisualStyleBackColor = false;
            // 
            // SettingsControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            Controls.Add(tableLayoutPanel1);
            Name = "SettingsControl";
            Size = new Size(1378, 931);
            Load += SettingsControl_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private TextBox confirmPasswordTextBox;
        private TextBox newPasswordTextBox;
        private TextBox currentPasswordTextBox;
        private TextBox userNameTextBox;
        private TextBox fullnameTextBox;
        private Label label1;
        private Button accountApplyButton;
    }
}
