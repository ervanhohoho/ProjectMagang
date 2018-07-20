using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA
{
    public partial class dasborExtension : Form
    {
        public dasborExtension()
        {
            InitializeComponent();
            reloadKanwil();
            reloadTahun();
            reloadBulan();
            reloadTambahan1();
            reloadTambahan2();
            reloadTambahan3();
            reloadTambahan4();
            comboBulan.Visible = false;
            comboTahun.Visible = false;
            comboBox1.Visible = false;
            comboBox2.Visible = false;
        }

        private void reloadKanwil()
        {
            List<String> kanwil = new List<String>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select distinct kanwil from pkt";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        kanwil.Add(reader[0].ToString());
                    }
                    comboKanwil.DataSource = kanwil;
                }
            }
        }

        private void reloadTahun()
        {
            List<String> tahun = new List<String>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select distinct year(tanggal) from TransaksiAtms order by year(tanggal) desc ";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        tahun.Add(reader[0].ToString());
                    }
                    comboTahun.DataSource = tahun;
                }
            }
        }

        private void reloadBulan()
        {
            List<String> bulan = new List<String>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select distinct month(tanggal) from TransaksiAtms where year(tanggal) = "+comboTahun.SelectedValue.ToString()+" order by month(tanggal) desc ";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        bulan.Add(reader[0].ToString());
                    }
                    comboBulan.DataSource = bulan;
                }
            }
        }

        private void reloadTahunSampai()
        {
            List<String> tahun = new List<String>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select distinct year(tanggal) from TransaksiAtms order by year(tanggal) desc ";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        tahun.Add(reader[0].ToString());
                    }
                    comboBox2.DataSource = tahun;
                }
            }
        }

        private void reloadBulanSampai()
        {
            List<String> bulan = new List<String>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select distinct month(tanggal) from TransaksiAtms where year(tanggal) = " + comboTahun.SelectedValue.ToString() + " order by month(tanggal) desc ";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        bulan.Add(reader[0].ToString());
                    }
                    comboBox1.DataSource = bulan;
                }
            }
        }

        private void reloadTambahan3()
        {
            List<SislokCRMAvg> sca = new List<SislokCRMAvg>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select transaksiatms.kodepkt, avg(sislokcrm100+sislokcrm50+sislokcrm20) from transaksiatms join Pkt on TransaksiAtms.kodePkt = Pkt.kodePkt"
                                      + " where tanggal between '" + dateTimePicker1.Value.ToShortDateString() + "' and '" + dateTimePicker2.Value.ToShortDateString() + "' and kanwil = '"+ comboKanwil.SelectedValue.ToString() +"'"
                                      + " group by transaksiatms.kodepkt";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        sca.Add(new SislokCRMAvg
                        {
                            kodePkt = reader[0].ToString(),
                            sislokCRMAvg = Int64.Parse(reader[1].ToString())
                        });
                    }
                    dataGridView3.DataSource = sca;

                    for (int i = 0; i < dataGridView3.Columns.Count; i++)
                    {
                        dataGridView3.Columns[i].DefaultCellStyle.Format = "c0";
                        dataGridView3.Columns[i].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
                    }

                }
            }

        }

        private void reloadTambahan4()
        {
            List<SislokATMAvg> sta = new List<SislokATMAvg>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select transaksiatms.kodepkt, avg(sislokatm100+sislokatm50+sislokatm20) from transaksiatms join Pkt on TransaksiAtms.kodePkt = Pkt.kodePkt"
                                      + " where tanggal between '" + dateTimePicker1.Value.ToShortDateString() + "' and '" + dateTimePicker2.Value.ToShortDateString() + "' and kanwil = '" + comboKanwil.SelectedValue.ToString() + "'"
                                      + " group by transaksiatms.kodepkt";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        sta.Add(new SislokATMAvg
                        {
                            kodePkt = reader[0].ToString(),
                            sislokATMAvg = Int64.Parse(reader[1].ToString())
                        });
                    }
                    dataGridView4.DataSource = sta;

                    for (int i = 0; i < dataGridView4.Columns.Count; i++)
                    {
                        dataGridView4.Columns[i].DefaultCellStyle.Format = "c0";
                        dataGridView4.Columns[i].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
                    }

                }
            }

        }

        private void reloadTambahan1()
        {

            List<SislokCRM> sc = new List<SislokCRM>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select transaksiAtms.kodePkt ,[Sislok CRM] = sum(sislokCRM100+sislokCRM20+sislokCRM50)"
                                        +" from TransaksiAtms join Pkt on TransaksiAtms.kodePkt = Pkt.kodePkt"
                                        //+" where kanwil = '"+comboKanwil.SelectedValue.ToString()+"' and year(tanggal) = "+comboTahun.SelectedValue.ToString()+" and month(tanggal) = "+comboBulan.SelectedValue.ToString()+""
                                        + " where kanwil = '" + comboKanwil.SelectedValue.ToString() + "' and tanggal between  '" + dateTimePicker1.Value.ToShortDateString() + "' and '" + dateTimePicker2.Value.ToShortDateString() + "'"
                                        + " group by TransaksiAtms.kodePkt";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        sc.Add(new SislokCRM
                        {
                            kodePkt = reader[0].ToString(),
                            sislokCRM = (Int64)reader[1]
                        });
                    }
                    dataGridView2.DataSource = sc;

                    for (int i = 0; i < dataGridView2.Columns.Count; i++)
                    {
                        dataGridView2.Columns[i].DefaultCellStyle.Format = "c0";
                        dataGridView2.Columns[i].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
                    }

                }
            }
        }

        private void reloadTambahan2()
        {
            List<SislokATMPersen> sap = new List<SislokATMPersen>();

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select transaksiAtms.kodePkt ,[Sislok ATMPersen] = CASE WHEN SUM(isiATM100+isiATM50+isiATM20) > 0 THEN CAST(sum(sislokATM100 + sislokATM50 + sislokATM20) AS float)/ sum(isiATM100 + isiATM50 + isiATM20) ELSE 0 END"
                                      + " from TransaksiAtms join Pkt on TransaksiAtms.kodePkt = Pkt.kodePkt"
                                      + " where kanwil = '" + comboKanwil.SelectedValue.ToString() + "' and tanggal between  '" + dateTimePicker1.Value.ToShortDateString() + "' and '" + dateTimePicker2.Value.ToShortDateString() + "'"
                                      //+ " where kanwil = '" + comboKanwil.SelectedValue.ToString() + "' and year(tanggal) = " + comboTahun.SelectedValue.ToString() + " and month(tanggal) = " + comboBulan.SelectedValue.ToString() + ""
                                      + " group by TransaksiAtms.kodePkt";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        sap.Add(new SislokATMPersen
                        {
                            kodePkt = reader[0].ToString(),
                            sislokATMPersen = (Math.Round(Double.Parse(reader[1].ToString()),3) * 100).ToString() + " %"
                        });
                    }
                    dataGridView1.DataSource = sap;
                }
            }
        }

        class SislokCRM
        {
            public String kodePkt { set; get; }
            public Int64 sislokCRM { set; get; }

        }

        class SislokATMPersen
        {
            public String kodePkt { set; get; }
            public String sislokATMPersen { set; get; }
        }

        class SislokCRMAvg
        {
            public String kodePkt { set; get; }
            public Int64 sislokCRMAvg { set; get; }
        }
        
        class SislokATMAvg
        {
            public String kodePkt { set; get; }
            public Int64 sislokATMAvg { set; get; }
        }
             

        private void comboKanwil_SelectionChangeCommitted(object sender, EventArgs e)
        {
            reloadTambahan1();
            reloadTambahan2();
            reloadTambahan3();
            reloadTambahan4();
        }

        private void comboTahun_SelectionChangeCommitted(object sender, EventArgs e)
        {
            reloadBulan();
            reloadTambahan1();
            reloadTambahan2();
            reloadTambahan3();
            reloadTambahan4();
        }

        private void comboBulan_SelectionChangeCommitted(object sender, EventArgs e)
        {
            reloadTambahan1();
            reloadTambahan2();
            reloadTambahan3();
            reloadTambahan4();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            reloadTambahan1();
            reloadTambahan2();
            reloadTambahan3();
            reloadTambahan4();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            reloadTambahan1();
            reloadTambahan2();
            reloadTambahan3();
            reloadTambahan4();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
