using CaseStudyAPI.Data;
using CaseStudyAPI.DTO;
using CaseStudyAPI.Repository.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using CaseStudyAPI.Models;

namespace CaseStudyTest
{
    [TestFixture]
    public class ApplicationServicesTest
    {
        private ApplicationServices _applicationServices;
        private ApplicationDBContext _dbContext;
        private Mock<ILogger<ApplicationServices>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ApplicationDBContext(options);
            _mockLogger = new Mock<ILogger<ApplicationServices>>();

            _applicationServices = new ApplicationServices(_dbContext, _mockLogger.Object);
        }


        [Test]
        public async Task CreateApplicationAsync_ShouldReturnApplication_WhenSuccessful()
        {
            var applicationDTO = new ApplicationDTO
            {
                JobListingId = "job123"
            };
            string jobSeekerId = Guid.NewGuid().ToString();

            var result = await _applicationServices.CreateApplicationAsync(applicationDTO, jobSeekerId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.JobListingId, Is.EqualTo("job123"));
            Assert.That(result.JobSeekerId, Is.EqualTo(jobSeekerId));
            Assert.That(result.ApplicationStatus, Is.EqualTo("Pending"));
        }

        [Test]
        public async Task CreateApplicationAsync_ShouldReturnNull_WhenExceptionOccurs()
        {
            _dbContext.Dispose();
            var applicationDTO = new ApplicationDTO
            {
                JobListingId = "job123"
            };

            var result = await _applicationServices.CreateApplicationAsync(applicationDTO, "seeker123");

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task DeleteApplicationAsync_ShouldReturnTrue_WhenApplicationDeleted()
        {
            var application = new Application
            {
                ApplicationId = "app123",
                JobListingId = Guid.NewGuid().ToString(),
                JobSeekerId = "seeker123",
                ApplicationDate = DateTime.UtcNow,
                ApplicationStatus = "Pending"
            };

            await _dbContext.Applications.AddAsync(application);
            await _dbContext.SaveChangesAsync();

            var result = await _applicationServices.DeleteApplicationAsync("app123", "seeker123");

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task DeleteApplicationAsync_ShouldReturnFalse_WhenApplicationNotFound()
        {
            var result = await _applicationServices.DeleteApplicationAsync("nonexistentApp", "seeker123");

            Assert.That(result,Is.False);
        }

        [Test]
        public async Task GetApplicationByIdAsync_ShouldReturnApplication_WhenApplicationExists()
        {
            var application = new Application
            {
                ApplicationId = "app123",
                JobSeekerId = "seeker123",
                JobListingId = Guid.NewGuid().ToString(),
                ApplicationDate = DateTime.UtcNow,
                ApplicationStatus = "Pending"
            };
            await _dbContext.Applications.AddAsync(application);
            await _dbContext.SaveChangesAsync();

            var result = await _applicationServices.GetApplicationByIdAsync("app123");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ApplicationId, Is.EqualTo("app123"));
        }

        [Test]
        public async Task GetApplicationByIdAsync_ShouldReturnNull_WhenApplicationNotFound()
        {
            var result = await _applicationServices.GetApplicationByIdAsync("nonexistentApp");

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetApplicationByJSIdAsync_ShouldReturnApplications_WhenJobSeekerHasApplications()
        {
            var applications = new List<Application>
            {
                new() {
                    ApplicationId = "app1",
                    JobSeekerId = "seeker123",
                    JobListingId = Guid.NewGuid().ToString(),
                    ApplicationDate = DateTime.UtcNow,
                    ApplicationStatus = "Pending"
                },
                new() {
                    ApplicationId = "app2",
                    JobSeekerId = "seeker123",
                    JobListingId = Guid.NewGuid().ToString(),
                    ApplicationDate = DateTime.UtcNow,
                    ApplicationStatus = "Pending"
                }
            };
            await _dbContext.Applications.AddRangeAsync(applications);
            await _dbContext.SaveChangesAsync();

            var result = await _applicationServices.GetApplicationByJSIdAsync("seeker123");

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task UpdateApplicationAsync_ShouldReturnTrue_WhenApplicationUpdated()
        {
            var application = new Application
            {
                ApplicationId = "app123",
                JobSeekerId = "seeker123",
                JobListingId = Guid.NewGuid().ToString(),
                ApplicationDate = DateTime.UtcNow,
                ApplicationStatus = "Pending"
            };
            await _dbContext.Applications.AddAsync(application);
            await _dbContext.SaveChangesAsync();

            var result = await _applicationServices.UpdateApplicationAsync("app123", "Approved");

            Assert.That(result, Is.True);
            var updatedApplication = await _dbContext.Applications.FirstOrDefaultAsync(a => a.ApplicationId == "app123");
            Assert.That(updatedApplication.ApplicationStatus, Is.EqualTo("Approved"));
        }

        [Test]
        public async Task UpdateApplicationAsync_ShouldReturnFalse_WhenApplicationNotFound()
        {
            var result = await _applicationServices.UpdateApplicationAsync("nonexistentApp", "Approved");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetAllApplicationsAsync_ShouldReturnAllApplications()
        {
            var applications = new List<Application>
            {
                new () {
                    ApplicationId = "app1",
                    JobSeekerId = "seeker123",
                    JobListingId = Guid.NewGuid().ToString(),
                    ApplicationDate = DateTime.UtcNow,
                    ApplicationStatus = "Pending"
                },
                new () {
                    ApplicationId = "app2",
                    JobSeekerId = "seeker123",
                    JobListingId = Guid.NewGuid().ToString(),
                    ApplicationDate = DateTime.UtcNow,
                    ApplicationStatus = "Pending"
                }
            };
            await _dbContext.Applications.AddRangeAsync(applications);
            await _dbContext.SaveChangesAsync();

            var result = await _applicationServices.GetAllApplicationsAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
        }
    }
}
