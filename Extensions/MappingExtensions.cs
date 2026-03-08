//using Mapster;
using AutoMapper;
using StudentsApi.Dtos;
using StudentsApi.Models;

namespace StudentsApi.Extensions
{
    //public static class MappingExtensions
    //{
    //    public static IServiceCollection AddCustomMappings(this IServiceCollection services)
    //    {
    //        services.AddMapster();

    //        TypeAdapterConfig<CreateStudentDto, Student>
    //            .NewConfig()
    //            .Map(dest => dest.DateOfBirth, src => src.DateOfBirth);

    //        TypeAdapterConfig<Student, StudentResponseDto>
    //            .NewConfig()
    //            .Map(dest => dest.Id, src => src.Id);

    //        return services;
    //    }
    //}

    // run the following command in the terminal to use AutoMapper:
    // dotnet remove package Mapster
    // dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Student, CreateStudentDto>().ReverseMap();
            CreateMap<Student, StudentResponseDto>().ReverseMap();
        }
    }
}
