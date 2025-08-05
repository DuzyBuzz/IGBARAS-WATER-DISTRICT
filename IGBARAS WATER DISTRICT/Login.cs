using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using IGBARAS_WATER_DISTRICT.Helpers;
using MySql.Data.MySqlClient;


namespace IGBARAS_WATER_DISTRICT
{
    public partial class Login : Form
    {
        private bool isPasswordShown = false;
        private bool isPlaceholderActive = true;
        public Login()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            this.AcceptButton = loginButton;
            PlaceholderHelper.AddPlaceholder(userNameTextBox, "🔑 Username");
            PlaceholderHelper.AddPlaceholder(passwordTextBox, "🔐 Password");
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            this.ControlBox = true;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;


            try
            {
                using (OleDbConnection conn = new OleDbConnection(DbConfig.ConnectionString))
                {
                    conn.Open();
                    Debug.WriteLine("✅ Connected to Access database.");
                    // statusLabel.Text = "Connected";
                    // statusLabel.ForeColor = Color.Green;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Cannot connect to database.\n" + ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine("❌ Connection error: " + ex.Message);
                // statusLabel.Text = "Disconnected";
                // statusLabel.ForeColor = Color.Red;
            }
        }
        private void SetPasswordPlaceholder()
        {
            passwordTextBox.UseSystemPasswordChar = false;
            passwordTextBox.Text = "🔐 Password";
            passwordTextBox.ForeColor = Color.Gray;
            isPlaceholderActive = true;
        }
        private void RemovePasswordPlaceholder()
        {
            if (isPlaceholderActive)
            {
                passwordTextBox.Clear();
                passwordTextBox.UseSystemPasswordChar = true;
                passwordTextBox.ForeColor = Color.Black;
                isPlaceholderActive = false;
            }
        }
        private void loginButton_Click(object sender, EventArgs e)
        {
            if (userNameTextBox.Text == "🔑 Username" || passwordTextBox.Text == "🔐 Password")
            {
                MessageBox.Show("Please enter your username and password.", "Missing Fields", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string username = userNameTextBox.Text.Trim();
            string password = passwordTextBox.Text.Trim();

            using (var conn = new OleDbConnection(DbConfig.ConnectionString))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT userID, userName, fullName, password FROM Users WHERE userName = ?";
                    using (var cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("?", username);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string dbUsername = reader["userName"].ToString();
                                string dbPassword = reader["password"].ToString();

                                // Case-sensitive comparison
                                if (dbUsername == username && dbPassword == password)
                                {
                                    // Save credentials
                                    UserCredentials.UserId = Convert.ToInt32(reader["userID"]);
                                    UserCredentials.Username = dbUsername;
                                    UserCredentials.Fullname = reader["fullName"].ToString();

                                    var dashboard = new MainForm();
                                    dashboard.Show();
                                    this.Hide();
                                }
                                else
                                {
                                    MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



        private void userNameTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void passwordTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void showPasswordCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void togglePasswordButton_Click(object sender, EventArgs e)
        {
            isPasswordShown = !isPasswordShown;

            passwordTextBox.UseSystemPasswordChar = !isPasswordShown;
            togglePasswordButton.Text = isPasswordShown ? "🔒" : "👁";
        }

        private void passwordTextBox_Enter(object sender, EventArgs e)
        {
            RemovePasswordPlaceholder();
        }

        private void passwordTextBox_Leave(object sender, EventArgs e)
        {

        }

        private void passwordTextBox_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void Login_Paint(object sender, PaintEventArgs e)
        {
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Show confirmation only if it's a user-initiated close (not from Application.Exit)
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show(
                    "You are about to close the application.\n\nDo you want to exit now?",
                    "Confirm Exit",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }

                // Close all running forms safely
                foreach (Form form in Application.OpenForms.Cast<Form>().ToList())
                {
                    form.FormClosing -= Login_FormClosing; // Unsubscribe to avoid second trigger
                    form.Close();
                }

                Application.Exit(); // Exit the application
            }
        }
    }
}
