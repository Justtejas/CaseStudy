using CaseStudyAPI.Data;
using CaseStudyAPI.Models;
using CaseStudyAPI.Repository.Services;
using CaseStudyAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.Extensions.Logging;

namespace CaseStudyTest
{
    [TestFixture]
    public class JobSeekerServicesTests
    {
        private Mock<IAuthorizationService> _mockAuthorizationService;
        private Mock<ILogger<JobSeekerServices>> _mockLogger;
        private ApplicationDBContext _dbContext;
        private JobSeekerServices _jobSeekerService;

        [SetUp]
        public void Setup()
        {
            // Setup In-memory database for ApplicationDBContext
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ApplicationDBContext(options);

            // Initialize the mocked dependencies
            _mockAuthorizationService = new Mock<IAuthorizationService>();
            _mockLogger = new Mock<ILogger<JobSeekerServices>>();

            // Initialize the service
            _jobSeekerService = new JobSeekerServices(_mockAuthorizationService.Object, _dbContext, _mockLogger.Object);
        }

        [Test]
        public async Task CreateJobSeekerAsync_ShouldCreateJobSeeker_WhenValidRequest()
        {
            // Arrange
            var jobSeeker = new JobSeeker
            {
                UserName = "johndoe123",
                JobSeekerName = "John Doe",
                Password = "password123",
                Email = "johndoe@example.com",
                Address = "123 Main Street, City, Country",
                Gender = "Male",
                ContactPhone = "123-456-7890",
                DateOfBirth = new DateTime(1990, 5, 20),
                Qualification = "Bachelor's in Computer Science",
                Description = "A Developer with a passion to develop great software.",
                Specialization = "Software Engineering",
                Institute = "Tech University",
                Year = 2020,
                CGPA = 3.8m,
                CompanyName = "TechCorp",
                Position = "Software Developer",
                StartDate = new DateTime(2021, 6, 1),
                EndDate = new DateTime(2023, 6, 1)
            };

            _mockAuthorizationService.Setup(x => x.HashPassword(It.IsAny<string>())).Returns("hashedPassword");

            // Act
            var response = await _jobSeekerService.CreateJobSeekerAsync(jobSeeker);

            // Assert
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("User Created Successfully."));

            // Verify the job seeker was added to the database
            var savedJobSeeker = await _dbContext.JobSeekers.FirstOrDefaultAsync(j => j.UserName == jobSeeker.UserName);
            Assert.That(savedJobSeeker, Is.Not.Null);
            Assert.That(savedJobSeeker.UserName, Is.EqualTo("johndoe123"));
        }

        [Test]
        public async Task CreateJobSeekerAsync_ShouldReturnFailure_WhenUserNameOrEmailExists()
        {
            // Arrange
            var jobSeeker1 = new JobSeeker
            {
                JobSeekerId = Guid.NewGuid().ToString(),
                UserName = "johndoe123",
                JobSeekerName = "John Doe",
                Password = "password123",
                Email = "johndoe@example.com",
                Address = "123 Main Street, City, Country",
                Gender = "Male",
                ContactPhone = "123-456-7890",
                DateOfBirth = new DateTime(1990, 5, 20),
                Qualification = "Bachelor's in Computer Science",
                Specialization = "Software Engineering",
                Description = "A Developer with a passion to develop great software.",
                Institute = "Tech University",
                Year = 2020,
                CGPA = 3.8m,
                CompanyName = "TechCorp",
                Position = "Software Developer",
                StartDate = new DateTime(2021, 6, 1),
                EndDate = new DateTime(2023, 6, 1)
            };
            await _dbContext.JobSeekers.AddAsync(jobSeeker1);
            await _dbContext.SaveChangesAsync();

            var jobSeeker2 = new JobSeeker
            {
                JobSeekerId = Guid.NewGuid().ToString(),
                UserName = "johndoe123", // Same username as jobSeeker1
                JobSeekerName = "Jane Doe",
                Password = "password456",
                Email = "janedoe@example.com",
                Address = "456 Secondary Street, City, Country",
                Gender = "Female",
                ContactPhone = "987-654-3210",
                DateOfBirth = new DateTime(1992, 10, 12),
                Qualification = "Master's in Computer Science",
                Description = "A Developer with a passion to develop great software.",
                Specialization = "AI & ML",
                Institute = "Tech University",
                Year = 2021,
                CGPA = 3.9m,
                CompanyName = "InnoTech",
                Position = "AI Engineer",
                StartDate = new DateTime(2022, 7, 1),
                EndDate = new DateTime(2024, 7, 1)
            };

            // Act
            var response = await _jobSeekerService.CreateJobSeekerAsync(jobSeeker2);

            // Assert
            Assert.That(response.Status, Is.EqualTo("Failure"));
            Assert.That(response.Message, Is.EqualTo("An Job Seeker with this username or email already exists."));
        }

        [Test]
        public async Task DeleteJobSeekerAsync_ShouldDeleteJobSeeker_WhenValidId()
        {
            // Arrange
            var jobSeeker = new JobSeeker
            {
                JobSeekerId = Guid.NewGuid().ToString(),
                UserName = "janedoe321",
                JobSeekerName = "Jane Doe",
                Password = "password456",
                Email = "janedoe@example.com",
                Address = "456 Secondary Street, City, Country",
                Gender = "Female",
                ContactPhone = "987-654-3210",
                DateOfBirth = new DateTime(1992, 10, 12),
                Description = "A Developer with a passion to develop great software.",
                Qualification = "Master's in Computer Science",
                Specialization = "AI & ML",
                Institute = "Tech University",
                Year = 2021,
                CGPA = 3.9m,
                CompanyName = "InnoTech",
                Position = "AI Engineer",
                StartDate = new DateTime(2022, 7, 1),
                EndDate = new DateTime(2024, 7, 1)
            };
            await _dbContext.JobSeekers.AddAsync(jobSeeker);
            await _dbContext.SaveChangesAsync();

            // Act
            var response = await _jobSeekerService.DeleteJobSeekerAsync(jobSeeker.JobSeekerId);

            // Assert
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Job Seeker deleted successfully."));

            var deletedJobSeeker = await _dbContext.JobSeekers.FirstOrDefaultAsync(j => j.JobSeekerId == jobSeeker.JobSeekerId);
            Assert.That(deletedJobSeeker, Is.Null);
        }

        [Test]
        public async Task DeleteJobSeekerAsync_ShouldReturnFailure_WhenJobSeekerNotFound()
        {
            // Act
            var response = await _jobSeekerService.DeleteJobSeekerAsync(Guid.NewGuid().ToString());

            // Assert
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Job Seeker not found with the given ID."));
        }

        [Test]
        public async Task GetAllJobSeekersAsync_ShouldReturnJobSeekers()
        {
            // Arrange
            var jobSeeker1 = new JobSeeker
            {
                JobSeekerId = Guid.NewGuid().ToString(),
                UserName = "user1",
                JobSeekerName = "User One",
                Email = "user1@example.com",
                Password = "password456",
                Address = "Address 1",
                Gender = "Male",
                ContactPhone = "123-555-0001",
                DateOfBirth = new DateTime(1995, 4, 10),
                Qualification = "Bachelor's in IT",
                Specialization = "Web Development",
                Description = "A Developer with a passion to develop great software.",
                Institute = "Tech College",
                Year = 2018,
                CGPA = 3.7m,
                CompanyName = "WebCo",
                Position = "Frontend Developer",
                StartDate = new DateTime(2019, 3, 1),
                EndDate = new DateTime(2022, 3, 1)
            };
            var jobSeeker2 = new JobSeeker
            {
                JobSeekerId = Guid.NewGuid().ToString(),
                UserName = "user2",
                JobSeekerName = "User Two",
                Email = "user2@example.com",
                Password = "password456",
                Address = "Address 2",
                Gender = "Female",
                ContactPhone = "123-555-0002",
                DateOfBirth = new DateTime(1997, 7, 20),
                Qualification = "Master's in Software Engineering",
                Specialization = "Backend Development",
                Description = "A Developer with a passion to develop great software.",
                Institute = "Engineering University",
                Year = 2020,
                CGPA = 3.9m,
                CompanyName = "CodeWorks",
                Position = "Backend Developer",
                StartDate = new DateTime(2021, 6, 1),
                EndDate = new DateTime(2023, 6, 1)
            };
            await _dbContext.JobSeekers.AddRangeAsync(jobSeeker1, jobSeeker2);
            await _dbContext.SaveChangesAsync();

            // Act
            var jobSeekers = await _jobSeekerService.GetAllJobSeekersAsync();

            // Assert
            Assert.That(jobSeekers.Count, Is.EqualTo(2));
            Assert.That(jobSeekers[0].UserName, Is.EqualTo("user1"));
            Assert.That(jobSeekers[1].UserName, Is.EqualTo("user2"));
        }

        [Test]
        public async Task GetJobSeekerByUserNameAsync_ShouldReturnJobSeeker_WhenExists()
        {
            // Arrange
            var jobSeeker = new JobSeeker
            {
                JobSeekerId = Guid.NewGuid().ToString(),
                UserName = "testuser",
                JobSeekerName = "Test User",
                Email = "testuser@example.com",
                Password = "password456",
                Address = "Address 2",
                Gender = "Female",
                ContactPhone = "123-555-0002",
                DateOfBirth = new DateTime(1997, 7, 20),
                Qualification = "Master's in Software Engineering",
                Specialization = "Backend Development",
                Description = "A Developer with a passion to develop great software.",
                Institute = "Engineering University",
                Year = 2020,
                CGPA = 3.9m,
                CompanyName = "CodeWorks",
                Position = "Backend Developer",
                StartDate = new DateTime(2021, 6, 1),
                EndDate = new DateTime(2023, 6, 1)
            };
            await _dbContext.JobSeekers.AddAsync(jobSeeker);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _jobSeekerService.GetJobSeekerByUserNameAsync("testuser");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserName, Is.EqualTo("testuser"));
        }

        [Test]
        public async Task GetJobSeekerByUserNameAsync_ShouldReturnNull_WhenNotFound()
        {
            // Act
            var result = await _jobSeekerService.GetJobSeekerByUserNameAsync("nonexistentuser");

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task UpdateJobSeekerAsync_ShouldUpdateJobSeeker_WhenValidRequest()
        {
            // Arrange
            var jobSeeker = new JobSeeker
            {
                JobSeekerId = Guid.NewGuid().ToString(),
                UserName = "testuser",
                JobSeekerName = "Old Name",
                Email = "oldemail@example.com",
                Password = "password456",
                Address = "Address 2",
                Gender = "Female",
                ContactPhone = "123-555-0002",
                DateOfBirth = new DateTime(1997, 7, 20),
                Qualification = "Master's in Software Engineering",
                Description = "A Developer with a passion to develop great software.",
                Specialization = "Backend Development",
                Institute = "Engineering University",
                Year = 2020,
                CGPA = 3.9m,
                CompanyName = "CodeWorks",
                Position = "Backend Developer",
                StartDate = new DateTime(2021, 6, 1),
                EndDate = new DateTime(2023, 6, 1)
            };
            await _dbContext.JobSeekers.AddAsync(jobSeeker);
            await _dbContext.SaveChangesAsync();

            var updatedJobSeeker = new JobSeeker
            {
                JobSeekerName = "New Name",
                Email = "newemail@example.com",
                Password = "password456",
                Address = "Address 1",
                Gender = "Male",
                ContactPhone = "123-555-0001",
                DateOfBirth = new DateTime(1995, 4, 10),
                Qualification = "Bachelor's in IT",
                Specialization = "Web Development",
                Description = "A Developer with a passion to develop great software.",
                Institute = "Tech College",
                Year = 2018,
                CGPA = 3.7m,
                CompanyName = "WebCo",
                Position = "Frontend Developer",
                StartDate = new DateTime(2019, 3, 1),
                EndDate = new DateTime(2022, 3, 1)
            };

            // Act
            var response = await _jobSeekerService.UpdateJobSeekerAsync(jobSeeker.JobSeekerId, updatedJobSeeker);

            // Assert
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Job Seeker updated successfully."));

            var jobSeekerInDb = await _dbContext.JobSeekers.FirstOrDefaultAsync(j => j.JobSeekerId == jobSeeker.JobSeekerId);
            Assert.That(jobSeekerInDb.JobSeekerName, Is.EqualTo("New Name"));
            Assert.That(jobSeekerInDb.Email, Is.EqualTo("newemail@example.com"));
        }

        [Test]
        public async Task UpdateJobSeekerAsync_ShouldReturnFailure_WhenJobSeekerNotFound()
        {
            // Act
            var response = await _jobSeekerService.UpdateJobSeekerAsync(Guid.NewGuid().ToString(), new JobSeeker());

            // Assert
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Job Seeker not found with the given ID."));
        }
    }
}

