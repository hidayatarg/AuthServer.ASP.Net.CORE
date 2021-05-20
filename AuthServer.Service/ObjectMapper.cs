using System;
using AutoMapper;

namespace AuthServer.Service
{
    public static class ObjectMapper
    {
        // lazy loading 
        // dont place in memory from the start, load when required
        private static readonly Lazy<IMapper> Lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                // put all mapper 
                cfg.AddProfile<DtoMapper>();
            });

            return config.CreateMapper();
        });
        
        // calling from here
        public static IMapper Mapper => Lazy.Value;
    }
}