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
        }

        private void SelectFilesButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Multiselect = true;
            of.Filter = "Microsoft Excel | *.xls; *xlsx; *xlsm";
            if(of.ShowDialog() == DialogResult.OK)
            {
                files = of.FileNames;
                foreach (String temp in files)
                {
                    filesList.Items.Add(temp.Substring(temp.LastIndexOf("\\")+1, temp.LastIndexOf(".xls") - temp.LastIndexOf("\\")-1));
                }
                inputIntoCollection();
                MessageBox.Show("Done!");
            }
        }
        private void inputIntoCollection()
        {
            foreach(String path in files)
            {
                collectionTransaksiPkt.Add(loadSheetsIntoClassList(Util.openExcel(path)));
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

                //Pengambilan data hitungan dari db
                if (x == 0 && (from q in db.TransaksiAtms where q.kodePkt == pkt.kodePkt select q).FirstOrDefault() != null)
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
                        MessageBox.Show("Data tidak akan dimasukkan karena record hari sebelumnya tidak ada");
                        continue;
                    }
                }
                else
                {
                    if (x == 0)
                        pkt.saldoAwalHitungan = pkt.saldoAwal;
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
                for (int i = 0; i < 7; i++)
                {
                    List<Int64> tempList = new List<Int64>();
                    if ((table.Rows[22 + i][6].ToString() == "" || table.Rows[22 + i][6].ToString() == "0") &&
                        (table.Rows[22 + i][7].ToString() == "" || table.Rows[22 + i][7].ToString() == "0") &&
                        (table.Rows[22 + i][8].ToString() == "" || table.Rows[22 + i][8].ToString() == "0"))
                        continue;
                    for (int j = 0; j < 4; j++)
                    {
                        if (table.Rows[22 + i][6 + j].ToString() != "0" && table.Rows[22 + i][6 + j].ToString() != "")
                            tempList.Add(Int64.Parse(table.Rows[22 + i][6 + j].ToString()));
                        else
                            tempList.Add(0);
                    }
                    pkt.bonAtmYangDisetujui.Add(tempList);
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
                    if ((table.Rows[34 + i][6].ToString() == "" || table.Rows[34 + i][6].ToString() == "0") &&
                        (table.Rows[34 + i][7].ToString() == "" || table.Rows[34 + i][7].ToString() == "0") &&
                        (table.Rows[34 + i][8].ToString() == "" || table.Rows[34 + i][8].ToString() == "0"))
                        continue;
                    for (int j = 0; j < 4; j++)
                    {
                        if (table.Rows[34 + i][6 + j].ToString() != "0" && table.Rows[34 + i][6 + j].ToString() != "")
                            tempList.Add(Int64.Parse(table.Rows[34 + i][6 + j].ToString()));
                        else
                            tempList.Add(0);
                    }
                    pkt.permintaanBon.Add(tempList);
                }

                pkt.hitungSaldoAkhir();

                if (pkt.saldoAwalHitungan[0] != pkt.saldoAwal[0] || pkt.saldoAwalHitungan[1] != pkt.saldoAwal[1] || pkt.saldoAwalHitungan[2] != pkt.saldoAwal[2])
                {
                    MessageBox.Show("Laporan " + pkt.kodePkt + " Tanggal: " + pkt.tanggalPengajuan.ToShortDateString() + " Tidak sesuai"+
                        "\nSaldo awal hitungan 100: Rp. " + pkt.saldoAwalHitungan[0] + "\nSaldo awal laporan 100: Rp. " + pkt.saldoAwal[0] +
                        "\nSaldo awal hitungan 50: Rp. " + pkt.saldoAwalHitungan[1] + "\nSaldo awal laporan 50: Rp. " + pkt.saldoAwal[1] +
                        "\nSaldo awal hitungan 20: Rp. " + pkt.saldoAwalHitungan[2] + "\nSaldo awal laporan 20: Rp." + pkt.saldoAwal[2] +
                        "\nSaldo akhir hitungan 100: Rp. " + pkt.saldoAkhirHitungan[0] + "\nSaldo akhir laporan 100: Rp. " + pkt.saldoAkhir[0] +
                        "\nSaldo akhir hitungan 50: Rp. " + pkt.saldoAkhirHitungan[1] + "\nSaldo akhir laporan 50: Rp. " + pkt.saldoAkhir[1] +
                        "\nSaldo akhir hitungan 20: Rp. " + pkt.saldoAkhirHitungan[2] + "\nSaldo akhir laporan 20: Rp." + pkt.saldoAkhir[2], "Warning!");
                }

                list.Add(pkt);
            }
            return list;
        }

        private void inputBtn_Click(object sender, EventArgs e)
        {
            inputDataToDB();
        }
    }
}
