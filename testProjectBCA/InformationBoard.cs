using System;
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
    public partial class InformationBoard : Form
    {
        List<String> KodePkt;
        List<Int64> prediksiIsiAtm100;
        List<Int64> prediksiIsiAtm50;
        List<Int64> prediksiIsiAtm20;
        int pktIndex;
        DateTime tanggalOpti; 
        /**
            Opti itu datanya pasti selalu H+1, H+2,...
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

                foreach (String temp2 in tempListKodePkt)
                {
                    var query = (from x in db.Optis
                                 where x.Cashpoint.kodePkt == temp2
                                 select x).ToList();
                    if (query.Count > 0)
                        KodePkt.Add(temp2);
                }
                pktComboBox.DataSource = KodePkt;
            }
            dataGridView1.Columns.Add("Tanggal", "Tanggal");
            dataGridView1.Columns.Add("100000", "100000");
            dataGridView1.Columns.Add("50000", "50000");
            dataGridView1.Columns.Add("20000", "20000");
            loadPrediksi();
            /*tanggalOpti = (DateTime) query[0].tanggal*/;
            //Console.WriteLine(query[0]);
            //MessageBox.Show(tanggalOpti.ToShortDateString());
        }

        private void pktComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            pktIndex = pktComboBox.SelectedIndex;
            loadPrediksi();
        }
        private void loadPrediksi()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            prediksiIsiAtm100 = new List<Int64>();
            prediksiIsiAtm50 = new List<Int64>();
            prediksiIsiAtm20 = new List<Int64>();
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

                            //Denom 100
                            cell = new DataGridViewTextBoxCell();
                            cmd.CommandText = "SELECT SUM(prediksi) " +
                                              "FROM Opti o " +
                                              "JOIN Cashpoint c on o.idCashpoint = c.idCashpoint " +
                                              "WHERE kodePkt = '" + KodePkt[pktIndex] + "' AND tanggal = '" + minDate.ToShortDateString() + "' AND denom = '100000' " +
                                              "GROUP BY kodePkt";
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            { cell.Value = reader[0].ToString(); prediksiIsiAtm100.Add((Int64)reader[0]); }
                            else
                            { cell.Value = 0; prediksiIsiAtm100.Add(0); }
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
                            { cell.Value = reader[0].ToString(); prediksiIsiAtm50.Add((Int64)reader[0]); }
                            else
                            { cell.Value = 0; prediksiIsiAtm50.Add(0); }
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
                            { cell.Value = reader[0].ToString(); prediksiIsiAtm20.Add((Int64)reader[0]); }
                            else
                            { cell.Value = 0; prediksiIsiAtm20.Add(0); }
                            row.Cells.Add(cell);
                            reader.Close();

                            minDate = minDate.AddDays(1);
                            Console.WriteLine(minDate);
                            dataGridView1.Rows.Add(row);
                        }
                    }
                    sqlConnection1.Close();
                }
            }
            // Data is accessible through the DataReader object here.

           
        }
    }
}
