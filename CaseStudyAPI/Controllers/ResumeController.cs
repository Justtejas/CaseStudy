﻿using CaseStudyAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CaseStudyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResumeController:ControllerBase
    {
        private readonly IResumeServices _resumeServices;
        private readonly ILogger<ResumeController> _logger;

        public ResumeController(IResumeServices resumeServices, ILogger<ResumeController> logger)
        {
            _resumeServices = resumeServices;
            _logger = logger;
        }

        [Authorize(Roles = "JobSeeker")]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return BadRequest("JobSeekerId not found");
            }
            try
            {
                var response = await _resumeServices.CreateResumeAsync(userId, file);
                return Ok(response);
            }
            catch (InvalidOperationException ioex)
            {
                _logger.LogError(ioex, message: ioex.Message);
                return BadRequest(new { Error = ioex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,$"Error uploading the file {file.FileName}");
                return StatusCode(500, new { Error = "An unexpected error occurred. Please try again later." });
            }
        }

        [Authorize(Roles = "Employer")]
        [HttpGet("download/{jobSeekerId}/{resumeId}")]
        public async Task<IActionResult> DownloadFile(string jobSeekerId,string resumeId)
        {
            var file = await _resumeServices.GetResumeAsync(resumeId, jobSeekerId);
            if (file == null)
            {
                return NotFound("File not found");
            }
            return File(file.FileData, file.FileType, file.FileName);
        }

        [Authorize(Roles = "JobSeeker")]
        [HttpDelete("delete/{resumeId}")]
        public async Task<IActionResult> DeleteFile(string resumeId)
        {
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userID == null)
            {
                return NotFound("JobSeeker Not Found");
            }
            var status = await _resumeServices.DeleteResumeAsync(resumeId, userID);
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
        [HttpPut("update/{resumeId}")]
        public async Task<IActionResult> UpdateFile(string resumeId, IFormFile newFile) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return BadRequest("UserID not found");
            }
            try
            {
                var updatedFileName = await _resumeServices.UpdateResumeAsync(resumeId, userId, newFile);
                return Ok($"Updated file to {newFile.FileName}");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
