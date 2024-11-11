using CaseStudyAPI.Authentication;
using CaseStudyAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CaseStudyAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileServices _fileService;
        private readonly UserManager<ApplicationUser> _userManager;

        public FileController(IFileServices fileService, UserManager<ApplicationUser> userManager)
        {
            _fileService = fileService;
            _userManager = userManager;
        }

        [Authorize(Roles = "JobSeeker")]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return BadRequest("UserID not found");
            }
            try
            {
                var fileName = await _fileService.AddFile(userId, file);
                return Ok($"Uploaded File {new { FileName = fileName }}");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Error = "An unexpected error occurred. Please try again later." });
            }
        }

        [Authorize(Roles = "Employer")]
        [HttpGet("download/{jobSeekerId}/{fileId}")]
        public async Task<IActionResult> DownloadFile(string jobSeekerId,int fileId)
        {
            var file = await _fileService.GetFile(fileId, jobSeekerId);
            if (file == null)
            {
                return NotFound("File not found");
            }
            return File(file.FileData, file.FileType, file.FileName);
        }
        [Authorize(Roles = "JobSeeker")]
        [HttpDelete("delete/{fileID}")]
        public async Task<IActionResult> DeleteFile(int fileID)
        {
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userID == null)
            {
                return NotFound("User Not Found");
            }
            var status = await _fileService.DeleteFile(fileID, userID);
            if (status)
            {
                return Ok("File Deleted Successfully");
            }
            else
            {
                return NotFound("File ID not found");
            }
        }

        [Authorize(Roles = "JobSeeker")]
        [HttpPut("update/{fileId}")]
        public async Task<IActionResult> UpdateFile(int fileId, IFormFile newFile)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return BadRequest("UserID not found");
            }
            try
            {
                var updatedFileName = await _fileService.UpdateFile(fileId, userId, newFile);
                return Ok($"Updated file to {new { FileName = updatedFileName }}");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
