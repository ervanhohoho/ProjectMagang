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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA
{
    public partial class DailyStockForm : Form
    {
        Database1Entities db = new Database1Entities();
        List<String> listPkt = new List<string>();
        List<DailyStockDisplay> listTb = new List<DailyStockDisplay>();
        DateTime minTanggal = new DateTime();
        DateTime maxTanggal = new DateTime();

        public DailyStockForm()
        {

            InitializeComponent();
            checkedListBox1.Visible = false;
            checkedListBox2.Visible = false;
            loadComboPkt();
            loadComboTahun();
            loadComboBulan();
            comboJenisTampilan.SelectedIndex = 0;
            comboOption.SelectedIndex = 0;
        }
        private void loadComboTahun()
        {
            minTanggal = (from x in db.DailyStocks select x.tanggal).Min(x => x.Value);
            maxTanggal = (from x in db.DailyStocks select x.tanggal).Max(x => x.Value);

            List<int> listTahun = new List<int>();
            List<int> listBulan = new List<int>();
            List<int> listbulan2 = new List<int>();
            DateTime tempTanggal = minTanggal;

            int bulan = minTanggal.Month;
            int tahun = minTanggal.Year;
            while (tempTanggal.Year <= maxTanggal.Year)
            {
                listTahun.Add(tempTanggal.Year);
                tempTanggal = tempTanggal.AddYears(1);
            }
            comboTahun.DataSource = listTahun;
            comboTahun.SelectedIndex = 0;
        }

        private void loadComboBulan()
        {
            int tahun = (int)comboTahun.SelectedItem;
            int minBulan = (from x in db.DailyStocks.AsEnumerable() where ((DateTime)x.tanggal).Year == tahun select ((DateTime)x.tanggal).Month).Min();
            int maxBulan = (from x in db.DailyStocks.AsEnumerable() where ((DateTime)x.tanggal).Year == tahun select ((DateTime)x.tanggal).Month).Max();

            List<int> bulan = new List<int>();
            List<int> bulan2 = new List<int>();

            for (int a = minBulan; a <= maxBulan; a++)
            {
                bulan.Add(a);
                bulan2.Add(a);
            }
            comboBulan.DataSource = bulan;
            comboBulan2.DataSource = bulan2;
        }
        private void loadComboPkt()
        {
            List<String> tempListPkt = (from x in db.Pkts select x.vendor).Distinct().ToList();
            tempListPkt.Remove("ABACUS DANA PENSIUN");
            listPkt = new List<string>();
            foreach (var temp in tempListPkt)
            {
                if (temp.Contains(" "))
                {
                    listPkt.Add(temp.Substring(0, temp.IndexOf(" ")));
                }
                else
                {
                    listPkt.Add(temp);
                }
            }
            listPkt.Add("All Vendor");
            comboPkt.DataSource = listPkt;
        }
        private void loadTable()
        {
            dataGridView1.DataSource = listTb;
        }

        private void loadAllin()
        {
            String pkt = listPkt[comboPkt.SelectedIndex];
            int tahun = (int)comboTahun.SelectedItem;
            int bulanAwal = (int)comboBulan.SelectedItem;
            int bulanAkhir = (int)comboBulan2.SelectedItem;
            listTb = new List<DailyStockDisplay>();
            String query = "SELECT tanggal"
                           + ", ISNULL(SUM(BN100K),0)"
                           + ", ISNULL(SUM(BN50K),0)"
                           + ", ISNULL(SUM(BN20K),0)"
                           + ", ISNULL(SUM(BN10K),0)"
                           + ", ISNULL(SUM(BN5K),0)"
                           + ", ISNULL(SUM(BN2K),0)"
                           + ", ISNULL(SUM(BN1K),0)"
                           + ", ISNULL(SUM(BN500),0)"
                           + ", ISNULL(SUM(BN200),0)"
                           + ", ISNULL(SUM(BN100),0)"
                           + ", ISNULL(SUM(CN1K),0)"
                           + ", ISNULL(SUM(CN500),0)"
                           + ", ISNULL(SUM(CN200),0)"
                           + ", ISNULL(SUM(CN100),0)"
                           + ", ISNULL(SUM(CN50),0)"
                           + ", ISNULL(SUM(CN25),0)"
                           + " FROM DailyStock d"
                           + " JOIN Pkt p ON p.kodePkt = d.kodePkt"
                           + " AND [in/out] LIKE 'IN'"
                           + " AND YEAR(tanggal) = " + tahun
                           + " AND MONTH(tanggal) BETWEEN " + bulanAwal + " AND " + bulanAkhir
                           + " GROUP BY tanggal";
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    cmd.CommandText = query;
                    sql.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        listTb.Add(new DailyStockDisplay
                        {
                            TANGGAL = (DateTime)reader[0],
                            BN100K = (Int64)reader[1],
                            BN50K = (Int64)reader[2],
                            BN20K = (Int64)reader[3],
                            BN10K = (Int64)reader[4],
                            BN5K = (Int64)reader[5],
                            BN2K = (Int64)reader[6],
                            BN1K = (Int64)reader[7],
                            BN500 = (Int64)reader[8],
                            BN200 = (Int64)reader[9],
                            BN100 = (Int64)reader[10],
                            CN1K = (Int64)reader[11],
                            CN500 = (Int64)reader[12],
                            CN200 = (Int64)reader[13],
                            CN100 = (Int64)reader[14],
                            CN50 = (Int64)reader[15],
                            CN25 = (Int64)reader[16],
                            INOUT = "IN",
                            KETERANGAN = "TOTAL"

                        });
                    }
                }
            }
        }

        private void loadAllout()
        {

            String pkt = listPkt[comboPkt.SelectedIndex];
            int tahun = (int)comboTahun.SelectedItem;
            int bulanAwal = (int)comboBulan.SelectedItem;
            int bulanAkhir = (int)comboBulan2.SelectedItem;
            listTb = new List<DailyStockDisplay>();
            String query = "SELECT tanggal"
                           + ", ISNULL(SUM(BN100K),0)"
                           + ", ISNULL(SUM(BN50K),0)"
                           + ", ISNULL(SUM(BN20K),0)"
                           + ", ISNULL(SUM(BN10K),0)"
                           + ", ISNULL(SUM(BN5K),0)"
                           + ", ISNULL(SUM(BN2K),0)"
                           + ", ISNULL(SUM(BN1K),0)"
                           + ", ISNULL(SUM(BN500),0)"
                           + ", ISNULL(SUM(BN200),0)"
                           + ", ISNULL(SUM(BN100),0)"
                           + ", ISNULL(SUM(CN1K),0)"
                           + ", ISNULL(SUM(CN500),0)"
                           + ", ISNULL(SUM(CN200),0)"
                           + ", ISNULL(SUM(CN100),0)"
                           + ", ISNULL(SUM(CN50),0)"
                           + ", ISNULL(SUM(CN25),0)"
                           + " FROM DailyStock d"
                           + " JOIN Pkt p ON p.kodePkt = d.kodePkt"
                           + " AND [in/out] LIKE 'OUT'"
                           + " AND YEAR(tanggal) = " + tahun
                           + " AND MONTH(tanggal) BETWEEN " + bulanAwal + " AND " + bulanAkhir
                           + " GROUP BY tanggal";

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    cmd.CommandText = query;
                    sql.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        listTb.Add(new DailyStockDisplay
                        {
                            TANGGAL = (DateTime)reader[0],
                            BN100K = (Int64)reader[1],
                            BN50K = (Int64)reader[2],
                            BN20K = (Int64)reader[3],
                            BN10K = (Int64)reader[4],
                            BN5K = (Int64)reader[5],
                            BN2K = (Int64)reader[6],
                            BN1K = (Int64)reader[7],
                            BN500 = (Int64)reader[8],
                            BN200 = (Int64)reader[9],
                            BN100 = (Int64)reader[10],
                            CN1K = (Int64)reader[11],
                            CN500 = (Int64)reader[12],
                            CN200 = (Int64)reader[13],
                            CN100 = (Int64)reader[14],
                            CN50 = (Int64)reader[15],
                            CN25 = (Int64)reader[16],
                            INOUT = "OUT",
                            KETERANGAN = "TOTAL"

                        });
                    }
                }
            }
        }

        private String queryBuilderForCustomALL(int bulanAwal, int bulanAkhir, int tahun, String jenisTransaksi)
        {
            String query = "SELECT tanggal"
                           + ", ISNULL(SUM(BN100K),0)"
                           + ", ISNULL(SUM(BN50K),0)"
                           + ", ISNULL(SUM(BN20K),0)"
                           + ", ISNULL(SUM(BN10K),0)"
                           + ", ISNULL(SUM(BN5K),0)"
                           + ", ISNULL(SUM(BN2K),0)"
                           + ", ISNULL(SUM(BN1K),0)"
                           + ", ISNULL(SUM(BN500),0)"
                           + ", ISNULL(SUM(BN200),0)"
                           + ", ISNULL(SUM(BN100),0)"
                           + ", ISNULL(SUM(CN1K),0)"
                           + ", ISNULL(SUM(CN500),0)"
                           + ", ISNULL(SUM(CN200),0)"
                           + ", ISNULL(SUM(CN100),0)"
                           + ", ISNULL(SUM(CN50),0)"
                           + ", ISNULL(SUM(CN25),0)"
                           + " FROM DailyStock d"
                           + " JOIN Pkt p ON p.kodePkt = d.kodePkt"
                           + " AND jenisTransaksi like '%" + jenisTransaksi + "'"
                           + " AND YEAR(tanggal) = " + tahun
                           + " AND MONTH(tanggal) BETWEEN " + bulanAwal + " AND " + bulanAkhir
                           + " GROUP BY tanggal";
            return query;
        }

        private void loadOut()
        {
            String pkt = listPkt[comboPkt.SelectedIndex];
            int tahun = (int)comboTahun.SelectedItem;
            int bulanAwal = (int)comboBulan.SelectedItem;
            int bulanAkhir = (int)comboBulan2.SelectedItem;
            listTb = new List<DailyStockDisplay>();
            String query = "SELECT tanggal"
                           + ", ISNULL(SUM(BN100K),0)"
                           + ", ISNULL(SUM(BN50K),0)"
                           + ", ISNULL(SUM(BN20K),0)"
                           + ", ISNULL(SUM(BN10K),0)"
                           + ", ISNULL(SUM(BN5K),0)"
                           + ", ISNULL(SUM(BN2K),0)"
                           + ", ISNULL(SUM(BN1K),0)"
                           + ", ISNULL(SUM(BN500),0)"
                           + ", ISNULL(SUM(BN200),0)"
                           + ", ISNULL(SUM(BN100),0)"
                           + ", ISNULL(SUM(CN1K),0)"
                           + ", ISNULL(SUM(CN500),0)"
                           + ", ISNULL(SUM(CN200),0)"
                           + ", ISNULL(SUM(CN100),0)"
                           + ", ISNULL(SUM(CN50),0)"
                           + ", ISNULL(SUM(CN25),0)"
                           + " FROM DailyStock d"
                           + " JOIN Pkt p ON p.kodePkt = d.kodePkt"
                           + " WHERE vendor = '" + listPkt[comboPkt.SelectedIndex] + "'"
                           + " AND [in/out] LIKE 'OUT'"
                           + " AND YEAR(tanggal) = " + tahun
                           + " AND MONTH(tanggal) BETWEEN " + bulanAwal + " AND " + bulanAkhir
                           + " GROUP BY tanggal";

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    cmd.CommandText = query;
                    sql.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        listTb.Add(new DailyStockDisplay
                        {
                            TANGGAL = (DateTime)reader[0],
                            BN100K = (Int64)reader[1],
                            BN50K = (Int64)reader[2],
                            BN20K = (Int64)reader[3],
                            BN10K = (Int64)reader[4],
                            BN5K = (Int64)reader[5],
                            BN2K = (Int64)reader[6],
                            BN1K = (Int64)reader[7],
                            BN500 = (Int64)reader[8],
                            BN200 = (Int64)reader[9],
                            BN100 = (Int64)reader[10],
                            CN1K = (Int64)reader[11],
                            CN500 = (Int64)reader[12],
                            CN200 = (Int64)reader[13],
                            CN100 = (Int64)reader[14],
                            CN50 = (Int64)reader[15],
                            CN25 = (Int64)reader[16],
                            INOUT = "OUT",
                            KETERANGAN = "TOTAL"

                        });
                    }
                }
            }
        }
        private void loadInflow()
        {
            String pkt = listPkt[comboPkt.SelectedIndex];
            int tahun = (int)comboTahun.SelectedItem;
            int bulanAwal = (int)comboBulan.SelectedItem;
            int bulanAkhir = (int)comboBulan2.SelectedItem;
            listTb = new List<DailyStockDisplay>();
            String query = "SELECT tanggal"
                           + ", ISNULL(SUM(BN100K),0)"
                           + ", ISNULL(SUM(BN50K),0)"
                           + ", ISNULL(SUM(BN20K),0)"
                           + ", ISNULL(SUM(BN10K),0)"
                           + ", ISNULL(SUM(BN5K),0)"
                           + ", ISNULL(SUM(BN2K),0)"
                           + ", ISNULL(SUM(BN1K),0)"
                           + ", ISNULL(SUM(BN500),0)"
                           + ", ISNULL(SUM(BN200),0)"
                           + ", ISNULL(SUM(BN100),0)"
                           + ", ISNULL(SUM(CN1K),0)"
                           + ", ISNULL(SUM(CN500),0)"
                           + ", ISNULL(SUM(CN200),0)"
                           + ", ISNULL(SUM(CN100),0)"
                           + ", ISNULL(SUM(CN50),0)"
                           + ", ISNULL(SUM(CN25),0)"
                           + " FROM DailyStock d"
                           + " JOIN Pkt p ON p.kodePkt = d.kodePkt"
                           + " WHERE vendor = '" + listPkt[comboPkt.SelectedIndex] + "'"
                           + " AND [in/out] LIKE 'IN'"
                           + " AND YEAR(tanggal) = " + tahun
                           + " AND MONTH(tanggal) BETWEEN " + bulanAwal + " AND " + bulanAkhir
                           + " GROUP BY tanggal";
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    cmd.CommandText = query;
                    sql.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        listTb.Add(new DailyStockDisplay
                        {
                            TANGGAL = (DateTime)reader[0],
                            BN100K = (Int64)reader[1],
                            BN50K = (Int64)reader[2],
                            BN20K = (Int64)reader[3],
                            BN10K = (Int64)reader[4],
                            BN5K = (Int64)reader[5],
                            BN2K = (Int64)reader[6],
                            BN1K = (Int64)reader[7],
                            BN500 = (Int64)reader[8],
                            BN200 = (Int64)reader[9],
                            BN100 = (Int64)reader[10],
                            CN1K = (Int64)reader[11],
                            CN500 = (Int64)reader[12],
                            CN200 = (Int64)reader[13],
                            CN100 = (Int64)reader[14],
                            CN50 = (Int64)reader[15],
                            CN25 = (Int64)reader[16],
                            INOUT = "IN",
                            KETERANGAN = "TOTAL"

                        });
                    }
                }
            }
        }
        private void loadCustomALL()
        {
            listTb = new List<DailyStockDisplay>();
            int tahun = (int)comboTahun.SelectedItem;
            int bulanAwal = (int)comboBulan.SelectedItem;
            int bulanAkhir = (int)comboBulan2.SelectedItem;

            foreach (var item in checkedListBox1.CheckedItems)
            {

                String query = "";
                if (item.ToString() == "Coll. Cabang Process")
                {
                    query = queryBuilderForCustomALL(bulanAwal, bulanAkhir, tahun, "Collection Cabang - Full - Process");

                }
                else if (item.ToString() == "Coll. Cabang Pass Trough")
                {
                    query = queryBuilderForCustomALL(bulanAwal, bulanAkhir, tahun, "Collection Cabang - Passtrough");

                }
                else if (item.ToString() == "Coll. Retail")
                {
                    query = queryBuilderForCustomALL(bulanAwal, bulanAkhir, tahun, "Collection Retail");

                }
                else if (item.ToString() == "Coll. Curex")
                {
                    query = queryBuilderForCustomALL(bulanAwal, bulanAkhir, tahun, "Collection Lainnya - Tukaran");

                }
                else if (item.ToString() == "Coll. ATM Reguler")
                {
                    query = queryBuilderForCustomALL(bulanAwal, bulanAkhir, tahun, "Collection Lainnya - ATM Reguler");

                }
                else if (item.ToString() == "Coll. ATM Adhoc")
                {
                    query = queryBuilderForCustomALL(bulanAwal, bulanAkhir, tahun, "Collection Lainnya - ATM Adhoc");

                }
                else if (item.ToString() == "Coll. BI")
                {
                    query = queryBuilderForCustomALL(bulanAwal, bulanAkhir, tahun, "Collection Lainnya - BI");

                }
                else if (item.ToString() == "Coll. Interbank")
                {
                    query = queryBuilderForCustomALL(bulanAwal, bulanAkhir, tahun, "Collection Lainnya - Interbank");

                }
                else if (item.ToString() == "Coll. Antar CPC")
                {
                    query = queryBuilderForCustomALL(bulanAwal, bulanAkhir, tahun, "Collection Lainnya - Antar CPC");

                }


                using (SqlConnection sql = new SqlConnection(Variables.connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = sql;
                        cmd.CommandText = query;
                        sql.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            listTb.Add(new DailyStockDisplay
                            {
                                TANGGAL = (DateTime)reader[0],
                                BN100K = (Int64)reader[1],
                                BN50K = (Int64)reader[2],
                                BN20K = (Int64)reader[3],
                                BN10K = (Int64)reader[4],
                                BN5K = (Int64)reader[5],
                                BN2K = (Int64)reader[6],
                                BN1K = (Int64)reader[7],
                                BN500 = (Int64)reader[8],
                                BN200 = (Int64)reader[9],
                                BN100 = (Int64)reader[10],
                                CN1K = (Int64)reader[11],
                                CN500 = (Int64)reader[12],
                                CN200 = (Int64)reader[13],
                                CN100 = (Int64)reader[14],
                                CN50 = (Int64)reader[15],
                                CN25 = (Int64)reader[16],
                                INOUT = "IN",
                                KETERANGAN = item.ToString()
                            });
                        }
                    }
                }

            }

            foreach (var item in checkedListBox2.CheckedItems)
            {

                String query = "";

                if (item.ToString() == "Del. Cabang Reguler")
                {

                    query = queryBuilderForCustomALL(bulanAwal, bulanAkhir, tahun, "Delivery Cabang - Reguler");

                }
                else if (item.ToString() == "Del. Cabang Adhoc")
                {
                    query = queryBuilderForCustomALL(bulanAwal, bulanAkhir, tahun, "Delivery Cabang - Adhoc");

                }
                else if (item.ToString() == "Del. Retail")
                {
                    query = queryBuilderForCustomALL(bulanAwal, bulanAkhir, tahun, "Delivery Retail");

                }
                else if (item.ToString() == "Del. Curex")
                {
                    query = queryBuilderForCustomALL(bulanAwal, bulanAkhir, tahun, "Delivery Lainnya - Tukaran");

                }
                else if (item.ToString() == "Del. ATM Reguler")
                {
                    query = queryBuilderForCustomALL(bulanAwal, bulanAkhir, tahun, "Delivery Lainnya - ATM Reguler");

                }
                else if (item.ToString() == "Del. ATM Adhoc")
                {
                    query = queryBuilderForCustomALL(bulanAwal, bulanAkhir, tahun, "Delivery Lainnya - ATM Adhoc");

                }
                else if (item.ToString() == "Del. BI")
                {
                    query = queryBuilderForCustomALL(bulanAwal, bulanAkhir, tahun, "Delivery Lainnya - BI");

                }
                else if (item.ToString() == "Del. Interbank")
                {
                    query = queryBuilderForCustomALL(bulanAwal, bulanAkhir, tahun, "Delivery Lainnya - Interbank");

                }
                else if (item.ToString() == "Del. Antar CPC")
                {
                    query = queryBuilderForCustomALL(bulanAwal, bulanAkhir, tahun, "Delivery Lainnya - Antar CPC");

                }


                using (SqlConnection sql = new SqlConnection(Variables.connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {

                        cmd.Connection = sql;
                        cmd.CommandText = query;
                        sql.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            listTb.Add(new DailyStockDisplay
                            {
                                TANGGAL = (DateTime)reader[0],
                                BN100K = (Int64)reader[1],
                                BN50K = (Int64)reader[2],
                                BN20K = (Int64)reader[3],
                                BN10K = (Int64)reader[4],
                                BN5K = (Int64)reader[5],
                                BN2K = (Int64)reader[6],
                                BN1K = (Int64)reader[7],
                                BN500 = (Int64)reader[8],
                                BN200 = (Int64)reader[9],
                                BN100 = (Int64)reader[10],
                                CN1K = (Int64)reader[11],
                                CN500 = (Int64)reader[12],
                                CN200 = (Int64)reader[13],
                                CN100 = (Int64)reader[14],
                                CN50 = (Int64)reader[15],
                                CN25 = (Int64)reader[16],
                                INOUT = "OUT",
                                KETERANGAN = item.ToString()
                            });
                        }
                    }
                }

            }
        }
        private void loadCustom()
        {
            listTb = new List<DailyStockDisplay>();
            int tahun = (int)comboTahun.SelectedItem;
            int bulanAwal = (int)comboBulan.SelectedItem;
            int bulanAkhir = (int)comboBulan2.SelectedItem;

            foreach (var item in checkedListBox1.CheckedItems)
            {

                String query = "";
                if (item.ToString() == "Coll. Cabang Process")
                {
                    query = queryBuilderForCustom(bulanAwal, bulanAkhir, tahun, "Collection Cabang - Full - Process");

                }
                else if (item.ToString() == "Coll. Cabang Pass Trough")
                {
                    query = queryBuilderForCustom(bulanAwal, bulanAkhir, tahun, "Collection Cabang - Passtrough");

                }
                else if (item.ToString() == "Coll. Retail")
                {
                    query = queryBuilderForCustom(bulanAwal, bulanAkhir, tahun, "Collection Retail");

                }
                else if (item.ToString() == "Coll. Curex")
                {
                    query = queryBuilderForCustom(bulanAwal, bulanAkhir, tahun, "Collection Lainnya - Tukaran");

                }
                else if (item.ToString() == "Coll. ATM Reguler")
                {
                    query = queryBuilderForCustom(bulanAwal, bulanAkhir, tahun, "Collection Lainnya - ATM Reguler");

                }
                else if (item.ToString() == "Coll. ATM Adhoc")
                {
                    query = queryBuilderForCustom(bulanAwal, bulanAkhir, tahun, "Collection Lainnya - ATM Adhoc");

                }
                else if (item.ToString() == "Coll. BI")
                {
                    query = queryBuilderForCustom(bulanAwal, bulanAkhir, tahun, "Collection Lainnya - BI");

                }
                else if (item.ToString() == "Coll. Interbank")
                {
                    query = queryBuilderForCustom(bulanAwal, bulanAkhir, tahun, "Collection Lainnya - Interbank");

                }
                else if (item.ToString() == "Coll. Antar CPC")
                {
                    query = queryBuilderForCustom(bulanAwal, bulanAkhir, tahun, "Collection Lainnya - Antar CPC");

                }


                using (SqlConnection sql = new SqlConnection(Variables.connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = sql;
                        cmd.CommandText = query;
                        sql.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            listTb.Add(new DailyStockDisplay
                            {
                                TANGGAL = (DateTime)reader[0],
                                BN100K = (Int64)reader[1],
                                BN50K = (Int64)reader[2],
                                BN20K = (Int64)reader[3],
                                BN10K = (Int64)reader[4],
                                BN5K = (Int64)reader[5],
                                BN2K = (Int64)reader[6],
                                BN1K = (Int64)reader[7],
                                BN500 = (Int64)reader[8],
                                BN200 = (Int64)reader[9],
                                BN100 = (Int64)reader[10],
                                CN1K = (Int64)reader[11],
                                CN500 = (Int64)reader[12],
                                CN200 = (Int64)reader[13],
                                CN100 = (Int64)reader[14],
                                CN50 = (Int64)reader[15],
                                CN25 = (Int64)reader[16],
                                INOUT = "IN",
                                KETERANGAN = item.ToString()
                            });
                        }
                    }
                }

            }

            foreach (var item in checkedListBox2.CheckedItems)
            {

                String query = "";

                if (item.ToString() == "Del. Cabang Reguler")
                {

                    query = queryBuilderForCustom(bulanAwal, bulanAkhir, tahun, "Delivery Cabang - Reguler");

                }
                else if (item.ToString() == "Del. Cabang Adhoc")
                {
                    query = queryBuilderForCustom(bulanAwal, bulanAkhir, tahun, "Delivery Cabang - Adhoc");

                }
                else if (item.ToString() == "Del. Retail")
                {
                    query = queryBuilderForCustom(bulanAwal, bulanAkhir, tahun, "Delivery Retail");

                }
                else if (item.ToString() == "Del. Curex")
                {
                    query = queryBuilderForCustom(bulanAwal, bulanAkhir, tahun, "Delivery Lainnya - Tukaran");

                }
                else if (item.ToString() == "Del. ATM Reguler")
                {
                    query = queryBuilderForCustom(bulanAwal, bulanAkhir, tahun, "Delivery Lainnya - ATM Reguler");

                }
                else if (item.ToString() == "Del. ATM Adhoc")
                {
                    query = queryBuilderForCustom(bulanAwal, bulanAkhir, tahun, "Delivery Lainnya - ATM Adhoc");

                }
                else if (item.ToString() == "Del. BI")
                {
                    query = queryBuilderForCustom(bulanAwal, bulanAkhir, tahun, "Delivery Lainnya - BI");

                }
                else if (item.ToString() == "Del. Interbank")
                {
                    query = queryBuilderForCustom(bulanAwal, bulanAkhir, tahun, "Delivery Lainnya - Interbank");

                }
                else if (item.ToString() == "Del. Antar CPC")
                {
                    query = queryBuilderForCustom(bulanAwal, bulanAkhir, tahun, "Delivery Lainnya - Antar CPC");

                }


                using (SqlConnection sql = new SqlConnection(Variables.connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {

                        cmd.Connection = sql;
                        cmd.CommandText = query;
                        sql.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            listTb.Add(new DailyStockDisplay
                            {
                                TANGGAL = (DateTime)reader[0],
                                BN100K = (Int64)reader[1],
                                BN50K = (Int64)reader[2],
                                BN20K = (Int64)reader[3],
                                BN10K = (Int64)reader[4],
                                BN5K = (Int64)reader[5],
                                BN2K = (Int64)reader[6],
                                BN1K = (Int64)reader[7],
                                BN500 = (Int64)reader[8],
                                BN200 = (Int64)reader[9],
                                BN100 = (Int64)reader[10],
                                CN1K = (Int64)reader[11],
                                CN500 = (Int64)reader[12],
                                CN200 = (Int64)reader[13],
                                CN100 = (Int64)reader[14],
                                CN50 = (Int64)reader[15],
                                CN25 = (Int64)reader[16],
                                INOUT = "OUT",
                                KETERANGAN = item.ToString()
                            });
                        }
                    }
                }

            }
        }
        private String queryBuilderForCustom(int bulanAwal, int bulanAkhir, int tahun, String jenisTransaksi)
        {
            String query = "SELECT tanggal"
                           + ", ISNULL(SUM(BN100K),0)"
                           + ", ISNULL(SUM(BN50K),0)"
                           + ", ISNULL(SUM(BN20K),0)"
                           + ", ISNULL(SUM(BN10K),0)"
                           + ", ISNULL(SUM(BN5K),0)"
                           + ", ISNULL(SUM(BN2K),0)"
                           + ", ISNULL(SUM(BN1K),0)"
                           + ", ISNULL(SUM(BN500),0)"
                           + ", ISNULL(SUM(BN200),0)"
                           + ", ISNULL(SUM(BN100),0)"
                           + ", ISNULL(SUM(CN1K),0)"
                           + ", ISNULL(SUM(CN500),0)"
                           + ", ISNULL(SUM(CN200),0)"
                           + ", ISNULL(SUM(CN100),0)"
                           + ", ISNULL(SUM(CN50),0)"
                           + ", ISNULL(SUM(CN25),0)"
                           + " FROM DailyStock d"
                           + " JOIN Pkt p ON p.kodePkt = d.kodePkt"
                           + " WHERE vendor = '" + listPkt[comboPkt.SelectedIndex] + "'"
                           + " AND jenisTransaksi like '%" + jenisTransaksi + "'"
                           + " AND YEAR(tanggal) = " + tahun
                           + " AND MONTH(tanggal) BETWEEN " + bulanAwal + " AND " + bulanAkhir
                           + " GROUP BY tanggal";
            return query;
        }
        private void comboJenisTampilan_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboJenisTampilan.SelectedIndex == 2)
            {
                checkedListBox1.Visible = true;
                checkedListBox2.Visible = true;

            }
            else
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)

                {

                    checkedListBox1.SetItemChecked(i, false);

                }
                for (int i = 0; i < checkedListBox2.Items.Count; i++)

                {

                    checkedListBox2.SetItemChecked(i, false);

                }
                checkedListBox1.Visible = false;
                checkedListBox2.Visible = false;
            }

        }

        private void buttonShow_Click(object sender, EventArgs e)
        {
            if (comboJenisTampilan.SelectedIndex == 0)
            {
                if (comboPkt.SelectedIndex == 5)
                {
                    loadAllin();
                }
                else
                {
                    loadInflow();
                }

            }

            if (comboJenisTampilan.SelectedIndex == 1)

            {
                if (comboPkt.SelectedIndex == 5)
                {
                    loadAllout();
                }
                else
                {
                    loadOut();
                }
            }
            if (comboJenisTampilan.SelectedIndex == 2)
            {
                if (comboPkt.SelectedIndex == 5)
                {
                    loadCustomALL();
                }
                else
                {
                    loadCustom();
                }

            }



            if (comboOption.SelectedIndex == 1)
            {
                foreach (var temp in listTb)
                {
                    temp.BN100K /= 1000000;
                    temp.BN50K /= 50000;
                    temp.BN20K /= 20000;
                    temp.BN10K /= 10000;
                    temp.BN5K /= 5000;
                    temp.BN2K /= 2000;
                    temp.BN1K /= 1000;
                    temp.BN500 /= 500;
                    temp.BN200 /= 200;
                    temp.BN100 /= 100;
                    temp.CN1K /= 1000;
                    temp.CN500 /= 500;
                    temp.CN200 /= 200;
                    temp.CN100 /= 100;
                    temp.CN50 /= 50;
                    temp.CN25 /= 25;
                }

            }
            loadTable();
            if (comboOption.SelectedIndex == 0)
            {
                for (int a = 3; a < dataGridView1.ColumnCount; a++)
                {
                    dataGridView1.Columns[a].DefaultCellStyle.Format = "C";
                    dataGridView1.Columns[a].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("ID-id");
                }
            }
            else
            {
                for (int a = 3; a < dataGridView1.ColumnCount; a++)
                {
                    dataGridView1.Columns[a].DefaultCellStyle.Format = "N0";
                }
            }

        }

        private void DailyStockForm_Load(object sender, EventArgs e)
        {


        }

        private void comboTahun_SelectionChangeCommitted(object sender, EventArgs e)
        {
            loadComboBulan();
        }

        void SaveDataGridViewToCSV(string filename)
        {
            // Choose whether to write header. Use EnableWithoutHeaderText instead to omit header.
            dataGridView1.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            // Select all the cells
            dataGridView1.SelectAll();
            // Copy selected cells to DataObject
            DataObject dataObject = dataGridView1.GetClipboardContent();
            // Get the text of the DataObject, and serialize it to a file
            File.WriteAllText(filename, dataObject.GetText(TextDataFormat.CommaSeparatedValue));
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog sv = new SaveFileDialog();
            sv.Filter = Variables.csvFilter;
            if (sv.ShowDialog() == DialogResult.OK)
            {
                SaveDataGridViewToCSV(sv.FileName);
            }
        }
    }
}
class DailyStockDisplay
{
    public DateTime TANGGAL { set; get; }
    public String INOUT { set; get; }
    public String KETERANGAN { set; get; }
    public Int64 BN100K { get; set; }
    public Int64 BN50K { get; set; }
    public Int64 BN20K { get; set; }
    public Int64 BN10K { get; set; }
    public Int64 BN5K { get; set; }
    public Int64 BN2K { get; set; }
    public Int64 BN1K { get; set; }
    public Int64 BN500 { get; set; }
    public Int64 BN200 { get; set; }
    public Int64 BN100 { get; set; }
    public Int64 CN1K { get; set; }
    public Int64 CN500 { get; set; }
    public Int64 CN200 { get; set; }
    public Int64 CN100 { get; set; }
    public Int64 CN50 { get; set; }
    public Int64 CN25 { get; set; }

}