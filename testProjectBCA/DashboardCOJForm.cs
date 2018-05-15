using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.Wpf;
using System.Data.SqlClient;

namespace testProjectBCA
{
    public partial class DashboardCOJForm : Form
    {
        public DashboardCOJForm()
        {
            InitializeComponent();
            reloadTahun();
            reloadBulan();
            reloadTanggal();
            reloadUbVsUk();
            reloadStokSaldoCoj();
        }

        public void reloadTahun()
        {
            List<String> tahun = new List<String>();

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select distinct year(tanggal) from StokPosisi order by year(tanggal) desc";
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        tahun.Add(reader[0].ToString());
                    }
                }
            }
            comboTahun.DataSource = tahun;
        }

        public void reloadBulan()
        {
            List<String> bulan = new List<String>();

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select distinct month(tanggal) from StokPosisi  order by month(tanggal) asc";
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        bulan.Add(reader[0].ToString());
                    }
                }
            }
            comboBulan.DataSource = bulan;
        }

        public void reloadTanggal()
        {
            List<String> tanggal = new List<String>();

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select distinct day(tanggal) from StokPosisi  order by day(tanggal) asc";
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        tanggal.Add(reader[0].ToString());
                    }
                }
            }
            comboTanggal.DataSource = tanggal;
        }

        public void reloadStokSaldoCoj()
        {
            pieChartStokSaldoCoj.Series.Clear();

            Int64 unprocessed = 0;
            Int64 fit = 0;
            Int64 unfit = 0;
            Int64 mayorminor = 0;
            Int64 gress = 0;

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select unprocessed = sum(unprocessed)," +
                                      " fit = sum(fitBaru+fitBaru+fitNKRI)," +
                                      " unfit = sum(unfitBaru+unfitLama+unfitNKRI), " +
                                      " [mayor minor] = sum(RRMBaru + RRMLama + RRMNKRI + RupiahRusakMayor)," +
                                      " gress = sum(newBaru + newLama) " +
                                      " from StokPosisi " +
                                      " where month(tanggal) = 4 and year(tanggal) = 2018 and day(tanggal) = 11";
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        unprocessed = Int64.Parse(reader[0].ToString());
                        fit = Int64.Parse(reader[1].ToString());
                        unfit = Int64.Parse(reader[2].ToString());
                        mayorminor = Int64.Parse(reader[3].ToString());
                        gress = (Int64.Parse(reader[4].ToString()));
                    }

                    Func<ChartPoint, string> labelPoint = chartPoint =>
                    string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);

                    pieChartStokSaldoCoj.Series = new SeriesCollection
                    {
                        new PieSeries
                        {
                            Title = "Unprocessed",
                            Values = new ChartValues<Int64>
                            {
                                unprocessed
                            },
                            DataLabels = true,
                            LabelPoint = labelPoint
                        },
                        new PieSeries
                        {
                            Title = "Fit",
                            Values = new ChartValues<Int64>
                            {
                                fit
                            },
                            DataLabels = true,
                            LabelPoint = labelPoint
                        },
                        new PieSeries
                        {
                            Title = "unfit",
                            Values = new ChartValues<Int64>
                            {
                                unfit
                            },
                            DataLabels = true,
                            LabelPoint = labelPoint
                        },
                        new PieSeries
                        {
                            Title = "Mayor Minor",
                            Values = new ChartValues<Int64>
                            {
                                mayorminor
                            },
                            DataLabels = true,
                            LabelPoint = labelPoint
                        },
                        new PieSeries
                        {
                            Title = "Gress",
                            Values = new ChartValues<Int64>
                            {
                                gress
                            },
                            DataLabels = true,
                            LabelPoint = labelPoint
                        }
                    };

                    pieChartStokSaldoCoj.LegendLocation = LegendLocation.Bottom;

                }
            }
        }

        public void reloadUbVsUk()
        {
            pieChartUbVsUk.Series.Clear();

            Int64 uk = 0;
            Int64 ub = 0;


            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select[uangbesar],[uangkecil]"
                                     + " from"
                                     + " (select[uangbesar] = sum(unprocessed + newBaru + newLama + fitBaru + fitLama + passThrough + unfitBaru + unfitNKRI + unfitLama + RRMBaru + RRMNKRI + RRMLama + RupiahRusakMayor)"
                                     + " from StokPosisi"
                                     + " where (denom = 100000 or denom = 50000) and day(tanggal) = 2 and month(tanggal) = 4 and year(tanggal) = 2018)a,"
                                     + " (select[uangkecil] = sum(unprocessed + newBaru + newLama + fitBaru + fitLama + passThrough + unfitBaru + unfitNKRI +  unfitLama + RRMBaru+ RRMNKRI + RRMLama + RupiahRusakMayor)"
                                     + " from StokPosisi"
                                     + " where(denom != 100000 or denom != 50000) and day(tanggal) = 2 and month(tanggal) = 4 and year(tanggal) = 2018)b";

                   SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        ub = Int64.Parse(reader[0].ToString());
                        uk = Int64.Parse(reader[1].ToString());
                    }

                    Func<ChartPoint, string> labelPoint = chartPoint =>
                   string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);

                    pieChartUbVsUk.Series = new SeriesCollection
                    {
                        new PieSeries
                        {
                            Title = "Uang Besar",
                            Values = new ChartValues<Int64>
                            {
                                ub
                            },
                            DataLabels = true,
                            LabelPoint = labelPoint
                        },
                        new PieSeries
                        {
                            Title = "Uang Kecil",
                            Values = new ChartValues<Int64>
                            {
                                uk
                            },
                            DataLabels = true,
                            LabelPoint = labelPoint
                        }
                    };

                    pieChartStokSaldoCoj.LegendLocation = LegendLocation.Bottom;
                }
            }
        }

        private void comboTahun_SelectionChangeCommitted(object sender, EventArgs e)
        {
          
        }

        private void comboBulan_SelectionChangeCommitted(object sender, EventArgs e)
        {
          
        }

        private void comboTanggal_SelectionChangeCommitted(object sender, EventArgs e)
        {

        }
    }
}
