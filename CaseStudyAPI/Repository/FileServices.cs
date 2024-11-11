using CaseStudyAPI.Data;
using CaseStudyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CaseStudyAPI.Repository
{
    public class FileServices : IFileServices
    {
        private const long FILE_SIZE_LIMIT = 5 * 1024 * 1024;
        private readonly ApplicationDBContext _context;
        public FileServices(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<string> AddFile(string userId, IFormFile file)
        {
            if (file.ContentType != "application/pdf")
                throw new InvalidOperationException("Only PDF files are allowed.");

            if (file.Length > FILE_SIZE_LIMIT)
                throw new InvalidOperationException("File size cannot exceed 5 MB.");

            using var dataStream = new MemoryStream();
            await file.CopyToAsync(dataStream);

            var fileModel = new FileModel
            {
                FileName = file.FileName,
                FileType = file.ContentType,
                FileSize = file.Length,
                UploadDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                FileData = dataStream.ToArray(),
                UserId = userId
            };

            await _context.Files.AddAsync(fileModel);
            await _context.SaveChangesAsync();

            return fileModel.FileName;
        }

        public async Task<bool> DeleteFile(int fileID, string userID)
        {
            var file = await _context.Files.Where(f => f.Id == fileID && f.UserId == userID).SingleOrDefaultAsync();
            if (file != null)
            {
                _context.Remove(file);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<FileModel> GetFile(int fileId, string userId)
        {
            var file = await _context.Files
           .Where(f => f.Id == fileId && f.UserId == userId)
           .SingleOrDefaultAsync();

            if (file == null)
            {
                return null;
            }
            return file;
        }

        public async Task<string> UpdateFile(int fileId, string userId, IFormFile newFile)
        {
            var existingFile = await _context.Files
        .Where(f => f.Id == fileId && f.UserId == userId)
        .SingleOrDefaultAsync();

            if (existingFile == null)
            {
                throw new InvalidOperationException("File not found or you do not have permission to update it.");
            }

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

            existingFile.FileName = newFile.FileName;
            existingFile.FileType = newFile.ContentType;
            existingFile.FileSize = newFile.Length;
            existingFile.FileData = dataStream.ToArray();

            await _context.SaveChangesAsync();
            return existingFile.FileName;
        }
    }
}
