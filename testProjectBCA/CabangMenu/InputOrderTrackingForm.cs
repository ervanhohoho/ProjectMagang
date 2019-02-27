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
using testProjectBCA.CabangMenu;
namespace testProjectBCA
{
    public partial class InputOrderTrackingForm : Form
    {
        Database1Entities en = new Database1Entities();
        public InputOrderTrackingForm()
        {
            InitializeComponent();
            loadPktCombo();
            button5.Enabled = false;
            button4.Enabled = false;
            button6.Enabled = false;
            button8.Enabled = false;
            button9.Enabled = false;
            button10.Enabled = false;
        }
        void loadPktCombo()
        {
            Database1Entities db = new Database1Entities();
            List<String> listPkt = (from x in db.Pkts
                                    where x.kanwil.ToLower().Contains("jabo") && x.kodePktCabang.Length > 1
                                    select x.kodePktCabang.Contains("CCAS") ? "CCAS" : x.kodePktCabang).Distinct().ToList();
            pktComboBox.DataSource = listPkt;
            pktComboBox.SelectedIndex = 0;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            en = new Database1Entities();
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.excelFilter;


            if (of.ShowDialog() == DialogResult.OK)
            {
                loadForm.ShowSplashScreen();
                String filename = of.FileName;
                DataSet ds = Util.openExcel(filename);
                DataTable dt = ds.Tables[0];
                Console.WriteLine(ds.Tables.Count);


                var query = (from x in en.OrderTrackings.AsEnumerable()
                             where ((DateTime)x.timestamp).Date == dateTimePicker1.Value.Date
                             select x).ToList();
                Console.WriteLine("Query Count: " + query.Count);
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
                            DateTime dueDate, timestamp;
                            String reference_master = "", 
                                sched_id = "", 
                                actn_id = "",
                                state_blog="";

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
                                        Console.WriteLine("PROCESS ROW [" + i + "]");
                                        Console.WriteLine("nominalDispute: " + dt.Rows[i][19].ToString().Split(':')[1]);
                                        kodePkt = dt.Rows[i][0].ToString();
                                        kodeCabang = dt.Rows[i][2].ToString();
                                        timestamp = Convert.ToDateTime(dt.Rows[i][10].ToString());
                                        DateTime timeLimit = new DateTime(timestamp.Year, timestamp.Month, timestamp.Day, 21, 0, 0);
                                        if (timestamp > timeLimit)
                                            tanggal = timestamp.AddDays(1).Date;
                                        else
                                            tanggal = timestamp;
                                        Int64 buf;
                                        if (Int64.TryParse(dt.Rows[i][19].ToString().Split(':')[1].Trim('\"'), out buf))
                                            nominalDispute = buf;
                                        else
                                        {
                                            MessageBox.Show("Data ROW " + i + " Nominal dispute tidak bisa di parse, akan diskip!");
                                            continue;
                                        }
                                        dueDate = Convert.ToDateTime(dt.Rows[i][4].ToString());
                                        reference_master = dt.Rows[i][7].ToString();
                                        sched_id = dt.Rows[i][8].ToString();
                                        actn_id = dt.Rows[i][9].ToString();
                                        state_blog = dt.Rows[i][14].ToString();
                                        if (state_blog.Contains("|"))
                                            state_blog = state_blog.Split('|')[1];
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
                                        nominalDispute = nominalDispute,
                                        dueDate = dueDate,
                                        timestamp = timestamp,
                                        reference_master = reference_master,
                                        sched_id = sched_id,
                                        actn_id = actn_id,
                                        state_blog = state_blog
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
                    Console.WriteLine("ADD NEW");
                    for (int i = 1; i < dt.Rows.Count; i++)
                    {
                        Console.WriteLine(i);
                        Console.WriteLine(dt.Rows[i][1].ToString());
                        Console.WriteLine(dt.Rows[i][4].ToString());

                        String kodePkt = "";
                        String kodeCabang = "";
                        DateTime tanggal = new DateTime(1, 1, 1);
                        Int64 nominalDispute = 0;
                        DateTime dueDate, timestamp;
                        String reference_master = "",
                            sched_id = "",
                            actn_id = "",
                            state_blog = "";
                        var rows = dt.Select("");
                        if (dt.Rows[i][13].ToString() == null || dt.Rows[i][13].ToString() == "")
                        {
                            Console.WriteLine("ROW [" + i + "][13] IS NULL OR EMPTY");
                            continue;
                        }
                        else
                        {
                            if (dt.Rows[i][19].ToString() == null || dt.Rows[i][19].ToString() == "")
                            {
                                Console.WriteLine("ROW [" + i + "][19] IS NULL OR EMPTY");
                                continue;
                            }
                            else
                            {
                                Console.WriteLine(Convert.ToDateTime(dt.Rows[i][10].ToString()).Date);
                                Console.WriteLine(dateTimePicker1.Value.Date);
                                if (Convert.ToDateTime(dt.Rows[i][10].ToString()).Date == dateTimePicker1.Value.Date && dt.Rows[i][13].ToString() == "CONFIRMED" && dt.Rows[i][9].ToString() == "2")
                                {

                                    Console.WriteLine("PROCESS ROW ["+i+"]");
                                    Console.WriteLine("nominalDispute: " + dt.Rows[i][19].ToString().Split(':')[1]);
                                    kodePkt = dt.Rows[i][0].ToString();
                                    kodeCabang = dt.Rows[i][2].ToString();
                                    timestamp = Convert.ToDateTime(dt.Rows[i][10].ToString());
                                    DateTime timeLimit = new DateTime(timestamp.Year, timestamp.Month, timestamp.Day, 21, 0, 0);
                                    if (timestamp > timeLimit)
                                        tanggal = timestamp.AddDays(1).Date;
                                    else
                                        tanggal = timestamp.Date;
                                    Int64 buf;
                                    if(Int64.TryParse(dt.Rows[i][19].ToString().Split(':')[1].Trim('\"'), out buf))
                                        nominalDispute = buf;
                                    else
                                    {
                                        MessageBox.Show("Data ROW " + i + " Nominal dispute tidak bisa di parse, akan diskip!");
                                        continue;
                                    }
                                    dueDate = Convert.ToDateTime(dt.Rows[i][4].ToString());
                                    reference_master = dt.Rows[i][7].ToString();
                                    sched_id = dt.Rows[i][8].ToString();
                                    actn_id = dt.Rows[i][9].ToString();
                                    state_blog = dt.Rows[i][14].ToString();
                                    if (state_blog.Contains("|"))
                                        state_blog = state_blog.Split('|')[1];
                                }
                                else
                                {
                                    Console.WriteLine("DATE ROW [" + i + "][10] Tidak Sama");
                                    continue;
                                }
                                en.OrderTrackings.Add(new OrderTracking()
                                {
                                    kodePkt = kodePkt,
                                    kodeCabang = kodeCabang,
                                    tanggal = tanggal,
                                    nominalDispute = nominalDispute,
                                    dueDate = dueDate,
                                    timestamp = timestamp,
                                    reference_master = reference_master,
                                    sched_id = sched_id,
                                    actn_id = actn_id,
                                    state_blog = state_blog
                                });
                                en.SaveChanges();
                            }
                        }
                    }
                }
                loadForm.CloseForm();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.excelFilter;

            if (of.ShowDialog() == DialogResult.OK)
            {
                loadForm.ShowSplashScreen();
                String filename = of.FileName;
                DataSet ds = Util.openExcel(filename);
                DataTable dt = ds.Tables[0];

                DataRow[] rows = dt.Select("Column17 is null or (Column4 not like 'CABANG' and Column4 not like 'MEMO')");
                //DataRow[] rows = dt.Select("Column17 is null or (Column4 not like 'CABANG' and Column4 not like 'MEMO')");

                foreach (var row in rows)
                {
                    dt.Rows.Remove(row);
                }

                var query = (from x in en.RekapSelisihAmbilSetors
                             where x.tanggalTransaksi == dateTimePicker1.Value.Date
                             select x).ToList();

                

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
                            String transaksi = "";
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
                                if (!String.IsNullOrEmpty(dt.Rows[i][4].ToString()))
                                {
                                    transaksi = dt.Rows[i][4].ToString();
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
                                    keterangan = keterangan,
                                    transaksi = transaksi

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
                        Console.WriteLine(i);
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
                        String transaksi = "";
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
                            if (!String.IsNullOrEmpty(dt.Rows[i][4].ToString()))
                            {
                                transaksi = dt.Rows[i][4].ToString();
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
                                keterangan = keterangan,
                                transaksi = transaksi

                            });

                        }
                        else
                        {
                            continue;
                        }

                    }
                }
                en.SaveChanges();
                loadForm.CloseForm();
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
                    cmd.CommandText =
                                          "select[kodepkt] = isnull(kodepkt, kodepenerimadana), "
                                            + "  [kodepenerimadana] = isnull(kodepenerimadana, kodepkt), "
                                            + "  [ordertracking] = isnull(nominalDispute,0), "
                                            + "  [total rekap] = isnull(isnull([Tidak Dibatalkan],0) - isnull([dibatalkan],0) ,0),"
                                            + "  [dibatalkan] = isnull([dibatalkan],0),"
                                            + "  [tidak dibatalkan] = isnull([Tidak Dibatalkan],0),"
                                            + "   [Keterangan] = case when isnull(nominalDispute,0) =  isnull(isnull([Tidak Dibatalkan],0) - isnull([dibatalkan],0) ,0)"
                                            + "  then 'SAMA' else 'BERBEDA' end"
                                            + "  from"
                                            + "  ("
                                            + "  select OrderTracking.kodePkt, nominalDispute = sum(nominalDispute)"
                                            + "  from OrderTracking join Pkt on (CASE WHEN OrderTracking.kodePkt LIKE 'CCAS%' THEN 'CCASA' ELSE OrderTracking.kodePkt END) = Pkt.kodePktCabang"
                                            + "  where tanggal = '" + dateTimePicker1.Value.Date + "' and kanwil like 'Jabotabek'"
                                            + "  group by OrderTracking.kodePkt)a full outer join"
                                            + "  ("
                                            + "  select kodePenerimaDana, [Tidak Dibatalkan] = sum(case when lower(keterangan) not like '%batal%'  then total else 0 end), [dibatalkan] = sum(case when lower(keterangan) = '%batal%' then total else 0 end)"
                                            + "  from RekapSelisihAmbilSetor join Pkt on (CASE WHEN RekapSelisihAmbilSetor.kodePenerimaDana LIKE 'CCAS%' THEN 'CCASA' ELSE kodePenerimaDana END) = Pkt.kodePktCabang"
                                            + "  where tanggalTransaksi = '"+dateTimePicker1.Value.Date+"' and LEN(noTxn) = 6 and kanwil like 'Jabotabek'"
                                            + "  group by kodePenerimaDana"
                                            + "  )b"
                                            + "  on a.kodePkt = b.kodePenerimaDana";
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        otl.Add(new orderTrackingLoad
                        {
                            kodePkt = reader[0].ToString(),
                            orderTracking = Int64.Parse(reader[2].ToString()),
                            //rekapSelesai = Int64.Parse(reader[3].ToString()),
                            //rekapBelumSelesai = Int64.Parse(reader[4].ToString()),
                            totalRekap = Int64.Parse(reader[3].ToString()),
                            selisih = Math.Abs(Int64.Parse(reader[3].ToString()) - Int64.Parse(reader[2].ToString())),
                            tidakDibatalkan = Int64.Parse(reader[5].ToString()),
                            dibatalkan = Int64.Parse(reader[4].ToString()),
                            keterangan = reader[6].ToString()

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

                    var check2 = (from x in otl
                                  where x.kodePkt.Contains("CCAS")
                                  select x).ToList();
                    if (check2 != null)
                    {
                        foreach (var item in check2)
                        {
                            item.keterangan = "";
                            item.selisih = 0;
                        }
                    }



                    //adding ccas-ordertracking
                    var query2 = (from x in otl
                                  where x.kodePkt.Contains("CCAS") && x.kodePkt.Length > 4
                                  select x).ToList();

                    Int64 orderTrackingbuff = query2.Sum(x => x.orderTracking);
                    //Int64 rekapSelesaibuff = query2.Sum(x => x.rekapSelesai);
                    //Int64 rekapBelumSelesaibuff = query2.Sum(x => x.rekapBelumSelesai);
                    Int64 tidakDibatalkanbuff = query2.Sum(x => x.tidakDibatalkan);
                    Int64 dibatalkanbuff = query2.Sum(x => x.dibatalkan);
                    Int64 totalRekapBuff = query2.Sum(x => x.totalRekap);

                    otl.Add(new orderTrackingLoad
                    {
                        kodePkt = "CCAS-OrderTracking",
                        orderTracking = orderTrackingbuff,
                        //rekapSelesai = rekapBelumSelesaibuff,
                        //rekapBelumSelesai = rekapBelumSelesaibuff,
                        tidakDibatalkan = tidakDibatalkanbuff,
                        dibatalkan = dibatalkanbuff,
                        totalRekap = totalRekapBuff,
                        keterangan = "SUM CCAS-OT"
                    });
                    // 

                    //CCAS ganti keterangan
                    var check = (from x in otl
                                 where x.kodePkt == "CCAS"
                                 select x).FirstOrDefault();
                    if (check != null)
                    {
                        if (check.totalRekap == otl[otl.Count - 1].orderTracking)
                        {
                            check.keterangan = "SAMA";
                        }
                        else
                        {
                            check.keterangan = "BERBEDA";
                        }
                        check.selisih = Math.Abs(otl[otl.Count - 1].orderTracking - check.totalRekap);

                    }


                    dataGridView1.DataSource = otl;
                    for (int i = 0; i < 8; i++)
                    {
                        //dataGridView1.Columns[i].ReadOnly = true;
                    }
                    for (int i = 1; i < 6; i++)
                    {
                        dataGridView1.Columns[i].DefaultCellStyle.Format = "c0";
                        dataGridView1.Columns[i].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
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
            //public Int64 rekapSelesai { set; get; }
            // public Int64 rekapBelumSelesai { set; get; }
            public Int64 tidakDibatalkan { set; get; }
            public Int64 dibatalkan { set; get; }
            public Int64 totalRekap { set; get; }
            public Int64 selisih { set; get; }
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
            //loadForm.ShowSplashScreen();
            List<SaveRekap> sr = new List<SaveRekap>();

            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                DateTime tanggal = dateTimePicker1.Value.Date;
                String komentar = "";
                String kodePkt = dataGridView1.Rows[i].Cells["kodePkt"].Value.ToString();
                Int64 orderTracking = Int64.Parse(dataGridView1.Rows[i].Cells["orderTracking"].Value.ToString());
                Int64 tidakDibatalkan = Int64.Parse(dataGridView1.Rows[i].Cells["tidakDibatalkan"].Value.ToString());
                Int64 dibatalkan = Int64.Parse(dataGridView1.Rows[i].Cells["dibatalkan"].Value.ToString());
                Int64 totalRekap = Int64.Parse(dataGridView1.Rows[i].Cells["totalRekap"].Value.ToString());
                Int64 selisih = Int64.Parse(dataGridView1.Rows[i].Cells["selisih"].Value.ToString());
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
                        tidakDibatalkan = tidakDibatalkan,
                        dibatalkan = dibatalkan,
                        totalRekap = totalRekap,
                        keterangan = keterangan,
                        komentar = komentar,
                        selisih = selisih,
                        tanggal = tanggal
                    });

                }
                else
                {
                    query.tanggal = tanggal;
                    query.komentar = komentar;
                    query.kodePkt = kodePkt;
                    query.orderTracking = orderTracking;
                    query.dibatalkan = dibatalkan;
                    query.tidakDibatalkan = tidakDibatalkan;
                    query.totalRekap = totalRekap;
                    query.selisih = selisih;
                    query.keterangan = keterangan;

                }


            }
            en.SaveRekaps.AddRange(sr);
            en.SaveChanges();
            //loadForm.CloseForm();
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
            loadForm.ShowSplashScreen();
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
            loadForm.CloseForm();
        }

        private void InputOrderTrackingForm_Load(object sender, EventArgs e)
        {

        }

       

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewSelectedCellCollection cells = dataGridView1.SelectedCells;
            foreach (DataGridViewCell cell in cells)
            {
                int rowidx = cell.RowIndex;
                int colidx = cell.ColumnIndex;
                if (colidx > 0)
                {
                    dataGridView1.Rows[rowidx].Cells[colidx].Style.Format = "F0";
                }

            }
            for (int a = 0; a < dataGridView1.Rows.Count; a++)
            {
                for (int b = 3; b < dataGridView1.Columns.Count; b++)
                {
                    if (!cells.Contains(dataGridView1.Rows[a].Cells[b]))
                    {
                        Int64 buf;
                        dataGridView1.Rows[a].Cells[b].Style.Format = "C0";
                        dataGridView1.Rows[a].Cells[b].Style.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
                    }
                }
            }
        }
        private void buttonShowDetails_Click(object sender, EventArgs e)
        {
            String kodePkt = pktComboBox.SelectedItem.ToString();
            DateTime tanggal = dateTimePicker1.Value.Date;
            DetailsOrderTracking dot = new DetailsOrderTracking(kodePkt, tanggal);
            dot.Show();
        }
        private void rekapSelisihBtn_Click(object sender, EventArgs e)
        {
            String kodePkt = pktComboBox.SelectedItem.ToString();
            DateTime tanggal = dateTimePicker1.Value.Date;
            DetailsSelisihAmbilSetor dsas = new DetailsSelisihAmbilSetor(kodePkt, tanggal, false);
            dsas.Show();
        }

        private void showDetailsAskButton_Click(object sender, EventArgs e)
        {
            String kodePkt = pktComboBox.SelectedItem.ToString();
            DateTime tanggal = dateTimePicker1.Value.Date;
            DetailsSelisihAmbilSetor dsas = new DetailsSelisihAmbilSetor(kodePkt, tanggal, true);
            dsas.Show();
        }
    }
}