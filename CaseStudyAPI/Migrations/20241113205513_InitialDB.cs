using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CaseStudyAPI.Migrations
{
    public partial class InitialDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employer",
                columns: table => new
                {
                    EmployerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmployerName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", nullable: true, defaultValue: "Employer")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employer", x => x.EmployerId);
                    table.CheckConstraint("CHK_GENDERS", "Gender IN ('Male', 'Female')");
                });

            migrationBuilder.CreateTable(
                name: "JobSeeker",
                columns: table => new
                {
                    JobSeekerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    JobSeekerName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime", nullable: false),
                    Qualification = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Specialization = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Institute = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    CGPA = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", nullable: true, defaultValue: "JobSeeker")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSeeker", x => x.JobSeekerId);
                    table.CheckConstraint("CHK_CGPA", "CGPA >= 0.0 AND CGPA <= 10.0");
                    table.CheckConstraint("CHK_GENDER", "Gender IN ('Male', 'Female')");
                    table.CheckConstraint("CHK_Year", "Year >= 1970 AND Year <= 2024");
                });

            migrationBuilder.CreateTable(
                name: "JobListing",
                columns: table => new
                {
                    JobListingId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmployerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    JobDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    HiringWorkflow = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EligibilityCriteria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequiredSkills = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AboutCompany = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PostedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime", nullable: false),
                    VacancyOfJob = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobListing", x => x.JobListingId);
                    table.CheckConstraint("CHK_Salary", "Salary > 0");
                    table.ForeignKey(
                        name: "FK_JobListing_Employer",
                        column: x => x.EmployerId,
                        principalTable: "Employer",
                        principalColumn: "EmployerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Resume",
                columns: table => new
                {
                    ResumeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    FileData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    JobSeekerId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resume", x => x.ResumeId);
                    table.ForeignKey(
                        name: "FK_Resume_JobSeeker",
                        column: x => x.JobSeekerId,
                        principalTable: "JobSeeker",
                        principalColumn: "JobSeekerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Application",
                columns: table => new
                {
                    ApplicationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    JobListingId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    JobSeekerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ApplicationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ApplicationStatus = table.Column<string>(type: "nvarchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application", x => x.ApplicationId);
                    table.CheckConstraint("CHK_Status", "ApplicationStatus IN ('Confirmed', 'Pending','Cancelled')");
                    table.ForeignKey(
                        name: "FK_Application_JobListing",
                        column: x => x.JobListingId,
                        principalTable: "JobListing",
                        principalColumn: "JobListingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Application_JobSeeker",
                        column: x => x.JobSeekerId,
                        principalTable: "JobSeeker",
                        principalColumn: "JobSeekerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Application_JobListingId",
                table: "Application",
                column: "JobListingId");

            migrationBuilder.CreateIndex(
                name: "IX_Application_JobSeekerId",
                table: "Application",
                column: "JobSeekerId");

            migrationBuilder.CreateIndex(
                name: "IX_Employer_ContactPhone",
                table: "Employer",
                column: "ContactPhone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employer_Email",
                table: "Employer",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employer_Username",
                table: "Employer",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobListing_EmployerId",
                table: "JobListing",
                column: "EmployerId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSeeker_ContactEmail",
                table: "JobSeeker",
                column: "ContactEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobSeeker_ContactPhone",
                table: "JobSeeker",
                column: "ContactPhone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobSeeker_Username",
                table: "JobSeeker",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resume_JobSeekerId",
                table: "Resume",
                column: "JobSeekerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Application");

            migrationBuilder.DropTable(
                name: "Resume");

            migrationBuilder.DropTable(
                name: "JobListing");

            migrationBuilder.DropTable(
                name: "JobSeeker");

            migrationBuilder.DropTable(
                name: "Employer");
        }
    }
}
