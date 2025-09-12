using System;
using System.Collections.Generic;
using CrimeManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CrimeManagement.Context;

public partial class CrimeDbContext : DbContext
{
    public CrimeDbContext()
    {
    }

    public CrimeDbContext(DbContextOptions<CrimeDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CaseNote> CaseNotes { get; set; }

    public virtual DbSet<ComplaintIoassign> ComplaintIoassigns { get; set; }

    public virtual DbSet<ComplaintRequest> ComplaintRequests { get; set; }

    public virtual DbSet<ComplaintRequestBackup> ComplaintRequestBackups { get; set; }

    public virtual DbSet<CountryMaster> CountryMasters { get; set; }

    public virtual DbSet<CrimeCriminal> CrimeCriminals { get; set; }

    public virtual DbSet<CrimeType> CrimeTypes { get; set; }

    public virtual DbSet<Criminal> Criminals { get; set; }

    public virtual DbSet<DesignationMaster> DesignationMasters { get; set; }

    public virtual DbSet<EvidenceAttachment> EvidenceAttachments { get; set; }

    public virtual DbSet<EvidenceAttachmentBackup> EvidenceAttachmentBackups { get; set; }

    public virtual DbSet<EvidenceType> EvidenceTypes { get; set; }

    public virtual DbSet<Firattachment> Firattachments { get; set; }

    public virtual DbSet<Investigation> Investigations { get; set; }

    public virtual DbSet<InvestigationStageHistory> InvestigationStageHistories { get; set; }

    public virtual DbSet<IojurisdictionAssign> IojurisdictionAssigns { get; set; }

    public virtual DbSet<JurisdictionMaster> JurisdictionMasters { get; set; }

    public virtual DbSet<LocationMaster> LocationMasters { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<StateMaster> StateMasters { get; set; }

    public virtual DbSet<UserLoginLog> UserLoginLogs { get; set; }

    public virtual DbSet<UserMaster> UserMasters { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOP-NONCGQKS;Database=CrimeManagement;User Id=sa;Password=123456;TrustServerCertificate=True;MultipleActiveResultSets=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CaseNote>(entity =>
        {
            entity.HasKey(e => e.CaseNoteId).HasName("PK__CaseNote__DF5F93B1FFD317BB");

            entity.Property(e => e.CaseNote1)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("CaseNote");
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Identifier)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.ModifyOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<ComplaintIoassign>(entity =>
        {
            entity.HasKey(e => e.AssignId).HasName("PK__Complain__9FFF4CAF1AC4F040");

            entity.ToTable("ComplaintIOAssign");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Identifier)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.ModifyOn).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("userId");
        });

        modelBuilder.Entity<ComplaintRequest>(entity =>
        {
            entity.HasKey(e => e.ComplaintRequestId).HasName("PK__Complain__6102B364C404DDF8");

            entity.ToTable("ComplaintRequest");

            entity.Property(e => e.ComplaintName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.CrimeDescription)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.DateReported).HasColumnType("datetime");
            entity.Property(e => e.Identifier)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("identifier");
            entity.Property(e => e.IoofficerId).HasColumnName("IOofficerId");
            entity.Property(e => e.JurisdictionId).HasColumnName("JurisdictionID");
            entity.Property(e => e.ModifyOn).HasColumnType("datetime");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.Priority)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ComplaintRequestBackup>(entity =>
        {
            entity.HasKey(e => e.ComplaintRequestBackUpId).HasName("PK__Complain__24B629EEE85ED4CD");

            entity.ToTable("ComplaintRequestBackup");

            entity.Property(e => e.ComplaintName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.CrimeDescription)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.DateReported).HasColumnType("datetime");
            entity.Property(e => e.Identifier)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("identifier");
            entity.Property(e => e.IoofficerId).HasColumnName("IOofficerId");
            entity.Property(e => e.JurisdictionId).HasColumnName("JurisdictionID");
            entity.Property(e => e.ModifyOn).HasColumnType("datetime");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.Priority)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CountryMaster>(entity =>
        {
            entity.HasKey(e => e.CountryId).HasName("PK__CountryM__10D1609F7F30FA14");

            entity.ToTable("CountryMaster");

            entity.Property(e => e.CountryName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Identifier)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.ModifyOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<CrimeCriminal>(entity =>
        {
            entity.HasKey(e => e.CrimeCriminalId).HasName("PK__CrimeCri__A93631C59DB63C14");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.EvidenceAttachment)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Identifier)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.ModifyOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<CrimeType>(entity =>
        {
            entity.HasKey(e => e.CrimeId).HasName("PK__CrimeTyp__83ED04EDF299E555");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.CrimeName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Identifier)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.ModifyOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<Criminal>(entity =>
        {
            entity.HasKey(e => e.CriminalId).HasName("PK__Criminal__7A1D5369B6A09262");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.CriminalName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Identifier)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.ModifyOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<DesignationMaster>(entity =>
        {
            entity.HasKey(e => e.DesignationId).HasName("PK__Designat__BABD60DEB8BE353A");

            entity.ToTable("DesignationMaster");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DesignationName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Identifier)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.ModifyOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<EvidenceAttachment>(entity =>
        {
            entity.HasKey(e => e.EvidenceAttachmentId).HasName("PK__Evidence__83245C3480316F2C");

            entity.ToTable("EvidenceAttachment");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.EvidenceAttachmentPath)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Identifier)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.ModifyOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<EvidenceAttachmentBackup>(entity =>
        {
            entity.HasKey(e => e.EvidenceAttachmentBackupId).HasName("PK__Evidence__19EE66034E23107E");

            entity.ToTable("EvidenceAttachmentBackup");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.EvidenceAttachmentPath)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Identifier)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.ModifyOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<EvidenceType>(entity =>
        {
            entity.HasKey(e => e.EvidenceTypeId).HasName("PK__Evidence__23E94FDE01AD3CCC");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.EvidenceName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Identifier)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.ModifyOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<Firattachment>(entity =>
        {
            entity.HasKey(e => e.FirattachmentId).HasName("PK__FIRAttac__2CAEF3AAB824B3BF");

            entity.ToTable("FIRAttachment");

            entity.Property(e => e.FirattachmentId).HasColumnName("FIRAttachmentId");
            entity.Property(e => e.AttachmentPath)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifyOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<Investigation>(entity =>
        {
            entity.HasKey(e => e.InvestigationId).HasName("PK__Investig__83714CF46F11BEB2");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.FirattachmentId).HasColumnName("FIRAttachmentId");
            entity.Property(e => e.Identifier)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.InvestigationDescription)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.ModifyOn).HasColumnType("datetime");
            entity.Property(e => e.Priority)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<InvestigationStageHistory>(entity =>
        {
            entity.HasKey(e => e.StageId).HasName("PK__Investig__03EB7AD8D4DD2873");

            entity.ToTable("InvestigationStageHistory");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.Identifier)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.InvestigationStageHistory1)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("InvestigationStageHistory");
            entity.Property(e => e.ModifyOn).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<IojurisdictionAssign>(entity =>
        {
            entity.HasKey(e => e.IojurisdictionId).HasName("PK__IOJurisd__505E7A127BA23C74");

            entity.ToTable("IOJurisdictionAssign");

            entity.Property(e => e.IojurisdictionId).HasColumnName("IOJurisdictionId");
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Identifier)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.ModifyOn).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("userId");
        });

        modelBuilder.Entity<JurisdictionMaster>(entity =>
        {
            entity.HasKey(e => e.JurisdictionId).HasName("PK__Jurisdic__160C652CCBB10A3F");

            entity.ToTable("JurisdictionMaster");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Identifier)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.JurisdictionName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifyOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<LocationMaster>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("PK__Location__E7FEA49789508303");

            entity.ToTable("LocationMaster");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Identifier)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.LocationName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifyOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E126ECA0E35");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Identifier)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.Message)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ModifyOn).HasColumnType("datetime");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("userId");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1AB6D473AD");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Identifier)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.ModifyOn).HasColumnType("datetime");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<StateMaster>(entity =>
        {
            entity.HasKey(e => e.StateId).HasName("PK__StateMas__C3BA3B3ADDAF17CD");

            entity.ToTable("StateMaster");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Identifier)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.ModifyOn).HasColumnType("datetime");
            entity.Property(e => e.StateName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UserLoginLog>(entity =>
        {
            entity.HasKey(e => e.UserLoginId).HasName("PK__UserLogi__107D568C56DA7A6C");

            entity.ToTable("UserLoginLog");

            entity.Property(e => e.LoginDateTime)
                .HasColumnType("datetime")
                .HasColumnName("loginDateTime");
            entity.Property(e => e.UserId).HasColumnName("userId");
        });

        modelBuilder.Entity<UserMaster>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__UserMast__CB9A1CFF6A7FE601");

            entity.ToTable("UserMaster");

            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.Aadhaar)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.Address)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("createdOn");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("emailId");
            entity.Property(e => e.EmergencyContact)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.Firstname)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.HashPassword)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Identifier)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("identifier");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("lastname");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifyOn).HasColumnType("datetime");
            entity.Property(e => e.Pan)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("pan");
            entity.Property(e => e.PhoneNo)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.ProfilePhotoPath)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("profilePhotoPath");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("userName");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
