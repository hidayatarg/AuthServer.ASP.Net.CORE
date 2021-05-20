using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AutoMapper;

namespace AuthServer.Service
{
    /// <summary>
    /// The class can be reached in this project
    /// </summary>
    class DtoMapper: Profile
    {
        public DtoMapper()
        {
            // The properties are interchangeable
            CreateMap<ProductDto, Product>().ReverseMap();
            CreateMap<UserAppDto, UserApp>().ReverseMap();
            
        }
    }
}