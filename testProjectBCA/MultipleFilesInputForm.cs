using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA
{
    public partial class MultipleFilesInputForm : Form
    {
        Database1Entities db = new Database1Entities();
        String[] files;
        List<List<transaksiPkt>> collectionTransaksiPkt = new List<List<transaksiPkt>>();
        public MultipleFilesInputForm()
        {
            InitializeComponent();

            loadListDataYangSudahAda();
            loadDataYangBelumAda();
        }
        void loadListDataYangSudahAda()
        {
            dataYangSudahAdaList.Items.Clear();
            DateTime hariKemarin = DateTime.Now.AddDays(-1);
            String simpleDate = hariKemarin.ToShortDateString();
            DateTime tempTgl = Convert.ToDateTime(simpleDate);
            Console.WriteLine(tempTgl);
            var list = (from x in db.TransaksiAtms where x.tanggal == tempTgl select x).ToList();
            foreach(var temp in list)
            {
                dataYangSudahAdaList.Items.Add(temp.kodePkt);
            }
           
        }

        void loadDataYangBelumAda()
        {
            List<String> yangSudahAda=new List<String>();
            List<String> semuaPkt = new List<String>();

            filesListBelumMasuk.Items.Clear();

            var query = (from x in db.Pkts where x.kodePktATM != "" select x.kodePkt).ToList();
            semuaPkt = query;

            foreach(var item in dataYangSudahAdaList.Items)
            {
                yangSudahAda.Add(item.ToString());
            }
            //Console.WriteLine(yangSudahAda[0]);
            foreach (var temp in semuaPkt)
            {
                bool flag = false;
                foreach(var temp2 in yangSudahAda)
                {
                    if(temp2==temp)
                    {
                        flag = true;break;
                    }
                }
                if(!flag)
                {
                    filesListBelumMasuk.Items.Add(temp);
                }
            }
            
        }
        private void SelectFilesButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Multiselect = true;
            of.Filter = "Microsoft Excel | *.xls; *xlsx; *xlsm";
            
            if(of.ShowDialog() == DialogResult.OK)
            {
                loadForm.ShowSplashScreen();
                files = of.FileNames;
                filesList.Items.Clear();
                foreach (String temp in files)
                {
                    filesList.Items.Add(temp.Substring(temp.LastIndexOf("\\")+1, temp.LastIndexOf(".xls") - temp.LastIndexOf("\\")-1));
                }
                Database1Entities db = new Database1Entities();
                var q = (from x in db.Pkts select x).ToList();
                int counter = 0;
                List<Pkt> tempP = new List<Pkt>();
                foreach(string temp in filesList.Items)
                {
                    Console.WriteLine(counter++);
                    foreach (var temp2 in q)
                    {
                        if (temp2.kodePkt == temp)
                        {
                            tempP.Add(temp2);
                        }
                    }
                }
                foreach (var temp in tempP)
                {
                    q.Remove(temp);
                }

                //foreach(var temp in q)
                //{
                //    filesListBelumMasuk.Items.Add(temp.kodePkt);
                //}
                inputIntoCollection();
                loadForm.CloseForm();
                MessageBox.Show("Done!");
            }
        }
        private void inputIntoCollection()
        {
            foreach(String path in files)
            {
                DataSet ds = Util.openExcel(path);
                try
                {
                    collectionTransaksiPkt.Add(loadSheetsIntoClassList(Util.openExcel(path)));
                }
                catch (Exception e)
                {
                    MessageBox.Show(path + " Bermasalah");
                }
                GC.Collect();
            }
        }
        private void inputDataToDB()
        {
            foreach (List<transaksiPkt> list in collectionTransaksiPkt)
            {
                foreach (transaksiPkt temp in list)
                {
                    Util.inputDataTransaksiATMToDB(temp);
                }
            }
        }
        private List<transaksiPkt> loadSheetsIntoClassList(DataSet data)
        {
            List<transaksiPkt> list = new List<transaksiPkt>();
            int counter = 0;
            int counterList = 0;
            for (int x = 0; x < data.Tables.Count; x++)
            {
                var table = data.Tables[x];
                //Validasi Sheet Kosong
                if (table.Rows.Count < 10)
                    continue;
                try
                {
                    //tanggal pengajuan
                    DateTime testanggal = (DateTime)table.Rows[12][5];
                }
                catch (Exception e)
                {
                    continue;
                }

                Console.WriteLine(table.TableName);
                transaksiPkt pkt = new transaksiPkt();
                //Kode Pkt
                pkt.kodePkt = table.Rows[5][5].ToString();
                //tanggal pengajuan
                pkt.tanggalPengajuan = (DateTime)table.Rows[12][5];

                //Check record di db
                var checkRecord = (from q in db.TransaksiAtms where q.kodePkt == pkt.kodePkt && q.tanggal == pkt.tanggalPengajuan select q).FirstOrDefault();
                if (checkRecord != null)
                {
                    //MessageBox.Show("Data " + pkt.kodePkt + " Tanggal " + pkt.tanggalPengajuan.ToShortDateString() + " Sudah ada\nData tidak akan diinput", "Duplicate data");
                    continue;
                }

                //Pengambilan data hitungan dari db
                if (counter == 0 && (from q in db.TransaksiAtms where q.kodePkt == pkt.kodePkt select q).FirstOrDefault() != null)
                {
                    DateTime hariSebelomnya = pkt.tanggalPengajuan.AddDays(-1);
                    var query = (from q in db.TransaksiAtms where q.kodePkt == pkt.kodePkt && q.tanggal == hariSebelomnya select q).FirstOrDefault();
                    if (query != null)
                    {
                        pkt.saldoAwalHitungan.Add((Int64)query.saldoAkhir100);
                        pkt.saldoAwalHitungan.Add((Int64)query.saldoAkhir50);
                        pkt.saldoAwalHitungan.Add((Int64)query.saldoAkhir20);
                    }
                    else
                    {
                        MessageBox.Show("Data "+table.TableName+" tidak akan dimasukkan karena record hari sebelumnya tidak ada");
                        continue;
                    }
                    counter++;
                }
                else
                {
                    if (counterList == 0)
                        pkt.saldoAwalHitungan = pkt.saldoAwal;
                    else
                        pkt.saldoAwalHitungan = list[counterList-1].saldoAkhirHitungan;
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
                for (int i = 0; i < 7; i++)
                {
                    List<Int64> tempList = new List<Int64>();
                    if (table.Rows[22+i][5].ToString().Trim() == "" && (table.Rows[22 + i][6].ToString().Trim() == "" || table.Rows[22 + i][6].ToString().Trim() == "0") &&
                        (table.Rows[22 + i][7].ToString().Trim() == "" || table.Rows[22 + i][7].ToString().Trim() == "0") &&
                        (table.Rows[22 + i][8].ToString().Trim() == "" || table.Rows[22 + i][8].ToString().Trim() == "0"))
                        continue;
                    for (int j = 0; j < 3; j++)
                    {
                        Console.WriteLine(i + " " + j);
                        Console.WriteLine(table.Rows[22 + i][6 + j].ToString());
                        if (table.Rows[22 + i][6 + j].ToString().Trim() != "0" && table.Rows[22 + i][6 + j].ToString().Trim() != "" && table.Rows[22 + i][6 + j].ToString().Trim() != "-")
                            tempList.Add(Int64.Parse(table.Rows[22 + i][6 + j].ToString()));
                        else
                            tempList.Add(0);
                    }
                    pkt.bonAtmYangDisetujui.Add(tempList);
                    //MessageBox.Show(pkt.bonAtmYangDisetujui[0][0].ToString());
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
                for (int i = 0; i < 7; i++)
                {
                    List<Int64> tempList = new List<Int64>();
                    if (table.Rows[34 + i][5].ToString().Trim() == "" && 
                        (table.Rows[34 + i][6].ToString().Trim() == "" || table.Rows[34 + i][6].ToString().Trim() == "0" || table.Rows[34 + i][6].ToString().Trim() == ".") &&
                        (table.Rows[34 + i][7].ToString().Trim() == "" || table.Rows[34 + i][7].ToString().Trim() == "0" || table.Rows[34 + i][7].ToString().Trim() == ".") &&
                        (table.Rows[34 + i][8].ToString().Trim() == "" || table.Rows[34 + i][8].ToString().Trim() == "0" || table.Rows[34 + i][8].ToString().Trim() == "."))
                        continue;

                    for (int j = 0; j < 4; j++)
                    {
                        if (table.Rows[34 + i][6 + j].ToString().Trim() != "0" && table.Rows[34 + i][6 + j].ToString().Trim() != "" && table.Rows[34 + i][6+j].ToString() != ".")
                            tempList.Add(Int64.Parse(table.Rows[34 + i][6 + j].ToString()));
                        else
                            tempList.Add(0);
                    }
                    pkt.permintaanBon.Add(tempList);

                }
                if (!String.IsNullOrEmpty(table.Rows[44][6].ToString()) && !String.IsNullOrEmpty(table.Rows[44][7].ToString()) && !String.IsNullOrEmpty(table.Rows[44][8].ToString()))
                {
                    Int64 buf;
                    if (String.IsNullOrEmpty(table.Rows[44][6].ToString()))
                        pkt.permintaanAdhoc.Add(0);
                    else if (Int64.TryParse(table.Rows[44][6].ToString(), out buf))
                        pkt.permintaanAdhoc.Add(buf);
                    else
                        pkt.permintaanAdhoc.Add(0);

                    if (String.IsNullOrEmpty(table.Rows[44][7].ToString()))
                        pkt.permintaanAdhoc.Add(0);
                    else if (Int64.TryParse(table.Rows[44][7].ToString(), out buf))
                        pkt.permintaanAdhoc.Add(buf);
                    else
                        pkt.permintaanAdhoc.Add(0);

                    if (String.IsNullOrEmpty(table.Rows[44][8].ToString()))
                        pkt.permintaanAdhoc.Add(0);
                    else if (Int64.TryParse(table.Rows[44][8].ToString(), out buf))
                        pkt.permintaanAdhoc.Add(buf);
                    else
                        pkt.permintaanAdhoc.Add(0);
                }
                else
                {
                    pkt.permintaanAdhoc.Add(0);
                    pkt.permintaanAdhoc.Add(0);
                    pkt.permintaanAdhoc.Add(0);
                } 
                pkt.hitungSaldoAkhir();



                //Bagian Checking 
                String errMsg = "";
                bool isError = false;
                if (pkt.saldoAwalHitungan[0] != pkt.saldoAwal[0] || pkt.saldoAwalHitungan[1] != pkt.saldoAwal[1] || pkt.saldoAwalHitungan[2] != pkt.saldoAwal[2])
                {
                    isError = true;
                    errMsg+="Saldo awal tidak sesuai\n=====================\n"+
                        "\nHitungan 100: Rp. " + pkt.saldoAwalHitungan[0].ToString("n0") + "\nLaporan 100: Rp. " + pkt.saldoAwal[0].ToString("n0") +
                        "\nHitungan 50: Rp. " + pkt.saldoAwalHitungan[1].ToString("n0") + "\nLaporan 50: Rp. " + pkt.saldoAwal[1].ToString("n0") +
                        "\nHitungan 20: Rp. " + pkt.saldoAwalHitungan[2].ToString("n0") + "\nLaporan 20: Rp." + pkt.saldoAwal[2].ToString("n0");
                }

                var queryApproval = (from a in db.Approvals
                                     join da in db.DetailApprovals on a.idApproval equals da.idApproval
                                     where a.kodePkt == pkt.kodePkt && pkt.tanggalPengajuan == da.tanggal
                                     select new { Approval = a, DetailApproval = da }).ToList();
                if (queryApproval.Count>0 && queryApproval != null)
                {
                    if(queryApproval[queryApproval.Count-1].DetailApproval.bon100 != pkt.penerimaanBon[0] || queryApproval[queryApproval.Count - 1].DetailApproval.bon50 != pkt.penerimaanBon[1] || queryApproval[queryApproval.Count - 1].DetailApproval.bon20 != pkt.penerimaanBon[2])
                    {
                        isError = true;
                        errMsg += "\nBon menurut approval : \n=====================\n" +
                            "\nHitungan 100: Rp. " + ((Int64)queryApproval[queryApproval.Count - 1].DetailApproval.bon100).ToString("n0") + "\nLaporan 100: Rp. " + pkt.penerimaanBon[0].ToString("n0") +
                            "\nHitungan 50: Rp. " + ((Int64)queryApproval[queryApproval.Count - 1].DetailApproval.bon50).ToString("n0") + "\nLaporan 50: Rp. " + pkt.penerimaanBon[1].ToString("n0") +
                            "\nHitungan 20: Rp. " + ((Int64)queryApproval[queryApproval.Count - 1].DetailApproval.bon20).ToString("n0") + "\nLaporan 20: Rp." + pkt.penerimaanBon[2].ToString("n0"); 
                    }
                }

                if (isError)
                {
                    String errMsg2 = "Laporan " + pkt.kodePkt + " Tanggal: " + pkt.tanggalPengajuan.ToShortDateString() + " Tidak sesuai\n";
                    errMsg2 += errMsg;
                    MessageBox.Show(errMsg2, "Warning!");
                }
                list.Add(pkt);
                counterList++;
            }
            return list;
        }

        private void inputBtn_Click(object sender, EventArgs e)
        {
            loadForm.ShowSplashScreen();
            inputDataToDB();
            loadListDataYangSudahAda();
            loadDataYangBelumAda();
            loadForm.CloseForm();
        }
    }
}
