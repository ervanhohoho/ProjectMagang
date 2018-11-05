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
    public partial class RekonSaldoForm : Form
    {
        String kodePkt;
        List<PivotCPC_BIBL> listReturnBIdanBankLain = new List<PivotCPC_BIBL>();
        List<PivotPerVendor_setoran> listSetoranCabang = new List<PivotPerVendor_setoran>();
        List<PivotCPC_ATMR> listATMReturn;
        List<PivotCPC_BIBLD> listDeliveryBIdanBankLain = new List<PivotCPC_BIBLD>();
        List<PivotPerVendor_bon> listBonCabang = new List<PivotPerVendor_bon>();
        List<PivotCPC_ATMD> listATMDelivery;
        List<TampilanRekonSaldo> listSistem;
        
        public RekonSaldoForm()
        {
            InitializeComponent();
            loadComboPkt();
            pktComboBox.SelectedIndex = 0;
        }
        void loadComboPkt()
        {
            Database1Entities db = new Database1Entities();
            List<String> listKodePkt = db.Pkts.Where(x => x.kanwil.ToLower().Contains("jabo") && x.kodePktCabang.Length > 1).Select(x => x.kodePktCabang).OrderBy(x=>x).ToList();
            pktComboBox.DataSource = listKodePkt;
        }
        private void loadBtn_Click(object sender, EventArgs e)
        {
            Database1Entities db = new Database1Entities();
            loadTableSistem();
            loadTableInputan();
        }

        private void loadTableSistem()
        {
            Database1Entities db = new Database1Entities();
            List<String> listKodePktCabang = db.Pkts.Select(x => x.kodePktCabang).Where(x => x.Length > 1).ToList();
            listSistem = new List<TampilanRekonSaldo>();
            Int64 validasiOptiSetoran = 0;
            Int64 belumValidasiOptiSetoran = 0;
            loadListReturnBIdanBankLain();
            loadListSetoranCabang();
            loadListATMReturn();
            loadListDeliveryBIdanBankLain();
            loadListBonCabang();
            loadListATMDelivery();

            //SETOR
            if (listSetoranCabang.Any())
            {
                validasiOptiSetoran = listSetoranCabang[0].sudahValidasi;
                belumValidasiOptiSetoran = listSetoranCabang[0].belumValidasi;
            }
            //Setoran Cabang untuk format sesuai WP
            listSistem.Add(new TampilanRekonSaldo()
            {
                in_out = "IN",
                jenis = "Setoran Cabang",
                validasi = "Belum Validasi Opti",
                value = belumValidasiOptiSetoran
            });
            listSistem.Add(new TampilanRekonSaldo()
            {
                in_out = "IN",
                jenis = "Setoran Cabang",
                validasi = "Validasi Opti",
                value = validasiOptiSetoran
            });
            //BI Return untuk format sesuai WP
            listSistem.AddRange(listReturnBIdanBankLain.Where(x => x.fundingSource.ToLower() == "bi").Select(x => new TampilanRekonSaldo()
            {
                in_out = "IN",
                jenis = "BI Return",
                validasi = "Belum Validasi",
                value = x.plannedReturnNotVal + x.emergencyReturnNotVal
            }));
            listSistem.AddRange(listReturnBIdanBankLain.Where(x => x.fundingSource.ToLower() == "bi").Select(x => new TampilanRekonSaldo()
            {
                in_out = "IN",
                jenis = "BI Return",
                validasi = "Validasi",
                value = x.plannedReturnVal + x.emergencyReturnVal
            }));
            //Bank Lain Return untuk format sesuai WP
            listSistem.AddRange(listReturnBIdanBankLain.Where(x => x.fundingSource.ToLower() != "bi").Select(x => new TampilanRekonSaldo()
            {
                in_out = "IN",
                jenis = "Bank Lain Return",
                validasi = "Belum Validasi",
                value = x.plannedReturnNotVal + x.emergencyReturnNotVal
            }));
            listSistem.AddRange(listReturnBIdanBankLain.Where(x => x.fundingSource.ToLower() != "bi").Select(x => new TampilanRekonSaldo()
            {
                in_out = "IN",
                jenis = "Bank Lain Return",
                validasi = "Validasi",
                value = x.plannedReturnVal + x.emergencyReturnVal
            }));
            //ATM Return untuk format sesuai WP
            listSistem.AddRange(listATMReturn
                .Where(x => !listKodePktCabang.Where(y => y == x.fundingSource).ToList().Any())
                .Select(x => new TampilanRekonSaldo()
                {
                    in_out = "IN",
                    jenis = "ATM Return",
                    validasi = "Belum Validasi",
                    value = x.plannedReturnNotVal + x.emergencyReturnNotVal
                }).GroupBy(x => new { x.in_out, x.jenis, x.validasi })
                .Select(x => new TampilanRekonSaldo()
                {
                    in_out = x.Key.in_out,
                    jenis = x.Key.jenis,
                    validasi = x.Key.validasi,
                    value = x.Sum(y => y.value)
                }));
            listSistem.AddRange(listATMReturn
                .Where(x => !listKodePktCabang.Where(y => y == x.fundingSource).ToList().Any())
                .Select(x => new TampilanRekonSaldo()
                {
                    in_out = "IN",
                    jenis = "ATM Return",
                    validasi = "Validasi",
                    value = x.plannedReturnVal + x.emergencyReturnVal
                })
                .GroupBy(x => new { x.in_out, x.jenis, x.validasi })
                .Select(x => new TampilanRekonSaldo()
                {
                    in_out = x.Key.in_out,
                    jenis = x.Key.jenis,
                    validasi = x.Key.validasi,
                    value = x.Sum(y => y.value)
                }));


            //BON
            Int64 validasiOptiBon = 0;
            Int64 belumValidasiOptiBon = 0;
            if (listBonCabang.Any())
            {
                validasiOptiBon = listBonCabang[0].sudahValidasi;
                belumValidasiOptiBon = listBonCabang[0].belumValidasi;
            }
            //Setoran Cabang untuk format sesuai WP
            listSistem.Add(new TampilanRekonSaldo()
            {
                in_out = "OUT",
                jenis = "Bon Cabang",
                validasi = "Belum Validasi Opti",
                value = belumValidasiOptiBon
            });
            listSistem.Add(new TampilanRekonSaldo()
            {
                in_out = "OUT",
                jenis = "Bon Cabang",
                validasi = "Validasi Opti",
                value = validasiOptiBon
            });
            //BI Return untuk format sesuai WP
            listSistem.AddRange(listDeliveryBIdanBankLain.Where(x => x.fundingSource.ToLower() == "bi").Select(x => new TampilanRekonSaldo()
            {
                in_out = "OUT",
                jenis = "BI Return",
                validasi = "Belum Validasi",
                value = x.plannedDeliveryNotVal + x.emergencyDeliveryNotVal
            }));
            listSistem.AddRange(listDeliveryBIdanBankLain.Where(x => x.fundingSource.ToLower() == "bi").Select(x => new TampilanRekonSaldo()
            {
                in_out = "OU",
                jenis = "BI Return",
                validasi = "Validasi",
                value = x.plannedDeliveryVal + x.emergencyDeliveryVal
            }));
            //Bank Lain Return untuk format sesuai WP
            listSistem.AddRange(listDeliveryBIdanBankLain.Where(x => x.fundingSource.ToLower() != "bi").Select(x => new TampilanRekonSaldo()
            {
                in_out = "OUT",
                jenis = "Bank Lain Return",
                validasi = "Belum Validasi",
                value = x.plannedDeliveryNotVal + x.emergencyDeliveryNotVal
            }));
            listSistem.AddRange(listDeliveryBIdanBankLain.Where(x => x.fundingSource.ToLower() != "bi").Select(x => new TampilanRekonSaldo()
            {
                in_out = "OUT",
                jenis = "Bank Lain Return",
                validasi = "Validasi",
                value = x.plannedDeliveryNotVal + x.emergencyDeliveryNotVal
            }));

            //ATM Return untuk format sesuai WP
            listSistem.AddRange(listATMDelivery
                .Where(x => !listKodePktCabang.Where(y => y == x.fundingSource).ToList().Any())
                .Select(x => new TampilanRekonSaldo()
                {
                    in_out = "OUT",
                    jenis = "ATM Return",
                    validasi = "Belum Validasi",
                    value = x.plannedDeliveryNotVal + x.emergencyDeliveryNotVal
                }).GroupBy(x => new { x.in_out, x.jenis, x.validasi })
                .Select(x => new TampilanRekonSaldo()
                {
                    in_out = x.Key.in_out,
                    jenis = x.Key.jenis,
                    validasi = x.Key.validasi,
                    value = x.Sum(y => y.value)
                }));
            listSistem.AddRange(listATMDelivery
                .Where(x => !listKodePktCabang.Where(y => y == x.fundingSource).ToList().Any())
                .Select(x => new TampilanRekonSaldo()
                {
                    in_out = "IN",
                    jenis = "ATM Return",
                    validasi = "Validasi",
                    value = x.plannedDeliveryNotVal + x.emergencyDeliveryNotVal
                })
                .GroupBy(x => new { x.in_out, x.jenis, x.validasi })
                .Select(x => new TampilanRekonSaldo()
                {
                    in_out = x.Key.in_out,
                    jenis = x.Key.jenis,
                    validasi = x.Key.validasi,
                    value = x.Sum(y => y.value)
                }));



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
        //IN
        private void loadListReturnBIdanBankLain()
        {
            listReturnBIdanBankLain = new List<PivotCPC_BIBL>();
            Database1Entities db = new Database1Entities();

            var query = (from x in db.RekonSaldoVaults.AsEnumerable()
                         where !String.IsNullOrEmpty(x.fundingSoure)
                         && x.vaultId == kodePkt
                         select x).ToList();
            query = (from x in query
                     where (x.fundingSoure == "BI" || x.fundingSoure.Contains("OB")) && (((DateTime)x.dueDate).Date == dateTimePicker1.Value.Date || ((DateTime)x.realDate).Date == dateTimePicker1.Value.Date)
                     select x).ToList();

            List<PivotCPC> pc = new List<PivotCPC>();

            String bufferVaultId = "";

            foreach (var item in query)
            {
                pc.Add(new PivotCPC
                {
                    vaultId = item.vaultId,
                    confId = item.confId,
                    action = item.actionRekon,
                    status = item.statusRekon,
                    blogMessage = item.blogMessage,
                    orderDate = ((DateTime)item.orderDate).Date,
                    dueDate = ((DateTime)item.dueDate).Date,
                    timeStamp = (DateTime)item.timeStampRekon,
                    currencyAmmount = Int64.Parse(item.currencyAmmount.ToString()),
                    fundingSource = item.fundingSoure,
                    realDate = ((DateTime)item.timeStampRekon).Hour < 21 ? ((DateTime)item.timeStampRekon).Date : ((DateTime)item.timeStampRekon).AddDays(1).Date,
                    validation = item.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(item.timeStampRekon.ToString()).Hour < 21 ? item.timeStampRekon : DateTime.Parse(item.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker1.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                });

            }
            var pivot = pc.GroupBy(c => new { c.vaultId, c.fundingSource, c.dueDate, c.realDate }).Select(g => new
            {
                dueDate = g.Key.dueDate,
                realDate = g.Key.realDate,
                vaultID = g.Key.vaultId,
                fundingSource = g.Key.fundingSource,
                emergencyReturn = g.Where(c => c.action == "Emergency Return").Sum(c => c.currencyAmmount),
                emergencyReturnVal = g.Where(c => c.action == "Emergency Return" && c.validation == "VALIDATED").Sum(c => c.currencyAmmount),
                emergencyReturnNotVal = g.Where(c => c.action == "Emergency Return" && c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount),
                plannedReturn = g.Where(c => c.action == "Planned Return").Sum(c => c.currencyAmmount),
                plannedReturnVal = g.Where(c => c.action == "Planned Return" && c.validation == "VALIDATED").Sum(c => c.currencyAmmount),
                plannedReturnNotVal = g.Where(c => c.action == "Planned Return" && c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount)

            }).ToList();
            var pivot2 = (from x in pivot
                          select new { x.dueDate, x.vaultID, x.fundingSource, x.emergencyReturn, x.emergencyReturnVal, x.emergencyReturnNotVal, x.plannedReturn, x.plannedReturnVal, x.plannedReturnNotVal, grandTotal = x.emergencyReturn + x.plannedReturn, x.realDate });

            var bibl = new List<PivotCPC_BIBL>();

            foreach (var item in pivot2)
            {
                if (!(item.plannedReturn == 0 && item.emergencyReturn == 0))
                {
                    bibl.Add(new PivotCPC_BIBL
                    {
                        dueDate = item.dueDate,
                        realDate = item.realDate,
                        vaultId = item.vaultID,
                        fundingSource = item.fundingSource,
                        //emergencyReturn = item.emergencyReturn,
                        emergencyReturnVal = item.emergencyReturnVal,
                        emergencyReturnNotVal = item.emergencyReturnNotVal,
                        //plannedReturn = item.plannedReturn,
                        plannedReturnVal = item.plannedReturnVal,
                        plannedReturnNotVal = item.plannedReturnNotVal,
                        grandTotal = item.grandTotal
                    });
                }

            }

            listReturnBIdanBankLain = bibl;
        }
        private void loadListATMReturn()
        {
            listATMReturn = new List<PivotCPC_ATMR>();
            Database1Entities db = new Database1Entities();
            //preparing data for pivot - atm return
            var queryar = (from x in db.RekonSaldoVaults.AsEnumerable()
                           join y in db.Pkts.AsEnumerable() on x.vaultId equals y.kodePktCabang
                           where !String.IsNullOrEmpty(x.fundingSoure) && y.kanwil.Like("Jabotabek")
                           select x).ToList();
            queryar = (from x in queryar
                       where (x.fundingSoure != "BI" && !x.fundingSoure.Contains("OB")) && (((DateTime)x.dueDate).Date == dateTimePicker1.Value.Date || ((DateTime)x.realDate).Date == dateTimePicker1.Value.Date)
                       select x).ToList();

            var pc = new List<PivotCPC>();

            String bufferVaultId4 = "";

            foreach (var item in queryar)
            {
                pc.Add(new PivotCPC
                {
                    vaultId = item.vaultId,
                    confId = item.confId,
                    action = item.actionRekon,
                    blogMessage = item.blogMessage,
                    status = item.statusRekon,
                    orderDate = ((DateTime)item.orderDate).Date,
                    dueDate = ((DateTime)item.dueDate).Date,
                    timeStamp = (DateTime)item.timeStampRekon,
                    currencyAmmount = Int64.Parse(item.currencyAmmount.ToString()),
                    fundingSource = item.fundingSoure,
                    realDate = ((DateTime)item.timeStampRekon).Hour < 21 ? ((DateTime)item.timeStampRekon).Date : ((DateTime)item.timeStampRekon).AddDays(1).Date,
                    validation = item.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(item.timeStampRekon.ToString()).Hour < 21 ? item.timeStampRekon : DateTime.Parse(item.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker1.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                });

            }

            //creating pivotCPC - atm return
            var pivotar = pc.GroupBy(c => new { c.vaultId, c.fundingSource, c.dueDate, c.realDate }).Select(g => new
            {
                dueDate = g.Key.dueDate,
                realDate = g.Key.realDate,
                vaultID = g.Key.vaultId,
                fundingSource = g.Key.fundingSource,
                emergencyReturn = g.Where(c => c.action == "Emergency Return").Sum(c => c.currencyAmmount),
                emergencyReturnVal = g.Where(c => c.action == "Emergency Return" && c.validation == "VALIDATED").Sum(c => c.currencyAmmount),
                emergencyReturnNotVal = g.Where(c => c.action == "Emergency Return" && c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount),
                plannedReturn = g.Where(c => c.action == "Planned Return").Sum(c => c.currencyAmmount),
                plannedReturnVal = g.Where(c => c.action == "Planned Return" && c.validation == "VALIDATED").Sum(c => c.currencyAmmount),
                plannedReturnNotVal = g.Where(c => c.action == "Planned Return" && c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount)
            }).ToList();

            var pivotar2 = (from x in pivotar
                            select new { x.dueDate, x.vaultID, x.realDate, x.fundingSource, x.emergencyReturn, x.plannedReturn, grandTotal = x.emergencyReturn + x.plannedReturn, x.plannedReturnVal, x.plannedReturnNotVal, x.emergencyReturnVal, x.emergencyReturnNotVal });

            var atmr = new List<PivotCPC_ATMR>();

            foreach (var item in pivotar2)
            {
                if (!(item.plannedReturn == 0 && item.emergencyReturn == 0))
                {
                    atmr.Add(new PivotCPC_ATMR
                    {
                        dueDate = item.dueDate,
                        realDate = item.realDate,
                        vaultId = item.vaultID,
                        fundingSource = item.fundingSource,
                        // emergencyReturn = item.emergencyReturn,
                        emergencyReturnVal = item.emergencyReturnVal,
                        emergencyReturnNotVal = item.emergencyReturnNotVal,
                        // plannedReturn = item.plannedReturn,
                        plannedReturnVal = item.plannedReturnVal,
                        plannedReturnNotVal = item.plannedReturnNotVal,
                        grandTotal = item.grandTotal
                    });
                }

            }

            listATMReturn = atmr;
        }
        private void loadListSetoranCabang()
        {
            listSetoranCabang = new List<PivotPerVendor_setoran>();
            Database1Entities db = new Database1Entities();
            var queryS = (from x in db.RekonSaldoPerVendors.AsEnumerable()
                          where !String.IsNullOrEmpty(x.vendor) && x.vendor == kodePkt
                          select x).ToList();
            var querysetoran = (from x in queryS
                                where (x.actionRekon.Contains("Return") && x.statusRekon.Equals("In Transit")) && (((DateTime)x.dueDate).Date == dateTimePicker1.Value.Date || ((DateTime)x.realDate).Date == dateTimePicker1.Value.Date)
                                select new
                                {
                                    cashPointId = x.cashPointtId,
                                    confId = x.confId,
                                    orderDate = x.orderDate,
                                    vendor = x.vendor,
                                    actionRekon = x.actionRekon,
                                    statusRekon = x.statusRekon,
                                    dueDate = x.dueDate,
                                    blogTime = x.blogTime,
                                    currencyAmmount = x.currencyAmmount,
                                    realDate = x.realDate,
                                    blogMessage = x.blogMessage,
                                    validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker1.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                    //DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED"
                                }).ToList();
            var ppvs = new List<PivotPerVendor_setoran>();

            var pivotsetoran = querysetoran.GroupBy(c => new { c.vendor, c.dueDate, c.realDate }).Select(g => new
            {
                dueDate = g.Key.dueDate,
                valDate = g.Key.realDate,
                vendor = g.Key.vendor,
                belumValidasi = g.Where(c => c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount),
                sudahValidasi = g.Where(c => c.validation == "VALIDATED").Sum(c => c.currencyAmmount),
                grandTotal = g.Where(c => c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount) +
                             g.Where(c => c.validation == "VALIDATED").Sum(c => c.currencyAmmount)

            }).ToList();

            foreach (var item in pivotsetoran)
            {
                ppvs.Add(new PivotPerVendor_setoran
                {
                    dueDate = ((DateTime)item.dueDate).Date,
                    valDate = ((DateTime)item.valDate).Date,
                    vendor = item.vendor,
                    belumValidasi = (Int64)item.belumValidasi,
                    sudahValidasi = (Int64)item.sudahValidasi,
                    grandTotal = (Int64)item.grandTotal
                });
            }

            listSetoranCabang = ppvs;
        }
        //OUT
        private void loadListDeliveryBIdanBankLain()
        {
            listDeliveryBIdanBankLain = new List<PivotCPC_BIBLD>();
            Database1Entities db = new Database1Entities();
            //preparing data for Pivot CPC - BI dan BankLain delivery
            var queryd = (from x in db.RekonSaldoVaults.AsEnumerable()
                          join y in db.Pkts.AsEnumerable() on x.vaultId equals y.kodePktCabang
                          where !String.IsNullOrEmpty(x.fundingSoure) && y.kanwil.Like("Jabotabek")
                          select x).ToList();
            queryd = (from x in queryd
                      where (x.fundingSoure == "BI" || x.fundingSoure.Contains("OB")) && (((DateTime)x.dueDate).Date == dateTimePicker1.Value.Date || ((DateTime)x.realDate).Date == dateTimePicker1.Value.Date)
                      select x).ToList();

            var pc = new List<PivotCPC>();

            foreach (var item in queryd)
            {
                pc.Add(new PivotCPC
                {
                    vaultId = item.vaultId,
                    confId = item.confId,
                    action = item.actionRekon,
                    status = item.statusRekon,
                    blogMessage = item.blogMessage,
                    orderDate = ((DateTime)item.orderDate).Date,
                    dueDate = ((DateTime)item.dueDate).Date,
                    timeStamp = (DateTime)item.timeStampRekon,
                    currencyAmmount = Int64.Parse(item.currencyAmmount.ToString()),
                    fundingSource = item.fundingSoure,
                    realDate = ((DateTime)item.timeStampRekon).Hour < 21 ? ((DateTime)item.timeStampRekon).Date : ((DateTime)item.timeStampRekon).AddDays(1).Date,
                    validation = item.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(item.timeStampRekon.ToString()).Hour < 21 ? item.timeStampRekon : DateTime.Parse(item.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker1.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                });

            }

            //creating pivotCPC - BI dan BankLain Delivery
            var pivotd = pc.GroupBy(c => new { c.vaultId, c.fundingSource, c.dueDate, c.realDate }).Select(g => new
            {
                dueDate = g.Key.dueDate,
                realDate = g.Key.realDate,
                vaultID = g.Key.vaultId,
                fundingSource = g.Key.fundingSource,
                emergencyDelivery = g.Where(c => c.action == "Emergency Delivery").Sum(c => c.currencyAmmount),
                emergencyDeliveryVal = g.Where(c => c.action == "Emergency Delivery" && c.validation == "VALIDATED").Sum(c => c.currencyAmmount),
                emergencyDeliveryNotVal = g.Where(c => c.action == "Emergency Delivery" && c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount),
                plannedDelivery = g.Where(c => c.action == "Planned Delivery").Sum(c => c.currencyAmmount),
                plannedDeliveryVal = g.Where(c => c.action == "Planned Delivery" && c.validation == "VALIDATED").Sum(c => c.currencyAmmount),
                plannedDeliveryNotVal = g.Where(c => c.action == "Planned Delivery" && c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount)
            }).ToList();

            var pivotd2 = (from x in pivotd
                           select new { x.dueDate, x.vaultID, x.fundingSource, x.emergencyDelivery, x.plannedDelivery, grandTotal = x.emergencyDelivery + x.plannedDelivery, x.plannedDeliveryVal, x.plannedDeliveryNotVal, x.emergencyDeliveryNotVal, x.emergencyDeliveryVal, x.realDate });

            var bibld = new List<PivotCPC_BIBLD>();

            foreach (var item in pivotd2)
            {
                if (!(item.emergencyDelivery == 0 && item.plannedDelivery == 0))
                {
                    bibld.Add(new PivotCPC_BIBLD
                    {
                        dueDate = item.dueDate,
                        realDate = item.realDate,
                        vaultId = item.vaultID,
                        fundingSource = item.fundingSource,
                        // emergencyDelivery = item.emergencyDelivery,
                        emergencyDeliveryVal = item.emergencyDeliveryVal,
                        emergencyDeliveryNotVal = item.emergencyDeliveryNotVal,
                        //  plannedDelivery = item.plannedDelivery,
                        plannedDeliveryVal = item.plannedDeliveryVal,
                        plannedDeliveryNotVal = item.plannedDeliveryNotVal,
                        grandTotal = item.grandTotal
                    });
                }

            }
            listDeliveryBIdanBankLain = bibld;
        }
        private void loadListATMDelivery()
        {
            Database1Entities db = new Database1Entities();
            //preparing data for pivot - ATM delivery
            var queryad = (from x in db.RekonSaldoVaults.AsEnumerable()
                           join y in db.Pkts.AsEnumerable() on x.vaultId equals y.kodePktCabang
                           where !String.IsNullOrEmpty(x.fundingSoure) && y.kanwil.Like("Jabotabek")
                           select x).ToList();
            queryad = (from x in queryad
                       where (x.fundingSoure != "BI" && !x.fundingSoure.Contains("OB")) && (((DateTime)x.dueDate).Date == dateTimePicker1.Value.Date || ((DateTime)x.realDate).Date == dateTimePicker1.Value.Date)
                       select x).ToList();

            var pc = new List<PivotCPC>();

            String bufferVaultId3 = "";

            foreach (var item in queryad)
            {
                pc.Add(new PivotCPC
                {
                    vaultId = item.vaultId,
                    confId = item.confId,
                    action = item.actionRekon,
                    blogMessage = item.blogMessage,
                    status = item.statusRekon,
                    orderDate = ((DateTime)item.orderDate).Date,
                    dueDate = ((DateTime)item.dueDate).Date,
                    timeStamp = (DateTime)item.timeStampRekon,
                    currencyAmmount = Int64.Parse(item.currencyAmmount.ToString()),
                    fundingSource = item.fundingSoure,
                    realDate = ((DateTime)item.timeStampRekon).Hour < 21 ? ((DateTime)item.timeStampRekon).Date : ((DateTime)item.timeStampRekon).AddDays(1).Date,
                    validation = item.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(item.timeStampRekon.ToString()).Hour < 21 ? item.timeStampRekon : DateTime.Parse(item.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker1.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                });

            }

            //creating pivot - atm delivery
            var pivotad = pc.GroupBy(c => new { c.vaultId, c.fundingSource, c.dueDate, c.realDate }).Select(g => new
            {
                dueDate = g.Key.dueDate,
                realDate = g.Key.realDate,
                vaultID = g.Key.vaultId,
                fundingSource = g.Key.fundingSource,
                emergencyDelivery = g.Where(c => c.action == "Emergency Delivery").Sum(c => c.currencyAmmount),
                emergencyDeliveryVal = g.Where(c => c.action == "Emergency Delivery" && c.validation == "VALIDATED").Sum(c => c.currencyAmmount),
                emergencyDeliveryNotVal = g.Where(c => c.action == "Emergency Delivery" && c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount),
                plannedDelivery = g.Where(c => c.action == "Planned Delivery").Sum(c => c.currencyAmmount),
                plannedDeliveryVal = g.Where(c => c.action == "Planned Delivery" && c.validation == "VALIDATED").Sum(c => c.currencyAmmount),
                plannedDeliveryNotVal = g.Where(c => c.action == "Planned Delivery" && c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount)
            }).ToList();

            var pivotad2 = (from x in pivotad
                            select new { x.dueDate, x.vaultID, x.fundingSource, x.emergencyDelivery, x.plannedDelivery, grandTotal = x.emergencyDelivery + x.plannedDelivery, x.plannedDeliveryNotVal, x.plannedDeliveryVal, x.emergencyDeliveryNotVal, x.emergencyDeliveryVal, x.realDate });

            var atmd = new List<PivotCPC_ATMD>();

            foreach (var item in pivotad2)
            {
                if (!(item.emergencyDelivery == 0 && item.plannedDelivery == 0))
                {
                    atmd.Add(new PivotCPC_ATMD
                    {
                        dueDate = item.dueDate,
                        realDate = item.realDate,
                        vaultId = item.vaultID,
                        fundingSource = item.fundingSource,
                        //  emergencyDelivery = item.emergencyDelivery,
                        emergencyDeliveryVal = item.emergencyDeliveryVal,
                        emergencyDeliveryNotVal = item.emergencyDeliveryNotVal,
                        //  plannedDelivery = item.plannedDelivery,
                        plannedDeliveryVal = item.plannedDeliveryVal,
                        plannedDeliveryNotVal = item.plannedDeliveryNotVal,
                        grandTotal = item.grandTotal
                    });
                }

            }

            listATMDelivery = atmd;

        }
        private void loadListBonCabang()
        {
            listBonCabang = new List<PivotPerVendor_bon>();
            Database1Entities db = new Database1Entities();
            var queryS = (from x in db.RekonSaldoPerVendors.AsEnumerable()
                          join y in db.Pkts.AsEnumerable() on x.vendor equals y.kodePktCabang
                          where !String.IsNullOrEmpty(x.vendor) && y.kanwil.Like("Jabotabek")
                          select x).ToList();
            var query = (from x in queryS
                         join y in db.Pkts.AsEnumerable() on x.vendor equals y.kodePktCabang
                         where (!String.IsNullOrEmpty(x.vendor) && y.kanwil.Like("Jabotabek") && x.actionRekon.Contains("Delivery") && x.statusRekon.Equals("Confirmed")) && (((DateTime)x.dueDate).Date == dateTimePicker1.Value.Date || ((DateTime)x.realDate).Date == dateTimePicker1.Value.Date)
                         select new
                         {
                             cashPointId = x.cashPointtId,
                             confId = x.confId,
                             orderDate = x.orderDate,
                             vendor = x.vendor,
                             actionRekon = x.actionRekon,
                             statusRekon = x.statusRekon,
                             dueDate = x.dueDate,
                             blogTime = x.blogTime,
                             currencyAmmount = x.currencyAmmount,
                             realDate = x.realDate,
                             blogMessage = x.blogMessage,
                             validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker1.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                         }).ToList();

            //generating pivotbon
            var ppvb = new List<PivotPerVendor_bon>();

            var pivot = query.GroupBy(c => new { c.vendor, c.dueDate, c.realDate }).Select(g => new
            {
                dueDate = g.Key.dueDate,
                valDate = g.Key.realDate,
                vendor = g.Key.vendor,
                belumValidasi = g.Where(c => c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount),
                sudahValidasi = g.Where(c => c.validation == "VALIDATED").Sum(c => c.currencyAmmount),
                grandTotal = g.Where(c => c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount) +
                             g.Where(c => c.validation == "VALIDATED").Sum(c => c.currencyAmmount)

            }).ToList();

            Console.WriteLine(pivot.Count);

            foreach (var item in pivot)
            {
                ppvb.Add(new PivotPerVendor_bon
                {
                    dueDate = ((DateTime)item.dueDate).Date,
                    valDate = ((DateTime)item.valDate).Date,
                    vendor = item.vendor,
                    belumValidasi = (Int64)item.belumValidasi,
                    sudahValidasi = (Int64)item.sudahValidasi,
                    grandTotal = (Int64)item.grandTotal
                });
            }

            listBonCabang = ppvb;
        }
        private void pktComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            kodePkt = pktComboBox.SelectedItem.ToString();
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            Database1Entities db = new Database1Entities();
            DateTime tanggal = dateTimePicker1.Value.Date;
            List<RekonSaldoInputanUser> rekonSaldoInputanUsers = new List<RekonSaldoInputanUser>();
            for(int a=0;a<dataTambahanGridView.Rows.Count - 1 ;a++)
            {
                DataGridViewRow temp = dataTambahanGridView.Rows[a];
                String id = temp.Cells["id"].Value == null ? "-1" : temp.Cells["id"].Value.ToString(),
                    in_out = temp.Cells["in_out"].Value.ToString(),
                    jenis = temp.Cells["jenis"].Value.ToString(),
                    value = temp.Cells["value"].Value.ToString(),
                    keterangan = temp.Cells["keterangan"].Value.ToString();
                Console.WriteLine(id + " " + in_out + " " + jenis + " " + value);
                rekonSaldoInputanUsers.Add(new RekonSaldoInputanUser() {
                    Id = Int32.Parse(id),
                    in_out = in_out,
                    jenis = jenis,
                    tanggal = tanggal,
                    kodePkt = kodePkt,
                    keterangan = keterangan,
                    value = Int64.Parse(value)
                });
            }
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
    }
    class TampilanRekonSaldo
    {
        public String in_out { set; get; }
        public String jenis { set; get; }
        public String validasi { set; get; }
        public Int64 value { set; get; }
    }
}

