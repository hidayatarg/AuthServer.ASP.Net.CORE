using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using AuthServer.Core.Configuration;
using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Configurations;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

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

        private IEnumerable<Claim> GetClaims(UserApp userApp, List<string> audience)
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

        private IEnumerable<Claim> GetClaimsByClient(Client client)
        {
            var claims = new List<Claim>();
            claims.AddRange(client.Audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
            // random Guid
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());
            // subject => token belongs to
            new Claim(JwtRegisteredClaimNames.Sub, client.Id.ToString());

            return claims;

        }
        
        public TokenDto CreateToken(UserApp userApp)
        {
            // in minute
            var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.AccessTokenExpiration);
            var refreshTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.RefreshTokenExpiration);

            var securityKey = SignService.GetSymmetricSecurityKey(_tokenOption.SecurityKey);

            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _tokenOption.Issuer,
                expires: accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: GetClaims(userApp, _tokenOption.Audience),
                signingCredentials: signingCredentials
            );

            var handler = new JwtSecurityTokenHandler();

            var token = handler.WriteToken(jwtSecurityToken);

            var tokenDto = new TokenDto
            {
                AccessToken = token,
                RefreshToken = CreateRefreshToken(),
                AccessTokenExpiration = accessTokenExpiration,
                RefreshTokenExpiration = refreshTokenExpiration
            };

            return tokenDto;
        }

        public ClientTokenDto CreateTokenByClient(Client client)
        {
            // in minute
            var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.AccessTokenExpiration);

            var securityKey = SignService.GetSymmetricSecurityKey(_tokenOption.SecurityKey);

            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _tokenOption.Issuer,
                expires: accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: GetClaimsByClient(client),
                signingCredentials: signingCredentials
            );

            var handler = new JwtSecurityTokenHandler();

            var token = handler.WriteToken(jwtSecurityToken);

            var tokenDto = new ClientTokenDto
            {
                AccessToken = token,
                AccessTokenExpiration = accessTokenExpiration,
            };

            return tokenDto;
        }
    }
}