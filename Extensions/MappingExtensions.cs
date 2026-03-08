using AutoMapper;
using Mapster;
using Microsoft.EntityFrameworkCore;
using StudentsApi.Dtos;
using StudentsApi.Models;
using IConfigurationProvider = Microsoft.Extensions.Configuration.IConfigurationProvider;
using IMapper = MapsterMapper.IMapper;

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
    public static class MappingExtensions
    {
        // Simple object-to-object mapping
        public static TDestination MapTo<TDestination>(this object source, IMapper mapper)
            => mapper.Map<TDestination>(source);

        // Collection/enumerable mapping
        public static List<TDestination> MapToList<TDestination>(this IEnumerable<object> source, IMapper mapper)
            => mapper.Map<List<TDestination>>(source);

        // IQueryable projection (useful for Entity Framework)
        public static IQueryable<TDestination> ProjectTo<TDestination>(this IQueryable source, IConfigurationProvider configuration)
            => source.ProjectTo<TDestination>(configuration);

        // Async version to retrieve a projected list (EF Core)
        public static Task<List<TDestination>> ProjectToListAsync<TDestination>(this IQueryable source, IConfigurationProvider configuration)
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Student, CreateStudentDto>().ReverseMap();
            CreateMap<Student, StudentResponseDto>().ReverseMap();
        }
    }
}
