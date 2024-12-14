using CaseStudyAPI.Data;
using CaseStudyAPI.DTO;
using CaseStudyAPI.Models;
using CaseStudyAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CaseStudyAPI.Repository.Services
{
    public class JobSeekerServices : IJobSeekerServices
    {

        private readonly IAuthorizationService _authorizationServices;
        private readonly ApplicationDBContext _appDBContext;
        private readonly ILogger<JobSeekerServices> _logger;
        public JobSeekerServices(IAuthorizationService authServices, ApplicationDBContext appDBContext, ILogger<JobSeekerServices> logger)
        {
            _authorizationServices = authServices;
            _appDBContext = appDBContext;
            _logger = logger;
        }
        public async Task<Response> CreateJobSeekerAsync(JobSeeker jobseeker)
        {
            try
            {
                jobseeker.Password = _authorizationServices.HashPassword(jobseeker.Password);
                jobseeker.JobSeekerId = Guid.NewGuid().ToString();
                var jobSeekerExists = await _appDBContext.JobSeekers.FirstOrDefaultAsync(j => j.UserName == jobseeker.UserName || j.Email == jobseeker.Email);
                if (jobSeekerExists != null)
                {
                    return new Response { Status = "Failure", Message = "An Job Seeker with this username or email already exists." };
                }
                if (jobseeker.Role != null)
                {
                    return new Response { Status = "Failure", Message = "Invalid Request Body." };
                }
                await _appDBContext.JobSeekers.AddAsync(jobseeker);
                await _appDBContext.SaveChangesAsync();
                return new Response { Status = "Success", Message = "User Created Successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new Response { Status = "Failure", Message = "Invalid Request Body." };
            }
        }

        public async Task<Response> DeleteJobSeekerAsync(string id)
        {
            try
            {
                var jobseeker = await _appDBContext.JobSeekers.FirstOrDefaultAsync(jobseeker => jobseeker.JobSeekerId == id);
                if (jobseeker == null)
                {
                    return new Response
                    {
                        Status = "Success",
                        Message = "Job Seeker not found with the given ID."
                    };
                }
                _appDBContext.JobSeekers.Remove(jobseeker);
                await _appDBContext.SaveChangesAsync();
                return new Response
                {
                    Status = "Success",
                    Message = "Job Seeker deleted successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new Response
                {
                    Status = "Failure",
                    Message = $"An error occurred while deleting the Job Seeker: {ex.Message}"
                };
            }
        }

        public async Task<List<JobSeeker>> GetAllJobSeekersAsync()
        {
            var jobSeekers = await _appDBContext.JobSeekers.ToListAsync();
            if (jobSeekers == null || !jobSeekers.Any())
            {
                return new List<JobSeeker> { };
            }
            return jobSeekers;

        }

        public async Task<JobSeeker> GetJobSeekerByJobSeekerIdAsync(string jobSeekerId)
        {
            var existingJobSeeker = await _appDBContext.JobSeekers.FirstOrDefaultAsync(j => j.JobSeekerId == jobSeekerId);
            if (existingJobSeeker == null)
            {
                return null;
            }
            return existingJobSeeker;
        }

        public async Task<JobSeeker> GetJobSeekerByUserNameAsync(string userName)
        {
            var existingJobSeeker = await _appDBContext.JobSeekers.FirstOrDefaultAsync(j => j.UserName == userName);
            if (existingJobSeeker == null)
            {
                return null;
            }
            return existingJobSeeker;
        }

        public async Task<Response> UpdateJobSeekerAsync(string id, UpdateJobSeekerDTO jobseekerModel)
        {
            try
            {
                if (jobseekerModel == null)
                {
                    return new Response
                    {
                        Status = "Failure",
                        Message = "Invalid request body."
                    };
                }

                var jobseeker = await _appDBContext.JobSeekers.FirstOrDefaultAsync(j => j.JobSeekerId == id);
                if (jobseeker == null)
                {
                    return new Response
                    {
                        Status = "Failure",
                        Message = "Job Seeker not found with the given ID."
                    };
                }

                var jobSeekerExists = await _appDBContext.JobSeekers
                    .FirstOrDefaultAsync(j => (j.UserName == jobseekerModel.UserName || j.Email == jobseekerModel.Email)
                                               && j.JobSeekerId != id);
                if (jobSeekerExists != null)
                {
                    return new Response
                    {
                        Status = "Failure",
                        Message = "A Job Seeker with this username or email already exists."
                    };
                }

                if (!string.IsNullOrEmpty(jobseekerModel.UserName)) jobseeker.UserName = jobseekerModel.UserName;
                if (!string.IsNullOrEmpty(jobseekerModel.JobSeekerName)) jobseeker.JobSeekerName = jobseekerModel.JobSeekerName;
                if (!string.IsNullOrEmpty(jobseekerModel.Address)) jobseeker.Address = jobseekerModel.Address;
                if (jobseekerModel.Gender != null) jobseeker.Gender = jobseekerModel.Gender;
                if (jobseekerModel.DateOfBirth != null) jobseeker.DateOfBirth = jobseekerModel.DateOfBirth;
                if (jobseekerModel.CGPA > 0.0m && jobseekerModel.CGPA <= 10.0m)  jobseeker.CGPA = jobseekerModel.CGPA;
                if (!string.IsNullOrEmpty(jobseekerModel.ContactPhone)) jobseeker.ContactPhone = jobseekerModel.ContactPhone;
                if (!string.IsNullOrEmpty(jobseekerModel.CompanyName)) jobseeker.CompanyName = jobseekerModel.CompanyName;
                if (!string.IsNullOrEmpty(jobseekerModel.Specialization)) jobseeker.Specialization = jobseekerModel.Specialization;
                if (!string.IsNullOrEmpty(jobseekerModel.Institute)) jobseeker.Institute = jobseekerModel.Institute;
                if (!string.IsNullOrEmpty(jobseekerModel.Qualification)) jobseeker.Qualification = jobseekerModel.Qualification;
                if (!string.IsNullOrEmpty(jobseekerModel.Description)) jobseeker.Description = jobseekerModel.Description;
                if (!string.IsNullOrEmpty(jobseekerModel.Position)) jobseeker.Position = jobseekerModel.Position;
                if (jobseekerModel.Year > 1900 && jobseekerModel.Year < 2100) jobseeker.Year = jobseekerModel.Year;
                if (jobseekerModel.StartDate != null) jobseeker.StartDate = jobseekerModel.StartDate;
                if (jobseekerModel.EndDate != null) jobseeker.EndDate = jobseekerModel.EndDate;
                if (!string.IsNullOrEmpty(jobseekerModel.Email)) jobseeker.Email = jobseekerModel.Email;

                await _appDBContext.SaveChangesAsync();

                return new Response
                {
                    Status = "Success",
                    Message = "Job Seeker updated successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating Job Seeker with ID {id}: {ex}");
                return new Response
                {
                    Status = "Failure",
                    Message = $"An error occurred while updating the Job Seeker: {ex.Message}"
                };
            }
        }
    }
}
