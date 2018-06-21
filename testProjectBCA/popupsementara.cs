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
using System.Windows.Media;
using System.Data.SqlClient;
using FastMember;
using System.Windows.Forms.VisualStyles;
using System.Windows;
using System.Globalization;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;

namespace testProjectBCA
{
    public partial class popupsementara : Form
    {
        public popupsementara()
        {
            InitializeComponent();
          
            List<String> combo6 = new List<String>();
            List<String> combo7 = new List<String>();
            List<String> comboarea = new List<String>();

            comboarea.Add("Nasional");
            comboarea.Add("Jabo");
            comboarea.Add("Non Jabo");
            comboarea.Add("Jabo E2E");
            comboarea.Add("Jabo Non E2E");
            comboarea.Add("Non Jabo E2E");
            comboarea.Add("Non Jabo Non E2E");
            comboArea.DataSource = comboarea;
            comboArea.SelectedIndex = 0;

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "SELECT DISTINCT[Tahun] = YEAR(tanggal) FROM TransaksiAtms ORDER BY Tahun DESC";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        combo6.Add(reader[0].ToString());
                    }
                    comboTahun3.DataSource = combo6;
                }
            }

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "SELECT DISTINCT[Bulan] = MONTH(tanggal) FROM TransaksiAtms ORDER BY Bulan";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        combo7.Add(reader[0].ToString());
                    }
                    comboBulan3.DataSource = combo7;
                }
            }

            reload3();
            
        }

        public void reload3()
        {
            cartesianChart3.AxisX.Clear();
            cartesianChart3.AxisY.Clear();
            cartesianChart3.Series.Clear();

            List<Int64> saldo = new List<Int64>();
            List<Int64> bon = new List<Int64>();
            List<Int64> adhoc = new List<Int64>();
            List<Int64> isi = new List<Int64>();
            List<Int64> isiCrm = new List<Int64>();
            List<String> hari = new List<String>();

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select saldo = sum(saldoAwal20 + saldoAwal50 + saldoAwal100), bon = SUM(bon20 + bon50 + bon100), adhoc = SUM(adhoc100+ adhoc50+adhoc20), isiATM = SUM(isiATM100 + isiATM50 + isiATM20) ,hari= DAY(tanggal), isiCRM = SUM(isiCRM100+ isiCRM50+ isiCRM20) from TransaksiAtms ta join Pkt p on ta.kodePkt = p.kodePkt where year(tanggal) = " + comboTahun3.SelectedValue.ToString() + areaChoose(comboArea.SelectedValue.ToString()) + " and month(tanggal) = " + comboBulan3.SelectedValue.ToString() + " group by DAY(tanggal) order by day(tanggal)";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        saldo.Add((Int64)reader[0]);
                        bon.Add((Int64)reader[1]);
                        adhoc.Add((Int64)reader[2]);
                        isi.Add((Int64)reader[3]);
                        hari.Add(reader[4].ToString());
                        isiCrm.Add((Int64)reader[5]);
                    }

                    ChartValues<String> cv = new ChartValues<String>();
                    foreach (String temp in hari)
                    {
                        cv.Add(temp);
                    }
                    ChartValues<Int64> cv2 = new ChartValues<Int64>();
                    foreach (Int64 temp in isi)
                    {
                        cv2.Add(temp);
                    }
                    ChartValues<Int64> cv5 = new ChartValues<Int64>();
                    foreach (Int64 temp in saldo)
                    {
                        cv5.Add(temp);
                    }
                    ChartValues<Int64> cv6 = new ChartValues<Int64>();
                    foreach (var temp in isiCrm)
                        cv6.Add(temp);
                    cartesianChart3.Series = new SeriesCollection
                    {
                        new ColumnSeries
                        {
                            Title = "Saldo",
                            Values = cv5,
                            
                            //DataLabels = true,
                            LabelPoint = p=>(Math.Round((p.Y/1000000000000),1)).ToString() + "T",
                            Foreground = System.Windows.Media.Brushes.Black,
                            DataLabels = true,
                            FontSize = 10,
                            LabelsPosition = BarLabelPosition.Top
                        },

                        new LineSeries
                        {
                            Title = "Isi ATM",
                            Values = cv2,
                            LabelPoint = p=>(Math.Round((p.Y/1000000000000),1)).ToString() + "T",
                            LineSmoothness = 0,
                            DataLabels = true,
                            Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255,0,0))

                        },
                        new LineSeries
                        {
                            Title = "Isi CRM",
                            Values = cv6,
                            LabelPoint = p=>(Math.Round((p.Y/1000000000),2)).ToString() + "M",
                            LineSmoothness = 0,
                            DataLabels = true,
                            Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0,0,0)),
                            FontSize = 8

                        },

                    };


                    cartesianChart3.AxisX.Add(new Axis
                    {
                        Title = "Saldo ATM Nasional",
                        Labels = hari,
                        //Separator = DefaultAxes.CleanSeparator,
                        Separator = new Separator // force the separator step to 1, so it always display all labels
                        {

                            Step = 1,
                            IsEnabled = false //disable it to make it invisible.
                        }

                    });

                    cartesianChart3.AxisY.Add(new Axis
                    {
                        Title = "Jumlah",
                        LabelFormatter = value => (value / 1000000000000).ToString() + "T",
                        MinValue = 0
                    });

                    //cartesianChart3.AxisY.Add(new Axis
                    //{
                    //    Title = "Jumlah",
                    //    LabelFormatter = value => value.ToString(),
                    //    MinValue = 0,
                    //    Position =AxisPosition.RightTop
                    //});

                    cartesianChart3.LegendLocation = LegendLocation.Right;

                    //example

                }
            }
        }

        private void comboTahun3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            reload3();
        }

        private void comboBulan3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            reload3();
        }

        private void comboArea_SelectionChangeCommitted(object sender, EventArgs e)
        {
            areaChoose(comboArea.SelectedIndex.ToString());
            
            reload3();
           
        }

        public String areaChoose(String areaMasuk)
        {
            String area = areaMasuk;
            String area2 = "";
            if (area.Equals("Nasional"))
            {
                area2 = "";
            }
            else if (area.Equals("Jabo"))
            {
                area2 = "and p.kanwil like 'Jabo%'";
            }
            else if (area.Equals("Non Jabo"))
            {
                area2 = "and  p.kanwil not like 'Jabo%'";
            }
            else if (area.Equals("Jabo E2E"))
            {
                area2 = "and p.e2e= 'E2E' and p.kanwil like 'Jabo%'";
            }
            else if (area.Equals("Jabo Non E2E"))
            {
                area2 = "and p.e2e= 'Non E2E' and p.kanwil like 'Jabo%'";
            }
            else if (area.Equals("Non Jabo E2E"))
            {
                area2 = "and p.e2e= 'E2E' and p.kanwil not like 'Jabo%'";
            }
            else if (area.Equals("Non Jabo Non E2E"))
            {
                area2 = "and p.e2e= 'Non E2E' and p.kanwil not like 'Jabo%'";
            }
            return area2;
        }

    }
}
