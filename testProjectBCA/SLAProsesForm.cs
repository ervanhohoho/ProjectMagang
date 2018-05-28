using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA
{
    public partial class SLAProsesForm : Form
    {
        Database1Entities db = new Database1Entities();
        List<slaProsesDisplay> sla;
        List<String> tahun1 = new List<String>();
        List<String> tahun2 = new List<String>();
        int jumlahPkt;
        public SLAProsesForm()
        {
            InitializeComponent();


            loadComboPkt();

            comboNamaPkt.SelectedIndex = 0;

            initCombos();

        }
        void initCombos()
        {
            comboTahun1.DataSource = null;
            comboTahun2.DataSource = null;
            comboBulan1.DataSource = null;
            comboBulan2.DataSource = null;
            loadComboTahun1();
            loadComboTahun2();

            if (tahun1.Count > 0)
            {
                comboTahun1.SelectedIndex = 0;
                loadComboBulan1();
            }
            if (tahun2.Count > 0)
            {
                comboTahun2.SelectedIndex = 0;
                loadComboBulan2();
            }
        }
        void loadComboTahun1()
        {
            tahun1 = new List<String>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {

                    /***Untuk penghitungan pake dailyStock***/
                    //cmd.Connection = sql;
                    //sql.Open();
                    //if(comboNamaPkt.SelectedIndex<jumlahPkt)
                    //    cmd.CommandText = "SELECT DISTINCT[Tahun] = YEAR(tanggal) FROM DailyStock ds JOIN Pkt ON ds.kodePkt = Pkt.kodePkt WHERE namaPkt = '"+comboNamaPkt.SelectedValue.ToString()+"' ORDER BY Tahun";
                    //else
                    //    cmd.CommandText = "SELECT DISTINCT[Tahun] = YEAR(tanggal) FROM DailyStock ds JOIN Pkt ON ds.kodePkt = Pkt.kodePkt ORDER BY Tahun";
                    //SqlDataReader reader = cmd.ExecuteReader();
                    //while (reader.Read())
                    //{
                    //    tahun1.Add(reader[0].ToString());
                    //}
                    //comboTahun1.DataSource = tahun1;

                    /***Untuk penghitungan pake stokposisi***/
                    cmd.Connection = sql;
                    sql.Open();
                    if (comboNamaPkt.SelectedIndex < jumlahPkt)
                        cmd.CommandText = "SELECT DISTINCT[Tahun] = YEAR(tanggal) FROM StokPosisi ds WHERE namaPkt LIKE '%" + comboNamaPkt.SelectedValue.ToString() + "%' ORDER BY Tahun";
                    else                                                               
                        cmd.CommandText = "SELECT DISTINCT[Tahun] = YEAR(tanggal) FROM StokPosisi ds ORDER BY Tahun";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        tahun1.Add(reader[0].ToString());
                    }
                    comboTahun1.DataSource = tahun1.OrderByDescending(x=>x).ToList();
                }
            }
        }
        void loadComboTahun2()
        {
            tahun2 = new List<String>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    /***Untuk penghitungan pake dailyStock***/
                    //cmd.Connection = sql;
                    //sql.Open();
                    //if (comboNamaPkt.SelectedIndex < jumlahPkt)
                    //    cmd.CommandText = "SELECT DISTINCT[Tahun] = YEAR(tanggal) FROM DailyStock ds JOIN Pkt ON ds.kodePkt = Pkt.kodePkt WHERE namaPkt LIKE '%" + comboNamaPkt.SelectedValue.ToString() + "%' ORDER BY Tahun";
                    //else
                    //    cmd.CommandText = "SELECT DISTINCT[Tahun] = YEAR(tanggal) FROM DailyStock ds JOIN Pkt ON ds.kodePkt = Pkt.kodePkt ORDER BY Tahun";
                    //SqlDataReader reader = cmd.ExecuteReader();
                    //while (reader.Read())
                    //{
                    //    tahun2.Add(reader[0].ToString());
                    //}
                    //comboTahun2.DataSource = tahun2;

                    /***Untuk penghitungan pake stokposisi***/
                    cmd.Connection = sql;
                    sql.Open();
                    if (comboNamaPkt.SelectedIndex < jumlahPkt)
                        cmd.CommandText = "SELECT DISTINCT[Tahun] = YEAR(tanggal) FROM StokPosisi ds WHERE namaPkt LIKE '%" + comboNamaPkt.SelectedValue.ToString() + "%' ORDER BY Tahun";
                    else                                                                             
                        cmd.CommandText = "SELECT DISTINCT[Tahun] = YEAR(tanggal) FROM StokPosisi ds ORDER BY Tahun";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        tahun2.Add(reader[0].ToString());
                    }
                    comboTahun2.DataSource = tahun2.OrderByDescending(x=> x).ToList();
                }
            }
        }
        void loadComboBulan1()
        {
            List<String> bulan1 = new List<String>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
               

                
                using (SqlCommand cmd = new SqlCommand())
                {
                    /***Penghitungan menggunakan Dailystock***/

                    //    cmd.Connection = sql;
                    //    sql.Open();
                    //    if (comboNamaPkt.SelectedIndex < jumlahPkt)
                    //        cmd.CommandText = "SELECT DISTINCT [Month] = Month(tanggal) FROM DailyStock ds JOIN Pkt ON ds.kodePkt = Pkt.kodePkt WHERE YEAR(Tanggal) = " + comboTahun1.SelectedValue + " AND namaPkt LIKE '%" + comboNamaPkt.SelectedValue.ToString() + "%' ORDER BY [Month]";
                    //    else
                    //        cmd.CommandText = "SELECT DISTINCT [Month] = Month(tanggal) FROM DailyStock ds JOIN Pkt ON ds.kodePkt = Pkt.kodePkt WHERE YEAR(Tanggal) = " + comboTahun1.SelectedValue + " ORDER BY [Month]";
                    //    SqlDataReader reader = cmd.ExecuteReader();
                    //    while (reader.Read())
                    //    {
                    //        bulan1.Add(reader[0].ToString());
                    //    }
                    //    comboBulan1.DataSource = bulan1;

                    /***Penghitungan Menggunakan Stok Posisi***/
                    cmd.Connection = sql;
                    sql.Open();
                    if (comboNamaPkt.SelectedIndex < jumlahPkt)
                        cmd.CommandText = "SELECT DISTINCT [Month] = Month(tanggal) FROM StokPosisi ds WHERE YEAR(Tanggal) = " + comboTahun1.SelectedValue + " AND namaPkt LIKE '%" + comboNamaPkt.SelectedValue.ToString() + "%' ORDER BY [Month]";
                    else                                                                     
                        cmd.CommandText = "SELECT DISTINCT [Month] = Month(tanggal) FROM StokPosisi ds WHERE YEAR(Tanggal) = " + comboTahun1.SelectedValue + " ORDER BY [Month]";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        bulan1.Add(reader[0].ToString());
                    }

                    bulan1 = bulan1.OrderByDescending(x=>x).ToList();
                    comboBulan1.DataSource = bulan1;

                }
            }
        }
        void loadComboBulan2()
        {
            List<String> bulan2 = new List<String>();
            
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    /***Penghitungan menggunakan Dailystock***/
                    //cmd.Connection = sql;
                    //sql.Open();
                    //if (comboNamaPkt.SelectedIndex < jumlahPkt)
                    //    cmd.CommandText = "SELECT DISTINCT[Tahun] = Month(tanggal) FROM DailyStock ds JOIN Pkt ON ds.kodePkt = Pkt.kodePkt WHERE YEAR(Tanggal) = " + comboTahun2.SelectedValue + " AND namaPkt = '"+ comboNamaPkt.SelectedValue.ToString() + "' ORDER BY Tahun";
                    //else
                    //    cmd.CommandText = "SELECT DISTINCT[Tahun] = Month(tanggal) FROM DailyStock ds JOIN Pkt ON ds.kodePkt = Pkt.kodePkt WHERE YEAR(Tanggal) = " + comboTahun2.SelectedValue + " ORDER BY Tahun";
                    //SqlDataReader reader = cmd.ExecuteReader();
                    //while (reader.Read())
                    //{
                    //    bulan2.Add(reader[0].ToString());
                    //}
                    //comboBulan2.DataSource = bulan2;

                    /***Penghitungan menggunakan Stok Posisi***/
                    cmd.Connection = sql;
                    sql.Open();
                    if (comboNamaPkt.SelectedIndex < jumlahPkt)
                        cmd.CommandText = "SELECT DISTINCT[Tahun] = Month(tanggal) FROM StokPosisi ds WHERE YEAR(Tanggal) = " + comboTahun2.SelectedValue + " AND namaPkt LIKE '%" + comboNamaPkt.SelectedValue.ToString() + "%' ORDER BY Tahun";
                    else                                                                
                        cmd.CommandText = "SELECT DISTINCT[Tahun] = Month(tanggal) FROM StokPosisi ds WHERE YEAR(Tanggal) = " + comboTahun2.SelectedValue + " ORDER BY Tahun";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        bulan2.Add(reader[0].ToString());
                    }
                    bulan2 = (from x in bulan2
                              orderby x descending
                              select x).ToList();
                    comboBulan2.DataSource = bulan2;
                }
            }
        }

        void loadComboPkt()
        {
            List<String>listPkt = (from x in db.Pkts where x.kanwil.ToLower().Contains("JABO") select x.namaPkt).ToList();
            listPkt = listPkt.Select(x => x.ToUpper().Contains("CASH PROCESSING CENTER ALAM SUTERA") ? "CASH PROCESSING CENTER ALAM SUTERA" : x).Distinct().ToList();

            jumlahPkt = listPkt.Count;
            
            listPkt.Add("All Vendor(JABO)");
            comboNamaPkt.DataSource = listPkt;
        }
        public void reloadData()
        {
            sla = new List<slaProsesDisplay>();
            //using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            //{
            //    using (SqlCommand cmd = new SqlCommand())
            //    {
            //        cmd.Connection = sql;
            //        cmd.CommandText = "";
            //        sql.Open();
            //        SqlDataReader reader = cmd.ExecuteReader();
            //        while (reader.Read())
            //        {
            //            sla.Add(new slaProsesDisplay
            //            {

            //            });
            //        }
            //    }
            //}

            List<DailyStock> listData;
            if(comboNamaPkt.SelectedIndex<jumlahPkt)
                listData= (from x in db.DailyStocks.AsEnumerable()
                                         join y in db.Pkts on x.kodePkt equals y.kodePkt
                                         where ((DateTime)x.tanggal).Month >= Int32.Parse(comboBulan1.SelectedValue.ToString()) 
                                         &&((DateTime)x.tanggal).Year >= Int32.Parse(comboTahun1.SelectedValue.ToString())
                                         &&((DateTime)x.tanggal).Month <= Int32.Parse(comboBulan2.SelectedValue.ToString())
                                         &&((DateTime)x.tanggal).Year <= Int32.Parse(comboTahun2.SelectedValue.ToString())
                                         && y.namaPkt == comboNamaPkt.SelectedValue.ToString()
                                         select x).ToList();
            else
                listData = (from x in db.DailyStocks.AsEnumerable()
                            join y in db.Pkts on x.kodePkt equals y.kodePkt
                            where ((DateTime)x.tanggal).Month >= Int32.Parse(comboBulan1.SelectedValue.ToString())
                            && ((DateTime)x.tanggal).Year >= Int32.Parse(comboTahun1.SelectedValue.ToString())
                            && ((DateTime)x.tanggal).Month <= Int32.Parse(comboBulan2.SelectedValue.ToString())
                            && ((DateTime)x.tanggal).Year <= Int32.Parse(comboTahun2.SelectedValue.ToString())
                            select x).ToList();

            for (int thn = Int32.Parse(comboTahun1.SelectedValue.ToString()); thn <= Int32.Parse(comboTahun2.SelectedValue.ToString()); thn++)
            {
                for (int bln = Int32.Parse(comboBulan1.SelectedValue.ToString()); bln <= Int32.Parse(comboBulan2.SelectedValue.ToString()); bln++)
                {
                    for (int tgl = 1; tgl <= DateTime.DaysInMonth(thn, bln); tgl++)
                    {
                        Int64 unprocessedBesar = (Int64)(from x in listData
                                                         where ((DateTime)x.tanggal).Day == tgl && x.jenisTransaksi.Like("%Passtrough%")
                                                         && ((DateTime)x.tanggal).Month == bln
                                                         && ((DateTime)x.tanggal).Year == thn
                                                         select x.BN100K/100000 + x.BN50K/50000).Sum();
                        Int64 unprocessedKecil = (Int64)(from x in listData
                                                         where ((DateTime)x.tanggal).Day == tgl && x.jenisTransaksi.Like("%Passtrough%")
                                                         && ((DateTime)x.tanggal).Month == bln
                                                         && ((DateTime)x.tanggal).Year == thn
                                                         select x.BN20K/20000 + x.BN10K/10000 + x.BN5K/5000 + x.BN2K/2000 + x.BN1K/1000 + x.BN500/500 + x.BN200/200 + x.BN100/100 + x.CN1K/1000 + x.CN500/500 + x.CN200/200 + x.CN100/100 + x.CN50/50 + x.CN25/25).Sum();

                        Int64 inCabangBesar = (Int64)(from x in listData
                                                      where ((DateTime)x.tanggal).Day == tgl
                                                      && ((DateTime)x.tanggal).Month == bln
                                                      && ((DateTime)x.tanggal).Year == thn
                                                      && x.jenisTransaksi.Like("%Collection Cabang%")
                                                      select x.BN100K / 100000 + x.BN50K / 50000).Sum();

                        Int64 inCabangKecil = (Int64)(from x in listData
                                                      where ((DateTime)x.tanggal).Day == tgl
                                                      && ((DateTime)x.tanggal).Month == bln
                                                      && ((DateTime)x.tanggal).Year == thn
                                                      && x.jenisTransaksi.Like("%Collection Cabang%")
                                                      select x.BN20K / 20000 + x.BN10K / 10000 + x.BN5K / 5000 + x.BN2K / 2000 + x.BN1K / 1000 + x.BN500 / 500 + x.BN200 / 200 + x.BN100 / 100 + x.CN1K / 1000 + x.CN500 / 500 + x.CN200 / 200 + x.CN100 / 100 + x.CN50 / 50 + x.CN25 / 25).Sum();

                        Int64 inRetailBesar = (Int64)(from x in listData
                                                      where ((DateTime)x.tanggal).Day == tgl
                                                      && ((DateTime)x.tanggal).Month == bln
                                                      && ((DateTime)x.tanggal).Year == thn
                                                      && x.jenisTransaksi.Like("%Collection Retail%")
                                                      select x.BN100K / 100000 + x.BN50K / 50000).Sum();

                        Int64 inRetailKecil = (Int64)(from x in listData
                                                      where ((DateTime)x.tanggal).Day == tgl
                                                      && ((DateTime)x.tanggal).Month == bln
                                                      && ((DateTime)x.tanggal).Year == thn
                                                      && x.jenisTransaksi.Like("%Collection Retail%")
                                                      select x.BN20K / 20000 + x.BN10K / 10000 + x.BN5K / 5000 + x.BN2K / 2000 + x.BN1K / 1000 + x.BN500 / 500 + x.BN200 / 200 + x.BN100 / 100 + x.CN1K / 1000 + x.CN500 / 500 + x.CN200 / 200 + x.CN100 / 100 + x.CN50 / 50 + x.CN25 / 25).Sum();
                        Console.WriteLine(thn + " " + bln + " " + tgl);
                        sla.Add(new slaProsesDisplay()
                        {
                            tanggal = new DateTime(thn, bln, tgl),
                            unprocUangBesar = unprocessedBesar,
                            unprocUangKecil = unprocessedKecil,
                            inCabangUangBesar = inCabangBesar,
                            inCabangUangKecil = inCabangKecil,
                            inRetailUangBesar = inRetailBesar,
                            inRetailUangKecil = inRetailKecil,
                            slaProsesBesar = 0,
                            slaProsesKecil = 0,
                            totalProsesUangBesar = 0,
                            totalProsesUangKecil = 0
                        });
                    }
                }
            }
            for(int i=0;i<sla.Count;i++)
            {
                if(i==sla.Count-1)
                {
                    DateTime tgl = sla[i].tanggal.AddDays(1);
                    var q = (from x in db.DailyStocks.AsEnumerable()
                             join y in db.Pkts.AsEnumerable() on x.kodePkt equals y.kodePkt
                             where ((DateTime)x.tanggal).Month == tgl.Month
                             && ((DateTime)x.tanggal).Year == tgl.Year
                             && ((DateTime)x.tanggal).Day == tgl.Day
                             && x.jenisTransaksi.Like("%Passtrough%")
                             select new {UnprocUangBesar = x.BN100K / 100000 + x.BN50K / 50000 , UnprocUangKecil = x.BN20K / 20000 + x.BN10K / 10000 + x.BN5K / 5000 + x.BN2K / 2000 + x.BN1K / 1000 + x.BN500 / 500 + x.BN200 / 200 + x.BN100 / 100 + x.CN1K / 1000 + x.CN500 / 500 + x.CN200 / 200 + x.CN100 / 100 + x.CN50 / 50 + x.CN25 / 25 }
                             ).FirstOrDefault();

                    sla[i].totalProsesUangBesar = sla[i].unprocUangBesar + sla[i].inCabangUangBesar + sla[i].inRetailUangBesar;
                    sla[i].totalProsesUangKecil = sla[i].unprocUangKecil + sla[i].inCabangUangKecil + sla[i].inRetailUangKecil;
                    if (q != null)
                    {
                        sla[i].totalProsesUangBesar -= (Int64)q.UnprocUangBesar;
                        sla[i].totalProsesUangKecil -= (Int64)q.UnprocUangKecil;
                    }
                }
                else
                {
                    sla[i].totalProsesUangBesar = sla[i].unprocUangBesar + sla[i].inCabangUangBesar + sla[i].inRetailUangBesar - sla[i + 1].unprocUangBesar;
                    sla[i].totalProsesUangKecil = sla[i].unprocUangKecil + sla[i].inCabangUangKecil + sla[i].inRetailUangKecil - sla[i + 1].unprocUangKecil;
                }
                if (sla[i].unprocUangBesar + sla[i].inCabangUangBesar + sla[i].inRetailUangBesar != 0)
                    sla[i].slaProsesBesar = sla[i].totalProsesUangBesar / (sla[i].unprocUangBesar + sla[i].inCabangUangBesar + sla[i].inRetailUangBesar);
                if (sla[i].unprocUangKecil + sla[i].inCabangUangKecil + sla[i].inRetailUangKecil != 0)
                    sla[i].slaProsesKecil = sla[i].totalProsesUangKecil / (sla[i].unprocUangKecil + (sla[i].inCabangUangKecil/2) + sla[i].inRetailUangKecil);
            }

            slaProsesDisplay avg = new slaProsesDisplay()
            {
                tanggal = new DateTime(1, 1, 1),
                inCabangUangBesar = (Int64)Math.Round(sla.Average(x => x.inCabangUangBesar), 0),
                inCabangUangKecil = (Int64)Math.Round(sla.Average(x => x.inCabangUangKecil), 0),
                inRetailUangBesar = (Int64)Math.Round(sla.Average(x => x.inRetailUangBesar), 0),
                inRetailUangKecil = (Int64)Math.Round(sla.Average(x => x.inRetailUangKecil), 0),
                unprocUangBesar = (Int64)Math.Round(sla.Average(x => x.unprocUangBesar), 0),
                unprocUangKecil = (Int64)Math.Round(sla.Average(x => x.unprocUangKecil), 0),
                totalProsesUangBesar = (Int64)Math.Round(sla.Average(x => x.totalProsesUangBesar), 0),
                totalProsesUangKecil = (Int64)Math.Round(sla.Average(x => x.totalProsesUangKecil), 0),
                slaProsesBesar = sla.Average(x => x.slaProsesBesar),
                slaProsesKecil = sla.Average(x => x.slaProsesKecil)
            };
            slaProsesDisplay sum = new slaProsesDisplay()
            {
                tanggal = new DateTime(1, 1, 1),
                inCabangUangBesar = (Int64)sla.Sum(x => x.inCabangUangBesar),
                inCabangUangKecil = (Int64)sla.Sum(x => x.inCabangUangKecil),
                inRetailUangBesar = (Int64)sla.Sum(x => x.inRetailUangBesar),
                inRetailUangKecil = (Int64)sla.Sum(x => x.inRetailUangKecil),
                unprocUangBesar = (Int64)sla.Sum(x => x.unprocUangBesar),
                unprocUangKecil = (Int64)sla.Sum(x => x.unprocUangKecil),
                totalProsesUangBesar = (Int64)sla.Sum(x => x.totalProsesUangBesar),
                totalProsesUangKecil = (Int64)sla.Sum(x => x.totalProsesUangKecil),
                slaProsesBesar = 0,
                slaProsesKecil = 0
            };

            sla.Add(sum);
            sla.Add(avg);

            dataGridView1.DataSource = sla;
            for (int a = 1; a < dataGridView1.Columns.Count - 2; a++)
            {
                dataGridView1.Columns[a].DefaultCellStyle.Format = "N0";
                dataGridView1.Columns[a].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            }
            dataGridView1.Columns[dataGridView1.Columns.Count - 2].DefaultCellStyle.Format = "0.%";
            dataGridView1.Columns[dataGridView1.Columns.Count - 1].DefaultCellStyle.Format = "0.%";

            dataGridView1.Rows[dataGridView1.Rows.Count - 1].DefaultCellStyle.BackColor = Color.LightSkyBlue;
            dataGridView1.Rows[dataGridView1.Rows.Count - 2].DefaultCellStyle.BackColor = Color.LightGreen;
        }
        public void reloadDataFromStokPosisi()
        {
            sla = new List<slaProsesDisplay>();
            //using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            //{
            //    using (SqlCommand cmd = new SqlCommand())
            //    {
            //        cmd.Connection = sql;
            //        cmd.CommandText = "";
            //        sql.Open();
            //        SqlDataReader reader = cmd.ExecuteReader();
            //        while (reader.Read())
            //        {
            //            sla.Add(new slaProsesDisplay
            //            {

            //            });
            //        }
            //    }
            //}

            List<StokPosisi> listData;
            Console.WriteLine(jumlahPkt);
            Console.WriteLine(comboNamaPkt.SelectedIndex);
            if (comboNamaPkt.SelectedIndex < jumlahPkt)
                listData = (from x in db.StokPosisis.AsEnumerable()
                            where ((DateTime)x.tanggal).Month >= Int32.Parse(comboBulan1.SelectedValue.ToString())
                            && ((DateTime)x.tanggal).Year >= Int32.Parse(comboTahun1.SelectedValue.ToString())
                            && ((DateTime)x.tanggal).Month <= Int32.Parse(comboBulan2.SelectedValue.ToString())
                            && ((DateTime)x.tanggal).Year <= Int32.Parse(comboTahun2.SelectedValue.ToString())
                            && x.namaPkt.Contains(comboNamaPkt.SelectedValue.ToString())
                            select x).ToList();
            else
                listData = (from x in db.StokPosisis.AsEnumerable()
                            where ((DateTime)x.tanggal).Month >= Int32.Parse(comboBulan1.SelectedValue.ToString())
                            && ((DateTime)x.tanggal).Year >= Int32.Parse(comboTahun1.SelectedValue.ToString())
                            && ((DateTime)x.tanggal).Month <= Int32.Parse(comboBulan2.SelectedValue.ToString())
                            && ((DateTime)x.tanggal).Year <= Int32.Parse(comboTahun2.SelectedValue.ToString())
                            select x).ToList();


            for (int thn = Int32.Parse(comboTahun1.SelectedValue.ToString()); thn <= Int32.Parse(comboTahun2.SelectedValue.ToString()); thn++)
            {
                for (int bln = Int32.Parse(comboBulan1.SelectedValue.ToString()); bln <= Int32.Parse(comboBulan2.SelectedValue.ToString()); bln++)
                {
                    for (int tgl = 1; tgl <= DateTime.DaysInMonth(thn, bln); tgl++)
                    {
                        var allDenom = (from x in listData
                                        where ((DateTime)x.tanggal).Day == tgl
                                        && ((DateTime)x.tanggal).Month == bln
                                        && ((DateTime)x.tanggal).Year == thn
                                        select x).ToList();

                        var allDenomIn = (from x in listData
                                          where ((DateTime)x.tanggal).Day == tgl + 1
                                          && ((DateTime)x.tanggal).Month == bln
                                          && ((DateTime)x.tanggal).Year == thn
                                          select x).ToList();

                        if (tgl == DateTime.DaysInMonth(thn, bln))
                        {
                            int tTgl = 1;

                            if (bln == 12)
                            {
                                allDenomIn = (from x in listData
                                              where ((DateTime)x.tanggal).Day == tTgl
                                              && ((DateTime)x.tanggal).Month == 1
                                              && ((DateTime)x.tanggal).Year == thn + 1
                                              select x).ToList();
                            }
                            else
                            {
                                allDenomIn = (from x in listData
                                              where ((DateTime)x.tanggal).Day == tTgl
                                              && ((DateTime)x.tanggal).Month == bln + 1
                                              && ((DateTime)x.tanggal).Year == thn
                                              select x).ToList();
                            }
                        }
                        else
                        {
                            DateTime temp = new DateTime(thn, bln, tgl + 1);
                            Console.WriteLine(temp);
                            allDenomIn = (from x in listData
                                          where (DateTime)x.tanggal == temp
                                          select x).ToList();
                        }


                        Int64 unprocessedBesar = (Int64)(from x in allDenom
                                                         where x.denom == "100000"
                                                         || x.denom == "50000"
                                                         select new { value = hitungPcs((Int64)x.unprocessed, x.denom) }).Sum(x => x.value);

                        Int64 unprocessedKecil = (Int64)(from x in allDenom
                                                         where x.denom == "20000"
                                                         || x.denom == "10000"
                                                         || x.denom == "5000"
                                                         || x.denom == "2000"
                                                         || x.denom == "1000"
                                                         || x.denom == "500"
                                                         || x.denom == "200"
                                                         || x.denom == "100"
                                                         || x.denom == "50"
                                                         || x.denom == "25"
                                                         select new { value = hitungPcs((Int64)x.unprocessed, x.denom) }).Sum(x => x.value);

                        Int64 inCabangBesar = (Int64)(from x in allDenomIn
                                                      where x.denom == "100000"
                                                      || x.denom == "50000"
                                                      select new { value = hitungPcs((Int64)x.inCabang, x.denom) }).Sum(x => x.value);

                        Int64 inCabangKecil = (Int64)(from x in allDenomIn
                                                      where !(x.denom == "100000" || x.denom == "50000")
                                                      select new { value = hitungPcs((Int64)x.inCabang, x.denom) }).Sum(x => x.value);
                        Int64 inRetailBesar = (Int64)(from x in allDenomIn
                                                      where x.denom == "100000"
                                                      || x.denom == "50000"
                                                      select new { value = hitungPcs((Int64)x.inRetail, x.denom) }).Sum(x => x.value);

                        Int64 inRetailKecil = (Int64)(from x in allDenomIn
                                                      where x.denom == "20000"
                                                      || x.denom == "10000"
                                                      || x.denom == "5000"
                                                      || x.denom == "2000"
                                                      || x.denom == "1000"
                                                      || x.denom == "500"
                                                      || x.denom == "200"
                                                      || x.denom == "100"
                                                      || x.denom == "50"
                                                      || x.denom == "25"
                                                      select new { value = hitungPcs((Int64)x.inRetail, x.denom) }).Sum(x => x.value);
                        Console.WriteLine(thn + " " + bln + " " + tgl);
                        sla.Add(new slaProsesDisplay()
                        {
                            tanggal = new DateTime(thn, bln, tgl),
                            unprocUangBesar = unprocessedBesar,
                            unprocUangKecil = unprocessedKecil,
                            inCabangUangBesar = inCabangBesar,
                            inCabangUangKecil = inCabangKecil,
                            inRetailUangBesar = inRetailBesar,
                            inRetailUangKecil = inRetailKecil,
                            slaProsesBesar = 0,
                            slaProsesKecil = 0,
                            totalProsesUangBesar = 0,
                            totalProsesUangKecil = 0
                        });
                    }
                }
            }
            for (int i = 0; i < sla.Count; i++)
            {
                if (i == sla.Count - 1)
                {
                    DateTime tgl = sla[i].tanggal.AddDays(1);
                    Int64 UnprocUangBesar = (from x in db.StokPosisis.AsEnumerable()
                                             where (x.denom == "100000"
                                             || x.denom == "50000")
                                             && x.tanggal == tgl
                                             select new { value = hitungPcs((Int64)x.unprocessed, x.denom) }
                             ).Sum(x => x.value);
                    Int64 UnprocUangKecil = (from x in db.StokPosisis.AsEnumerable()
                                             where !(x.denom == "100000"
                                             || x.denom == "50000")
                                             && x.tanggal == tgl
                                             select new { value = hitungPcs((Int64)x.unprocessed, x.denom) }
                             ).Sum(x => x.value);

                    sla[i].totalProsesUangBesar = sla[i].unprocUangBesar + sla[i].inCabangUangBesar + sla[i].inRetailUangBesar;
                    sla[i].totalProsesUangKecil = sla[i].unprocUangKecil + sla[i].inCabangUangKecil + sla[i].inRetailUangKecil;

                    sla[i].totalProsesUangBesar -= UnprocUangBesar;
                    sla[i].totalProsesUangKecil -= UnprocUangKecil;
                }
                else
                {
                    sla[i].totalProsesUangBesar = sla[i].unprocUangBesar + sla[i].inCabangUangBesar + sla[i].inRetailUangBesar - sla[i + 1].unprocUangBesar;
                    sla[i].totalProsesUangKecil = sla[i].unprocUangKecil + sla[i].inCabangUangKecil + sla[i].inRetailUangKecil - sla[i + 1].unprocUangKecil;
                }

                //if (sla[i].unprocUangBesar + sla[i].inCabangUangBesar + sla[i].inRetailUangBesar != 0)
                //{
                sla[i].slaProsesBesar = (Double)sla[i].totalProsesUangBesar / (sla[i].unprocUangBesar + sla[i].inCabangUangBesar + sla[i].inRetailUangBesar);
                //}
                //if (sla[i].unprocUangKecil + sla[i].inCabangUangKecil + sla[i].inRetailUangKecil != 0)
                //{
                sla[i].slaProsesKecil = (Double)sla[i].totalProsesUangKecil / (sla[i].unprocUangKecil + (sla[i].inCabangUangKecil / 2) + sla[i].inRetailUangKecil);
                //}
                //if (sla[i].unprocUangBesar + sla[i].inCabangUangBesar + sla[i].inRetailUangBesar + sla[i].unprocUangKecil + sla[i].inCabangUangKecil + sla[i].inRetailUangKecil != 0)
                //{
                if (!Double.IsNaN(sla[i].slaProsesBesar) && !Double.IsNaN(sla[i].slaProsesKecil))
                    sla[i].slaGabung = ((Double)sla[i].totalProsesUangBesar + (Double)sla[i].totalProsesUangKecil) / (sla[i].unprocUangBesar + sla[i].inCabangUangBesar + sla[i].inRetailUangBesar + sla[i].unprocUangKecil + (sla[i].inCabangUangKecil / 2) + sla[i].inRetailUangKecil);
                else if (!Double.IsNaN(sla[i].slaProsesBesar))
                    sla[i].slaGabung = sla[i].slaProsesKecil;
                else if (!Double.IsNaN(sla[i].slaProsesBesar))
                    sla[i].slaGabung = sla[i].slaProsesBesar;
                else
                    sla[i].slaGabung = Double.NaN;
                //}


                if (!Double.IsNaN(sla[i].slaProsesBesar))
                {
                    if (sla[i].slaProsesBesar > 1)
                        sla[i].slaProsesBesar = 1;
                    if (sla[i].slaProsesBesar < 0)
                        sla[i].slaProsesBesar = 0;
                }
                if (!Double.IsNaN(sla[i].slaProsesKecil))
                {
                    if (sla[i].slaProsesKecil > 1)
                        sla[i].slaProsesKecil = 1;
                    if (sla[i].slaProsesKecil < 0)
                        sla[i].slaProsesKecil = 0;
                }
                if (!Double.IsNaN(sla[i].slaGabung))
                {
                    if (sla[i].slaGabung > 1)
                        sla[i].slaGabung = 1;
                    if (sla[i].slaGabung < 0)
                        sla[i].slaGabung = 0;
                }
            }
            Double buf;

            List<slaProsesDisplay> testq = (from x in sla
                         where !Double.IsNaN(x.slaProsesBesar)
                         select x).ToList();
            foreach(var temp in testq)
            {
                Console.WriteLine(temp.slaProsesBesar);
            }
            slaProsesDisplay avg = new slaProsesDisplay()
            {
                tanggal = new DateTime(1, 1, 1),
                inCabangUangBesar = (Int64)Math.Round(sla.Average(x => x.inCabangUangBesar), 0),
                inCabangUangKecil = (Int64)Math.Round(sla.Average(x => x.inCabangUangKecil), 0),
                inRetailUangBesar = (Int64)Math.Round(sla.Average(x => x.inRetailUangBesar), 0),
                inRetailUangKecil = (Int64)Math.Round(sla.Average(x => x.inRetailUangKecil), 0),
                unprocUangBesar = (Int64)Math.Round(sla.Average(x => x.unprocUangBesar), 0),
                unprocUangKecil = (Int64)Math.Round(sla.Average(x => x.unprocUangKecil), 0),
                totalProsesUangBesar = (Int64)Math.Round(sla.Average(x => x.totalProsesUangBesar), 0),
                totalProsesUangKecil = (Int64)Math.Round(sla.Average(x => x.totalProsesUangKecil), 0),
                slaProsesBesar = sla.Where(x=> !Double.IsNaN(x.slaProsesBesar)).Average(x => x.slaProsesBesar),
                slaProsesKecil = sla.Where(x=> !Double.IsNaN(x.slaProsesKecil)).Average(x => x.slaProsesKecil),
                slaGabung = sla.Where(x => !Double.IsNaN(x.slaGabung)).Average(x => x.slaGabung)
            };
            slaProsesDisplay sum = new slaProsesDisplay()
            {
                tanggal = new DateTime(1, 1, 1),
                inCabangUangBesar = (Int64)sla.Sum(x => x.inCabangUangBesar),
                inCabangUangKecil = (Int64)sla.Sum(x => x.inCabangUangKecil),
                inRetailUangBesar = (Int64)sla.Sum(x => x.inRetailUangBesar),
                inRetailUangKecil = (Int64)sla.Sum(x => x.inRetailUangKecil),
                unprocUangBesar = (Int64)sla.Sum(x => x.unprocUangBesar),
                unprocUangKecil = (Int64)sla.Sum(x => x.unprocUangKecil),
                totalProsesUangBesar = (Int64)sla.Sum(x => x.totalProsesUangBesar),
                totalProsesUangKecil = (Int64)sla.Sum(x => x.totalProsesUangKecil),
                slaProsesBesar = 0,
                slaProsesKecil = 0
            };

            sla.Add(sum);
            sla.Add(avg);

            dataGridView1.DataSource = sla;
            for (int a = 1; a < dataGridView1.Columns.Count - 2; a++)
            {
                dataGridView1.Columns[a].DefaultCellStyle.Format = "N0";
                dataGridView1.Columns[a].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            }
            dataGridView1.Columns[dataGridView1.Columns.Count - 3].DefaultCellStyle.Format = "0.%";
            dataGridView1.Columns[dataGridView1.Columns.Count - 2].DefaultCellStyle.Format = "0.%";
            dataGridView1.Columns[dataGridView1.Columns.Count - 1].DefaultCellStyle.Format = "0.%";

            dataGridView1.Rows[dataGridView1.Rows.Count - 1].DefaultCellStyle.BackColor = Color.LightSkyBlue;
            dataGridView1.Rows[dataGridView1.Rows.Count - 2].DefaultCellStyle.BackColor = Color.LightGreen;
        }
        public Int64 hitungPcs(Int64 value, String denom)
        {
            return value / Int32.Parse(denom);
        }
        class slaProsesDisplay
        {
            public DateTime tanggal { set; get; }
            public Int64 unprocUangBesar { set; get; }
            public Int64 unprocUangKecil { set; get; }
            public Int64 inCabangUangBesar { set; get; }
            public Int64 inCabangUangKecil { set; get; }
            public Int64 inRetailUangBesar { set; get; }
            public Int64 inRetailUangKecil { set; get; }
            public Int64 totalProsesUangBesar { set; get; }
            public Int64 totalProsesUangKecil { set; get; }
            public Double slaProsesBesar { set; get; }
            public Double slaProsesKecil { set; get; }
            public Double slaGabung { set; get; }
        }

        private void comboTahun1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            loadComboBulan1();
        }

        private void comboTahun2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            loadComboBulan2();
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            //reloadData();
            reloadDataFromStokPosisi();
        }

        private void comboNamaPkt_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //Console.WriteLine(comboNamaPkt.SelectedIndex);
            initCombos();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog sv = new SaveFileDialog();
            sv.Filter = Variables.csvFilter;
            if(sv.ShowDialog() == DialogResult.OK)
            {
                String csv = ServiceStack.Text.CsvSerializer.SerializeToString(sla);
                File.WriteAllText(sv.FileName, csv);
            }
        }
    }
}
public static class MyStringExtensions
{
    public static bool Like(this string toSearch, string toFind)
    {
        return new Regex(@"\A" + new Regex(@"\.|\$|\^|\{|\[|\(|\||\)|\*|\+|\?|\\").Replace(toFind, ch => @"\" + ch).Replace('_', '.').Replace("%", ".*") + @"\z", RegexOptions.Singleline).IsMatch(toSearch);
    }
}
