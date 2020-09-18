using Microsoft.AspNetCore.Identity;
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
using TasksApi.Model.Auth;

namespace TasksApi.Service
{
    public class UserService : IAuthService
    {
        private List<User> users = new List<User>
        {
            new User { Id = 1, Username="test", FirstName = "Test", LastName = "User", Email = "test", Password = "test" }
        };

        private readonly AppSettings appSettings;

        public UserService(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        public bool Create(object obj)
        {
            int oldSize = users.Count;
            users.Add((User)obj);
            return users.Count != oldSize;
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> GetAll()
        {
            return users;
        }

        public object GetOne(int id)
        {
            return users.FirstOrDefault(user => user.Id == id);
        }

        public object Search(string key, string value)
        {
            throw new NotImplementedException();
        }

        public bool Update(object obj)
        {
            throw new NotImplementedException();
        }

        public object Authenticate(AuthRequest req)
        {
            var user = users.SingleOrDefault(u => u.Username == req.Username && u.Password == req.Password);
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
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(appSettings.ExpirationInDays),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        #endregion

    }
}
