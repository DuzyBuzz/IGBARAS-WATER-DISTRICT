using DocumentFormat.OpenXml.Office.Word;
using IGBARAS_WATER_DISTRICT.Helpers;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IGBARAS_WATER_DISTRICT
{
    public partial class EditWaterMeterForm : Form
    {
        private readonly string _accountno;

        public EditWaterMeterForm(string accountno)
        {
            InitializeComponent();
            _accountno = accountno;
            this.Load += EditWaterMeterForm_Load;
        }

        private async void EditWaterMeterForm_Load(object sender, EventArgs e)
        {
            await LoadConcessionaireInfoAsync();
            firstnameTextBox.Enabled = false;
            lastnameTextBox.Enabled = false;
            miTextBox.Enabled = false;
        }

        private async Task LoadConcessionaireInfoAsync()
        {
            try
            {
                using var conn = new MySqlConnection(DbConfig.ConnectionString);
                await conn.OpenAsync();

                string query = @"SELECT * FROM tb_concessionaire WHERE accountno = @accountno";
                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@accountno", _accountno);

                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    accountnoTextBox.Text = reader["accountno"].ToString();
                    concessionairenoTextBox.Text = reader["concessionaireno"].ToString();
                    districtnoTextBox.Text = reader["districtno"].ToString();
                    concessionairecodeTextBox.Text = reader["concessionairecode"].ToString();
                    zonecodeTextBox.Text = reader["zonecode"].ToString();
                    zoneTextBox.Text = reader["zone"].ToString();
                    servicecodeTextBox.Text = reader["servicecode"].ToString();
                    servicetypeTextBox.Text = reader["servicetype"].ToString();
                    pipesizeTextBox.Text = reader["pipesize"].ToString();
                    servicerateTextBox.Text = reader["servicerate"].ToString();
                    connectionnoTextBox.Text = reader["connectionno"].ToString();
                    dateinstalledPicker.Value = reader["dateinstalled"] is DBNull ? DateTime.Today : Convert.ToDateTime(reader["dateinstalled"]);
                    lastnameTextBox.Text = reader["lastname"].ToString();
                    firstnameTextBox.Text = reader["firstname"].ToString();
                    miTextBox.Text = reader["mi"].ToString();
                    businessnameTextBox.Text = reader["businessname"].ToString();
                    contactnoTextBox.Text = reader["contactno"].ToString();
                    barangayTextBox.Text = reader["barangay"].ToString();
                    barangaycodeTextBox.Text = reader["barangaycode"].ToString();
                    addressTextBox.Text = reader["address"].ToString();
                    routenoTextBox.Text = reader["routeno"].ToString();
                    statusTextBox.Text = reader["status"].ToString();
                    applyBillCheckBox.Checked = Convert.ToBoolean(reader["applybill"]);
                    meternoTextBox.Text = reader["meterno"].ToString();
                    brandTextBox.Text = reader["brand"].ToString();
                    watermetercodeTextBox.Text = reader["watermetercode"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                using var conn = new MySqlConnection(DbConfig.ConnectionString);
                await conn.OpenAsync();

                // ✅ Step 1: Update tb_concessionaire using new accountno
                string updateQuery = @"
            UPDATE tb_concessionaire SET 
                accountno = @accountno,
                concessionaireno = @concessionaireno,
                districtno = @districtno,
                concessionairecode = @concessionairecode,
                zonecode = @zonecode,
                zone = @zone,
                servicecode = @servicecode,
                servicetype = @servicetype,
                pipesize = @pipesize,
                servicerate = @servicerate,
                dateinstalled = @dateinstalled,
                businessname = @businessname,
                contactno = @contactno,
                barangay = @barangay,
                barangaycode = @barangaycode,
                address = @address,
                routeno = @routeno,
                status = @status,
                applybill = @applybill,
                meterno = @meterno,
                brand = @brand,
                watermetercode = @watermetercode
            WHERE accountno = @original_accountno";

                using var cmd = new MySqlCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("@accountno", accountnoTextBox.Text);
                cmd.Parameters.AddWithValue("@concessionaireno", concessionairenoTextBox.Text);
                cmd.Parameters.AddWithValue("@districtno", districtnoTextBox.Text);
                cmd.Parameters.AddWithValue("@concessionairecode", concessionairecodeTextBox.Text);
                cmd.Parameters.AddWithValue("@zonecode", zonecodeTextBox.Text);
                cmd.Parameters.AddWithValue("@zone", zoneTextBox.Text);
                cmd.Parameters.AddWithValue("@servicecode", servicecodeTextBox.Text);
                cmd.Parameters.AddWithValue("@servicetype", servicetypeTextBox.Text);
                cmd.Parameters.AddWithValue("@pipesize", pipesizeTextBox.Text);
                cmd.Parameters.AddWithValue("@servicerate", servicerateTextBox.Text);
                cmd.Parameters.AddWithValue("@dateinstalled", dateinstalledPicker.Value.Date);
                cmd.Parameters.AddWithValue("@businessname", businessnameTextBox.Text);
                cmd.Parameters.AddWithValue("@contactno", contactnoTextBox.Text);
                cmd.Parameters.AddWithValue("@barangay", barangayTextBox.Text);
                cmd.Parameters.AddWithValue("@barangaycode", barangaycodeTextBox.Text);
                cmd.Parameters.AddWithValue("@address", addressTextBox.Text);
                cmd.Parameters.AddWithValue("@routeno", routenoTextBox.Text);
                cmd.Parameters.AddWithValue("@status", statusTextBox.Text);
                cmd.Parameters.AddWithValue("@applybill", applyBillCheckBox.Checked ? 1 : 0);
                cmd.Parameters.AddWithValue("@meterno", meternoTextBox.Text);
                cmd.Parameters.AddWithValue("@brand", brandTextBox.Text);
                cmd.Parameters.AddWithValue("@watermetercode", watermetercodeTextBox.Text);
                cmd.Parameters.AddWithValue("@original_accountno", _accountno); // old accountno

                await cmd.ExecuteNonQueryAsync();

                // ✅ Step 2: Get the latest id in tb_watermeter_history (no filtering by accountno)
                string getLastIdQuery = "SELECT MAX(id) FROM tb_watermeter_history";
                int latestId = 0;

                using (var getIdCmd = new MySqlCommand(getLastIdQuery, conn))
                {
                    var result = await getIdCmd.ExecuteScalarAsync();
                    if (result != DBNull.Value && result != null)
                    {
                        latestId = Convert.ToInt32(result);
                    }
                }

                // ✅ Step 3: Update devicecode in the latest inserted row
                if (latestId > 0)
                {
                    string updateDeviceQuery = @"
                UPDATE tb_watermeter_history 
                SET devicecode = @devicecode 
                WHERE id = @id";

                    using var updateCmd = new MySqlCommand(updateDeviceQuery, conn);
                    updateCmd.Parameters.AddWithValue("@devicecode", UserCredentials.DeviceCode);
                    updateCmd.Parameters.AddWithValue("@id", latestId);

                    await updateCmd.ExecuteNonQueryAsync();
                }

                MessageBox.Show("Water meter information updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Update failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



    }
}
