using CaseStudyAPI.Data;
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
        public JobSeekerServices( IAuthorizationService authServices, ApplicationDBContext appDBContext, ILogger<JobSeekerServices> logger)
        {
            _authorizationServices = authServices;
            _appDBContext = appDBContext;
            _logger = logger;
        }
        public async Task<Response> CreateJobSeekerAsync(JobSeeker jobseeker)
        {
            try
            {
                jobseeker.Password =  _authorizationServices.HashPassword(jobseeker.Password);
                jobseeker.JobSeekerId = Guid.NewGuid().ToString();
                var jobSeekerExists = await _appDBContext.JobSeekers.FirstOrDefaultAsync(j => j.UserName == jobseeker.UserName || j.Email == jobseeker.Email);
                if (jobSeekerExists != null)
                {
                    return new Response { Status = "Failure", Message = "An Job Seeker with this username or email already exists." };
                }
                if(jobseeker.Role != null)
                {
                    return new Response { Status = "Failure", Message = "Invalid Request Body."};
                }
                await _appDBContext.JobSeekers.AddAsync(jobseeker);
                await _appDBContext.SaveChangesAsync();
                return new Response { Status = "Success", Message = "User Created Successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,ex.Message);
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

        public async Task<JobSeeker> GetJobSeekerByUserNameAsync(string userName)
        {
            var existingJobSeeker = await _appDBContext.JobSeekers.FirstOrDefaultAsync(j => j.UserName == userName);
            if (existingJobSeeker == null)
            {
                return null;
            }
            return existingJobSeeker;
        }

        public async Task<Response> UpdateJobSeekerAsync(string id, JobSeeker jobseekerModel)
        {
            try { 
                var jobseeker = await _appDBContext.JobSeekers.FirstOrDefaultAsync(jobseeker => jobseeker.JobSeekerId == id);
                if (jobseeker == null)
                {
                    return new Response
                    {
                        Status = "Success",
                        Message = "Job Seeker not found with the given ID."
                    };
                }
                jobseeker.UserName = jobseekerModel.UserName;
                jobseeker.JobSeekerName = jobseekerModel.JobSeekerName;
                jobseeker.Address = jobseekerModel.Address;
                jobseeker.Gender = jobseekerModel.Gender;
                jobseeker.DateOfBirth = jobseekerModel.DateOfBirth;
                jobseeker.CGPA = jobseekerModel.CGPA;
                jobseeker.ContactPhone = jobseekerModel.ContactPhone;
                jobseeker.CompanyName = jobseekerModel.CompanyName;
                jobseeker.Specialization = jobseekerModel.Specialization;
                jobseeker.Institute = jobseekerModel.Institute;
                jobseeker.Qualification = jobseekerModel.Qualification;
                jobseeker.Description = jobseekerModel.Description;
                jobseeker.Position = jobseekerModel.Position;
                jobseeker.Year = jobseekerModel.Year;
                jobseeker.StartDate = jobseekerModel.StartDate;
                jobseeker.EndDate = jobseekerModel.EndDate;
                jobseeker.Email = jobseekerModel.Email;

                await _appDBContext.SaveChangesAsync();

                return new Response
                {
                    Status = "Success",
                    Message = "Job Seeker updated successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new Response
                {
                    Status = "Failure",
                    Message = $"An error occurred while updating the Job Seeker: {ex.Message}"
                };
            }
        }
    }
}
