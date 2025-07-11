namespace IGBARAS_WATER_DISTRICT
{
    partial class Login
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            pictureBox1 = new PictureBox();
            userNameTextBox = new TextBox();
            passwordTextBox = new TextBox();
            loginButton = new Button();
            label3 = new Label();
            togglePasswordButton = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Left;
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(57, 41);
            pictureBox1.Margin = new Padding(4, 3, 4, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(264, 193);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // userNameTextBox
            // 
            userNameTextBox.Anchor = AnchorStyles.Left;
            userNameTextBox.Location = new Point(57, 256);
            userNameTextBox.Margin = new Padding(4, 3, 4, 3);
            userNameTextBox.Name = "userNameTextBox";
            userNameTextBox.Size = new Size(264, 26);
            userNameTextBox.TabIndex = 2;
            userNameTextBox.TextChanged += userNameTextBox_TextChanged;
            // 
            // passwordTextBox
            // 
            passwordTextBox.Anchor = AnchorStyles.Bottom;
            passwordTextBox.Location = new Point(57, 304);
            passwordTextBox.Margin = new Padding(4, 3, 4, 3);
            passwordTextBox.Name = "passwordTextBox";
            passwordTextBox.Size = new Size(264, 26);
            passwordTextBox.TabIndex = 4;
            passwordTextBox.TextChanged += passwordTextBox_TextChanged;
            passwordTextBox.Enter += passwordTextBox_Enter;
            passwordTextBox.KeyDown += passwordTextBox_KeyDown;
            passwordTextBox.Leave += passwordTextBox_Leave;
            // 
            // loginButton
            // 
            loginButton.Anchor = AnchorStyles.Left;
            loginButton.BackColor = Color.MidnightBlue;
            loginButton.FlatStyle = FlatStyle.Popup;
            loginButton.ForeColor = SystemColors.ButtonFace;
            loginButton.Location = new Point(141, 355);
            loginButton.Margin = new Padding(4, 3, 4, 3);
            loginButton.Name = "loginButton";
            loginButton.Size = new Size(96, 27);
            loginButton.TabIndex = 6;
            loginButton.Text = "Login";
            loginButton.UseVisualStyleBackColor = false;
            loginButton.Click += loginButton_Click;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(9, 410);
            label3.Name = "label3";
            label3.Size = new Size(48, 13);
            label3.TabIndex = 7;
            label3.Text = "v.1.0.0.0";
            // 
            // togglePasswordButton
            // 
            togglePasswordButton.Anchor = AnchorStyles.Bottom;
            togglePasswordButton.Location = new Point(283, 304);
            togglePasswordButton.Name = "togglePasswordButton";
            togglePasswordButton.Size = new Size(38, 26);
            togglePasswordButton.TabIndex = 9;
            togglePasswordButton.Text = "🔒";
            togglePasswordButton.UseVisualStyleBackColor = true;
            togglePasswordButton.Click += togglePasswordButton_Click;
            // 
            // Login
            // 
            AutoScaleDimensions = new SizeF(9F, 18F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightSkyBlue;
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(378, 424);
            Controls.Add(togglePasswordButton);
            Controls.Add(label3);
            Controls.Add(loginButton);
            Controls.Add(passwordTextBox);
            Controls.Add(userNameTextBox);
            Controls.Add(pictureBox1);
            DoubleBuffered = true;
            Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "Login";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Login";
            Load += Login_Load;
            Paint += Login_Paint;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private PictureBox pictureBox1;
        private TextBox userNameTextBox;
        private TextBox passwordTextBox;
        private Button loginButton;
        private Label label3;
        private Button togglePasswordButton;
    }
}