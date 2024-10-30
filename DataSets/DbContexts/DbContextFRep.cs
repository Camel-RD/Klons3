using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using KlonsLIB.Data;
using Klons3.ModelsA;

namespace Klons3.ModelsFRep;
public partial class DbContextFRep : MyDbContext
{
    public DbContextFRep(DbContextOptions<DbContextFRep> options)
        : base(options)
    {
    }
    public DbContextFRep(string connectionString, string username) : base(connectionString, username)
    {
    }

    public virtual DbSet<F_ROPS1A> F_ROPS1A { get; set; }
    public virtual DbSet<F_ROPS2A> F_ROPS2A { get; set; }
    public virtual DbSet<F_ROPS2B> F_ROPS2B { get; set; }
    public virtual DbSet<F_ROPS3A> F_ROPS3A { get; set; }
    public virtual DbSet<F_TREPA1> F_TREPA1 { get; set; }
    public virtual DbSet<F_TREPAPAN> F_TREPAPAN { get; set; }
    public virtual DbSet<F_TREPDARZ1> F_TREPDARZ1 { get; set; }
    public virtual DbSet<F_TREPDARZ2> F_TREPDARZ2 { get; set; }
    public virtual DbSet<F_TREPDOCS1> F_TREPDOCS1 { get; set; }
    public virtual DbSet<F_TREPDOCS2> F_TREPDOCS2 { get; set; }
    public virtual DbSet<F_TREPDOCSS> F_TREPDOCSS { get; set; }
    public virtual DbSet<F_TREPMT> F_TREPMT { get; set; }
    public virtual DbSet<F_TREPOPS> F_TREPOPS { get; set; }
    public virtual DbSet<F_TREPOPSD> F_TREPOPSD { get; set; }
    public virtual DbSet<F_TREPPVNZ1> F_TREPPVNZ1 { get; set; }
    public virtual DbSet<F_TREPUDOCS> F_TREPUDOCS { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<F_ROPS1A>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__F_F_ROPS1A");

            entity.HasIndex(e => e.CLID, "F_ROPS1A_CLID");

            entity.HasIndex(e => e.CLID2, "F_ROPS1A_CLID1");

            entity.HasIndex(e => e.DID, "F_ROPS1A_DID");

            entity.HasIndex(e => e.SPID, "F_ROPS1A_SPID");

            entity.HasIndex(e => e.ID, "PK__F_F_ROPS1A").IsUnique();

            entity.Property(e => e.AC1).HasMaxLength(8);
            entity.Property(e => e.AC2).HasMaxLength(8);
            entity.Property(e => e.CLID).HasMaxLength(15);
            entity.Property(e => e.CLID2).HasMaxLength(15);
            entity.Property(e => e.CUR).HasMaxLength(3);
            entity.Property(e => e.DESCR).HasMaxLength(80);
            entity.Property(e => e.DESCR2).HasMaxLength(80);
            entity.Property(e => e.DETE).HasColumnType("DATE");
            entity.Property(e => e.DOCNR).HasMaxLength(15);
            entity.Property(e => e.DOCST).HasMaxLength(6);
            entity.Property(e => e.DOCTYP).HasMaxLength(10);
            entity.Property(e => e.DOCTYP1).HasMaxLength(10);
            entity.Property(e => e.DOCTYP2).HasMaxLength(10);
            entity.Property(e => e.NAME).HasMaxLength(100);
            entity.Property(e => e.NAME2).HasMaxLength(100);
            entity.Property(e => e.NRX).HasMaxLength(5);
            entity.Property(e => e.RAC).HasMaxLength(8);
            entity.Property(e => e.RAC2).HasMaxLength(8);
            entity.Property(e => e.RAC3).HasMaxLength(5);
            entity.Property(e => e.RAC4).HasMaxLength(8);
            entity.Property(e => e.RAC5).HasMaxLength(5);
            entity.Property(e => e.REGNR).HasMaxLength(15);
            entity.Property(e => e.REGNR2).HasMaxLength(15);
            entity.Property(e => e.SAC1).HasMaxLength(100);
            entity.Property(e => e.SAC2).HasMaxLength(100);
        });

        modelBuilder.Entity<F_ROPS2A>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__F_ROPS2A");

            entity.HasIndex(e => e.AC, "F_ROPS2A_AC");

            entity.HasIndex(e => e.ID, "F_ROPS2A_ID");

            entity.HasIndex(e => e.SPID, "F_ROPS2A_SPID");

            entity.HasIndex(e => e.ID, "PK__F_ROPS2A").IsUnique();

            entity.Property(e => e.AC).HasMaxLength(8);
            entity.Property(e => e.CLID).HasMaxLength(15);
            entity.Property(e => e.NAME).HasMaxLength(150);
            entity.Property(e => e.NAME1).HasMaxLength(100);
            entity.Property(e => e.RAC).HasMaxLength(8);
            entity.Property(e => e.REGNR).HasMaxLength(15);
            entity.Property(e => e.RNAME).HasMaxLength(150);

            entity.Ignore(e => e.ADB);
            entity.Ignore(e => e.ACR);
            entity.Ignore(e => e.BDB);
            entity.Ignore(e => e.BCR);
        });

        modelBuilder.Entity<F_ROPS2A1>(entity =>
        {
            entity.Ignore(e => e.ADB);
            entity.Ignore(e => e.ACR);
            entity.Ignore(e => e.BDB);
            entity.Ignore(e => e.BCR);

            entity.Ignore(x => x.DSTR);
            entity.Ignore(x => x.CSTR);
        });

        modelBuilder.Entity<F_ROPS2B>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__F_ROPS2B");

            entity.HasIndex(e => e.AC, "F_ROPS2B_AC");

            entity.HasIndex(e => e.ID, "F_ROPS2B_ID");

            entity.HasIndex(e => e.SPID, "F_ROPS2B_SPID");

            entity.HasIndex(e => e.ID, "PK__F_ROPS2B").IsUnique();

            entity.Property(e => e.AC).HasMaxLength(8);
            entity.Property(e => e.NAME).HasMaxLength(100);
        });

        modelBuilder.Entity<F_ROPS3A>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__F_ROPS3A");

            entity.HasIndex(e => e.ID, "F_ROPS3A_ID");

            entity.HasIndex(e => e.ID, "PK__F_ROPS3A").IsUnique();

            entity.Property(e => e.AC1).HasMaxLength(8);
            entity.Property(e => e.CLID).HasMaxLength(15);
            entity.Property(e => e.NAME).HasMaxLength(100);
            entity.Property(e => e.RAC).HasMaxLength(8);
        });

        modelBuilder.Entity<F_TREPA1>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__F_TREPA1");

            entity.HasIndex(e => e.ID, "F_TREPA1_ID");

            entity.HasIndex(e => e.SPID, "F_TREPA1_SPID");

            entity.HasIndex(e => e.ID, "PK__F_TREPA1").IsUnique();

            entity.Property(e => e.DT).HasColumnType("DATE");
            entity.Property(e => e.I1).HasMaxLength(15);
            entity.Property(e => e.NM).HasMaxLength(100);
            entity.Property(e => e.NR).HasMaxLength(15);
            entity.Property(e => e.ST).HasMaxLength(6);
        });

        modelBuilder.Entity<F_TREPAPAN>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__F_TREPAPAN");

            entity.HasIndex(e => e.ID, "F_TREPAPAN_ID");

            entity.HasIndex(e => e.SPID, "F_TREPAPAN_SPID");

            entity.HasIndex(e => e.ID, "PK__F_TREPAPAN").IsUnique();

            entity.Property(e => e.AC1).HasMaxLength(8);
            entity.Property(e => e.AC2).HasMaxLength(8);
            entity.Property(e => e.AC3).HasMaxLength(15);
            entity.Property(e => e.AC4).HasMaxLength(15);
            entity.Property(e => e.AC5).HasMaxLength(15);
            entity.Property(e => e.CL).HasMaxLength(15);
        });

        modelBuilder.Entity<F_TREPDARZ1>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__F_TREPDARZ1");

            entity.HasIndex(e => e.AC1, "F_TREPDARZ1_AC1");

            entity.HasIndex(e => e.AC2, "F_TREPDARZ1_AC2");

            entity.HasIndex(e => e.CLID, "F_TREPDARZ1_CLID");

            entity.HasIndex(e => e.DID, "F_TREPDARZ1_DID");

            entity.HasIndex(e => e.ID, "F_TREPDARZ1_ID");

            entity.HasIndex(e => e.RID, "F_TREPDARZ1_RID");

            entity.HasIndex(e => e.SPID, "F_TREPDARZ1_SPID");

            entity.HasIndex(e => e.ID, "PK__F_TREPDARZ1").IsUnique();

            entity.Property(e => e.AC1).HasMaxLength(8);
            entity.Property(e => e.AC2).HasMaxLength(8);
            entity.Property(e => e.CLID).HasMaxLength(15);
            entity.Property(e => e.DESCR).HasMaxLength(80);
            entity.Property(e => e.DETE).HasColumnType("DATE");
            entity.Property(e => e.DOCNR).HasMaxLength(15);
            entity.Property(e => e.DOCST).HasMaxLength(6);
            entity.Ignore(e => e.DOCSTR);
            entity.Property(e => e.DOCTYP).HasMaxLength(10);
            entity.Property(e => e.NAME).HasMaxLength(100);
            entity.Property(e => e.NRX).HasMaxLength(5);
            entity.Property(e => e.REGNR).HasMaxLength(15);
        });

        modelBuilder.Entity<F_TREPDARZ2>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__F_TREPDARZ2");

            entity.HasIndex(e => e.ID, "F_TREPDARZ2_ID");

            entity.HasIndex(e => e.SPID, "F_TREPDARZ2_SPID");

            entity.HasIndex(e => e.ID, "PK__F_TREPDARZ2").IsUnique();
        });

        modelBuilder.Entity<F_TREPDOCS1>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__F_TREPDOCS1");

            entity.HasIndex(e => e.ID, "F_TREPDOCS1_ID");

            entity.HasIndex(e => e.SPID, "F_TREPDOCS1_SPID");

            entity.HasIndex(e => e.ID, "PK__F_TREPDOCS1").IsUnique();

            entity.Property(e => e.NR).HasMaxLength(9);
            entity.Property(e => e.SR).HasMaxLength(6);
        });

        modelBuilder.Entity<F_TREPDOCS2>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__F_TREPDOCS2");

            entity.HasIndex(e => e.ID, "F_TREPDOCS2_ID");

            entity.HasIndex(e => e.SPID, "F_TREPDOCS2_SPID");

            entity.HasIndex(e => e.ID, "PK__F_TREPDOCS2").IsUnique();

            entity.Property(e => e.NR1).HasMaxLength(9);
            entity.Property(e => e.NR2).HasMaxLength(9);
            entity.Property(e => e.SR).HasMaxLength(6);
            entity.Property(e => e.VX).HasColumnType("CHAR(1)");
        });

        modelBuilder.Entity<F_TREPDOCSS>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__F_TREPDOCSS");

            entity.HasIndex(e => e.DID, "F_TREPDOCSS_DID");

            entity.HasIndex(e => e.ID, "F_TREPDOCSS_ID");

            entity.HasIndex(e => e.SPID, "F_TREPDOCSS_SPID");

            entity.HasIndex(e => e.ID, "PK__F_TREPDOCSS").IsUnique();
        });

        modelBuilder.Entity<F_TREPMT>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__F_TREPMT");

            entity.HasIndex(e => e.ID, "F_TREPMT_ID");

            entity.HasIndex(e => e.SPID, "F_TREPMT_SPID");

            entity.HasIndex(e => e.ID, "PK__F_TREPMT").IsUnique();

            entity.Property(e => e.AC1).HasMaxLength(15);
            entity.Property(e => e.AC2).HasMaxLength(15);
            entity.Property(e => e.ACN1).HasMaxLength(100);
            entity.Property(e => e.ACN2).HasMaxLength(100);
            entity.Property(e => e.AD).HasMaxLength(3);
        });

        modelBuilder.Entity<F_TREPOPS>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("F_TREPOPS");

            entity.HasIndex(e => e.ID, "F_TREPOPS").IsUnique();

            entity.Property(e => e.AC11)
                .IsRequired()
                .HasMaxLength(8);
            entity.Property(e => e.AC12).HasMaxLength(8);
            entity.Property(e => e.AC13).HasMaxLength(5);
            entity.Property(e => e.AC14).HasMaxLength(8);
            entity.Property(e => e.AC15).HasMaxLength(5);
            entity.Property(e => e.AC21)
                .IsRequired()
                .HasMaxLength(8);
            entity.Property(e => e.AC22).HasMaxLength(8);
            entity.Property(e => e.AC23).HasMaxLength(5);
            entity.Property(e => e.AC24).HasMaxLength(8);
            entity.Property(e => e.AC25).HasMaxLength(5);
            entity.Property(e => e.CUR).HasMaxLength(3);
            entity.Property(e => e.DESCR).HasMaxLength(50);
        });

        modelBuilder.Entity<F_TREPOPSD>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_TREPOPSD");

            entity.HasIndex(e => e.ID, "PK_TREPOPSD").IsUnique();

            entity.Property(e => e.CLID).HasMaxLength(15);
            entity.Property(e => e.CLID2).HasMaxLength(15);
            entity.Property(e => e.DESCR).HasMaxLength(80);
            entity.Property(e => e.DETE).HasColumnType("DATE");
            entity.Property(e => e.DOCNR).HasMaxLength(15);
            entity.Property(e => e.DOCST).HasMaxLength(6);
            entity.Property(e => e.DOCTYP).HasMaxLength(10);
            entity.Property(e => e.DT2).HasColumnType("DATE");
            entity.Property(e => e.NRX).HasMaxLength(5);
            entity.Property(e => e.ZU).HasMaxLength(20);
        });

        modelBuilder.Entity<F_TREPPVNZ1>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__F_TREPPVNZ1");

            entity.HasIndex(e => e.CLID, "F_TREPPVNZ1_CLID");

            entity.HasIndex(e => e.DID, "F_TREPPVNZ1_DID");

            entity.HasIndex(e => e.ID, "F_TREPPVNZ1_ID");

            entity.HasIndex(e => e.SPID, "F_TREPPVNZ1_SPID");

            entity.HasIndex(e => e.ID, "PK__F_TREPPVNZ1").IsUnique();

            entity.Property(e => e.A11).HasMaxLength(8);
            entity.Property(e => e.CLID).HasMaxLength(15);
            entity.Property(e => e.DESCR).HasMaxLength(80);
            entity.Property(e => e.DETE).HasColumnType("DATE");
            entity.Property(e => e.DOCNR).HasMaxLength(15);
            entity.Property(e => e.DOCST).HasMaxLength(6);
            entity.Property(e => e.DOCSTR).HasMaxLength(200);
            entity.Property(e => e.DOCTYP).HasMaxLength(10);
            entity.Property(e => e.NAME).HasMaxLength(100);
            entity.Property(e => e.NRX).HasMaxLength(5);
            entity.Property(e => e.REGNR).HasMaxLength(15);
        });

        modelBuilder.Entity<F_TREPUDOCS>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__F_TREPUDOCS");

            entity.HasIndex(e => e.ID, "F_TREPUDOCS_ID");

            entity.HasIndex(e => e.SPID, "F_TREPUDOCS_SPID");

            entity.HasIndex(e => e.ID, "PK__F_TREPUDOCS").IsUnique();

            entity.Property(e => e.DT1).HasMaxLength(27);
            entity.Property(e => e.N1).HasMaxLength(9);
            entity.Property(e => e.N2).HasMaxLength(9);
            entity.Property(e => e.SR).HasMaxLength(6);
            entity.Property(e => e.T1).HasColumnType("CHAR(1)");
            entity.Property(e => e.T2).HasColumnType("CHAR(1)");
            entity.Property(e => e.T3).HasColumnType("CHAR(2)");
            entity.Property(e => e.TT).HasMaxLength(100);
        });

        modelBuilder.Entity<F_SP_DIFF_12>(entity =>
        {
            entity.HasNoKey();
        });

        modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotifications);
    }

}

