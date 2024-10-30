using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using KlonsLIB.Data;
using System.ComponentModel;
using Klons3.ModelsFRep;

namespace Klons3.ModelsM;

public partial class DbContextM : MyDbContext
{
    public DbContextM(DbContextOptions<DbContextM> options): base(options)
    {
    }
    public DbContextM(string connectionString, string username) : base(connectionString, username)
    {
    }

    public override void InitDbSetList()
    {
        base.InitDbSetList();
        InitBindingListMembers();
    }

    public virtual DbSet<M_ACCOUNTS> M_ACCOUNTS { get; set; }
    public virtual DbSet<M_ACCOUNTTYPE> M_ACCOUNTTYPE { get; set; }
    public virtual DbSet<M_ADDRESSSES> M_ADDRESSSES { get; set; }
    public virtual DbSet<M_BANKACCOUNTS> M_BANKACCOUNTS { get; set; }
    public virtual DbSet<M_BANKS> M_BANKS { get; set; }
    public virtual DbSet<M_CONTACTS> M_CONTACTS { get; set; }
    public virtual DbSet<M_COUNTRIES> M_COUNTRIES { get; set; }
    public virtual DbSet<M_DISC_LISTS> M_DISC_LISTS { get; set; }
    public virtual DbSet<M_DISC_LISTS_P> M_DISC_LISTS_P { get; set; }
    public virtual DbSet<M_DISC_LISTS_R> M_DISC_LISTS_R { get; set; }
    public virtual DbSet<M_DOCS> M_DOCS { get; set; }
    public virtual DbSet<M_DOCTYPES> M_DOCTYPES { get; set; }
    public virtual DbSet<M_INV_DOCS> M_INV_DOCS { get; set; }
    public virtual DbSet<M_INV_ROWS> M_INV_ROWS { get; set; }
    public virtual DbSet<M_ITEMS> M_ITEMS { get; set; }
    public virtual DbSet<M_ITEMS_CAT> M_ITEMS_CAT { get; set; }
    public virtual DbSet<M_ITEMS_PER_STORE> M_ITEMS_PER_STORE { get; set; }
    public virtual DbSet<M_LINKS> M_LINKS { get; set; }
    public virtual DbSet<M_PAYMENTTYPE> M_PAYMENTTYPE { get; set; }
    public virtual DbSet<M_PRICE_LISTS> M_PRICE_LISTS { get; set; }
    public virtual DbSet<M_PRICE_LISTS_P> M_PRICE_LISTS_P { get; set; }
    public virtual DbSet<M_PRICE_LISTS_R> M_PRICE_LISTS_R { get; set; }
    public virtual DbSet<M_PVNRATES> M_PVNRATES { get; set; }
    public virtual DbSet<M_PVNRATES2> M_PVNRATES2 { get; set; }
    public virtual DbSet<M_PVNTEXTS> M_PVNTEXTS { get; set; }
    public virtual DbSet<M_PVNTYPE> M_PVNTYPE { get; set; }
    public virtual DbSet<M_ROWS> M_ROWS { get; set; }
    public virtual DbSet<M_STORES> M_STORES { get; set; }
    public virtual DbSet<M_STORES_CAT> M_STORES_CAT { get; set; }
    public virtual DbSet<M_STORETYPE> M_STORETYPE { get; set; }
    public virtual DbSet<M_TRANSACTIONTYPE> M_TRANSACTIONTYPE { get; set; }
    public virtual DbSet<M_UNITS> M_UNITS { get; set; }
    public virtual DbSet<M_VEHICLES> M_VEHICLES { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<M_ACCOUNTS>(entity =>
        {
            entity.Property(e => e.ID).HasMaxLength(8);
            entity.Property(e => e.NAME).HasMaxLength(100);

            entity.HasOne(d => d.AccountType).WithMany()
                .HasForeignKey(d => d.TP)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_ACCOUNTS_TP");
            
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");

            entity.Ignore(x => x.XAccountType);
        });

        modelBuilder.Entity<M_ACCOUNTTYPE>(entity =>
        {
            entity.Property(e => e.NAME).HasMaxLength(20);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<M_ADDRESSSES>(entity =>
        {
            entity.Property(e => e.ADDRESS)
                .IsRequired()
                .HasMaxLength(300);
            entity.Property(e => e.NAME)
                .IsRequired()
                .HasMaxLength(300);

            entity.HasOne(d => d.Country).WithMany()
                .HasForeignKey(d => d.IDCOUNTRY)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_ADDRESSSES_IDCOUNTRY");

            entity.HasOne(d => d.Store).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.IDSTORE)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_ADDRESSSES_IDSTORE");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<M_BANKACCOUNTS>(entity =>
        {
            entity.Property(e => e.ACCOUNT)
                .IsRequired()
                .HasMaxLength(30);

            entity.HasOne(d => d.Bank).WithMany()
                .HasForeignKey(d => d.IDBANK)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_BANKACCOUNTS_IDBANK");

            entity.HasOne(d => d.Store).WithMany(p => p.BankAccounts)
                .HasForeignKey(d => d.IDSTORE)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_BANKACCOUNTS_IDSTORE");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<M_BANKS>(entity =>
        {
            entity.Property(e => e.CODE).HasMaxLength(10);
            entity.Property(e => e.NAME).HasMaxLength(100);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<M_CONTACTS>(entity =>
        {
            entity.Property(e => e.DESCR).HasMaxLength(200);
            entity.Property(e => e.EMAIL).HasMaxLength(50);
            entity.Property(e => e.NAME)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.PHONE).HasMaxLength(20);

            entity.HasOne(d => d.Store).WithMany(p => p.Contacts)
                .HasForeignKey(d => d.IDSTORE)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_CONTACTS_IDSTORE");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<M_COUNTRIES>(entity =>
        {
            entity.Property(e => e.CODE2)
                .IsRequired()
                .HasMaxLength(5);
            entity.Property(e => e.CODE3)
                .IsRequired()
                .HasMaxLength(5);
            entity.Property(e => e.NAME)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<M_DISC_LISTS>(entity =>
        {
            entity.Property(e => e.CODE)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.NAME)
                .IsRequired()
                .HasMaxLength(300);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<M_DISC_LISTS_P>(entity =>
        {
            entity.HasOne(d => d.DiscountList).WithMany(p => p.ForStores)
                .HasForeignKey(d => d.IDL)
                .OnDelete(DeleteBehavior.ClientCascade)
                .HasConstraintName("FK_M_DISC_LISTS_P_IDL");

            entity.HasOne(d => d.Store).WithMany()
                .HasForeignKey(d => d.IDSTORE)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_DISC_LISTS_P_IDSTORE");

            entity.HasOne(d => d.StoresCategory).WithMany()
                .HasForeignKey(d => d.IDSTORESCAT)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_DISC_LISTS_P_IDSTORESCAT");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<M_DISC_LISTS_R>(entity =>
        {
            entity.HasOne(d => d.Item).WithMany(p => p.M_DISC_LISTS_R)
                .HasForeignKey(d => d.IDITEM)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_DISC_LISTS_R_IDITEM");

            entity.HasOne(d => d.ItemsCategory).WithMany()
                .HasForeignKey(d => d.IDITEMSCAT)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_DISC_LISTS_R_IDITEMSCAT");

            entity.HasOne(d => d.DiscountList).WithMany(p => p.ForItems)
                .HasForeignKey(d => d.IDL)
                .OnDelete(DeleteBehavior.ClientCascade)
                .HasConstraintName("FK_M_DISC_LISTS_R_IDL");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<M_DOCS>(entity =>
        {
            entity.Property(e => e.ACCIN)
                .IsRequired()
                .HasMaxLength(8);
            entity.Property(e => e.ACCOUT)
                .IsRequired()
                .HasMaxLength(8);
            entity.Property(e => e.CREDDOCDT).HasColumnType("DATE");
            entity.Property(e => e.CREDDOCNR).HasMaxLength(20);
            entity.Property(e => e.CREDDOCSR).HasMaxLength(10);
            entity.Property(e => e.DT).HasColumnType("DATE");
            entity.Property(e => e.DUEDATE).HasColumnType("DATE");
            entity.Property(e => e.NR).HasMaxLength(20);
            entity.Property(e => e.SR).HasMaxLength(10);

            entity.HasOne(d => d.AddressIn).WithMany()
                .HasForeignKey(d => d.IDADDRESSIN)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_DOCS_IDADDRESSIN");

            entity.HasOne(d => d.AddressOut).WithMany()
                .HasForeignKey(d => d.IDADDRESSOUT)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_DOCS_IDADDRESSOUT");

            entity.HasOne(d => d.Carrier).WithMany()
                .HasForeignKey(d => d.IDCARRIER)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_DOCS_IDCARRIER");

            entity.HasOne(d => d.Driver).WithMany()
                .HasForeignKey(d => d.IDDRIVER)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_DOCS_IDDRIVER");

            entity.HasOne(d => d.PaymentType).WithMany()
                .HasForeignKey(d => d.IDPAYMENTTYPE)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_DOCS_IDPAYMENTTYPE");

            entity.HasOne(d => d.StoreIn).WithMany()
                .HasForeignKey(d => d.IDSTOREIN)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_DOCS_IDSTOREIN");

            entity.HasOne(d => d.StoreOut).WithMany()
                .HasForeignKey(d => d.IDSTOREOUT)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_DOCS_IDSTOREOUT");

            entity.HasOne(d => d.TransactionType).WithMany()
                .HasForeignKey(d => d.IDTRANSACTIONTYPE)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_DOCS_IDTRANSACTIONTYPE");

            entity.HasOne(d => d.Vehicle).WithMany()
                .HasForeignKey(d => d.IDVEHICLE)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_DOCS_IDVEHICLE");

            entity.HasOne(d => d.DocType).WithMany()
                .HasForeignKey(d => d.TP)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_DOCS_TP");

            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");

            entity.Ignore(x => x.XState);
            entity.Ignore(x => x.XPVNType);
            entity.Ignore(x => x.XDocType);
            entity.Ignore(x => x.XDocType2);
            entity.Ignore(x => x.XAccountingType);
            entity.Ignore(x => x.XDoAutoFinOps);
            entity.Ignore(x => x.XIncludeInCostCalc);
            entity.Ignore(x => x.XWeVATPayer);
            entity.Ignore(x => x.IsOpenForChanges);
        });

        modelBuilder.Entity<M_DOCTYPES>(entity =>
        {
            entity.Property(e => e.CODE).HasMaxLength(50);
            entity.Property(e => e.NAME).HasMaxLength(200);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<M_INV_DOCS>(entity =>
        {
            entity.Property(e => e.DT).HasColumnType("DATE");
            entity.Property(e => e.NR).HasMaxLength(10);
            entity.Property(e => e.PERSONS).HasMaxLength(200);

            entity.HasOne(d => d.Store).WithMany()
                .HasForeignKey(d => d.IDSTORE)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_INV_DOCS_IDSTORE");

            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");

            entity.Ignore(x => x.XState);
        });

        modelBuilder.Entity<M_INV_ROWS>(entity =>
        {
            entity.HasOne(d => d.Doc).WithMany(p => p.Rows)
                .HasForeignKey(d => d.IDDOC)
                .OnDelete(DeleteBehavior.ClientCascade)
                .HasConstraintName("FK_M_INV_ROWS_IDDOC");

            entity.HasOne(d => d.Item).WithMany()
                .HasForeignKey(d => d.IDITEM)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_INV_ROWS_IDITEM");

            entity.HasOne(d => d.Units).WithMany()
                .HasForeignKey(d => d.IDUNITS)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_INV_ROWS_IDUNITS");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<M_ITEMS>(entity =>
        {
            entity.Property(e => e.BARCODE)
                .IsRequired()
                .HasMaxLength(30);
            entity.Property(e => e.LASTBUYDATE).HasColumnType("DATE");
            entity.Property(e => e.LASTSALEDATE).HasColumnType("DATE");
            entity.Property(e => e.NAME)
                .IsRequired()
                .HasMaxLength(300);

            entity.HasOne(d => d.Category).WithMany()
                .HasForeignKey(d => d.CAT)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_ITEMS_CAT");

            entity.HasOne(d => d.PVNRateRow).WithMany()
                .HasForeignKey(d => d.PVNRATE)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_ITEMS_PVNRATE");

            entity.HasOne(d => d.StoreMain).WithMany(p => p.Items)
                .HasForeignKey(d => d.STORE1)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_ITEMS_STORE1");

            entity.HasOne(d => d.UnitsRow).WithMany()
                .HasForeignKey(d => d.UNITS)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_ITEMS_UNITS");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<M_ITEMS_CAT>(entity =>
        {
            entity.Property(e => e.ACC6)
                .IsRequired()
                .HasMaxLength(8);
            entity.Property(e => e.ACC7)
                .IsRequired()
                .HasMaxLength(8);
            entity.Property(e => e.CODE)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.NAME)
                .IsRequired()
                .HasMaxLength(300);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");

            entity.Ignore(x => x.XIsProduced);
            entity.Ignore(x => x.XIsServices);
            entity.Ignore(x => x.XMethod);
        });

        modelBuilder.Entity<M_ITEMS_PER_STORE>(entity =>
        {
            entity.HasKey(e => new { e.IDITEM, e.IDSTORE });
            entity.HasOne(d => d.Item).WithMany(p => p.ItemsPerStore)
                .HasForeignKey(d => d.IDITEM)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_ITEMS_PER_STORE_IDITEM");

            entity.HasOne(d => d.Store).WithMany(p => p.M_ITEMS_PER_STORE)
                .HasForeignKey(d => d.IDSTORE)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_ITEMS_PER_STORE_IDSTORE");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<M_LINKS>(entity =>
        {
            entity.HasKey(e => new { e.ID1, e.ID2 });

            entity.HasOne(d => d.ID1Navigation).WithMany()
                .HasForeignKey(d => d.ID1)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_LINKS_ID1");

            entity.HasOne(d => d.ID2Navigation).WithMany()
                .HasForeignKey(d => d.ID2)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_LINKS_ID2");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<M_PAYMENTTYPE>(entity =>
        {
            entity.Property(e => e.NAME).HasMaxLength(200);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<M_PRICE_LISTS>(entity =>
        {
            entity.Property(e => e.CODE)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.NAME)
                .IsRequired()
                .HasMaxLength(300);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<M_PRICE_LISTS_P>(entity =>
        {
            entity.HasOne(d => d.PriceList).WithMany(p => p.ForStores)
                .HasForeignKey(d => d.IDL)
                .OnDelete(DeleteBehavior.ClientCascade)
                .HasConstraintName("FK_M_PRICE_LISTS_P_IDL");

            entity.HasOne(d => d.Store).WithMany()
                .HasForeignKey(d => d.IDSTORE)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_PRICE_LISTS_P_IDSTORE");

            entity.HasOne(d => d.StoresCategory).WithMany()
                .HasForeignKey(d => d.IDSTORESCAT)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_PRICE_LISTS_P_IDSTORESCAT");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<M_PRICE_LISTS_R>(entity =>
        {
            entity.HasOne(d => d.Item).WithMany(p => p.M_PRICE_LISTS_R)
                .HasForeignKey(d => d.IDITEM)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_PRICE_LISTS_R_IDITEM");

            entity.HasOne(d => d.PriceList).WithMany(p => p.ForItems)
                .HasForeignKey(d => d.IDL)
                .OnDelete(DeleteBehavior.ClientCascade)
                .HasConstraintName("FK_M_PRICE_LISTS_R_IDL");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<M_PVNRATES>(entity =>
        {
            entity.Property(e => e.CODE)
                .IsRequired()
                .HasMaxLength(10);
            entity.Property(e => e.NAME).HasMaxLength(300);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<M_PVNRATES2>(entity =>
        {
            entity.Property(e => e.BASE_CRED_PVN).HasMaxLength(10);
            entity.Property(e => e.BASE_DEB_PVN).HasMaxLength(10);
            entity.Property(e => e.PVN_CRED_PVN).HasMaxLength(10);
            entity.Property(e => e.PVN_DEB_PVN).HasMaxLength(10);

            entity.HasOne(d => d.PVNRate).WithMany(p => p.PVNRates2Rows)
                .HasForeignKey(d => d.IDRATE)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_PVNRATES2_IDRATE");

            entity.HasOne(d => d.PVNType).WithMany(p => p.PVNRates2Rows)
                .HasForeignKey(d => d.IDTP)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_PVNRATES2_IDTP");
            
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");

            entity.Ignore(x => x.XPvnType);
            entity.Ignore(x => x.XDocType);
            entity.Ignore(x => x.XBaseDebFin);
            entity.Ignore(x => x.XBaseCredFin);
            entity.Ignore(x => x.XPvnDebFin);
            entity.Ignore(x => x.XPvnCredFin);
            entity.Ignore(x => x.XInCurrentMonth);
            entity.Ignore(x => x.XChangeSign);
        });

        modelBuilder.Entity<M_PVNTEXTS>(entity =>
        {
            entity.Property(e => e.CODE)
                .IsRequired()
                .HasMaxLength(10);
            entity.Property(e => e.TAG1).HasMaxLength(300);
            entity.Property(e => e.TAG2).HasMaxLength(300);
            entity.Property(e => e.VERSION).HasMaxLength(10);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<M_PVNTYPE>(entity =>
        {
            entity.Property(e => e.CODE).HasMaxLength(15);
            entity.Property(e => e.NAME).HasMaxLength(200);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<M_ROWS>(entity =>
        {
            entity.Property(e => e.ACC6)
                .IsRequired()
                .HasMaxLength(8);
            entity.Property(e => e.ACC7)
                .IsRequired()
                .HasMaxLength(8);

            entity.HasOne(d => d.Doc).WithMany(p => p.Rows)
                .HasForeignKey(d => d.IDDOC)
                .OnDelete(DeleteBehavior.ClientCascade)
                .HasConstraintName("FK_M_ROWS_IDDOC");

            entity.HasOne(d => d.Item).WithMany()
                .HasForeignKey(d => d.IDITEM)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_ROWS_IDITEM");

            entity.HasOne(d => d.PVNRate).WithMany()
                .HasForeignKey(d => d.IDPVNRATE)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_ROWS_IDPVNRATE");

            entity.HasOne(d => d.UnitsRow).WithMany()
                .HasForeignKey(d => d.UNITS)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_ROWS_UNITS");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<M_STORES>(entity =>
        {
            entity.Property(e => e.ACC21)
                .IsRequired()
                .HasMaxLength(8);
            entity.Property(e => e.ACC23)
                .IsRequired()
                .HasMaxLength(8);
            entity.Property(e => e.ACC53)
                .IsRequired()
                .HasMaxLength(8);
            entity.Property(e => e.ADDR).HasMaxLength(500);
            entity.Property(e => e.CODE).HasMaxLength(20);
            entity.Property(e => e.NAME).HasMaxLength(200);
            entity.Property(e => e.PVNREGNR).HasMaxLength(20);
            entity.Property(e => e.REGNR).HasMaxLength(20);

            entity.HasOne(d => d.Category).WithMany()
                .HasForeignKey(d => d.IDCAT)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_STORES_IDCAT");

            entity.HasOne(d => d.PVNType).WithMany()
                .HasForeignKey(d => d.PVNTP)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_STORES_PVNTP");

            entity.HasOne(d => d.StoreType).WithMany()
                .HasForeignKey(d => d.TP)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_STORES_TP");
            
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");

            entity.Ignore(x => x.XStoreType);
            entity.Ignore(x => x.XStorePVNType);
        });

        modelBuilder.Entity<M_STORES_CAT>(entity =>
        {
            entity.Property(e => e.CODE)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.NAME)
                .IsRequired()
                .HasMaxLength(300);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<M_STORETYPE>(entity =>
        {
            entity.Property(e => e.NAME).HasMaxLength(200);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<M_TRANSACTIONTYPE>(entity =>
        {
            entity.Property(e => e.NAME).HasMaxLength(200);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<M_UNITS>(entity =>
        {
            entity.Property(e => e.CODE)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.NAME).HasMaxLength(100);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<M_VEHICLES>(entity =>
        {
            entity.Property(e => e.NAME).HasMaxLength(20);
            entity.Property(e => e.REGNR).HasMaxLength(15);

            entity.HasOne(d => d.Store).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.IDSTORE)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_M_VEHICLES_IDSTORE");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotifications);
    }
    #region BindingLists from DbSets

    public BindingList<M_ACCOUNTS> BL_M_ACCOUNTS { get; set; }
    public BindingList<M_ACCOUNTTYPE> BL_M_ACCOUNTTYPE { get; set; }
    public BindingList<M_ADDRESSSES> BL_M_ADDRESSSES { get; set; }
    public BindingList<M_BANKACCOUNTS> BL_M_BANKACCOUNTS { get; set; }
    public BindingList<M_BANKS> BL_M_BANKS { get; set; }
    public BindingList<M_CONTACTS> BL_M_CONTACTS { get; set; }
    public BindingList<M_COUNTRIES> BL_M_COUNTRIES { get; set; }
    public BindingList<M_DISC_LISTS> BL_M_DISC_LISTS { get; set; }
    public BindingList<M_DISC_LISTS_P> BL_M_DISC_LISTS_P { get; set; }
    public BindingList<M_DISC_LISTS_R> BL_M_DISC_LISTS_R { get; set; }
    public BindingList<M_DOCS> BL_M_DOCS { get; set; }
    public BindingList<M_DOCTYPES> BL_M_DOCTYPES { get; set; }
    public BindingList<M_INV_DOCS> BL_M_INV_DOCS { get; set; }
    public BindingList<M_INV_ROWS> BL_M_INV_ROWS { get; set; }
    public BindingList<M_ITEMS> BL_M_ITEMS { get; set; }
    public BindingList<M_ITEMS_CAT> BL_M_ITEMS_CAT { get; set; }
    public BindingList<M_ITEMS_PER_STORE> BL_M_ITEMS_PER_STORE { get; set; }
    public BindingList<M_LINKS> BL_M_LINKS { get; set; }
    public BindingList<M_PAYMENTTYPE> BL_M_PAYMENTTYPE { get; set; }
    public BindingList<M_PRICE_LISTS> BL_M_PRICE_LISTS { get; set; }
    public BindingList<M_PRICE_LISTS_P> BL_M_PRICE_LISTS_P { get; set; }
    public BindingList<M_PRICE_LISTS_R> BL_M_PRICE_LISTS_R { get; set; }
    public BindingList<M_PVNRATES> BL_M_PVNRATES { get; set; }
    public BindingList<M_PVNRATES2> BL_M_PVNRATES2 { get; set; }
    public BindingList<M_PVNTEXTS> BL_M_PVNTEXTS { get; set; }
    public BindingList<M_PVNTYPE> BL_M_PVNTYPE { get; set; }
    public BindingList<M_ROWS> BL_M_ROWS { get; set; }
    public BindingList<M_STORES> BL_M_STORES { get; set; }
    public BindingList<M_STORES_CAT> BL_M_STORES_CAT { get; set; }
    public BindingList<M_STORETYPE> BL_M_STORETYPE { get; set; }
    public BindingList<M_TRANSACTIONTYPE> BL_M_TRANSACTIONTYPE { get; set; }
    public BindingList<M_UNITS> BL_M_UNITS { get; set; }
    public BindingList<M_VEHICLES> BL_M_VEHICLES { get; set; }

    #endregion

}
