using IGBARAS_WATER_DISTRICT.Helpers;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace IGBARAS_WATER_DISTRICT
{
    public partial class ReportsControl : UserControl
    {
        public ReportsControl()
        {
            InitializeComponent();
        }

        private DataTable billingDataTable = new DataTable();

        private async void ReportsControl_Load(object sender, EventArgs e)
        {
            using (var loadingForm = new LoadingForm())
            {
                billingDataTable = await DGVHelper.LoadDataToDataTableAsync("tb_bill", loadingForm);
            }
            // Billing Summary
            LoadBillingSummary(dailyBillingReportDataGridView, dailyBillingReportChart, "daily");
            LoadBillingSummary(monthlyBillingReportDataGridView, monthlyBillingReportChart, "monthly");
            LoadBillingSummary(yearlyBillingReportDataGridView, yearlyBillingReportChart, "yearly");

            // Collection Summary
            LoadCollectionSummary(dailyCollectionReportDataGridView, dailyCollectionReportChart, "daily");
            LoadCollectionSummary(monthlyCollectionReportDataGridView, monthlyCollectionReportChart, "monthly");
            LoadCollectionSummary(yearlyCollectionReportDataGridView, yearlyCollectionReportChart, "yearly");

            LoadBillingPerZoneChart();
        }

        /// <summary>
        /// Loads a billing chart into the given Chart control using data from a DataGridView.
        /// Supports grouping by "Yearly", "Monthly", or "Daily".
        /// </summary>
        /// <param name="sourceGrid">The source DataGridView with billing data.</param>
        /// <param name="targetChart">The Chart control to draw the data.</param>
        /// <param name="groupBy">"Yearly", "Monthly", or "Daily"</param>
        /// <param name="title">Title of the chart</param>
        public void LoadBillingSummary(DataGridView grid, Chart chart, string groupBy)
        {
            // Ensure the table is not empty
            if (billingDataTable == null || billingDataTable.Rows.Count == 0)
            {
                MessageBox.Show("No billing data available.");
                return;
            }

            // 🧠 Group data by: Daily, Monthly, or Yearly
            var groupedData = billingDataTable.AsEnumerable()
                .Where(row => !row.IsNull("datebilled"))
                .GroupBy(row =>
                {
                    var date = Convert.ToDateTime(row["datebilled"]);
                    return groupBy.ToLower() switch
                    {
                        "daily" => date.ToString("yyyy-MM-dd"),
                        "monthly" => date.ToString("yyyy-MM"),
                        "yearly" => date.Year.ToString(),
                        _ => date.ToString("yyyy-MM")
                    };
                })
                .Select(g => new
                {
                    Period = g.Key,
                    BillCount = g.Count(),
                    TotalCharges = g.Sum(r => Convert.ToDouble(r["totalbillcharge"])),
                    TotalPaid = g.Sum(r => Convert.ToDouble(r["amountpaid"])),
                    TotalBalance = g.Sum(r => Convert.ToDouble(r["balance"]))
                })
                .OrderByDescending(x => x.Period)
                .ToList();

            // ⬇️ Load into DataGridView
            DataTable resultTable = new DataTable();
            resultTable.Columns.Add(groupBy switch
            {
                "daily" => "Billing Date",
                "monthly" => "Billing Month",
                "yearly" => "Billing Year",
                _ => "Period"
            });
            resultTable.Columns.Add("Number of Bills", typeof(int));
            resultTable.Columns.Add("Total Charges", typeof(string));  // formatted string
            resultTable.Columns.Add("Total Paid", typeof(string));
            resultTable.Columns.Add("Total Balance", typeof(string));

            foreach (var row in groupedData)
            {
                resultTable.Rows.Add(
                    row.Period,
                    row.BillCount,
                    row.TotalCharges.ToString("₱#,##0.00"),
                    row.TotalPaid.ToString("₱#,##0.00"),
                    row.TotalBalance.ToString("₱#,##0.00")
                );
            }

            // 👀 Assign to grid
            grid.DataSource = resultTable;

            // 📊 Load chart
            chart.Series.Clear();
            chart.ChartAreas.Clear();
            chart.Titles.Clear();
            chart.Legends.Clear();

            ChartArea area = new ChartArea();
            area.AxisX.Title = groupBy switch
            {
                "daily" => "Date",
                "yearly" => "Year",
                _ => "Month"
            };
            area.AxisY.Title = "Amount (₱)";
            area.AxisX.LabelStyle.Angle = -45;
            area.AxisY.LabelStyle.Format = "₱#,##0";
            chart.ChartAreas.Add(area);

            Series chargeSeries = new Series("Total Charges") { ChartType = SeriesChartType.Column, Color = Color.SteelBlue };
            Series paidSeries = new Series("Total Paid") { ChartType = SeriesChartType.Column, Color = Color.SeaGreen };
            Series balanceSeries = new Series("Total Balance") { ChartType = SeriesChartType.Column, Color = Color.IndianRed };

            foreach (var row in groupedData)
            {
                chargeSeries.Points.AddXY(row.Period, row.TotalCharges);
                paidSeries.Points.AddXY(row.Period, row.TotalPaid);
                balanceSeries.Points.AddXY(row.Period, row.TotalBalance);
            }

            chart.Series.Add(chargeSeries);
            chart.Series.Add(paidSeries);
            chart.Series.Add(balanceSeries);

            chart.Titles.Add(new Title($"{groupBy} Billing Summary", Docking.Top, new Font("Arial", 12, FontStyle.Bold), Color.Black));

            chart.Legends.Add(new Legend
            {
                Docking = Docking.Bottom,
                Font = new Font("Arial", 9),
                Alignment = StringAlignment.Center
            });
        }

        public void LoadCollectionSummary(DataGridView grid, Chart chart, string groupBy)
        {
            if (billingDataTable == null || billingDataTable.Rows.Count == 0)
            {
                MessageBox.Show("No data loaded.");
                return;
            }

            var filteredRows = billingDataTable.AsEnumerable()
                .Where(row =>
                    !row.IsNull("datebilled") &&
                    !row.IsNull("amountpaid") &&
                    Convert.ToDouble(row["amountpaid"]) > 0)
                .ToList();

            if (filteredRows.Count == 0)
            {
                MessageBox.Show("No payment data found.");
                return;
            }

            var groupedData = filteredRows
                .GroupBy(row =>
                {
                    DateTime date = Convert.ToDateTime(row["datebilled"]);
                    return groupBy.ToLower() switch
                    {
                        "daily" => date.ToString("yyyy-MM-dd"),
                        "monthly" => date.ToString("yyyy-MM"),
                        "yearly" => date.ToString("yyyy"),
                        _ => date.ToString("yyyy-MM")
                    };
                })
                .Select(g => new
                {
                    Period = g.Key,
                    PaymentCount = g.Count(),
                    TotalCollected = g.Sum(r => Convert.ToDouble(r["amountpaid"]))
                })
                .OrderByDescending(x => x.Period)
                .ToList();

            // 🔁 Fill DataGridView
            var dt = new DataTable();
            string periodHeader = groupBy.ToLower() switch
            {
                "daily" => "Collection Date",
                "monthly" => "Collection Month",
                "yearly" => "Collection Year",
                _ => "Period"
            };

            dt.Columns.Add(periodHeader);
            dt.Columns.Add("Number of Payments", typeof(int));
            dt.Columns.Add("Total Collected", typeof(string)); // formatted

            foreach (var item in groupedData)
            {
                dt.Rows.Add(item.Period, item.PaymentCount, item.TotalCollected.ToString("₱#,##0.00"));
            }

            grid.DataSource = dt;

            // 📊 Chart setup
            chart.Series.Clear();
            chart.ChartAreas.Clear();
            chart.Titles.Clear();
            chart.Legends.Clear();

            ChartArea area = new ChartArea("CollectionArea");
            area.AxisX.Title = periodHeader;
            area.AxisY.Title = "Collected (₱)";
            area.AxisX.LabelStyle.Angle = -45;
            area.AxisY.LabelStyle.Format = "₱#,##0";
            chart.ChartAreas.Add(area);

            Series series = new Series("Total Collected")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.MediumSeaGreen,
                Font = new Font("Arial", 9)
            };

            foreach (var item in groupedData)
            {
                series.Points.AddXY(item.Period, item.TotalCollected);
            }

            chart.Series.Add(series);
            chart.Titles.Add(new Title($"{groupBy} Collection Summary", Docking.Top, new Font("Arial", 12, FontStyle.Bold), Color.Black));

            chart.Legends.Add(new Legend
            {
                Docking = Docking.Bottom,
                Font = new Font("Arial", 9),
                Alignment = StringAlignment.Center
            });
        }




        private void LoadBillingPerZoneChart()
        {
            // 🔄 Reset chart
            billingPerZoneChart.Series.Clear();
            billingPerZoneChart.ChartAreas.Clear();
            billingPerZoneChart.Titles.Clear();
            billingPerZoneChart.Legends.Clear();

            // 📊 Chart Area
            ChartArea area = new ChartArea("PieArea");
            area.BackColor = Color.White;
            area.Area3DStyle.Enable3D = true;
            billingPerZoneChart.ChartAreas.Add(area);

            // 🧾 Title
            billingPerZoneChart.Titles.Add("Billing Charges per Zone");
            billingPerZoneChart.Titles[0].Font = new Font("Arial", 14, FontStyle.Bold);
            billingPerZoneChart.Titles[0].ForeColor = Color.Black;

            // 🧾 Legend
            Legend legend = new Legend("Legend");
            legend.Docking = Docking.Right;
            legend.Font = new Font("Arial", 9, FontStyle.Regular);
            legend.IsTextAutoFit = false;
            legend.ForeColor = Color.Black;
            billingPerZoneChart.Legends.Add(legend);

            // 📌 Pie Series Setup
            Series pieSeries = new Series("Zone Billing")
            {
                ChartType = SeriesChartType.Pie,
                Font = new Font("Arial", 9),
                IsValueShownAsLabel = true,
                LabelForeColor = Color.Black,
                BorderColor = Color.White,
                BorderWidth = 1,
            };

            // Format and Group Data
            var summary = billingDataTable.AsEnumerable()
                .Where(row => !row.IsNull("zonecode") && !row.IsNull("totalbillcharge") && !row.IsNull("balance"))
                .GroupBy(row => row["zonecode"].ToString())
                .Select(g => new
                {
                    Zone = g.Key,
                    TotalCharges = g.Sum(r => Convert.ToDecimal(r["totalbillcharge"])),
                    Outstanding = g.Sum(r => Convert.ToDecimal(r["balance"]))
                })
                .OrderBy(z => z.Zone)
                .ToList();

            foreach (var zone in summary)
            {
                // 📎 Format amounts
                string formattedCharge = $"₱{zone.TotalCharges:N2}";
                string formattedOutstanding = $"₱{zone.Outstanding:N2}";

                // ➕ Add data point
                DataPoint dp = new DataPoint
                {
                    AxisLabel = zone.Zone, // 🔹 Label is just zone code (e.g., "001")
                    YValues = new[] { (double)zone.TotalCharges },
                    Label = zone.Zone,
                    ToolTip = $"Zone {zone.Zone}"
                };

                // 🗒️ Add detailed legend entry
                dp.LegendText = $"\nZone {zone.Zone} - {formattedCharge}\n" +
                    $"(Outstanding: {formattedOutstanding})";

                pieSeries.Points.Add(dp);
            }

            // 🔧 Pie Style Settings
            pieSeries["PieLabelStyle"] = "Outside";
            pieSeries["PieDrawingStyle"] = "SoftEdge";
            pieSeries["CollectedThreshold"] = "1"; // combine very small zones

            billingPerZoneChart.Series.Add(pieSeries);
        }




        private void billingReportChart_Click(object sender, EventArgs e)
        {

        }
        private DateTime SafeToDateTime(object dateObj)
        {
            return dateObj is MySql.Data.Types.MySqlDateTime mysqlDate
                ? mysqlDate.GetDateTime()
                : Convert.ToDateTime(dateObj);
        }

    }
}
