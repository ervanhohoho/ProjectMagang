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
        public List<Denom> sislokCrm { set; get; }
        public List<Denom> sislokCrmDenganStdDeviasi { set; get; }
        public List<Denom> sislokCdm { set; get; }
        public List<Denom> sislokCdmDenganStdDeviasi { set; get; }
        public List<Rasio> rasioSislokAtm { set; get; } 
        public List<Rasio> rasioSislokATMDenganStdDeviasi { set; get; }
        public List<Denom> isiCrm2 { set; get; }
        public List<Denom> isiCrmDenganStdDeviasi { set; get; }
        public List<Denom> prediksiIsiAtmOpti { set; get; }
        public List<Denom> prediksiIsiAtm { set; get; }//dalem class denom ada 100,50,20
        public List<Denom> prediksiIsiAtmDenganStdDeviasi { set; get; }

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
            sislokCrm = new List<Denom>();
            sislokCrmDenganStdDeviasi = new List<Denom>();
            sislokCdm = new List<Denom>();
            sislokCdmDenganStdDeviasi = new List<Denom>();
            rasioSislokATMDenganStdDeviasi = new List<Rasio>();
            rasioSislokAtm = new List<Rasio>();
            isiCrm2 = new List<Denom>();
            isiCrmDenganStdDeviasi = new List<Denom>();
            prediksiIsiAtm = new List<Denom>();
            prediksiIsiAtmDenganStdDeviasi = new List<Denom>();

            success = true;

            pktCondition = "kodePkt = '" + kodePkt + "'";
            if (kodePkt.ToLower().Contains("kanwil") || kodePkt.ToLower().Contains("jabo"))
                pktCondition = "kanwil = '" + kodePkt + "'";
            if (kodePkt.ToLower() == "all")
                pktCondition = "true";


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
            loadRasioSislokAtmDenganStdDeviasi();
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
        List<Denom> loadPrediksiHistoris(String jenis)
        {

            DateTime startDate = tanggalOptiMin;//Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = tanggalOptiMax;//Convert.ToDateTime(dataGridView1.Rows[rowCount - 1].Cells[0].Value);
            DateTime tempDate = startDate;
            List<Denom> ret = new List<Denom>();
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
                        Denom tempIsiAtm = new Denom();
                        String cText = "SELECT AVG(" + jenis + "100), AVG(" + jenis + "50), AVG(" + jenis + "20) FROM ViewTransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE "+pktCondition+" AND DATENAME(WEEKDAY, TA.Tanggal) = '" + tempDate.DayOfWeek.ToString() + "'";
                        EventAndCondition eac = loadEventWhereCondition(tempDate);

                        if (eac.eventType != "Event 1" && eac.eventType != "Event 4")
                            cText = "SELECT AVG(" + jenis + "100), AVG(" + jenis + "50), AVG(" + jenis + "20) FROM ViewTransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE " + pktCondition;

                        cText += eac.whereCondition;
                        if (eac.eventType != "Event 4")
                            cText += ")";
                        cmd.CommandText = cText;
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            tempIsiAtm.d50 = (Int64)reader[1];
                            tempIsiAtm.d20 = (Int64)reader[2];
                            tempIsiAtm.tgl = tempDate;
                            tempIsiAtm.d100 = (Int64)reader[0];
                            reader.Close();
                        }
                        ret.Add(tempIsiAtm);
                        tempDate = tempDate.AddDays(1);
                    }

                }
                sql.Close();
            }
            return ret;
        }
        List<Denom> loadPrediksiStdDeviasi(String jenis)
        {
            int rowcount = prediksiIsiAtmOpti.Count;//dataGridView1.Rows.Count;
            DateTime startDate = tanggalOptiMin;//Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = tanggalOptiMax;
            DateTime tempDate = startDate;

            //Load Std Deviasi
            List<Rasio> stdDeviasi = new List<Rasio>();
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
                        Rasio tempStdDeviasi = new Rasio();

                        String kondisi = " WHERE " + pktCondition + " AND DATENAME(WEEKDAY, TA.Tanggal) = '" + tempDate.DayOfWeek.ToString() + "'";
                        int count = 0;

                        EventAndCondition eac = loadEventWhereCondition(tempDate);
                        if (eac.eventType != "Event 1" && eac.eventType != "Event 4")
                        {
                            kondisi = " WHERE kodePkt = '" + kodePkt + "' ";
                        }
                        kondisi += eac.whereCondition;
                        if (eac.eventType != "Event 4")
                            kondisi += ")";

                        String subqueryTblAverage = "(SELECT ISNULL(AVG(" + jenis + "100),0) AS Average100 , ISNULL(AVG(" + jenis + "50),0) AS Average50 , ISNULL(AVG(" + jenis + "20),0) AS Average20 FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal";
                        subqueryTblAverage += kondisi;
                        subqueryTblAverage += ") avt";

                        String query = "";
                         if(jenis.ToLower().Contains("sislok"))
                        {
                           query = "SELECT "
                                    + "[AverageStdDeviasi100] = AVG(CAST((" + jenis + "100 - [Average100]) AS FLOAT) / (CASE WHEN [Average100] = 0 THEN 1 ELSE [Average100] END)), "
                                    + "[AverageStdDeviasi50] = AVG(CAST((" + jenis + "50 - [Average50])AS FLOAT) / (CASE WHEN [Average50] = 0 THEN 1 ELSE [Average50] END)), "
                                    + "[AverageStdDeviasi20] = AVG(CAST((" + jenis + "20 - [Average20])AS FLOAT) / (CASE WHEN [Average20] = 0 THEN 1 ELSE [Average20] END)) "
                                    + "FROM ViewTransaksiAtms TA JOIN EventTanggal ET ON Ta.tanggal = ET.tanggal, " + subqueryTblAverage + kondisi;
                        }
                        else
                        {
                            query = "SELECT "
                                    + "[AverageStdDeviasi100] = AVG(CAST(ABS(" + jenis + "100 - [Average100]) AS FLOAT) / (CASE WHEN [Average100] = 0 THEN 1 ELSE [Average100] END)), "
                                    + "[AverageStdDeviasi50] = AVG(CAST(ABS(" + jenis + "50 - [Average50])AS FLOAT) / (CASE WHEN [Average50] = 0 THEN 1 ELSE [Average50] END)), "
                                    + "[AverageStdDeviasi20] = AVG(CAST(ABS(" + jenis + "20 - [Average20])AS FLOAT) / (CASE WHEN [Average20] = 0 THEN 1 ELSE [Average20] END)) "
                                    + "FROM ViewTransaksiAtms TA JOIN EventTanggal ET ON Ta.tanggal = ET.tanggal, " + subqueryTblAverage + kondisi;

                        }

                        cmd.CommandText = query;
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {

                            tempStdDeviasi.d100 = (Double)reader[0];
                            tempStdDeviasi.d50 = (Double)reader[1];
                            tempStdDeviasi.d20 = (Double)reader[2];
                            tempStdDeviasi.tgl = tempDate;
                            reader.Close();
                        }
                        stdDeviasi.Add(tempStdDeviasi);
                        tempDate = tempDate.AddDays(1);
                    }
                }
                sql.Close();
            }
            List<Denom> multiplier = new List<Denom>();
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
            List<Denom> ret = new List<Denom>();
            for (int a = 0; a < prediksiIsiAtm.Count; a++)
            {
                Denom temp = new Denom();
                temp.d100 = multiplier[a].d100 + (Int64)Math.Round(stdDeviasi[a].d100 * multiplier[a].d100);
                temp.d50 = multiplier[a].d50 + (Int64)Math.Round(stdDeviasi[a].d50 * multiplier[a].d50);
                temp.d20 = multiplier[a].d20 + (Int64)Math.Round(stdDeviasi[a].d20 * multiplier[a].d20);
                temp.tgl = multiplier[a].tgl;
                ret.Add(temp);
            }
            return ret;
        }
        void loadPrediksiOpti()
        {
            //dataGridView1.Rows.Clear();
            //dataGridView1.Refresh();

            //init
            prediksiIsiAtmOpti = new List<Denom>();
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

                            Denom tempPrediksi = new Denom();
                            tempPrediksi.tgl = minDate;
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
            prediksiIsiAtm = new List<Denom>();
            prediksiIsiAtm = loadPrediksiHistoris("isiATM");
        }
        void loadIsiAtmHistorisDenganStandarDeviasi()
        {
            prediksiIsiAtmDenganStdDeviasi = new List<Denom>();
            prediksiIsiAtmDenganStdDeviasi = loadPrediksiStdDeviasi("isiAtm");
        }
        void loadIsiCrm()
        {
            isiCrm2 = new List<Denom>();
            isiCrm2 = loadPrediksiHistoris("isiCrm");
        }
        void loadIsiCrmDenganStdDeviasi()
        {
            isiCrmDenganStdDeviasi = new List<Denom>();
            isiCrmDenganStdDeviasi = loadPrediksiStdDeviasi("isiCrm");
        }
        void loadSislokCrm()
        {
            sislokCrm = new List<Denom>();
            sislokCrm = loadPrediksiHistoris("sislokCRM");
        }
        void loadSislokCrmDenganStdDeviasi()
        {
            sislokCrmDenganStdDeviasi = new List<Denom>();
            sislokCrmDenganStdDeviasi = loadPrediksiStdDeviasi("sislokCrm");
        }
        void loadSislokCdm()
        {
            sislokCdm = new List<Denom>();
            sislokCdm = loadPrediksiHistoris("sislokCdm");
        }
        void loadSislokCdmDenganStdDeviasi()
        {
            sislokCdmDenganStdDeviasi = new List<Denom>();
            sislokCdmDenganStdDeviasi = loadPrediksiStdDeviasi("sislokCdm");
        }
        void loadRasioSislokAtm()
        {
            Console.WriteLine();
            Console.WriteLine("Load rasio sislok atm");
            Console.WriteLine("======================");
            rasioSislokAtm = new List<Rasio>();
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
                        Rasio tempSislokAtm = new Rasio();
                        String cText = "SELECT ISNULL(AVG(CAST(sislokAtm100 AS FLOAT)/NULLIF(isiAtm100,0)),0), ISNULL(AVG(CAST(sislokAtm50 AS FLOAT)/NULLIF(isiAtm50,0)),0), ISNULL(AVG(CAST(sislokAtm20 AS FLOAT)/NULLIF(isiAtm20,0)),0) FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE kodePkt = '" + kodePkt + "' AND DATENAME(WEEKDAY, TA.Tanggal) = '" + tempDate.DayOfWeek.ToString() + "'";

                        EventAndCondition eac = loadEventWhereCondition(tempDate);
                        if(eac.eventType != "Event 1")
                        {
                            cText = "SELECT ISNULL(AVG(CAST(sislokAtm100 AS FLOAT)/NULLIF(isiAtm100,0)),0), ISNULL(AVG(CAST(sislokAtm50 AS FLOAT)/NULLIF(isiAtm50,0)),0), ISNULL(AVG(CAST(sislokAtm20 AS FLOAT)/NULLIF(isiAtm20,0)),0) FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE kodePkt = '" + kodePkt + "'";
                        }
                        cText += eac.whereCondition;
                        if (eac.eventType != "Event 4")
                            cText += ")";
                        cmd.CommandText = cText;
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            tempSislokAtm.d100 = Convert.ToDouble(reader[0].ToString());
                            tempSislokAtm.d50 = Convert.ToDouble(reader[1].ToString());
                            tempSislokAtm.d20 = Convert.ToDouble(reader[2].ToString());
                            tempSislokAtm.tgl = tempDate;
                            reader.Close();
                        }
                        rasioSislokAtm.Add(tempSislokAtm);
                        tempDate = tempDate.AddDays(1);
                    }
                }
                sql.Close();
                Console.WriteLine("Rasio ATM");
                foreach (var temp in rasioSislokAtm)
                {
                    Console.WriteLine(temp.tgl + " " + temp.d100 + " " + temp.d20 + " " + temp.d50);
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
            List<Rasio> stdDeviasi = new List<Rasio>();
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
                        Rasio tempStdDeviasi = new Rasio();

                        String kondisi = " WHERE kodePkt = '" + kodePkt + "' AND DATENAME(WEEKDAY, TA.Tanggal) = '" + tempDate.DayOfWeek.ToString() + "'";
                        int count = 0;

                        EventAndCondition eac = loadEventWhereCondition(tempDate);
                        if(eac.eventType != "Event 1")
                        {
                            kondisi = " WHERE kodePkt = '" + kodePkt + "' ";
                        }
                        kondisi += eac.whereCondition;
                        if(eac.eventType != "Event 4")
                            kondisi += ")";

                        String subqueryTblAverage = "(SELECT AVG(CAST(sislokAtm100 AS FLOAT)/NULLIF(isiAtm100,0)) AS Average100 ,AVG(CAST(sislokAtm50 AS FLOAT)/NULLIF(isiAtm50,0)) AS Average50 , AVG(CAST(sislokAtm20 AS FLOAT)/NULLIF(isiAtm20,0)) AS Average20 FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal";
                        subqueryTblAverage += kondisi;
                        subqueryTblAverage += ") avt";

                        String query = "SELECT "
                                    + "[AverageStdDeviasi100] = AVG(CAST((CAST(sislokAtm100 AS FLOAT)/NULLIF(isiAtm100,0) - [Average100]) AS FLOAT) / (CASE WHEN [Average100] = 0 THEN 1 ELSE [Average100] END)), "
                                    + "[AverageStdDeviasi50] = AVG(CAST((CAST(sislokAtm50 AS FLOAT)/NULLIF(isiAtm50,0) - [Average50])AS FLOAT) / (CASE WHEN [Average50] = 0 THEN 1 ELSE [Average50] END)), "
                                    + "[AverageStdDeviasi20] = AVG(CAST((CAST(sislokAtm20 AS FLOAT)/NULLIF(isiAtm20,0) - [Average20])AS FLOAT) / (CASE WHEN [Average20] = 0 THEN 1 ELSE [Average20] END)) "
                                    + "FROM TransaksiAtms TA JOIN EventTanggal ET ON Ta.tanggal = ET.tanggal, " + subqueryTblAverage + kondisi;

                        cmd.CommandText = query;
                        reader = cmd.ExecuteReader();
                        bool event1 = true;
                        if (reader.Read())
                        {
                            if (String.IsNullOrEmpty(reader[0].ToString()) || String.IsNullOrEmpty(reader[1].ToString()) || String.IsNullOrEmpty(reader[2].ToString()))
                                event1 = false;
                            else
                            {
                                tempStdDeviasi.d100 = (Double)reader[0];
                                tempStdDeviasi.d50 = (Double)reader[1];
                                tempStdDeviasi.d20 = (Double)reader[2];
                                tempStdDeviasi.tgl = tempDate;
                            }
                            reader.Close();
                        }
                        stdDeviasi.Add(tempStdDeviasi);
                        tempDate = tempDate.AddDays(1);
                    }
                }
                sql.Close();
                Console.WriteLine("Deviasi Sislok ATM");
                Console.WriteLine(rasioSislokATMDenganStdDeviasi.Count);
                foreach (var temp in stdDeviasi)
                {
                    Console.WriteLine(temp.tgl + " " + temp.d100 + " " + temp.d50 + " " + temp.d20 + " ");
                }
            }
            rasioSislokATMDenganStdDeviasi = new List<Rasio>();
            for (int a = 0; a < rasioSislokAtm.Count; a++)
            {
                Rasio temp = new Rasio();
                temp.d100 = rasioSislokAtm[a].d100 + (Int64)Math.Round(stdDeviasi[a].d100 * rasioSislokAtm[a].d100);
                temp.d50 = rasioSislokAtm[a].d50 + (Int64)Math.Round(stdDeviasi[a].d50 * rasioSislokAtm[a].d50);
                temp.d20 = rasioSislokAtm[a].d20 + (Int64)Math.Round(stdDeviasi[a].d20 * rasioSislokAtm[a].d20);
                temp.tgl = rasioSislokAtm[a].tgl;
                rasioSislokATMDenganStdDeviasi.Add(temp);
            }

            Console.WriteLine("Sislok ATM Dengan Std Deviasi");
            Console.WriteLine("===========================");
            foreach (var temp in rasioSislokATMDenganStdDeviasi)
            {
                Console.WriteLine(temp.tgl.ToShortDateString() + " " + temp.d100 + " " + temp.d50 + " " + temp.d20);
            }
        }
    }
    public class EventAndCondition
    {
        public String eventType { set;get; }
        public String whereCondition { set; get; }
    }
}
