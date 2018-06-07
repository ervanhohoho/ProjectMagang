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
                    try
                    {
                        loadData(filename);
                    }
                    catch (Exception er)
                    {
                        MessageBox.Show(filename + "Error!");
                    }
                }
                loadForm.CloseForm();
                MessageBox.Show("Done!");
            }

        }

        private void loadData(String filename)
        {
            DataSet ds = Util.openExcel(filename);
            //dataGridView1.DataSource = ds.Tables[0];
            for (int i = 0; i < ds.Tables.Count; i++)
            {

                DataTable dt = ds.Tables[i];
                String sheetname = dt.TableName;

                var query3 = (from x in en.StokPosisis.AsEnumerable()
                              where x.namaPkt == ds.Tables[0].Rows[5][11].ToString()
                              && ((DateTime)x.tanggal).ToShortDateString() == Convert.ToDateTime(dt.Rows[8][1].ToString()).ToShortDateString()
                              select x).ToList();
                var query4 = (from x in en.StokPosisis.AsEnumerable()
                              where ((DateTime)x.tanggal).ToShortDateString() == Convert.ToDateTime(dt.Rows[8][1].ToString()).AddDays(-1).ToShortDateString()
                              && x.namaPkt == ds.Tables[0].Rows[5][11].ToString()
                              select x).ToList();

                Console.WriteLine(sheetname);

                if (query4.Any())
                {
                    Console.WriteLine("Query 4: " + query4.Count);
                }

                if (query3.Any())
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
                        Int64 inCabang = 0;
                        Int64 inRetail = 0;
                        Int64 outCabang = 0;
                        Int64 buf;
                        DateTime tanggal;


                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][27].ToString()) && Int64.TryParse(dt.Rows[12 + j][27].ToString(), out buf))
                            unprocessed = Int64.Parse(dt.Rows[12 + j][27].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][28].ToString()) && Int64.TryParse(dt.Rows[12 + j][28].ToString(), out buf))
                            newBaru = Int64.Parse(dt.Rows[12 + j][28].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][29].ToString()) && Int64.TryParse(dt.Rows[12 + j][29].ToString(), out buf))
                            newLama = Int64.Parse(dt.Rows[12 + j][29].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][30].ToString()) && Int64.TryParse(dt.Rows[12 + j][30].ToString(), out buf))
                            fitBaru = Int64.Parse(dt.Rows[12 + j][30].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][31].ToString()) && Int64.TryParse(dt.Rows[12 + j][31].ToString(), out buf))
                            fitNKRI = Int64.Parse(dt.Rows[12 + j][31].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][32].ToString()) && Int64.TryParse(dt.Rows[12 + j][32].ToString(), out buf))
                            fitLama = Int64.Parse(dt.Rows[12 + j][32].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][33].ToString()) && Int64.TryParse(dt.Rows[12 + j][33].ToString(), out buf))
                            passThrough = Int64.Parse(dt.Rows[12 + j][33].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][34].ToString()) && Int64.TryParse(dt.Rows[12 + j][34].ToString(), out buf))
                            unfitBaru = Int64.Parse(dt.Rows[12 + j][34].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][35].ToString()) && Int64.TryParse(dt.Rows[12 + j][35].ToString(), out buf))
                            unfitNKRI = Int64.Parse(dt.Rows[12 + j][35].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][36].ToString()) && Int64.TryParse(dt.Rows[12 + j][36].ToString(), out buf))
                            unfitLama = Int64.Parse(dt.Rows[12 + j][36].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][37].ToString()) && Int64.TryParse(dt.Rows[12 + j][37].ToString(), out buf))
                            RRMBaru = Int64.Parse(dt.Rows[12 + j][37].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][38].ToString()) && Int64.TryParse(dt.Rows[12 + j][38].ToString(), out buf))
                            RRMNKRI = Int64.Parse(dt.Rows[12 + j][38].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][39].ToString()) && Int64.TryParse(dt.Rows[12 + j][39].ToString(), out buf))
                            RRMLama = Int64.Parse(dt.Rows[12 + j][39].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][40].ToString()) && Int64.TryParse(dt.Rows[12 + j][40].ToString(), out buf))
                            RupiahRusakMayor = Int64.Parse(dt.Rows[12 + j][40].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][41].ToString()) && Int64.TryParse(dt.Rows[12 + j][41].ToString(), out buf))
                            cekLaporan = Int64.Parse(dt.Rows[12 + j][41].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][2].ToString()) && Int64.TryParse(dt.Rows[12 + j][2].ToString(), out buf))
                            inCabang = Int64.Parse(dt.Rows[12 + j][2].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][3].ToString()) && Int64.TryParse(dt.Rows[12 + j][3].ToString(), out buf))
                            inRetail = Int64.Parse(dt.Rows[12 + j][3].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][9].ToString()) && Int64.TryParse(dt.Rows[12 + j][9].ToString(), out buf))
                            outCabang = Int64.Parse(dt.Rows[12 + j][9].ToString());

                        //
                        tanggal = Convert.ToDateTime(dt.Rows[8][1].ToString());

                        var check = (from x in query3
                                     where ((DateTime)x.tanggal).Date == tanggal.Date
                                     && x.denom == dt.Rows[12 + j][0].ToString()
                                     && x.jenis == "Kertas"
                                     select x).FirstOrDefault();
                        if (check != null)
                        {
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
                        }
                        else
                        {
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
                        }
                        if (query4.Any())
                        {
                            query4[j].inCabang = inCabang;
                            query4[j].inRetail = inRetail;
                            query4[j].outCabang = outCabang;
                        }
                        else
                        {
                            en.StokPosisis.Add(new StokPosisi()
                            {
                                denom = dt.Rows[12 + j][0].ToString(),
                                namaPkt = dt.Rows[5][11].ToString(),
                                tanggal = tanggal.AddDays(-1),
                                jenis = "Kertas",
                                fitBaru = 0,
                                fitLama = 0,
                                fitNKRI = 0,
                                newBaru = 0,
                                newLama = 0,
                                passThrough = 0,
                                RRMBaru = 0,
                                RRMLama = 0,
                                RRMNKRI = 0,
                                RupiahRusakMayor = 0,
                                unfitBaru = 0,
                                unfitLama = 0,
                                unfitNKRI = 0,
                                unprocessed = 0,
                                cekLaporan = 0,
                                inCabang = inCabang,
                                inRetail = inRetail,
                                outCabang = outCabang
                            });
                        }
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
                        Int64 inCabang = 0;
                        Int64 inRetail = 0;
                        Int64 outCabang = 0;
                        Int64 buf;
                        DateTime tanggal;

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][27].ToString()) && Int64.TryParse(dt.Rows[23 + j][27].ToString(), out buf))
                            unprocessed = Int64.Parse(dt.Rows[23 + j][27].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][28].ToString()) && Int64.TryParse(dt.Rows[23 + j][28].ToString(), out buf))
                            newBaru = Int64.Parse(dt.Rows[23 + j][28].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][29].ToString()) && Int64.TryParse(dt.Rows[23 + j][29].ToString(), out buf))
                            newLama = Int64.Parse(dt.Rows[23 + j][29].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][30].ToString()) && Int64.TryParse(dt.Rows[23 + j][30].ToString(), out buf))
                            fitBaru = Int64.Parse(dt.Rows[23 + j][30].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][31].ToString()) && Int64.TryParse(dt.Rows[23 + j][31].ToString(), out buf))
                            fitNKRI = Int64.Parse(dt.Rows[23 + j][31].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][32].ToString()) && Int64.TryParse(dt.Rows[23 + j][32].ToString(), out buf))
                            fitLama = Int64.Parse(dt.Rows[23 + j][32].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][33].ToString()) && Int64.TryParse(dt.Rows[23 + j][33].ToString(), out buf))
                            passThrough = Int64.Parse(dt.Rows[23 + j][33].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][34].ToString()) && Int64.TryParse(dt.Rows[23 + j][34].ToString(), out buf))
                            unfitBaru = Int64.Parse(dt.Rows[23 + j][34].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][35].ToString()) && Int64.TryParse(dt.Rows[23 + j][35].ToString(), out buf))
                            unfitNKRI = Int64.Parse(dt.Rows[23 + j][35].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][36].ToString()) && Int64.TryParse(dt.Rows[23 + j][36].ToString(), out buf))
                            unfitLama = Int64.Parse(dt.Rows[23 + j][36].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][37].ToString()) && Int64.TryParse(dt.Rows[23 + j][37].ToString(), out buf))
                            RRMBaru = Int64.Parse(dt.Rows[23 + j][37].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][38].ToString()) && Int64.TryParse(dt.Rows[23 + j][38].ToString(), out buf))
                            RRMNKRI = Int64.Parse(dt.Rows[23 + j][38].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][39].ToString()) && Int64.TryParse(dt.Rows[23 + j][39].ToString(), out buf))
                            RRMLama = Int64.Parse(dt.Rows[23 + j][39].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][40].ToString()) && Int64.TryParse(dt.Rows[23 + j][40].ToString(), out buf))
                            RupiahRusakMayor = Int64.Parse(dt.Rows[23 + j][40].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][41].ToString()) && Int64.TryParse(dt.Rows[23 + j][41].ToString(), out buf))
                            cekLaporan = Int64.Parse(dt.Rows[23 + j][41].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][2].ToString()) && Int64.TryParse(dt.Rows[23 + j][2].ToString(), out buf))
                            inCabang = Int64.Parse(dt.Rows[23 + j][2].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][3].ToString()) && Int64.TryParse(dt.Rows[23 + j][3].ToString(), out buf))
                            inRetail = Int64.Parse(dt.Rows[23 + j][3].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][9].ToString()) && Int64.TryParse(dt.Rows[23 + j][9].ToString(), out buf))
                            outCabang = Int64.Parse(dt.Rows[23 + j][9].ToString());


                        tanggal = Convert.ToDateTime(dt.Rows[8][1].ToString());

                        var check = (from x in query3
                                     where ((DateTime)x.tanggal).Date == tanggal.Date
                                     && x.denom == dt.Rows[23 + j][0].ToString()
                                     && x.jenis == "Koin"
                                     select x).FirstOrDefault();
                        if (check != null)
                        {
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
                        }
                        else
                        {
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
                                jenis = "Koin",
                                tanggal = tanggal,
                                namaPkt = dt.Rows[5][11].ToString()
                            });
                        }

                        //});

                        if (query4.Any())
                        {
                            query4[j + 8].inCabang = inCabang;
                            query4[j + 8].inRetail = inRetail;
                            query4[j + 8].outCabang = outCabang;
                        }
                        else
                        {
                            en.StokPosisis.Add(new StokPosisi()
                            {
                                denom = dt.Rows[23 + j][0].ToString(),
                                namaPkt = dt.Rows[5][11].ToString(),
                                tanggal = tanggal.AddDays(-1),
                                jenis = "Koin",
                                fitBaru = 0,
                                fitLama = 0,
                                fitNKRI = 0,
                                newBaru = 0,
                                newLama = 0,
                                passThrough = 0,
                                RRMBaru = 0,
                                RRMLama = 0,
                                RRMNKRI = 0,
                                RupiahRusakMayor = 0,
                                unfitBaru = 0,
                                unfitLama = 0,
                                unfitNKRI = 0,
                                unprocessed = 0,
                                cekLaporan = 0,
                                inCabang = inCabang,
                                inRetail = inRetail,
                                outCabang = outCabang
                            });
                        }
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
                        Int64 inCabang = 0;
                        Int64 inRetail = 0;
                        Int64 outCabang = 0;
                        Int64 buf;
                        DateTime tanggal;

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][27].ToString()) && Int64.TryParse(dt.Rows[12 + j][27].ToString(), out buf))
                            unprocessed = Int64.Parse(dt.Rows[12 + j][27].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][28].ToString()) && Int64.TryParse(dt.Rows[12 + j][28].ToString(), out buf))
                            newBaru = Int64.Parse(dt.Rows[12 + j][28].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][29].ToString()) && Int64.TryParse(dt.Rows[12 + j][29].ToString(), out buf))
                            newLama = Int64.Parse(dt.Rows[12 + j][29].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][30].ToString()) && Int64.TryParse(dt.Rows[12 + j][30].ToString(), out buf))
                            fitBaru = Int64.Parse(dt.Rows[12 + j][30].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][31].ToString()) && Int64.TryParse(dt.Rows[12 + j][31].ToString(), out buf))
                            fitNKRI = Int64.Parse(dt.Rows[12 + j][31].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][32].ToString()) && Int64.TryParse(dt.Rows[12 + j][32].ToString(), out buf))
                            fitLama = Int64.Parse(dt.Rows[12 + j][32].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][33].ToString()) && Int64.TryParse(dt.Rows[12 + j][33].ToString(), out buf))
                            passThrough = Int64.Parse(dt.Rows[12 + j][33].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][34].ToString()) && Int64.TryParse(dt.Rows[12 + j][34].ToString(), out buf))
                            unfitBaru = Int64.Parse(dt.Rows[12 + j][34].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][35].ToString()) && Int64.TryParse(dt.Rows[12 + j][35].ToString(), out buf))
                            unfitNKRI = Int64.Parse(dt.Rows[12 + j][35].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][36].ToString()) && Int64.TryParse(dt.Rows[12 + j][36].ToString(), out buf))
                            unfitLama = Int64.Parse(dt.Rows[12 + j][36].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][37].ToString()) && Int64.TryParse(dt.Rows[12 + j][37].ToString(), out buf))
                            RRMBaru = Int64.Parse(dt.Rows[12 + j][37].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][38].ToString()) && Int64.TryParse(dt.Rows[12 + j][38].ToString(), out buf))
                            RRMNKRI = Int64.Parse(dt.Rows[12 + j][38].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][39].ToString()) && Int64.TryParse(dt.Rows[12 + j][39].ToString(), out buf))
                            RRMLama = Int64.Parse(dt.Rows[12 + j][39].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][40].ToString()) && Int64.TryParse(dt.Rows[12 + j][40].ToString(), out buf))
                            RupiahRusakMayor = Int64.Parse(dt.Rows[12 + j][40].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][41].ToString()) && Int64.TryParse(dt.Rows[12 + j][41].ToString(), out buf))
                            cekLaporan = Int64.Parse(dt.Rows[12 + j][41].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][2].ToString()) && Int64.TryParse(dt.Rows[12 + j][2].ToString(), out buf))
                            inCabang = Int64.Parse(dt.Rows[12 + j][2].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][3].ToString()) && Int64.TryParse(dt.Rows[12 + j][3].ToString(), out buf))
                            inRetail = Int64.Parse(dt.Rows[12 + j][3].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[12 + j][9].ToString()) && Int64.TryParse(dt.Rows[12 + j][9].ToString(), out buf))
                            outCabang = Int64.Parse(dt.Rows[12 + j][9].ToString());

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
                            denom = dt.Rows[12 + j][0].ToString(),
                            jenis = "Kertas",
                            tanggal = tanggal,
                            namaPkt = dt.Rows[5][11].ToString(),
                            inCabang = 0,
                            inRetail = 0,
                            outCabang = 0
                        });

                        if (query4.Any())
                        {
                            query4[j].inCabang = inCabang;
                            query4[j].inRetail = inRetail;
                            query4[j].outCabang = outCabang;
                        }
                        else
                        {
                            en.StokPosisis.Add(new StokPosisi()
                            {
                                denom = dt.Rows[12 + j][0].ToString(),
                                namaPkt = dt.Rows[5][11].ToString(),
                                tanggal = tanggal.AddDays(-1),
                                jenis = "Kertas",
                                fitBaru = 0,
                                fitLama = 0,
                                fitNKRI = 0,
                                newBaru = 0,
                                newLama = 0,
                                passThrough = 0,
                                RRMBaru = 0,
                                RRMLama = 0,
                                RRMNKRI = 0,
                                RupiahRusakMayor = 0,
                                unfitBaru = 0,
                                unfitLama = 0,
                                unfitNKRI = 0,
                                unprocessed = 0,
                                cekLaporan = 0,
                                inCabang = inCabang,
                                inRetail = inRetail,
                                outCabang = outCabang
                            });
                        }
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
                        Int64 inCabang = 0;
                        Int64 inRetail = 0;
                        Int64 outCabang = 0;
                        Int64 buf;
                        DateTime tanggal;

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][27].ToString()) && Int64.TryParse(dt.Rows[23 + j][27].ToString(), out buf))
                            unprocessed = Int64.Parse(dt.Rows[23 + j][27].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][28].ToString()) && Int64.TryParse(dt.Rows[23 + j][28].ToString(), out buf))
                            newBaru = Int64.Parse(dt.Rows[23 + j][28].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][29].ToString()) && Int64.TryParse(dt.Rows[23 + j][29].ToString(), out buf))
                            newLama = Int64.Parse(dt.Rows[23 + j][29].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][30].ToString()) && Int64.TryParse(dt.Rows[23 + j][30].ToString(), out buf))
                            fitBaru = Int64.Parse(dt.Rows[23 + j][30].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][31].ToString()) && Int64.TryParse(dt.Rows[23 + j][31].ToString(), out buf))
                            fitNKRI = Int64.Parse(dt.Rows[23 + j][31].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][32].ToString()) && Int64.TryParse(dt.Rows[23 + j][32].ToString(), out buf))
                            fitLama = Int64.Parse(dt.Rows[23 + j][32].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][33].ToString()) && Int64.TryParse(dt.Rows[23 + j][33].ToString(), out buf))
                            passThrough = Int64.Parse(dt.Rows[23 + j][33].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][34].ToString()) && Int64.TryParse(dt.Rows[23 + j][34].ToString(), out buf))
                            unfitBaru = Int64.Parse(dt.Rows[23 + j][34].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][35].ToString()) && Int64.TryParse(dt.Rows[23 + j][35].ToString(), out buf))
                            unfitNKRI = Int64.Parse(dt.Rows[23 + j][35].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][36].ToString()) && Int64.TryParse(dt.Rows[23 + j][36].ToString(), out buf))
                            unfitLama = Int64.Parse(dt.Rows[23 + j][36].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][37].ToString()) && Int64.TryParse(dt.Rows[23 + j][37].ToString(), out buf))
                            RRMBaru = Int64.Parse(dt.Rows[23 + j][37].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][38].ToString()) && Int64.TryParse(dt.Rows[23 + j][38].ToString(), out buf))
                            RRMNKRI = Int64.Parse(dt.Rows[23 + j][38].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][39].ToString()) && Int64.TryParse(dt.Rows[23 + j][39].ToString(), out buf))
                            RRMLama = Int64.Parse(dt.Rows[23 + j][39].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][40].ToString()) && Int64.TryParse(dt.Rows[23 + j][40].ToString(), out buf))
                            RupiahRusakMayor = Int64.Parse(dt.Rows[23 + j][40].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][41].ToString()) && Int64.TryParse(dt.Rows[23 + j][41].ToString(), out buf))
                            cekLaporan = Int64.Parse(dt.Rows[23 + j][41].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][2].ToString()) && Int64.TryParse(dt.Rows[23 + j][2].ToString(), out buf))
                            inCabang = Int64.Parse(dt.Rows[23 + j][2].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][3].ToString()) && Int64.TryParse(dt.Rows[23 + j][3].ToString(), out buf))
                            inRetail = Int64.Parse(dt.Rows[23 + j][3].ToString());

                        if (!String.IsNullOrEmpty(dt.Rows[23 + j][9].ToString()) && Int64.TryParse(dt.Rows[23 + j][9].ToString(), out buf))
                            outCabang = Int64.Parse(dt.Rows[23 + j][9].ToString());


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
                            namaPkt = dt.Rows[5][11].ToString(),
                            inRetail = 0,
                            inCabang = 0,
                            outCabang = 0
                        });
                        if (query4.Any())
                        {
                            query4[j + 8].inCabang = inCabang;
                            query4[j + 8].inRetail = inRetail;
                            query4[j + 8].outCabang = outCabang;
                        }
                        else
                        {
                            en.StokPosisis.Add(new StokPosisi()
                            {
                                denom = dt.Rows[23 + j][0].ToString(),
                                namaPkt = dt.Rows[5][11].ToString(),
                                tanggal = tanggal.AddDays(-1),
                                jenis = "Koin",
                                fitBaru = 0,
                                fitLama = 0,
                                fitNKRI = 0,
                                newBaru = 0,
                                newLama = 0,
                                passThrough = 0,
                                RRMBaru = 0,
                                RRMLama = 0,
                                RRMNKRI = 0,
                                RupiahRusakMayor = 0,
                                unfitBaru = 0,
                                unfitLama = 0,
                                unfitNKRI = 0,
                                unprocessed = 0,
                                cekLaporan = 0,
                                inCabang = inCabang,
                                inRetail = inRetail,
                                outCabang = outCabang
                            });
                        }
                        en.SaveChanges();
                    }
                }
            }
        }


        public void insertToDatabase()
        {
            //hayo cari apa disini
        }
    }
}