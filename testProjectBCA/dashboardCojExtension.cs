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
    public partial class dashboardCojExtension : Form
    {
        Database1Entities en = new Database1Entities();
        public dashboardCojExtension()
        {
            InitializeComponent();
            reloadArea();
            reloadnasional();
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

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            reloadnasional();

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            reloadnasional();
        }
    }
}
