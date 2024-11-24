using CaseStudyAPI.Data;
using CaseStudyAPI.DTO;
using CaseStudyAPI.Models;
using CaseStudyAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace CaseStudyAPI.Repository.Services
{
    public class ApplicationServices : IApplicationServices
    {
        private readonly ApplicationDBContext _appDBContext;
        private readonly ILogger<ApplicationServices> _logger;

        public ApplicationServices(ApplicationDBContext appDBContext, ILogger<ApplicationServices> logger)
        {
            _appDBContext = appDBContext;
            _logger = logger;
        }

        public async Task<Application> CreateApplicationAsync(ApplicationDTO applicationDTO,string jobSeekerId)
        {
            try
            {
                var application = new Application
                {
                    ApplicationId = Guid.NewGuid().ToString(),
                    JobListingId = applicationDTO.JobListingId,
                    ApplicationStatus = "Pending",
                    JobSeekerId = jobSeekerId,
                    ApplicationDate = DateTime.Now,
                };

                var createdApplication = await _appDBContext.Applications.AddAsync(application);
                await _appDBContext.SaveChangesAsync();

                return createdApplication.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<bool> DeleteApplicationAsync(string applicationId, string jobSeekerId)
        {
            try
            {
                 var application = await _appDBContext.Applications.FindAsync(applicationId);
                if (application == null)
                {
                    return false;
                }

                _appDBContext.Applications.Remove(application);
                await _appDBContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public async Task<Application> GetApplicationByIdAsync(string id)
        {
            try
            {
                var application = await _appDBContext.Applications.FindAsync(id);
                if (application == null)
                {
                    return null;
                }

                return application;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<List<Application>> GetApplicationByJSIdAsync(string id)
        {
            try
            {
                var applications = await _appDBContext.Applications.Where(a => a.JobSeekerId == id).ToListAsync();
                return applications;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<List<Application>> GetApplicationByJobListingIdAsync(string id)
        {
            try
            {
                var applications = await _appDBContext.Applications.Where(a => a.JobListingId == id).ToListAsync();
                return applications;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<Application>> GetApplicationByEmployerIDAsync(string id)
        {
            try
            {
                var applications = await _appDBContext.Applications.Include(a => a.JobListing).Where(a => a.JobListing.EmployerId == id).ToListAsync();
                return applications;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }
        public async Task<List<Application>> GetAllApplicationsAsync()
        {
            try
            {
                var applications = await _appDBContext.Applications.ToListAsync();
                return applications;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<bool> UpdateApplicationAsync(string applicationId, string applicationStatus)
        {
            try
            {
                var existingApplication = await _appDBContext.Applications.FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

                if (existingApplication == null)
                {
                    return false;
                }
                existingApplication.ApplicationStatus = applicationStatus;
                await _appDBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }
    }
}
