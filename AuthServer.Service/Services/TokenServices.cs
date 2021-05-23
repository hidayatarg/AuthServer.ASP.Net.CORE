using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using AuthServer.Core.Configuration;
using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
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

        private IEnumerable<Claim> GetClaim(UserApp userApp, List<string> audience)
        {
            // all data to add in the payload as claim
            var userList = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userApp.Id),
                // new Claim("email", userApp.Email) same
                new Claim(JwtRegisteredClaimNames.Email, userApp.Email),
                new Claim(ClaimTypes.Name, userApp.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            
            userList.AddRange(audience.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));

            return userList;
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