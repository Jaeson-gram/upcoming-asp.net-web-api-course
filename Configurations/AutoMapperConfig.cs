using AutoMapper;
using WebAPI2.Data;
using WebAPI2.Models;

namespace WebAPI2.Configurations;

public class AutoMapperConfig : Profile
{
    public AutoMapperConfig()
    {
        // map the data from source to destination
        // CreateMap<Student, StudentDTO>();
        // CreateMap<StudentDTO, Student>();
        
        
        // config to map with different property names
        // CreateMap<Student, StudentDTO>().ForMember(s => s.DOB, opt => opt.MapFrom(s => s.DateOfBirth)).ReverseMap();

        
        // config to ignore a particular property.. the side of the ReverseMap() you place it will determine whether it will work on instances before the data reverse or after
        // CreateMap<Student, StudentDTO>().ForMember(s => s.Email, opt => opt.Ignore()).ReverseMap();

        
        // config to transform data before it's mapped - for general instances
        // CreateMap<Student, StudentDTO>().AddTransform<string>(d => string.IsNullOrEmpty(d) ? "no data found" : d).ReverseMap();
        
        // the address field is the only one nullable in our StudentConfig file so we can be more specific with the display message as the next CreateMap instance below

        
        // this'll make it work for specific fields
        
        // CreateMap<Student, StudentDTO>()
        //     .ForMember(s => s.Address, opt => opt.MapFrom(a => string.IsNullOrEmpty(a.Address) ? "no address found" : a.Address))
        //     .ForMember(s => s.Email, opt => opt.MapFrom(a => string.IsNullOrEmpty(a.Email) ? "no email found" : a.Email)).ReverseMap();

        // custom 
        CreateMap<Student, StudentDTO>().AddTransform<string>(d => string.IsNullOrEmpty(d) ? "no data found" : d).ForMember(s => s.DOB, opt => opt.MapFrom(s => s.DateOfBirth)).ReverseMap();

    }   
}