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
        bool firstRun = true;

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

        List<LaporanPermintaanBon> laporanPermintaanBon = new List<LaporanPermintaanBon>();
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
        public InformationBoard()
        {
            InitializeComponent();
            labelKodePkt.Text = "";
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
            tglSetor.MinDate = DateTime.Today.AddDays(1);
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
                MessageBox.Show("Data Laporan Kemarin Belum Ada");

            pktIndex = 0;
            loadE2E();
            MetodePrediksiComboBox.SelectedIndex = 0;
            MetodeHitungLainnyaComboBox.SelectedIndex = 0;
            /*tanggalOpti = (DateTime) query[0].tanggal*/;
            //Console.WriteLine(query[0]);
            //MessageBox.Show(tanggalOpti.ToShortDateString());
            tanggalPrediksiMaxPicker.MinDate = DateTime.Today.AddDays(2);

            setorAdhoc100Txt.Increment = 0;
        }
        void loadComboBox()
        {
            using (Database1Entities db = new Database1Entities())
            {
                DateTime harikemarin = DateTime.Today.Date.AddDays(-1);
                List<String> tempListKodePkt = (from x in db.TransaksiAtms
                                                where x.tanggal == harikemarin
                                                orderby x.kodePkt
                                                select x.kodePkt).ToList();

                List<String> dataYangAdaDiApproval = (from x in db.Approvals where x.tanggal == DateTime.Today.Date select x.kodePkt).ToList();

                KodePkt = tempListKodePkt;

                foreach (var temp in dataYangAdaDiApproval)
                {
                    KodePkt.Remove(temp);
                }
                pktComboBox.DataSource = KodePkt;
                tanggalKalenderMax = (from x in db.EventTanggals select x).Max(x => x.tanggal);
            }
        }
        private void pktComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            pktIndex = pktComboBox.SelectedIndex;
            labelKodePkt.Text = KodePkt[pktIndex];
            loadE2E();
            Database1Entities db = new Database1Entities();
            firstRun = true;
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
            laporanPermintaanBon= (from x in db.LaporanPermintaanBons where x.kodePkt == kodePkt && x.tanggal > tanggalOptiMin select x).Distinct().ToList();

            DateTime tanggalKemaren = tanggalOptiMin.AddDays(-1);
            //Delete data yang udah ada di approval
            var laporanBons = (from x in db.Approvals
                               join y in db.DetailApprovals on x.idApproval equals y.idApproval
                               where x.kodePkt == kodePkt 
                               && y.tanggal > tanggalOptiMin
                               && y.bon100 != -1
                               && x.idApproval == db.Approvals.Where(a=>a.kodePkt == kodePkt).Max(a=>a.idApproval)
                               select y).ToList();
            List<LaporanPermintaanBon> toDelete = new List<LaporanPermintaanBon>();
            for (int a = 0; a < laporanPermintaanBon.Count; a++)
                if(laporanBons.Where(x=>x.tanggal == laporanPermintaanBon[a].tanggal).FirstOrDefault() != null)
                    toDelete.Add(laporanPermintaanBon[a]);

            foreach (var temp in toDelete)
                laporanPermintaanBon.Remove(temp);

            permintaanBonGridView.Columns.Add("Tanggal", "Tanggal");
            permintaanBonGridView.Columns.Add("100", "100");
            permintaanBonGridView.Columns.Add("50", "50");
            permintaanBonGridView.Columns.Add("20", "20");
            for (int i = 1; i < 4; i++)
            {
                permintaanBonGridView.Columns[i].DefaultCellStyle.Format = "c";
                permintaanBonGridView.Columns[i].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            }
            var tempLaporanPermintaanBon = new List<LaporanPermintaanBon>(laporanPermintaanBon);
            tempLaporanPermintaanBon.Add(new LaporanPermintaanBon() { tanggal = new DateTime(1,1,1), C100 = laporanPermintaanBon.Sum(x=>x.C100), C50 = laporanPermintaanBon.Sum(x=>x.C50),C20 = laporanPermintaanBon.Sum(x=>x.C20)});
            foreach (var temp in tempLaporanPermintaanBon)
            {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = ((DateTime)temp.tanggal).ToShortDateString();
                if (temp.tanggal == new DateTime(1, 1, 1))
                    cell.Value = "SUM";
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
            if(permintaanBonGridView.Rows.Count>1)
            {
                permintaanBonGridView.Rows[permintaanBonGridView.RowCount - 1].DefaultCellStyle.BackColor = Color.PapayaWhip;
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
                        bool event1 = true,event2 = true;
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
                        if(!event1)
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
                                    event2 = false;
                                    
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
                        }
                        if (!event2)
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
                                            cText += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                        else
                                            cText += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
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
                        }
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
                        if(!event1)
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
                        if(!event1)
                        {
                            reader.Close();
                            cText = "SELECT AVG(isiCrm100),AVG(isiCrm50), AVG(isiCrm20) FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE kodePkt = '" + KodePkt[pktIndex] + "' ";
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
                                    event2 = false;
                                else
                                {
                                    tempIsiCrm.d100 = (Int64)reader[0];
                                    tempIsiCrm.d50 = (Int64)reader[1];
                                    tempIsiCrm.d20 = (Int64)reader[2];
                                    tempIsiCrm.tgl = tempDate;
                                }
                                reader.Close();
                            }
                        }
                        if (!event2)
                        {
                            Console.WriteLine("ISI CRM MASUK EVENT 3");
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
                                            cText += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                        else
                                            cText += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
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
                            else{
                                
                                tempStdDeviasi.d100 = (Double)reader[0];
                                tempStdDeviasi.d50 = (Double)reader[1];
                                tempStdDeviasi.d20 = (Double)reader[2];
                                tempStdDeviasi.tgl = tempDate;
                                reader.Close();
                            }
                        }
                        if(!event1)
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
                        bool event2 = true;
                        if(reader.Read())
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
                        if(!event1)
                        {
                            reader.Close();
                            //QUERY UNTUK CONDITION TANPA HARI
                            cText = "SELECT AVG(sislokCrm100), AVG(sislokCrm50), AVG(sislokCrm20) FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE kodePkt = '" + KodePkt[pktIndex] + "' ";
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
                                    event2 = false;
                                }
                                else
                                {
                                    tempSislokCrm.d100 = (Int64)reader[0];
                                    tempSislokCrm.d50 = (Int64)reader[1];
                                    tempSislokCrm.d20 = (Int64)reader[2];
                                    tempSislokCrm.tgl = tempDate;
                                }
                            }
                        }
                        if(!event2)
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
                                            cText += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                        else
                                            cText += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "'))";
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
                        if(!event1)
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
                    String errMsg = "";
                    bool iserror = false;
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
                        bool event1 = true, event2 = true;
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
                        if(!event1)
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
                                    event2 = false;
                                }
                                else
                                {
                                    iserror = true;
                                    errMsg=errMsg+"\nPrediksi Lainnya Tanggal " + tempDate.ToShortDateString() + " Menggunakan Event2";
                                    tempSislokCdm.d100 = (Int64)reader[0];
                                    tempSislokCdm.d50 = (Int64)reader[1];
                                    tempSislokCdm.d20 = (Int64)reader[2];
                                    tempSislokCdm.tgl = tempDate;
                                }
                                reader.Close();
                            }
                        }
                        if(!event2)
                        {
                            reader.Close();

                            cText = "SELECT ISNULL(AVG(sislokCdm100),0), ISNULL(AVG(sislokCdm50),0), ISNULL(AVG(sislokCdm20),0) FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE kodePkt = '" + KodePkt[pktIndex] + "' ";
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
                                            cText += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                        else
                                            cText += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
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
                                    iserror = true;
                                    errMsg = errMsg + "Data Lainnya tanggal" + tempDate.ToShortDateString() + "Tidak ada!\nValue menjadi 0";
                                }
                                else
                                {
                                    iserror = true;
                                    errMsg = errMsg + "\nPrediksi Lainnya Tanggal " + tempDate.ToShortDateString() + " Menggunakan Event 3";
                                    Console.WriteLine("READER SISLOK CDM " + reader[0].ToString());
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
                    if(iserror)
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
                        bool event1 = true, event2 = true;
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
                        if(!event1)
                        {
                            reader.Close();
                            //SislokCdm
                            cText = "SELECT AVG(CAST(sislokAtm100 AS FLOAT)/NULLIF(isiAtm100,0)), AVG(CAST(sislokAtm50 AS FLOAT)/NULLIF(isiAtm50,0)), AVG(CAST(sislokAtm20 AS FLOAT)/NULLIF(isiAtm20,0)) FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE kodePkt = '" + KodePkt[pktIndex] + "' ";
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
                                if (String.IsNullOrEmpty(reader[0].ToString()) || String.IsNullOrEmpty(reader[1].ToString()) || String.IsNullOrEmpty(reader[2].ToString()))
                                    event2 = false;
                                else
                                {
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
                                }
                                reader.Close();
                            }
                        }
                        if (!event2)
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
                                            cText += " AND ((YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                                        else
                                            cText += "OR (YEAR(TA.tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(TA.tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
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
                        if(!event1)
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
            Database1Entities db = new Database1Entities();

            String kodePkt = KodePkt[pktIndex];
            var query = (from a in db.Approvals
                         join da in db.DetailApprovals on a.idApproval equals da.idApproval
                         where a.kodePkt == kodePkt
                         select new { Approval = a, DetailApproval = da }).ToList();
            if (!query.Any())
            {
                bon.Add(new Denom() { tgl = Variables.todayDate, d100 = 0, d20 = 0, d50 = 0 });
            }
            int maxIdApproval = query.Max(x => x.Approval.idApproval);
            Console.WriteLine("Max Id Approval = " + maxIdApproval);
            query = query.Where(x => x.Approval.idApproval == maxIdApproval).ToList();
            if (query.Any())
            {
                bon = (from x in query
                       where x.DetailApproval.tanggal >= tanggalOptiMin
                       && x.DetailApproval.bon100 != -1
                       select new Denom()
                       {
                           tgl = (DateTime)x.DetailApproval.tanggal,
                           d100 = (Int64)x.DetailApproval.bon100,
                           d50 = (Int64)x.DetailApproval.bon50,
                           d20 = (Int64)x.DetailApproval.bon20,
                       }).ToList();
                if (!bon.Any())
                {
                    bon.Add(new Denom() { tgl = Variables.todayDate, d100 = 0, d20 = 0, d50 = 0 });
                }
                jumlahBonLaporan = bon.Count;
            }
            if(!bon.Any())
            {
                bon.Add(new Denom() { tgl = DateTime.Today, d100 = 0, d20 = 0, d50 = 0 });
            }
            Console.WriteLine("Bon yang disetujui");
            foreach (var temp in bon)
            {
                Console.WriteLine("Tgl: " + temp.tgl + " 100: " + temp.d100 + " 50: " + temp.d50 + " 20: " + temp.d20);
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
                      where x.kodePkt == kodepkt && (y.tanggal >= tanggalOptiMin) && ((DateTime)x.tanggal) < today select new {Approval = x, DetailApproval = y}).ToList();
            int maxIdApproval = 0;
            if(q2.Any())
                maxIdApproval = q2.Max(x => x.Approval.idApproval);
            q2 = (from x in q2
                  where x.Approval.idApproval == maxIdApproval
                  select x).ToList();
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
            for (int a = 1 + jumlahBonLaporan ; a<prediksiIsiAtm.Count;a++)
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
            for(int a = rekomendasiBon.Count-1;a>0;a--)
            {
                if(tanggalSkip.Contains(rekomendasiBon[a].tgl))
                {
                    if(a>0)
                    { rekomendasiBon[a - 1].d100 += rekomendasiBon[a].d100;
                    rekomendasiBon[a - 1].d50 += rekomendasiBon[a].d50;
                    rekomendasiBon[a - 1].d20 += rekomendasiBon[a].d20;

                    rekomendasiBon[a].d100 = 0;
                    rekomendasiBon[a].d50 = 0;
                    rekomendasiBon[a].d20 = 0;
                        }
                }
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

            if (rasio50Txt.Text.Trim().Length == 0)
                targetRasio50 = 0;
            else
                targetRasio50 = Double.Parse(rasio50Txt.Text);
            if (rasio20Txt.Text.Trim().Length == 0 )
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
            if (Math.Round((Double)saldoAkhirH.d100 / (prediksiIsiAtm[prediksiIsiAtm.Count - 1].d100 + isiCrm[prediksiIsiAtm.Count - 1].d100),2) <= targetRasio100)
            {
                Console.WriteLine();
                Console.WriteLine((Double)saldoAkhirH.d100 / (prediksiIsiAtm[prediksiIsiAtm.Count - 1].d100 + isiCrm[prediksiIsiAtm.Count - 1].d100));
                flag100rasiounder = true;
            }
            if (Math.Round((Double)saldoAkhirH.d50 / (prediksiIsiAtm[prediksiIsiAtm.Count - 1].d50 + isiCrm[prediksiIsiAtm.Count - 1].d50),2) <= targetRasio50)
            {
                Console.WriteLine((Double)saldoAkhirH.d50 / (prediksiIsiAtm[prediksiIsiAtm.Count - 1].d50 + isiCrm[prediksiIsiAtm.Count - 1].d50));
                flag50rasiounder = true;
            }
            if (Math.Round((Double)saldoAkhirH.d20 / (prediksiIsiAtm[prediksiIsiAtm.Count - 1].d20 + isiCrm[prediksiIsiAtm.Count - 1].d20),2) <= targetRasio20)
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
            //Add Average
            var avg = (from x in rekomendasiBon
                       group x by true into g
                       select new
                       {
                           tgl = new DateTime(1, 1, 1),
                           d100 = g.Sum(x=>x.d100),
                           d50 = g.Sum(x=>x.d50),
                           d20 = g.Sum(x=>x.d20)
                       }).FirstOrDefault();
            if(avg!=null)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.DefaultCellStyle.BackColor = Color.PapayaWhip;
                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = "SUM";
                row.Cells.Add(cell);
                DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
                cell1.Value = avg.d100;
                row.Cells.Add(cell1);
                DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                cell2.Value = avg.d50;
                row.Cells.Add(cell2);
                DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                cell3.Value = avg.d20;
                row.Cells.Add(cell3);
                rekomendasiBonGridView.Rows.Add(row);
            }
        }
        void loadTableBon()
        {
            if (firstRun)
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
                DateTime tempDate = tanggalOptiMin.AddDays(jumlahBonLaporan);
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
                firstRun = false;
            }

            //for(int a=0;a<bonGridView.Rows.Count;a++)
            //{
            //    var row = bonGridView.Rows[a];
            //    var q = laporanPermintaanBon.Where(x => ((DateTime)x.tanggal).Date == DateTime.Parse(row.Cells[0].ToString()).Date).FirstOrDefault();
            //    if(q!=null)
            //    {
            //        row.Cells[1].Value = q.C100;
            //        row.Cells[2].Value = q.C50;
            //        row.Cells[3].Value = q.C20;
            //    }
            //}
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
                    counter++;
                }
                else
                {
                    saldoAkhir.d100 = saldoAwal.d100 + sislokCdm[counter].d100 + sislokCrm[counter].d100 + (Int64)(rasioSislokAtm[counter].d100 * prediksiIsiAtm[counter].d100) - prediksiIsiAtm[counter].d100 - isiCrm[counter].d100 + bon[counter].d100 + bonAdhoc.d100 - setorAdhoc.d100;
                    saldoAkhir.d50 = saldoAwal.d50 + sislokCdm[counter].d50 + sislokCrm[counter].d50 + (Int64)(rasioSislokAtm[counter].d50 * prediksiIsiAtm[counter].d50) - prediksiIsiAtm[counter].d50 - isiCrm[counter].d50 + bon[counter].d50 + bonAdhoc.d50 - setorAdhoc.d50;
                    saldoAkhir.d20 = saldoAwal.d20 + sislokCdm[counter].d20 + sislokCrm[counter].d20 + (Int64)(rasioSislokAtm[counter].d20 * prediksiIsiAtm[counter].d20) - prediksiIsiAtm[counter].d20 - isiCrm[counter].d20 + bon[counter].d20 + bonAdhoc.d20 - setorAdhoc.d20;
                    counter++;
                }
            }
            else
            {
                saldoAkhir.d100 = saldoAwal.d100 + sislokCdm[counter].d100 + sislokCrm[counter].d100 + (Int64)(rasioSislokAtm[counter].d100 * prediksiIsiAtm[counter].d100) - prediksiIsiAtm[counter].d100 - isiCrm[counter].d100 + bon[counter].d100 + bonAdhoc.d100 - setorAdhoc.d100;
                saldoAkhir.d50 = saldoAwal.d50 + sislokCdm[counter].d50 + sislokCrm[counter].d50 + (Int64)(rasioSislokAtm[counter].d50 * prediksiIsiAtm[counter].d50) - prediksiIsiAtm[counter].d50 - isiCrm[counter].d50 + bon[counter].d50 + bonAdhoc.d50 - setorAdhoc.d50;
                saldoAkhir.d20 = saldoAwal.d20 + sislokCdm[counter].d20 + sislokCrm[counter].d20 + (Int64)(rasioSislokAtm[counter].d20 * prediksiIsiAtm[counter].d20) - prediksiIsiAtm[counter].d20 - isiCrm[counter].d20 + bon[counter].d20 + bonAdhoc.d20 - setorAdhoc.d20;
                counter++;
            }
            if (setorBaru.tgl.ToShortDateString() == tanggalOptiMin.ToShortDateString())
            {
                saldoAkhir.d100 -= setorBaru.d100;
                saldoAkhir.d50 -= setorBaru.d50;
                saldoAkhir.d20 -= setorBaru.d20;
            }
           

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
                if (bonGridView.Rows[a].Cells[1].Value.ToString() == "0" || String.IsNullOrEmpty(bonGridView.Rows[a].Cells[1].Value.ToString()))
                {
                    bon100 = 0;
                }
                else
                {
                    String temp = (String) bonGridView.Rows[a].Cells[1].Value.ToString();
                    temp = temp.Replace("Rp", "");
                    temp = temp.Replace(".", "");
                    bon100 = Int64.Parse(temp);
                }
                if (bonGridView.Rows[a].Cells[2].Value.ToString() == "0" || String.IsNullOrEmpty(bonGridView.Rows[a].Cells[2].Value.ToString()))
                {
                    bon50 = 0;
                }
                else
                {
                    String temp = (String)bonGridView.Rows[a].Cells[2].Value.ToString();
                    temp = temp.Replace("Rp", "");
                    temp = temp.Replace(".", "");
                    bon50 = Int64.Parse(temp);
                }
                if (bonGridView.Rows[a].Cells[3].Value.ToString()== "0" || String.IsNullOrEmpty(bonGridView.Rows[a].Cells[3].Value.ToString()))
                {
                    bon20 = 0;
                }
                else
                {
                    String temp = (String)bonGridView.Rows[a].Cells[3].Value.ToString();
                    temp = temp.Replace("Rp", "");
                    temp = temp.Replace(".", "");
                    bon20 = Int64.Parse(temp);
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
                Console.WriteLine(counter);
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
            rasio.d100 = Double.NaN;
            rasio.d50 = Double.NaN;
            rasio.d20 = Double.NaN;
            if (counter < isiCrm.Count)
            {
                rasio.d100 = saldoAkhir.d100 / ((Double)prediksiIsiAtm[counter].d100 + (Double)isiCrm[counter].d100);
                rasio.d50 = saldoAkhir.d50 / ((Double)prediksiIsiAtm[counter].d50 + (Double)isiCrm[counter].d50);
                rasio.d20 = saldoAkhir.d20 / ((Double)prediksiIsiAtm[counter].d20 + (Double)isiCrm[counter].d20);
            }
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
        void loadTableIsiCRM()
        {
            DataTable table = new DataTable();
            List<Denom> totalIsi = new List<Denom>();
            for (int i=0;i<isiCrm.Count;i++)
            {
                var temp = isiCrm[i];
                Denom tempD = new Denom();
                tempD.tgl = temp.tgl;
                tempD.d100 = temp.d100;
                tempD.d50 = temp.d50;
                tempD.d20 = temp.d20;
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

            
            isiCrmGridView.DataSource = table;
            isiCrmGridView.Columns["100"].DefaultCellStyle.Format = "n0";
            isiCrmGridView.Columns["50"].DefaultCellStyle.Format = "n0";
            isiCrmGridView.Columns["20"].DefaultCellStyle.Format = "n0";
            isiCrmGridView.Rows[isiCrmGridView.Rows.Count-1].DefaultCellStyle.BackColor = Color.PapayaWhip;
        }
        void loadTableIsiATM()
        {
            DataTable table = new DataTable();
            List<Denom> totalIsi = new List<Denom>();
            for (int i = 0; i < prediksiIsiAtm.Count; i++)
            {
                var temp = prediksiIsiAtm[i];
                Denom tempD = new Denom();
                tempD.tgl = temp.tgl;
                tempD.d100 = temp.d100;
                tempD.d50 = temp.d50;
                tempD.d20 = temp.d20;
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


            isiAtmGridView.DataSource = table;
            isiAtmGridView.Columns["100"].DefaultCellStyle.Format = "n0";
            isiAtmGridView.Columns["50"].DefaultCellStyle.Format = "n0";
            isiAtmGridView.Columns["20"].DefaultCellStyle.Format = "n0";
            isiAtmGridView.Rows[isiCrmGridView.Rows.Count - 1].DefaultCellStyle.BackColor = Color.PapayaWhip;
        }
        void loadTableSislokCRM()
        {
            DataTable table = new DataTable();
            List<Denom> totalSislok = new List<Denom>();

            Console.WriteLine("");
            Console.WriteLine("Load Table Total Sislok");
            Console.WriteLine("========================");

            for (int i = 0; i < sislokCrm.Count; i++)
            {
                var temp = sislokCrm[i];
                Denom tempD = new Denom();
                tempD.tgl = temp.tgl;
                tempD.d100 = sislokCrm[i].d100;
                tempD.d50 = sislokCrm[i].d50;
                tempD.d20 = sislokCrm[i].d20;
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
            sislokCrmGridView.DataSource = table;
            sislokCrmGridView.Columns["100"].DefaultCellStyle.Format = "n0";
            sislokCrmGridView.Columns["50"].DefaultCellStyle.Format = "n0";
            sislokCrmGridView.Columns["20"].DefaultCellStyle.Format = "n0";
                 
            sislokCrmGridView.Rows[sislokAtmGridView.Rows.Count - 1].DefaultCellStyle.BackColor = Color.PapayaWhip;

        }
        void loadTableSislokATM()
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
                tempD.tgl = temp.tgl;
                tempD.d100 = (Int64)Math.Round((temp.d100 * rasioSislokAtm[i].d100), 0);
                tempD.d50 = (Int64)Math.Round((temp.d50 * rasioSislokAtm[i].d50), 0);
                tempD.d20 = (Int64)Math.Round((temp.d20 * rasioSislokAtm[i].d20), 0);
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
            sislokAtmGridView.DataSource = table;
            sislokAtmGridView.Columns["100"].DefaultCellStyle.Format = "n0";
            sislokAtmGridView.Columns["50"].DefaultCellStyle.Format = "n0";
            sislokAtmGridView.Columns["20"].DefaultCellStyle.Format = "n0";

            sislokAtmGridView.Rows[sislokAtmGridView.Rows.Count - 1].DefaultCellStyle.BackColor = Color.PapayaWhip;

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
        void validasiPermintaanBon()
        {
            String tkodePkt = KodePkt[pktIndex];
            Database1Entities db1 = new Database1Entities();
            var query = (from x in db1.Approvals
                         join y in db1.DetailApprovals on x.idApproval equals y.idApproval
                         where x.kodePkt == tkodePkt
                         && y.tanggal >= tanggalOptiMin
                         select new { Approval = x, DetailApproval = y }).ToList();

            foreach (var temp in bon)
            {
                var q2 = (from x in query
                          where x.DetailApproval.tanggal == temp.tgl
                          orderby x.DetailApproval.idDetailApproval descending
                          select new
                          {
                              bon100 = x.DetailApproval.bon100,
                              bon50 = x.DetailApproval.bon50,
                              bon20 = x.DetailApproval.bon20
                          }).ToList();
                if (q2.Any())
                {
                    if (q2[0].bon100 != temp.d100 || q2[0].bon50 != temp.d50 || q2[0].bon20 != temp.d20)
                        MessageBox.Show("Data bon yang disetujui tidak sesuai dengan approval!");
                }
            }
        }
        private void loadPrediksiBtn_Click(object sender, EventArgs e)
        {
            pktComboBox.Enabled = false;
            PopupInformationBoard pib = new PopupInformationBoard(KodePkt[pktIndex]);
            pib.Show();
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
                //TESTING

                //ENDTESTING
                loadSislokCrm();
                loadSetor();
                //Hitungan dengan metode kedua
                if(MetodeHitungLainnyaComboBox.SelectedItem.ToString() == "Std Deviasi")
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
                if(MetodeHitungLainnyaComboBox.SelectedItem.ToString() == "Historis + Std Deviasi")
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

                loadTableIsiCRM();
                loadTableIsiATM();
                loadTableSislokATM();
                loadTableSislokCRM();
                loadTableBonYangDisetujui();

                loadPermintaanAdhoc();

                validasiPermintaanBon();
            //}catch(Exception err)
            //{
            //    MessageBox.Show(err.ToString());
            //}
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
            if(MessageBox.Show("Approve Bon "+KodePkt[pktIndex]+"?", "Approve Bon", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                labelKodePkt.Text = "";
                List<Denom> bonYangDisetujui = loadBonYangDisetujuiFromTable();
                Denom bonAdhoc = loadBonAdhocFromTxt();
                Denom setorAdhoc = loadSetorAdhocFromTxt();
                Denom setorTxt = loadSetorFromTxt();
                
                Approval newA = new Approval();
                newA.tanggal = tanggalOptiMin;
                newA.kodePkt = KodePkt[pktIndex];
                db.Approvals.Add(newA);
                db.SaveChanges();
                var lastApproval = (from x in db.Approvals select x).ToList();
                DateTime tempTanggal = tanggalOptiMin;
                int count = 0;
                int jumlahbon = 0;
                for (int i=0;i<bon.Count;i++)
                {
                    DetailApproval newDetailA = new DetailApproval()
                    {
                        adhoc100 = 0,
                        adhoc20 = 0,
                        adhoc50 = 0,
                        bon100 = 0,
                        bon20 = 0,
                        bon50 = 0,
                        isiATM100 = 0,
                        isiATM20 = 0,
                        isiATM50 = 0,
                        isiCRM100 = 0,
                        isiCRM20 = 0,
                        isiCRM50 = 0,
                        saldoAwal100 = 0,
                        saldoAwal20 = 0,
                        saldoAwal50 = 0,
                        setor100 = 0,
                        setor50 = 0,
                        setor20 = 0,
                        setorAdhoc100 = 0,
                        setorAdhoc20 = 0,
                        setorAdhoc50 = 0,
                        sislokATM100 = 0,
                        sislokATM20 = 0,
                        sislokATM50 = 0,
                        sislokCDM100 = 0,
                        sislokCDM20 = 0,
                        sislokCDM50 = 0,
                        sislokCRM100 = 0,
                        sislokCRM20 = 0,
                        sislokCRM50 = 0
                    };
                    newDetailA.idApproval = lastApproval[lastApproval.Count - 1].idApproval;
                    newDetailA.tanggal = tempTanggal;
                    
                    if (i == 0)
                    {
                        //Adhoc
                        newDetailA.adhoc100 = bonAdhoc.d100;
                        newDetailA.adhoc50 = bonAdhoc.d50;
                        newDetailA.adhoc20 = bonAdhoc.d20;
                        newDetailA.setorAdhoc100 = setorAdhoc.d100;
                        newDetailA.setorAdhoc50 = setorAdhoc.d50;
                        newDetailA.setorAdhoc20 = setorAdhoc.d20;

                    }
                    if (((DateTime)newDetailA.tanggal).ToShortDateString() == tglSetor.Value.ToShortDateString())
                    {
                        newDetailA.setor100 = setorTxt.d100;
                        newDetailA.setor50 = setorTxt.d50;
                        newDetailA.setor20 = setorTxt.d20;
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
                    newDetailA.sislokCDM100 = sislokCdm[i].d100;
                    newDetailA.sislokCDM50 = sislokCdm[i].d50;
                    newDetailA.sislokCDM20 = sislokCdm[i].d20;
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

                   

                    if (setor.Where(x => ((DateTime)x.tgl).Date == tempTanggal.Date).FirstOrDefault() != null)
                    {
                        Denom tS = setor.Where(x => ((DateTime)x.tgl).Date == tempTanggal.Date).FirstOrDefault();
                        newDetailA.setor100 = tS.d100;
                        newDetailA.setor50 = tS.d50;
                        newDetailA.setor20 = tS.d20;
                    }
                    if (newDetailA.tanggal.Value.ToShortDateString() == tglSetor.Value.ToShortDateString())
                    {
                        newDetailA.setor100 = setorTxt.d100;
                        newDetailA.setor50 = setorTxt.d50;
                        newDetailA.setor20 = setorTxt.d20;
                    }
                    db.DetailApprovals.Add(newDetailA);
                    tempTanggal = tempTanggal.AddDays(1);
                    db.SaveChanges();
                    count++;
                    jumlahbon++;
                }

                for (int i=0;i<=bonYangDisetujui.Count ;i++)
                {
                    DetailApproval newDetailA = new DetailApproval()
                    {
                        adhoc100 = 0,
                        adhoc20 = 0,
                        adhoc50 = 0,
                        bon100 = 0,
                        bon20 = 0,
                        bon50 = 0,
                        isiATM100 = 0,
                        isiATM20 = 0,
                        isiATM50 = 0,
                        isiCRM100 = 0,
                        isiCRM20 = 0,
                        isiCRM50 = 0,
                        saldoAwal100 = 0,
                        saldoAwal20 = 0,
                        saldoAwal50 = 0,
                        setor100 = 0,
                        setor50 = 0,
                        setor20 = 0,
                        setorAdhoc100 = 0,
                        setorAdhoc20 = 0,
                        setorAdhoc50 = 0,
                        sislokATM100 = 0,
                        sislokATM20 = 0,
                        sislokATM50 = 0,
                        sislokCDM100 = 0,
                        sislokCDM20 = 0,
                        sislokCDM50 = 0,
                        sislokCRM100 = 0,
                        sislokCRM20 = 0,
                        sislokCRM50 = 0
                    };
                    newDetailA.idApproval = lastApproval[lastApproval.Count-1].idApproval;
                    newDetailA.tanggal = tempTanggal;

                   
                    
                    if(i<bonYangDisetujui.Count)
                    {
                        //Bon
                        newDetailA.bon100 = bonYangDisetujui[i].d100;
                        newDetailA.bon50 = bonYangDisetujui[i].d50;
                        newDetailA.bon20 = bonYangDisetujui[i].d20;
                    }                              
                    else
                    {
                        newDetailA.bon100 = -1;
                        newDetailA.bon50 = -1;
                        newDetailA.bon20 = -1;
                    }

                    //Saldo awal
                    newDetailA.saldoAwal100 = saldo[jumlahbon + i].d100;
                    newDetailA.saldoAwal50 = saldo[jumlahbon + i].d50;
                    newDetailA.saldoAwal20 = saldo[jumlahbon + i].d20;
                    
                    if (count < sislokCrm.Count)
                    {
                        
                        newDetailA.sislokCRM100 = sislokCrm[count].d100;
                        newDetailA.sislokCRM50 = sislokCrm[count].d50;
                        newDetailA.sislokCRM20 = sislokCrm[count].d20;
                        newDetailA.sislokCDM100 = sislokCdm[count].d100;
                        newDetailA.sislokCDM50 = sislokCdm[count].d50;
                        newDetailA.sislokCDM20 = sislokCdm[count].d20;
                        newDetailA.sislokATM100 = (Int64)(rasioSislokAtm[count].d100 * prediksiIsiAtm[count].d100);
                        newDetailA.sislokATM50 = (Int64)(rasioSislokAtm[count].d50 * prediksiIsiAtm[count].d50);
                        newDetailA.sislokATM20 = (Int64)(rasioSislokAtm[count].d20 * prediksiIsiAtm[count].d20);
                        //Isi
                        newDetailA.isiATM100 = prediksiIsiAtm[count].d100;
                        newDetailA.isiATM50 = prediksiIsiAtm[count].d50;
                        newDetailA.isiATM20 = prediksiIsiAtm[count].d20;
                        newDetailA.isiCRM100 = isiCrm[count].d100;
                        newDetailA.isiCRM50 = isiCrm[count].d50;
                        newDetailA.isiCRM20 = isiCrm[count].d20;
                    }
                    else
                    {
                        //Sislok
                        newDetailA.sislokCRM100 = 0;
                        newDetailA.sislokCRM50 = 0;
                        newDetailA.sislokCRM20 = 0;
                        newDetailA.sislokCDM100 = 0;
                        newDetailA.sislokCDM50 = 0;
                        newDetailA.sislokCDM20 = 0;
                        newDetailA.sislokATM100 = 0;
                        newDetailA.sislokATM50 = 0;
                        newDetailA.sislokATM20 = 0;
                        //Isi
                        newDetailA.isiATM100 =0;
                        newDetailA.isiATM50 = 0;
                        newDetailA.isiATM20 = 0;
                        newDetailA.isiCRM100 =0;
                        newDetailA.isiCRM50 = 0;
                        newDetailA.isiCRM20 = 0;
                    }
                    
                  

                    if(setor.Where(x=>((DateTime)x.tgl).Date == tempTanggal.Date).FirstOrDefault()!=null)
                    {
                        Denom tS = setor.Where(x => ((DateTime)x.tgl).Date == tempTanggal.Date).FirstOrDefault();
                        newDetailA.setor100 = tS.d100;
                        newDetailA.setor50 = tS.d50;
                        newDetailA.setor20 = tS.d20;
                    }
                    if (newDetailA.tanggal.Value.ToShortDateString() == tglSetor.Value.ToShortDateString())
                    {
                        newDetailA.setor100 = setorTxt.d100;
                        newDetailA.setor50 = setorTxt.d50;
                        newDetailA.setor20 = setorTxt.d20;
                    }
                    tempTanggal = tempTanggal.AddDays(1);

                    db.DetailApprovals.Add(newDetailA);
                    db.SaveChanges();
                    count++;
                }
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
                pktComboBox.Enabled = true;
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
            sislokAtmGridView.DataSource = null;
            isiCrmGridView.DataSource = null;
            saldoGridView.DataSource = null;
            permintaanAdhocGridView.DataSource = null;
        }
        Denom loadBonAdhocFromTxt()
        {
            Denom bonAdhoc = new Denom();
            bonAdhoc.d100 = (Int64)bonAdhoc100Txt.Value;
            bonAdhoc.d50 = (Int64)bonAdhoc50Txt.Value;
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
            firstRun = true;
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
        void pktComboBox_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
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
            labelKodePkt.Text = KodePkt[pktIndex];
            loadE2E();
            firstRun = true;
        }

        private void Txt_ValueChanged(object sender, EventArgs e)
        {
            loadRasio();
            loadTableRasio();
            loadTableSaldo();
        }

        private void setorAdhoc50Txt_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void setorAdhoc50Txt_Scroll(object sender, ScrollEventArgs e)
        {
            
        }

        private void bufferIsiAtm100Num_ValueChanged(object sender, EventArgs e)
        {
        }

        private void permintaanBonGridView_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewSelectedCellCollection cells = permintaanBonGridView.SelectedCells;
            foreach (DataGridViewCell cell in cells)
            {
                int rowidx = cell.RowIndex;
                int colidx = cell.ColumnIndex;
                permintaanBonGridView.Rows[rowidx].Cells[colidx].Value = permintaanBonGridView.Rows[rowidx].Cells[colidx].Value.ToString();
            }
            for (int a = 0; a < permintaanBonGridView.Rows.Count; a++)
            {
                for (int b = 0; b < permintaanBonGridView.Columns.Count; b++)
                {
                    if (!cells.Contains(permintaanBonGridView.Rows[a].Cells[b]))
                    {
                        Int64 buf;
                        if (Int64.TryParse(permintaanBonGridView.Rows[a].Cells[b].Value.ToString(), out buf))
                            permintaanBonGridView.Rows[a].Cells[b].Value = Int64.Parse(permintaanBonGridView.Rows[a].Cells[b].Value.ToString().Replace("Rp.", "").Replace(".", "").Trim());
                    }
                }
            }
        }

        private void permintaanAdhocGridView_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewSelectedCellCollection cells = permintaanAdhocGridView.SelectedCells;
            foreach (DataGridViewCell cell in cells)
            {
                int rowidx = cell.RowIndex;
                int colidx = cell.ColumnIndex;
                permintaanAdhocGridView.Rows[rowidx].Cells[colidx].Value = permintaanAdhocGridView.Rows[rowidx].Cells[colidx].Value.ToString();
            }
            for (int a = 0; a < permintaanAdhocGridView.Rows.Count; a++)
            {
                for (int b = 0; b < permintaanAdhocGridView.Columns.Count; b++)
                {
                    if (!cells.Contains(permintaanAdhocGridView.Rows[a].Cells[b]))
                    {
                        Int64 buf;
                        if (Int64.TryParse(permintaanAdhocGridView.Rows[a].Cells[b].Value.ToString(), out buf))
                            permintaanAdhocGridView.Rows[a].Cells[b].Value = Int64.Parse(permintaanAdhocGridView.Rows[a].Cells[b].Value.ToString().Replace("Rp.", "").Replace(".", "").Trim());
                    }
                }
            }
        }

        private void rasioGridView_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewSelectedCellCollection cells = rasioGridView.SelectedCells;
            foreach (DataGridViewCell cell in cells)
            {
                int rowidx = cell.RowIndex;
                int colidx = cell.ColumnIndex;
                rasioGridView.Rows[rowidx].Cells[colidx].Value = rasioGridView.Rows[rowidx].Cells[colidx].Value.ToString();
            }
            for (int a = 0; a < rasioGridView.Rows.Count; a++)
            {
                for (int b = 0; b < rasioGridView.Columns.Count; b++)
                {
                    if (!cells.Contains(rasioGridView.Rows[a].Cells[b]))
                    {
                        Int64 buf;
                        if (Int64.TryParse(rasioGridView.Rows[a].Cells[b].Value.ToString(), out buf))
                            rasioGridView.Rows[a].Cells[b].Value = Int64.Parse(rasioGridView.Rows[a].Cells[b].Value.ToString().Replace("Rp.", "").Replace(".", "").Trim());
                    }
                }
            }
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
public class Denom
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
public class Rasio
{
    public DateTime tgl { set; get; }
    public Double d100 { set; get; }
    public Double d50 { set; get; }
    public Double d20 { set; get; }
}
