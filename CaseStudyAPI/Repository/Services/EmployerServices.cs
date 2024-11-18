using CaseStudyAPI.Data;
using CaseStudyAPI.Models;
using CaseStudyAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CaseStudyAPI.Repository.Services
{
    public class EmployerServices : IEmployerServices
    {
        private readonly IAuthorizationService _authorizationServices;
        private readonly ApplicationDBContext _appDBContext;
        public EmployerServices(IAuthorizationService authServices, ApplicationDBContext appDBContext)
        {
            _authorizationServices = authServices;
            _appDBContext = appDBContext;
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
                if(employer.Role != null)
                {
                    return new Response { Status = "Failure", Message = "Invalid Request Body"};
                }
                employer.Password = await _authorizationServices.HashPasswordAsync(employer.Password);
                employer.EmployerId = Guid.NewGuid().ToString();
                await _appDBContext.Employers.AddAsync(employer);
                await _appDBContext.SaveChangesAsync();
                return new Response { Status = "Success", Message = "User Created Successfully" };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
                Console.WriteLine(ex.Message);
                return new Response
                {
                    Status = "Failure",
                    Message = $"An error occurred while deleting the employer: {ex.Message}"
                };
            }
        }

        public async Task<Employer> GetEmployerByUserName(string UserName)
        {
            var existingEmployer = await _appDBContext.Employers.FirstOrDefaultAsync(e => e.UserName == UserName);
            if (existingEmployer == null)
            {
                return null;
            }
            return existingEmployer;
        }
    }
}
