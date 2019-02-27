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
        Dictionary<string, int> pairs = new Dictionary<string, int>();
        Database1Entities db = new Database1Entities();
        List<RekapApprovalData> listRekapApproval = new List<RekapApprovalData>();
        List<RekapApprovalData> dataLama = new List<RekapApprovalData>();
        List<Staff> staffs = new List<Staff>();
        public RekapApprovalForm()
        {
            InitializeComponent();
            pairs.Add("Bon CIT Reguler", 0);
            pairs.Add("Bon CIT Adhoc", 1);
            pairs.Add("Setor Reguler", 2);
            pairs.Add("Setor Adhoc", 3);
            //loadGridView();
            loadKanwilCheckListBox();
            InputGridView.AllowUserToAddRows = false;
            kanwilCheckListBox.CheckOnClick = true;
            Database1Entities db = new Database1Entities();
            staffs = db.Staffs.ToList();
            
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
            String query = "SELECT [Tanggal Proses] = A.tanggal, [Tanggal Order] = DA.tanggal, [KodePkt] = A.kodePkt, [Bon 100] = bon100, [Bon 50] = bon50, [Bon 20] = bon20, ISNULL(DA.inputOpr,''), ISNULL(DA.inputSpv,''), ISNULL(DA.inputNoTxn,''), ISNULL(DA.validasiOpr,''), ISNULL(DA.validasiSpv,''), ISNULL(DA.validasiNoTxn,''), A.idApproval, DA.idDetailApproval, kanwil, koordinator"
                    + " FROM Approvals A JOIN DetailApprovals DA ON A.idApproval = DA.idApproval JOIN Pkt P ON A.kodePkt = P.kodePkt"
                    + " WHERE A.tanggal = '" +tanggal.ToShortDateString()+"' AND DA.tanggal >= '"+tanggalMin+"' AND bon100 != -1";
            List<DataGridViewRow> rows = new List<DataGridViewRow>();

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                SqlCommand cmd = new SqlCommand(query,sql);
                sql.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                listRekapApproval = new List<RekapApprovalData>();

                int indexSplit = pairs["Bon CIT Reguler"];
                while(reader.Read())
                {

                    String inputOpr = reader[6].ToString().Contains(";") ? staffs.Where(x => x.staffId.ToString() == reader[6].ToString().Split(';')[indexSplit]).Select(x => x.staffName).FirstOrDefault() : reader[6].ToString(),
                        inputSpv = reader[7].ToString().Contains(";") ? staffs.Where(x => x.staffId.ToString() == reader[7].ToString().Split(';')[indexSplit]).Select(x => x.staffName).FirstOrDefault() : reader[7].ToString(),
                        validasiOpr = reader[9].ToString().Contains(";") ? staffs.Where(x => x.staffId.ToString() == reader[9].ToString().Split(';')[indexSplit]).Select(x => x.staffName).FirstOrDefault() : reader[9].ToString(),
                        validasiSpv = reader[10].ToString().Contains(";") ? staffs.Where(x => x.staffId.ToString() == reader[10].ToString().Split(';')[indexSplit]).Select(x => x.staffName).FirstOrDefault() : reader[10].ToString();
                    listRekapApproval.Add(new RekapApprovalData()
                    {
                        tanggal = (DateTime) reader[0],
                        tanggalOrder = (DateTime) reader[1],
                        kodePkt = (String) reader[2],
                        d100 = (Int64) reader[3],
                        d50 = (Int64) reader[4],
                        d20 = (Int64) reader[5],
                        inputOpr = String.IsNullOrWhiteSpace(inputOpr) ? "" : inputOpr,
                        inputSpv = String.IsNullOrWhiteSpace(inputSpv) ? "" : inputSpv,
                        inputNoTxn = reader[8].ToString().Contains(";") ? reader[8].ToString().Split(';')[indexSplit] : reader[8].ToString(),
                        validasiSpv = String.IsNullOrWhiteSpace(validasiOpr) ? "" : validasiOpr,
                        validasiOpr = String.IsNullOrWhiteSpace(validasiSpv) ? "" : validasiSpv,
                        validasiNoTxn = reader[11].ToString().Contains(";") ? reader[11].ToString().Split(';')[indexSplit] : reader[11].ToString(),
                        order = "Bon CIT",
                        orderType = "Reguler",
                        id = (int) reader[12],
                        idDetailApproval = (int)reader[13],
                        kanwil = reader[14].ToString(),
                        koordinator = reader[15].ToString()
                    });
                }
                reader.Close();
                indexSplit = pairs["Bon CIT Adhoc"];
                query = "SELECT [Tanggal Proses] = A.tanggal, [Tanggal Order] = DA.tanggal, [KodePkt] = A.kodePkt, [Bon 100] = ISNULL(adhoc100,0), [Bon 50] = ISNULL(adhoc50,0), [Bon 20] = ISNULL(adhoc20,0), ISNULL(DA.inputOpr,''), ISNULL(DA.inputSpv,''), ISNULL(DA.inputNoTxn,''), ISNULL(DA.validasiOpr,''), ISNULL(DA.validasiSpv,''), ISNULL(DA.validasiNoTxn,''), A.idApproval, DA.idDetailApproval, kanwil, koordinator"
                    + " FROM Approvals A JOIN DetailApprovals DA ON A.idApproval = DA.idApproval JOIN Pkt P ON A.kodePkt = P.kodePkt"
                    + " WHERE A.tanggal = '" + tanggal.ToShortDateString() + "' AND (ISNULL(adhoc100,0)!= 0 OR ISNULL(adhoc50,0)!= 0 OR ISNULL(adhoc20,0)!= 0)";
                cmd.CommandText = query;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    String inputOpr = reader[6].ToString().Contains(";") ? staffs.Where(x => x.staffId.ToString() == reader[6].ToString().Split(';')[indexSplit]).Select(x => x.staffName).FirstOrDefault() : reader[6].ToString(),
                       inputSpv = reader[7].ToString().Contains(";") ? staffs.Where(x => x.staffId.ToString() == reader[7].ToString().Split(';')[indexSplit]).Select(x => x.staffName).FirstOrDefault() : reader[7].ToString(),
                       validasiOpr = reader[9].ToString().Contains(";") ? staffs.Where(x => x.staffId.ToString() == reader[9].ToString().Split(';')[indexSplit]).Select(x => x.staffName).FirstOrDefault() : reader[9].ToString(),
                       validasiSpv = reader[10].ToString().Contains(";") ? staffs.Where(x => x.staffId.ToString() == reader[10].ToString().Split(';')[indexSplit]).Select(x => x.staffName).FirstOrDefault() : reader[10].ToString();
                    listRekapApproval.Add(new RekapApprovalData()
                    {
                        tanggal = (DateTime)reader[0],
                        tanggalOrder = (DateTime)reader[1],
                        kodePkt = (String)reader[2],
                        d100 = (Int64)reader[3],
                        d50 = (Int64)reader[4],
                        d20 = (Int64)reader[5],
                        inputOpr = String.IsNullOrWhiteSpace(inputOpr) ? "" : inputOpr,
                        inputSpv = String.IsNullOrWhiteSpace(inputSpv) ? "" : inputSpv,
                        inputNoTxn = reader[8].ToString().Contains(";") ? reader[8].ToString().Split(';')[indexSplit] : reader[8].ToString(),
                        validasiSpv = String.IsNullOrWhiteSpace(validasiOpr) ? "" : validasiOpr,
                        validasiOpr = String.IsNullOrWhiteSpace(validasiSpv) ? "" : validasiSpv,
                        validasiNoTxn = reader[11].ToString().Contains(";") ? reader[11].ToString().Split(';')[indexSplit] : reader[11].ToString(),
                        order = "Bon CIT",
                        orderType = "Adhoc",
                        id = (int)reader[12],
                        idDetailApproval = (int)reader[13],
                        kanwil = reader[14].ToString(),
                        koordinator = reader[15].ToString()
                    });
                }
                reader.Close();

                indexSplit = pairs["Setor Reguler"];
                query = "SELECT [Tanggal Proses] = A.tanggal, [Tanggal Order] = DA.tanggal, [KodePkt] = A.kodePkt, [Setor 100] = ISNULL(setor100,0), [Setor 50] = ISNULL(setor50,0), [Setor 20] = ISNULL(setor20,0), ISNULL(DA.inputOpr,''), ISNULL(DA.inputSpv,''), ISNULL(DA.inputNoTxn,''), ISNULL(DA.validasiOpr,''), ISNULL(DA.validasiSpv,''), ISNULL(DA.validasiNoTxn,''), A.idApproval, DA.idDetailApproval, kanwil, koordinator"
                    + " FROM Approvals A JOIN DetailApprovals DA ON A.idApproval = DA.idApproval JOIN Pkt P ON A.kodePkt = P.kodePkt"
                    + " WHERE A.tanggal = '" + tanggal.ToShortDateString() + "' AND (ISNULL(setor100,0) != 0 OR ISNULL(setor50,0) != 0 OR ISNULL(setor20,0) != 0)  AND DA.tanggal >= '" + tanggalMin + "'";
                cmd.CommandText = query;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    String inputOpr = reader[6].ToString().Contains(";") ? staffs.Where(x => x.staffId.ToString() == reader[6].ToString().Split(';')[indexSplit]).Select(x => x.staffName).FirstOrDefault() : reader[6].ToString(),
                       inputSpv = reader[7].ToString().Contains(";") ? staffs.Where(x => x.staffId.ToString() == reader[7].ToString().Split(';')[indexSplit]).Select(x => x.staffName).FirstOrDefault() : reader[7].ToString(),
                       validasiOpr = reader[9].ToString().Contains(";") ? staffs.Where(x => x.staffId.ToString() == reader[9].ToString().Split(';')[indexSplit]).Select(x => x.staffName).FirstOrDefault() : reader[9].ToString(),
                       validasiSpv = reader[10].ToString().Contains(";") ? staffs.Where(x => x.staffId.ToString() == reader[10].ToString().Split(';')[indexSplit]).Select(x => x.staffName).FirstOrDefault() : reader[10].ToString();
                    listRekapApproval.Add(new RekapApprovalData()
                    {
                        tanggal = (DateTime)reader[0],
                        tanggalOrder = (DateTime)reader[1],
                        kodePkt = (String)reader[2],
                        d100 = (Int64)reader[3],
                        d50 = (Int64)reader[4],
                        d20 = (Int64)reader[5],
                        inputOpr = String.IsNullOrWhiteSpace(inputOpr) ? "" : inputOpr,
                        inputSpv = String.IsNullOrWhiteSpace(inputSpv) ? "" : inputSpv,
                        inputNoTxn = reader[8].ToString().Contains(";") ? reader[8].ToString().Split(';')[indexSplit] : reader[8].ToString(),
                        validasiSpv = String.IsNullOrWhiteSpace(validasiOpr) ? "" : validasiOpr,
                        validasiOpr = String.IsNullOrWhiteSpace(validasiSpv) ? "" : validasiSpv,
                        validasiNoTxn = reader[11].ToString().Contains(";") ? reader[11].ToString().Split(';')[indexSplit] : reader[11].ToString(),
                        order = "Setor",
                        orderType = "Reguler",
                        id = (int)reader[12],
                        idDetailApproval = (int)reader[13],
                        kanwil = reader[14].ToString(),
                        koordinator = reader[15].ToString()
                    });
                }
                reader.Close();
                indexSplit = pairs["Setor Adhoc"];
                query = "SELECT [Tanggal Proses] = A.tanggal, [Tanggal Order] = DA.tanggal, [KodePkt] = A.kodePkt, [Setor 100] = ISNULL(setorAdhoc100,0), [Setor 50] = ISNULL(setorAdhoc50,0), [Setor 20] = ISNULL(setorAdhoc20,0), ISNULL(DA.inputOpr,''), ISNULL(DA.inputSpv,''), ISNULL(DA.inputNoTxn,''), ISNULL(DA.validasiOpr,''), ISNULL(DA.validasiSpv,''), ISNULL(DA.validasiNoTxn,''), A.idApproval, DA.idDetailApproval, kanwil, koordinator"
                    + " FROM Approvals A JOIN DetailApprovals DA ON A.idApproval = DA.idApproval JOIN Pkt P ON A.kodePkt = P.kodePkt"
                    + " WHERE A.tanggal = '" + tanggal.ToShortDateString() + "' AND (ISNULL(setorAdhoc100,0) != 0 OR ISNULL(setorAdhoc50,0)!= 0 OR ISNULL(setorAdhoc20,0) != 0)";
                cmd.CommandText = query;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    String inputOpr = reader[6].ToString().Contains(";") ? staffs.Where(x => x.staffId.ToString() == reader[6].ToString().Split(';')[indexSplit]).Select(x => x.staffName).FirstOrDefault() : reader[6].ToString(),
                       inputSpv = reader[7].ToString().Contains(";") ? staffs.Where(x => x.staffId.ToString() == reader[7].ToString().Split(';')[indexSplit]).Select(x => x.staffName).FirstOrDefault() : reader[7].ToString(),
                       validasiOpr = reader[9].ToString().Contains(";") ? staffs.Where(x => x.staffId.ToString() == reader[9].ToString().Split(';')[indexSplit]).Select(x => x.staffName).FirstOrDefault() : reader[9].ToString(),
                       validasiSpv = reader[10].ToString().Contains(";") ? staffs.Where(x => x.staffId.ToString() == reader[10].ToString().Split(';')[indexSplit]).Select(x => x.staffName).FirstOrDefault() : reader[10].ToString();
                    listRekapApproval.Add(new RekapApprovalData()
                    {
                        tanggal = (DateTime)reader[0],
                        tanggalOrder = (DateTime)reader[1],
                        kodePkt = (String)reader[2],
                        d100 = (Int64)reader[3],
                        d50 = (Int64)reader[4],
                        d20 = (Int64)reader[5],
                        inputOpr = String.IsNullOrWhiteSpace(inputOpr) ? "" : inputOpr,
                        inputSpv = String.IsNullOrWhiteSpace(inputSpv) ? "" : inputSpv,
                        inputNoTxn = reader[8].ToString().Contains(";") ? reader[8].ToString().Split(';')[indexSplit] : reader[8].ToString(),
                        validasiSpv = String.IsNullOrWhiteSpace(validasiOpr) ? "" : validasiOpr,
                        validasiOpr = String.IsNullOrWhiteSpace(validasiSpv) ? "" : validasiSpv,
                        validasiNoTxn = reader[11].ToString().Contains(";") ? reader[11].ToString().Split(';')[indexSplit] : reader[11].ToString(),
                        order = "Setor",
                        orderType = "Adhoc",
                        id = (int)reader[12],
                        idDetailApproval = (int)reader[13],
                        kanwil = reader[14].ToString(),
                        koordinator = reader[15].ToString()
                    });
                }
                reader.Close();

                

                initGridView();

                if (listRekapApproval.Any())
                {
                    Console.WriteLine("Load Data!");
                    DateTime excludeDate = listRekapApproval.Max(x => x.tanggal);
                    if (kanwilYangDibaca.Any())
                    {
                        listRekapApproval = listRekapApproval.Where(x => kanwilYangDibaca.Where(y => y == x.kanwil).FirstOrDefault() != null && x.d100 != -1).ToList();
                        foreach (var temp in listRekapApproval)
                        {
                            rows.Add(new DataGridViewRow());
                            rows[rows.Count - 1].CreateCells(InputGridView, 
                                temp.id, 
                                temp.idDetailApproval, 
                                temp.tanggal, 
                                temp.tanggalOrder, 
                                temp.order,
                                temp.orderType,
                                temp.kodePkt,
                                temp.d100,
                                temp.d50,
                                temp.d20,
                                temp.inputOpr,
                                temp.inputSpv,
                                temp.inputNoTxn,
                                temp.validasiOpr,
                                temp.validasiSpv,
                                temp.validasiNoTxn,
                                temp.kanwil,
                                temp.koordinator);
                            //dataTambahanGridView.Rows.Add(row);
                        }
                    }
                    else
                    {
                        foreach (var temp in listRekapApproval)
                        {
                            rows.Add(new DataGridViewRow());
                            rows[rows.Count - 1].CreateCells(InputGridView,
                                temp.id,
                                temp.idDetailApproval,
                                temp.tanggal,
                                temp.tanggalOrder,
                                temp.order,
                                temp.orderType,
                                temp.kodePkt,
                                temp.d100,
                                temp.d50,
                                temp.d20,
                                temp.inputOpr,
                                temp.inputSpv,
                                temp.inputNoTxn,
                                temp.validasiOpr,
                                temp.validasiSpv,
                                temp.validasiNoTxn,
                                temp.kanwil,
                                temp.koordinator);
                            //dataTambahanGridView.Rows.Add(row);
                        }
                    }
                }
                else
                {
                    //if (kanwilYangDibaca.Any())
                    //    InputGridView.DataSource = listRekapApproval.Where(x => kanwilYangDibaca.Where(y => y == x.kanwil).FirstOrDefault() != null).ToList();
                    //else
                    //    InputGridView.DataSource = listRekapApproval;
                }
            }
            InputGridView.Rows.AddRange(rows.ToArray());
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
        void initGridView()
        {
            InputGridView.Columns.Clear();
            InputGridView.Rows.Clear();
            List<String> namaStafs = db.Staffs.Select(x => x.staffName).ToList();
            namaStafs = namaStafs.OrderBy(x => x).ToList();
            DataGridViewTextBoxColumn id = new DataGridViewTextBoxColumn()
            {
                ValueType = typeof(String),
                HeaderText = "id",
                Name = "id",
            };
            DataGridViewTextBoxColumn idDetailApproval = new DataGridViewTextBoxColumn()
            {
                ValueType = typeof(String),
                HeaderText = "idDetailApproval",
                Name = "idDetailApproval",
            };
            DataGridViewTextBoxColumn tanggal = new DataGridViewTextBoxColumn()
            {
                ValueType = typeof(DateTime),
                HeaderText = "tanggal",
                Name = "tanggal"
            };
            DataGridViewTextBoxColumn tanggalOrder = new DataGridViewTextBoxColumn()
            {
                ValueType = typeof(DateTime),
                HeaderText = "tanggalOrder",
                Name = "tanggalOrder"
            };
            DataGridViewTextBoxColumn order = new DataGridViewTextBoxColumn()
            {
                ValueType = typeof(String),
                HeaderText = "order",
                Name = "order"
            };
            DataGridViewTextBoxColumn orderType = new DataGridViewTextBoxColumn()
            {
                ValueType = typeof(String),
                HeaderText = "orderType",
                Name = "orderType"
            };
            DataGridViewTextBoxColumn kodePkt = new DataGridViewTextBoxColumn()
            {
                ValueType = typeof(String),
                HeaderText = "kodePkt",
                Name = "kodePkt"
            };
            DataGridViewTextBoxColumn d100 = new DataGridViewTextBoxColumn()
            {
                ValueType = typeof(Int64),
                HeaderText = "d100",
                Name = "d100"
            };
            DataGridViewTextBoxColumn d50 = new DataGridViewTextBoxColumn()
            {
                ValueType = typeof(Int64),
                HeaderText = "d50",
                Name = "d50"
            };
            DataGridViewTextBoxColumn d20 = new DataGridViewTextBoxColumn()
            {
                ValueType = typeof(Int64),
                HeaderText = "d20",
                Name = "d20"
            };
            DataGridViewComboBoxColumn inputOpr = new DataGridViewComboBoxColumn()
            {
                DataSource = namaStafs,
                Name = "inputOpr",
                HeaderText = "inputOpr",
            };
            DataGridViewComboBoxColumn inputSpv = new DataGridViewComboBoxColumn()
            {
                DataSource = namaStafs,
                Name = "inputSpv",
                HeaderText = "inputSpv"
            };
            DataGridViewTextBoxColumn inputNoTxn = new DataGridViewTextBoxColumn()
            {
                ValueType = typeof(String),
                HeaderText = "inputNoTxn",
                Name = "inputNoTxn"
            };
            DataGridViewComboBoxColumn validasiOpr = new DataGridViewComboBoxColumn()
            {
                DataSource = namaStafs,
                Name = "validasiOpr",
                HeaderText = "validasiOpr"
            };
            DataGridViewComboBoxColumn validasiSpv = new DataGridViewComboBoxColumn()
            {
                DataSource = namaStafs,
                Name = "validasiSpv",
                HeaderText = "validasiSpv"
            };
            DataGridViewTextBoxColumn validasiNoTxn = new DataGridViewTextBoxColumn()
            {
                ValueType = typeof(String),
                HeaderText = "validasiNoTxn",
                Name = "validasiNoTxn"
            };
            DataGridViewTextBoxColumn kanwil = new DataGridViewTextBoxColumn()
            {
                ValueType = typeof(String),
                HeaderText = "kanwil",
                Name = "kanwil"
            };
            DataGridViewTextBoxColumn koordinator = new DataGridViewTextBoxColumn()
            {
                ValueType = typeof(String),
                HeaderText = "koordinator",
                Name = "koordinator"
            };
            InputGridView.Columns.Add(id);
            InputGridView.Columns.Add(idDetailApproval);
            InputGridView.Columns.Add(tanggal);
            InputGridView.Columns.Add(tanggalOrder);
            InputGridView.Columns.Add(order);
            InputGridView.Columns.Add(orderType);
            InputGridView.Columns.Add(kodePkt);
            InputGridView.Columns.Add(d100);
            InputGridView.Columns.Add(d50);
            InputGridView.Columns.Add(d20);
            InputGridView.Columns.Add(inputOpr);
            InputGridView.Columns.Add(inputSpv);
            InputGridView.Columns.Add(inputNoTxn);
            InputGridView.Columns.Add(validasiOpr);
            InputGridView.Columns.Add(validasiSpv);
            InputGridView.Columns.Add(validasiNoTxn);
            InputGridView.Columns.Add(kanwil);
            InputGridView.Columns.Add(koordinator);
        }
        private void TanggalPicker_ValueChanged(object sender, EventArgs e)
        {
            tanggalMinPicker.MinDate = TanggalPicker.Value;
            tanggalMinPicker.Value = TanggalPicker.Value;
        }

        private void InputButton_Click(object sender, EventArgs e)
        {
            bool passwdReq = false;
            foreach(var check in listRekapApproval)
            {
                if (dataLama.Where(x => x.idDetailApproval == check.idDetailApproval && x.d100 == check.d100 && x.d50 == check.d50 && x.d20 == check.d20 && x.order == check.order && x.orderType == check.orderType).FirstOrDefault() == null)
                {
                    Console.WriteLine("Password Required!");
                    passwdReq = true;
                    break;
                }
            }
            if(passwdReq)
            {
                InputPromptForm ipf = new InputPromptForm("Input Password", "Input");
                if (ipf.ShowDialog() == DialogResult.OK)
                {
                    if (!BCrypt.Net.BCrypt.Verify(ipf.value, db.Passwords.Select(x => x.password1).FirstOrDefault()))
                    {
                        MessageBox.Show("Password Salah");
                    }
                    else
                    {
                        loadForm.ShowSplashScreen();
                        foreach (var temp in listRekapApproval)
                        {
                            db = new Database1Entities();
                            Approval ap = (from x in db.Approvals where x.idApproval == temp.id select x).FirstOrDefault();
                            //ap.inputOpr = temp.inputOpr;
                            //ap.inputSpv = temp.inputSpv;
                            //ap.inputNoTxn = temp.inputNoTxn;
                            //ap.validasiOpr = temp.validasiOpr;
                            //ap.validasiSpv = temp.validasiSpv;
                            //ap.validasiNoTxn = temp.validasiNoTxn;


                            DetailApproval da = (from x in db.DetailApprovals where x.idApproval == temp.id && x.idDetailApproval == temp.idDetailApproval select x).FirstOrDefault();

                            if (String.IsNullOrWhiteSpace(da.inputOpr))
                                da.inputOpr = ";;;";
                            if (String.IsNullOrEmpty(da.inputSpv))
                                da.inputSpv = ";;;";
                            if (String.IsNullOrEmpty(da.inputNoTxn))
                                da.inputNoTxn = ";;;";
                            if (String.IsNullOrEmpty(da.validasiOpr))
                                da.validasiOpr = ";;;";
                            if (String.IsNullOrEmpty(da.validasiSpv))
                                da.validasiSpv = ";;;";
                            if (String.IsNullOrEmpty(da.validasiNoTxn))
                                da.validasiNoTxn = ";;;";


                            int indexSplit = pairs[temp.order + " " + temp.orderType];

                            Console.WriteLine("Index Split: " + indexSplit);
                            String[] tempSplitted = da.inputOpr.Split(';');
                            tempSplitted[indexSplit] = temp.inputOpr;
                            Console.WriteLine("tempSplitted Count: " + tempSplitted.Length);
                            da.inputOpr = String.Join(";", tempSplitted);

                            tempSplitted = da.inputSpv.Split(';');
                            tempSplitted[indexSplit] = temp.inputSpv;
                            da.inputSpv = String.Join(";", tempSplitted);

                            tempSplitted = da.inputNoTxn.Split(';');
                            tempSplitted[indexSplit] = temp.inputNoTxn;
                            da.inputNoTxn = String.Join(";", tempSplitted);

                            tempSplitted = da.validasiOpr.Split(';');
                            tempSplitted[indexSplit] = temp.validasiOpr;
                            da.validasiOpr = String.Join(";", tempSplitted);

                            tempSplitted = da.validasiSpv.Split(';');
                            tempSplitted[indexSplit] = temp.validasiSpv;
                            da.validasiSpv = String.Join(";", tempSplitted);

                            tempSplitted = da.validasiNoTxn.Split(';');
                            tempSplitted[indexSplit] = temp.validasiNoTxn;
                            da.validasiNoTxn = String.Join(";", tempSplitted);
                            if (temp.order.ToLower() == "setor")
                            {
                                if (temp.orderType.ToLower() == "adhoc")
                                {

                                    Int64 selisih100 = (Int64)da.setorAdhoc100 - temp.d100,
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
                                        if (dah1 != null)
                                        {
                                            dah1.saldoAwal100 += selisih100;
                                            dah1.saldoAwal50 += selisih50;
                                            dah1.saldoAwal20 += selisih20;
                                        }
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
                                        if (dah1 != null)
                                        {
                                            dah1.saldoAwal100 += selisih100;
                                            dah1.saldoAwal50 += selisih50;
                                            dah1.saldoAwal20 += selisih20;
                                        }
                                        counter++;
                                    }
                                }
                            }
                            if (temp.order.ToLower() == "bon cit")
                            {
                                if (temp.orderType.ToLower() == "adhoc")
                                {
                                    Int64 selisih100 = temp.d100 - (Int64)da.adhoc100,
                                        selisih50 = temp.d50 - (Int64)da.adhoc50,
                                        selisih20 = temp.d20 - (Int64)da.adhoc20;

                                    da.adhoc100 = temp.d100;
                                    da.adhoc50 = temp.d50;
                                    da.adhoc20 = temp.d20;

                                    DetailApproval dah1 = (from x in db.DetailApprovals where x.idApproval == temp.id && x.idDetailApproval == temp.idDetailApproval + 1 select x).FirstOrDefault();
                                    int counter = 1;
                                    while (dah1 != null)
                                    {
                                        dah1 = (from x in db.DetailApprovals where x.idApproval == temp.id && x.idDetailApproval == temp.idDetailApproval + counter select x).FirstOrDefault();
                                        if (dah1 != null)
                                        {
                                            dah1.saldoAwal100 += selisih100;
                                            dah1.saldoAwal50 += selisih50;
                                            dah1.saldoAwal20 += selisih20;
                                        }
                                        counter++;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Temp: " + temp);
                                    Console.WriteLine("Temp 100: " + temp.d100);
                                    Console.WriteLine("Temp 50: " + temp.d50);
                                    Console.WriteLine("Temp 20: " + temp.d20);
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
                                        if (dah1 != null)
                                        {
                                            dah1.saldoAwal100 += selisih100;
                                            dah1.saldoAwal50 += selisih50;
                                            dah1.saldoAwal20 += selisih20;
                                        }
                                        counter++;
                                    }
                                }
                            }
                            db.SaveChanges();
                        }
                    }
                    loadForm.CloseForm();

                }
            }
            else
            {
                loadForm.ShowSplashScreen();
                foreach (var temp in listRekapApproval)
                {
                    db = new Database1Entities();
                    Approval ap = (from x in db.Approvals where x.idApproval == temp.id select x).FirstOrDefault();
                    //ap.inputOpr = temp.inputOpr;
                    //ap.inputSpv = temp.inputSpv;
                    //ap.inputNoTxn = temp.inputNoTxn;
                    //ap.validasiOpr = temp.validasiOpr;
                    //ap.validasiSpv = temp.validasiSpv;
                    //ap.validasiNoTxn = temp.validasiNoTxn;


                    DetailApproval da = (from x in db.DetailApprovals where x.idApproval == temp.id && x.idDetailApproval == temp.idDetailApproval select x).FirstOrDefault();

                    if (String.IsNullOrWhiteSpace(da.inputOpr))
                        da.inputOpr = ";;;";
                    if (String.IsNullOrEmpty(da.inputSpv))
                        da.inputSpv = ";;;";
                    if (String.IsNullOrEmpty(da.inputNoTxn))
                        da.inputNoTxn = ";;;";
                    if (String.IsNullOrEmpty(da.validasiOpr))
                        da.validasiOpr = ";;;";
                    if (String.IsNullOrEmpty(da.validasiSpv))
                        da.validasiSpv = ";;;";
                    if (String.IsNullOrEmpty(da.validasiNoTxn))
                        da.validasiNoTxn = ";;;";


                    int indexSplit = pairs[temp.order + " " + temp.orderType];


                    int idInputOpr = staffs.Where(x => x.staffName == temp.inputOpr).Select(x => x.staffId).FirstOrDefault();
                    int idInputSpv = staffs.Where(x => x.staffName == temp.inputSpv).Select(x => x.staffId).FirstOrDefault();
                    int idValidasiOpr = staffs.Where(x => x.staffName == temp.validasiOpr).Select(x => x.staffId).FirstOrDefault();
                    int idValidasiSpv = staffs.Where(x => x.staffName == temp.validasiSpv).Select(x => x.staffId).FirstOrDefault();

                    String[] tempSplitted = da.inputOpr.Split(';');
                    tempSplitted[indexSplit] = idInputOpr.ToString();
                    da.inputOpr = String.Join(";", tempSplitted);

                    tempSplitted = da.inputSpv.Split(';');
                    tempSplitted[indexSplit] = idInputSpv.ToString();
                    da.inputSpv = String.Join(";", tempSplitted);

                    tempSplitted = da.inputNoTxn.Split(';');
                    tempSplitted[indexSplit] = temp.inputNoTxn;
                    da.inputNoTxn = String.Join(";", tempSplitted);

                    tempSplitted = da.validasiOpr.Split(';');
                    tempSplitted[indexSplit] = idValidasiOpr.ToString();
                    da.validasiOpr = String.Join(";", tempSplitted);

                    tempSplitted = da.validasiSpv.Split(';');
                    tempSplitted[indexSplit] = idValidasiSpv.ToString();
                    da.validasiSpv = String.Join(";", tempSplitted);

                    tempSplitted = da.validasiNoTxn.Split(';');
                    tempSplitted[indexSplit] = temp.validasiNoTxn;
                    da.validasiNoTxn = String.Join(";", tempSplitted);
                    if (temp.order.ToLower() == "setor")
                    {
                        if (temp.orderType.ToLower() == "adhoc")
                        {

                            Int64 selisih100 = (Int64)da.setorAdhoc100 - temp.d100,
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
                                if (dah1 != null)
                                {
                                    dah1.saldoAwal100 += selisih100;
                                    dah1.saldoAwal50 += selisih50;
                                    dah1.saldoAwal20 += selisih20;
                                }
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
                                if (dah1 != null)
                                {
                                    dah1.saldoAwal100 += selisih100;
                                    dah1.saldoAwal50 += selisih50;
                                    dah1.saldoAwal20 += selisih20;
                                }
                                counter++;
                            }
                        }
                    }
                    if (temp.order.ToLower() == "bon cit")
                    {
                        if (temp.orderType.ToLower() == "adhoc")
                        {
                            Int64 selisih100 = temp.d100 - (Int64)da.adhoc100,
                                selisih50 = temp.d50 - (Int64)da.adhoc50,
                                selisih20 = temp.d20 - (Int64)da.adhoc20;

                            da.adhoc100 = temp.d100;
                            da.adhoc50 = temp.d50;
                            da.adhoc20 = temp.d20;

                            DetailApproval dah1 = (from x in db.DetailApprovals where x.idApproval == temp.id && x.idDetailApproval == temp.idDetailApproval + 1 select x).FirstOrDefault();
                            int counter = 1;
                            while (dah1 != null)
                            {
                                dah1 = (from x in db.DetailApprovals where x.idApproval == temp.id && x.idDetailApproval == temp.idDetailApproval + counter select x).FirstOrDefault();
                                if (dah1 != null)
                                {
                                    dah1.saldoAwal100 += selisih100;
                                    dah1.saldoAwal50 += selisih50;
                                    dah1.saldoAwal20 += selisih20;
                                }
                                counter++;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Temp: " + temp);
                            Console.WriteLine("Temp 100: " + temp.d100);
                            Console.WriteLine("Temp 50: " + temp.d50);
                            Console.WriteLine("Temp 20: " + temp.d20);
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
                                if (dah1 != null)
                                {
                                    dah1.saldoAwal100 += selisih100;
                                    dah1.saldoAwal50 += selisih50;
                                    dah1.saldoAwal20 += selisih20;
                                }
                                counter++;
                            }
                        }
                    }
                    db.SaveChanges();
                }
                loadForm.CloseForm();
            }
        }

        private void InputGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            int col = e.ColumnIndex;
            for (int a = 0; a < InputGridView.Rows.Count; a++)
            {
                //if (InputGridView.Rows[a].Cells[0].Value.ToString() == listRekapApproval[a].id.ToString())
                //{
                //    if(String.IsNullOrWhiteSpace(InputGridView.Rows[a].Cells["inputOpr"].Value.ToString()))
                //        InputGridView.Rows[a].Cells["inputOpr"].Value = InputGridView.Rows[row].Cells["inputOpr"].Value.ToString();
                //    if (String.IsNullOrWhiteSpace(listRekapApproval[a].inputSpv))
                //        listRekapApproval[a].inputSpv = listRekapApproval[row].inputSpv;
                //    //listRekapApproval[a].inputNoTxn = listRekapApproval[row].inputNoTxn;
                //    if (String.IsNullOrWhiteSpace(listRekapApproval[a].validasiOpr))
                //        listRekapApproval[a].validasiOpr = listRekapApproval[row].validasiOpr;
                //    if (String.IsNullOrWhiteSpace(listRekapApproval[a].validasiSpv))
                //        listRekapApproval[a].validasiSpv = listRekapApproval[row].validasiSpv;
                //    //listRekapApproval[a].validasiNoTxn = listRekapApproval[row].validasiNoTxn;
                //}
            }
            listRekapApproval = new List<RekapApprovalData>();
            for (int a = 0; a < InputGridView.Rows.Count; a++)
            {
                DataGridViewRow rowData = InputGridView.Rows[a];
                Console.WriteLine("ID: " + rowData.Cells[0].ToString());
                int id = Int32.Parse(rowData.Cells[0].Value.ToString());
                int idDetailApproval = Int32.Parse(rowData.Cells[1].Value.ToString());
                DateTime tanggal = DateTime.Parse(rowData.Cells[2].Value.ToString());
                DateTime tanggalOrder = DateTime.Parse(rowData.Cells[3].Value.ToString());
                string order = rowData.Cells[4].Value.ToString();
                string orderType = rowData.Cells[5].Value.ToString();
                string kodePkt = rowData.Cells[6].Value.ToString();
                Int64 d100 = Int64.Parse(rowData.Cells[7].Value.ToString());
                Int64 d50 = Int64.Parse(rowData.Cells[8].Value.ToString());
                Int64 d20 = Int64.Parse(rowData.Cells[9].Value.ToString());
                string inputOpr = rowData.Cells[10].Value.ToString();
                string inputSpv = rowData.Cells[11].Value.ToString();
                string inputNoTxn = rowData.Cells[12].Value.ToString();
                string validasiOpr = rowData.Cells[13].Value.ToString();
                string validasiSpv = rowData.Cells[14].Value.ToString();
                string validasiNoTxn = rowData.Cells[15].Value.ToString();
                string kanwil = rowData.Cells[16].Value.ToString();
                string koordinator = rowData.Cells[17].Value.ToString();
                listRekapApproval.Add(new RekapApprovalData() {
                    id = Int32.Parse(rowData.Cells[0].Value.ToString()),
                    idDetailApproval = Int32.Parse(rowData.Cells[1].Value.ToString()),
                    tanggal = DateTime.Parse(rowData.Cells[2].Value.ToString()),
                    tanggalOrder = DateTime.Parse(rowData.Cells[3].Value.ToString()),
                    order = rowData.Cells[4].Value.ToString(),
                    orderType = rowData.Cells[5].Value.ToString(),
                    kodePkt = rowData.Cells[6].Value.ToString(),
                    d100 = Int64.Parse(rowData.Cells[7].Value.ToString()),
                    d50 = Int64.Parse(rowData.Cells[8].Value.ToString()),
                    d20 = Int64.Parse(rowData.Cells[9].Value.ToString()),
                    inputOpr = rowData.Cells[10].Value.ToString(),
                    inputSpv = rowData.Cells[11].Value.ToString(),
                    inputNoTxn = rowData.Cells[12].Value.ToString(),
                    validasiOpr = rowData.Cells[13].Value.ToString(),
                    validasiSpv = rowData.Cells[14].Value.ToString(),
                    validasiNoTxn = rowData.Cells[15].Value.ToString(),
                    kanwil = rowData.Cells[16].Value.ToString(),
                    koordinator = rowData.Cells[17].Value.ToString(),
                });
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
                            select new { tanggalProses = x.tanggal, tanggalOrder = x.tanggalOrder, namaPkt = y.namaPkt, kodePkt = x.kodePkt, order = x.order, orderType = x.orderType, d100 = x.d100, d50 = x.d50, d20 = x.d20, kanwil = x.kanwil, koordinator = x.koordinator, x.inputOpr, x.inputSpv, x.inputNoTxn, x.validasiOpr, x.validasiSpv, x.validasiNoTxn }
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
            dataLama = new List<RekapApprovalData>();
            foreach(var temp in listRekapApproval)
            {
                dataLama.Add(new RekapApprovalData() {
                    d100 = temp.d100,
                    d20 = temp.d20,
                    d50 = temp.d50,
                    id = temp.id,
                    idDetailApproval = temp.idDetailApproval,
                    inputNoTxn = temp.inputNoTxn,
                    inputOpr = temp.inputOpr,
                    inputSpv = temp.inputSpv,
                    kanwil = temp.kanwil,
                    kodePkt = temp.kodePkt,
                    koordinator = temp.koordinator,
                    order = temp.order,
                    orderType = temp.orderType,
                    tanggal = temp.tanggal,
                    tanggalOrder = temp.tanggalOrder,
                    validasiNoTxn = temp.validasiNoTxn,
                    validasiOpr = temp.validasiOpr,
                    validasiSpv = temp.validasiSpv,
                });
            }
            
        }

        private void InputGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

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

