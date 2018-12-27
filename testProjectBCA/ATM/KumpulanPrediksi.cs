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

            pktCondition = "kodePkt = '" + kodePkt + "'";
            if (kodePkt.ToLower().Contains("kanwil") || kodePkt.ToLower().Contains("jabo"))
                pktCondition = "kanwil = '" + kodePkt + "'";
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
                sislokCdm = sislokCdmDenganStdDeviasi;
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
                multiplier = sislokCrm;
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
                            tempStdDeviasi.d100 = reader[0] == null ? 0 : (Double)reader[0];
                            tempStdDeviasi.d50 = reader[1] == null ? 0 : (Double)reader[1];
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
