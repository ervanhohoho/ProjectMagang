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
            List<String> comboAreaData = new List<String>();
            comboAreaData.Add("Nasional");
            comboAreaData.Add("Jabo");
            comboAreaData.Add("Non Jabo");

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select distinct kanwil from pkt p join transaksiatms t on p.kodepkt = t.kodepkt";
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        comboAreaData.Add(reader[0].ToString());
                    }
                    comboAreaData.RemoveAt(3);
                }
            }

            comboArea.DataSource = comboAreaData;
            comboArea.SelectedIndex = 0;
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

                    if (comboArea.SelectedIndex == 0)
                    {
                        //cmd.CommandText = "select "
                        //             + " kodePkt, "
                        //             + " [Rasio] = CAST(SUM(saldoAwal100 + saldoAwal50 + saldoAwal20) AS FLOAT)/(SUM(isiATM100+isiATM50+isiATM20+isiCRM100+isiCRM50+isiCRM20)),"
                        //             + " [Jumlah adhoc] = count(adhoc100+adhoc50+adhoc20),"
                        //             + " [Persen] = cast(sum(adhoc100+adhoc50+adhoc20)as float)/ cast(sum(adhoc100+adhoc50+adhoc20+bon100+bon50+bon20) as float)"
                        //             + " from"
                        //             + " transaksiatms"
                        //             + " where"
                        //             + " tanggal between '" + dateTimePicker1.Value.ToString() + "' and '" + dateTimePicker2.Value.ToString() + "'"
                        //             + " and(adhoc100 + adhoc20 + adhoc50) != 0"
                        //             + " group by"
                        //             + " kodePkt"
                        //             + " order by"
                        //             + " [Rasio]";

                        cmd.CommandText = "select "
                                          + " a.kodepkt,"
                                          + " b.[Rasio],"
                                          + " a.[jumlah adhoc],"
                                          + " c.[Persen]"
                                          + " FROM"
                                          + " ("
                                          + " SELECT kodePkt,"
                                          + " [jumlah adhoc] = SUM(adatidak)"
                                          + " FROM"
                                          + " (select kodepkt, tanggal, [adatidak] = case when adhoc100 + adhoc50 + adhoc20 = 0 then 0 else 1 end"
                                          + " from TransaksiAtms"
                                          + " where tanggal between '" + dateTimePicker1.Value.ToString() + "' and '" + dateTimePicker2.Value.ToString() + "'"
                                          + " )src"
                                          + " GROUP BY kodePkt"
                                          + " ) a join"
                                          + " ("
                                          + " select kodepkt,"
                                          + " [Rasio] =  case when(SUM(isiATM100 + isiATM50 + isiATM20 + isiCRM100 + isiCRM50 + isiCRM20)) = 0 then 0 else CAST(SUM(saldoAwal100 + saldoAwal50 + saldoAwal20) AS FLOAT) / (SUM(isiATM100 + isiATM50 + isiATM20 + isiCRM100 + isiCRM50 + isiCRM20)) end"
                                          + " from TransaksiAtms"
                                          + " where tanggal between '" + dateTimePicker1.Value.ToString() + "' and '" + dateTimePicker2.Value.ToString() + "'"
                                          + " group by kodepkt"
                                          + " ) b on a.kodePkt = b.kodePkt join"
                                          + " (select kodepkt,"
                                          + " [Persen] = case when cast(sum(adhoc100 + adhoc50 + adhoc20 + bon100 + bon50 + bon20) as float) = 0 then 0 else cast(sum(adhoc100 + adhoc50 + adhoc20) as float) / cast(sum(adhoc100 + adhoc50 + adhoc20 + bon100 + bon50 + bon20) as float) end"
                                          + " from TransaksiAtms"
                                          + " where tanggal between '" + dateTimePicker1.Value.ToString() + "' and '" + dateTimePicker2.Value.ToString() + "'"
                                          + " group by kodePkt)c on b.kodePkt = c.kodePkt  order by b.[Rasio] desc";

                    }
                    else if (comboArea.SelectedIndex == 1)
                    {
                        cmd.CommandText = "select "
                                         + " a.kodepkt,"
                                         + " b.[Rasio],"
                                         + " a.[jumlah adhoc],"
                                         + " c.[Persen]"
                                         + " FROM"
                                         + " ("
                                         + " SELECT kodePkt,"
                                         + " [jumlah adhoc] = SUM(adatidak)"
                                         + " FROM"
                                         + " (select p.kodepkt, tanggal, [adatidak] = case when adhoc100 + adhoc50 + adhoc20 = 0 then 0 else 1 end"
                                         + " from TransaksiAtms join Pkt p on TransaksiAtms.kodePkt = p.kodePkt"
                                         + " where tanggal between '" + dateTimePicker1.Value.ToString() + "' and '" + dateTimePicker2.Value.ToString() + "' and p.kanwil like 'Jabo%'"
                                         + " )src"
                                         + " GROUP BY kodePkt"
                                         + " ) a join"
                                         + " ("
                                         + " select p.kodepkt,"
                                         + " [Rasio] =  case when(SUM(isiATM100 + isiATM50 + isiATM20 + isiCRM100 + isiCRM50 + isiCRM20)) = 0 then 0 else CAST(SUM(saldoAwal100 + saldoAwal50 + saldoAwal20) AS FLOAT) / (SUM(isiATM100 + isiATM50 + isiATM20 + isiCRM100 + isiCRM50 + isiCRM20)) end"
                                         + " from TransaksiAtms  join Pkt p on TransaksiAtms.kodePkt = p.kodePkt"
                                         + " where tanggal between '" + dateTimePicker1.Value.ToString() + "' and '" + dateTimePicker2.Value.ToString() + "' and p.kanwil like 'Jabo%'"
                                         + " group by p.kodepkt"
                                         + " ) b on a.kodePkt = b.kodePkt join"
                                         + " (select p.kodepkt,"
                                         + " [Persen] = case when cast(sum(adhoc100 + adhoc50 + adhoc20 + bon100 + bon50 + bon20) as float) = 0 then 0 else cast(sum(adhoc100 + adhoc50 + adhoc20) as float) / cast(sum(adhoc100 + adhoc50 + adhoc20 + bon100 + bon50 + bon20) as float) end"
                                         + " from TransaksiAtms  join Pkt p on TransaksiAtms.kodePkt = p.kodePkt"
                                         + " where tanggal between '" + dateTimePicker1.Value.ToString() + "' and '" + dateTimePicker2.Value.ToString() + "' and p.kanwil like 'Jabo%'"
                                         + " group by p.kodePkt)c on b.kodePkt = c.kodePkt order by b.[Rasio] desc";

                        //cmd.CommandText = "select "
                        //             + " t.kodePkt, "
                        //             + " [Rasio] = CAST(SUM(saldoAwal100 + saldoAwal50 + saldoAwal20) AS FLOAT)/(SUM(isiATM100+isiATM50+isiATM20+isiCRM100+isiCRM50+isiCRM20)),"
                        //             + " [Jumlah adhoc] = count(adhoc100+adhoc50+adhoc20),"
                        //             + " [Persen] = cast(sum(adhoc100+adhoc50+adhoc20)as float)/ cast(sum(adhoc100+adhoc50+adhoc20+bon100+bon50+bon20) as float)"
                        //             + " from"
                        //             + " transaksiatms t join pkt p on t.kodepkt = p.kodepkt"
                        //             + " where"
                        //             + " tanggal between '" + dateTimePicker1.Value.ToString() + "' and '" + dateTimePicker2.Value.ToString() + "' and p.kanwil like 'Jabo%'"
                        //             + " and(adhoc100 + adhoc20 + adhoc50) != 0"
                        //             + " group by"
                        //             + " t.kodePkt"
                        //             + " order by"
                        //             + " [Rasio]";
                    }
                    else if (comboArea.SelectedIndex == 2)
                    {
                        cmd.CommandText = "select "
                                         + " a.kodepkt,"
                                         + " b.[Rasio],"
                                         + " a.[jumlah adhoc],"
                                         + " c.[Persen]"
                                         + " FROM"
                                         + " ("
                                         + " SELECT kodePkt,"
                                         + " [jumlah adhoc] = SUM(adatidak)"
                                         + " FROM"
                                         + " (select p.kodepkt, tanggal, [adatidak] = case when adhoc100 + adhoc50 + adhoc20 = 0 then 0 else 1 end"
                                         + " from TransaksiAtms join Pkt p on TransaksiAtms.kodePkt = p.kodePkt"
                                         + " where tanggal between '" + dateTimePicker1.Value.ToString() + "' and '" + dateTimePicker2.Value.ToString() + "' and p.kanwil not like 'Jabo%'"
                                         + " )src"
                                         + " GROUP BY kodePkt"
                                         + " ) a join"
                                         + " ("
                                         + " select p.kodepkt,"
                                         + " [Rasio] =  case when(SUM(isiATM100 + isiATM50 + isiATM20 + isiCRM100 + isiCRM50 + isiCRM20)) = 0 then 0 else CAST(SUM(saldoAwal100 + saldoAwal50 + saldoAwal20) AS FLOAT) / (SUM(isiATM100 + isiATM50 + isiATM20 + isiCRM100 + isiCRM50 + isiCRM20)) end"
                                         + " from TransaksiAtms  join Pkt p on TransaksiAtms.kodePkt = p.kodePkt"
                                         + " where tanggal between '" + dateTimePicker1.Value.ToString() + "' and '" + dateTimePicker2.Value.ToString() + "' and p.kanwil not like 'Jabo%'"
                                         + " group by p.kodepkt"
                                         + " ) b on a.kodePkt = b.kodePkt join"
                                         + " (select p.kodepkt,"
                                         + " [Persen] = case when cast(sum(adhoc100 + adhoc50 + adhoc20 + bon100 + bon50 + bon20) as float) = 0 then 0 else cast(sum(adhoc100 + adhoc50 + adhoc20) as float) / cast(sum(adhoc100 + adhoc50 + adhoc20 + bon100 + bon50 + bon20) as float) end"
                                         + " from TransaksiAtms  join Pkt p on TransaksiAtms.kodePkt = p.kodePkt"
                                         + " where tanggal between '" + dateTimePicker1.Value.ToString() + "' and '" + dateTimePicker2.Value.ToString() + "' and p.kanwil not like 'Jabo%'"
                                         + " group by p.kodePkt)c on b.kodePkt = c.kodePkt order by b.[Rasio] desc";

                        //cmd.CommandText = "select "
                        //             + " t.kodePkt, "
                        //             + " [Rasio] = CAST(SUM(saldoAwal100 + saldoAwal50 + saldoAwal20) AS FLOAT)/(SUM(isiATM100+isiATM50+isiATM20+isiCRM100+isiCRM50+isiCRM20)),"
                        //             + " [Jumlah adhoc] = count(adhoc100+adhoc50+adhoc20),"
                        //             + " [Persen] = cast(sum(adhoc100+adhoc50+adhoc20)as float)/ cast(sum(adhoc100+adhoc50+adhoc20+bon100+bon50+bon20) as float)"
                        //             + " from"
                        //             + " transaksiatms t join pkt p on t.kodepkt = p.kodepkt"
                        //             + " where"
                        //             + " tanggal between '" + dateTimePicker1.Value.ToString() + "' and '" + dateTimePicker2.Value.ToString() + "' and p.kanwil not like 'Jabo%'"
                        //             + " and(adhoc100 + adhoc20 + adhoc50) != 0"
                        //             + " group by"
                        //             + " t.kodePkt"
                        //             + " order by"
                        //             + " [Rasio]";
                    }
                    else
                    {
                        cmd.CommandText = "select "
                                         + " a.kodepkt,"
                                         + " b.[Rasio],"
                                         + " a.[jumlah adhoc],"
                                         + " c.[Persen]"
                                         + " FROM"
                                         + " ("
                                         + " SELECT kodePkt,"
                                         + " [jumlah adhoc] = SUM(adatidak)"
                                         + " FROM"
                                         + " (select p.kodepkt, tanggal, [adatidak] = case when adhoc100 + adhoc50 + adhoc20 = 0 then 0 else 1 end"
                                         + " from TransaksiAtms join Pkt p on TransaksiAtms.kodePkt = p.kodePkt"
                                         + " where tanggal between '" + dateTimePicker1.Value.ToString() + "' and '" + dateTimePicker2.Value.ToString() + "' and p.kanwil like '" + comboArea.SelectedValue.ToString() + "' "
                                         + " )src"
                                         + " GROUP BY kodePkt"
                                         + " ) a join"
                                         + " ("
                                         + " select p.kodepkt,"
                                         + " [Rasio] =  case when(SUM(isiATM100 + isiATM50 + isiATM20 + isiCRM100 + isiCRM50 + isiCRM20)) = 0 then 0 else CAST(SUM(saldoAwal100 + saldoAwal50 + saldoAwal20) AS FLOAT) / (SUM(isiATM100 + isiATM50 + isiATM20 + isiCRM100 + isiCRM50 + isiCRM20)) end"
                                         + " from TransaksiAtms  join Pkt p on TransaksiAtms.kodePkt = p.kodePkt"
                                         + " where tanggal between '" + dateTimePicker1.Value.ToString() + "' and '" + dateTimePicker2.Value.ToString() + "' and p.kanwil like '" + comboArea.SelectedValue.ToString() + "' "
                                         + " group by p.kodepkt"
                                         + " ) b on a.kodePkt = b.kodePkt join"
                                         + " (select p.kodepkt,"
                                         + " [Persen] = case when cast(sum(adhoc100 + adhoc50 + adhoc20 + bon100 + bon50 + bon20) as float) = 0 then 0 else cast(sum(adhoc100 + adhoc50 + adhoc20) as float) / cast(sum(adhoc100 + adhoc50 + adhoc20 + bon100 + bon50 + bon20) as float) end"
                                         + " from TransaksiAtms  join Pkt p on TransaksiAtms.kodePkt = p.kodePkt"
                                         + " where tanggal between '" + dateTimePicker1.Value.ToString() + "' and '" + dateTimePicker2.Value.ToString() + "' and p.kanwil like '" + comboArea.SelectedValue.ToString() + "' "
                                         + " group by p.kodePkt)c on b.kodePkt = c.kodePkt order by b.[Rasio] desc";

                        //cmd.CommandText = "select "
                        //             + " t.kodePkt, "
                        //             + " [Rasio] = CAST(SUM(saldoAwal100 + saldoAwal50 + saldoAwal20) AS FLOAT)/(SUM(isiATM100+isiATM50+isiATM20+isiCRM100+isiCRM50+isiCRM20)),"
                        //             + " [Jumlah adhoc] = count(adhoc100+adhoc50+adhoc20),"
                        //             + " [Persen] = cast(sum(adhoc100+adhoc50+adhoc20)as float)/ cast(sum(adhoc100+adhoc50+adhoc20+bon100+bon50+bon20) as float)"
                        //             + " from"
                        //             + " transaksiatms t join pkt p on t.kodepkt = p.kodepkt"
                        //             + " where"
                        //             + " tanggal between '" + dateTimePicker1.Value.ToString() + "' and '" + dateTimePicker2.Value.ToString() + "' and p.kanwil like '"+comboArea.SelectedValue.ToString()+"' "
                        //             + " and(adhoc100 + adhoc20 + adhoc50) != 0"
                        //             + " group by"
                        //             + " t.kodePkt"
                        //             + " order by"
                        //             + " [Rasio]";
                    }


                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        per.Add(new performancePkt
                        {
                            pkt = reader[0].ToString(),
                            ratio = Math.Round(double.Parse(reader[1].ToString()), 1),
                            frekuensiAdhoc = Int64.Parse(reader[2].ToString()),
                            persenAdhoc = Math.Round(double.Parse(reader[3].ToString()) * 100, 0).ToString() + " %"
                        });
                    }
                    dataGridView1.DataSource = per;
                    if (dataGridView1.Rows.Count != 0)
                    {
                        for (int i = 0; i < dataGridView1.Rows.Count; i++)
                        {
                            if (Double.Parse(dataGridView1.Rows[i].Cells[1].Value.ToString()) >= 1.2)
                            {
                                dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                            }
                        }

                    }
                }
            }
        }

        class performancePkt
        {
            public String pkt { set; get; }
            public double ratio { set; get; }
            public Int64 frekuensiAdhoc { set; get; }
            public String persenAdhoc { set; get; }

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            reloadDataPerformancePkt();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            reloadDataPerformancePkt();
        }

        private void comboArea_SelectionChangeCommitted(object sender, EventArgs e)
        {
            reloadDataPerformancePkt();
        }
    }
}