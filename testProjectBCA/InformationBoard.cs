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
        
        List<Denom> sislokCrm = new List<Denom>();
        List<Denom> sislokCdm = new List<Denom>();
        List<Rasio> rasioSislokAtm = new List<Rasio>();

        List<Denom> isiCrm = new List<Denom>();

        List<Denom> bon = new List<Denom>();
        List<Denom> rekomendasiBon = new List<Denom>();

        Denom rekomendasiAdhoc = new Denom();
        List<Denom> setor = new List<Denom>();

        List<Denom> saldoAwalIdeal = new List<Denom>();
        
        List<Denom> prediksiIsiAtm; //dalem class denom ada 100,50,20

        
        int pktIndex;
        DateTime tanggalOptiMin ,tanggalOptiMax; 
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
                List<String> tempListKodePkt = (from x in db.Pkts
                                                select x.kodePkt).ToList();

                KodePkt = new List<String>();
                var query = (from x in db.Optis
                             select x.Cashpoint.kodePkt).ToList();
                foreach (String temp2 in tempListKodePkt)
                {
                    foreach(var temp3 in query)
                    {
                        if (temp2 == temp3)
                        { KodePkt.Add(temp2); break; }
                    }
                }
                pktComboBox.DataSource = KodePkt;
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
            dataGridView1.Columns.Add("Tanggal", "Tanggal");
            dataGridView1.Columns.Add("100000", "100000");
            dataGridView1.Columns.Add("50000", "50000");
            dataGridView1.Columns.Add("20000", "20000");
            for (int i = 1; i < 4; i++)
            {
                dataGridView1.Columns[i].DefaultCellStyle.Format = "c";
                dataGridView1.Columns[i].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            }
            loadPrediksiOpti();
         
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
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            
            //init
            prediksiIsiAtm = new List<Denom>();
            
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
                            Denom tempSislokCrm = new Denom();

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

                            prediksiIsiAtm.Add(tempPrediksi);
                            reader.Close();

                            minDate = minDate.AddDays(1);
                            Console.WriteLine(minDate);
                            dataGridView1.Rows.Add(row);
                        }
                    }
                    cmd.CommandText = "SELECT e2e FROM Pkt WHERE kodePkt = '" + KodePkt[pktIndex]+"'";
                    reader = cmd.ExecuteReader();
                    reader.Read();
                    e2eTxt.Text = "Jenis E2E: " + reader[0];
                    sqlConnection1.Close();
                }
                
            }
            // Data is accessible through the DataReader object here.

           
        }
        private void loadPrediksiBtn_Click(object sender, EventArgs e)
        {
            if (rasio100Txt.Text == "" || rasio50Txt.Text == "" || rasio20Txt.Text == "")
            {
                MessageBox.Show("Input target rasio!");
            }
            else
            {
                //loadSislokCrm();

                loadSislokCdm();
                loadRasioSislokAtm();

                loadSaldoAwal();
                loadBon();

                loadIsiCrm();
                loadSislokCrm();
                //Console.WriteLine(sislokCrm[0].d100);
                loadRekomendasiBon();
                
                loadTableRekomendasiBon();

                loadTableBon();
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
                    cmd.CommandText = "SELECT saldoAkhir100, saldoAkhir50, saldoAkhir20 FROM TransaksiAtms WHERE kodePkt = '" + KodePkt[pktIndex] + "' AND tanggal = '" + tanggalOptiMin.AddDays(-1).ToShortDateString() + "'";
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
        void loadIsiCrm()
        {
            Console.WriteLine();
            Console.WriteLine("Load Isi CRM");
            Console.WriteLine("======================");
            isiCrm = new List<Denom>();
            int rowCount = dataGridView1.Rows.Count;
            //dataGridView1.Hide();
            DateTime startDate = Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = Convert.ToDateTime(dataGridView1.Rows[rowCount - 1].Cells[0].Value);
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
                        String cText = "SELECT AVG(isiCrm100), AVG(isiCrm50), AVG(isiCrm20) FROM TransaksiAtms WHERE kodePkt = '" + KodePkt[pktIndex] + "' AND DATENAME(WEEKDAY, Tanggal) = '" + tempDate.DayOfWeek.ToString() + "' AND ";
                        int count = 0;
                        for (int i = 0; i < treeView1.Nodes.Count; i++)
                        {
                            for (int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++)
                            {
                                if (treeView1.Nodes[i].Nodes[j].Checked)
                                {
                                    //MessageBox.Show("A");
                                    if (count++ == 0)
                                        cText += "((YEAR(tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = " + tempDate.GetWeekOfMonth() + ") ";
                                    else
                                        cText += "OR (YEAR(tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1) = " + tempDate.GetWeekOfMonth() + ") ";
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
                    Console.WriteLine(temp.tgl + " " + temp.d100 + " ");
                }
            }
        }
        private void loadSislokCrm()
        {
            Console.WriteLine();
            Console.WriteLine("Load Sislok CRM");
            Console.WriteLine("======================");
            sislokCrm = new List<Denom>();
            int rowCount = dataGridView1.Rows.Count;
            //dataGridView1.Hide();
            DateTime startDate = Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = Convert.ToDateTime(dataGridView1.Rows[rowCount - 1].Cells[0].Value);
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
                        String cText = "SELECT AVG(sislokCrm100), AVG(sislokCrm50), AVG(sislokCrm20) FROM TransaksiAtms WHERE kodePkt = '" + KodePkt[pktIndex] + "' AND DATENAME(WEEKDAY, Tanggal) = '" + tempDate.DayOfWeek.ToString() + "' AND ";
                        int count = 0;
                        for (int i = 0; i < treeView1.Nodes.Count; i++)
                        {
                            for (int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++)
                            {
                                if (treeView1.Nodes[i].Nodes[j].Checked)
                                {
                                    //MessageBox.Show("A");
                                    if (count++ == 0)
                                        cText += "((YEAR(tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = " + tempDate.GetWeekOfMonth() + ") ";
                                    else
                                        cText += "OR (YEAR(tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1) = " + tempDate.GetWeekOfMonth() + ") ";
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
                    Console.WriteLine(temp.tgl + " " + temp.d100 + " ");
                }
            }
        }
        void loadSislokCdm()
        {
            Console.WriteLine();
            Console.WriteLine("Load sislok cdm");
            Console.WriteLine("======================");
            sislokCdm = new List<Denom>();
            int rowCount = dataGridView1.Rows.Count;
            //dataGridView1.Hide();
            DateTime startDate = Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = Convert.ToDateTime(dataGridView1.Rows[rowCount - 1].Cells[0].Value);
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
                        String cText = "SELECT AVG(sislokCdm100), AVG(sislokCdm50), AVG(sislokCdm20) FROM TransaksiAtms WHERE kodePkt = '" + KodePkt[pktIndex] + "' AND DATENAME(WEEKDAY, Tanggal) = '" + tempDate.DayOfWeek.ToString() + "' AND ";
                        int count = 0;
                        for (int i = 0; i < treeView1.Nodes.Count; i++)
                        {
                            for (int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++)
                            {
                                if (treeView1.Nodes[i].Nodes[j].Checked)
                                {
                                    //MessageBox.Show("A");
                                    if (count++ == 0)
                                        cText += "((YEAR(tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = " + tempDate.GetWeekOfMonth() + ") ";
                                    else
                                        cText += "OR (YEAR(tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1) = " + tempDate.GetWeekOfMonth() + ") ";
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
                    Console.WriteLine(temp.tgl + " " + temp.d100 + " ");
                }
            }
        }
        void loadRasioSislokAtm()
        {
            Console.WriteLine();
            Console.WriteLine("Load rasio sislok atm");
            Console.WriteLine("======================");
            rasioSislokAtm = new List<Rasio>();
            int rowCount = dataGridView1.Rows.Count;
            //dataGridView1.Hide();
            DateTime startDate = Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = Convert.ToDateTime(dataGridView1.Rows[rowCount - 1].Cells[0].Value);
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
                        String cText = "SELECT AVG(CAST(sislokAtm100 AS FLOAT)/NULLIF(isiAtm100,0)), AVG(CAST(sislokAtm50 AS FLOAT)/NULLIF(isiAtm50,0)), AVG(CAST(sislokAtm20 AS FLOAT)/NULLIF(isiAtm20,0)) FROM TransaksiAtms WHERE kodePkt = '" + KodePkt[pktIndex] + "' AND DATENAME(WEEKDAY, Tanggal) = '" + tempDate.DayOfWeek.ToString() + "' AND ";
                        int count = 0;
                        for (int i = 0; i < treeView1.Nodes.Count; i++)
                        {
                            for (int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++)
                            {
                                if (treeView1.Nodes[i].Nodes[j].Checked)
                                {
                                    //MessageBox.Show("A");
                                    if (count++ == 0)
                                        cText += "((YEAR(tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = " + tempDate.GetWeekOfMonth() + ") ";
                                    else
                                        cText += "OR (YEAR(tanggal) = " + treeView1.Nodes[i].Text + " AND MONTH(tanggal) = " + treeView1.Nodes[i].Nodes[j].Text + " AND (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1) = " + tempDate.GetWeekOfMonth() + ") ";
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
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = sql;
                cmd.CommandText = "SELECT [tanggal], [100], [50], [20] FROM laporanBon WHERE kodePkt = '" + KodePkt[pktIndex] + "' AND tanggal BETWEEN '" + tanggalOptiMin.ToShortDateString() + "' AND '" + tanggalOptiMax.ToShortDateString() + "'";
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
                }
                sql.Close();
            }
            Console.WriteLine("BON");
            Console.WriteLine(bon[0].d100);
        }
        void loadSetor()
        {

        }
        void loadRekomendasiBon()
        {
            Console.WriteLine();
            Console.WriteLine("Load rekomendasi bon");
            Console.WriteLine("======================");
            rekomendasiBon = new List<Denom>();
            saldoAwalIdeal = new List<Denom>();
            rekomendasiAdhoc = new Denom();
            Double targetRasio100 = Double.Parse(rasio100Txt.Text);
            Double targetRasio50 = Double.Parse(rasio50Txt.Text);
            Double targetRasio20 = Double.Parse(rasio20Txt.Text);
            Denom saldoAkhirH = new Denom();

            Console.WriteLine(prediksiIsiAtm[0].d100);
            Console.WriteLine(rasioSislokAtm[0].d100);
            Console.WriteLine(sislokCdm[0].d100);
            Console.WriteLine(sislokCrm.Count);
            Console.WriteLine(sislokCrm[0].d100);
            Console.WriteLine(isiCrm[0].d100);
            Console.WriteLine(bon[0].d100);
            //Hitung saldo akhir hari h
            saldoAkhirH.d100 = saldoAwal.d100 + (Int64)Math.Round((rasioSislokAtm[0].d100 * prediksiIsiAtm[0].d100)) + sislokCdm[0].d100 + sislokCrm[0].d100 - prediksiIsiAtm[0].d100 - isiCrm[0].d100 + bon[0].d100;
            saldoAkhirH.d50 = saldoAwal.d50 + (Int64)Math.Round((rasioSislokAtm[0].d50 * prediksiIsiAtm[0].d50)) + sislokCdm[0].d50 + sislokCrm[0].d50 - prediksiIsiAtm[0].d50 - isiCrm[0].d50 + bon[0].d50;
            saldoAkhirH.d20 = saldoAwal.d20 + (Int64)Math.Round((rasioSislokAtm[0].d20 * prediksiIsiAtm[0].d20)) + sislokCdm[0].d20 + sislokCrm[0].d20 - prediksiIsiAtm[0].d20 - isiCrm[0].d20 + bon[0].d20;

            rekomendasiAdhoc.d100 = 0;
            rekomendasiAdhoc.d50 = 0;
            rekomendasiAdhoc.d20 = 0;

            if (saldoAkhirH.d100 < 0)
                rekomendasiAdhoc.d100 = -saldoAkhirH.d100;
            if (saldoAkhirH.d50 < 0)
                rekomendasiAdhoc.d50 = -saldoAkhirH.d50;
            if (saldoAkhirH.d20 < 0)
                rekomendasiAdhoc.d20 = -saldoAkhirH.d20;

            //Hitung saldo ideal h+2, h+3, ..
            for (int a = 2 ; a<prediksiIsiAtm.Count;a++)
            {
                Denom temp = new Denom();
                temp.d100 = (Int64)Math.Round(((Double)prediksiIsiAtm[a].d100 + (Double)isiCrm[a].d100) * targetRasio100);
                temp.d50 = (Int64)Math.Round(((Double)prediksiIsiAtm[a].d50 + (Double)isiCrm[a].d50) * targetRasio50);
                temp.d20 = (Int64)Math.Round(((Double)prediksiIsiAtm[a].d20 + (Double)isiCrm[a].d20) * targetRasio20);
                temp.tgl = prediksiIsiAtm[a].tgl;
                saldoAwalIdeal.Add(temp);
            }

            //Ambil saldo akhir ideal h+1 dari saldo awal ideal h+2
            Denom saldoAkhirH1Ideal = new Denom();
            saldoAkhirH1Ideal.d100 = saldoAwalIdeal[0].d100;
            saldoAkhirH1Ideal.d50 = saldoAwalIdeal[0].d50;
            saldoAkhirH1Ideal.d20 = saldoAwalIdeal[0].d20;

            //Hitung rekomendasiBon untuk h+1 (Belom ada setor dan adhoc)
            Denom tempRekomendasiBon = new Denom();
            tempRekomendasiBon.d100 = saldoAkhirH1Ideal.d100
                - saldoAkhirH.d100                                                      //Ambil saldo akhir di hari h (jadi saldo awal h+1)
                - (Int64)Math.Round((rasioSislokAtm[0].d100 * prediksiIsiAtm[0].d100))
                - sislokCrm[0].d100
                - sislokCdm[0].d100
                + prediksiIsiAtm[0].d100
                + isiCrm[0].d100;
            tempRekomendasiBon.d50 = saldoAkhirH1Ideal.d50
                - saldoAkhirH.d50
                - (Int64)Math.Round((rasioSislokAtm[0].d50 * prediksiIsiAtm[0].d50))
                - sislokCrm[0].d50
                - sislokCdm[0].d50
                + prediksiIsiAtm[0].d50
                + isiCrm[0].d50;
            tempRekomendasiBon.d20 = saldoAkhirH1Ideal.d20
                - saldoAkhirH.d20
                - (Int64)Math.Round((rasioSislokAtm[0].d20 * prediksiIsiAtm[0].d20))
                - sislokCrm[0].d20
                - sislokCdm[0].d20
                + prediksiIsiAtm[0].d20
                + isiCrm[0].d20;

            tempRekomendasiBon.tgl = tanggalOptiMin.AddDays(1);
            rekomendasiBon.Add(tempRekomendasiBon);

            //Hitung rekomendasiBon h+2 keatas
            for(int a = 0; a<saldoAwalIdeal.Count - 1;a++)
            {
                tempRekomendasiBon = new Denom();
                tempRekomendasiBon.d100 = saldoAwalIdeal[a + 1].d100                                 //Saldo akhir h+1 
                    - saldoAwalIdeal[a].d100                                                      //saldo awal hari h
                    - (Int64)Math.Round((rasioSislokAtm[0].d100 * prediksiIsiAtm[0].d100))
                    - sislokCrm[0].d100
                    - sislokCdm[0].d100
                    + prediksiIsiAtm[0].d100
                    + isiCrm[0].d100;
                tempRekomendasiBon.d50 = saldoAwalIdeal[a + 1].d50                                 //Saldo akhir h+1 
                    - saldoAwalIdeal[a].d50                                                      //saldo awal hari h
                    - (Int64)Math.Round((rasioSislokAtm[0].d50 * prediksiIsiAtm[0].d50))
                    - sislokCrm[0].d50
                    - sislokCdm[0].d50
                    + prediksiIsiAtm[0].d50
                    + isiCrm[0].d50;
                tempRekomendasiBon.d20 = saldoAwalIdeal[a + 1].d20                                 //Saldo akhir h+1 
                    - saldoAwalIdeal[a].d20                                                      //saldo awal hari h
                    - (Int64)Math.Round((rasioSislokAtm[0].d20 * prediksiIsiAtm[0].d20))
                    - sislokCrm[0].d20
                    - sislokCdm[0].d20
                    + prediksiIsiAtm[0].d20
                    + isiCrm[0].d20;
                tempRekomendasiBon.tgl = tanggalOptiMin.AddDays(2 + a);
                rekomendasiBon.Add(tempRekomendasiBon);

            }
            Console.WriteLine("Rekomendasi Bon");
            foreach(var temp in rekomendasiBon)
            {
                Console.WriteLine(temp.tgl + " " + temp.d100 + " " + temp.d50 + " " + temp.d20);
            }

            rekomendasiAdhoc100Lbl.Text = rekomendasiAdhoc.d100.ToString();
            rekomendasiAdhoc50Lbl.Text = rekomendasiAdhoc.d50.ToString();
            rekomendasiAdhoc20Lbl.Text = rekomendasiAdhoc.d20.ToString();
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
            for (int i = 1; i < 4; i++)
            {
                bonGridView.Columns[i].DefaultCellStyle.Format = "c";
                bonGridView.Columns[i].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            }
            DateTime tempDate = tanggalOptiMin.AddDays(1);
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
                    if (i == 1)
                    {
                        newDetailA.setor100 = setor.d100;
                        newDetailA.setor50 = setor.d50;
                        newDetailA.setor20 = setor.d20;
                    }
                    //Bon
                    newDetailA.bon100 = bon[i].d100;
                    newDetailA.bon50 = bon[i].d50;
                    newDetailA.bon20 = bon[i].d20;
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

                    if(count==1)
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

        private Denom loadBonAdhocFromTxt()
        {
            Denom bonAdhoc = new Denom();
            //Bon Adhoc
            if (bonAdhoc100Txt.Text == "")
                bonAdhoc.d100 = 0;
            else
                bonAdhoc.d100 = Int64.Parse(bonAdhoc100Txt.Text);

            if (bonAdhoc50Txt.Text == "")
                bonAdhoc.d50 = 0;
            else
                bonAdhoc.d50 = Int64.Parse(bonAdhoc50Txt.Text);

            if (bonAdhoc20Txt.Text == "")
                bonAdhoc.d20 = 0;
            else
                bonAdhoc.d20 = Int64.Parse(bonAdhoc20Txt.Text);

            return bonAdhoc;
        }
        private Denom loadSetorAdhocFromTxt()
        {
            Denom setorAdhoc = new Denom();
            //Setor Adhoc
            if (setorAdhoc100Txt.Text == "")
                setorAdhoc.d100 = 0;
            else
                setorAdhoc.d100 = Int64.Parse(setorAdhoc100Txt.Text);

            if (setorAdhoc50Txt.Text == "")
                setorAdhoc.d50 = 0;
            else
                setorAdhoc.d50 = Int64.Parse(setorAdhoc50Txt.Text);

            if (setorAdhoc20Txt.Text == "")
                setorAdhoc.d20 = 0;
            else
                setorAdhoc.d20 = Int64.Parse(setorAdhoc20Txt.Text);
            return setorAdhoc;
        }
        private Denom loadSetorFromTxt()
        {
            Denom setor = new Denom();
            //Setor
            if (setor100Txt.Text == "")
                setor.d100 = 0;
            else
                setor.d100 = Int64.Parse(setor100Txt.Text);
            if (setor50Txt.Text == "")
                setor.d50 = 0;
            else
                setor.d50 = Int64.Parse(setor50Txt.Text);
            if (setor20Txt.Text == "")
                setor.d20 = 0;
            else
                setor.d20 = Int64.Parse(setor20Txt.Text);
            return setor;
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
