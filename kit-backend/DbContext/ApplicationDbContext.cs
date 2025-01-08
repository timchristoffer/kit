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
    public DbSet<UploadedFile> UploadedFile { get; set; } = null!;
    public DbSet<CodeSnippet> CodeSnippet { get; set; } = null!;
    public DbSet<ErrorLog> ErrorLog { get; set; } = null!;
    public DbSet<Project> Project { get; set; } = null!;
    public DbSet<User> User { get; set; } = null!;

    // Request Models
    public DbSet<AnalysisRequest> AnalysisRequest { get; set; } = null!;

    // Response Models
    public DbSet<AnalysisReport> AnalysisReport { get; set; } = null!;
    public DbSet<AnalysisStatus> AnalysisStatus { get; set; } = null!;
    public DbSet<CodeAnalysisResult> CodeAnalysisResult { get; set; } = null!;
    public DbSet<Metric> Metric { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Definiera relationer

        // Projekt -> Användare (One-to-Many relation)
        modelBuilder.Entity<Project>()
            .HasOne(p => p.User)
            .WithMany() // Ingen samling på användaren, så vi använder WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // UploadedFile -> Project (Many-to-One relation)
        modelBuilder.Entity<UploadedFile>()
            .HasOne(uf => uf.Project)
            .WithMany(p => p.UploadedFiles)
            .HasForeignKey(uf => uf.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
