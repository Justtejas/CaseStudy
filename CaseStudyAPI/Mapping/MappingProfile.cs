using AutoMapper;
using CaseStudyAPI.DTO;
using CaseStudyAPI.Models;

namespace CaseStudyAPI.Mapping
{
    public class MappingProfile:Profile
    {
         public MappingProfile()
        {
            CreateMap<JobSeeker, RegisterJobSeekerDTO>().ReverseMap();
            CreateMap<Employer, RegisterEmployerDTO>().ReverseMap();
            CreateMap<Application, ApplicationDTO>().ReverseMap();
            CreateMap<JobListing, JobListingDTO>().ReverseMap();
            CreateMap<Employer, UpdateEmployerDTO>().ReverseMap();
            CreateMap<JobSeeker, UpdateJobSeekerDTO>().ReverseMap();
        }
    }
}
