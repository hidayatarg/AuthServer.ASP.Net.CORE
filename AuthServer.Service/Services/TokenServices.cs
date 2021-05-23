using System;
using System.Security.Cryptography;
using AuthServer.Core.Configuration;
using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SharedLibrary.Configurations;

namespace AuthServer.Service.Services
{
    public class TokenServices: ITokenService
    {
        // DI
        private readonly UserManager<UserApp> _userManager;
        private readonly CustomTokenOption _tokenOption;

        public TokenServices(UserManager<UserApp> userManager,IOptions<CustomTokenOption> options)
        {
            _userManager = userManager;
            _tokenOption = options.Value;
        }
        
        // private method to create random token
        private string CreateRefreshToken()
        {
            var numberBytes = new byte[32];
            // bellow code is in using block
            using var random = RandomNumberGenerator.Create();
            random.GetBytes(numberBytes);

            return Convert.ToBase64String(numberBytes);
        }
        
        public TokenDto CreateToken(UserApp userApp)
        {
            throw new System.NotImplementedException();
        }

        public ClientTokenDto CreateTokenByClient(Client client)
        {
            throw new System.NotImplementedException();
        }
    }
}