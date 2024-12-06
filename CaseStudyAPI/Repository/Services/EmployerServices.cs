﻿using CaseStudyAPI.Data;
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
                if(employer.Role != null)
                {
                    return new Response { Status = "Failure", Message = "Invalid Request Body"};
                }
                employer.Password =  _authorizationServices.HashPassword(employer.Password);
                employer.EmployerId = Guid.NewGuid().ToString();
                await _appDBContext.Employers.AddAsync(employer);
                await _appDBContext.SaveChangesAsync();
                return new Response { Status = "Success", Message = "Employer Created Successfully" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,ex.Message);
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
                _logger.LogError(ex,ex.Message);
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
            if(employers == null)
            {
                return null;
            }
            return employers;
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

          public async Task<Response> UpdateEmployerAsync(string employerId,Employer employer)
        {
            try
            {
                var employerUser = await _appDBContext.Employers.FirstOrDefaultAsync(employer => employer.EmployerId == employerId);
                if (employerUser == null)
                {
                    _logger.LogError("Employer not found with given Id");
                    return new Response
                    {
                        Status = "Success",
                        Message = "Employer not found with the given ID."
                    };
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
                _logger.LogError(ex.Message);
                return new Response
                {
                    Status = "Failure",
                    Message = $"An error occurred while updating the Employer: {ex.Message}"
                };
            }
        }
    }
}
