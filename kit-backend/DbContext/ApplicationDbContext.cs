using Microsoft.EntityFrameworkCore;
using KitBackend.Models;
using KitBackend.Models.Data;
using KitBackend.Models.Requests;
using KitBackend.Models.Responses;
using System.Collections.Generic;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // Data Models
    public virtual DbSet<UploadedFile> UploadedFile { get; set; } = null!;
    public virtual DbSet<CodeSnippet> CodeSnippet { get; set; } = null!;
    public virtual DbSet<ErrorLog> ErrorLog { get; set; } = null!;
    public virtual DbSet<Project> Project { get; set; } = null!;
    public virtual DbSet<User> User { get; set; } = null!;

    // Request Models
    public virtual DbSet<AnalysisRequest> AnalysisRequest { get; set; } = null!;

    // Response Models
    public virtual DbSet<AnalysisReport> AnalysisReport { get; set; } = null!;
    public virtual DbSet<AnalysisStatus> AnalysisStatus { get; set; } = null!;
    public virtual DbSet<CodeAnalysisResult> CodeAnalysisResult { get; set; } = null!;
    public virtual DbSet<Metric> Metric { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UploadedFile>()
            .HasOne(uf => uf.Project)
            .WithMany(p => p.UploadedFiles)
            .HasForeignKey(uf => uf.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
