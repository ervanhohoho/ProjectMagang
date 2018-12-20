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

namespace testProjectBCA
{
    public partial class OlahanOptiVault : Form
    {
        Database1Entities en = new Database1Entities();
        public OlahanOptiVault()
        {
            InitializeComponent();
            buttonProcessVault.Visible = false;
            buttonProcessSetoranCpc.Visible = false;
            buttonProsesOrderBlogHistory.Visible = false;
            dataGridView1.Visible = false;
            dateTimePicker1.Visible = false;
            //buttonGeneratePivot.Enabled = false;
            //buttonGeneratePivotPerVendor.Enabled = false;
            dataGridView2.Visible = false;
            dataGridView3.Visible = false;
            dataGridView4.Visible = false;
            dataGridView5.Visible = false;
            dataGridView6.Visible = false;
            dataGridView7.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
            label7.Visible = false;
 





        }


        List<VOBH> vobh;
        List<VO> vo;
        List<OBH> obh;
        List<VaultProcessed> vp;
        List<SetoranCPC> sc;
        List<PivotCPC> pc;
        List<PivotCPC_BIBL> bibl;
        List<PivotCPC_BIBLD> bibld;
        List<PivotCPC_ATMD> atmd;
        List<PivotCPC_ATMR> atmr;
        List<OBHProcessed> obhp;
        List<PivotPerVendor_bon> ppvb;
        List<PivotPerVendor_setoran> ppvs;
        List<PivotPerVendor_Out> pivotPerVendor_Out;
        List<PivotPerVendor_In> pivotPerVendor_In;
        List<PivotVault_In> pivotVault_In;
        List<PivotVault_Out> pivotVault_Out;


        private void buttonUploadVaultOrderBlogHistory_Click(object sender, EventArgs e)
        {
            int flagUpVOBH = 0;
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.csvFilter;

            if (of.ShowDialog() == DialogResult.OK)
            {
                String filename = of.FileName;
                DataSet ds = Util.openCsv(filename);
                DataTable dt = ds.Tables[0];

                dataGridView1.DataSource = dt;

                vobh = new List<VOBH>();

                for (int i = 1; i < dt.Rows.Count - 1; i++)
                {
                    if (String.IsNullOrWhiteSpace(dt.Rows[i][3].ToString()))
                    {
                        continue;
                    }
                    Console.WriteLine(i);
                    Console.WriteLine(dt.Rows[i][7].ToString());
                    vobh.Add(new VOBH
                    {
                        vaultId = dt.Rows[i][0].ToString(),
                        confId = dt.Rows[i][4].ToString(),
                        orderDate = Convert.ToDateTime(dt.Rows[i][5].ToString()).Date,
                        dueDate = Convert.ToDateTime(dt.Rows[i][6].ToString()).Date,
                        timeStamp = DateTime.ParseExact(dt.Rows[i][7].ToString(), "M/d/yyyy H:mm", System.Globalization.CultureInfo.InvariantCulture),
                        blogMessage = dt.Rows[i][9].ToString(),
                        action = dt.Rows[i][2].ToString(),
                        status = dt.Rows[i][3].ToString(),
                        currencyAmmount = CurrencyFill(String.IsNullOrWhiteSpace(dt.Rows[i][11].ToString()) ? dt.Rows[i - 1][11].ToString().Replace("IDR:", "") : dt.Rows[i][11].ToString().Replace("IDR:", ""))
                        //currencyAmmount = String.IsNullOrEmpty(dt.Rows[i][11].ToString()) ? Int64.Parse(dt.Rows[i - 1][11].ToString().Replace("IDR:", "")) : Int64.Parse(dt.Rows[i][11].ToString().Replace("IDR:", ""))
                    });
                }
                label1.Text = filename.Substring(filename.LastIndexOf('\\'), filename.Length - filename.LastIndexOf('\\'));
            }

            flagUpVOBH = 1;
        }

        private void buttonUploadVaultOrder_Click(object sender, EventArgs e)
        {
            int flagUpVO = 0;
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.csvFilter;

            if (of.ShowDialog() == DialogResult.OK)
            {
                String filename = of.FileName;
                DataSet ds = Util.openCsv(filename);
                DataTable dt = ds.Tables[0];

                dataGridView1.DataSource = dt;

                vo = new List<VO>();

                for (int i = 1; i < dt.Rows.Count - 1; i++)
                {

                    vo.Add(new VO
                    {
                        vaultId = dt.Rows[i][0].ToString(),
                        confId = dt.Rows[i][2].ToString(),
                        fundingSource = dt.Rows[i][3].ToString(),
                        orderDate = Convert.ToDateTime(dt.Rows[i][5].ToString()).Date,
                        dueDate = Convert.ToDateTime(dt.Rows[i][6].ToString()).Date,
                        action = dt.Rows[i][1].ToString(),
                        status = dt.Rows[i][4].ToString(),
                        currencyAmmount = CurrencyFill(String.IsNullOrWhiteSpace(dt.Rows[i][11].ToString()) ? dt.Rows[i - 1][11].ToString().Replace("IDR:", "") : dt.Rows[i][11].ToString().Replace("IDR:", ""))
                        //currencyAmmount = Int64.Parse(dt.Rows[i][11].ToString())
                    });
                    Console.WriteLine("data ke: " + i);
                }
                label2.Text = filename.Substring(filename.LastIndexOf('\\'), filename.Length - filename.LastIndexOf('\\'));
            }
            flagUpVO = 1;
        }


        private void buttonUploadOrderBlogHistory_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.csvFilter;
            if (of.ShowDialog() == DialogResult.OK)
            {
                String filename = of.FileName;
                DataSet ds = Util.openCsv(filename);
                DataTable dt = ds.Tables[0];

                dataGridView1.DataSource = dt;

                obh = new List<OBH>();

                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][6].ToString() == "Confirmed" || dt.Rows[i][6].ToString() == "In Transit")
                    {
                        String cashpointId = dt.Rows[i][0].ToString();
                        String confId = dt.Rows[i][5].ToString();
                        DateTime orderDate = Convert.ToDateTime(dt.Rows[i][3].ToString()).Date;
                        DateTime dueDate = Convert.ToDateTime(dt.Rows[i][4].ToString()).Date;
                        DateTime blogTime = DateTime.ParseExact(dt.Rows[i][9].ToString(), "M/d/yyyy H:mm", System.Globalization.CultureInfo.InvariantCulture);
                        String action = dt.Rows[i][2].ToString();
                        String status = dt.Rows[i][6].ToString();
                        String blogMessage = dt.Rows[i][10].ToString();
                        String tampungan = " ";
                        for (int z = i; z >= 0; z--)
                        {
                            if (String.IsNullOrWhiteSpace(tampungan))
                            {
                                tampungan = dt.Rows[z][11].ToString();
                            }
                            else
                                break;
                        }
                        Int64 currencyAmmount = CurrencyFill(String.IsNullOrWhiteSpace(dt.Rows[i][11].ToString()) ? dt.Rows[i - 1][11].ToString().Replace("IDR:", "") : dt.Rows[i][11].ToString().Replace("IDR:", ""));

                        obh.Add(new OBH
                        {
                            cashpointId = dt.Rows[i][0].ToString(),
                            confId = dt.Rows[i][5].ToString(),
                            orderDate = Convert.ToDateTime(dt.Rows[i][3].ToString()).Date,
                            dueDate = Convert.ToDateTime(dt.Rows[i][4].ToString()).Date,
                            blogMessage = dt.Rows[i][10].ToString(),
                            blogTime = DateTime.ParseExact(dt.Rows[i][9].ToString(), "M/d/yyyy H:mm", System.Globalization.CultureInfo.InvariantCulture),
                            action = dt.Rows[i][2].ToString(),
                            status = dt.Rows[i][6].ToString(),
                            currencyAmmount = CurrencyFill(tampungan.Replace("IDR:", "")),
                            //currencyAmmount = CurrencyFill(String.IsNullOrWhiteSpace(dt.Rows[i][11].ToString()) ? dt.Rows[i - 1][11].ToString().Replace("IDR:", "") : dt.Rows[i][11].ToString().Replace("IDR:", ""))
                        });
                        Console.WriteLine("currency yg keluar: " + currencyAmmount);
                    }

                }

                label3.Text = filename.Substring(filename.LastIndexOf('\\'), filename.Length - filename.LastIndexOf('\\'));
                dataGridView1.DataSource = obh;

            }
        }

        private void buttonProsesOrderBlogHistory_Click(object sender, EventArgs e)
        {

            obhp = new List<OBHProcessed>();

            var cabangs = en.Cabangs.Select(x => x).ToList();



            var query = (from x in obh
                         join y in cabangs on x.cashpointId.TrimStart('B').TrimStart('0') equals y.kodeCabang
                         select new { x.cashpointId, x.confId, x.orderDate, x.dueDate, x.blogTime, x.action, x.status, x.currencyAmmount, y.kodePkt }
                         ).ToList();

            foreach (var item in query)
            {
                obhp.Add(new OBHProcessed
                {
                    cashpointId = item.cashpointId,
                    vendor = item.kodePkt,
                    confId = item.confId,
                    orderDate = item.orderDate,
                    dueDate = item.dueDate,
                    blogTime = item.blogTime,
                    action = item.action,
                    status = item.status,
                    currencyAmmount = item.currencyAmmount,
                    realDate = item.blogTime.Hour < 21 ? item.blogTime.Date : item.blogTime.AddDays(1).Date,
                    validation = (item.blogTime.Hour < 21 ? item.blogTime : item.blogTime.AddDays(1)).Date <= item.dueDate.Date ? "VALIDATED" : "NOT VALIDATED"
                });
            }
            dataGridView1.DataSource = obhp;

        }

        private void buttonProcessVault_Click(object sender, EventArgs e)
        {
            var query = (from x in vobh
                         select new { x.vaultId, x.confId, x.action, x.status, x.orderDate, x.dueDate, x.timeStamp, x.currencyAmmount, fundingSource = vo.Where(y => y.confId == x.confId).Select(y => y.fundingSource).FirstOrDefault() }
                         ).ToList();

            vp = new List<VaultProcessed>();

            foreach (var item in query)
            {
                vp.Add(new VaultProcessed
                {
                    vaultId = item.vaultId,
                    confId = item.confId,
                    action = item.action,
                    status = item.status,
                    orderDate = item.orderDate,
                    dueDate = item.dueDate,
                    timeStamp = item.timeStamp,
                    currencyAmmount = item.currencyAmmount,
                    fundingSource = item.fundingSource,
                    realDate = item.timeStamp.Hour < 21 ? item.timeStamp.Date : item.timeStamp.AddDays(1).Date,
                    validation = (item.timeStamp.Hour < 21 ? item.timeStamp : item.timeStamp.AddDays(1)).Date <= item.dueDate.Date ? "VALIDATED" : "NOT VALIDATED"
                });
            }
            Console.WriteLine(vp.Count);
            dataGridView1.DataSource = vp;

        }

        private void buttonProcessSetoranCpc_Click(object sender, EventArgs e)
        {
            var query = (from x in vp
                         where x.status == "Confirmed"
                         select x
                         ).ToList();

            sc = new List<SetoranCPC>();

            foreach (var item in query)
            {
                sc.Add(new SetoranCPC
                {
                    vaultId = item.vaultId,
                    confId = item.confId,
                    action = item.action,
                    status = item.status,
                    orderDate = item.orderDate,
                    dueDate = item.dueDate,
                    timeStamp = item.timeStamp,
                    currencyAmmount = item.currencyAmmount,
                    fundingSource = item.fundingSource,
                    realDate = item.timeStamp.Hour < 21 ? item.timeStamp : item.timeStamp.AddDays(1),
                    validation = (item.timeStamp.Hour < 21 ? item.timeStamp : item.timeStamp.AddDays(1)).Date <= item.dueDate.Date ? "VALIDATED" : "NOT VALIDATED"
                });
            }
            Console.WriteLine(sc.Count);
            dataGridView1.DataSource = sc;
        }

        private void buttonGeneratePivotPerVendor_Click(object sender, EventArgs e)
        {
            //ProsesOrderBlogHistory();
            var queryS = (from x in en.RekonSaldoPerVendors.AsEnumerable()
                          join y in en.Pkts.AsEnumerable() on x.vendor equals y.kodePktCabang
                          where !String.IsNullOrEmpty(x.vendor) && y.kanwil.Like("Jabotabek")
                          select x).ToList();

            //preparing data for pivot pervendor bon
            var query = (from x in queryS
                         join y in en.Pkts.AsEnumerable() on x.vendor equals y.kodePktCabang
                         where (!String.IsNullOrEmpty(x.vendor) && y.kanwil.Like("Jabotabek") && x.actionRekon.Contains("Delivery") && x.statusRekon.Equals("Confirmed")) && (((DateTime)x.dueDate).Date == dateTimePicker2.Value.Date || ((DateTime)x.realDate).Date == dateTimePicker2.Value.Date)
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
                             validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                             // x.realDate <= dateTimePicker2.Value ? "VALIDATED" : "NOT VALIDATED"
                             //&& x.blogMessage.Contains("GL Processed")
                             //DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED"
                         }).ToList();

            //generating pivotbon
            ppvb = new List<PivotPerVendor_bon>();

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

            dataGridView6.DataSource = ppvb;

            //preparing data for pivot pervendor setoran
            var querysetoran = (from x in queryS
                                where (x.actionRekon.Contains("Return") && x.statusRekon.Equals("In Transit")) && (((DateTime)x.dueDate).Date == dateTimePicker2.Value.Date || ((DateTime)x.realDate).Date == dateTimePicker2.Value.Date)
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
                                    validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                    //DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED"
                                }).ToList();

            //generating pivot
            ppvs = new List<PivotPerVendor_setoran>();

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
            dataGridView7.DataSource = ppvs;

            if (dataGridView6.Rows.Count > 0)
            {
                for (int i = 2; i < dataGridView6.Columns.Count; i++)
                {
                    dataGridView6.Columns[i].DefaultCellStyle.Format = "c";
                    dataGridView6.Columns[i].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("ID-id");
                }
            }

            if (dataGridView7.Rows.Count > 0)
            {
                for (int i = 2; i < dataGridView7.Columns.Count; i++)
                {
                    dataGridView7.Columns[i].DefaultCellStyle.Format = "c";
                    dataGridView7.Columns[i].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("ID-id");
                }
            }

        }


        private void buttonGeneratePivot_Click(object sender, EventArgs e)
        {
            //ProsesVault();
            //ProsesSetoranCPC();


            //creating pivotCPC - BI dan BankLain Return
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

            bibl = new List<PivotCPC_BIBL>();

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

            dataGridView2.DataSource = bibl;

            //preparing data for Pivot CPC - BI dan BankLain delivery
            var queryd = (from x in en.RekonSaldoVaults.AsEnumerable()
                          join y in en.Pkts.AsEnumerable() on x.vaultId equals y.kodePktCabang
                          where !String.IsNullOrEmpty(x.fundingSoure) && y.kanwil.Like("Jabotabek")
                          select x).ToList();
            queryd = (from x in queryd
                      where (x.fundingSoure.Substring(0,2) == "BI" || x.fundingSoure.Substring(0, 2) == "OB") && (((DateTime)x.dueDate).Date == dateTimePicker2.Value.Date || ((DateTime)x.realDate).Date == dateTimePicker2.Value.Date)
                      select x).ToList();

            pc = new List<PivotCPC>();

            String bufferVaultId2 = "";

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
                    validation = item.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(item.timeStampRekon.ToString()).Hour < 21 ? item.timeStampRekon : DateTime.Parse(item.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
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

            bibld = new List<PivotCPC_BIBLD>();

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

            dataGridView3.DataSource = bibld;

            //preparing data for pivot - ATM delivery
            var queryad = (from x in en.RekonSaldoVaults.AsEnumerable()
                           join y in en.Pkts.AsEnumerable() on x.vaultId equals y.kodePktCabang
                           where !String.IsNullOrEmpty(x.fundingSoure) && y.kanwil.Like("Jabotabek")
                           select x).ToList();
            queryad = (from x in queryad
                       where (x.fundingSoure.Substring(0,2) != "BI" && !x.fundingSoure.Substring(0,2).Contains("OB")) && (((DateTime)x.dueDate).Date == dateTimePicker2.Value.Date || ((DateTime)x.realDate).Date == dateTimePicker2.Value.Date)
                       select x).ToList();

            pc = new List<PivotCPC>();

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
                    validation = item.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(item.timeStampRekon.ToString()).Hour < 21 ? item.timeStampRekon : DateTime.Parse(item.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
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

            atmd = new List<PivotCPC_ATMD>();

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

            dataGridView4.DataSource = atmd;

            //preparing data for pivot - atm return
            var queryar = (from x in en.RekonSaldoVaults.AsEnumerable()
                           join y in en.Pkts.AsEnumerable() on x.vaultId equals y.kodePktCabang
                           where !String.IsNullOrEmpty(x.fundingSoure) && y.kanwil.Like("Jabotabek")
                           select x).ToList();
            queryar = (from x in queryar
                       where (x.fundingSoure.Substring(0,2) != "BI" && !x.fundingSoure.Substring(0,2).Contains("OB")) && (((DateTime)x.dueDate).Date == dateTimePicker2.Value.Date || ((DateTime)x.realDate).Date == dateTimePicker2.Value.Date)
                       select x).ToList();

            pc = new List<PivotCPC>();

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
                    validation = item.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(item.timeStampRekon.ToString()).Hour < 21 ? item.timeStampRekon : DateTime.Parse(item.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
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

            atmr = new List<PivotCPC_ATMR>();

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

            dataGridView5.DataSource = atmr;

            //formatting
            if (dataGridView2.Rows.Count > 0)
            {
                for (int i = 3; i < dataGridView2.Columns.Count; i++)
                {
                    dataGridView2.Columns[i].DefaultCellStyle.Format = "c";
                    dataGridView2.Columns[i].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("ID-id");
                }
            }
            if (dataGridView3.Rows.Count > 0)
            {
                for (int i = 3; i < dataGridView3.Columns.Count; i++)
                {
                    dataGridView3.Columns[i].DefaultCellStyle.Format = "c";
                    dataGridView3.Columns[i].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("ID-id");
                }
            }
            if (dataGridView4.Rows.Count > 0)
            {
                for (int i = 3; i < dataGridView4.Columns.Count; i++)
                {
                    dataGridView4.Columns[i].DefaultCellStyle.Format = "c";
                    dataGridView4.Columns[i].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("ID-id");
                }
            }
            if (dataGridView5.Rows.Count > 0)
            {
                for (int i = 3; i < dataGridView5.Columns.Count; i++)
                {
                    dataGridView5.Columns[i].DefaultCellStyle.Format = "c";
                    dataGridView5.Columns[i].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("ID-id");
                }
            }


        }







        public Int64 CurrencyFill(String a)
        {
            Int64 result = 0;
            Console.WriteLine("yang masuk: " + a);
            if (a.Contains("e") || a.Contains("E"))
            {
                result = (Int64)Double.Parse(a, System.Globalization.NumberStyles.Float);
            }
            else
            {
                if (String.IsNullOrEmpty(a))
                { result = 0; }
                else
                    result = (Int64)Double.Parse(a);
            }

            return result;
        }

        public void ProsesVault()
        {
            {
                var query = (from x in vobh
                             select new { x.vaultId, x.confId, x.action, x.status, x.orderDate, x.dueDate, x.blogMessage, x.timeStamp, x.currencyAmmount, fundingSource = vo.Where(y => y.confId == x.confId).Select(y => y.fundingSource).FirstOrDefault() }
                             ).ToList();

                vp = new List<VaultProcessed>();

                foreach (var item in query)
                {
                    vp.Add(new VaultProcessed
                    {
                        vaultId = item.vaultId,
                        confId = item.confId,
                        action = item.action,
                        status = item.status,
                        orderDate = item.orderDate,
                        dueDate = item.dueDate,
                        timeStamp = item.timeStamp,
                        blogMessage = item.blogMessage,
                        currencyAmmount = item.currencyAmmount,
                        fundingSource = item.fundingSource,
                        realDate = item.timeStamp.Hour < 21 ? item.timeStamp.Date : item.timeStamp.AddDays(1).Date,

                    });
                }
                Console.WriteLine(vp.Count);
                dataGridView1.DataSource = vp;
            }
        }

        public void ProsesSetoranCPC()
        {
            var query = (from x in vp
                         where x.status == "Confirmed"
                         select x
                         ).ToList();

            sc = new List<SetoranCPC>();

            foreach (var item in query)
            {
                sc.Add(new SetoranCPC
                {
                    vaultId = item.vaultId,
                    confId = item.confId,
                    action = item.action,
                    status = item.status,
                    orderDate = item.orderDate,
                    dueDate = item.dueDate,
                    timeStamp = item.timeStamp,
                    currencyAmmount = item.currencyAmmount,
                    fundingSource = item.fundingSource,
                    realDate = item.timeStamp.Hour < 21 ? item.timeStamp.Date : item.timeStamp.AddDays(1).Date,
                    blogMessage = item.blogMessage

                });
            }
            Console.WriteLine(sc.Count);
            dataGridView1.DataSource = sc;
        }

        public void ProsesOrderBlogHistory()
        {
            obhp = new List<OBHProcessed>();

            var cabangs = en.Cabangs.Select(x => x).ToList();



            var query = (from x in obh
                         join y in cabangs on x.cashpointId.TrimStart('B') equals y.kodeCabang
                         select new
                         {
                             x.cashpointId,
                             x.confId,
                             x.orderDate,
                             x.dueDate,
                             x.blogTime,
                             x.action,
                             x.status,
                             x.currencyAmmount,
                             x.blogMessage,
                             y.kodePkt
                         }
                         ).ToList();

            foreach (var item in query)
            {
                obhp.Add(new OBHProcessed
                {
                    cashpointId = item.cashpointId,
                    vendor = item.kodePkt,
                    confId = item.confId,
                    orderDate = item.orderDate,
                    dueDate = item.dueDate,
                    blogTime = item.blogTime,
                    blogMessage = item.blogMessage,
                    action = item.action,
                    status = item.status,
                    currencyAmmount = item.currencyAmmount,
                    realDate = item.blogTime.Hour < 21 ? item.blogTime.Date : item.blogTime.AddDays(1).Date,
                    validation = (item.blogTime.Hour < 21 ? item.blogTime : item.blogTime.AddDays(1)).Date <= item.dueDate.Date ? "VALIDATED" : "NOT VALIDATED"
                });
            }
            dataGridView1.DataSource = obhp;
        }

        private void buttonSaveToDB_Click(object sender, EventArgs e)
        {
            loadForm.ShowSplashScreen();
            ProsesVault();
            ProsesSetoranCPC();
            //save to db rs-vault
            List<RekonSaldoVault> rsv = (from x in sc
                                         select new RekonSaldoVault()
                                         {
                                             vaultId = x.vaultId,
                                             confId = x.confId,
                                             fundingSoure = x.fundingSource == null ? "" : x.fundingSource,
                                             actionRekon = x.action,
                                             statusRekon = x.status,
                                             orderDate = x.orderDate,
                                             dueDate = x.dueDate,
                                             timeStampRekon = x.timeStamp,
                                             currencyAmmount = x.currencyAmmount,
                                             realDate = x.realDate,
                                             blogMessage = x.blogMessage
                                         }).ToList();


            DateTime maxdate2 = rsv.Max(x => (DateTime)x.dueDate);
            DateTime mindate2 = rsv.Min(x => (DateTime)x.dueDate);

            var query3 = (from x in en.RekonSaldoVaults
                          where (DateTime)x.dueDate >= mindate2 && (DateTime)x.dueDate <= maxdate2
                          select x
                         ).ToList();

            List<RekonSaldoVault> rsvToRemove = new List<RekonSaldoVault>();

            foreach (var item in rsv)
            {
                var query4 = (from x in query3
                              where x.confId == item.confId && x.dueDate == item.dueDate
                              select x).FirstOrDefault();
                if (query4 != null)
                {
                    query4.vaultId = item.vaultId;
                    query4.confId = item.confId;
                    query4.orderDate = item.orderDate;
                    query4.fundingSoure = item.fundingSoure;
                    query4.actionRekon = item.actionRekon;
                    query4.statusRekon = item.statusRekon;
                    query4.dueDate = item.dueDate;
                    query4.timeStampRekon = item.timeStampRekon;
                    query4.currencyAmmount = item.currencyAmmount;
                    query4.realDate = item.realDate;
                    query4.blogMessage = item.blogMessage;
                    en.SaveChanges();

                    rsvToRemove.Add(item);

                }
            }

            foreach (var item in rsvToRemove)
            {
                rsv.Remove(item);
            }


            en.RekonSaldoVaults.AddRange(rsv);
            en.SaveChanges();
            loadForm.CloseForm();

            //-------------------------=======================---------------------------//

        }

        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewSelectedCellCollection cells = dataGridView2.SelectedCells;
            foreach (DataGridViewCell cell in cells)
            {
                int rowidx = cell.RowIndex;
                int colidx = cell.ColumnIndex;
                Console.WriteLine(rowidx + ", " + colidx);
                Console.WriteLine(dataGridView2.Rows[rowidx].Cells[colidx].Value.ToString().Replace("Rp.", "").Replace(".", ""));
                if (colidx > 2)
                {
                    dataGridView2.Rows[rowidx].Cells[colidx].Style.Format = "F0";
                }

            }
            for (int a = 0; a < dataGridView2.Rows.Count; a++)
            {
                for (int b = 3; b < dataGridView2.Columns.Count; b++)
                {
                    if (!cells.Contains(dataGridView2.Rows[a].Cells[b]))
                    {
                        Int64 buf;
                        dataGridView2.Rows[a].Cells[b].Style.Format = "C0";
                        dataGridView2.Rows[a].Cells[b].Style.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
                    }
                }
            }
        }

        private void dataGridView3_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewSelectedCellCollection cells = dataGridView3.SelectedCells;
            foreach (DataGridViewCell cell in cells)
            {
                int rowidx = cell.RowIndex;
                int colidx = cell.ColumnIndex;
                Console.WriteLine(rowidx + ", " + colidx);
                Console.WriteLine(dataGridView3.Rows[rowidx].Cells[colidx].Value.ToString().Replace("Rp.", "").Replace(".", ""));
                if (colidx > 2)
                {
                    dataGridView3.Rows[rowidx].Cells[colidx].Style.Format = "F0";
                }
            }
            for (int a = 0; a < dataGridView3.Rows.Count; a++)
            {
                for (int b = 3; b < dataGridView3.Columns.Count; b++)
                {
                    if (!cells.Contains(dataGridView3.Rows[a].Cells[b]))
                    {
                        Int64 buf;
                        dataGridView3.Rows[a].Cells[b].Style.Format = "C0";
                        dataGridView3.Rows[a].Cells[b].Style.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
                    }
                }
            }
        }

        private void dataGridView6_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewSelectedCellCollection cells = dataGridView6.SelectedCells;
            foreach (DataGridViewCell cell in cells)
            {
                int rowidx = cell.RowIndex;
                int colidx = cell.ColumnIndex;
                Console.WriteLine(rowidx + ", " + colidx);
                Console.WriteLine(dataGridView6.Rows[rowidx].Cells[colidx].Value.ToString().Replace("Rp.", "").Replace(".", ""));
                if (colidx >= 2)
                {
                    dataGridView6.Rows[rowidx].Cells[colidx].Style.Format = "F0";
                }
            }
            for (int a = 0; a < dataGridView6.Rows.Count; a++)
            {
                for (int b = 2; b < dataGridView6.Columns.Count; b++)
                {
                    if (!cells.Contains(dataGridView6.Rows[a].Cells[b]))
                    {
                        Int64 buf;
                        dataGridView6.Rows[a].Cells[b].Style.Format = "C0";
                        dataGridView6.Rows[a].Cells[b].Style.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
                    }
                }
            }
        }

        private void dataGridView5_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewSelectedCellCollection cells = dataGridView5.SelectedCells;
            foreach (DataGridViewCell cell in cells)
            {
                int rowidx = cell.RowIndex;
                int colidx = cell.ColumnIndex;
                Console.WriteLine(rowidx + ", " + colidx);
                Console.WriteLine(dataGridView5.Rows[rowidx].Cells[colidx].Value.ToString().Replace("Rp.", "").Replace(".", ""));
                if (colidx > 2)
                {
                    dataGridView5.Rows[rowidx].Cells[colidx].Style.Format = "F0";
                }
            }
            for (int a = 0; a < dataGridView5.Rows.Count; a++)
            {
                for (int b = 3; b < dataGridView5.Columns.Count; b++)
                {
                    if (!cells.Contains(dataGridView5.Rows[a].Cells[b]))
                    {
                        Int64 buf;
                        dataGridView5.Rows[a].Cells[b].Style.Format = "C0";
                        dataGridView5.Rows[a].Cells[b].Style.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
                    }
                }
            }
        }

        private void dataGridView4_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewSelectedCellCollection cells = dataGridView4.SelectedCells;
            foreach (DataGridViewCell cell in cells)
            {
                int rowidx = cell.RowIndex;
                int colidx = cell.ColumnIndex;
                Console.WriteLine(rowidx + ", " + colidx);
                Console.WriteLine(dataGridView4.Rows[rowidx].Cells[colidx].Value.ToString().Replace("Rp.", "").Replace(".", ""));
                if (colidx > 2)
                {
                    dataGridView4.Rows[rowidx].Cells[colidx].Style.Format = "F0";
                }
            }
            for (int a = 0; a < dataGridView4.Rows.Count; a++)
            {
                for (int b = 3; b < dataGridView4.Columns.Count; b++)
                {
                    if (!cells.Contains(dataGridView4.Rows[a].Cells[b]))
                    {
                        Int64 buf;
                        dataGridView4.Rows[a].Cells[b].Style.Format = "C0";
                        dataGridView4.Rows[a].Cells[b].Style.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
                    }
                }
            }
        }

        private void dataGridView7_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewSelectedCellCollection cells = dataGridView7.SelectedCells;
            foreach (DataGridViewCell cell in cells)
            {
                int rowidx = cell.RowIndex;
                int colidx = cell.ColumnIndex;
                Console.WriteLine(rowidx + ", " + colidx);
                Console.WriteLine(dataGridView7.Rows[rowidx].Cells[colidx].Value.ToString().Replace("Rp.", "").Replace(".", ""));
                if (colidx >= 2)
                {
                    dataGridView7.Rows[rowidx].Cells[colidx].Style.Format = "F0";
                }
            }
            for (int a = 0; a < dataGridView7.Rows.Count; a++)
            {
                for (int b = 2; b < dataGridView7.Columns.Count; b++)
                {
                    if (!cells.Contains(dataGridView7.Rows[a].Cells[b]))
                    {
                        Int64 buf;
                        dataGridView7.Rows[a].Cells[b].Style.Format = "C0";
                        dataGridView7.Rows[a].Cells[b].Style.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            loadForm.ShowSplashScreen();
            //save to db rs-pervendor
            ProsesOrderBlogHistory();
            List<RekonSaldoPerVendor> rspv = (from x in obhp
                                              select new RekonSaldoPerVendor()
                                              {
                                                  cashPointtId = x.cashpointId,
                                                  confId = x.confId,
                                                  orderDate = x.orderDate,
                                                  vendor = x.vendor,
                                                  actionRekon = x.action,
                                                  statusRekon = x.status,
                                                  dueDate = x.dueDate,
                                                  blogMessage = x.blogMessage,
                                                  blogTime = x.blogTime,
                                                  currencyAmmount = x.currencyAmmount,
                                                  realDate = x.realDate
                                              }).ToList();

            DateTime maxdate = rspv.Max(x => (DateTime)x.dueDate);
            DateTime mindate = rspv.Min(x => (DateTime)x.dueDate);

            var query = (from x in en.RekonSaldoPerVendors
                         where (DateTime)x.dueDate >= mindate && (DateTime)x.dueDate <= maxdate
                         select x
                         ).ToList();

            List<RekonSaldoPerVendor> rspvToRemove = new List<RekonSaldoPerVendor>();

            foreach (var item in rspv)
            {
                var query2 = (from x in query
                              where x.cashPointtId == item.cashPointtId
                              && x.confId == item.confId
                              && x.orderDate == item.orderDate
                              //&& x.vendor == item.vendor
                              && x.actionRekon == item.actionRekon
                              //&& x.blogMessage == item.blogMessage
                              && x.statusRekon == item.statusRekon
                              //&& ((DateTime)x.dueDate).Date == item.dueDate 
                              //&& (DateTime)x.blogTime == item.blogTime 
                              //&& x.currencyAmmount == item.currencyAmmount 
                              //&& ((DateTime)x.realDate).Date == item.realDate 
                              //&& x.validation == item.validation
                              select x).ToList();
                /*
                var query3 = (from x in query2
                              where x.cashPointtId == item.cashPointtId
                              && x.confId == item.confId
                              && x.orderDate == item.orderDate
                              //&& x.vendor == item.vendor
                              && x.actionRekon == item.actionRekon
                              && x.statusRekon == item.statusRekon
                              //&& ((DateTime)x.dueDate).Date == item.dueDate 
                              //&& (DateTime)x.blogTime == item.blogTime 
                              //&& x.currencyAmmount == item.currencyAmmount 
                              //&& ((DateTime)x.realDate).Date == item.realDate 
                              //&& x.validation == item.validation
                              select x).ToList();
                 */


                if (query2 != null)
                {
                    for (int i = 0; i < query2.Count; i++)
                    {
                        Console.WriteLine("UPDATE");
                        query2[i].cashPointtId = item.cashPointtId;
                        query2[i].confId = item.confId;
                        query2[i].orderDate = item.orderDate;
                        query2[i].blogMessage = item.blogMessage;
                        Console.WriteLine("update blogmessage: " + item.blogMessage);
                        query2[i].vendor = item.vendor;
                        query2[i].actionRekon = item.actionRekon;
                        //query2[i].statusRekon = item.statusRekon;
                        //query2[i].dueDate = item.dueDate;
                        //query2[i].blogTime = item.blogTime;
                        query2[i].currencyAmmount = item.currencyAmmount;
                        //query2[i].realDate = item.realDate;
                        //query2[i].validation = item.validation;

                        en.SaveChanges();

                        rspvToRemove.Add(item);
                    }
                }
                else
                {
                    Console.WriteLine("ELSE RSPV");
                }
            }

            foreach (var item in rspvToRemove)
            {
                rspv.Remove(item);
            }

            en.RekonSaldoPerVendors.AddRange(rspv);
            en.SaveChanges();
            loadForm.CloseForm();
        }

        public void pivotVault()
        {
            //area for BI
            var queryVaultBI = (from x in en.RekonSaldoVaults.AsEnumerable()
                                join y in en.Pkts.AsEnumerable() on x.vaultId.Substring(0, 4) equals y.kodePktCabang == "CCASA" ? "CCAS" : y.kodePktCabang
                                where !String.IsNullOrEmpty(x.fundingSoure)
                                select x).ToList();

            //area for atm
            var queryVaultATM = (from x in en.RekonSaldoVaults.AsEnumerable()
                                 join y in en.Pkts.AsEnumerable() on x.vaultId.Substring(0, 4) equals y.kodePktCabang == "CCASA" ? "CCAS" : y.kodePktCabang
                                 where !String.IsNullOrEmpty(x.fundingSoure) && y.kanwil.Contains("Jabo")
                                 select x).ToList();



            //QUERY VAULT IN BI- BUFFER
            var queryInBIBuffer = (from x in queryVaultBI
                                   where (x.fundingSoure.Substring(0,2) == "BI" || x.fundingSoure.Substring(0,2).Contains("OB")) && (((DateTime)x.dueDate).Date == dateTimePicker2.Value.Date || ((DateTime)x.realDate).Date == dateTimePicker2.Value.Date)
                                   select x).ToList();

            List<PivotCPC> pc = new List<PivotCPC>();

            foreach (var item in queryInBIBuffer)
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
                    validation = item.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(item.timeStampRekon.ToString()).Hour < 21 ? item.timeStampRekon : DateTime.Parse(item.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                });

            }

            //PREPARING PIVOT VAULT IN BI - BUFFER // catatan: pake vaultid // 
            var pivotInBiBuffer = pc.GroupBy(c => new { c.vaultId }).Select(g => new
            {
                vaultID = g.Key.vaultId,
                sudahValidasi = g.Where(c => c.validation == "VALIDATED" && c.action.ToLower().Contains("delivery")).Sum(c => c.currencyAmmount),
                belumValidasi = g.Where(c => c.validation == "NOT VALIDATED" && c.action.ToLower().Contains("delivery")).Sum(c => c.currencyAmmount),
                total = g.Where(c => c.validation == "NOT VALIDATED" && c.action.ToLower().Contains("delivery")).Sum(c => c.currencyAmmount) + g.Where(c => c.validation == "VALIDATED" && c.action.ToLower().Contains("delivery")).Sum(c => c.currencyAmmount)

            }).ToList();

            var pivotInBiBufferReady = (from x in pivotInBiBuffer
                                        select new { vaultID = x.vaultID.Substring(0, 4), x.sudahValidasi, x.belumValidasi, x.total }).ToList();



            //QUERY VAULT IN ATM - BUFFER
            var queryInATMBuffer = (from x in queryVaultATM
                                    where (x.fundingSoure.Substring(0,2) != "BI" && !x.fundingSoure.Substring(0,2).Contains("OB")) && (((DateTime)x.dueDate).Date == dateTimePicker2.Value.Date || ((DateTime)x.realDate).Date == dateTimePicker2.Value.Date)
                                    select x).ToList();

            List<PivotCPC> pc2 = new List<PivotCPC>();
            foreach (var item in queryInATMBuffer)
            {
                pc2.Add(new PivotCPC
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
                    validation = item.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(item.timeStampRekon.ToString()).Hour < 21 ? item.timeStampRekon : DateTime.Parse(item.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                });

            }

            //PREPARING PIVOT VAULT IN ATM - BUFFER // catatan: pake vaultid // 
            var pivotInAtmBuffer = pc2.GroupBy(c => new { c.vaultId, }).Select(g => new
            {
                vaultID = g.Key.vaultId,
                sudahValidasi = g.Where(c => c.validation == "VALIDATED" && c.action.ToLower().Contains("delivery")).Sum(c => c.currencyAmmount),
                belumValidasi = g.Where(c => c.validation == "NOT VALIDATED" && c.action.ToLower().Contains("delivery")).Sum(c => c.currencyAmmount),
                total = g.Where(c => c.validation == "NOT VALIDATED" && c.action.ToLower().Contains("delivery")).Sum(c => c.currencyAmmount) + g.Where(c => c.validation == "VALIDATED" && c.action.ToLower().Contains("delivery")).Sum(c => c.currencyAmmount)

            }).ToList();

            var pivotInAtmBufferReady = (from x in pivotInAtmBuffer
                                         select new { vaultID = x.vaultID.Substring(0, 4), x.sudahValidasi, x.belumValidasi, x.total }).ToList();

            //PENGGABUNGAN PIVOT VAULT IN
            var unionIn = pivotInBiBufferReady.Union(pivotInAtmBufferReady).ToList();

            var inReady = unionIn.GroupBy(z => new { z.vaultID }).Select(q => new
            {
                vaultID = q.Key.vaultID,
                sudahValidasi = q.Sum(z => z.sudahValidasi),
                belumValidasi = q.Sum(z => z.belumValidasi),
                total = q.Sum(z => z.total)
            }).ToList();

            pivotVault_In = new List<PivotVault_In>();

            foreach (var item in inReady)
            {
                pivotVault_In.Add(new PivotVault_In
                {
                    vendor = item.vaultID,
                    belumValidasi = item.belumValidasi,
                    sudahValidasi = item.sudahValidasi,
                    total = item.total
                });
            }


            dataGridViewIn.DataSource = pivotVault_In;

            //END OF IN


            //QUERY VAULT OUT BI- BUFFER
            var queryOutBIBuffer = (from x in queryVaultBI
                                    where (x.fundingSoure.Substring(0,2) == "BI" || x.fundingSoure.Substring(0,2).Contains("OB")) && (((DateTime)x.dueDate).Date == dateTimePicker2.Value.Date || ((DateTime)x.realDate).Date == dateTimePicker2.Value.Date)
                                    select x).ToList();

            List<PivotCPC> cp = new List<PivotCPC>();

            foreach (var item in queryOutBIBuffer)
            {
                cp.Add(new PivotCPC
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
                    validation = item.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(item.timeStampRekon.ToString()).Hour < 21 ? item.timeStampRekon : DateTime.Parse(item.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                });

            }

            //PREPARING PIVOT VAULT OUT BI - BUFFER // catatan: pake vaultid // 
            var pivotOutBiBuffer = cp.GroupBy(c => new { c.vaultId }).Select(g => new
            {
                vaultID = g.Key.vaultId,
                sudahValidasi = g.Where(c => c.validation == "VALIDATED" && c.action.ToLower().Contains("return")).Sum(c => c.currencyAmmount),
                belumValidasi = g.Where(c => c.validation == "NOT VALIDATED" && c.action.ToLower().Contains("return")).Sum(c => c.currencyAmmount),
                total = g.Where(c => c.validation == "VALIDATED" && c.action.ToLower().Contains("return")).Sum(c => c.currencyAmmount) + g.Where(c => c.validation == "NOT VALIDATED" && c.action.ToLower().Contains("return")).Sum(c => c.currencyAmmount)

            }).ToList();

            var pivotOutBiBufferReady = (from x in pivotOutBiBuffer
                                         select new { vaultID = x.vaultID.Substring(0, 4), x.sudahValidasi, x.belumValidasi, x.total }).ToList();


            //QUERY VAULT OUT ATM - BUFFER
            var queryOutAtmBuffer = (from x in queryVaultATM
                                     where (x.fundingSoure.Substring(0,2) != "BI" && !x.fundingSoure.Substring(0,2).Contains("OB")) && (((DateTime)x.dueDate).Date == dateTimePicker2.Value.Date || ((DateTime)x.realDate).Date == dateTimePicker2.Value.Date)
                                     select x).ToList();

            List<PivotCPC> cp2 = new List<PivotCPC>();

            foreach (var item in queryOutAtmBuffer)
            {
                cp2.Add(new PivotCPC
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
                    validation = item.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(item.timeStampRekon.ToString()).Hour < 21 ? item.timeStampRekon : DateTime.Parse(item.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                });

            }

            //PREPARING PIVOT VAULT OUT ATM - BUFFER // catatan: pake vaultid // 
            var pivotOutAtmBuffer = cp2.GroupBy(c => new { c.vaultId, }).Select(g => new
            {
                vaultID = g.Key.vaultId,
                sudahValidasi = g.Where(c => c.validation == "VALIDATED" && c.action.ToLower().Contains("return")).Sum(c => c.currencyAmmount),
                belumValidasi = g.Where(c => c.validation == "NOT VALIDATED" && c.action.ToLower().Contains("return")).Sum(c => c.currencyAmmount),
                total = g.Where(c => c.validation == "VALIDATED" && c.action.ToLower().Contains("return")).Sum(c => c.currencyAmmount) + g.Where(c => c.validation == "NOT VALIDATED" && c.action.ToLower().Contains("return")).Sum(c => c.currencyAmmount)

            }).ToList();

            var pivotOutAtmBufferReady = (from x in pivotOutAtmBuffer
                                          select new { vaultID = x.vaultID.Substring(0, 4), x.sudahValidasi, x.belumValidasi, x.total }).ToList();

            //PENGGABUNGAN PIVOT VAULT OUT
            var unionOut = pivotOutBiBufferReady.Union(pivotOutAtmBufferReady).ToList();

            var outReady = unionOut.GroupBy(z => new { z.vaultID }).Select(q => new
            {
                vaultID = q.Key.vaultID,
                sudahValidasi = q.Sum(z => z.sudahValidasi),
                belumValidasi = q.Sum(z => z.belumValidasi),
                total = q.Sum(z => z.total)
            }).ToList();

            pivotVault_Out = new List<PivotVault_Out>();

            foreach (var item in outReady)
            {
                pivotVault_Out.Add(new PivotVault_Out
                {
                    vendor = item.vaultID,
                    belumValidasi = item.belumValidasi,
                    sudahValidasi = item.sudahValidasi,
                    total = item.total
                });
            }
            dataGridViewOut.DataSource = pivotVault_Out;
            //end of test 
        }

        public void pivotPerVendor()
        {
            var queryPerVendor = (from x in en.RekonSaldoPerVendors.AsEnumerable()
                                  join y in en.Pkts.AsEnumerable() on x.vendor.Substring(0, 4) equals y.kodePktCabang == "CCASA" ? "CCAS" : y.kodePktCabang
                                  where !String.IsNullOrEmpty(x.vendor) && y.kanwil.Contains("Jabo")
                                  select x).ToList();

            //QUERY PER VENDOR IN
            var queryIn = (from x in queryPerVendor
                           where (x.actionRekon.Contains("Return") && x.statusRekon.Equals("In Transit")) && (((DateTime)x.dueDate).Date == dateTimePicker2.Value.Date || ((DateTime)x.realDate).Date == dateTimePicker2.Value.Date)
                           select new
                           {
                               cashPointId = x.cashPointtId,
                               confId = x.confId,
                               orderDate = x.orderDate,
                               vendor = x.vendor.Substring(0, 4),
                               actionRekon = x.actionRekon,
                               statusRekon = x.statusRekon,
                               dueDate = x.dueDate,
                               blogTime = x.blogTime,
                               currencyAmmount = x.currencyAmmount,
                               realDate = x.realDate,
                               blogMessage = x.blogMessage,
                               validation = x.blogMessage.Trim().Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                           }).ToList();



            //GENERATING PIVOT IN
            var pivotIn = queryIn.GroupBy(c => new { c.vendor }).Select(g => new
            {
                vendor = g.Key.vendor,
                belumValidasi = g.Where(c => c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount),
                sudahValidasi = g.Where(c => c.validation == "VALIDATED").Sum(c => c.currencyAmmount),
                total = g.Where(c => c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount) +
                             g.Where(c => c.validation == "VALIDATED").Sum(c => c.currencyAmmount)

            }).ToList();

            pivotPerVendor_In = new List<PivotPerVendor_In>();

            foreach (var item in pivotIn)
            {
                pivotPerVendor_In.Add(new PivotPerVendor_In
                {
                    vendor = item.vendor,
                    belumValidasi = (Int64)item.belumValidasi,
                    sudahValidasi = (Int64)item.sudahValidasi,
                    total = (Int64)item.total,
                });
            }

            dataGridViewIn.DataSource = pivotPerVendor_In;

            //QUERY PER VENDOR OUT

            var queryOut = (from x in queryPerVendor
                            where (x.actionRekon.Contains("Delivery") && x.statusRekon.Equals("Confirmed")) && (((DateTime)x.dueDate).Date == dateTimePicker2.Value.Date || ((DateTime)x.realDate).Date == dateTimePicker2.Value.Date)
                            select new
                            {
                                cashPointId = x.cashPointtId,
                                confId = x.confId,
                                orderDate = x.orderDate,
                                vendor = x.vendor.Substring(0, 4),
                                actionRekon = x.actionRekon,
                                statusRekon = x.statusRekon,
                                dueDate = x.dueDate,
                                blogTime = x.blogTime,
                                currencyAmmount = x.currencyAmmount,
                                realDate = x.realDate,
                                blogMessage = x.blogMessage,
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();

            //GENERATING PIVOT OUT
            var pivotOut = queryOut.GroupBy(c => new { c.vendor }).Select(g => new
            {
                vendor = g.Key.vendor,
                belumValidasi = g.Where(c => c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount),
                sudahValidasi = g.Where(c => c.validation == "VALIDATED").Sum(c => c.currencyAmmount),
                total = g.Where(c => c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount) +
                             g.Where(c => c.validation == "VALIDATED").Sum(c => c.currencyAmmount)

            }).ToList();

            pivotPerVendor_Out = new List<PivotPerVendor_Out>();

            foreach (var item in pivotOut)
            {
                pivotPerVendor_Out.Add(new PivotPerVendor_Out
                {
                    vendor = item.vendor,
                    belumValidasi = (Int64)item.belumValidasi,
                    sudahValidasi = (Int64)item.sudahValidasi,
                    total = (Int64)item.total,
                });
            }
            dataGridViewOut.DataSource = pivotPerVendor_Out;
        }

        private void buttonVault_Click(object sender, EventArgs e)
        {
            pivotVault();
            if (dataGridViewIn.Rows.Count > 0)
            {
                for (int i = 1; i < dataGridViewIn.Columns.Count; i++)
                {
                    dataGridViewIn.Columns[i].DefaultCellStyle.Format = "c";
                    dataGridViewIn.Columns[i].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("ID-id");
                }
            }
            if (dataGridViewOut.Rows.Count > 0)
            {
                for (int i = 1; i < dataGridViewOut.Columns.Count; i++)
                {
                    dataGridViewOut.Columns[i].DefaultCellStyle.Format = "c";
                    dataGridViewOut.Columns[i].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("ID-id");
                }
            }

        }

        private void buttonPerVendor_Click(object sender, EventArgs e)
        {
            pivotPerVendor();
            if (dataGridViewIn.Rows.Count > 0)
            {
                for (int i = 1; i < dataGridViewIn.Columns.Count; i++)
                {
                    dataGridViewIn.Columns[i].DefaultCellStyle.Format = "c";
                    dataGridViewIn.Columns[i].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("ID-id");
                }
            }
            if (dataGridViewOut.Rows.Count > 0)
            {
                for (int i = 1; i < dataGridViewOut.Columns.Count; i++)
                {
                    dataGridViewOut.Columns[i].DefaultCellStyle.Format = "c";
                    dataGridViewOut.Columns[i].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("ID-id");
                }
            }
        }

        private void buttonAll_Click(object sender, EventArgs e)
        {
            pivotVault();
            pivotPerVendor();
            var penggabunganInVault = (from x in pivotVault_In
                                       select new
                                       {
                                           vendor = x.vendor,
                                           belumValidasi = x.belumValidasi,
                                           sudahValidasi = x.sudahValidasi,
                                           total = x.total
                                       }).ToList();

            var penggabunganInPerVendor = (from x in pivotPerVendor_In
                                           select new
                                           {
                                               vendor = x.vendor,
                                               belumValidasi = x.belumValidasi,
                                               sudahValidasi = x.sudahValidasi,
                                               total = x.total
                                           }).ToList();

            var unionIn = penggabunganInVault.Union(penggabunganInPerVendor);
            var pivotAllIn = unionIn.GroupBy(x => new { x.vendor }).Select(g => new
            {
                vendor = g.Key.vendor,
                sudahValidasi = g.Sum(x => x.sudahValidasi),
                belumValidasi = g.Sum(x => x.belumValidasi),
                total = g.Sum(x => x.total)

            }).ToList();

            dataGridViewIn.DataSource = pivotAllIn;

            var penggabunganOutVault = (from x in pivotVault_Out
                                       select new
                                       {
                                           vendor = x.vendor,
                                           belumValidasi = x.belumValidasi,
                                           sudahValidasi = x.sudahValidasi,
                                           total = x.total
                                       }).ToList();

            var penggabunganOutPerVendor = (from x in pivotPerVendor_Out
                                           select new
                                           {
                                               vendor = x.vendor,
                                               belumValidasi = x.belumValidasi,
                                               sudahValidasi = x.sudahValidasi,
                                               total = x.total
                                           }).ToList();
            var unionOut = penggabunganOutVault.Union(penggabunganOutPerVendor);
            var pivotAllOut = unionOut.GroupBy(x => new { x.vendor }).Select(g => new
            {
                vendor = g.Key.vendor,
                sudahValidasi = g.Sum(x => x.sudahValidasi),
                belumValidasi = g.Sum(x => x.belumValidasi),
                total = g.Sum(x => x.total)

            }).ToList();

            dataGridViewOut.DataSource = pivotAllOut;

            if (dataGridViewIn.Rows.Count > 0)
            {
                for (int i = 1; i < dataGridViewIn.Columns.Count; i++)
                {
                    dataGridViewIn.Columns[i].DefaultCellStyle.Format = "c";
                    dataGridViewIn.Columns[i].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("ID-id");
                }
            }
            if (dataGridViewOut.Rows.Count > 0)
            {
                for (int i = 1; i < dataGridViewOut.Columns.Count; i++)
                {
                    dataGridViewOut.Columns[i].DefaultCellStyle.Format = "c";
                    dataGridViewOut.Columns[i].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("ID-id");
                }
            }


        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void dataGridViewIn_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewSelectedCellCollection cells = dataGridViewIn.SelectedCells;
            foreach (DataGridViewCell cell in cells)
            {
                int rowidx = cell.RowIndex;
                int colidx = cell.ColumnIndex;
                if (colidx > 0)
                {
                    dataGridViewIn.Rows[rowidx].Cells[colidx].Style.Format = "F0";
                }

            }
            for (int a = 0; a < dataGridViewIn.Rows.Count; a++)
            {
                for (int b = 1; b < dataGridViewIn.Columns.Count; b++)
                {
                    if (!cells.Contains(dataGridViewIn.Rows[a].Cells[b]))
                    {
                        Int64 buf;
                        dataGridViewIn.Rows[a].Cells[b].Style.Format = "C0";
                        dataGridViewIn.Rows[a].Cells[b].Style.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
                    }
                }
            }
        }

        private void dataGridViewOut_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewSelectedCellCollection cells = dataGridViewOut.SelectedCells;
            foreach (DataGridViewCell cell in cells)
            {
                int rowidx = cell.RowIndex;
                int colidx = cell.ColumnIndex;
                if (colidx > 0)
                {
                    dataGridViewOut.Rows[rowidx].Cells[colidx].Style.Format = "F0";
                }

            }
            for (int a = 0; a < dataGridViewOut.Rows.Count; a++)
            {
                for (int b = 1; b < dataGridViewOut.Columns.Count; b++)
                {
                    if (!cells.Contains(dataGridViewOut.Rows[a].Cells[b]))
                    {
                        Int64 buf;
                        dataGridViewOut.Rows[a].Cells[b].Style.Format = "C0";
                        dataGridViewOut.Rows[a].Cells[b].Style.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
                    }
                }
            }
        }
    }




    class VOBH //vault order blog history
    {
        public String vaultId { set; get; }
        public String confId { set; get; }
        public DateTime orderDate { set; get; }
        public DateTime dueDate { set; get; }
        public DateTime timeStamp { set; get; }
        public String blogMessage { set; get; }
        public String action { set; get; }
        public String status { set; get; }
        public Int64 currencyAmmount { set; get; }
    }

    class VO //vault order
    {
        public String vaultId { set; get; }
        public String confId { set; get; }
        public String fundingSource { set; get; }
        public DateTime orderDate { set; get; }
        public DateTime dueDate { set; get; }
        public String action { set; get; }
        public String status { set; get; }
        public Int64 currencyAmmount { set; get; }
    }

    class VaultProcessed //class menampung proses vault 
    {
        public String vaultId { set; get; }
        public String confId { set; get; }
        public String fundingSource { set; get; }
        public String action { set; get; }
        public String status { set; get; }
        public DateTime orderDate { set; get; }
        public DateTime dueDate { set; get; }
        public DateTime timeStamp { set; get; }
        public String blogMessage { set; get; }
        public Int64 currencyAmmount { set; get; }
        public DateTime realDate { set; get; }
        public String validation { set; get; }

    }

    class SetoranCPC //class menampung setoran cpc (vaultprocessed yang sudah di proses)
    {
        public String vaultId { set; get; }
        public String confId { set; get; }
        public String fundingSource { set; get; }
        public String action { set; get; }
        public String status { set; get; }
        public DateTime orderDate { set; get; }
        public DateTime dueDate { set; get; }
        public DateTime timeStamp { set; get; }
        public String blogMessage { set; get; }
        public Int64 currencyAmmount { set; get; }
        public DateTime realDate { set; get; }
        public String validation { set; get; }

    }

    class OBH //order blog history 
    {
        public String cashpointId { set; get; }
        public String confId { set; get; }
        public DateTime orderDate { set; get; }
        public DateTime dueDate { set; get; }
        public DateTime blogTime { set; get; }
        public String action { set; get; }
        public String status { set; get; }
        public Int64 currencyAmmount { set; get; }
        public String blogMessage { set; get; }
    }

    class OBHProcessed //obh yang di proses
    {
        public String cashpointId { set; get; }
        public String vendor { set; get; }
        public String confId { set; get; }
        public DateTime orderDate { set; get; }
        public DateTime dueDate { set; get; }
        public DateTime blogTime { set; get; }
        public String blogMessage { set; get; }
        public String action { set; get; }
        public String status { set; get; }
        public Int64 currencyAmmount { set; get; }
        public DateTime realDate { set; get; }
        public String validation { set; get; }
    }

    class PivotCPC //class pivotCPC (canvas for detailed pivot)
    {
        public String vaultId { set; get; }
        public String confId { set; get; }
        public String fundingSource { set; get; }
        public String action { set; get; }
        public String status { set; get; }
        public DateTime orderDate { set; get; }
        public DateTime dueDate { set; get; }
        public DateTime timeStamp { set; get; }
        public String blogMessage { set; get; }
        public Int64 currencyAmmount { set; get; }
        public DateTime realDate { set; get; }
        public String validation { set; get; }

    }

    class PivotCPC_BIBL //pivotCPC - BI dan BankLain return
    {
        public DateTime dueDate { set; get; }
        public DateTime realDate { set; get; }
        public String vaultId { set; get; }
        public String fundingSource { set; get; }
        // public Int64 plannedReturn { set; get; }
        public Int64 plannedReturnVal { set; get; }
        public Int64 plannedReturnNotVal { set; get; }
        // public Int64 emergencyReturn { set; get; }
        public Int64 emergencyReturnVal { set; get; }
        public Int64 emergencyReturnNotVal { set; get; }
        public Int64 grandTotal { set; get; }

    }

    class PivotCPC_BIBLD//pivotCPC - BI dan BankLain delivery
    {
        public DateTime dueDate { set; get; }
        public DateTime realDate { set; get; }
        public String vaultId { set; get; }
        public String fundingSource { set; get; }
        // public Int64 plannedDelivery { set; get; }
        public Int64 plannedDeliveryVal { set; get; }
        public Int64 plannedDeliveryNotVal { set; get; }
        // public Int64 emergencyDelivery { set; get; }
        public Int64 emergencyDeliveryVal { set; get; }
        public Int64 emergencyDeliveryNotVal { set; get; }
        public Int64 grandTotal { set; get; }

    }

    class PivotCPC_ATMD //pivotcpc - atm delivery
    {
        public DateTime dueDate { set; get; }
        public DateTime realDate { set; get; }
        public String vaultId { set; get; }
        public String fundingSource { set; get; }
        // public Int64 plannedDelivery { set; get; }
        public Int64 plannedDeliveryVal { set; get; }
        public Int64 plannedDeliveryNotVal { set; get; }
        //  public Int64 emergencyDelivery { set; get; }
        public Int64 emergencyDeliveryVal { set; get; }
        public Int64 emergencyDeliveryNotVal { set; get; }
        public Int64 grandTotal { set; get; }

    }

    class PivotCPC_ATMR //pivotcpc - atm return
    {
        public DateTime dueDate { set; get; }
        public DateTime realDate { set; get; }
        public String vaultId { set; get; }
        public String fundingSource { set; get; }
        //public Int64 plannedReturn { set; get; }
        public Int64 plannedReturnVal { set; get; }
        public Int64 plannedReturnNotVal { set; get; }
        //public Int64 emergencyReturn { set; get; }
        public Int64 emergencyReturnVal { set; get; }
        public Int64 emergencyReturnNotVal { set; get; }
        public Int64 grandTotal { set; get; }

    }

    class PivotPerVendor_bon
    {
        public DateTime dueDate { set; get; }
        public DateTime valDate { set; get; }
        public String vendor { set; get; }
        public Int64 belumValidasi { set; get; }
        public Int64 sudahValidasi { set; get; }
        public Int64 grandTotal { set; get; }

    }

    class PivotPerVendor_setoran
    {
        public DateTime dueDate { set; get; }
        public DateTime valDate { set; get; }
        public String vendor { set; get; }
        public Int64 belumValidasi { set; get; }
        public Int64 sudahValidasi { set; get; }
        public Int64 grandTotal { set; get; }

    }

    class PivotPerVendor_In
    {
        public String vendor { set; get; }
        public Int64 belumValidasi { set; get; }
        public Int64 sudahValidasi { set; get; }
        public Int64 total { set; get; }
    }

    class PivotPerVendor_Out
    {
        public String vendor { set; get; }
        public Int64 belumValidasi { set; get; }
        public Int64 sudahValidasi { set; get; }
        public Int64 total { set; get; }
    }

    class PivotVault_In
    {
        public String vendor { set; get; }
        public Int64 belumValidasi { set; get; }
        public Int64 sudahValidasi { set; get; }
        public Int64 total { set; get; }
    }
    class PivotVault_Out
    {
        public String vendor { set; get; }
        public Int64 belumValidasi { set; get; }
        public Int64 sudahValidasi { set; get; }
        public Int64 total { set; get; }
    }
}