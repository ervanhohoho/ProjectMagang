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
                                        +" where kanwil = '"+comboKanwil.SelectedValue.ToString()+"' and year(tanggal) = "+comboTahun.SelectedValue.ToString()+" and month(tanggal) = "+comboBulan.SelectedValue.ToString()+""
                                        +" group by TransaksiAtms.kodePkt";
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
                    cmd.CommandText = "select transaksiAtms.kodePkt ,[Sislok ATMPersen] = sum( case when(isiATM100 + isiATM20 + isiATM50 )!=0 then (sislokATM100+sislokATM20+sislokATM50)/(isiATM100 + isiATM20 + isiATM50 )else 0 end)"
                                      +" from TransaksiAtms join Pkt on TransaksiAtms.kodePkt = Pkt.kodePkt"
                                      + " where kanwil = '" + comboKanwil.SelectedValue.ToString() + "' and year(tanggal) = " + comboTahun.SelectedValue.ToString() + " and month(tanggal) = " + comboBulan.SelectedValue.ToString() + ""
                                      + " group by TransaksiAtms.kodePkt";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        sap.Add(new SislokATMPersen
                        {
                            kodePkt = reader[0].ToString(),
                            sislokATMPersen = (Int64)reader[1]
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
            public Int64 sislokATMPersen { set; get; }
        }

        private void comboKanwil_SelectionChangeCommitted(object sender, EventArgs e)
        {
            reloadTambahan1();
            reloadTambahan2();
        }

        private void comboTahun_SelectionChangeCommitted(object sender, EventArgs e)
        {
            reloadBulan();
            reloadTambahan1();
            reloadTambahan2();
        }

        private void comboBulan_SelectionChangeCommitted(object sender, EventArgs e)
        {
            reloadTambahan1();
            reloadTambahan2();
        }
    }
}
