using CaseStudyAPI.Data;
using CaseStudyAPI.Models;
using CaseStudyAPI.Repository.Interfaces;
using CaseStudyAPI.Repository.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace CaseStudyTest
{
    [TestFixture]
    public class EmployerServicesTest
    {
        private Mock<IAuthorizationService> _mockAuthorizationService;
        private Mock<ILogger<EmployerServices>> _mockLogger;
        private ApplicationDBContext _dbContext;
        private EmployerServices _employerServices;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ApplicationDBContext(options);

            _mockAuthorizationService = new Mock<IAuthorizationService>();
            _mockLogger = new Mock<ILogger<EmployerServices>>();

            _employerServices = new EmployerServices(_mockAuthorizationService.Object, _dbContext, _mockLogger.Object);
        }

        [Test]
        public async Task CreateEmployerAsync_ShouldReturnSuccess_WhenEmployerCreatedSuccessfully()
        {
            var employer = new Employer
            {
                EmployerId = Guid.NewGuid().ToString(),
                UserName = "newuser",
                EmployerName = "New User",
                Email = "newuser@example.com",
                Password = "password123",
                Gender = "Male",
                CompanyName = "TechInnovations",
                ContactPhone = "+910987654321"
            };

            _mockAuthorizationService.Setup(a => a.HashPassword(It.IsAny<string>())).Returns("hashedpassword");

            var response = await _employerServices.CreateEmployerAsync(employer);

            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Employer Created Successfully"));

            var createdEmployer = await _dbContext.Employers.FirstOrDefaultAsync(e => e.UserName == employer.UserName);
            Assert.That(createdEmployer, Is.Not.Null);
            Assert.That(createdEmployer.Password, Is.EqualTo("hashedpassword"));
        }

        [Test]
        public async Task CreateEmployerAsync_ShouldReturnFailure_WhenEmployerAlreadyExists()
        {
            var existingEmployer = new Employer
            {
                EmployerId = Guid.NewGuid().ToString(),
                UserName = "existinguser",
                EmployerName = "Existing User",
                Email = "existing@example.com",
                Password = "password123",
                Gender = "Male",
                CompanyName = "TechInnovations",
                ContactPhone = "+910987654321"
            };
            await _dbContext.Employers.AddAsync(existingEmployer);
            await _dbContext.SaveChangesAsync();

            var newEmployer = new Employer
            {
                EmployerId = Guid.NewGuid().ToString(),
                UserName = "existinguser",
                EmployerName = "Existing User",
                Email = "new@example.com",
                Password = "password123",
                Gender = "Male",
                CompanyName = "TechInnovations",
                ContactPhone = "+910987654321"
            };

            var response = await _employerServices.CreateEmployerAsync(newEmployer);

            Assert.That(response.Status, Is.EqualTo("Failure"));
            Assert.That(response.Message, Is.EqualTo("An employer with this username or email already exists."));
        }
        
        [Test]
        public async Task CreateEmployerAsync_ShouldReturnFailure_WhenRoleIsNotNull()
        {
            var employer = new Employer
            {
                EmployerId = Guid.NewGuid().ToString(),
                UserName = "existinguser",
                EmployerName = "Existing User",
                Email = "new@example.com",
                Password = "password123",
                Gender = "Male",
                CompanyName = "TechInnovations",
                ContactPhone = "+910987654321",
                Role = "Admin"
            };

            var response = await _employerServices.CreateEmployerAsync(employer);

            Assert.That(response.Status, Is.EqualTo("Failure"));
            Assert.That(response.Message, Is.EqualTo("Invalid Request Body"));
        }

        [Test]
        public async Task DeleteEmployerAsync_ShouldReturnSuccess_WhenEmployerDeleted()
        {
            var employer = new Employer
            {
                EmployerId = Guid.NewGuid().ToString(),
                UserName = "userToDelete",
                EmployerName = "User To Delete",
                Email = "new@example.com",
                Password = "password123",
                Gender = "Male",
                CompanyName = "TechInnovations",
                ContactPhone = "+910987654321",
            };
            await _dbContext.Employers.AddAsync(employer);
            await _dbContext.SaveChangesAsync();

            var response = await _employerServices.DeleteEmployerAsync(employer.EmployerId);

            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Employer deleted successfully."));

            var deletedEmployer = await _dbContext.Employers.FirstOrDefaultAsync(e => e.EmployerId == employer.EmployerId);
            Assert.That(deletedEmployer, Is.Null);
        }

        [Test]
        public async Task DeleteEmployerAsync_ShouldReturnFailure_WhenEmployerNotFound()
        {
            var employerId = Guid.NewGuid().ToString();

            var response = await _employerServices.DeleteEmployerAsync(employerId);

            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Employer not found with the given ID."));
        }

        [Test]
        public async Task GetEmployerByUserName_ShouldReturnNull_WhenEmployerDoesNotExist()
        {
            var result = await _employerServices.GetEmployerByUserName("nonexistentuser");

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetEmployerByUserName_ShouldReturnEmployer_WhenEmployerExists()
        {
            var employer = new Employer
            {
                EmployerId = Guid.NewGuid().ToString(),
                UserName = "existinguser",
                EmployerName = "Existing User",
                Email = "existing@example.com",
                Password = "password123",
                Gender = "Male",
                CompanyName = "TechInnovations",
                ContactPhone = "+910987654321",
            };
            await _dbContext.Employers.AddAsync(employer);
            await _dbContext.SaveChangesAsync();

            var result = await _employerServices.GetEmployerByUserName("existinguser");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserName, Is.EqualTo("existinguser"));
        }
    }
}
