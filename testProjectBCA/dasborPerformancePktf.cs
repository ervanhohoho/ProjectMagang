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
    public partial class dasborPerformancePktf : Form
    {
        public dasborPerformancePktf()
        {
            InitializeComponent();
            reloadDataPerformancePkt();
        }

        public void reloadDataPerformancePkt()
        {
            List<performancePkt> per = new List<performancePkt>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select "
                                     +" kodePkt, "
                                     +" [Rasio] = CAST(SUM(saldoAwal100 + saldoAwal50 + saldoAwal20) AS FLOAT)/(SUM(isiATM100+isiATM50+isiATM20+isiCRM100+isiCRM50+isiCRM20)),"
                                     +" [Jumlah adhoc] = count(adhoc100+adhoc50+adhoc20),"
                                     +" [Persen] = cast(sum(adhoc100+adhoc50+adhoc20)as float)/ cast(sum(adhoc100+adhoc50+adhoc20+bon100+bon50+bon20) as float)"
                                     +" from"
                                     +" transaksiatms"
                                     +" where"
                                     +" tanggal between '"+dateTimePicker1.Value.ToString()+"' and '"+dateTimePicker2.Value.ToString()+"'"
                                     +" and(adhoc100 + adhoc20 + adhoc50) != 0"
                                     +" group by"
                                     +" kodePkt"
                                     +" order by"
                                     +" [Rasio]";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        per.Add(new performancePkt
                        {
                            pkt = reader[0].ToString(),
                            ratio = Math.Round(double.Parse(reader[1].ToString()),1),
                            frekuensiAdhoc = Int64.Parse(reader[2].ToString()),
                            persenAdhoc = Math.Round(double.Parse(reader[3].ToString()),1)
                        });
                    }
                    dataGridView1.DataSource = per;
                    if (dataGridView1.Rows.Count != 0)
                    {
                        dataGridView1.Rows[0].DefaultCellStyle.BackColor = Color.Yellow;
                        dataGridView1.Rows[1].DefaultCellStyle.BackColor = Color.Yellow;
                    }
                }
            }
        }

        class performancePkt
        {
            public String pkt { set; get; }
            public double ratio { set; get; }
            public Int64 frekuensiAdhoc { set; get; }
            public double persenAdhoc { set; get; }

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            reloadDataPerformancePkt();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            reloadDataPerformancePkt();
        }
    }
}
