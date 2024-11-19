using CaseStudyAPI.Data;
using CaseStudyAPI.Models;
using CaseStudyAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace CaseStudyAPI.Repository.Services
{
    public class JobSeekerServices : IJobSeekerServices
    {

        private readonly IAuthorizationService _authorizationServices;
        private readonly ApplicationDBContext _appDBContext;
        public JobSeekerServices(IAuthorizationService authServices, ApplicationDBContext appDBContext)
        {
            _authorizationServices = authServices;
            _appDBContext = appDBContext;
        }
        public async Task<Response> CreateJobSeekerAsync(JobSeeker jobseeker)
        {
            try
            {
                jobseeker.Password = await _authorizationServices.HashPasswordAsync(jobseeker.Password);
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
                Console.WriteLine(ex.Message);
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
                Console.WriteLine(ex.Message);
                return new Response
                {
                    Status = "Failure",
                    Message = $"An error occurred while deleting the Job Seeker: {ex.Message}"
                };
            }
        }

        public async Task<JobSeeker> GetJobSeekerByUserName(string userName)
        {
            var existingJobSeeker = await _appDBContext.JobSeekers.FirstOrDefaultAsync(j => j.UserName == userName);
            if (existingJobSeeker == null)
            {
                return null;
            }
            return existingJobSeeker;
        }
    }
}
