using CaseStudyAPI.Data;
using CaseStudyAPI.Models;
using CaseStudyAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace CaseStudyAPI.Repository.Services
{
    public class ApplicationServices : IApplicationServices
    {
        private readonly ApplicationDBContext _appDBContext;
        public ApplicationServices(ApplicationDBContext appDBContext)
        {
            _appDBContext = appDBContext;
        }

        public async Task<Application> CreateApplicationAsync(Application application)
        {
            try
            {

                application.ApplicationId = Guid.NewGuid().ToString();
                application.ApplicationDate = DateTime.UtcNow;
                await _appDBContext.Applications.AddAsync(application);
                await _appDBContext.SaveChangesAsync();
                return application;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<bool> DeleteApplicationAsync(string id, string jobSeekerId)
        {
            try
            {
                 var application = await _appDBContext.Applications.FindAsync(id);
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
                Console.WriteLine(ex.Message);
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
                Console.WriteLine(ex.Message);
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
                return null;
            }
        }

        public async Task<bool> UpdateApplicationAsync(Application application)
        {
            try
            {
                var existingApplication = await _appDBContext.Applications.FirstOrDefaultAsync(a => a.ApplicationId == application.ApplicationId);

                if (existingApplication == null)
                {
                    return false;
                }
                existingApplication.ApplicationStatus = application.ApplicationStatus;
                existingApplication.ApplicationDate = DateTime.Now;
                await _appDBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
