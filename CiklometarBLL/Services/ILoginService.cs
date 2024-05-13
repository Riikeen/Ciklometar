using CiklometarBLL.DTO;
using CiklometarDAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CiklometarBLL.Services
{
   public interface ILoginService
    {
        public AccessTokenDataDTO AuthenticateUser(UserLoginDTO model);
        public AccessTokenDataDTO CreateUser(UserRegisterDTO model);
        public AuthenticateResult GenerateJSONWebToken(AccessTokenDataDTO userInfo);
        public AuthenticateResult RefreshAccessToken(string token);
        void SetCredentials(UserSetCredentialsDTO credentials, Guid userId);
        AccessTokenDataDTO CreateAccessTokenSignature(User user);
    }
}
