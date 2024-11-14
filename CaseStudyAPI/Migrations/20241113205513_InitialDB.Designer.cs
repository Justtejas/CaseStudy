﻿// <auto-generated />
using System;
using CaseStudyAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CaseStudyAPI.Migrations
{
    [DbContext(typeof(ApplicationDBContext))]
    [Migration("20241113205513_InitialDB")]
    partial class InitialDB
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.35")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("CaseStudyAPI.Models.Application", b =>
                {
                    b.Property<string>("ApplicationId")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("ApplicationId");

                    b.Property<DateTime>("ApplicationDate")
                        .HasColumnType("datetime")
                        .HasColumnName("ApplicationDate");

                    b.Property<string>("ApplicationStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("ApplicationStatus");

                    b.Property<string>("JobListingId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("JobListingId");

                    b.Property<string>("JobSeekerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("JobSeekerId");

                    b.HasKey("ApplicationId");

                    b.HasIndex("JobListingId");

                    b.HasIndex("JobSeekerId");

                    b.ToTable("Application", (string)null);

                    b.HasCheckConstraint("CHK_Status", "ApplicationStatus IN ('Confirmed', 'Pending','Cancelled')");
                });

            modelBuilder.Entity("CaseStudyAPI.Models.Employer", b =>
                {
                    b.Property<string>("EmployerId")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("EmployerId");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("CompanyName");

                    b.Property<string>("ContactPhone")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("ContactPhone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("Email");

                    b.Property<string>("EmployerName")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("EmployerName");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("Gender");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("Password");

                    b.Property<string>("Role")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(50)")
                        .HasDefaultValue("Employer")
                        .HasColumnName("Role");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("Username");

                    b.HasKey("EmployerId");

                    b.HasIndex("ContactPhone")
                        .IsUnique();

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("Employer", (string)null);

                    b.HasCheckConstraint("CHK_GENDERS", "Gender IN ('Male', 'Female')");
                });

            modelBuilder.Entity("CaseStudyAPI.Models.JobListing", b =>
                {
                    b.Property<string>("JobListingId")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("JobListingId");

                    b.Property<string>("AboutCompany")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("AboutCompany");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("CompanyName");

                    b.Property<DateTime>("Deadline")
                        .HasColumnType("datetime")
                        .HasColumnName("Deadline");

                    b.Property<string>("EligibilityCriteria")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("EligibilityCriteria");

                    b.Property<string>("EmployerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("EmployerId");

                    b.Property<string>("HiringWorkflow")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("HiringWorkflow");

                    b.Property<string>("JobDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("JobDescription");

                    b.Property<string>("JobTitle")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("JobTitle");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("Location");

                    b.Property<DateTime>("PostedDate")
                        .HasColumnType("datetime")
                        .HasColumnName("PostedDate");

                    b.Property<string>("RequiredSkills")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("RequiredSkills");

                    b.Property<decimal>("Salary")
                        .HasColumnType("decimal(10,2)")
                        .HasColumnName("Salary");

                    b.Property<bool?>("VacancyOfJob")
                        .IsRequired()
                        .HasColumnType("bit")
                        .HasColumnName("VacancyOfJob");

                    b.HasKey("JobListingId");

                    b.HasIndex("EmployerId");

                    b.ToTable("JobListing", (string)null);

                    b.HasCheckConstraint("CHK_Salary", "Salary > 0");
                });

            modelBuilder.Entity("CaseStudyAPI.Models.JobSeeker", b =>
                {
                    b.Property<string>("JobSeekerId")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("JobSeekerId");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("Address");

                    b.Property<decimal>("CGPA")
                        .HasColumnType("decimal(4,2)")
                        .HasColumnName("CGPA");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("CompanyName");

                    b.Property<string>("ContactPhone")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("ContactPhone");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime")
                        .HasColumnName("DateOfBirth");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("Description");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("ContactEmail");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime")
                        .HasColumnName("EndDate");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("Gender");

                    b.Property<string>("Institute")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("Institute");

                    b.Property<string>("JobSeekerName")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("JobSeekerName");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("Password");

                    b.Property<string>("Position")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("Position");

                    b.Property<string>("Qualification")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("Qualification");

                    b.Property<string>("Role")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(50)")
                        .HasDefaultValue("JobSeeker")
                        .HasColumnName("Role");

                    b.Property<string>("Specialization")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("Specialization");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime")
                        .HasColumnName("StartDate");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("Username");

                    b.Property<int>("Year")
                        .HasColumnType("int")
                        .HasColumnName("Year");

                    b.HasKey("JobSeekerId");

                    b.HasIndex("ContactPhone")
                        .IsUnique();

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("JobSeeker", (string)null);

                    b.HasCheckConstraint("CHK_CGPA", "CGPA >= 0.0 AND CGPA <= 10.0");

                    b.HasCheckConstraint("CHK_GENDER", "Gender IN ('Male', 'Female')");

                    b.HasCheckConstraint("CHK_Year", "Year >= 1970 AND Year <= 2024");
                });

            modelBuilder.Entity("CaseStudyAPI.Models.Resume", b =>
                {
                    b.Property<string>("ResumeId")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("ResumeId");

                    b.Property<byte[]>("FileData")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("FileSize")
                        .HasColumnType("bigint");

                    b.Property<string>("FileType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("JobSeekerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("JobSeekerId");

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("UploadDate")
                        .HasColumnType("datetime");

                    b.HasKey("ResumeId");

                    b.HasIndex("JobSeekerId");

                    b.ToTable("Resume", (string)null);
                });

            modelBuilder.Entity("CaseStudyAPI.Models.Application", b =>
                {
                    b.HasOne("CaseStudyAPI.Models.JobListing", "JobListing")
                        .WithMany("Applications")
                        .HasForeignKey("JobListingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Application_JobListing");

                    b.HasOne("CaseStudyAPI.Models.JobSeeker", "JobSeeker")
                        .WithMany("Applications")
                        .HasForeignKey("JobSeekerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Application_JobSeeker");

                    b.Navigation("JobListing");

                    b.Navigation("JobSeeker");
                });

            modelBuilder.Entity("CaseStudyAPI.Models.JobListing", b =>
                {
                    b.HasOne("CaseStudyAPI.Models.Employer", "Employer")
                        .WithMany("JobListings")
                        .HasForeignKey("EmployerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_JobListing_Employer");

                    b.Navigation("Employer");
                });

            modelBuilder.Entity("CaseStudyAPI.Models.Resume", b =>
                {
                    b.HasOne("CaseStudyAPI.Models.JobSeeker", "JobSeeker")
                        .WithMany("Resumes")
                        .HasForeignKey("JobSeekerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Resume_JobSeeker");

                    b.Navigation("JobSeeker");
                });

            modelBuilder.Entity("CaseStudyAPI.Models.Employer", b =>
                {
                    b.Navigation("JobListings");
                });

            modelBuilder.Entity("CaseStudyAPI.Models.JobListing", b =>
                {
                    b.Navigation("Applications");
                });

            modelBuilder.Entity("CaseStudyAPI.Models.JobSeeker", b =>
                {
                    b.Navigation("Applications");

                    b.Navigation("Resumes");
                });
#pragma warning restore 612, 618
        }
    }
}
