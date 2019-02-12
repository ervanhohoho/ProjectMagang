using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA.CabangMenu
{
    public partial class WPRekonSaldoForm : Form
    {
        String kodePkt;
        DateTime tanggal;
        List<PivotCPC_BIBL> listReturnBIdanBankLain = new List<PivotCPC_BIBL>();
        List<PivotPerVendor_setoran> listSetoranCabang = new List<PivotPerVendor_setoran>();
        List<PivotCPC_ATMR> listATMReturn;
        List<PivotCPC_BIBLD> listDeliveryBIdanBankLain = new List<PivotCPC_BIBLD>();
        List<PivotPerVendor_bon> listBonCabang = new List<PivotPerVendor_bon>();
        List<PivotCPC_ATMD> listATMDelivery;
        List<RekonSaldoInputanUser> rekonSaldoInputanUsers = new List<RekonSaldoInputanUser>();
        List<TampilanWPRekonSaldo> listSistem;
        List<ImportDataTambahan> listImport;
        public WPRekonSaldoForm()
        {
            InitializeComponent();
            loadComboPkt();
            pktComboBox.SelectedIndex = 0;
        }
        void loadComboPkt()
        {
            Database1Entities db = new Database1Entities();
            List<String> listKodePkt = db.Pkts.Where(x => x.kanwil.ToLower().Contains("jabo") && x.kodePktCabang.Length > 1).Select(x => x.kodePktCabang.Substring(0,4) == "CCAS"? "CCAS":x.kodePktCabang).OrderBy(x=>x).Distinct().ToList();
            pktComboBox.DataSource = listKodePkt;
        }
        private void loadBtn_Click(object sender, EventArgs e)
        {
            Database1Entities db = new Database1Entities();
            loadForm.ShowSplashScreen();
            tanggal = dateTimePicker1.Value.Date;
            ImportDataTambahanBtn.Enabled = true;
            loadTableSistem();
            loadTableInputan();
            listImport = new List<ImportDataTambahan>();
            loadForm.CloseForm();
        }

        private void loadTableSistem()
        {
            Database1Entities db = new Database1Entities();
            List<String> listKodePktCabang = db.Pkts.Select(x => x.kodePktCabang.Substring(0,4) == "CCAS" ? "CCAS" : x.kodePktCabang).Where(x => x.Length > 1).ToList();
            listSistem = new List<TampilanWPRekonSaldo>();
            Int64 validasiOptiSetoran = 0;
            Int64 belumValidasiOptiSetoran = 0;
            DateTime tanggal = dateTimePicker1.Value.Date;
            KumpulanQueryRekonSaldo kqrs = new KumpulanQueryRekonSaldo(kodePkt, tanggal);
            listReturnBIdanBankLain = kqrs.listReturnBIdanBankLain;
            listSetoranCabang = kqrs.listSetoranCabang;
            listATMReturn = kqrs.listReturnBIdanBankLain.Where(x=> !x.fundingSource.Contains("OB") && !x.fundingSource.Contains("BI")).Select(x=>new PivotCPC_ATMR() { dueDate = x.dueDate, fundingSource = x.fundingSource, emergencyReturnNotVal = x.emergencyReturnNotVal, emergencyReturnVal = x.emergencyReturnVal, grandTotal = x.grandTotal, plannedReturnNotVal = x.plannedReturnNotVal, plannedReturnVal = x.plannedReturnVal, realDate = x.realDate, vaultId = x.vaultId}).ToList();
            listDeliveryBIdanBankLain = kqrs.listDeliveryBIdanBankLain;
            listBonCabang = kqrs.listBonCabang;
            listATMDelivery = kqrs.listATMDelivery.Where(x => !x.fundingSource.Contains("OB") && !x.fundingSource.Contains("BI")).Select(x => new PivotCPC_ATMD() { dueDate = x.dueDate, fundingSource = x.fundingSource, emergencyDeliveryNotVal = x.emergencyDeliveryNotVal, emergencyDeliveryVal = x.emergencyDeliveryNotVal, grandTotal = x.grandTotal, plannedDeliveryNotVal = x.plannedDeliveryNotVal, plannedDeliveryVal = x.plannedDeliveryVal, realDate = x.realDate, vaultId = x.vaultId }).ToList();
            
            //IN
            listSetoranCabang = listSetoranCabang.GroupBy(x => x.vendor).Select(x => new PivotPerVendor_setoran() {
                vendor = x.Key,
                belumValidasi = x.Sum(z=>z.belumValidasi),
                sudahValidasi = x.Sum(z=>z.sudahValidasi),
                grandTotal = x.Sum(z=>z.grandTotal),
            }).ToList();

            //SETOR
            if (listSetoranCabang.Any())
            {
                validasiOptiSetoran = listSetoranCabang[0].sudahValidasi;
                belumValidasiOptiSetoran = listSetoranCabang[0].belumValidasi;
            }
            //Setoran Cabang untuk format sesuai WP
            listSistem.Add(new TampilanWPRekonSaldo()
            {
                in_out = "IN",
                jenis = "Setoran Cabang",
                validasi = "Belum Validasi Opti",
                value = belumValidasiOptiSetoran
            });
            listSistem.Add(new TampilanWPRekonSaldo()
            {
                in_out = "IN",
                jenis = "Setoran Cabang",
                validasi = "Validasi Opti",
                value = validasiOptiSetoran
            });
            //BI Return untuk format sesuai WP
            listSistem.AddRange(listReturnBIdanBankLain.Where(x => (x.fundingSource == null ? false : x.fundingSource.ToLower() == "bi")).Select(x => new TampilanWPRekonSaldo()
            {
                in_out = "OUT",
                jenis = "BI Return",
                validasi = "Belum Validasi",
                value = x.plannedReturnNotVal + x.emergencyReturnNotVal
            }));
            listSistem.AddRange(listReturnBIdanBankLain.Where(x => (x.fundingSource == null ? false : x.fundingSource.ToLower() == "bi")).Select(x => new TampilanWPRekonSaldo()
            {
                in_out = "OUT",
                jenis = "BI Return",
                validasi = "Validasi",
                value = x.plannedReturnVal + x.emergencyReturnVal
            }));
            //Bank Lain Return untuk format sesuai WP
            listSistem.AddRange(listReturnBIdanBankLain.Where(x => (x.fundingSource == null ? false : x.fundingSource.ToLower().Contains("ob"))).Select(x => new TampilanWPRekonSaldo()
            {
                in_out = "OUT",
                jenis = "Bank Lain Return",
                validasi = "Belum Validasi",
                value = x.plannedReturnNotVal + x.emergencyReturnNotVal
            }));
            listSistem.AddRange(listReturnBIdanBankLain.Where(x => (x.fundingSource == null ? false : x.fundingSource.ToLower().Contains("ob"))).Select(x => new TampilanWPRekonSaldo()
            {
                in_out = "OUT",
                jenis = "Bank Lain Return",
                validasi = "Validasi",
                value = x.plannedReturnVal + x.emergencyReturnVal
            }));
            //ATM Return untuk format sesuai WP
            listSistem.AddRange(listATMReturn
                .Where(x => !listKodePktCabang.Where(y => y == x.fundingSource).ToList().Any())
                .Select(x => new TampilanWPRekonSaldo()
                {
                    in_out = "OUT",
                    jenis = "ATM Return",
                    validasi = "Belum Validasi",
                    value = x.plannedReturnNotVal + x.emergencyReturnNotVal
                }).GroupBy(x => new { x.in_out, x.jenis, x.validasi })
                .Select(x => new TampilanWPRekonSaldo()
                {
                    in_out = x.Key.in_out,
                    jenis = x.Key.jenis,
                    validasi = x.Key.validasi,
                    value = x.Sum(y => y.value)
                }));
            listSistem.AddRange(listATMReturn
                .Where(x => !listKodePktCabang.Where(y => y == x.fundingSource).ToList().Any())
                .Select(x => new TampilanWPRekonSaldo()
                {
                    in_out = "OUT",
                    jenis = "ATM Return",
                    validasi = "Validasi",
                    value = x.plannedReturnVal + x.emergencyReturnVal
                })
                .GroupBy(x => new { x.in_out, x.jenis, x.validasi })
                .Select(x => new TampilanWPRekonSaldo()
                {
                    in_out = x.Key.in_out,
                    jenis = x.Key.jenis,
                    validasi = x.Key.validasi,
                    value = x.Sum(y => y.value)
                }));


            //BON
            Int64 validasiOptiBon = 0;
            Int64 belumValidasiOptiBon = 0;

            listBonCabang = listBonCabang.GroupBy(x => x.vendor).Select(x=>new PivotPerVendor_bon() {
                vendor = x.Key,
                sudahValidasi = x.Sum(z=>z.sudahValidasi),
                belumValidasi = x.Sum(z=>z.belumValidasi),
                grandTotal = x.Sum(z => z.grandTotal),
            }).ToList();
            if (listBonCabang.Any())
            {
                validasiOptiBon = listBonCabang[0].sudahValidasi;
                belumValidasiOptiBon = listBonCabang[0].belumValidasi;
            }
            //Setoran Cabang untuk format sesuai WP
            listSistem.Add(new TampilanWPRekonSaldo()
            {
                in_out = "OUT",
                jenis = "Bon Cabang",
                validasi = "Belum Validasi Opti",
                value = belumValidasiOptiBon
            });
            listSistem.Add(new TampilanWPRekonSaldo()
            {
                in_out = "OUT",
                jenis = "Bon Cabang",
                validasi = "Validasi Opti",
                value = validasiOptiBon
            });
            //BI Return untuk format sesuai WP
            listSistem.AddRange(listDeliveryBIdanBankLain.Where(x => (x.fundingSource == null ? false : x.fundingSource.ToLower() == "bi")).Select(x => new TampilanWPRekonSaldo()
            {
                in_out = "IN",
                jenis = "BI Delivery",
                validasi = "Belum Validasi",
                value = x.plannedDeliveryNotVal + x.emergencyDeliveryNotVal
            }));
            listSistem.AddRange(listDeliveryBIdanBankLain.Where(x => (x.fundingSource == null ? false : x.fundingSource.ToLower() == "bi")).Select(x => new TampilanWPRekonSaldo()
            {
                in_out = "IN",
                jenis = "BI Return",
                validasi = "Validasi",
                value = x.plannedDeliveryVal + x.emergencyDeliveryVal
            }));
            //Bank Lain Return untuk format sesuai WP
            listSistem.AddRange(listDeliveryBIdanBankLain.Where(x => (x.fundingSource == null ? false : x.fundingSource.ToLower().Contains("ob"))).Select(x => new TampilanWPRekonSaldo()
            {
                in_out = "IN",
                jenis = "Bank Lain Delivery",
                validasi = "Belum Validasi",
                value = x.plannedDeliveryNotVal + x.emergencyDeliveryNotVal
            }));
            listSistem.AddRange(listDeliveryBIdanBankLain.Where(x => (x.fundingSource == null ? false : x.fundingSource.ToLower().Contains("ob"))).Select(x => new TampilanWPRekonSaldo()
            {
                in_out = "IN",
                jenis = "Bank Lain Delivery",
                validasi = "Validasi",
                value = x.plannedDeliveryNotVal + x.emergencyDeliveryNotVal
            }));

            //ATM Return untuk format sesuai WP
            listSistem.AddRange(listATMDelivery
                .Where(x => !listKodePktCabang.Where(y => y == x.fundingSource).ToList().Any())
                .Select(x => new TampilanWPRekonSaldo()
                {
                    in_out = "IN",
                    jenis = "ATM Delivery",
                    validasi = "Belum Validasi",
                    value = x.plannedDeliveryNotVal + x.emergencyDeliveryNotVal
                }).GroupBy(x => new { x.in_out, x.jenis, x.validasi })
                .Select(x => new TampilanWPRekonSaldo()
                {
                    in_out = x.Key.in_out,
                    jenis = x.Key.jenis,
                    validasi = x.Key.validasi,
                    value = x.Sum(y => y.value)
                }));
            listSistem.AddRange(listATMDelivery
                .Where(x => !listKodePktCabang.Where(y => y == x.fundingSource).ToList().Any())
                .Select(x => new TampilanWPRekonSaldo()
                {
                    in_out = "IN",
                    jenis = "ATM Delivery",
                    validasi = "Validasi",
                    value = x.plannedDeliveryNotVal + x.emergencyDeliveryNotVal
                })
                .GroupBy(x => new { x.in_out, x.jenis, x.validasi })
                .Select(x => new TampilanWPRekonSaldo()
                {
                    in_out = x.Key.in_out,
                    jenis = x.Key.jenis,
                    validasi = x.Key.validasi,
                    value = x.Sum(y => y.value)
                }));


            listSistem = listSistem.GroupBy(x => new { x.in_out, x.jenis, x.validasi }).Select(x => new TampilanWPRekonSaldo() { in_out = x.Key.in_out, jenis = x.Key.jenis, validasi = x.Key.validasi, value = x.Sum(z=>z.value) }).ToList();
            dataSistemGridView.DataSource = listSistem;
            dataSistemGridView.Columns[3].DefaultCellStyle.Format = "C0";
            dataSistemGridView.Columns[3].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
        }
        private void loadTableInputan()
        {
            Database1Entities db = new Database1Entities();
            dataTambahanGridView.Rows.Clear();
            dataTambahanGridView.Columns.Clear();
            //Bentuk format table awal
            DateTime tanggal = dateTimePicker1.Value.Date;
            List<String> listJenis = new List<String>()
            { "Cabang",
              "Retail",
              "Curex",
              "ATM/CDM",
              "BI",
              "Bank Lain",
              "Luar Kota",
              "Antar CPC",
              "Lain Lain"
            };
            List<String> in_out = new List<String>()
            {
                "in",
                "out"
            };

            DataGridViewComboBoxColumn colJenis = new DataGridViewComboBoxColumn()
            {
                Name = "jenis",
                HeaderText = "Jenis",
                DataSource = listJenis
            };
            DataGridViewComboBoxColumn colIn_out = new DataGridViewComboBoxColumn()
            {
                Name = "in_out",
                HeaderText = "IN/OUT",
                DataSource = in_out
            };

            dataTambahanGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "id",
                HeaderText = "ID",
                Width = 0,
                ReadOnly = true
            });
            dataTambahanGridView.Columns.Add(colIn_out);
            dataTambahanGridView.Columns.Add(colJenis);
            dataTambahanGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "value",
                HeaderText = "Value",
                CellTemplate = new DataGridViewTextBoxCell() { Value = 0 },
                ValueType = typeof(Int64),
                DefaultCellStyle = new DataGridViewCellStyle() { Format = "C0", FormatProvider = CultureInfo.GetCultureInfo("id-ID")}
            });
            dataTambahanGridView.Columns.Add(new DataGridViewTextBoxColumn() {
                Name = "keterangan",
                HeaderText = "Keterangan",
                CellTemplate = new DataGridViewTextBoxCell() { Value = "" },
            });

            //Load Data Existing
            List<RekonSaldoInputanUser> dataExisting = db.RekonSaldoInputanUsers.Where(x => x.tanggal == tanggal && x.kodePkt == kodePkt).ToList();
            List<DataGridViewRow> rows = new List<DataGridViewRow>();
            foreach (var temp in dataExisting)
            {
                rows.Add(new DataGridViewRow());
                rows[rows.Count - 1].CreateCells(dataTambahanGridView, temp.Id, temp.in_out, temp.jenis, temp.value, temp.keterangan);
                //dataTambahanGridView.Rows.Add(row);
            }
            dataTambahanGridView.Rows.AddRange(rows.ToArray());
        }
        private void pktComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            kodePkt = pktComboBox.SelectedItem.ToString();
        }
        private void loadDataInputanUser()
        {
            DateTime tanggal = dateTimePicker1.Value.Date;
            rekonSaldoInputanUsers = new List<RekonSaldoInputanUser>();
            for (int a = 0; a < dataTambahanGridView.Rows.Count - 1; a++)
            {
                DataGridViewRow temp = dataTambahanGridView.Rows[a];
                String id = temp.Cells["id"].Value == null ? "-1" : temp.Cells["id"].Value.ToString(),
                    in_out = temp.Cells["in_out"].Value.ToString(),
                    jenis = temp.Cells["jenis"].Value.ToString(),
                    value = temp.Cells["value"].Value.ToString(),
                    keterangan = temp.Cells["keterangan"].Value == null ? "" : temp.Cells["keterangan"].Value.ToString();
                Console.WriteLine(id + " " + in_out + " " + jenis + " " + value);
                rekonSaldoInputanUsers.Add(new RekonSaldoInputanUser()
                {
                    Id = Int32.Parse(id),
                    in_out = in_out.ToUpper(),
                    jenis = jenis,
                    tanggal = tanggal,
                    kodePkt = kodePkt,
                    keterangan = keterangan,
                    value = Int64.Parse(value)
                });
            }
        }
        private void saveBtn_Click(object sender, EventArgs e)
        {
            Database1Entities db = new Database1Entities();
            loadDataInputanUser();
            var toInputs = rekonSaldoInputanUsers.Where(x => x.Id == -1).ToList();
            db.RekonSaldoInputanUsers.AddRange(toInputs);
            var toUpdates = rekonSaldoInputanUsers.Where(x => x.Id != -1).ToList();
            foreach(var toUpdate in toUpdates)
            {
                var datadb = db.RekonSaldoInputanUsers.Where(x => x.Id == toUpdate.Id).FirstOrDefault();
                datadb.in_out = toUpdate.in_out;
                datadb.jenis = toUpdate.jenis;
                datadb.value = toUpdate.value;
                datadb.keterangan = toUpdate.keterangan;
            }
            db.SaveChanges();
        }
        

        private void ImportDataTambahanBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.excelFilter;
            if(of.ShowDialog() == DialogResult.OK)
            {
                listImport = new List<ImportDataTambahan>();
                Int64 buf;
                DataSet ds = Util.openExcel(of.FileName);
                if (ds.Tables.Count == 0)
                    return;
                DataTable dt = ds.Tables[0];
                var toDelete = dt.Select("Column0 is null OR Column1 is null OR Column2 is null OR Column3 is null");
                foreach (var temp in toDelete)
                    dt.Rows.Remove(temp);
                for (int a = 1; a < dt.Rows.Count; a++)
                {
                    DataRow row = dt.Rows[a];
                    listImport.Add(new ImportDataTambahan() {
                        in_out = row[0].ToString(),
                        jenis = row[1].ToString(),
                        keterangan = row[2].ToString(),
                        value = (Int64.TryParse(row[3].ToString(), out buf) ? buf : 0)
                    });
                }
                foreach (var temp in listImport)
                {
                    DataGridViewRow dr = (DataGridViewRow)dataTambahanGridView.Rows[0].Clone();
                    dr.Cells[1].Value = temp.in_out;
                    dr.Cells[2].Value = temp.jenis;
                    dr.Cells[3].Value = temp.value;
                    dr.Cells[4].Value = temp.keterangan;
                    dr.ReadOnly = true;
                    dataTambahanGridView.Rows.Add(dr);
                }
            }
        }
        private void loadPerbandinganButton_Click(object sender, EventArgs e)
        {
            Database1Entities db = new Database1Entities();
            loadDataInputanUser();
            Dictionary<string, string> mapOpti = new Dictionary<string, string>();
            mapOpti.Add("Setoran Cabang", "Cabang");
            mapOpti.Add("BI Return", "BI");
            mapOpti.Add("Bank Lain Return", "Bank Lain");
            mapOpti.Add("ATM Return", "ATM");
            mapOpti.Add("Bon Cabang", "Cabang");
            mapOpti.Add("BI Delivery", "BI");
            mapOpti.Add("Bank Lain Delivery", "Bank Lain");
            mapOpti.Add("ATM Delivery", "ATM");


            String bufString;
            var allDataOpti = listSistem.GroupBy(x => new { x.in_out, jenis = (mapOpti.TryGetValue(x.jenis, out bufString) ? bufString : "")}).Select(x => new { x.Key.in_out, x.Key.jenis, value = x.Sum(y => y.value) }).ToList();

            allDataOpti.AddRange(rekonSaldoInputanUsers.GroupBy(x => new { x.in_out, x.jenis }).Select(x => new { x.Key.in_out, x.Key.jenis, value = (Int64)x.Sum(y => y.value) }).ToList());
            allDataOpti = allDataOpti.GroupBy(x => new { x.in_out, x.jenis }).Select(x => new { x.Key.in_out, x.Key.jenis, value = (Int64)x.Sum(y => y.value) }).ToList();


            Dictionary<string, string> mapDailystock = new Dictionary<string, string>();
            mapDailystock.Add("Collection Cabang", "Cabang");
            mapDailystock.Add("Delivery BI", "BI");
            mapDailystock.Add("Tukaran", "Tukaran");
            mapDailystock.Add("ATM", "ATM");
            mapDailystock.Add("Delivery Cabang", "Cabang");
            mapDailystock.Add("Collection BI", "BI");

            var dailyStocks = db.DailyStocks.Where(x => x.kodePkt == kodePkt && x.tanggal == tanggal).Select(x=> new {
                in_out = x.in_out,
                jenis = (mapDailystock.Where(y=>x.jenisTransaksi.Contains(y.Key)).Select(y=>y.Value).FirstOrDefault() == null ? "" : mapDailystock.Where(y => x.jenisTransaksi.Contains(y.Key)).Select(y => y.Value).FirstOrDefault()),
                value = x.BN100K + x.BN50K + x.BN20K + x.BN10K + x.BN5K + x.BN2K + x.BN1K + x.BN500 + x.BN200 + x.CN1K + x.CN500 + x.CN200 + x.CN100 + x.CN50 + x.CN25 
            }).ToList();

            dailyStocks = dailyStocks.GroupBy(x => new { x.in_out, x.jenis }).Select(x => new { x.Key.in_out, x.Key.jenis, value = x.Sum(y => y.value) }).ToList();

            dataGridView1.DataSource = allDataOpti;
        }
        class TampilanWPRekonSaldo
        {
            public String in_out { set; get; }
            public String jenis { set; get; }
            public String validasi { set; get; }
            public Int64 value { set; get; }
        }
        public class ImportDataTambahan
        {
            public String in_out { set; get; }
            public String jenis { set; get; }
            public String keterangan { set; get; }
            public Int64 value { set; get; }
        }

        
    }
   
}

