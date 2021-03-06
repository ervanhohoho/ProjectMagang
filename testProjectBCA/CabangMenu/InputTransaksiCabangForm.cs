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
    public partial class InputTransaksiCabangForm : Form
    {
        Database1Entities db = new Database1Entities();
        String kodePkt = "";
        int SHEET_COL_CABANG = -1,
            SHEET_COL_RETAIL = -1,
            SHEET_COL_LAINNYA = -1,
            SHEET_DEL_CABANG = -1,
            SHEET_DEL_RETAIL = -1,
            SHEET_DEL_LAINNYA = -1,
            SHEET_DAILYSTOCK = -1,
            SHEET_BA = -1;
        const String SHEET_NAME_COL_CABANG = "collection cabang",
            SHEET_NAME_COL_RETAIL = "collection retail",
            SHEET_NAME_COL_LAINNYA = "collection lainnya (in)",
            SHEET_NAME_DEL_CABANG = "delivery cabang",
            SHEET_NAME_DEL_RETAIL = "delivery retail",
            SHEET_NAME_DEL_LAINNYA = "delivery lainnya (out)",
            SHEET_NAME_DAILYSTOCK = "dailystock",
            SHEET_NAME_BA = "rincian ba";
        String currentFileName = "";
        DataTable collectionLainnya;
        DataTable collectionCabang;
        DataTable collectionRetail;
        DataTable deliveryCabang;
        DataTable deliveryRetail;
        DataTable deliveryLainnya;
        DataTable dailystock;
        DataTable ba;
        List<Pkt> listPkt;
        public InputTransaksiCabangForm()
        {
            InitializeComponent();
        }

        private void SelectFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Multiselect = true;
            of.Filter = Variables.excelFilter;
            listPkt = (from x in db.Pkts
                       select x).ToList();
            if(of.ShowDialog() == DialogResult.OK)
            {
                loadForm.ShowSplashScreen();
                String [] filenames = of.FileNames;


               
                foreach (String filename in filenames)
                {
                    currentFileName = filename;
                    //List<Pkt> listPkt = (from x in db.Pkts select x).ToList(); 

                    collectionCabang = new DataTable();
                    collectionRetail = new DataTable();
                    collectionLainnya = new DataTable();
                    deliveryCabang = new DataTable();
                    deliveryRetail = new DataTable();
                    deliveryLainnya = new DataTable();
                    dailystock = new DataTable();
                    ba = new DataTable();


                    SHEET_COL_CABANG = -1;
                    SHEET_COL_RETAIL = -1;
                    SHEET_COL_LAINNYA = -1;
                    SHEET_DEL_CABANG = -1;
                    SHEET_DEL_RETAIL = -1;
                    SHEET_DEL_LAINNYA = -1;
                    SHEET_DAILYSTOCK = -1;
                    SHEET_BA = -1;

                    //Console.WriteLine(kodePkt);
                    DataSet ds = Util.openExcel(filename);

                    if (ds.Tables.Count == 0)
                        break;
                    for (int a = 0; a < ds.Tables.Count; a++)
                    {
                        switch (ds.Tables[a].TableName.ToLower())
                        {
                            case SHEET_NAME_COL_CABANG:
                                SHEET_COL_CABANG = a;
                                break;
                            case SHEET_NAME_COL_RETAIL:
                                SHEET_COL_RETAIL = a;
                                break;
                            case SHEET_NAME_COL_LAINNYA:
                                SHEET_COL_LAINNYA = a;
                                break;
                            case SHEET_NAME_DEL_CABANG:
                                SHEET_DEL_CABANG = a;
                                break;
                            case SHEET_NAME_DEL_RETAIL:
                                SHEET_DEL_RETAIL = a;
                                break;
                            case SHEET_NAME_DEL_LAINNYA:
                                SHEET_DEL_LAINNYA = a;
                                break;
                            case SHEET_NAME_DAILYSTOCK:
                                SHEET_DAILYSTOCK = a;
                                break;
                            case SHEET_NAME_BA:
                                SHEET_BA = a;
                                break;
                            default:
                                break;
                        }
                    }
                    //DateTime date = (DateTime)ds.Tables[0].Rows[2][0];
                    //deleteFromDB(date, kodePkt);

                    bool flagCollectionCabang = false,
                        flagCollectionRetail = false,
                        flagCollectionLainnya = false,
                        flagDeliveryCabang = false,
                        flagDeliveryRetail = false,
                        flagDeliveryLainnya = false,
                        flagDailystock = false,
                        flagBA = false;


                    if (SHEET_COL_CABANG != -1)
                    {
                        collectionCabang = ds.Tables[SHEET_COL_CABANG];
                        flagCollectionCabang = true;
                    }
                    if (SHEET_COL_RETAIL != -1)
                    {
                        collectionRetail = ds.Tables[SHEET_COL_RETAIL];
                        flagCollectionRetail = true;
                    }
                    if (SHEET_COL_LAINNYA != -1)
                    {
                        collectionLainnya = ds.Tables[SHEET_COL_LAINNYA];
                        flagCollectionLainnya = true;
                    }
                    if (SHEET_DEL_CABANG != -1)
                    {
                        deliveryCabang = ds.Tables[SHEET_DEL_CABANG];
                        flagDeliveryCabang = true;
                    }
                    if (SHEET_DEL_RETAIL != -1)
                    {
                        deliveryRetail = ds.Tables[SHEET_DEL_RETAIL];
                        flagDeliveryRetail = true;
                    }
                    if (SHEET_DEL_LAINNYA != -1)
                    {
                        deliveryLainnya = ds.Tables[SHEET_DEL_LAINNYA];
                        flagDeliveryLainnya = true;
                    }
                    if (SHEET_DAILYSTOCK != -1)
                    {
                        dailystock = ds.Tables[SHEET_DAILYSTOCK];
                        flagDailystock = true;
                    }
                    if (SHEET_BA != -1)
                    {
                        ba = ds.Tables[SHEET_BA];
                        flagBA = true;
                    }
                    Console.WriteLine(filename);
                    if (flagCollectionCabang)
                    {
                        //try
                        //{
                            readCollectionCabang();
                        //}
                        //catch (Exception ee)
                        //{
                        //    MessageBox.Show("File\t: " + filename + "\nSheet\t: " + collectionCabang.TableName + "\nError. Data sheet ini tidak dimasukkan. Data sheet lain tetap diproses");
                        //}
                    }
                    else
                    {
                        MessageBox.Show("File\t: " + filename + " Tidak memiliki sheet " + SHEET_NAME_COL_CABANG);
                    }
                    if (flagCollectionRetail)
                    {
                        try
                        {
                            readCollectionRetail();
                        }
                        catch
                        {
                            MessageBox.Show("File\t: " + filename + "\nSheet\t: " + collectionRetail.TableName + "\nError. Data sheet ini tidak dimasukkan. Data sheet lain tetap diproses");
                        }
                    }
                    else
                    {
                        MessageBox.Show("File\t: " + filename + " Tidak memiliki sheet " + SHEET_NAME_COL_RETAIL);
                    }
                    if (flagCollectionLainnya)
                    {
                        try
                        {
                            readCollectionLainnya();
                        }
                        catch
                        {
                            MessageBox.Show("File\t: " + filename + "\nSheet\t: " + collectionLainnya.TableName + "\nError. Data sheet ini tidak dimasukkan. Data sheet lain tetap diproses");
                        }
                    }
                    else
                    {
                        MessageBox.Show("File\t: " + filename + " Tidak memiliki sheet " + SHEET_NAME_COL_LAINNYA);
                    }
                    if (flagDeliveryCabang)
                    {
                        try
                        {
                            readDeliveryCabang();
                        }
                        catch
                        {
                            MessageBox.Show("File\t: " + filename + "\nSheet\t: " + deliveryCabang.TableName + "\nError. Data sheet ini tidak dimasukkan. Data sheet lain tetap diproses");
                        }
                    }
                    else
                    {
                        MessageBox.Show("File\t: " + filename + " Tidak memiliki sheet " + SHEET_NAME_DEL_CABANG);
                    }
                    if (flagDeliveryRetail)
                    {
                        try
                        {
                            readDeliveryRetail();
                        }
                        catch
                        {
                            MessageBox.Show("File\t: " + filename + "\nSheet\t: " + deliveryRetail.TableName + "\nError. Data sheet ini tidak dimasukkan. Data sheet lain tetap diproses");
                        }
                    }
                    else
                    {
                        MessageBox.Show("File\t: " + filename + " Tidak memiliki sheet " + SHEET_NAME_DEL_RETAIL);
                    }
                    if (flagDeliveryLainnya)
                    {
                        try
                        {
                            readDeliveryLainnya();
                        }
                        catch
                        {
                            MessageBox.Show("File\t: " + filename + "\nSheet\t: " + deliveryLainnya.TableName + "\nError. Data sheet ini tidak dimasukkan. Data sheet lain tetap diproses");
                        }
                    }
                    else
                    {
                        MessageBox.Show("File\t: " + filename + " Tidak memiliki sheet " + SHEET_NAME_DEL_LAINNYA);
                    }
                    if (flagDailystock)
                    {
                        try
                        {
                            readDailyStock();
                        }
                        catch
                        {
                            MessageBox.Show("File\t: " + filename + "\nSheet\t: " + dailystock.TableName + "\nError. Data sheet ini tidak dimasukkan. Data sheet lain tetap diproses");
                        }
                    }
                    else
                    {
                        MessageBox.Show("File\t: " + filename + " Tidak memiliki sheet " + SHEET_NAME_DAILYSTOCK);
                    }
                    if (flagBA)
                    {
                        try
                        {
                            readBA();
                        }
                        catch
                        {
                            MessageBox.Show("File\t: " + filename + "\nSheet\t: " + ba.TableName + "\nError. Data sheet ini tidak dimasukkan. Data sheet lain tetap diproses");
                        }
                    }
                    else
                    {
                        MessageBox.Show("File\t: " + filename + " Tidak memiliki sheet " + SHEET_NAME_BA);

                    }
                }
                hilangkanNull();
                loadForm.CloseForm();
                MessageBox.Show("Done!");
            }
        }
        private void deleteFromDB(DateTime date, string kodePkt)
        {
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "DELETE FROM DailyStock WHERE kodePkt = '" + kodePkt + "' AND tanggal = '" + date.ToShortDateString()+"'";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void hilangkanNull()
        {
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();

                    cmd.CommandText = "UPDATE DailyStock SET BN100K = 0 WHERE BN100K is NULL";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE DailyStock SET BN50K = 0 WHERE BN50K is NULL";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE DailyStock SET BN20K = 0 WHERE BN20K is NULL";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE DailyStock SET BN10K = 0 WHERE BN10K is NULL";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE DailyStock SET BN5K = 0 WHERE BN5K is NULL";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE DailyStock SET BN2K = 0 WHERE BN2K is NULL";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE DailyStock SET BN1K = 0 WHERE BN1K is NULL";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE DailyStock SET BN500 = 0 WHERE BN500 is NULL";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE DailyStock SET BN200 = 0 WHERE BN200 is NULL";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE DailyStock SET BN100 = 0 WHERE BN100 is NULL";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "UPDATE DailyStock SET CN1K = 0 WHERE CN1K is NULL";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE DailyStock SET CN500 = 0 WHERE CN500 is NULL";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE DailyStock SET CN200 = 0 WHERE CN200 is NULL";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE DailyStock SET CN100 = 0 WHERE CN100 is NULL";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE DailyStock SET CN50 = 0 WHERE CN50 is NULL";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE DailyStock SET CN25 = 0 WHERE CN25 is NULL";
                    cmd.ExecuteNonQuery();
                }
            }
        }
        private bool readCollectionCabang()
        {
            DataTable dt = collectionCabang;
            //Ambil Nama PKT
            String namaPkt = dt.Rows[0][1].ToString().Replace("PT ","");
            kodePkt = (from x in listPkt
                       where x.namaPkt.Contains(namaPkt)
                       select x.kodePktCabang).FirstOrDefault();

            DataTable dt2 = dt.Clone();
            dt2.Columns[2].DataType = typeof(String);
            foreach (DataRow row in dt.Rows)
            {
                dt2.ImportRow(row);
            }
            DataRow[] rows = dt2.Select("not LEN(Column2) = 4 OR Column1 IN('Vendor', 'Tanggal%')");
            foreach (var row in rows)
            {
                dt2.Rows.Remove(row);
            }
            dt2.Rows.Remove(dt2.Rows[0]); dt2.Rows.Remove(dt2.Rows[dt2.Rows.Count - 1]);
            //Console.WriteLine(dt2.Rows.Count);
            //Console.WriteLine(dt2.Rows[dt2.Rows.Count - 1][2].ToString());
            dt2.Columns.Add("kodePkt", typeof(String));
            dt2.Columns["kodePkt"].DefaultValue = kodePkt;
            for (int a = 0; a < dt2.Rows.Count; a++)
            {
                dt2.Rows[a]["kodePkt"] = kodePkt;
            }

            dt2.Columns.Add("jenisTransaksi", typeof(String));
            for (int a = 0; a < dt2.Rows.Count; a++)
            {
                dt2.Rows[a]["jenisTransaksi"] = "Collection Cabang - " + dt2.Rows[a][1].ToString();
            }

            dt2.Columns.Add("inout", typeof(String));
            for (int a = 0; a < dt2.Rows.Count; a++)
            {
                dt2.Rows[a]["inout"] = "IN";
            }
            //Console.WriteLine(dt.Rows[dt.Rows.Count - 1][dt.Columns.Count - 1].ToString());
            //using (SqlBulkCopy sbc = new SqlBulkCopy(Variables.connectionString))
            //{
            //    sbc.DestinationTableName = "dbo.DailyStock";
            //    sbc.ColumnMappings.Add(2, 5); //Kode Cabang
            //    sbc.ColumnMappings.Add(3, 6); //Nama Cabang
            //    sbc.ColumnMappings.Add(4, 7); //Keterangan Cabang
            //    sbc.ColumnMappings.Add(dt.Columns.Count - 3, 2); //kodePkt
            //    sbc.ColumnMappings.Add(dt.Columns.Count - 2, 4); //Jenis Transaksi
            //    sbc.ColumnMappings.Add(dt.Columns.Count - 1, 3); //IN-OUT
            //    sbc.ColumnMappings.Add(0, 1); //Tanggal
            //    for (int a = 0; a < 15; a++)
            //        sbc.ColumnMappings.Add(6 + a, 8 + a); //BN + Coin
            //    sbc.WriteToServer(dt);
            //    sbc.Close();
            //}

            //Masukin ke list untuk update/insert
            List<DailyStock> listCollectionCabangDariExcel = new List<DailyStock>();
            foreach(DataRow row in dt2.Rows)
            {
                if (String.IsNullOrWhiteSpace(row[2].ToString())||String.IsNullOrEmpty(row['X'-'A'].ToString()) || row['X'-'A'].ToString() == "0")
                    continue;

                Int64 BN100K = 0,
                    BN50K = 0,
                    BN20K = 0,
                    BN10K = 0,
                    BN5K = 0,
                    BN2K = 0,
                    BN1K = 0,
                    BN500 = 0,
                    BN100 = 0,
                    C1K = 0,
                    C500 = 0,
                    C200 = 0,
                    C100 = 0,
                    C50 = 0,
                    C25 = 0,
                    buf;
                String SBN100K = row[6].ToString(),
                       SBN50K = row[7].ToString(),
                       SBN20K = row[8].ToString(),
                       SBN10K = row[9].ToString(),
                       SBN5K = row[10].ToString(),
                       SBN2K = row[11].ToString(),
                       SBN1K = row[12].ToString(),
                       SBN500 = row[13].ToString(),
                       SBN100 = row[14].ToString(),
                       SC1K = row[15].ToString(),
                       SC500 = row[16].ToString(),
                       SC200 = row[17].ToString(),
                       SC100 = row[18].ToString(),
                       SC50 = row[19].ToString(),
                       SC25 = row[20].ToString();
                if (!String.IsNullOrEmpty(SBN100K))
                    if (Int64.TryParse(SBN100K, out buf))
                        BN100K = buf;
                if (!String.IsNullOrEmpty(SBN50K))
                    if (Int64.TryParse(SBN50K, out buf))
                        BN50K = buf;
                if (!String.IsNullOrEmpty(SBN20K))
                    if (Int64.TryParse(SBN20K, out buf))
                        BN20K = buf;
                if (!String.IsNullOrEmpty(SBN10K))
                    if (Int64.TryParse(SBN10K, out buf))
                        BN10K = buf;
                if (!String.IsNullOrEmpty(SBN5K))
                    if (Int64.TryParse(SBN5K, out buf))
                        BN5K = buf;
                if (!String.IsNullOrEmpty(SBN2K))
                    if (Int64.TryParse(SBN2K, out buf))
                        BN2K = buf;
                if (!String.IsNullOrEmpty(SBN1K))
                    if (Int64.TryParse(SBN1K, out buf))
                        BN1K = buf;
                if (!String.IsNullOrEmpty(SBN500))
                    if (Int64.TryParse(SBN500, out buf))
                        BN500 = buf;
                if (!String.IsNullOrEmpty(SBN100))
                    if (Int64.TryParse(SBN100, out buf))
                        BN100 = buf;
                if (!String.IsNullOrEmpty(SC1K))
                    if (Int64.TryParse(SC1K, out buf))
                        C1K = buf;
                if (!String.IsNullOrEmpty(SC500))
                    if (Int64.TryParse(SC500, out buf))
                        C500 = buf;
                if (!String.IsNullOrEmpty(SC200))
                    if (Int64.TryParse(SC200, out buf))
                        C200 = buf;
                if (!String.IsNullOrEmpty(SC100))
                    if (Int64.TryParse(SC100, out buf))
                        C100 = buf;
                if (!String.IsNullOrEmpty(SC50))
                    if (Int64.TryParse(SC50, out buf))
                        C50 = buf;
                if (!String.IsNullOrEmpty(SC25))
                    if (Int64.TryParse(SC25, out buf))
                        C25 = buf;

                DateTime dateBuf;
                if(!DateTime.TryParse(row[0].ToString(), out dateBuf))
                {
                    MessageBox.Show("Format tanggal file " + currentFileName + " Sheet " + SHEET_NAME_COL_CABANG + " Format tanggal salah!");
                    return false;
                }

                listCollectionCabangDariExcel.Add(new DailyStock() {
                    kode = row[2].ToString(),
                    nama = row[3].ToString(),
                    keterangan = row[4].ToString(),
                    kodePkt = row["kodePkt"].ToString(),
                    in_out = row["inout"].ToString(),
                    jenisTransaksi = row["jenisTransaksi"].ToString(),
                    tanggal = DateTime.Parse(row[0].ToString()),
                    BN100K = BN100K,
                    BN50K = BN50K,
                    BN20K = BN20K,
                    BN10K = BN10K,
                    BN5K = BN5K,
                    BN2K = BN2K,
                    BN1K = BN1K,
                    CN1K = C1K,
                    CN500 = C500,
                    CN200 = C200,
                    CN100 = C100,
                    CN50 = C50,
                    CN25 = C25,
                    BN500 = BN500,
                    BN100 = BN100,
                    //udah gadipake
                    BN200 = 0,
                });
            }
            listCollectionCabangDariExcel = listCollectionCabangDariExcel.GroupBy(x => new { x.tanggal, x.kodePkt, x.kode, x.nama, x.jenisTransaksi }).Select(x => new DailyStock()
            {
                tanggal = x.Key.tanggal,
                kode = x.Key.kode,
                kodePkt = x.Key.kodePkt,
                nama = x.Key.nama,
                jenisTransaksi = x.Key.jenisTransaksi,
                keterangan = x.Select(y => y.keterangan).First(),
                in_out = x.Select(y => y.in_out).First(),
                BN100K = x.Sum(y => y.BN100K),
                BN50K = x.Sum(y => y.BN50K),
                BN20K = x.Sum(y => y.BN20K),
                BN10K = x.Sum(y => y.BN10K),
                BN5K = x.Sum(y => y.BN5K),
                BN2K = x.Sum(y => y.BN2K),
                BN1K = x.Sum(y => y.BN1K),
                BN500 = x.Sum(y => y.BN500),
                BN200 = x.Sum(y => y.BN200),
                BN100 = x.Sum(y => y.BN100),
                CN1K = x.Sum(y => y.CN1K),
                CN500 = x.Sum(y => y.CN500),
                CN200 = x.Sum(y => y.CN200),
                CN100 = x.Sum(y => y.CN100),
                CN50 = x.Sum(y => y.CN50),
                CN25 = x.Sum(y => y.CN25),
            }).ToList();
            List<DailyStock> listDataDb = db.DailyStocks.Where(x => x.kodePkt == kodePkt).ToList();
            List<DailyStock> toInput = new List<DailyStock>();
            foreach(DailyStock checkUpdate in listCollectionCabangDariExcel)
            {
                var toUpdate = listDataDb.Where(x => x.kode == checkUpdate.kode && x.tanggal == checkUpdate.tanggal && x.jenisTransaksi == checkUpdate.jenisTransaksi).FirstOrDefault();
                if (toUpdate != null)
                {
                    toUpdate.BN100K = checkUpdate.BN100K;
                    toUpdate.BN50K = checkUpdate.BN50K;
                    toUpdate.BN20K = checkUpdate.BN20K;
                    toUpdate.BN10K = checkUpdate.BN10K;
                    toUpdate.BN5K = checkUpdate.BN5K;
                    toUpdate.BN2K = checkUpdate.BN2K;
                    toUpdate.BN1K = checkUpdate.BN1K;
                    toUpdate.BN500 = checkUpdate.BN500;
                    toUpdate.BN100 = checkUpdate.BN100;
                    toUpdate.CN1K = checkUpdate.CN1K;
                    toUpdate.CN500 = checkUpdate.CN500;
                    toUpdate.CN200 = checkUpdate.CN200;
                    toUpdate.CN100 = checkUpdate.CN100;
                    toUpdate.CN50 = checkUpdate.CN50;
                    toUpdate.CN25 = checkUpdate.CN25;
                }
                else
                {
                    toInput.Add(checkUpdate);
                }
            }
            db.DailyStocks.AddRange(toInput);

            Console.WriteLine("Print collection cabang\n=========================");
            foreach (var temp in listCollectionCabangDariExcel)
                Console.WriteLine(temp.tanggal + " " + temp.kodePkt + " " + temp.jenisTransaksi + " " + temp.BN100K);
            db.SaveChanges();
            return true;
        }
        private bool readCollectionRetail()
        {
            DataTable dt = collectionRetail;
            dt.Rows.RemoveAt(0);
            dt.Rows.Remove(dt.Rows[0]); dt.Rows.Remove(dt.Rows[dt.Rows.Count - 1]);

            DataRow[] rows = dt.Select("Column2 is NULL");
            foreach (var row in rows)
            {
                dt.Rows.Remove(row);
            }
            
            
            //Console.WriteLine(dt.Rows.Count);
            //Console.WriteLine(dt.Rows[4][4]);
            //Console.WriteLine(dt.Rows[dt.Rows.Count - 1][2].ToString());
            dt.Columns.Add("kodePkt", typeof(String));
            dt.Columns["kodePkt"].DefaultValue = kodePkt;
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["kodePkt"] = kodePkt;
            }

            dt.Columns.Add("jenisTransaksi", typeof(String));
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["jenisTransaksi"] = "Collection Retail";
            }

            dt.Columns.Add("inout", typeof(String));
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["inout"] = "IN";
            }

            //Console.WriteLine(dt.Rows[dt.Rows.Count - 1][dt.Columns.Count - 1].ToString());
            //using (SqlBulkCopy sbc = new SqlBulkCopy(Variables.connectionString))
            //{
            //    sbc.DestinationTableName = "dbo.DailyStock";
            //    //Kode Nasabah
            //    sbc.ColumnMappings.Add(1, 5);
            //    //Nama Nasabah
            //    sbc.ColumnMappings.Add(2, 6);
            //    //Kode Pkt
            //    sbc.ColumnMappings.Add(dt.Columns.Count - 3, 2);
            //    //Jenis Transaksi
            //    sbc.ColumnMappings.Add(dt.Columns.Count - 2, 4);
            //    //IN - OUT
            //    sbc.ColumnMappings.Add(dt.Columns.Count - 1, 3);
            //    //Tanggal
            //    sbc.ColumnMappings.Add(0, 1);
            //    //Uang
            //    for (int a = 0; a < 15; a++)
            //        sbc.ColumnMappings.Add(4 + a, 8 + a);
            //    sbc.WriteToServer(dt);
            //    sbc.Close();
            //}

            //Masukin ke list untuk update/insert
            List<DailyStock> listCollectionRetailDariExcel = new List<DailyStock>();
            foreach (DataRow row in dt.Rows)
            {
                if (String.IsNullOrWhiteSpace(row[1].ToString()) || String.IsNullOrEmpty(row['V'-'A'].ToString()) || row['V'-'A'].ToString() == "0")
                    continue;

                Int64 BN100K = 0,
                    BN50K = 0,
                    BN20K = 0,
                    BN10K = 0,
                    BN5K = 0,
                    BN2K = 0,
                    BN1K = 0,
                    BN500 = 0,
                    BN100 = 0,
                    C1K = 0,
                    C500 = 0,
                    C200 = 0,
                    C100 = 0,
                    C50 = 0,
                    C25 = 0,
                    buf;
                String SBN100K = row[4].ToString(),
                       SBN50K = row[5].ToString(),
                       SBN20K = row[6].ToString(),
                       SBN10K = row[7].ToString(),
                       SBN5K = row[8].ToString(),
                       SBN2K = row[9].ToString(),
                       SBN1K = row[10].ToString(),
                       SBN500 = row[11].ToString(),
                       SBN100 = row[12].ToString(),
                       SC1K = row[13].ToString(),
                       SC500 = row[14].ToString(),
                       SC200 = row[15].ToString(),
                       SC100 = row[16].ToString(),
                       SC50 = row[17].ToString(),
                       SC25 = row[18].ToString();
                if (!String.IsNullOrEmpty(SBN100K))
                    if (Int64.TryParse(SBN100K, out buf))
                        BN100K = buf;
                if (!String.IsNullOrEmpty(SBN50K))
                    if (Int64.TryParse(SBN50K, out buf))
                        BN50K = buf;
                if (!String.IsNullOrEmpty(SBN20K))
                    if (Int64.TryParse(SBN20K, out buf))
                        BN20K = buf;
                if (!String.IsNullOrEmpty(SBN10K))
                    if (Int64.TryParse(SBN10K, out buf))
                        BN10K = buf;
                if (!String.IsNullOrEmpty(SBN5K))
                    if (Int64.TryParse(SBN5K, out buf))
                        BN5K = buf;
                if (!String.IsNullOrEmpty(SBN2K))
                    if (Int64.TryParse(SBN2K, out buf))
                        BN2K = buf;
                if (!String.IsNullOrEmpty(SBN1K))
                    if (Int64.TryParse(SBN1K, out buf))
                        BN1K = buf;
                if (!String.IsNullOrEmpty(SBN500))
                    if (Int64.TryParse(SBN500, out buf))
                        BN500 = buf;
                if (!String.IsNullOrEmpty(SBN100))
                    if (Int64.TryParse(SBN100, out buf))
                        BN100 = buf;
                if (!String.IsNullOrEmpty(SC1K))
                    if (Int64.TryParse(SC1K, out buf))
                        C1K = buf;
                if (!String.IsNullOrEmpty(SC500))
                    if (Int64.TryParse(SC500, out buf))
                        C500 = buf;
                if (!String.IsNullOrEmpty(SC200))
                    if (Int64.TryParse(SC200, out buf))
                        C200 = buf;
                if (!String.IsNullOrEmpty(SC100))
                    if (Int64.TryParse(SC100, out buf))
                        C100 = buf;
                if (!String.IsNullOrEmpty(SC50))
                    if (Int64.TryParse(SC50, out buf))
                        C50 = buf;
                if (!String.IsNullOrEmpty(SC25))
                    if (Int64.TryParse(SC25, out buf))
                        C25 = buf;
                DateTime dateBuf;
                if (!DateTime.TryParse(row[0].ToString(), out dateBuf))
                {
                    MessageBox.Show("Format tanggal file " + currentFileName + " Sheet " + SHEET_NAME_COL_RETAIL + " Format tanggal salah!");
                    return false;
                }
                listCollectionRetailDariExcel.Add(new DailyStock()
                {
                    kode = row[1].ToString(),
                    nama = row[2].ToString(),
                    keterangan = "",
                    kodePkt = row[dt.Columns.Count - 3].ToString(),
                    in_out = row[dt.Columns.Count - 1].ToString(),
                    jenisTransaksi = row[dt.Columns.Count - 2].ToString(),
                    tanggal = DateTime.Parse(row[0].ToString()),
                    BN100K = BN100K,
                    BN50K = BN50K,
                    BN20K = BN20K,
                    BN10K = BN10K,
                    BN5K = BN5K,
                    BN2K = BN2K,
                    BN1K = BN1K,
                    CN1K = C1K,
                    CN500 = C500,
                    CN200 = C200,
                    CN100 = C100,
                    CN50 = C50,
                    CN25 = C25,
                    BN500 = BN500,
                    BN100 = BN100,
                    //udah gadipake
                    BN200 = 0,
                });
            }
            listCollectionRetailDariExcel = listCollectionRetailDariExcel.GroupBy(x => new { x.tanggal, x.kodePkt, x.kode, x.nama, x.jenisTransaksi }).Select(x => new DailyStock()
            {
                tanggal = x.Key.tanggal,
                kode = x.Key.kode,
                kodePkt = x.Key.kodePkt,
                nama = x.Key.nama,
                jenisTransaksi = x.Key.jenisTransaksi,
                keterangan = x.Select(y => y.keterangan).First(),
                in_out = x.Select(y => y.in_out).First(),
                BN100K = x.Sum(y => y.BN100K),
                BN50K = x.Sum(y => y.BN50K),
                BN20K = x.Sum(y => y.BN20K),
                BN10K = x.Sum(y => y.BN10K),
                BN5K = x.Sum(y => y.BN5K),
                BN2K = x.Sum(y => y.BN2K),
                BN1K = x.Sum(y => y.BN1K),
                BN500 = x.Sum(y => y.BN500),
                BN200 = x.Sum(y => y.BN200),
                BN100 = x.Sum(y => y.BN100),
                CN1K = x.Sum(y => y.CN1K),
                CN500 = x.Sum(y => y.CN500),
                CN200 = x.Sum(y => y.CN200),
                CN100 = x.Sum(y => y.CN100),
                CN50 = x.Sum(y => y.CN50),
                CN25 = x.Sum(y => y.CN25),
            }).ToList();
            List<DailyStock> listDataDb = db.DailyStocks.Where(x => x.kodePkt == kodePkt).ToList();
            List<DailyStock> toInput = new List<DailyStock>();
            foreach (DailyStock checkUpdate in listCollectionRetailDariExcel)
            {
                var toUpdate = listDataDb.Where(x => x.kode == checkUpdate.kode && x.tanggal == checkUpdate.tanggal && x.jenisTransaksi == checkUpdate.jenisTransaksi).FirstOrDefault();
                if (toUpdate != null)
                {
                    toUpdate.BN100K = checkUpdate.BN100K;
                    toUpdate.BN50K = checkUpdate.BN50K;
                    toUpdate.BN20K = checkUpdate.BN20K;
                    toUpdate.BN10K = checkUpdate.BN10K;
                    toUpdate.BN5K = checkUpdate.BN5K;
                    toUpdate.BN2K = checkUpdate.BN2K;
                    toUpdate.BN1K = checkUpdate.BN1K;
                    toUpdate.BN500 = checkUpdate.BN500;
                    toUpdate.BN100 = checkUpdate.BN100;
                    toUpdate.CN1K = checkUpdate.CN1K;
                    toUpdate.CN500 = checkUpdate.CN500;
                    toUpdate.CN200 = checkUpdate.CN200;
                    toUpdate.CN100 = checkUpdate.CN100;
                    toUpdate.CN50 = checkUpdate.CN50;
                    toUpdate.CN25 = checkUpdate.CN25;
                }
                else
                {
                    toInput.Add(checkUpdate);
                }
            }
            db.DailyStocks.AddRange(toInput);
            db.SaveChanges();
            return true;
        }
        private bool readCollectionLainnya()
        {
            DataTable dt = collectionLainnya;
            dt.Rows.RemoveAt(0);
            dt.Rows.Remove(dt.Rows[0]); dt.Rows.Remove(dt.Rows[dt.Rows.Count - 1]);

            DataRow[] rows = dt.Select("Column2 is NULL");
            foreach (var row in rows)
            {
                dt.Rows.Remove(row);
            }


            //Console.WriteLine(dt.Rows.Count);
            //Console.WriteLine(dt.Rows[0][2]);
            //Console.WriteLine(dt.Rows[dt.Rows.Count - 1][2].ToString());
            dt.Columns.Add("kodePkt", typeof(String));
            dt.Columns["kodePkt"].DefaultValue = kodePkt;
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["kodePkt"] = kodePkt;
            }

            dt.Columns.Add("jenisTransaksi", typeof(String));
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["jenisTransaksi"] = "Collection Lainnya - "+dt.Rows[a][1].ToString();
            }

            dt.Columns.Add("inout", typeof(String));
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["inout"] = "IN";
            }

            //Console.WriteLine(dt.Rows[dt.Rows.Count - 1][dt.Columns.Count - 1].ToString());
            //using (SqlBulkCopy sbc = new SqlBulkCopy(Variables.connectionString))
            //{
            //    sbc.DestinationTableName = "dbo.DailyStock";
            //    //Kode Pkt
            //    sbc.ColumnMappings.Add(dt.Columns.Count - 3, 2);
            //    //Jenis Transaksi
            //    sbc.ColumnMappings.Add(dt.Columns.Count - 2, 4);
            //    //IN-OUT
            //    sbc.ColumnMappings.Add(dt.Columns.Count - 1, 3);
            //    //Tanggal
            //    sbc.ColumnMappings.Add(0, 1);
            //    //Keterangan Sumber Dana
            //    sbc.ColumnMappings.Add(2, 7);
            //    //Uang
            //    for (int a = 0; a < 15; a++)
            //        sbc.ColumnMappings.Add(4 + a, 8 + a);
            //    sbc.WriteToServer(dt);
            //    sbc.Close();
            //}

            //Masukin ke list untuk update/insert
            List<DailyStock> listCollectionLainnyaDariExcel = new List<DailyStock>();
            foreach (DataRow row in dt.Rows)
            {
                if (String.IsNullOrWhiteSpace(row[1].ToString()) || String.IsNullOrEmpty(row['V' - 'A'].ToString()) || row['V' - 'A'].ToString() == "0")
                    continue;

                Int64 BN100K = 0,
                    BN50K = 0,
                    BN20K = 0,
                    BN10K = 0,
                    BN5K = 0,
                    BN2K = 0,
                    BN1K = 0,
                    BN500 = 0,
                    BN100 = 0,
                    C1K = 0,
                    C500 = 0,
                    C200 = 0,
                    C100 = 0,
                    C50 = 0,
                    C25 = 0,
                    buf;
                String SBN100K = row[4].ToString(),
                       SBN50K = row[5].ToString(),
                       SBN20K = row[6].ToString(),
                       SBN10K = row[7].ToString(),
                       SBN5K = row[8].ToString(),
                       SBN2K = row[9].ToString(),
                       SBN1K = row[10].ToString(),
                       SBN500 = row[11].ToString(),
                       SBN100 = row[12].ToString(),
                       SC1K = row[13].ToString(),
                       SC500 = row[14].ToString(),
                       SC200 = row[15].ToString(),
                       SC100 = row[16].ToString(),
                       SC50 = row[17].ToString(),
                       SC25 = row[18].ToString();
                if (!String.IsNullOrEmpty(SBN100K))
                    if (Int64.TryParse(SBN100K, out buf))
                        BN100K = buf;
                if (!String.IsNullOrEmpty(SBN50K))
                    if (Int64.TryParse(SBN50K, out buf))
                        BN50K = buf;
                if (!String.IsNullOrEmpty(SBN20K))
                    if (Int64.TryParse(SBN20K, out buf))
                        BN20K = buf;
                if (!String.IsNullOrEmpty(SBN10K))
                    if (Int64.TryParse(SBN10K, out buf))
                        BN10K = buf;
                if (!String.IsNullOrEmpty(SBN5K))
                    if (Int64.TryParse(SBN5K, out buf))
                        BN5K = buf;
                if (!String.IsNullOrEmpty(SBN2K))
                    if (Int64.TryParse(SBN2K, out buf))
                        BN2K = buf;
                if (!String.IsNullOrEmpty(SBN1K))
                    if (Int64.TryParse(SBN1K, out buf))
                        BN1K = buf;
                if (!String.IsNullOrEmpty(SBN500))
                    if (Int64.TryParse(SBN500, out buf))
                        BN500 = buf;
                if (!String.IsNullOrEmpty(SBN100))
                    if (Int64.TryParse(SBN100, out buf))
                        BN100 = buf;
                if (!String.IsNullOrEmpty(SC1K))
                    if (Int64.TryParse(SC1K, out buf))
                        C1K = buf;
                if (!String.IsNullOrEmpty(SC500))
                    if (Int64.TryParse(SC500, out buf))
                        C500 = buf;
                if (!String.IsNullOrEmpty(SC200))
                    if (Int64.TryParse(SC200, out buf))
                        C200 = buf;
                if (!String.IsNullOrEmpty(SC100))
                    if (Int64.TryParse(SC100, out buf))
                        C100 = buf;
                if (!String.IsNullOrEmpty(SC50))
                    if (Int64.TryParse(SC50, out buf))
                        C50 = buf;
                if (!String.IsNullOrEmpty(SC25))
                    if (Int64.TryParse(SC25, out buf))
                        C25 = buf;
                DateTime dateBuf;
                if (!DateTime.TryParse(row[0].ToString(), out dateBuf))
                {
                    MessageBox.Show("Format tanggal file " + currentFileName + " Sheet " + SHEET_NAME_COL_LAINNYA + " Format tanggal salah!");
                    return false;
                }
                listCollectionLainnyaDariExcel.Add(new DailyStock()
                {
                    kode = "",
                    nama = "",
                    keterangan = row[2].ToString(),
                    kodePkt = row[dt.Columns.Count - 3].ToString(),
                    in_out = row[dt.Columns.Count - 1].ToString(),
                    jenisTransaksi = row[dt.Columns.Count - 2].ToString(),
                    tanggal = DateTime.Parse(row[0].ToString()),
                    BN100K = BN100K,
                    BN50K = BN50K,
                    BN20K = BN20K,
                    BN10K = BN10K,
                    BN5K = BN5K,
                    BN2K = BN2K,
                    BN1K = BN1K,
                    CN1K = C1K,
                    CN500 = C500,
                    CN200 = C200,
                    CN100 = C100,
                    CN50 = C50,
                    CN25 = C25,
                    BN500 = BN500,
                    BN100 = BN100,
                    //udah gadipake
                    BN200 = 0,
                });
            }
            listCollectionLainnyaDariExcel = listCollectionLainnyaDariExcel.GroupBy(x => new { x.tanggal, x.kodePkt, x.kode, x.nama, x.jenisTransaksi }).Select(x => new DailyStock()
            {
                tanggal = x.Key.tanggal,
                kode = x.Key.kode,
                kodePkt = x.Key.kodePkt,
                nama = x.Key.nama,
                jenisTransaksi = x.Key.jenisTransaksi,
                keterangan = x.Select(y => y.keterangan).First(),
                in_out = x.Select(y => y.in_out).First(),
                BN100K = x.Sum(y => y.BN100K),
                BN50K = x.Sum(y => y.BN50K),
                BN20K = x.Sum(y => y.BN20K),
                BN10K = x.Sum(y => y.BN10K),
                BN5K = x.Sum(y => y.BN5K),
                BN2K = x.Sum(y => y.BN2K),
                BN1K = x.Sum(y => y.BN1K),
                BN500 = x.Sum(y => y.BN500),
                BN200 = x.Sum(y => y.BN200),
                BN100 = x.Sum(y => y.BN100),
                CN1K = x.Sum(y => y.CN1K),
                CN500 = x.Sum(y => y.CN500),
                CN200 = x.Sum(y => y.CN200),
                CN100 = x.Sum(y => y.CN100),
                CN50 = x.Sum(y => y.CN50),
                CN25 = x.Sum(y => y.CN25),
            }).ToList();
            List<DailyStock> listDataDb = db.DailyStocks.Where(x => x.kodePkt == kodePkt).ToList();
            List<DailyStock> toInput = new List<DailyStock>();
            foreach (DailyStock checkUpdate in listCollectionLainnyaDariExcel)
            {
                var toUpdate = listDataDb.Where(x => x.kode == checkUpdate.kode && x.tanggal == checkUpdate.tanggal && x.jenisTransaksi == checkUpdate.jenisTransaksi).FirstOrDefault();
                if (toUpdate != null)
                {
                    toUpdate.BN100K = checkUpdate.BN100K;
                    toUpdate.BN50K = checkUpdate.BN50K;
                    toUpdate.BN20K = checkUpdate.BN20K;
                    toUpdate.BN10K = checkUpdate.BN10K;
                    toUpdate.BN5K = checkUpdate.BN5K;
                    toUpdate.BN2K = checkUpdate.BN2K;
                    toUpdate.BN1K = checkUpdate.BN1K;
                    toUpdate.BN500 = checkUpdate.BN500;
                    toUpdate.BN100 = checkUpdate.BN100;
                    toUpdate.CN1K = checkUpdate.CN1K;
                    toUpdate.CN500 = checkUpdate.CN500;
                    toUpdate.CN200 = checkUpdate.CN200;
                    toUpdate.CN100 = checkUpdate.CN100;
                    toUpdate.CN50 = checkUpdate.CN50;
                    toUpdate.CN25 = checkUpdate.CN25;
                }
                else
                {
                    toInput.Add(checkUpdate);
                }
            }
            db.DailyStocks.AddRange(toInput);
            db.SaveChanges();
            return true;
        }
        private bool readDeliveryCabang()
        {
            DataTable dt = deliveryCabang;
            dt.Rows.RemoveAt(0);
            dt.Rows.Remove(dt.Rows[0]);
            dt.Rows.Remove(dt.Rows[dt.Rows.Count - 1]);

            DataRow[] rows = dt.Select("Column1 is NULL");
            foreach (var row in rows)
            {
                dt.Rows.Remove(row);
            }

            dt.Columns.Add("kodePkt", typeof(String));
            dt.Columns["kodePkt"].DefaultValue = kodePkt;
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["kodePkt"] = kodePkt;
            }

            dt.Columns.Add("jenisTransaksi", typeof(String));
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["jenisTransaksi"] = "Delivery Cabang - " + dt.Rows[a][1].ToString();
            }

            dt.Columns.Add("inout", typeof(String));
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["inout"] = "OUT";
            }

            //using (SqlBulkCopy sbc = new SqlBulkCopy(Variables.connectionString))
            //{
            //    sbc.DestinationTableName = "dbo.DailyStock";
            //    sbc.ColumnMappings.Add(2, 5); //Kode Cabang
            //    sbc.ColumnMappings.Add(3, 6); //Nama Cabang
            //    sbc.ColumnMappings.Add(4, 7); //Keterangan Cabang
            //    sbc.ColumnMappings.Add(dt.Columns.Count - 3, 2); //kodePkt
            //    sbc.ColumnMappings.Add(dt.Columns.Count - 2, 4); //Jenis Transaksi
            //    sbc.ColumnMappings.Add(dt.Columns.Count - 1, 3); //IN-OUT
            //    sbc.ColumnMappings.Add(0, 1); //Tanggal
            //    for (int a = 0; a < 15; a++)
            //        sbc.ColumnMappings.Add(5 + a, 8 + a); //BN + Coin
            //    sbc.WriteToServer(dt);
            //    sbc.Close();
            //}
            //Masukin ke list untuk update/insert
            List<DailyStock> listDeliveryCabangDariExcel = new List<DailyStock>();
            int rownum = 1;
            foreach (DataRow row in dt.Rows)
            {
                Console.WriteLine(rownum++);
                if (String.IsNullOrWhiteSpace(row[2].ToString()) || String.IsNullOrEmpty(row['W'-'A'].ToString()) || row['W'-'A'].ToString() == "0")
                    continue;

                Int64 BN100K = 0,
                    BN50K = 0,
                    BN20K = 0,
                    BN10K = 0,
                    BN5K = 0,
                    BN2K = 0,
                    BN1K = 0,
                    BN500 = 0,
                    BN100 = 0,
                    C1K = 0,
                    C500 = 0,
                    C200 = 0,
                    C100 = 0,
                    C50 = 0,
                    C25 = 0,
                    buf;
                String SBN100K = row[5].ToString(),
                       SBN50K = row[6].ToString(),
                       SBN20K = row[7].ToString(),
                       SBN10K = row[8].ToString(),
                       SBN5K = row[9].ToString(),
                       SBN2K = row[10].ToString(),
                       SBN1K = row[11].ToString(),
                       SBN500 = row[12].ToString(),
                       SBN100 = row[13].ToString(),
                       SC1K = row[14].ToString(),
                       SC500 = row[15].ToString(),
                       SC200 = row[16].ToString(),
                       SC100 = row[17].ToString(),
                       SC50 = row[18].ToString(),
                       SC25 = row[19].ToString();
                if (!String.IsNullOrEmpty(SBN100K))
                    if (Int64.TryParse(SBN100K, out buf))
                        BN100K = buf;
                if (!String.IsNullOrEmpty(SBN50K))
                    if (Int64.TryParse(SBN50K, out buf))
                        BN50K = buf;
                if (!String.IsNullOrEmpty(SBN20K))
                    if (Int64.TryParse(SBN20K, out buf))
                        BN20K = buf;
                if (!String.IsNullOrEmpty(SBN10K))
                    if (Int64.TryParse(SBN10K, out buf))
                        BN10K = buf;
                if (!String.IsNullOrEmpty(SBN5K))
                    if (Int64.TryParse(SBN5K, out buf))
                        BN5K = buf;
                if (!String.IsNullOrEmpty(SBN2K))
                    if (Int64.TryParse(SBN2K, out buf))
                        BN2K = buf;
                if (!String.IsNullOrEmpty(SBN1K))
                    if (Int64.TryParse(SBN1K, out buf))
                        BN1K = buf;
                if (!String.IsNullOrEmpty(SBN500))
                    if (Int64.TryParse(SBN500, out buf))
                        BN500 = buf;
                if (!String.IsNullOrEmpty(SBN100))
                    if (Int64.TryParse(SBN100, out buf))
                        BN100 = buf;
                if (!String.IsNullOrEmpty(SC1K))
                    if (Int64.TryParse(SC1K, out buf))
                        C1K = buf;
                if (!String.IsNullOrEmpty(SC500))
                    if (Int64.TryParse(SC500, out buf))
                        C500 = buf;
                if (!String.IsNullOrEmpty(SC200))
                    if (Int64.TryParse(SC200, out buf))
                        C200 = buf;
                if (!String.IsNullOrEmpty(SC100))
                    if (Int64.TryParse(SC100, out buf))
                        C100 = buf;
                if (!String.IsNullOrEmpty(SC50))
                    if (Int64.TryParse(SC50, out buf))
                        C50 = buf;
                if (!String.IsNullOrEmpty(SC25))
                    if (Int64.TryParse(SC25, out buf))
                        C25 = buf;
                DateTime dateBuf;
                if (!DateTime.TryParse(row[0].ToString(), out dateBuf))
                {
                    MessageBox.Show("Format tanggal file " + currentFileName + " Sheet " + SHEET_NAME_DEL_CABANG + " Format tanggal salah!");
                    return false;
                }
                listDeliveryCabangDariExcel.Add(new DailyStock()
                {
                    kode = row[2].ToString(),
                    nama = row[3].ToString(),
                    keterangan = row[4].ToString(),
                    kodePkt = row[dt.Columns.Count - 3].ToString(),
                    in_out = row[dt.Columns.Count - 1].ToString(),
                    jenisTransaksi = row[dt.Columns.Count - 2].ToString(),
                    tanggal = DateTime.Parse(row[0].ToString()),
                    BN100K = BN100K,
                    BN50K = BN50K,
                    BN20K = BN20K,
                    BN10K = BN10K,
                    BN5K = BN5K,
                    BN2K = BN2K,
                    BN1K = BN1K,
                    CN1K = C1K,
                    CN500 = C500,
                    CN200 = C200,
                    CN100 = C100,
                    CN50 = C50,
                    CN25 = C25,
                    BN500 = BN500,
                    BN100 = BN100,
                    //udah gadipake
                    BN200 = 0,
                });
            }
            listDeliveryCabangDariExcel = listDeliveryCabangDariExcel.GroupBy(x => new { x.tanggal, x.kodePkt, x.kode, x.nama, x.jenisTransaksi }).Select(x => new DailyStock() {
                tanggal = x.Key.tanggal,
                kode = x.Key.kode,
                kodePkt = x.Key.kodePkt,
                nama = x.Key.nama,
                jenisTransaksi = x.Key.jenisTransaksi,
                keterangan = x.Select(y=>y.keterangan).First(),
                in_out = x.Select(y=>y.in_out).First(),
                BN100K = x.Sum(y=>y.BN100K),
                BN50K = x.Sum(y=>y.BN50K),
                BN20K = x.Sum(y=>y.BN20K),
                BN10K = x.Sum(y=>y.BN10K),
                BN5K = x.Sum(y=>y.BN5K),
                BN2K = x.Sum(y=>y.BN2K),
                BN1K = x.Sum(y=>y.BN1K),
                BN500 = x.Sum(y=>y.BN500),
                BN200 = x.Sum(y=>y.BN200),
                BN100 = x.Sum(y=>y.BN100),
                CN1K = x.Sum(y=>y.CN1K),
                CN500 = x.Sum(y=>y.CN500),
                CN200 = x.Sum(y=>y.CN200),
                CN100 = x.Sum(y=>y.CN100),
                CN50 = x.Sum(y=>y.CN50),
                CN25 = x.Sum(y=>y.CN25),
            }).ToList();

            List<DailyStock> listDataDb = db.DailyStocks.Where(x => x.kodePkt == kodePkt).ToList();
            List<DailyStock> toInput = new List<DailyStock>();
            foreach (DailyStock checkUpdate in listDeliveryCabangDariExcel)
            {
                var toUpdate = listDataDb.Where(x => x.kode == checkUpdate.kode && x.tanggal == checkUpdate.tanggal && x.jenisTransaksi == checkUpdate.jenisTransaksi).FirstOrDefault();
                if (toUpdate != null)
                {
                    toUpdate.BN100K = checkUpdate.BN100K;
                    toUpdate.BN50K = checkUpdate.BN50K;
                    toUpdate.BN20K = checkUpdate.BN20K;
                    toUpdate.BN10K = checkUpdate.BN10K;
                    toUpdate.BN5K = checkUpdate.BN5K;
                    toUpdate.BN2K = checkUpdate.BN2K;
                    toUpdate.BN1K = checkUpdate.BN1K;
                    toUpdate.BN500 = checkUpdate.BN500;
                    toUpdate.BN100 = checkUpdate.BN100;
                    toUpdate.CN1K = checkUpdate.CN1K;
                    toUpdate.CN500 = checkUpdate.CN500;
                    toUpdate.CN200 = checkUpdate.CN200;
                    toUpdate.CN100 = checkUpdate.CN100;
                    toUpdate.CN50 = checkUpdate.CN50;
                    toUpdate.CN25 = checkUpdate.CN25;
                }
                else
                {
                    toInput.Add(checkUpdate);
                }
            }
            db.DailyStocks.AddRange(toInput);
            db.SaveChanges();
            return true;
        }
        private bool readDeliveryRetail()
        {
            DataTable dt = deliveryRetail;

            //dataGridView1.DataSource = dt;
            dt.Rows.RemoveAt(0);
            dt.Rows.Remove(dt.Rows[0]); dt.Rows.Remove(dt.Rows[dt.Rows.Count - 1]);
            DataRow[] rows = dt.Select("Column2 is NULL");
            foreach (var row in rows)
            {
                dt.Rows.Remove(row);
            }


            //Console.WriteLine(dt.Rows.Count);
            //Console.WriteLine(dt.Rows[0][4]);
            //Console.WriteLine(dt.Rows[dt.Rows.Count - 1][2].ToString());
            dt.Columns.Add("kodePkt", typeof(String));
            dt.Columns["kodePkt"].DefaultValue = kodePkt;
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["kodePkt"] = kodePkt;
            }

            dt.Columns.Add("jenisTransaksi", typeof(String));
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["jenisTransaksi"] = "Delivery Retail";
            }

            dt.Columns.Add("inout", typeof(String));
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["inout"] = "OUT";
            }

            //Console.WriteLine(dt.Rows[dt.Rows.Count - 1][dt.Columns.Count - 1].ToString());
            //using (SqlBulkCopy sbc = new SqlBulkCopy(Variables.connectionString))
            //{
            //    sbc.DestinationTableName = "dbo.DailyStock";
            //    //Kode Nasabah
            //    sbc.ColumnMappings.Add(1, 5);
            //    //Nama Nasabah
            //    sbc.ColumnMappings.Add(2, 6);
            //    //Kode Pkt
            //    sbc.ColumnMappings.Add(dt.Columns.Count - 3, 2);
            //    //Jenis Transaksi
            //    sbc.ColumnMappings.Add(dt.Columns.Count - 2, 4);
            //    //IN - OUT
            //    sbc.ColumnMappings.Add(dt.Columns.Count - 1, 3);
            //    //Tanggal
            //    sbc.ColumnMappings.Add(0, 1);
            //    //Uang
            //    for (int a = 0; a < 15; a++)
            //        sbc.ColumnMappings.Add(4 + a, 8 + a);
            //    sbc.WriteToServer(dt);
            //    sbc.Close();
            //}

            //Masukin ke list untuk update/insert
            List<DailyStock> listDeliveryRetailDariExcel = new List<DailyStock>();
            foreach (DataRow row in dt.Rows)
            {
                if (String.IsNullOrWhiteSpace(row[1].ToString()) || String.IsNullOrEmpty(row['V' - 'A'].ToString()) || row['V' - 'A'].ToString() == "0")
                    continue;

                Int64 BN100K = 0,
                    BN50K = 0,
                    BN20K = 0,
                    BN10K = 0,
                    BN5K = 0,
                    BN2K = 0,
                    BN1K = 0,
                    BN500 = 0,
                    BN100 = 0,
                    C1K = 0,
                    C500 = 0,
                    C200 = 0,
                    C100 = 0,
                    C50 = 0,
                    C25 = 0,
                    buf;
                String SBN100K = row[4].ToString(),
                       SBN50K = row[5].ToString(),
                       SBN20K = row[6].ToString(),
                       SBN10K = row[7].ToString(),
                       SBN5K = row[8].ToString(),
                       SBN2K = row[9].ToString(),
                       SBN1K = row[10].ToString(),
                       SBN500 = row[11].ToString(),
                       SBN100 = row[12].ToString(),
                       SC1K = row[13].ToString(),
                       SC500 = row[14].ToString(),
                       SC200 = row[15].ToString(),
                       SC100 = row[16].ToString(),
                       SC50 = row[17].ToString(),
                       SC25 = row[18].ToString();
                if (!String.IsNullOrEmpty(SBN100K))
                    if (Int64.TryParse(SBN100K, out buf))
                        BN100K = buf;
                if (!String.IsNullOrEmpty(SBN50K))
                    if (Int64.TryParse(SBN50K, out buf))
                        BN50K = buf;
                if (!String.IsNullOrEmpty(SBN20K))
                    if (Int64.TryParse(SBN20K, out buf))
                        BN20K = buf;
                if (!String.IsNullOrEmpty(SBN10K))
                    if (Int64.TryParse(SBN10K, out buf))
                        BN10K = buf;
                if (!String.IsNullOrEmpty(SBN5K))
                    if (Int64.TryParse(SBN5K, out buf))
                        BN5K = buf;
                if (!String.IsNullOrEmpty(SBN2K))
                    if (Int64.TryParse(SBN2K, out buf))
                        BN2K = buf;
                if (!String.IsNullOrEmpty(SBN1K))
                    if (Int64.TryParse(SBN1K, out buf))
                        BN1K = buf;
                if (!String.IsNullOrEmpty(SBN500))
                    if (Int64.TryParse(SBN500, out buf))
                        BN500 = buf;
                if (!String.IsNullOrEmpty(SBN100))
                    if (Int64.TryParse(SBN100, out buf))
                        BN100 = buf;
                if (!String.IsNullOrEmpty(SC1K))
                    if (Int64.TryParse(SC1K, out buf))
                        C1K = buf;
                if (!String.IsNullOrEmpty(SC500))
                    if (Int64.TryParse(SC500, out buf))
                        C500 = buf;
                if (!String.IsNullOrEmpty(SC200))
                    if (Int64.TryParse(SC200, out buf))
                        C200 = buf;
                if (!String.IsNullOrEmpty(SC100))
                    if (Int64.TryParse(SC100, out buf))
                        C100 = buf;
                if (!String.IsNullOrEmpty(SC50))
                    if (Int64.TryParse(SC50, out buf))
                        C50 = buf;
                if (!String.IsNullOrEmpty(SC25))
                    if (Int64.TryParse(SC25, out buf))
                        C25 = buf;
                DateTime dateBuf;
                if (!DateTime.TryParse(row[0].ToString(), out dateBuf))
                {
                    MessageBox.Show("Format tanggal file " + currentFileName + " Sheet " + SHEET_NAME_DEL_RETAIL + " Format tanggal salah!");
                    return false;
                }
                listDeliveryRetailDariExcel.Add(new DailyStock()
                {
                    kode = row[1].ToString(),
                    nama = row[2].ToString(),
                    keterangan = "",
                    kodePkt = row[dt.Columns.Count - 3].ToString(),
                    in_out = row[dt.Columns.Count - 1].ToString(),
                    jenisTransaksi = row[dt.Columns.Count - 2].ToString(),
                    tanggal = DateTime.Parse(row[0].ToString()),
                    BN100K = BN100K,
                    BN50K = BN50K,
                    BN20K = BN20K,
                    BN10K = BN10K,
                    BN5K = BN5K,
                    BN2K = BN2K,
                    BN1K = BN1K,
                    CN1K = C1K,
                    CN500 = C500,
                    CN200 = C200,
                    CN100 = C100,
                    CN50 = C50,
                    CN25 = C25,
                    BN500 = BN500,
                    BN100 = BN100,
                    //udah gadipake
                    BN200 = 0,
                });
            }

            listDeliveryRetailDariExcel = listDeliveryRetailDariExcel.GroupBy(x => new { x.tanggal, x.kodePkt, x.kode, x.nama, x.jenisTransaksi }).Select(x => new DailyStock()
            {
                tanggal = x.Key.tanggal,
                kode = x.Key.kode,
                kodePkt = x.Key.kodePkt,
                nama = x.Key.nama,
                jenisTransaksi = x.Key.jenisTransaksi,
                keterangan = x.Select(y => y.keterangan).First(),
                in_out = x.Select(y => y.in_out).First(),
                BN100K = x.Sum(y => y.BN100K),
                BN50K = x.Sum(y => y.BN50K),
                BN20K = x.Sum(y => y.BN20K),
                BN10K = x.Sum(y => y.BN10K),
                BN5K = x.Sum(y => y.BN5K),
                BN2K = x.Sum(y => y.BN2K),
                BN1K = x.Sum(y => y.BN1K),
                BN500 = x.Sum(y => y.BN500),
                BN200 = x.Sum(y => y.BN200),
                BN100 = x.Sum(y => y.BN100),
                CN1K = x.Sum(y => y.CN1K),
                CN500 = x.Sum(y => y.CN500),
                CN200 = x.Sum(y => y.CN200),
                CN100 = x.Sum(y => y.CN100),
                CN50 = x.Sum(y => y.CN50),
                CN25 = x.Sum(y => y.CN25),
            }).ToList();

            List<DailyStock> listDataDb = db.DailyStocks.Where(x => x.kodePkt == kodePkt).ToList();
            List<DailyStock> toInput = new List<DailyStock>();
            foreach (DailyStock checkUpdate in listDeliveryRetailDariExcel)
            {
                var toUpdate = listDataDb.Where(x => x.kode == checkUpdate.kode && x.tanggal == checkUpdate.tanggal && x.jenisTransaksi == checkUpdate.jenisTransaksi).FirstOrDefault();
                if (toUpdate != null)
                {
                    toUpdate.BN100K = checkUpdate.BN100K;
                    toUpdate.BN50K = checkUpdate.BN50K;
                    toUpdate.BN20K = checkUpdate.BN20K;
                    toUpdate.BN10K = checkUpdate.BN10K;
                    toUpdate.BN5K = checkUpdate.BN5K;
                    toUpdate.BN2K = checkUpdate.BN2K;
                    toUpdate.BN1K = checkUpdate.BN1K;
                    toUpdate.BN500 = checkUpdate.BN500;
                    toUpdate.BN100 = checkUpdate.BN100;
                    toUpdate.CN1K = checkUpdate.CN1K;
                    toUpdate.CN500 = checkUpdate.CN500;
                    toUpdate.CN200 = checkUpdate.CN200;
                    toUpdate.CN100 = checkUpdate.CN100;
                    toUpdate.CN50 = checkUpdate.CN50;
                    toUpdate.CN25 = checkUpdate.CN25;
                }
                else
                {
                    toInput.Add(checkUpdate);
                }
            }
            db.DailyStocks.AddRange(toInput);
            db.SaveChanges();
            return true;
        }
        private bool readDeliveryLainnya()
        {
            DataTable dt = deliveryLainnya;
            dt.Rows.RemoveAt(0);
            dt.Rows.Remove(dt.Rows[0]); dt.Rows.Remove(dt.Rows[dt.Rows.Count - 1]);

            //dataGridView1.DataSource = dt;

            DataRow[] rows = dt.Select("Column2 is NULL");
            foreach (var row in rows)
            {
                dt.Rows.Remove(row);
            }


            //Console.WriteLine(dt.Rows.Count);
            //Console.WriteLine(dt.Rows[0][2]);
            //Console.WriteLine(dt.Rows[dt.Rows.Count - 1][2].ToString());
            dt.Columns.Add("kodePkt", typeof(String));
            dt.Columns["kodePkt"].DefaultValue = kodePkt;
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["kodePkt"] = kodePkt;
            }
            dt.Columns.Add("jenisTransaksi", typeof(String));
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["jenisTransaksi"] = "Delivery Lainnya - " + dt.Rows[a][1].ToString();
            }

            dt.Columns.Add("inout", typeof(String));
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["inout"] = "OUT";
            }

            //Console.WriteLine(dt.Rows[dt.Rows.Count - 1][dt.Columns.Count - 1].ToString());
            //using (SqlBulkCopy sbc = new SqlBulkCopy(Variables.connectionString))
            //{
            //    sbc.DestinationTableName = "dbo.DailyStock";
            //    //Kode Pkt
            //    sbc.ColumnMappings.Add(dt.Columns.Count - 3, 2);
            //    //Jenis Transaksi
            //    sbc.ColumnMappings.Add(dt.Columns.Count - 2, 4);
            //    //IN-OUT
            //    sbc.ColumnMappings.Add(dt.Columns.Count - 1, 3);
            //    //Tanggal
            //    sbc.ColumnMappings.Add(0, 1);
            //    //Keterangan Sumber Dana
            //    sbc.ColumnMappings.Add(1, 7);
            //    //Uang
            //    for (int a = 0; a < 15; a++)
            //        sbc.ColumnMappings.Add(4 + a, 8 + a);
            //    sbc.WriteToServer(dt);
            //    sbc.Close();
            //}

            //Masukin ke list untuk update/insert
            List<DailyStock> listDeliveryLainnyaDariExcel = new List<DailyStock>();
            foreach (DataRow row in dt.Rows)
            {
                if (String.IsNullOrWhiteSpace(row[1].ToString()) || String.IsNullOrEmpty(row['V' - 'A'].ToString()) || row['V' - 'A'].ToString() == "0")
                    continue;

                Int64 BN100K = 0,
                    BN50K = 0,
                    BN20K = 0,
                    BN10K = 0,
                    BN5K = 0,
                    BN2K = 0,
                    BN1K = 0,
                    BN500 = 0,
                    BN100 = 0,
                    C1K = 0,
                    C500 = 0,
                    C200 = 0,
                    C100 = 0,
                    C50 = 0,
                    C25 = 0,
                    buf;
                String SBN100K = row[4].ToString(),
                       SBN50K = row[5].ToString(),
                       SBN20K = row[6].ToString(),
                       SBN10K = row[7].ToString(),
                       SBN5K = row[8].ToString(),
                       SBN2K = row[9].ToString(),
                       SBN1K = row[10].ToString(),
                       SBN500 = row[11].ToString(),
                       SBN100 = row[12].ToString(),
                       SC1K = row[13].ToString(),
                       SC500 = row[14].ToString(),
                       SC200 = row[15].ToString(),
                       SC100 = row[16].ToString(),
                       SC50 = row[17].ToString(),
                       SC25 = row[18].ToString();
                if (!String.IsNullOrEmpty(SBN100K))
                    if (Int64.TryParse(SBN100K, out buf))
                        BN100K = buf;
                if (!String.IsNullOrEmpty(SBN50K))
                    if (Int64.TryParse(SBN50K, out buf))
                        BN50K = buf;
                if (!String.IsNullOrEmpty(SBN20K))
                    if (Int64.TryParse(SBN20K, out buf))
                        BN20K = buf;
                if (!String.IsNullOrEmpty(SBN10K))
                    if (Int64.TryParse(SBN10K, out buf))
                        BN10K = buf;
                if (!String.IsNullOrEmpty(SBN5K))
                    if (Int64.TryParse(SBN5K, out buf))
                        BN5K = buf;
                if (!String.IsNullOrEmpty(SBN2K))
                    if (Int64.TryParse(SBN2K, out buf))
                        BN2K = buf;
                if (!String.IsNullOrEmpty(SBN1K))
                    if (Int64.TryParse(SBN1K, out buf))
                        BN1K = buf;
                if (!String.IsNullOrEmpty(SBN500))
                    if (Int64.TryParse(SBN500, out buf))
                        BN500 = buf;
                if (!String.IsNullOrEmpty(SBN100))
                    if (Int64.TryParse(SBN100, out buf))
                        BN100 = buf;
                if (!String.IsNullOrEmpty(SC1K))
                    if (Int64.TryParse(SC1K, out buf))
                        C1K = buf;
                if (!String.IsNullOrEmpty(SC500))
                    if (Int64.TryParse(SC500, out buf))
                        C500 = buf;
                if (!String.IsNullOrEmpty(SC200))
                    if (Int64.TryParse(SC200, out buf))
                        C200 = buf;
                if (!String.IsNullOrEmpty(SC100))
                    if (Int64.TryParse(SC100, out buf))
                        C100 = buf;
                if (!String.IsNullOrEmpty(SC50))
                    if (Int64.TryParse(SC50, out buf))
                        C50 = buf;
                if (!String.IsNullOrEmpty(SC25))
                    if (Int64.TryParse(SC25, out buf))
                        C25 = buf;
                DateTime dateBuf;
                if (!DateTime.TryParse(row[0].ToString(), out dateBuf))
                {
                    MessageBox.Show("Format tanggal file " + currentFileName + " Sheet " + SHEET_NAME_COL_RETAIL + " Format tanggal salah!");
                    return false;
                }
                listDeliveryLainnyaDariExcel.Add(new DailyStock()
                {
                    kode = "",
                    nama = "",
                    keterangan = row[2].ToString(),
                    kodePkt = row[dt.Columns.Count - 3].ToString(),
                    in_out = row[dt.Columns.Count - 1].ToString(),
                    jenisTransaksi = row[dt.Columns.Count - 2].ToString(),
                    tanggal = DateTime.Parse(row[0].ToString()),
                    BN100K = BN100K,
                    BN50K = BN50K,
                    BN20K = BN20K,
                    BN10K = BN10K,
                    BN5K = BN5K,
                    BN2K = BN2K,
                    BN1K = BN1K,
                    CN1K = C1K,
                    CN500 = C500,
                    CN200 = C200,
                    CN100 = C100,
                    CN50 = C50,
                    CN25 = C25,
                    BN500 = BN500,
                    BN100 = BN100,
                    //udah gadipake
                    BN200 = 0,
                });
            }
            listDeliveryLainnyaDariExcel = listDeliveryLainnyaDariExcel.GroupBy(x => new { x.tanggal, x.kodePkt, x.kode, x.nama, x.jenisTransaksi }).Select(x => new DailyStock()
            {
                tanggal = x.Key.tanggal,
                kode = x.Key.kode,
                kodePkt = x.Key.kodePkt,
                nama = x.Key.nama,
                jenisTransaksi = x.Key.jenisTransaksi,
                keterangan = x.Select(y => y.keterangan).First(),
                in_out = x.Select(y => y.in_out).First(),
                BN100K = x.Sum(y => y.BN100K),
                BN50K = x.Sum(y => y.BN50K),
                BN20K = x.Sum(y => y.BN20K),
                BN10K = x.Sum(y => y.BN10K),
                BN5K = x.Sum(y => y.BN5K),
                BN2K = x.Sum(y => y.BN2K),
                BN1K = x.Sum(y => y.BN1K),
                BN500 = x.Sum(y => y.BN500),
                BN200 = x.Sum(y => y.BN200),
                BN100 = x.Sum(y => y.BN100),
                CN1K = x.Sum(y => y.CN1K),
                CN500 = x.Sum(y => y.CN500),
                CN200 = x.Sum(y => y.CN200),
                CN100 = x.Sum(y => y.CN100),
                CN50 = x.Sum(y => y.CN50),
                CN25 = x.Sum(y => y.CN25),
            }).ToList();
            List<DailyStock> listDataDb = db.DailyStocks.Where(x => x.kodePkt == kodePkt).ToList();
            List<DailyStock> toInput = new List<DailyStock>();
            foreach (DailyStock checkUpdate in listDeliveryLainnyaDariExcel)
            {
                var toUpdate = listDataDb.Where(x => x.kode == checkUpdate.kode && x.tanggal == checkUpdate.tanggal && x.jenisTransaksi == checkUpdate.jenisTransaksi).FirstOrDefault();
                if (toUpdate != null)
                {
                    toUpdate.BN100K = checkUpdate.BN100K;
                    toUpdate.BN50K = checkUpdate.BN50K;
                    toUpdate.BN20K = checkUpdate.BN20K;
                    toUpdate.BN10K = checkUpdate.BN10K;
                    toUpdate.BN5K = checkUpdate.BN5K;
                    toUpdate.BN2K = checkUpdate.BN2K;
                    toUpdate.BN1K = checkUpdate.BN1K;
                    toUpdate.BN500 = checkUpdate.BN500;
                    toUpdate.BN100 = checkUpdate.BN100;
                    toUpdate.CN1K = checkUpdate.CN1K;
                    toUpdate.CN500 = checkUpdate.CN500;
                    toUpdate.CN200 = checkUpdate.CN200;
                    toUpdate.CN100 = checkUpdate.CN100;
                    toUpdate.CN50 = checkUpdate.CN50;
                    toUpdate.CN25 = checkUpdate.CN25;
                }
                else
                {
                    toInput.Add(checkUpdate);
                }
            }
            db.DailyStocks.AddRange(toInput);
            db.SaveChanges();
            return true;
        }
        private bool readDailyStock()
        {
            int COL_NAMA_PKT = 'N' - 'A',
                COL_TANGGAL = 'C'-'A',
                COL_OPEN_BALANCE = 'D' - 'A',
                COL_ENDING_BALANCE = 'Y' - 'A',
                ROW_NAMA_PKT = 1,
                ROW_TANGGAL = 3,
                ROW_BN100K = 7,
                ROW_BN50K = 8,
                ROW_BN20K = 9,
                ROW_BN10K = 10,
                ROW_BN5K = 11,
                ROW_BN2K = 12,
                ROW_BN1K = 13,
                ROW_BN500 = 14,
                ROW_BN100 = 15,

                ROW_C1000 = 18,
                ROW_C500 = 19,
                ROW_C200 = 20,
                ROW_C100 = 21,
                ROW_C50 = 22,
                ROW_C25 = 23;
            Int64 buf;
            DataTable dt = dailystock;
            String namaPkt = dt.Rows[ROW_NAMA_PKT][COL_NAMA_PKT].ToString();

            DateTime bufDate, tanggal;
            if (!DateTime.TryParse(dt.Rows[ROW_TANGGAL][COL_TANGGAL].ToString(), out bufDate))
            {
                MessageBox.Show("Format tanggal file " + currentFileName + " Sheet " + SHEET_NAME_DAILYSTOCK + " Format tanggal salah!");
                return false;
            }
            tanggal = DateTime.Parse(dt.Rows[ROW_TANGGAL][COL_TANGGAL].ToString());
            //kodePkt = db.Pkts.Where(x => x.namaPkt == namaPkt).Select(x => x.kodePktCabang).FirstOrDefault();
            Int64 OpenBN100k = Int64.TryParse(dt.Rows[ROW_BN100K][COL_OPEN_BALANCE].ToString(), out buf) ? buf : 0,
                OpenBN50k = Int64.TryParse(dt.Rows[ROW_BN50K][COL_OPEN_BALANCE].ToString(), out buf) ? buf : 0,
                OpenBN20k = Int64.TryParse(dt.Rows[ROW_BN20K][COL_OPEN_BALANCE].ToString(), out buf) ? buf : 0,
                OpenBN10k = Int64.TryParse(dt.Rows[ROW_BN10K][COL_OPEN_BALANCE].ToString(), out buf) ? buf : 0,
                OpenBN5k = Int64.TryParse(dt.Rows[ROW_BN5K][COL_OPEN_BALANCE].ToString(), out buf) ? buf : 0,
                OpenBN2k = Int64.TryParse(dt.Rows[ROW_BN2K][COL_OPEN_BALANCE].ToString(), out buf) ? buf : 0,
                OpenBN1k = Int64.TryParse(dt.Rows[ROW_BN1K][COL_OPEN_BALANCE].ToString(), out buf) ? buf : 0,
                OpenBN500 = Int64.TryParse(dt.Rows[ROW_BN500][COL_OPEN_BALANCE].ToString(), out buf) ? buf : 0,
                OpenBN100 = Int64.TryParse(dt.Rows[ROW_BN100][COL_OPEN_BALANCE].ToString(), out buf) ? buf : 0,
                OpenC1000 = Int64.TryParse(dt.Rows[ROW_C1000][COL_OPEN_BALANCE].ToString(), out buf) ? buf : 0,
                OpenC500 = Int64.TryParse(dt.Rows[ROW_C500][COL_OPEN_BALANCE].ToString(), out buf) ? buf : 0,
                OpenC200 = Int64.TryParse(dt.Rows[ROW_C200][COL_OPEN_BALANCE].ToString(), out buf) ? buf : 0,
                OpenC100 = Int64.TryParse(dt.Rows[ROW_C100][COL_OPEN_BALANCE].ToString(), out buf) ? buf : 0,
                OpenC50 = Int64.TryParse(dt.Rows[ROW_C50][COL_OPEN_BALANCE].ToString(), out buf) ? buf : 0,
                OpenC25 = Int64.TryParse(dt.Rows[ROW_C25][COL_OPEN_BALANCE].ToString(), out buf) ? buf : 0,
                EndingBN100k = Int64.TryParse(dt.Rows[ROW_BN100K][COL_ENDING_BALANCE].ToString(), out buf) ? buf : 0,
                EndingBN50k = Int64.TryParse(dt.Rows[ROW_BN50K][COL_ENDING_BALANCE].ToString(), out buf) ? buf : 0,
                EndingBN20k = Int64.TryParse(dt.Rows[ROW_BN20K][COL_ENDING_BALANCE].ToString(), out buf) ? buf : 0,
                EndingBN10k = Int64.TryParse(dt.Rows[ROW_BN10K][COL_ENDING_BALANCE].ToString(), out buf) ? buf : 0,
                EndingBN5k = Int64.TryParse(dt.Rows[ROW_BN5K][COL_ENDING_BALANCE].ToString(), out buf) ? buf : 0,
                EndingBN2k = Int64.TryParse(dt.Rows[ROW_BN2K][COL_ENDING_BALANCE].ToString(), out buf) ? buf : 0,
                EndingBN1k = Int64.TryParse(dt.Rows[ROW_BN1K][COL_ENDING_BALANCE].ToString(), out buf) ? buf : 0,
                EndingBN500 = Int64.TryParse(dt.Rows[ROW_BN500][COL_ENDING_BALANCE].ToString(), out buf) ? buf : 0,
                EndingBN100 = Int64.TryParse(dt.Rows[ROW_BN100][COL_ENDING_BALANCE].ToString(), out buf) ? buf : 0,
                EndingC1000 = Int64.TryParse(dt.Rows[ROW_C1000][COL_ENDING_BALANCE].ToString(), out buf) ? buf : 0,
                EndingC500 = Int64.TryParse(dt.Rows[ROW_C500][COL_ENDING_BALANCE].ToString(), out buf) ? buf : 0,
                EndingC200 = Int64.TryParse(dt.Rows[ROW_C200][COL_ENDING_BALANCE].ToString(), out buf) ? buf : 0,
                EndingC100 = Int64.TryParse(dt.Rows[ROW_C100][COL_ENDING_BALANCE].ToString(), out buf) ? buf : 0,
                EndingC50 = Int64.TryParse(dt.Rows[ROW_C50][COL_ENDING_BALANCE].ToString(), out buf) ? buf : 0,
                EndingC25 = Int64.TryParse(dt.Rows[ROW_C25][COL_ENDING_BALANCE].ToString(), out buf) ? buf : 0;
            DailyStock openBalance = new DailyStock()
            {
                kodePkt = kodePkt,
                tanggal = tanggal,
                BN100K = OpenBN100k,
                BN50K = OpenBN50k,
                BN20K = OpenBN20k,
                BN10K = OpenBN10k,
                BN5K = OpenBN5k,
                BN2K = OpenBN2k,
                BN1K = OpenBN1k,
                BN500 = OpenBN500,
                BN200 = 0,
                BN100 = OpenBN100,
                CN1K = OpenC1000,
                CN500 = OpenC500,
                CN200 = OpenC200,
                CN100 = OpenC100,
                CN50 = OpenC50,
                CN25 = OpenC25,
                in_out = "",
                jenisTransaksi = "OPEN BALANCE",
                keterangan = "OPEN BALANCE",
                nama = "",
                kode = "",
            };
            DailyStock endingBalance = new DailyStock()
            {
                kodePkt = kodePkt,
                tanggal = tanggal,
                BN100K = EndingBN100k,
                BN50K = EndingBN50k,
                BN20K = EndingBN20k,
                BN10K = EndingBN10k,
                BN5K = EndingBN5k,
                BN2K = EndingBN2k,
                BN1K = EndingBN1k,
                BN500 = EndingBN500,
                BN200 = 0,
                BN100 = EndingBN100,
                CN1K = EndingC1000,
                CN500 = EndingC500,
                CN200 = EndingC200,
                CN100 = EndingC100,
                CN50 = EndingC50,
                CN25 = EndingC25,
                in_out = "",
                jenisTransaksi = "ENDING BALANCE",
                keterangan = "ENDING BALANCE",
                nama = "",
                kode = "",
            };
            if (db.DailyStocks.Where(x => x.tanggal == openBalance.tanggal && x.kodePkt == openBalance.kodePkt && x.jenisTransaksi == "OPEN BALANCE").FirstOrDefault() != null)
            {
                var dbOpenBalance = db.DailyStocks.Where(x => x.tanggal == openBalance.tanggal && x.kodePkt == openBalance.kodePkt && x.jenisTransaksi == "OPEN BALANCE").FirstOrDefault();
                int id = dbOpenBalance.idTransaksi;
                dbOpenBalance.BN100K = openBalance.BN100K;
                dbOpenBalance.BN50K = openBalance.BN50K;
                dbOpenBalance.BN20K = openBalance.BN20K;
                dbOpenBalance.BN10K = openBalance.BN10K;
                dbOpenBalance.BN5K = openBalance.BN5K;
                dbOpenBalance.BN2K = openBalance.BN2K;
                dbOpenBalance.BN1K = openBalance.BN1K;
                dbOpenBalance.BN500 = openBalance.BN500;
                dbOpenBalance.BN100 = openBalance.BN100;

                dbOpenBalance.CN1K = openBalance.CN1K;
                dbOpenBalance.CN500 = openBalance.CN500;
                dbOpenBalance.CN200 = openBalance.CN200;
                dbOpenBalance.CN100 = openBalance.CN100;
                dbOpenBalance.CN50 = openBalance.CN50;
                dbOpenBalance.CN25 = openBalance.CN25;

                db.SaveChanges();
            }
            else
            {
                db.DailyStocks.Add(openBalance);
            }
            if (db.DailyStocks.Where(x => x.tanggal == openBalance.tanggal && x.kodePkt == openBalance.kodePkt && x.jenisTransaksi == "ENDING BALANCE").FirstOrDefault() != null)
            {
                var dbEndingBalance = db.DailyStocks.Where(x => x.tanggal == openBalance.tanggal && x.kodePkt == openBalance.kodePkt && x.jenisTransaksi == "ENDING BALANCE").FirstOrDefault();
                int id = dbEndingBalance.idTransaksi;
                dbEndingBalance.BN100K = endingBalance.BN100K;
                dbEndingBalance.BN50K = endingBalance.BN50K;
                dbEndingBalance.BN20K = endingBalance.BN20K;
                dbEndingBalance.BN10K = endingBalance.BN10K;
                dbEndingBalance.BN5K = endingBalance.BN5K;
                dbEndingBalance.BN2K = endingBalance.BN2K;
                dbEndingBalance.BN1K = endingBalance.BN1K;
                dbEndingBalance.BN500 = endingBalance.BN500;
                dbEndingBalance.BN100 = endingBalance.BN100;

                dbEndingBalance.CN1K = endingBalance.CN1K;
                dbEndingBalance.CN500 = endingBalance.CN500;
                dbEndingBalance.CN200 = endingBalance.CN200;
                dbEndingBalance.CN100 = endingBalance.CN100;
                dbEndingBalance.CN50 = endingBalance.CN50;
                dbEndingBalance.CN25 = endingBalance.CN25;
                db.SaveChanges();
            }
            else
            {
                db.DailyStocks.Add(endingBalance);
            }
            db.SaveChanges();
            return true;
        }
        private bool readBA()
        {
            DataTable dt = ba;
            DataRow[] toDelete = dt.Select("Column2 is null OR Column0 is null OR Column1 is null");
            foreach (DataRow row in toDelete)
                dt.Rows.Remove(row);
            int COL_TANGGAL_BA = Util.ExcelColumnNameToNumber("A"),
                COL_KODE_CABANG = Util.ExcelColumnNameToNumber("B"),
                COL_NO_BERITA_ACARA = Util.ExcelColumnNameToNumber("C"),
                COL_D100K = Util.ExcelColumnNameToNumber("D"),
                COL_D50K = Util.ExcelColumnNameToNumber("E"),
                COL_D20K = Util.ExcelColumnNameToNumber("F"),
                COL_D10K = Util.ExcelColumnNameToNumber("G"),
                COL_D5K = Util.ExcelColumnNameToNumber("H"),
                COL_D2K = Util.ExcelColumnNameToNumber("I"),
                COL_D1K = Util.ExcelColumnNameToNumber("J"),
                COL_C1K = Util.ExcelColumnNameToNumber("K"),
                COL_C500 = Util.ExcelColumnNameToNumber("L"),
                COL_C200 = Util.ExcelColumnNameToNumber("M"),
                COL_C100 = Util.ExcelColumnNameToNumber("N"),
                COL_C50 = Util.ExcelColumnNameToNumber("O"),
                COL_C25 = Util.ExcelColumnNameToNumber("P"),
                COL_NOMINAL = Util.ExcelColumnNameToNumber("Q"),
                COL_STATUS = Util.ExcelColumnNameToNumber("R"),
                COL_TANGGAL_SETORAN = Util.ExcelColumnNameToNumber("S"),
                COL_NO_TXN = Util.ExcelColumnNameToNumber("T"),
                
                ROW_DATA_START = 1;
            List<BeritaAcara> fromExcel = new List<BeritaAcara>();
            for (int a = ROW_DATA_START; a < dt.Rows.Count; a++)
            {
                DataRow row = dt.Rows[a];
                Int64 d100k = 0,
                    d50k = 0,
                    d20k = 0,
                    d10k = 0,
                    d5k = 0,
                    d2k = 0,
                    d1k = 0,
                    c1k = 0,
                    c500 = 0,
                    c200 = 0,
                    c100 = 0,
                    c50 = 0,
                    c25 = 0,
                    nominal = 0,
                    buf;
                DateTime tanggalBA = DateTime.MinValue,
                    tanggalSetor = DateTime.MinValue,
                    bufTgl;
                if (Int64.TryParse(row[COL_D100K].ToString(), out buf))
                    d100k = buf;
                if (Int64.TryParse(row[COL_D50K].ToString(), out buf))
                    d50k = buf;
                if (Int64.TryParse(row[COL_D20K].ToString(), out buf))
                    d20k = buf;
                if (Int64.TryParse(row[COL_D10K].ToString(), out buf))
                    d10k = buf;
                if (Int64.TryParse(row[COL_D5K].ToString(), out buf))
                    d5k = buf;
                if (Int64.TryParse(row[COL_D2K].ToString(), out buf))
                    d2k = buf;
                if (Int64.TryParse(row[COL_D1K].ToString(), out buf))
                    d1k = buf;
                if (Int64.TryParse(row[COL_C1K].ToString(), out buf))
                    c1k = buf;
                if (Int64.TryParse(row[COL_C500].ToString(), out buf))
                    c500 = buf;
                if (Int64.TryParse(row[COL_C200].ToString(), out buf))
                    c200 = buf;
                if (Int64.TryParse(row[COL_C100].ToString(), out buf))
                    c100 = buf;
                if (Int64.TryParse(row[COL_C50].ToString(), out buf))
                    c50 = buf;
                if (Int64.TryParse(row[COL_C25].ToString(), out buf))
                    c25 = buf;
                if (Int64.TryParse(row[COL_NOMINAL].ToString(), out buf))
                    nominal = buf;


                if (DateTime.TryParse(row[COL_TANGGAL_BA].ToString(), out bufTgl))
                    tanggalBA = bufTgl;
                else
                {
                    MessageBox.Show("Format tanggal file " + currentFileName + " Sheet " + SHEET_NAME_BA + " Format tanggal BA salah!");
                    return false;
                }
                if (DateTime.TryParse(row[COL_TANGGAL_SETORAN].ToString(), out bufTgl))
                    tanggalSetor = bufTgl;
                else
                {
                    MessageBox.Show("Format tanggal file " + currentFileName + " Sheet " + SHEET_NAME_BA + " Format tanggal setor salah!");
                    return false;
                }

                BeritaAcara beritaAcara = new BeritaAcara()
                {
                    kodePkt = kodePkt,
                    bn100k = d100k,
                    bn50k = d50k,
                    bn20k = d20k,
                    bn10k = d10k,
                    bn5k = d5k,
                    bn2k = d2k,
                    bn1k = d1k,
                    c1k = c1k,
                    c500 = c500,
                    c200 = c200,
                    c100 = c100,
                    c50 = c50,
                    c25 = c25,
                    noTxn = row[COL_NO_TXN].ToString().Trim(),
                    tanggal = tanggalBA,
                    tanggalSetor = tanggalSetor,
                    kodeCabang = row[COL_KODE_CABANG].ToString().Trim(),
                    noBeritaAcara = row[COL_NO_BERITA_ACARA].ToString().Trim(),
                    nominal = nominal,
                    jenisIsi = row[COL_STATUS].ToString().ToLower().Trim()
                };
                fromExcel.Add(beritaAcara);
            }

            fromExcel = (from x in fromExcel
                         group x by new { x.noBeritaAcara, x.kodePkt, x.tanggal, x.tanggalSetor, x.kodeCabang, x.jenisIsi, x.noTxn } into g
                         select new BeritaAcara()
                         {
                             noBeritaAcara = g.Key.noBeritaAcara,
                             kodePkt = g.Key.kodePkt,
                             jenisIsi = g.Key.jenisIsi,
                             kodeCabang = g.Key.kodeCabang,
                             noTxn = g.Key.noTxn,
                             tanggal = g.Key.tanggal,
                             tanggalSetor = g.Key.tanggalSetor,
                             bn100k = g.Sum(y => y.bn100k),
                             bn50k = g.Sum(y => y.bn50k),
                             bn20k = g.Sum(y => y.bn20k),
                             bn10k = g.Sum(y => y.bn10k),
                             bn5k = g.Sum(y => y.bn5k),
                             bn2k = g.Sum(y => y.bn2k),
                             bn1k = g.Sum(y => y.bn1k),
                             c1k = g.Sum(y => y.c1k),
                             c500 = g.Sum(y => y.c500),
                             c200 = g.Sum(y => y.c200),
                             c100 = g.Sum(y => y.c100),
                             c50 = g.Sum(y => y.c50),
                             c25 = g.Sum(y => y.c25),
                             nominal = g.Sum(y => y.nominal),
                         }).ToList();
            Console.WriteLine(fromExcel.Count);
            foreach (BeritaAcara toInsertOrUpdate in fromExcel)
            {
                var check = db.BeritaAcaras
                    .Where(x =>
                        x.noBeritaAcara == toInsertOrUpdate.noBeritaAcara
                        && x.jenisIsi == toInsertOrUpdate.jenisIsi
                        && x.noTxn == toInsertOrUpdate.noTxn
                        && x.tanggal == toInsertOrUpdate.tanggal
                        && x.tanggalSetor == toInsertOrUpdate.tanggalSetor
                        && x.kodeCabang == toInsertOrUpdate.kodeCabang
                    ).FirstOrDefault();
                if (check != null)
                {
                    check.bn100k = toInsertOrUpdate.bn100k;
                    check.bn50k = toInsertOrUpdate.bn50k;
                    check.bn20k = toInsertOrUpdate.bn20k;
                    check.bn10k = toInsertOrUpdate.bn10k;
                    check.bn5k = toInsertOrUpdate.bn5k;
                    check.bn2k = toInsertOrUpdate.bn2k;
                    check.bn1k = toInsertOrUpdate.bn1k;

                    check.c1k = toInsertOrUpdate.c1k;
                    check.c500 = toInsertOrUpdate.c500;
                    check.c200 = toInsertOrUpdate.c200;
                    check.c100 = toInsertOrUpdate.c100;
                    check.c50 = toInsertOrUpdate.c50;
                    check.c25 = toInsertOrUpdate.c25;
                    check.nominal = toInsertOrUpdate.nominal;
                }
                else
                    db.BeritaAcaras.Add(toInsertOrUpdate);
                db.SaveChanges();
            }
            return true;
        }
        private void InputButton_Click(object sender, EventArgs e)
        {
            
        }
    }
}
