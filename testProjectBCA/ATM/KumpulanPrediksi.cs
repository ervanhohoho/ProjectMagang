using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testProjectBCA.ATM
{
    public class KumpulanPrediksi
    {
        public List<PktDenom> sislokCrm { set; get; }
        public List<PktDenom> sislokCrmDenganStdDeviasi { set; get; }
        public List<PktDenom> sislokCdm { set; get; }
        public List<PktDenom> sislokCdmDenganStdDeviasi { set; get; }
        public List<PktRasio> rasioSislokAtm { set; get; } 
        public List<PktRasio> rasioSislokATMDenganStdDeviasi { set; get; }
        public List<PktDenom> isiCrm2 { set; get; }
        public List<PktDenom> isiCrmDenganStdDeviasi { set; get; }
        public List<PktDenom> prediksiIsiAtmOpti { set; get; }
        public List<PktDenom> prediksiIsiAtm { set; get; }//dalem class denom ada 100,50,20
        public List<PktDenom> prediksiIsiAtmDenganStdDeviasi { set; get; }
        public Denom loadSaldoAwal(String kodepkt)
        {
            Denom saldoAwal = new Denom();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    cmd.CommandText = "SELECT saldoAkhir100, saldoAkhir50, saldoAkhir20, tanggal FROM TransaksiAtms WHERE kodePkt = '" + kodepkt + "' AND tanggal >= '" + tanggalOptiMin.AddDays(-1).ToShortDateString() + "'";
                    sql.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        saldoAwal.d100 = (Int64)reader[0];
                        saldoAwal.d50 = (Int64)reader[1];
                        saldoAwal.d20 = (Int64)reader[2];
                        saldoAwal.tgl = ((DateTime)reader[3]).AddDays(1);
                    }
                    Console.WriteLine("Saldo Awal " + kodepkt);
                    Console.WriteLine(saldoAwal.d100 + " " + saldoAwal.d50 + " " + saldoAwal.d20);
                }
            }
            return saldoAwal;
        }
        List<Denom> loadBon(String kodepkt)
        {
            List<Denom> bon = new List<Denom>();
            int jumlahBonLaporan = 0;
            Database1Entities db = new Database1Entities();

            
            var query = (from a in db.Approvals
                         join da in db.DetailApprovals on a.idApproval equals da.idApproval
                         where a.kodePkt == kodepkt
                         select new { Approval = a, DetailApproval = da }).ToList();
            if (!query.Any())
            {
                bon.Add(new Denom() { tgl = Variables.todayDate, d100 = 0, d20 = 0, d50 = 0 });
                return bon;
            }
            int maxIdApproval = query.Max(x => x.Approval.idApproval);
            Console.WriteLine("Max Id Approval = " + maxIdApproval);
            query = query.Where(x => x.Approval.idApproval == maxIdApproval).ToList();
            if (query.Any())
            {
                bon = (from x in query
                       where x.DetailApproval.tanggal >= tanggalOptiMin
                       && x.DetailApproval.bon100 != -1
                       select new Denom()
                       {
                           tgl = (DateTime)x.DetailApproval.tanggal,
                           d100 = (Int64)x.DetailApproval.bon100,
                           d50 = (Int64)x.DetailApproval.bon50,
                           d20 = (Int64)x.DetailApproval.bon20,
                       }).ToList();
                if (!bon.Any())
                {
                    bon.Add(new Denom() { tgl = Variables.todayDate, d100 = 0, d20 = 0, d50 = 0 });
                }
                jumlahBonLaporan = bon.Count;
            }
            if (!bon.Any())
            {
                bon.Add(new Denom() { tgl = DateTime.Today, d100 = 0, d20 = 0, d50 = 0 });
            }
            Console.WriteLine("Bon yang disetujui");
            foreach (var temp in bon)
            {
                Console.WriteLine("Tgl: " + temp.tgl + " 100: " + temp.d100 + " 50: " + temp.d50 + " 20: " + temp.d20);
            }
            return bon;
        }
        List<Denom> loadSetor(String kodepkt)
        {
            Database1Entities db = new Database1Entities();
            List<Denom> setor = new List<Denom>();
            DateTime today = DateTime.Today.Date;
            Denom temp = new Denom();
            var q2 = (from x in db.Approvals
                      join y in db.DetailApprovals on x.idApproval equals y.idApproval
                      where x.kodePkt == kodepkt && (y.tanggal >= tanggalOptiMin) && ((DateTime)x.tanggal) < today
                      select new { Approval = x, DetailApproval = y }).ToList();
            int maxIdApproval = 0;
            if (q2.Any())
                maxIdApproval = q2.Max(x => x.Approval.idApproval);
            q2 = (from x in q2
                  where x.Approval.idApproval == maxIdApproval
                  select x).ToList();
            foreach (var s in q2)
            {
                temp = new Denom();
                if (s.DetailApproval.setor100 == null)
                    temp.d100 = 0;
                else
                    temp.d100 = (Int64)s.DetailApproval.setor100;
                if (s.DetailApproval.setor100 == null)
                    temp.d50 = 0;
                else
                    temp.d50 = (Int64)s.DetailApproval.setor50;
                if (s.DetailApproval.setor100 == null)
                    temp.d20 = 0;
                else
                    temp.d20 = (Int64)s.DetailApproval.setor20;
                temp.tgl = (DateTime)s.DetailApproval.tanggal;
                setor.Add(temp);
            }
            return setor;
        }
        List<Denom> loadRekomendasiBonNonE2E(String kodepkt, Double targetRasio100, Double targetRasio50, Double targetRasio20)
        {
            int counter = 0;
            int setorCounter = 0;
            //Disini hari h dianggap hari terakhir ada laporan bon
            Console.WriteLine();
            Console.WriteLine("Load rekomendasi bon");
            Console.WriteLine("======================");
            List<Denom> rekomendasiBon = new List<Denom>();
            List<Denom> saldoAwalIdeal = new List<Denom>();
            Denom rekomendasiAdhoc = new Denom();
            List<Denom> bon = loadBon(kodepkt);
            List<Denom> setor = loadSetor(kodepkt);
            int jumlahBonLaporan = bon.Count;
            Denom saldoAwal = loadSaldoAwal(kodepkt);
            Denom saldoAkhirH = new Denom();
            List<Denom> prediksiIsiAtm = this.prediksiIsiAtm.Where(x => x.kodePkt == kodepkt).Select(x => new Denom() { tgl = x.tanggal, d100 = x.d100, d50 = x.d50, d20 = x.d20 }).ToList();
            List<Rasio> rasioSislokAtm = this.rasioSislokAtm.Where(x => x.kodePkt == kodepkt).Select(x => new Rasio() { tgl = x.tanggal, d100 = x.d100, d50 = x.d50, d20 = x.d20 }).ToList();
            List<Denom> isiCrm = this.isiCrm2.Where(x => x.kodePkt == kodepkt).Select(x => new Denom() { tgl = x.tanggal, d100 = x.d100, d50 = x.d50, d20 = x.d20 }).ToList();
            List<Denom> sislokCdm = this.sislokCdm.Where(x => x.kodePkt == kodepkt).Select(x => new Denom() { tgl = x.tanggal, d100 = x.d100, d50 = x.d50, d20 = x.d20 }).ToList();
            List<Denom> sislokCrm = this.sislokCrm.Where(x => x.kodePkt == kodepkt).Select(x => new Denom() { tgl = x.tanggal, d100 = x.d100, d50 = x.d50, d20 = x.d20 }).ToList();

            Console.WriteLine("Target Rasio 100: " + targetRasio100);
            Console.WriteLine("Target Rasio 50: " + targetRasio50);
            Console.WriteLine("Target Rasio 20: " + targetRasio20);
            Console.WriteLine("Saldo Awal");
            Console.WriteLine(saldoAwal.d100 + " " + saldoAwal.d50 + " " + saldoAwal.d20);

            //Hitung saldo akhir hari h untuk jadi saldo awal h+1
            saldoAkhirH.d100 = saldoAwal.d100 + (Int64)Math.Round((rasioSislokAtm[0].d100 * prediksiIsiAtm[0].d100)) + sislokCdm[0].d100 + sislokCrm[0].d100 - prediksiIsiAtm[0].d100 - isiCrm[0].d100 + bon[0].d100;
            saldoAkhirH.d50 = saldoAwal.d50 + (Int64)Math.Round((rasioSislokAtm[0].d50 * prediksiIsiAtm[0].d50)) + sislokCdm[0].d50 + sislokCrm[0].d50 - prediksiIsiAtm[0].d50 - isiCrm[0].d50 + bon[0].d50;
            saldoAkhirH.d20 = saldoAwal.d20 + (Int64)Math.Round((rasioSislokAtm[0].d20 * prediksiIsiAtm[0].d20)) + sislokCdm[0].d20 + sislokCrm[0].d20 - prediksiIsiAtm[0].d20 - isiCrm[0].d20 + bon[0].d20;
            saldoAkhirH.tgl = tanggalOptiMin;


            if (setorCounter < setor.Count && saldoAkhirH.tgl == setor[setorCounter].tgl)
            {
                saldoAkhirH.d100 -= setor[setorCounter].d100;
                saldoAkhirH.d50 -= setor[setorCounter].d50;
                saldoAkhirH.d20 -= setor[setorCounter++].d20;
            }

            ////Rekomendasi Adhoc
            //rekomendasiAdhoc.d100 = 0;
            //rekomendasiAdhoc.d50 = 0;
            //rekomendasiAdhoc.d20 = 0;
            //if (saldoAwal.d100 - prediksiIsiAtm[0].d100 - isiCrm[0].d100 + bon[0].d100 < 0)
            //{
            //    rekomendasiAdhoc.d100 = -(saldoAwal.d100 - prediksiIsiAtm[0].d100 - isiCrm[0].d100 + bon[0].d100);
            //    Double rasio = (saldoAkhirH.d100 + rekomendasiAdhoc.d100) / ((Double)prediksiIsiAtm[1].d100 + (Double)isiCrm[1].d100); // Saldo akhir hari h dibagi dengan prediksi isi atm h+1
            //    rasio100Lbl.Text = Math.Round(rasio, 2).ToString();
            //}
            //if (saldoAwal.d50 - prediksiIsiAtm[0].d50 - isiCrm[0].d50 + bon[0].d50 < 0)
            //{
            //    rekomendasiAdhoc.d50 = -(saldoAwal.d50 - prediksiIsiAtm[0].d50 - isiCrm[0].d50 + bon[0].d50);
            //    Double rasio = (saldoAkhirH.d50 + rekomendasiAdhoc.d50) / ((Double)prediksiIsiAtm[1].d50 + (Double)isiCrm[1].d100); // Saldo akhir hari h dibagi dengan prediksi isi atm h+1
            //    rasio50Lbl.Text = Math.Round(rasio, 2).ToString();
            //}
            //if (saldoAwal.d20 - prediksiIsiAtm[0].d20 - isiCrm[0].d20 + bon[0].d20 < 0)
            //{
            //    rekomendasiAdhoc.d20 = -(saldoAwal.d20 - prediksiIsiAtm[0].d20 - isiCrm[0].d20 + bon[0].d20);
            //    Double rasio = (saldoAkhirH.d20 + rekomendasiAdhoc.d20) / ((Double)prediksiIsiAtm[1].d20 + (Double)isiCrm[1].d100); // Saldo akhir hari h dibagi dengan prediksi isi atm h+1
            //    rasio20Lbl.Text = Math.Round(rasio, 2).ToString();
            //}

            //Kalau ternyata di laporan bonnya lebih dari 1 hitung sampe hari terakhir ada bon yang disetujui
            for (int a = 1; a < jumlahBonLaporan; a++)
            {
                //Hitung saldo akhir hari h
                saldoAkhirH.tgl = tanggalOptiMin.AddDays(a);
                saldoAkhirH.d100 = saldoAkhirH.d100 + (Int64)Math.Round((rasioSislokAtm[a].d100 * prediksiIsiAtm[a].d100)) + sislokCdm[a].d100 + sislokCrm[a].d100 - prediksiIsiAtm[a].d100 - isiCrm[a].d100 + bon[a].d100;
                saldoAkhirH.d50 = saldoAkhirH.d50 + (Int64)Math.Round((rasioSislokAtm[a].d50 * prediksiIsiAtm[a].d50)) + sislokCdm[a].d50 + sislokCrm[a].d50 - prediksiIsiAtm[a].d50 - isiCrm[a].d50 + bon[a].d50;
                saldoAkhirH.d20 = saldoAkhirH.d20 + (Int64)Math.Round((rasioSislokAtm[a].d20 * prediksiIsiAtm[a].d20)) + sislokCdm[a].d20 + sislokCrm[a].d20 - prediksiIsiAtm[a].d20 - isiCrm[a].d20 + bon[a].d20;


                if (setorCounter < setor.Count && saldoAkhirH.tgl == setor[setorCounter].tgl)
                {
                    saldoAkhirH.d100 -= setor[setorCounter].d100;
                    saldoAkhirH.d50 -= setor[setorCounter].d50;
                    saldoAkhirH.d20 -= setor[setorCounter++].d20;
                }
            }



            //Hitung saldo ideal h+2, h+3, ..
            for (int a = 1 + jumlahBonLaporan; a < prediksiIsiAtm.Count; a++)
            {
                Denom temp = new Denom();
                temp.d100 = (Int64)Math.Round(((Double)prediksiIsiAtm[a].d100 + (Double)isiCrm2[a].d100) * targetRasio100);
                temp.d50 = (Int64)Math.Round(((Double)prediksiIsiAtm[a].d50 + (Double)isiCrm2[a].d50) * targetRasio50);
                temp.d20 = (Int64)Math.Round(((Double)prediksiIsiAtm[a].d20 + (Double)isiCrm2[a].d20) * targetRasio20);
                temp.tgl = prediksiIsiAtm[a].tgl;
                saldoAwalIdeal.Add(temp);
            }

            Console.WriteLine("Saldo awal ideal\n===============");
            foreach (var temp in saldoAwalIdeal)
            {
                Console.WriteLine(temp.tgl + " " + temp.d100 + " " + temp.d50 + " " + temp.d20);
            }
            //MessageBox.Show("Jumlah Bon Laporan: " + jumlahBonLaporan);

            //Ambil saldo akhir ideal h+1 dari saldo awal ideal h+2
            //MessageBox.Show("Jumlah Bon Laporan: "+jumlahBonLaporan);
            Denom saldoAkhirH1Ideal = new Denom();
            saldoAkhirH1Ideal.d100 = saldoAwalIdeal[0].d100;
            saldoAkhirH1Ideal.d50 = saldoAwalIdeal[0].d50;
            saldoAkhirH1Ideal.d20 = saldoAwalIdeal[0].d20;

            Console.WriteLine("Saldo Akhir Hari H");
            Console.WriteLine(saldoAkhirH.tgl + " " + saldoAkhirH.d100 + " " + saldoAkhirH.d50 + " " + saldoAkhirH.d20);

            //Hitung rekomendasiBon untuk h+1 (Belom ada setor dan adhoc)
            Denom tempRekomendasiBon = new Denom();


            //Ini untuk hitung rekomendasi bon H+2 (rekomendasi pertama)
            tempRekomendasiBon.d100 = saldoAkhirH1Ideal.d100
                - saldoAkhirH.d100                                                      //Ambil saldo akhir di hari h (jadi saldo awal h+1)
                - (Int64)Math.Round((rasioSislokAtm[jumlahBonLaporan].d100 * prediksiIsiAtm[jumlahBonLaporan].d100))
                - sislokCrm[jumlahBonLaporan].d100
                - sislokCdm[jumlahBonLaporan].d100
                + prediksiIsiAtm[jumlahBonLaporan].d100
                + isiCrm2[jumlahBonLaporan].d100;

            tempRekomendasiBon.d50 = saldoAkhirH1Ideal.d50
                - saldoAkhirH.d50
                - (Int64)Math.Round((rasioSislokAtm[jumlahBonLaporan].d50 * prediksiIsiAtm[jumlahBonLaporan].d50))
                - sislokCrm[jumlahBonLaporan].d50
                - sislokCdm[jumlahBonLaporan].d50
                + prediksiIsiAtm[jumlahBonLaporan].d50
                + isiCrm2[jumlahBonLaporan].d50;

            tempRekomendasiBon.d20 = saldoAkhirH1Ideal.d20
                - saldoAkhirH.d20
                - (Int64)Math.Round((rasioSislokAtm[jumlahBonLaporan].d20 * prediksiIsiAtm[jumlahBonLaporan].d20))
                - sislokCrm[jumlahBonLaporan].d20
                - sislokCdm[jumlahBonLaporan].d20
                + prediksiIsiAtm[jumlahBonLaporan].d20
                + isiCrm2[jumlahBonLaporan].d20;

            tempRekomendasiBon.tgl = tanggalOptiMin.AddDays(jumlahBonLaporan);


            if (setorCounter < setor.Count && tempRekomendasiBon.tgl == setor[setorCounter].tgl)
            {
                tempRekomendasiBon.d100 += setor[setorCounter].d100;
                tempRekomendasiBon.d50 += setor[setorCounter].d50;
                tempRekomendasiBon.d20 += setor[setorCounter++].d20;
            }

            rekomendasiBon.Add(tempRekomendasiBon);

            counter = jumlahBonLaporan + 1;
            //Hitung rekomendasiBon h+3 keatas
            for (int a = 0; a < saldoAwalIdeal.Count - 1; a++)
            {
                tempRekomendasiBon = new Denom();
                Denom saldoAkhirIdeal = saldoAwalIdeal[a + 1];
                //Denom saldoSementara = new Denom();`
                //saldoSementara.d100 = saldoAwalIdeal[a].d100 - isiCrm[counter].d100;
                tempRekomendasiBon.d100 = saldoAwalIdeal[a + 1].d100                                 //Saldo akhir h+1 
                    - saldoAwalIdeal[a].d100                                                      //saldo awal hari h
                    - (Int64)Math.Round((rasioSislokAtm[counter + a].d100 * prediksiIsiAtm[counter + a].d100))
                    - sislokCrm[counter + a].d100
                    - sislokCdm[counter + a].d100
                    + prediksiIsiAtm[counter + a].d100
                    + isiCrm2[counter + a].d100;
                tempRekomendasiBon.d50 = saldoAwalIdeal[a + 1].d50                                 //Saldo akhir h+1 
                    - saldoAwalIdeal[a].d50                                                      //saldo awal hari h
                    - (Int64)Math.Round((rasioSislokAtm[counter + a].d50 * prediksiIsiAtm[counter + a].d50))
                    - sislokCrm[counter + a].d50
                    - sislokCdm[counter + a].d50
                    + prediksiIsiAtm[counter + a].d50
                    + isiCrm2[counter + a].d50;
                tempRekomendasiBon.d20 = saldoAwalIdeal[a + 1].d20                                 //Saldo akhir h+1 
                    - saldoAwalIdeal[a].d20                                                      //saldo awal hari h
                    - (Int64)Math.Round((rasioSislokAtm[counter + a].d20 * prediksiIsiAtm[counter + a].d20))
                    - sislokCrm[counter + a].d20
                    - sislokCdm[counter + a].d20
                    + prediksiIsiAtm[counter + a].d20
                    + isiCrm2[counter + a].d20;
                tempRekomendasiBon.tgl = tanggalOptiMin.AddDays(counter + a);

                if (setorCounter < setor.Count && tempRekomendasiBon.tgl == setor[setorCounter].tgl)
                {
                    tempRekomendasiBon.d100 += setor[setorCounter].d100;
                    tempRekomendasiBon.d50 += setor[setorCounter].d50;
                    tempRekomendasiBon.d20 += setor[setorCounter++].d20;
                }

                rekomendasiBon.Add(tempRekomendasiBon);
                //counter++;
            }
            return rekomendasiBon;
        }
        List<Denom> loadRekomendasiBonE2E(String kodepkt, Double targetRasio100, Double targetRasio50, Double targetRasio20)
        {

            int counter = 0;
            int setorCounter = 0;
            Console.WriteLine();
            Console.WriteLine("Load rekomendasi bon");
            Console.WriteLine("======================");
            List<Denom> rekomendasiBonNonE2E = new List<Denom>();
            List<Denom> saldoAwalIdeal = new List<Denom>();
            List<Denom> rekomendasiBon = new List<Denom>();

            Denom rekomendasiAdhoc = new Denom();
            List<Denom> bon = loadBon(kodepkt);
            List<Denom> setor = loadSetor(kodepkt);
            int jumlahBonLaporan = bon.Count;
            Denom saldoAwal = loadSaldoAwal(kodepkt);
            Denom saldoAkhirH = new Denom();
            List<Denom> prediksiIsiAtm = this.prediksiIsiAtm.Where(x => x.kodePkt == kodepkt).Select(x => new Denom() { tgl = x.tanggal, d100 = x.d100, d50 = x.d50, d20 = x.d20 }).ToList();
            List<Rasio> rasioSislokAtm = this.rasioSislokAtm.Where(x => x.kodePkt == kodepkt).Select(x => new Rasio() { tgl = x.tanggal, d100 = x.d100, d50 = x.d50, d20 = x.d20 }).ToList();
            List<Denom> isiCrm = this.isiCrm2.Where(x => x.kodePkt == kodepkt).Select(x => new Denom() { tgl = x.tanggal, d100 = x.d100, d50 = x.d50, d20 = x.d20 }).ToList();
            List<Denom> sislokCdm = this.sislokCdm.Where(x => x.kodePkt == kodepkt).Select(x => new Denom() { tgl = x.tanggal, d100 = x.d100, d50 = x.d50, d20 = x.d20 }).ToList();
            List<Denom> sislokCrm = this.sislokCrm.Where(x => x.kodePkt == kodepkt).Select(x => new Denom() { tgl = x.tanggal, d100 = x.d100, d50 = x.d50, d20 = x.d20 }).ToList();



            Console.WriteLine("Saldo Awal");
            Console.WriteLine(saldoAwal.d100 + " " + saldoAwal.d50 + " " + saldoAwal.d20);

            /**********************************************/
            /*********START OF ITUNGAN BON NON E2E*********/
            /**********************************************/

            Console.WriteLine("Sislok ATM 100: " + (rasioSislokAtm[0].d100 * prediksiIsiAtm[0].d100));
            Console.WriteLine("Sislok CDM 100: " + (sislokCdm[0].d100));
            Console.WriteLine("Sislok CRM 100: " + (sislokCrm[0].d100));
            Console.WriteLine("Isi ATM 100: " + (prediksiIsiAtm[0].d100));
            Console.WriteLine("Isi CRM 100: " + (isiCrm[0].d100));
            Console.WriteLine("Bon 100: " + (bon[0].d100));

            //Hitung saldo akhir hari h untuk jadi saldo awal h+1
            saldoAkhirH.d100 = saldoAwal.d100 + (Int64)Math.Round((rasioSislokAtm[0].d100 * prediksiIsiAtm[0].d100)) + sislokCdm[0].d100 + sislokCrm[0].d100 - prediksiIsiAtm[0].d100 - isiCrm[0].d100 + bon[0].d100;
            saldoAkhirH.d50 = saldoAwal.d50 + (Int64)Math.Round((rasioSislokAtm[0].d50 * prediksiIsiAtm[0].d50)) + sislokCdm[0].d50 + sislokCrm[0].d50 - prediksiIsiAtm[0].d50 - isiCrm[0].d50 + bon[0].d50;
            saldoAkhirH.d20 = saldoAwal.d20 + (Int64)Math.Round((rasioSislokAtm[0].d20 * prediksiIsiAtm[0].d20)) + sislokCdm[0].d20 + sislokCrm[0].d20 - prediksiIsiAtm[0].d20 - isiCrm[0].d20 + bon[0].d20;
            saldoAkhirH.tgl = tanggalOptiMin;


            if (setorCounter < setor.Count && saldoAkhirH.tgl == setor[setorCounter].tgl)
            {
                saldoAkhirH.d100 -= setor[setorCounter].d100;
                saldoAkhirH.d50 -= setor[setorCounter].d50;
                saldoAkhirH.d20 -= setor[setorCounter++].d20;
            }

            //Kalau ternyata di laporan bonnya lebih dari 1 hitung sampe hari terakhir ada bon yang disetujui
            for (int a = 1; a < jumlahBonLaporan; a++)
            {
                //Hitung saldo akhir hari h
                saldoAkhirH.tgl = tanggalOptiMin.AddDays(a);
                saldoAkhirH.d100 = saldoAkhirH.d100 + (Int64)Math.Round((rasioSislokAtm[a].d100 * prediksiIsiAtm[a].d100)) + sislokCdm[a].d100 + sislokCrm[a].d100 - prediksiIsiAtm[a].d100 - isiCrm[a].d100 + bon[a].d100;
                saldoAkhirH.d50 = saldoAkhirH.d50 + (Int64)Math.Round((rasioSislokAtm[a].d50 * prediksiIsiAtm[a].d50)) + sislokCdm[a].d50 + sislokCrm[a].d50 - prediksiIsiAtm[a].d50 - isiCrm[a].d50 + bon[a].d50;
                saldoAkhirH.d20 = saldoAkhirH.d20 + (Int64)Math.Round((rasioSislokAtm[a].d20 * prediksiIsiAtm[a].d20)) + sislokCdm[a].d20 + sislokCrm[a].d20 - prediksiIsiAtm[a].d20 - isiCrm[a].d20 + bon[a].d20;


                if (setorCounter < setor.Count && saldoAkhirH.tgl == setor[setorCounter].tgl)
                {
                    saldoAkhirH.d100 -= setor[setorCounter].d100;
                    saldoAkhirH.d50 -= setor[setorCounter].d50;
                    saldoAkhirH.d20 -= setor[setorCounter++].d20;
                }
            }



            //Hitung saldo ideal h+2, h+3, ..
            for (int a = 2 + jumlahBonLaporan - 1; a < prediksiIsiAtm.Count; a++)
            {
                Denom temp = new Denom();
                temp.d100 = (Int64)Math.Round(((Double)prediksiIsiAtm[a].d100 + (Double)isiCrm[a].d100) * targetRasio100);
                temp.d50 = (Int64)Math.Round(((Double)prediksiIsiAtm[a].d50 + (Double)isiCrm[a].d50) * targetRasio50);
                temp.d20 = (Int64)Math.Round(((Double)prediksiIsiAtm[a].d20 + (Double)isiCrm[a].d20) * targetRasio20);
                temp.tgl = prediksiIsiAtm[a].tgl;
                saldoAwalIdeal.Add(temp);
            }

            //MessageBox.Show("Jumlah Bon Laporan: " + jumlahBonLaporan);

            //Ambil saldo akhir ideal h+1 dari saldo awal ideal h+2
            //MessageBox.Show("Jumlah Bon Laporan: "+jumlahBonLaporan);
            Denom saldoAkhirH1Ideal = new Denom();
            saldoAkhirH1Ideal.d100 = saldoAwalIdeal[0].d100;
            saldoAkhirH1Ideal.d50 = saldoAwalIdeal[0].d50;
            saldoAkhirH1Ideal.d20 = saldoAwalIdeal[0].d20;

            //Hitung rekomendasiBonNonE2E untuk h+1 (Belom ada setor dan adhoc)
            Denom temprekomendasiBonNonE2E = new Denom();


            //Ini untuk hitung rekomendasi bon H+2 (rekomendasi pertama)
            temprekomendasiBonNonE2E.d100 = saldoAkhirH1Ideal.d100
                - saldoAkhirH.d100                                                      //Ambil saldo akhir di hari h (jadi saldo awal h+1)
                - (Int64)Math.Round((rasioSislokAtm[jumlahBonLaporan].d100 * prediksiIsiAtm[jumlahBonLaporan].d100))
                - sislokCrm[jumlahBonLaporan].d100
                - sislokCdm[jumlahBonLaporan].d100
                + prediksiIsiAtm[jumlahBonLaporan].d100
                + isiCrm[jumlahBonLaporan].d100;

            temprekomendasiBonNonE2E.d50 = saldoAkhirH1Ideal.d50
                - saldoAkhirH.d50
                - (Int64)Math.Round((rasioSislokAtm[jumlahBonLaporan].d50 * prediksiIsiAtm[jumlahBonLaporan].d50))
                - sislokCrm[jumlahBonLaporan].d50
                - sislokCdm[jumlahBonLaporan].d50
                + prediksiIsiAtm[jumlahBonLaporan].d50
                + isiCrm[jumlahBonLaporan].d50;

            temprekomendasiBonNonE2E.d20 = saldoAkhirH1Ideal.d20
                - saldoAkhirH.d20
                - (Int64)Math.Round((rasioSislokAtm[jumlahBonLaporan].d20 * prediksiIsiAtm[jumlahBonLaporan].d20))
                - sislokCrm[jumlahBonLaporan].d20
                - sislokCdm[jumlahBonLaporan].d20
                + prediksiIsiAtm[jumlahBonLaporan].d20
                + isiCrm[jumlahBonLaporan].d20;

            temprekomendasiBonNonE2E.tgl = tanggalOptiMin.AddDays(1 + jumlahBonLaporan - 1);

            if (setorCounter < setor.Count && temprekomendasiBonNonE2E.tgl == setor[setorCounter].tgl)
            {
                temprekomendasiBonNonE2E.d100 += setor[setorCounter].d100;
                temprekomendasiBonNonE2E.d50 += setor[setorCounter].d50;
                temprekomendasiBonNonE2E.d20 += setor[setorCounter++].d20;
            }

            rekomendasiBonNonE2E.Add(temprekomendasiBonNonE2E);

            counter = jumlahBonLaporan + 1;
            //Hitung rekomendasiBonNonE2E h+3 keatas
            for (int a = 0; a < saldoAwalIdeal.Count - 1; a++)
            {
                temprekomendasiBonNonE2E = new Denom();
                Denom saldoAkhirIdeal = saldoAwalIdeal[a + 1];
                //Denom saldoSementara = new Denom();`
                //saldoSementara.d100 = saldoAwalIdeal[a].d100 - isiCrm[counter].d100;
                temprekomendasiBonNonE2E.d100 = saldoAwalIdeal[a + 1].d100                                 //Saldo akhir h+1 
                    - saldoAwalIdeal[a].d100                                                      //saldo awal hari h
                    - (Int64)Math.Round((rasioSislokAtm[counter + a].d100 * prediksiIsiAtm[counter + a].d100))
                    - sislokCrm[counter + a].d100
                    - sislokCdm[counter + a].d100
                    + prediksiIsiAtm[counter + a].d100
                    + isiCrm[counter + a].d100;
                temprekomendasiBonNonE2E.d50 = saldoAwalIdeal[a + 1].d50                                 //Saldo akhir h+1 
                    - saldoAwalIdeal[a].d50                                                      //saldo awal hari h
                    - (Int64)Math.Round((rasioSislokAtm[counter + a].d50 * prediksiIsiAtm[counter + a].d50))
                    - sislokCrm[counter + a].d50
                    - sislokCdm[counter + a].d50
                    + prediksiIsiAtm[counter + a].d50
                    + isiCrm[counter + a].d50;
                temprekomendasiBonNonE2E.d20 = saldoAwalIdeal[a + 1].d20                                 //Saldo akhir h+1 
                    - saldoAwalIdeal[a].d20                                                      //saldo awal hari h
                    - (Int64)Math.Round((rasioSislokAtm[counter + a].d20 * prediksiIsiAtm[counter + a].d20))
                    - sislokCrm[counter + a].d20
                    - sislokCdm[counter + a].d20
                    + prediksiIsiAtm[counter + a].d20
                    + isiCrm[counter + a].d20;
                temprekomendasiBonNonE2E.tgl = tanggalOptiMin.AddDays(counter + a);

                if (setorCounter < setor.Count && temprekomendasiBonNonE2E.tgl == setor[setorCounter].tgl)
                {
                    temprekomendasiBonNonE2E.d100 += setor[setorCounter].d100;
                    temprekomendasiBonNonE2E.d50 += setor[setorCounter].d50;
                    temprekomendasiBonNonE2E.d20 += setor[setorCounter++].d20;
                }

                rekomendasiBonNonE2E.Add(temprekomendasiBonNonE2E);
                //counter++;
            }
            Console.WriteLine("Rekomendasi Bon");
            foreach (var temp in rekomendasiBonNonE2E)
            {
                Console.WriteLine(temp.tgl + " " + temp.d100 + " " + temp.d50 + " " + temp.d20);
            }
            /************************************************/
            /************END OF ITUNGAN BON NON E2E**********/
            /***********************************************/



            /**********************************************/
            /*********START OF ITUNGAN BON E2E*************/
            /**********************************************/

            List<Denom> rekomendasiBonE2E = new List<Denom>();
            //Hitungan untuk bon

            //Hitung saldo akhir hari h untuk jadi saldo awal h+1
            saldoAkhirH.d100 = saldoAwal.d100 + (Int64)Math.Round((rasioSislokAtm[0].d100 * prediksiIsiAtm[0].d100)) + sislokCdm[0].d100 + sislokCrm[0].d100 - prediksiIsiAtm[0].d100 - isiCrm[0].d100 + bon[0].d100;
            saldoAkhirH.d50 = saldoAwal.d50 + (Int64)Math.Round((rasioSislokAtm[0].d50 * prediksiIsiAtm[0].d50)) + sislokCdm[0].d50 + sislokCrm[0].d50 - prediksiIsiAtm[0].d50 - isiCrm[0].d50 + bon[0].d50;
            saldoAkhirH.d20 = saldoAwal.d20 + (Int64)Math.Round((rasioSislokAtm[0].d20 * prediksiIsiAtm[0].d20)) + sislokCdm[0].d20 + sislokCrm[0].d20 - prediksiIsiAtm[0].d20 - isiCrm[0].d20 + bon[0].d20;
            saldoAkhirH.tgl = tanggalOptiMin;


            if (setorCounter < setor.Count && saldoAkhirH.tgl == setor[setorCounter].tgl)
            {
                saldoAkhirH.d100 -= setor[setorCounter].d100;
                saldoAkhirH.d50 -= setor[setorCounter].d50;
                saldoAkhirH.d20 -= setor[setorCounter++].d20;


            }

            //Kalau ternyata di laporan bonnya lebih dari 1 hitung sampe hari terakhir ada bon yang disetujui
            for (int a = 1; a < jumlahBonLaporan; a++)
            {
                //Hitung saldo akhir hari h
                saldoAkhirH.tgl = tanggalOptiMin.AddDays(a);
                saldoAkhirH.d100 = saldoAkhirH.d100 + (Int64)Math.Round((rasioSislokAtm[a].d100 * prediksiIsiAtm[a].d100)) + sislokCdm[a].d100 + sislokCrm[a].d100 - prediksiIsiAtm[a].d100 - isiCrm[a].d100 + bon[a].d100;
                saldoAkhirH.d50 = saldoAkhirH.d50 + (Int64)Math.Round((rasioSislokAtm[a].d50 * prediksiIsiAtm[a].d50)) + sislokCdm[a].d50 + sislokCrm[a].d50 - prediksiIsiAtm[a].d50 - isiCrm[a].d50 + bon[a].d50;
                saldoAkhirH.d20 = saldoAkhirH.d20 + (Int64)Math.Round((rasioSislokAtm[a].d20 * prediksiIsiAtm[a].d20)) + sislokCdm[a].d20 + sislokCrm[a].d20 - prediksiIsiAtm[a].d20 - isiCrm[a].d20 + bon[a].d20;


                if (setorCounter < setor.Count && saldoAkhirH.tgl == setor[setorCounter].tgl)
                {
                    saldoAkhirH.d100 -= setor[setorCounter].d100;
                    saldoAkhirH.d50 -= setor[setorCounter].d50;
                    saldoAkhirH.d20 -= setor[setorCounter++].d20;
                }
            }
            for (int a = jumlahBonLaporan; a < prediksiIsiAtm.Count - 1; a++)
            {
                Denom tempRek = new Denom();
                tempRek.tgl = tanggalOptiMin.AddDays(a);
                tempRek.d100 = (prediksiIsiAtm[a].d100 + isiCrm[a].d100) - (saldoAkhirH.d100);
                tempRek.d50 = (prediksiIsiAtm[a].d50 + isiCrm[a].d50) - (saldoAkhirH.d50);
                tempRek.d20 = (prediksiIsiAtm[a].d20 + isiCrm[a].d20) - (saldoAkhirH.d20);
                if (setorCounter < setor.Count)
                {
                    tempRek.d100 += setor[setorCounter].d100;
                    tempRek.d50 += setor[setorCounter].d50;
                    tempRek.d20 += setor[setorCounter++].d20;
                }
                rekomendasiBonE2E.Add(tempRek);
                saldoAkhirH.d100 = saldoAkhirH.d100 + (Int64)Math.Round((rasioSislokAtm[a].d100 * prediksiIsiAtm[a].d100)) + sislokCdm[a].d100 + sislokCrm[a].d100 - prediksiIsiAtm[a].d100 - isiCrm[a].d100 + tempRek.d100;
                saldoAkhirH.d50 = saldoAkhirH.d50 + (Int64)Math.Round((rasioSislokAtm[a].d50 * prediksiIsiAtm[a].d50)) + sislokCdm[a].d50 + sislokCrm[a].d50 - prediksiIsiAtm[a].d50 - isiCrm[a].d50 + tempRek.d50;
                saldoAkhirH.d20 = saldoAkhirH.d20 + (Int64)Math.Round((rasioSislokAtm[a].d20 * prediksiIsiAtm[a].d20)) + sislokCdm[a].d20 + sislokCrm[a].d20 - prediksiIsiAtm[a].d20 - isiCrm[a].d20 + tempRek.d20;
                if (setorCounter < setor.Count)
                {
                    saldoAkhirH.d100 -= setor[setorCounter - 1].d100;
                    saldoAkhirH.d50 -= setor[setorCounter - 1].d50;
                    saldoAkhirH.d20 -= setor[setorCounter - 1].d20;
                }
            }

            //Cek rasio hari terakhir
            bool flag100rasiounder = false, flag50rasiounder = false, flag20rasiounder = false;
            if (Math.Round((Double)saldoAkhirH.d100 / (prediksiIsiAtm[prediksiIsiAtm.Count - 1].d100 + isiCrm[prediksiIsiAtm.Count - 1].d100), 2) <= targetRasio100)
            {
                Console.WriteLine();
                Console.WriteLine((Double)saldoAkhirH.d100 / (prediksiIsiAtm[prediksiIsiAtm.Count - 1].d100 + isiCrm[prediksiIsiAtm.Count - 1].d100));
                flag100rasiounder = true;
            }
            if (Math.Round((Double)saldoAkhirH.d50 / (prediksiIsiAtm[prediksiIsiAtm.Count - 1].d50 + isiCrm[prediksiIsiAtm.Count - 1].d50), 2) <= targetRasio50)
            {
                Console.WriteLine((Double)saldoAkhirH.d50 / (prediksiIsiAtm[prediksiIsiAtm.Count - 1].d50 + isiCrm[prediksiIsiAtm.Count - 1].d50));
                flag50rasiounder = true;
            }
            if (Math.Round((Double)saldoAkhirH.d20 / (prediksiIsiAtm[prediksiIsiAtm.Count - 1].d20 + isiCrm[prediksiIsiAtm.Count - 1].d20), 2) <= targetRasio20)
            {
                Console.WriteLine((Double)saldoAkhirH.d20 / (prediksiIsiAtm[prediksiIsiAtm.Count - 1].d20 + isiCrm[prediksiIsiAtm.Count - 1].d20));
                flag20rasiounder = true;
            }

            if (flag20rasiounder && flag50rasiounder && flag100rasiounder)
            {
                rekomendasiBon = rekomendasiBonNonE2E;
            }
            if (!flag20rasiounder && flag50rasiounder && flag100rasiounder)
            {
                rekomendasiBon = new List<Denom>();
                int idx = 0;
                foreach (var temp in rekomendasiBonNonE2E)
                {
                    Denom temp2 = new Denom();
                    temp2.tgl = temp.tgl;
                    temp2.d20 = rekomendasiBonE2E[idx++].d20;
                    temp2.d50 = temp.d50;
                    temp2.d100 = temp.d100;
                    rekomendasiBon.Add(temp2);
                }
            }
            if (flag20rasiounder && !flag50rasiounder && flag100rasiounder)
            {
                rekomendasiBon = new List<Denom>();
                int idx = 0;
                foreach (var temp in rekomendasiBonNonE2E)
                {
                    Denom temp2 = new Denom();
                    temp2.tgl = temp.tgl;
                    temp2.d50 = rekomendasiBonE2E[idx++].d50;
                    temp2.d20 = temp.d20;
                    temp2.d100 = temp.d100;
                    rekomendasiBon.Add(temp2);
                }
            }
            if (flag20rasiounder && flag50rasiounder && !flag100rasiounder)
            {
                rekomendasiBon = new List<Denom>();
                int idx = 0;
                foreach (var temp in rekomendasiBonNonE2E)
                {
                    Denom temp2 = new Denom();
                    temp2.tgl = temp.tgl;
                    temp2.d100 = rekomendasiBonE2E[idx++].d100;
                    temp2.d50 = temp.d50;
                    temp2.d20 = temp.d20;
                    rekomendasiBon.Add(temp2);
                }
            }
            if (flag20rasiounder && !flag50rasiounder && !flag100rasiounder)
            {
                rekomendasiBon = new List<Denom>();
                int idx = 0;
                foreach (var temp in rekomendasiBonE2E)
                {
                    Denom temp2 = new Denom();
                    temp2.tgl = temp.tgl;
                    temp2.d20 = rekomendasiBonNonE2E[idx++].d20;
                    temp2.d50 = temp.d50;
                    temp2.d100 = temp.d100;
                    rekomendasiBon.Add(temp2);
                }
            }
            if (!flag20rasiounder && flag50rasiounder && !flag100rasiounder)
            {
                rekomendasiBon = new List<Denom>();
                int idx = 0;
                foreach (var temp in rekomendasiBonE2E)
                {
                    Denom temp2 = new Denom();
                    temp2.tgl = temp.tgl;
                    temp2.d50 = rekomendasiBonNonE2E[idx++].d50;
                    temp2.d20 = temp.d20;
                    temp2.d100 = temp.d100;
                    rekomendasiBon.Add(temp2);
                }
            }
            if (!flag20rasiounder && !flag50rasiounder && flag100rasiounder)
            {
                rekomendasiBon = new List<Denom>();
                int idx = 0;
                foreach (var temp in rekomendasiBonE2E)
                {
                    Denom temp2 = new Denom();
                    temp2.tgl = temp.tgl;
                    temp2.d100 = rekomendasiBonNonE2E[idx++].d100;
                    temp2.d50 = temp.d50;
                    temp2.d20 = temp.d20;
                    rekomendasiBon.Add(temp2);
                }
            }
            if (!flag20rasiounder && !flag50rasiounder && !flag100rasiounder)
            {
                rekomendasiBon = rekomendasiBonE2E;
            }
            return rekomendasiBon;
        }

        public List<PktDenom>loadRekomendasiBon(float targetRasio100, float targetRasio50, float targetRasio20, String e2e)
        {
            List<PktDenom> ret = new List<PktDenom>();
            if(kodePkt.ToLower().Contains("jabo") || kodePkt.ToLower().Contains("kanwil"))
            {

                Database1Entities db = new Database1Entities();
                List<String> listKodePkt = db.Pkts.Where(x => x.kanwil == kodePkt && !x.kodePkt.Contains("CCAS")).Select(x => x.kodePkt).ToList();
                foreach(var temp in listKodePkt)
                {
                    Console.WriteLine("TEMP PKT: " + temp);
                    List<Denom> rekomendasiBon = new List<Denom>();
                    if(e2e.ToLower() == "non e2e")
                        rekomendasiBon = loadRekomendasiBonNonE2E(temp, targetRasio100, targetRasio50, targetRasio20);
                    else if(e2e.ToLower() == "e2e")
                        rekomendasiBon = loadRekomendasiBonE2E(temp, targetRasio100, targetRasio50, targetRasio20);

                    ret.AddRange(rekomendasiBon.Select(x => new PktDenom()
                    {
                        kodePkt = temp,
                        d100 = x.d100,
                        d50 = x.d50,
                        d20 = x.d20,
                        tanggal = x.tgl
                    }).ToList());
                    int maxIdApproval = db.ApprovalViews.Where(x => x.kodePkt == temp).Max(x => x.idApproval);
                    ret.AddRange(db.ApprovalViews.Where(x => x.idApproval == maxIdApproval && x.tanggalDetailApproval >= tanggalOptiMin && x.tanggalDetailApproval <= tanggalOptiMax && x.bon100 != -1).Select(x=>new PktDenom() { kodePkt = temp, tanggal = (DateTime)x.tanggalDetailApproval, d100 = (Int64)x.bon100, d50 = (Int64)x.bon50, d20 = (Int64)x.bon20 }).ToList());
                }
                return ret;
            }
            else if(kodePkt.ToLower() == "all")
            {
                Database1Entities db = new Database1Entities();
                List<String> listKodePkt = db.Pkts.Where(x=>!x.kodePkt.Contains("CCAS")).Select(x => x.kodePkt).ToList();
                foreach (var temp in listKodePkt)
                {
                    List<Denom> rekomendasiBon = new List<Denom>();
                    if (e2e.ToLower() == "non e2e")
                        rekomendasiBon = loadRekomendasiBonNonE2E(temp, targetRasio100, targetRasio50, targetRasio20);
                    else if (e2e.ToLower() == "e2e")
                        rekomendasiBon = loadRekomendasiBonE2E(temp, targetRasio100, targetRasio50, targetRasio20);
                    ret.AddRange(rekomendasiBon.Select(x => new PktDenom()
                    {
                        kodePkt = temp,
                        d100 = x.d100,
                        d50 = x.d50,
                        d20 = x.d20,
                        tanggal = x.tgl
                    }).ToList());
                    int maxIdApproval = db.ApprovalViews.Where(x => x.kodePkt == temp).Max(x => x.idApproval);
                    ret.AddRange(db.ApprovalViews.Where(x => x.idApproval == maxIdApproval && x.tanggalDetailApproval >= tanggalOptiMin && x.tanggalDetailApproval <= tanggalOptiMax && x.bon100 != -1).Select(x => new PktDenom() { kodePkt = temp, tanggal = (DateTime)x.tanggalDetailApproval, d100 = (Int64)x.bon100, d50 = (Int64)x.bon50, d20 = (Int64)x.bon20 }).ToList());
                }
                return ret;
            }
            else
            {
                Database1Entities db = new Database1Entities();
                List<Denom> rekomendasiBon = new List<Denom>();
                if (e2e.ToLower() == "non e2e")
                    rekomendasiBon = loadRekomendasiBonNonE2E(kodePkt, targetRasio100, targetRasio50, targetRasio20);
                else if (e2e.ToLower() == "e2e")
                    rekomendasiBon = loadRekomendasiBonE2E(kodePkt, targetRasio100, targetRasio50, targetRasio20);

                ret = rekomendasiBon.Select(x => new PktDenom() {
                    kodePkt = kodePkt,
                    d100 = x.d100,
                    d50 = x.d50,
                    d20 = x.d20,
                    tanggal = x.tgl
                }).ToList();
                int maxIdApproval = db.ApprovalViews.Where(x => x.kodePkt == kodePkt).Max(x => x.idApproval);
                
                var tampung = db.ApprovalViews.Where(x => x.idApproval == maxIdApproval && x.tanggalDetailApproval >= tanggalOptiMin && x.tanggalDetailApproval <= tanggalOptiMax && x.bon100 != -1).Select(x => new PktDenom() { kodePkt = x.kodePkt, tanggal = (DateTime)x.tanggalDetailApproval, d100 = (Int64)x.bon100, d50 = (Int64)x.bon50, d20 = (Int64)x.bon20 }).ToList();
                Console.WriteLine("Tampung: " + tampung.Count);
                Console.WriteLine("max id approval: " + maxIdApproval);
                Console.WriteLine("tanggal opti min: " + tanggalOptiMin);
                Console.WriteLine("tanggal opti max: " + tanggalOptiMax);
                ret.AddRange(tampung);
                return ret;
            }
        }
        public List<DateTime> kumpulanTanggal { set; get; }
        public String kodePkt { set; get; }
        public DateTime tanggalOptiMin { set; get; }
        public DateTime tanggalOptiMax { set; get; }
        public String jenisPrediksiIsiATM { set; get; }
        public String jenisPrediksiLainnya { set; get; }
        public String eventWhereCondition { set; get; }
        public List<String> eventType { set; get; }
        public String message { set; get; }
        public bool success { set; get; }
        String pktCondition;
        public KumpulanPrediksi(String kodePkt, List<DateTime> kumpulanTanggal, DateTime tanggalOptiMin, DateTime tanggalOptiMax, String jenisPrediksiIsiATM,String jenisPrediksiLainnya)
        {
            this.kodePkt = kodePkt;
            this.kumpulanTanggal = kumpulanTanggal;
            this.tanggalOptiMin = tanggalOptiMin;
            this.tanggalOptiMax = tanggalOptiMax;
            this.jenisPrediksiIsiATM = jenisPrediksiIsiATM;
            this.jenisPrediksiLainnya = jenisPrediksiLainnya;
            this.eventType = new List<string>();
            sislokCrm = new List<PktDenom>();
            sislokCrmDenganStdDeviasi = new List<PktDenom>();
            sislokCdm = new List<PktDenom>();
            sislokCdmDenganStdDeviasi = new List<PktDenom>();
            rasioSislokATMDenganStdDeviasi = new List<PktRasio>();
            rasioSislokAtm = new List<PktRasio>();
            isiCrm2 = new List<PktDenom>();
            isiCrmDenganStdDeviasi = new List<PktDenom>();
            prediksiIsiAtm = new List<PktDenom>();
            prediksiIsiAtmDenganStdDeviasi = new List<PktDenom>();

            success = true;

            pktCondition = "TA.kodePkt = '" + kodePkt + "'";
            if (kodePkt.ToLower().Contains("kanwil") || kodePkt.ToLower().Contains("jabo"))
                pktCondition = "TA.kanwil = '" + kodePkt + "'";
            if (kodePkt.ToLower() == "all")
                pktCondition = "1=1";


            loadPrediksiOpti();

            
            if (jenisPrediksiIsiATM == "Opti")
            {
                prediksiIsiAtm = prediksiIsiAtmOpti;
            }
            if (jenisPrediksiIsiATM == "Historis")
            {
                loadIsiAtmHistoris();
            }
            if (jenisPrediksiIsiATM == "Opti + Historis")
            {
                loadIsiAtmHistoris();
                for (int a = 0; a < prediksiIsiAtmOpti.Count; a++)
                {
                    prediksiIsiAtm[a].d100 = (prediksiIsiAtm[a].d100 + prediksiIsiAtmOpti[a].d100) / 2;
                    prediksiIsiAtm[a].d50 = (prediksiIsiAtm[a].d50 + prediksiIsiAtmOpti[a].d50) / 2;
                    prediksiIsiAtm[a].d20 = (prediksiIsiAtm[a].d20 + prediksiIsiAtmOpti[a].d20) / 2;
                }
            }
            if (jenisPrediksiIsiATM == "Std Deviasi")
            {
                loadIsiAtmHistoris();
                loadIsiAtmHistorisDenganStandarDeviasi();
                prediksiIsiAtm = prediksiIsiAtmDenganStdDeviasi;
            }
            if (jenisPrediksiIsiATM == "Historis + Std Deviasi")
            {
                loadIsiAtmHistoris();
                loadIsiAtmHistorisDenganStandarDeviasi();
                prediksiIsiAtm = prediksiIsiAtmDenganStdDeviasi;
            }
            if (jenisPrediksiIsiATM == "Opti + Historis + Std Deviasi")
            {
                loadIsiAtmHistoris();
                loadIsiAtmHistorisDenganStandarDeviasi();
                prediksiIsiAtm = prediksiIsiAtmDenganStdDeviasi;
                for (int a = 0; a < prediksiIsiAtmOpti.Count; a++)
                {
                    prediksiIsiAtm[a].d100 = (prediksiIsiAtm[a].d100 + prediksiIsiAtmOpti[a].d100) / 2;
                    prediksiIsiAtm[a].d50 = (prediksiIsiAtm[a].d50 + prediksiIsiAtmOpti[a].d50) / 2;
                    prediksiIsiAtm[a].d20 = (prediksiIsiAtm[a].d20 + prediksiIsiAtmOpti[a].d20) / 2;
                }
            }

            loadSislokCdm();
            loadRasioSislokAtm();
            //loadRasioSislokAtmDenganStdDeviasi();
            loadSislokCrm();
            loadIsiCrm();
            if (jenisPrediksiLainnya == "Std Deviasi")
            {
                loadSislokCdmDenganStdDeviasi();
                loadIsiCrmDenganStdDeviasi();
                loadSislokCrmDenganStdDeviasi();
                loadRasioSislokAtmDenganStdDeviasi();

                Console.WriteLine("SISLOK CDM 100 SEBELUM: " + sislokCdm[0].d100);

                sislokCdm = sislokCdmDenganStdDeviasi;
                Console.WriteLine("SISLOK CDM 100 SESUDAH: " + sislokCdm[0].d100);

                isiCrm2 = isiCrmDenganStdDeviasi;
                sislokCrm = sislokCrmDenganStdDeviasi;
                rasioSislokAtm = rasioSislokATMDenganStdDeviasi;
            }
            if (jenisPrediksiLainnya == "Historis + Std Deviasi")
            {
                loadSislokCdmDenganStdDeviasi();
                loadIsiCrmDenganStdDeviasi();
                loadSislokCrmDenganStdDeviasi();
                loadRasioSislokAtmDenganStdDeviasi();
                for (int a = 0; a < sislokCdm.Count; a++)
                {
                    sislokCdm[a].d100 = (sislokCdm[a].d100 + sislokCdmDenganStdDeviasi[a].d100) / 2;
                    sislokCdm[a].d50 = (sislokCdm[a].d50 + sislokCdmDenganStdDeviasi[a].d50) / 2;
                    sislokCdm[a].d20 = (sislokCdm[a].d20 + sislokCdmDenganStdDeviasi[a].d20) / 2;

                    isiCrm2[a].d100 = (isiCrm2[a].d100 + isiCrmDenganStdDeviasi[a].d100) / 2;
                    isiCrm2[a].d50 = (isiCrm2[a].d50 + isiCrmDenganStdDeviasi[a].d50) / 2;
                    isiCrm2[a].d20 = (isiCrm2[a].d20 + isiCrmDenganStdDeviasi[a].d20) / 2;

                    sislokCrm[a].d100 = (sislokCrm[a].d100 + sislokCrmDenganStdDeviasi[a].d100) / 2;
                    sislokCrm[a].d50 = (sislokCrm[a].d50 + sislokCrmDenganStdDeviasi[a].d50) / 2;
                    sislokCrm[a].d20 = (sislokCrm[a].d20 + sislokCrmDenganStdDeviasi[a].d20) / 2;

                    rasioSislokAtm[a].d100 = (rasioSislokAtm[a].d100 + rasioSislokATMDenganStdDeviasi[a].d100) / 2;
                    rasioSislokAtm[a].d50 = (rasioSislokAtm[a].d50 + rasioSislokATMDenganStdDeviasi[a].d50) / 2;
                    rasioSislokAtm[a].d20 = (rasioSislokAtm[a].d20 + rasioSislokATMDenganStdDeviasi[a].d20) / 2;
                }
            }

            
            eventType = eventType.Distinct().ToList();
        }

        EventAndCondition loadEventWhereCondition(DateTime tanggal)
        {
            int rowcount = prediksiIsiAtmOpti.Count;//dataGridView1.Rows.Count;
                                                    //dataGridView1.Hide();
            DateTime tempDate = tanggal;
            String whereCondition = "";
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    SqlDataReader reader;
                    Denom tempIsiCrm = new Denom();
                    String cText = "SELECT AVG(isiCrm100), AVG(isiCrm50), AVG(isiCrm20) FROM ViewTransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE "+pktCondition+" AND DATENAME(WEEKDAY, TA.Tanggal) = '" + tempDate.DayOfWeek.ToString() + "'";
                    int count = 0;
                    foreach (var tempTgl in kumpulanTanggal)
                    {
                        if (count++ == 0)
                            whereCondition += " AND ((YEAR(TA.tanggal) = " + tempTgl.Year + " AND MONTH(TA.tanggal) = " + tempTgl.Month + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                        else
                            whereCondition += "OR (YEAR(TA.tanggal) = " + tempTgl.Year + " AND MONTH(TA.tanggal) = " + tempTgl.Month + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";

                    }
                    cText += whereCondition;
                    cText += ")";
                    cmd.CommandText = cText;
                    reader = cmd.ExecuteReader();
                    bool event1 = true;
                    bool event2 = true;
                    bool event3 = true;
                    if (reader.Read())
                    {
                        if (String.IsNullOrEmpty(reader[0].ToString()) || String.IsNullOrEmpty(reader[1].ToString()) || String.IsNullOrEmpty(reader[2].ToString()))
                        {
                            event1 = false;
                        }
                        else
                        {
                            reader.Close();
                            eventType.Add("Tanggal " + tempDate.ToShortDateString() + " Menggunakan Event 1");
                            return new EventAndCondition() {eventType = "Event 1", whereCondition = whereCondition };
                        }
                    }
                    if (!event1)
                    {
                        reader.Close();
                        cText = "SELECT AVG(isiCrm100),AVG(isiCrm50), AVG(isiCrm20) FROM ViewTransaksiAtms  TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE " + pktCondition;
                        count = 0;

                        //Reset Where Condition
                        whereCondition = "";
                        foreach (var tempTgl in kumpulanTanggal)
                        {
                            //;
                            //Kalo cari minggu (DATEPART(WEEK, tanggal) - DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM, 0, tanggal), 0)) + 1 ) = (minggu keberapa)
                            if (count++ == 0)
                                whereCondition += " AND ((YEAR(TA.tanggal) = " + tempTgl.Year + " AND MONTH(TA.tanggal) = " + tempTgl.Month + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                            else
                                whereCondition += "OR (YEAR(TA.tanggal) = " + tempTgl.Year + " AND MONTH(TA.tanggal) = " + tempTgl.Month + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "') AND [event] = (SELECT [event] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";

                        }
                        cText += whereCondition;
                        cText += ")";
                        cmd.CommandText = cText;
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            if (String.IsNullOrEmpty(reader[0].ToString()) || String.IsNullOrEmpty(reader[1].ToString()) || String.IsNullOrEmpty(reader[2].ToString()))
                            {
                                event2 = false;
                            }
                            else
                            {
                                reader.Close();
                                eventType.Add("Tanggal " + tempDate.ToShortDateString() + " Menggunakan Event 2");
                                return new EventAndCondition() { eventType = "Event 2",whereCondition = whereCondition };
                            }
                        }
                    }
                    if (!event2)
                    {
                        reader.Close();
                        cText = "SELECT AVG(isiCrm100), AVG(isiCrm50), AVG(isiCrm20) FROM ViewTransaksiAtms  TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE "+pktCondition;
                        count = 0;

                        //Reset Where Condition
                        whereCondition = "";
                        foreach (var tempTgl in kumpulanTanggal)
                        {
                            if (count++ == 0)
                                whereCondition += " AND ((YEAR(TA.tanggal) = " + tempTgl.Year + " AND MONTH(TA.tanggal) = " + tempTgl.Month + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                            else
                                whereCondition += "OR (YEAR(TA.tanggal) = " + tempTgl.Year + " AND MONTH(TA.tanggal) = " + tempTgl.Month + " AND [workDay] = (SELECT [workDay] FROM EventTanggal WHERE tanggal = '" + tempDate.ToShortDateString() + "')) ";
                        }
                        cText += whereCondition;
                        cText += ")";
                        cmd.CommandText = cText;
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            if (String.IsNullOrEmpty(reader[0].ToString()) || String.IsNullOrEmpty(reader[1].ToString()) || String.IsNullOrEmpty(reader[2].ToString()))
                            {
                                event3 = false;
                            }
                            else
                            {
                                reader.Close();
                                eventType.Add("Tanggal " + tempDate.ToShortDateString() + " Menggunakan Event 3");
                                return new EventAndCondition() { eventType = "Event 3", whereCondition = whereCondition };
                            }
                        }
                        else
                        {
                            reader.Close();
                        }
                    }
                    if (!event3)
                    {
                        reader.Close();
                        cText = "SELECT AVG(isiCrm100), AVG(isiCrm50), AVG(isiCrm20) FROM ViewTransaksiAtms  TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE " + pktCondition + " AND DATENAME(WEEKDAY, TA.Tanggal) = '" + tempDate.DayOfWeek.ToString() + "'";;
                        count = 0;

                        //Reset Where Condition
                        whereCondition = "";
                        cText += whereCondition;
                        cmd.CommandText = cText;
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            eventType.Add("Tanggal " + tempDate.ToShortDateString() + " Menggunakan Event 4");
                            return new EventAndCondition() { eventType = "Event 4", whereCondition = whereCondition };
                        }
                        else
                        {
                            reader.Close();
                        }
                    }
                    tempDate = tempDate.AddDays(1);
                }
            }
            return new EventAndCondition();
        }
        List<PktDenom> loadPrediksiHistoris(String jenis)
        {

            DateTime startDate = tanggalOptiMin;//Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = tanggalOptiMax;//Convert.ToDateTime(dataGridView1.Rows[rowCount - 1].Cells[0].Value);
            DateTime tempDate = startDate;
            List<PktDenom> ret = new List<PktDenom>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    while (tempDate <= endDate)
                    {
                        //SislokCrm
                        SqlDataReader reader;
                        PktDenom tempIsiAtm = new PktDenom();
                        String cText = "SELECT AVG(" + jenis + "100), AVG(" + jenis + "50), AVG(" + jenis + "20), kodePkt FROM ViewTransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE "+pktCondition+" AND DATENAME(WEEKDAY, TA.Tanggal) = '" + tempDate.DayOfWeek.ToString() + "'";
                        EventAndCondition eac = loadEventWhereCondition(tempDate);

                        if (eac.eventType != "Event 1" && eac.eventType != "Event 4")
                            cText = "SELECT AVG(" + jenis + "100), AVG(" + jenis + "50), AVG(" + jenis + "20), kodePkt FROM ViewTransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE " + pktCondition;

                        cText += eac.whereCondition;
                        if (eac.eventType != "Event 4")
                            cText += ")";
                        cText += "\nGROUP BY kodePkt";
                        cmd.CommandText = cText;
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            tempIsiAtm = new PktDenom();
                            tempIsiAtm.d50 = (Int64)reader[1];
                            tempIsiAtm.d20 = (Int64)reader[2];
                            tempIsiAtm.tanggal = tempDate;
                            tempIsiAtm.d100 = (Int64)reader[0];
                            tempIsiAtm.kodePkt = reader[3].ToString();
                            ret.Add(tempIsiAtm);
                        }
                        reader.Close();
                        tempDate = tempDate.AddDays(1);
                    }

                }
                sql.Close();
            }
            return ret;
        }
        List<PktDenom> loadPrediksiStdDeviasi(String jenis)
        {
            int rowcount = prediksiIsiAtmOpti.Count;//dataGridView1.Rows.Count;
            DateTime startDate = tanggalOptiMin;//Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = tanggalOptiMax;
            DateTime tempDate = startDate;

            //Load Std Deviasi
            List<PktRasio> stdDeviasi = new List<PktRasio>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    while (tempDate <= endDate)
                    {
                        //SislokAtm
                        SqlDataReader reader;
                        PktRasio tempStdDeviasi = new PktRasio();

                        String kondisi = " WHERE " + pktCondition + " AND DATENAME(WEEKDAY, TA.Tanggal) = '" + tempDate.DayOfWeek.ToString() + "'";
                        int count = 0;

                        EventAndCondition eac = loadEventWhereCondition(tempDate);
                        if (eac.eventType != "Event 1" && eac.eventType != "Event 4")
                        {
                            kondisi = " WHERE " + pktCondition;
                        }
                        kondisi += eac.whereCondition;
                        if (eac.eventType != "Event 4")
                            kondisi += ")";

                        String subqueryTblAverage = "(SELECT ISNULL(AVG(" + jenis + "100),0) AS Average100 , ISNULL(AVG(" + jenis + "50),0) AS Average50 , ISNULL(AVG(" + jenis + "20),0) AS Average20, kodePkt FROM ViewTransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal";
                        subqueryTblAverage += kondisi;
                        subqueryTblAverage += " GROUP BY kodePkt";
                        subqueryTblAverage += ") avt";

                        String query = "";
                         if(jenis.ToLower().Contains("sislok"))
                        {
                           query = "SELECT "
                                    + "[AverageStdDeviasi100] = AVG(CAST((" + jenis + "100 - [Average100]) AS FLOAT) / (CASE WHEN [Average100] = 0 THEN 1 ELSE [Average100] END)), "
                                    + "[AverageStdDeviasi50] = AVG(CAST((" + jenis + "50 - [Average50])AS FLOAT) / (CASE WHEN [Average50] = 0 THEN 1 ELSE [Average50] END)), "
                                    + "[AverageStdDeviasi20] = AVG(CAST((" + jenis + "20 - [Average20])AS FLOAT) / (CASE WHEN [Average20] = 0 THEN 1 ELSE [Average20] END)), "
                                    + "TA.kodePkt "
                                    + "FROM ViewTransaksiAtms TA JOIN EventTanggal ET ON Ta.tanggal = ET.tanggal JOIN " + subqueryTblAverage + " ON avt.kodePkt = TA.kodePkt " + kondisi;
                        }
                        else
                        {
                            query = "SELECT "
                                    + "[AverageStdDeviasi100] = AVG(CAST(ABS(" + jenis + "100 - [Average100]) AS FLOAT) / (CASE WHEN [Average100] = 0 THEN 1 ELSE [Average100] END)), "
                                    + "[AverageStdDeviasi50] = AVG(CAST(ABS(" + jenis + "50 - [Average50])AS FLOAT) / (CASE WHEN [Average50] = 0 THEN 1 ELSE [Average50] END)), "
                                    + "[AverageStdDeviasi20] = AVG(CAST(ABS(" + jenis + "20 - [Average20])AS FLOAT) / (CASE WHEN [Average20] = 0 THEN 1 ELSE [Average20] END)), "
                                    + "TA.kodePkt "
                                    + "FROM ViewTransaksiAtms TA JOIN EventTanggal ET ON Ta.tanggal = ET.tanggal JOIN " + subqueryTblAverage + " ON avt.kodePkt = TA.kodePkt " + kondisi;

                        }
                        query += " GROUP BY TA.kodePkt";
                        cmd.CommandText = query;
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            tempStdDeviasi = new PktRasio();
                            tempStdDeviasi.d100 = (Double)reader[0];
                            tempStdDeviasi.d50 = (Double)reader[1];
                            tempStdDeviasi.d20 = (Double)reader[2];
                            tempStdDeviasi.tanggal = tempDate;
                            tempStdDeviasi.kodePkt = reader[3].ToString();
                            stdDeviasi.Add(tempStdDeviasi);
                        }
                        reader.Close();
                        tempDate = tempDate.AddDays(1);
                    }
                }
                sql.Close();
            }
            List<PktDenom> multiplier = new List<PktDenom>();
            if (jenis.ToLower() == "isiatm")
            {
                multiplier = prediksiIsiAtm;
            }
            if (jenis.ToLower() == "isicrm")
            {
                multiplier = isiCrm2;
            }
            if (jenis.ToLower() == "sislokcrm")
            {
                multiplier = sislokCrm;
            }
            if (jenis.ToLower() == "sislokcdm")
            {
                multiplier = sislokCdm;
            }
            List<PktDenom> ret = new List<PktDenom>();
            for (int a = 0; a < prediksiIsiAtm.Count; a++)
            {
                PktDenom temp = new PktDenom();
                temp.d100 = multiplier[a].d100 + (Int64)Math.Round(stdDeviasi.Where(x=>x.kodePkt == multiplier[a].kodePkt).Select(x=>x.d100).FirstOrDefault() * multiplier[a].d100);
                temp.d50 = multiplier[a].d50 + (Int64)Math.Round(stdDeviasi.Where(x => x.kodePkt == multiplier[a].kodePkt).Select(x => x.d50).FirstOrDefault() * multiplier[a].d50);
                temp.d20 = multiplier[a].d20 + (Int64)Math.Round(stdDeviasi.Where(x => x.kodePkt == multiplier[a].kodePkt).Select(x => x.d20).FirstOrDefault() * multiplier[a].d20);
                temp.kodePkt = stdDeviasi[a].kodePkt;
                temp.tanggal= multiplier[a].tanggal;
                ret.Add(temp);
            }
            return ret;
        }
        void loadPrediksiOpti()
        {
            //dataGridView1.Rows.Clear();
            //dataGridView1.Refresh();

            //init
            prediksiIsiAtmOpti = new List<PktDenom>();
            Database1Entities db = new Database1Entities();
            String kodePkt = (from x in db.Pkts
                              where x.kodePkt == this.kodePkt
                              select x.kodeOpti).FirstOrDefault();
            using (SqlConnection sqlConnection1 = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    SqlDataReader reader;

                    cmd.CommandText = "SELECT COUNT (*) FROM Opti o " +
                                      "JOIN Cashpoint c ON o.idCashpoint = c.idCashpoint " +
                                      "WHERE kodePkt = '" + kodePkt + "'";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = sqlConnection1;

                    sqlConnection1.Open();

                    reader = cmd.ExecuteReader();
                    reader.Read();
                    if ((int)reader[0] == 0)
                    {

                    }
                    else
                    {
                        reader.Close();
                        cmd.CommandText = "SELECT MIN(tanggal), MAX(tanggal) FROM Opti o JOIN Cashpoint c on o.idCashpoint = c.idCashpoint WHERE kodePkt = '"+kodePkt+"'GROUP BY kodePkt ";
                        reader = cmd.ExecuteReader();
                        reader.Read();
                        DateTime minDate = (DateTime)reader[0];
                        DateTime maxDate = (DateTime)reader[1];
                        reader.Close();
                        while (minDate <= maxDate)
                        {

                            PktDenom tempPrediksi = new PktDenom();
                            tempPrediksi.tanggal = minDate;
                            //Denom 100
                            cmd.CommandText = "SELECT SUM(prediksi) " +
                                              "FROM Opti o " +
                                              "JOIN Cashpoint c on o.idCashpoint = c.idCashpoint " +
                                              "WHERE kodePkt = '" + kodePkt + "' AND tanggal = '" + minDate.ToShortDateString() + "' AND denom = '100000' " +
                                              "GROUP BY kodePkt";
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                                tempPrediksi.d100 = (Int64)reader[0]; 
                            reader.Close();

                            //Denom 50
                            cmd.CommandText = "SELECT SUM(prediksi) " +
                                              "FROM Opti o " +
                                              "JOIN Cashpoint c on o.idCashpoint = c.idCashpoint " +
                                              "WHERE kodePkt = '" + kodePkt + "' AND tanggal = '" + minDate.ToShortDateString() + "' AND denom = '50000' " +
                                              "GROUP BY kodePkt";
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            { tempPrediksi.d50 = ((Int64)reader[0]); }
                            reader.Close();

                            //Denom 20
                            cmd.CommandText = "SELECT SUM(prediksi) " +
                                              "FROM Opti o " +
                                              "JOIN Cashpoint c on o.idCashpoint = c.idCashpoint " +
                                              "WHERE kodePkt = '" + kodePkt + "' AND tanggal = '" + minDate.ToShortDateString() + "' AND denom = '20000' " +
                                              "GROUP BY kodePkt";
                            reader = cmd.ExecuteReader();
                            if (reader.Read())
                            { tempPrediksi.d20 = ((Int64)reader[0]); }

                            tempPrediksi.kodePkt = kodePkt;
                            prediksiIsiAtmOpti.Add(tempPrediksi);
                            reader.Close();

                            minDate = minDate.AddDays(1);
                            //dataGridView1.Rows.Add(row);
                        }
                    }
                    sqlConnection1.Close();
                }

            }
            // Data is accessible through the DataReader object here.
        }
        void loadIsiAtmHistoris()
        {
            Console.WriteLine("Load Isi ATM");
            prediksiIsiAtm = new List<PktDenom>();
            prediksiIsiAtm = loadPrediksiHistoris("isiATM");
        }
        void loadIsiAtmHistorisDenganStandarDeviasi()
        {
            prediksiIsiAtmDenganStdDeviasi = new List<PktDenom>();
            prediksiIsiAtmDenganStdDeviasi = loadPrediksiStdDeviasi("isiAtm");
            Console.WriteLine("prediksiIsiAtmDenganStdDeviasi Count: " + prediksiIsiAtm.Count);
        }
        void loadIsiCrm()
        {
            isiCrm2 = new List<PktDenom>();
            isiCrm2 = loadPrediksiHistoris("isiCrm");
        }
        void loadIsiCrmDenganStdDeviasi()
        {
            isiCrmDenganStdDeviasi = new List<PktDenom>();
            isiCrmDenganStdDeviasi = loadPrediksiStdDeviasi("isiCrm");
        }
        void loadSislokCrm()
        {
            sislokCrm = new List<PktDenom>();
            sislokCrm = loadPrediksiHistoris("sislokCRM");
        }
        void loadSislokCrmDenganStdDeviasi()
        {
            sislokCrmDenganStdDeviasi = new List<PktDenom>();
            sislokCrmDenganStdDeviasi = loadPrediksiStdDeviasi("sislokCrm");
        }
        void loadSislokCdm()
        {
            sislokCdm = new List<PktDenom>();
            sislokCdm = loadPrediksiHistoris("sislokCdm");
        }
        void loadSislokCdmDenganStdDeviasi()
        {
            sislokCdmDenganStdDeviasi = new List<PktDenom>();
            sislokCdmDenganStdDeviasi = loadPrediksiStdDeviasi("sislokCdm");

            Console.WriteLine("SISLOK CDM DENGAN STD DEV 100: " + sislokCdmDenganStdDeviasi[0].d100);
        }
        void loadRasioSislokAtm()
        {
            Console.WriteLine();
            Console.WriteLine("Load rasio sislok atm");
            Console.WriteLine("======================");
            rasioSislokAtm = new List<PktRasio>();
            int rowcount = prediksiIsiAtmOpti.Count; //dataGridView1.Rows.Count;
            //dataGridView1.Hide();
            DateTime startDate = tanggalOptiMin;//Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = tanggalOptiMax;
            DateTime tempDate = startDate;
            Console.WriteLine(startDate.DayOfWeek.ToString());
            Console.WriteLine(endDate);

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    while (tempDate <= endDate)
                    {
                        //SislokCdm
                        SqlDataReader reader;
                        PktRasio tempSislokAtm = new PktRasio();
                        String cText = "SELECT ISNULL(AVG(CAST(sislokAtm100 AS FLOAT)/NULLIF(isiAtm100,0)),0), ISNULL(AVG(CAST(sislokAtm50 AS FLOAT)/NULLIF(isiAtm50,0)),0), ISNULL(AVG(CAST(sislokAtm20 AS FLOAT)/NULLIF(isiAtm20,0)),0), kodePkt FROM ViewTransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE "+pktCondition+" AND DATENAME(WEEKDAY, TA.Tanggal) = '" + tempDate.DayOfWeek.ToString() + "'";

                        EventAndCondition eac = loadEventWhereCondition(tempDate);
                        if(eac.eventType != "Event 1")
                        {
                            cText = "SELECT ISNULL(AVG(CAST(sislokAtm100 AS FLOAT)/NULLIF(isiAtm100,0)),0), ISNULL(AVG(CAST(sislokAtm50 AS FLOAT)/NULLIF(isiAtm50,0)),0), ISNULL(AVG(CAST(sislokAtm20 AS FLOAT)/NULLIF(isiAtm20,0)),0), kodePkt FROM ViewTransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE " + pktCondition;
                        }
                        cText += eac.whereCondition;
                        if (eac.eventType != "Event 4")
                            cText += ")";

                        cText += "\nGROUP BY kodePkt";
                        cmd.CommandText = cText;
                        reader = cmd.ExecuteReader();
                        while(reader.Read())
                        {
                            tempSislokAtm = new PktRasio();
                            tempSislokAtm.d100 = Convert.ToDouble(reader[0].ToString());
                            tempSislokAtm.d50 = Convert.ToDouble(reader[1].ToString());
                            tempSislokAtm.d20 = Convert.ToDouble(reader[2].ToString());
                            tempSislokAtm.kodePkt = reader[3].ToString();
                            tempSislokAtm.tanggal = tempDate;
                            rasioSislokAtm.Add(tempSislokAtm);
                        }
                        reader.Close();
                        tempDate = tempDate.AddDays(1);
                    }
                }
                sql.Close();
                Console.WriteLine("Rasio ATM");
                foreach (var temp in rasioSislokAtm)
                {
                    Console.WriteLine(temp.tanggal + " " + temp.d100 + " " + temp.d20 + " " + temp.d50);
                }
            }
        }
        void loadRasioSislokAtmDenganStdDeviasi()
        {
            Console.WriteLine();
            Console.WriteLine("Load Deviasi Sislok ATM");
            Console.WriteLine("======================");

            int rowcount = prediksiIsiAtmOpti.Count;//dataGridView1.Rows.Count;

            //dataGridView1.Hide();
            DateTime startDate = tanggalOptiMin;//Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = tanggalOptiMax;
            DateTime tempDate = startDate;
            Console.WriteLine(startDate.DayOfWeek.ToString());
            Console.WriteLine(endDate);

            //Load Std Deviasi
            List<PktRasio> stdDeviasi = new List<PktRasio>();
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    while (tempDate <= endDate)
                    {
                        //SislokATM
                        SqlDataReader reader;
                        PktRasio tempStdDeviasi = new PktRasio();

                        String kondisi = " WHERE "+pktCondition+" AND DATENAME(WEEKDAY, TA.Tanggal) = '" + tempDate.DayOfWeek.ToString() + "'";
                        int count = 0;

                        EventAndCondition eac = loadEventWhereCondition(tempDate);
                        if(eac.eventType != "Event 1")
                        {
                            kondisi = " WHERE " + pktCondition;
                        }
                        kondisi += eac.whereCondition;
                        if(eac.eventType != "Event 4")
                            kondisi += ")";

                        String subqueryTblAverage = "(SELECT AVG(CAST(sislokAtm100 AS FLOAT)/NULLIF(isiAtm100,0)) AS Average100 ,AVG(CAST(sislokAtm50 AS FLOAT)/NULLIF(isiAtm50,0)) AS Average50 , AVG(CAST(sislokAtm20 AS FLOAT)/NULLIF(isiAtm20,0)) AS Average20, kodePkt FROM ViewTransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal";
                        subqueryTblAverage += kondisi;
                        subqueryTblAverage += " GROUP BY kodePkt";
                        subqueryTblAverage += ") avt";

                        String query = "SELECT "
                                    + "\n[AverageStdDeviasi100] = AVG(CAST((CAST(sislokAtm100 AS FLOAT)/NULLIF(isiAtm100,0) - [Average100]) AS FLOAT) / (CASE WHEN [Average100] = 0 THEN 1 ELSE [Average100] END)), "
                                    + "\n[AverageStdDeviasi50] = AVG(CAST((CAST(sislokAtm50 AS FLOAT)/NULLIF(isiAtm50,0) - [Average50])AS FLOAT) / (CASE WHEN [Average50] = 0 THEN 1 ELSE [Average50] END)), "
                                    + "\n[AverageStdDeviasi20] = AVG(CAST((CAST(sislokAtm20 AS FLOAT)/NULLIF(isiAtm20,0) - [Average20])AS FLOAT) / (CASE WHEN [Average20] = 0 THEN 1 ELSE [Average20] END)), "
                                    + "\nTA.kodePkt"
                                    + "\nFROM ViewTransaksiAtms TA JOIN EventTanggal ET ON Ta.tanggal = ET.tanggal " 
                                    + "\nJOIN "+ subqueryTblAverage +" ON avt.kodePkt = TA.kodePkt"+ kondisi;

                        query += "\nGROUP BY TA.kodePkt";
                        cmd.CommandText = query;
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            tempStdDeviasi = new PktRasio();
                            tempStdDeviasi.d100 = String.IsNullOrEmpty(reader[0].ToString()) ? 0 : (Double)reader[0];
                            tempStdDeviasi.d50 = String.IsNullOrEmpty(reader[1].ToString()) ? 0 : (Double)reader[1];
                            tempStdDeviasi.d20 = String.IsNullOrEmpty(reader[2].ToString()) ? 0 : (Double)reader[2];
                            tempStdDeviasi.kodePkt = reader[3].ToString();
                            tempStdDeviasi.tanggal = tempDate;
                            stdDeviasi.Add(tempStdDeviasi);
                        }
                        reader.Close();
                        tempDate = tempDate.AddDays(1);
                    }
                }
                sql.Close();
                Console.WriteLine("Deviasi Sislok ATM");
                Console.WriteLine(rasioSislokATMDenganStdDeviasi.Count);
                foreach (var temp in stdDeviasi)
                {
                    Console.WriteLine(temp.tanggal + " " + temp.d100 + " " + temp.d50 + " " + temp.d20 + " ");
                }
            }
            rasioSislokATMDenganStdDeviasi = new List<PktRasio>();
            Console.WriteLine("stdDeviasi Count: " + stdDeviasi.Count + "rasioSislokAtm Count: " + rasioSislokAtm.Count);
            for (int a = 0; a < rasioSislokAtm.Count; a++)
            {
                PktRasio temp = new PktRasio();
                temp.d100 = rasioSislokAtm[a].d100 + (Int64)Math.Round(stdDeviasi.Where(x=>x.kodePkt == rasioSislokAtm[a].kodePkt).Select(x=>x.d100).FirstOrDefault() * rasioSislokAtm[a].d100);
                temp.d50 = rasioSislokAtm[a].d50 + (Int64)Math.Round(stdDeviasi.Where(x => x.kodePkt == rasioSislokAtm[a].kodePkt).Select(x => x.d50).FirstOrDefault() * rasioSislokAtm[a].d50);
                temp.d20 = rasioSislokAtm[a].d20 + (Int64)Math.Round(stdDeviasi.Where(x => x.kodePkt == rasioSislokAtm[a].kodePkt).Select(x => x.d20).FirstOrDefault() * rasioSislokAtm[a].d20);
                temp.kodePkt = rasioSislokAtm[a].kodePkt;
                temp.tanggal = rasioSislokAtm[a].tanggal;
                rasioSislokATMDenganStdDeviasi.Add(temp);
            }

            Console.WriteLine("Sislok ATM Dengan Std Deviasi");
            Console.WriteLine("===========================");
            foreach (var temp in rasioSislokATMDenganStdDeviasi)
            {
                Console.WriteLine(temp.tanggal.ToShortDateString() + " " + temp.d100 + " " + temp.d50 + " " + temp.d20);
            }
        }
    }
    public class EventAndCondition
    {
        public String eventType { set;get; }
        public String whereCondition { set; get; }
    }
}
