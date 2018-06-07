﻿using System;
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

                Console.WriteLine("Saldo Awal Hitungan 100: " + pkt.saldoAwalHitungan[0]);
                Console.WriteLine("Saldo Awal Hitungan 50: " + pkt.saldoAwalHitungan[1]);
                Console.WriteLine("Saldo Awal Hitungan 20: " + pkt.saldoAwalHitungan[2]);

                Console.WriteLine("Pengisian ATM Hitungan 100: " + pkt.pengisianAtm[0]);
                Console.WriteLine("Pengisian ATM Hitungan 50: " + pkt.pengisianAtm[1]);
                Console.WriteLine("Pengisian ATM Hitungan 20: " + pkt.pengisianAtm[2]);

                pkt.hitungSaldoAkhir();

                Console.WriteLine("Saldo Akhir 100 : " + pkt.saldoAkhir[0]);
                Console.WriteLine("Saldo Akhir 50 : " + pkt.saldoAkhir[1]);
                Console.WriteLine("Saldo Akhir 20 : " + pkt.saldoAkhir[2]);

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
