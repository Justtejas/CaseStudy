using CaseStudyAPI.Data;
using CaseStudyAPI.Models;
using CaseStudyAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CaseStudyAPI.Repository.Services
{
    public class ResumeServices : IResumeServices
    {
        private const long FILE_SIZE_LIMIT = 5 * 1024 * 1024;
        private readonly ApplicationDBContext _context;
        public ResumeServices(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<Response> CreateResumeAsync(string jobSeekerId, IFormFile resume)
        {
            if (resume.ContentType != "application/pdf")
                throw new InvalidOperationException("Only PDF resumes are allowed.");

            if (resume.Length > FILE_SIZE_LIMIT)
                throw new InvalidOperationException("Resume size cannot exceed 5 MB.");

            using var dataStream = new MemoryStream();
            await resume.CopyToAsync(dataStream);

            var existingResume = await _context.Resumes.FirstOrDefaultAsync(r => r.JobSeekerId == jobSeekerId);

            if (existingResume != null)
            {
                existingResume.FileName = resume.FileName;
                existingResume.FileType = resume.ContentType;
                existingResume.FileSize = resume.Length;
                existingResume.ModifiedDate = DateTime.Now;
                existingResume.FileData = dataStream.ToArray();
            }
            else
            {
                var newResume = new Resume
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

                await _context.Resumes.AddAsync(newResume);
            }

            await _context.SaveChangesAsync();

            return new Response
            {
                Status = "Success",
                Message = existingResume != null
                    ? $"Updated resume {resume.FileName}"
                    : $"Uploaded resume {resume.FileName}"
            };
        }

        public async Task<bool> DeleteResumeAsync(string resumeId, string jobSeekerId)
        {
            var resume = await _context.Resumes.Where(r => r.ResumeId == resumeId && r.JobSeekerId == jobSeekerId).SingleOrDefaultAsync() ?? throw new InvalidOperationException("File not found or you do not have permission to delete it."); ;
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

        public async Task<Resume> GetResumeAsync(string jobSeekerId)
        {
            var resume = await _context.Resumes.Where(r => r.JobSeekerId == jobSeekerId).SingleOrDefaultAsync();
            if (resume == null)
            {
                return null;
            }
            return resume;
        }

    }
}
