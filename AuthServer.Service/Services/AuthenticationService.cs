using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthServer.Core.Configuration;
using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedLibrary.Dtos;

namespace AuthServer.Service.Services
{
    public class AuthenticationService: IAuthenticationService
    {
        private readonly List<Client> _clients;
        private readonly ITokenService _tokenService;
        private readonly UserManager<UserApp> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserRefreshToken> _userRefreshTokenService;

        // Dependency Injection
        public AuthenticationService(IOptions<List<Client>> optionClients, ITokenService tokenService, UserManager<UserApp> userManager,
            IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken> userRefreshTokenService)
        {
            _clients = optionClients.Value;
            _tokenService = tokenService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _userRefreshTokenService = userRefreshTokenService;
        }
        
        
        public async Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto)
        {
            if (loginDto == null)
            {
                throw new ArgumentNullException(nameof(loginDto));
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
            {
                return Response<TokenDto>.Fail("Email or Password is wrong. ", 400, true);
            }

            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return Response<TokenDto>.Fail("Email or Password is wrong. ", 400, true);
            }

            var token = _tokenService.CreateToken(user);

            var userRefreshToken = await _userRefreshTokenService
                .Where(x => x.UserId == user.Id)
                .SingleOrDefaultAsync();

            if (userRefreshToken == null)
            {
                await _userRefreshTokenService.AddAsync(new UserRefreshToken
                {
                    UserId = user.Id,
                    Code = token.RefreshToken,
                    Expiration = token.AccessTokenExpiration
                });
            }
            else
            {
                userRefreshToken.Code = token.RefreshToken;
                userRefreshToken.Expiration = token.RefreshTokenExpiration;
            }
            
            // unit of work
            await _unitOfWork.CommitAsync();
            
            return Response<TokenDto>.Success(token, 200);
        }

        public Task<Response<TokenDto>> CreateTokenByRefreshToken(string refreshToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<Response<ClientTokenDto>> CreateTokenByClient(ClientLoginDto clientLoginDto)
        {
            throw new System.NotImplementedException();
        }
    }
}