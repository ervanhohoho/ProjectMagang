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
            loadKanwilCheckListBox();
            kanwilCheckListBox.CheckOnClick = true;
        }
        void loadKanwilCheckListBox()
        {
            List<String> listKanwil = (from x in db.Pkts orderby x.kanwil select x.kanwil).Distinct().ToList();
            foreach(String temp in listKanwil)
                kanwilCheckListBox.Items.Add(temp);

        }
        void loadGridView()
        {
            List<String> kanwilYangDibaca = new List<String>();
            var checkeditems = kanwilCheckListBox.CheckedItems;
            foreach(var temp in checkeditems)
            {
                Console.WriteLine(temp.ToString());
                kanwilYangDibaca.Add(temp.ToString());
            }
            DateTime tanggal = TanggalPicker.Value.Date;
            DateTime tanggalMin = tanggalMinPicker.Value.Date;
            String query = "SELECT [Tanggal Proses] = A.tanggal, [Tanggal Order] = DA.tanggal, [KodePkt] = A.kodePkt, [Bon 100] = bon100, [Bon 50] = bon50, [Bon 20] = bon20, ISNULL(inputOpr,''), ISNULL(inputSpv,''), ISNULL(inputNoTxn,''), ISNULL(validasiOpr,''), ISNULL(validasiSpv,''), ISNULL(validasiNoTxn,''), A.idApproval, DA.idDetailApproval, kanwil, koordinator"
                    + " FROM Approvals A JOIN DetailApprovals DA ON A.idApproval = DA.idApproval JOIN Pkt P ON A.kodePkt = P.kodePkt"
                    + " WHERE A.tanggal = '" +tanggal.ToShortDateString()+"' AND DA.tanggal >= '"+tanggalMin+"' AND bon100 != -1";
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
                        d100 = (Int64) reader[3],
                        d50 = (Int64) reader[4],
                        d20 = (Int64) reader[5],
                        inputOpr = (String) reader[6],
                        inputSpv = (String) reader[7],
                        inputNoTxn = (String) reader[8],
                        validasiSpv = (String)reader[9],
                        validasiOpr = (String) reader[10],
                        validasiNoTxn = (String) reader[11],
                        order = "Bon CIT",
                        orderType = "Reguler",
                        id = (int) reader[12],
                        idDetailApproval = (int)reader[13],
                        kanwil = reader[14].ToString(),
                        koordinator = reader[15].ToString()
                    });
                }
                reader.Close();

                query = "SELECT [Tanggal Proses] = A.tanggal, [Tanggal Order] = DA.tanggal, [KodePkt] = A.kodePkt, [Bon 100] = ISNULL(adhoc100,0), [Bon 50] = ISNULL(adhoc50,0), [Bon 20] = ISNULL(adhoc20,0), ISNULL(inputOpr,''), ISNULL(inputSpv,''), ISNULL(inputNoTxn,''), ISNULL(validasiOpr,''), ISNULL(validasiSpv,''), ISNULL(validasiNoTxn,''), A.idApproval, DA.idDetailApproval, kanwil, koordinator"
                    + " FROM Approvals A JOIN DetailApprovals DA ON A.idApproval = DA.idApproval JOIN Pkt P ON A.kodePkt = P.kodePkt"
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
                        d100 = (Int64)reader[3],
                        d50 = (Int64)reader[4],
                        d20 = (Int64)reader[5],
                        inputOpr = (String)reader[6],
                        inputSpv = (String)reader[7],
                        inputNoTxn = (String)reader[8],
                        validasiSpv = (String)reader[9],
                        validasiOpr = (String)reader[10],
                        validasiNoTxn = (String)reader[11],
                        order = "Bon CIT",
                        orderType = "Adhoc",
                        id = (int)reader[12],
                        idDetailApproval = (int)reader[13],
                        kanwil = reader[14].ToString(),
                        koordinator = reader[15].ToString()
                    });
                }
                reader.Close();
                query = "SELECT [Tanggal Proses] = A.tanggal, [Tanggal Order] = DA.tanggal, [KodePkt] = A.kodePkt, [Setor 100] = ISNULL(setor100,0), [Setor 50] = ISNULL(setor50,0), [Setor 20] = ISNULL(setor20,0), ISNULL(inputOpr,''), ISNULL(inputSpv,''), ISNULL(inputNoTxn,''), ISNULL(validasiOpr,''), ISNULL(validasiSpv,''), ISNULL(validasiNoTxn,''), A.idApproval, DA.idDetailApproval, kanwil, koordinator"
                    + " FROM Approvals A JOIN DetailApprovals DA ON A.idApproval = DA.idApproval JOIN Pkt P ON A.kodePkt = P.kodePkt"
                    + " WHERE A.tanggal = '" + tanggal.ToShortDateString() + "' AND (ISNULL(setor100,0) != 0 OR ISNULL(setor50,0) != 0 OR ISNULL(setor20,0) != 0)  AND DA.tanggal >= '" + tanggalMin + "'";
                cmd.CommandText = query;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    listRekapApproval.Add(new RekapApprovalData()
                    {
                        tanggal = (DateTime)reader[0],
                        tanggalOrder = (DateTime)reader[1],
                        kodePkt = (String)reader[2],
                        d100 = (Int64)reader[3],
                        d50 = (Int64)reader[4],
                        d20 = (Int64)reader[5],
                        inputOpr = (String)reader[6],
                        inputSpv = (String)reader[7],
                        inputNoTxn = (String)reader[8],
                        validasiSpv = (String)reader[9],
                        validasiOpr = (String)reader[10],
                        validasiNoTxn = (String)reader[11],
                        order = "Setor",
                        orderType = "Reguler",
                        id = (int)reader[12],
                        idDetailApproval = (int)reader[13],
                        kanwil = reader[14].ToString(),
                        koordinator = reader[15].ToString()
                    });
                }
                reader.Close();
                query = "SELECT [Tanggal Proses] = A.tanggal, [Tanggal Order] = DA.tanggal, [KodePkt] = A.kodePkt, [Setor 100] = ISNULL(setorAdhoc100,0), [Setor 50] = ISNULL(setorAdhoc50,0), [Setor 20] = ISNULL(setorAdhoc20,0), ISNULL(inputOpr,''), ISNULL(inputSpv,''), ISNULL(inputNoTxn,''), ISNULL(validasiOpr,''), ISNULL(validasiSpv,''), ISNULL(validasiNoTxn,''), A.idApproval, DA.idDetailApproval, kanwil, koordinator"
                    + " FROM Approvals A JOIN DetailApprovals DA ON A.idApproval = DA.idApproval JOIN Pkt P ON A.kodePkt = P.kodePkt"
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
                        d100 = (Int64)reader[3],
                        d50 = (Int64)reader[4],
                        d20 = (Int64)reader[5],
                        inputOpr = (String)reader[6],
                        inputSpv = (String)reader[7],
                        inputNoTxn = (String)reader[8],
                        validasiSpv = (String)reader[9],
                        validasiOpr = (String)reader[10],
                        validasiNoTxn = (String)reader[11],
                        order = "Setor",
                        orderType = "Adhoc",
                        id = (int)reader[12],
                        idDetailApproval = (int)reader[13],
                        kanwil = reader[14].ToString(),
                        koordinator = reader[15].ToString()
                    });
                }
                reader.Close();
                if (listRekapApproval.Any())
                {
                    DateTime excludeDate = listRekapApproval.Max(x => x.tanggal);
                    if (kanwilYangDibaca.Any())
                        InputGridView.DataSource = listRekapApproval.Where(x => kanwilYangDibaca.Where(y => y == x.kanwil).FirstOrDefault() != null && x.d100 != -1).ToList();
                    else
                        InputGridView.DataSource = listRekapApproval;
                }
                else
                {
                    if (kanwilYangDibaca.Any())
                        InputGridView.DataSource = listRekapApproval.Where(x => kanwilYangDibaca.Where(y => y == x.kanwil).FirstOrDefault() != null).ToList();
                    else
                        InputGridView.DataSource = listRekapApproval;
                }
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
            tanggalMinPicker.MinDate = TanggalPicker.Value;
            tanggalMinPicker.Value = TanggalPicker.Value;
        }

        private void InputButton_Click(object sender, EventArgs e)
        {
            loadForm.ShowSplashScreen();
            foreach(var temp in listRekapApproval)
            {
                Approval ap = (from x in db.Approvals where x.idApproval == temp.id select x).FirstOrDefault();
                ap.inputOpr = temp.inputOpr;
                ap.inputSpv = temp.inputSpv;
                ap.inputNoTxn = temp.inputNoTxn;
                ap.validasiOpr = temp.validasiOpr;
                ap.validasiSpv = temp.validasiSpv;
                ap.validasiNoTxn = temp.validasiNoTxn;


                DetailApproval da = (from x in db.DetailApprovals where x.idApproval == temp.id && x.idDetailApproval == temp.idDetailApproval select x).FirstOrDefault();
                if(temp.order.ToLower() == "setor")
                {
                    if(temp.orderType.ToLower() == "adhoc")
                    {
                        Int64 selisih100 = (Int64) da.setorAdhoc100 - temp.d100, 
                            selisih50 = (Int64)da.setorAdhoc50 - temp.d50, 
                            selisih20 = (Int64)da.setorAdhoc20 - temp.d20;
                        da.setorAdhoc100 = temp.d100;
                        da.setorAdhoc50 = temp.d50;
                        da.setorAdhoc20 = temp.d20;
                        DetailApproval dah1 = (from x in db.DetailApprovals where x.idApproval == temp.id && x.idDetailApproval == temp.idDetailApproval + 1 select x).FirstOrDefault();
                        int counter = 1;
                        while (dah1 != null)
                        {
                            dah1 = (from x in db.DetailApprovals where x.idApproval == temp.id && x.idDetailApproval == temp.idDetailApproval + counter select x).FirstOrDefault();
                            dah1.saldoAwal100 += selisih100;
                            dah1.saldoAwal50 += selisih50;
                            dah1.saldoAwal20 += selisih20;
                            counter++;
                        }
                    }
                    else
                    {
                        Int64 selisih100 = (Int64)da.setor100 - temp.d100,
                            selisih50 = (Int64)da.setor50 - temp.d50,
                            selisih20 = (Int64)da.setor20 - temp.d20;
                        da.setor100 = temp.d100;
                        da.setor50 = temp.d50;
                        da.setor20 = temp.d20;
                        DetailApproval dah1 = (from x in db.DetailApprovals where x.idApproval == temp.id && x.idDetailApproval == temp.idDetailApproval + 1 select x).FirstOrDefault();
                        int counter = 1;
                        while (dah1 != null)
                        {
                            dah1 = (from x in db.DetailApprovals where x.idApproval == temp.id && x.idDetailApproval == temp.idDetailApproval + counter select x).FirstOrDefault();
                            dah1.saldoAwal100 += selisih100;
                            dah1.saldoAwal50 += selisih50;
                            dah1.saldoAwal20 += selisih20;
                            counter++;
                        }
                    }
                }
                if(temp.order.ToLower() == "bon cit")
                {
                    if (temp.orderType.ToLower() == "adhoc")
                    {
                        Int64 selisih100 = temp.d100 - (Int64)da.adhoc100,
                           selisih50 = temp.d50 - (Int64)da.adhoc50,
                           selisih20 = temp.d20 - (Int64)da.adhoc20 ;

                        da.adhoc100 = temp.d100;
                        da.adhoc50 = temp.d50;
                        da.adhoc20 = temp.d20;

                        DetailApproval dah1 = (from x in db.DetailApprovals where x.idApproval == temp.id && x.idDetailApproval == temp.idDetailApproval + 1 select x).FirstOrDefault();
                        int counter = 1;
                        while (dah1 != null)
                        {
                            dah1 = (from x in db.DetailApprovals where x.idApproval == temp.id && x.idDetailApproval == temp.idDetailApproval + counter select x).FirstOrDefault();
                            dah1.saldoAwal100 += selisih100;
                            dah1.saldoAwal50 += selisih50;
                            dah1.saldoAwal20 += selisih20;
                            counter++;
                        }
                    }
                    else
                    {
                        Int64 selisih100 = temp.d100 - (Int64)da.bon100,
                           selisih50 = temp.d50 - (Int64)da.bon50,
                           selisih20 = temp.d20 - (Int64)da.bon20;
                        da.bon100 = temp.d100;
                        da.bon50 = temp.d50;
                        da.bon20 = temp.d20;
                        DetailApproval dah1 = (from x in db.DetailApprovals where x.idApproval == temp.id && x.idDetailApproval == temp.idDetailApproval + 1 select x).FirstOrDefault();
                        int counter = 1;
                        while (dah1 != null)
                        {
                            dah1 = (from x in db.DetailApprovals where x.idApproval == temp.id && x.idDetailApproval == temp.idDetailApproval + counter select x).FirstOrDefault();
                            dah1.saldoAwal100 += selisih100;
                            dah1.saldoAwal50 += selisih50;
                            dah1.saldoAwal20 += selisih20;
                            counter++;
                        }
                    }
                }

                db.SaveChanges();
            }
            loadForm.CloseForm();
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
                Console.WriteLine("Export Button Clicked");
                List<String> kanwilYangDilihat = new List<String>();
                var listKanwil = kanwilCheckListBox.CheckedItems;
                foreach (var temp in listKanwil)
                {
                    kanwilYangDilihat.Add(temp.ToString());
                    Console.WriteLine(temp.ToString());
                }
                

                var test = (from x in listRekapApproval
                            join y in db.Pkts on x.kodePkt equals y.kodePkt
                            select new { tanggalProses = x.tanggal, tanggalOrder = x.tanggalOrder, namaPkt = y.namaPkt, kodePkt = x.kodePkt, order = x.order, orderType = x.orderType, kodeTxn = x.inputNoTxn, d100 = x.d100, d50 = x.d50, d20 = x.d20, kanwil = x.kanwil, koordinator = x.koordinator }
                            ).ToList();
                if(kanwilYangDilihat.Any())
                {
                    test = test.Where(x => kanwilYangDilihat.Where(y => y == x.kanwil).FirstOrDefault() != null && x.d100 != -1).ToList();
                }
                String csv = ServiceStack.Text.CsvSerializer.SerializeToString(test);
                File.WriteAllText(sv.FileName, csv);
            }
        }

        void SaveDataGridViewToCSV(string filename)
        {
            
            // Copy selected cells to DataObject
            
            // Get the text of the DataObject, and serialize it to a file
            
        }

        private void kanwilCheckListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            loadGridView();
        }
    }
    class RekapApprovalData
    {
        public int id { set; get; }
        public int idDetailApproval { set; get; }
        public DateTime tanggal { set; get; }
        public DateTime tanggalOrder { set; get; }
        public String order { set; get; }
        public String orderType { set; get; }

        public String kodePkt { set; get; }
        public Int64 d100 { set; get; }
        public Int64 d50 { set; get; }
        public Int64 d20 { set; get; }
        public String inputOpr { set; get; }
        public String inputSpv { set; get; }
        public String inputNoTxn { set; get; }
        public String validasiOpr { set; get; }
        public String validasiSpv { set; get; }
        public String validasiNoTxn { set; get; }
        public String kanwil { set; get; }
        public String koordinator { set; get; }
    }
}

