﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace testProjectBCA
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Database1Entities : DbContext
    {
        public Database1Entities()
            : base("name=Database1Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Abaca> Abacas { get; set; }
        public virtual DbSet<Approval> Approvals { get; set; }
        public virtual DbSet<AsuransiLayanan> AsuransiLayanans { get; set; }
        public virtual DbSet<Cabang> Cabangs { get; set; }
        public virtual DbSet<Cashpoint> Cashpoints { get; set; }
        public virtual DbSet<DailyStock> DailyStocks { get; set; }
        public virtual DbSet<DetailApproval> DetailApprovals { get; set; }
        public virtual DbSet<EventTanggal> EventTanggals { get; set; }
        public virtual DbSet<ForecastDetail> ForecastDetails { get; set; }
        public virtual DbSet<ForecastHeader> ForecastHeaders { get; set; }
        public virtual DbSet<HargaLayanan> HargaLayanans { get; set; }
        public virtual DbSet<laporanBon> laporanBons { get; set; }
        public virtual DbSet<LaporanPermintaanAdhoc> LaporanPermintaanAdhocs { get; set; }
        public virtual DbSet<LaporanPermintaanBon> LaporanPermintaanBons { get; set; }
        public virtual DbSet<Nasabah> Nasabahs { get; set; }
        public virtual DbSet<Opti> Optis { get; set; }
        public virtual DbSet<OrderTracking> OrderTrackings { get; set; }
        public virtual DbSet<Password> Passwords { get; set; }
        public virtual DbSet<Pkt> Pkts { get; set; }
        public virtual DbSet<RekapSelisihAmbilSetor> RekapSelisihAmbilSetors { get; set; }
        public virtual DbSet<RekonSaldoPerVendor> RekonSaldoPerVendors { get; set; }
        public virtual DbSet<RekonSaldoVault> RekonSaldoVaults { get; set; }
        public virtual DbSet<SaveAsk> SaveAsks { get; set; }
        public virtual DbSet<saveBeeHive> saveBeeHives { get; set; }
        public virtual DbSet<saveMc> saveMcs { get; set; }
        public virtual DbSet<SaveRekap> SaveRekaps { get; set; }
        public virtual DbSet<StokPosisi> StokPosisis { get; set; }
        public virtual DbSet<TabelTarikan> TabelTarikans { get; set; }
        public virtual DbSet<TransaksiAtm> TransaksiAtms { get; set; }
        public virtual DbSet<VaultOrderBlogHistory> VaultOrderBlogHistories { get; set; }
        public virtual DbSet<TabelTarikanSetoranCRM> TabelTarikanSetoranCRMs { get; set; }
        public virtual DbSet<ApprovalPembagianSaldo> ApprovalPembagianSaldoes { get; set; }
        public virtual DbSet<DataBankLain> DataBankLains { get; set; }
        public virtual DbSet<BeritaAcara> BeritaAcaras { get; set; }
        public virtual DbSet<RekonSaldoInputanUser> RekonSaldoInputanUsers { get; set; }
        public virtual DbSet<SaldoAwalRekonSaldo> SaldoAwalRekonSaldoes { get; set; }
        public virtual DbSet<ApprovalView> ApprovalViews { get; set; }
        public virtual DbSet<ViewTransaksiAtm> ViewTransaksiAtms { get; set; }
        public virtual DbSet<SaldoMesin> SaldoMesins { get; set; }
    }
}
