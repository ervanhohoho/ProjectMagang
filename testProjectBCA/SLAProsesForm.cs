using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
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

        List<String> tahun1 = new List<String>();
        List<String> tahun2 = new List<String>();
        public SLAProsesForm()
        {
            InitializeComponent();


            loadComboPkt();

            comboNamaPkt.SelectedIndex = 0;

            initCombos();

        }
        void initCombos()
        {
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
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "SELECT DISTINCT[Tahun] = YEAR(tanggal) FROM DailyStock ds JOIN Pkt ON ds.kodePkt = Pkt.kodePkt WHERE namaPkt = '"+comboNamaPkt.SelectedValue.ToString()+"' ORDER BY Tahun";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        tahun1.Add(reader[0].ToString());
                    }
                    comboTahun1.DataSource = tahun1;
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
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "SELECT DISTINCT[Tahun] = YEAR(tanggal) FROM DailyStock ds JOIN Pkt ON ds.kodePkt = Pkt.kodePkt WHERE namaPkt = '" + comboNamaPkt.SelectedValue.ToString() + "' ORDER BY Tahun";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        tahun2.Add(reader[0].ToString());
                    }
                    comboTahun2.DataSource = tahun2;
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
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "SELECT DISTINCT [Month] = Month(tanggal) FROM DailyStock ds JOIN Pkt ON ds.kodePkt = Pkt.kodePkt WHERE YEAR(Tanggal) = " + comboTahun1.SelectedValue + " AND namaPkt = '" + comboNamaPkt.SelectedValue.ToString() + "' ORDER BY [Month]";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        bulan1.Add(reader[0].ToString());
                    }
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
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "SELECT DISTINCT[Tahun] = Month(tanggal) FROM DailyStock ds JOIN Pkt ON ds.kodePkt = Pkt.kodePkt WHERE YEAR(Tanggal) = " + comboTahun2.SelectedValue + " AND namaPkt = '"+ comboNamaPkt.SelectedValue.ToString() + "' ORDER BY Tahun";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        bulan2.Add(reader[0].ToString());
                    }
                    comboBulan2.DataSource = bulan2;
                }
            }
        }
        void loadComboPkt()
        {
            List<String>listPkt = (from x in db.Pkts select x.namaPkt).ToList();
            comboNamaPkt.DataSource = listPkt;
        }
        public void reloadData()
        {

            List<slaProsesDisplay> sla = new List<slaProsesDisplay>();
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
         
            List<DailyStock> listData = (from x in db.DailyStocks.AsEnumerable()
                                         join y in db.Pkts on x.kodePkt equals y.kodePkt
                                         where ((DateTime)x.tanggal).Month >= Int32.Parse(comboBulan1.SelectedValue.ToString()) 
                                         &&((DateTime)x.tanggal).Year >= Int32.Parse(comboTahun1.SelectedValue.ToString())
                                         &&((DateTime)x.tanggal).Month <= Int32.Parse(comboBulan2.SelectedValue.ToString())
                                         &&((DateTime)x.tanggal).Year <= Int32.Parse(comboTahun2.SelectedValue.ToString())
                                         && y.namaPkt == comboNamaPkt.SelectedValue.ToString()
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
                            persenProsesUangBesar = 0,
                            persenProsesUangKecil = 0,
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
                    sla[i].persenProsesUangBesar = sla[i].totalProsesUangBesar / (sla[i].unprocUangBesar + sla[i].inCabangUangBesar + sla[i].inRetailUangBesar);
                if (sla[i].unprocUangKecil + sla[i].inCabangUangKecil + sla[i].inRetailUangKecil != 0)
                    sla[i].persenProsesUangKecil = sla[i].totalProsesUangKecil / (sla[i].unprocUangKecil + sla[i].inCabangUangKecil + sla[i].inRetailUangKecil);
            }
            dataGridView1.DataSource = sla;
            for(int a=1;a<dataGridView1.Columns.Count-2; a++)
            {
                dataGridView1.Columns[a].DefaultCellStyle.Format = "N0";
                dataGridView1.Columns[a].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            }
            dataGridView1.Columns[dataGridView1.Columns.Count - 2].DefaultCellStyle.Format = "0.%";
            dataGridView1.Columns[dataGridView1.Columns.Count - 1].DefaultCellStyle.Format = "0.%";
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
            public Double persenProsesUangBesar { set; get; }
            public Double persenProsesUangKecil { set; get; }

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
            reloadData();
        }

        private void comboNamaPkt_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //Console.WriteLine(comboNamaPkt.SelectedIndex);
            initCombos();
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
