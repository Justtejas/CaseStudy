using CaseStudyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CaseStudyAPI.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext() { }
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }
        public virtual DbSet<Application> Applications { get; set; } = null!;
        public virtual DbSet<Employer> Employers { get; set; } = null!;
        public virtual DbSet<JobListing> JobListings { get; set; } = null!;
        public virtual DbSet<JobSeeker> JobSeekers { get; set; } = null!;
        public virtual DbSet<Resume> Resumes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Application>(entity =>
            {
                entity.ToTable("Application");

                entity.HasKey(a => a.ApplicationId);

                entity.Property(a => a.ApplicationId).HasColumnName("ApplicationId").IsRequired();
                entity.Property(a => a.JobListingId).HasColumnName("JobListingId").IsRequired();
                entity.Property(a => a.JobSeekerId).HasColumnName("JobSeekerId").IsRequired();
                entity.Property(a => a.ApplicationDate).HasColumnName("ApplicationDate").HasColumnType("datetime").IsRequired();
                entity.Property(a => a.ApplicationStatus).HasColumnName("ApplicationStatus").HasColumnType("nvarchar(100)").IsRequired();
                entity.HasCheckConstraint("CHK_Status", "ApplicationStatus IN ('Confirmed', 'Pending','Cancelled')");


                entity.HasOne(a => a.JobListing)
                    .WithMany(l => l.Applications)
                    .HasForeignKey(a => a.JobListingId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Application_JobListing");

                entity.HasOne(a => a.JobSeeker)
                    .WithMany(j => j.Applications)
                    .HasForeignKey(a => a.JobSeekerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Application_JobSeeker");
            });

            modelBuilder.Entity<Employer>(entity =>
            {
                entity.ToTable("Employer");
                entity.HasKey(e => e.EmployerId);
                entity.Property(e => e.EmployerId).HasColumnName("EmployerId").IsRequired();
                entity.Property(e => e.UserName).HasColumnName("Username").HasColumnType("nvarchar(100)").IsRequired();
                entity.HasIndex(e => e.UserName).IsUnique();
                entity.Property(e => e.Password).HasColumnName("Password").HasColumnType("nvarchar(100)").IsRequired();
                entity.Property(e => e.Gender).HasColumnName("Gender").HasColumnType("nvarchar(100)").IsRequired();
                entity.HasCheckConstraint("CHK_GENDERS", "Gender IN ('Male', 'Female')");
                entity.Property(e => e.EmployerName).HasColumnName("EmployerName").HasColumnType("nvarchar(100)").IsRequired();
                entity.Property(e => e.CompanyName).HasColumnName("CompanyName").HasColumnType("nvarchar(100)").IsRequired();
                entity.Property(e => e.Email).HasColumnName("Email").HasColumnType("nvarchar(100)").IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.ContactPhone).HasColumnName("ContactPhone").HasColumnType("nvarchar(100)").IsRequired();
                entity.HasIndex(e => e.ContactPhone).IsUnique();
                entity.Property(e => e.Role).HasColumnName("Role").HasColumnType("nvarchar(50)").HasDefaultValue("Employer");
            });

            modelBuilder.Entity<JobListing>(entity =>
            {
                entity.ToTable("JobListing");

                entity.HasKey(l => l.JobListingId);

                entity.Property(l => l.JobListingId).HasColumnName("JobListingId").IsRequired();
                entity.Property(l => l.EmployerId).HasColumnName("EmployerId").IsRequired();
                entity.Property(l => l.JobTitle).HasColumnName("JobTitle").HasColumnType("nvarchar(100)").IsRequired();
                entity.Property(l => l.JobDescription).HasColumnName("JobDescription").HasColumnType("nvarchar(max)").IsRequired();
                entity.Property(l => l.CompanyName).HasColumnName("CompanyName").HasColumnType("nvarchar(100)").IsRequired();
                entity.Property(l => l.HiringWorkflow).HasColumnName("HiringWorkflow").HasColumnType("nvarchar(max)").IsRequired();
                entity.Property(l => l.EligibilityCriteria).HasColumnName("EligibilityCriteria").HasColumnType("nvarchar(max)").IsRequired();
                entity.Property(l => l.RequiredSkills).HasColumnName("RequiredSkills").HasColumnType("nvarchar(max)").IsRequired();
                entity.Property(l => l.AboutCompany).HasColumnName("AboutCompany").HasColumnType("nvarchar(max)").IsRequired();
                entity.Property(l => l.Location).HasColumnName("Location").HasColumnType("nvarchar(100)").IsRequired();
                entity.Property(l => l.Salary).HasColumnName("Salary").HasColumnType("decimal(10, 2)").IsRequired();
                entity.HasCheckConstraint("CHK_Salary", "Salary > 0");
                entity.Property(l => l.PostedDate).HasColumnName("PostedDate").HasColumnType("datetime").IsRequired();
                entity.Property(l => l.Deadline).HasColumnName("Deadline").HasColumnType("datetime").IsRequired();
                entity.Property(l => l.VacancyOfJob).HasColumnName("VacancyOfJob").HasColumnType("bit").IsRequired();

                entity.HasOne(l => l.Employer)
                .WithMany(e => e.JobListings)
                .HasForeignKey(l => l.EmployerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_JobListing_Employer");
            });

            modelBuilder.Entity<JobSeeker>(entity =>
            {
                entity.ToTable("JobSeeker");

                entity.HasKey(j => j.JobSeekerId);

                entity.Property(j => j.JobSeekerId).HasColumnName("JobSeekerId").IsRequired();
                entity.Property(j => j.UserName).HasColumnName("Username").HasColumnType("nvarchar(100)").IsRequired();
                entity.HasIndex(j => j.UserName).IsUnique();
                entity.Property(j => j.Password).HasColumnName("Password").HasColumnType("nvarchar(100)").IsRequired();
                entity.Property(j => j.JobSeekerName).HasColumnName("JobSeekerName").HasColumnType("nvarchar(100)").IsRequired();
                entity.Property(j => j.Description).HasColumnName("Description").HasColumnType("nvarchar(100)").IsRequired();
                entity.Property(j => j.Email).HasColumnName("ContactEmail").HasColumnType("nvarchar(100)").IsRequired();
                entity.HasIndex(j => j.Email).IsUnique();
                entity.Property(j => j.ContactPhone).HasColumnName("ContactPhone").HasColumnType("nvarchar(100)").IsRequired();
                entity.HasIndex(j => j.ContactPhone).IsUnique();
                entity.Property(j => j.DateOfBirth).HasColumnName("DateOfBirth").HasColumnType("datetime").IsRequired();
                entity.Property(j => j.Gender).HasColumnName("Gender").HasColumnType("nvarchar(100)").IsRequired();
                entity.HasCheckConstraint("CHK_GENDER", "Gender IN ('Male', 'Female')");
                entity.Property(j => j.Address).HasColumnName("Address").HasColumnType("nvarchar(100)").IsRequired();
                entity.Property(j => j.Qualification).HasColumnName("Qualification").HasColumnType("nvarchar(100)").IsRequired();
                entity.Property(j => j.Specialization).HasColumnName("Specialization").HasColumnType("nvarchar(100)").IsRequired();
                entity.Property(j => j.Institute).HasColumnName("Institute").HasColumnType("nvarchar(100)").IsRequired();
                entity.Property(j => j.Year).HasColumnName("Year").HasColumnType("int").IsRequired();
                entity.HasCheckConstraint("CHK_Year", "Year >= 1970 AND Year <= 2024");
                entity.Property(j => j.CGPA).HasColumnName("CGPA").HasColumnType("decimal(4,2)").IsRequired();
                entity.HasCheckConstraint("CHK_CGPA", "CGPA >= 0.0 AND CGPA <= 10.0");
                entity.Property(j => j.CompanyName).HasColumnName("CompanyName").HasColumnType("nvarchar(100)").IsRequired();
                entity.Property(j => j.Position).HasColumnName("Position").HasColumnType("nvarchar(100)").IsRequired();
                entity.Property(j => j.StartDate).HasColumnName("StartDate").HasColumnType("datetime").IsRequired();
                entity.Property(j => j.EndDate).HasColumnName("EndDate").HasColumnType("datetime").IsRequired();
                entity.Property(j => j.Role).HasColumnName("Role").HasColumnType("nvarchar(50)").HasDefaultValue("JobSeeker");
            });

            modelBuilder.Entity<Resume>(entity =>
            {
                entity.ToTable("Resume");

                entity.Property(e => e.ResumeId)
                    .ValueGeneratedNever()
                    .HasColumnName("ResumeId");

                entity.Property(e => e.JobSeekerId).HasColumnName("JobSeekerId");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.UploadDate).HasColumnType("datetime");

                entity.HasOne(d => d.JobSeeker)
                    .WithMany(p => p.Resumes)
                    .HasForeignKey(d => d.JobSeekerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Resume_JobSeeker");
            });
        }

    }
}
