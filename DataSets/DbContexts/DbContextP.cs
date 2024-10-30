using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using KlonsLIB.Data;
using Klons3.ModelsM;

namespace Klons3.ModelsP;

public partial class DbContextP : MyDbContext
{
    public DbContextP(DbContextOptions<DbContextP> options): base(options)
    {
    }
    public DbContextP(string connectionString, string username) : base(connectionString, username)
    {
    }

    public virtual DbSet<P_CAT1> P_CAT1 { get; set; }
    public virtual DbSet<P_CATD> P_CATD { get; set; }
    public virtual DbSet<P_CATT> P_CATT { get; set; }
    public virtual DbSet<P_DEPARTMENTS> P_DEPARTMENTS { get; set; }
    public virtual DbSet<P_EVENTS> P_EVENTS { get; set; }
    public virtual DbSet<P_ITEMS> P_ITEMS { get; set; }
    public virtual DbSet<P_ITEMS_EVENTS> P_ITEMS_EVENTS { get; set; }
    public virtual DbSet<P_PLACES> P_PLACES { get; set; }
    public virtual DbSet<P_TAXDEPRECYEAR> P_TAXDEPRECYEAR { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<P_CAT1>(entity =>
        {
            entity.Property(e => e.CODE)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.DESCR).HasMaxLength(150);

            entity.HasOne(d => d.CATDRow).WithMany()
                .HasForeignKey(d => d.IDCD)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_P_CAT1_IDCD");

            entity.HasOne(d => d.CATTRow).WithMany()
                .HasForeignKey(d => d.IDCT)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_P_CAT1_IDCT");

            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<P_CATD>(entity =>
        {
            entity.Property(e => e.CODE)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.DESCR).HasMaxLength(150);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<P_CATT>(entity =>
        {
            entity.Property(e => e.CODE)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.DESCR).HasMaxLength(150);
            entity.Property(e => e.VALUE0).HasColumnType("DECIMAL(15,2)");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<P_DEPARTMENTS>(entity =>
        {
            entity.Property(e => e.CODE)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.DESCR).HasMaxLength(150);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<P_EVENTS>(entity =>
        {
            entity.Property(e => e.CODE)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.DESCR).HasMaxLength(150);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
            entity.Ignore(e => e.XEvent);
        });

        modelBuilder.Entity<P_ITEMS>(entity =>
        {
            entity.Property(e => e.DATE1).HasColumnType("DATE");
            entity.Property(e => e.DATE2).HasColumnType("DATE");
            entity.Property(e => e.DEPREC).HasColumnType("DECIMAL(15,2)");
            entity.Property(e => e.NAME).HasMaxLength(150);
            entity.Property(e => e.REG_NR)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.VALUE0).HasColumnType("DECIMAL(15,2)");
            entity.Property(e => e.VALUE_LEFT).HasColumnType("DECIMAL(15,2)");
            entity.Property(e => e.ZU).HasMaxLength(15);

            entity.HasOne(d => d.Cat1Row).WithMany()
                .HasForeignKey(d => d.CAT1)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_P_ITEMS_CAT1");

            entity.HasOne(d => d.CatDRow).WithMany()
                .HasForeignKey(d => d.CATD)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_P_ITEMS_CATD");

            entity.HasOne(d => d.CatTRow).WithMany()
                .HasForeignKey(d => d.CATT)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_P_ITEMS_CATT");

            entity.HasOne(d => d.DepartmentRow).WithMany()
                .HasForeignKey(d => d.DEPARTMENT)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_P_ITEMS_DEPARTMENT");

            entity.HasOne(d => d.PlaceRow).WithMany()
                .HasForeignKey(d => d.PLACE)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_P_ITEMS_PLACE");

            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");

            entity.Ignore(e => e.TDATE1);
            entity.Ignore(e => e.TDATE2);
            entity.Ignore(e => e.TCAT1);
            entity.Ignore(e => e.TCATD);
            entity.Ignore(e => e.TCATT);
            entity.Ignore(e => e.TDEPARTMENT);
            entity.Ignore(e => e.TPLACE);
            entity.Ignore(e => e.TDEPREC0);
            entity.Ignore(e => e.TVALUE0);
            entity.Ignore(e => e.TVALUE_LEFT);
            entity.Ignore(e => e.TERROR);
            entity.Ignore(e => e.TSTATE);
            entity.Ignore(e => e.XState);
            entity.Ignore(e => e.XTState);
        });

        modelBuilder.Entity<P_ITEMS_EVENTS>(entity =>
        {
            entity.Property(e => e.DEPREC_0).HasColumnType("DECIMAL(15,2)");
            entity.Property(e => e.DEPREC_C).HasColumnType("DECIMAL(15,2)");
            entity.Property(e => e.DESCR).HasMaxLength(200);
            entity.Property(e => e.DOCNR).HasMaxLength(50);
            entity.Property(e => e.DT).HasColumnType("DATE");
            entity.Property(e => e.DTREG).HasColumnType("DATE");
            entity.Property(e => e.RATE_D_MT).HasColumnType("DECIMAL(15,2)");
            entity.Property(e => e.SELL_VALUE).HasColumnType("DECIMAL(15,2)");
            entity.Property(e => e.TAX_VAL).HasColumnType("DECIMAL(15,2)");
            entity.Property(e => e.TAX_VAL_C).HasColumnType("DECIMAL(15,2)");
            entity.Property(e => e.TAX_VAL_LEFT).HasColumnType("DECIMAL(15,2)");
            entity.Property(e => e.VALUE_0).HasColumnType("DECIMAL(15,2)");
            entity.Property(e => e.VALUE_C).HasColumnType("DECIMAL(15,2)");
            entity.Property(e => e.VALUE_LEFT).HasColumnType("DECIMAL(15,2)");
            entity.Property(e => e.ZU).HasMaxLength(15);

            entity.HasOne(d => d.CAT1Row).WithMany()
                .HasForeignKey(d => d.CAT1)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_P_ITEMS_EVENTS_CAT1");

            entity.HasOne(d => d.CATDRow).WithMany()
                .HasForeignKey(d => d.CATD)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_P_ITEMS_EVENTS_CATD");

            entity.HasOne(d => d.CATTRow).WithMany()
                .HasForeignKey(d => d.CATT)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_P_ITEMS_EVENTS_CATT");

            entity.HasOne(d => d.DEPARTMENTRow).WithMany()
                .HasForeignKey(d => d.DEPARTMENT)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_P_ITEMS_EVENTS_DEPARTMENT");

            entity.HasOne(d => d.EVENTRow).WithMany()
                .HasForeignKey(d => d.EVENT)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_P_ITEMS_EVENTS_EVENT");

            entity.HasOne(d => d.ITEMRow).WithMany(p => p.Events)
                .HasForeignKey(d => d.IDIT)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_P_ITEMS_EVENTS_IDIT");

            entity.HasOne(d => d.PLACERow).WithMany()
                .HasForeignKey(d => d.PLACE)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_P_ITEMS_EVENTS_PLACE");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");

            entity.Ignore(e => e.XEvent);
            entity.Ignore(e => e.XChColSet);
        });

        modelBuilder.Entity<P_PLACES>(entity =>
        {
            entity.Property(e => e.ADDR).HasMaxLength(500);
            entity.Property(e => e.CODE)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.DESCR).HasMaxLength(150);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<P_TAXDEPRECYEAR>(entity =>
        {
            entity.Property(e => e.DEPREC).HasColumnType("DECIMAL(15,2)");
            entity.Property(e => e.VALUE0).HasColumnType("DECIMAL(15,2)");
            entity.Property(e => e.VALUE1).HasColumnType("DECIMAL(15,2)");
            entity.Property(e => e.VALUEC).HasColumnType("DECIMAL(15,2)");
            entity.Property(e => e.VALUED).HasColumnType("DECIMAL(15,2)");
            entity.Property(e => e.VALUE_ADD).HasColumnType("DECIMAL(15,2)");
            entity.Property(e => e.VALUE_COR).HasColumnType("DECIMAL(15,2)");
            entity.Property(e => e.VALUE_EXCL).HasColumnType("DECIMAL(15,2)");
            entity.Property(e => e.VALUE_NEW).HasColumnType("DECIMAL(15,2)");
            entity.Property(e => e.VALUE_REM).HasColumnType("DECIMAL(15,2)");

            entity.HasOne(d => d.CATTRow).WithMany()
                .HasForeignKey(d => d.CATT)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_P_TAXDEPRECYEAR_CATT");

            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotifications);
    }

}
