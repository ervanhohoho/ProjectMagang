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
using System.Globalization;

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
            reloadSebaranSaldoCoj();
            reloadKomposisiUbVsUk();
            reloadUangBesar();
            reloadUangKecil();
            reloadTop5UangBesar();
            reloadUangBesarSum();
            reloadTop5UangKecil();
            reloadUangKecilSum();
            reloadLabelTotalCoj();
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
                    cmd.CommandText = "select distinct month(tanggal) from StokPosisi  where year(tanggal) = "+ comboTahun.SelectedValue.ToString()+" order by month(tanggal) desc";
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
                    cmd.CommandText = "select distinct day(tanggal) from StokPosisi where year(tanggal) = "+comboTahun.SelectedValue.ToString()+" and month(tanggal) = "+comboBulan.SelectedValue.ToString()+" order by day(tanggal) desc";
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
                                      " where month(tanggal) = "+comboBulan.SelectedValue.ToString()+" and year(tanggal) = "+comboTahun.SelectedValue.ToString()+" and day(tanggal) = "+comboTanggal.SelectedValue.ToString()+" ";
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
                            LabelPoint =  p=>(Math.Round((p.Y/1000000000),0)).ToString() + " M",
                            
                        },
                        new PieSeries
                        {
                            Title = "Fit",
                            Values = new ChartValues<Int64>
                            {
                                fit
                            },
                            DataLabels = true,
                            LabelPoint = p=>(Math.Round((p.Y/1000000000),0)).ToString() + " M",
                        },
                        new PieSeries
                        {
                            Title = "Unfit",
                            Values = new ChartValues<Int64>
                            {
                                unfit
                            },
                            DataLabels = true,
                            LabelPoint = p=>(Math.Round((p.Y/1000000000),0)).ToString() + " M",
                        },
                        new PieSeries
                        {
                            Title = "Mayor Minor",
                            Values = new ChartValues<Int64>
                            {
                                mayorminor
                            },
                            DataLabels = true,
                            LabelPoint = p=>(Math.Round((p.Y/1000000000),0)).ToString() + " M",
                        },
                        new PieSeries
                        {
                            Title = "Gress",
                            Values = new ChartValues<Int64>
                            {
                                gress
                            },
                            DataLabels = true,
                            LabelPoint = p=>(Math.Round((p.Y/1000000000),0)).ToString() + " M",
                        }
                    };

                    pieChartStokSaldoCoj.LegendLocation = LegendLocation.Bottom;

                }
            }
        }

        public void reloadLabelTotalCoj()
        {

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    float a = 0;

                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select sum(unprocessed + newBaru + newLama + fitBaru + fitLama + passThrough + unfitBaru + unfitNKRI + unfitLama + RRMBaru + RRMNKRI + RRMLama + RupiahRusakMayor) from stokposisi"
                        + " where day(tanggal) = " + comboTanggal.SelectedValue.ToString() + " and month(tanggal) = " + comboBulan.SelectedValue.ToString() + " and year(tanggal) = " + comboTahun.SelectedValue.ToString() + "";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        a = Int64.Parse(reader[0].ToString());
                    }
                    label36.Text = Math.Round((a/1000000000),0).ToString() + " M";
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
                                     + " where (denom = 100000 or denom = 50000) and day(tanggal) = " + comboTanggal.SelectedValue.ToString() + " and month(tanggal) = " + comboBulan.SelectedValue.ToString() + " and year(tanggal) = " + comboTahun.SelectedValue.ToString() + " )a, "
                                     + " (select[uangkecil] = sum(unprocessed + newBaru + newLama + fitBaru + fitLama + passThrough + unfitBaru + unfitNKRI +  unfitLama + RRMBaru+ RRMNKRI + RRMLama + RupiahRusakMayor)"
                                     + " from StokPosisi"
                                     + " where(denom != 100000 AND denom != 50000) and day(tanggal) = "+comboTanggal.SelectedValue.ToString()+" and month(tanggal) = "+comboBulan.SelectedValue.ToString()+" and year(tanggal) = "+comboTahun.SelectedValue.ToString()+" )b";

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
                            LabelPoint =  p=>(Math.Round((p.Y/1000000000),0)).ToString() + " M",
                        },
                        new PieSeries
                        {
                            Title = "Uang Kecil",
                            Values = new ChartValues<Int64>
                            {
                                uk
                            },
                            DataLabels = true,
                            LabelPoint =  p=>(Math.Round((p.Y/1000000000),0)).ToString() + " M",
                        }
                    };

                    pieChartUbVsUk.LegendLocation = LegendLocation.Bottom;
                }
            }
        }

        public void reloadSebaranSaldoCoj()
        {
            cartesianChartSebaranSaldoCoj.Series.Clear();
            cartesianChartSebaranSaldoCoj.AxisX.Clear();
            cartesianChartSebaranSaldoCoj.AxisY.Clear();

            List<Int64> sebaran = new List<Int64>();
            List<String> kodepkt = new List<String>();

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select p.kodePkt, sum(unprocessed + newBaru + newLama + fitBaru + fitLama + passThrough + unfitBaru + unfitNKRI +  unfitLama + RRMBaru+ RRMNKRI + RRMLama + RupiahRusakMayor)"
                                        + " from StokPosisi s join Pkt p on s.namaPkt = p.namaPkt"
                                        + " where month(tanggal) = " + comboBulan.SelectedValue.ToString() + " and year(tanggal) = " + comboTahun.SelectedValue.ToString() + " and day(tanggal) = " + comboTanggal.SelectedValue.ToString() + " "
                                        + " group by p.kodePkt"
                                        + " order by kodePkt desc";

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        kodepkt.Add(reader[0].ToString());
                        sebaran.Add(Int64.Parse(reader[1].ToString()));
                    }

                    ChartValues<Int64> cv = new ChartValues<Int64>();
                    foreach (var item in sebaran)
                    {
                        cv.Add(item);
                    }

                    cartesianChartSebaranSaldoCoj.Series = new SeriesCollection
                    {
                        new ColumnSeries
                        {
                            Title = "Sebaran Saldo COJ",
                            Values = cv,
                            DataLabels = true,
                            LabelPoint = p=>(Math.Round((p.Y/1000000000),0)).ToString() + " M"
                        }
                       
                        
                    };

                    cartesianChartSebaranSaldoCoj.AxisX.Add(new Axis
                    {
                        Title = "PKT",
                        Labels = kodepkt,
                        Separator = new Separator
                        {
                            Step = 1
                        }                        

                    });

                    cartesianChartSebaranSaldoCoj.AxisY.Add(new Axis
                    {
                        Title = "Jumlah",
                        LabelFormatter = value => (value / 1000000000).ToString() + " M",
                        MinValue = 0
                    });
                    cartesianChartSebaranSaldoCoj.LegendLocation = LegendLocation.Bottom;

                }
            }

        }

        public void reloadKomposisiUbVsUk()
        {
            cartesianChartKomposisiUbVsUk.Series.Clear();
            cartesianChartKomposisiUbVsUk.AxisX.Clear();
            cartesianChartKomposisiUbVsUk.AxisY.Clear();

            List<Int64> ub = new List<Int64>();
            List<Int64> uk = new List<Int64>();
            List<String> kodepkt = new List<String>();

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select * from("
                                       + " select[uangbesar] = sum(unprocessed + newBaru + newLama + fitBaru + fitLama + passThrough + unfitBaru + unfitNKRI + unfitLama + RRMBaru + RRMNKRI + RRMLama + RupiahRusakMayor), kodePkt, [tipe] = 'BESAR'"
                                       + " from StokPosisi s join Pkt p on s.namaPkt = p.namaPkt"
                                       + " where (denom = 100000 or denom = 50000) and day(tanggal) = "+comboTanggal.SelectedValue.ToString()+" and month(tanggal) = "+comboBulan.SelectedValue.ToString()+" and year(tanggal) = "+comboTahun.SelectedValue.ToString()+""
                                       + " group by p.kodePkt"
                                       + " union"
                                       + " select[uangkecil] = sum(unprocessed + newBaru + newLama + fitBaru + fitLama + passThrough + unfitBaru + unfitNKRI +  unfitLama + RRMBaru+ RRMNKRI + RRMLama + RupiahRusakMayor), kodePkt, [tipe] = 'KECIL'"
                                       + " from StokPosisi s join Pkt p on s.namaPkt = p.namaPkt"
                                       + " where(denom != 100000 AND denom != 50000) and day(tanggal) = " + comboTanggal.SelectedValue.ToString() + " and month(tanggal) = " + comboBulan.SelectedValue.ToString() + " and year(tanggal) = " + comboTahun.SelectedValue.ToString() + ""
                                       + " group by p.kodePkt)a"
                                       + " pivot(sum(uangbesar) for [tipe] in ([BESAR],[KECIL])) as asd";


                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        kodepkt.Add(reader[0].ToString());
                        ub.Add(Int64.Parse(reader[1].ToString()));
                        uk.Add(Int64.Parse(reader[2].ToString()));
                    }

                    ChartValues<Int64> cv = new ChartValues<Int64>();
                    foreach (var item in ub)
                    {
                        cv.Add(item);
                    }

                    ChartValues<Int64> cv2 = new ChartValues<Int64>();
                    foreach (var item in uk)
                    {
                        cv2.Add(item);
                    }

                    ChartValues<String> cv3 = new ChartValues<String>();
                    foreach (var item in kodepkt)
                    {
                        cv3.Add(item);
                    }

                    cartesianChartKomposisiUbVsUk.Series = new SeriesCollection
                    {
                        new StackedColumnSeries
                        {
                            Title = "Uang Besar",
                            Values = cv,
                            StackMode = StackMode.Percentage,
                            DataLabels = true,
                           LabelPoint = p=>(Math.Round((p.Y/1000000000),0)).ToString() + " M"

                        },
                        new StackedColumnSeries
                        {
                            Title = "Uang Kecil",
                            Values = cv2,
                            StackMode = StackMode.Percentage,
                            DataLabels = true,
                            LabelPoint = p=>(Math.Round((p.Y/1000000000),0)).ToString() + " M"
                        }
                    };

                    cartesianChartKomposisiUbVsUk.AxisX.Add(new Axis
                    {
                        Title = "PKT",
                        Labels = kodepkt,
                        Separator = new Separator
                        {
                            Step = 1
                        }
                    });

                    cartesianChartKomposisiUbVsUk.AxisY.Add(new Axis
                    {
                        Title = "Persen",
                        LabelFormatter = value => value.ToString() + " %",
                        MinValue = 0,
                        
                    });

                    cartesianChartKomposisiUbVsUk.LegendLocation = LegendLocation.Bottom;

                }
            }
        }

        public void reloadUangBesar()
        {
            cartesianChartUangBesar.Series.Clear();
            cartesianChartUangBesar.AxisX.Clear();
            cartesianChartUangBesar.AxisY.Clear();

            List<Int64> unfitmm = new List<Int64>();
            List<Int64> gress = new List<Int64>();
            List<String> kodepkt = new List<String>();

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select [unfitmayorminor] = sum(unfitBaru + unfitLama + unfitNKRI + RRMBaru + RRMLama + RRMNKRI + RupiahRusakMayor),[gress] = sum(newBaru + newLama) ,kodePkt"
                                      + " from StokPosisi s join Pkt p on s.namaPkt = p.namaPkt"
                                      + " where (denom = 100000 or denom = 50000) and day(tanggal) = " + comboTanggal.SelectedValue.ToString() + " and month(tanggal) = " + comboBulan.SelectedValue.ToString() + " and year(tanggal) = " + comboTahun.SelectedValue.ToString() + " "
                                      + " group by kodePkt";
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        kodepkt.Add(reader[2].ToString());
                        unfitmm.Add(Int64.Parse(reader[0].ToString()));
                        gress.Add(Int64.Parse(reader[1].ToString()));
                    }

                    ChartValues<Int64> cv = new ChartValues<Int64>();
                    foreach (var item in unfitmm)
                    {
                        cv.Add(item);
                    }

                    ChartValues<Int64> cv2 = new ChartValues<Int64>();
                    foreach (var item in gress)
                    {
                        cv2.Add(item);
                    }

                    ChartValues<String> cv3 = new ChartValues<String>();
                    foreach (var item in kodepkt)
                    {
                        cv3.Add(item);
                    }

                    cartesianChartUangBesar.Series = new SeriesCollection
                    {
                        new StackedColumnSeries
                        {
                            Title = "Unfit, MayorMinor",
                            Values = cv,
                            LabelPoint = p=>(Math.Round((p.Y/1000000000),0)).ToString() + " M",
                            DataLabels = true
                        },
                        new StackedColumnSeries
                        {
                            Title = "Gress",
                            Values = cv2,
                            LabelPoint = p=>(Math.Round((p.Y/1000000000),0)).ToString() + " M",
                            DataLabels = true
                        }
                    };

                    cartesianChartUangBesar.AxisX.Add(new Axis
                    {
                        Labels = kodepkt,
                        Separator = new Separator
                        {
                            Step = 1
                        }
                    });
                    

                    cartesianChartUangBesar.AxisY.Add(new Axis
                    {
                        Title = "Uang Besar",
                        LabelFormatter = value => (value / 1000000000).ToString() + "M",
                    });

                    cartesianChartUangBesar.LegendLocation = LegendLocation.Bottom;

                }
            }
        }

        public void reloadUangKecil()
        {
            cartesianChartUangKecil.Series.Clear();
            cartesianChartUangKecil.AxisX.Clear();
            cartesianChartUangKecil.AxisY.Clear();

            List<Int64> unfitmm = new List<Int64>();
            List<Int64> gress = new List<Int64>();
            List<Int64> unprocessed = new List<Int64>();
            List<String> kodepkt = new List<String>();

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select [unprocessed] = sum(unprocessed), [unfitmayorminor] = sum(unfitBaru + unfitLama + unfitNKRI + RRMBaru + RRMLama + RRMNKRI + RupiahRusakMayor),[gress] = sum(newBaru + newLama) ,kodePkt"
                                       +" from StokPosisi s join Pkt p on s.namaPkt = p.namaPkt"
                                       + " where (denom != 100000 AND denom != 50000) and day(tanggal) = " + comboTanggal.SelectedValue.ToString() + " and month(tanggal) = " + comboBulan.SelectedValue.ToString() + " and year(tanggal) = " + comboTahun.SelectedValue.ToString() + " "
                                       + " group by kodePkt";

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        kodepkt.Add(reader[3].ToString());
                        unfitmm.Add(Int64.Parse(reader[1].ToString()));
                        gress.Add(Int64.Parse(reader[2].ToString()));
                        unprocessed.Add(Int64.Parse(reader[0].ToString()));
                    }

                    ChartValues<Int64> cv = new ChartValues<Int64>();
                    foreach (var item in unfitmm)
                    {
                        cv.Add(item);
                    }

                    ChartValues<Int64> cv2 = new ChartValues<Int64>();
                    foreach (var item in gress)
                    {
                        cv2.Add(item);
                    }

                    ChartValues<String> cv3 = new ChartValues<String>();
                    foreach (var item in kodepkt)
                    {
                        cv3.Add(item);
                    }

                    ChartValues<Int64> cv4 = new ChartValues<Int64>();
                    foreach (var item in unprocessed)
                    {
                        cv4.Add(item);
                    }

                    cartesianChartUangKecil.Series = new SeriesCollection
                    {
                        new StackedColumnSeries
                        {
                            Title = "Unfit MayorMinor",
                            Values = cv,
                            DataLabels = true,
                            LabelPoint = p=>(Math.Round((p.Y/1000000000),0)).ToString() + " M"
                        },
                        new StackedColumnSeries
                        {
                            Title = "Gress",
                            Values = cv2,
                            DataLabels = true,
                            LabelPoint = p=>(Math.Round((p.Y/1000000000),0)).ToString() + " M"
                        },
                        new StackedColumnSeries
                        {
                            Title = "Unprocessed",
                            Values = cv4,
                            DataLabels = true,
                            LabelPoint = p=>(Math.Round((p.Y/1000000000),0)).ToString() + " M"
                        }
                    };

                    cartesianChartUangKecil.AxisX.Add(new Axis
                    {
                        Labels = kodepkt,
                        Separator = new Separator
                        {
                            Step = 1
                        }
                    });

                    cartesianChartUangKecil.AxisY.Add(new Axis
                    {
                        Title = "Uang Kecil",
                        LabelFormatter = value => (value / 1000000000).ToString() + "M",
                    });

                    cartesianChartUangKecil.LegendLocation = LegendLocation.Bottom;


                }
            }
        }

        public void reloadTop5UangBesar()
        {
            List<Int64> gress = new List<Int64>();
            List<String> kodepkt = new List<String>();

            label3.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
            label7.Visible = false;

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select top 5 [total] =  sum(unfitBaru + unfitLama + unfitNKRI + RRMBaru + RRMLama + RRMNKRI + RupiahRusakMayor) + sum(newBaru + newLama) ,kodePkt"
                                      + " from StokPosisi s join Pkt p on s.namaPkt = p.namaPkt"
                                      + " where (denom = 100000 or denom = 50000) and day(tanggal) = " + comboTanggal.SelectedValue.ToString() + " and month(tanggal) = " + comboBulan.SelectedValue.ToString() + " and year(tanggal) = " + comboTahun.SelectedValue.ToString() + " "
                                      + " group by kodePkt"
                                      + " order by [total] desc";
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        kodepkt.Add(reader[1].ToString());
                        gress.Add(Int64.Parse(reader[0].ToString()));
                    }

                    List<Label> asd = new List<Label>();
                    asd.Add(label3);
                    asd.Add(label4);
                    asd.Add(label5);
                    asd.Add(label6);
                    asd.Add(label7);

                    for (int i = 0; i < kodepkt.Count; i++)
                    {
                        asd[i].Visible = true;
                        asd[i].Text = kodepkt[i];
                    }

                    //if (kodepkt[0].ToString() == null || kodepkt[0].ToString() == "")
                    //{
                        
                    //}
                    //else
                    //{
                    //    label3.Visible = true;
                    //    label3.Text = kodepkt[0];

                    //    if (kodepkt[1].ToString() != null || kodepkt[1].ToString() != "")
                    //    {
                    //        label4.Visible = true;
                    //        label4.Text = kodepkt[1];
                    //    }
                    //    if (kodepkt[2].ToString() != null || kodepkt[2].ToString() != "")
                    //    {
                    //        label5.Visible = true;
                    //        label5.Text = kodepkt[2];
                    //    }
                    //    if (kodepkt[3].ToString() != null || kodepkt[3].ToString() != "")
                    //    {
                    //        label6.Visible = true;
                    //        label6.Text = kodepkt[3];
                    //    }
                    //    if (kodepkt[4].ToString() != null || kodepkt[4].ToString() != "")
                    //    {
                    //        label7.Visible = true;
                    //        label7.Text = kodepkt[4];
                    //    }
                    //}
                    
                  

                    //label3.Text = kodepkt[0];
                    //label4.Text = kodepkt[1];
                    //label5.Text = kodepkt[2];
                    //label6.Text = kodepkt[3];
                    //label7.Text = kodepkt[4];

                }
            }
        }


        private void reloadUangBesarSum()
        {
            Int64 umm = 0;
            Int64 gress = 0;

            label15.Visible = false;
            label16.Visible = false;

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select [umm] =  sum(unfitBaru + unfitLama + unfitNKRI + RRMBaru + RRMLama + RRMNKRI + RupiahRusakMayor) , [gress] = sum(newBaru + newLama) "
                                      + " from StokPosisi s join Pkt p on s.namaPkt = p.namaPkt"
                                      + " where (denom = 100000 or denom = 50000)  and day(tanggal) = " + comboTanggal.SelectedValue.ToString() + " and month(tanggal) = " + comboBulan.SelectedValue.ToString() + " and year(tanggal) = " + comboTahun.SelectedValue.ToString() + " ";
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        umm = (Int64.Parse(reader[0].ToString()));
                        gress = (Int64.Parse(reader[1].ToString()));

                    }

                    if (umm != 0)
                    {
                        label15.Visible = true;
                        label15.Text = umm.ToString("C",CultureInfo.GetCultureInfo("id-ID"));
                    }
                    if (gress != 0)
                    {
                        label16.Visible = true;
                        label16.Text = gress.ToString("C", CultureInfo.GetCultureInfo("id-ID"));
                    }


                }
            }
        }


        public void reloadTop5UangKecil()
        {
            List<Int64> gress = new List<Int64>();
            List<String> kodepkt = new List<String>();

            label26.Visible = false;
            label25.Visible = false;
            label24.Visible = false;
            label23.Visible = false;
            label22.Visible = false;

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select top 5 [total] =  sum(unprocessed) ,kodePkt"
                                      + " from StokPosisi s join Pkt p on s.namaPkt = p.namaPkt"
                                      + " where (denom != 100000 AND denom != 50000) and day(tanggal) = " + comboTanggal.SelectedValue.ToString() + " and month(tanggal) = " + comboBulan.SelectedValue.ToString() + " and year(tanggal) = " + comboTahun.SelectedValue.ToString() + " "
                                      + " group by kodePkt"
                                      + " order by [total] desc";
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        kodepkt.Add(reader[1].ToString());
                        gress.Add(Int64.Parse(reader[0].ToString()));
                    }

                    List<Label> asd = new List<Label>();
                    asd.Add(label26);
                    asd.Add(label25);
                    asd.Add(label24);
                    asd.Add(label23);
                    asd.Add(label22);

                    for (int i = 0; i < kodepkt.Count; i++)
                    {
                        asd[i].Visible = true;
                        asd[i].Text = kodepkt[i];
                    }

                    //if (kodepkt[0].ToString() == null || kodepkt[0].ToString() == "")
                    //{

                    //}
                    //else
                    //{
                    //    label3.Visible = true;
                    //    label3.Text = kodepkt[0];

                    //    if (kodepkt[1].ToString() != null || kodepkt[1].ToString() != "")
                    //    {
                    //        label4.Visible = true;
                    //        label4.Text = kodepkt[1];
                    //    }
                    //    if (kodepkt[2].ToString() != null || kodepkt[2].ToString() != "")
                    //    {
                    //        label5.Visible = true;
                    //        label5.Text = kodepkt[2];
                    //    }
                    //    if (kodepkt[3].ToString() != null || kodepkt[3].ToString() != "")
                    //    {
                    //        label6.Visible = true;
                    //        label6.Text = kodepkt[3];
                    //    }
                    //    if (kodepkt[4].ToString() != null || kodepkt[4].ToString() != "")
                    //    {
                    //        label7.Visible = true;
                    //        label7.Text = kodepkt[4];
                    //    }
                    //}



                    //label3.Text = kodepkt[0];
                    //label4.Text = kodepkt[1];
                    //label5.Text = kodepkt[2];
                    //label6.Text = kodepkt[3];
                    //label7.Text = kodepkt[4];

                }
            }
        }

        private void reloadUangKecilSum()
        {
            Int64 umm = 0;
            Int64 gress = 0;
            Int64 unproc = 0;

            label30.Visible = false;
            label33.Visible = false;
            label29.Visible = false;

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select [umm] =  sum(unfitBaru + unfitLama + unfitNKRI + RRMBaru + RRMLama + RRMNKRI + RupiahRusakMayor) , [gress] = sum(newBaru + newLama), [unproc] = sum(unprocessed) "
                                      + " from StokPosisi s join Pkt p on s.namaPkt = p.namaPkt"
                                      + " where (denom != 100000 AND denom != 50000)  and day(tanggal) = " + comboTanggal.SelectedValue.ToString() + " and month(tanggal) = " + comboBulan.SelectedValue.ToString() + " and year(tanggal) = " + comboTahun.SelectedValue.ToString() + " ";
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        umm = (Int64.Parse(reader[0].ToString()));
                        gress = (Int64.Parse(reader[1].ToString()));
                        unproc = (Int64.Parse(reader[2].ToString()));

                    }

                    if (umm != 0)
                    {
                        label30.Visible = true;
                        label30.Text = umm.ToString("C", CultureInfo.GetCultureInfo("id-ID"));
                    }
                    if (gress != 0)
                    {
                        label29.Visible = true;
                        label29.Text = gress.ToString("C", CultureInfo.GetCultureInfo("id-ID"));
                    }
                    if (unproc != 0)
                    {
                        label33.Visible = true;
                        label33.Text = unproc.ToString("C", CultureInfo.GetCultureInfo("id-ID"));
                    }


                }
            }
        }


        private void comboTahun_SelectionChangeCommitted(object sender, EventArgs e)
        {
            reloadBulan();
            reloadTanggal();
            reloadUbVsUk();
            reloadStokSaldoCoj();
            reloadSebaranSaldoCoj();
            reloadKomposisiUbVsUk();
            reloadUangBesar();
            reloadUangKecil();
            reloadTop5UangBesar();
            reloadUangBesarSum();
            reloadTop5UangKecil();
            reloadUangKecilSum();
            reloadLabelTotalCoj();
        }

        private void comboBulan_SelectionChangeCommitted(object sender, EventArgs e)
        {
            reloadTanggal();
            reloadUbVsUk();
            reloadStokSaldoCoj();
            reloadSebaranSaldoCoj();
            reloadKomposisiUbVsUk();
            reloadUangBesar();
            reloadUangKecil();
            reloadTop5UangBesar();
            reloadUangBesarSum();
            reloadTop5UangKecil();
            reloadUangKecilSum();
            reloadLabelTotalCoj();
        }

        private void comboTanggal_SelectionChangeCommitted(object sender, EventArgs e)
        {
            reloadUbVsUk();
            reloadStokSaldoCoj();
            reloadSebaranSaldoCoj();
            reloadKomposisiUbVsUk();
            reloadUangBesar();
            reloadUangKecil();
            reloadTop5UangBesar();
            reloadUangBesarSum();
            reloadTop5UangKecil();
            reloadUangKecilSum();
            reloadLabelTotalCoj();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void DashboardCOJForm_Load(object sender, EventArgs e)
        {

        }

        private void label27_Click(object sender, EventArgs e)
        {

        }

        private void label28_Click(object sender, EventArgs e)
        {

        }
    }
}
