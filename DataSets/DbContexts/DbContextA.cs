using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using KlonsLIB;
using KlonsLIB.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Klons3.ModelsA;

public partial class DbContextA : MyDbContext
{
    public DbContextA(DbContextOptions<DbContextA> options): base(options)
    {
    }

    public DbContextA(string connectionString, string username) : base(connectionString, username)
    {
    }

    public override void InitDbSetList()
    {
        base.InitDbSetList();
        InitBindingListMembers();
    }

    public virtual DbSet<A_BANKS> A_BANKS { get; set; }
    public virtual DbSet<A_DEPARTMENTS> A_DEPARTMENTS { get; set; }
    public virtual DbSet<A_EVENTS> A_EVENTS { get; set; }
    public virtual DbSet<A_EVENT_TYPES> A_EVENT_TYPES { get; set; }
    public virtual DbSet<A_EVENT_TYPES2> A_EVENT_TYPES2 { get; set; }
    public virtual DbSet<A_FP_PAYLISTS> A_FP_PAYLISTS { get; set; }
    public virtual DbSet<A_FP_PAYLISTS_R> A_FP_PAYLISTS_R { get; set; }
    public virtual DbSet<A_HOLIDAYS> A_HOLIDAYS { get; set; }
    public virtual DbSet<A_INCOME_CODES> A_INCOME_CODES { get; set; }
    public virtual DbSet<A_PASTDATA> A_PASTDATA { get; set; }
    public virtual DbSet<A_PASTDATA_2> A_PASTDATA_2 { get; set; }
    public virtual DbSet<A_PAYLISTS> A_PAYLISTS { get; set; }
    public virtual DbSet<A_PAYLISTS_R> A_PAYLISTS_R { get; set; }
    public virtual DbSet<A_PAYLIST_TEMPL> A_PAYLIST_TEMPL { get; set; }
    public virtual DbSet<A_PAYLIST_TEMPL_R> A_PAYLIST_TEMPL_R { get; set; }
    public virtual DbSet<A_PERSONS> A_PERSONS { get; set; }
    public virtual DbSet<A_PERSONS_FIZ> A_PERSONS_FIZ { get; set; }
    public virtual DbSet<A_PERSONS_R> A_PERSONS_R { get; set; }
    public virtual DbSet<A_PIECEWORK> A_PIECEWORK { get; set; }
    public virtual DbSet<A_PIECEWORK_CATALOG> A_PIECEWORK_CATALOG { get; set; }
    public virtual DbSet<A_PIECEWORK_CATSTRUCT> A_PIECEWORK_CATSTRUCT { get; set; }
    public virtual DbSet<A_PLUSMINUS_FROM> A_PLUSMINUS_FROM { get; set; }
    public virtual DbSet<A_PLUSMINUS_TYPES> A_PLUSMINUS_TYPES { get; set; }
    public virtual DbSet<A_POSITIONS> A_POSITIONS { get; set; }
    public virtual DbSet<A_POSITIONS_PLUSMINUS> A_POSITIONS_PLUSMINUS { get; set; }
    public virtual DbSet<A_POSITIONS_R> A_POSITIONS_R { get; set; }
    public virtual DbSet<A_PROFESSIONS> A_PROFESSIONS { get; set; }
    public virtual DbSet<A_RATES> A_RATES { get; set; }
    public virtual DbSet<A_REPORT_CODES> A_REPORT_CODES { get; set; }
    public virtual DbSet<A_SALARY_PLUSMINUS> A_SALARY_PLUSMINUS { get; set; }
    public virtual DbSet<A_SALARY_SHEETS> A_SALARY_SHEETS { get; set; }
    public virtual DbSet<A_SALARY_SHEETS_R> A_SALARY_SHEETS_R { get; set; }
    public virtual DbSet<A_SALARY_SHEETS_R_HIST> A_SALARY_SHEETS_R_HIST { get; set; }
    public virtual DbSet<A_SALARY_SHEET_TEMPL> A_SALARY_SHEET_TEMPL { get; set; }
    public virtual DbSet<A_SALARY_SHEET_TEMPL_R> A_SALARY_SHEET_TEMPL_R { get; set; }
    public virtual DbSet<A_TERITORIAL_CODES> A_TERITORIAL_CODES { get; set; }
    public virtual DbSet<A_TIMEPLAN_LIST> A_TIMEPLAN_LIST { get; set; }
    public virtual DbSet<A_TIMESHEET> A_TIMESHEET { get; set; }
    public virtual DbSet<A_TIMESHEET_LISTS> A_TIMESHEET_LISTS { get; set; }
    public virtual DbSet<A_TIMESHEET_LISTS_R> A_TIMESHEET_LISTS_R { get; set; }
    public virtual DbSet<A_TIMESHEET_TEMPL> A_TIMESHEET_TEMPL { get; set; }
    public virtual DbSet<A_TIMESHEET_TEMPL_R> A_TIMESHEET_TEMPL_R { get; set; }
    public virtual DbSet<A_UNTAXED_MIN> A_UNTAXED_MIN { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<A_BANKS>(entity =>
        {
            entity.Property(e => e.NAME)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.SID)
                .IsRequired()
                .HasMaxLength(8);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_DEPARTMENTS>(entity =>
        {
            entity.Property(e => e.ID).HasMaxLength(15);
            entity.Property(e => e.ACC1).HasMaxLength(15);
            entity.Property(e => e.ACC2).HasMaxLength(15);
            entity.Property(e => e.ACC3).HasMaxLength(15);
            entity.Property(e => e.ACC4).HasMaxLength(15);
            entity.Property(e => e.ACC5).HasMaxLength(15);
            entity.Property(e => e.ACC6).HasMaxLength(15);
            entity.Property(e => e.DESCR).HasMaxLength(50);
            entity.Property(e => e.USED_DT1).HasColumnType("DATE");
            entity.Property(e => e.USED_DT2).HasColumnType("DATE");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_EVENTS>(entity =>
        {
            entity.Property(e => e.DATE1).HasColumnType("DATE");
            entity.Property(e => e.DATE2).HasColumnType("DATE");
            entity.Property(e => e.DATE3).HasColumnType("DATE");
            entity.Property(e => e.DESCR).HasMaxLength(200);
            entity.Property(e => e.DOCNR).HasMaxLength(20);
            entity.Property(e => e.OCCUPATION_CODE).HasMaxLength(7);
            entity.Property(e => e.SCODE).HasMaxLength(5);

            entity.HasOne(d => d.Position).WithMany(p => p.Events)
                .HasForeignKey(d => d.IDA)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_EVENTS_IDA");

            entity.HasOne(d => d.EventType).WithMany()
                .HasForeignKey(d => d.IDN)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_EVENTS_IDN");

            entity.HasOne(d => d.EvenType2).WithMany()
                .HasForeignKey(d => d.IDN2)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_EVENTS_IDN2");

            entity.HasOne(d => d.Person).WithMany(p => p.Events)
                .HasForeignKey(d => d.IDP)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_EVENTS_IDP");

            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");

            entity.Ignore(e => e.EventCode);
        });

        modelBuilder.Entity<A_EVENT_TYPES>(entity =>
        {
            entity.Property(e => e.DESCR).HasMaxLength(100);
            entity.Property(e => e.SCODE).HasMaxLength(5);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_EVENT_TYPES2>(entity =>
        {
            entity.Property(e => e.DESCR)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.TAG)
                .IsRequired()
                .HasMaxLength(10);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_FP_PAYLISTS>(entity =>
        {
            entity.Property(e => e.DESCR).HasMaxLength(50);
            entity.Property(e => e.PAYDATE).HasColumnType("DATE");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_FP_PAYLISTS_R>(entity =>
        {
            entity.Property(e => e.DATE1).HasColumnType("DATE");
            entity.Property(e => e.DATE2).HasColumnType("DATE");
            entity.Property(e => e.DESCR).HasMaxLength(50);
            entity.Property(e => e.INCOME_ID)
                .IsRequired()
                .HasMaxLength(4);
            entity.Property(e => e.PAYDATE).HasColumnType("DATE");

            entity.HasOne(d => d.Person).WithMany()
                .HasForeignKey(d => d.IDP)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_FP_PAYLISTS_R_IDP");

            entity.HasOne(d => d.FpPaylist).WithMany(p => p.FpPaylistRows)
                .HasForeignKey(d => d.IDS)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_FP_PAYLISTS_R_IDS");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_HOLIDAYS>(entity =>
        {
            entity.HasKey(e => e.DT);
            entity.Property(e => e.DT).HasColumnType("DATE");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_INCOME_CODES>(entity =>
        {
            entity.Property(e => e.ID).HasMaxLength(4);
            entity.Property(e => e.DESCR)
                .IsRequired()
                .HasMaxLength(400);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_PASTDATA>(entity =>
        {
            entity.HasOne(d => d.Person).WithMany(p => p.PastDataRows)
                .HasForeignKey(d => d.IDP)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_PASTDATA_IDP");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_PASTDATA_2>(entity =>
        {
            entity.Property(e => e.DT).HasColumnType("DATE");

            entity.HasOne(d => d.IDPNavigation).WithMany(p => p.PastDat2Rows)
                .HasForeignKey(d => d.IDP)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_PASTDATA_2_IDP");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_PAYLISTS>(entity =>
        {
            entity.Property(e => e.DEP).HasMaxLength(15);
            entity.Property(e => e.DESCR)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.DT).HasColumnType("DATE");

            entity.HasOne(d => d.Department).WithMany()
                .HasForeignKey(d => d.DEP)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_PAYLISTS_DEP");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_PAYLISTS_R>(entity =>
        {
            entity.Property(e => e.DT1).HasColumnType("DATE");
            entity.Property(e => e.DT2).HasColumnType("DATE");

            entity.HasOne(d => d.Position).WithMany()
                .HasForeignKey(d => d.IDAM)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_PAYLISTS_R_IDAM");

            entity.HasOne(d => d.Person).WithMany(p => p.PayListRows)
                .HasForeignKey(d => d.IDP)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_PAYLISTS_R_IDP");

            entity.HasOne(d => d.PayList).WithMany(p => p.PayListRows)
                .HasForeignKey(d => d.IDS)
                .OnDelete(DeleteBehavior.ClientCascade)
                .HasConstraintName("FK_A_PAYLISTS_R_IDS");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_PAYLIST_TEMPL>(entity =>
        {
            entity.Property(e => e.CODE).HasMaxLength(15);
            entity.Property(e => e.DEP).HasMaxLength(15);
            entity.Property(e => e.DESCR).HasMaxLength(50);

            entity.HasOne(d => d.Department).WithMany()
                .HasForeignKey(d => d.DEP)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_PAYLIST_TEMPL_DEP");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_PAYLIST_TEMPL_R>(entity =>
        {
            entity.HasOne(d => d.Position).WithMany()
                .HasForeignKey(d => d.IDAM)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_PAYLIST_TEMPL_R_IDAM");

            entity.HasOne(d => d.Person).WithMany(p => p.PayListTemplateRows)
                .HasForeignKey(d => d.IDP)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_PAYLIST_TEMPL_R_IDP");

            entity.HasOne(d => d.PaylistTemplate).WithMany(p => p.PayListTemplateRows)
                .HasForeignKey(d => d.IDS)
                .OnDelete(DeleteBehavior.ClientCascade)
                .HasConstraintName("FK_A_PAYLIST_TEMPL_R_IDS");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_PERSONS>(entity =>
        {
            entity.Property(e => e.ADDRESS).HasMaxLength(200);
            entity.Property(e => e.BANK_ACC).HasMaxLength(50);
            entity.Property(e => e.BIRTH_DATE).HasColumnType("DATE");
            entity.Property(e => e.CITY).HasMaxLength(20);
            entity.Property(e => e.COMMENTS).HasMaxLength(1000);
            entity.Property(e => e.COUNTRY).HasMaxLength(20);
            entity.Property(e => e.EMAIL).HasMaxLength(50);
            entity.Property(e => e.FNAME)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.LNAME)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.NAME_ACCUSATIVE).HasMaxLength(40);
            entity.Property(e => e.NAME_DATIVE).HasMaxLength(40);
            entity.Property(e => e.PASSPORT_DATE).HasColumnType("DATE");
            entity.Property(e => e.PASSPORT_ISSUER).HasMaxLength(50);
            entity.Property(e => e.PASSPORT_NO).HasMaxLength(20);
            entity.Property(e => e.PHONE).HasMaxLength(30);
            entity.Property(e => e.PK).HasMaxLength(20);
            entity.Property(e => e.POSTAL_CODE).HasMaxLength(10);
            entity.Property(e => e.STATE).HasMaxLength(20);
            entity.Property(e => e.TERRITORIAL_CODE).HasMaxLength(10);
            entity.Property(e => e.USED_DT1).HasColumnType("DATE");
            entity.Property(e => e.USED_DT2).HasColumnType("DATE");

            entity.HasOne(d => d.BANK).WithMany()
                .HasForeignKey(d => d.BANK_ID)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_PERSONS_BANK_ID");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_PERSONS_FIZ>(entity =>
        {
            entity.Property(e => e.ADDRESS).HasMaxLength(200);
            entity.Property(e => e.BANK_ACC).HasMaxLength(50);
            entity.Property(e => e.BIRTH_DATE).HasColumnType("DATE");
            entity.Property(e => e.CITY).HasMaxLength(20);
            entity.Property(e => e.COMMENTS).HasMaxLength(1000);
            entity.Property(e => e.COUNTRY).HasMaxLength(20);
            entity.Property(e => e.EMAIL).HasMaxLength(50);
            entity.Property(e => e.FNAME)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.LNAME)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.NAME_ACCUSATIVE).HasMaxLength(40);
            entity.Property(e => e.NAME_DATIVE).HasMaxLength(40);
            entity.Property(e => e.PASSPORT_DATE).HasColumnType("DATE");
            entity.Property(e => e.PASSPORT_ISSUER).HasMaxLength(50);
            entity.Property(e => e.PASSPORT_NO).HasMaxLength(20);
            entity.Property(e => e.PHONE).HasMaxLength(30);
            entity.Property(e => e.PK).HasMaxLength(20);
            entity.Property(e => e.POSTAL_CODE).HasMaxLength(10);
            entity.Property(e => e.STATE).HasMaxLength(20);
            entity.Property(e => e.TAXREG_NO).HasMaxLength(20);
            entity.Property(e => e.TERRITORIAL_CODE).HasMaxLength(10);
            entity.Property(e => e.USED_DT1).HasColumnType("DATE");
            entity.Property(e => e.USED_DT2).HasColumnType("DATE");

            entity.HasOne(d => d.Bank).WithMany()
                .HasForeignKey(d => d.BANK_ID)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_PERSONS_FIZ_BANK_ID");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_PERSONS_R>(entity =>
        {
            entity.Property(e => e.EDIT_DATE).HasColumnType("DATE");
            entity.Property(e => e.FNAME)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.LNAME)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.PERSON_CODE)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.TAXDOC_ISSUER).HasMaxLength(50);
            entity.Property(e => e.TAXDOC_NO).HasMaxLength(20);
            entity.Property(e => e.TAXDOC_SERIAL).HasMaxLength(10);
            entity.Property(e => e.TAXREG_NO).HasMaxLength(20);
            entity.Property(e => e.TERRITORIAL_CODE).HasMaxLength(10);

            entity.HasOne(d => d.Person).WithMany(p => p.PersonsStateRows)
                .HasForeignKey(d => d.IDP)
                .HasConstraintName("FK_A_PERSONS_R_IDP");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_PIECEWORK>(entity =>
        {
            entity.Property(e => e.DT).HasColumnType("DATE");
            entity.Property(e => e.UNIT)
                .IsRequired()
                .HasMaxLength(10);

            entity.HasOne(d => d.Position).WithMany()
                .HasForeignKey(d => d.IDA)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_PIECEWORK_IDA");

            entity.HasOne(d => d.Catalog).WithMany()
                .HasForeignKey(d => d.IDK)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_PIECEWORK_IDK");

            entity.HasOne(d => d.Person).WithMany(p => p.PieceWorkRows)
                .HasForeignKey(d => d.IDP)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_PIECEWORK_IDP");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_PIECEWORK_CATALOG>(entity =>
        {
            entity.Property(e => e.CODE)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.DESCR)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.UNIT)
                .IsRequired()
                .HasMaxLength(10);

            entity.HasOne(d => d.CatalogSection).WithMany(p => p.CalatlogRows)
                .HasForeignKey(d => d.IDS)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_PIECEWORK_CATALOG_IDS");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_PIECEWORK_CATSTRUCT>(entity =>
        {
            entity.Property(e => e.CODE)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.DESCR)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_PLUSMINUS_FROM>(entity =>
        {
            entity.Property(e => e.DESCR).HasMaxLength(50);
            entity.Property(e => e.DESCR2).HasMaxLength(50);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_PLUSMINUS_TYPES>(entity =>
        {
            entity.Property(e => e.DESCR).HasMaxLength(50);
            entity.Property(e => e.DESCR2).HasMaxLength(15);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_POSITIONS>(entity =>
        {
            entity.Property(e => e.IDDEP).HasMaxLength(15);
            entity.Property(e => e.TITLE)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.USED_DT1).HasColumnType("DATE");

            entity.HasOne(d => d.Department).WithMany()
                .HasForeignKey(d => d.IDDEP)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_POSITIONS_IDDEP");

            entity.HasOne(d => d.Person).WithMany(p => p.Positions)
                .HasForeignKey(d => d.IDP)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_POSITIONS_IDP");

            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_POSITIONS_PLUSMINUS>(entity =>
        {
            entity.Property(e => e.DATE1).HasColumnType("DATE");
            entity.Property(e => e.DATE2).HasColumnType("DATE");
            entity.Property(e => e.DESCR).HasMaxLength(50);

            entity.HasOne(d => d.Position).WithMany(p => p.PositionsPlusMinusRows)
                .HasForeignKey(d => d.IDA)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_POSITIONS_PLUSMINUS_IDA");

            entity.HasOne(d => d.PlusMinusFromRow).WithMany()
                .HasForeignKey(d => d.IDNO)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_POSITIONS_PLUSMINUS_IDNO");

            entity.HasOne(d => d.Person).WithMany(p => p.PositionsPlusMinusRows)
                .HasForeignKey(d => d.IDP)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_POSITIONS_PLUSMINUS_IDP");

            entity.HasOne(d => d.PlusMinusType).WithMany()
                .HasForeignKey(d => d.IDSV)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_POSITIONS_PLUSMINUS_IDSV");

            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");

            entity.Ignore(e => e.XBonusFrom);
            entity.Ignore(e => e.XBonusType);
        });

        modelBuilder.Entity<A_POSITIONS_R>(entity =>
        {
            entity.Property(e => e.EDIT_DATE).HasColumnType("DATE");
            entity.Property(e => e.IDDEP).HasMaxLength(15);
            entity.Property(e => e.OCCUPATION_CODE).HasMaxLength(7);
            entity.Property(e => e.TITLE)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.TITLE_ACCUSATIVE)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.TITLE_DATIVE)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.Position).WithMany(p => p.PositionStateRows)
                .HasForeignKey(d => d.IDA)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_POSITIONS_R_IDA");

            entity.HasOne(d => d.Department).WithMany()
                .HasForeignKey(d => d.IDDEP)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_POSITIONS_R_IDDEP");

            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");

            entity.Ignore(e => e.XRateType);
        });

        modelBuilder.Entity<A_PROFESSIONS>(entity =>
        {
            entity.Property(e => e.ID).HasMaxLength(7);
            entity.Property(e => e.DESCR).HasMaxLength(300);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_RATES>(entity =>
        {
            entity.Property(e => e.ONDATE).HasColumnType("DATE");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_REPORT_CODES>(entity =>
        {
            entity.Property(e => e.CODE)
                .IsRequired()
                .HasMaxLength(5);
            entity.Property(e => e.DESCR).HasMaxLength(700);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_SALARY_PLUSMINUS>(entity =>
        {
            entity.Property(e => e.DESCR).HasMaxLength(50);

            entity.HasOne(d => d.Position).WithMany()
                .HasForeignKey(d => d.IDA)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_SALARY_PLUSMINUS_IDA");

            entity.HasOne(d => d.PositionsPlusMinusRow).WithMany()
                .HasForeignKey(d => d.IDAP)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_SALARY_PLUSMINUS_IDAP");

            entity.HasOne(d => d.PlusMinusFrom).WithMany()
                .HasForeignKey(d => d.IDNO)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_SALARY_PLUSMINUS_IDNO");

            entity.HasOne(d => d.Person).WithMany(p => p.SalaryPlusMinusRows)
                .HasForeignKey(d => d.IDP)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_SALARY_PLUSMINUS_IDP");

            entity.HasOne(d => d.PlusMinusType).WithMany()
                .HasForeignKey(d => d.IDSV)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_SALARY_PLUSMINUS_IDSV");

            entity.HasOne(d => d.SalarySheetRow).WithMany(p => p.SalaryPlusMinusRows)
                .HasForeignKey(d => d.IDSX)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_SALARY_PLUSMINUS_IDSX");

            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");

            entity.Ignore(e => e.XBonusFrom);
            entity.Ignore(e => e.XBonusType);
            entity.Ignore(e => e.XRateType);
        });

        modelBuilder.Entity<A_SALARY_SHEETS>(entity =>
        {
            entity.Property(e => e.DEP).HasMaxLength(15);
            entity.Property(e => e.DESCR).HasMaxLength(50);
            entity.Property(e => e.DT1).HasColumnType("DATE");
            entity.Property(e => e.DT2).HasColumnType("DATE");

            entity.HasOne(d => d.Department).WithMany()
                .HasForeignKey(d => d.DEP)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_SALARY_SHEETS_DEP");

            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");

            entity.Ignore(e => e.XKind);
            entity.Ignore(e => e.ISFIRSTMT);
        });

        modelBuilder.Entity<A_SALARY_SHEETS_R>(entity =>
        {
            entity.Property(e => e.COMMENTS).HasMaxLength(255);
            entity.Property(e => e.FNAME).HasMaxLength(20);
            entity.Property(e => e.LNAME).HasMaxLength(20);
            entity.Property(e => e.PAY_DATE).HasColumnType("DATE");
            entity.Property(e => e.POSITION_TITLE).HasMaxLength(50);
            entity.Property(e => e.TERRITORIAL_CODE).HasMaxLength(20);

            entity.HasOne(d => d.Position).WithMany()
                .HasForeignKey(d => d.IDAM)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_SALARY_SHEETS_R_IDAM");

            entity.HasOne(d => d.Person).WithMany(p => p.SalartSheetRows)
                .HasForeignKey(d => d.IDP)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_SALARY_SHEETS_R_IDP");

            entity.HasOne(d => d.SalarySheet).WithMany(p => p.SalarySheetRows)
                .HasForeignKey(d => d.IDS)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_SALARY_SHEETS_R_IDS");

            entity.HasOne(d => d.RowSetTotalRow).WithMany(d => d.RowSetRows)
                .HasForeignKey(d => d.IDSX)
                .OnDelete(DeleteBehavior.ClientNoAction);

            entity.HasOne(d => d.SalarySheetTotal).WithMany(p => p.SalarySheetTotalRows)
                .HasForeignKey(d => d.IDST)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_SALARY_SHEETS_R_IDST");

            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");

            entity.Ignore(e => e.XType);
        });

        modelBuilder.Entity<A_SALARY_SHEETS_R_HIST>(entity =>
        {
            entity.HasKey(e => e.IDH);
            entity.Property(e => e.COMMENTS).HasMaxLength(255);
            entity.Property(e => e.FNAME).HasMaxLength(20);
            entity.Property(e => e.LNAME).HasMaxLength(20);
            entity.Property(e => e.PAY_DATE).HasColumnType("DATE");
            entity.Property(e => e.POSITION_TITLE).HasMaxLength(50);
            entity.Property(e => e.TERRITORIAL_CODE).HasMaxLength(20);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_SALARY_SHEET_TEMPL>(entity =>
        {
            entity.Property(e => e.CODE).HasMaxLength(15);
            entity.Property(e => e.DEP).HasMaxLength(15);
            entity.Property(e => e.DESCR)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.Department).WithMany()
                .HasForeignKey(d => d.DEP)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_SALARY_SHEET_TEMPL_DEP");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_SALARY_SHEET_TEMPL_R>(entity =>
        {
            entity.HasOne(d => d.Position).WithMany()
                .HasForeignKey(d => d.IDAM)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_SALARY_SHEET_TEMPL_R_IDAM");

            entity.HasOne(d => d.Person).WithMany(p => p.SalartSheetTemplateRows)
                .HasForeignKey(d => d.IDP)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_SALARY_SHEET_TEMPL_R_IDP");

            entity.HasOne(d => d.SalarySheetTemplate).WithMany(p => p.SalarySheetTemplateRows)
                .HasForeignKey(d => d.IDS)
                .OnDelete(DeleteBehavior.ClientCascade)
                .HasConstraintName("FK_A_SALARY_SHEET_TEMPL_R_IDS");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_TERITORIAL_CODES>(entity =>
        {
            entity.Property(e => e.ID).HasMaxLength(10);
            entity.Property(e => e.DESCR).HasMaxLength(50);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_TIMEPLAN_LIST>(entity =>
        {
            entity.Property(e => e.CODE).HasMaxLength(15);
            entity.Property(e => e.DESCR).HasMaxLength(50);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_TIMESHEET>(entity =>
        {
            entity.HasOne(d => d.TimeSheetListRow).WithMany(p => p.TimeSheetRows)
                .HasForeignKey(d => d.IDL)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_TIMESHEET_IDL");

            entity.HasOne(d => d.TimePlan).WithMany(d => d.TimesheetRows)
                .HasForeignKey(d => d.IDP)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_TIMESHEET_IDP");

            entity.HasOne(d => d.Person).WithMany()
                .HasForeignKey(d => d.PERID)
                .OnDelete(DeleteBehavior.ClientNoAction);

            entity.HasOne(d => d.Position).WithMany()
                .HasForeignKey(d => d.AMID)
                .OnDelete(DeleteBehavior.ClientNoAction);

            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");

            entity.Ignore(e => e.XKind1);
        });

        modelBuilder.Entity<A_TIMESHEET_LISTS>(entity =>
        {
            entity.Property(e => e.DEP).HasMaxLength(15);
            entity.Property(e => e.DESCR)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.Department).WithMany()
                .HasForeignKey(d => d.DEP)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_TIMESHEET_LISTS_DEP");

            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");

            entity.Ignore(e => e.ISFIRSTMT);
        });

        modelBuilder.Entity<A_TIMESHEET_LISTS_R>(entity =>
        {
            entity.HasOne(d => d.Position).WithMany()
                .HasForeignKey(d => d.IDAM)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_TIMESHEET_LISTS_R_IDAM");

            entity.HasOne(d => d.Person).WithMany(p => p.TimesheetListRows)
                .HasForeignKey(d => d.IDP)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_TIMESHEET_LISTS_R_IDP");

            entity.HasOne(d => d.Timesheet).WithMany(p => p.TimesheetListRows)
                .HasForeignKey(d => d.IDS)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_TIMESHEET_LISTS_R_IDS");

            entity.HasOne(d => d.TimePlan).WithMany()
                .HasForeignKey(d => d.IDPL)
                .OnDelete(DeleteBehavior.ClientNoAction);

            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");

            entity.Ignore(e => e.XPlanType);
        });

        modelBuilder.Entity<A_TIMESHEET_TEMPL>(entity =>
        {
            entity.Property(e => e.CODE).HasMaxLength(15);
            entity.Property(e => e.DEP).HasMaxLength(15);
            entity.Property(e => e.DESCR)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.DEPNavigation).WithMany()
                .HasForeignKey(d => d.DEP)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_TIMESHEET_TEMPL_DEP");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_TIMESHEET_TEMPL_R>(entity =>
        {
            entity.HasOne(d => d.Position).WithMany()
                .HasForeignKey(d => d.IDAM)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_TIMESHEET_TEMPL_R_IDAM");

            entity.HasOne(d => d.Person).WithMany(p => p.TimeSheetTemplateRows)
                .HasForeignKey(d => d.IDP)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_TIMESHEET_TEMPL_R_IDP");

            entity.HasOne(d => d.TimePlan).WithMany()
                .HasForeignKey(d => d.IDPL)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_TIMESHEET_TEMPL_R_IDPL");

            entity.HasOne(d => d.TimeSheetTemplate).WithMany(p => p.TimeSheetTemplateRows)
                .HasForeignKey(d => d.IDS)
                .OnDelete(DeleteBehavior.ClientCascade)
                .HasConstraintName("FK_A_TIMESHEET_TEMPL_R_IDS");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<A_UNTAXED_MIN>(entity =>
        {
            entity.Property(e => e.ONDATE).HasColumnType("DATE");

            entity.HasOne(d => d.Person).WithMany(p => p.UntaxedMinRows)
                .HasForeignKey(d => d.IDP)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_A_UNTAXED_MIN_IDP");

            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");

            entity.Ignore(x => x.FilterTag);
        });

        modelBuilder.Entity<A_AVPAYCALC>(entity => {
            entity.HasNoKey();
        });

        modelBuilder.Entity<A_PAYSALDO>(entity => {
            entity.HasNoKey();
        });

        modelBuilder.Entity<A_SP_PAY_MATCHLISTS>(entity => {
            entity.HasNoKey();
        });

        modelBuilder.Entity<A_SP_PAY_MATCHLISTS_1X>(entity => {
            entity.HasNoKey();
        });

        modelBuilder.Entity<A_SP_PAY_MATCHLISTS_2X>(entity => {
            entity.HasNoKey();
        });

        modelBuilder.Entity<A_SP_GET_MINMAX>(entity => {
            entity.HasNoKey();
        });

        modelBuilder.Entity<A_SP_REP_AGGREGATE>(entity => {
            entity.HasNoKey();
        });
        

        modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotifications);
    }
    #region BindingLists from DbSets

    public BindingList<A_BANKS> BL_A_BANKS { get; private set; }
    public BindingList<A_DEPARTMENTS> BL_A_DEPARTMENTS { get; private set; }
    public BindingList<A_EVENTS> BL_A_EVENTS { get; private set; }
    public BindingList<A_EVENT_TYPES> BL_A_EVENT_TYPES { get; private set; }
    public BindingList<A_EVENT_TYPES2> BL_A_EVENT_TYPES2 { get; private set; }
    public BindingList<A_FP_PAYLISTS> BL_A_FP_PAYLISTS { get; private set; }
    public BindingList<A_FP_PAYLISTS_R> BL_A_FP_PAYLISTS_R { get; private set; }
    public BindingList<A_HOLIDAYS> BL_A_HOLIDAYS { get; private set; }
    public BindingList<A_INCOME_CODES> BL_A_INCOME_CODES { get; private set; }
    public BindingList<A_PASTDATA> BL_A_PASTDATA { get; private set; }
    public BindingList<A_PASTDATA_2> BL_A_PASTDATA_2 { get; private set; }
    public BindingList<A_PAYLISTS> BL_A_PAYLISTS { get; private set; }
    public BindingList<A_PAYLISTS_R> BL_A_PAYLISTS_R { get; private set; }
    public BindingList<A_PAYLIST_TEMPL> BL_A_PAYLIST_TEMPL { get; private set; }
    public BindingList<A_PAYLIST_TEMPL_R> BL_A_PAYLIST_TEMPL_R { get; private set; }
    public BindingList<A_PERSONS> BL_A_PERSONS { get; private set; }
    public BindingList<A_PERSONS_FIZ> BL_A_PERSONS_FIZ { get; private set; }
    public BindingList<A_PERSONS_R> BL_A_PERSONS_R { get; private set; }
    public BindingList<A_PIECEWORK> BL_A_PIECEWORK { get; private set; }
    public BindingList<A_PIECEWORK_CATALOG> BL_A_PIECEWORK_CATALOG { get; private set; }
    public BindingList<A_PIECEWORK_CATSTRUCT> BL_A_PIECEWORK_CATSTRUCT { get; private set; }
    public BindingList<A_PLUSMINUS_FROM> BL_A_PLUSMINUS_FROM { get; private set; }
    public BindingList<A_PLUSMINUS_TYPES> BL_A_PLUSMINUS_TYPES { get; private set; }
    public BindingList<A_POSITIONS> BL_A_POSITIONS { get; private set; }
    public BindingList<A_POSITIONS_PLUSMINUS> BL_A_POSITIONS_PLUSMINUS { get; private set; }
    public BindingList<A_POSITIONS_R> BL_A_POSITIONS_R { get; private set; }
    public BindingList<A_PROFESSIONS> BL_A_PROFESSIONS { get; private set; }
    public BindingList<A_RATES> BL_A_RATES { get; private set; }
    public BindingList<A_REPORT_CODES> BL_A_REPORT_CODES { get; private set; }
    public BindingList<A_SALARY_PLUSMINUS> BL_A_SALARY_PLUSMINUS { get; private set; }
    public BindingList<A_SALARY_SHEETS> BL_A_SALARY_SHEETS { get; private set; }
    public BindingList<A_SALARY_SHEETS_R> BL_A_SALARY_SHEETS_R { get; private set; }
    public BindingList<A_SALARY_SHEETS_R_HIST> BL_A_SALARY_SHEETS_R_HIST { get; private set; }
    public BindingList<A_SALARY_SHEET_TEMPL> BL_A_SALARY_SHEET_TEMPL { get; private set; }
    public BindingList<A_SALARY_SHEET_TEMPL_R> BL_A_SALARY_SHEET_TEMPL_R { get; private set; }
    public BindingList<A_TERITORIAL_CODES> BL_A_TERITORIAL_CODES { get; private set; }
    public BindingList<A_TIMEPLAN_LIST> BL_A_TIMEPLAN_LIST { get; private set; }
    public BindingList<A_TIMESHEET> BL_A_TIMESHEET { get; private set; }
    public BindingList<A_TIMESHEET_LISTS> BL_A_TIMESHEET_LISTS { get; private set; }
    public BindingList<A_TIMESHEET_LISTS_R> BL_A_TIMESHEET_LISTS_R { get; private set; }
    public BindingList<A_TIMESHEET_TEMPL> BL_A_TIMESHEET_TEMPL { get; private set; }
    public BindingList<A_TIMESHEET_TEMPL_R> BL_A_TIMESHEET_TEMPL_R { get; private set; }
    public BindingList<A_UNTAXED_MIN> BL_A_UNTAXED_MIN { get; private set; }

    #endregion

}
