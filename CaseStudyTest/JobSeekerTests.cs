using CaseStudyAPI.Data;
using CaseStudyAPI.Models;
using CaseStudyAPI.Repository.Interfaces;
using CaseStudyAPI.Repository.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace CaseStudyTest
{
    public class JobSeekerTests
    {
        private Mock<IAuthorizationService> _mockAuthService;
        private Mock<ApplicationDBContext> _mockDbContext;
        private Mock<ILogger<JobSeekerServices>> _mockLogger;
        private JobSeekerServices _jobSeekerServices;

        [SetUp]
        public void SetUp()
        {
            _mockAuthService = new Mock<IAuthorizationService>();
            _mockDbContext = new Mock<ApplicationDBContext>();
            _mockLogger = new Mock<ILogger<JobSeekerServices>>();
            _jobSeekerServices = new JobSeekerServices(_mockAuthService.Object, _mockDbContext.Object, _mockLogger.Object);
        }

        [Test]
        public async Task CreateJobSeekerAsync_WhenJobSeekerAlreadyExists_ReturnsFailureResponse()
        {
            // Arrange
            var existingJobSeeker = new JobSeeker { UserName = "testUser", Email = "test@test.com" };
            var newJobSeeker = new JobSeeker { UserName = "testUser", Email = "test@test.com" };
            // Act
            var response = await _jobSeekerServices.CreateJobSeekerAsync(newJobSeeker);

            // Assert
            Assert.AreEqual("Failure", response.Status);
            Assert.AreEqual("An Job Seeker with this username or email already exists.", response.Message);
        }

        [Test]
        public async Task CreateJobSeekerAsync_WhenJobSeekerIsCreated_ReturnsSuccessResponse()
        {
            // Arrange
            var newJobSeeker = new JobSeeker { UserName = "newUser", Email = "new@test.com" };
            _mockDbContext.Setup(db => db.JobSeekers.FirstOrDefaultAsync(It.IsAny<Func<JobSeeker, bool>>()))
                          .ReturnsAsync((JobSeeker)null);
            _mockAuthService.Setup(auth => auth.HashPassword(It.IsAny<string>())).Returns("hashedPassword");

            // Act
            var response = await _jobSeekerServices.CreateJobSeekerAsync(newJobSeeker);

            // Assert
            Assert.AreEqual("Success", response.Status);
            Assert.AreEqual("User Created Successfully.", response.Message);
        }

        [Test]
        public async Task DeleteJobSeekerAsync_WhenJobSeekerDoesNotExist_ReturnsNotFoundResponse()
        {
            // Arrange
            _mockDbContext.Setup(db => db.JobSeekers.FirstOrDefaultAsync(It.IsAny<Func<JobSeeker, bool>>()))
                          .ReturnsAsync((JobSeeker)null);

            // Act
            var response = await _jobSeekerServices.DeleteJobSeekerAsync("nonexistent-id");

            // Assert
            Assert.AreEqual("Success", response.Status);
            Assert.AreEqual("Job Seeker not found with the given ID.", response.Message);
        }

        [Test]
        public async Task DeleteJobSeekerAsync_WhenJobSeekerIsDeleted_ReturnsSuccessResponse()
        {
            // Arrange
            var jobSeeker = new JobSeeker { JobSeekerId = "123" };
            _mockDbContext.Setup(db => db.JobSeekers.FirstOrDefaultAsync(It.IsAny<Func<JobSeeker, bool>>()))
                          .ReturnsAsync(jobSeeker);

            // Act
            var response = await _jobSeekerServices.DeleteJobSeekerAsync("123");

            // Assert
            Assert.AreEqual("Success", response.Status);
            Assert.AreEqual("Job Seeker deleted successfully.", response.Message);
        }

        [Test]
        public async Task GetAllJobSeekersAsync_WhenJobSeekersExist_ReturnsList()
        {
            // Arrange
            var jobSeekers = new List<JobSeeker> { new JobSeeker(), new JobSeeker() };
            _mockDbContext.Setup(db => db.JobSeekers.ToListAsync())
                          .ReturnsAsync(jobSeekers);

            // Act
            var result = await _jobSeekerServices.GetAllJobSeekersAsync();

            // Assert
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task GetAllJobSeekersAsync_WhenNoJobSeekersExist_ReturnsEmptyList()
        {
            // Arrange
            _mockDbContext.Setup(db => db.JobSeekers.ToListAsync())
                          .ReturnsAsync(new List<JobSeeker>());

            // Act
            var result = await _jobSeekerServices.GetAllJobSeekersAsync();

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetJobSeekerByUserNameAsync_WhenJobSeekerExists_ReturnsJobSeeker()
        {
            // Arrange
            var jobSeeker = new JobSeeker { UserName = "testUser" };
            _mockDbContext.Setup(db => db.JobSeekers.FirstOrDefaultAsync(It.IsAny<Func<JobSeeker, bool>>()))
                          .ReturnsAsync(jobSeeker);

            // Act
            var result = await _jobSeekerServices.GetJobSeekerByUserNameAsync("testUser");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("testUser", result.UserName);
        }

        [Test]
        public async Task UpdateJobSeekerAsync_WhenJobSeekerDoesNotExist_ReturnsNotFoundResponse()
        {
            // Arrange
            _mockDbContext.Setup(db => db.JobSeekers.FirstOrDefaultAsync(It.IsAny<Func<JobSeeker, bool>>()))
                          .ReturnsAsync((JobSeeker)null);

            // Act
            var response = await _jobSeekerServices.UpdateJobSeekerAsync("nonexistent-id", new JobSeeker());

            // Assert
            Assert.AreEqual("Success", response.Status);
            Assert.AreEqual("Job Seeker not found with the given ID.", response.Message);
        }

        [Test]
        public async Task UpdateJobSeekerAsync_WhenJobSeekerIsUpdated_ReturnsSuccessResponse()
        {
            // Arrange
            var existingJobSeeker = new JobSeeker { JobSeekerId = "123" };
            _mockDbContext.Setup(db => db.JobSeekers.FirstOrDefaultAsync(It.IsAny<Func<JobSeeker, bool>>()))
                          .ReturnsAsync(existingJobSeeker);

            // Act
            var response = await _jobSeekerServices.UpdateJobSeekerAsync("123", new JobSeeker { UserName = "updatedUser" });

            // Assert
            Assert.AreEqual("Success", response.Status);
            Assert.AreEqual("Job Seeker updated successfully.", response.Message);
        }
    }
}
