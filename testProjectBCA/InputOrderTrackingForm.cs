using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA
{
    public partial class InputOrderTrackingForm : Form
    {
        Database1Entities en = new Database1Entities();
        public InputOrderTrackingForm()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.excelFilter;
          

            if (of.ShowDialog() == DialogResult.OK)
            {
                String filename = of.FileName;
                DataSet ds = Util.openExcel(filename);
                DataTable dt = ds.Tables[0];
                Console.WriteLine(ds.Tables.Count);
                dataGridView1.DataSource = dt;

                var query = (from x in en.OrderTrackings
                             where x.tanggal == dateTimePicker1.Value.Date
                             select x).FirstOrDefault();

                if (query != null)
                {
                    MessageBox.Show("Tanggal " + dateTimePicker1.Value.Date.ToShortDateString() + " Sudah Ada Di Database");
                }
                else
                {
                    for (int i = 1; i < dt.Rows.Count; i++)
                    {
                        Console.WriteLine(i);
                        Console.WriteLine(dt.Rows[i][1].ToString());
                        Console.WriteLine(dt.Rows[i][4].ToString());

                        String kodePkt = "";
                        String kodeCabang = "";
                        DateTime tanggal = new DateTime(1, 1, 1);
                        Int64 nominalDispute = 0;
                        var rows = dt.Select("");
                        if (dt.Rows[i][13].ToString() == null || dt.Rows[i][13].ToString() == "")
                        {
                            continue;
                        }
                        else
                        {
                            if (dt.Rows[i][19].ToString() == null || dt.Rows[i][19].ToString() == "")
                            {
                                continue;
                            }
                            else
                            {
                                if (Convert.ToDateTime(dt.Rows[i][4].ToString()).Date == dateTimePicker1.Value.Date && dt.Rows[i][13].ToString() == "CONFIRMED")
                                {
                                    Console.WriteLine("cus");
                                    kodePkt = dt.Rows[i][0].ToString();
                                    kodeCabang = dt.Rows[i][2].ToString();
                                    tanggal = Convert.ToDateTime(dt.Rows[i][4].ToString()).Date;
                                    nominalDispute = Int64.Parse(dt.Rows[i][19].ToString().Split(':')[1].Trim('\"'));
                                }
                                else
                                {
                                    Console.WriteLine("sor");
                                    continue;
                                }
                                en.OrderTrackings.Add(new OrderTracking()
                                {
                                    kodePkt = kodePkt,
                                    kodeCabang = kodeCabang,
                                    tanggal = tanggal,
                                    nominalDispute = nominalDispute
                                });
                                en.SaveChanges();
                            }

                        }


                    }
                }
                

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.excelFilter;

            if (of.ShowDialog() == DialogResult.OK)
            {
                String filename = of.FileName;
                DataSet ds = Util.openExcel(filename);
                DataTable dt = ds.Tables[0];
                dataGridView1.DataSource = dt;

                var query = (from x in en.RekapSelisihAmbilSetors
                            where x.tanggalTransaksi == dateTimePicker1.Value.Date
                            select x).FirstOrDefault();

                DataRow[] rows = dt.Select("Column17 is null or Column4 not like 'CABANG'");

                foreach (var row in rows)
                {
                    dt.Rows.Remove(row);
                }

                if (query != null)
                {
                    MessageBox.Show("Tanggal " + dateTimePicker1.Value.Date.ToShortDateString() + " Sudah Ada Di Database");
                }

                else
                {
                    for (int i = 1; i < dt.Rows.Count; i++)
                    {
                        String kodePenerimaDana = "";
                        String kodeSumberDana = "";
                        Int64 lebih = 0;
                        Int64 kurang = 0;
                        Int64 palsu = 0;
                        Int64 mutilasi = 0;
                        Int64 total = 0;
                        String noTxn = "";
                        DateTime tanggalTransaksi = new DateTime(1 / 1 / 1);
                        DateTime tanggalTemu = new DateTime(1 / 1 / 1);
                        String noBA = "";
                        String keterangan = "";
                        Int64 buff;


                        //Console.WriteLine(dt.Rows[i][17].ToString().Substring(0, dt.Rows[i][17].ToString().IndexOf(" ")-1));

                        if (Int64.TryParse(dt.Rows[i][0].ToString(), out buff) == false && Convert.ToDateTime(dt.Rows[i][17].ToString()).Date == dateTimePicker1.Value.Date)
                        {

                            if (!String.IsNullOrEmpty(dt.Rows[i][0].ToString()))
                            {
                                kodePenerimaDana = dt.Rows[i][0].ToString();
                            }
                            if (!String.IsNullOrEmpty(dt.Rows[i][8].ToString()))
                            {
                                kodeSumberDana = dt.Rows[i][8].ToString();
                            }
                            if (!String.IsNullOrEmpty(dt.Rows[i][9].ToString()))
                            {
                                lebih = Int64.Parse(dt.Rows[i][9].ToString());
                            }
                            if (!String.IsNullOrEmpty(dt.Rows[i][10].ToString()))
                            {
                                kurang = Int64.Parse(dt.Rows[i][10].ToString());
                            }
                            if (!String.IsNullOrEmpty(dt.Rows[i][11].ToString()))
                            {
                                palsu = Int64.Parse(dt.Rows[i][11].ToString());
                            }
                            if (!String.IsNullOrEmpty(dt.Rows[i][12].ToString()))
                            {
                                mutilasi = Int64.Parse(dt.Rows[i][12].ToString());
                            }
                            if (!String.IsNullOrEmpty(dt.Rows[i][13].ToString()))
                            {
                                total = Int64.Parse(dt.Rows[i][13].ToString());
                            }
                            if (!String.IsNullOrEmpty(dt.Rows[i][16].ToString()))
                            {
                                noTxn = dt.Rows[i][16].ToString();
                            }
                            if (!String.IsNullOrEmpty(dt.Rows[i][16].ToString()))
                            {
                                noTxn = dt.Rows[i][16].ToString();
                            }
                            if (!String.IsNullOrEmpty(dt.Rows[i][17].ToString()))
                            {
                                tanggalTransaksi = Convert.ToDateTime(dt.Rows[i][17].ToString()).Date;
                            }
                            if (!String.IsNullOrEmpty(dt.Rows[i][18].ToString()))
                            {
                                tanggalTemu = Convert.ToDateTime(dt.Rows[i][18].ToString()).Date;
                            }
                            if (!String.IsNullOrEmpty(dt.Rows[i][23].ToString()))
                            {
                                noBA = dt.Rows[i][23].ToString();
                            }
                            if (!String.IsNullOrEmpty(dt.Rows[i][31].ToString()))
                            {
                                keterangan = dt.Rows[i][31].ToString();
                            }

                            en.RekapSelisihAmbilSetors.Add(new RekapSelisihAmbilSetor()
                            {
                                kodePenerimaDana = kodePenerimaDana,
                                kodeSumberDana = kodeSumberDana,
                                lebih = lebih,
                                kurang = kurang,
                                palsu = palsu,
                                mutilasi = mutilasi,
                                total = total,
                                noTxn = noTxn,
                                tanggalTransaksi = tanggalTransaksi,
                                tanggalTemu = tanggalTemu,
                                noBA = noBA,
                                keterangan = keterangan

                            });

                        }
                        else
                        {
                            continue;
                        }

                    }
                }
                en.SaveChanges();
            }
        }
    }
}
