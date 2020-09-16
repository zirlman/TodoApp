using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using TasksApi.Interfaces;
using TasksApi.Middleware;
using TasksApi.Model;

namespace TasksApi.Service
{
    public class UserService : ICRUDService
    {
        private List<User> users = new List<User>
        {
            new User { Id = 1, FirstName = "Test", LastName = "User", Email = "test", Password = "test" }
        };

        private readonly AppSettings appSettings;

        public UserService(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        public bool Create(object obj)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> GetAll()
        {
            throw new NotImplementedException();
        }

        public object GetOne(int id)
        {
            throw new NotImplementedException();
        }

        public object Search(string key, string value)
        {
            throw new NotImplementedException();
        }

        public bool Update(object obj)
        {
            throw new NotImplementedException();
        }

        public object Authenticate(User user)
        {
            var res = users.SingleOrDefault(x => x.FirstName == user.FirstName && x.Password == user.Password);
            if (user == null)
            {
                return null;
            }
            var token = GenerateJwtToken(user);
            return new { user, token };
        }

        #region ============================== HELPER FUNCTIONS ==============================
        private string GenerateJwtToken(User user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        #endregion

    }
}
