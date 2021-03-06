﻿using System;
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
        List<slaProsesDisplayAllVendor> slapav;
        List<slaProsesDisplayAllVendor> resultAllVendor;
        List<String> tahun1 = new List<String>();
        List<String> tahun2 = new List<String>();
        int jumlahPkt;
        String koordinator;
        String jenisTampilan;
        public SLAProsesForm()
        {
            InitializeComponent();
            tampilanUangComboBox.SelectedIndex = 0;
            //dataGridView1.AutoGenerateColumns = false;
            if (loadComboSLA())
            {
                loadComboPkt();

                comboNamaPkt.SelectedIndex = 0;

                initCombos();
            }
            else
            {
                MessageBox.Show("Data tidak ada!");
                this.Close();
            }
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

                    bulan1 = bulan1.OrderByDescending(x=>Int32.Parse(x)).ToList();
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
                              orderby Int32.Parse(x) descending
                              select x).ToList();
                    comboBulan2.DataSource = bulan2;
                }
            }
        }
        bool loadComboSLA()
        {
            List<String> listSLA = (from x in db.Pkts select x.koordinator).Distinct().ToList();
            listSLA.AddRange((from x in db.Pkts select x.vendor).Distinct().ToList());
            if (listSLA.Any())
            {
                slaComboBox.DataSource = listSLA;
                slaComboBox.SelectedIndex = 0;
                koordinator = listSLA[0];
                return true;
            }
            else
            {
                return false;
            }
        }
        void loadComboPkt()
        {
            String koordinator = slaComboBox.SelectedItem.ToString();

            List<String>listPkt = (from x in db.Pkts where (x.koordinator == koordinator || x.vendor == koordinator) && x.kodePktCabang != "" select x.namaPkt).ToList();
            listPkt = listPkt.Select(x => x.ToUpper().Contains("CASH PROCESSING CENTER ALAM SUTERA") ? "CASH PROCESSING CENTER ALAM SUTERA" : x).Distinct().ToList();

            jumlahPkt = listPkt.Count;

            listPkt.Add("All Vendor ("+ koordinator +")");
            listPkt.Add("All Vendor (" + koordinator + ")" + " Detail");
            comboNamaPkt.DataSource = listPkt;
        }
        public void reloadData()
        {
            sla = new List<slaProsesDisplay>();

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
                        //Console.WriteLine(thn + " " + bln + " " + tgl);
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
            Console.WriteLine(jumlahPkt);
            Console.WriteLine(comboNamaPkt.SelectedIndex);
            if (comboNamaPkt.SelectedIndex < jumlahPkt)
            {
                loadDataPerPKTStokPosisi();
                String jenisTampilan = tampilanUangComboBox.SelectedItem.ToString();
                if(jenisTampilan == "Nominal")
                {
                    sla = sla.Select(x => new slaProsesDisplay()
                    {
                        inCabangUangBesar = x.inCabangUangBesar,
                        inCabangUangKecil = x.inCabangUangKecil,
                        inRetailUangBesar = x.inRetailUangBesar,
                        inRetailUangKecil = x.inRetailUangKecil,
                        slaGabung = 0,
                        slaProsesBesar = 0,
                        slaProsesKecil = 0,
                        tanggal = x.tanggal,
                        totalProsesUangBesar = x.totalProsesUangBesar,
                        totalProsesUangKecil = x.totalProsesUangKecil, 
                        unprocUangBesar = x.unprocUangBesar,
                        unprocUangKecil = x.unprocUangKecil
                    }).ToList();
                    dataGridView1.DataSource = sla;
                }
            }
            else if (comboNamaPkt.SelectedIndex == jumlahPkt)
            {
                var res = loadDataAllVendorStokPosisi();
                resultAllVendor = res;
                if (jenisTampilan == "Nominal")
                {
                    resultAllVendor = res.Select(x => new slaProsesDisplayAllVendor()
                    {
                        namaPkt = x.namaPkt,
                        inCabangUangBesar = x.inCabangUangBesar,
                        inCabangUangKecil = x.inCabangUangKecil,
                        inRetailUangBesar = x.inRetailUangBesar,
                        inRetailUangKecil = x.inRetailUangKecil,
                        slaGabung = 0,
                        slaProsesBesar = 0,
                        slaProsesKecil = 0,
                        tanggal = x.tanggal,
                        totalProsesUangBesar = x.totalProsesUangBesar,
                        totalProsesUangKecil = x.totalProsesUangKecil,
                        unprocUangBesar = x.unprocUangBesar,
                        unprocUangKecil = x.unprocUangKecil
                    }).ToList();
                    dataGridView1.DataSource = res;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].DefaultCellStyle.BackColor = Color.LightSkyBlue;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 2].DefaultCellStyle.BackColor = Color.LightGreen;
                }
            }
            else
            {
                var res = loadDataAllVendorDetailStokPosisi();
                if (jenisTampilan == "Nominal")
                {
                    res = res.Select(x => new Result()
                    {
                        namaPkt = x.namaPkt,
                        InCabangBesar = x.InCabangBesar,
                        InCabangKecil = x.InCabangKecil,
                        InRetailBesar = x.InRetailBesar,
                        InRetailKecil = x.InRetailKecil,
                        slaGabung = 0,
                        slaBesar = 0,
                        slaKecil = 0,
                        tanggal = x.tanggal,
                        UnprocessedBesar = x.UnprocessedBesar,
                        UnprocessedKecil = x.UnprocessedKecil,
                        TotalProcessUangBesar = x.TotalProcessUangBesar,
                        TotalProcessUangKecil = x.TotalProcessUangKecil
                    }).ToList();
                    dataGridView1.DataSource = res;
                }
            }
        }
        public List<slaProsesDisplayAllVendor> loadDataAllVendorStokPosisi()
        {
            slapav = new List<slaProsesDisplayAllVendor>();

            var listAllStokPosisiData = (from x in db.StokPosisis
                                         join z in db.Pkts on x.namaPkt equals z.namaPkt
                                         where (z.koordinator == koordinator || z.vendor == koordinator)
                                         select x).ToList();
            var toChange = listAllStokPosisiData.Where(x => x.namaPkt.Contains("CASH PROCESSING CENTER ALAM SUTERA")).ToList();
            foreach (var temp in toChange)
                temp.namaPkt = "CASH PROCESSING CENTER ALAM SUTERA";
            var listData = (from x in db.StokPosisis.AsEnumerable()
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
                        var unprocessedBesar = (from x in allDenom
                                                group x by new { x.namaPkt, x.denom, x.tanggal } into g
                                                where g.Key.denom == "100000"
                                                || g.Key.denom == "50000"
                                                select new { namapkt = g.Key.namaPkt, value = tampilanUang((Int64)g.Sum(x => x.unprocessed), g.Key.denom, jenisTampilan), tanggal = g.Key.tanggal }).GroupBy(x => new { x.namapkt, x.tanggal }).Select(x => new { namapkt = x.Key.namapkt, tanggal = x.Key.tanggal, value = x.Sum(y => y.value) }).ToList();

                        var unprocessedKecil = (from x in allDenom
                                                group x by new { x.namaPkt, x.denom, x.tanggal } into g
                                                where !(g.Key.denom == "100000"
                                                        || g.Key.denom == "50000")
                                                select new { namapkt = g.Key.namaPkt, value = tampilanUang((Int64)g.Sum(x => x.unprocessed), g.Key.denom, jenisTampilan), tanggal = g.Key.tanggal }).GroupBy(x => new { x.namapkt, x.tanggal }).Select(x => new { namapkt = x.Key.namapkt, tanggal = x.Key.tanggal, value = x.Sum(y => y.value) }).ToList();

                        var inCabangBesar = (from x in allDenom
                                             group x by new { x.namaPkt, x.denom, x.tanggal } into g
                                             where g.Key.denom == "100000"
                                                     || g.Key.denom == "50000"
                                             select new { namapkt = g.Key.namaPkt, value = tampilanUang((Int64)g.Sum(x => x.inCabang), g.Key.denom, jenisTampilan), tanggal = g.Key.tanggal }).GroupBy(x => new { x.namapkt, x.tanggal }).Select(x => new { namapkt = x.Key.namapkt, tanggal = x.Key.tanggal, value = x.Sum(y => y.value) }).ToList();

                        var inCabangKecil = (from x in allDenom
                                             group x by new { x.namaPkt, x.denom, x.tanggal } into g
                                             where !(g.Key.denom == "100000" || g.Key.denom == "50000")
                                             select new { namapkt = g.Key.namaPkt, value = tampilanUang((Int64)g.Sum(x => x.inCabang), g.Key.denom, jenisTampilan), tanggal = g.Key.tanggal }).GroupBy(x => new { x.namapkt, x.tanggal }).Select(x => new { namapkt = x.Key.namapkt, tanggal = x.Key.tanggal, value = x.Sum(y => y.value) }).ToList();
                        var inRetailBesar = (from x in allDenom
                                             group x by new { x.namaPkt, x.denom, x.tanggal } into g
                                             where g.Key.denom == "100000"
                                                     || g.Key.denom == "50000"
                                             select new { namapkt = g.Key.namaPkt, value = tampilanUang((Int64)g.Sum(x => x.inRetail), g.Key.denom, jenisTampilan), tanggal = g.Key.tanggal }).GroupBy(x => new { x.namapkt, x.tanggal }).Select(x => new { namapkt = x.Key.namapkt, tanggal = x.Key.tanggal, value = x.Sum(y => y.value) }).ToList();

                        var inRetailKecil = (from x in allDenom
                                             group x by new { x.namaPkt, x.denom,x.tanggal } into g
                                             where !(g.Key.denom == "100000"
                                                     || g.Key.denom == "50000")
                                             select new { namapkt = g.Key.namaPkt, value = tampilanUang((Int64)g.Sum(x => x.inRetail), g.Key.denom, jenisTampilan), tanggal = g.Key.tanggal }).GroupBy(x => new { x.namapkt, x.tanggal }).Select(x => new { namapkt = x.Key.namapkt, tanggal = x.Key.tanggal, value = x.Sum(y => y.value) }).ToList();


                        DateTime date = new DateTime(thn, bln, tgl);
                        var unprocessedBesarH1 = (from x in listAllStokPosisiData.AsEnumerable()
                                                  where x.tanggal == date.AddDays(1)
                                                  && (x.denom == "100000" || x.denom == "50000")
                                                  select new { namapkt = x.namaPkt, value = tampilanUang((Int64) x.unprocessed, x.denom, jenisTampilan), x.tanggal }).GroupBy(x=>new { x.tanggal, x.namapkt }).Select(x=> new { namapkt = x.Key.namapkt, value = x.Sum(y => y.value) }).ToList();

                        var unprocessedKecilH1 = (from x in listAllStokPosisiData.AsEnumerable()
                                                  where x.tanggal == date.AddDays(1)
                                                  && !(x.denom == "100000" || x.denom == "50000")
                                                  select new { namapkt = x.namaPkt, value = tampilanUang((Int64)x.unprocessed, x.denom, jenisTampilan), x.tanggal }).GroupBy(x => new { x.tanggal, x.namapkt }).Select(x => new { namapkt = x.Key.namapkt, value = x.Sum(y => y.value) }).ToList();


                        var q = (from ub in unprocessedBesar
                                 join uk in unprocessedKecil on ub.namapkt equals uk.namapkt
                                 join icb in inCabangBesar on ub.namapkt equals icb.namapkt
                                 join ick in inCabangKecil on ub.namapkt equals ick.namapkt
                                 join irb in inRetailBesar on ub.namapkt equals irb.namapkt
                                 join irk in inRetailKecil on ub.namapkt equals irk.namapkt
                                 join ubh in unprocessedBesarH1 on ub.namapkt equals ubh.namapkt
                                 join ukh in unprocessedKecilH1 on ub.namapkt equals ukh.namapkt
                                 select new slaProsesDisplayAllVendor()
                                 {
                                     namaPkt = ub.namapkt,
                                     unprocUangBesar = (Int64)ub.value,
                                     unprocUangKecil = (Int64)uk.value,
                                     inCabangUangBesar = (Int64)icb.value,
                                     inCabangUangKecil = (Int64)ick.value,
                                     inRetailUangBesar = (Int64)irb.value,
                                     inRetailUangKecil = (Int64)irk.value,
                                     tanggal = date,
                                     slaProsesBesar = hitungSLA((Int64)ub.value, (Int64)irb.value, (Int64)icb.value, ubh.value, "Besar"),
                                     slaProsesKecil = hitungSLA((Int64)uk.value, (Int64)irk.value, (Int64)ick.value, ukh.value, "Kecil"),
                                     slaGabung = hitungSLA((Int64)ub.value, (Int64)irb.value, (Int64)icb.value, ubh.value,(Int64)uk.value,(Int64)irk.value, (Int64)ick.value,(Int64)ukh.value)
                                 }).ToList();
                        slapav.AddRange(q);
                        //sampe siNI OK
                    }
                }
            }
            Double buf;

            int slaBesarNonNANCount = slapav.Where(x => !Double.IsNaN(x.slaProsesBesar)).ToList().Count,
                slaKecilNonNANCount = slapav.Where(x => !Double.IsNaN(x.slaProsesKecil)).ToList().Count;

            var tempSlaBesar = (from x in slapav
                                group x by x.namaPkt into g
                                select new { namaPkt = g.Key, slaBesar = (g.Where(x => !Double.IsNaN(x.slaProsesBesar)).ToList().Count > 0 ? g.Where(x => !Double.IsNaN(x.slaProsesBesar)).Average(x => x.slaProsesBesar) : Double.NaN) }).ToList();
            var tempSlaKecil = (from x in slapav
                                group x by x.namaPkt into g
                                select new { namaPkt = g.Key, slaKecil = (g.Where(x => !Double.IsNaN(x.slaProsesKecil)).ToList().Count > 0 ? g.Where(x => !Double.IsNaN(x.slaProsesKecil)).Average(x => x.slaProsesKecil) : Double.NaN) }).ToList();
            var tempSlaGabung = (from x in slapav
                                group x by x.namaPkt into g
                                select new { namaPkt = g.Key, slaGabung = g.Where(x => !Double.IsNaN(x.slaGabung)).Average(x => x.slaGabung) }).ToList();

            var qt = (from x in slapav
                      group x by x.namaPkt into g
                      join y in tempSlaBesar on g.Key equals y.namaPkt
                      join z in tempSlaKecil on g.Key equals z.namaPkt
                      join zz in tempSlaGabung on g.Key equals zz.namaPkt
                      select new slaProsesDisplayAllVendor()
                      {
                          namaPkt = g.Key,
                          unprocUangBesar = (Int64)g.Average(x=>x.unprocUangBesar),
                          unprocUangKecil = (Int64)g.Average(x=>x.unprocUangKecil),
                          inRetailUangBesar = (Int64)g.Average(x=>x.inRetailUangBesar),
                          inRetailUangKecil = (Int64)g.Average(x=>x.inRetailUangKecil),
                          inCabangUangBesar= (Int64)g.Average(x=>x.inCabangUangBesar),
                          inCabangUangKecil = (Int64)g.Average(x=>x.inCabangUangKecil),
                          totalProsesUangBesar = (Int64)(g.Average(x => x.unprocUangBesar) + g.Average(x => x.inRetailUangBesar) + g.Average(x => x.inCabangUangBesar)),
                          totalProsesUangKecil = (Int64)(g.Average(x => x.unprocUangKecil) + g.Average(x => x.inRetailUangKecil) + g.Average(x => x.inCabangUangKecil)),
                          slaProsesBesar = y.slaBesar,
                          slaProsesKecil = z.slaKecil,
                          slaGabung = zz.slaGabung
                      }).ToList();
            Console.WriteLine("QT COUNT: " + qt.Count);
            var avg = (from x in qt
                       group x by true into g
                       select new slaProsesDisplayAllVendor()
                       {
                           namaPkt = "",
                           unprocUangBesar = (Int64)g.Average(x => x.unprocUangBesar),
                           unprocUangKecil = (Int64)g.Average(x => x.unprocUangKecil),
                           inRetailUangBesar = (Int64)g.Average(x => x.inRetailUangBesar),
                           inRetailUangKecil = (Int64)g.Average(x => x.inRetailUangKecil),
                           inCabangUangBesar = (Int64)g.Average(x => x.inCabangUangBesar),
                           inCabangUangKecil = (Int64)g.Average(x => x.inCabangUangKecil),
                           totalProsesUangBesar = (Int64)g.Average(x => x.totalProsesUangBesar),
                           totalProsesUangKecil = (Int64)g.Average(x => x.totalProsesUangKecil),
                           slaProsesBesar = g.Average(x => x.slaProsesBesar),
                           slaProsesKecil = g.Average(x => x.slaProsesKecil),
                           slaGabung = g.Average(x => x.slaGabung)
                       }).First();
            var sum = (from x in qt
                       group x by true into g
                       select new slaProsesDisplayAllVendor()
                       {
                           namaPkt = "",
                           //unprocUangBesar = (Int64)g.Sum(x => x.unprocUangBesar),
                           //unprocUangKecil = (Int64)g.Sum(x => x.unprocUangKecil),
                           //inRetailUangBesar = (Int64)g.Sum(x => x.inRetailUangBesar),
                           //inRetailUangKecil = (Int64)g.Sum(x => x.inRetailUangKecil),
                           //inCabangUangBesar = (Int64)g.Sum(x => x.inCabangUangBesar),
                           //inCabangUangKecil = (Int64)g.Sum(x => x.inCabangUangKecil),
                           //totalProsesUangBesar = (Int64)g.Sum(x => x.totalProsesUangBesar),
                           //totalProsesUangKecil = (Int64)g.Sum(x => x.totalProsesUangKecil),
                           //slaProsesBesar = (Double)0,
                           //slaProsesKecil = (Double)0,
                           //slaGabung = (Double)0
                       }).First();
            qt.Add(sum);
            qt.Add(avg);
            dataGridView1.DataSource = qt;
            for(int a=1;a<dataGridView1.Columns.Count;a++)
            {
                if(a >=10 && a<=12)
                    dataGridView1.Columns[a].DefaultCellStyle.Format = "P";
                else
                    dataGridView1.Columns[a].DefaultCellStyle.Format = "N2";
            }
            dataGridView1.Rows[dataGridView1.Rows.Count - 1].DefaultCellStyle.BackColor = Color.LightSkyBlue;
            dataGridView1.Rows[dataGridView1.Rows.Count - 2].DefaultCellStyle.BackColor = Color.LightGreen;
            return qt;
        }
        public List<Result> loadDataAllVendorDetailStokPosisi()
        {
            slapav = new List<slaProsesDisplayAllVendor>();

            var listAllStokPosisiData = (from x in db.StokPosisis
                                         join z in db.Pkts on x.namaPkt equals z.namaPkt
                                         where (z.koordinator == koordinator || z.vendor == koordinator)
                                         select x).ToList();
            var toChange = listAllStokPosisiData.Where(x => x.namaPkt.Contains("CASH PROCESSING CENTER ALAM SUTERA")).ToList();
            foreach (var temp in toChange)
                temp.namaPkt = "CASH PROCESSING CENTER ALAM SUTERA";
            var listData = (from x in db.StokPosisis.AsEnumerable()
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
                        var unprocessedBesar = (from x in allDenom
                                                group x by new { x.namaPkt, x.denom, x.tanggal } into g
                                                where g.Key.denom == "100000"
                                                || g.Key.denom == "50000"
                                                select new { namapkt = g.Key.namaPkt, value = tampilanUang((Int64)g.Sum(x => x.unprocessed), g.Key.denom, jenisTampilan), tanggal = g.Key.tanggal }).GroupBy(x => new { x.namapkt, x.tanggal }).Select(x => new { namapkt = x.Key.namapkt, tanggal = x.Key.tanggal, value = x.Sum(y => y.value) }).ToList();

                        var unprocessedKecil = (from x in allDenom
                                                group x by new { x.namaPkt, x.denom, x.tanggal } into g
                                                where !(g.Key.denom == "100000"
                                                        || g.Key.denom == "50000")
                                                select new { namapkt = g.Key.namaPkt, value = tampilanUang((Int64)g.Sum(x => x.unprocessed), g.Key.denom, jenisTampilan), tanggal = g.Key.tanggal }).GroupBy(x => new { x.namapkt, x.tanggal }).Select(x => new { namapkt = x.Key.namapkt, tanggal = x.Key.tanggal, value = x.Sum(y => y.value) }).ToList();

                        var inCabangBesar = (from x in allDenom
                                             group x by new { x.namaPkt, x.denom, x.tanggal } into g
                                             where g.Key.denom == "100000"
                                                     || g.Key.denom == "50000"
                                             select new { namapkt = g.Key.namaPkt, value = tampilanUang((Int64)g.Sum(x => x.inCabang), g.Key.denom, jenisTampilan), tanggal = g.Key.tanggal }).GroupBy(x => new { x.namapkt, x.tanggal }).Select(x => new { namapkt = x.Key.namapkt, tanggal = x.Key.tanggal, value = x.Sum(y => y.value) }).ToList();

                        var inCabangKecil = (from x in allDenom
                                             group x by new { x.namaPkt, x.denom, x.tanggal } into g
                                             where !(g.Key.denom == "100000" || g.Key.denom == "50000")
                                             select new { namapkt = g.Key.namaPkt, value = tampilanUang((Int64)g.Sum(x => x.inCabang), g.Key.denom, jenisTampilan), tanggal = g.Key.tanggal }).GroupBy(x => new { x.namapkt, x.tanggal }).Select(x => new { namapkt = x.Key.namapkt, tanggal = x.Key.tanggal, value = x.Sum(y => y.value) }).ToList();
                        var inRetailBesar = (from x in allDenom
                                             group x by new { x.namaPkt, x.denom, x.tanggal } into g
                                             where g.Key.denom == "100000"
                                                     || g.Key.denom == "50000"
                                             select new { namapkt = g.Key.namaPkt, value = tampilanUang((Int64)g.Sum(x => x.inRetail), g.Key.denom, jenisTampilan), tanggal = g.Key.tanggal }).GroupBy(x => new { x.namapkt, x.tanggal }).Select(x => new { namapkt = x.Key.namapkt, tanggal = x.Key.tanggal, value = x.Sum(y => y.value) }).ToList();

                        var inRetailKecil = (from x in allDenom
                                             group x by new { x.namaPkt, x.denom, x.tanggal } into g
                                             where !(g.Key.denom == "100000"
                                                     || g.Key.denom == "50000")
                                             select new { namapkt = g.Key.namaPkt, value = tampilanUang((Int64)g.Sum(x => x.inRetail), g.Key.denom, jenisTampilan), tanggal = g.Key.tanggal }).GroupBy(x => new { x.namapkt, x.tanggal }).Select(x => new { namapkt = x.Key.namapkt, tanggal = x.Key.tanggal, value = x.Sum(y => y.value) }).ToList();


                        DateTime date = new DateTime(thn, bln, tgl);
                        var unprocessedBesarH1 = (from x in listAllStokPosisiData.AsEnumerable()
                                                  where x.tanggal == date.AddDays(1)
                                                  && (x.denom == "100000" || x.denom == "50000")
                                                  select new { namapkt = x.namaPkt, value = tampilanUang((Int64)x.unprocessed, x.denom, jenisTampilan), x.tanggal }).GroupBy(x => new { x.tanggal, x.namapkt }).Select(x => new { namapkt = x.Key.namapkt, value = x.Sum(y => y.value), tanggal = x.Key.tanggal }).ToList();

                        var unprocessedKecilH1 = (from x in listAllStokPosisiData.AsEnumerable()
                                                  where x.tanggal == date.AddDays(1)
                                                  && !(x.denom == "100000" || x.denom == "50000")
                                                  select new { namapkt = x.namaPkt, value = tampilanUang((Int64)x.unprocessed, x.denom, jenisTampilan), x.tanggal }).GroupBy(x => new { x.tanggal, x.namapkt }).Select(x => new { namapkt = x.Key.namapkt, value = x.Sum(y => y.value), tanggal = x.Key.tanggal }).ToList();


                        var q = (from ub in unprocessedBesar
                                 from uk in unprocessedKecil.Where(x=>x.namapkt == ub.namapkt && x.tanggal == ub.tanggal).DefaultIfEmpty()
                                 from icb in inCabangBesar.Where(x => x.namapkt == ub.namapkt && x.tanggal == ub.tanggal).DefaultIfEmpty()
                                 from ick in inCabangKecil.Where(x => x.namapkt == ub.namapkt && x.tanggal == ub.tanggal).DefaultIfEmpty()
                                 from irb in inRetailBesar.Where(x => x.namapkt == ub.namapkt && x.tanggal == ub.tanggal).DefaultIfEmpty()
                                 from irk in inRetailKecil.Where(x=>x.namapkt == ub.namapkt && x.tanggal == ub.tanggal).DefaultIfEmpty()
                                 join ubh in unprocessedBesarH1 on ub.namapkt equals ubh.namapkt
                                 join ukh in unprocessedKecilH1 on ub.namapkt equals ukh.namapkt
                                 select new slaProsesDisplayAllVendor()
                                 {
                                     namaPkt = ub.namapkt,
                                     unprocUangBesar = (Int64)ub.value,
                                     unprocUangKecil = (Int64)uk.value,
                                     inCabangUangBesar = (Int64)icb.value,
                                     inCabangUangKecil = (Int64)ick.value,
                                     inRetailUangBesar = (Int64)irb.value,
                                     inRetailUangKecil = (Int64)irk.value,
                                     tanggal = date,
                                     slaProsesBesar = hitungSLA((Int64)ub.value, (Int64)irb.value, (Int64)icb.value, ubh.value, "Besar"),
                                     slaProsesKecil = hitungSLA((Int64)uk.value, (Int64)irk.value, (Int64)ick.value, ukh.value, "Kecil"),
                                     slaGabung = hitungSLA((Int64)ub.value, (Int64)irb.value, (Int64)icb.value, ubh.value, (Int64)uk.value, (Int64)irk.value, (Int64)ick.value, (Int64)ukh.value) 
                                 }).ToList();
                        
                        slapav.AddRange(q);
                        //sampe siNI OK
                    }
                }
            }
            Double buf;

            var tempSlaBesar = (from x in slapav
                                group x by new { x.namaPkt,x.tanggal} into g
                                select new { namaPkt = g.Key.namaPkt, slaBesar = g.Average(x => x.slaProsesBesar), tanggal = g.Key.tanggal }).ToList();
            var tempSlaKecil = (from x in slapav
                                group x by new { x.namaPkt, x.tanggal } into g
                                select new { namaPkt = g.Key.namaPkt, slaKecil = g.Average(x => x.slaProsesKecil), tanggal = g.Key.tanggal }).ToList();
            var tempSlaGabung = (from x in slapav
                                 group x by new { x.namaPkt, x.tanggal } into g
                                 select new { namaPkt = g.Key.namaPkt, slaGabung = g.Average(x => x.slaGabung), tanggal = g.Key.tanggal }).ToList();
            
            var qt = (from x in slapav
                      group x by new { x.namaPkt, x.tanggal } into g
                      from y in tempSlaBesar.Where(a=>a.namaPkt == g.Key.namaPkt && a.tanggal == g.Key.tanggal)
                      from z in tempSlaKecil.Where(a => a.namaPkt == g.Key.namaPkt && a.tanggal == g.Key.tanggal)
                      from zz in tempSlaGabung.Where(a => a.namaPkt == g.Key.namaPkt && a.tanggal == g.Key.tanggal)
                      select new Result()
                      {
                          namaPkt = g.Key.namaPkt,
                          tanggal = g.Key.tanggal,
                          UnprocessedBesar = g.Average(x => x.unprocUangBesar),
                          UnprocessedKecil = g.Average(x => x.unprocUangKecil),
                          InCabangBesar = g.Average(x => x.inCabangUangBesar),
                          InCabangKecil = g.Average(x => x.inCabangUangKecil),
                          InRetailBesar = g.Average(x => x.inRetailUangBesar),
                          InRetailKecil = g.Average(x => x.inRetailUangKecil),
                          TotalProcessUangBesar = g.Average(x => x.unprocUangBesar) + g.Average(x => x.inRetailUangBesar) + g.Average(x => x.inCabangUangBesar),
                          TotalProcessUangKecil = g.Average(x => x.unprocUangKecil) + g.Average(x => x.inRetailUangKecil) + g.Average(x => x.inCabangUangKecil),
                          slaBesar = y.slaBesar,
                          slaKecil = z.slaKecil,
                          slaGabung = zz.slaGabung
                      }).ToList();

            int maxDayInMonth = DateTime.DaysInMonth(qt.Max(x => x.tanggal).Year, qt.Max(x => x.tanggal).Month);

            qt = (from x in qt
                  select new Result()
                  {
                     
                      namaPkt = printpkt(x.namaPkt),
                      tanggal = printTanggal(x.tanggal),
                      UnprocessedBesar = x.UnprocessedBesar,
                      UnprocessedKecil = x.UnprocessedKecil,
                      InCabangBesar = x.InCabangBesar,
                      InCabangKecil = x.InCabangKecil,
                      InRetailBesar = x.InRetailBesar,
                      InRetailKecil = x.InRetailKecil,
                      TotalProcessUangBesar =  x.tanggal.Day == maxDayInMonth ? hitungTotalProsesBulanDpn(x.TotalProcessUangBesar, x.tanggal, x.namaPkt, "Besar") : x.TotalProcessUangBesar - qt.Where(y => y.tanggal == x.tanggal.AddDays(1) && y.namaPkt == x.namaPkt).Select(y => y.UnprocessedBesar).FirstOrDefault(),
                      TotalProcessUangKecil =  x.tanggal.Day == maxDayInMonth ? hitungTotalProsesBulanDpn(x.TotalProcessUangKecil, x.tanggal, x.namaPkt, "Kecil") : x.TotalProcessUangKecil - qt.Where(y => y.tanggal == x.tanggal.AddDays(1) && y.namaPkt == x.namaPkt).Select(y => y.UnprocessedKecil).FirstOrDefault(),
                      slaBesar = x.slaBesar,
                      slaKecil = x.slaKecil,
                      slaGabung = x.slaGabung
                  }).ToList();

            int SLABesarNonNANCount = qt.Where(x => !Double.IsNaN(x.slaBesar)).ToList().Count,
                SLAKecilNonNANCount = qt.Where(x => !Double.IsNaN(x.slaKecil)).ToList().Count;

            var avg = (from x in qt
                       group x by true into g
                       select new Result()
                       {
                           namaPkt = "",
                           tanggal = new DateTime(1, 1, 1),
                           UnprocessedBesar = g.Average(x => x.UnprocessedBesar),
                           UnprocessedKecil = g.Average(x => x.UnprocessedKecil),
                           InCabangBesar = g.Average(x => x.InCabangBesar),
                           InCabangKecil = g.Average(x => x.InCabangKecil),
                           InRetailBesar = g.Average(x => x.InRetailBesar),
                           InRetailKecil = g.Average(x => x.InRetailKecil),
                           TotalProcessUangBesar = g.Average(x => x.TotalProcessUangBesar),
                           TotalProcessUangKecil = g.Average(x => x.TotalProcessUangKecil),
                           slaBesar = SLABesarNonNANCount > 0 ? g.Where(x => !Double.IsNaN(x.slaBesar)).Average(x => x.slaBesar) : Double.NaN,
                           slaKecil = SLAKecilNonNANCount > 0 ? g.Where(x => !Double.IsNaN(x.slaKecil)).Average(x => x.slaKecil) : Double.NaN,
                           slaGabung = g.Where(x => !Double.IsNaN(x.slaGabung)).Average(x => x.slaGabung)
                       }).First();
            var sum = (from x in qt
                       group x by true into g
                       select new Result()
                       {
                           namaPkt = "",
                           tanggal = new DateTime(1,1,1),
                           UnprocessedBesar = g.Sum(x => x.UnprocessedBesar),
                           UnprocessedKecil = g.Sum(x => x.UnprocessedKecil),
                           InCabangBesar = g.Sum(x => x.InCabangBesar),
                           InCabangKecil = g.Sum(x => x.InCabangKecil),
                           InRetailBesar = g.Sum(x => x.InRetailBesar),
                           InRetailKecil = g.Sum(x => x.InRetailKecil),
                           TotalProcessUangBesar = g.Sum(x => x.TotalProcessUangBesar),
                           TotalProcessUangKecil = g.Sum(x => x.TotalProcessUangKecil),
                           slaBesar = (Double)0,
                           slaKecil = (Double)0,
                           slaGabung = (Double)0
                       }).First();
            qt.Add(sum);
            qt.Add(avg);
            dataGridView1.DataSource = qt;
            for (int a = 2; a < dataGridView1.Columns.Count; a++)
            {
                if (a >= 10 && a <= 12)
                    dataGridView1.Columns[a].DefaultCellStyle.Format = "P";
                else
                    dataGridView1.Columns[a].DefaultCellStyle.Format = "N2";
            }
            dataGridView1.Rows[dataGridView1.Rows.Count - 1].DefaultCellStyle.BackColor = Color.LightSkyBlue;
            dataGridView1.Rows[dataGridView1.Rows.Count - 2].DefaultCellStyle.BackColor = Color.LightGreen;
            return qt;
        }
        public void loadDataPerPKTStokPosisi()
        {
            sla = new List<slaProsesDisplay>();
            String namaPkt = comboNamaPkt.SelectedItem.ToString();
            var listData = (from x in db.StokPosisis.AsEnumerable()
                        where ((DateTime)x.tanggal).Month >= Int32.Parse(comboBulan1.SelectedValue.ToString())
                        && ((DateTime)x.tanggal).Year >= Int32.Parse(comboTahun1.SelectedValue.ToString())
                        && ((DateTime)x.tanggal).Month <= Int32.Parse(comboBulan2.SelectedValue.ToString())
                        && ((DateTime)x.tanggal).Year <= Int32.Parse(comboTahun2.SelectedValue.ToString())
                        && x.namaPkt.Contains(namaPkt)
                        select x).ToList();
            int slaRetailBesar = (int)slaRetailBesarNum.Value,
                slaRetailKecil = (int)slaRetailKecilNum.Value,
                slaCabangBesar = (int)slaCabangBesarNum.Value,
                slaCabangKecil = (int)slaCabangKecilNum.Value,
                slaUnprocBesar = (int)slaCabangBesarNum.Value - 1,
                slaUnprocKecil = (int)slaCabangKecilNum.Value - 1;

            if (slaUnprocBesar <= 1)
                slaUnprocBesar = 1;
            if (slaUnprocKecil <= 1)
                slaUnprocKecil = 1;
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
                        if (!allDenom.Any())
                            continue;

                        Int64 unprocessedBesar = (Int64)(from x in allDenom
                                                         where x.denom == "100000"
                                                         || x.denom == "50000"
                                                         select new { value = tampilanUang((Int64)x.unprocessed, x.denom, jenisTampilan) }).Sum(x => x.value);

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
                                                         select new { value = tampilanUang((Int64)x.unprocessed, x.denom, jenisTampilan) }).Sum(x => x.value);

                        Int64 inCabangBesar = (Int64)(from x in allDenom
                                                      where x.denom == "100000"
                                                      || x.denom == "50000"
                                                      select new { value = tampilanUang(x.inCabang == null ? 0 : (Int64)x.inCabang, x.denom, jenisTampilan) }).Sum(x => x.value);

                        Int64 inCabangKecil = (Int64)(from x in allDenom
                                                      where !(x.denom == "100000" || x.denom == "50000")
                                                      select new { value = tampilanUang(x.inCabang == null ? 0 : (Int64)x.inCabang, x.denom, jenisTampilan) }).Sum(x => x.value);
                        Int64 inRetailBesar = (Int64)(from x in allDenom
                                                      where x.denom == "100000"
                                                      || x.denom == "50000"
                                                      select new { value = tampilanUang(x.inRetail == null ? 0 : (Int64) x.inRetail, x.denom, jenisTampilan) }).Sum(x => x.value);

                        Int64 inRetailKecil = (Int64)(from x in allDenom
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
                                                      select new { value = tampilanUang((x.inRetail == null ? 0 : (Int64)x.inRetail), x.denom, jenisTampilan) }).Sum(x => x.value);
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
            List<int> toDelete = new List<int>();
            for (int i = 0; i < sla.Count; i++)
            {
                if (i == sla.Count - 1)
                {
                    //Mesti diganti
                    DateTime tglSLA = sla[i].tanggal;
                    var UnprocUangBesar = (from x in db.StokPosisis.AsEnumerable()
                                           where (x.denom == "100000"
                                           || x.denom == "50000")
                                           && x.tanggal == tglSLA.AddDays(1)
                                           && x.namaPkt.Contains(comboNamaPkt.SelectedValue.ToString())
                                           select new { value = tampilanUang((Int64)x.unprocessed, x.denom, jenisTampilan) }
                                 ).ToList();
                    var UnprocUangKecil = (from x in db.StokPosisis.AsEnumerable()
                                           where !(x.denom == "100000"
                                           || x.denom == "50000")
                                           && x.tanggal == tglSLA.AddDays(1)
                                           && x.namaPkt.Contains(comboNamaPkt.SelectedValue.ToString())
                                           select new { value = tampilanUang((Int64)x.unprocessed, x.denom, jenisTampilan) }
                             ).ToList();


                    if (UnprocUangBesar.Any())
                    {
                        Console.WriteLine("UNPROC UANG BESAR ADA: " + UnprocUangBesar.Count);
                        sla[i].totalProsesUangBesar = sla[i].unprocUangBesar + sla[i].inCabangUangBesar + sla[i].inRetailUangBesar - UnprocUangBesar.Sum(x => x.value);
                    }
                    else
                    {
                        Console.WriteLine("UNPROC UANG BESAR TIDAK ADA");
                        sla[i].totalProsesUangBesar = double.NaN;

                    }
                    if (UnprocUangKecil.Any())
                        sla[i].totalProsesUangKecil = sla[i].unprocUangKecil + sla[i].inCabangUangKecil + sla[i].inRetailUangKecil - UnprocUangKecil.Sum(x => x.value);
                    else
                        sla[i].totalProsesUangKecil = double.NaN;

                }
                else
                {
                    DateTime tglSLA = sla[i].tanggal;
                    DateTime tglNextDay = sla[i + 1].tanggal;
                    if (tglNextDay == tglSLA.AddDays(1))
                    {
                    
                        sla[i].totalProsesUangBesar = sla[i].unprocUangBesar + sla[i].inCabangUangBesar + sla[i].inRetailUangBesar - sla[i + 1].unprocUangBesar;
                        sla[i].totalProsesUangKecil = sla[i].unprocUangKecil + sla[i].inCabangUangKecil + sla[i].inRetailUangKecil - sla[i + 1].unprocUangKecil;
                    }
                    else
                    {
                        sla[i].totalProsesUangBesar = Double.NaN;
                        sla[i].totalProsesUangKecil = Double.NaN;
                    }
                }


                if (Double.IsNaN(sla[i].totalProsesUangBesar))
                    sla[i].slaProsesBesar = Double.NaN;
                else
                    sla[i].slaProsesBesar = (Double)sla[i].totalProsesUangBesar / (sla[i].unprocUangBesar / (slaUnprocBesar) + sla[i].inCabangUangBesar / slaCabangBesar + sla[i].inRetailUangBesar / slaRetailBesar);

                if (Double.IsNaN(sla[i].totalProsesUangKecil))
                    sla[i].slaProsesKecil = Double.NaN;
                else
                    sla[i].slaProsesKecil = (Double)sla[i].totalProsesUangKecil / (sla[i].unprocUangKecil / (slaUnprocKecil) + (sla[i].inCabangUangKecil / slaCabangKecil) + sla[i].inRetailUangKecil / slaRetailKecil);


                if (!Double.IsNaN(sla[i].slaProsesBesar) && !Double.IsNaN(sla[i].slaProsesKecil))
                    sla[i].slaGabung = ((Double)sla[i].totalProsesUangBesar + (Double)sla[i].totalProsesUangKecil) / (sla[i].unprocUangBesar / (slaUnprocBesar) + sla[i].inCabangUangBesar / slaCabangBesar + sla[i].inRetailUangBesar / slaRetailBesar + sla[i].unprocUangKecil / (slaUnprocKecil) + (sla[i].inCabangUangKecil / slaCabangKecil) + sla[i].inRetailUangKecil / slaRetailKecil);
                else if (!Double.IsNaN(sla[i].slaProsesBesar))
                    sla[i].slaGabung = sla[i].slaProsesBesar;
                else if (!Double.IsNaN(sla[i].slaProsesKecil))
                    sla[i].slaGabung = sla[i].slaProsesKecil;
                else
                    sla[i].slaGabung = Double.NaN;


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

            sla = sla.Where(x => !Double.IsNaN(x.slaProsesBesar) && !Double.IsNaN(x.slaProsesKecil)).ToList();

            int slaProsesBesarNonNANCount = sla.Where(x => !Double.IsNaN(x.slaProsesBesar)).ToList().Count,
                slaProsesKecilNonNANCount = sla.Where(x => !Double.IsNaN(x.slaProsesKecil)).ToList().Count,
                slaProsesGabungNonNANCount = sla.Where(x => !Double.IsNaN(x.slaGabung)).ToList().Count;

            Console.WriteLine("SLA COUNT: " + sla.Count);

            slaProsesDisplay avg = new slaProsesDisplay()
            {
                tanggal = new DateTime(1, 1, 1),
                inCabangUangBesar = (Int64)Math.Round(sla.Average(x => x.inCabangUangBesar), 0),
                inCabangUangKecil = (Int64)Math.Round(sla.Average(x => x.inCabangUangKecil), 0),
                inRetailUangBesar = (Int64)Math.Round(sla.Average(x => x.inRetailUangBesar), 0),
                inRetailUangKecil = (Int64)Math.Round(sla.Average(x => x.inRetailUangKecil), 0),
                unprocUangBesar = (Int64)Math.Round(sla.Average(x => x.unprocUangBesar), 0),
                unprocUangKecil = (Int64)Math.Round(sla.Average(x => x.unprocUangKecil), 0),
                totalProsesUangBesar = (Int64)Math.Round(sla.Where(x => !Double.IsNaN(x.totalProsesUangBesar)).Average(x => x.totalProsesUangBesar), 0),
                totalProsesUangKecil = (Int64)Math.Round(sla.Where(x => !Double.IsNaN(x.totalProsesUangKecil)).Average(x => x.totalProsesUangKecil), 0),
                slaProsesBesar = slaProsesBesarNonNANCount > 0 ? sla.Where(x => !Double.IsNaN(x.slaProsesBesar)).Average(x => x.slaProsesBesar) : 0,
                slaProsesKecil = slaProsesKecilNonNANCount > 0 ? sla.Where(x => !Double.IsNaN(x.slaProsesKecil)).Average(x => x.slaProsesKecil) : 0,
                slaGabung = slaProsesGabungNonNANCount > 0 ? sla.Where(x => !Double.IsNaN(x.slaGabung)).Average(x => x.slaGabung) : 0
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
                totalProsesUangBesar = (Int64)sla.Where(x => !Double.IsNaN(x.totalProsesUangBesar)).Sum(x => x.totalProsesUangBesar),
                totalProsesUangKecil = (Int64)sla.Where(x => !Double.IsNaN(x.totalProsesUangKecil)).Sum(x => x.totalProsesUangKecil),
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
        public Double hitungTotalProsesBulanDpn(Double totalProses, DateTime tanggal , String namapkt, String bk)
        {

            DateTime tgl = tanggal.AddDays(1);
            Console.WriteLine("Tanggal Bulan Depan: " + tgl);
            var query = (from x in db.StokPosisis.AsEnumerable()
                         where x.namaPkt.Contains(namapkt)
                         && ((DateTime)x.tanggal).Date == tgl.Date
                         select x
                         ).ToList();
            if (query.Any())
            {
                if(bk == "Besar")
                {
                    return totalProses - (Double) query.Where(x => x.denom == "100000" || x.denom == "50000").Select(x => new { unprocessesed = tampilanUang((Int64) x.unprocessed,x.denom, jenisTampilan) }).Select(x=>x.unprocessesed).Sum();
                }
                else
                {
                    return totalProses - (Double)query.Where(x => !(x.denom == "100000" || x.denom == "50000")).Select(x => new { unprocessesed = tampilanUang((Int64)x.unprocessed, x.denom, jenisTampilan) }).Select(x => x.unprocessesed).Sum();
                }
            }
            else
            {
                MessageBox.Show("Data tanggal " + tgl + " PKT " + namapkt + "Tidak ada!");
                return 0;
            }
        }
        public DateTime printTanggal(DateTime tanggal)
        {
            Console.WriteLine(tanggal);
            return tanggal;
        }
        public String printpkt(string pkt)
        {
            Console.WriteLine(pkt);
            return pkt;
        }
        public Double hitungSLA(Int64 unproc, Int64 retail, Int64 cabang, Int64 unprocH1, String bk)
        {
            int slaRetailBesar  = (int)slaRetailBesarNum.Value, 
                slaRetailKecil = (int)slaRetailKecilNum.Value, 
                slaCabangBesar  = (int)slaCabangBesarNum.Value, 
                slaCabangKecil = (int)slaCabangKecilNum.Value,
                slaUnprocBesar = (int)slaCabangBesarNum.Value - 1,
                slaUnprocKecil = (int)slaCabangKecilNum.Value - 1;
            if (slaUnprocBesar <= 1)
                slaUnprocBesar = 1;
            if (slaUnprocKecil <= 1)
                slaUnprocKecil = 1;

            Double sla;
            if (bk == "Besar")
                sla= (Double)(unproc + retail + cabang - unprocH1) / (unproc / (slaUnprocBesar) + retail/slaRetailBesar + cabang/slaCabangBesar);
            else
                sla = (Double)(unproc + retail + cabang - unprocH1) / (unproc / (slaUnprocKecil) + retail/slaRetailKecil + cabang/slaCabangKecil);
            if (sla > 1)
                sla = 1;
            else if (sla < 0)
                sla = 0;
            return sla;
        }
        public Double hitungSLA(Int64 unprocBesar, Int64 retailBesar, Int64 cabangBesar, Int64 unprocH1Besar, Int64 unprocKecil, Int64 retailKecil, Int64 cabangKecil, Int64 unprocH1Kecil)
        {
            int slaRetailBesar = (int)slaRetailBesarNum.Value,
                slaRetailKecil= (int)slaRetailKecilNum.Value,
                slaCabangBesar = (int)slaCabangBesarNum.Value,
                slaCabangKecil= (int)slaCabangKecilNum.Value,
                slaUnprocBesar = (int)slaCabangBesarNum.Value - 1,
                slaUnprocKecil = (int)slaCabangKecilNum.Value - 1;
            Double returndata;
            if (slaUnprocBesar <= 1)
                slaUnprocBesar = 1;
            if (slaUnprocKecil <= 1)
                slaUnprocKecil = 1;
            if ((unprocBesar + retailBesar + cabangBesar) == 0 && (unprocKecil + retailKecil + cabangKecil) == 0)
            {
                return Double.NaN;
            }
            else if((unprocBesar + retailBesar + cabangBesar) == 0)
            {
                returndata = (Double)(unprocKecil + retailKecil + cabangKecil - unprocH1Kecil) / (unprocKecil / (slaUnprocKecil) + retailKecil / slaRetailKecil + cabangKecil / slaCabangKecil);
            }
            else if((unprocKecil + retailKecil+cabangKecil)==0)
            {
                returndata = (Double)(unprocBesar + retailBesar + cabangBesar - unprocH1Besar) / (unprocBesar / (slaUnprocBesar) + retailBesar/slaRetailBesar + cabangBesar/slaCabangBesar);
            }
            else
            {
                returndata = (Double)((unprocBesar + retailBesar + cabangBesar - unprocH1Besar) + (unprocKecil + retailKecil + cabangKecil - unprocH1Kecil)) / ((unprocBesar / (slaUnprocBesar) + retailBesar/slaRetailBesar + cabangBesar/slaCabangBesar)+ (unprocKecil / (slaUnprocKecil) + retailKecil/slaRetailKecil + cabangKecil / slaCabangKecil));
            }
            return returndata < 0 ? 0 : returndata > 1 ? 1 : returndata;
        }
        public Int64 tampilanUang(Double value, String denom, String jenisTampilan)
        {
            if (jenisTampilan.ToLower() == "pcs")
                return (Int64) value / Int32.Parse(denom);
            else
                return (Int64) value;
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
            public Double totalProsesUangBesar { set; get; }
            public Double totalProsesUangKecil { set; get; }
            public Double slaProsesBesar { set; get; }
            public Double slaProsesKecil { set; get; }
            public Double slaGabung { set; get; }
        }
        public class slaProsesDisplayAllVendor
        {
            public DateTime tanggal { set; get; }
            public String namaPkt { set; get; }
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
            dataGridView1.DataSource = null;
            jenisTampilan = tampilanUangComboBox.SelectedItem.ToString();
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
                String csv = "";
                if (comboNamaPkt.SelectedIndex < jumlahPkt)
                    csv = ServiceStack.Text.CsvSerializer.SerializeToString(sla);
                else if(comboNamaPkt.SelectedIndex == jumlahPkt)
                {
                    //int slaBesarNonNANCount = slapav.Where(x => !Double.IsNaN(x.slaProsesBesar)).ToList().Count,
                    //slaKecilNonNANCount = slapav.Where(x => !Double.IsNaN(x.slaProsesKecil)).ToList().Count;
                    //var tempSlaBesar = (from x in slapav
                    //                    group x by x.namaPkt into g
                    //                    select new { namaPkt = g.Key, slaBesar = (slaBesarNonNANCount > 0 ? g.Where(x => !Double.IsNaN(x.slaProsesBesar)).Average(x => x.slaProsesBesar) : Double.NaN) }).ToList();
                    //var tempSlaKecil = (from x in slapav
                    //                    group x by x.namaPkt into g
                    //                    select new { namaPkt = g.Key, slaKecil = (slaKecilNonNANCount > 0 ? g.Where(x => !Double.IsNaN(x.slaProsesKecil)).Average(x => x.slaProsesKecil) : Double.NaN) }).ToList();
                    //var tempSlaGabung = (from x in slapav
                    //                     group x by x.namaPkt into g
                    //                     select new { namaPkt = g.Key, slaGabung = g.Where(x => !Double.IsNaN(x.slaGabung)).Average(x => x.slaGabung) }).ToList();

                    //var qt = (from x in slapav
                    //          group x by x.namaPkt into g
                    //          join y in tempSlaBesar on g.Key equals y.namaPkt
                    //          join z in tempSlaKecil on g.Key equals z.namaPkt
                    //          join zz in tempSlaGabung on g.Key equals zz.namaPkt
                    //          select new
                    //          {
                    //              namaPkt = g.Key,
                    //              UnprocessedBesar = g.Average(x => x.unprocUangBesar),
                    //              UnprocessedKecil = g.Average(x => x.unprocUangKecil),
                    //              InRetailBesar = g.Average(x => x.inRetailUangBesar),
                    //              InRetailKecil = g.Average(x => x.inRetailUangKecil),
                    //              InCabangBesar = g.Average(x => x.inCabangUangBesar),
                    //              InCabangKecil = g.Average(x => x.inCabangUangKecil),
                    //              TotalProcessUangBesar = g.Average(x => x.unprocUangBesar) + g.Average(x => x.inRetailUangBesar) + g.Average(x => x.inCabangUangBesar),
                    //              TotalProcessUangKecil = g.Average(x => x.unprocUangKecil) + g.Average(x => x.inRetailUangKecil) + g.Average(x => x.inCabangUangKecil),
                    //              slaBesar = y.slaBesar,
                    //              slaKecil = z.slaKecil,
                    //              slaGabung = zz.slaGabung
                    //          }).ToList();
                    var qt = resultAllVendor.Select(x=> new {
                        namaPkt = x.namaPkt,
                        UnprocessedBesar = x.unprocUangBesar,
                        UnprocessedKecil = x.unprocUangKecil,
                        InRetailBesar = x.inRetailUangBesar,
                        InRetailKecil = x.inRetailUangKecil,
                        InCabangBesar = x.inCabangUangBesar,
                        InCabangKecil = x.inCabangUangKecil,
                        TotalProcessUangBesar = x.totalProsesUangBesar,
                        TotalProcessUangKecil = x.totalProsesUangKecil,
                        slaBesar = x.slaProsesBesar,
                        slaKecil = x.slaProsesKecil,
                        slaGabung = x.slaGabung
                    }).ToList();
                    if (qt.Count > 2)
                    {
                        qt.RemoveAt(qt.Count - 1);
                        qt.RemoveAt(qt.Count - 1);
                    }
                    csv = ServiceStack.Text.CsvSerializer.SerializeToString(qt);
                }
                else
                {
                    var tempSlaBesar = (from x in slapav
                                        group x by new { x.namaPkt, x.tanggal } into g
                                        select new { namaPkt = g.Key.namaPkt, slaBesar = g.Average(x => x.slaProsesBesar), tanggal = g.Key.tanggal }).ToList();
                    var tempSlaKecil = (from x in slapav
                                        group x by new { x.namaPkt, x.tanggal } into g
                                        select new { namaPkt = g.Key.namaPkt, slaKecil = g.Average(x => x.slaProsesKecil), tanggal = g.Key.tanggal }).ToList();
                    var tempSlaGabung = (from x in slapav
                                         group x by new { x.namaPkt, x.tanggal } into g
                                         select new { namaPkt = g.Key.namaPkt, slaGabung = g.Average(x => x.slaGabung), tanggal = g.Key.tanggal }).ToList();
                    var qt = (from x in slapav
                              group x by new { x.namaPkt, x.tanggal } into g
                              from y in tempSlaBesar.Where(a => a.namaPkt == g.Key.namaPkt && a.tanggal == g.Key.tanggal)
                              from z in tempSlaKecil.Where(a => a.namaPkt == g.Key.namaPkt && a.tanggal == g.Key.tanggal)
                              from zz in tempSlaGabung.Where(a => a.namaPkt == g.Key.namaPkt && a.tanggal == g.Key.tanggal)
                              select new Result()
                              {
                                  namaPkt = g.Key.namaPkt,
                                  tanggal = g.Key.tanggal,
                                  UnprocessedBesar = g.Average(x => x.unprocUangBesar),
                                  UnprocessedKecil = g.Average(x => x.unprocUangKecil),
                                  InCabangBesar = g.Average(x => x.inCabangUangBesar),
                                  InCabangKecil = g.Average(x => x.inCabangUangKecil),
                                  InRetailBesar = g.Average(x => x.inRetailUangBesar),
                                  InRetailKecil = g.Average(x => x.inRetailUangKecil),
                                  TotalProcessUangBesar = g.Average(x => x.unprocUangBesar) + g.Average(x => x.inRetailUangBesar) + g.Average(x => x.inCabangUangBesar),
                                  TotalProcessUangKecil = g.Average(x => x.unprocUangKecil) + g.Average(x => x.inRetailUangKecil) + g.Average(x => x.inCabangUangKecil),
                                  slaBesar = y.slaBesar,
                                  slaKecil = z.slaKecil,
                                  slaGabung = zz.slaGabung
                              }).ToList();
                    int maxDayInMonth = DateTime.DaysInMonth(qt.Max(x => x.tanggal).Year, qt.Max(x => x.tanggal).Month);
                    qt = (from x in qt
                          select new Result()
                          {

                              namaPkt = printpkt(x.namaPkt),
                              tanggal = printTanggal(x.tanggal),
                              UnprocessedBesar = x.UnprocessedBesar,
                              UnprocessedKecil = x.UnprocessedKecil,
                              InCabangBesar = x.InCabangBesar,
                              InCabangKecil = x.InCabangKecil,
                              InRetailBesar = x.InRetailBesar,
                              InRetailKecil = x.InRetailKecil,
                              TotalProcessUangBesar = x.tanggal.Day == maxDayInMonth ? hitungTotalProsesBulanDpn(x.TotalProcessUangBesar, x.tanggal, x.namaPkt, "Besar") : x.TotalProcessUangBesar - qt.Where(y => y.tanggal == x.tanggal.AddDays(1) && y.namaPkt == x.namaPkt).Select(y => y.UnprocessedBesar).FirstOrDefault(),
                              TotalProcessUangKecil = x.tanggal.Day == maxDayInMonth ? hitungTotalProsesBulanDpn(x.TotalProcessUangKecil, x.tanggal, x.namaPkt, "Kecil") : x.TotalProcessUangKecil - qt.Where(y => y.tanggal == x.tanggal.AddDays(1) && y.namaPkt == x.namaPkt).Select(y => y.UnprocessedKecil).FirstOrDefault(),
                              slaBesar = x.slaBesar,
                              slaKecil = x.slaKecil,
                              slaGabung = x.slaGabung > 1 ? 1 : x.slaGabung < 0 ? 0 : x.slaGabung
                          }).ToList();
                    csv = ServiceStack.Text.CsvSerializer.SerializeToString(qt);
                }
                File.WriteAllText(sv.FileName, csv);
            }
        }

        private void slaComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            koordinator = slaComboBox.SelectedItem.ToString();
            loadComboPkt();
            initCombos();
        }
    }
    public class Result
    {
        public String namaPkt {set;get;}
        public DateTime tanggal {set;get;}
        public Double UnprocessedBesar {set;get;} 
        public Double UnprocessedKecil {set;get;} 
        public Double InCabangBesar {set;get;} 
        public Double InCabangKecil {set;get;} 
        public Double InRetailBesar {set;get;} 
        public Double InRetailKecil {set;get;} 
        public Double TotalProcessUangBesar {set;get;} 
        public Double TotalProcessUangKecil {set;get;} 
        public Double slaBesar {set;get;} 
        public Double slaKecil {set;get;} 
        public Double slaGabung {set;get;}
    }
}
public static class MyStringExtensions
{
    public static bool Like(this string toSearch, string toFind)
    {
        return new Regex(@"\A" + new Regex(@"\.|\$|\^|\{|\[|\(|\||\)|\*|\+|\?|\\").Replace(toFind, ch => @"\" + ch).Replace('_', '.').Replace("%", ".*") + @"\z", RegexOptions.Singleline).IsMatch(toSearch);
    }
}
