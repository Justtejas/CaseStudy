using AutoMapper;
using CaseStudyAPI.Data;
using CaseStudyAPI.DTO;
using CaseStudyAPI.Models;
using CaseStudyAPI.Repository.Interfaces;

namespace CaseStudyAPI.Repository.Services
{
    public class UserServices : IUserServices
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IEmployerServices _employerServices;
        private readonly IJobSeekerServices _jobSeekerServices;
        private readonly IMapper _mapper;
        public UserServices(IAuthorizationService authorizationService, IEmployerServices employerServices, IMapper mapper, IJobSeekerServices jobSeekerServices)
        {
            _authorizationService = authorizationService;
            _employerServices = employerServices;
            _mapper = mapper;
            _jobSeekerServices = jobSeekerServices;
        }
        public async Task<TokenResponse> LoginAsync<T>(T login)
        {
            var token = await _authorizationService.GenerateJWTTokenAsync(login);
            return token;
        }

        public async Task<Response> RegisterEmployerAsync(RegisterEmployerDTO regEmployer)
        {
            var employeeToRegister = _mapper.Map<Employer>(regEmployer);
            var result = await _employerServices.CreateEmployerAsync(employeeToRegister);
            return result;
        }

        public async Task<Response> RegisterJobSeekerAsync(RegisterJobSeekerDTO regJobSeeker)
        {
            var jobSeekerToRegister = _mapper.Map<JobSeeker>(regJobSeeker);
            var result = await _jobSeekerServices.CreateJobSeekerAsync(jobSeekerToRegister);
            return result;
        }
    }
}
