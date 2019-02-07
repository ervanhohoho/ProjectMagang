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
        List<String> files = new List<String>();
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
            var list = (from x in db.TransaksiAtms where x.tanggal == tempTgl select x).OrderBy(x=>x.kodePkt).ToList();
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
            files = new List<String>();
            if(of.ShowDialog() == DialogResult.OK)
            {
                
                collectionTransaksiPkt = new List<List<transaksiPkt>>(); 
                loadForm.ShowSplashScreen();
                String [] tempFileNames = of.FileNames;
                foreach (var temp in tempFileNames)
                    files.Add(temp);

                filesList.Items.Clear();
                foreach (String temp in files)
                {
                    filesList.Items.Add(temp.Substring(temp.LastIndexOf("\\")+1, temp.LastIndexOf(".xls") - temp.LastIndexOf("\\")-1));
                }
                GC.Collect();
                db = new Database1Entities();
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
                checkSheetDates();
                inputIntoCollection();
                loadForm.CloseForm();
                MessageBox.Show("Done!");
                if( MessageBox.Show("Input File?","Reminder", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    inputDataToDB();
                    loadListDataYangSudahAda();
                    loadDataYangBelumAda();
                }
            }
        }
        private void checkSheetDates()
        {
            List<String> toDelete = new List<String>();
            foreach (String temp in files)
            {
                DataSet ds = Util.openExcel(temp);
                DateTime tempDate = new DateTime(1,1,1);
                for(int a=0;a<ds.Tables.Count;a++)
                {
                    Console.WriteLine("ROW COUNT: " + ds.Tables[a].Rows.Count);
                    if(ds.Tables[a].Columns.Count<12 || !ds.Tables[a].Rows[53][1].ToString().Contains("Approval") )
                    {
                        MessageBox.Show(temp + " Sheet " + ds.Tables[a].TableName + " Format excel salah\nSheet berikutnya tetap diproses.");
                        continue;
                    }
                    String sDate = ds.Tables[a].Rows[12][5].ToString();
                    if(a==0)
                    {
                        try {
                            tempDate = Convert.ToDateTime(sDate);
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(temp + " Sheet: " + ds.Tables[a].TableName + " Format excel salah\nSheet berikutnya tetap diproses.");
                        }
                    }
                    else
                    {
                        DateTime tempDate2 = new DateTime(1,1,1);
                        try
                        {
                            tempDate2 = Convert.ToDateTime(sDate);
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(temp + " Sheet: " + ds.Tables[a].TableName + " Bermasalah");
                        }
                        if (tempDate2 != tempDate.AddDays(1))
                        {
                            toDelete.Add(temp);
                            MessageBox.Show("File " + temp + " Tidak lengkap/tidak urut tanggalnya!\nData tidak dimasukkan","Warning");
                            break;
                        }
                        tempDate = Convert.ToDateTime(sDate); 
                    }
                }
            }
            foreach(String temp in toDelete)
            {
                files.Remove(temp);
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
                if (table.Rows.Count < 10 || table.Columns.Count < 12)
                    continue;
                if(String.IsNullOrEmpty(table.Rows[22][5].ToString()) && x == data.Tables.Count-1)
                {
                    MessageBox.Show("Sheet " + table.TableName + " Tidak ada bon yang disetujui!");
                    break;
                }

                try
                {
                    //tanggal pengajuan
                    DateTime testanggal = (DateTime)table.Rows[12][5];
                }
                catch (Exception e)
                {
                    MessageBox.Show("Data Laporan " + data.DataSetName + " Sheet " + table.TableName + " tidak sesuai format");
                    continue;
                }

                Console.WriteLine(table.TableName);
                transaksiPkt pkt = new transaksiPkt();
                //Kode Pkt
                pkt.kodePkt = table.Rows[5][5].ToString();
                if(db.Pkts.Where(z=>z.kodePkt == pkt.kodePkt).FirstOrDefault() == null)
                {
                    MessageBox.Show("Sheet " + table.TableName + " Kode PKT tidak ada di master PKT");
                    continue;
                }
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
                        pkt.saldoAwal.Add((Int64) Double.Parse(table.Rows[12][6 + a].ToString()));
                    else
                        pkt.saldoAwal.Add(0);
                }
                //Pengambilan setor uang dari excel
                for (int a = 0; a < 4; a++)
                {
                    if (table.Rows[14][6 + a].ToString() != "0" && table.Rows[14][6 + a].ToString() != "")
                        pkt.setorUang.Add((Int64) Double.Parse(table.Rows[14][6 + a].ToString()));
                    else
                        pkt.setorUang.Add(0);
                }
                //Pengambilan penerimaan bon dari excel
                for (int a = 0; a < 4; a++)
                {
                    if (table.Rows[15][6 + a].ToString() != "0" && table.Rows[15][6 + a].ToString() != "")
                        pkt.penerimaanBon.Add((Int64) Double.Parse(table.Rows[15][6 + a].ToString()));
                    else
                        pkt.penerimaanBon.Add(0);
                }
                //Pengambilan penerimaan bon adhoc dari excel
                for (int a = 0; a < 4; a++)
                {
                    if (table.Rows[16][6 + a].ToString() != "0" && table.Rows[16][6 + a].ToString() != "")
                        pkt.penerimaanBonAdhoc.Add((Int64) Double.Parse(table.Rows[16][6 + a].ToString()));
                    else
                        pkt.penerimaanBonAdhoc.Add(0);
                }
                //Pengambilan pengisian atm dari excel
                for (int a = 0; a < 4; a++)
                {
                    if (table.Rows[17][6 + a].ToString() != "0" && table.Rows[17][6 + a].ToString() != "")
                        pkt.pengisianAtm.Add((Int64) Double.Parse(table.Rows[17][6 + a].ToString()));
                    else
                        pkt.pengisianAtm.Add(0);
                }
                //Pengambilan pengisian crm dari excel
                for (int a = 0; a < 4; a++)
                {
                    if (table.Rows[18][6 + a].ToString() != "0" && table.Rows[18][6 + a].ToString() != "")
                        pkt.pengisianCrm.Add((Int64) Double.Parse(table.Rows[18][6 + a].ToString()));
                    else
                        pkt.pengisianCrm.Add(0);
                }
                //Pengambilan bongkaran ATM dari excel
                for (int a = 0; a < 4; a++)
                {
                    if (table.Rows[19][6 + a].ToString() != "0" && table.Rows[19][6 + a].ToString() != "")
                        pkt.bongkaranAtm.Add((Int64) Double.Parse(table.Rows[19][6 + a].ToString()));
                    else
                        pkt.bongkaranAtm.Add(0);
                }
                //Pengambilan bongkaran cdm dari excel
                for (int a = 0; a < 4; a++)
                {
                    if (table.Rows[20][6 + a].ToString() != "0" && table.Rows[20][6 + a].ToString() != "")
                        pkt.bongkaranCdm.Add((Int64) Double.Parse(table.Rows[20][6 + a].ToString()));
                    else
                        pkt.bongkaranCdm.Add(0);
                }
                //Pengambilan bongkaran crm dari excel
                for (int a = 0; a < 4; a++)
                {
                    if (table.Rows[21][6 + a].ToString() != "0" && table.Rows[21][6 + a].ToString() != "")
                        pkt.bongkaranCrm.Add((Int64) Double.Parse(table.Rows[21][6 + a].ToString()));
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
                    Int64 d100, d50, d20, buf1;
                    if (String.IsNullOrEmpty(tanggalE) || !DateTime.TryParse(tanggalE, out tanggal))
                    {
                        //Kalo Gaada Tanggal
                    }
                    else
                    {
                        //Tanggal
                        tanggal = Convert.ToDateTime(tanggalE);

                        //Denom 100.000
                        if (!String.IsNullOrEmpty(row[6].ToString()))
                        {
                            if (Int64.TryParse(d100E, out buf1))
                                d100 = buf1;
                            else
                                d100 = 0;
                        }
                        else
                            d100 = 0;

                        //Denom 50.000
                        if (!String.IsNullOrEmpty(row[7].ToString()))
                        {
                            if (Int64.TryParse(d50E, out buf1))
                                d50 = buf1;
                            else
                                d50 = 0;
                        }
                        else
                            d50 = 0;

                        //Denom 20.000
                        if (!String.IsNullOrEmpty(row[8].ToString()))
                        {
                            if (Int64.TryParse(d20E, out buf1))
                                d20 = buf1;
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
                }
                //Pengambilan saldo akhir dari excel
                for (int a = 0; a < 4; a++)
                {
                    if (table.Rows[38][6 + a].ToString() != "0" && table.Rows[32][6 + a].ToString() != "")
                        pkt.saldoAkhir.Add((Int64) Double.Parse(table.Rows[32][6 + a].ToString()));
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

                    if (String.IsNullOrEmpty(tanggalE) || !DateTime.TryParse(tanggalE, out tanggal))
                    {
                        //Kalo Gaada Tanggal
                    }
                    else
                    {
                        Console.WriteLine(table.TableName + " Permintaan Bon" + tanggalE + " 100: " + d100E + " 50: " + d50E + " 20: " + d20E);
                        Int64 d100, d50, d20, buf1;

                        //Tanggal
                        tanggal = Convert.ToDateTime(tanggalE);

                        //Denom 100.000
                        if (!String.IsNullOrEmpty(row[6].ToString()))
                        {
                            if (Int64.TryParse(d100E, out buf1))
                                d100 = buf1;
                            else
                                d100 = 0;
                        }
                        else
                            d100 = 0;

                        //Denom 50.000
                        if (!String.IsNullOrEmpty(row[7].ToString()))
                        {
                            if (Int64.TryParse(d50E, out buf1))
                                d50 = buf1;
                            else
                                d50 = 0;
                        }
                        else
                            d50 = 0;

                        //Denom 20.000
                        if (!String.IsNullOrEmpty(row[8].ToString()))
                        {
                            if (Int64.TryParse(d20E, out buf1))
                                d20 = buf1;
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
                }

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

                pkt.hitungSaldoAkhir();



                //Bagian Checking 
                String errMsg = "";
                String summary = "";
                bool isError = false;
                if (pkt.saldoAwalHitungan[0] != pkt.saldoAwal[0] || pkt.saldoAwalHitungan[1] != pkt.saldoAwal[1] || pkt.saldoAwalHitungan[2] != pkt.saldoAwal[2])
                {
                    isError = true;
                    errMsg+="Saldo awal tidak sesuai\n=====================\n"+
                        "\nHitungan 100: Rp. " + pkt.saldoAwalHitungan[0].ToString("n0") + "\nLaporan 100: Rp. " + pkt.saldoAwal[0].ToString("n0") +
                        "\nHitungan 50: Rp. " + pkt.saldoAwalHitungan[1].ToString("n0") + "\nLaporan 50: Rp. " + pkt.saldoAwal[1].ToString("n0") +
                        "\nHitungan 20: Rp. " + pkt.saldoAwalHitungan[2].ToString("n0") + "\nLaporan 20: Rp." + pkt.saldoAwal[2].ToString("n0");
                    if (pkt.saldoAwalHitungan[0] != pkt.saldoAwal[0])
                        summary += "\nSaldo Awal 100 " + (pkt.saldoAwalHitungan[0] - pkt.saldoAwal[0]).ToString("n0");
                    if (pkt.saldoAwalHitungan[1] != pkt.saldoAwal[1])
                        summary += "\nSaldo Awal 50 " + (pkt.saldoAwalHitungan[1] - pkt.saldoAwal[1]).ToString("n0");
                    if (pkt.saldoAwalHitungan[2] != pkt.saldoAwal[2])
                        summary += "\nSaldo Awal 20 " + (pkt.saldoAwalHitungan[2] - pkt.saldoAwal[2]).ToString("n0");
                }

                var queryApproval = (from a in db.Approvals
                                     join da in db.DetailApprovals on a.idApproval equals da.idApproval
                                     where a.kodePkt == pkt.kodePkt && pkt.tanggalPengajuan == da.tanggal
                                     && da.bon100 != -1
                                     orderby da.idDetailApproval
                                     select new { Approval = a, DetailApproval = da }).ToList();
                if (queryApproval.Count>0 && queryApproval != null)
                {
                    if(queryApproval[queryApproval.Count-1].DetailApproval.bon100 != pkt.penerimaanBon[0] || queryApproval[queryApproval.Count - 1].DetailApproval.bon50 != pkt.penerimaanBon[1] || queryApproval[queryApproval.Count - 1].DetailApproval.bon20 != pkt.penerimaanBon[2])
                    {
                        isError = true;
                        errMsg += "\nBon menurut approval\n=====================" +
                            "\nApproval 100: Rp. " + ((Int64)queryApproval[queryApproval.Count - 1].DetailApproval.bon100).ToString("n0") + "\nLaporan 100: Rp. " + pkt.penerimaanBon[0].ToString("n0") +
                            "\nApproval 50: Rp. " + ((Int64)queryApproval[queryApproval.Count - 1].DetailApproval.bon50).ToString("n0") + "\nLaporan 50: Rp. " + pkt.penerimaanBon[1].ToString("n0") +
                            "\nApproval 20: Rp. " + ((Int64)queryApproval[queryApproval.Count - 1].DetailApproval.bon20).ToString("n0") + "\nLaporan 20: Rp." + pkt.penerimaanBon[2].ToString("n0");
                        if (queryApproval[queryApproval.Count - 1].DetailApproval.bon100 != pkt.penerimaanBon[0])
                            summary += "\nBon 100 " + ((Int64) queryApproval[queryApproval.Count - 1].DetailApproval.bon100 - (Int64)pkt.penerimaanBon[0]).ToString("n0");
                        if (queryApproval[queryApproval.Count - 1].DetailApproval.bon50 != pkt.penerimaanBon[1])
                            summary += "\nBon 50 " + ((Int64)queryApproval[queryApproval.Count - 1].DetailApproval.bon50 - (Int64)pkt.penerimaanBon[1]).ToString("n0");
                        if (queryApproval[queryApproval.Count - 1].DetailApproval.bon20 != pkt.penerimaanBon[2])
                            summary += "\nBon 20 " + ((Int64)queryApproval[queryApproval.Count - 1].DetailApproval.bon20 - (Int64)pkt.penerimaanBon[2]).ToString("n0");
                    }
                    
                }

                queryApproval = (from a in db.Approvals
                                 join da in db.DetailApprovals on a.idApproval equals da.idApproval
                                 where a.kodePkt == pkt.kodePkt
                                 && da.bon100 != -1
                                 orderby da.idDetailApproval
                                 select new { Approval = a, DetailApproval = da }).ToList();
                if(queryApproval.Any())
                {
                    foreach(var bonygdisetujui in pkt.bonAtmYangDisetujui)
                    {
                        var qa = queryApproval.Where(a => a.DetailApproval.tanggal == bonygdisetujui.tgl).OrderByDescending(a => a.DetailApproval.idDetailApproval).Select(a => a.DetailApproval).FirstOrDefault();
                        if(qa!=null)
                        {
                            if(qa.bon100 != bonygdisetujui.d100 || qa.bon50 != bonygdisetujui.d50 || qa.bon20 != bonygdisetujui.d20)
                            {
                                isError = true;
                                errMsg += "\nBon tanggal " + ((DateTime)qa.tanggal).ToShortDateString() + " yang disetujui beda\n=====================" +
                                    "\nApproval 100: Rp. " + ((Int64)qa.bon100).ToString("n0") + "\nLaporan 100: Rp. " + bonygdisetujui.d100.ToString("n0") +
                                    "\nApproval 50: Rp. " + ((Int64)qa.bon50).ToString("n0") + "\nLaporan 50: Rp. " + bonygdisetujui.d50.ToString("n0") +
                                    "\nApproval 20: Rp. " + ((Int64)qa.bon20).ToString("n0") + "\nLaporan 20: Rp. " + bonygdisetujui.d20.ToString("n0");
                            }
                            if (qa.bon100 != bonygdisetujui.d100)
                                summary += "\nSelisih bon tanggal " + ((DateTime)qa.tanggal).ToShortDateString() + " yang disetujui 100: " + ((Int64)qa.bon100 - (Int64)bonygdisetujui.d100);
                            if (qa.bon50 != bonygdisetujui.d50)
                                summary += "\nSelisih bon tanggal " + ((DateTime)qa.tanggal).ToShortDateString() + " yang disetujui 50: " + ((Int64)qa.bon50 - (Int64)bonygdisetujui.d50);
                            if (qa.bon20 != bonygdisetujui.d20)
                                summary += "\nSelisih bon tanggal " + ((DateTime)qa.tanggal).ToShortDateString() + " yang disetujui 20: " + ((Int64)qa.bon20 - (Int64)bonygdisetujui.d20);
                        }
                    }
                }

                var queryApprovalAdhoc = (from a in db.Approvals.AsEnumerable()
                                          join da in db.DetailApprovals.AsEnumerable() on a.idApproval equals da.idApproval
                                          where a.kodePkt == pkt.kodePkt && da.tanggal == pkt.tanggalPengajuan.Date
                                          orderby a.idApproval
                                          select new { DetailApproval = da }).ToList();
                if (queryApprovalAdhoc.Any())
                {
                    var tempQueryApprovalAdhoc = queryApprovalAdhoc[queryApprovalAdhoc.Count - 1];

                    if (tempQueryApprovalAdhoc.DetailApproval.adhoc100 != null)
                    {
                        if (tempQueryApprovalAdhoc.DetailApproval.adhoc100 != pkt.penerimaanBonAdhoc[0] || tempQueryApprovalAdhoc.DetailApproval.adhoc50 != pkt.penerimaanBonAdhoc[1] || tempQueryApprovalAdhoc.DetailApproval.adhoc20 != pkt.penerimaanBonAdhoc[2])
                        {
                            isError = true;
                            errMsg += "\nAdhoc menurut Approval\n===================="
                            + "\n Approval Adhoc 100: " + ((Int64)tempQueryApprovalAdhoc.DetailApproval.adhoc100 + " Laporan Adhoc 100: " + ((Int64)pkt.penerimaanBonAdhoc[0]))
                            + "\n Approval Adhoc 50: " + ((Int64)tempQueryApprovalAdhoc.DetailApproval.adhoc50 + " Laporan Adhoc 50: " + ((Int64)pkt.penerimaanBonAdhoc[1]))
                            + "\n Approval Adhoc 20: " + ((Int64)tempQueryApprovalAdhoc.DetailApproval.adhoc20 + " Laporan Adhoc 20: " + ((Int64)pkt.penerimaanBonAdhoc[2]))
                            ;
                            if (tempQueryApprovalAdhoc.DetailApproval.adhoc100 != pkt.penerimaanBonAdhoc[0])
                                summary += "\nAdhoc 100 tidak sesuai";
                            if (tempQueryApprovalAdhoc.DetailApproval.adhoc50 != pkt.penerimaanBonAdhoc[1])
                                summary += "\nAdhoc 50 tidak sesuai";
                            if (tempQueryApprovalAdhoc.DetailApproval.adhoc20 != pkt.penerimaanBonAdhoc[2])
                                summary += "\nAdhoc 20 tidak sesuai";
                        }
                    }
                }

                Int64 setor100=0, setor50=0, setor20=0;
                //Check Setor
                var queryApprovalSetor = (from a in db.Approvals.AsEnumerable()
                                          join da in db.DetailApprovals.AsEnumerable() on a.idApproval equals da.idApproval
                                          where a.kodePkt == pkt.kodePkt && da.tanggal == pkt.tanggalPengajuan
                                          && (da.setor100 > 0 || da.setor50 > 0 || da.setor20 > 0)
                                          orderby da.idDetailApproval ascending
                                          select new { DetailApproval = da }).ToList();
                if (queryApprovalSetor.Any())
                {
                    var tempQueryApprovalSetor = queryApprovalSetor[queryApprovalSetor.Count - 1];
                    if (tempQueryApprovalSetor.DetailApproval.setor100 != null)
                    {
                        setor100 = (Int64)tempQueryApprovalSetor.DetailApproval.setor100;
                        setor50 = (Int64)tempQueryApprovalSetor.DetailApproval.setor50;
                        setor20 = (Int64)tempQueryApprovalSetor.DetailApproval.setor20;
                    }
                }


                var queryApprovalSetorAdhoc = (from a in db.Approvals.AsEnumerable()
                                          join da in db.DetailApprovals.AsEnumerable() on a.idApproval equals da.idApproval
                                          where a.kodePkt == pkt.kodePkt && da.tanggal == pkt.tanggalPengajuan.Date
                                          orderby a.idApproval
                                          select new { DetailApproval = da }).ToList();
                if (queryApprovalSetorAdhoc.Any())
                {
                    var tempQueryApprovalSetorAdhoc = queryApprovalSetorAdhoc[queryApprovalSetorAdhoc.Count - 1];

                    if (tempQueryApprovalSetorAdhoc.DetailApproval.adhoc100 != null)
                    {
                        if (tempQueryApprovalSetorAdhoc.DetailApproval.setorAdhoc100 + setor100 != pkt.setorUang[0] || tempQueryApprovalSetorAdhoc.DetailApproval.setorAdhoc50 + setor50 != pkt.setorUang[1] || tempQueryApprovalSetorAdhoc.DetailApproval.setorAdhoc20 + setor20 != pkt.setorUang[2])
                        {
                            isError = true;
                            errMsg += "\nJumlah Setor menurut Approval\n===================="
                            + "\n Approval Setor + Setor Adhoc 100 : " + ((Int64)tempQueryApprovalSetorAdhoc.DetailApproval.setorAdhoc100 + setor100).ToString("N0") + " Laporan Setor Adhoc 100: " + ((Int64)pkt.setorUang[0]).ToString("N0")
                            + "\n Approval Setor + Setor Adhoc 50: " + ((Int64)tempQueryApprovalSetorAdhoc.DetailApproval.setorAdhoc50 + setor50).ToString("N0") + " Laporan Setor Adhoc 50: " + ((Int64)pkt.setorUang[1]).ToString("N0")
                            + "\n Approval Setor + Setor Adhoc 20: " + ((Int64)tempQueryApprovalSetorAdhoc.DetailApproval.setorAdhoc20 + setor20).ToString("N0") + " Laporan Setor Adhoc 20: " + ((Int64)pkt.setorUang[2]).ToString("N0")
                            ;
                            if (tempQueryApprovalSetorAdhoc.DetailApproval.setor100 + setor100 != pkt.setorUang[0])
                                summary += "\nSetor 100 tidak sesuai";
                            if (tempQueryApprovalSetorAdhoc.DetailApproval.setor50  + setor50 != pkt.setorUang[1])
                                summary += "\nSetor 50 tidak sesuai";
                            if (tempQueryApprovalSetorAdhoc.DetailApproval.setor20  + setor20 != pkt.setorUang[2])
                                summary += "\nSetor 20 tidak sesuai";
                        }
                    }
                }

                if (isError)
                {
                    String errMsg2 = "Laporan " + pkt.kodePkt + " Tanggal: " + pkt.tanggalPengajuan.ToShortDateString() + " Tidak sesuai\n";
                    errMsg2 += errMsg;
                    errMsg2 += "\n\nSummary\n======================" + summary;
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
