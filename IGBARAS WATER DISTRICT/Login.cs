using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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


            // ✅ Check MySQL connection
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();
                    // Optional: Display success message (debug mode)
                    Console.WriteLine("✅ Connected to MySQL database.");
                    // Or show status label
                    // statusLabel.Text = "Connected";
                    // statusLabel.ForeColor = Color.Green;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Cannot connect to database.\n" + ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            using (MySqlConnection conn = new MySqlConnection(DbConfig.ConnectionString))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT * FROM tb_tabletuser WHERE username = @username AND password = @password";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password); // Note: in production, hash passwords

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Get the values from database
                                string userId = reader["deviceuserid"].ToString();
                                string usernameFromDb = reader["username"].ToString();

                                // Store in static class
                                UserCredentials.UserId = userId;
                                UserCredentials.Username = usernameFromDb;

                                // Launch MainForm
                                var dashboard = new MainForm();
                                dashboard.Show();
                                this.Hide();
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

            if (!isPlaceholderActive && string.IsNullOrWhiteSpace(passwordTextBox.Text))
            {
                passwordTextBox.UseSystemPasswordChar = false;
                passwordTextBox.ForeColor = Color.Gray;
                passwordTextBox.Text = "🔐 Password";
                passwordTextBox.SelectionStart = passwordTextBox.Text.Length;
                isPlaceholderActive = true;
            }
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
            RemovePasswordPlaceholder();
            if (string.IsNullOrWhiteSpace(passwordTextBox.Text))
            {
                SetPasswordPlaceholder();
            }
        }

        private void passwordTextBox_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void Login_Paint(object sender, PaintEventArgs e)
        {
        }
    }
}
