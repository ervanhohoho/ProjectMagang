using FastMember;
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
    public partial class RevisiInformationBoard : Form
    {
        List<String> KodePkt;
        int jumlahBonLaporan;

        Denom saldoAwal = new Denom();
        List<Denom> saldo = new List<Denom>();
        
        List<Denom> sislokCrm = new List<Denom>();
        List<Denom> sislokCrmDenganStdDeviasi = new List<Denom>();
        List<Denom> sislokCdm = new List<Denom>();
        List<Denom> sislokCdmDenganStdDeviasi = new List<Denom>();
        List<Rasio> rasioSislokAtm = new List<Rasio>();
        List<Rasio> rasioSislokATMDenganStdDeviasi = new List<Rasio>();

        List<Denom> isiCrm = new List<Denom>();
        List<Denom> isiCrmDenganStdDeviasi = new List<Denom>();

        List<Denom> bon = new List<Denom>();
        List<Denom> rekomendasiBon = new List<Denom>();

        Denom rekomendasiAdhoc = new Denom();
        List<Denom> setor = new List<Denom>();

        List<Denom> saldoAwalIdeal = new List<Denom>();

        List<Denom> prediksiIsiAtmOpti;
        List<Denom> prediksiIsiAtm;//dalem class denom ada 100,50,20
        List<Denom> prediksiIsiAtmDenganStdDeviasi = new List<Denom>();
        List<Rasio> listRasio = new List<Rasio>();

        List<DateTime> tanggalSkip;
        int pktIndex;
        DateTime tanggalOptiMin ,tanggalOptiMax, tanggalKalenderMax; 
        /**
            Opti itu datanya pasti selalu H, H+1,...
            Belom tentu semua pkt ada data opti
        **/
        public RevisiInformationBoard()
        {
            InitializeComponent();
            pktComboBox.MouseWheel += new MouseEventHandler(pktComboBox_MouseWheel);
            pktIndex = 0;
            loadComboBox();
            //Load Tree
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    cmd.CommandText = "SELECT MIN(Tanggal), MAX(Tanggal) FROM TransaksiAtms";
                    sql.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        DateTime minTanggal = (DateTime)reader[0];
                        DateTime maxTanggal = (DateTime)reader[1];
                        Console.WriteLine("Min Date: " + minTanggal.ToShortDateString());
                        Console.WriteLine("Max Date: " + maxTanggal.ToShortDateString());

                        DateTime tempTanggal = new DateTime(minTanggal.Year, minTanggal.Month,1) ;
                        int counter = 0;
                        bool firstRun = true;
                        while(tempTanggal<=maxTanggal)
                        {
                            treeView1.Nodes.Add(tempTanggal.Year.ToString());
                            int monthCounter;
                            if (firstRun)
                            {
                                monthCounter = minTanggal.Month;
                                firstRun = false;
                            }
                            else
                                monthCounter = 1;
                            while(tempTanggal<=maxTanggal && monthCounter <= 12)
                            {
                                Console.WriteLine(monthCounter);
                                treeView1.Nodes[counter].Nodes.Add((monthCounter++).ToString());
                                tempTanggal = tempTanggal.AddMonths(1);
                                Console.WriteLine("Temp Tanggal: " + tempTanggal.ToShortDateString());
                            }
                            counter++;
                        }
                    }
                }
            }
            MessageBox.Show(KodePkt.Count.ToString());
            //dataGridView1.Columns.Add("Tanggal", "Tanggal");
            //dataGridView1.Columns.Add("100000", "100000");
            //dataGridView1.Columns.Add("50000", "50000");
            //dataGridView1.Columns.Add("20000", "20000");
            //for (int i = 1; i < 4; i++)
            //{
            //    dataGridView1.Columns[i].DefaultCellStyle.Format = "c";
            //    dataGridView1.Columns[i].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            //}
            if(KodePkt.Count<=0)
                MessageBox.Show("Data Laporan Belum Ada Yang Approved");

            pktIndex = 0;
            loadE2E();
            MetodePrediksiComboBox.SelectedIndex = 0;
            MetodeHitungLainnyaComboBox.SelectedIndex = 0;
            /*tanggalOpti = (DateTime) query[0].tanggal*/;
            //Console.WriteLine(query[0]);
            //MessageBox.Show(tanggalOpti.ToShortDateString());
            tanggalPrediksiMaxPicker.MinDate = DateTime.Today.AddDays(2);
        }
        void loadComboBox()
        {
            using (Database1Entities db = new Database1Entities())
            {
                DateTime harikemarin = DateTime.Today.Date.AddDays(-1);
                List<String> tempListKodePkt = (from x in db.Approvals
                                                where x.tanggal == DateTime.Today.Date
                                                orderby x.kodePkt
                                                select x.kodePkt).Distinct().ToList();

                KodePkt = tempListKodePkt;
                
                pktComboBox.DataSource = KodePkt;
                tanggalKalenderMax = (from x in db.EventTanggals select x).Max(x => x.tanggal);
            }
        }
        private void pktComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            pktIndex = pktComboBox.SelectedIndex;
            loadE2E();
            Database1Entities db = new Database1Entities();
        }
        private void loadPrediksiOpti()
        {
            //dataGridView1.Rows.Clear();
            //dataGridView1.Refresh();
            
            //init
            prediksiIsiAtmOpti = new List<Denom>();
            
            using (SqlConnection sqlConnection1 = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    SqlDataReader reader;

                    cmd.CommandText = "SELECT COUNT (*) FROM Opti o " +
                                      "JOIN Cashpoint c ON o.idCashpoint = c.idCashpoint " +
                                      "WHERE kodePkt = '" + KodePkt[pktIndex] + "'";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = sqlConnection1;

                    sqlConnection1.Open();

                    reader = cmd.ExecuteReader();
                    reader.Read();
                    if ((int)reader[0] == 0)
                    {

                    }
                    else
                    {
                        reader.Close();
                        cmd.CommandText = "SELECT MIN(tanggal), MAX(tanggal) FROM Opti o JOIN Cashpoint c on o.idCashpoint = c.idCashpoint GROUP BY kodePkt ";
                        reader = cmd.ExecuteReader();
                        reader.Read();
                        DateTime minDate = (DateTime)reader[0];
                        DateTime maxDate = (DateTime)reader[1];
                        reader.Close();
                        while(minDate<=maxDate)
                        {
                            DataGridViewRow row = new DataGridViewRow();
                            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                            cell.Value = minDate.ToShortDateString();
                            row.Cells.Add(cell);

                            Denom tempPrediksi = new Denom();
                            tempPrediksi.tgl = minDate;
                            //Denom 100
                            cell = new DataGridViewTextBoxCell();
                            cmd.CommandText = "SELECT SUM(prediksi) " +
                                              "FROM Opti o " +
                                              "JOIN Cashpoint c on o.idCashpoint = c.idCashpoint " +
                                              "WHERE kodePkt = '" + KodePkt[pktIndex] + "' AND tanggal = '" + minDate.ToShortDateString() + "' AND denom = '100000' " +
                                              "GROUP BY kodePkt";
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            { cell.Value = reader[0]; tempPrediksi.d100 = (Int64)reader[0]; }
                            else
                            { cell.Value = 0; tempPrediksi.d100 = 0; }
                            row.Cells.Add(cell);
                            reader.Close();

                            //Denom 50
                            cell = new DataGridViewTextBoxCell();
                            cmd.CommandText = "SELECT SUM(prediksi) " +
                                              "FROM Opti o " +
                                              "JOIN Cashpoint c on o.idCashpoint = c.idCashpoint " +
                                              "WHERE kodePkt = '" + KodePkt[pktIndex] + "' AND tanggal = '" + minDate.ToShortDateString() + "' AND denom = '50000' " +
                                              "GROUP BY kodePkt";
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            { cell.Value = reader[0]; tempPrediksi.d50 = ((Int64)reader[0]); }
                            else
                            { cell.Value = 0; tempPrediksi.d50 = (0); }
                            row.Cells.Add(cell);
                            reader.Close();

                            //Denom 20
                            cell = new DataGridViewTextBoxCell();
                            cmd.CommandText = "SELECT SUM(prediksi) " +
                                              "FROM Opti o " +
                                              "JOIN Cashpoint c on o.idCashpoint = c.idCashpoint " +
                                              "WHERE kodePkt = '" + KodePkt[pktIndex] + "' AND tanggal = '" + minDate.ToShortDateString() + "' AND denom = '20000' " +
                                              "GROUP BY kodePkt";
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            { cell.Value = reader[0]; tempPrediksi.d20 = ((Int64)reader[0]); }
                            else
                            { cell.Value = 0; tempPrediksi.d20 = (0); }
                            row.Cells.Add(cell);

                            prediksiIsiAtmOpti.Add(tempPrediksi);
                            reader.Close();

                            minDate = minDate.AddDays(1);
                            Console.WriteLine(minDate);
                            //dataGridView1.Rows.Add(row);
                        }
                    }
                   
                    sqlConnection1.Close();
                }
                
            }
            // Data is accessible through the DataReader object here.
            Console.WriteLine("Prediksi isi ATM Opti");
            Console.WriteLine("===================");
            foreach(var temp in prediksiIsiAtmOpti)
            {
                Console.WriteLine(temp.tgl + " " + temp.d100 + " " + temp.d50 + " " + temp.d20);
            }
        }
        void loadE2E()
        {
            Database1Entities db = new Database1Entities();
            if (KodePkt.Count <= 0)
                return;
            String tipe = (from x in db.Pkts.AsEnumerable() where x.kodePkt == KodePkt[pktIndex] select x.e2e).FirstOrDefault();
            e2eTxt.Text = "Jenis E2E: " + tipe;
            if (tipe == "E2E")
                e2eComboBox.SelectedIndex = 0;
            else
                e2eComboBox.SelectedIndex = 1;
        }
        void loadTablePermintaanBon()
        {
            Database1Entities db = new Database1Entities();
            permintaanBonGridView.AllowUserToAddRows = false;
            permintaanBonGridView.ReadOnly = true;
            permintaanBonGridView.Rows.Clear(); permintaanBonGridView.Columns.Clear();
            String kodePkt = KodePkt[pktIndex];
            List<LaporanPermintaanBon> laporanPermintaanBon = (from x in db.LaporanPermintaanBons where x.kodePkt == kodePkt && x.tanggal > tanggalOptiMin select x).Distinct().ToList();

            DateTime tanggalKemaren = tanggalOptiMin.AddDays(-1);
            //Delete data yang udah ada di approval
            var detailApprovals = (from x in db.laporanBons where x.kodePkt == kodePkt && x.tanggal > tanggalOptiMin select x).ToList();
            for (int a = 0; a < detailApprovals.Count - 1; a++)
                if (laporanPermintaanBon.Any())
                    laporanPermintaanBon.RemoveAt(0);

            permintaanBonGridView.Columns.Add("Tanggal", "Tanggal");
            permintaanBonGridView.Columns.Add("100", "100");
            permintaanBonGridView.Columns.Add("50", "50");
            permintaanBonGridView.Columns.Add("20", "20");
            for (int i = 1; i < 4; i++)
            {
                permintaanBonGridView.Columns[i].DefaultCellStyle.Format = "c";
                permintaanBonGridView.Columns[i].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            }
            foreach (var temp in laporanPermintaanBon)
            {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = ((DateTime)temp.tanggal).ToShortDateString();
                row.Cells.Add(cell);
                cell = new DataGridViewTextBoxCell();
                cell.Value = temp.C100;
                row.Cells.Add(cell);
                cell = new DataGridViewTextBoxCell();
                cell.Value = temp.C50;
                row.Cells.Add(cell);
                cell = new DataGridViewTextBoxCell();
                cell.Value = temp.C20;
                row.Cells.Add(cell);
                permintaanBonGridView.Rows.Add(row);
            }
        }
        void loadSaldoAwal()
        {
            saldoAwal = new Denom();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    cmd.CommandText = "SELECT saldoAkhir100, saldoAkhir50, saldoAkhir20, tanggal FROM TransaksiAtms WHERE kodePkt = '" + KodePkt[pktIndex] + "' AND tanggal >= '" + tanggalOptiMin.AddDays(-1).ToShortDateString() + "'";
                    sql.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        saldoAwal.d100 = (Int64)reader[0];
                        saldoAwal.d50 = (Int64)reader[1];
                        saldoAwal.d20 = (Int64)reader[2];
                        saldoAwal.tgl = Convert.ToDateTime(reader[3].ToString());
                    }
                    saldoAwal.tgl = saldoAwal.tgl.AddDays(1);
                    Console.WriteLine("Saldo Awal");
                    Console.WriteLine(saldoAwal.d100 + " " + saldoAwal.d50 + " " + saldoAwal.d20);
                }
            }

        }
        void loadIsiAtmHistoris()
        {
            Console.WriteLine();
            Console.WriteLine("Load Isi ATM");
            Console.WriteLine("======================");
            isiCrm = new List<Denom>();
            //dataGridView1.Hide();
            DateTime startDate = tanggalOptiMin;//Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = tanggalOptiMax;//Convert.ToDateTime(dataGridView1.Rows[rowCount - 1].Cells[0].Value);
            DateTime tempDate = startDate;
            Console.WriteLine(startDate.DayOfWeek.ToString());
            Console.WriteLine(endDate);

            prediksiIsiAtm = new List<Denom>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    while (tempDate <= endDate)
                    {
                        //SislokCrm
                        SqlDataReader reader;
                        Denom tempIsiAtm = new Denom();
                        String cText = "SELECT AVG(isiAtm100), AVG(isiAtm50), AVG(isiAtm20) FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE kodePkt = '" + KodePkt[pktIndex] + "' AND DATENAME(WEEKDAY, TA.Tanggal) = '" + tempDate.DayOfWeek.ToString() + "'";
                        int count = 0;
                        for (int i = 0; i < treeView1.Nodes.Count; i++)
                        {
                            for (int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++)
                            {
                                if (treeView1.Nodes[i].Nodes[j].Checked)
                                {
                                    //;
                                    //Kalo cari minggu (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = (minggu keberapa)
                                    if (count++ == 0)
                                        cText += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                    else
                                        cText += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                }
                            }
                        }
                        cText += ")";
                        cmd.CommandText = cText;
                        reader = cmd.ExecuteReader();
                        bool event1 = true;
                        if (reader.Read())
                        {
                            if (String.IsNullOrEmpty(reader[0].ToString()) || String.IsNullOrEmpty(reader[1].ToString()) || String.IsNullOrEmpty(reader[2].ToString()))
                            {
                                event1 = false;
                            }
                            else
                            {
                                tempIsiAtm.d50 = (Int64)reader[1];
                                tempIsiAtm.d20 = (Int64)reader[2];
                                tempIsiAtm.tgl = tempDate;
                                tempIsiAtm.d100 = (Int64)reader[0];
                            }
                            reader.Close();
                        }
                        if (!event1)
                        {
                            reader.Close();
                            cText = "SELECT AVG(isiAtm100), AVG(isiAtm50), AVG(isiAtm20) FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE kodePkt = '" + KodePkt[pktIndex] + "' ";
                            count = 0;
                            for (int i = 0; i < treeView1.Nodes.Count; i++)
                            {
                                for (int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++)
                                {
                                    if (treeView1.Nodes[i].Nodes[j].Checked)
                                    {
                                        //;
                                        //Kalo cari minggu (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = (minggu keberapa)
                                        if (count++ == 0)
                                            cText += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                        else
                                            cText += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                    }
                                }
                            }
                            cText += ")";
                            cmd.CommandText = cText;
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                if (String.IsNullOrEmpty(reader[0].ToString()) || String.IsNullOrEmpty(reader[1].ToString()) || String.IsNullOrEmpty(reader[2].ToString()))
                                {
                                    tempIsiAtm.d50 = 0;
                                    tempIsiAtm.d20 = 0;
                                    tempIsiAtm.tgl = tempDate;
                                    tempIsiAtm.d100 = 0;
                                }
                                else
                                {
                                    tempIsiAtm.d50 = (Int64)reader[1];
                                    tempIsiAtm.d20 = (Int64)reader[2];
                                    tempIsiAtm.tgl = tempDate;
                                    tempIsiAtm.d100 = (Int64)reader[0];
                                }
                                reader.Close();
                            }
                        }//Console.WriteLine(tempSislokCrm.d100.ToString());
                        prediksiIsiAtm.Add(tempIsiAtm);
                        tempDate = tempDate.AddDays(1);
                    }
                }
                sql.Close();
                Console.WriteLine("Isi ATM");
                Console.WriteLine(prediksiIsiAtm.Count);
                foreach (var temp in prediksiIsiAtm)
                {
                    Console.WriteLine(temp.tgl + " " + temp.d100 + " " + temp.d50 + " " + temp.d20 + " ");
                }
            }
        }
        void loadIsiAtmHistorisDenganStandarDeviasi()
        {

            Console.WriteLine();
            Console.WriteLine("Load Deviasi Isi Atm");
            Console.WriteLine("======================");

            int rowcount = prediksiIsiAtmOpti.Count;//dataGridView1.Rows.Count;

            //dataGridView1.Hide();
            DateTime startDate = tanggalOptiMin;//Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = tanggalOptiMax;
            DateTime tempDate = startDate;
            Console.WriteLine(startDate.DayOfWeek.ToString());
            Console.WriteLine(endDate);

            //Load Std Deviasi
            List<Rasio> stdDeviasi = new List<Rasio>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    while (tempDate <= endDate)
                    {
                        //SislokAtm
                        SqlDataReader reader;
                        Rasio tempStdDeviasi = new Rasio();

                        String kondisi = " WHERE kodePkt = '" + KodePkt[pktIndex] + "' AND DATENAME(WEEKDAY, TA.Tanggal) = '" + tempDate.DayOfWeek.ToString() + "'";
                        int count = 0;

                        for (int i = 0; i < treeView1.Nodes.Count; i++)
                        {
                            for (int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++)
                            {
                                if (treeView1.Nodes[i].Nodes[j].Checked)
                                {
                                    //;
                                    //Kalo cari minggu (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = (minggu keberapa)
                                    if (count++ == 0)
                                        kondisi += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                    else
                                        kondisi += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                }
                            }
                        }
                        kondisi += ")";

                        String subqueryTblAverage = "(SELECT ISNULL(AVG(isiAtm100),0) AS Average100 , ISNULL(AVG(isiAtm50),0) AS Average50 , ISNULL(AVG(isiAtm20),0) AS Average20 FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal";
                        subqueryTblAverage += kondisi;
                        subqueryTblAverage += ") avt";

                        String query = "SELECT "
                                    + "[AverageStdDeviasi100] = AVG(CAST(ABS(isiAtm100 - [Average100]) AS FLOAT) / (CASE WHEN [Average100] = 0 THEN 1 ELSE [Average100] END)), "
                                    + "[AverageStdDeviasi50] = AVG(CAST(ABS(isiAtm50 - [Average50])AS FLOAT) / (CASE WHEN [Average50] = 0 THEN 1 ELSE [Average50] END)), "
                                    + "[AverageStdDeviasi20] = AVG(CAST(ABS(isiAtm20 - [Average20])AS FLOAT) / (CASE WHEN [Average20] = 0 THEN 1 ELSE [Average20] END)) "
                                    + "FROM TransaksiAtms TA JOIN EventTanggal ET ON Ta.tanggal = ET.tanggal, " + subqueryTblAverage + kondisi;

                        cmd.CommandText = query;
                        reader = cmd.ExecuteReader();
                        bool event1 = true;
                        if (reader.Read())
                        {
                            if (String.IsNullOrEmpty(reader[0].ToString()) || String.IsNullOrEmpty(reader[1].ToString()) || String.IsNullOrEmpty(reader[2].ToString()))
                                event1 = false;
                            else
                            {
                                tempStdDeviasi.d100 = (Double)reader[0];
                                tempStdDeviasi.d50 = (Double)reader[1];
                                tempStdDeviasi.d20 = (Double)reader[2];
                                tempStdDeviasi.tgl = tempDate;
                            }
                            reader.Close();
                        }
                        if (!event1)
                        {
                            reader.Close();
                            kondisi = " WHERE kodePkt = '" + KodePkt[pktIndex] + "' ";
                            count = 0;

                            for (int i = 0; i < treeView1.Nodes.Count; i++)
                            {
                                for (int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++)
                                {
                                    if (treeView1.Nodes[i].Nodes[j].Checked)
                                    {
                                        //;
                                        //Kalo cari minggu (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = (minggu keberapa)
                                        if (count++ == 0)
                                            kondisi += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                        else
                                            kondisi += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                    }
                                }
                            }
                            kondisi += ")";

                            subqueryTblAverage = "(SELECT ISNULL(AVG(isiAtm100),0) AS Average100 , ISNULL(AVG(isiAtm50),0) AS Average50 , ISNULL(AVG(isiAtm20),0) AS Average20 FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal";
                            subqueryTblAverage += kondisi;
                            subqueryTblAverage += ") avt";

                            query = "SELECT "
                                        + "[AverageStdDeviasi100] = AVG(CAST(ABS(isiAtm100 - [Average100]) AS FLOAT) / (CASE WHEN [Average100] = 0 THEN 1 ELSE [Average100] END)), "
                                        + "[AverageStdDeviasi50] = AVG(CAST(ABS(isiAtm50 - [Average50])AS FLOAT) / (CASE WHEN [Average50] = 0 THEN 1 ELSE [Average50] END)), "
                                        + "[AverageStdDeviasi20] = AVG(CAST(ABS(isiAtm20 - [Average20])AS FLOAT) / (CASE WHEN [Average20] = 0 THEN 1 ELSE [Average20] END)) "
                                        + "FROM TransaksiAtms TA JOIN EventTanggal ET ON Ta.tanggal = ET.tanggal, " + subqueryTblAverage + kondisi;

                            cmd.CommandText = query;
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                tempStdDeviasi.d100 = (Double)reader[0];
                                tempStdDeviasi.d50 = (Double)reader[1];
                                tempStdDeviasi.d20 = (Double)reader[2];
                                tempStdDeviasi.tgl = tempDate;
                                reader.Close();
                            }
                        }
                        //Console.WriteLine(tempSislokAtm.d100.ToString());
                        stdDeviasi.Add(tempStdDeviasi);
                        tempDate = tempDate.AddDays(1);
                    }
                }
                sql.Close();
                Console.WriteLine("Deviasi Atm");
                foreach (var temp in stdDeviasi)
                {
                    Console.WriteLine(temp.tgl + " " + temp.d100 + " " + temp.d50 + " " + temp.d20 + " ");
                }
            }

            prediksiIsiAtmDenganStdDeviasi = new List<Denom>();
            for (int a = 0; a < prediksiIsiAtm.Count; a++)
            {
                Denom temp = new Denom();
                temp.d100 = prediksiIsiAtm[a].d100 + (Int64)Math.Round(stdDeviasi[a].d100 * prediksiIsiAtm[a].d100);
                temp.d50 = prediksiIsiAtm[a].d50 + (Int64)Math.Round(stdDeviasi[a].d50 * prediksiIsiAtm[a].d50);
                temp.d20 = prediksiIsiAtm[a].d20 + (Int64)Math.Round(stdDeviasi[a].d20 * prediksiIsiAtm[a].d20);
                temp.tgl = prediksiIsiAtm[a].tgl;
                prediksiIsiAtmDenganStdDeviasi.Add(temp);
            }

            Console.WriteLine("Isi Atm Dengan Std Deviasi");
            Console.WriteLine("===========================");
            foreach (var temp in prediksiIsiAtmDenganStdDeviasi)
            {
                Console.WriteLine(temp.tgl.ToShortDateString() + " " + temp.d100 + " " + temp.d50 + " " + temp.d20);
            }
        }
        void loadIsiCrm()
        {
            Console.WriteLine();
            Console.WriteLine("Load Isi CRM");
            Console.WriteLine("======================");
            isiCrm = new List<Denom>();
            int rowcount = prediksiIsiAtmOpti.Count;//dataGridView1.Rows.Count;
                                                    //dataGridView1.Hide();
            DateTime startDate = tanggalOptiMin;//Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = tanggalOptiMax;
            DateTime tempDate = startDate;
            Console.WriteLine(startDate.DayOfWeek.ToString());
            Console.WriteLine(endDate);

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    while (tempDate <= endDate)
                    {
                        //SislokCrm
                        SqlDataReader reader;
                        Denom tempIsiCrm = new Denom();
                        String cText = "SELECT AVG(isiCrm100), AVG(isiCrm50), AVG(isiCrm20) FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE kodePkt = '" + KodePkt[pktIndex] + "' AND DATENAME(WEEKDAY, TA.Tanggal) = '" + tempDate.DayOfWeek.ToString() + "'";
                        int count = 0;
                        for (int i = 0; i < treeView1.Nodes.Count; i++)
                        {
                            for (int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++)
                            {
                                if (treeView1.Nodes[i].Nodes[j].Checked)
                                {
                                    //;
                                    //Kalo cari minggu (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = (minggu keberapa)
                                    if (count++ == 0)
                                        cText += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                    else
                                        cText += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                }
                            }
                        }
                        cText += ")";
                        cmd.CommandText = cText;
                        reader = cmd.ExecuteReader();
                        bool event1 = true;
                        bool event2 = true;
                        bool event3 = true;
                        if (reader.Read())
                        {
                            if (String.IsNullOrEmpty(reader[0].ToString()) || String.IsNullOrEmpty(reader[1].ToString()) || String.IsNullOrEmpty(reader[2].ToString()))
                                event1 = false;
                            else
                            {
                                tempIsiCrm.d100 = (Int64)reader[0];
                                tempIsiCrm.d50 = (Int64)reader[1];
                                tempIsiCrm.d20 = (Int64)reader[2];
                                tempIsiCrm.tgl = tempDate;
                            }
                            reader.Close();
                        }
                        if (!event1)
                        {
                            reader.Close();
                            cText = "SELECT ISNULL(AVG(isiCrm100),0), ISNULL(AVG(isiCrm50),0), ISNULL(AVG(isiCrm20),0) FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE kodePkt = '" + KodePkt[pktIndex] + "' ";
                            count = 0;
                            for (int i = 0; i < treeView1.Nodes.Count; i++)
                            {
                                for (int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++)
                                {
                                    if (treeView1.Nodes[i].Nodes[j].Checked)
                                    {
                                        //;
                                        //Kalo cari minggu (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = (minggu keberapa)
                                        if (count++ == 0)
                                            cText += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                        else
                                            cText += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                    }
                                }
                            }
                            cText += ")";
                            cmd.CommandText = cText;
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                tempIsiCrm.d100 = (Int64)reader[0];
                                tempIsiCrm.d50 = (Int64)reader[1];
                                tempIsiCrm.d20 = (Int64)reader[2];
                                tempIsiCrm.tgl = tempDate;
                                reader.Close();
                            }
                        }
                        //Console.WriteLine(tempSislokCrm.d100.ToString());
                        isiCrm.Add(tempIsiCrm);
                        tempDate = tempDate.AddDays(1);
                    }
                }
                sql.Close();
                Console.WriteLine("Isi CRM");
                Console.WriteLine(isiCrm.Count);
                foreach (var temp in isiCrm)
                {
                    Console.WriteLine(temp.tgl + " " + temp.d100 + " " + temp.d50 + " " + temp.d20 + " ");
                }
            }
        }
        void loadIsiCrmDenganStdDeviasi()
        {
            Console.WriteLine();
            Console.WriteLine("Load Deviasi Isi CRM");
            Console.WriteLine("======================");

            int rowcount = prediksiIsiAtmOpti.Count;//dataGridView1.Rows.Count;

            //dataGridView1.Hide();
            DateTime startDate = tanggalOptiMin;//Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = tanggalOptiMax;
            DateTime tempDate = startDate;
            Console.WriteLine(startDate.DayOfWeek.ToString());
            Console.WriteLine(endDate);

            //Load Std Deviasi
            List<Rasio> stdDeviasi = new List<Rasio>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    while (tempDate <= endDate)
                    {
                        //SislokCrm
                        SqlDataReader reader;
                        Rasio tempStdDeviasi = new Rasio();

                        String kondisi = " WHERE kodePkt = '" + KodePkt[pktIndex] + "' AND DATENAME(WEEKDAY, TA.Tanggal) = '" + tempDate.DayOfWeek.ToString() + "'";
                        int count = 0;

                        for (int i = 0; i < treeView1.Nodes.Count; i++)
                        {
                            for (int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++)
                            {
                                if (treeView1.Nodes[i].Nodes[j].Checked)
                                {
                                    //;
                                    //Kalo cari minggu (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = (minggu keberapa)
                                    if (count++ == 0)
                                        kondisi += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                    else
                                        kondisi += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "'))";
                                }
                            }
                        }
                        kondisi += ")";

                        String subqueryTblAverage = "(SELECT AVG(isiCrm100) AS Average100 , AVG(isiCrm50) AS Average50 , AVG(isiCrm20) AS Average20 FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal";
                        subqueryTblAverage += kondisi;
                        subqueryTblAverage += ") avt";

                        String query = "SELECT "
                                    + "[AverageStdDeviasi100] = AVG(CAST(ABS(isiCrm100 - [Average100]) AS FLOAT) / (CASE WHEN [Average100] = 0 THEN 1 ELSE [Average100] END)), "
                                    + "[AverageStdDeviasi50] = AVG(CAST(ABS(isiCrm50 - [Average50])AS FLOAT) / (CASE WHEN [Average50] = 0 THEN 1 ELSE [Average50] END)), "
                                    + "[AverageStdDeviasi20] = AVG(CAST(ABS(isiCrm20 - [Average20])AS FLOAT) / (CASE WHEN [Average20] = 0 THEN 1 ELSE [Average20] END)) "
                                    + "FROM TransaksiAtms TA JOIN EventTanggal ET ON Ta.tanggal = ET.tanggal, " + subqueryTblAverage + kondisi;

                        cmd.CommandText = query;
                        bool event1 = true;
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            if (String.IsNullOrEmpty(reader[0].ToString()) || String.IsNullOrEmpty(reader[1].ToString()) || String.IsNullOrEmpty(reader[2].ToString()))
                                event1 = false;
                            else
                            {

                                tempStdDeviasi.d100 = (Double)reader[0];
                                tempStdDeviasi.d50 = (Double)reader[1];
                                tempStdDeviasi.d20 = (Double)reader[2];
                                tempStdDeviasi.tgl = tempDate;
                                reader.Close();
                            }
                        }
                        if (!event1)
                        {
                            reader.Close();
                            kondisi = " WHERE kodePkt = '" + KodePkt[pktIndex] + "' ";
                            count = 0;

                            for (int i = 0; i < treeView1.Nodes.Count; i++)
                            {
                                for (int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++)
                                {
                                    if (treeView1.Nodes[i].Nodes[j].Checked)
                                    {
                                        //;
                                        //Kalo cari minggu (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = (minggu keberapa)
                                        if (count++ == 0)
                                            kondisi += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                        else
                                            kondisi += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                    }
                                }
                            }
                            kondisi += ")";

                            subqueryTblAverage = "(SELECT ISNULL(AVG(isiCrm100),0) AS Average100 , ISNULL(AVG(isiCrm50),0) AS Average50 , ISNULL(AVG(isiCrm20),0) AS Average20 FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal";
                            subqueryTblAverage += kondisi;
                            subqueryTblAverage += ") avt";

                            query = "SELECT "
                                        + "[AverageStdDeviasi100] = AVG(CAST(ABS(isiCrm100 - [Average100]) AS FLOAT) / (CASE WHEN [Average100] = 0 THEN 1 ELSE [Average100] END)), "
                                        + "[AverageStdDeviasi50] = AVG(CAST(ABS(isiCrm50 - [Average50])AS FLOAT) / (CASE WHEN [Average50] = 0 THEN 1 ELSE [Average50] END)), "
                                        + "[AverageStdDeviasi20] = AVG(CAST(ABS(isiCrm20 - [Average20])AS FLOAT) / (CASE WHEN [Average20] = 0 THEN 1 ELSE [Average20] END)) "
                                        + "FROM TransaksiAtms TA JOIN EventTanggal ET ON Ta.tanggal = ET.tanggal, " + subqueryTblAverage + kondisi;

                            cmd.CommandText = query;
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                tempStdDeviasi.d100 = (Double)reader[0];
                                tempStdDeviasi.d50 = (Double)reader[1];
                                tempStdDeviasi.d20 = (Double)reader[2];
                                tempStdDeviasi.tgl = tempDate;
                                reader.Close();
                            }
                        }
                        //Console.WriteLine(tempSislokCrm.d100.ToString());
                        stdDeviasi.Add(tempStdDeviasi);
                        tempDate = tempDate.AddDays(1);
                    }
                }
                sql.Close();
                Console.WriteLine("Deviasi CRM");
                Console.WriteLine(isiCrm.Count);
                foreach (var temp in stdDeviasi)
                {
                    Console.WriteLine(temp.tgl + " " + temp.d100 + " " + temp.d50 + " " + temp.d20 + " ");
                }
            }

            isiCrmDenganStdDeviasi = new List<Denom>();
            for (int a = 0; a < isiCrm.Count; a++)
            {
                Denom temp = new Denom();
                temp.d100 = isiCrm[a].d100 + (Int64)Math.Round(stdDeviasi[a].d100 * isiCrm[a].d100);
                temp.d50 = isiCrm[a].d50 + (Int64)Math.Round(stdDeviasi[a].d50 * isiCrm[a].d50);
                temp.d20 = isiCrm[a].d20 + (Int64)Math.Round(stdDeviasi[a].d20 * isiCrm[a].d20);
                temp.tgl = isiCrm[a].tgl;
                isiCrmDenganStdDeviasi.Add(temp);
            }

            Console.WriteLine("Isi CRM Dengan Std Deviasi");
            Console.WriteLine("===========================");
            foreach (var temp in isiCrmDenganStdDeviasi)
            {
                Console.WriteLine(temp.tgl.ToShortDateString() + " " + temp.d100 + " " + temp.d50 + " " + temp.d20);
            }
        }
        private void loadSislokCrm()
        {
            Console.WriteLine();
            Console.WriteLine("Load Sislok CRM");
            Console.WriteLine("======================");
            sislokCrm = new List<Denom>();
            int rowcount = prediksiIsiAtmOpti.Count;//dataGridView1.Rows.Count;
                                                    //dataGridView1.Hide();
            DateTime startDate = tanggalOptiMin;//Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = tanggalOptiMax;
            DateTime tempDate = startDate;
            Console.WriteLine(startDate.DayOfWeek.ToString());
            Console.WriteLine(endDate);

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    while (tempDate <= endDate)
                    {
                        //SislokCrm
                        SqlDataReader reader;
                        Denom tempSislokCrm = new Denom();
                        String cText = "SELECT AVG(sislokCrm100), AVG(sislokCrm50), AVG(sislokCrm20) FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE kodePkt = '" + KodePkt[pktIndex] + "' AND DATENAME(WEEKDAY, TA.Tanggal) = '" + tempDate.DayOfWeek.ToString() + "'";
                        int count = 0;
                        for (int i = 0; i < treeView1.Nodes.Count; i++)
                        {
                            for (int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++)
                            {
                                if (treeView1.Nodes[i].Nodes[j].Checked)
                                {
                                    //;
                                    //Kalo cari minggu (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = (minggu keberapa)
                                    if (count++ == 0)
                                        cText += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                    else
                                        cText += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                }
                            }
                        }
                        cText += ")";
                        cmd.CommandText = cText;
                        reader = cmd.ExecuteReader();

                        bool flag = false;
                        bool event1 = true;
                        if (reader.Read())
                        {
                            if (String.IsNullOrEmpty(reader[0].ToString()) || String.IsNullOrEmpty(reader[1].ToString()) || String.IsNullOrEmpty(reader[2].ToString()))
                                event1 = false;
                            else
                            {
                                tempSislokCrm.d100 = (Int64)reader[0];
                                tempSislokCrm.d50 = (Int64)reader[1];
                                tempSislokCrm.d20 = (Int64)reader[2];
                                tempSislokCrm.tgl = tempDate;
                            }
                        }
                        if (!event1)
                        {
                            reader.Close();
                            //QUERY UNTUK CONDITION TANPA HARI
                            cText = "SELECT ISNULL(AVG(sislokCrm100),0), ISNULL(AVG(sislokCrm50),0), ISNULL(AVG(sislokCrm20),0) FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE kodePkt = '" + KodePkt[pktIndex] + "' ";
                            count = 0;
                            for (int i = 0; i < treeView1.Nodes.Count; i++)
                            {
                                for (int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++)
                                {
                                    if (treeView1.Nodes[i].Nodes[j].Checked)
                                    {
                                        //;
                                        //Kalo cari minggu (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = (minggu keberapa)
                                        if (count++ == 0)
                                            cText += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                        else
                                            cText += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                    }
                                }
                            }
                            cText += ")";
                            cmd.CommandText = cText;
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                tempSislokCrm.d100 = (Int64)reader[0];
                                tempSislokCrm.d50 = (Int64)reader[1];
                                tempSislokCrm.d20 = (Int64)reader[2];
                                tempSislokCrm.tgl = tempDate;
                            }
                        }
                        reader.Close();
                        //Console.WriteLine(tempSislokCrm.d100.ToString());
                        sislokCrm.Add(tempSislokCrm);
                        tempDate = tempDate.AddDays(1);
                    }
                }
                sql.Close();
                Console.WriteLine("Sislok CRM");
                Console.WriteLine(sislokCrm.Count);
                foreach (var temp in sislokCrm)
                {
                    Console.WriteLine(temp.tgl + " " + temp.d100 + " " + temp.d50 + " " + temp.d20);
                }
            }
        }
        void loadSislokCrmDenganStdDeviasi()
        {
            Console.WriteLine();
            Console.WriteLine("Load Deviasi Sislok CRM");
            Console.WriteLine("======================");

            int rowcount = prediksiIsiAtmOpti.Count;//dataGridView1.Rows.Count;

            //dataGridView1.Hide();
            DateTime startDate = tanggalOptiMin;//Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = tanggalOptiMax;
            DateTime tempDate = startDate;
            Console.WriteLine(startDate.DayOfWeek.ToString());
            Console.WriteLine(endDate);

            //Load Std Deviasi
            List<Rasio> stdDeviasi = new List<Rasio>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    while (tempDate <= endDate)
                    {
                        //SislokCrm
                        SqlDataReader reader;
                        Rasio tempStdDeviasi = new Rasio();

                        String kondisi = " WHERE kodePkt = '" + KodePkt[pktIndex] + "' AND DATENAME(WEEKDAY, TA.Tanggal) = '" + tempDate.DayOfWeek.ToString() + "'";
                        int count = 0;

                        for (int i = 0; i < treeView1.Nodes.Count; i++)
                        {
                            for (int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++)
                            {
                                if (treeView1.Nodes[i].Nodes[j].Checked)
                                {
                                    //Kalo cari minggu (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = (minggu keberapa)
                                    if (count++ == 0)
                                        kondisi += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                    else
                                        kondisi += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                }
                            }
                        }
                        kondisi += ")";

                        String subqueryTblAverage = "(SELECT AVG(sislokCrm100) AS Average100 , AVG(sislokCrm50) AS Average50 , AVG(sislokCrm20) AS Average20 FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal";
                        subqueryTblAverage += kondisi;
                        subqueryTblAverage += ") avt";

                        String query = "SELECT "
                                    + "[AverageStdDeviasi100] = AVG(CAST((sislokCrm100 - [Average100]) AS FLOAT) / (CASE WHEN [Average100] = 0 THEN 1 ELSE [Average100] END)), "
                                    + "[AverageStdDeviasi50] = AVG(CAST((sislokCrm50 - [Average50])AS FLOAT) / (CASE WHEN [Average50] = 0 THEN 1 ELSE [Average50] END)), "
                                    + "[AverageStdDeviasi20] = AVG(CAST((sislokCrm20 - [Average20])AS FLOAT) / (CASE WHEN [Average20] = 0 THEN 1 ELSE [Average20] END)) "
                                    + "FROM TransaksiAtms TA JOIN EventTanggal ET ON Ta.tanggal = ET.tanggal, " + subqueryTblAverage + kondisi;

                        cmd.CommandText = query;
                        reader = cmd.ExecuteReader();
                        bool event1 = true;
                        if (reader.Read())
                        {
                            if (String.IsNullOrEmpty(reader[0].ToString()) || String.IsNullOrEmpty(reader[1].ToString()) || String.IsNullOrEmpty(reader[2].ToString()))
                                event1 = false;
                            else
                            {
                                tempStdDeviasi.d100 = (Double)reader[0];
                                tempStdDeviasi.d50 = (Double)reader[1];
                                tempStdDeviasi.d20 = (Double)reader[2];
                                tempStdDeviasi.tgl = tempDate;
                            }
                            reader.Close();
                        }
                        if (!event1)
                        {
                            reader.Close();
                            kondisi = " WHERE kodePkt = '" + KodePkt[pktIndex] + "' ";
                            count = 0;

                            for (int i = 0; i < treeView1.Nodes.Count; i++)
                            {
                                for (int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++)
                                {
                                    if (treeView1.Nodes[i].Nodes[j].Checked)
                                    {
                                        //;
                                        //Kalo cari minggu (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = (minggu keberapa)
                                        if (count++ == 0)
                                            kondisi += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                        else
                                            kondisi += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                    }
                                }
                            }
                            kondisi += ")";

                            subqueryTblAverage = "(SELECT ISNULL(AVG(sislokCrm100),0) AS Average100 , ISNULL(AVG(sislokCrm50),0) AS Average50 , ISNULL(AVG(sislokCrm20),0) AS Average20 FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal";
                            subqueryTblAverage += kondisi;
                            subqueryTblAverage += ") avt";

                            query = "SELECT "
                                        + "[AverageStdDeviasi100] = AVG(CAST((sislokCrm100 - [Average100]) AS FLOAT) / (CASE WHEN [Average100] = 0 THEN 1 ELSE [Average100] END)), "
                                        + "[AverageStdDeviasi50] = AVG(CAST((sislokCrm50 - [Average50])AS FLOAT) / (CASE WHEN [Average50] = 0 THEN 1 ELSE [Average50] END)), "
                                        + "[AverageStdDeviasi20] = AVG(CAST((sislokCrm20 - [Average20])AS FLOAT) / (CASE WHEN [Average20] = 0 THEN 1 ELSE [Average20] END)) "
                                        + "FROM TransaksiAtms TA JOIN EventTanggal ET ON Ta.tanggal = ET.tanggal, " + subqueryTblAverage + kondisi;

                            cmd.CommandText = query;
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                tempStdDeviasi.d100 = (Double)reader[0];
                                tempStdDeviasi.d50 = (Double)reader[1];
                                tempStdDeviasi.d20 = (Double)reader[2];
                                tempStdDeviasi.tgl = tempDate;
                                reader.Close();
                            }
                        }
                        //Console.WriteLine(tempSislokCrm.d100.ToString());
                        stdDeviasi.Add(tempStdDeviasi);
                        tempDate = tempDate.AddDays(1);
                    }
                }
                sql.Close();
                Console.WriteLine("Deviasi Sislok CRM");
                Console.WriteLine(sislokCrm.Count);
                foreach (var temp in stdDeviasi)
                {
                    Console.WriteLine(temp.tgl + " " + temp.d100 + " " + temp.d50 + " " + temp.d20 + " ");
                }
            }
            sislokCrmDenganStdDeviasi = new List<Denom>();
            for (int a = 0; a < sislokCrm.Count; a++)
            {
                Denom temp = new Denom();
                temp.d100 = sislokCrm[a].d100 + (Int64)Math.Round(stdDeviasi[a].d100 * sislokCrm[a].d100);
                temp.d50 = sislokCrm[a].d50 + (Int64)Math.Round(stdDeviasi[a].d50 * sislokCrm[a].d50);
                temp.d20 = sislokCrm[a].d20 + (Int64)Math.Round(stdDeviasi[a].d20 * sislokCrm[a].d20);
                temp.tgl = sislokCrm[a].tgl;
                sislokCrmDenganStdDeviasi.Add(temp);
            }

            Console.WriteLine("Sislok CRM Dengan Std Deviasi");
            Console.WriteLine("===========================");
            foreach (var temp in sislokCrmDenganStdDeviasi)
            {
                Console.WriteLine(temp.tgl.ToShortDateString() + " " + temp.d100 + " " + temp.d50 + " " + temp.d20);
            }
        }
        void loadSislokCdm()
        {
            Console.WriteLine();
            Console.WriteLine("Load sislok cdm");
            Console.WriteLine("======================");
            sislokCdm = new List<Denom>();
            int rowcount = prediksiIsiAtmOpti.Count; //dataGridView1.Rows.Count;
                                                     //dataGridView1.Hide();
            DateTime startDate = tanggalOptiMin;//Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = tanggalOptiMax;
            DateTime tempDate = startDate;
            Console.WriteLine(startDate.DayOfWeek.ToString());
            Console.WriteLine(endDate);

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    bool isError = false;
                    String errMsg = "";
                    while (tempDate <= endDate)
                    {
                        //SislokCdm
                        SqlDataReader reader;
                        Denom tempSislokCdm = new Denom();
                        String cText = "SELECT AVG(sislokCdm100), AVG(sislokCdm50), AVG(sislokCdm20) FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE kodePkt = '" + KodePkt[pktIndex] + "' AND DATENAME(WEEKDAY, TA.Tanggal) = '" + tempDate.DayOfWeek.ToString() + "'";
                        int count = 0;
                        for (int i = 0; i < treeView1.Nodes.Count; i++)
                        {
                            for (int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++)
                            {
                                if (treeView1.Nodes[i].Nodes[j].Checked)
                                {
                                    //;
                                    //Kalo cari minggu (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = (minggu keberapa)
                                    if (count++ == 0)
                                        cText += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                    else
                                        cText += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                }
                            }
                        }
                        cText += ")";
                        cmd.CommandText = cText;
                        reader = cmd.ExecuteReader();
                        bool event1 = true;
                        if (reader.Read())
                        {
                            if (String.IsNullOrEmpty(reader[0].ToString()) || String.IsNullOrEmpty(reader[1].ToString()) || String.IsNullOrEmpty(reader[2].ToString()))
                                event1 = false;
                            else
                            {
                                tempSislokCdm.d100 = (Int64)reader[0];
                                tempSislokCdm.d50 = (Int64)reader[1];
                                tempSislokCdm.d20 = (Int64)reader[2];
                                tempSislokCdm.tgl = tempDate;
                            }
                            reader.Close();
                        }
                        if (!event1)
                        {
                            reader.Close();

                            cText = "SELECT AVG(sislokCdm100), AVG(sislokCdm50), AVG(sislokCdm20) FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE kodePkt = '" + KodePkt[pktIndex] + "' ";
                            count = 0;
                            for (int i = 0; i < treeView1.Nodes.Count; i++)
                            {
                                for (int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++)
                                {
                                    if (treeView1.Nodes[i].Nodes[j].Checked)
                                    {
                                        //;
                                        //Kalo cari minggu (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = (minggu keberapa)
                                        if (count++ == 0)
                                            cText += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                        else
                                            cText += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                    }
                                }
                            }
                            cText += ")";
                            cmd.CommandText = cText;
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                if (String.IsNullOrEmpty(reader[0].ToString()) || String.IsNullOrEmpty(reader[1].ToString()) || String.IsNullOrEmpty(reader[2].ToString()))
                                {
                                    isError = true;
                                    errMsg = errMsg + "Data Lainnya tanggal" + tempDate.ToShortDateString() + "Tidak ada! Value menjadi 0";
                                }
                                else
                                {
                                    isError = true;
                                    errMsg = errMsg + "Prediksi Lainnya Tanggal " + tempDate.ToShortDateString() + " Menggunakan Event2";
                                    tempSislokCdm.d100 = (Int64)reader[0];
                                    tempSislokCdm.d50 = (Int64)reader[1];
                                    tempSislokCdm.d20 = (Int64)reader[2];
                                    tempSislokCdm.tgl = tempDate;
                                }
                                reader.Close();
                            }
                            else
                            {
                                MessageBox.Show("Kurang Data Kalender");
                            }
                        }
                        //Console.WriteLine(tempSislokCdm.d100.ToString());
                        sislokCdm.Add(tempSislokCdm);
                        tempDate = tempDate.AddDays(1);
                        reader.Close();
                    }
                    if(isError)
                    {
                        MessageBox.Show(errMsg);
                    }
                }
                sql.Close();
               
                Console.WriteLine("CDM");
                foreach (var temp in sislokCdm)
                {
                    Console.WriteLine(temp.tgl + " " + temp.d100 + " " + temp.d50 + " " + temp.d20 + " ");
                }
            }
        }
        void loadSislokCdmDenganStdDeviasi()
        {
            Console.WriteLine();
            Console.WriteLine("Load Deviasi Sislok CDM");
            Console.WriteLine("======================");

            int rowcount = prediksiIsiAtmOpti.Count;//dataGridView1.Rows.Count;

            //dataGridView1.Hide();
            DateTime startDate = tanggalOptiMin;//Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = tanggalOptiMax;
            DateTime tempDate = startDate;
            Console.WriteLine(startDate.DayOfWeek.ToString());
            Console.WriteLine(endDate);

            //Load Std Deviasi
            List<Rasio> stdDeviasi = new List<Rasio>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    while (tempDate <= endDate)
                    {
                        //Sislokcdm
                        SqlDataReader reader;
                        Rasio tempStdDeviasi = new Rasio();

                        String kondisi = " WHERE kodePkt = '" + KodePkt[pktIndex] + "' AND DATENAME(WEEKDAY, TA.Tanggal) = '" + tempDate.DayOfWeek.ToString() + "'";
                        int count = 0;

                        for (int i = 0; i < treeView1.Nodes.Count; i++)
                        {
                            for (int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++)
                            {
                                if (treeView1.Nodes[i].Nodes[j].Checked)
                                {
                                    //;
                                    //Kalo cari minggu (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = (minggu keberapa)
                                    if (count++ == 0)
                                        kondisi += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                    else
                                        kondisi += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                }
                            }
                        }
                        kondisi += ")";

                        String subqueryTblAverage = "(SELECT AVG(sislokcdm100) AS Average100 , AVG(sislokCdm50) AS Average50 , AVG(sislokCdm20) AS Average20 FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal";
                        subqueryTblAverage += kondisi;
                        subqueryTblAverage += ") avt";

                        String query = "SELECT "
                                    + "[AverageStdDeviasi100] = AVG(CAST((sislokCdm100 - [Average100]) AS FLOAT) / (CASE WHEN [Average100] = 0 THEN 1 ELSE [Average100] END)), "
                                    + "[AverageStdDeviasi50] = AVG(CAST((sislokCdm50 - [Average50])AS FLOAT) / (CASE WHEN [Average50] = 0 THEN 1 ELSE [Average50] END)), "
                                    + "[AverageStdDeviasi20] = AVG(CAST((sislokCdm20 - [Average20])AS FLOAT) / (CASE WHEN [Average20] = 0 THEN 1 ELSE [Average20] END)) "
                                    + "FROM TransaksiAtms TA JOIN EventTanggal ET ON Ta.tanggal = ET.tanggal, " + subqueryTblAverage + kondisi;

                        cmd.CommandText = query;
                        reader = cmd.ExecuteReader();
                        bool event1 = true;
                        if (reader.Read())
                        {
                            if (String.IsNullOrEmpty(reader[0].ToString()) || String.IsNullOrEmpty(reader[1].ToString()) || String.IsNullOrEmpty(reader[2].ToString()))
                                event1 = false;
                            else
                            {
                                tempStdDeviasi.d100 = (Double)reader[0];
                                tempStdDeviasi.d50 = (Double)reader[1];
                                tempStdDeviasi.d20 = (Double)reader[2];
                                tempStdDeviasi.tgl = tempDate;
                            }
                            reader.Close();
                        }
                        if (!event1)
                        {
                            reader.Close();
                            kondisi = " WHERE kodePkt = '" + KodePkt[pktIndex] + "' ";
                            count = 0;

                            for (int i = 0; i < treeView1.Nodes.Count; i++)
                            {
                                for (int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++)
                                {
                                    if (treeView1.Nodes[i].Nodes[j].Checked)
                                    {
                                        //;
                                        //Kalo cari minggu (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = (minggu keberapa)
                                        if (count++ == 0)
                                            kondisi += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                        else
                                            kondisi += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                    }
                                }
                            }
                            kondisi += ")";

                            subqueryTblAverage = "(SELECT AVG(sislokcdm100) AS Average100 , AVG(sislokCdm50) AS Average50 , AVG(sislokCdm20) AS Average20 FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal";
                            subqueryTblAverage += kondisi;
                            subqueryTblAverage += ") avt";

                            query = "SELECT "
                                        + "[AverageStdDeviasi100] = AVG(CAST((sislokCdm100 - [Average100]) AS FLOAT) / (CASE WHEN [Average100] = 0 THEN 1 ELSE [Average100] END)), "
                                        + "[AverageStdDeviasi50] = AVG(CAST((sislokCdm50 - [Average50])AS FLOAT) / (CASE WHEN [Average50] = 0 THEN 1 ELSE [Average50] END)), "
                                        + "[AverageStdDeviasi20] = AVG(CAST((sislokCdm20 - [Average20])AS FLOAT) / (CASE WHEN [Average20] = 0 THEN 1 ELSE [Average20] END)) "
                                        + "FROM TransaksiAtms TA JOIN EventTanggal ET ON Ta.tanggal = ET.tanggal, " + subqueryTblAverage + kondisi;

                            cmd.CommandText = query;
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                tempStdDeviasi.d100 = (Double)reader[0];
                                tempStdDeviasi.d50 = (Double)reader[1];
                                tempStdDeviasi.d20 = (Double)reader[2];
                                tempStdDeviasi.tgl = tempDate;
                                reader.Close();
                            }
                        }
                        //Console.WriteLine(tempSislokCdm.d100.ToString());
                        stdDeviasi.Add(tempStdDeviasi);
                        tempDate = tempDate.AddDays(1);
                    }
                }
                sql.Close();
                Console.WriteLine("Deviasi Sislok Cdm");
                Console.WriteLine(sislokCdm.Count);
                foreach (var temp in stdDeviasi)
                {
                    Console.WriteLine(temp.tgl + " " + temp.d100 + " " + temp.d50 + " " + temp.d20 + " ");
                }
            }
            sislokCdmDenganStdDeviasi = new List<Denom>();
            for (int a = 0; a < sislokCdm.Count; a++)
            {
                Denom temp = new Denom();
                temp.d100 = sislokCdm[a].d100 + (Int64)Math.Round(stdDeviasi[a].d100 * sislokCdm[a].d100);
                temp.d50 = sislokCdm[a].d50 + (Int64)Math.Round(stdDeviasi[a].d50 * sislokCdm[a].d50);
                temp.d20 = sislokCdm[a].d20 + (Int64)Math.Round(stdDeviasi[a].d20 * sislokCdm[a].d20);
                temp.tgl = sislokCdm[a].tgl;
                sislokCdmDenganStdDeviasi.Add(temp);
            }

            Console.WriteLine("Sislok Cdm Dengan Std Deviasi");
            Console.WriteLine("===========================");
            foreach (var temp in sislokCdmDenganStdDeviasi)
            {
                Console.WriteLine(temp.tgl.ToShortDateString() + " " + temp.d100 + " " + temp.d50 + " " + temp.d20);
            }
        }
        void loadRasioSislokAtm()
        {
            Console.WriteLine();
            Console.WriteLine("Load rasio sislok atm");
            Console.WriteLine("======================");
            rasioSislokAtm = new List<Rasio>();
            int rowcount = prediksiIsiAtmOpti.Count; //dataGridView1.Rows.Count;
            //dataGridView1.Hide();
            DateTime startDate = tanggalOptiMin;//Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = tanggalOptiMax;
            DateTime tempDate = startDate;
            Console.WriteLine(startDate.DayOfWeek.ToString());
            Console.WriteLine(endDate);

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    while (tempDate <= endDate)
                    {
                        //SislokCdm
                        SqlDataReader reader;
                        Rasio tempSislokAtm = new Rasio();
                        String cText = "SELECT AVG(CAST(sislokAtm100 AS FLOAT)/NULLIF(isiAtm100,0)), AVG(CAST(sislokAtm50 AS FLOAT)/NULLIF(isiAtm50,0)), AVG(CAST(sislokAtm20 AS FLOAT)/NULLIF(isiAtm20,0)) FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE kodePkt = '" + KodePkt[pktIndex] + "' AND DATENAME(WEEKDAY, TA.Tanggal) = '" + tempDate.DayOfWeek.ToString() + "'";
                        int count = 0;
                        for (int i = 0; i < treeView1.Nodes.Count; i++)
                        {
                            for (int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++)
                            {
                                if (treeView1.Nodes[i].Nodes[j].Checked)
                                {
                                    //;
                                    //Kalo cari minggu (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = (minggu keberapa)
                                    if (count++ == 0)
                                        cText += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                    else
                                        cText += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                }
                            }
                        }
                        cText += ")";
                        cmd.CommandText = cText;
                        reader = cmd.ExecuteReader();
                        bool event1 = true;
                        if (reader.Read())
                        {//Console.WriteLine(reader[2]);
                            if (String.IsNullOrEmpty(reader[0].ToString()) || String.IsNullOrEmpty(reader[1].ToString()) || String.IsNullOrEmpty(reader[2].ToString()))
                                event1 = false;
                            else
                            {
                                tempSislokAtm.d100 = Convert.ToDouble(reader[0].ToString());
                                tempSislokAtm.d50 = Convert.ToDouble(reader[1].ToString());
                                tempSislokAtm.d20 = Convert.ToDouble(reader[2].ToString());
                                tempSislokAtm.tgl = tempDate;
                            }
                            reader.Close();
                        }
                        if (!event1)
                        {
                            reader.Close();
                            //SislokCdm
                            cText = "SELECT ISNULL(AVG(CAST(sislokAtm100 AS FLOAT)/NULLIF(isiAtm100,0)),0), ISNULL(AVG(CAST(sislokAtm50 AS FLOAT)/NULLIF(isiAtm50,0)),0), ISNULL(AVG(CAST(sislokAtm20 AS FLOAT)/NULLIF(isiAtm20,0)),0) FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE kodePkt = '" + KodePkt[pktIndex] + "' ";
                            count = 0;
                            for (int i = 0; i < treeView1.Nodes.Count; i++)
                            {
                                for (int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++)
                                {
                                    if (treeView1.Nodes[i].Nodes[j].Checked)
                                    {
                                        //;
                                        //Kalo cari minggu (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = (minggu keberapa)
                                        if (count++ == 0)
                                            cText += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                        else
                                            cText += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                    }
                                }
                            }
                            cText += ")";
                            cmd.CommandText = cText;
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {//Console.WriteLine(reader[2]);
                                if (reader[0] != null)
                                    tempSislokAtm.d100 = Convert.ToDouble(reader[0].ToString());
                                else
                                    tempSislokAtm.d100 = 0;

                                if (reader[1] != null)
                                    tempSislokAtm.d50 = Convert.ToDouble(reader[1].ToString());
                                else
                                    tempSislokAtm.d50 = 0;

                                if (reader[2].ToString() != "")
                                    tempSislokAtm.d20 = Convert.ToDouble(reader[2].ToString());
                                else
                                    tempSislokAtm.d20 = 0;
                                tempSislokAtm.tgl = tempDate;
                                reader.Close();
                            }
                        }
                        //Console.WriteLine(tempSislokCdm.d100.ToString());
                        rasioSislokAtm.Add(tempSislokAtm);
                        tempDate = tempDate.AddDays(1);
                    }
                }
                sql.Close();
                Console.WriteLine("Rasio ATM");
                foreach (var temp in rasioSislokAtm)
                {
                    Console.WriteLine(temp.tgl + " " + temp.d100 + " " + temp.d20 + " " + temp.d50);
                }
            }
        }
        void loadRasioSislokAtmDenganStdDeviasi()
        {
            Console.WriteLine();
            Console.WriteLine("Load Deviasi Sislok ATM");
            Console.WriteLine("======================");

            int rowcount = prediksiIsiAtmOpti.Count;//dataGridView1.Rows.Count;

            //dataGridView1.Hide();
            DateTime startDate = tanggalOptiMin;//Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = tanggalOptiMax;
            DateTime tempDate = startDate;
            Console.WriteLine(startDate.DayOfWeek.ToString());
            Console.WriteLine(endDate);

            //Load Std Deviasi
            List<Rasio> stdDeviasi = new List<Rasio>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    while (tempDate <= endDate)
                    {
                        //SislokATM
                        SqlDataReader reader;
                        Rasio tempStdDeviasi = new Rasio();

                        String kondisi = " WHERE kodePkt = '" + KodePkt[pktIndex] + "' AND DATENAME(WEEKDAY, TA.Tanggal) = '" + tempDate.DayOfWeek.ToString() + "'";
                        int count = 0;

                        for (int i = 0; i < treeView1.Nodes.Count; i++)
                        {
                            for (int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++)
                            {
                                if (treeView1.Nodes[i].Nodes[j].Checked)
                                {
                                    //;
                                    //Kalo cari minggu (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = (minggu keberapa)
                                    if (count++ == 0)
                                        kondisi += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                    else
                                        kondisi += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                }
                            }
                        }
                        kondisi += ")";

                        String subqueryTblAverage = "(SELECT AVG(CAST(sislokAtm100 AS FLOAT)/NULLIF(isiAtm100,0)) AS Average100 ,AVG(CAST(sislokAtm50 AS FLOAT)/NULLIF(isiAtm50,0)) AS Average50 , AVG(CAST(sislokAtm20 AS FLOAT)/NULLIF(isiAtm20,0)) AS Average20 FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal";
                        subqueryTblAverage += kondisi;
                        subqueryTblAverage += ") avt";

                        String query = "SELECT "
                                    + "[AverageStdDeviasi100] = AVG(CAST((CAST(sislokAtm100 AS FLOAT)/NULLIF(isiAtm100,0) - [Average100]) AS FLOAT) / (CASE WHEN [Average100] = 0 THEN 1 ELSE [Average100] END)), "
                                    + "[AverageStdDeviasi50] = AVG(CAST((CAST(sislokAtm50 AS FLOAT)/NULLIF(isiAtm50,0) - [Average50])AS FLOAT) / (CASE WHEN [Average50] = 0 THEN 1 ELSE [Average50] END)), "
                                    + "[AverageStdDeviasi20] = AVG(CAST((CAST(sislokAtm20 AS FLOAT)/NULLIF(isiAtm20,0) - [Average20])AS FLOAT) / (CASE WHEN [Average20] = 0 THEN 1 ELSE [Average20] END)) "
                                    + "FROM TransaksiAtms TA JOIN EventTanggal ET ON Ta.tanggal = ET.tanggal, " + subqueryTblAverage + kondisi;

                        cmd.CommandText = query;
                        reader = cmd.ExecuteReader();
                        bool event1 = true;
                        if (reader.Read())
                        {
                            if (String.IsNullOrEmpty(reader[0].ToString()) || String.IsNullOrEmpty(reader[1].ToString()) || String.IsNullOrEmpty(reader[2].ToString()))
                                event1 = false;
                            else
                            {
                                tempStdDeviasi.d100 = (Double)reader[0];
                                tempStdDeviasi.d50 = (Double)reader[1];
                                tempStdDeviasi.d20 = (Double)reader[2];
                                tempStdDeviasi.tgl = tempDate;
                            }
                            reader.Close();
                        }
                        if (!event1)
                        {
                            reader.Close();
                            kondisi = " WHERE kodePkt = '" + KodePkt[pktIndex] + "' ";
                            count = 0;

                            for (int i = 0; i < treeView1.Nodes.Count; i++)
                            {
                                for (int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++)
                                {
                                    if (treeView1.Nodes[i].Nodes[j].Checked)
                                    {
                                        //;
                                        //Kalo cari minggu (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = (minggu keberapa)
                                        if (count++ == 0)
                                            kondisi += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                        else
                                            kondisi += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                    }
                                }
                            }
                            kondisi += ")";

                            subqueryTblAverage = "(SELECT AVG(CAST(sislokAtm100 AS FLOAT)/NULLIF(isiAtm100,0)) AS Average100 ,AVG(CAST(sislokAtm50 AS FLOAT)/NULLIF(isiAtm50,0)) AS Average50 , AVG(CAST(sislokAtm20 AS FLOAT)/NULLIF(isiAtm20,0)) AS Average20 FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal";
                            subqueryTblAverage += kondisi;
                            subqueryTblAverage += ") avt";

                            query = "SELECT "
                                        + "[AverageStdDeviasi100] = ISNULL(AVG(CAST((CAST(sislokAtm100 AS FLOAT)/NULLIF(isiAtm100,0) - [Average100]) AS FLOAT) / (CASE WHEN [Average100] = 0 THEN 1 ELSE [Average100] END)),0), "
                                        + "[AverageStdDeviasi50] = ISNULL(AVG(CAST((CAST(sislokAtm50 AS FLOAT)/NULLIF(isiAtm50,0) - [Average50])AS FLOAT) / (CASE WHEN [Average50] = 0 THEN 1 ELSE [Average50] END)),0), "
                                        + "[AverageStdDeviasi20] = ISNULL(AVG(CAST((CAST(sislokAtm20 AS FLOAT)/NULLIF(isiAtm20,0) - [Average20])AS FLOAT) / (CASE WHEN [Average20] = 0 THEN 1 ELSE [Average20] END)),0) "
                                        + "FROM TransaksiAtms TA JOIN EventTanggal ET ON Ta.tanggal = ET.tanggal, " + subqueryTblAverage + kondisi;

                            cmd.CommandText = query;
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                tempStdDeviasi.d100 = (Double)reader[0];
                                tempStdDeviasi.d50 = (Double)reader[1];
                                tempStdDeviasi.d20 = (Double)reader[2];
                                tempStdDeviasi.tgl = tempDate;
                                reader.Close();
                            }
                        }
                        //Console.WriteLine(tempSislokATM.d100.ToString());
                        stdDeviasi.Add(tempStdDeviasi);
                        tempDate = tempDate.AddDays(1);
                    }
                }
                sql.Close();
                Console.WriteLine("Deviasi Sislok ATM");
                Console.WriteLine(rasioSislokATMDenganStdDeviasi.Count);
                foreach (var temp in stdDeviasi)
                {
                    Console.WriteLine(temp.tgl + " " + temp.d100 + " " + temp.d50 + " " + temp.d20 + " ");
                }
            }
            rasioSislokATMDenganStdDeviasi = new List<Rasio>();
            for (int a = 0; a < rasioSislokAtm.Count; a++)
            {
                Rasio temp = new Rasio();
                temp.d100 = rasioSislokAtm[a].d100 + (Int64)Math.Round(stdDeviasi[a].d100 * rasioSislokAtm[a].d100);
                temp.d50 = rasioSislokAtm[a].d50 + (Int64)Math.Round(stdDeviasi[a].d50 * rasioSislokAtm[a].d50);
                temp.d20 = rasioSislokAtm[a].d20 + (Int64)Math.Round(stdDeviasi[a].d20 * rasioSislokAtm[a].d20);
                temp.tgl = rasioSislokAtm[a].tgl;
                rasioSislokATMDenganStdDeviasi.Add(temp);
            }

            Console.WriteLine("Sislok ATM Dengan Std Deviasi");
            Console.WriteLine("===========================");
            foreach (var temp in rasioSislokATMDenganStdDeviasi)
            {
                Console.WriteLine(temp.tgl.ToShortDateString() + " " + temp.d100 + " " + temp.d50 + " " + temp.d20);
            }
        }
        void loadBon()
        {
            bon = new List<Denom>();
            jumlahBonLaporan = 0;
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = sql;
                cmd.CommandText = "SELECT DISTINCT [tanggal], [100], [50], [20] FROM laporanBon WHERE kodePkt = '" + KodePkt[pktIndex] + "' AND tanggal BETWEEN '" + tanggalOptiMin.ToShortDateString() + "' AND '" + tanggalOptiMax.ToShortDateString() + "'";
                sql.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Denom temp = new Denom();
                    temp.tgl = (DateTime)reader[0];
                    temp.d100 = Convert.ToInt64(reader[1]);
                    temp.d50 = Convert.ToInt64(reader[2]);
                    temp.d20 = Convert.ToInt64(reader[3]);
                    bon.Add(temp);
                    jumlahBonLaporan++;
                }
                sql.Close();
            }
            //Console.WriteLine("BON");
            //Console.WriteLine(bon[0].d100);
        }
        void loadSetor()
        {
            Database1Entities db = new Database1Entities();
            setor = new List<Denom>();
            String kodepkt = KodePkt[pktIndex];
            DateTime today = DateTime.Today.Date;
            Denom temp = new Denom();
            var q2 = (from x in db.Approvals
                      join y in db.DetailApprovals on x.idApproval equals y.idApproval
                      where x.kodePkt == kodepkt && (y.tanggal >= tanggalOptiMin)
                      && x.tanggal < today
                      select new { Approval = x, DetailApproval = y }).ToList();
            foreach (var s in q2)
            {
                temp = new Denom();
                if (s.DetailApproval.setor100 == null)
                    temp.d100 = 0;
                else
                    temp.d100 = (Int64)s.DetailApproval.setor100;
                if (s.DetailApproval.setor100 == null)
                    temp.d50 = 0;
                else
                    temp.d50 = (Int64)s.DetailApproval.setor50;
                if (s.DetailApproval.setor100 == null)
                    temp.d20 = 0;
                else
                    temp.d20 = (Int64)s.DetailApproval.setor20;
                temp.tgl = (DateTime)s.DetailApproval.tanggal;
                setor.Add(temp);
            }
        }
        void loadRekomendasiBonNonE2E()
        {
            int counter = 0;
            int setorCounter = 0;
            //Disini hari h dianggap hari terakhir ada laporan bon
            Console.WriteLine();
            Console.WriteLine("Load rekomendasi bon");
            Console.WriteLine("======================");
            rekomendasiBon = new List<Denom>();
            saldoAwalIdeal = new List<Denom>();
            rekomendasiAdhoc = new Denom();

            Double targetRasio100, targetRasio50, targetRasio20;
            if (rasio100Txt.Text.Trim().Length == 0)
                targetRasio100 = 0;
            else
                targetRasio100 = Double.Parse(rasio100Txt.Text);
            if (rasio50Txt.Text.Trim().Length == 0)
                targetRasio50 = 0;
            else
                targetRasio50 = Double.Parse(rasio50Txt.Text);
            if (rasio20Txt.Text.Trim().Length == 0)
                targetRasio20 = 0;
            else
                targetRasio20 = Double.Parse(rasio20Txt.Text);

            Denom saldoAkhirH = new Denom();

            rasio100Lbl.Text = "-";
            rasio50Lbl.Text = "-";
            rasio20Lbl.Text = "-";


            Console.WriteLine("Target Rasio 100: " + targetRasio100);
            Console.WriteLine("Target Rasio 50: " + targetRasio50);
            Console.WriteLine("Target Rasio 20: " + targetRasio20);
            Console.WriteLine("Saldo Awal");
            Console.WriteLine(saldoAwal.d100 + " " + saldoAwal.d50 + " " + saldoAwal.d20);

            //Hitung saldo akhir hari h untuk jadi saldo awal h+1
            saldoAkhirH.d100 = saldoAwal.d100 + (Int64)Math.Round((rasioSislokAtm[0].d100 * prediksiIsiAtm[0].d100)) + sislokCdm[0].d100 + sislokCrm[0].d100 - prediksiIsiAtm[0].d100 - isiCrm[0].d100 + bon[0].d100;
            saldoAkhirH.d50 = saldoAwal.d50 + (Int64)Math.Round((rasioSislokAtm[0].d50 * prediksiIsiAtm[0].d50)) + sislokCdm[0].d50 + sislokCrm[0].d50 - prediksiIsiAtm[0].d50 - isiCrm[0].d50 + bon[0].d50;
            saldoAkhirH.d20 = saldoAwal.d20 + (Int64)Math.Round((rasioSislokAtm[0].d20 * prediksiIsiAtm[0].d20)) + sislokCdm[0].d20 + sislokCrm[0].d20 - prediksiIsiAtm[0].d20 - isiCrm[0].d20 + bon[0].d20;
            saldoAkhirH.tgl = tanggalOptiMin;


            if (setorCounter < setor.Count && saldoAkhirH.tgl == setor[setorCounter].tgl)
            {
                saldoAkhirH.d100 -= setor[setorCounter].d100;
                saldoAkhirH.d50 -= setor[setorCounter].d50;
                saldoAkhirH.d20 -= setor[setorCounter++].d20;
            }

            //Rekomendasi Adhoc
            rekomendasiAdhoc.d100 = 0;
            rekomendasiAdhoc.d50 = 0;
            rekomendasiAdhoc.d20 = 0;
            if (saldoAwal.d100 - prediksiIsiAtm[0].d100 - isiCrm[0].d100 + bon[0].d100 < 0)
            {
                rekomendasiAdhoc.d100 = -(saldoAwal.d100 - prediksiIsiAtm[0].d100 - isiCrm[0].d100 + bon[0].d100);
                Double rasio = (saldoAkhirH.d100 + rekomendasiAdhoc.d100) / ((Double)prediksiIsiAtm[1].d100 + (Double)isiCrm[1].d100); // Saldo akhir hari h dibagi dengan prediksi isi atm h+1
                rasio100Lbl.Text = Math.Round(rasio, 2).ToString();
            }
            if (saldoAwal.d50 - prediksiIsiAtm[0].d50 - isiCrm[0].d50 + bon[0].d50 < 0)
            {
                rekomendasiAdhoc.d50 = -(saldoAwal.d50 - prediksiIsiAtm[0].d50 - isiCrm[0].d50 + bon[0].d50);
                Double rasio = (saldoAkhirH.d50 + rekomendasiAdhoc.d50) / ((Double)prediksiIsiAtm[1].d50 + (Double)isiCrm[1].d100); // Saldo akhir hari h dibagi dengan prediksi isi atm h+1
                rasio50Lbl.Text = Math.Round(rasio, 2).ToString();
            }
            if (saldoAwal.d20 - prediksiIsiAtm[0].d20 - isiCrm[0].d20 + bon[0].d20 < 0)
            {
                rekomendasiAdhoc.d20 = -(saldoAwal.d20 - prediksiIsiAtm[0].d20 - isiCrm[0].d20 + bon[0].d20);
                Double rasio = (saldoAkhirH.d20 + rekomendasiAdhoc.d20) / ((Double)prediksiIsiAtm[1].d20 + (Double)isiCrm[1].d100); // Saldo akhir hari h dibagi dengan prediksi isi atm h+1
                rasio20Lbl.Text = Math.Round(rasio, 2).ToString();
            }

            //Kalau ternyata di laporan bonnya lebih dari 1 hitung sampe hari terakhir ada bon yang disetujui
            for (int a = 1; a < jumlahBonLaporan; a++)
            {
                //Hitung saldo akhir hari h
                saldoAkhirH.tgl = tanggalOptiMin.AddDays(a);
                saldoAkhirH.d100 = saldoAkhirH.d100 + (Int64)Math.Round((rasioSislokAtm[a].d100 * prediksiIsiAtm[a].d100)) + sislokCdm[a].d100 + sislokCrm[a].d100 - prediksiIsiAtm[a].d100 - isiCrm[a].d100 + bon[a].d100;
                saldoAkhirH.d50 = saldoAkhirH.d50 + (Int64)Math.Round((rasioSislokAtm[a].d50 * prediksiIsiAtm[a].d50)) + sislokCdm[a].d50 + sislokCrm[a].d50 - prediksiIsiAtm[a].d50 - isiCrm[a].d50 + bon[a].d50;
                saldoAkhirH.d20 = saldoAkhirH.d20 + (Int64)Math.Round((rasioSislokAtm[a].d20 * prediksiIsiAtm[a].d20)) + sislokCdm[a].d20 + sislokCrm[a].d20 - prediksiIsiAtm[a].d20 - isiCrm[a].d20 + bon[a].d20;


                if (setorCounter < setor.Count && saldoAkhirH.tgl == setor[setorCounter].tgl)
                {
                    saldoAkhirH.d100 -= setor[setorCounter].d100;
                    saldoAkhirH.d50 -= setor[setorCounter].d50;
                    saldoAkhirH.d20 -= setor[setorCounter++].d20;
                }
            }



            //Hitung saldo ideal h+2, h+3, ..
            for (int a = 1 + jumlahBonLaporan; a < prediksiIsiAtm.Count; a++)
            {
                Denom temp = new Denom();
                temp.d100 = (Int64)Math.Round(((Double)prediksiIsiAtm[a].d100 + (Double)isiCrm[a].d100) * targetRasio100);
                temp.d50 = (Int64)Math.Round(((Double)prediksiIsiAtm[a].d50 + (Double)isiCrm[a].d50) * targetRasio50);
                temp.d20 = (Int64)Math.Round(((Double)prediksiIsiAtm[a].d20 + (Double)isiCrm[a].d20) * targetRasio20);
                temp.tgl = prediksiIsiAtm[a].tgl;
                saldoAwalIdeal.Add(temp);
            }

            Console.WriteLine("Saldo awal ideal\n===============");
            foreach (var temp in saldoAwalIdeal)
            {
                Console.WriteLine(temp.tgl + " " + temp.d100 + " " + temp.d50 + " " + temp.d20);
            }
            //MessageBox.Show("Jumlah Bon Laporan: " + jumlahBonLaporan);

            //Ambil saldo akhir ideal h+1 dari saldo awal ideal h+2
            //MessageBox.Show("Jumlah Bon Laporan: "+jumlahBonLaporan);
            Denom saldoAkhirH1Ideal = new Denom();
            saldoAkhirH1Ideal.d100 = saldoAwalIdeal[0].d100;
            saldoAkhirH1Ideal.d50 = saldoAwalIdeal[0].d50;
            saldoAkhirH1Ideal.d20 = saldoAwalIdeal[0].d20;

            Console.WriteLine("Saldo Akhir Hari H");
            Console.WriteLine(saldoAkhirH.tgl + " " + saldoAkhirH.d100 + " " + saldoAkhirH.d50 + " " + saldoAkhirH.d20);

            //Hitung rekomendasiBon untuk h+1 (Belom ada setor dan adhoc)
            Denom tempRekomendasiBon = new Denom();


            //Ini untuk hitung rekomendasi bon H+2 (rekomendasi pertama)
            tempRekomendasiBon.d100 = saldoAkhirH1Ideal.d100
                - saldoAkhirH.d100                                                      //Ambil saldo akhir di hari h (jadi saldo awal h+1)
                - (Int64)Math.Round((rasioSislokAtm[jumlahBonLaporan].d100 * prediksiIsiAtm[jumlahBonLaporan].d100))
                - sislokCrm[jumlahBonLaporan].d100
                - sislokCdm[jumlahBonLaporan].d100
                + prediksiIsiAtm[jumlahBonLaporan].d100
                + isiCrm[jumlahBonLaporan].d100;

            tempRekomendasiBon.d50 = saldoAkhirH1Ideal.d50
                - saldoAkhirH.d50
                - (Int64)Math.Round((rasioSislokAtm[jumlahBonLaporan].d50 * prediksiIsiAtm[jumlahBonLaporan].d50))
                - sislokCrm[jumlahBonLaporan].d50
                - sislokCdm[jumlahBonLaporan].d50
                + prediksiIsiAtm[jumlahBonLaporan].d50
                + isiCrm[jumlahBonLaporan].d50;

            tempRekomendasiBon.d20 = saldoAkhirH1Ideal.d20
                - saldoAkhirH.d20
                - (Int64)Math.Round((rasioSislokAtm[jumlahBonLaporan].d20 * prediksiIsiAtm[jumlahBonLaporan].d20))
                - sislokCrm[jumlahBonLaporan].d20
                - sislokCdm[jumlahBonLaporan].d20
                + prediksiIsiAtm[jumlahBonLaporan].d20
                + isiCrm[jumlahBonLaporan].d20;

            tempRekomendasiBon.tgl = tanggalOptiMin.AddDays(jumlahBonLaporan);


            if (setorCounter < setor.Count && tempRekomendasiBon.tgl == setor[setorCounter].tgl)
            {
                tempRekomendasiBon.d100 += setor[setorCounter].d100;
                tempRekomendasiBon.d50 += setor[setorCounter].d50;
                tempRekomendasiBon.d20 += setor[setorCounter++].d20;
            }

            rekomendasiBon.Add(tempRekomendasiBon);

            counter = jumlahBonLaporan + 1;
            //Hitung rekomendasiBon h+3 keatas
            for (int a = 0; a < saldoAwalIdeal.Count - 1; a++)
            {
                tempRekomendasiBon = new Denom();
                Denom saldoAkhirIdeal = saldoAwalIdeal[a + 1];
                //Denom saldoSementara = new Denom();`
                //saldoSementara.d100 = saldoAwalIdeal[a].d100 - isiCrm[counter].d100;
                tempRekomendasiBon.d100 = saldoAwalIdeal[a + 1].d100                                 //Saldo akhir h+1 
                    - saldoAwalIdeal[a].d100                                                      //saldo awal hari h
                    - (Int64)Math.Round((rasioSislokAtm[counter + a].d100 * prediksiIsiAtm[counter + a].d100))
                    - sislokCrm[counter + a].d100
                    - sislokCdm[counter + a].d100
                    + prediksiIsiAtm[counter + a].d100
                    + isiCrm[counter + a].d100;
                tempRekomendasiBon.d50 = saldoAwalIdeal[a + 1].d50                                 //Saldo akhir h+1 
                    - saldoAwalIdeal[a].d50                                                      //saldo awal hari h
                    - (Int64)Math.Round((rasioSislokAtm[counter + a].d50 * prediksiIsiAtm[counter + a].d50))
                    - sislokCrm[counter + a].d50
                    - sislokCdm[counter + a].d50
                    + prediksiIsiAtm[counter + a].d50
                    + isiCrm[counter + a].d50;
                tempRekomendasiBon.d20 = saldoAwalIdeal[a + 1].d20                                 //Saldo akhir h+1 
                    - saldoAwalIdeal[a].d20                                                      //saldo awal hari h
                    - (Int64)Math.Round((rasioSislokAtm[counter + a].d20 * prediksiIsiAtm[counter + a].d20))
                    - sislokCrm[counter + a].d20
                    - sislokCdm[counter + a].d20
                    + prediksiIsiAtm[counter + a].d20
                    + isiCrm[counter + a].d20;
                tempRekomendasiBon.tgl = tanggalOptiMin.AddDays(counter + a);

                if (setorCounter < setor.Count && tempRekomendasiBon.tgl == setor[setorCounter].tgl)
                {
                    tempRekomendasiBon.d100 += setor[setorCounter].d100;
                    tempRekomendasiBon.d50 += setor[setorCounter].d50;
                    tempRekomendasiBon.d20 += setor[setorCounter++].d20;
                }

                rekomendasiBon.Add(tempRekomendasiBon);
                //counter++;
            }
            for (int a = rekomendasiBon.Count - 1; a > 0; a--)
            {
                if (tanggalSkip.Contains(rekomendasiBon[a].tgl))
                {
                    if (a > 0)
                    {
                        rekomendasiBon[a - 1].d100 += rekomendasiBon[a].d100;
                        rekomendasiBon[a - 1].d50 += rekomendasiBon[a].d50;
                        rekomendasiBon[a - 1].d20 += rekomendasiBon[a].d20;

                        rekomendasiBon[a].d100 = 0;
                        rekomendasiBon[a].d50 = 0;
                        rekomendasiBon[a].d20 = 0;
                    }
                }
            }
            Console.WriteLine("Rekomendasi Bon");
            foreach (var temp in rekomendasiBon)
            {
                Console.WriteLine(temp.tgl + " " + temp.d100 + " " + temp.d50 + " " + temp.d20);
            }

            rekomendasiAdhoc100Lbl.Text = rekomendasiAdhoc.d100.ToString("#,##0");
            rekomendasiAdhoc50Lbl.Text = rekomendasiAdhoc.d50.ToString("#,##0");
            rekomendasiAdhoc20Lbl.Text = rekomendasiAdhoc.d20.ToString("#,##0");
        }
        void loadRekomendasiBonE2E()
        {
            int counter = 0;
            int setorCounter = 0;
            Console.WriteLine();
            Console.WriteLine("Load rekomendasi bon");
            Console.WriteLine("======================");
            List<Denom> rekomendasiBonNonE2E = new List<Denom>();
            saldoAwalIdeal = new List<Denom>();
            rekomendasiAdhoc = new Denom();


            Double targetRasio100, targetRasio50, targetRasio20;
            if (rasio100Txt.Text.Trim().Length == 0)
                targetRasio100 = 0;
            else
                targetRasio100 = Double.Parse(rasio100Txt.Text);

            if (rasio50Txt.Text.Trim().Length == 0)
                targetRasio50 = 0;
            else
                targetRasio50 = Double.Parse(rasio50Txt.Text);
            if (rasio20Txt.Text.Trim().Length == 0)
                targetRasio20 = 0;
            else
                targetRasio20 = Double.Parse(rasio20Txt.Text);

            Denom saldoAkhirH = new Denom();
            Console.WriteLine("Saldo Awal");
            Console.WriteLine(saldoAwal.d100 + " " + saldoAwal.d50 + " " + saldoAwal.d20);

            /**********************************************/
            /*********START OF ITUNGAN BON NON E2E*********/
            /**********************************************/

            //Hitung saldo akhir hari h untuk jadi saldo awal h+1
            saldoAkhirH.d100 = saldoAwal.d100 + (Int64)Math.Round((rasioSislokAtm[0].d100 * prediksiIsiAtm[0].d100)) + sislokCdm[0].d100 + sislokCrm[0].d100 - prediksiIsiAtm[0].d100 - isiCrm[0].d100 + bon[0].d100;
            saldoAkhirH.d50 = saldoAwal.d50 + (Int64)Math.Round((rasioSislokAtm[0].d50 * prediksiIsiAtm[0].d50)) + sislokCdm[0].d50 + sislokCrm[0].d50 - prediksiIsiAtm[0].d50 - isiCrm[0].d50 + bon[0].d50;
            saldoAkhirH.d20 = saldoAwal.d20 + (Int64)Math.Round((rasioSislokAtm[0].d20 * prediksiIsiAtm[0].d20)) + sislokCdm[0].d20 + sislokCrm[0].d20 - prediksiIsiAtm[0].d20 - isiCrm[0].d20 + bon[0].d20;
            saldoAkhirH.tgl = tanggalOptiMin;


            if (setorCounter < setor.Count && saldoAkhirH.tgl == setor[setorCounter].tgl)
            {
                saldoAkhirH.d100 -= setor[setorCounter].d100;
                saldoAkhirH.d50 -= setor[setorCounter].d50;
                saldoAkhirH.d20 -= setor[setorCounter++].d20;
            }

            //Rekomendasi Adhoc
            rekomendasiAdhoc.d100 = 0;
            rekomendasiAdhoc.d50 = 0;
            rekomendasiAdhoc.d20 = 0;
            if (saldoAwal.d100 - prediksiIsiAtm[0].d100 - isiCrm[0].d100 + bon[0].d100 < 0)
            {
                rekomendasiAdhoc.d100 = -(saldoAwal.d100 - prediksiIsiAtm[0].d100 - isiCrm[0].d100 + bon[0].d100);
                Double rasio = (saldoAkhirH.d100 + rekomendasiAdhoc.d100) / ((Double)prediksiIsiAtm[1].d100 + (Double)isiCrm[1].d100); // Saldo akhir hari h dibagi dengan prediksi isi atm h+1
                rasio100Lbl.Text = Math.Round(rasio, 2).ToString();
            }
            if (saldoAwal.d50 - prediksiIsiAtm[0].d50 - isiCrm[0].d50 + bon[0].d50 < 0)
            {
                rekomendasiAdhoc.d50 = -(saldoAwal.d50 - prediksiIsiAtm[0].d50 - isiCrm[0].d50 + bon[0].d50);
                Double rasio = (saldoAkhirH.d50 + rekomendasiAdhoc.d50) / ((Double)prediksiIsiAtm[1].d50 + (Double)isiCrm[1].d100); // Saldo akhir hari h dibagi dengan prediksi isi atm h+1
                rasio50Lbl.Text = Math.Round(rasio, 2).ToString();
            }
            if (saldoAwal.d20 - prediksiIsiAtm[0].d20 - isiCrm[0].d20 + bon[0].d20 < 0)
            {
                rekomendasiAdhoc.d20 = -(saldoAwal.d20 - prediksiIsiAtm[0].d20 - isiCrm[0].d20 + bon[0].d20);
                Double rasio = (saldoAkhirH.d20 + rekomendasiAdhoc.d20) / ((Double)prediksiIsiAtm[1].d20 + (Double)isiCrm[1].d100); // Saldo akhir hari h dibagi dengan prediksi isi atm h+1
                rasio20Lbl.Text = Math.Round(rasio, 2).ToString();
            }

            //Kalau ternyata di laporan bonnya lebih dari 1 hitung sampe hari terakhir ada bon yang disetujui
            for (int a = 1; a < jumlahBonLaporan; a++)
            {
                //Hitung saldo akhir hari h
                saldoAkhirH.tgl = tanggalOptiMin.AddDays(a);
                saldoAkhirH.d100 = saldoAkhirH.d100 + (Int64)Math.Round((rasioSislokAtm[a].d100 * prediksiIsiAtm[a].d100)) + sislokCdm[a].d100 + sislokCrm[a].d100 - prediksiIsiAtm[a].d100 - isiCrm[a].d100 + bon[a].d100;
                saldoAkhirH.d50 = saldoAkhirH.d50 + (Int64)Math.Round((rasioSislokAtm[a].d50 * prediksiIsiAtm[a].d50)) + sislokCdm[a].d50 + sislokCrm[a].d50 - prediksiIsiAtm[a].d50 - isiCrm[a].d50 + bon[a].d50;
                saldoAkhirH.d20 = saldoAkhirH.d20 + (Int64)Math.Round((rasioSislokAtm[a].d20 * prediksiIsiAtm[a].d20)) + sislokCdm[a].d20 + sislokCrm[a].d20 - prediksiIsiAtm[a].d20 - isiCrm[a].d20 + bon[a].d20;


                if (setorCounter < setor.Count && saldoAkhirH.tgl == setor[setorCounter].tgl)
                {
                    saldoAkhirH.d100 -= setor[setorCounter].d100;
                    saldoAkhirH.d50 -= setor[setorCounter].d50;
                    saldoAkhirH.d20 -= setor[setorCounter++].d20;
                }
            }



            //Hitung saldo ideal h+2, h+3, ..
            for (int a = 2 + jumlahBonLaporan - 1; a < prediksiIsiAtm.Count; a++)
            {
                Denom temp = new Denom();
                temp.d100 = (Int64)Math.Round(((Double)prediksiIsiAtm[a].d100 + (Double)isiCrm[a].d100) * targetRasio100);
                temp.d50 = (Int64)Math.Round(((Double)prediksiIsiAtm[a].d50 + (Double)isiCrm[a].d50) * targetRasio50);
                temp.d20 = (Int64)Math.Round(((Double)prediksiIsiAtm[a].d20 + (Double)isiCrm[a].d20) * targetRasio20);
                temp.tgl = prediksiIsiAtm[a].tgl;
                saldoAwalIdeal.Add(temp);
            }

            //MessageBox.Show("Jumlah Bon Laporan: " + jumlahBonLaporan);

            //Ambil saldo akhir ideal h+1 dari saldo awal ideal h+2
            //MessageBox.Show("Jumlah Bon Laporan: "+jumlahBonLaporan);
            Denom saldoAkhirH1Ideal = new Denom();
            saldoAkhirH1Ideal.d100 = saldoAwalIdeal[0].d100;
            saldoAkhirH1Ideal.d50 = saldoAwalIdeal[0].d50;
            saldoAkhirH1Ideal.d20 = saldoAwalIdeal[0].d20;

            //Hitung rekomendasiBonNonE2E untuk h+1 (Belom ada setor dan adhoc)
            Denom temprekomendasiBonNonE2E = new Denom();


            //Ini untuk hitung rekomendasi bon H+2 (rekomendasi pertama)
            temprekomendasiBonNonE2E.d100 = saldoAkhirH1Ideal.d100
                - saldoAkhirH.d100                                                      //Ambil saldo akhir di hari h (jadi saldo awal h+1)
                - (Int64)Math.Round((rasioSislokAtm[jumlahBonLaporan].d100 * prediksiIsiAtm[jumlahBonLaporan].d100))
                - sislokCrm[jumlahBonLaporan].d100
                - sislokCdm[jumlahBonLaporan].d100
                + prediksiIsiAtm[jumlahBonLaporan].d100
                + isiCrm[jumlahBonLaporan].d100;

            temprekomendasiBonNonE2E.d50 = saldoAkhirH1Ideal.d50
                - saldoAkhirH.d50
                - (Int64)Math.Round((rasioSislokAtm[jumlahBonLaporan].d50 * prediksiIsiAtm[jumlahBonLaporan].d50))
                - sislokCrm[jumlahBonLaporan].d50
                - sislokCdm[jumlahBonLaporan].d50
                + prediksiIsiAtm[jumlahBonLaporan].d50
                + isiCrm[jumlahBonLaporan].d50;

            temprekomendasiBonNonE2E.d20 = saldoAkhirH1Ideal.d20
                - saldoAkhirH.d20
                - (Int64)Math.Round((rasioSislokAtm[jumlahBonLaporan].d20 * prediksiIsiAtm[jumlahBonLaporan].d20))
                - sislokCrm[jumlahBonLaporan].d20
                - sislokCdm[jumlahBonLaporan].d20
                + prediksiIsiAtm[jumlahBonLaporan].d20
                + isiCrm[jumlahBonLaporan].d20;

            temprekomendasiBonNonE2E.tgl = tanggalOptiMin.AddDays(1 + jumlahBonLaporan - 1);

            if (setorCounter < setor.Count && temprekomendasiBonNonE2E.tgl == setor[setorCounter].tgl)
            {
                temprekomendasiBonNonE2E.d100 += setor[setorCounter].d100;
                temprekomendasiBonNonE2E.d50 += setor[setorCounter].d50;
                temprekomendasiBonNonE2E.d20 += setor[setorCounter++].d20;
            }

            rekomendasiBonNonE2E.Add(temprekomendasiBonNonE2E);

            counter = jumlahBonLaporan + 1;
            //Hitung rekomendasiBonNonE2E h+3 keatas
            for (int a = 0; a < saldoAwalIdeal.Count - 1; a++)
            {
                temprekomendasiBonNonE2E = new Denom();
                Denom saldoAkhirIdeal = saldoAwalIdeal[a + 1];
                //Denom saldoSementara = new Denom();`
                //saldoSementara.d100 = saldoAwalIdeal[a].d100 - isiCrm[counter].d100;
                temprekomendasiBonNonE2E.d100 = saldoAwalIdeal[a + 1].d100                                 //Saldo akhir h+1 
                    - saldoAwalIdeal[a].d100                                                      //saldo awal hari h
                    - (Int64)Math.Round((rasioSislokAtm[counter + a].d100 * prediksiIsiAtm[counter + a].d100))
                    - sislokCrm[counter + a].d100
                    - sislokCdm[counter + a].d100
                    + prediksiIsiAtm[counter + a].d100
                    + isiCrm[counter + a].d100;
                temprekomendasiBonNonE2E.d50 = saldoAwalIdeal[a + 1].d50                                 //Saldo akhir h+1 
                    - saldoAwalIdeal[a].d50                                                      //saldo awal hari h
                    - (Int64)Math.Round((rasioSislokAtm[counter + a].d50 * prediksiIsiAtm[counter + a].d50))
                    - sislokCrm[counter + a].d50
                    - sislokCdm[counter + a].d50
                    + prediksiIsiAtm[counter + a].d50
                    + isiCrm[counter + a].d50;
                temprekomendasiBonNonE2E.d20 = saldoAwalIdeal[a + 1].d20                                 //Saldo akhir h+1 
                    - saldoAwalIdeal[a].d20                                                      //saldo awal hari h
                    - (Int64)Math.Round((rasioSislokAtm[counter + a].d20 * prediksiIsiAtm[counter + a].d20))
                    - sislokCrm[counter + a].d20
                    - sislokCdm[counter + a].d20
                    + prediksiIsiAtm[counter + a].d20
                    + isiCrm[counter + a].d20;
                temprekomendasiBonNonE2E.tgl = tanggalOptiMin.AddDays(counter + a);

                if (setorCounter < setor.Count && temprekomendasiBonNonE2E.tgl == setor[setorCounter].tgl)
                {
                    temprekomendasiBonNonE2E.d100 += setor[setorCounter].d100;
                    temprekomendasiBonNonE2E.d50 += setor[setorCounter].d50;
                    temprekomendasiBonNonE2E.d20 += setor[setorCounter++].d20;
                }

                rekomendasiBonNonE2E.Add(temprekomendasiBonNonE2E);
                //counter++;
            }
            Console.WriteLine("Rekomendasi Bon");
            foreach (var temp in rekomendasiBonNonE2E)
            {
                Console.WriteLine(temp.tgl + " " + temp.d100 + " " + temp.d50 + " " + temp.d20);
            }

            rekomendasiAdhoc100Lbl.Text = rekomendasiAdhoc.d100.ToString("#,##0");
            rekomendasiAdhoc50Lbl.Text = rekomendasiAdhoc.d50.ToString("#,##0");
            rekomendasiAdhoc20Lbl.Text = rekomendasiAdhoc.d20.ToString("#,##0");
            /************************************************/
            /************END OF ITUNGAN BON NON E2E**********/
            /***********************************************/



            /**********************************************/
            /*********START OF ITUNGAN BON E2E*************/
            /**********************************************/

            List<Denom> rekomendasiBonE2E = new List<Denom>();
            //Hitungan untuk bon

            //Hitung saldo akhir hari h untuk jadi saldo awal h+1
            saldoAkhirH.d100 = saldoAwal.d100 + (Int64)Math.Round((rasioSislokAtm[0].d100 * prediksiIsiAtm[0].d100)) + sislokCdm[0].d100 + sislokCrm[0].d100 - prediksiIsiAtm[0].d100 - isiCrm[0].d100 + bon[0].d100;
            saldoAkhirH.d50 = saldoAwal.d50 + (Int64)Math.Round((rasioSislokAtm[0].d50 * prediksiIsiAtm[0].d50)) + sislokCdm[0].d50 + sislokCrm[0].d50 - prediksiIsiAtm[0].d50 - isiCrm[0].d50 + bon[0].d50;
            saldoAkhirH.d20 = saldoAwal.d20 + (Int64)Math.Round((rasioSislokAtm[0].d20 * prediksiIsiAtm[0].d20)) + sislokCdm[0].d20 + sislokCrm[0].d20 - prediksiIsiAtm[0].d20 - isiCrm[0].d20 + bon[0].d20;
            saldoAkhirH.tgl = tanggalOptiMin;


            if (setorCounter < setor.Count && saldoAkhirH.tgl == setor[setorCounter].tgl)
            {
                saldoAkhirH.d100 -= setor[setorCounter].d100;
                saldoAkhirH.d50 -= setor[setorCounter].d50;
                saldoAkhirH.d20 -= setor[setorCounter++].d20;


            }

            //Kalau ternyata di laporan bonnya lebih dari 1 hitung sampe hari terakhir ada bon yang disetujui
            for (int a = 1; a < jumlahBonLaporan; a++)
            {
                //Hitung saldo akhir hari h
                saldoAkhirH.tgl = tanggalOptiMin.AddDays(a);
                saldoAkhirH.d100 = saldoAkhirH.d100 + (Int64)Math.Round((rasioSislokAtm[a].d100 * prediksiIsiAtm[a].d100)) + sislokCdm[a].d100 + sislokCrm[a].d100 - prediksiIsiAtm[a].d100 - isiCrm[a].d100 + bon[a].d100;
                saldoAkhirH.d50 = saldoAkhirH.d50 + (Int64)Math.Round((rasioSislokAtm[a].d50 * prediksiIsiAtm[a].d50)) + sislokCdm[a].d50 + sislokCrm[a].d50 - prediksiIsiAtm[a].d50 - isiCrm[a].d50 + bon[a].d50;
                saldoAkhirH.d20 = saldoAkhirH.d20 + (Int64)Math.Round((rasioSislokAtm[a].d20 * prediksiIsiAtm[a].d20)) + sislokCdm[a].d20 + sislokCrm[a].d20 - prediksiIsiAtm[a].d20 - isiCrm[a].d20 + bon[a].d20;


                if (setorCounter < setor.Count && saldoAkhirH.tgl == setor[setorCounter].tgl)
                {
                    saldoAkhirH.d100 -= setor[setorCounter].d100;
                    saldoAkhirH.d50 -= setor[setorCounter].d50;
                    saldoAkhirH.d20 -= setor[setorCounter++].d20;
                }
            }
            for (int a = jumlahBonLaporan; a < prediksiIsiAtm.Count - 1; a++)
            {
                Denom tempRek = new Denom();
                tempRek.tgl = tanggalOptiMin.AddDays(a);
                tempRek.d100 = (prediksiIsiAtm[a].d100 + isiCrm[a].d100) - (saldoAkhirH.d100);
                tempRek.d50 = (prediksiIsiAtm[a].d50 + isiCrm[a].d50) - (saldoAkhirH.d50);
                tempRek.d20 = (prediksiIsiAtm[a].d20 + isiCrm[a].d20) - (saldoAkhirH.d20);
                if (setorCounter < setor.Count)
                {
                    tempRek.d100 += setor[setorCounter].d100;
                    tempRek.d50 += setor[setorCounter].d50;
                    tempRek.d20 += setor[setorCounter++].d20;
                }
                rekomendasiBonE2E.Add(tempRek);
                saldoAkhirH.d100 = saldoAkhirH.d100 + (Int64)Math.Round((rasioSislokAtm[a].d100 * prediksiIsiAtm[a].d100)) + sislokCdm[a].d100 + sislokCrm[a].d100 - prediksiIsiAtm[a].d100 - isiCrm[a].d100 + tempRek.d100;
                saldoAkhirH.d50 = saldoAkhirH.d50 + (Int64)Math.Round((rasioSislokAtm[a].d50 * prediksiIsiAtm[a].d50)) + sislokCdm[a].d50 + sislokCrm[a].d50 - prediksiIsiAtm[a].d50 - isiCrm[a].d50 + tempRek.d50;
                saldoAkhirH.d20 = saldoAkhirH.d20 + (Int64)Math.Round((rasioSislokAtm[a].d20 * prediksiIsiAtm[a].d20)) + sislokCdm[a].d20 + sislokCrm[a].d20 - prediksiIsiAtm[a].d20 - isiCrm[a].d20 + tempRek.d20;
                if (setorCounter < setor.Count)
                {
                    saldoAkhirH.d100 -= setor[setorCounter - 1].d100;
                    saldoAkhirH.d50 -= setor[setorCounter - 1].d50;
                    saldoAkhirH.d20 -= setor[setorCounter - 1].d20;
                }
            }

            //Cek rasio hari terakhir
            bool flag100rasiounder = false, flag50rasiounder = false, flag20rasiounder = false;
            if (Math.Round((Double)saldoAkhirH.d100 / (prediksiIsiAtm[prediksiIsiAtm.Count - 1].d100 + isiCrm[prediksiIsiAtm.Count - 1].d100), 2) <= targetRasio100)
            {
                Console.WriteLine();
                Console.WriteLine((Double)saldoAkhirH.d100 / (prediksiIsiAtm[prediksiIsiAtm.Count - 1].d100 + isiCrm[prediksiIsiAtm.Count - 1].d100));
                flag100rasiounder = true;
            }
            if (Math.Round((Double)saldoAkhirH.d50 / (prediksiIsiAtm[prediksiIsiAtm.Count - 1].d50 + isiCrm[prediksiIsiAtm.Count - 1].d50), 2) <= targetRasio50)
            {
                Console.WriteLine((Double)saldoAkhirH.d50 / (prediksiIsiAtm[prediksiIsiAtm.Count - 1].d50 + isiCrm[prediksiIsiAtm.Count - 1].d50));
                flag50rasiounder = true;
            }
            if (Math.Round((Double)saldoAkhirH.d20 / (prediksiIsiAtm[prediksiIsiAtm.Count - 1].d20 + isiCrm[prediksiIsiAtm.Count - 1].d20), 2) <= targetRasio20)
            {
                Console.WriteLine((Double)saldoAkhirH.d20 / (prediksiIsiAtm[prediksiIsiAtm.Count - 1].d20 + isiCrm[prediksiIsiAtm.Count - 1].d20));
                flag20rasiounder = true;
            }

            if (flag20rasiounder && flag50rasiounder && flag100rasiounder)
            {
                rekomendasiBon = rekomendasiBonNonE2E;
            }
            if (!flag20rasiounder && flag50rasiounder && flag100rasiounder)
            {
                rekomendasiBon = new List<Denom>();
                int idx = 0;
                foreach (var temp in rekomendasiBonNonE2E)
                {
                    Denom temp2 = new Denom();
                    temp2.tgl = temp.tgl;
                    temp2.d20 = rekomendasiBonE2E[idx++].d20;
                    temp2.d50 = temp.d50;
                    temp2.d100 = temp.d100;
                    rekomendasiBon.Add(temp2);
                }
            }
            if (flag20rasiounder && !flag50rasiounder && flag100rasiounder)
            {
                rekomendasiBon = new List<Denom>();
                int idx = 0;
                foreach (var temp in rekomendasiBonNonE2E)
                {
                    Denom temp2 = new Denom();
                    temp2.tgl = temp.tgl;
                    temp2.d50 = rekomendasiBonE2E[idx++].d50;
                    temp2.d20 = temp.d20;
                    temp2.d100 = temp.d100;
                    rekomendasiBon.Add(temp2);
                }
            }
            if (flag20rasiounder && flag50rasiounder && !flag100rasiounder)
            {
                rekomendasiBon = new List<Denom>();
                int idx = 0;
                foreach (var temp in rekomendasiBonNonE2E)
                {
                    Denom temp2 = new Denom();
                    temp2.tgl = temp.tgl;
                    temp2.d100 = rekomendasiBonE2E[idx++].d100;
                    temp2.d50 = temp.d50;
                    temp2.d20 = temp.d20;
                    rekomendasiBon.Add(temp2);
                }
            }
            if (flag20rasiounder && !flag50rasiounder && !flag100rasiounder)
            {
                rekomendasiBon = new List<Denom>();
                int idx = 0;
                foreach (var temp in rekomendasiBonE2E)
                {
                    Denom temp2 = new Denom();
                    temp2.tgl = temp.tgl;
                    temp2.d20 = rekomendasiBonNonE2E[idx++].d20;
                    temp2.d50 = temp.d50;
                    temp2.d100 = temp.d100;
                    rekomendasiBon.Add(temp2);
                }
            }
            if (!flag20rasiounder && flag50rasiounder && !flag100rasiounder)
            {
                rekomendasiBon = new List<Denom>();
                int idx = 0;
                foreach (var temp in rekomendasiBonE2E)
                {
                    Denom temp2 = new Denom();
                    temp2.tgl = temp.tgl;
                    temp2.d50 = rekomendasiBonNonE2E[idx++].d50;
                    temp2.d20 = temp.d20;
                    temp2.d100 = temp.d100;
                    rekomendasiBon.Add(temp2);
                }
            }
            if (!flag20rasiounder && !flag50rasiounder && flag100rasiounder)
            {
                rekomendasiBon = new List<Denom>();
                int idx = 0;
                foreach (var temp in rekomendasiBonE2E)
                {
                    Denom temp2 = new Denom();
                    temp2.tgl = temp.tgl;
                    temp2.d100 = rekomendasiBonNonE2E[idx++].d100;
                    temp2.d50 = temp.d50;
                    temp2.d20 = temp.d20;
                    rekomendasiBon.Add(temp2);
                }
            }
            if (!flag20rasiounder && !flag50rasiounder && !flag100rasiounder)
            {
                rekomendasiBon = rekomendasiBonE2E;
            }
            for (int a = rekomendasiBon.Count - 1; a > 0; a--)
            {
                if (tanggalSkip.Contains(rekomendasiBon[a].tgl))
                {
                    rekomendasiBon[a - 1].d100 += rekomendasiBon[a].d100;
                    rekomendasiBon[a - 1].d50 += rekomendasiBon[a].d50;
                    rekomendasiBon[a - 1].d20 += rekomendasiBon[a].d20;

                    rekomendasiBon[a].d100 = 0;
                    rekomendasiBon[a].d50 = 0;
                    rekomendasiBon[a].d20 = 0;
                }
            }
        }
        void loadTableRekomendasiBon()
        {
            rekomendasiBonGridView.Columns.Clear();
            rekomendasiBonGridView.Rows.Clear();
            //MessageBox.Show(rekomendasiBon.Count.ToString());
            rekomendasiBonGridView.Columns.Add("Tanggal", "Tanggal");
            rekomendasiBonGridView.Columns.Add("100", "100");
            rekomendasiBonGridView.Columns.Add("50", "50");
            rekomendasiBonGridView.Columns.Add("20", "20");

            for (int i = 1; i < 4; i++)
            {
                rekomendasiBonGridView.Columns[i].DefaultCellStyle.Format = "c";
                rekomendasiBonGridView.Columns[i].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            }
            foreach (var temp in rekomendasiBon)
            {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = temp.tgl.ToShortDateString();
                row.Cells.Add(cell);
                DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
                cell1.Value = temp.d100;
                row.Cells.Add(cell1);
                DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                cell2.Value = temp.d50;
                row.Cells.Add(cell2);
                DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                cell3.Value = temp.d20;
                row.Cells.Add(cell3);
                rekomendasiBonGridView.Rows.Add(row);
            }
        }
        void loadTableBon()
        {
            bonGridView.Columns.Clear();
            bonGridView.Rows.Clear();
            //Show(rekomendasiBon.Count.ToString());
            bonGridView.Columns.Add("Tanggal", "Tanggal");
            bonGridView.Columns.Add("100", "100");
            bonGridView.Columns.Add("50", "50");
            bonGridView.Columns.Add("20", "20");
            bonGridView.Columns["Tanggal"].ReadOnly = true;
            //for (int i = 1; i < 4; i++)
            //{
            //    bonGridView.Columns[i].DefaultCellStyle.Format = "c";
            //    bonGridView.Columns[i].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            //}
            DateTime tempDate = tanggalOptiMin.AddDays(1+jumlahBonLaporan-1);
            Database1Entities db = new Database1Entities();
            var query = (from x in db.Approvals.AsEnumerable()
                         join y in db.DetailApprovals.AsEnumerable() on x.idApproval equals y.idApproval
                         where x.kodePkt == KodePkt[pktIndex]
                         && x.tanggal == DateTime.Today.Date
                         && y.tanggal > DateTime.Today.Date
                         select new { y.tanggal, y.bon100, y.bon50, y.bon20 }
                         ).ToList();
            int counter = 0;
            while (tempDate <= tanggalOptiMax.AddDays(-1))
            {
                Console.WriteLine("Masuk while");
                DataGridViewRow row = new DataGridViewRow();

                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = tempDate.ToShortDateString();
                row.Cells.Add(cell);
                cell = new DataGridViewTextBoxCell();
                cell.Value = 0;
                cell.Value = query[counter].bon100;
                row.Cells.Add(cell);
                cell = new DataGridViewTextBoxCell();
                cell.Value = 0;
                cell.Value = query[counter].bon50;
                row.Cells.Add(cell);
                cell = new DataGridViewTextBoxCell();
                cell.Value = 0;
                cell.Value = query[counter].bon20;
                row.Cells.Add(cell);

                bonGridView.Rows.Add(row);
                tempDate = tempDate.AddDays(1);

                Console.WriteLine(query[counter].bon100);
                Console.WriteLine(query[counter].bon50);
                Console.WriteLine(query[counter].bon20);
                counter++;
            }
            bonGridView.Columns["100"].DefaultCellStyle.Format = "c";
            bonGridView.Columns["100"].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            bonGridView.Columns["100"].DefaultCellStyle.NullValue = 0;
            bonGridView.Columns["50"].DefaultCellStyle.Format = "c";
            bonGridView.Columns["50"].DefaultCellStyle.NullValue = 0;
            bonGridView.Columns["50"].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            bonGridView.Columns["20"].DefaultCellStyle.Format = "c";
            bonGridView.Columns["20"].DefaultCellStyle.NullValue = 0;
            bonGridView.Columns["20"].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");

        }
        List<Denom> loadBonFromGridView()
        {
            List<Denom> bonFromGridView = new List<Denom>();

            for (int a = 0; a < bonGridView.Rows.Count; a++)
            {
                String s100 = bonGridView[1, a].Value.ToString().Replace("Rp", "").Replace(".", "").Trim(' '), 
                       s50 = bonGridView[2, a].Value.ToString().Replace("Rp", "").Replace(".", "").Trim(' '), 
                       s20 = bonGridView[3, a].Value.ToString().Replace("Rp", "").Replace(".", "").Trim(' '), 
                       sdate = bonGridView[0, a].Value.ToString();

                Console.WriteLine(s100 + " " + s50 + " " + s20);
                Int64 d100 = Int64.Parse(s100), 
                      d50 = Int64.Parse(s50), 
                      d20 = Int64.Parse(s20);
                DateTime tgl = Convert.ToDateTime(sdate);

                bonFromGridView.Add(new Denom()
                {
                    tgl = tgl,
                    d100 = d100,
                    d20 = d20,
                    d50 = d50
                });
            }
            return bonFromGridView;
        }
        //Sekalian load saldoAwal untuk setiap hari
        void loadRasio()
        {
            Console.WriteLine("RASIO & Saldo Awal");
            Console.WriteLine("===============");
            saldo = new List<Denom>();
            DateTime tanggalAwal = saldoAwal.tgl;
            DateTime tanggalAkhir = tanggalPrediksiMaxPicker.Value.Date;
            List<Denom> bonFromGridView = loadBonFromGridView();

            listRasio.RemoveAll(x=>true);

            Console.WriteLine(saldoAwal.d100 + " " + saldoAwal.d50 + " " + saldoAwal.d20);
            //Bon Adhoc
            Denom bonAdhoc = loadBonAdhocFromTxt();
            //Setor Adhoc
            Denom setorAdhoc = loadSetorAdhocFromTxt();
            //Setor
            Denom setorBaru = loadSetorFromTxt();

            DateTime tgl = saldoAwal.tgl;

            Denom saldoAkhir = new Denom() {
                tgl = saldoAwal.tgl,
                d100 = saldoAwal.d100,
                d50 = saldoAwal.d50,
                d20 = saldoAwal.d20
            };

            Console.WriteLine("Load Saldo Awal\n=================");
            while(tgl <= tanggalAkhir)
            {
                Console.WriteLine(tgl.ToShortDateString());
                Denom tIsiAtm = prediksiIsiAtm.Where(x => x.tgl.Date == tgl).FirstOrDefault(),
                      tIsiCrm = isiCrm.Where(x => x.tgl == tgl).FirstOrDefault(),
                      tSislokCrm = sislokCrm.Where(x => x.tgl == tgl).FirstOrDefault(),
                      tSislokCdm = sislokCdm.Where(x => x.tgl == tgl).FirstOrDefault(),
                      tBon = bon.Where(x => x.tgl == tgl).FirstOrDefault(),
                      tBonGridView = bonFromGridView.Where(x => x.tgl == tgl).FirstOrDefault(),
                      tSetor = setor.Where(x => x.tgl == tgl).FirstOrDefault();
                Rasio tSislokAtm = rasioSislokAtm.Where(x => x.tgl == tgl).FirstOrDefault();


                Console.WriteLine("isi ATM: " + tIsiAtm.tgl + " " + tIsiAtm.d100 + " " + tIsiAtm.d50 + " " + tIsiAtm.d50);
                Console.WriteLine("isi Crm: " + tIsiCrm.tgl + " " + tIsiCrm.d100 + " " + tIsiCrm.d50 + " " + tIsiCrm.d50);

                Console.WriteLine("Sislok ATM: " + tSislokAtm.tgl + " " + tSislokAtm.d100 + " " + tSislokAtm.d50 + " " + tSislokAtm.d50);
                Console.WriteLine("Sislok Crm: " + tSislokCrm.tgl + " " + tSislokCrm.d100 + " " + tSislokCrm.d50 + " " + tSislokCrm.d50);
                Console.WriteLine("Sislok Cdm: " + tSislokCdm.tgl + " " + tSislokCdm.d100 + " " + tSislokCdm.d50 + " " + tSislokCdm.d50);

                


                saldo.Add(new Denom()
                {
                    tgl = tgl,
                    d100 = saldoAkhir.d100,
                    d50 = saldoAkhir.d50,
                    d20 = saldoAkhir.d20
                });
                listRasio.Add(new Rasio()
                {
                    tgl = tgl,
                    d100 = saldoAkhir.d100 / ((Double)(tIsiAtm.d100 + tIsiCrm.d100)),
                    d50 = saldoAkhir.d50 / ((Double)(tIsiAtm.d50 + tIsiCrm.d50)),
                    d20 = saldoAkhir.d20 / ((Double)(tIsiAtm.d20 + tIsiCrm.d20)),
                });

                

                if (tIsiAtm == null)
                    Console.WriteLine("T Isi ATM NULL");
                if (tIsiCrm == null)
                    Console.WriteLine("T Isi CRM NULL");
                if (tSislokCrm == null)
                    Console.WriteLine("T Sislok CRM NULL");
                if (tSislokCdm == null)
                    Console.WriteLine("T Sislok CDM NULL");

                saldoAkhir.d100 = saldoAkhir.d100 - tIsiAtm.d100 - tIsiCrm.d100 + tSislokCrm.d100 + tSislokCdm.d100 + ((Int64)Math.Round((tSislokAtm.d100 * tIsiAtm.d100),0));
                saldoAkhir.d50 = saldoAkhir.d50 - tIsiAtm.d50 - tIsiCrm.d50 + tSislokCrm.d50 + tSislokCdm.d50 + ((Int64)Math.Round((tSislokAtm.d50 * tIsiAtm.d50),0));
                saldoAkhir.d20 = saldoAkhir.d20 - tIsiAtm.d20 - tIsiCrm.d20 + tSislokCrm.d20 + tSislokCdm.d20 + ((Int64)Math.Round((tSislokAtm.d20 * tIsiAtm.d20),0));

                if (tgl == saldoAwal.tgl)
                {
                    saldoAkhir.d100 += bonAdhoc.d100;
                    saldoAkhir.d50 += bonAdhoc.d50;
                    saldoAkhir.d20 += bonAdhoc.d20;

                    saldoAkhir.d100 -= setorAdhoc.d100;
                    saldoAkhir.d50 -= setorAdhoc.d50;
                    saldoAkhir.d20 -= setorAdhoc.d20;
                }

                if(setorBaru.tgl == tgl)
                {
                    saldoAkhir.d100 -= setorBaru.d100;
                    saldoAkhir.d50 -= setorBaru.d50;
                    saldoAkhir.d20 -= setorBaru.d20;
                }
                if (tSetor != null)
                {
                    saldoAkhir.d100 -= tSetor.d100;
                    saldoAkhir.d50 -= tSetor.d50;
                    saldoAkhir.d20 -= tSetor.d20;
                }
                if(tBon != null)
                {
                    Console.WriteLine("Bon: " + tBon.tgl + " " + tBon.d100 + " " + tBon.d50 + " " + tBon.d20);
                    saldoAkhir.d100 += tBon.d100;
                    saldoAkhir.d50 += tBon.d50;
                    saldoAkhir.d20 += tBon.d20;
                }
                else if(tBonGridView != null)
                {
                    Console.WriteLine("BonGridView: " + tBonGridView.tgl + " " + tBonGridView.d100 + " " + tBonGridView.d50 + " " + tBonGridView.d20);
                    saldoAkhir.d100 += tBonGridView.d100;
                    saldoAkhir.d50 += tBonGridView.d50;
                    saldoAkhir.d20 += tBonGridView.d20;
                }
                else
                {
                   
                }
                tgl = tgl.AddDays(1);
                
            }

                    
            
        }
        void loadTableRasio()
        {
            rasioGridView.Enabled = false;
            rasioGridView.Columns.Clear();
            rasioGridView.Rows.Clear();
            rasioGridView.Columns.Add("Tanggal", "Tanggal");
            rasioGridView.Columns.Add("100", "100");
            rasioGridView.Columns.Add("50", "50");
            rasioGridView.Columns.Add("20", "20");
            foreach(var temp in listRasio)
            {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = temp.tgl.ToShortDateString();
                row.Cells.Add(cell);
                cell = new DataGridViewTextBoxCell();
                cell.Value = Math.Round(temp.d100,2);
                row.Cells.Add(cell);
                cell = new DataGridViewTextBoxCell();
                cell.Value = Math.Round(temp.d50,2);
                row.Cells.Add(cell);
                cell = new DataGridViewTextBoxCell();
                cell.Value = Math.Round(temp.d20,2);
                row.Cells.Add(cell);
                rasioGridView.Rows.Add(row);
            }
        }
        void loadTableSaldo()
        {   
            DataTable table = new DataTable();
            using (var reader = ObjectReader.Create(saldo, "tgl","d100","d50","d20"))
            {
                table.Load(reader);
            }
            table.Columns["tgl"].ColumnName = "Tanggal";
            table.Columns["d100"].ColumnName = "100";
            table.Columns["d50"].ColumnName = "50";
            table.Columns["d20"].ColumnName = "20";
            saldoGridView.DataSource = table;
            saldoGridView.Columns["100"].DefaultCellStyle.Format = "n0";
            saldoGridView.Columns["50"].DefaultCellStyle.Format = "n0";
            saldoGridView.Columns["20"].DefaultCellStyle.Format = "n0";
        }
        void loadTableTotalIsi()
        {
            DataTable table = new DataTable();
            List<Denom> totalIsi = new List<Denom>();
            for (int i=0;i<prediksiIsiAtm.Count;i++)
            {
                var temp = prediksiIsiAtm[i];
                Denom tempD = new Denom();
                tempD.tgl = temp.tgl;
                tempD.d100 = temp.d100 + isiCrm[i].d100;
                tempD.d50 = temp.d50 + isiCrm[i].d50;
                tempD.d20 = temp.d20 + isiCrm[i].d20;
                totalIsi.Add(tempD);
            }

            using (var reader = ObjectReader.Create(totalIsi, "tgl", "d100", "d50", "d20"))
            {
                table.Load(reader);
            }
            table.Columns["tgl"].ColumnName = "Tanggal";
            table.Columns["d100"].ColumnName = "100";
            table.Columns["d50"].ColumnName = "50";
            table.Columns["d20"].ColumnName = "20";

            Int64 avg100 = (Int64)Math.Round(totalIsi.Average(x => x.d100), 0);
            Int64 avg50 = (Int64)Math.Round(totalIsi.Average(x => x.d50), 0);
            Int64 avg20 = (Int64)Math.Round(totalIsi.Average(x => x.d20), 0);

            DataRow row = table.NewRow();
            row["Tanggal"] = new DateTime(0001, 1, 1);
            row["100"] = avg100;
            row["50"] = avg50;
            row["20"] = avg20;
            table.Rows.Add(row);

            
            totalIsiGridView.DataSource = table;
            totalIsiGridView.Columns["100"].DefaultCellStyle.Format = "n0";
            totalIsiGridView.Columns["50"].DefaultCellStyle.Format = "n0";
            totalIsiGridView.Columns["20"].DefaultCellStyle.Format = "n0";
            totalIsiGridView.Rows[totalIsiGridView.Rows.Count-1].DefaultCellStyle.BackColor = Color.PapayaWhip;
        }
        void loadTableTotalSislok()
        {
            DataTable table = new DataTable();
            List<Denom> totalSislok = new List<Denom>();

            Console.WriteLine("");
            Console.WriteLine("Load Table Total Sislok");
            Console.WriteLine("========================");

            for (int i = 0; i < prediksiIsiAtm.Count; i++)
            {
                var temp = prediksiIsiAtm[i];
                Denom tempD = new Denom();
                Console.WriteLine(temp.d100 +" "+rasioSislokAtm[i].d100+" "+temp.d100 * rasioSislokAtm[i].d100);
                tempD.tgl = temp.tgl;
                tempD.d100 = (Int64) Math.Round((temp.d100* rasioSislokAtm[i].d100),0) + sislokCdm[i].d100 + sislokCrm[i].d100;
                tempD.d50 = (Int64)Math.Round((temp.d50 * rasioSislokAtm[i].d50), 0) + sislokCdm[i].d50 + sislokCrm[i].d50;
                tempD.d20 = (Int64)Math.Round((temp.d20 * rasioSislokAtm[i].d20), 0) + sislokCdm[i].d20 + sislokCrm[i].d20;
                totalSislok.Add(tempD);
            }

            using (var reader = ObjectReader.Create(totalSislok, "tgl", "d100", "d50", "d20"))
            {
                table.Load(reader);
            }
            table.Columns["tgl"].ColumnName = "Tanggal";
            table.Columns["d100"].ColumnName = "100";
            table.Columns["d50"].ColumnName = "50";
            table.Columns["d20"].ColumnName = "20";

            Int64 avg100 = (Int64)Math.Round(totalSislok.Average(x => x.d100), 0);
            Int64 avg50 = (Int64)Math.Round(totalSislok.Average(x => x.d50), 0);
            Int64 avg20 = (Int64)Math.Round(totalSislok.Average(x => x.d20), 0);

            DataRow row = table.NewRow();
            row["Tanggal"] = new DateTime(0001, 1, 1);
            row["100"] = avg100;
            row["50"] = avg50;
            row["20"] = avg20;
            table.Rows.Add(row);
            totalSislokGridView.DataSource = table;
            totalSislokGridView.Columns["100"].DefaultCellStyle.Format = "n0";
            totalSislokGridView.Columns["50"].DefaultCellStyle.Format = "n0";
            totalSislokGridView.Columns["20"].DefaultCellStyle.Format = "n0";

            totalSislokGridView.Rows[totalSislokGridView.Rows.Count - 1].DefaultCellStyle.BackColor = Color.PapayaWhip;

        }
        void loadTableBonYangDisetujui()
        {
            DataTable table = new DataTable();
            using (var reader = ObjectReader.Create(bon, "tgl", "d100", "d50", "d20"))
            {
                table.Load(reader);
            }
            table.Columns["tgl"].ColumnName = "Tanggal";
            table.Columns["d100"].ColumnName = "100";
            table.Columns["d50"].ColumnName = "50";
            table.Columns["d20"].ColumnName = "20";
            bonYangSudahDisetujuiGridView.DataSource = table;
            bonYangSudahDisetujuiGridView.Columns["100"].DefaultCellStyle.Format = "n0";
            bonYangSudahDisetujuiGridView.Columns["50"].DefaultCellStyle.Format = "n0";
            bonYangSudahDisetujuiGridView.Columns["20"].DefaultCellStyle.Format = "n0";
        }

        void loadCheckedDariSkipPrediksiTreeView()
        {
            tanggalSkip = new List<DateTime>();
            for(int a=0;a<skipPrediksiTreeView.Nodes.Count;a++)
            {
                for(int b=0;b<skipPrediksiTreeView.Nodes[a].Nodes.Count;b++)
                {
                    for(int c = 0;c < skipPrediksiTreeView.Nodes[a].Nodes[b].Nodes.Count;c++)
                    {
                        if (skipPrediksiTreeView.Nodes[a].Nodes[b].Nodes[c].Checked == true)
                            tanggalSkip.Add(new DateTime(Int32.Parse(skipPrediksiTreeView.Nodes[a].Text), Int32.Parse(skipPrediksiTreeView.Nodes[a].Nodes[b].Text), Int32.Parse(skipPrediksiTreeView.Nodes[a].Nodes[b].Nodes[c].Text)));
                    }
                }
            }
            foreach(var temp in tanggalSkip)
            {
                Console.WriteLine(temp);
            }
        }
        void loadPermintaanAdhoc()
        {
            Database1Entities db = new Database1Entities();
            var listPermintaanAdhoc = (from x in db.LaporanPermintaanAdhocs.AsEnumerable()
                                                                where x.tanggal == DateTime.Today.Date
                                                                && x.kodePkt == KodePkt[pktIndex]
                                                                select new { x.tanggal,  x.C100, x.C50, x.C20 }).ToList();
            permintaanAdhocGridView.DataSource = listPermintaanAdhoc;
            permintaanAdhocGridView.Columns[1].DefaultCellStyle.Format = "C";
            permintaanAdhocGridView.Columns[2].DefaultCellStyle.Format = "C";
            permintaanAdhocGridView.Columns[3].DefaultCellStyle.Format = "C";

            permintaanAdhocGridView.Columns[1].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            permintaanAdhocGridView.Columns[2].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            permintaanAdhocGridView.Columns[3].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
        }
        private void loadPrediksiBtn_Click(object sender, EventArgs e)
        {
            Double buf;
            loadPrediksiOpti();
            loadCheckedDariSkipPrediksiTreeView();
            tanggalOptiMax = tanggalPrediksiMaxPicker.Value.Date;
            tanggalOptiMin = DateTime.Today.Date;
            if (tanggalKalenderMax < tanggalOptiMax)
            {
                MessageBox.Show("Data tanggal di table kalender kurang");
            }
            else if ((!Double.TryParse(rasio100Txt.Text, out buf) || !Double.TryParse(rasio50Txt.Text, out buf) || !Double.TryParse(rasio20Txt.Text,out buf)) && (!String.IsNullOrEmpty(rasio100Txt.Text) && !String.IsNullOrEmpty(rasio50Txt.Text) && !String.IsNullOrEmpty(rasio20Txt.Text) ))
            {
                MessageBox.Show("Target rasio harus berupa angka!");
            }
            else
            {
            //loadSislokCrm();
            //try
            //{
                if(MetodePrediksiComboBox.SelectedIndex == 0)
                {
                    Database1Entities db = new Database1Entities();
                    if(!(from x in db.Optis select x).Any())
                    {
                        MessageBox.Show("Data Opti Tidak Ada!");
                        return;
                    }
                    if(prediksiIsiAtmOpti[prediksiIsiAtmOpti.Count-1].tgl < tanggalOptiMax)
                    {
                        MessageBox.Show("Data Opti Salah");
                        return;
                    }
                    else
                        prediksiIsiAtm = prediksiIsiAtmOpti;
                }
                if (MetodePrediksiComboBox.SelectedIndex == 1)
                {
                    loadIsiAtmHistoris();
                }
                if (MetodePrediksiComboBox.SelectedIndex == 2)
                {
                    loadIsiAtmHistoris();
                    for(int a=0;a<prediksiIsiAtmOpti.Count;a++)
                    {
                        prediksiIsiAtm[a].d100 = (prediksiIsiAtm[a].d100 + prediksiIsiAtmOpti[a].d100) / 2;
                        prediksiIsiAtm[a].d50 = (prediksiIsiAtm[a].d50 + prediksiIsiAtmOpti[a].d50) / 2;
                        prediksiIsiAtm[a].d20 = (prediksiIsiAtm[a].d20 + prediksiIsiAtmOpti[a].d20) / 2;
                    }
                }
                if (MetodePrediksiComboBox.SelectedIndex == 3)
                {
                    loadIsiAtmHistoris();
                    loadIsiAtmHistorisDenganStandarDeviasi();
                    prediksiIsiAtm = prediksiIsiAtmDenganStdDeviasi;
                }
                if (MetodePrediksiComboBox.SelectedIndex == 4)
                {
                    loadIsiAtmHistoris();
                    loadIsiAtmHistorisDenganStandarDeviasi();
                    prediksiIsiAtm = prediksiIsiAtmDenganStdDeviasi;
                    for (int a = 0; a < prediksiIsiAtmOpti.Count; a++)
                    {
                        prediksiIsiAtm[a].d100 = (prediksiIsiAtm[a].d100 + prediksiIsiAtmOpti[a].d100) / 2;
                        prediksiIsiAtm[a].d50 = (prediksiIsiAtm[a].d50 + prediksiIsiAtmOpti[a].d50) / 2;
                        prediksiIsiAtm[a].d20 = (prediksiIsiAtm[a].d20 + prediksiIsiAtmOpti[a].d20) / 2;
                    }

                }
                loadSislokCdm();
             
                    
                loadRasioSislokAtm();
                loadRasioSislokAtmDenganStdDeviasi();

                loadSaldoAwal();
                loadBon();
                loadTablePermintaanBon();

                loadIsiCrm();

                loadSislokCrm();
                   
                loadSetor();

                //Hitungan dengan metode kedua
                if(MetodeHitungLainnyaComboBox.SelectedIndex == 1)
                {
                    loadSislokCdmDenganStdDeviasi();
                    loadIsiCrmDenganStdDeviasi();
                    loadSislokCrmDenganStdDeviasi();
                    loadRasioSislokAtmDenganStdDeviasi();
                    sislokCdm = sislokCdmDenganStdDeviasi;
                    isiCrm = isiCrmDenganStdDeviasi;
                    sislokCrm = sislokCrmDenganStdDeviasi;
                    rasioSislokAtm = rasioSislokATMDenganStdDeviasi;
                }
                //Hitungan dengan metode ketiga
                if(MetodeHitungLainnyaComboBox.SelectedIndex == 2)
                {
                    loadSislokCdmDenganStdDeviasi();
                    loadIsiCrmDenganStdDeviasi();
                    loadSislokCrmDenganStdDeviasi();
                    loadRasioSislokAtmDenganStdDeviasi();
                    for (int a = 0; a < sislokCdm.Count; a++)
                    {
                        sislokCdm[a].d100 = (sislokCdm[a].d100 + sislokCdmDenganStdDeviasi[a].d100) / 2;
                        sislokCdm[a].d50 = (sislokCdm[a].d50 + sislokCdmDenganStdDeviasi[a].d50) / 2;
                        sislokCdm[a].d20 = (sislokCdm[a].d20 + sislokCdmDenganStdDeviasi[a].d20) / 2;

                        isiCrm[a].d100 = (isiCrm[a].d100 + isiCrmDenganStdDeviasi[a].d100) / 2;
                        isiCrm[a].d50 = (isiCrm[a].d50 + isiCrmDenganStdDeviasi[a].d50) / 2;
                        isiCrm[a].d20 = (isiCrm[a].d20 + isiCrmDenganStdDeviasi[a].d20) / 2;

                        sislokCrm[a].d100 = (sislokCrm[a].d100 + sislokCrmDenganStdDeviasi[a].d100) / 2;
                        sislokCrm[a].d50 = (sislokCrm[a].d50 + sislokCrmDenganStdDeviasi[a].d50) / 2;
                        sislokCrm[a].d20 = (sislokCrm[a].d20 + sislokCrmDenganStdDeviasi[a].d20) / 2;

                        rasioSislokAtm[a].d100 = (rasioSislokAtm[a].d100 + rasioSislokATMDenganStdDeviasi[a].d100) / 2;
                        rasioSislokAtm[a].d50 = (rasioSislokAtm[a].d50 + rasioSislokATMDenganStdDeviasi[a].d50) / 2;
                        rasioSislokAtm[a].d20 = (rasioSislokAtm[a].d20 + rasioSislokATMDenganStdDeviasi[a].d20) / 2;
                    }
                }

                //BUFFER ISI ATM
                Double bufferIsiATM100 = (Double)bufferIsiAtm100Num.Value / 100;
                Double bufferIsiATM50 = (Double)bufferIsiAtm50Num.Value / 100;
                foreach (var temp in prediksiIsiAtm)
                {
                    temp.d100 = (Int64)(temp.d100 * (1 + bufferIsiATM100));
                    temp.d50 = (Int64)(temp.d50 * (1 + bufferIsiATM50));
                }

                if (e2eComboBox.SelectedIndex == 1)
                    loadRekomendasiBonNonE2E();
                else
                    loadRekomendasiBonE2E();
                loadTableRekomendasiBon();

                loadTableBon();

                loadRasio();
                loadTableRasio();
                loadTableSaldo();

                loadTableTotalIsi();
                loadTableTotalSislok();
                loadTableBonYangDisetujui();

                loadPermintaanAdhoc();

                List<DetailApproval> list = loadDetailApproval();
                loadSetorFromApproval(list);
                loadAdhocFromApproval(list);
                loadSetorAdhocFromApproval(list);
                //}catch(Exception err)
                //{
                //    MessageBox.Show(err.ToString());
                //}
            }
        }
        List<DetailApproval> loadDetailApproval()
        {
            Database1Entities db = new Database1Entities();
            String kodePkt = KodePkt[pktIndex];
            DateTime today = DateTime.Today.Date;
            List<Approval> q = (
                    from x in db.Approvals
                    where x.kodePkt == kodePkt
                    && x.tanggal == today
                    orderby x.tanggal descending
                    select x).ToList();
            Approval lastApproval = q[0];
            List<DetailApproval> res = (from x in db.DetailApprovals
                                  where x.idApproval == lastApproval.idApproval
                                  select x).ToList();
            return res;
        }
        void loadSetorFromApproval(List<DetailApproval> list)
        {
            var temp = list.Where(x => !String.IsNullOrEmpty(x.setor100.ToString())).ToList();
            if(temp.Any())
            {
                setor100Txt.Value = (decimal)temp[0].setor100;
                setor50Txt.Value = (decimal)temp[0].setor50;
                setor20Txt.Value = (decimal)temp[0].setor20;
                tglSetor.Value = (DateTime)temp[0].tanggal;
            }
        }
        void loadAdhocFromApproval(List<DetailApproval> list)
        {
            var temp = list[0];
            bonAdhoc100Txt.Value = (decimal)temp.adhoc100;
            bonAdhoc50Txt.Value = (decimal)temp.adhoc50;
            bonAdhoc20Txt.Value = (decimal)temp.adhoc20;
        }
        void loadSetorAdhocFromApproval(List<DetailApproval> list)
        {
            var temp = list[0];
            if (!String.IsNullOrEmpty(temp.setorAdhoc100.ToString()))
                setorAdhoc100Txt.Value = (decimal)temp.setorAdhoc100;
            else
                setorAdhoc100Txt.Value = 0;
            if (!String.IsNullOrEmpty(temp.setorAdhoc50.ToString()))
                setorAdhoc50Txt.Value = (decimal)temp.setorAdhoc50;
            else
                setorAdhoc50Txt.Value = 0;
            if (!String.IsNullOrEmpty(temp.setorAdhoc20.ToString()))
                setorAdhoc20Txt.Value = (decimal)temp.setorAdhoc20;
            else
                setorAdhoc20Txt.Value = 0;
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            CheckTreeViewNode(e.Node, e.Node.Checked);
        }
        private void treeView1_ParentChanged(object sender, EventArgs e)
        {
            
        }
        void pktComboBox_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }
        private void CheckTreeViewNode(TreeNode node, Boolean isChecked)
        {
            foreach (TreeNode item in node.Nodes)
            {
                item.Checked = isChecked;

                if (item.Nodes.Count > 0)
                {
                    this.CheckTreeViewNode(item, isChecked);
                }
            }
        }
        private void approveButton_Click(object sender, EventArgs e)
        {
            Database1Entities db = new Database1Entities();
            if(MessageBox.Show("Approve Bon?", "Approve Bon", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                List<Denom> bonYangDisetujui = loadBonYangDisetujuiFromTable();
                Denom bonAdhoc = loadBonAdhocFromTxt();
                Denom setorAdhoc = loadSetorAdhocFromTxt();
                Denom setor = loadSetorFromTxt();
                
                Approval newA = new Approval();
                newA.tanggal = tanggalOptiMin;
                newA.kodePkt = KodePkt[pktIndex];
                var lastApproval = (from x in db.Approvals.AsEnumerable()
                                    where x.kodePkt == KodePkt[pktIndex]
                                    && x.tanggal == DateTime.Today.Date
                                    select x).ToList();
                DateTime tempTanggal = DateTime.Today.Date;
                int count = 0;
                int jumlahBon = 0;

                for (int i = 0; i < bon.Count; i++)
                {
                    tempTanggal = tempTanggal.AddDays(1);
                    jumlahBon++;
                }
                for (int i=0;i<=bonYangDisetujui.Count ;i++)
                {
                    DetailApproval newDetailA = (from x in db.DetailApprovals.AsEnumerable()
                                                 where x.idApproval == lastApproval[lastApproval.Count - 1].idApproval
                                                 && x.tanggal == tempTanggal
                                                 select x).FirstOrDefault();
                    newDetailA.idApproval = lastApproval[lastApproval.Count - 1].idApproval;
                    newDetailA.tanggal = tempTanggal;

                    if(newDetailA.tanggal.Value.ToShortDateString()==tglSetor.Value.ToShortDateString())
                    {
                        newDetailA.setor100 = setor.d100;
                        newDetailA.setor50 = setor.d50;
                        newDetailA.setor20 = setor.d20;
                    }
                    
                    if(i<bonYangDisetujui.Count)
                    {
                        //Bon
                        newDetailA.bon100 = bonYangDisetujui[i].d100;
                        newDetailA.bon50 = bonYangDisetujui[i].d50;
                        newDetailA.bon20 = bonYangDisetujui[i].d20;
                    }                              
                    else
                    {
                        newDetailA.bon100 = 0;
                        newDetailA.bon50 = 0;
                        newDetailA.bon20 = 0;
                    }

                    //Saldo awal
                    newDetailA.saldoAwal100 = saldo[i + jumlahBon].d100;
                    newDetailA.saldoAwal50 = saldo[i + jumlahBon].d50;
                    newDetailA.saldoAwal20 = saldo[i + jumlahBon].d20;

                    //Sislok
                    newDetailA.sislokCRM100 = sislokCrm[count].d100;
                    newDetailA.sislokCRM50 = sislokCrm[count].d50;
                    newDetailA.sislokCRM20 = sislokCrm[count].d20;
                    newDetailA.sislokCDM100 = sislokCdm[count].d100;
                    newDetailA.sislokCDM50 = sislokCdm[count].d50;
                    newDetailA.sislokCDM20 = sislokCdm[count].d20;
                    newDetailA.sislokATM100 =(Int64)(rasioSislokAtm[count].d100 * prediksiIsiAtm[count].d100);
                    newDetailA.sislokATM50 = (Int64)(rasioSislokAtm[count].d50 * prediksiIsiAtm[count].d50);
                    newDetailA.sislokATM20 = (Int64)(rasioSislokAtm[count].d20 * prediksiIsiAtm[count].d20);

                    //Isi
                    newDetailA.isiATM100 = prediksiIsiAtm[count].d100;
                    newDetailA.isiATM50 = prediksiIsiAtm[count].d50;
                    newDetailA.isiATM20 = prediksiIsiAtm[count].d20;
                    newDetailA.isiCRM100 = isiCrm[count].d100;
                    newDetailA.isiCRM50 = isiCrm[count].d50;
                    newDetailA.isiCRM20 = isiCrm[count].d20;

                    tempTanggal = tempTanggal.AddDays(1);


                    
                    count++;
                }
                DateTime today = DateTime.Today.Date;
                var adhocs = (from x in db.DetailApprovals.AsEnumerable()
                              where x.idApproval == lastApproval[lastApproval.Count - 1].idApproval
                              && x.tanggal == today
                              select x).FirstOrDefault();
                if(adhocs!=null)
                {
                    adhocs.adhoc100 = bonAdhoc.d100;
                    adhocs.adhoc50 = bonAdhoc.d50;
                    adhocs.adhoc20 = bonAdhoc.d20;
                    adhocs.setorAdhoc100 = setorAdhoc.d100;
                    adhocs.setorAdhoc50 = setorAdhoc.d50;
                    adhocs.setorAdhoc20 = setorAdhoc.d20;
                }


                db.SaveChanges();
                loadForm.ShowSplashScreen();
                loadComboBox();
                loadForm.CloseForm();
                MessageBox.Show("Approved!");
                bonAdhoc100Txt.Value = 0;
                bonAdhoc50Txt.Value = 0;
                bonAdhoc20Txt.Value = 0;

                setor100Txt.Value = 0;
                setor50Txt.Value = 0;
                setor20Txt.Value = 0;

                setorAdhoc100Txt.Value = 0;
                setorAdhoc50Txt.Value = 0;
                setorAdhoc20Txt.Value = 0;

                cleanGridViews();
            }
            else
            {

            }
            
        }
        void cleanGridViews()
        {
            //Cleaning Gridviews
            bonGridView.Rows.Clear();
            permintaanBonGridView.Rows.Clear();
            rasioGridView.Rows.Clear();
            rekomendasiBonGridView.Rows.Clear();


            bonYangSudahDisetujuiGridView.DataSource = null;
            totalSislokGridView.DataSource = null;
            totalIsiGridView.DataSource = null;
            saldoGridView.DataSource = null;

        }
        Denom loadBonAdhocFromTxt()
        {
            Denom bonAdhoc = new Denom();
            bonAdhoc.d100 = (Int64) bonAdhoc100Txt.Value;
            bonAdhoc.d50 = (Int64) bonAdhoc50Txt.Value;
            bonAdhoc.d20 = (Int64)bonAdhoc20Txt.Value;
            return bonAdhoc;
        }
        private Denom loadSetorAdhocFromTxt()
        {
            Denom setorAdhoc = new Denom();
            //Setor Adhoc
            setorAdhoc.d100 = (Int64)setorAdhoc100Txt.Value;
            setorAdhoc.d50 = (Int64)setorAdhoc50Txt.Value;
            setorAdhoc.d20 = (Int64)setorAdhoc20Txt.Value;
            return setorAdhoc;
        }
        private Denom loadSetorFromTxt()
        {
            Denom setorBaru = new Denom();
            //Setor
            setorBaru.tgl = tglSetor.Value;
            setorBaru.d100 = (Int64)setor100Txt.Value;
            setorBaru.d50 = (Int64)setor50Txt.Value;
            setorBaru.d20 = (Int64)setor20Txt.Value;
            return setorBaru;
        }
        private List<Denom> loadBonYangDisetujuiFromTable()
        {
            List<Denom> bonYangDisetujui = new List<Denom>();
            for (int i = 0; i < bonGridView.Rows.Count; i++)
            {
                Denom temp = new Denom();
                temp.tgl = Convert.ToDateTime(bonGridView.Rows[i].Cells[0].Value.ToString());
                temp.d100 = Int64.Parse(bonGridView.Rows[i].Cells[1].Value.ToString());
                temp.d50 = Int64.Parse(bonGridView.Rows[i].Cells[2].Value.ToString());
                temp.d20 = Int64.Parse(bonGridView.Rows[i].Cells[3].Value.ToString());
                bonYangDisetujui.Add(temp);
            }
            return bonYangDisetujui;
        }
        private void bonGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            for (int a = 0; a < bonGridView.Rows.Count; a++)
            {
                for (int b = 1; b < bonGridView.Columns.Count; b++)
                {
                    if (bonGridView.Rows[a].Cells[b].Value.ToString().Trim() != "")
                    {
                        Int64 buf;
                        if(Int64.TryParse(bonGridView.Rows[a].Cells[b].Value.ToString(), out buf))
                            bonGridView.Rows[a].Cells[b].Value = Int64.Parse(bonGridView.Rows[a].Cells[b].Value.ToString().Trim());
                    }
                }
            }
            loadRasio();
            loadTableRasio();
            loadTableSaldo();
        }
       
        private void Txt_TextChanged(object sender, EventArgs e)
        {
            
        }
        private void tglSetor_ValueChanged(object sender, EventArgs e)
        {
            Console.WriteLine(tglSetor.Value.ToShortDateString() == new DateTime(2018,3,23).ToShortDateString());
        }
        private void bonGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            int rowidx = e.RowIndex , colidx = e.ColumnIndex;
            bonGridView.Rows[rowidx].Cells[colidx].Value = bonGridView.Rows[rowidx].Cells[colidx].Value.ToString();
        }
        private void rekomendasiBonGridView_SelectionChanged(object sender, EventArgs e)
        {
            sumLabel.Text = "SUM: ";
            var temp = rekomendasiBonGridView.SelectedCells;
            Int64 sum = 0;
            for(int a=0;a<temp.Count;a++)
            {
                var temp2 = temp[a].Value;
                Int64 buf;
                if (Int64.TryParse(temp2.ToString(), out buf))
                {
                    Console.WriteLine(temp2);
                    sum += Int64.Parse(temp2.ToString());
                }
            }
            sumLabel.Text += "Rp. ";
            sumLabel.Text += sum.ToString("#,##0");
        }
        private void tanggalPrediksiMaxPicker_ValueChanged(object sender, EventArgs e)
        {
            skipPrediksiTreeView.CheckBoxes = true;
            Console.WriteLine(skipPrediksiTreeView.GetNodeCount(false));
            skipPrediksiTreeView.Nodes.Clear();
            for (int year = DateTime.Today.Year; year <= tanggalPrediksiMaxPicker.Value.Year; year++)
            {
                skipPrediksiTreeView.Nodes.Add(year.ToString());
                if (year == DateTime.Today.Year)
                    loadMonthNodes(year, true);
                else
                    loadMonthNodes(year, false);
            }
        }
        void loadMonthNodes(int year, bool first)
        {
            if (year < tanggalPrediksiMaxPicker.Value.Year)
            {
                int fmonth = DateTime.Today.Month;
                if (!first)
                    fmonth = 1;
                for (int month = fmonth; month <= 12; month++)
                {
                    skipPrediksiTreeView.Nodes[year - DateTime.Today.Year].Nodes.Add(month.ToString());
                    if (DateTime.Today.Month == month && DateTime.Today.Year == year)
                        loadDayNodes(year, month, true, true);
                    else if(DateTime.Today.Year == year)
                        loadDayNodes(year, month, false ,true);
                    else
                        loadDayNodes(year, month, false, false);
                }
            }
            else
            {
                int fmonth = DateTime.Today.Month;
                if (!first)
                    fmonth = 1;
                for (int month = fmonth; month <= tanggalPrediksiMaxPicker.Value.Month; month++)
                {
                    skipPrediksiTreeView.Nodes[year - DateTime.Today.Year].Nodes.Add(month.ToString());
                    if (DateTime.Today.Month == month && DateTime.Today.Year == year)
                        loadDayNodes(year, month, true, true);
                    else if (DateTime.Today.Year == year)
                        loadDayNodes(year, month, false, true);
                    else
                        loadDayNodes(year, month, false, false);
                }
            }
        }
        void loadDayNodes(int year, int month, bool todayMonth, bool todayYear)
        {
            
                if (year < tanggalPrediksiMaxPicker.Value.Year)
                {
                    int fday = DateTime.Today.Day;
                    if (!todayMonth || !todayYear)
                        fday = 1;
                    for (int day = fday; day <= DateTime.DaysInMonth(year, month); day++)
                    {
                        skipPrediksiTreeView.Nodes[year - DateTime.Today.Year].Nodes[month - DateTime.Today.Month].Nodes.Add(day.ToString());
                    }
                }
                else
                {
                    if (year == DateTime.Today.Year)
                    {
                        if (month < tanggalPrediksiMaxPicker.Value.Month)
                        {
                            int fday = DateTime.Today.Day;
                            if (!todayMonth || !todayYear)
                                fday = 1;
                            for (int day = fday; day <= DateTime.DaysInMonth(year, month); day++)
                            {
                                skipPrediksiTreeView.Nodes[year - DateTime.Today.Year].Nodes[month - DateTime.Today.Month].Nodes.Add(day.ToString());
                            }
                        }
                        else
                        {
                            int fday = DateTime.Today.Day;
                            if (!todayMonth || !todayYear)
                                fday = 1;
                            for (int day = fday; day <= tanggalPrediksiMaxPicker.Value.Day; day++)
                            {
                                skipPrediksiTreeView.Nodes[year - DateTime.Today.Year].Nodes[month - DateTime.Today.Month].Nodes.Add(day.ToString());
                            }
                        }
                    }
                    else
                    {
                        if (month < tanggalPrediksiMaxPicker.Value.Month)
                        {
                            int fday = DateTime.Today.Day;
                            if (!todayMonth || !todayYear)
                                fday = 1;
                            for (int day = fday; day <= DateTime.DaysInMonth(year, month); day++)
                            {
                                skipPrediksiTreeView.Nodes[year - DateTime.Today.Year].Nodes[month - 1].Nodes.Add(day.ToString());
                            }
                        }
                        else
                        {
                            int fday = DateTime.Today.Day;
                            if (!todayMonth || !todayYear)
                                fday = 1;
                            for (int day = fday; day <= tanggalPrediksiMaxPicker.Value.Day; day++)
                            {
                                skipPrediksiTreeView.Nodes[year - DateTime.Today.Year].Nodes[month - 1].Nodes.Add(day.ToString());
                            }
                        }
                    }
                }
           
        }

        private void bonGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            int rowidx = e.RowIndex, colidx = e.ColumnIndex;
            bonGridView.Rows[rowidx].Cells[colidx].Value = bonGridView.Rows[rowidx].Cells[colidx].Value.ToString();
        }

        private void bonGridView_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewSelectedCellCollection cells = bonGridView.SelectedCells;
            foreach (DataGridViewCell cell in cells)
            {
                int rowidx = cell.RowIndex;
                int colidx = cell.ColumnIndex;
                bonGridView.Rows[rowidx].Cells[colidx].Value = bonGridView.Rows[rowidx].Cells[colidx].Value.ToString();
            }
            for(int a=0;a<bonGridView.Rows.Count;a++)
            {
                for(int b=0;b<bonGridView.Columns.Count;b++)
                {
                    if(!cells.Contains(bonGridView.Rows[a].Cells[b]))
                    {
                        Int64 buf;
                        if (Int64.TryParse(bonGridView.Rows[a].Cells[b].Value.ToString(), out buf))
                            bonGridView.Rows[a].Cells[b].Value = Int64.Parse(bonGridView.Rows[a].Cells[b].Value.ToString().Trim());
                    }
                }
            }
        }

        private void pktComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            int idx = KodePkt.IndexOf(pktComboBox.SelectedValue.ToString());
            pktIndex = idx;
        }

       

        private void bonGridView_SelectionChanged_1(object sender, EventArgs e)
        {
            DataGridViewSelectedCellCollection cells = bonGridView.SelectedCells;
            foreach (DataGridViewCell cell in cells)
            {
                int rowidx = cell.RowIndex;
                int colidx = cell.ColumnIndex;
                bonGridView.Rows[rowidx].Cells[colidx].Value = bonGridView.Rows[rowidx].Cells[colidx].Value.ToString();
            }
            for (int a = 0; a < bonGridView.Rows.Count; a++)
            {
                for (int b = 0; b < bonGridView.Columns.Count; b++)
                {
                    if (!cells.Contains(bonGridView.Rows[a].Cells[b]))
                    {
                        Int64 buf;
                        if (Int64.TryParse(bonGridView.Rows[a].Cells[b].Value.ToString(), out buf))
                            bonGridView.Rows[a].Cells[b].Value = Int64.Parse(bonGridView.Rows[a].Cells[b].Value.ToString().Trim());
                    }
                }
            }
        }

        private void Txt_ValueChanged(object sender, EventArgs e)
        {
            loadRasio();
            loadTableRasio();
            loadTableSaldo();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsPunctuation(e.KeyChar);
        }
        SqlDataReader query(String q)
        {
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    cmd.CommandText = q;
                    sql.Open();
                    return cmd.ExecuteReader();
                }
            }
        }
    }
}