﻿using LumenWorks.Framework.IO.Csv;
using Oracle.DataAccess.Client;
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
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            //Console.WriteLine(Variables.todayDate.AddDays(-1));
        }

        private void inputOptiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputOpti();
        }
        private void inputDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MultipleFilesInputForm input = new MultipleFilesInputForm();
            input.MdiParent = this;
            input.Show();
        }
        private void revisiDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Revisi rev = new Revisi();
            rev.MdiParent = this;
            rev.Show();

        }
        private void inputDataPktToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputDataPkt();
        }
        private void inputDataDenomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputDataDenom();
        }
        private void informationBoardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadForm.ShowSplashScreen();
            InformationBoard ib = new InformationBoard();
            loadForm.CloseForm();
            ib.MdiParent = this;
            ib.Show();

        }
        private void inputOpti()
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.csvFilter;
            String connectionString = Variables.connectionString;
            
            
            if (of.ShowDialog() == DialogResult.OK)
            {
                loadForm.ShowSplashScreen();
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand("DELETE FROM Opti", con))
                        {
                            command.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                }
                catch (SystemException ex)
                {
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
                }

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("CREATE TABLE #TEMP ([idCashpoint] VARCHAR(255), [tanggal] VARCHAR(30), [prediksi] BIGINT)", con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (var csv = new CachedCsvReader(new StreamReader(of.FileName), true))
                    {
                        // Field headers will automatically be used as column names
                        
                        try
                        {
                            using (var sbc = new SqlBulkCopy(con))
                            {
                                sbc.DestinationTableName = "dbo.#TEMP";
                                sbc.BatchSize = 1000;

                                sbc.AddColumnMapping(0, 0);
                                sbc.AddColumnMapping(12, 2);
                                sbc.AddColumnMapping(2, 1);
                                sbc.WriteToServer(csv);
                            }
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show(err.ToString());
                        }
                    }
                    SqlCommand command = new SqlCommand();
                    command.Connection = con;
                    command.CommandText = "UPDATE #TEMP SET tanggal = REPLACE(tanggal,'   M', '   ') WHERE tanggal like '%M%'";
                    command.ExecuteNonQuery();
                    command.CommandText = "UPDATE #TEMP SET tanggal = REPLACE(tanggal,'   S', '   ') WHERE tanggal like '%S%'";
                    command.ExecuteNonQuery();
                    command.CommandText = "UPDATE #TEMP SET tanggal = REPLACE(tanggal,'   U', '') WHERE tanggal like '%U%'";
                    command.ExecuteNonQuery();
                    command.CommandText = "UPDATE #TEMP SET tanggal = REPLACE(tanggal,'   H', '') WHERE tanggal like '%H%'";
                    command.ExecuteNonQuery();

                    command.CommandText = "DELETE FROM #TEMP WHERE [idCashpoint] IN('AVERAGE','SUMMARY','ATM Horizons - Detail','Cashpoint')";
                    command.ExecuteNonQuery();

                    command.CommandText = "UPDATE #TEMP SET tanggal = RTRIM(tanggal)";
                    command.ExecuteNonQuery();
                    //SqlDataReader reader;
                    //command.CommandText = "SELECT * FROM #TEMP";
                    //reader = command.ExecuteReader();
                    //while (reader.Read())
                    //{
                    //    Console.WriteLine(reader[0] + " " + reader[1] + " " + reader[2]);
                    //}
                    //reader.Close();
                    //command.CommandText = "DBCC CHECKIDENT(\"dbo.Opti\", RESEED, 0)";
                    //command.ExecuteNonQuery();
                    try
                    {
                        command.CommandText = "INSERT INTO Opti(idCashpoint, tanggal, prediksi) SELECT idCashpoint, CAST(tanggal AS DATE), prediksi FROM #TEMP TT";
                        command.ExecuteNonQuery();
                        command.CommandText = "DROP TABLE #TEMP";
                        command.ExecuteNonQuery();
                    }
                    catch(Exception er)
                    {
                        MessageBox.Show("File opti belum sesuai template");
                    }
                    con.Close();
                }
                loadForm.CloseForm();
            }
        }
        private void inputDataPkt()
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.excelFilter;
            if (of.ShowDialog() == DialogResult.OK)
            {
                loadForm.ShowSplashScreen();
                DataSet ds = Util.openExcel(of.FileName);
                //for (int i = 1; i < ds.Tables[0].Rows.Count; i++)
                //{
                //    Database1Entities db = new Database1Entities();
                //    Console.WriteLine(ds.Tables[0].Rows[i][0].ToString());
                //    if (ds.Tables[0].Rows[i][0].ToString().Substring(0,1) == "B")
                //        continue;
                //    String temp = ds.Tables[0].Rows[i][0].ToString();
                //    var newC = (from x in db.Cashpoints where x.idCashpoint == temp select x).FirstOrDefault();
                //    if (newC == null)
                //    {
                //        newC = new Cashpoint();
                //        newC.idCashpoint = ds.Tables[0].Rows[i][0].ToString();
                //        newC.kodePkt = ds.Tables[0].Rows[i][5].ToString();
                //        Console.WriteLine(newC.idCashpoint);
                //        Console.WriteLine(newC.kodePkt);
                //        db.Cashpoints.Add(newC);
                //    }
                //    else
                //    {
                //        newC.kodePkt = ds.Tables[0].Rows[i][5].ToString();
                //    }
                //    db.SaveChanges();
                //}
                DataTable dt = ds.Tables[0];
                DataRow[] rows = ds.Tables[0].Select("Column0 not like 'A%' OR LEN(Column0) > 5");
                foreach (DataRow row in rows)
                {
                    dt.Rows.Remove(row);
                }
                List<DataRow> listRow = new List<DataRow>();
                for(int a=0;a<dt.Rows.Count;a++)
                {
                    int buf;
                    if(Int32.TryParse(dt.Rows[a][4].ToString(), out buf))
                    {
                        listRow.Add(dt.Rows[a]);
                    }
                }
                foreach(DataRow row in listRow)
                {
                    dt.Rows.Remove(row);
                }
                UpdateDataCashpointPkt(dt);
                loadForm.CloseForm();
            }
        }
        private void inputDataDenom()
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.excelFilter;
            if (of.ShowDialog() == DialogResult.OK)
            {
                //Database1Entities db = new Database1Entities();
                DataSet ds = Util.openExcel(of.FileName);
                loadForm.ShowSplashScreen();

                //for (int i = 1; i < ds.Tables[0].Rows.Count; i++)
                //{
                //    Console.WriteLine(i);
                //    String idCashpoint = ds.Tables[0].Rows[i][0].ToString();
                //    Cashpoint newC = (from x in db.Cashpoints where x.idCashpoint == idCashpoint select x).FirstOrDefault();
                //    if(newC!=null)
                //    {
                //        Console.WriteLine(ds.Tables[0].Rows[i][6]);
                //        String denom = ds.Tables[0].Rows[i][6].ToString();
                //        newC.denom = denom;
                //    }
                //    else
                //    {
                //        MessageBox.Show("Data PKT Cashpoint " + ds.Tables[0].Rows[i][0].ToString() + " Tidak ada");
                //        newC = new Cashpoint();
                //        newC.idCashpoint = ds.Tables[0].Rows[i][0].ToString();
                //        db.Cashpoints.Add(newC);
                //    }
                //    db.SaveChanges();
                //}
                UpdateDataCashpointDenom(ds.Tables[0]);
                loadForm.CloseForm();
            }
            else
            {

            }
        }
        public static void UpdateDataCashpointPkt(DataTable dt)
        {
            using (SqlConnection conn = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand command = new SqlCommand("", conn))
                {
                    try
                    {
                        conn.Open();

                        //Creating temp table on database
                        command.CommandText = "CREATE TABLE #TempTable(idCashpoint VARCHAR(255), kodePkt VARCHAR(255))";
                        command.ExecuteNonQuery();

                        //Bulk insert into temp table
                        using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn))
                        {
                            //bulkcopy.BulkCopyTimeout = 660;
                            bulkcopy.DestinationTableName = "#TempTable";
                            bulkcopy.ColumnMappings.Add(0, 0);
                            bulkcopy.ColumnMappings.Add(4, 1);
                            bulkcopy.WriteToServer(dt);
                            bulkcopy.Close();
                        }

                        // Updating destination table, and dropping temp table
                        command.CommandTimeout = 300;
                        //Filtering data di temptable
                        command.CommandText = "DELETE FROM #TempTable WHERE kodePkt = 'AMRT' OR kodePkt = 'TAGT'";
                        command.ExecuteNonQuery();
                        command.CommandText = "UPDATE #TempTable SET kodePkt = 'AMRT' WHERE kodePkt = 'AMRT2'";
                        command.ExecuteNonQuery();
                        command.CommandText = "UPDATE #TempTable SET kodePkt = 'TAGT' WHERE kodePkt = 'TAGT2'";
                        command.ExecuteNonQuery();
                        command.CommandText = "UPDATE #TempTable SET kodePkt = 'ACBB' WHERE kodePkt = 'ACBD'";
                        command.ExecuteNonQuery();
                        command.CommandText = "UPDATE #TempTable SET kodePkt = 'ACMS' WHERE kodePkt = 'ACCM'";
                        command.ExecuteNonQuery();
                        command.CommandText = "UPDATE #TempTable SET kodePkt = 'ACKI' WHERE kodePkt = 'ACKD'";
                        command.ExecuteNonQuery();
                        command.CommandText = "UPDATE #TempTable SET kodePkt = 'ACMG' WHERE kodePkt = 'ACML'";
                        command.ExecuteNonQuery();
                        command.CommandText = "UPDATE #TempTable SET kodePkt = 'ANDB' WHERE kodePkt = 'ANBD'";
                        command.ExecuteNonQuery();
                        command.CommandText = "UPDATE #TempTable SET kodePkt = 'ANDJ' WHERE kodePkt = 'ANJK'";
                        command.ExecuteNonQuery();

                        command.CommandText = "INSERT INTO Cashpoint(idCashpoint, kodePkt) SELECT TT.idCashpoint, TT.kodePkt FROM #TempTable TT LEFT JOIN Cashpoint C ON TT.idCashpoint = C.idCashpoint WHERE C.idCashpoint IS  NULL";
                        command.ExecuteNonQuery();
                        command.CommandText = "UPDATE T SET T.kodePkt = TT.kodePkt FROM Cashpoint T INNER JOIN #TempTable TT ON TT.idCashpoint = T.idCashpoint; DROP TABLE #TempTable;";
                        command.ExecuteNonQuery();
                        
                    }
                    catch (Exception ex)
                    {
                        // Handle exception properly
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }
        public static void UpdateDataCashpointDenom(DataTable dt)
        {
            using (SqlConnection conn = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand command = new SqlCommand("", conn))
                {
                    try
                    {
                        conn.Open();

                        //Creating temp table on database
                        command.CommandText = "CREATE TABLE #TempTable(idCashpoint VARCHAR(255), denom VARCHAR(255))";
                        command.ExecuteNonQuery();

                        dt.Rows.RemoveAt(0);
                        //Bulk insert into temp table
                        using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn))
                        {
                            //bulkcopy.BulkCopyTimeout = 660;
                            bulkcopy.DestinationTableName = "#TempTable";
                            bulkcopy.ColumnMappings.Add(0, 0);
                            bulkcopy.ColumnMappings.Add(6, 1);
                            bulkcopy.WriteToServer(dt);
                            bulkcopy.Close();
                        }

                        // Updating destination table, and dropping temp table
                        command.CommandTimeout = 300;
                        command.CommandText = "INSERT INTO Cashpoint(idCashpoint, denom) SELECT TT.idCashpoint, TT.denom FROM #TempTable TT LEFT JOIN Cashpoint C ON TT.idCashpoint = C.idCashpoint WHERE C.idCashpoint IS  NULL";
                        command.ExecuteNonQuery();
                        command.CommandText = "UPDATE T SET T.denom = TT.denom FROM Cashpoint T INNER JOIN #TempTable TT ON TT.idCashpoint = T.idCashpoint; DROP TABLE #TempTable;";
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Handle exception properly
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        private void insertDataCabangToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            if(of.ShowDialog() == DialogResult.OK)
            {
                insertDataPktCabang(of.FileName);
            }
        }

        private void insertDataPktCabang(String filename)
        {
            using (var csv = new CsvReader(new StreamReader(filename), true))
            {
                using (var sbc = new SqlBulkCopy(Variables.connectionString))
                {
                    sbc.DestinationTableName = "dbo.Cabang";
                    sbc.BatchSize = 1000;

                    sbc.AddColumnMapping(0, 0);
                    sbc.AddColumnMapping(4, 1);
                    sbc.WriteToServer(csv);
                }
            }
            using(SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    sql.Open();
                    cmd.Connection = sql;
                    cmd.CommandText = "UPDATE CABANG SET kodeCabang = RIGHT(kodeCabang,4)";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE CABANG SET kodeCabang = IIF(kodeCabang like '000%',RIGHT(kodeCabang,1),IIF(kodeCabang like '00%', RIGHT(kodeCabang,2), IIF(kodeCabang like '0%', RIGHT(kodeCabang,3),kodeCabang)))";
                    cmd.ExecuteNonQuery();
                    sql.Close();
                }
            } 
            
        }

        private void insertDataKanwilCabangToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            if (of.ShowDialog() == DialogResult.OK)
            {
                insertDataKanwilCabang(of.FileName);
            }
        }
        private void insertDataKanwilCabang(String filepath)
        {

            DataSet ds = Util.openExcel(filepath);
            DataTable dt = ds.Tables[0];
            DataRow[] rows = dt.Select("Column3 not like 'JABO%'");


            foreach(var row in rows)
            {
                dt.Rows.Remove(row);
            }
            Console.WriteLine(dt.Rows[0][0].ToString());
            Console.WriteLine(dt.Rows[0][1].ToString());
            Console.WriteLine(dt.Rows[0][2].ToString());
            using (SqlConnection conn = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand command = new SqlCommand("", conn))
                {
                    try
                    {
                        conn.Open();

                        //Creating temp table on database
                        command.CommandText = "CREATE TABLE #TempTable(kodeCabang VARCHAR(255), kanwil VARCHAR(255))";
                        command.ExecuteNonQuery();

                        //Bulk insert into temp table
                        using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn))
                        {
                            //bulkcopy.BulkCopyTimeout = 660;
                            bulkcopy.DestinationTableName = "#TempTable";
                            bulkcopy.ColumnMappings.Add(0, 0);
                            bulkcopy.ColumnMappings.Add(2, 1);
                            bulkcopy.WriteToServer(dt);
                            bulkcopy.Close();
                        }

                        // Updating destination table, and dropping temp table
                        command.CommandTimeout = 300;
                        command.CommandText = "INSERT INTO Cabang(kodeCabang, kanwil) SELECT TT.kodeCabang, TT.kanwil FROM #TempTable TT LEFT JOIN Cabang C ON TT.kodeCabang = C.kodeCabang WHERE C.kodeCabang IS  NULL";
                        command.ExecuteNonQuery();
                        command.CommandText = "UPDATE T SET T.kanwil = TT.kanwil FROM Cabang T INNER JOIN #TempTable TT ON TT.kodeCabang = T.kodeCabang; DROP TABLE #TempTable;";
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Handle exception properly
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        private void inputTransaksiCabangToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InputTransaksiCabangForm itcf = new InputTransaksiCabangForm();
            itcf.MdiParent = this;
            itcf.Show();
        }

        private void initTransToolStripMenuItem_Click(object sender, EventArgs e)
        {
            initTransaksi it = new initTransaksi();
            it.MdiParent = this;
            it.Show();
            
        }

        private void inputDataKalenderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.excelFilter;
            if(of.ShowDialog() == DialogResult.OK)
            {
                DataSet ds = Util.openExcel(of.FileName);
                DataTable dt = ds.Tables[2];
                dt.Rows.RemoveAt(0);
                using (SqlConnection sql = new SqlConnection(Variables.connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        sql.Open();
                        cmd.Connection = sql;
                        cmd.CommandText = "DELETE FROM dbo.EventTanggal";
                        cmd.ExecuteNonQuery();

                        
                        using (SqlBulkCopy sbc = new SqlBulkCopy(sql))
                        {
                            sbc.DestinationTableName = "dbo.EventTanggal";
                            sbc.ColumnMappings.Add(0, 0);
                            sbc.ColumnMappings.Add(1, 1);
                            sbc.ColumnMappings.Add(2, 2);
                            sbc.WriteToServer(dt);
                            sbc.Close();
                        }
                        sql.Close();
                    }
                }
               
            }
        }

        private void loadMasterPKTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadPkt idp = new loadPkt();
            idp.MdiParent = this;
            idp.Show();
        }

        private void rekapApprovalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AkurasiPrediksiForm ra = new AkurasiPrediksiForm();
            ra.MdiParent = this;
            ra.Show();
        }

        private void dashboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadForm.ShowSplashScreen();
            dasbor d = new dasbor();
            loadForm.CloseForm();
            d.MdiParent = this;
            d.WindowState = FormWindowState.Maximized;
            d.Show();
        }

        private void dailyStockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DailyStockForm dsf = new DailyStockForm();
            dsf.MdiParent = this;
            dsf.Show();

        }

        private void rekapApprovalToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RekapApprovalForm raf = new RekapApprovalForm();
            raf.MdiParent = this;
            raf.Show();
        }

        private void stokPosisiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StokPosisiForm spf = new StokPosisiForm();
            spf.MdiParent = this;
            spf.Show();
        }
    }
}
public static class SqlBulkCopyExtensions
{
    public static SqlBulkCopyColumnMapping AddColumnMapping(this SqlBulkCopy sbc, int sourceColumnOrdinal, int targetColumnOrdinal)
    {
        var map = new SqlBulkCopyColumnMapping(sourceColumnOrdinal, targetColumnOrdinal);
        sbc.ColumnMappings.Add(map);

        return map;
    }

    public static SqlBulkCopyColumnMapping AddColumnMapping(this SqlBulkCopy sbc, string sourceColumn, string targetColumn)
    {
        var map = new SqlBulkCopyColumnMapping(sourceColumn, targetColumn);
        sbc.ColumnMappings.Add(map);

        return map;
    }
}
static class DateTimeExtensions
{
    static GregorianCalendar _gc = new GregorianCalendar();
    public static int GetWeekOfMonth(this DateTime time)
    {
        DateTime first = new DateTime(time.Year, time.Month, 1);
        return time.GetWeekOfYear() - first.GetWeekOfYear() + 1;
    }

    static int GetWeekOfYear(this DateTime time)
    {
        return _gc.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
    }
}