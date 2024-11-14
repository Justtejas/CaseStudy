using CaseStudyAPI.Data;
using CaseStudyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CaseStudyAPI.Repository
{
    public class ResumeServices : IResumeServices
    {
        private const long FILE_SIZE_LIMIT = 5 * 1024 * 1024;
        private readonly ApplicationDBContext _context;
        public ResumeServices(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<Response> CreateResumeAsync(string jobSeekerId,IFormFile resume)
        {
             if (resume.ContentType != "application/pdf")
                throw new InvalidOperationException("Only PDF resumes are allowed.");

            if (resume.Length > FILE_SIZE_LIMIT)
                throw new InvalidOperationException("resume size cannot exceed 5 MB.");

            using var dataStream = new MemoryStream();
            await resume.CopyToAsync(dataStream);

            var resumeModel = new Resume
            {
                ResumeId = Guid.NewGuid().ToString(),
                FileName = resume.FileName,
                FileType = resume.ContentType,
                FileSize = resume.Length,
                UploadDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                FileData = dataStream.ToArray(),
                JobSeekerId = jobSeekerId
            };

            await _context.Resumes.AddAsync(resumeModel);
            await _context.SaveChangesAsync();
            return new Response
            {
                Status = "Success",
                Message = $"Uploaded resume {resume.FileName}"
            };
        }

        public async Task<bool> DeleteResumeAsync(string resumeId, string jobSeekerId)
        {
              var resume = await _context.Resumes.Where(r => r.ResumeId == resumeId && r.JobSeekerId == jobSeekerId).SingleOrDefaultAsync();
            if (resume != null)
            {
                _context.Remove(resume);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<Resume> GetResumeAsync(string resumeId, string jobSeekerId)
        {
            var resume = await _context.Resumes.Where(r => r.ResumeId == resumeId && r.JobSeekerId == jobSeekerId).SingleOrDefaultAsync();
            if (resume == null)
            {
                return null;
            }
            return resume;
        }

        public async Task<string> UpdateResumeAsync(string resumeId, string jobSeekerId, IFormFile newFile)
        {
            var existingResume = await _context.Resumes
           .Where(r => r.ResumeId == resumeId && r.JobSeekerId == jobSeekerId)
           .SingleOrDefaultAsync() ?? throw new InvalidOperationException("File not found or you do not have permission to update it.");
            if (newFile.ContentType != "application/pdf")
            {
                throw new InvalidOperationException("Only PDF files are allowed.");
            }

            if (newFile.Length > FILE_SIZE_LIMIT)
            {
                throw new InvalidOperationException("File size cannot exceed 5 MB.");
            }

            using var dataStream = new MemoryStream();
            await newFile.CopyToAsync(dataStream);

            existingResume.FileName = newFile.FileName;
            existingResume.FileType = newFile.ContentType;
            existingResume.FileSize = newFile.Length;
            existingResume.FileData = dataStream.ToArray();
            existingResume.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return existingResume.FileName;
        }
    }
}
