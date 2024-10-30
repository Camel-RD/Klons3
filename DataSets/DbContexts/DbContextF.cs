using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using KlonsLIB.Data;
using Klons3.ModelsA;

namespace Klons3.ModelsF;

public partial class DbContextF : MyDbContext
{
    public DbContextF(DbContextOptions<DbContextF> options)
        : base(options)
    {
    }

    public DbContextF(string connectionString, string username) : base(connectionString, username)
    {
    }

    public virtual DbSet<F_ACP1> F_ACP1 { get; set; }
    public virtual DbSet<F_ACP21> F_ACP21 { get; set; }
    public virtual DbSet<F_ACP211> F_ACP211 { get; set; }
    public virtual DbSet<F_ACP22> F_ACP22 { get; set; }
    public virtual DbSet<F_ACP23> F_ACP23 { get; set; }
    public virtual DbSet<F_ACP24> F_ACP24 { get; set; }
    public virtual DbSet<F_ACP25> F_ACP25 { get; set; }
    public virtual DbSet<F_ACP25A> F_ACP25A { get; set; }
    public virtual DbSet<F_ACP6> F_ACP6 { get; set; }
    public virtual DbSet<F_ACPLAN> F_ACPLAN { get; set; }
    public virtual DbSet<F_ACPVN> F_ACPVN { get; set; }
    public virtual DbSet<F_BAL0> F_BAL0 { get; set; }
    public virtual DbSet<F_BALA1> F_BALA1 { get; set; }
    public virtual DbSet<F_BALA2> F_BALA2 { get; set; }
    public virtual DbSet<F_BALA21> F_BALA21 { get; set; }
    public virtual DbSet<F_BALA3> F_BALA3 { get; set; }
    public virtual DbSet<F_BANKS> F_BANKS { get; set; }
    public virtual DbSet<F_CURRENCY> F_CURRENCY { get; set; }
    public virtual DbSet<F_DOCS0> F_DOCS0 { get; set; }
    public virtual DbSet<F_DOCTYP> F_DOCTYP { get; set; }
    public virtual DbSet<F_DOCTYPA> F_DOCTYPA { get; set; }
    public virtual DbSet<F_DOCTYPB> F_DOCTYPB { get; set; }
    public virtual DbSet<F_LOGX> F_LOGX { get; set; }
    public virtual DbSet<F_LOPS> F_LOPS { get; set; }
    public virtual DbSet<F_LOPSD> F_LOPSD { get; set; }
    public virtual DbSet<F_LXOP> F_LXOP { get; set; }
    public virtual DbSet<F_OPS> F_OPS { get; set; }
    public virtual DbSet<F_OPSD> F_OPSD { get; set; }
    public virtual DbSet<F_PERSONS> F_PERSONS { get; set; }
    public virtual DbSet<F_PERSONTYP> F_PERSONTYP { get; set; }
    public virtual DbSet<F_TUDOCLISTS> F_TUDOCLISTS { get; set; }
    public virtual DbSet<F_TUDOCS> F_TUDOCS { get; set; }
    public virtual DbSet<F_TUPDFIELDS> F_TUPDFIELDS { get; set; }
    public virtual DbSet<F_TUPVN1> F_TUPVN1 { get; set; }
    public virtual DbSet<F_VW_OPS> F_VW_OPS { get; set; }
    public virtual DbSet<PARAMS> PARAMS { get; set; }
    public virtual DbSet<USERS> USERS { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<F_ACP1>(entity =>
        {
            entity.HasKey(e => e.ACPID).HasName("PK__F_ACP1");

            entity.Property(e => e.ACPID).HasColumnType("CHAR(1)");
            entity.Property(e => e.NAME).HasMaxLength(50);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<F_ACP21>(entity =>
        {
            entity.HasKey(e => e.AC).HasName("PK__F_ACP21");

            entity.Property(e => e.AC).HasMaxLength(8);
            entity.Property(e => e.ID1).HasMaxLength(15);
            entity.Property(e => e.ID2).HasMaxLength(15);
            entity.Property(e => e.NAME).HasMaxLength(100);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<F_ACP211>(entity =>
        {
            entity.HasKey(e => e.IDX).HasName("PK__F_ACP211");
            entity.Property(e => e.IDX).HasMaxLength(15);
            entity.Property(e => e.ID1).HasColumnType("CHAR(2)");
            entity.Property(e => e.NAME).HasMaxLength(50);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<F_ACP22>(entity =>
        {
            entity.HasKey(e => e.IDX).HasName("PK__F_ACP22");

            entity.Property(e => e.IDX).HasMaxLength(15);
            entity.Property(e => e.NAME).HasMaxLength(50);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<F_ACP23>(entity =>
        {
            entity.HasKey(e => e.IDX).HasName("PK__F_ACP23");
            entity.Property(e => e.IDX).HasMaxLength(5);
            entity.Property(e => e.ID1).HasColumnType("CHAR(2)");
            entity.Property(e => e.NAME).HasMaxLength(50);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<F_ACP24>(entity =>
        {
            entity.HasKey(e => e.IDX).HasName("PK__F_ACP24");

            entity.Property(e => e.IDX).HasMaxLength(8);
            entity.Property(e => e.NAME).HasMaxLength(50);
            entity.Property(e => e.UNIT).HasMaxLength(10);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<F_ACP25>(entity =>
        {
            entity.HasKey(e => e.IDX).HasName("PK__F_ACP25");
            entity.Property(e => e.IDX).HasMaxLength(5);
            entity.Property(e => e.ID1).HasColumnType("CHAR(2)");
            entity.Property(e => e.NAME).HasMaxLength(150);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<F_ACP25A>(entity =>
        {
            entity.HasKey(e => e.IDX).HasName("PK__F_ACP25A");

            entity.Property(e => e.IDX).HasMaxLength(5);
            entity.Property(e => e.NAME).HasMaxLength(50);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<F_ACP6>(entity =>
        {
            entity.HasKey(e => e.IDX).HasName("PK__F_ACP6");

            entity.Property(e => e.IDX).HasMaxLength(15);
            entity.Property(e => e.NAME).HasMaxLength(50);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<F_ACPLAN>(entity =>
        {
            entity.HasKey(e => e.AC).HasName("PK__F_ACPLAN");
            entity.Property(e => e.AC).HasMaxLength(8);
            entity.Property(e => e.AP1).HasMaxLength(15);
            entity.Property(e => e.AP2).HasMaxLength(15);
            entity.Property(e => e.AP3).HasMaxLength(15);
            entity.Property(e => e.AP4).HasMaxLength(15);
            entity.Property(e => e.AP5).HasMaxLength(15);
            entity.Property(e => e.NAME).HasMaxLength(90);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<F_ACPVN>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__F_ACPVN");

            entity.Property(e => e.ID).HasMaxLength(8);
            entity.Property(e => e.NM).HasMaxLength(250);
            entity.Property(e => e.PZ1).HasMaxLength(5);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<F_BAL0>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__F_BAL0");
            entity.Property(e => e.AC11)
                .IsRequired()
                .HasMaxLength(8);
            entity.Property(e => e.AC24).HasMaxLength(8);
            entity.Property(e => e.CLID).HasMaxLength(15);
            entity.Property(e => e.CUR)
                .IsRequired()
                .HasMaxLength(3);

            entity.HasOne(d => d.AC11Row).WithMany()
                .HasForeignKey(d => d.AC11)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .IsRequired()
                .HasConstraintName("FK_F_BAL0_AC1_ACP21_AC");

            entity.HasOne(d => d.AC24Row).WithMany()
                .HasForeignKey(d => d.AC24)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_F_BAL0_AC4_ACP24_IDX");

            entity.HasOne(d => d.PersonRow).WithMany()
                .HasForeignKey(d => d.CLID)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_F_BAL0_CLID_PERSONS_CLID");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<F_BALA1>(entity =>
        {
            entity.HasKey(e => e.BALID).HasName("PK__F_BALA1");

            entity.Property(e => e.BALID).HasMaxLength(10);
            entity.Property(e => e.DESCR).HasMaxLength(50);
            entity.Property(e => e.TA).HasMaxLength(50);
            entity.Property(e => e.TP).HasMaxLength(50);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<F_BALA2>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__F_BALA2");

            entity.Property(e => e.BALID).HasMaxLength(10);
            entity.Property(e => e.DC).HasMaxLength(2);
            entity.Property(e => e.DESCR).HasMaxLength(150);
            entity.Property(e => e.NR).HasMaxLength(5);
            entity.Property(e => e.NR1).HasMaxLength(5);
            entity.Property(e => e.NR2).HasMaxLength(5);
            entity.Property(e => e.TP).HasMaxLength(2);

            entity.HasOne(d => d.BALA1Row).WithMany(p => p.BALA2Rows)
                .HasForeignKey(d => d.BALID)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired()
                .HasConstraintName("FK_F_BALA2_BALID_BALA1_BALID");

            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<F_BALA21>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__F_BALA21");

            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<F_BALA22>(entity =>
        {
        });

        modelBuilder.Entity<F_BALA3>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__F_BALA3");

            entity.Property(e => e.AC).HasMaxLength(8);
            entity.Property(e => e.TP).HasMaxLength(2);

            entity.HasOne(d => d.BALA2Row).WithMany(p => p.BALA3Rows)
                .HasForeignKey(d => d.ID2)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_F_BALA3_ID2_BALA2_ID");

            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<F_BANKS>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__F_BANKS");
            entity.Property(e => e.ID).HasMaxLength(8);
            entity.Property(e => e.NAME)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<F_CURRENCY>(entity =>
        {
            entity.HasKey(e => new { e.ID, e.DETE }).HasName("PK__F_CURRENCY");
            entity.Property(e => e.ID).HasMaxLength(3);
            entity.Property(e => e.DETE).HasColumnType("DATE");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<F_DOCS0>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_DOCS0");
            entity.Property(e => e.AC)
                .IsRequired()
                .HasMaxLength(8);
            entity.Property(e => e.CLID).HasMaxLength(15);
            entity.Property(e => e.DESCR).HasMaxLength(80);
            entity.Property(e => e.DETE).HasColumnType("DATE");
            entity.Property(e => e.DOCNR).HasMaxLength(15);
            entity.Property(e => e.DOCST).HasMaxLength(6);
            entity.Property(e => e.DOCTYP).HasMaxLength(10);

            entity.HasOne(d => d.ACRows).WithMany()
                .HasForeignKey(d => d.AC)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .IsRequired()
                .HasConstraintName("FK_F_DOCS0_AC_ACP21_AC");

            entity.HasOne(d => d.PersonRow).WithMany()
                .HasForeignKey(d => d.CLID)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_F_DOCS0_CLID_PERSONS_CLID");

            entity.HasOne(d => d.DOCTYPRow).WithMany()
                .HasForeignKey(d => d.DOCTYP)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_F_DOCS0_DOCTYP_DOCTYP_ID");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<F_DOCTYP>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__F_DOCTYP");
            entity.Property(e => e.ID).HasMaxLength(10);
            entity.Property(e => e.ID1).HasMaxLength(10);
            entity.Property(e => e.NAME).HasMaxLength(50);
            entity.Property(e => e.NAME1).HasMaxLength(50);
            entity.Property(e => e.PVNPAZ).HasMaxLength(10);
            entity.Property(e => e.TPZ).HasMaxLength(10);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<F_DOCTYPA>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__F_DOCTYPA");

            entity.Property(e => e.ID).HasMaxLength(10);
            entity.Property(e => e.NAME).HasMaxLength(50);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<F_DOCTYPB>(entity =>
        {
            entity.HasKey(e => new { e.IDA, e.ID }).HasName("PK__F_DOCTYPB");
            entity.Property(e => e.IDA).HasMaxLength(10);
            entity.Property(e => e.ID).HasMaxLength(10);

            entity.HasOne(d => d.DOCTYPRows).WithMany()
                .HasForeignKey(d => d.ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_F_DOCTYPB_ID_DOCTYP_ID");

            entity.HasOne(d => d.DOCTYPARow).WithMany(d => d.DOCTYPBRows)
                .HasForeignKey(d => d.IDA)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_F_DOCTYPB_IDA_DOCTYPA_ID");

            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<F_LOGX>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__F_LOGX");
            entity.Property(e => e.LID).HasMaxLength(5);
            entity.Property(e => e.USR).HasMaxLength(15);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<F_LOPS>(entity =>
        {
            entity.HasKey(e => e.IDL).HasName("PK__F_LOPS");

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
            entity.Property(e => e.CUR)
                .IsRequired()
                .HasMaxLength(3);
            entity.Property(e => e.USL).HasMaxLength(15);
        });

        modelBuilder.Entity<F_LOPSD>(entity =>
        {
            entity.HasKey(e => e.IDL).HasName("PK__F_LOPSD");

            entity.Property(e => e.CLID).HasMaxLength(15);
            entity.Property(e => e.CLID2).HasMaxLength(15);
            entity.Property(e => e.DESCR).HasMaxLength(80);
            entity.Property(e => e.DETE).HasColumnType("DATE");
            entity.Property(e => e.DOCNR).HasMaxLength(15);
            entity.Property(e => e.DOCST).HasMaxLength(6);
            entity.Property(e => e.DOCTYP).HasMaxLength(10);
            entity.Property(e => e.NRX).HasMaxLength(5);
            entity.Property(e => e.USL).HasMaxLength(15);
            entity.Property(e => e.ZU).HasMaxLength(20);
        });

        modelBuilder.Entity<F_LXOP>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__F_LXOP");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<F_OPS>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__F_OPS");
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
            entity.Property(e => e.CUR)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("EUR");
            entity.Property(e => e.DESCR).HasMaxLength(50);

            entity.HasOne(d => d.AC11Row).WithMany()
                .HasForeignKey(d => d.AC11)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_F_OPS_AC11_ACP21_AC");

            entity.HasOne(d => d.AC12Row).WithMany()
                .HasForeignKey(d => d.AC12)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_F_OPS_AC12_ACP21_AC");

            entity.HasOne(d => d.AC13Row).WithMany()
                .HasForeignKey(d => d.AC13)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_F_OPS_AC13_ACP23_IDX");

            entity.HasOne(d => d.AC14Row).WithMany()
                .HasForeignKey(d => d.AC14)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_F_OPS_AC14_ACP24_IDX");

            entity.HasOne(d => d.AC15Row).WithMany()
                .HasForeignKey(d => d.AC15)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_F_OPS_AC15_ACP25_IDX");

            entity.HasOne(d => d.AC21Row).WithMany()
                .HasForeignKey(d => d.AC21)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_F_OPS_AC21_ACP21_AC");

            entity.HasOne(d => d.AC22Row).WithMany()
                .HasForeignKey(d => d.AC22)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_F_OPS_AC22_ACP21_AC");

            entity.HasOne(d => d.AC23Row).WithMany()
                .HasForeignKey(d => d.AC23)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_F_OPS_AC23_ACP23_IDX");

            entity.HasOne(d => d.AC24Row).WithMany()
                .HasForeignKey(d => d.AC24)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_F_OPS_AC24_ACP24_IDX");

            entity.HasOne(d => d.AC25Row).WithMany()
                .HasForeignKey(d => d.AC25)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("FK_F_OPS_AC25_ACP25_IDX");

            entity.HasOne(d => d.DOCRow).WithMany(p => p.OPSRows)
                .HasForeignKey(d => d.DOCID)
                .OnDelete(DeleteBehavior.ClientCascade)
                .HasConstraintName("FK_F_OPS_DOCID_OPSD_ID");

            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        /*modelBuilder.Entity<F_OPS>()
            .UpdateUsingStoredProcedure("SP_F_OPS_UPDATE_2",
                storedProcedureBuilder =>
                {
                    storedProcedureBuilder.HasOriginalValueParameter(a => a.ID);
                    storedProcedureBuilder.HasOriginalValueParameter(a => a.TS);
                    storedProcedureBuilder.HasParameter(a => a.DOCID);
                    storedProcedureBuilder.HasParameter(a => a.DESCR);
                    storedProcedureBuilder.HasParameter(a => a.AC11);
                    storedProcedureBuilder.HasParameter(a => a.AC12);
                    storedProcedureBuilder.HasParameter(a => a.AC13);
                    storedProcedureBuilder.HasParameter(a => a.AC14);
                    storedProcedureBuilder.HasParameter(a => a.AC15);
                    storedProcedureBuilder.HasParameter(a => a.AC21);
                    storedProcedureBuilder.HasParameter(a => a.AC22);
                    storedProcedureBuilder.HasParameter(a => a.AC23);
                    storedProcedureBuilder.HasParameter(a => a.AC24);
                    storedProcedureBuilder.HasParameter(a => a.AC25);
                    storedProcedureBuilder.HasParameter(a => a.SUMMC);
                    storedProcedureBuilder.HasParameter(a => a.CUR);
                    storedProcedureBuilder.HasParameter(a => a.SUMM);
                    storedProcedureBuilder.HasParameter(a => a.QV);
                    storedProcedureBuilder.HasParameter(a => a.ZDT);
                    storedProcedureBuilder.HasParameter(a => a.NL);
                    storedProcedureBuilder.HasResultColumn(a => a.TS);
                })
            .DeleteUsingStoredProcedure("SP_F_OPS_DELETE_2",
                storedProcedureBuilder =>
                {
                    storedProcedureBuilder.HasOriginalValueParameter(a => a.ID);
                    storedProcedureBuilder.HasOriginalValueParameter(a => a.TS);
                    storedProcedureBuilder.HasRowsAffectedResultColumn();
                });*/

        modelBuilder.Entity<F_OPSD>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__F_OPSD");

            entity.Property(e => e.ZNR).ValueGeneratedOnAddOrUpdate();
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

            entity.HasOne(d => d.PersonRow).WithMany()
                .HasForeignKey(d => d.CLID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_F_OPSD_CLID_PERSONS_CLID");

            entity.HasOne(d => d.Person2Row).WithMany()
                .HasForeignKey(d => d.CLID2)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_F_OPSD_CLID2_PERSONS_CLID");

            entity.HasOne(d => d.DocTypeRow).WithMany()
                .HasForeignKey(d => d.DOCTYP)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_F_OPSD_DOCTYP_DOCTYP_ID");
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        /*modelBuilder.Entity<F_OPSD>()
            .UpdateUsingStoredProcedure("SP_F_OPSD_UPDATE_2",
                storedProcedureBuilder =>
                {
                    storedProcedureBuilder.HasOriginalValueParameter(a => a.ID);
                    storedProcedureBuilder.HasOriginalValueParameter(a => a.TS);
                    storedProcedureBuilder.HasParameter(a => a.DETE);
                    storedProcedureBuilder.HasParameter(a => a.NRX);
                    storedProcedureBuilder.HasParameter(a => a.DOCTYP);
                    storedProcedureBuilder.HasParameter(a => a.DOCST);
                    storedProcedureBuilder.HasParameter(a => a.DOCNR);
                    storedProcedureBuilder.HasParameter(a => a.CLID);
                    storedProcedureBuilder.HasParameter(a => a.CLID2);
                    storedProcedureBuilder.HasParameter(a => a.DESCR);
                    storedProcedureBuilder.HasParameter(a => a.SUMM);
                    storedProcedureBuilder.HasParameter(a => a.PVN);
                    storedProcedureBuilder.HasParameter(a => a.OPT);
                    storedProcedureBuilder.HasParameter(a => a.ZU);
                    storedProcedureBuilder.HasParameter(a => a.ZDT);
                    storedProcedureBuilder.HasParameter(a => a.DT2);
                    storedProcedureBuilder.HasParameter(a => a.IDDOCM);
                    storedProcedureBuilder.HasParameter(a => a.NL);
                    storedProcedureBuilder.HasResultColumn(a => a.ZNR);
                    storedProcedureBuilder.HasResultColumn(a => a.TS);
                })
            .DeleteUsingStoredProcedure("SP_F_OPSD_DELETE_2",
                storedProcedureBuilder =>
                {
                    storedProcedureBuilder.HasOriginalValueParameter(a => a.ID);
                    storedProcedureBuilder.HasOriginalValueParameter(a => a.TS);
                    storedProcedureBuilder.HasRowsAffectedResultColumn();
                });*/

        modelBuilder.Entity<F_PERSONS>(entity =>
        {
            entity.HasKey(e => e.CLID).HasName("PK__F_PERSONS");

            entity.Property(e => e.CLID).HasMaxLength(15);
            entity.Property(e => e.ADDR).HasMaxLength(100);
            entity.Property(e => e.ADDR2).HasMaxLength(100);
            entity.Property(e => e.ATK).HasMaxLength(6);
            entity.Property(e => e.BANK).HasMaxLength(50);
            entity.Property(e => e.BANKACC).HasMaxLength(25);
            entity.Property(e => e.BANKID).HasMaxLength(8);
            entity.Property(e => e.NAME).HasMaxLength(100);
            entity.Property(e => e.PHONE).HasMaxLength(10);
            entity.Property(e => e.PVNREGNR).HasMaxLength(15);
            entity.Property(e => e.REGNR).HasMaxLength(15);
            entity.Property(e => e.TP).HasMaxLength(15);
            entity.Property(e => e.TP2).HasMaxLength(5);
            entity.Property(e => e.TP3).HasMaxLength(5);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<F_PERSONTYP>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__F_PERSONTY");
            entity.Property(e => e.IDX)
                .IsRequired()
                .HasMaxLength(15);
            entity.Property(e => e.NAME).HasMaxLength(50);
            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });
        modelBuilder.Entity<F_TUDOCLISTS>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__F_TUDOCLIS");

            entity.Property(e => e.DT1).HasMaxLength(27);
            entity.Property(e => e.DT2).HasMaxLength(27);
            entity.Property(e => e.T1).HasColumnType("CHAR(1)");
        });

        modelBuilder.Entity<F_TUDOCS>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__F_TUDOCS");
            entity.Property(e => e.DT1).HasMaxLength(27);
            entity.Property(e => e.N1).HasMaxLength(9);
            entity.Property(e => e.N2).HasMaxLength(9);
            entity.Property(e => e.SR).HasMaxLength(6);
            entity.Property(e => e.T1).HasColumnType("CHAR(1)");
            entity.Property(e => e.T2).HasColumnType("CHAR(1)");
            entity.Property(e => e.T3).HasColumnType("CHAR(1)");
        });

        modelBuilder.Entity<F_TUPVN1>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__F_TUPVN1");

            entity.Property(e => e.TS).HasColumnName("TS").IsRowVersion().HasDefaultValueSql("localtimestamp");
        });

        modelBuilder.Entity<F_VW_OPS>(entity =>
        {
            entity.HasKey(e => e.ID);

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
            entity.Property(e => e.CLID).HasMaxLength(15);
            entity.Property(e => e.CLID2).HasMaxLength(15);
            entity.Property(e => e.CUR)
                .IsRequired()
                .HasMaxLength(3);
            entity.Property(e => e.DESCR).HasMaxLength(80);
            entity.Property(e => e.DESCR2).HasMaxLength(80);
            entity.Property(e => e.DETE).HasColumnType("DATE");
            entity.Property(e => e.DOCNR).HasMaxLength(15);
            entity.Property(e => e.DOCST).HasMaxLength(6);
            entity.Property(e => e.DOCTYP).HasMaxLength(10);
            entity.Property(e => e.DOCTYP2).HasMaxLength(10);
            entity.Property(e => e.NAME).HasMaxLength(100);
            entity.Property(e => e.NAME2).HasMaxLength(100);
            entity.Property(e => e.NRX).HasMaxLength(5);
            entity.Property(e => e.REGNR).HasMaxLength(15);
            entity.Property(e => e.REGNR2).HasMaxLength(15);
        });

        modelBuilder.Entity<PARAMS>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__PARAMS");
            entity.Property(e => e.PNAME).HasMaxLength(20);
            entity.Property(e => e.PVALUE).HasMaxLength(50);
            entity.Property(e => e.USR).HasMaxLength(15);

            entity.HasOne(d => d.USRRow).WithMany()
                .HasForeignKey(d => d.USR)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PARAMS_USR_USERS_NM");
        });

        modelBuilder.Entity<USERS>(entity =>
        {
            entity.HasKey(e => e.NM).HasName("PK__USERS");
            entity.Property(e => e.NM).HasMaxLength(15);
            entity.Property(e => e.PSW).HasMaxLength(15);
            entity.Property(e => e.TP).HasColumnType("CHAR(1)");
        });

        modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotifications);
    }

}
