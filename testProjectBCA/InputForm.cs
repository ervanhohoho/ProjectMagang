using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA
{
    public partial class InputForm : Form
    {
        private int sheetIndex = 0;
        private int pktIndex = 0;
        List<List<String>> listSheetNames = new List<List<String>>();
        private List<DataSet> ds = new List<DataSet>();
        private bool firstrun = true;
        
        List<List<transaksiPkt>> collectionTransaksiPkt = new List<List<transaksiPkt>>();
        List<List<Int64>> saldoAwalHitungan = new List<List<Int64>>();
        Database1Entities db = new Database1Entities();
        public InputForm()
        {
            InitializeComponent();
            //dataGridView1.Columns.Add("jenisTransaksi","Jenis Transaksi");
            //dataGridView1.Rows.Add(nRow);
            //dataGridView1.Enabled = false;
        }

        private void getDataButtonClick(object sender, EventArgs e)
        {
            Database1Entities db = new Database1Entities();
            List<String> pkts = (from x in db.Pkts select x.kodePkt).ToList();
            pktComboBox.DataSource = pkts;

            /**
            load semua file pkt
            **/
            loadForm lf = new loadForm();
            lf.Show();
            Application.DoEvents();
            FolderBrowserDialog fdb = new FolderBrowserDialog();
            String folderPath;
            if (fdb.ShowDialog() == DialogResult.OK)
            {
                folderPath = fdb.SelectedPath;
                foreach (String temp in pkts)
                {
                    String filePath = folderPath +"\\"+ temp + ".xlsx";
                    //Console.WriteLine(filePath);
                    //filePath = Variables.parentFolder + Variables.todayYear + "\\" + Variables.todayMonth + "\\" + Variables.todayDay + "\\" + temp + ".xlsx";
                    //Console.WriteLine(filePath);
                    bool found = false;
                    try
                    {
                        Util.openExcel(filePath);

                        //Reinisialisasi
                        sheetIndex = 0;
                        dataGridView1.Rows.Clear();
                        dataGridView1.Refresh();

                        DataSet data = Util.openExcel(filePath);
                        ds.Add(data);

                        List<transaksiPkt> pktSheets = loadDataIntoClass(data);
                        collectionTransaksiPkt.Add(pktSheets);

                        List<String> sheetnames = new List<String>();
                        for (int i = 0; i < data.Tables.Count; i++)
                        {
                            sheetnames.Add(data.Tables[i].TableName.ToString());
                        }
                        listSheetNames.Add(sheetnames);

                        //comboBox1.DataSource = sheetnames;
                        //loadGridView(data);

                        Util.closeExcel();
                        found = true;
                    }
                    catch (Exception err)
                    {
                    }
                    if (!found)
                    {
                        try
                        {
                            filePath = folderPath + "\\" + temp + ".xls";
                            //Console.WriteLine(filePath);
                            //filePath = Variables.parentFolder + Variables.todayYear + "\\" + Variables.todayMonth + "\\" + Variables.todayDay + "\\" + temp + ".xls";
                            //Console.WriteLine(filePath);
                            //Reinisialisasi
                            sheetIndex = 0;
                            dataGridView1.Rows.Clear();
                            dataGridView1.Refresh();

                            DataSet data = Util.openExcel(filePath);
                            List<String> sheetnames = new List<String>();

                            for (int i = 0; i < data.Tables.Count; i++)
                            {
                                sheetnames.Add(data.Tables[i].TableName.ToString());
                                //Console.WriteLine(data.Tables[i].TableName.ToString());
                            }
                            listSheetNames.Add(sheetnames);
                            ds.Add(data);

                            List<transaksiPkt> pktSheets = loadDataIntoClass(data);
                            collectionTransaksiPkt.Add(pktSheets);

                            Util.closeExcel();
                            found = true;
                        }
                        catch (Exception err2)
                        {
                            //MessageBox.Show("File " + temp + " tidak ada");
                        }
                    }
                    if(!found)
                    {
                        //MessageBox.Show("File " + temp + " tidak ada");
                    }
                }
            }
            lf.Close();
            MessageBox.Show("Done");
            comboBox1.DataSource = listSheetNames[0];
            loadGridView(ds[0]);
        }

        private void loadGridView(DataSet data)
        {
            
            List<transaksiPkt> pkt = loadDataIntoClass(data);
            if(firstrun)
            {
                //Inisialisasi header
                dataGridView1.Columns.Add("Keterangan", "Keterangan");
                dataGridView1.Columns.Add("100000", "Rp. 100.000");
                dataGridView1.Columns.Add("50000", "Rp. 50.000");
                dataGridView1.Columns.Add("20000", "Rp. 20.000");
                dataGridView1.Columns.Add("Lain-Lain", "Lain-Lain");
                firstrun = false;
            }
            dataGridView1.Columns[0].DefaultCellStyle.BackColor = Color.Azure;
            for(int i = 1; i < 4; i++) { 
                dataGridView1.Columns[i].DefaultCellStyle.Format = "c2";
                dataGridView1.Columns[i].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            }

           
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewCell keterangan = new DataGridViewTextBoxCell();

            //Tanggal
            tanggalLbl.Text = pkt[sheetIndex].tanggalPengajuan.ToString();

            //Baris saldo awal
            keterangan.Value = "Saldo Awal";
            row.Cells.Add(keterangan);

            foreach (Int64 temp in pkt[sheetIndex].saldoAwal)
            {
                DataGridViewCell cell = new DataGridViewTextBoxCell();
                cell.Value = temp;
                row.Cells.Add(cell);
            }
            dataGridView1.Rows.Add(row);

            //Saldo Awal Hitungan
            row = new DataGridViewRow();
            keterangan = new DataGridViewTextBoxCell();
            keterangan.Value = "Saldo Awal Hitungan";
            row.Cells.Add(keterangan);
            DataGridViewCell cell1 = new DataGridViewTextBoxCell();
            cell1.Value = collectionTransaksiPkt[pktIndex][sheetIndex].saldoAwalHitungan[0];
            row.Cells.Add(cell1);
            cell1 = new DataGridViewTextBoxCell();
            cell1.Value = collectionTransaksiPkt[pktIndex][sheetIndex].saldoAwalHitungan[1];
            row.Cells.Add(cell1);
            cell1 = new DataGridViewTextBoxCell();
            cell1.Value = collectionTransaksiPkt[pktIndex][sheetIndex].saldoAwalHitungan[2];
            row.Cells.Add(cell1);
            dataGridView1.Rows.Add(row);

            //Saldo Akhir
            row = new DataGridViewRow();
            keterangan = new DataGridViewTextBoxCell();
            keterangan.Value = "Saldo Akhir";
            row.Cells.Add(keterangan);
            foreach (Int64 temp in pkt[sheetIndex].saldoAkhir)
            {
                DataGridViewCell cell = new DataGridViewTextBoxCell();
                cell.Value = temp;
                row.Cells.Add(cell);
            }
            dataGridView1.Rows.Add(row);

            //Setor
            row = new DataGridViewRow();
            keterangan = new DataGridViewTextBoxCell();
            keterangan.Value = "Setor";
            row.Cells.Add(keterangan);
            foreach (Int64 temp in pkt[sheetIndex].setorUang)
            {
                DataGridViewCell cell = new DataGridViewTextBoxCell();
                cell.Value = temp;
                row.Cells.Add(cell);
            }
            dataGridView1.Rows.Add(row);

            //Penerimaan Bon CIT
            row = new DataGridViewRow();
            keterangan = new DataGridViewTextBoxCell();
            keterangan.Value = "Penerimaan BON CIT";
            row.Cells.Add(keterangan);
            foreach (Int64 temp in pkt[sheetIndex].penerimaanBon)
            {
                DataGridViewCell cell = new DataGridViewTextBoxCell();
                cell.Value = temp;
                row.Cells.Add(cell);
            }
            dataGridView1.Rows.Add(row);

            //Penerimaan Adhoc
            row = new DataGridViewRow();
            keterangan = new DataGridViewTextBoxCell();
            keterangan.Value = "Adhoc";
            row.Cells.Add(keterangan);
            foreach (Int64 temp in pkt[sheetIndex].penerimaanBonAdhoc)
            {
                DataGridViewCell cell = new DataGridViewTextBoxCell();
                cell.Value = temp;
                row.Cells.Add(cell);
            }
            dataGridView1.Rows.Add(row);

            //Pengisian ATM
            row = new DataGridViewRow();
            keterangan = new DataGridViewTextBoxCell();
            keterangan.Value = "Pengisian ATM";
            row.Cells.Add(keterangan);
            foreach (Int64 temp in pkt[sheetIndex].pengisianAtm)
            {
                DataGridViewCell cell = new DataGridViewTextBoxCell();
                cell.Value = temp;
                row.Cells.Add(cell);
            }
            dataGridView1.Rows.Add(row);

            //Pengisian CRM
            row = new DataGridViewRow();
            keterangan = new DataGridViewTextBoxCell();
            keterangan.Value = "Pengisian CRM";
            row.Cells.Add(keterangan);
            foreach (Int64 temp in pkt[sheetIndex].pengisianCrm)
            {
                DataGridViewCell cell = new DataGridViewTextBoxCell();
                cell.Value = temp;
                row.Cells.Add(cell);
            }
            dataGridView1.Rows.Add(row);

            //Bongkaran ATM
            row = new DataGridViewRow();
            keterangan = new DataGridViewTextBoxCell();
            keterangan.Value = "Bongkaran ATM";
            row.Cells.Add(keterangan);
            foreach (Int64 temp in pkt[sheetIndex].bongkaranAtm)
            {
                DataGridViewCell cell = new DataGridViewTextBoxCell();
                cell.Value = temp;
                row.Cells.Add(cell);
            }
            dataGridView1.Rows.Add(row);

            //Bongkaran CDM
            row = new DataGridViewRow();
            keterangan = new DataGridViewTextBoxCell();
            keterangan.Value = "Bongkaran CDM";
            row.Cells.Add(keterangan);
            foreach (Int64 temp in pkt[sheetIndex].bongkaranCdm)
            {
                DataGridViewCell cell = new DataGridViewTextBoxCell();
                cell.Value = temp;
                row.Cells.Add(cell);
            }
            dataGridView1.Rows.Add(row);

            //Bongkaran CRM
            row = new DataGridViewRow();
            keterangan = new DataGridViewTextBoxCell();
            keterangan.Value = "Bongkaran CRM";
            row.Cells.Add(keterangan);
            foreach (Int64 temp in pkt[sheetIndex].bongkaranCrm)
            {
                DataGridViewCell cell = new DataGridViewTextBoxCell();
                cell.Value = temp;
                row.Cells.Add(cell);
            }
            dataGridView1.Rows.Add(row);

            //Penerimaan BON CIT
            foreach (var temp in pkt[sheetIndex].bonAtmYangDisetujui)
            {
                row = new DataGridViewRow();
                keterangan = new DataGridViewTextBoxCell();
                keterangan.Value = "Bon ATM Yang Disetujui";
                row.Cells.Add(keterangan);
                
                dataGridView1.Rows.Add(row);
            }
            
        }
        private List<transaksiPkt> loadDataIntoClass(DataSet data)
        {
            List<transaksiPkt> list = new List<transaksiPkt>();
            for (int x = 0; x < data.Tables.Count; x++)
            {
                var table = data.Tables[x];
                transaksiPkt pkt = new transaksiPkt();
                //Kode Pkt
                pkt.kodePkt = table.Rows[5][5].ToString();
                //tanggal pengajuan
                pkt.tanggalPengajuan = (DateTime)table.Rows[12][5];
                
                //Pengambilan data hitungan dari db
                if(x==0)
                {
                    DateTime hariSebelomnya = pkt.tanggalPengajuan.AddDays(-1);
                    var query = (from q in db.TransaksiAtms where q.kodePkt == pkt.kodePkt && q.tanggal == hariSebelomnya select q).FirstOrDefault();            
                    
                    pkt.saldoAwalHitungan.Add((Int64)query.saldoAkhir100);
                    pkt.saldoAwalHitungan.Add((Int64)query.saldoAkhir50);
                    pkt.saldoAwalHitungan.Add((Int64)query.saldoAkhir20);
                }
                else
                {
                    pkt.saldoAwalHitungan = list[x - 1].saldoAkhirHitungan;
                }


                //Pengambilan saldo awal dari excel
                for (int a = 0; a < 4; a++)
                {
                    if (table.Rows[12][6 + a].ToString() != "0" && table.Rows[12][6 + a].ToString() != "")
                        pkt.saldoAwal.Add(Int64.Parse(table.Rows[12][6 + a].ToString()));
                    else
                        pkt.saldoAwal.Add(0);
                }
                //Pengambilan setor uang dari excel
                for (int a = 0; a < 4; a++)
                {
                    if (table.Rows[14][6 + a].ToString() != "0" && table.Rows[14][6 + a].ToString() != "")
                        pkt.setorUang.Add(Int64.Parse(table.Rows[14][6 + a].ToString()));
                    else
                        pkt.setorUang.Add(0);
                }
                //Pengambilan penerimaan bon dari excel
                for (int a = 0; a < 4; a++)
                {
                    if (table.Rows[15][6 + a].ToString() != "0" && table.Rows[15][6 + a].ToString() != "")
                        pkt.penerimaanBon.Add(Int64.Parse(table.Rows[15][6 + a].ToString()));
                    else
                        pkt.penerimaanBon.Add(0);
                }
                //Pengambilan penerimaan bon adhoc dari excel
                for (int a = 0; a < 4; a++)
                {
                    if (table.Rows[16][6 + a].ToString() != "0" && table.Rows[16][6 + a].ToString() != "")
                        pkt.penerimaanBonAdhoc.Add(Int64.Parse(table.Rows[16][6 + a].ToString()));
                    else
                        pkt.penerimaanBonAdhoc.Add(0);
                }
                //Pengambilan pengisian atm dari excel
                for (int a = 0; a < 4; a++)
                {
                    if (table.Rows[17][6 + a].ToString() != "0" && table.Rows[17][6 + a].ToString() != "")
                        pkt.pengisianAtm.Add(Int64.Parse(table.Rows[17][6 + a].ToString()));
                    else
                        pkt.pengisianAtm.Add(0);
                }
                //Pengambilan pengisian crm dari excel
                for (int a = 0; a < 4; a++)
                {
                    if (table.Rows[18][6 + a].ToString() != "0" && table.Rows[18][6 + a].ToString() != "")
                        pkt.pengisianCrm.Add(Int64.Parse(table.Rows[18][6 + a].ToString()));
                    else
                        pkt.pengisianCrm.Add(0);
                }
                //Pengambilan bongkaran ATM dari excel
                for (int a = 0; a < 4; a++)
                {
                    if (table.Rows[19][6 + a].ToString() != "0" && table.Rows[19][6 + a].ToString() != "")
                        pkt.bongkaranAtm.Add(Int64.Parse(table.Rows[19][6 + a].ToString()));
                    else
                        pkt.bongkaranAtm.Add(0);
                }
                //Pengambilan bongkaran cdm dari excel
                for (int a = 0; a < 4; a++)
                {
                    if (table.Rows[20][6 + a].ToString() != "0" && table.Rows[20][6 + a].ToString() != "")
                        pkt.bongkaranCdm.Add(Int64.Parse(table.Rows[20][6 + a].ToString()));
                    else
                        pkt.bongkaranCdm.Add(0);
                }
                //Pengambilan bongkaran crm dari excel
                for (int a = 0; a < 4; a++)
                {
                    if (table.Rows[21][6 + a].ToString() != "0" && table.Rows[21][6 + a].ToString() != "")
                        pkt.bongkaranCrm.Add(Int64.Parse(table.Rows[21][6 + a].ToString()));
                    else
                        pkt.bongkaranCrm.Add(0);
                }
                //Pengambilan bon yang disetujui dari excel
                for (int i = 0; i < 15; i++)
                {
                    DataRow row = table.Rows[22 + i];
                    if (row[5].ToString().Trim() == "" && (String.IsNullOrEmpty(row[6].ToString().Trim()) || row[6].ToString().Trim() == "0") && (String.IsNullOrEmpty(row[7].ToString().Trim()) || row[7].ToString().Trim() == "0") && (String.IsNullOrEmpty(row[8].ToString().Trim()) || row[8].ToString().Trim() == "0"))
                        continue;

                    String tanggalE = row[5].ToString(), d100E = row[6].ToString(), d50E = row[7].ToString(), d20E = row[8].ToString();

                    DateTime tanggal;
                    Int64 d100, d50, d20, buf;

                    //Tanggal
                    tanggal = Convert.ToDateTime(tanggalE);

                    //Denom 100.000
                    if (!String.IsNullOrEmpty(row[6].ToString()))
                    {
                        if (Int64.TryParse(d100E, out buf))
                            d100 = buf;
                        else
                            d100 = 0;
                    }
                    else
                        d100 = 0;

                    //Denom 50.000
                    if (!String.IsNullOrEmpty(row[7].ToString()))
                    {
                        if (Int64.TryParse(d50E, out buf))
                            d50 = buf;
                        else
                            d50 = 0;
                    }
                    else
                        d50 = 0;

                    //Denom 20.000
                    if (!String.IsNullOrEmpty(row[8].ToString()))
                    {
                        if (Int64.TryParse(d20E, out buf))
                            d20 = buf;
                        else
                            d20 = 0;
                    }
                    else
                        d20 = 0;
                    pkt.bonAtmYangDisetujui.Add(new Denom()
                    {
                        tgl = tanggal,
                        d100 = d100,
                        d50 = d50,
                        d20 = d20
                    });
                }
                //Pengambilan saldo akhir dari excel
                for (int a = 0; a < 4; a++)
                {
                    if (table.Rows[32][6 + a].ToString() != "0" && table.Rows[32][6 + a].ToString() != "")
                        pkt.saldoAkhir.Add(Int64.Parse(table.Rows[32][6 + a].ToString()));
                    else
                        pkt.saldoAkhir.Add(0);
                }
                //Pengambilan data permintaan bon
                for (int i = 0; i < 10; i++)
                {
                    DataRow row = table.Rows[40 + i];
                    if (row[5].ToString().Trim() == "" &&
                        (String.IsNullOrEmpty(row[6].ToString().Trim()) || row[6].ToString().Trim() == "0" || row[6].ToString().Trim() == ".") &&
                        (String.IsNullOrEmpty(row[7].ToString().Trim()) || row[7].ToString().Trim() == "0" || row[7].ToString().Trim() == ".") &&
                        (String.IsNullOrEmpty(row[8].ToString().Trim()) || row[8].ToString().Trim() == "0" || row[8].ToString().Trim() == "."))
                        continue;
                    String tanggalE = row[5].ToString(), d100E = row[6].ToString(), d50E = row[7].ToString(), d20E = row[8].ToString();

                    DateTime tanggal;
                    Int64 d100, d50, d20, buf;

                    //Tanggal
                    tanggal = Convert.ToDateTime(tanggalE);

                    //Denom 100.000
                    if (!String.IsNullOrEmpty(row[6].ToString()))
                    {
                        if (Int64.TryParse(d100E, out buf))
                            d100 = buf;
                        else
                            d100 = 0;
                    }
                    else
                        d100 = 0;

                    //Denom 50.000
                    if (!String.IsNullOrEmpty(row[7].ToString()))
                    {
                        if (Int64.TryParse(d50E, out buf))
                            d50 = buf;
                        else
                            d50 = 0;
                    }
                    else
                        d50 = 0;

                    //Denom 20.000
                    if (!String.IsNullOrEmpty(row[8].ToString()))
                    {
                        if (Int64.TryParse(d20E, out buf))
                            d20 = buf;
                        else
                            d20 = 0;
                    }
                    else
                        d20 = 0;

                    pkt.permintaanBon.Add(new Denom()
                    {
                        tgl = tanggal,
                        d100 = d100,
                        d50 = d50,
                        d20 = d20
                    });
                }

                pkt.hitungSaldoAkhir();
                list.Add(pkt);
            }
            return list;
        }
        private void inputDataToDB()
        {
            foreach (DataSet temp1 in ds)
            {
                List<transaksiPkt> list = loadDataIntoClass(temp1);
                foreach (transaksiPkt temp in list)
                {
                    Util.inputDataTransaksiATMToDB(temp);
                }
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            sheetIndex = comboBox1.SelectedIndex;
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            loadGridView(ds[pktIndex]);
        }

        private void inputButton_Click(object sender, EventArgs e)
        {
            if(ds!=null)
                inputDataToDB();
            this.Close();
        }

        private void pktComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            pktIndex = pktComboBox.SelectedIndex;
            sheetIndex = 0;
            comboBox1.DataSource = listSheetNames[pktIndex];
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            loadGridView(ds[pktIndex]);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if(fbd.ShowDialog() == DialogResult.OK)
            {
                Variables.parentFolder = fbd.SelectedPath;
            }
        }
    }
}
