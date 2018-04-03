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
    public partial class InformationBoard : Form
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
        
        int pktIndex;
        DateTime tanggalOptiMin ,tanggalOptiMax, tanggalKalenderMax; 
        /**
            Opti itu datanya pasti selalu H, H+1,...
            Belom tentu semua pkt ada data opti
        **/
        public InformationBoard()
        {
            InitializeComponent();
            pktIndex = 0;
            using (Database1Entities db = new Database1Entities())
            {
                DateTime tgl = (DateTime) (from x in db.Optis select x).Min(x => x.tanggal);
                DateTime harikemarin = tgl.AddDays(-1);
                List<String> tempListKodePkt = (from x in db.TransaksiAtms
                                                where x.tanggal == harikemarin
                                                select x.kodePkt).ToList();
                List<String> dataYangAdaDiApproval = (from x in db.Approvals where x.tanggal == tgl select x.kodePkt).ToList();

                KodePkt = new List<String>();
                var query = (from x in db.Optis join y in db.Cashpoints on x.idCashpoint equals y.idCashpoint
                             select y.kodePkt).ToList();
                foreach (String temp2 in tempListKodePkt)
                {
                    foreach(var temp3 in query)
                    {
                        if (temp2 == temp3)
                        { KodePkt.Add(temp2); break; }
                    }
                }
                foreach(var temp in dataYangAdaDiApproval)
                {
                    KodePkt.Remove(temp);
                }
                pktComboBox.DataSource = KodePkt;
                tanggalKalenderMax = (from x in db.EventTanggals select x).Max(x => x.tanggal);
            }
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
                        DateTime tempTanggal = minTanggal;
                        int counter = 0;
                        while(tempTanggal<maxTanggal)
                        {
                            treeView1.Nodes.Add(tempTanggal.Year.ToString());
                            int monthCounter = 1;
                            while(tempTanggal<maxTanggal && monthCounter <= 12)
                            {
                                treeView1.Nodes[counter].Nodes.Add((monthCounter++).ToString());
                                tempTanggal = tempTanggal.AddMonths(1);
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
            if(KodePkt.Count>0)
                loadPrediksiOpti();
            else
            {
                MessageBox.Show("Data opti belum ada");
            }
            MetodePrediksiComboBox.SelectedIndex = 0;
            MetodeHitungLainnyaComboBox.SelectedIndex = 0;
            /*tanggalOpti = (DateTime) query[0].tanggal*/;
            //Console.WriteLine(query[0]);
            //MessageBox.Show(tanggalOpti.ToShortDateString());
        }
        private void pktComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            pktIndex = pktComboBox.SelectedIndex;
            loadPrediksiOpti();
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
                        tanggalOptiMin = minDate;
                        tanggalOptiMax = maxDate;
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
                    cmd.CommandText = "SELECT e2e FROM Pkt WHERE kodePkt = '" + KodePkt[pktIndex]+"'";
                    reader = cmd.ExecuteReader();
                    reader.Read();
                    e2eTxt.Text = "Jenis E2E: " + reader[0];
                    if (reader[0].ToString() == "E2E")
                        e2eComboBox.SelectedIndex = 0;
                    else
                        e2eComboBox.SelectedIndex = 1;
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
            for (int a = 0; a < detailApprovals.Count-1; a++)
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
                    cmd.CommandText = "SELECT saldoAkhir100, saldoAkhir50, saldoAkhir20 FROM TransaksiAtms WHERE kodePkt = '" + KodePkt[pktIndex] + "' AND tanggal >= '" + tanggalOptiMin.AddDays(-1).ToShortDateString() + "'";
                    sql.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        saldoAwal.d100 = (Int64)reader[0];
                        saldoAwal.d50 = (Int64)reader[1];
                        saldoAwal.d20 = (Int64)reader[2];
                    }
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
            int rowcount = prediksiIsiAtmOpti.Count;//dataGridView1.Rows.Count;
            //dataGridView1.Hide();
            DateTime startDate = prediksiIsiAtmOpti[0].tgl;//Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = prediksiIsiAtmOpti[prediksiIsiAtmOpti.Count - 1].tgl;//Convert.ToDateTime(dataGridView1.Rows[rowCount - 1].Cells[0].Value);
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
                                    //MessageBox.Show("A");
                                    //Kalo cari minggu (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = (minggu keberapa)
                                    if (count++ == 0)
                                        cText += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                    else
                                        cText += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                }
                            }
                        }
                        cText += ")";
                        cmd.CommandText = cText;
                        reader = cmd.ExecuteReader();
                        reader.Read();
                        tempIsiAtm.d50 = (Int64)reader[1];
                        tempIsiAtm.d20 = (Int64)reader[2];
                        tempIsiAtm.tgl = tempDate;
                        tempIsiAtm.d100 = (Int64)reader[0];
                        reader.Close();
                        //Console.WriteLine(tempSislokCrm.d100.ToString());
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
            DateTime startDate = prediksiIsiAtmOpti[0].tgl;//Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = prediksiIsiAtmOpti[prediksiIsiAtmOpti.Count - 1].tgl;//Convert.ToDateTime(dataGridView1.Rows[rowCount - 1].Cells[0].Value);
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
                                    //MessageBox.Show("A");
                                    //Kalo cari minggu (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = (minggu keberapa)
                                    if (count++ == 0)
                                        kondisi += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                    else
                                        kondisi += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                }
                            }
                        }
                        kondisi += ")";

                        String subqueryTblAverage = "(SELECT AVG(isiAtm100) AS Average100 , AVG(isiAtm50) AS Average50 , AVG(isiAtm20) AS Average20 FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal";
                        subqueryTblAverage += kondisi;
                        subqueryTblAverage += ") avt";

                        String query = "SELECT "
                                    + "[AverageStdDeviasi100] = AVG(CAST(ABS(isiAtm100 - [Average100]) AS FLOAT) / IIF([Average100] = 0, 1, [Average100])), "
                                    + "[AverageStdDeviasi50] = AVG(CAST(ABS(isiAtm50 - [Average50])AS FLOAT) / IIF([Average50] = 0, 1, [Average50])), "
                                    + "[AverageStdDeviasi20] = AVG(CAST(ABS(isiAtm20 - [Average20])AS FLOAT) / IIF([Average20] = 0, 1, [Average20])) "
                                    + "FROM TransaksiAtms TA JOIN EventTanggal ET ON Ta.tanggal = ET.tanggal, " + subqueryTblAverage + kondisi;

                        cmd.CommandText = query;
                        reader = cmd.ExecuteReader();
                        reader.Read();
                        tempStdDeviasi.d100 = (Double)reader[0];
                        tempStdDeviasi.d50 = (Double)reader[1];
                        tempStdDeviasi.d20 = (Double)reader[2];
                        tempStdDeviasi.tgl = tempDate;
                        reader.Close();
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
            DateTime startDate = prediksiIsiAtmOpti[0].tgl;//Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = prediksiIsiAtmOpti[prediksiIsiAtmOpti.Count - 1].tgl;//Convert.ToDateTime(dataGridView1.Rows[rowCount - 1].Cells[0].Value);
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
                                    //MessageBox.Show("A");
                                    //Kalo cari minggu (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = (minggu keberapa)
                                    if (count++ == 0)
                                        cText += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                    else
                                        cText += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                }
                            }
                        }
                        cText += ")";
                        cmd.CommandText = cText;
                        reader = cmd.ExecuteReader();
                        reader.Read();
                        tempIsiCrm.d100 = (Int64)reader[0];
                        tempIsiCrm.d50 = (Int64)reader[1];
                        tempIsiCrm.d20 = (Int64)reader[2];
                        tempIsiCrm.tgl = tempDate;
                        reader.Close();
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
            DateTime startDate = prediksiIsiAtmOpti[0].tgl;//Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = prediksiIsiAtmOpti[prediksiIsiAtmOpti.Count - 1].tgl;//Convert.ToDateTime(dataGridView1.Rows[rowCount - 1].Cells[0].Value);
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
                                    //MessageBox.Show("A");
                                    //Kalo cari minggu (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = (minggu keberapa)
                                    if (count++ == 0)
                                        kondisi += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                    else
                                        kondisi += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                }
                            }
                        }
                        kondisi += ")";

                        String subqueryTblAverage = "(SELECT AVG(isiCrm100) AS Average100 , AVG(isiCrm50) AS Average50 , AVG(isiCrm20) AS Average20 FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal";
                        subqueryTblAverage += kondisi;
                        subqueryTblAverage += ") avt";

                        String query = "SELECT "
                                    + "[AverageStdDeviasi100] = AVG(CAST(ABS(isiCrm100 - [Average100]) AS FLOAT) / IIF([Average100] = 0, 1, [Average100])), "
                                    + "[AverageStdDeviasi50] = AVG(CAST(ABS(isiCrm50 - [Average50])AS FLOAT) / IIF([Average50] = 0, 1, [Average50])), "
                                    + "[AverageStdDeviasi20] = AVG(CAST(ABS(isiCrm20 - [Average20])AS FLOAT) / IIF([Average20] = 0, 1, [Average20])) "
                                    + "FROM TransaksiAtms TA JOIN EventTanggal ET ON Ta.tanggal = ET.tanggal, " + subqueryTblAverage + kondisi;

                        cmd.CommandText = query;
                        reader = cmd.ExecuteReader();
                        reader.Read();
                        tempStdDeviasi.d100 = (Double)reader[0];
                        tempStdDeviasi.d50 = (Double)reader[1];
                        tempStdDeviasi.d20 = (Double)reader[2];
                        tempStdDeviasi.tgl = tempDate;
                        reader.Close();
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
            for(int a=0;a<isiCrm.Count;a++)
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
            DateTime startDate = prediksiIsiAtmOpti[0].tgl;//Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = prediksiIsiAtmOpti[prediksiIsiAtmOpti.Count - 1].tgl;//Convert.ToDateTime(dataGridView1.Rows[rowCount - 1].Cells[0].Value);
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
                                    //MessageBox.Show("A");
                                    //Kalo cari minggu (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = (minggu keberapa)
                                    if (count++ == 0)
                                        cText += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                    else
                                        cText += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                }
                            }
                        }
                        cText += ")";
                        cmd.CommandText = cText;
                        reader = cmd.ExecuteReader();
                        reader.Read();
                        tempSislokCrm.d100 = (Int64)reader[0];
                        tempSislokCrm.d50 = (Int64)reader[1];
                        tempSislokCrm.d20 = (Int64)reader[2];
                        tempSislokCrm.tgl = tempDate;
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
            DateTime startDate = prediksiIsiAtmOpti[0].tgl;//Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = prediksiIsiAtmOpti[prediksiIsiAtmOpti.Count - 1].tgl;//Convert.ToDateTime(dataGridView1.Rows[rowCount - 1].Cells[0].Value);
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
                                    //MessageBox.Show("A");
                                    //Kalo cari minggu (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = (minggu keberapa)
                                    if (count++ == 0)
                                        kondisi += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                    else
                                        kondisi += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                }
                            }
                        }
                        kondisi += ")";

                        String subqueryTblAverage = "(SELECT AVG(sislokCrm100) AS Average100 , AVG(sislokCrm50) AS Average50 , AVG(sislokCrm20) AS Average20 FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal";
                        subqueryTblAverage += kondisi;
                        subqueryTblAverage += ") avt";

                        String query = "SELECT "
                                    + "[AverageStdDeviasi100] = AVG(CAST(ABS(sislokCrm100 - [Average100]) AS FLOAT) / IIF([Average100] = 0, 1, [Average100])), "
                                    + "[AverageStdDeviasi50] = AVG(CAST(ABS(sislokCrm50 - [Average50])AS FLOAT) / IIF([Average50] = 0, 1, [Average50])), "
                                    + "[AverageStdDeviasi20] = AVG(CAST(ABS(sislokCrm20 - [Average20])AS FLOAT) / IIF([Average20] = 0, 1, [Average20])) "
                                    + "FROM TransaksiAtms TA JOIN EventTanggal ET ON Ta.tanggal = ET.tanggal, " + subqueryTblAverage + kondisi;

                        cmd.CommandText = query;
                        reader = cmd.ExecuteReader();
                        reader.Read();
                        tempStdDeviasi.d100 = (Double)reader[0];
                        tempStdDeviasi.d50 = (Double)reader[1];
                        tempStdDeviasi.d20 = (Double)reader[2];
                        tempStdDeviasi.tgl = tempDate;
                        reader.Close();
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
            DateTime startDate = prediksiIsiAtmOpti[0].tgl;//Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = prediksiIsiAtmOpti[prediksiIsiAtmOpti.Count - 1].tgl;//Convert.ToDateTime(dataGridView1.Rows[rowCount - 1].Cells[0].Value);
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
                        Denom tempSislokCdm = new Denom();
                        String cText = "SELECT AVG(sislokCdm100), AVG(sislokCdm50), AVG(sislokCdm20) FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE kodePkt = '" + KodePkt[pktIndex] + "' AND DATENAME(WEEKDAY, TA.Tanggal) = '" + tempDate.DayOfWeek.ToString() + "'";
                        int count = 0;
                        for (int i = 0; i < treeView1.Nodes.Count; i++)
                        {
                            for (int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++)
                            {
                                if (treeView1.Nodes[i].Nodes[j].Checked)
                                {
                                    //MessageBox.Show("A");
                                    //Kalo cari minggu (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = (minggu keberapa)
                                    if (count++ == 0)
                                        cText += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                    else
                                        cText += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                }
                            }
                        }
                        cText += ")";
                        cmd.CommandText = cText;
                        reader = cmd.ExecuteReader();
                        reader.Read();
                        tempSislokCdm.d100 = (Int64)reader[0];
                        tempSislokCdm.d50 = (Int64)reader[1];
                        tempSislokCdm.d20 = (Int64)reader[2];
                        tempSislokCdm.tgl = tempDate;
                        reader.Close();
                        //Console.WriteLine(tempSislokCdm.d100.ToString());
                        sislokCdm.Add(tempSislokCdm);
                        tempDate = tempDate.AddDays(1);
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
            DateTime startDate = prediksiIsiAtmOpti[0].tgl;//Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = prediksiIsiAtmOpti[prediksiIsiAtmOpti.Count - 1].tgl;//Convert.ToDateTime(dataGridView1.Rows[rowCount - 1].Cells[0].Value);
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
                                    //MessageBox.Show("A");
                                    //Kalo cari minggu (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = (minggu keberapa)
                                    if (count++ == 0)
                                        kondisi += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                    else
                                        kondisi += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                }
                            }
                        }
                        kondisi += ")";

                        String subqueryTblAverage = "(SELECT AVG(sislokcdm100) AS Average100 , AVG(sislokCdm50) AS Average50 , AVG(sislokCdm20) AS Average20 FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal";
                        subqueryTblAverage += kondisi;
                        subqueryTblAverage += ") avt";

                        String query = "SELECT "
                                    + "[AverageStdDeviasi100] = AVG(CAST(ABS(sislokCdm100 - [Average100]) AS FLOAT) / IIF([Average100] = 0, 1, [Average100])), "
                                    + "[AverageStdDeviasi50] = AVG(CAST(ABS(sislokCdm50 - [Average50])AS FLOAT) / IIF([Average50] = 0, 1, [Average50])), "
                                    + "[AverageStdDeviasi20] = AVG(CAST(ABS(sislokCdm20 - [Average20])AS FLOAT) / IIF([Average20] = 0, 1, [Average20])) "
                                    + "FROM TransaksiAtms TA JOIN EventTanggal ET ON Ta.tanggal = ET.tanggal, " + subqueryTblAverage + kondisi;

                        cmd.CommandText = query;
                        reader = cmd.ExecuteReader();
                        reader.Read();
                        tempStdDeviasi.d100 = (Double)reader[0];
                        tempStdDeviasi.d50 = (Double)reader[1];
                        tempStdDeviasi.d20 = (Double)reader[2];
                        tempStdDeviasi.tgl = tempDate;
                        reader.Close();
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
            DateTime startDate = prediksiIsiAtmOpti[0].tgl;//Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = prediksiIsiAtmOpti[prediksiIsiAtmOpti.Count - 1].tgl;//Convert.ToDateTime(dataGridView1.Rows[rowCount - 1].Cells[0].Value);
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
                                    //MessageBox.Show("A");
                                    //Kalo cari minggu (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = (minggu keberapa)
                                    if (count++ == 0)
                                        cText += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                    else
                                        cText += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                }
                            }
                        }
                        cText += ")";
                        cmd.CommandText = cText;
                        reader = cmd.ExecuteReader();
                        reader.Read();
                        //Console.WriteLine(reader[2]);
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
                while(reader.Read())
                {
                    Denom temp = new Denom();
                    temp.tgl = (DateTime) reader[0];
                    temp.d100 = Convert.ToInt64(reader[1]);
                    temp.d50 = Convert.ToInt64(reader[2]);
                    temp.d20 = Convert.ToInt64(reader[3]);
                    bon.Add(temp);
                    jumlahBonLaporan++;
                }
                sql.Close();
            }
            Console.WriteLine("BON");
            Console.WriteLine(bon[0].d100);
        }
        void loadSetor()
        {
            Database1Entities db = new Database1Entities();
            setor = new List<Denom>();
            String kodepkt = KodePkt[pktIndex];
            Denom temp = new Denom();
            var q2 = (from x in db.Approvals
                      join y in db.DetailApprovals on x.idApproval equals y.idApproval
                      where x.kodePkt == kodepkt && (y.tanggal >= tanggalOptiMin) select new {Approval = x, DetailApproval = y}).ToList();
            foreach(var s in q2)
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
                temp.tgl = (DateTime) s.DetailApproval.tanggal;
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

            if (rasio100Txt.Text.Trim().Length == 0)
                targetRasio50 = 0;
            else
                targetRasio50 = Double.Parse(rasio50Txt.Text);
            if (rasio100Txt.Text.Trim().Length == 0)
                targetRasio20 = 0;
            else
                targetRasio20 = Double.Parse(rasio20Txt.Text);

            Denom saldoAkhirH = new Denom();

            rasio100Lbl.Text = "-";
            rasio50Lbl.Text = "-";
            rasio20Lbl.Text = "-";

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
            for (int a = 2 + jumlahBonLaporan-1 ; a<prediksiIsiAtm.Count;a++)
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

            tempRekomendasiBon.tgl = tanggalOptiMin.AddDays(1+jumlahBonLaporan-1);

            if (setorCounter < setor.Count && tempRekomendasiBon.tgl == setor[setorCounter].tgl)
            {
                tempRekomendasiBon.d100 += setor[setorCounter].d100;
                tempRekomendasiBon.d50 += setor[setorCounter].d50;
                tempRekomendasiBon.d20 += setor[setorCounter++].d20;
            }

            rekomendasiBon.Add(tempRekomendasiBon);

            counter = jumlahBonLaporan+1; 
            //Hitung rekomendasiBon h+3 keatas
            for(int a = 0; a<saldoAwalIdeal.Count - 1;a++)
            {
                tempRekomendasiBon = new Denom();
                Denom saldoAkhirIdeal = saldoAwalIdeal[a + 1];
                //Denom saldoSementara = new Denom();`
                //saldoSementara.d100 = saldoAwalIdeal[a].d100 - isiCrm[counter].d100;
                tempRekomendasiBon.d100 = saldoAwalIdeal[a + 1].d100                                 //Saldo akhir h+1 
                    - saldoAwalIdeal[a].d100                                                      //saldo awal hari h
                    - (Int64)Math.Round((rasioSislokAtm[counter+a].d100 * prediksiIsiAtm[counter+a].d100))
                    - sislokCrm[counter+a].d100
                    - sislokCdm[counter+a].d100
                    + prediksiIsiAtm[counter+a].d100
                    + isiCrm[counter+a].d100;
                tempRekomendasiBon.d50 = saldoAwalIdeal[a + 1].d50                                 //Saldo akhir h+1 
                    - saldoAwalIdeal[a].d50                                                      //saldo awal hari h
                    - (Int64)Math.Round((rasioSislokAtm[counter+a].d50 * prediksiIsiAtm[counter+a].d50))
                    - sislokCrm[counter+a].d50
                    - sislokCdm[counter+a].d50
                    + prediksiIsiAtm[counter+a].d50
                    + isiCrm[counter+a].d50;
                tempRekomendasiBon.d20 = saldoAwalIdeal[a + 1].d20                                 //Saldo akhir h+1 
                    - saldoAwalIdeal[a].d20                                                      //saldo awal hari h
                    - (Int64)Math.Round((rasioSislokAtm[counter+a].d20 * prediksiIsiAtm[counter+a].d20))
                    - sislokCrm[counter+a].d20
                    - sislokCdm[counter+a].d20
                    + prediksiIsiAtm[counter+a].d20
                    + isiCrm[counter+a].d20;
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
            Console.WriteLine("Rekomendasi Bon");
            foreach(var temp in rekomendasiBon)
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

            if (rasio100Txt.Text.Trim().Length == 0)
                targetRasio50 = 0;
            else
                targetRasio50 = Double.Parse(rasio50Txt.Text);
            if (rasio100Txt.Text.Trim().Length == 0)
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
            for (int a = jumlahBonLaporan; a < prediksiIsiAtm.Count-1; a++)
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
                    saldoAkhirH.d100 -= setor[setorCounter-1].d100;
                    saldoAkhirH.d50 -= setor[setorCounter-1].d50;
                    saldoAkhirH.d20 -= setor[setorCounter-1].d20;
                }
            }

            //Cek rasio hari terakhir
            bool flag100rasiounder = false, flag50rasiounder = false, flag20rasiounder = false;
            if (Math.Round((Double)saldoAkhirH.d100 / (prediksiIsiAtm[prediksiIsiAtmOpti.Count - 1].d100 + isiCrm[prediksiIsiAtm.Count - 1].d100),2) <= targetRasio100)
            {
                Console.WriteLine();
                Console.WriteLine((Double)saldoAkhirH.d100 / (prediksiIsiAtm[prediksiIsiAtmOpti.Count - 1].d100 + isiCrm[prediksiIsiAtm.Count - 1].d100));
                flag100rasiounder = true;
            }
            if (Math.Round((Double)saldoAkhirH.d50 / (prediksiIsiAtm[prediksiIsiAtmOpti.Count - 1].d50 + isiCrm[prediksiIsiAtm.Count - 1].d50),2) <= targetRasio50)
            {
                Console.WriteLine((Double)saldoAkhirH.d50 / (prediksiIsiAtm[prediksiIsiAtmOpti.Count - 1].d50 + isiCrm[prediksiIsiAtm.Count - 1].d50));
                flag50rasiounder = true;
            }
            if (Math.Round((Double)saldoAkhirH.d20 / (prediksiIsiAtm[prediksiIsiAtmOpti.Count - 1].d20 + isiCrm[prediksiIsiAtm.Count - 1].d20),2) <= targetRasio20)
            {
                Console.WriteLine((Double)saldoAkhirH.d20 / (prediksiIsiAtm[prediksiIsiAtmOpti.Count - 1].d20 + isiCrm[prediksiIsiAtm.Count - 1].d20));
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
                if (temp.d100 > 0)
                    cell1.Value = temp.d100;
                else
                    cell1.Value = 0;
                row.Cells.Add(cell1);
                DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                if (temp.d50 > 0)
                    cell2.Value = temp.d50;
                else
                    cell2.Value = 0;
                row.Cells.Add(cell2);
                DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                if (temp.d20 > 0)
                    cell3.Value = temp.d20;
                else
                    cell3.Value = 0;
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
            while (tempDate <= tanggalOptiMax.AddDays(-1))
            {
                DataGridViewRow row = new DataGridViewRow();

                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = tempDate.ToShortDateString();
                row.Cells.Add(cell);
                cell = new DataGridViewTextBoxCell();
                cell.Value = "0";
                row.Cells.Add(cell);
                cell = new DataGridViewTextBoxCell();
                cell.Value = "0";
                row.Cells.Add(cell);
                cell = new DataGridViewTextBoxCell();
                cell.Value = "0";
                row.Cells.Add(cell);
                bonGridView.Rows.Add(row);
                tempDate = tempDate.AddDays(1);
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
        //Sekalian load saldoAwal untuk setiap hari
        void loadRasio()
        {
            Console.WriteLine("RASIO & Saldo Awal");
            Console.WriteLine("===============");
            saldo = new List<Denom>();

           
            Console.WriteLine(saldoAwal.d100 + " " + saldoAwal.d50 + " " + saldoAwal.d20);
            //Bon Adhoc
            Denom bonAdhoc = loadBonAdhocFromTxt();
            //Setor Adhoc
            Denom setorAdhoc = loadSetorAdhocFromTxt();
            //Setor
            Denom setorBaru = loadSetorFromTxt();

            int counter = 0;
            int setorCounter = 0;
            listRasio = new List<Rasio>();
            DateTime tempTgl = tanggalOptiMin;
            Rasio rasio = new Rasio();
            rasio.tgl = tempTgl;
            rasio.d100 = saldoAwal.d100 / ((Double)prediksiIsiAtm[counter].d100 + isiCrm[counter].d100);
            rasio.d50 = saldoAwal.d50 / ((Double)prediksiIsiAtm[counter].d50 + isiCrm[counter].d100);
            rasio.d20 = saldoAwal.d20 / ((Double)prediksiIsiAtm[counter].d20 + isiCrm[counter].d100);
            listRasio.Add(rasio);
            saldo.Add(
                    new Denom
                    {
                        tgl = rasio.tgl,
                        d100 = saldoAwal.d100,
                        d50 = saldoAwal.d50,
                        d20 = saldoAwal.d20
                    }
                );
            Console.WriteLine("Saldo awal " + rasio.tgl.ToShortDateString() + ": " + saldoAwal.d100 + " " + saldoAwal.d50 + " " + saldoAwal.d20);
            Console.WriteLine();
            Denom saldoAkhir = new Denom();
            //Console.WriteLine("Jalan");
            if (setor.Count > setorCounter)
            {
                if (setor[setorCounter].tgl == tanggalOptiMin)
                {
                    saldoAkhir.d100 = saldoAwal.d100 + sislokCdm[counter].d100 + sislokCrm[counter].d100 + (Int64)Math.Round((rasioSislokAtm[counter].d100 * prediksiIsiAtm[counter].d100),0) - prediksiIsiAtm[counter].d100 - isiCrm[counter].d100 + bon[counter].d100 + bonAdhoc.d100 - setorAdhoc.d100 - setor[setorCounter].d100;
                    saldoAkhir.d50 = saldoAwal.d50 + sislokCdm[counter].d50 + sislokCrm[counter].d50 + (Int64)(rasioSislokAtm[counter].d50 * prediksiIsiAtm[counter].d50) - prediksiIsiAtm[counter].d50 - isiCrm[counter].d50 + bon[counter].d50 + bonAdhoc.d50 - setorAdhoc.d50 - setor[setorCounter].d50;
                    saldoAkhir.d20 = saldoAwal.d20 + sislokCdm[counter].d20 + sislokCrm[counter].d20 + (Int64)(rasioSislokAtm[counter].d20 * prediksiIsiAtm[counter].d20) - prediksiIsiAtm[counter].d20 - isiCrm[counter].d20 + bon[counter].d20 + bonAdhoc.d20 - setorAdhoc.d20 - setor[setorCounter].d20;
                    setorCounter++;
                }
                else
                {
                    saldoAkhir.d100 = saldoAwal.d100 + sislokCdm[counter].d100 + sislokCrm[counter].d100 + (Int64)(rasioSislokAtm[counter].d100 * prediksiIsiAtm[counter].d100) - prediksiIsiAtm[counter].d100 - isiCrm[counter].d100 + bon[counter].d100 + bonAdhoc.d100 - setorAdhoc.d100;
                    saldoAkhir.d50 = saldoAwal.d50 + sislokCdm[counter].d50 + sislokCrm[counter].d50 + (Int64)(rasioSislokAtm[counter].d50 * prediksiIsiAtm[counter].d50) - prediksiIsiAtm[counter].d50 - isiCrm[counter].d50 + bon[counter].d50 + bonAdhoc.d50 - setorAdhoc.d50;
                    saldoAkhir.d20 = saldoAwal.d20 + sislokCdm[counter].d20 + sislokCrm[counter].d20 + (Int64)(rasioSislokAtm[counter].d20 * prediksiIsiAtm[counter].d20) - prediksiIsiAtm[counter].d20 - isiCrm[counter].d20 + bon[counter].d20 + bonAdhoc.d20 - setorAdhoc.d20;
                }
            }
            else
            {
                saldoAkhir.d100 = saldoAwal.d100 + sislokCdm[counter].d100 + sislokCrm[counter].d100 + (Int64)(rasioSislokAtm[counter].d100 * prediksiIsiAtm[counter].d100) - prediksiIsiAtm[counter].d100 - isiCrm[counter].d100 + bon[counter].d100 + bonAdhoc.d100 - setorAdhoc.d100;
                saldoAkhir.d50 = saldoAwal.d50 + sislokCdm[counter].d50 + sislokCrm[counter].d50 + (Int64)(rasioSislokAtm[counter].d50 * prediksiIsiAtm[counter].d50) - prediksiIsiAtm[counter].d50 - isiCrm[counter].d50 + bon[counter].d50 + bonAdhoc.d50 - setorAdhoc.d50;
                saldoAkhir.d20 = saldoAwal.d20 + sislokCdm[counter].d20 + sislokCrm[counter].d20 + (Int64)(rasioSislokAtm[counter].d20 * prediksiIsiAtm[counter].d20) - prediksiIsiAtm[counter].d20 - isiCrm[counter].d20 + bon[counter].d20 + bonAdhoc.d20 - setorAdhoc.d20;
            }
            if (setorBaru.tgl.ToShortDateString() == tanggalOptiMin.ToShortDateString())
            {
                saldoAkhir.d100 -= setorBaru.d100;
                saldoAkhir.d50 -= setorBaru.d50;
                saldoAkhir.d20 -= setorBaru.d20;
            }
            counter++;

            for (int a=1;a<bon.Count;a++)
            {
                rasio = new Rasio();
                rasio.tgl = tempTgl.AddDays(counter);
                rasio.d100 = saldoAkhir.d100 / ((Double)prediksiIsiAtm[counter].d100 + isiCrm[counter].d100);
                rasio.d50 = saldoAkhir.d50 / ((Double)prediksiIsiAtm[counter].d50 + isiCrm[counter].d100);
                rasio.d20 = saldoAkhir.d20 / ((Double)prediksiIsiAtm[counter].d20 + isiCrm[counter].d100);
                listRasio.Add(rasio);
                Console.WriteLine("Saldo awal " + rasio.tgl.ToShortDateString() + ": " + saldoAkhir.d100 + " " + saldoAkhir.d50 + " " + saldoAkhir.d20);
                Console.WriteLine();
                saldo.Add(
                    new Denom
                    {
                        tgl = rasio.tgl,
                        d100 = saldoAkhir.d100,
                        d50 = saldoAkhir.d50,
                        d20 = saldoAkhir.d20
                    }
                );
                if (setor.Count > setorCounter)
                {
                    if (setor[setorCounter].tgl == rasio.tgl)
                    {
                        saldoAkhir.d100 = saldoAkhir.d100 + sislokCdm[counter].d100 + sislokCrm[counter].d100 + (Int64)(rasioSislokAtm[counter].d100 * prediksiIsiAtm[counter].d100) - prediksiIsiAtm[counter].d100 - isiCrm[counter].d100 + bon[counter].d100 - setor[setorCounter].d100;
                        saldoAkhir.d50 = saldoAkhir.d50 + sislokCdm[counter].d50 + sislokCrm[counter].d50 + (Int64)(rasioSislokAtm[counter].d50 * prediksiIsiAtm[counter].d50) - prediksiIsiAtm[counter].d50 - isiCrm[counter].d50 + bon[counter].d50 - setor[setorCounter].d50;
                        saldoAkhir.d20 = saldoAkhir.d20 + sislokCdm[counter].d20 + sislokCrm[counter].d20 + (Int64)(rasioSislokAtm[counter].d20 * prediksiIsiAtm[counter].d20) - prediksiIsiAtm[counter].d20 - isiCrm[counter].d20 + bon[counter].d20 - setor[setorCounter].d20;
                        setorCounter++;
                    }
                    else
                    {
                        saldoAkhir.d100 = saldoAkhir.d100 + sislokCdm[counter].d100 + sislokCrm[counter].d100 + (Int64)(rasioSislokAtm[counter].d100 * prediksiIsiAtm[counter].d100) - prediksiIsiAtm[counter].d100 - isiCrm[counter].d100 + bon[counter].d100;
                        saldoAkhir.d50 = saldoAkhir.d50 + sislokCdm[counter].d50 + sislokCrm[counter].d50 + (Int64)(rasioSislokAtm[counter].d50 * prediksiIsiAtm[counter].d50) - prediksiIsiAtm[counter].d50 - isiCrm[counter].d50 + bon[counter].d50;
                        saldoAkhir.d20 = saldoAkhir.d20 + sislokCdm[counter].d20 + sislokCrm[counter].d20 + (Int64)(rasioSislokAtm[counter].d20 * prediksiIsiAtm[counter].d20) - prediksiIsiAtm[counter].d20 - isiCrm[counter].d20 + bon[counter].d20;
                    }
                }
                else
                {
                    saldoAkhir.d100 = saldoAkhir.d100 + sislokCdm[counter].d100 + sislokCrm[counter].d100 + (Int64)(rasioSislokAtm[counter].d100 * prediksiIsiAtm[counter].d100) - prediksiIsiAtm[counter].d100 - isiCrm[counter].d100 + bon[counter].d100;
                    saldoAkhir.d50 = saldoAkhir.d50 + sislokCdm[counter].d50 + sislokCrm[counter].d50 + (Int64)(rasioSislokAtm[counter].d50 * prediksiIsiAtm[counter].d50) - prediksiIsiAtm[counter].d50 - isiCrm[counter].d50 + bon[counter].d50;
                    saldoAkhir.d20 = saldoAkhir.d20 + sislokCdm[counter].d20 + sislokCrm[counter].d20 + (Int64)(rasioSislokAtm[counter].d20 * prediksiIsiAtm[counter].d20) - prediksiIsiAtm[counter].d20 - isiCrm[counter].d20 + bon[counter].d20;
                }

                if (setorBaru.tgl.ToShortDateString() == rasio.tgl.ToShortDateString())
                {
                    saldoAkhir.d100 -= setorBaru.d100;
                    saldoAkhir.d50 -= setorBaru.d50;
                    saldoAkhir.d20 -= setorBaru.d20;
                }

                counter++;
            }
            
            for (int a=0;a<saldoAwalIdeal.Count;a++)
            {
                Int64 bon100,bon50,bon20;
                if (bonGridView.Rows[a].Cells[1].Value.ToString() == "0")
                {
                    bon100 = 0;
                }
                else
                {
                    bon100 = (Int64)bonGridView.Rows[a].Cells[1].Value;
                }
                if (bonGridView.Rows[a].Cells[2].Value.ToString() == "0")
                {
                    bon50 = 0;
                }
                else
                {
                    bon50 = (Int64)bonGridView.Rows[a].Cells[2].Value;
                }
                if (bonGridView.Rows[a].Cells[3].Value.ToString()== "0")
                {
                    bon20 = 0;
                }
                else
                {
                    bon20 = (Int64)bonGridView.Rows[a].Cells[3].Value;
                }
                rasio = new Rasio();
                rasio.tgl = tempTgl.AddDays(counter);

                Console.WriteLine("Saldo awal " + rasio.tgl.ToShortDateString() + ": " + saldoAkhir.d100 + " " + saldoAkhir.d50 + " " + saldoAkhir.d20);
                Console.WriteLine();
                saldo.Add(
                    new Denom
                    {
                        tgl = rasio.tgl,
                        d100 = saldoAkhir.d100,
                        d50 = saldoAkhir.d50,
                        d20 = saldoAkhir.d20
                    }
                );
                rasio.d100 = saldoAkhir.d100 / ((Double)prediksiIsiAtm[counter].d100 + (Double)isiCrm[counter].d100);
                rasio.d50 = saldoAkhir.d50 / ((Double)prediksiIsiAtm[counter].d50 + (Double)isiCrm[counter].d50);
                rasio.d20 = saldoAkhir.d20 / ((Double)prediksiIsiAtm[counter].d20 + (Double)isiCrm[counter].d20);
                listRasio.Add(rasio);


                if (setor.Count > setorCounter)
                {
                    if (setor[setorCounter].tgl == tempTgl.AddDays(counter))
                    {
                        saldoAkhir.d100 = saldoAkhir.d100 + sislokCdm[counter].d100 + sislokCrm[counter].d100 + (Int64)(rasioSislokAtm[counter].d100 * prediksiIsiAtm[counter].d100) - prediksiIsiAtm[counter].d100 - isiCrm[counter].d100 + bon100 - setor[setorCounter].d100;
                        saldoAkhir.d50 = saldoAkhir.d50 + sislokCdm[counter].d50 + sislokCrm[counter].d50 + (Int64)(rasioSislokAtm[counter].d50 * prediksiIsiAtm[counter].d50) - prediksiIsiAtm[counter].d50 - isiCrm[counter].d50 + bon50 - setor[setorCounter].d50;
                        saldoAkhir.d20 = saldoAkhir.d20 + sislokCdm[counter].d20 + sislokCrm[counter].d20 + (Int64)(rasioSislokAtm[counter].d20 * prediksiIsiAtm[counter].d20) - prediksiIsiAtm[counter].d20 - isiCrm[counter].d20 + bon20 - setor[setorCounter].d20;
                        setorCounter++;
                    }
                    else
                    {
                        saldoAkhir.d100 = saldoAkhir.d100 + sislokCdm[counter].d100 + sislokCrm[counter].d100 + (Int64)(rasioSislokAtm[counter].d100 * prediksiIsiAtm[counter].d100) - prediksiIsiAtm[counter].d100 - isiCrm[counter].d100 + bon100;
                        saldoAkhir.d50 = saldoAkhir.d50 + sislokCdm[counter].d50 + sislokCrm[counter].d50 + (Int64)(rasioSislokAtm[counter].d50 * prediksiIsiAtm[counter].d50) - prediksiIsiAtm[counter].d50 - isiCrm[counter].d50 + bon50;
                        saldoAkhir.d20 = saldoAkhir.d20 + sislokCdm[counter].d20 + sislokCrm[counter].d20 + (Int64)(rasioSislokAtm[counter].d20 * prediksiIsiAtm[counter].d20) - prediksiIsiAtm[counter].d20 - isiCrm[counter].d20 + bon20;
                    }
                    //counter++;
                    
                }
                else
                {
                    saldoAkhir.d100 = saldoAkhir.d100 + sislokCdm[counter].d100 + sislokCrm[counter].d100 + (Int64)(rasioSislokAtm[counter].d100 * prediksiIsiAtm[counter].d100) - prediksiIsiAtm[counter].d100 - isiCrm[counter].d100 + bon100;
                    saldoAkhir.d50 = saldoAkhir.d50 + sislokCdm[counter].d50 + sislokCrm[counter].d50 + (Int64)(rasioSislokAtm[counter].d50 * prediksiIsiAtm[counter].d50) - prediksiIsiAtm[counter].d50 - isiCrm[counter].d50 + bon50;
                    saldoAkhir.d20 = saldoAkhir.d20 + sislokCdm[counter].d20 + sislokCrm[counter].d20 + (Int64)(rasioSislokAtm[counter].d20 * prediksiIsiAtm[counter].d20) - prediksiIsiAtm[counter].d20 - isiCrm[counter].d20 + bon20;
                    //saldo.Add(saldoAkhir);
                }
                if (setorBaru.tgl.ToShortDateString() == tempTgl.AddDays(counter).ToShortDateString())
                {
                    saldoAkhir.d100 -= setorBaru.d100;
                    saldoAkhir.d50 -= setorBaru.d50;
                    saldoAkhir.d20 -= setorBaru.d20;
                }
                
                counter++;
            }
            rasio = new Rasio();
            rasio.tgl = tempTgl.AddDays(counter);
            rasio.d100 = saldoAkhir.d100 / ((Double)prediksiIsiAtm[counter].d100 + (Double)isiCrm[counter].d100);
            rasio.d50 = saldoAkhir.d50 / ((Double)prediksiIsiAtm[counter].d50 + (Double)isiCrm[counter].d50);
            rasio.d20 = saldoAkhir.d20 / ((Double)prediksiIsiAtm[counter].d20 + (Double)isiCrm[counter].d20);
            listRasio.Add(rasio);
            saldo.Add(
                    new Denom
                    {
                        tgl = rasio.tgl,
                        d100 = saldoAkhir.d100,
                        d50 = saldoAkhir.d50,
                        d20 = saldoAkhir.d20
                    }
                );

            foreach (var t in saldo)
            {
                Console.WriteLine("100: " + t.d100);
                Console.WriteLine("50: " + t.d50);
                Console.WriteLine("20: " + t.d20);
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
        private void loadPrediksiBtn_Click(object sender, EventArgs e)
        {
            Double buf;
            if (tanggalKalenderMax < tanggalOptiMax)
            {
                MessageBox.Show("Data tanggal di table kalender kurang");
            }
            else if (!Double.TryParse(rasio100Txt.Text, out buf) || !Double.TryParse(rasio50Txt.Text, out buf) || !Double.TryParse(rasio20Txt.Text,out buf))
            {
                MessageBox.Show("Target rasio harus berupa angka!");
            }
            else
            {
                //loadSislokCrm();
                try
                {
                    if(MetodePrediksiComboBox.SelectedIndex == 0)
                    {
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
                        sislokCdm = sislokCdmDenganStdDeviasi;
                        isiCrm = isiCrmDenganStdDeviasi;
                        sislokCrm = sislokCrmDenganStdDeviasi;
                    }
                    //Console.WriteLine(sislokCrm[0].d100);
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
                }catch(Exception err)
                {
                    MessageBox.Show(err.ToString());
                }
            }
        }
        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            CheckTreeViewNode(e.Node, e.Node.Checked);
        }
        private void treeView1_ParentChanged(object sender, EventArgs e)
        {
            
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
                db.Approvals.Add(newA);
                db.SaveChanges();
                var lastApproval = (from x in db.Approvals select x).ToList();
                DateTime tempTanggal = tanggalOptiMin;
                int count = 0;

                for (int i=0;i<bon.Count;i++)
                {
                    DetailApproval newDetailA = new DetailApproval();
                    newDetailA.idApproval = lastApproval[lastApproval.Count - 1].idApproval;
                    newDetailA.tanggal = tempTanggal;

                    if (i == 0)
                    {
                        //Adhoc
                        newDetailA.adhoc100 = bonAdhoc.d100;
                        newDetailA.adhoc50 = bonAdhoc.d50;
                        newDetailA.adhoc20 = bonAdhoc.d20;
                        newDetailA.setor100 = setorAdhoc.d100;
                        newDetailA.setor50 = setorAdhoc.d50;
                        newDetailA.setor20 = setorAdhoc.d20;

                    }
                    if (((DateTime)newDetailA.tanggal).ToShortDateString() == tglSetor.Value.ToShortDateString())
                    {
                        newDetailA.setor100 = setor.d100;
                        newDetailA.setor50 = setor.d50;
                        newDetailA.setor20 = setor.d20;
                    }
                    //Bon
                    newDetailA.bon100 = bon[i].d100;
                    newDetailA.bon50 = bon[i].d50;
                    newDetailA.bon20 = bon[i].d20;

                    //Saldo awal
                    newDetailA.saldoAwal100 =saldo[i].d100;
                    newDetailA.saldoAwal50 = saldo[i].d50;
                    newDetailA.saldoAwal20 = saldo[i].d20;

                    //Sislok
                    newDetailA.sislokCRM100 = sislokCrm[i].d100;
                    newDetailA.sislokCRM50 = sislokCrm[i].d50;
                    newDetailA.sislokCRM20 = sislokCrm[i].d20;
                    newDetailA.sislokCDM100 = sislokCrm[i].d100;
                    newDetailA.sislokCDM50 = sislokCrm[i].d50;
                    newDetailA.sislokCDM20 = sislokCrm[i].d20;
                    newDetailA.sislokATM100 = (Int64)(rasioSislokAtm[i].d100 * prediksiIsiAtm[i].d100);
                    newDetailA.sislokATM50 = (Int64)(rasioSislokAtm[i].d50 * prediksiIsiAtm[i].d50);
                    newDetailA.sislokATM20 = (Int64)(rasioSislokAtm[i].d20 * prediksiIsiAtm[i].d20);

                    //Isi
                    newDetailA.isiATM100 = prediksiIsiAtm[i].d100;
                    newDetailA.isiATM50 = prediksiIsiAtm[i].d50;
                    newDetailA.isiATM20 = prediksiIsiAtm[i].d20;
                    newDetailA.isiCRM100 = isiCrm[i].d100;
                    newDetailA.isiCRM50 = isiCrm[i].d50;
                    newDetailA.isiCRM20 = isiCrm[i].d20;

                    tempTanggal = tempTanggal.AddDays(1);

                    db.DetailApprovals.Add(newDetailA);
                    db.SaveChanges();
                    count++;
                }

                for (int i=0;i<bonYangDisetujui.Count + 1 ;i++)
                {
                    DetailApproval newDetailA = new DetailApproval();
                    newDetailA.idApproval = lastApproval[lastApproval.Count-1].idApproval;
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
                    newDetailA.saldoAwal100 = saldo[i].d100;
                    newDetailA.saldoAwal50 = saldo[i].d50;
                    newDetailA.saldoAwal20 = saldo[i].d20;

                    //Sislok
                    newDetailA.sislokCRM100 = sislokCrm[count].d100;
                    newDetailA.sislokCRM50 = sislokCrm[count].d50;
                    newDetailA.sislokCRM20 = sislokCrm[count].d20;
                    newDetailA.sislokCDM100 = sislokCrm[count].d100;
                    newDetailA.sislokCDM50 = sislokCrm[count].d50;
                    newDetailA.sislokCDM20 = sislokCrm[count].d20;
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

                    db.DetailApprovals.Add(newDetailA);
                    db.SaveChanges();
                    count++;
                }
                MessageBox.Show("Approved!");
            }
            else
            {

            }
        }
        Denom loadBonAdhocFromTxt()
        {
            Denom bonAdhoc = new Denom();
            if (bonAdhoc100Txt.Text.ToString().Trim() == "")
                bonAdhoc.d100 = 0;
            else
                bonAdhoc.d100 = Int64.Parse(bonAdhoc100Txt.Text);
            if (bonAdhoc50Txt.Text.ToString().Trim() == "")
                bonAdhoc.d50 = 0;
            else
                bonAdhoc.d50 = Int64.Parse(bonAdhoc50Txt.Text);
            if (bonAdhoc20Txt.Text.ToString().Trim() == "")
                bonAdhoc.d20 = 0;
            else
                bonAdhoc.d20 = Int64.Parse(bonAdhoc20Txt.Text);
            return bonAdhoc;
        }
        private Denom loadSetorAdhocFromTxt()
        {
            Denom setorAdhoc = new Denom();
            //Setor Adhoc
            if (setorAdhoc100Txt.Text.ToString().Trim() == "")
                setorAdhoc.d100 = 0;
            else
                setorAdhoc.d100 = Int64.Parse(setorAdhoc100Txt.Text);
            if (setorAdhoc50Txt.Text.ToString().Trim() == "")
                setorAdhoc.d50 = 0;
            else
                setorAdhoc.d50 = Int64.Parse(setorAdhoc50Txt.Text);
            if (setorAdhoc20Txt.Text.ToString().Trim() == "")
                setorAdhoc.d20 = 0;
            else
                setorAdhoc.d20 = Int64.Parse(setorAdhoc20Txt.Text);
            return setorAdhoc;
        }
        private Denom loadSetorFromTxt()
        {
            Denom setorBaru = new Denom();
            //Setor
            setorBaru.tgl = tglSetor.Value;
            if (setor100Txt.Text.ToString().Trim() == "")
                setorBaru.d100 = 0;
            else
                setorBaru.d100 = Int64.Parse(setor100Txt.Text);
            if (setor50Txt.Text.ToString().Trim() == "")
                setorBaru.d50 = 0;
            else
                setorBaru.d50 = Int64.Parse(setor50Txt.Text);
            if (setor20Txt.Text.ToString().Trim() == "")
                setorBaru.d20 = 0;
            else
                setorBaru.d20 = Int64.Parse(setor20Txt.Text);
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
            Int64 buf;
            if (Int64.TryParse(bonAdhoc100Txt.Text, out buf))
            {
                loadRasio();
                loadTableRasio();
                loadTableSaldo();
            }
            if (Int64.TryParse(bonAdhoc50Txt.Text, out buf))
            {
                loadRasio();
                loadTableRasio();
                loadTableSaldo();
            }
            if (Int64.TryParse(bonAdhoc20Txt.Text, out buf))
            {
                loadRasio();
                loadTableRasio();
                loadTableSaldo();
            }
            if (Int64.TryParse(setorAdhoc100Txt.Text, out buf))
            {
                loadRasio();
                loadTableRasio();
                loadTableSaldo();
            }
            if (Int64.TryParse(setorAdhoc50Txt.Text, out buf))
            {
                loadRasio();
                loadTableRasio();
                loadTableSaldo();
            }
            if (Int64.TryParse(setorAdhoc20Txt.Text, out buf))
            {
                loadRasio();
                loadTableRasio();
                loadTableSaldo();
            }
            if (Int64.TryParse(setor20Txt.Text, out buf))
            {
                loadRasio();
                loadTableRasio();
                loadTableSaldo();
            }
            if (Int64.TryParse(setor50Txt.Text, out buf))
            {
                loadRasio();
                loadTableRasio();
                loadTableSaldo();
            }
            if (Int64.TryParse(setor100Txt.Text, out buf))
            {
                loadRasio();
                loadTableRasio();
                loadTableSaldo();
            }
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
        private void bonGridView_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            Int64 buf;
            if(bonGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value!=null)
                if(Int64.TryParse(bonGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(),out buf))
                    bonGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Int64.Parse(bonGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Trim());
        }

        private void rekomendasiBonGridView_SelectionChanged(object sender, EventArgs e)
        {
            sumLabel.Text = "SUM: ";
            var temp = rekomendasiBonGridView.SelectedCells;
            Int64 sum = 0;
            Console.WriteLine("REFRESH");
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
class Denom
{
    public Denom()
    {
        d100 = 0;
        d50 = 0;
        d20 = 0;
    }

    public DateTime tgl { set; get; }
    public Int64 d100 { set; get; }
    public Int64 d50 { set; get; }
    public Int64 d20 { set; get; }
}
class Rasio
{
    public DateTime tgl { set; get; }
    public Double d100 { set; get; }
    public Double d50 { set; get; }
    public Double d20 { set; get; }
}
