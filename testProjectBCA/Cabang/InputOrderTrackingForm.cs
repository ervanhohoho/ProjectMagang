﻿using System;
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
    public partial class InputOrderTrackingForm : Form
    {
        Database1Entities en = new Database1Entities();
        public InputOrderTrackingForm()
        {
            InitializeComponent();
            button5.Enabled = false;
            button4.Enabled = false;
            button6.Enabled = false;
            button8.Enabled = false;
            button9.Enabled = false;
            button10.Enabled = false;
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


                var query = (from x in en.OrderTrackings
                             where x.tanggal == dateTimePicker1.Value.Date
                             select x).ToList();

                if (query.Any())
                {

                    var Result = MessageBox.Show("Tanggal " + dateTimePicker1.Value.Date.ToShortDateString() + " sudah ada di database, update?", "", MessageBoxButtons.YesNo);
                    if (Result == DialogResult.Yes)
                    {
                        en.OrderTrackings.RemoveRange(query);
                        en.SaveChanges();

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
                                    if (Convert.ToDateTime(dt.Rows[i][10].ToString()).Date == dateTimePicker1.Value.Date && dt.Rows[i][13].ToString() == "CONFIRMED" && dt.Rows[i][9].ToString() == "2")
                                    {
                                        //Console.WriteLine("cus");
                                        kodePkt = dt.Rows[i][0].ToString();
                                        kodeCabang = dt.Rows[i][2].ToString();
                                        tanggal = Convert.ToDateTime(dt.Rows[i][10].ToString()).Date;
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
                        MessageBox.Show("data berhasil diupdate");
                    }
                    else  /*(DialogResult ==DialogResult.No)*/
                    {
                        MessageBox.Show("data tidak diupdate");
                    }
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
                                if (Convert.ToDateTime(dt.Rows[i][10].ToString()).Date == dateTimePicker1.Value.Date && dt.Rows[i][13].ToString() == "CONFIRMED" && dt.Rows[i][9].ToString() == "2")
                                {
                                    Console.WriteLine("cus");
                                    kodePkt = dt.Rows[i][0].ToString();
                                    kodeCabang = dt.Rows[i][2].ToString();
                                    tanggal = Convert.ToDateTime(dt.Rows[i][10].ToString()).Date;
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


                var query = (from x in en.RekapSelisihAmbilSetors
                             where x.tanggalTransaksi == dateTimePicker1.Value.Date
                             select x).ToList();

                DataRow[] rows = dt.Select("Column17 is null or Column4 not like 'CABANG'");

                foreach (var row in rows)
                {
                    dt.Rows.Remove(row);
                }

                if (query.Any())
                {
                    var result = MessageBox.Show("Tanggal " + dateTimePicker1.Value.Date.ToShortDateString() + " sudah ada di database, update?", "", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        en.RekapSelisihAmbilSetors.RemoveRange(query);
                        en.SaveChanges();
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
                        MessageBox.Show("data berhasil diupdate");
                    }
                    else if (result == DialogResult.No)
                    {
                        MessageBox.Show("data tidak diupdate");
                    }
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

        private void reloadGridView()
        {

            //List<String> kodePkt = new List<String>();
            //List<Int64> orderTracking = new List<Int64>();
            //List<Int64> rekapSelesai = new List<Int64>();
            //List<Int64> rekapBelumSelesai = new List<Int64>();
            //List<Int64> totalRekap = new List<Int64>();
            //List<Int64> dibatalkan = new List<Int64>();

            List<orderTrackingLoad> otl = new List<orderTrackingLoad>();


            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select [kodepkt] =  isnull(kodepkt,kodepenerimadana), "
                                      + " [kodepenerimadana] = isnull(kodepenerimadana, kodepkt), "
                                      + " [ordertracking] = isnull(nominalDispute,0), [selesai] = isnull([Selesai],0), "
                                      + " [belum selesai] = isnull([belum selesai],0), "
                                      + " [total rekap] = isnull([selesai]+ [Belum Selesai]-[dibatalkan],0),"
                                      + " [dibatalkan] = isnull([dibatalkan],0),"
                                      + " [Keterangan] = case when isnull(nominalDispute,0) = [selesai]+ [Belum Selesai] then 'SAMA' else 'BERBEDA' end"
                                      + " from"
                                      + " ("
                                      + " select kodePkt, nominalDispute = sum(nominalDispute)"
                                      + " from OrderTracking"
                                      + " where tanggal = '" + dateTimePicker1.Value.ToShortDateString() + "'"
                                      + " group by kodePkt)a full outer join"
                                      + " ("
                                      + " select kodePenerimaDana, [Selesai] = sum(case when lower(keterangan) = 'selesai' then total else 0 end), [Belum Selesai] = sum(case when keterangan = '' then total else 0 end), [dibatalkan] = sum(case when lower(keterangan) = '%batal%' then total else 0 end)"
                                      + " from RekapSelisihAmbilSetor"
                                      + " where tanggalTransaksi = '" + dateTimePicker1.Value.ToShortDateString() + "' and LEN(noTxn)<=6"
                                      + " group by kodePenerimaDana)b"
                                      + " on a.kodePkt = b.kodePenerimaDana";
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        otl.Add(new orderTrackingLoad
                        {
                            kodePkt = reader[0].ToString(),
                            orderTracking = Int64.Parse(reader[2].ToString()),
                            rekapSelesai = Int64.Parse(reader[3].ToString()),
                            rekapBelumSelesai = Int64.Parse(reader[4].ToString()),
                            totalRekap = Int64.Parse(reader[5].ToString()),
                            dibatalkan = Int64.Parse(reader[6].ToString()),
                            keterangan = reader[7].ToString()

                        });
                        //kodePkt.Add(reader[0].ToString());
                        //orderTracking.Add(Int64.Parse(reader[2].ToString()));
                        //rekapSelesai.Add(Int64.Parse(reader[3].ToString()));
                        //rekapBelumSelesai.Add(Int64.Parse(reader[4].ToString()));
                        //totalRekap.Add(Int64.Parse(reader[5].ToString()));
                        //dibatalkan.Add(Int64.Parse(reader[6].ToString()));
                    }



                    var query = (from x in en.SaveRekaps
                                 select x).ToList();



                    foreach (var item in otl)
                    {

                        var q2 = query.Where(x => ((DateTime)x.tanggal).Date == dateTimePicker1.Value.Date && x.kodePkt == item.kodePkt).Select(x => x.komentar).ToList();
                        if (q2.Any())
                        {
                            item.komentar = q2[0];
                        }

                    }

                    //adding ccas-ordertracking
                    var query2 = (from x in otl
                                  where x.kodePkt.Contains("CCAS") && x.kodePkt.Length > 4
                                  select x).ToList();

                    Int64 orderTrackingbuff = query2.Sum(x => x.orderTracking);
                    Int64 rekapSelesaibuff = query2.Sum(x => x.rekapSelesai);
                    Int64 rekapBelumSelesaibuff = query2.Sum(x => x.rekapBelumSelesai);
                    Int64 dibatalkanbuff = query2.Sum(x => x.dibatalkan);
                    Int64 totalRekapBuff = query2.Sum(x => x.totalRekap);

                    otl.Add(new orderTrackingLoad
                    {
                        kodePkt = "CCAS-OrderTracking",
                        orderTracking = orderTrackingbuff,
                        rekapSelesai = rekapBelumSelesaibuff,
                        rekapBelumSelesai = rekapBelumSelesaibuff,
                        dibatalkan = dibatalkanbuff,
                        totalRekap = totalRekapBuff,
                        keterangan = "SUM CCAS-OT"
                    });
                    //

                    dataGridView1.DataSource = otl;
                    for (int i = 0; i < 8; i++)
                    {
                        dataGridView1.Columns[i].ReadOnly = true;
                    }
                    button4.Enabled = true;

                }
            }
        }

        void reloadGridView2()
        {
            //List<String> kodePkt = new List<String>();
            //List<Int64> orderTracking = new List<Int64>();
            //List<Int64> rekapSelesai = new List<Int64>();
            //List<Int64> rekapBelumSelesai = new List<Int64>();
            //List<Int64> totalRekap = new List<Int64>();
            //List<Int64> dibatalkan = new List<Int64>();

            List<orderTrackingSumUp6> otsu = new List<orderTrackingSumUp6>();


            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select "
                                      + " [total rekap] = isnull(sum(isnull([selesai]+ [Belum Selesai]-[dibatalkan],0)),0)"
                                      + " from"
                                      + " ("
                                      + " select kodePkt, nominalDispute = sum(nominalDispute)"
                                      + " from OrderTracking"
                                      + " where tanggal = '" + dateTimePicker1.Value.ToShortDateString() + "'"
                                      + " group by kodePkt)a full outer join"
                                      + " ("
                                      + " select kodePenerimaDana, [Selesai] = sum(case when lower(keterangan) = 'selesai' then total else 0 end), [Belum Selesai] = sum(case when keterangan = '' then total else 0 end), [dibatalkan] = sum(case when lower(keterangan) = '%batal%' then total else 0 end)"
                                      + " from RekapSelisihAmbilSetor"
                                      + " where tanggalTransaksi = '" + dateTimePicker1.Value.ToShortDateString() + "' and LEN(noTxn)>6"
                                      + " group by kodePenerimaDana)b"
                                      + " on a.kodePkt = b.kodePenerimaDana";
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        otsu.Add(new orderTrackingSumUp6
                        {

                            rekapAsk = Int64.Parse(reader[0].ToString()),
                            ask = 0,

                        });
                        //kodePkt.Add(reader[0].ToString());
                        //orderTracking.Add(Int64.Parse(reader[2].ToString()));
                        //rekapSelesai.Add(Int64.Parse(reader[3].ToString()));
                        //rekapBelumSelesai.Add(Int64.Parse(reader[4].ToString()));
                        //totalRekap.Add(Int64.Parse(reader[5].ToString()));
                        //dibatalkan.Add(Int64.Parse(reader[6].ToString()));
                    }
                    var query = (from x in en.SaveAsks
                                 select x).ToList();




                    foreach (var item in otsu)
                    {

                        var q2 = query.Where(x => ((DateTime)x.tanggal).Date == dateTimePicker1.Value.Date /*&& x.kodePkt == item.kodePkt*/).Select(x => x.komentar).ToList();
                        if (q2.Any())
                        {
                            item.komentar = q2[0];
                        }

                        var q3 = query.Where(x => ((DateTime)x.tanggal).Date == dateTimePicker1.Value.Date).Select(x => x.ask).ToList();
                        if (q3.Any())
                        {
                            item.ask = Int64.Parse(q3[0].ToString());
                        }

                    }

                    //adding ccas-ordertracking
                    //var query2 = (from x in otl
                    //              where x.kodePkt.Contains("CCAS") && x.kodePkt.Length > 4
                    //              select x).ToList();

                    //Int64 orderTrackingbuff = query2.Sum(x => x.orderTracking);
                    //Int64 rekapSelesaibuff = query2.Sum(x => x.rekapSelesai);
                    //Int64 rekapBelumSelesaibuff = query2.Sum(x => x.rekapBelumSelesai);
                    //Int64 dibatalkanbuff = query2.Sum(x => x.dibatalkan);
                    //Int64 totalRekapBuff = query2.Sum(x => x.totalRekap);

                    //otl.Add(new orderTrackingLoad
                    //{
                    //    kodePkt = "CCAS-OrderTracking",
                    //    orderTracking = orderTrackingbuff,
                    //    rekapSelesai = rekapBelumSelesaibuff,
                    //    rekapBelumSelesai = rekapBelumSelesaibuff,
                    //    dibatalkan = dibatalkanbuff,
                    //    totalRekap = totalRekapBuff,
                    //    keterangan = "SUM CCAS-OT"
                    //});
                    //

                    dataGridView2.DataSource = otsu;
                    for (int i = 0; i < 3; i++)
                    {
                        dataGridView2.Columns[i].ReadOnly = true;
                    }
                    //button4.Enabled = true;

                }
            }
        }

        class orderTrackingLoad
        {
            public String kodePkt { set; get; }
            public Int64 orderTracking { set; get; }
            public Int64 rekapSelesai { set; get; }
            public Int64 rekapBelumSelesai { set; get; }
            public Int64 dibatalkan { set; get; }
            public Int64 totalRekap { set; get; }
            public String keterangan { set; get; }
            public String komentar { set; get; }

        }

        class orderTrackingSumUp6
        {
            public Int64 rekapAsk { set; get; }
            public Int64 ask { set; get; }
            public String komentar { set; get; }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            reloadGridView();

            button6.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button4.Enabled = false;
            dataGridView1.Columns[7].ReadOnly = false;
            button5.Enabled = true;
            button6.Enabled = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            button4.Enabled = true;
            button5.Enabled = false;
            button6.Enabled = true;
            dataGridView1.Columns[7].ReadOnly = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            List<SaveRekap> sr = new List<SaveRekap>();

            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                DateTime tanggal = dateTimePicker1.Value.Date;
                String komentar = "";
                String kodePkt = dataGridView1.Rows[i].Cells["kodePkt"].Value.ToString();
                Int64 orderTracking = Int64.Parse(dataGridView1.Rows[i].Cells["orderTracking"].Value.ToString());
                Int64 rekapSelesai = Int64.Parse(dataGridView1.Rows[i].Cells["rekapSelesai"].Value.ToString());
                Int64 rekapBelumSelesai = Int64.Parse(dataGridView1.Rows[i].Cells["rekapBelumSelesai"].Value.ToString());
                Int64 totalRekap = Int64.Parse(dataGridView1.Rows[i].Cells["totalRekap"].Value.ToString());
                String keterangan = dataGridView1.Rows[i].Cells["keterangan"].Value.ToString();

                var query = (from x in en.SaveRekaps.AsEnumerable()
                             where x.kodePkt == kodePkt && ((DateTime)x.tanggal).Date == dateTimePicker1.Value.Date
                             select x).FirstOrDefault();

                if (dataGridView1.Rows[i].Cells["komentar"].Value != null)
                {
                    komentar = dataGridView1.Rows[i].Cells["komentar"].Value.ToString();
                }

                if (query == null)
                {
                    //insert

                    sr.Add(new SaveRekap()
                    {
                        kodePkt = kodePkt,
                        orderTracking = orderTracking,
                        rekapBelumSelesai = rekapBelumSelesai,
                        rekapSelesai = rekapSelesai,
                        totalRekap = totalRekap,
                        keterangan = keterangan,
                        komentar = komentar,
                        tanggal = tanggal
                    });

                }
                else
                {
                    query.tanggal = tanggal;
                    query.komentar = komentar;
                    query.kodePkt = kodePkt;
                    query.orderTracking = orderTracking;
                    query.rekapSelesai = rekapSelesai;
                    query.rekapBelumSelesai = rekapBelumSelesai;
                    query.totalRekap = totalRekap;
                    query.keterangan = keterangan;

                }


            }
            en.SaveRekaps.AddRange(sr);
            en.SaveChanges();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            reloadGridView2();
            button8.Enabled = true;
            dataGridView2.Columns[1].ReadOnly = true;
            dataGridView2.Columns[2].ReadOnly = true;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            button8.Enabled = false;
            dataGridView2.Columns[1].ReadOnly = false;
            dataGridView2.Columns[2].ReadOnly = false;
            button9.Enabled = true;
            button10.Enabled = false;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            dataGridView2.Columns[1].ReadOnly = true;
            dataGridView2.Columns[2].ReadOnly = true;
            button8.Enabled = true;
            button9.Enabled = false;
            button10.Enabled = true;
        }

        class saveAsk
        {
            public DateTime tanggal { set; get; }
            public Int64 rekapAsk { set; get; }
            public Int64 ask { set; get; }
            public String komentar { set; get; }

        }

        private void button10_Click(object sender, EventArgs e)
        {
            List<SaveAsk> sa = new List<SaveAsk>();

            for (int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                DateTime tanggal = dateTimePicker1.Value.Date;
                String komentar = "";
                Int64 rekapAsk = 0;
                Int64 ask = 0;

                var query = (from x in en.SaveAsks.AsEnumerable()
                             where /*x.kodePkt == kodePkt && */((DateTime)x.tanggal).Date == dateTimePicker1.Value.Date
                             select x).FirstOrDefault();

                if (dataGridView2.Rows[i].Cells["komentar"].Value != null)
                {
                    komentar = dataGridView2.Rows[i].Cells["komentar"].Value.ToString();
                }

                rekapAsk = Int64.Parse(dataGridView2.Rows[i].Cells["rekapAsk"].Value.ToString());
                ask = Int64.Parse(dataGridView2.Rows[i].Cells["ask"].Value.ToString());

                if (query == null)
                {
                    //insert

                    sa.Add(new SaveAsk()
                    {
                        ask = ask,
                        rekapAsk = rekapAsk,
                        komentar = komentar,
                        tanggal = tanggal
                    });

                }
                else
                {
                    query.tanggal = tanggal;
                    query.komentar = komentar;
                    query.ask = ask;
                    query.rekapAsk = rekapAsk;


                }


            }
            en.SaveAsks.AddRange(sa);
            en.SaveChanges();
        }
    }
}