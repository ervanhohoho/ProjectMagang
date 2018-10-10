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
            loadPrediksiOpti();
            if(jenisPrediksiIsiATM == "Opti")
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
                    String cText = "SELECT AVG(isiCrm100), AVG(isiCrm50), AVG(isiCrm20) FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE kodePkt = '" + kodePkt + "' AND DATENAME(WEEKDAY, TA.Tanggal) = '" + tempDate.DayOfWeek.ToString() + "'";
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
                    if (reader.Read())
                    {
                        if (String.IsNullOrEmpty(reader[0].ToString()) || String.IsNullOrEmpty(reader[1].ToString()) || String.IsNullOrEmpty(reader[2].ToString()))
                        {
                            Console.WriteLine("NOT EVENT 1");
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
                        cText = "SELECT AVG(isiCrm100),AVG(isiCrm50), AVG(isiCrm20) FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE kodePkt = '" + kodePkt + "' ";
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
                                Console.WriteLine("NOT EVENT 2");
                                event2 = false;
                            }
                            else
                            {
                                reader.Close();
                                eventType.Add("Tanggal " + tempDate.ToShortDateString() + " Menggunakan Event 2");
                                Console.WriteLine("Event 2: " + cText);
                                return new EventAndCondition() { eventType = "Event 2",whereCondition = whereCondition };
                            }
                        }
                    }
                    if (!event2)
                    {
                        reader.Close();
                        cText = "SELECT ISNULL(AVG(isiCrm100),0), ISNULL(AVG(isiCrm50),0), ISNULL(AVG(isiCrm20),0) FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE kodePkt = '" + kodePkt + "' ";
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
                            
                            eventType.Add("Tanggal " + tempDate.ToShortDateString() + " Menggunakan Event 3");
                            return new EventAndCondition() { eventType = "Event 3", whereCondition = whereCondition };
                        }
                        else
                        {
                            reader.Close();
                        }
                    }
                    //Console.WriteLine(tempSislokCrm.d100.ToString());
                    tempDate = tempDate.AddDays(1);
                }
            }
            return new EventAndCondition();
        }

        void loadPrediksiOpti()
        {
            //dataGridView1.Rows.Clear();
            //dataGridView1.Refresh();

            //init
            prediksiIsiAtmOpti = new List<Denom>();

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
                        cmd.CommandText = "SELECT MIN(tanggal), MAX(tanggal) FROM Opti o JOIN Cashpoint c on o.idCashpoint = c.idCashpoint GROUP BY kodePkt ";
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
                            Console.WriteLine(minDate);
                            //dataGridView1.Rows.Add(row);
                        }
                    }
                    sqlConnection1.Close();
                }

            }
            // Data is accessible through the DataReader object here.
            Console.WriteLine("Prediksi isi ATM Opti");
            Console.WriteLine("===================");
            foreach (var temp in prediksiIsiAtmOpti)
            {
                Console.WriteLine(temp.tgl + " " + temp.d100 + " " + temp.d50 + " " + temp.d20);
            }
        }
        void loadIsiAtmHistoris()
        {
            Console.WriteLine();
            Console.WriteLine("Load Isi ATM");
            Console.WriteLine("======================");
            prediksiIsiAtm = new List<Denom>();
            //dataGridView1.Hide();
            DateTime startDate = tanggalOptiMin;//Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = tanggalOptiMax;//Convert.ToDateTime(dataGridView1.Rows[rowCount - 1].Cells[0].Value);
            DateTime tempDate = startDate;
            Console.WriteLine(startDate.DayOfWeek.ToString());
            Console.WriteLine(endDate);

            prediksiIsiAtm = new List<Denom>();
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
                        String cText = "SELECT AVG(isiAtm100), AVG(isiAtm50), AVG(isiAtm20) FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE kodePkt = '" + kodePkt + "' AND DATENAME(WEEKDAY, TA.Tanggal) = '" + tempDate.DayOfWeek.ToString() + "'";
                        EventAndCondition eac = loadEventWhereCondition(tempDate);

                        if (eac.eventType!="Event 1")
                            cText = "SELECT AVG(isiAtm100), AVG(isiAtm50), AVG(isiAtm20) FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE kodePkt = '" + kodePkt + "'";

                        Console.WriteLine("Where cond: " + eac.whereCondition);
                        cText += eac.whereCondition;
                        cText += ")";
                        cmd.CommandText = cText;
                        Console.WriteLine(cmd.CommandText);
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            tempIsiAtm.d50 = (Int64)reader[1];
                            tempIsiAtm.d20 = (Int64)reader[2];
                            tempIsiAtm.tgl = tempDate;
                            tempIsiAtm.d100 = (Int64)reader[0];
                            reader.Close();
                        }
                        prediksiIsiAtm.Add(tempIsiAtm);
                        tempDate = tempDate.AddDays(1);
                    }

                }
                sql.Close();
                Console.WriteLine("Isi ATM");
                Console.WriteLine(prediksiIsiAtm.Count);
                foreach (var temp in prediksiIsiAtm)
                {
                    Console.WriteLine(temp.tgl + " " + temp.d100 + " " + temp.d50 + " " + temp.d20 + " ");
                }
            }
        }
        void loadIsiAtmHistorisDenganStandarDeviasi()
        {

            Console.WriteLine();
            Console.WriteLine("Load Deviasi Isi Atm");
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
                        //SislokAtm
                        SqlDataReader reader;
                        Rasio tempStdDeviasi = new Rasio();

                        String kondisi = " WHERE kodePkt = '" + kodePkt + "' AND DATENAME(WEEKDAY, TA.Tanggal) = '" + tempDate.DayOfWeek.ToString() + "'";
                        int count = 0;

                        EventAndCondition eac = loadEventWhereCondition(tempDate);
                        if (eac.eventType != "Event 1")
                        {
                            kondisi = " WHERE kodePkt = '" + kodePkt + "' ";
                        }
                        kondisi += eac.whereCondition;
                        kondisi += ")";

                        String subqueryTblAverage = "(SELECT ISNULL(AVG(isiAtm100),0) AS Average100 , ISNULL(AVG(isiAtm50),0) AS Average50 , ISNULL(AVG(isiAtm20),0) AS Average20 FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal";
                        subqueryTblAverage += kondisi;
                        subqueryTblAverage += ") avt";

                        String query = "SELECT "
                                    + "[AverageStdDeviasi100] = AVG(CAST(ABS(isiAtm100 - [Average100]) AS FLOAT) / (CASE WHEN [Average100] = 0 THEN 1 ELSE [Average100] END)), "
                                    + "[AverageStdDeviasi50] = AVG(CAST(ABS(isiAtm50 - [Average50])AS FLOAT) / (CASE WHEN [Average50] = 0 THEN 1 ELSE [Average50] END)), "
                                    + "[AverageStdDeviasi20] = AVG(CAST(ABS(isiAtm20 - [Average20])AS FLOAT) / (CASE WHEN [Average20] = 0 THEN 1 ELSE [Average20] END)) "
                                    + "FROM TransaksiAtms TA JOIN EventTanggal ET ON Ta.tanggal = ET.tanggal, " + subqueryTblAverage + kondisi;

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
                Console.WriteLine("Deviasi Atm");
                foreach (var temp in stdDeviasi)
                {
                    Console.WriteLine(temp.tgl + " " + temp.d100 + " " + temp.d50 + " " + temp.d20 + " ");
                }
            }

            prediksiIsiAtmDenganStdDeviasi = new List<Denom>();
            for (int a = 0; a < prediksiIsiAtm.Count; a++)
            {
                Denom temp = new Denom();
                temp.d100 = prediksiIsiAtm[a].d100 + (Int64)Math.Round(stdDeviasi[a].d100 * prediksiIsiAtm[a].d100);
                temp.d50 = prediksiIsiAtm[a].d50 + (Int64)Math.Round(stdDeviasi[a].d50 * prediksiIsiAtm[a].d50);
                temp.d20 = prediksiIsiAtm[a].d20 + (Int64)Math.Round(stdDeviasi[a].d20 * prediksiIsiAtm[a].d20);
                temp.tgl = prediksiIsiAtm[a].tgl;
                prediksiIsiAtmDenganStdDeviasi.Add(temp);
            }

            Console.WriteLine("Isi Atm Dengan Std Deviasi");
            Console.WriteLine("===========================");
            foreach (var temp in prediksiIsiAtmDenganStdDeviasi)
            {
                Console.WriteLine(temp.tgl.ToShortDateString() + " " + temp.d100 + " " + temp.d50 + " " + temp.d20);
            }
        }
        void loadIsiCrm()
        {
            Console.WriteLine();
            Console.WriteLine("Load isi CRM");
            Console.WriteLine("======================");
            isiCrm2 = new List<Denom>();
            int rowcount = prediksiIsiAtmOpti.Count;//dataGridView1.Rows.Count;
                                                    //dataGridView1.Hide();
            DateTime startDate = tanggalOptiMin;//Convert.ToDateTime(dataGridView1.Rows[0].Cells[0].Value);
            DateTime endDate = tanggalOptiMax;
            DateTime tempDate = startDate;
            Console.WriteLine(startDate.DayOfWeek.ToString());

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    while (tempDate <= endDate)
                    {
                        //isiCrm
                        SqlDataReader reader;
                        Denom tempisiCrm = new Denom();
                        String cText = "SELECT AVG(isiCrm100), AVG(isiCrm50), AVG(isiCrm20) FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE kodePkt = '" + kodePkt + "' AND DATENAME(WEEKDAY, TA.Tanggal) = '" + tempDate.DayOfWeek.ToString() + "'";
                        EventAndCondition eac = loadEventWhereCondition(tempDate);

                        if (eac.eventType != "Event 1")
                            cText = "SELECT AVG(isiCrm100), AVG(isiCrm50), AVG(isiCrm20) FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE kodePkt = '" + kodePkt + "'";

                        cText += eac.whereCondition;
                        cText += ")";
                        cmd.CommandText = cText;
                        reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            tempisiCrm.d100 = (Int64)reader[0];
                            tempisiCrm.d50 = (Int64)reader[1];
                            tempisiCrm.d20 = (Int64)reader[2];
                            tempisiCrm.tgl = tempDate;
                            reader.Close();
                        }

                        //Console.WriteLine(tempDate + " " + tempisiCrm.d100.ToString() + " " + tempisiCrm.d50.ToString() + " " + tempisiCrm.d20.ToString());
                        isiCrm2.Add(tempisiCrm);
                        tempDate = tempDate.AddDays(1);
                    }
                }
                sql.Close();
                Console.WriteLine("isi CRM");
                Console.WriteLine(isiCrm2.Count);
                foreach (var temp in isiCrm2)
                {
                    Console.WriteLine(temp.tgl + " " + temp.d100 + " " + temp.d50 + " " + temp.d20);
                }
            }
        }
        void loadIsiCrmDenganStdDeviasi()
        {
            Console.WriteLine();
            Console.WriteLine("Load Deviasi Isi CRM");
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
                        //SislokCrm
                        SqlDataReader reader;
                        Rasio tempStdDeviasi = new Rasio();

                        String kondisi = " WHERE kodePkt = '" + kodePkt + "' AND DATENAME(WEEKDAY, TA.Tanggal) = '" + tempDate.DayOfWeek.ToString() + "'";
                        int count = 0;

                        EventAndCondition eac = loadEventWhereCondition(tempDate);
                        if (eac.eventType != "Event 1")
                        {
                            kondisi = " WHERE kodePkt = '" + kodePkt + "' ";
                        }
                        kondisi += eac.whereCondition;
                        kondisi += ")";

                        String subqueryTblAverage = "(SELECT AVG(isiCrm100) AS Average100 , AVG(isiCrm50) AS Average50 , AVG(isiCrm20) AS Average20 FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal";
                        subqueryTblAverage += kondisi;
                        subqueryTblAverage += ") avt";

                        String query = "SELECT "
                                    + "[AverageStdDeviasi100] = AVG(CAST(ABS(isiCrm100 - [Average100]) AS FLOAT) / (CASE WHEN [Average100] = 0 THEN 1 ELSE [Average100] END)), "
                                    + "[AverageStdDeviasi50] = AVG(CAST(ABS(isiCrm50 - [Average50])AS FLOAT) / (CASE WHEN [Average50] = 0 THEN 1 ELSE [Average50] END)), "
                                    + "[AverageStdDeviasi20] = AVG(CAST(ABS(isiCrm20 - [Average20])AS FLOAT) / (CASE WHEN [Average20] = 0 THEN 1 ELSE [Average20] END)) "
                                    + "FROM TransaksiAtms TA JOIN EventTanggal ET ON Ta.tanggal = ET.tanggal, " + subqueryTblAverage + kondisi;

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
                Console.WriteLine("Deviasi CRM");
                Console.WriteLine(isiCrm2.Count);
                foreach (var temp in stdDeviasi)
                {
                    Console.WriteLine(temp.tgl + " " + temp.d100 + " " + temp.d50 + " " + temp.d20 + " ");
                }
            }

            isiCrmDenganStdDeviasi = new List<Denom>();
            for (int a = 0; a < isiCrm2.Count; a++)
            {
                Denom temp = new Denom();
                temp.d100 = isiCrm2[a].d100 + (Int64)Math.Round(stdDeviasi[a].d100 * isiCrm2[a].d100);
                temp.d50 = isiCrm2[a].d50 + (Int64)Math.Round(stdDeviasi[a].d50 * isiCrm2[a].d50);
                temp.d20 = isiCrm2[a].d20 + (Int64)Math.Round(stdDeviasi[a].d20 * isiCrm2[a].d20);
                temp.tgl = isiCrm2[a].tgl;
                isiCrmDenganStdDeviasi.Add(temp);
            }

            Console.WriteLine("Isi CRM Dengan Std Deviasi");
            Console.WriteLine("===========================");
            foreach (var temp in isiCrmDenganStdDeviasi)
            {
                Console.WriteLine(temp.tgl.ToShortDateString() + " " + temp.d100 + " " + temp.d50 + " " + temp.d20);
            }
        }
        void loadSislokCrm()
        {
            Console.WriteLine();
            Console.WriteLine("Load Sislok CRM");
            Console.WriteLine("======================");
            sislokCrm = new List<Denom>();
            int rowcount = prediksiIsiAtmOpti.Count;//dataGridView1.Rows.Count;
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
                        //SislokCrm
                        SqlDataReader reader;
                        Denom tempSislokCrm = new Denom();
                        String cText = "SELECT AVG(sislokCrm100), AVG(sislokCrm50), AVG(sislokCrm20) FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE kodePkt = '" + kodePkt + "' AND DATENAME(WEEKDAY, TA.Tanggal) = '" + tempDate.DayOfWeek.ToString() + "'";
                        int count = 0;

                        EventAndCondition eac = loadEventWhereCondition(tempDate);

                        if (eac.eventType != "Event 1")
                            cText = "SELECT AVG(sislokCrm100), AVG(sislokCrm50), AVG(sislokCrm20) FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE kodePkt = '" + kodePkt + "'";

                        cText += eac.whereCondition;
                        cText += ")";
                        cmd.CommandText = cText;
                        reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            tempSislokCrm.d100 = (Int64)reader[0];
                            tempSislokCrm.d50 = (Int64)reader[1];
                            tempSislokCrm.d20 = (Int64)reader[2];
                            tempSislokCrm.tgl = tempDate;
                            reader.Close();
                        }
                        
                        //Console.WriteLine(tempSislokCrm.d100.ToString());
                        sislokCrm.Add(tempSislokCrm);
                        tempDate = tempDate.AddDays(1);

                    }
                }
                sql.Close();
                Console.WriteLine("Sislok CRM");
                Console.WriteLine(sislokCrm.Count);
                foreach (var temp in sislokCrm)
                {
                    Console.WriteLine(temp.tgl + " " + temp.d100 + " " + temp.d50 + " " + temp.d20);
                }
            }
        }
        void loadSislokCrmDenganStdDeviasi()
        {
            Console.WriteLine();
            Console.WriteLine("Load Deviasi Sislok CRM");
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
                        //SislokCrm
                        SqlDataReader reader;
                        Rasio tempStdDeviasi = new Rasio();

                        String kondisi = " WHERE kodePkt = '" + kodePkt + "' AND DATENAME(WEEKDAY, TA.Tanggal) = '" + tempDate.DayOfWeek.ToString() + "'";
                        int count = 0;

                        EventAndCondition eac = loadEventWhereCondition(tempDate);
                        if (eac.eventType != "Event 1")
                        {
                            kondisi = " WHERE kodePkt = '" + kodePkt + "' ";
                        }
                        kondisi += eac.whereCondition;
                        kondisi += ")";

                        String subqueryTblAverage = "(SELECT AVG(sislokCrm100) AS Average100 , AVG(sislokCrm50) AS Average50 , AVG(sislokCrm20) AS Average20 FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal";
                        subqueryTblAverage += kondisi;
                        subqueryTblAverage += ") avt";

                        String query = "SELECT "
                                    + "[AverageStdDeviasi100] = AVG(CAST((sislokCrm100 - [Average100]) AS FLOAT) / (CASE WHEN [Average100] = 0 THEN 1 ELSE [Average100] END)), "
                                    + "[AverageStdDeviasi50] = AVG(CAST((sislokCrm50 - [Average50])AS FLOAT) / (CASE WHEN [Average50] = 0 THEN 1 ELSE [Average50] END)), "
                                    + "[AverageStdDeviasi20] = AVG(CAST((sislokCrm20 - [Average20])AS FLOAT) / (CASE WHEN [Average20] = 0 THEN 1 ELSE [Average20] END)) "
                                    + "FROM TransaksiAtms TA JOIN EventTanggal ET ON Ta.tanggal = ET.tanggal, " + subqueryTblAverage + kondisi;

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
                Console.WriteLine("Deviasi Sislok CRM");
                Console.WriteLine(sislokCrm.Count);
                foreach (var temp in stdDeviasi)
                {
                    Console.WriteLine(temp.tgl + " " + temp.d100 + " " + temp.d50 + " " + temp.d20 + " ");
                }
            }
            sislokCrmDenganStdDeviasi = new List<Denom>();
            for (int a = 0; a < sislokCrm.Count; a++)
            {
                Denom temp = new Denom();
                temp.d100 = sislokCrm[a].d100 + (Int64)Math.Round(stdDeviasi[a].d100 * sislokCrm[a].d100);
                temp.d50 = sislokCrm[a].d50 + (Int64)Math.Round(stdDeviasi[a].d50 * sislokCrm[a].d50);
                temp.d20 = sislokCrm[a].d20 + (Int64)Math.Round(stdDeviasi[a].d20 * sislokCrm[a].d20);
                temp.tgl = sislokCrm[a].tgl;
                sislokCrmDenganStdDeviasi.Add(temp);
            }

            Console.WriteLine("Sislok CRM Dengan Std Deviasi");
            Console.WriteLine("===========================");
            foreach (var temp in sislokCrmDenganStdDeviasi)
            {
                Console.WriteLine(temp.tgl.ToShortDateString() + " " + temp.d100 + " " + temp.d50 + " " + temp.d20);
            }
        }
        void loadSislokCdm()
        {
            Console.WriteLine();
            Console.WriteLine("Load sislok cdm");
            Console.WriteLine("======================");
            sislokCdm = new List<Denom>();
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
                    String errMsg = "";
                    bool iserror = false;
                    while (tempDate <= endDate)
                    {
                        //SislokCdm
                        SqlDataReader reader;
                        Denom tempSislokCdm = new Denom();
                        String cText = "SELECT AVG(sislokCdm100), AVG(sislokCdm50), AVG(sislokCdm20) FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE kodePkt = '" + kodePkt + "' AND DATENAME(WEEKDAY, TA.Tanggal) = '" + tempDate.DayOfWeek.ToString() + "'";
                        int count = 0;

                        EventAndCondition eac = loadEventWhereCondition(tempDate);
                        if (eac.eventType != "Event 1")
                            cText = "SELECT AVG(sislokcdm100), AVG(sislokcdm50), AVG(sislokcdm20) FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal WHERE kodePkt = '" + kodePkt + "'";

                        cText += eac.whereCondition;
                        cText += ")";
                        cmd.CommandText = cText;
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            tempSislokCdm.d100 = (Int64)reader[0];
                            tempSislokCdm.d50 = (Int64)reader[1];
                            tempSislokCdm.d20 = (Int64)reader[2];
                            tempSislokCdm.tgl = tempDate;
                            reader.Close();
                        }
                        //Console.WriteLine(tempSislokCdm.d100.ToString());
                        sislokCdm.Add(tempSislokCdm);
                        tempDate = tempDate.AddDays(1);
                        reader.Close();
                    }
                    if (iserror)
                    {
                        message+="\n" + errMsg;
                    }
                }
                sql.Close();
                Console.WriteLine("CDM");
                foreach (var temp in sislokCdm)
                {
                    Console.WriteLine(temp.tgl + " " + temp.d100 + " " + temp.d50 + " " + temp.d20 + " ");
                }
            }
        }
        void loadSislokCdmDenganStdDeviasi()
        {
            Console.WriteLine();
            Console.WriteLine("Load Deviasi Sislok CDM");
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
                        //Sislokcdm
                        SqlDataReader reader;
                        Rasio tempStdDeviasi = new Rasio();

                        String kondisi = " WHERE kodePkt = '" + kodePkt + "' AND DATENAME(WEEKDAY, TA.Tanggal) = '" + tempDate.DayOfWeek.ToString() + "'";
                        int count = 0;

                        EventAndCondition eac = loadEventWhereCondition(tempDate);
                        if (eac.eventType != "Event 1")
                        {
                            kondisi = " WHERE kodePkt = '" + kodePkt + "' ";
                        }
                        kondisi += eac.whereCondition;
                        kondisi += ")";

                        String subqueryTblAverage = "(SELECT AVG(sislokcdm100) AS Average100 , AVG(sislokCdm50) AS Average50 , AVG(sislokCdm20) AS Average20 FROM TransaksiAtms TA JOIN EventTanggal ET ON TA.tanggal = ET.tanggal";
                        subqueryTblAverage += kondisi;
                        subqueryTblAverage += ") avt";

                        String query = "SELECT "
                                    + "[AverageStdDeviasi100] = AVG(CAST((sislokCdm100 - [Average100]) AS FLOAT) / (CASE WHEN [Average100] = 0 THEN 1 ELSE [Average100] END)), "
                                    + "[AverageStdDeviasi50] = AVG(CAST((sislokCdm50 - [Average50])AS FLOAT) / (CASE WHEN [Average50] = 0 THEN 1 ELSE [Average50] END)), "
                                    + "[AverageStdDeviasi20] = AVG(CAST((sislokCdm20 - [Average20])AS FLOAT) / (CASE WHEN [Average20] = 0 THEN 1 ELSE [Average20] END)) "
                                    + "FROM TransaksiAtms TA JOIN EventTanggal ET ON Ta.tanggal = ET.tanggal, " + subqueryTblAverage + kondisi;

                        cmd.CommandText = query;
                        reader = cmd.ExecuteReader();
                        bool event1 = true, event2 = true;
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
                Console.WriteLine("Deviasi Sislok Cdm");
                Console.WriteLine(sislokCdm.Count);
                foreach (var temp in stdDeviasi)
                {
                    Console.WriteLine(temp.tgl + " " + temp.d100 + " " + temp.d50 + " " + temp.d20 + " ");
                }
            }
            sislokCdmDenganStdDeviasi = new List<Denom>();
            for (int a = 0; a < sislokCdm.Count; a++)
            {
                Denom temp = new Denom();
                temp.d100 = sislokCdm[a].d100 + (Int64)Math.Round(stdDeviasi[a].d100 * sislokCdm[a].d100);
                temp.d50 = sislokCdm[a].d50 + (Int64)Math.Round(stdDeviasi[a].d50 * sislokCdm[a].d50);
                temp.d20 = sislokCdm[a].d20 + (Int64)Math.Round(stdDeviasi[a].d20 * sislokCdm[a].d20);
                temp.tgl = sislokCdm[a].tgl;
                sislokCdmDenganStdDeviasi.Add(temp);
            }

            Console.WriteLine("Sislok Cdm Dengan Std Deviasi");
            Console.WriteLine("===========================");
            foreach (var temp in sislokCdmDenganStdDeviasi)
            {
                Console.WriteLine(temp.tgl.ToShortDateString() + " " + temp.d100 + " " + temp.d50 + " " + temp.d20);
            }
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
