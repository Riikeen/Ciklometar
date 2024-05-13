using AutoMapper;
using CiklometarBLL.DTO;
using CiklometarDAL.Models;
using CiklometarDAL.Repository;
using CiklometarDAL.Repositroy;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using CiklometarBLL;

namespace CiklometarBLL.Services
{  
    public class LoginService : ILoginService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<RefreshToken> _tokenRepository;
        private readonly IRequestService _requestService;
        private readonly IRoleService _roleService;
        private readonly IUserManager _checkRole;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IRepository<UserBan> _banRepository;
        private readonly ContextDb _save;
        private static readonly HttpClient client = new HttpClient();
        public LoginService(IRepository<User> repository, IMapper mapper, IConfiguration config,
            ContextDb save, IRepository<RefreshToken> tokenRepository, IUserManager checkRole,
            IRequestService requestService, IRoleService roleService, IRepository<UserBan> banRepository)
        {
            _userRepository = repository;
            _tokenRepository = tokenRepository;
            _requestService = requestService;
            _roleService = roleService;
            _mapper = mapper;
            _config = config;
            _save = save;
            _checkRole = checkRole;
            _banRepository = banRepository;
        }
        public AccessTokenDataDTO AuthenticateUser(UserLoginDTO model)
        {
            var user = _userRepository.GetAllAsReadOnly().FirstOrDefault(x => x.Email == model.Email);

            if (user == null) return null;

            if (!(user.Password == BLLFunctions.CreateHash(model.Password,user.Salt)))
            {
                return null;
            }

            var userDTO = CreateAccessTokenSignature(user);

            return userDTO;
        }
        public void SetCredentials(UserSetCredentialsDTO credentials, Guid userId)
        {
            User user = _userRepository.GetById(userId);

            if (user == null) throw new Exception("No such user.");

            if(credentials.Email != null)
            {
                user.Email = credentials.Email;
            }

            if(credentials.Password != null)
            {
                var salt = BLLFunctions.CreateSalt();
                var pwHash = BLLFunctions.CreateHash(credentials.Password, salt);
                user.Salt = salt;
                user.Password = pwHash;
            }

            _userRepository.Update(user);
            _save.SaveChanges();
        }
        public AuthenticateResult RefreshAccessToken(string refreshToken)
        {
            var token = _tokenRepository.GetAll().Where(t => t.Token == refreshToken).FirstOrDefault();
            
            if (token == null)
            {
                return null;
            }
            var user = _userRepository.GetById(token.UserId);

            var loginModel = CreateAccessTokenSignature(user);

            return GenerateJSONWebToken(loginModel);
        }

        public AuthenticateResult GenerateJSONWebToken(AccessTokenDataDTO userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.Now.AddHours(2);

            var adminOfOrganizationsString = string.Join(",", userInfo.AdminOfOrganizations.ToArray());
            var memberOfOrganizationsString = string.Join(",", userInfo.MemberOfOrganizations.ToArray());
            var organizationsSentRequestTo = string.Join(",", userInfo.OrganizationsSentRequestTo.ToArray());

            var claims = new List<Claim>()
            {
                new Claim("id",userInfo.Id.ToString()),
                new Claim("nickname", userInfo.Nickname),
                new Claim("expiration", expiration.ToString()),
                new Claim("isSuperAdmin", userInfo.IsSuperAdmin.ToString()),
                new Claim("adminOfOrganizations", adminOfOrganizationsString),
                new Claim("memberOfOrganizations", memberOfOrganizationsString),
                new Claim("organizationsSentRequestTo", organizationsSentRequestTo),
                new Claim("connectedWithStrava",userInfo.ConnectedWithStrava.ToString()),
                new Claim("noCredentials",userInfo.NoCredentials.ToString()),
            };
            
            var token = new JwtSecurityToken(
              _config["Jwt:Issuer"],
              _config["Jwt:Audience"],
              claims,
              expires: expiration,
              signingCredentials: credentials);

            return new AuthenticateResult() {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration,
                RefreshToken = GenerateRefreshToken(userInfo.Id)
            };
        }

        private string GenerateRefreshToken(Guid id)
        {
            var token = _tokenRepository.GetAll().Where(i => i.UserId == id).FirstOrDefault();
            if(token == null)
            {
                var refreshToken = new RefreshToken()
                {
                    UserId = id,
                    AddedDate = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddDays(30),
                    Token = BLLFunctions.GenerateRandomString(25) + Guid.NewGuid()
                };
                _tokenRepository.Insert(refreshToken);
                _save.SaveChanges();
                return refreshToken.Token;
            }
            if(token.ExpiryDate <= DateTime.UtcNow)
            {
                token.Token = BLLFunctions.GenerateRandomString(25) + Guid.NewGuid();
                token.ExpiryDate = DateTime.UtcNow.AddDays(30);
                _tokenRepository.Update(token);
                _save.SaveChanges();
                return token.Token;
            }
            return token.Token;
        }

        public AccessTokenDataDTO CreateAccessTokenSignature(User user)
        {
            AccessTokenDataDTO signature = _mapper.Map<User, AccessTokenDataDTO>(user);

            signature.NoCredentials = user.Email == null || user.Password == null;
            signature.ConnectedWithStrava = user.StravaId != null;

            if (user.Roles != null)
            {
                signature.AdminOfOrganizations = user.Roles.Where(role => role.UserType == UserType.Admin).Select(role => role.OrganizationId).ToList();
                signature.MemberOfOrganizations = user.Roles.Where(role => role.UserType == UserType.Cyclist).Select(role => role.OrganizationId).ToList();
            } else
            {
                signature.AdminOfOrganizations = new List<Guid>();
                signature.MemberOfOrganizations = new List<Guid>();
            }

            if(user.Requests != null)
            {
                signature.OrganizationsSentRequestTo = user.Requests.Select(request => request.OrganizationId).ToList();
            } else
            {
                signature.OrganizationsSentRequestTo = new List<Guid>();
            }

            return signature;
        }

        public AccessTokenDataDTO CreateUser(UserRegisterDTO model)
        {
            var user = _mapper.Map<UserRegisterDTO, User>(model);

            if (model.StravaId == null)
            {
                if (_userRepository.GetAll().Any(u => u.Email == model.Email))
                    throw new Exception("Email \"" + model.Email + "\" is already used");
                var salt = BLLFunctions.CreateSalt();
                var pwHash = BLLFunctions.CreateHash(user.Password, salt);
                user.Salt = salt;
                user.Password = pwHash;
            } else
            {
                if (_userRepository.GetAll().Any(u => u.StravaId == model.StravaId))
                    throw new Exception("Strava account \"" + model.StravaId + "\" is already used");
            }

            _userRepository.Insert(user);
            _save.SaveChanges();

            var returnUser = CreateAccessTokenSignature(user);
            return returnUser;
        }
    }
}
