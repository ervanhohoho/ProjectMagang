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
    public partial class MultipleFilesInputUpdateForm : Form
    {
        Database1Entities db = new Database1Entities();
        String[] files;
        List<List<transaksiPkt>> collectionTransaksiPkt = new List<List<transaksiPkt>>();
        int counter;
        int counterList;
        public MultipleFilesInputUpdateForm()
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
            counter = 0;
            counterList = 0;
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

                bool update = false;
                //Check record di db
                var checkRecord = (from q in db.TransaksiAtms where q.kodePkt == pkt.kodePkt && q.tanggal == pkt.tanggalPengajuan select q).FirstOrDefault();
                if (checkRecord != null)
                {
                    update = updateData(table, pkt.kodePkt, pkt.tanggalPengajuan);
                    continue;
                }
                if (!update)
                {
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
                            MessageBox.Show("Data " + table.TableName + " tidak akan dimasukkan karena record hari sebelumnya tidak ada");
                            continue;
                        }
                        counter++;
                    }
                    else
                    {
                        if (counterList == 0)
                            pkt.saldoAwalHitungan = pkt.saldoAwal;
                        else
                        {
                            pkt.saldoAwalHitungan = list[counterList - 1].saldoAkhirHitungan;
                        }
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
                        if (table.Rows[38][6 + a].ToString() != "0" && table.Rows[32][6 + a].ToString() != "")
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
                    if (!String.IsNullOrEmpty(table.Rows[52][6].ToString()) && !String.IsNullOrEmpty(table.Rows[52][7].ToString()) && !String.IsNullOrEmpty(table.Rows[52][8].ToString()))
                    {
                        Int64 buf;
                        if (String.IsNullOrEmpty(table.Rows[52][6].ToString()))
                            pkt.permintaanAdhoc.Add(0);
                        else if (Int64.TryParse(table.Rows[52][6].ToString(), out buf))
                            pkt.permintaanAdhoc.Add(buf);
                        else
                            pkt.permintaanAdhoc.Add(0);

                        if (String.IsNullOrEmpty(table.Rows[52][7].ToString()))
                            pkt.permintaanAdhoc.Add(0);
                        else if (Int64.TryParse(table.Rows[52][7].ToString(), out buf))
                            pkt.permintaanAdhoc.Add(buf);
                        else
                            pkt.permintaanAdhoc.Add(0);

                        if (String.IsNullOrEmpty(table.Rows[52][8].ToString()))
                            pkt.permintaanAdhoc.Add(0);
                        else if (Int64.TryParse(table.Rows[52][8].ToString(), out buf))
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
                        errMsg += "Saldo awal tidak sesuai\n=====================\n" +
                            "\nHitungan 100: Rp. " + pkt.saldoAwalHitungan[0].ToString("n0") + "\nLaporan 100: Rp. " + pkt.saldoAwal[0].ToString("n0") +
                            "\nHitungan 50: Rp. " + pkt.saldoAwalHitungan[1].ToString("n0") + "\nLaporan 50: Rp. " + pkt.saldoAwal[1].ToString("n0") +
                            "\nHitungan 20: Rp. " + pkt.saldoAwalHitungan[2].ToString("n0") + "\nLaporan 20: Rp." + pkt.saldoAwal[2].ToString("n0");
                    }

                    var queryApproval = (from a in db.Approvals
                                         join da in db.DetailApprovals on a.idApproval equals da.idApproval
                                         where a.kodePkt == pkt.kodePkt && pkt.tanggalPengajuan == da.tanggal
                                         select new { Approval = a, DetailApproval = da }).ToList();
                    if (queryApproval.Count > 0 && queryApproval != null)
                    {
                        if (queryApproval[queryApproval.Count - 1].DetailApproval.bon100 != pkt.penerimaanBon[0] || queryApproval[queryApproval.Count - 1].DetailApproval.bon50 != pkt.penerimaanBon[1] || queryApproval[queryApproval.Count - 1].DetailApproval.bon20 != pkt.penerimaanBon[2])
                        {
                            isError = true;
                            errMsg += "\nBon menurut approval : \n=====================\n" +
                                "\nHitungan 100: Rp. " + ((Int64)queryApproval[queryApproval.Count - 1].DetailApproval.bon100).ToString("n0") + "\nLaporan 100: Rp. " + pkt.penerimaanBon[0].ToString("n0") +
                                "\nHitungan 50: Rp. " + ((Int64)queryApproval[queryApproval.Count - 1].DetailApproval.bon50).ToString("n0") + "\nLaporan 50: Rp. " + pkt.penerimaanBon[1].ToString("n0") +
                                "\nHitungan 20: Rp. " + ((Int64)queryApproval[queryApproval.Count - 1].DetailApproval.bon20).ToString("n0") + "\nLaporan 20: Rp." + pkt.penerimaanBon[2].ToString("n0");
                        }
                    }
                    //Cek bon yang sudah disetujui
                    var queryApprovalDanBon = (from a in db.Approvals
                                         join da in db.DetailApprovals on a.idApproval equals da.idApproval
                                         from lb in db.laporanBons.Where( q => a.kodePkt == q.kodePkt && da.tanggal == q.tanggal).DefaultIfEmpty()
                                         where a.kodePkt == pkt.kodePkt && pkt.tanggalPengajuan == da.tanggal
                                         select new { Approval = a, DetailApproval = da }).ToList();
                    int bonAtmYangDisetujuiCounter = 1;
                    foreach (var temp in pkt.bonAtmYangDisetujui)
                    {

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
            }
            return list;
        }

        private bool updateData(DataTable table, string kodePkt,DateTime tanggal )
        {
            bool update = false;
            transaksiPkt pkt = new transaksiPkt();
            //Kode Pkt
            pkt.kodePkt = table.Rows[5][5].ToString();
            //tanggal pengajuan
            pkt.tanggalPengajuan = (DateTime)table.Rows[12][5];
            //Pengambilan data hitungan dari db
            if (counter == 0 && (from x in db.TransaksiAtms where x.kodePkt == pkt.kodePkt select x).FirstOrDefault() != null)
            {
                DateTime hariSebelomnya = pkt.tanggalPengajuan.AddDays(-1);
                var query = (from x in db.TransaksiAtms where x.kodePkt == pkt.kodePkt && x.tanggal == hariSebelomnya select x).FirstOrDefault();
                if (query != null)
                {
                    pkt.saldoAwalHitungan.Add((Int64)query.saldoAkhir100);
                    pkt.saldoAwalHitungan.Add((Int64)query.saldoAkhir50);
                    pkt.saldoAwalHitungan.Add((Int64)query.saldoAkhir20);
                }
                counter++;
            }
            else
            {
                pkt.saldoAwalHitungan = pkt.saldoAwal;
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

                DateTime Ttanggal;
                Int64 d100, d50, d20, buf;

                //Tanggal
                Ttanggal = Convert.ToDateTime(tanggalE);

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
                    tgl = Ttanggal,
                    d100 = d100,
                    d50 = d50,
                    d20 = d20
                });
            }
            //Pengambilan saldo akhir dari excel
            for (int a = 0; a < 4; a++)
            {
                if (table.Rows[38][6 + a].ToString() != "0" && table.Rows[32][6 + a].ToString() != "")
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

                DateTime Ttanggal;
                Int64 d100, d50, d20, buf;

                //Tanggal
                Ttanggal = Convert.ToDateTime(tanggalE);

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
                    tgl = Ttanggal,
                    d100 = d100,
                    d50 = d50,
                    d20 = d20
                });
            }
            if (!String.IsNullOrEmpty(table.Rows[52][6].ToString()) && !String.IsNullOrEmpty(table.Rows[52][7].ToString()) && !String.IsNullOrEmpty(table.Rows[52][8].ToString()))
            {
                Int64 buf;
                if (String.IsNullOrEmpty(table.Rows[52][6].ToString()))
                    pkt.permintaanAdhoc.Add(0);
                else if (Int64.TryParse(table.Rows[52][6].ToString(), out buf))
                    pkt.permintaanAdhoc.Add(buf);
                else
                    pkt.permintaanAdhoc.Add(0);

                if (String.IsNullOrEmpty(table.Rows[52][7].ToString()))
                    pkt.permintaanAdhoc.Add(0);
                else if (Int64.TryParse(table.Rows[52][7].ToString(), out buf))
                    pkt.permintaanAdhoc.Add(buf);
                else
                    pkt.permintaanAdhoc.Add(0);

                if (String.IsNullOrEmpty(table.Rows[52][8].ToString()))
                    pkt.permintaanAdhoc.Add(0);
                else if (Int64.TryParse(table.Rows[52][8].ToString(), out buf))
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


            var q = (from x in db.TransaksiAtms where x.kodePkt == pkt.kodePkt && x.tanggal == pkt.tanggalPengajuan select x).FirstOrDefault();
            if (q != null)
            {
                q.adhoc100 = pkt.penerimaanBonAdhoc[0];
                q.adhoc50 = pkt.penerimaanBonAdhoc[1];
                q.adhoc20 = pkt.penerimaanBonAdhoc[2];
                q.bon100 = pkt.penerimaanBon[0];
                q.bon50 = pkt.penerimaanBon[1];
                q.bon20 = pkt.penerimaanBon[2];
                q.isiATM100 = pkt.pengisianAtm[0];
                q.isiATM50 = pkt.pengisianAtm[1];
                q.isiATM20 = pkt.pengisianAtm[2];
                q.isiCRM100 = pkt.pengisianCrm[0];
                q.isiCRM50 = pkt.pengisianCrm[1];
                q.isiCRM20 = pkt.pengisianCrm[2];
                q.setor100 = pkt.setorUang[0];
                q.setor50 = pkt.setorUang[1];
                q.setor20 = pkt.setorUang[2];
                q.sislokATM100 = pkt.bongkaranAtm[0];
                q.sislokATM50 = pkt.bongkaranAtm[1];
                q.sislokATM20 = pkt.bongkaranAtm[2];
                q.sislokCDM100 = pkt.bongkaranCdm[0];
                q.sislokCDM50 = pkt.bongkaranCdm[1];
                q.sislokCDM20 = pkt.bongkaranCdm[2];
                q.sislokCRM100 = pkt.bongkaranCrm[0];
                q.sislokCRM50 = pkt.bongkaranCrm[1];
                q.sislokCRM20 = pkt.bongkaranCrm[2];

                Int64 selisih100 = pkt.saldoAkhirHitungan[0] - (Int64)q.saldoAkhir100;
                Int64 selisih50 = pkt.saldoAkhirHitungan[1] - (Int64)q.saldoAkhir50;
                Int64 selisih20 = pkt.saldoAkhirHitungan[2] - (Int64)q.saldoAkhir20;
                Console.WriteLine("Selisih 100: " + selisih100);
                Console.WriteLine("Selisih 50: " + selisih50);
                Console.WriteLine("Selisih 20: " + selisih20);

                q.saldoAkhir100 += selisih100;
                q.saldoAkhir50 += selisih50;
                q.saldoAkhir20 += selisih20;
                foreach (var temp in (from x in db.TransaksiAtms where x.kodePkt == pkt.kodePkt && x.tanggal > pkt.tanggalPengajuan select x).ToList())
                {
                    temp.saldoAwal100 += selisih100;
                    temp.saldoAwal50 += selisih50;
                    temp.saldoAwal20 += selisih20;

                    temp.saldoAkhir100 += selisih100;
                    temp.saldoAkhir50 += selisih50;
                    temp.saldoAkhir20 += selisih20;
                }
                update = true;
            }
            var q2 = (from x in db.LaporanPermintaanBons
                      where x.tanggal > pkt.tanggalPengajuan && x.kodePkt == pkt.kodePkt
                      select x).ToList();
            if (q2.Any())
            {
                for (int a = 0; a < pkt.permintaanBon.Count; a++)
                {
                    if (a >= q2.Count)
                        break;
                    q2[a].C100 = pkt.permintaanBon[a].d100;
                    q2[a].C50 = pkt.permintaanBon[a].d50;
                    q2[a].C20 = pkt.permintaanBon[a].d20;
                }
            }
            var q3 = (from x in db.LaporanPermintaanAdhocs
                      where x.tanggal == pkt.tanggalPengajuan && x.kodePkt == pkt.kodePkt
                      select x).ToList();
            if (q3.Any())
            {
                q3[0].C100 = pkt.permintaanAdhoc[0];
                q3[0].C50 = pkt.permintaanAdhoc[1];
                q3[0].C20 = pkt.permintaanAdhoc[2];
            }

            db.SaveChanges();
            return update;
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
