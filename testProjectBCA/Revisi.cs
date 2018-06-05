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
    public partial class Revisi : Form
    {
        Database1Entities db = new Database1Entities();
        List<List<transaksiPkt>> collectionTransaksiPkt = new List<List<transaksiPkt>>();
        public Revisi()
        {
            InitializeComponent();
        }

        private void selectFileBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Multiselect = true;
            of.Filter = Variables.excelFilter;
            if (of.ShowDialog() == DialogResult.OK)
            {
                foreach(String path in of.FileNames)
                {
                    collectionTransaksiPkt.Add(loadSheetsIntoClassList(Util.openExcel(path)));
                }
            }
            foreach(List<transaksiPkt> list in  collectionTransaksiPkt)
            {
                foreach(transaksiPkt pkt in list)
                {
                    var q = (from x in db.TransaksiAtms where x.kodePkt == pkt.kodePkt && x.tanggal == pkt.tanggalPengajuan select x).FirstOrDefault();
                    if (q!=null)
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

                        Int64 selisih100 = pkt.saldoAkhir[0] - (Int64)q.saldoAkhir100;
                        Int64 selisih50 = pkt.saldoAkhir[1] - (Int64)q.saldoAkhir50;
                        Int64 selisih20 = pkt.saldoAkhir[2] - (Int64)q.saldoAkhir20;
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
                            q2[a].C100 = pkt.permintaanBon[a][0];
                            q2[a].C50 = pkt.permintaanBon[a][1];
                            q2[a].C20 = pkt.permintaanBon[a][2];
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
                }
            }
            MessageBox.Show("Done");
        }
        private List<transaksiPkt> loadSheetsIntoClassList(DataSet data)
        {
            List<transaksiPkt> list = new List<transaksiPkt>();
            for (int x = 0; x < data.Tables.Count; x++)
            {
                var table = data.Tables[x];
                try
                {
                    table.Rows[5][5].ToString();
                    DateTime testanggal = (DateTime)table.Rows[12][5];
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
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

                    pkt.saldoAwalHitungan.Add((Int64)query.saldoAkhir100);
                    pkt.saldoAwalHitungan.Add((Int64)query.saldoAkhir50);
                    pkt.saldoAwalHitungan.Add((Int64)query.saldoAkhir20);
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

                if (pkt.saldoAwalHitungan[0] != pkt.saldoAwal[0] && pkt.saldoAwalHitungan[1] != pkt.saldoAwal[1])
                {
                    MessageBox.Show("Laporan " + pkt.kodePkt + " Tanggal: " + pkt.tanggalPengajuan.ToShortDateString() + " Tidak sesuai");
                }
                list.Add(pkt);
            }
            return list;
        }
    }
}
