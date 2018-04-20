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
    public partial class StokPosisiForm : Form
    {
        Database1Entities en = new Database1Entities();

        public StokPosisiForm()
        {
            InitializeComponent();
        }

        private void buttonSelectFiles_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            //of.Multiselect = true;
            of.Filter = "Microsoft Excel | *.xls; *xlsx; *xlsm";
            of.Multiselect = true;
            if (of.ShowDialog() == DialogResult.OK)
            {
                loadForm.ShowSplashScreen();
                String[] filenames = of.FileNames;
                foreach (var filename in filenames)
                {
                    DataSet ds = Util.openExcel(filename);
                    //dataGridView1.DataSource = ds.Tables[0];
                    for (int i = 0; i < ds.Tables.Count - 1; i++)
                    {
                        Console.WriteLine(i);
                        DataTable dt = ds.Tables[i];
                        String sheetname = dt.TableName;
                        var query = (from x in en.StokPosisis.AsEnumerable()
                                     where x.namaPkt == ds.Tables[0].Rows[5][11].ToString()
                                     select x).ToList();
                        var query2 = (from x in query
                                      where ((DateTime)x.tanggal).ToShortDateString() == Convert.ToDateTime(dt.Rows[8][1].ToString()).ToShortDateString()
                                      select x).FirstOrDefault();

                        var query3 = (from x in en.StokPosisis.AsEnumerable()
                                      where x.namaPkt == ds.Tables[0].Rows[5][11].ToString() && ((DateTime)x.tanggal).ToShortDateString() == Convert.ToDateTime(dt.Rows[8][1].ToString()).ToShortDateString()
                                      select x).ToList();
                        if (query3.Count() != 0)
                        {
                            //start of penimpaan
                            //Console.WriteLine(query);
                            //continue
                            for (int j = 0; j < 8; j++)
                            {
                                Int64 unprocessed = 0;
                                Int64 newBaru = 0;
                                Int64 newLama = 0;
                                Int64 fitBaru = 0;
                                Int64 fitNKRI = 0;
                                Int64 fitLama = 0;
                                Int64 passThrough = 0;
                                Int64 unfitBaru = 0;
                                Int64 unfitNKRI = 0;
                                Int64 unfitLama = 0;
                                Int64 RRMBaru = 0;
                                Int64 RRMLama = 0;
                                Int64 RRMNKRI = 0;
                                Int64 RupiahRusakMayor = 0;
                                Int64 cekLaporan = 0;
                                DateTime tanggal;


                                if (!String.IsNullOrEmpty(dt.Rows[12 + j][27].ToString()))
                                    unprocessed = Int64.Parse(dt.Rows[12 + j][27].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[12 + j][28].ToString()))
                                    newBaru = Int64.Parse(dt.Rows[12 + j][28].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[12 + j][29].ToString()))
                                    newLama = Int64.Parse(dt.Rows[12 + j][29].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[12 + j][30].ToString()))
                                    fitBaru = Int64.Parse(dt.Rows[12 + j][30].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[12 + j][31].ToString()))
                                    fitNKRI = Int64.Parse(dt.Rows[12 + j][31].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[12 + j][32].ToString()))
                                    fitLama = Int64.Parse(dt.Rows[12 + j][32].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[12 + j][33].ToString()))
                                    passThrough = Int64.Parse(dt.Rows[12 + j][33].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[12 + j][34].ToString()))
                                    unfitBaru = Int64.Parse(dt.Rows[12 + j][34].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[12 + j][35].ToString()))
                                    unfitNKRI = Int64.Parse(dt.Rows[12 + j][35].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[12 + j][36].ToString()))
                                    unfitLama = Int64.Parse(dt.Rows[12 + j][36].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[12 + j][37].ToString()))
                                    RRMBaru = Int64.Parse(dt.Rows[12 + j][37].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[12 + j][38].ToString()))
                                    RRMNKRI = Int64.Parse(dt.Rows[12 + j][38].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[12 + j][39].ToString()))
                                    RRMLama = Int64.Parse(dt.Rows[12 + j][39].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[12 + j][40].ToString()))
                                    RupiahRusakMayor = Int64.Parse(dt.Rows[12 + j][40].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[12 + j][41].ToString()))
                                    cekLaporan = Int64.Parse(dt.Rows[12 + j][41].ToString());

                                //
                                tanggal = Convert.ToDateTime(dt.Rows[8][1].ToString());

                                //

                                //en.StokPosisis.Add(new StokPosisi()
                                //{

                                query3[j].unprocessed = unprocessed;
                                query3[j].newBaru = newBaru;
                                query3[j].newLama = newLama;
                                query3[j].fitBaru = fitBaru;
                                query3[j].fitNKRI = fitNKRI;
                                query3[j].fitLama = fitLama;
                                query3[j].passThrough = passThrough;
                                query3[j].unfitBaru = unfitBaru;
                                query3[j].unfitNKRI = unfitNKRI;
                                query3[j].unfitLama = unfitLama;
                                query3[j].RRMBaru = RRMBaru;
                                query3[j].RRMNKRI = RRMNKRI;
                                query3[j].RRMLama = RRMLama;
                                query3[j].RupiahRusakMayor = RupiahRusakMayor;
                                query3[j].cekLaporan = cekLaporan;
                                query3[j].denom = dt.Rows[12 + j][0].ToString();
                                query3[j].jenis = "Kertas";
                                query3[j].tanggal = tanggal;
                                query3[j].namaPkt = dt.Rows[5][11].ToString();

                                //});
                                en.SaveChanges();

                            }
                            //koin

                            for (int j = 0; j < 6; j++)
                            {
                                Int64 unprocessed = 0;
                                Int64 newBaru = 0;
                                Int64 newLama = 0;
                                Int64 fitBaru = 0;
                                Int64 fitNKRI = 0;
                                Int64 fitLama = 0;
                                Int64 passThrough = 0;
                                Int64 unfitBaru = 0;
                                Int64 unfitNKRI = 0;
                                Int64 unfitLama = 0;
                                Int64 RRMBaru = 0;
                                Int64 RRMLama = 0;
                                Int64 RRMNKRI = 0;
                                Int64 RupiahRusakMayor = 0;
                                Int64 cekLaporan = 0;
                                DateTime tanggal;

                                if (!String.IsNullOrEmpty(dt.Rows[23 + j][27].ToString()))
                                    unprocessed = Int64.Parse(dt.Rows[23 + j][27].ToString());



                                if (!String.IsNullOrEmpty(dt.Rows[23 + j][28].ToString()))
                                    newBaru = Int64.Parse(dt.Rows[23 + j][28].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[23 + j][29].ToString()))
                                    newLama = Int64.Parse(dt.Rows[23 + j][29].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[23 + j][30].ToString()))
                                    fitBaru = Int64.Parse(dt.Rows[23 + j][30].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[23 + j][31].ToString()))
                                    fitNKRI = Int64.Parse(dt.Rows[23 + j][31].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[23 + j][32].ToString()))
                                    fitLama = Int64.Parse(dt.Rows[23 + j][32].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[23 + j][33].ToString()))
                                    passThrough = Int64.Parse(dt.Rows[23 + j][33].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[23 + j][34].ToString()))
                                    unfitBaru = Int64.Parse(dt.Rows[23 + j][34].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[23 + j][35].ToString()))
                                    unfitNKRI = Int64.Parse(dt.Rows[23 + j][35].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[23 + j][36].ToString()))
                                    unfitLama = Int64.Parse(dt.Rows[23 + j][36].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[23 + j][37].ToString()))
                                    RRMBaru = Int64.Parse(dt.Rows[23 + j][37].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[23 + j][38].ToString()))
                                    RRMNKRI = Int64.Parse(dt.Rows[23 + j][38].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[23 + j][39].ToString()))
                                    RRMLama = Int64.Parse(dt.Rows[23 + j][39].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[23 + j][40].ToString()))
                                    RupiahRusakMayor = Int64.Parse(dt.Rows[23 + j][40].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[23 + j][41].ToString()))
                                    cekLaporan = Int64.Parse(dt.Rows[23 + j][41].ToString());

                                tanggal = Convert.ToDateTime(dt.Rows[8][1].ToString());

                                //en.StokPosisis.Add(new StokPosisi()
                                //{
                                query3[j + 8].unprocessed = unprocessed;
                                query3[j + 8].newBaru = newBaru;
                                query3[j + 8].newLama = newLama;
                                query3[j + 8].fitBaru = fitBaru;
                                query3[j + 8].fitNKRI = fitNKRI;
                                query3[j + 8].fitLama = fitLama;
                                query3[j + 8].passThrough = passThrough;
                                query3[j + 8].unfitBaru = unfitBaru;
                                query3[j + 8].unfitNKRI = unfitNKRI;
                                query3[j + 8].unfitLama = unfitLama;
                                query3[j + 8].RRMBaru = RRMBaru;
                                query3[j + 8].RRMNKRI = RRMNKRI;
                                query3[j + 8].RRMLama = RRMLama;
                                query3[j + 8].RupiahRusakMayor = RupiahRusakMayor;
                                query3[j + 8].cekLaporan = cekLaporan;
                                query3[j + 8].denom = dt.Rows[23 + j][0].ToString();
                                query3[j + 8].jenis = "Koin";
                                query3[j + 8].tanggal = tanggal;
                                query3[j + 8].namaPkt = dt.Rows[5][11].ToString();
                                //});
                                en.SaveChanges();
                            }
                            //continue;
                            //end of penimpaan
                            //continue;
                        }

                        //kertas
                        else
                        {
                            for (int j = 0; j < 8; j++)
                            {

                                Int64 unprocessed = 0;
                                Int64 newBaru = 0;
                                Int64 newLama = 0;
                                Int64 fitBaru = 0;
                                Int64 fitNKRI = 0;
                                Int64 fitLama = 0;
                                Int64 passThrough = 0;
                                Int64 unfitBaru = 0;
                                Int64 unfitNKRI = 0;
                                Int64 unfitLama = 0;
                                Int64 RRMBaru = 0;
                                Int64 RRMLama = 0;
                                Int64 RRMNKRI = 0;
                                Int64 RupiahRusakMayor = 0;
                                Int64 cekLaporan = 0;
                                //
                                DateTime tanggal;

                                if (!String.IsNullOrEmpty(dt.Rows[12 + j][27].ToString()))
                                    unprocessed = Int64.Parse(dt.Rows[12 + j][27].ToString());


                                if (!String.IsNullOrEmpty(dt.Rows[12 + j][28].ToString()))
                                    newBaru = Int64.Parse(dt.Rows[12 + j][28].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[12 + j][29].ToString()))
                                    newLama = Int64.Parse(dt.Rows[12 + j][29].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[12 + j][30].ToString()))
                                    fitBaru = Int64.Parse(dt.Rows[12 + j][30].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[12 + j][31].ToString()))
                                    fitNKRI = Int64.Parse(dt.Rows[12 + j][31].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[12 + j][32].ToString()))
                                    fitLama = Int64.Parse(dt.Rows[12 + j][32].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[12 + j][33].ToString()))
                                    passThrough = Int64.Parse(dt.Rows[12 + j][33].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[12 + j][34].ToString()))
                                    unfitBaru = Int64.Parse(dt.Rows[12 + j][34].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[12 + j][35].ToString()))
                                    unfitNKRI = Int64.Parse(dt.Rows[12 + j][35].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[12 + j][36].ToString()))
                                    unfitLama = Int64.Parse(dt.Rows[12 + j][36].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[12 + j][37].ToString()))
                                    RRMBaru = Int64.Parse(dt.Rows[12 + j][37].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[12 + j][38].ToString()))
                                    RRMNKRI = Int64.Parse(dt.Rows[12 + j][38].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[12 + j][39].ToString()))
                                    RRMLama = Int64.Parse(dt.Rows[12 + j][39].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[12 + j][40].ToString()))
                                    RupiahRusakMayor = Int64.Parse(dt.Rows[12 + j][40].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[12 + j][41].ToString()))
                                    cekLaporan = Int64.Parse(dt.Rows[12 + j][41].ToString());

                                //
                                tanggal = Convert.ToDateTime(dt.Rows[8][1].ToString());

                                //



                                en.StokPosisis.Add(new StokPosisi()
                                {
                                    unprocessed = unprocessed,
                                    newBaru = newBaru,
                                    newLama = newLama,
                                    fitBaru = fitBaru,
                                    fitNKRI = fitNKRI,
                                    fitLama = fitLama,
                                    passThrough = passThrough,
                                    unfitBaru = unfitBaru,
                                    unfitNKRI = unfitNKRI,
                                    unfitLama = unfitLama,
                                    RRMBaru = RRMBaru,
                                    RRMNKRI = RRMNKRI,
                                    RRMLama = RRMLama,
                                    RupiahRusakMayor = RupiahRusakMayor,
                                    cekLaporan = cekLaporan,
                                    denom = dt.Rows[12 + j][0].ToString(),
                                    jenis = "Kertas",
                                    tanggal = tanggal,
                                    namaPkt = dt.Rows[5][11].ToString()

                                });
                                en.SaveChanges();
                            }
                            //koin

                            for (int j = 0; j < 6; j++)
                            {
                                Int64 unprocessed = 0;
                                Int64 newBaru = 0;
                                Int64 newLama = 0;
                                Int64 fitBaru = 0;
                                Int64 fitNKRI = 0;
                                Int64 fitLama = 0;
                                Int64 passThrough = 0;
                                Int64 unfitBaru = 0;
                                Int64 unfitNKRI = 0;
                                Int64 unfitLama = 0;
                                Int64 RRMBaru = 0;
                                Int64 RRMLama = 0;
                                Int64 RRMNKRI = 0;
                                Int64 RupiahRusakMayor = 0;
                                Int64 cekLaporan = 0;
                                DateTime tanggal;

                                if (!String.IsNullOrEmpty(dt.Rows[23 + j][27].ToString()))
                                    unprocessed = Int64.Parse(dt.Rows[23 + j][27].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[23 + j][28].ToString()))
                                    newBaru = Int64.Parse(dt.Rows[23 + j][28].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[23 + j][29].ToString()))
                                    newLama = Int64.Parse(dt.Rows[23 + j][29].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[23 + j][30].ToString()))
                                    fitBaru = Int64.Parse(dt.Rows[23 + j][30].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[23 + j][31].ToString()))
                                    fitNKRI = Int64.Parse(dt.Rows[23 + j][31].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[23 + j][32].ToString()))
                                    fitLama = Int64.Parse(dt.Rows[23 + j][32].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[23 + j][33].ToString()))
                                    passThrough = Int64.Parse(dt.Rows[23 + j][33].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[23 + j][34].ToString()))
                                    unfitBaru = Int64.Parse(dt.Rows[23 + j][34].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[23 + j][35].ToString()))
                                    unfitNKRI = Int64.Parse(dt.Rows[23 + j][35].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[23 + j][36].ToString()))
                                    unfitLama = Int64.Parse(dt.Rows[23 + j][36].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[23 + j][37].ToString()))
                                    RRMBaru = Int64.Parse(dt.Rows[23 + j][37].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[23 + j][38].ToString()))
                                    RRMNKRI = Int64.Parse(dt.Rows[23 + j][38].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[23 + j][39].ToString()))
                                    RRMLama = Int64.Parse(dt.Rows[23 + j][39].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[23 + j][40].ToString()))
                                    RupiahRusakMayor = Int64.Parse(dt.Rows[23 + j][40].ToString());

                                if (!String.IsNullOrEmpty(dt.Rows[23 + j][41].ToString()))
                                    cekLaporan = Int64.Parse(dt.Rows[23 + j][41].ToString());

                                tanggal = Convert.ToDateTime(dt.Rows[8][1].ToString());

                                en.StokPosisis.Add(new StokPosisi()
                                {
                                    unprocessed = unprocessed,
                                    newBaru = newBaru,
                                    newLama = newLama,
                                    fitBaru = fitBaru,
                                    fitNKRI = fitNKRI,
                                    fitLama = fitLama,
                                    passThrough = passThrough,
                                    unfitBaru = unfitBaru,
                                    unfitNKRI = unfitNKRI,
                                    unfitLama = unfitLama,
                                    RRMBaru = RRMBaru,
                                    RRMNKRI = RRMNKRI,
                                    RRMLama = RRMLama,
                                    RupiahRusakMayor = RupiahRusakMayor,
                                    cekLaporan = cekLaporan,
                                    denom = dt.Rows[23 + j][0].ToString(),
                                    jenis = "Koin",
                                    tanggal = tanggal,
                                    namaPkt = dt.Rows[5][11].ToString()
                                });
                                en.SaveChanges();
                            }
                        }
                    }
                }
                loadForm.CloseForm();
            }
          
        }


        public void insertToDatabase()
        {
            //hayo cari apa disini
        }

    }
}