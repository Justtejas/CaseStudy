using CaseStudyAPI.Data;
using CaseStudyAPI.DTO;
using CaseStudyAPI.Models;
using CaseStudyAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CaseStudyAPI.Repository.Services
{
    public class EmployerServices : IEmployerServices
    {
        private readonly IAuthorizationService _authorizationServices;
        private readonly ApplicationDBContext _appDBContext;
        private readonly ILogger<EmployerServices> _logger;
        public EmployerServices(IAuthorizationService authServices, ApplicationDBContext appDBContext, ILogger<EmployerServices> logger)
        {
            _authorizationServices = authServices;
            _appDBContext = appDBContext;
            _logger = logger;
        }

        public async Task<Response> CreateEmployerAsync(Employer employer)
        {
            try
            {
                var employerExists = await _appDBContext.Employers.FirstOrDefaultAsync(e => e.UserName == employer.UserName || e.Email == employer.Email);
                if (employerExists != null)
                {
                    return new Response { Status = "Failure", Message = "An employer with this username or email already exists." };
                }
                if (employer.Role != null)
                {
                    return new Response { Status = "Failure", Message = "Invalid Request Body" };
                }
                employer.Password = _authorizationServices.HashPassword(employer.Password);
                employer.EmployerId = Guid.NewGuid().ToString();
                await _appDBContext.Employers.AddAsync(employer);
                await _appDBContext.SaveChangesAsync();
                return new Response { Status = "Success", Message = "Employer Created Successfully" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public async Task<Response> DeleteEmployerAsync(string id)
        {
            try
            {
                var employer = await _appDBContext.Employers.FirstOrDefaultAsync(employer => employer.EmployerId == id);
                if (employer == null)
                {
                    return new Response
                    {
                        Status = "Success",
                        Message = "Employer not found with the given ID."
                    };
                }
                _appDBContext.Employers.Remove(employer);
                await _appDBContext.SaveChangesAsync();
                return new Response
                {
                    Status = "Success",
                    Message = "Employer deleted successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new Response
                {
                    Status = "Failure",
                    Message = $"An error occurred while deleting the employer: {ex.Message}"
                };
            }
        }

        public async Task<List<Employer>> GetAllEmployersAsync()
        {
            var employers = await _appDBContext.Employers.ToListAsync();
            if (employers == null)
            {
                return null;
            }
            return employers;
        }

        public async Task<Employer> GetEmployerByUserName(string userName)
        {
            var existingEmployer = await _appDBContext.Employers.FirstOrDefaultAsync(e => e.UserName == userName);
            if (existingEmployer == null)
            {
                return null;
            }
            return existingEmployer;
        }

        public async Task<Employer> GetEmployerByEmployerIdAsync(string employerId)
        {
            var existingEmployer = await _appDBContext.Employers.FirstOrDefaultAsync(e => e.EmployerId == employerId);
            if (existingEmployer == null)
            {
                return null;
            }
            return existingEmployer;
        }

        public async Task<Response> UpdateEmployerAsync(string employerId, UpdateEmployerDTO employer)
        {
            try
            {
                var employerUser = await _appDBContext.Employers.FirstOrDefaultAsync(e => e.EmployerId == employerId);
                if (employerUser == null)
                {
                    _logger.LogError("Employer not found with given ID.");
                    return new Response
                    {
                        Status = "Failure",
                        Message = "Employer not found with the given ID."
                    };
                }

                if (employer.UserName != employerUser.UserName)
                {
                    var usernameExists = await _appDBContext.Employers.AnyAsync(e => e.UserName == employer.UserName);
                    if (usernameExists)
                    {
                        return new Response
                        {
                            Status = "Failure",
                            Message = "The username is already taken by another employer."
                        };
                    }
                }

                if (employer.Email != employerUser.Email)
                {
                    var emailExists = await _appDBContext.Employers.AnyAsync(e => e.Email == employer.Email);
                    if (emailExists)
                    {
                        return new Response
                        {
                            Status = "Failure",
                            Message = "The email is already taken by another employer."
                        };
                    }
                }

                employerUser.EmployerName = employer.EmployerName;
                employerUser.UserName = employer.UserName;
                employerUser.CompanyName = employer.CompanyName;
                employerUser.Gender = employer.Gender;
                employerUser.ContactPhone = employer.ContactPhone;
                employerUser.Email = employer.Email;

                await _appDBContext.SaveChangesAsync();

                return new Response
                {
                    Status = "Success",
                    Message = "Employer updated successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating employer: {ex.Message}");
                return new Response
                {
                    Status = "Failure",
                    Message = $"An error occurred while updating the employer: {ex.Message}"
                };
            }
        }
    }
}
