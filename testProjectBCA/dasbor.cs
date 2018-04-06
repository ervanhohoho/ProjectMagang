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

namespace testProjectBCA
{
    public partial class dasbor : Form
    {
        String [] bulan = {"Jan","Feb","Mar","Apr","Mei","Jun","Jul","Ags","Sep","Okt","Nov","Des"};
        public dasbor()
        {
            InitializeComponent();
            Database1Entities en = new Database1Entities();
           
        }

        private void dasbor_Load(object sender, EventArgs e)
        {

            List<String> comboarea = new List<String>();
            List<String> combomode = new List<String>();



            comboarea.Add("Nasional");
            comboarea.Add("Jabo");
            comboarea.Add("Non Jabo");
            comboarea.Add("Jabo E2E");
            comboarea.Add("Jabo Non E2E");
            comboarea.Add("Non Jabo E2E");
            comboarea.Add("Non Jabo Non E2E");
            comboArea.DataSource = comboarea;
            comboArea.SelectedIndex = 0;
            combomode.Add("Rasio");
            combomode.Add("Frekuensi AdHoc");
            comboMode7.DataSource = combomode;
            comboMode7.SelectedIndex = 0;

            List<String> combo1 = new List<String>();
            List<String> combo2 = new List<String>();
            List<String> combo3 = new List<String>();
            List<String> combo4 = new List<String>();
            List<String> combo5 = new List<String>();
            List<String> combo6 = new List<String>();
            List<String> combo7 = new List<String>();
            List<String> combo8 = new List<String>();
            List<String> combo9 = new List<String>();
            List<String> combo10 = new List<String>();
            List<String> combo11 = new List<String>();
            List<String> combo12 = new List<String>();
            List<String> combo13 = new List<String>();
            List<String> combo14 = new List<String>();
            List<String> combo15 = new List<String>();
            List<String> combo16 = new List<String>();
            List<String> combo17 = new List<String>();
            List<String> combo18 = new List<String>();
            List<String> combo19= new List<String>();



            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "SELECT DISTINCT[Tahun] = YEAR(tanggal) FROM TransaksiAtms ORDER BY Tahun";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        combo1.Add(reader[0].ToString());
                    }
                    comboAvg1.DataSource = combo1;
                }
            } 

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
                        combo2.Add(reader[0].ToString());
                    }
                    comboTahun1.DataSource = combo2;
                }
            }

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "SELECT DISTINCT[Tahun] = YEAR(tanggal) FROM TransaksiAtms ORDER BY Tahun";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        combo3.Add(reader[0].ToString());
                    }
                    comboTahun2.DataSource = combo3;
                }
            } 

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "SELECT DISTINCT[Tahun] = YEAR(tanggal) FROM TransaksiAtms ORDER BY Tahun";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        combo4.Add(reader[0].ToString());
                    }
                    comboTambah1.DataSource = combo4;
                }
            }

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "SELECT DISTINCT[Tahun] = YEAR(tanggal) FROM TransaksiAtms ORDER BY Tahun";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        combo5.Add(reader[0].ToString());
                    }
                    comboTambah2.DataSource = combo5;
                }
            }

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "SELECT DISTINCT[Tahun] = YEAR(tanggal) FROM TransaksiAtms ORDER BY Tahun";
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

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "SELECT DISTINCT[Tahun] = YEAR(tanggal) FROM TransaksiAtms ORDER BY Tahun";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        combo8.Add(reader[0].ToString());
                    }
                    comboTahun4.DataSource = combo8;
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
                        combo9.Add(reader[0].ToString());
                    }
                    comboBulan4.DataSource = combo9;
                }
            }

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "SELECT DISTINCT Kanwil FROM Pkt";
                    SqlDataReader reader = cmd.ExecuteReader();
                    combo10.Add("Nasional");
                    while (reader.Read())
                    {
                        combo10.Add(reader[0].ToString());
                    }
                    comboKanwil5.DataSource = combo10;
                }
            }

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "SELECT DISTINCT[Tahun] = YEAR(tanggal) FROM TransaksiAtms ORDER BY Tahun";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        combo11.Add(reader[0].ToString());
                    }
                    comboTahun5.DataSource = combo11;
                }
            }

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "SELECT DISTINCT Kanwil FROM Pkt";
                    SqlDataReader reader = cmd.ExecuteReader();
                    combo12.Add("Nasional");
                    while (reader.Read())
                    {
                        combo12.Add(reader[0].ToString());
                    }
                    comboKanwil6.DataSource = combo12;
                }
            }

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "SELECT DISTINCT[Tahun] = YEAR(tanggal) FROM TransaksiAtms ORDER BY Tahun";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        combo13.Add(reader[0].ToString());
                    }
                    comboTahun6_1.DataSource = combo13;
                }
            }

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "SELECT DISTINCT[Tahun] = YEAR(tanggal) FROM TransaksiAtms ORDER BY Tahun";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        combo14.Add(reader[0].ToString());
                    }
                    comboTahun6_2.DataSource = combo14;
                }
            }

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "SELECT DISTINCT Vendor FROM Pkt";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        combo15.Add(reader[0].ToString());
                    }
                    comboVendor7.DataSource = combo15;
                }
            }

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "SELECT DISTINCT[Tahun] = YEAR(tanggal) FROM TransaksiAtms ORDER BY Tahun";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        combo16.Add(reader[0].ToString());
                    }
                    comboTahun7.DataSource = combo16;
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
                        combo17.Add(reader[0].ToString());
                    }
                    comboBulan7.DataSource = combo17;
                }
            }

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "SELECT DISTINCT[Tahun] = YEAR(tanggal) FROM TransaksiAtms ORDER BY Tahun";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        combo18.Add(reader[0].ToString());
                    }
                    comboTahun8.DataSource = combo18;
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
                        combo19.Add(reader[0].ToString());
                    }
                    comboBulan8.DataSource = combo19;
                }
            }



            reloadComboPkt7();

            reload3();
            reload4();
            reload5();
            reload6();
            reload7();
            reload8();
            reload9();
            reload10();
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

        public String kanwilChoose(String kanwilMasuk)
        {
            String a = kanwilMasuk;
            String b = "";

            if (a.Equals("Jabotabek"))
            {
                b = "and p.kanwil like 'Jabo%'";
            }
            else if (a.Equals("Kanwil I"))
            {
                b = "and p.kanwil = 'Kanwil I'";
            }
            else if (a.Equals("Kanwil II"))
            {
                b = "and p.kanwil = 'Kanwil II'";
            }
            else if (a.Equals("Kanwil III"))
            {
                b = "and p.kanwil = 'Kanwil III'";
            }
            else if (a.Equals("Kanwil IV"))
            {
                b = "and p.kanwil = 'Kanwil IV'";
            }
            else if (a.Equals("Kanwil V"))
            {
                b = "and p.kanwil = 'Kanwil V'";
            }
            else if (a.Equals("Kanwil VI"))
            {
                b = "and p.kanwil = 'Kanwil VI'";
            }
            else if (a.Equals("Kanwil VII"))
            {
                b = "and p.kanwil = 'Kanwil VII'";
            }
            else if (a.Equals("Kanwil XI"))
            {
                b = "and p.kanwil = 'Kanwil XI'";
            }
            else if (a.Equals("Nasional"))
            {
                b = "";
            }

            return b;
        }

        public String kanwilChooseGridView(String kanwilMasuk)
        {
            String a = kanwilMasuk;
            String b = "";

            if (a.Equals("Nasional"))
            {
                b = "select distinct upper( vendor ) from Pkt";
            }
            else if (a.Equals("Jabotabek"))
            {
                b = "select kodePkt from Pkt where kanwil like 'Jabo%'";
            }
            else if (a.Equals("Kanwil I"))
            {
                b = "select kodePkt from Pkt where kanwil like 'Kanwil I'";
            }
            else if (a.Equals("Kanwil II"))
            {
                b = "select kodePkt from Pkt where kanwil like 'Kanwil II'";
            }
            else if (a.Equals("Kanwil III"))
            {
                b = "select kodePkt from Pkt where kanwil like 'Kanwil III'";
            }
            else if (a.Equals("Kanwil IV"))
            {
                b = "select kodePkt from Pkt where kanwil like 'Kanwil IV'";
            }
            else if (a.Equals("Kanwil V"))
            {
                b = "select kodePkt from Pkt where kanwil like 'Kanwil V'";
            }
            else if (a.Equals("Kanwil VI"))
            {
                b = "select kodePkt from Pkt where kanwil like 'Kanwil VI'";
            }
            else if (a.Equals("Kanwil VII"))
            {
                b = "select kodePkt from Pkt where kanwil like 'Kanwil VII'";
            }
            else if (a.Equals("Kanwil XI"))
            {
                b = "select kodePkt from Pkt where kanwil like 'Kanwil XI'";
            }

            return b;
        }

        public String tahunPlusKanwilChoose_1(String kanwilMasuk, String tahunMasuk)
        {
            String a = kanwilMasuk;
            String b = tahunMasuk;
            String c = "";

            if (a.Equals("Nasional"))
            {
                c = "SELECT  distinct vendor , [Rasio] = CAST(SUM(saldoAwal100 + saldoAwal50 + saldoAwal20) AS FLOAT)/(SUM(isiATM100+isiATM50+isiATM20+isiCRM100+isiCRM50+isiCRM20)) FROM TransaksiAtms ta join Pkt p on ta.kodePkt = p.kodePkt WHERE YEAR(tanggal) = "+ b +" group by  vendor order by vendor";
            }
            else 
            {
                c = "SELECT p.kodePkt , [Rasio] = CAST(SUM(saldoAwal100 + saldoAwal50 + saldoAwal20) AS FLOAT)/(SUM(isiATM100+isiATM50+isiATM20+isiCRM100+isiCRM50+isiCRM20)) FROM TransaksiAtms ta join Pkt p on ta.kodePkt = p.kodePkt WHERE YEAR(tanggal) = " + b + "and kanwil like '"+ a +"' group by p.kodePkt order by p.kodePkt";
            }

            return c;
        }

        public String tahunPlusKanwilChoose_2(String kanwilMasuk, String tahunMasuk)
        {
            String a = kanwilMasuk;
            String b = tahunMasuk;
            String c = "";

            if (a.Equals("Nasional"))
            {
                c = "SELECT DISTINCT pkt.vendor, ISNULL([Jumlah adhoc],0)	                            "
                    +"FROM Pkt LEFT JOIN                                                                 "
                    +"(                                                                                  "
                    +"	select distinct vendor, [Jumlah adhoc] = count (adhoc100+adhoc50+adhoc20)       "
                    +"	from TransaksiAtms t join Pkt p on t.kodePkt = p.kodePkt                        "
                    +"	where ((adhoc100 + adhoc50 + adhoc20) != 0)  and YEAR(tanggal) = "+b+"           "
                    +"	group by  vendor                                                                "
                    +")t ON pkt.vendor = t.vendor";
            }
            else
            {
                c = "SELECT DISTINCT pkt.kodePkt, ISNULL([Jumlah adhoc],0)                                                      "
                    +"FROM Pkt LEFT JOIN                                                                                         "
                    +"(                                                                                                          "
                    +"	select distinct p.kodePkt, [Jumlah adhoc] = count (adhoc100+adhoc50+adhoc20)                            "
                    +"	from TransaksiAtms t join Pkt p on t.kodePkt = p.kodePkt                                                "
                    +"	where ((adhoc100 + adhoc50 + adhoc20) != 0)  and YEAR(tanggal) = "+b+" and p.kanwil like '"+a+"'     "
                    +"	group by  p.kodePkt                                                                                     "
                    +")t ON pkt.kodePkt = t.kodePkt                                                                              "
                    +"where pkt.kanwil like '"+a+"'";                                                                        
            }

            return c;
        }

        public void reloadComboPkt7()
        {
            List<String> combo16 = new List<String>();

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "SELECT KodePkt FROM Pkt WHERE Vendor like '" + comboVendor7.SelectedValue.ToString() + "' ";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        combo16.Add(reader[0].ToString());
                    }
                    comboPkt7.DataSource = combo16;
                }
            }
        }

        public void reload()
        {
            cartesianChart1.AxisX.Clear();
            cartesianChart1.AxisY.Clear();
            cartesianChart1.Series.Clear();
            
            List<Double> rasio = new List<Double>();
            List<String> bulan = new List<String>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "SELECT [Bulan] = MONTH(tanggal),[Rasio] = CAST(SUM(saldoAwal100 + saldoAwal50 + saldoAwal20) AS FLOAT)/(SUM(isiATM100+isiATM50+isiATM20+isiCRM100+isiCRM50+isiCRM20)) FROM TransaksiAtms ta join Pkt p on ta.kodePkt = p.kodePkt WHERE YEAR(tanggal) = " + comboTahun1.SelectedValue.ToString() + areaChoose(comboArea.SelectedValue.ToString()) + " GROUP BY MONTH(tanggal), YEAR(tanggal) ORDER BY Bulan ";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        bulan.Add(reader[0].ToString());
                        rasio.Add((Double)reader[1]);

                    }
                    ChartValues<Double> cv = new ChartValues<Double>();
                    foreach (Double temp in rasio)
                    {
                        cv.Add(Math.Round(temp,2));
                    }
                    cartesianChart1.Series = new SeriesCollection
                    {
                        new LineSeries
                        {
                            Title = comboTahun1.Text,
                            Values = cv,
                            LineSmoothness = 0
                        }
                    };
                    cartesianChart1.AxisX.Add(new Axis
                    {
                        Title = "Bulan",
                        Labels = bulan,
                        Separator = new Separator
                        {
                            Step = 1
                        }
                        //Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" }
                    });

                    cartesianChart1.AxisY.Add(new Axis
                    {
                        Title = "Rasio",
                        LabelFormatter = value => value.ToString()
                    });

                    cartesianChart1.LegendLocation = LegendLocation.Right;

                }
            }
        }

        public void reload2()
        {
            cartesianChart2.AxisX.Clear();
            cartesianChart2.AxisY.Clear();
            cartesianChart2.Series.Clear();

            List<Double> persen = new List<Double>();
            List<String> bulan = new List<String>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select [bulan] = month(tanggal),[persen] = cast(sum(adhoc100+adhoc50+adhoc20)as float)/ cast(sum(adhoc100+adhoc50+adhoc20+bon100+bon50+bon20) as float) from TransaksiAtms ta join Pkt p on ta.kodePkt = p.kodePkt where year(tanggal) = " + comboTahun2.SelectedValue.ToString()  +areaChoose(comboArea.SelectedValue.ToString()) + " group by MONTH(tanggal) order by[bulan]";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        bulan.Add(reader[0].ToString());
                        persen.Add((Double)reader[1]);

                    }
                    ChartValues<Double> cv = new ChartValues<Double>();
                    foreach (Double temp in persen)
                    {
                        cv.Add(Math.Round(temp, 4) * 100);
                    }
                    cartesianChart2.Series = new SeriesCollection
                    {
                        new LineSeries
                        {
                            Title = comboTahun2.Text,
                            Values = cv,
                            LineSmoothness = 0
                          
                        }
                    };
                    cartesianChart2.AxisX.Add(new Axis
                    {
                        Title = "Bulan",
                        Labels = bulan
                        //Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" }
                    });

                    cartesianChart2.AxisY.Add(new Axis
                    {
                        Title = "Persen",
                        LabelFormatter = value => value.ToString() + " %"
                    });

                    cartesianChart2.LegendLocation = LegendLocation.Right;

                }
            }
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
            List<String> hari = new List<String>();

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select saldo = sum(saldoAwal20 + saldoAwal50 + saldoAwal100), bon = SUM(bon20 + bon50 + bon100), adhoc = SUM(adhoc100+ adhoc50+adhoc20), isi = SUM(isiATM100 + isiATM50 + isiATM20 + isiCRM100+ isiCRM50+ isiCRM20) ,hari= DAY(tanggal) from TransaksiAtms ta join Pkt p on ta.kodePkt = p.kodePkt where year(tanggal) = " + comboTahun3.SelectedValue.ToString() + areaChoose(comboArea.SelectedValue.ToString()) + " and month(tanggal) = " + comboBulan3.SelectedValue.ToString()+" group by DAY(tanggal) order by day(tanggal)";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        saldo.Add((Int64)reader[0]);
                        bon.Add((Int64)reader[1]);
                        adhoc.Add((Int64)reader[2]);
                        isi.Add((Int64)reader[3]);
                        hari.Add(reader[4].ToString());
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
                    ChartValues<Int64> cv3 = new ChartValues<Int64>();
                    foreach (Int64 temp in adhoc)
                    {
                        cv3.Add(temp);
                    }
                    ChartValues<Int64> cv4 = new ChartValues<Int64>();
                    foreach (Int64 temp in bon)
                    {
                        cv4.Add(temp);
                    }
                    ChartValues<Int64> cv5 = new ChartValues<Int64>();
                    foreach (Int64 temp in saldo)
                    {
                        cv5.Add(temp);
                    }

                    cartesianChart3.Series = new SeriesCollection
                    {
                        new StackedColumnSeries
                        {
                            Title = "Saldo",
                            Values = cv5,
                            StackMode = StackMode.Values,
                            //DataLabels = true,
                            LabelPoint = p=>(Math.Round((p.Y/1000000000000),2)).ToString() + "T",
                            Foreground = System.Windows.Media.Brushes.Black
                        },
                        new StackedColumnSeries
                        {
                            Title = "Bon",
                            Values = cv4,
                            StackMode = StackMode.Values,
                            //DataLabels = true,
                            LabelPoint = p=>(Math.Round((p.Y/1000000000000),2)).ToString() + "T",
                            Foreground = System.Windows.Media.Brushes.Black
                        },
                        new StackedColumnSeries
                        {
                            Title = "Ad-hoc",
                            Values = cv3,
                            StackMode = StackMode.Values,
                            //DataLabels = true,
                            LabelPoint = p=>(Math.Round((p.Y/1000000000000),2)).ToString() + "T",
                            Foreground = System.Windows.Media.Brushes.Black
                        },
                        new LineSeries
                        {
                            Title = "Isi",
                            Values = cv2,
                            LabelPoint = p=>(Math.Round((p.Y/1000000000000),2)).ToString() + "T",
                            LineSmoothness = 0
                        },

                    };
                    

                    cartesianChart3.AxisX.Add(new Axis
                    {
                        Title = "Saldo ATM Nasional",
                        Labels = hari,
                        Separator = DefaultAxes.CleanSeparator
                    });

                    cartesianChart3.AxisY.Add(new Axis
                    {
                        Title = "Jumlah",
                        LabelFormatter = value => (value/1000000000000).ToString() + "T",
                        MinValue = 0
                    });

                    cartesianChart3.LegendLocation = LegendLocation.Right;

                    //example

                }
            }
        }

        public void reload4()
        {

            cartesianChart4.AxisX.Clear();
            cartesianChart4.AxisY.Clear();
            cartesianChart4.Series.Clear();

            List<Double> rasio = new List<Double>();
            List<String> tanggal = new List<String>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "SELECT [Hari] = day(tanggal),[Rasio] = CAST(SUM(saldoAwal100 + saldoAwal50 + saldoAwal20) AS FLOAT)/(SUM(isiATM100+isiATM50+isiATM20+isiCRM100+isiCRM50+isiCRM20)) FROM TransaksiAtms ta join Pkt p on ta.kodePkt = p.kodePkt WHERE YEAR(tanggal) = " + comboTahun4.SelectedValue.ToString() + areaChoose(comboArea.SelectedValue.ToString()) + "and Month(tanggal) = "+comboBulan4.SelectedValue.ToString()+" GROUP BY day(tanggal) ORDER BY Hari ";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        tanggal.Add(reader[0].ToString());
                        rasio.Add((Double)reader[1]);

                    }
                    ChartValues<Double> cv = new ChartValues<Double>();
                    foreach (Double temp in rasio)
                    {
                        cv.Add(Math.Round(temp, 2));
                    }
                    cartesianChart4.Series = new SeriesCollection
                    {
                        new LineSeries
                        {
                            Title = comboTahun4.Text,
                            Values = cv,
                            LineSmoothness = 0
                        }
                    };
                    cartesianChart4.AxisX.Add(new Axis
                    {
                        Title = "Tanggal",
                        Labels = tanggal
                        //Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" }
                    });

                    cartesianChart4.AxisY.Add(new Axis
                    {
                        Title = "Rasio",
                        LabelFormatter = value => value.ToString()
                    });

                    cartesianChart4.LegendLocation = LegendLocation.Right;
                }
            }
        }

        public void reload5()
        {
            cartesianChart5.AxisX.Clear();
            cartesianChart5.AxisY.Clear();
            cartesianChart5.Series.Clear();

            List<Double> rasio = new List<Double>();
            List<String> bulan = new List<String>();
            List<Double> persen = new List<Double>();

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "SELECT [Bulan] = MONTH(tanggal),[Rasio] = CAST(SUM(saldoAwal100 + saldoAwal50 + saldoAwal20) AS FLOAT)/(SUM(isiATM100+isiATM50+isiATM20+isiCRM100+isiCRM50+isiCRM20)),[persen] = cast(sum(adhoc100+adhoc50+adhoc20)as float)/ cast(sum(adhoc100+adhoc50+adhoc20+bon100+bon50+bon20) as float) FROM TransaksiAtms ta join Pkt p on ta.kodePkt = p.kodePkt WHERE YEAR(tanggal) = " + comboTahun5.SelectedValue.ToString() + kanwilChoose(comboKanwil5.SelectedValue.ToString()) + " GROUP BY MONTH(tanggal), YEAR(tanggal) ORDER BY Bulan";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        bulan.Add(reader[0].ToString());
                        rasio.Add((Double)reader[1]);
                        persen.Add((Double)reader[2]);

                    }
                    ChartValues<Double> cv = new ChartValues<Double>();
                    foreach (Double temp in rasio)
                    {
                        cv.Add(Math.Round(temp, 2));
                    }

                    ChartValues<Double> cv2 = new ChartValues<Double>();
                    foreach (Double temp in persen)
                    {
                        cv2.Add(Math.Round(temp, 4) * 100);
                    }

                    cartesianChart5.Series = new SeriesCollection
                    {
                        new ColumnSeries
                        {
                            Title = "Rasio "+ comboTahun5.Text,
                            Values = cv
                        },
                        new LineSeries
                        {
                            Title = "Persen "+comboTahun5.Text,
                            Values = cv2,
                            LineSmoothness = 0
                            
                        }
                    };
                    cartesianChart5.AxisX.Add(new Axis
                    {
                        Title = "Bulan",
                        Labels = bulan
                        //Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" }
                    });

                    cartesianChart5.AxisY.Add(new Axis
                    {
                        Title = "Rasio dan Persen",
                        LabelFormatter = value => value.ToString(),
                        MinValue = 0
                    });

                    cartesianChart5.LegendLocation = LegendLocation.Right;

                }
            }
        }

        public void reload6()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();

            List<String> kanwil = new List<String>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                { 
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = kanwilChooseGridView(comboKanwil6.SelectedValue.ToString());
                    //"select kodePkt from Pkt where kanwil like " + "'" + comboKanwil6.SelectedValue.ToString() + "'";


                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        kanwil.Add(reader[0].ToString());
                    }
                    dataGridView1.Columns.Add("PKT", "PKT");
                    DataGridViewRow rows = new DataGridViewRow();
                    DataGridViewTextBoxCell tbcell = new DataGridViewTextBoxCell();
                    foreach (var item in kanwil)
                    {
                       // Console.WriteLine(item);
                        rows = new DataGridViewRow();
                        tbcell = new DataGridViewTextBoxCell();
                        tbcell.Value = item;
                        rows.Cells.Add(tbcell);
                        dataGridView1.Rows.Add(rows);
                    }
                    
                }
            }  
        }

        public void reload7()
        {
            dataGridView2.Columns.Clear();
            dataGridView2.Rows.Clear();

            List<String> rasio = new List<String>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = tahunPlusKanwilChoose_1(comboKanwil6.SelectedValue.ToString(), comboTahun6_1.SelectedValue.ToString());
                        //kanwilChooseGridView(comboKanwil6.SelectedValue.ToString());

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        rasio.Add(reader[1].ToString());
                    }
                    dataGridView2.Columns.Add("Aktual "+comboTahun6_1.SelectedValue.ToString(), "Aktual "+comboTahun6_1.SelectedValue.ToString());
                    DataGridViewRow rows = new DataGridViewRow();
                    DataGridViewTextBoxCell tbcell = new DataGridViewTextBoxCell();
                    foreach (var item in rasio)
                    {
                        // Console.WriteLine(item);
                        rows = new DataGridViewRow();
                        tbcell = new DataGridViewTextBoxCell();
                        tbcell.Value = item;
                        rows.Cells.Add(tbcell);
                        dataGridView2.Rows.Add(rows);
                    }

                }
            }
        }

        public void reload8()
        {
            List<Bulan> rasio = new List<Bulan>();

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    

                        if (comboKanwil6.SelectedValue.ToString().Equals("Nasional"))
                    {
                        cmd.CommandText = "SELECT  [Jan] = ISNULL([1],0), [Feb] = ISNULL([2],0), [Mar] = ISNULL([3],0), [Apr] = ISNULL([4],0), [Mei] = ISNULL([5],0), [Jun] = ISNULL([6],0), [Jul] = ISNULL([7],0), [Agt] = ISNULL([8],0), [Sep] = ISNULL([9],0), [Okt] = ISNULL([10],0), [Nov] = ISNULL([11],0), [Des] = ISNULL([12],0) "
                        + " FROM (SELECT distinct [Bulan] = month(tanggal), vendor , [Rasio] = CAST(SUM(saldoAwal100 + saldoAwal50 + saldoAwal20) AS FLOAT) / (SUM(isiATM100 + isiATM50 + isiATM20 + isiCRM100 + isiCRM50 + isiCRM20))"
                        + " FROM TransaksiAtms ta join Pkt p on ta.kodePkt = p.kodePkt"
                        + " WHERE YEAR(tanggal) =" + comboTahun6_2.SelectedValue.ToString()
                        //+ " and kanwil like '" + comboKanwil6.SelectedValue.ToString() + "'"
                        + " group by vendor, MONTH(tanggal)"
                        + " ) AS T"
                        + " PIVOT"
                        + " ("
                        + " Max([Rasio])"
                        + " FOR[Bulan] IN([1], [2], [3], [4],"
                        + " [5], [6], [7], [8], [9], [10], [11], [12])"
                        + " )AS pvt";
                    }
                        else
                    {
                        cmd.CommandText = "SELECT  [Jan] = ISNULL([1],0), [Feb] = ISNULL([2],0), [Mar] = ISNULL([3],0), [Apr] = ISNULL([4],0), [Mei] = ISNULL([5],0), [Jun] = ISNULL([6],0), [Jul] = ISNULL([7],0), [Agt] = ISNULL([8],0), [Sep] = ISNULL([9],0), [Okt] = ISNULL([10],0), [Nov] = ISNULL([11],0), [Des] = ISNULL([12],0) "
                        + " FROM (SELECT[Bulan] = month(tanggal), p.kodePkt , [Rasio] = CAST(SUM(saldoAwal100 + saldoAwal50 + saldoAwal20) AS FLOAT) / (SUM(isiATM100 + isiATM50 + isiATM20 + isiCRM100 + isiCRM50 + isiCRM20))"
                        + " FROM TransaksiAtms ta join Pkt p on ta.kodePkt = p.kodePkt"
                        + " WHERE YEAR(tanggal) =" + comboTahun6_2.SelectedValue.ToString()
                        + " and kanwil like '" + comboKanwil6.SelectedValue.ToString() + "'"
                        + " group by p.kodePkt, MONTH(tanggal)"
                        + " ) AS T"
                        + " PIVOT"
                        + " ("
                        + " Max([Rasio])"
                        + " FOR[Bulan] IN([1], [2], [3], [4],"
                        + " [5], [6], [7], [8], [9], [10], [11], [12])"
                        + " )AS pvt";
                    }

                        

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                       
                        Bulan bul = new Bulan();
                        bul.Jan = reader[0].ToString();
                        bul.Feb = reader[1].ToString();
                        bul.Mar = reader[2].ToString();
                        bul.Apr = reader[3].ToString();
                        bul.Mei = reader[4].ToString();
                        bul.Jun = reader[5].ToString();
                        bul.Jul = reader[6].ToString();
                        bul.Agt = reader[7].ToString();
                        bul.Sep = reader[8].ToString();
                        bul.Okt = reader[9].ToString();
                        bul.Nov = reader[10].ToString();
                        bul.Des = reader[11].ToString();
                        rasio.Add(bul);
                    }
                    DataTable tb = new DataTable();
                    using (var reader1 = ObjectReader.Create(rasio,"Jan","Feb","Mar","Apr","Mei","Jun","Jul","Agt","Sep","Okt","Nov","Des"))
                    {
                        tb.Load(reader1);
                    }
                    dataGridView3.DataSource = tb;
                }
            }
        }

        public void reload9()
        {
            cartesianChart6.AxisX.Clear();
            cartesianChart6.AxisY.Clear();
            cartesianChart6.Series.Clear();

            List<Double> rasio = new List<Double>();
            List<String> tanggal = new List<String>();
            List<String> hari = new List<String>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "SELECT day(tanggal), DATENAME(dw,Tanggal),[Rasio] = CAST(SUM(saldoAwal100 + saldoAwal50 + saldoAwal20) AS FLOAT)/(SUM(isiATM100+isiATM50+isiATM20+isiCRM100+isiCRM50+isiCRM20))  FROM TransaksiAtms ta join Pkt p on ta.kodePkt = p.kodePkt WHERE YEAR(tanggal) = " +comboTahun7.SelectedValue.ToString() +" and p.kodePkt = '"+comboPkt7.SelectedValue.ToString()+"' and MONTH(tanggal) = "+ comboBulan7.SelectedValue.ToString() +" GROUP BY DATENAME(dw, tanggal), DATENAME(WEEK, tanggal), day(tanggal) order by day(tanggal)";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        tanggal.Add(reader[0].ToString());
                        hari.Add(reader[1].ToString() + " ("+reader[0].ToString()+")");
                        rasio.Add((Double)reader[2]);

                    }
                    Console.WriteLine(tanggal.Count);
                    ChartValues<Double> cv = new ChartValues<Double>();
                    foreach (Double temp in rasio)
                    {
                        cv.Add(Math.Round(temp, 2));
                    }
                    cartesianChart6.Series = new SeriesCollection
                    {
                        new LineSeries
                        {
                            
                            Title = comboPkt7.Text,
                            Values = cv,
                            LineSmoothness = 0

                        }
                    };

                    cartesianChart6.AxisX.Add(new Axis
                    {
                        Title = "Hari",
                        Labels = hari,
                        Separator = new Separator // force the separator step to 1, so it always display all labels
                        {
                            Step = 1,
                            IsEnabled = false //disable it to make it invisible.
                        },
                        LabelsRotation = 30,
                        //Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" }
                    }   
                    );
                   
                    cartesianChart6.AxisY.Add(new Axis
                    {
                        Title = "Rasio",
                        LabelFormatter = value => value.ToString(),
                        MinValue = 0
                        
                    });

                    cartesianChart6.LegendLocation = LegendLocation.Right;
                    
                }
            }
        }

        public void reload10()
        {
            cartesianChart7.AxisX.Clear();
            cartesianChart7.AxisY.Clear();
            cartesianChart7.Series.Clear();

            List<int> rasio = new List<int>();
            List<String> tanggal = new List<String>();
            List<String> hari = new List<String>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select [Jumlah adhoc] = count (adhoc100+adhoc50+adhoc20), [tanggal]  = day(tanggal), [Minggu] = (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) from TransaksiAtms t join Pkt p on t.kodePkt = p.kodePkt where ((adhoc100 + adhoc50 + adhoc20) != 0) AND MONTH(tanggal) =  " + comboBulan8.SelectedValue.ToString()+" and YEAR(tanggal) = "+ comboTahun8.SelectedValue.ToString()+ "  group by day(tanggal), (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) order by day(tanggal)";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        rasio.Add((int)reader[0]);
                        tanggal.Add(reader[1].ToString() +" (" +  reader[2].ToString()+")");

                    }
                    ChartValues<Double> cv = new ChartValues<Double>();
                    foreach (Double temp in rasio)
                    {
                        cv.Add(Math.Round(temp, 2));
                    }

                    cartesianChart7.Series = new SeriesCollection
                    {
                        new LineSeries
                        {

                            Title = comboTahun8.Text + " - " + bulan[(Int32)comboBulan8.SelectedIndex],
                            Values = cv,
                            LineSmoothness = 0
                        }
                    };
            
                    cartesianChart7.AxisX.Add(new Axis
                    {
                        Title = "Tanggal (Minggu)",
                        Labels = tanggal,
                        
                        Separator = new Separator // force the separator step to 1, so it always display all labels
                        {
                            Step = 1,
                            IsEnabled = false //disable it to make it invisible.
                        },
                        //LabelsRotation = 30,
                        
                        //Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" }
                    }

                    );

                    cartesianChart7.AxisY.Add(new Axis
                    {
                        Title = "Jumlah Ad-Hoc",
                        LabelFormatter = value => value.ToString(),
                        MinValue = 0

                    });

                    cartesianChart7.LegendLocation = LegendLocation.Right;

                }
            }
        }

        public void reload11()
        {
            dataGridView2.Columns.Clear();
            dataGridView2.Rows.Clear();

            List<String> rasio = new List<String>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = tahunPlusKanwilChoose_2(comboKanwil6.SelectedValue.ToString(), comboTahun6_1.SelectedValue.ToString());
                    //kanwilChooseGridView(comboKanwil6.SelectedValue.ToString());

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        rasio.Add(reader[1].ToString());
                    }
                    dataGridView2.Columns.Add("Aktual " + comboTahun6_1.SelectedValue.ToString(), "Aktual " + comboTahun6_1.SelectedValue.ToString());
                    DataGridViewRow rows = new DataGridViewRow();
                    DataGridViewTextBoxCell tbcell = new DataGridViewTextBoxCell();
                    foreach (var item in rasio)
                    {
                        // Console.WriteLine(item);
                        rows = new DataGridViewRow();
                        tbcell = new DataGridViewTextBoxCell();
                        tbcell.Value = item;
                        rows.Cells.Add(tbcell);
                        dataGridView2.Rows.Add(rows);
                    }

                }
            }
        }

        public void reload12()
        {
            List<Bulan> rasio = new List<Bulan>();

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();


                    if (comboKanwil6.SelectedValue.ToString().Equals("Nasional"))
                    {
                        cmd.CommandText = "SELECT DISTINCT pkt.vendor, "
                                          +"    [Jan] = ISNULL([Jan],0),"
                                          +"    [Feb] = ISNULL([Feb],0),"
                                          +"	[Mar] = ISNULL([Mar],0),"
                                          +"	[Apr] = ISNULL([Apr],0),"
                                          +"	[Mei] = ISNULL([Mei],0),"
                                          +"	[Jun] = ISNULL([Jun],0),"
                                          +"	[Jul] = ISNULL([Jul],0),"
                                          +"	[Agt] = ISNULL([Agt],0),"
                                          +"	[Sep] = ISNULL([Sep],0),"
                                          +"	[Okt] = ISNULL([Okt],0),"
                                          +"	[Nov] = ISNULL([Nov],0),"
                                          +"	[Des] = ISNULL([Des],0)"
                                          +" FROM pkt"
                                          +" LEFT JOIN("
                                          +" SELECT[Vendor] = vendor, [Jan] = ISNULL([1],0), [Feb] = ISNULL([2],0), [Mar] = ISNULL([3],0), [Apr] = ISNULL([4],0), [Mei] = ISNULL([5],0), [Jun] = ISNULL([6],0), [Jul] = ISNULL([7],0), [Agt] = ISNULL([8],0), [Sep] = ISNULL([9],0), [Okt] = ISNULL([10],0), [Nov] = ISNULL([11],0), [Des] = ISNULL([12],0)"
                                          +" FROM"
                                          +" (SELECT distinct [Bulan] = month(tanggal), vendor, [Jumlah adhoc] = count(adhoc100 + adhoc50 + adhoc20) "
                                          +" FROM TransaksiAtms ta join Pkt p on ta.kodePkt = p.kodePkt "
                                          +" WHERE YEAR(tanggal) = "+comboTahun6_2.SelectedValue.ToString()+" and((adhoc100 + adhoc50 + adhoc20) != 0) "
                                          +" group by vendor, MONTH(tanggal) "
                                          +" ) AS T "
                                          +" PIVOT "
                                          +" ( "
                                          +"     Max([Jumlah adhoc]) "
                                          +"     FOR[Bulan] IN([1], [2], [3], [4], "
                                          +"     [5], [6], [7], [8], [9], [10], [11], [12]) "
                                          +" )AS pvt "
                                          +" ) AS T2 ON pkt.vendor = T2.vendor ";
                    }
                    else
                    {
                        cmd.CommandText = "SELECT DISTINCT pkt.kodePkt,"
                                           + " [Jan] = ISNULL([Jan],0),"
                                           + " [Feb] = ISNULL([Feb],0),"
                                           + " [Mar] = ISNULL([Mar],0),"
                                           + " [Apr] = ISNULL([Apr],0),"
                                           + " [Mei] = ISNULL([Mei],0),"
                                           + " [Jun] = ISNULL([Jun],0),"
                                           + " [Jul] = ISNULL([Jul],0),"
                                           + " [Agt] = ISNULL([Agt],0),"
                                           + " [Sep] = ISNULL([Sep],0),"
                                           + " [Okt] = ISNULL([Okt],0),"
                                           + " [Nov] = ISNULL([Nov],0),"
                                           + " [Des] = ISNULL([Des],0)"
                                           + " FROM pkt"
                                           + " LEFT JOIN"
                                           + " ("
                                           + " SELECT[kodePkt]= kodePkt, [Jan] = ISNULL([1],0), [Feb] = ISNULL([2],0), [Mar] = ISNULL([3],0), [Apr] = ISNULL([4],0), [Mei] = ISNULL([5],0), [Jun] = ISNULL([6],0), [Jul] = ISNULL([7],0), [Agt] = ISNULL([8],0), [Sep] = ISNULL([9],0), [Okt] = ISNULL([10],0), [Nov] = ISNULL([11],0), [Des] = ISNULL([12],0)"
                                           + " FROM"
                                           + " ("
                                           + " SELECT[Bulan] = month(tanggal), p.kodePkt , [Jumlah adhoc] = count(adhoc100 + adhoc50 + adhoc20) "
                                           + " FROM TransaksiAtms ta join Pkt p on ta.kodePkt = p.kodePkt "
                                           + " WHERE YEAR(tanggal) = "+comboTahun6_2.SelectedValue.ToString()+" and((adhoc100 + adhoc50 + adhoc20) != 0) "
	                                       + " and kanwil like '"+comboKanwil6.SelectedValue.ToString()+"'"
	                                       +" group by p.kodePkt, MONTH(tanggal)"
	                                       +" ) AS T"
                                           +" PIVOT"
                                           +" ("
                                           +"     Max([Jumlah Adhoc])"
                                           +"     FOR[Bulan] IN([1], [2], [3], [4],"
                                           +"     [5], [6], [7], [8], [9], [10], [11], [12])"
	                                       +" )AS pvt"
                                           +" ) AS T2 ON pkt.kodePkt = T2.kodePkt"
                                           +" WHERE kanwil ='"+comboKanwil6.SelectedValue.ToString()+"'";
                    }



                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {

                        Bulan bul = new Bulan();
                        bul.Jan = reader[1].ToString();
                        bul.Feb = reader[2].ToString();
                        bul.Mar = reader[3].ToString();
                        bul.Apr = reader[4].ToString();
                        bul.Mei = reader[5].ToString();
                        bul.Jun = reader[6].ToString();
                        bul.Jul = reader[7].ToString();
                        bul.Agt = reader[8].ToString();
                        bul.Sep = reader[9].ToString();
                        bul.Okt = reader[10].ToString();
                        bul.Nov = reader[11].ToString();
                        bul.Des = reader[12].ToString();
                        rasio.Add(bul);
                    }
                    DataTable tb = new DataTable();
                    using (var reader1 = ObjectReader.Create(rasio, "Jan", "Feb", "Mar", "Apr", "Mei", "Jun", "Jul", "Agt", "Sep", "Okt", "Nov", "Des"))
                    {
                        tb.Load(reader1);
                    }
                    dataGridView3.DataSource = tb;
                }
            }
        }

        private void comboTahun1_SelectedIndexChanged(object sender, EventArgs e)
        {
            reload();
        }

        private void comboTahun2_SelectedIndexChanged(object sender, EventArgs e)
        {
            reload2();
        }

        private void comboAvg1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (checkAvg1.Checked)
            {
                reload();
                List<Double> rasio = new List<Double>();
                List<String> tahun = new List<String>();
                using (SqlConnection sql = new SqlConnection(Variables.connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = sql;
                        sql.Open();
                        cmd.CommandText = "SELECT [Tahun] = YEAR(tanggal),[Rasio] = CAST(SUM(saldoAwal100 + saldoAwal50 + saldoAwal20) AS FLOAT)/(SUM(isiATM100+isiATM50+isiATM20+isiCRM100+isiCRM50+isiCRM20)) FROM TransaksiAtms WHERE YEAR(tanggal) = " + comboAvg1.SelectedValue.ToString() + " GROUP BY  YEAR(tanggal)";
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            tahun.Add(reader[0].ToString());
                            rasio.Add((Double)reader[1]);

                        }

                        ChartValues<Double> cv = new ChartValues<Double>();
                        for (int i = 0; i < 12; i++)
                        {
                            cv.Add(Math.Round(rasio[0], 2));
                        }

                        /* foreach (Double temp in rasio)
                         {
                             cv.Add(Math.Round(temp, 2));
                         }*/


                        cartesianChart1.Series.Add(
                            new LineSeries()
                            {
                                Title = comboAvg1.Text + " Average",
                                Values = cv
                            }
                        );
                    }
                }

            }
        }

        private void comboArea_SelectionChangeCommitted(object sender, EventArgs e)
        {
            areaChoose(comboArea.SelectedIndex.ToString());
            reload();
            reload2();
            reload3();
            reload4();
        }

        private void comboKanwil6_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //cari apa disini hayo ehehehe
            if (comboMode7.SelectedValue.ToString().Equals("Rasio"))
            {
                reload6();
                reload7();
                reload8();
            }
            else
            {
                reload6();
                reload11();
                reload12();
            }

        }

        private void comboVendor7_SelectionChangeCommitted(object sender, EventArgs e)
        {
            reloadComboPkt7();
        }

        private void comboMode7_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboMode7.SelectedValue.ToString().Equals("Rasio"))
            {
                reload6();
                reload7();
                reload8();
            }
            else
            {
                reload6();
                reload11();
                reload12();
            }
        }

        private void buttonTambah1_Click(object sender, EventArgs e)
        {

            List<Double> rasio = new List<Double>();
            List<String> bulan = new List<String>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "SELECT [Bulan] = MONTH(tanggal),[Rasio] = CAST(SUM(saldoAwal100 + saldoAwal50 + saldoAwal20) AS FLOAT)/(SUM(isiATM100+isiATM50+isiATM20+isiCRM100+isiCRM50+isiCRM20)) FROM TransaksiAtms WHERE YEAR(tanggal) = " + comboTambah1.SelectedValue.ToString() + " GROUP BY MONTH(tanggal), YEAR(tanggal) ORDER BY Bulan";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        bulan.Add(reader[0].ToString());
                        rasio.Add((Double)reader[1]);

                    }
                    ChartValues<Double> cv = new ChartValues<Double>();
                    foreach (Double temp in rasio)
                    {
                        cv.Add(Math.Round(temp,2));
                    }


                    cartesianChart1.Series.Add(
                        new LineSeries()
                        {
                            Title = comboTambah1.Text,
                            Values=cv
                        }
                    );
                }
            }

         }

        private void buttonTambah2_Click(object sender, EventArgs e)
        {

            List<Double> persen = new List<Double>();
            List<String> bulan = new List<String>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select [bulan] = month(tanggal),[persen] = cast(sum(adhoc100+adhoc50+adhoc20)as float)/SUM(adhoc100+adhoc50+adhoc20+bon100+bon50+bon20) from TransaksiAtms where year(tanggal) = " + comboTambah2.SelectedValue.ToString() + " group by MONTH(tanggal) order by[bulan]";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        bulan.Add(reader[0].ToString());
                        persen.Add((Double)reader[1]);

                    }
                    ChartValues<Double> cv = new ChartValues<Double>();
                    foreach (Double temp in persen)
                    {
                        cv.Add(Math.Round(temp, 4)*100);
                    }


                    cartesianChart2.Series.Add(
                        new LineSeries()
                        {
                            Title = comboTambah2.Text,
                            Values = cv
                        }
                    );
                }
            }
        }

        private void buttonRefresh3_Click(object sender, EventArgs e)
        {
            reload3();
        }

        private void buttonRefresh4_Click(object sender, EventArgs e)
        {
            reload4();
        }

        private void buttonRefresh5_Click(object sender, EventArgs e)
        {
            reload5();
        }

        private void buttonRefresh6_Click(object sender, EventArgs e)
        {
            //if (comboMode7.SelectedValue.ToString().Equals("Rasio"))
            //{
            //    reload6();
            //    reload7();
            //    reload8();
            //}
            //else
            //{
            //    reload6();
            //    reload11();
            //    reload12();
            //}
            
        }

        private void buttonRefresh7_Click(object sender, EventArgs e)
        {
            reload9();
        }

        private void buttonRefresh8_Click(object sender, EventArgs e)
        {
            reload10();
        }

        private void buttonAdd7_Click(object sender, EventArgs e)
        {
            List<Double> rasio = new List<Double>();
            List<String> tanggal = new List<String>();
            List<String> hari = new List<String>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "SELECT day(tanggal), DATENAME(dw,Tanggal),[Rasio] = CAST(SUM(saldoAwal100 + saldoAwal50 + saldoAwal20) AS FLOAT)/(SUM(isiATM100+isiATM50+isiATM20+isiCRM100+isiCRM50+isiCRM20))  FROM TransaksiAtms ta join Pkt p on ta.kodePkt = p.kodePkt WHERE YEAR(tanggal) = " + comboTahun7.SelectedValue.ToString() + " and p.kodePkt = '" + comboPkt7.SelectedValue.ToString() + "' and MONTH(tanggal) = " + comboBulan7.SelectedValue.ToString() + " GROUP BY DATENAME(dw, tanggal), DATENAME(WEEK, tanggal), day(tanggal) order by day(tanggal)";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        tanggal.Add(reader[0].ToString());
                        hari.Add(reader[1].ToString());
                        rasio.Add((Double)reader[2]);

                    }
                    ChartValues<Double> cv = new ChartValues<Double>();
                    foreach (Double temp in rasio)
                    {
                        cv.Add(Math.Round(temp, 2));
                    }
                    cartesianChart6.Series.Add(

                        new LineSeries
                        {

                            Title = comboPkt7.Text,
                            Values = cv,
                            LineSmoothness = 0
                        }
                    );
                }
            }
        }

        
        class Bulan
        {
            public String Jan { set; get; }
            public String Feb { set; get; }
            public String Mar { set; get; }
            public String Apr { set; get; }
            public String Mei { set; get; }
            public String Jun { set; get; }
            public String Jul { set; get; }
            public String Agt { set; get; }
            public String Sep { set; get; }
            public String Okt { set; get; }
            public String Nov { set; get; }
            public String Des { set; get; }


        }

        private void comboTahun6_1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboMode7.SelectedValue.ToString().Equals("Rasio"))
            {
                reload6();
                reload7();
                reload8();
            }
            else
            {
                reload6();
                reload11();
                reload12();
            }
        }

        private void comboTahun6_2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboMode7.SelectedValue.ToString().Equals("Rasio"))
            {
                reload6();
                reload7();
                reload8();
            }
            else
            {
                reload6();
                reload11();
                reload12();
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

        private void comboTahun4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboTahun4_SelectionChangeCommitted(object sender, EventArgs e)
        {
            reload4();
        }

        private void comboBulan4_SelectionChangeCommitted(object sender, EventArgs e)
        {
            reload4();
        }

        private void comboKanwil5_SelectionChangeCommitted(object sender, EventArgs e)
        {
            reload5();
        }

        private void comboTahun8_SelectionChangeCommitted(object sender, EventArgs e)
        {
            reload10();
        }
    }
}

