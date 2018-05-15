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
    public partial class RekapApprovalForm : Form
    {
        Database1Entities db = new Database1Entities();
        List<RekapApprovalData> listRekapApproval = new List<RekapApprovalData>();
        public RekapApprovalForm()
        {
            InitializeComponent();
            loadGridView();
        }
        void loadGridView()
        {
            DateTime tanggal = TanggalPicker.Value.Date;
            String query = "SELECT [Tanggal Proses] = A.tanggal, [Tanggal Order] = DA.tanggal, [KodePkt] = A.kodePkt, [Bon 100] = bon100, [Bon 50] = bon50, [Bon 20] = bon20, ISNULL(inputOpr,''), ISNULL(inputSpv,''), ISNULL(inputNoTxn,''), ISNULL(validasiOpr,''), ISNULL(validasiSpv,''), ISNULL(validasiNoTxn,''), A.idApproval"
                    + " FROM Approvals A JOIN DetailApprovals DA ON A.idApproval = DA.idApproval"
                    +" WHERE A.tanggal = '" +tanggal.ToShortDateString()+"' AND DA.tanggal > '"+tanggal+"'";
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                SqlCommand cmd = new SqlCommand(query,sql);
                sql.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                listRekapApproval = new List<RekapApprovalData>();

                while(reader.Read())
                {
                    listRekapApproval.Add(new RekapApprovalData()
                    {
                        tanggal = (DateTime) reader[0],
                        tanggalOrder = (DateTime) reader[1],
                        kodePkt = (String) reader[2],
                        bon100 = (Int64) reader[3],
                        bon50 = (Int64) reader[4],
                        bon20 = (Int64) reader[5],
                        inputOpr = (String) reader[6],
                        inputSpv = (String) reader[7],
                        inputNoTxn = (String) reader[8],
                        validasiSpv = (String)reader[9],
                        validasiOpr = (String) reader[10],
                        validasiNoTxn = (String) reader[11],
                        order = "Bon CIT",
                        orderType = "Reguler",
                        id = (int) reader[12]
                    });
                }
                reader.Close();

                query = "SELECT [Tanggal Proses] = A.tanggal, [Tanggal Order] = DA.tanggal, [KodePkt] = A.kodePkt, [Bon 100] = ISNULL(adhoc100,0), [Bon 50] = ISNULL(adhoc50,0), [Bon 20] = ISNULL(adhoc20,0), ISNULL(inputOpr,''), ISNULL(inputSpv,''), ISNULL(inputNoTxn,''), ISNULL(validasiOpr,''), ISNULL(validasiSpv,''), ISNULL(validasiNoTxn,''), A.idApproval"
                    + " FROM Approvals A JOIN DetailApprovals DA ON A.idApproval = DA.idApproval"
                    + " WHERE A.tanggal = '" + tanggal.ToShortDateString() + "' AND (ISNULL(adhoc100,0)!= 0 OR ISNULL(adhoc50,0)!= 0 OR ISNULL(adhoc20,0)!= 0)";
                cmd.CommandText = query;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    listRekapApproval.Add(new RekapApprovalData()
                    {
                        tanggal = (DateTime)reader[0],
                        tanggalOrder = (DateTime)reader[1],
                        kodePkt = (String)reader[2],
                        bon100 = (Int64)reader[3],
                        bon50 = (Int64)reader[4],
                        bon20 = (Int64)reader[5],
                        inputOpr = (String)reader[6],
                        inputSpv = (String)reader[7],
                        inputNoTxn = (String)reader[8],
                        validasiSpv = (String)reader[9],
                        validasiOpr = (String)reader[10],
                        validasiNoTxn = (String)reader[11],
                        order = "Bon CIT",
                        orderType = "Adhoc",
                        id = (int)reader[12]
                    });
                }
                reader.Close();
                query = "SELECT [Tanggal Proses] = A.tanggal, [Tanggal Order] = DA.tanggal, [KodePkt] = A.kodePkt, [Setor 100] = ISNULL(setor100,0), [Setor 50] = ISNULL(setor50,0), [Setor 20] = ISNULL(setor20,0), ISNULL(inputOpr,''), ISNULL(inputSpv,''), ISNULL(inputNoTxn,''), ISNULL(validasiOpr,''), ISNULL(validasiSpv,''), ISNULL(validasiNoTxn,''), A.idApproval"
                    + " FROM Approvals A JOIN DetailApprovals DA ON A.idApproval = DA.idApproval"
                    + " WHERE A.tanggal = '" + tanggal.ToShortDateString() + "' AND (ISNULL(setor100,0) != 0 OR ISNULL(setor50,0) != 0 OR ISNULL(setor20,0) != 0)  AND DA.tanggal > '" + tanggal + "'";
                cmd.CommandText = query;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    listRekapApproval.Add(new RekapApprovalData()
                    {
                        tanggal = (DateTime)reader[0],
                        tanggalOrder = (DateTime)reader[1],
                        kodePkt = (String)reader[2],
                        bon100 = (Int64)reader[3],
                        bon50 = (Int64)reader[4],
                        bon20 = (Int64)reader[5],
                        inputOpr = (String)reader[6],
                        inputSpv = (String)reader[7],
                        inputNoTxn = (String)reader[8],
                        validasiSpv = (String)reader[9],
                        validasiOpr = (String)reader[10],
                        validasiNoTxn = (String)reader[11],
                        order = "Setor",
                        orderType = "Reguler",
                        id = (int)reader[12]
                    });
                }
                reader.Close();
                query = "SELECT [Tanggal Proses] = A.tanggal, [Tanggal Order] = DA.tanggal, [KodePkt] = A.kodePkt, [Setor 100] = ISNULL(setorAdhoc100,0), [Setor 50] = ISNULL(setorAdhoc50,0), [Setor 20] = ISNULL(setorAdhoc20,0), ISNULL(inputOpr,''), ISNULL(inputSpv,''), ISNULL(inputNoTxn,''), ISNULL(validasiOpr,''), ISNULL(validasiSpv,''), ISNULL(validasiNoTxn,''), A.idApproval"
                    + " FROM Approvals A JOIN DetailApprovals DA ON A.idApproval = DA.idApproval"
                    + " WHERE A.tanggal = '" + tanggal.ToShortDateString() + "' AND (ISNULL(setorAdhoc100,0) != 0 OR ISNULL(setorAdhoc50,0)!= 0 OR ISNULL(setorAdhoc20,0) != 0)";
                cmd.CommandText = query;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    listRekapApproval.Add(new RekapApprovalData()
                    {
                        tanggal = (DateTime)reader[0],
                        tanggalOrder = (DateTime)reader[1],
                        kodePkt = (String)reader[2],
                        bon100 = (Int64)reader[3],
                        bon50 = (Int64)reader[4],
                        bon20 = (Int64)reader[5],
                        inputOpr = (String)reader[6],
                        inputSpv = (String)reader[7],
                        inputNoTxn = (String)reader[8],
                        validasiSpv = (String)reader[9],
                        validasiOpr = (String)reader[10],
                        validasiNoTxn = (String)reader[11],
                        order = "Setor",
                        orderType = "Adhoc",
                        id = (int)reader[12]
                    });
                }
                reader.Close();

                InputGridView.DataSource = listRekapApproval;
            }
            InputGridView.Columns[0].ReadOnly = true;
            InputGridView.Columns[1].ReadOnly = true;
            InputGridView.Columns[2].ReadOnly = true;
            //InputGridView.Columns[3].ReadOnly = true;
            //InputGridView.Columns[4].ReadOnly = true;
            //InputGridView.Columns[5].ReadOnly = true;

            InputGridView.Columns[0].Width = 1;
            InputGridView.Columns[1].Width = 70;
            InputGridView.Columns[2].Width = 70;
            InputGridView.Columns[3].Width = 70;
            InputGridView.Columns[4].Width = 70;
            InputGridView.Columns[5].Width = 70;

            for (int a = 0; a < 3; a++)
            {
                InputGridView.Columns[6 + a].DefaultCellStyle.Format = "C";
                InputGridView.Columns[6 + a].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            }
        }

        private void TanggalPicker_ValueChanged(object sender, EventArgs e)
        {
            loadGridView();
        }

        private void InputButton_Click(object sender, EventArgs e)
        {
            foreach(var temp in listRekapApproval)
            {
                MessageBox.Show(temp.inputOpr);
                Approval ap = (from x in db.Approvals where x.idApproval == temp.id select x).FirstOrDefault();
                ap.inputOpr = temp.inputOpr;
                ap.inputSpv = temp.inputSpv;
                ap.inputNoTxn = temp.inputNoTxn;
                ap.validasiOpr = temp.validasiOpr;
                ap.validasiSpv = temp.validasiSpv;
                ap.validasiNoTxn = temp.validasiNoTxn;
                db.SaveChanges();
            }
        }

        private void InputGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            int col = e.ColumnIndex;
            for (int a = 0; a < listRekapApproval.Count; a++)
            {
                if (listRekapApproval[row].id == listRekapApproval[a].id)
                {
                    listRekapApproval[a].inputOpr = listRekapApproval[row].inputOpr;
                    listRekapApproval[a].inputSpv = listRekapApproval[row].inputSpv;
                    listRekapApproval[a].inputNoTxn = listRekapApproval[row].inputNoTxn;
                    listRekapApproval[a].validasiOpr = listRekapApproval[row].validasiOpr;
                    listRekapApproval[a].validasiSpv = listRekapApproval[row].validasiSpv;
                    listRekapApproval[a].validasiNoTxn = listRekapApproval[row].validasiNoTxn;
                }
            }
            InputGridView.Refresh();
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sv = new SaveFileDialog();
            sv.Filter = Variables.csvFilter;
            if (sv.ShowDialog() == DialogResult.OK)
            {
                var test = (from x in listRekapApproval
                            join y in db.Pkts on x.kodePkt equals y.kodePkt
                            select new { tanggalProses = x.tanggal, tanggalOrder = x.tanggalOrder, namaPkt = y.namaPkt, kodePkt = x.kodePkt, order = x.order, orderType = x.orderType, kodeTxn = x.inputNoTxn, bon100 = x.bon100, bon50 = x.bon50, bon20 = x.bon20 }
                            ).ToList();
                String csv = ServiceStack.Text.CsvSerializer.SerializeToString(test);
                File.WriteAllText(sv.FileName, csv);
            }
        }

        void SaveDataGridViewToCSV(string filename)
        {
            
            // Copy selected cells to DataObject
            
            // Get the text of the DataObject, and serialize it to a file
            
        }
    }
    class RekapApprovalData
    {
        public int id { set; get; }
        public DateTime tanggal { set; get; }
        public DateTime tanggalOrder { set; get; }
        public String order { set; get; }
        public String orderType { set; get; }

        public String kodePkt { set; get; }
        public Int64 bon100 { set; get; }
        public Int64 bon50 { set; get; }
        public Int64 bon20 { set; get; }
        public String inputOpr { set; get; }
        public String inputSpv { set; get; }
        public String inputNoTxn { set; get; }
        public String validasiOpr { set; get; }
        public String validasiSpv { set; get; }
        public String validasiNoTxn { set; get; }
    }
}

