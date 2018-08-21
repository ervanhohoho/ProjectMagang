using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;

namespace testProjectBCA
{
    public partial class graphSaldoATM : Form
    {
        public graphSaldoATM()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox1.Visible = false;
            reloadSaldoAwal();
        }

        public void reloadSaldoAwal()
        {
            //cartesianChart1.AxisX.Clear();
            //cartesianChart1.AxisY.Clear();
            //cartesianChart1.Series.Clear();

            List<allInOne> aio = new List<allInOne>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    if (comboBox2.SelectedIndex == 0)
                    {
                        cmd.CommandText = "select"
                                          + " tanggal,"
                                          + " [saldoAwal] = sum(saldoAwal100+saldoAwal20+saldoAkhir50),"
                                          + " [sislokCrm] = sum(SislokCRM20 + sislokCRM50 + sislokCRM100),"
                                          + " [sislokAtmPersen] =  cast(sum(SislokAtm100 + SislokAtm50 + SislokAtm20) as float) / cast(sum(IsiAtm100 + IsiAtm50 + IsiAtm20)as float),"
                                          + " [totalIsi] = sum(IsiAtm20 + IsiAtm50 + IsiAtm100 + IsiCrm20 + IsiCRm50 + IsiCrm100)"
                                          + " from"
                                          + " TransaksiAtms t join pkt p on t.kodePkt = p.kodePkt"
                                          + " where"
                                          + " tanggal between '"+dateTimePicker1.Value.ToString()+"' and '"+dateTimePicker2.Value.ToString()+"' "
                                          + " group by"
                                          + " tanggal order by tanggal";
                    }
                    else if(comboBox2.SelectedIndex == 1)
                    {
                        cmd.CommandText = "select"
                                          + " tanggal,"
                                          + " [saldoAwal] = sum(saldoAwal100+saldoAwal20+saldoAkhir50),"
                                          + " [sislokCrm] = sum(SislokCRM20 + sislokCRM50 + sislokCRM100),"
                                          + " [sislokAtmPersen] =  cast(sum(SislokAtm100 + SislokAtm50 + SislokAtm20) as float) / cast(sum(IsiAtm100 + IsiAtm50 + IsiAtm20)as float),"
                                          + " [totalIsi] = sum(IsiAtm20 + IsiAtm50 + IsiAtm100 + IsiCrm20 + IsiCRm50 + IsiCrm100)"
                                          + " from"
                                          + " TransaksiAtms t join pkt p on t.kodePkt = p.kodePkt"
                                          + " where"
                                          + " tanggal between '" + dateTimePicker1.Value.ToString() + "' and '" + dateTimePicker2.Value.ToString() + "' and kanwil like 'Jabo%' "
                                          + " group by"
                                          + " tanggal order by tanggal";
                    }
                    else if (comboBox2.SelectedIndex == 2)
                    {
                        cmd.CommandText = "select"
                                          + " tanggal,"
                                          + " [saldoAwal] = sum(saldoAwal100+saldoAwal20+saldoAkhir50),"
                                          + " [sislokCrm] = sum(SislokCRM20 + sislokCRM50 + sislokCRM100),"
                                          + " [sislokAtmPersen] =  cast(sum(SislokAtm100 + SislokAtm50 + SislokAtm20) as float) / cast(sum(IsiAtm100 + IsiAtm50 + IsiAtm20)as float),"
                                          + " [totalIsi] = sum(IsiAtm20 + IsiAtm50 + IsiAtm100 + IsiCrm20 + IsiCRm50 + IsiCrm100)"
                                          + " from"
                                          + " TransaksiAtms t join pkt p on t.kodePkt = p.kodePkt"
                                          + " where"
                                          + " tanggal between '" + dateTimePicker1.Value.ToString() + "' and '" + dateTimePicker2.Value.ToString() + "' and kanwil not like 'Jabo%' "
                                          + " group by"
                                          + " tanggal order by tanggal";
                    }
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        aio.Add(new allInOne
                        {
                            tanggal = DateTime.Parse(reader[0].ToString()).Date,
                            saldoAwal = Int64.Parse(reader[1].ToString()),
                            sislokCRM = Int64.Parse(reader[2].ToString()),
                            sislokATM = Math.Round(Double.Parse(reader[3].ToString())*100,2).ToString() + " %",
                            totalIsi = Int64.Parse(reader[4].ToString())
                        });
                    }
                    dataGridView1.DataSource = aio;

                    //ChartValues<Int64> cvSaldoAwal = new ChartValues<Int64>();
                    //foreach (var item in aio)
                    //{
                    //    cvSaldoAwal.Add(item.saldoAwal);
                    //}
                    //ChartValues<Int64> cvSislokCrm = new ChartValues<Int64>();
                    //foreach (var item in aio)
                    //{
                    //    cvSislokCrm.Add(item.sislokCRM);
                    //}
                    //ChartValues<Double> cvSislokAtm = new ChartValues<Double>();
                    //foreach (var item in aio)
                    //{
                    //    cvSislokAtm.Add(item.sislokATM * 100);
                    //}
                    //ChartValues<Int64> cvTotalIsi = new ChartValues<Int64>();
                    //foreach (var item in aio)
                    //{
                    //    cvTotalIsi.Add(item.totalIsi);
                    //}


                    //if (comboBox1.SelectedIndex == 0)
                    //{
                    //    cartesianChart1.Series = new SeriesCollection
                    //    {
                            
                    //        new LineSeries
                    //        {
                    //            Title = "Saldo Awal",
                    //            Values = cvSaldoAwal,
                    //            DataLabels = true

                    //        }
                    //    };
                    //    cartesianChart1.AxisY.Add(new Axis
                    //    {

                    //        LabelFormatter = value => (Math.Round(value/1000000000,1)).ToString() + " M",
                    //        MinValue = 0
                    //    });
                    //}
                    //else if (comboBox1.SelectedIndex == 1)
                    //{
                    //    cartesianChart1.Series = new SeriesCollection
                    //    {

                    //        new LineSeries
                    //        {
                    //            Title = "Sislok CRM Nominal",
                    //            Values = cvSislokCrm,
                    //            DataLabels = true
                    //        }
                    //    };
                    //    cartesianChart1.AxisY.Add(new Axis
                    //    {

                    //        LabelFormatter = value => (Math.Round(value / 1000000000, 1)).ToString() + " M",
                    //        MinValue = 0
                    //    });
                    //}
                    //else if (comboBox1.SelectedIndex ==  2)
                    //{
                    //    cartesianChart1.Series = new SeriesCollection
                    //    {

                    //        new LineSeries
                    //        {
                    //            Title = "Sislok ATM Persen",
                    //            Values = cvSislokAtm,
                    //            DataLabels = true
                    //        }
                    //    };
                    //    cartesianChart1.AxisY.Add(new Axis
                    //    {

                    //        LabelFormatter = value => (value).ToString() + " %",
                    //        MinValue = 0
                    //    });
                    //}
                    //else if (comboBox1.SelectedIndex == 3)
                    //{
                    //    cartesianChart1.Series = new SeriesCollection
                    //    {

                    //        new LineSeries
                    //        {
                    //            Title = "Total Isi",
                    //            Values = cvTotalIsi,
                    //            DataLabels = true
                    //        }
                    //    };
                    //    cartesianChart1.AxisY.Add(new Axis
                    //    {

                    //        LabelFormatter = value => (Math.Round(value / 1000000000, 1)).ToString() + " M",
                    //        MinValue = 0
                    //    });
                    //}
                    //cartesianChart1.AxisX.Add(new Axis
                    //{
                    //    Title = "Tanggal",
                    //    Labels = aio.Select(x => x.tanggal.ToShortDateString()).ToList(),
                    //    Separator = new Separator
                    //    {
                    //        Step = 1
                    //    }
                    //    ,
                    //    LabelsRotation = 30
                    //});
                }
            }
        }
        
        public class allInOne
        {
            public DateTime tanggal { set; get; }
            public Int64 saldoAwal { set; get; }
            public Int64 sislokCRM { set; get; }
            public String sislokATM { set; get; }
            public Int64 totalIsi { set; get; }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            reloadSaldoAwal();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            reloadSaldoAwal();
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            reloadSaldoAwal();
        }

        private void comboBox2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            reloadSaldoAwal();
        }
    }
}
