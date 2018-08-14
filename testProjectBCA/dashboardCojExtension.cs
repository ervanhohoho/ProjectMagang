﻿using System;
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
    public partial class dashboardCojExtension : Form
    {
        Database1Entities en = new Database1Entities();
        public dashboardCojExtension()
        {
            InitializeComponent();
            reloadArea();
            reloadnasional();
            reloadPieChartSaldoPerGroup();
            comboBox1.Visible = false;
        }
   
        public void reloadnasional()
        {
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    List<TotalNasional> total = new List<TotalNasional>();

                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select sum(unprocessed + newBaru + newLama+ fitBaru+ fitLama+ passThrough+ unfitBaru+ unfitLama+ unfitNKRI+ RRMBaru + RRMLama + RupiahRusakMayor + cekLaporan ),namaPkt"
                                      +" from stokposisi where tanggal between '"+dateTimePicker1.Value.ToShortDateString()+"' and '"+dateTimePicker2.Value.ToShortDateString()+"'"
                                      +" group by namaPkt";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        total.Add(new TotalNasional
                        {
                            pkt = reader[1].ToString(),
                            nominalTotal = Int64.Parse(reader[0].ToString())
                        });
                    }

                    dataGridView1.DataSource = total;
                }
            }
        }
        public void reloadNonJabo()
        {
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    List<TotalNonJabo> total = new List<TotalNonJabo>();

                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select sum(unprocessed + newBaru + newLama+ fitBaru+ fitLama+ passThrough+ unfitBaru+ unfitLama+ unfitNKRI+ RRMBaru + RRMLama + RupiahRusakMayor + cekLaporan ),namaPkt"
                                      + " from stokposisi where tanggal between '" + dateTimePicker1.Value.ToShortDateString() + "' and '" + dateTimePicker2.Value.ToShortDateString() + "'"
                                      + " group by namaPkt ";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        total.Add(new TotalNonJabo
                        {
                            pkt = reader[1].ToString(),
                            nominalTotal = Int64.Parse(reader[0].ToString())
                        });
                    }

                    dataGridView1.DataSource = total;
                }
            }
        }

        public void reloadPieChartSaldoPerGroup()
        {
            pieChart1.Series.Clear();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    List<PieChartTotalperVendor> pie = new List<PieChartTotalperVendor>();
               
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select sum(unprocessed + newBaru + newLama+ fitBaru+ fitLama+ passThrough+ unfitBaru+ unfitLama+ unfitNKRI+ RRMBaru + RRMLama + RupiahRusakMayor + cekLaporan ), p.vendor"
                                      +" from stokposisi s join pkt p on s.namapkt = p.namapkt where tanggal between '"+dateTimePicker3.Value.ToString() +"' and '"+dateTimePicker4.Value.ToString()+"'"
                                      +" group by  p.vendor order by p.vendor";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        pie.Add(new PieChartTotalperVendor
                        {
                            nominalTotal = Int64.Parse(reader[0].ToString()),
                            vendor = reader[1].ToString()
                        });
                    }

                    Func<ChartPoint, string> labelPoint = chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);

                    List<PieSeries> ps = new List<PieSeries>();

                    foreach (var item in pie)
                    {
                        ps.Add(new PieSeries
                        {
                            Title = item.vendor,
                            Values =new ChartValues<Int64>
                            {
                                item.nominalTotal
                            },
                            DataLabels = true,
                            LabelPoint = p => (Math.Round((p.Y / 1000000000), 0)).ToString() + " M",
                        });
                    }

                    pieChart1.Series.AddRange(ps);

                    pieChart1.LegendLocation = LegendLocation.Bottom;

                }
            }
        }

        public void reloadArea()
        {
            List<String> area = new List<String>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    

                    area.Add("Nasional");
                    area.Add("Non-Jabo");

                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select distinct kanwil from pkt";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        area.Add(reader[0].ToString());
                    }
                    
                }
            }
            comboBox1.DataSource = area;
        }
        class TotalNasional
        {
            public String pkt { set; get; }
            public Int64 nominalTotal { set; get; }
        }

        class TotalNonJabo
        {
            public String pkt { set; get; }
            public Int64 nominalTotal { set; get; }
        }

        public class PieChartTotalperVendor
        {
            public String vendor { set; get; }
            public Int64 nominalTotal { set; get; }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            reloadnasional();

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            reloadnasional();
        }

        private void dateTimePicker3_ValueChanged(object sender, EventArgs e)
        {
            reloadPieChartSaldoPerGroup();
        }

        private void dateTimePicker4_ValueChanged(object sender, EventArgs e)
        {
            reloadPieChartSaldoPerGroup();
        }
    }
}