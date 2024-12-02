using AutoMapper;
using CaseStudyAPI.Data;
using CaseStudyAPI.DTO;
using CaseStudyAPI.Models;
using CaseStudyAPI.Repository.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace CaseStudyTest
{
    [TestFixture]
    public class JobListingTests
    {
        private ApplicationDBContext _dbContext;
        private Mock<IMapper> _mockMapper;
        private Mock<ILogger<JobListingServices>> _mockLogger;
        private JobListingServices _jobListingServices;


        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ApplicationDBContext(options);

            _mockLogger = new Mock<ILogger<JobListingServices>>();
            _mockMapper = new Mock<IMapper>();

            _jobListingServices = new JobListingServices(_dbContext, _mockMapper.Object, _mockLogger.Object);
        }

        [Test]
        public async Task CreateJobListingAsync_ShouldCreateJobListing_WhenValidDataProvided()
        {
            var jobListingDto = new JobListingDTO
            {
                JobTitle = "Software Developer",
                JobDescription = "Develop software solutions.",
                CompanyName = "TechCorp",
                Location = "Remote",
                Salary = 60000,
                EligibilityCriteria = "Bachelor's Degree",
                RequiredSkills = "C#, ASP.NET",
                HiringWorkflow = "Resume Shortlisting -> Technical Round -> Managerial Round",
                AboutCompany = "A leading tech company.",
                Deadline = DateTime.UtcNow.AddDays(30),
                VacancyOfJob = true
            };

            var employerId = Guid.NewGuid().ToString();

            _mockMapper.Setup(m => m.Map<JobListing>(jobListingDto)).Returns(new JobListing
            {
                JobTitle = jobListingDto.JobTitle,
                JobDescription = jobListingDto.JobDescription,
                CompanyName = jobListingDto.CompanyName,
                Location = jobListingDto.Location,
                Salary = jobListingDto.Salary,
                EligibilityCriteria = jobListingDto.EligibilityCriteria,
                RequiredSkills = jobListingDto.RequiredSkills,
                HiringWorkflow = jobListingDto.HiringWorkflow,
                AboutCompany = jobListingDto.AboutCompany,
                Deadline = jobListingDto.Deadline,
                VacancyOfJob = jobListingDto.VacancyOfJob
            });

            var result = await _jobListingServices.CreateJobListingAsync(jobListingDto, employerId);

            Assert.That(result, Is.Not.Null, "JobListing should not be null");
            Assert.That(result.JobTitle, Is.EqualTo(jobListingDto.JobTitle), "Job title mismatch");
            Assert.That(result.EmployerId, Is.EqualTo(employerId), "Employer ID mismatch");
        }

        [Test]
        public async Task DeleteJobListingAsync_ShouldReturnSuccessMessage_WhenJobListingExists()
        {
            var employerId = Guid.NewGuid().ToString();
            var jobListing = new JobListing
            {
                JobListingId = Guid.NewGuid().ToString(),
                EmployerId = employerId,
                JobTitle = "Software Developer",
                JobDescription = "Develop software solutions.",
                CompanyName = "TechCorp",
                Location = "Remote",
                Salary = 60000,
                EligibilityCriteria = "Bachelor's Degree",
                RequiredSkills = "C#, ASP.NET",
                HiringWorkflow = "Resume Shortlisting -> Technical Round -> Managerial Round",
                AboutCompany = "A leading tech company.",
                Deadline = DateTime.UtcNow.AddDays(30),
                VacancyOfJob = true
            };
            await _dbContext.JobListings.AddAsync(jobListing);
            await _dbContext.SaveChangesAsync();

            var result = await _jobListingServices.DeleteJobListingAsync(jobListing.JobListingId, employerId);

            Assert.That(result, Is.EqualTo($"Deleted job listing successfully for job title {jobListing.JobTitle}"));
        }

        [Test]
        public async Task DeleteJobListingAsync_ShouldReturnErrorMessage_WhenJobListingNotFound()
        {
            var result = await _jobListingServices.DeleteJobListingAsync("InvalidId", "InvalidEmployerId");
            Assert.That(result, Is.EqualTo("Job Listing not found or you do not have permission to delete it."));
        }

        [Test]
        public async Task GetAllJobListingsAsync_ShouldReturnAllJobListings()
        {
            var employerId = Guid.NewGuid().ToString();
            var jobListing1 = new JobListing()
            {
                JobListingId = Guid.NewGuid().ToString(),
                EmployerId = employerId,
                JobTitle = ".Net Developer",
                JobDescription = "Develop .Net Applications.",
                CompanyName = "TechCorp",
                Location = "Remote",
                Salary = 70000,
                EligibilityCriteria = "Bachelor's Degree",
                RequiredSkills = "C#, ASP.NET",
                HiringWorkflow = "Resume Shortlisting -> Technical Round -> Managerial Round",
                AboutCompany = "A leading tech company.",
                Deadline = DateTime.UtcNow.AddDays(30),
                PostedDate = DateTime.UtcNow,
                VacancyOfJob = true
            };

            var jobListing2 = new JobListing() {
                    JobListingId = Guid.NewGuid().ToString(),
                    EmployerId = employerId,
                    JobTitle = "Software Developer",
                    JobDescription = "Develop software solutions.",
                    CompanyName = "TechCorp",
                    Location = "Remote",
                    Salary = 60000,
                    EligibilityCriteria = "Bachelor's Degree",
                    RequiredSkills = "C#, ASP.NET",
                    HiringWorkflow = "Resume Shortlisting -> Technical Round -> Managerial Round",
                    AboutCompany = "A leading tech company.",
                    Deadline = DateTime.UtcNow.AddDays(30),
                    PostedDate = DateTime.UtcNow,
                    VacancyOfJob = true
            };
            await _dbContext.JobListings.AddRangeAsync(jobListing1,jobListing2);
            await _dbContext.SaveChangesAsync();

            var result = await _jobListingServices.GetAllJobListingsAsync();

            Assert.That(result, Has.Count.EqualTo(2));
        }
        [Test]
        public async Task GetJobListingByAvailability_ShouldReturnMatchingJobListings()
        {
            var employerId = Guid.NewGuid().ToString();
            var jobListing1 = new JobListing()
            {
                JobListingId = Guid.NewGuid().ToString(),
                EmployerId = employerId,
                JobTitle = ".Net Developer",
                JobDescription = "Develop .Net Applications.",
                CompanyName = "TechCorp",
                Location = "Remote",
                Salary = 70000,
                EligibilityCriteria = "Bachelor's Degree",
                RequiredSkills = "C#, ASP.NET",
                HiringWorkflow = "Resume Shortlisting -> Technical Round -> Managerial Round",
                AboutCompany = "A leading tech company.",
                Deadline = DateTime.UtcNow.AddDays(30),
                PostedDate = DateTime.UtcNow,
                VacancyOfJob = false
            };

            var jobListing2 = new JobListing() {
                    JobListingId = Guid.NewGuid().ToString(),
                    EmployerId = employerId,
                    JobTitle = "Software Developer",
                    JobDescription = "Develop software solutions.",
                    CompanyName = "TechCorp",
                    Location = "Remote",
                    Salary = 60000,
                    EligibilityCriteria = "Bachelor's Degree",
                    RequiredSkills = "C#, ASP.NET",
                    HiringWorkflow = "Resume Shortlisting -> Technical Round -> Managerial Round",
                    AboutCompany = "A leading tech company.",
                    Deadline = DateTime.UtcNow.AddDays(30),
                    PostedDate = DateTime.UtcNow,
                    VacancyOfJob = true
            };
            await _dbContext.JobListings.AddRangeAsync(jobListing1,jobListing2);
            await _dbContext.SaveChangesAsync();

            var result = await _jobListingServices.GetJobListingByAvailability(true);

            Assert.That(result, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task GetJobListingByIdAsync_ShouldReturnJobListing_WhenJobListingExists()
        {
            var employerId = Guid.NewGuid().ToString();
            var jobListingId = Guid.NewGuid().ToString();
            var jobListing = new JobListing()
            {
                JobListingId = jobListingId,
                EmployerId = employerId,
                JobTitle = ".Net Developer",
                JobDescription = "Develop .Net Applications.",
                CompanyName = "TechCorp",
                Location = "Remote",
                Salary = 70000,
                EligibilityCriteria = "Bachelor's Degree",
                RequiredSkills = "C#, ASP.NET",
                HiringWorkflow = "Resume Shortlisting -> Technical Round -> Managerial Round",
                AboutCompany = "A leading tech company.",
                Deadline = DateTime.UtcNow.AddDays(30),
                PostedDate = DateTime.UtcNow,
                VacancyOfJob = false
            };
            await _dbContext.JobListings.AddAsync(jobListing);
            await _dbContext.SaveChangesAsync();

            var result = await _jobListingServices.GetJobListingByIdAsync(jobListingId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.JobListingId, Is.EqualTo(jobListingId));
        }

        [Test]
        public async Task UpdateJobListingAsync_ShouldUpdateJobListing_WhenValidDataProvided()
        {
            var employerId = Guid.NewGuid().ToString();
            var jobListing = new JobListing
            {
                JobListingId = Guid.NewGuid().ToString(),
                EmployerId = employerId,
                JobTitle = "Old Title",
                JobDescription = "Develop .Net Applications.",
                CompanyName = "TechCorp",
                Location = "Remote",
                Salary = 70000,
                EligibilityCriteria = "Bachelor's Degree",
                RequiredSkills = "C#, ASP.NET",
                HiringWorkflow = "Resume Shortlisting -> Technical Round -> Managerial Round",
                AboutCompany = "A leading tech company.",
                Deadline = DateTime.UtcNow.AddDays(30),
                PostedDate = DateTime.UtcNow,
                VacancyOfJob = false
            };
            await _dbContext.JobListings.AddAsync(jobListing);
            await _dbContext.SaveChangesAsync();

            var jobListingDto = new JobListingDTO
            {
                JobTitle = "Updated Title",
                JobDescription = "Updated Description",
                CompanyName = "Updated Company",
                Location = "Remote",
                Salary = 70000,
                EligibilityCriteria = "Bachelor's Degree",
                RequiredSkills = "C#, ASP.NET",
                HiringWorkflow = "Resume Shortlisting -> Technical Round -> Managerial Round",
                AboutCompany = "A leading tech company.",
                Deadline = DateTime.UtcNow.AddDays(30),
                VacancyOfJob = false
            };

            var result = await _jobListingServices.UpdateJobListingAsync(jobListing.JobListingId, employerId, jobListingDto);

            Assert.That(result, Is.EqualTo("Updated Job Listing Successfully"));

            var updatedJobListing = await _dbContext.JobListings.FirstOrDefaultAsync(j => j.JobListingId == jobListing.JobListingId);
            Assert.That(updatedJobListing.JobTitle, Is.EqualTo(jobListingDto.JobTitle));
        }
    }
}
