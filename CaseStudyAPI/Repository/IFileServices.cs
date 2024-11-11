using CaseStudyAPI.Models;

namespace CaseStudyAPI.Repository
{
    public interface IFileServices
    {
        public Task<string> AddFile(string userId, IFormFile file);
        public Task<FileModel> GetFile(int fileId, string userId);
        public Task<bool> DeleteFile(int fileID, string userID);
        public Task<string> UpdateFile(int fileId, string userId, IFormFile newFile);
    }
}
