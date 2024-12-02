using CaseStudyAPI.Data;
using CaseStudyAPI.Models;
using CaseStudyAPI.Repository.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CaseStudyTest
{
    [TestFixture]
    public class ResumeServicesTests
    {
        private ApplicationDBContext _dbContext;
        private ResumeServices _resumeServices;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ApplicationDBContext(options);
            _resumeServices = new ResumeServices(_dbContext);
        }

        [Test]
        public async Task CreateResumeAsync_ShouldCreateResume_WhenValidInputProvided()
        {
            var mockFile = new Mock<IFormFile>();
            var content = "Fake PDF content";
            var fileName = "resume.pdf";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.Length).Returns(1024);
            mockFile.Setup(f => f.ContentType).Returns("application/pdf");

            var jobSeekerId = Guid.NewGuid().ToString();

            var response = await _resumeServices.CreateResumeAsync(jobSeekerId, mockFile.Object);

            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo($"Uploaded resume {fileName}"));
            Assert.That(await _dbContext.Resumes.AnyAsync(r => r.JobSeekerId == jobSeekerId), Is.True);
        }

        [Test]
        public void CreateResumeAsync_ShouldThrowException_WhenFileIsNotPDF()
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.ContentType).Returns("application/msword");

            var jobSeekerId = Guid.NewGuid().ToString();

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _resumeServices.CreateResumeAsync(jobSeekerId, mockFile.Object));
            Assert.That(ex.Message, Is.EqualTo("Only PDF resumes are allowed."));
        }

        [Test]
        public void CreateResumeAsync_ShouldThrowException_WhenFileExceedsSizeLimit()
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(6 * 1024 * 1024);
            mockFile.Setup(f => f.ContentType).Returns("application/pdf");

            var jobSeekerId = Guid.NewGuid().ToString();

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _resumeServices.CreateResumeAsync(jobSeekerId, mockFile.Object));
            Assert.That(ex.Message, Is.EqualTo("resume size cannot exceed 5 MB."));
        }

        [Test]
        public async Task DeleteResumeAsync_ShouldDeleteResume_WhenResumeExists()
        {
            var jobSeekerId = Guid.NewGuid().ToString();
            var resume = new Resume
            {
                ResumeId = Guid.NewGuid().ToString(),
                JobSeekerId = jobSeekerId,
                FileName = "test_resume.pdf",
                FileType = "application/pdf",
                FileSize = 1024,
                FileData = Array.Empty<byte>()
            };

            await _dbContext.Resumes.AddAsync(resume);
            await _dbContext.SaveChangesAsync();

            var result = await _resumeServices.DeleteResumeAsync(resume.ResumeId, jobSeekerId);

            Assert.That(result, Is.True);
            Assert.That(await _dbContext.Resumes.AnyAsync(r => r.ResumeId == resume.ResumeId), Is.False);
        }

        [Test]
        public void DeleteResumeAsync_ShouldThrowException_WhenResumeDoesNotExist()
        {
            var resumeId = Guid.NewGuid().ToString();
            var jobSeekerId = Guid.NewGuid().ToString();

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _resumeServices.DeleteResumeAsync(resumeId, jobSeekerId));
            Assert.That(ex.Message, Is.EqualTo("File not found or you do not have permission to delete it."));
        }

        [Test]
        public async Task GetResumeAsync_ShouldReturnResume_WhenResumeExists()
        {
            var jobSeekerId = Guid.NewGuid().ToString();
            var resumeId = Guid.NewGuid().ToString();
            var resume = new Resume
            {
                ResumeId = resumeId,
                JobSeekerId = jobSeekerId,
                FileName = "test_resume.pdf",
                FileType = "application/pdf",
                FileSize = 1024,
                FileData = Array.Empty<byte>()
            };

            await _dbContext.Resumes.AddAsync(resume);
            await _dbContext.SaveChangesAsync();

            var result = await _resumeServices.GetResumeAsync(resumeId, jobSeekerId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.FileName, Is.EqualTo("test_resume.pdf"));
        }

        [Test]
        public async Task GetResumeAsync_ShouldReturnNull_WhenResumeDoesNotExist()
        {
            var resumeId = Guid.NewGuid().ToString();
            var jobSeekerId = Guid.NewGuid().ToString();

            var result = await _resumeServices.GetResumeAsync(resumeId, jobSeekerId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task UpdateResumeAsync_ShouldUpdateResume_WhenValidDataProvided()
        {
            var jobSeekerId = Guid.NewGuid().ToString();
            var resumeId = Guid.NewGuid().ToString();
            var oldResume = new Resume
            {
                ResumeId = resumeId,
                JobSeekerId = jobSeekerId,
                FileName = "old_resume.pdf",
                FileType = "application/pdf",
                FileSize = 1024,
                FileData = Array.Empty<byte>()
            };

            await _dbContext.Resumes.AddAsync(oldResume);
            await _dbContext.SaveChangesAsync();

            var mockNewFile = new Mock<IFormFile>();
            var content = "Updated PDF content";
            var newFileName = "updated_resume.pdf";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            mockNewFile.Setup(f => f.OpenReadStream()).Returns(stream);
            mockNewFile.Setup(f => f.FileName).Returns(newFileName);
            mockNewFile.Setup(f => f.Length).Returns(2048);
            mockNewFile.Setup(f => f.ContentType).Returns("application/pdf");

            var updatedFileName = await _resumeServices.UpdateResumeAsync(resumeId, jobSeekerId, mockNewFile.Object);

            Assert.That(updatedFileName, Is.EqualTo(newFileName));

            var updatedResume = await _dbContext.Resumes.FindAsync(resumeId);
            Assert.That(updatedResume, Is.Not.Null);
            Assert.That(updatedResume.FileName, Is.EqualTo(newFileName));
        }

        [Test]
        public async Task UpdateResumeAsync_ShouldThrowException_WhenFileIsNotPDF()
        {
            var jobSeekerId = Guid.NewGuid().ToString();
            var resumeId = Guid.NewGuid().ToString();
            var existingResume = new Resume
            {
                ResumeId = resumeId,
                JobSeekerId = jobSeekerId,
                FileName = "old_resume.pdf",
                FileType = "application/pdf",
                FileSize = 1024,
                FileData = Array.Empty<byte>()
            };

            await _dbContext.Resumes.AddAsync(existingResume);
            await _dbContext.SaveChangesAsync();

            var mockInvalidFile = new Mock<IFormFile>();
            mockInvalidFile.Setup(f => f.ContentType).Returns("application/msword");

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _resumeServices.UpdateResumeAsync(resumeId, jobSeekerId, mockInvalidFile.Object));

            Assert.That(ex.Message, Is.EqualTo("Only PDF files are allowed."));
        }
    }
}
