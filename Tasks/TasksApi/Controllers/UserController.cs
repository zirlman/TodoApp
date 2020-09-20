using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TasksApi.Middleware;
using TasksApi.Model;
using TasksApi.Model.Auth;
using TasksApi.Model.Context;

namespace TasksApi.Service
{

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private const int HASH_LENGHT = 256;
        private const string PRESALT = "Prointer";
        private readonly UserTaskContext _context;

        private readonly AppSettings _appSettings;
        private readonly ILogger<UserController> _logger;
        public static IDistributedCache _cache;

        public UserController(IDistributedCache cache, UserTaskContext context, IOptions<AppSettings> appSettings, ILogger<UserController> logger)
        {
            _cache = cache;
            _logger = logger;
            _context = context;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(AuthRequest user)
        {
            // Get password hash
            // In real world the passed password shouldn't be in plain text format
            var obj = GetPswdHash(user.Email, user.Password);
            user.Password = obj.password;

            // Check if user exists
            if (_context.User.Any(u => u.Email == user.Email && u.Password == user.Password))
            {
                // If exists, generate token
                var response = GetJwtToken(user);
                if (response == null)
                {
                    return BadRequest(new { message = "Couldn't authenticate with the provided credentials" });
                }
                return Ok(response);
            }
            else
            {
                return NotFound(new { message = "Unknown user" });
            }
        }

        [HttpPost("logout/{id}")]
        public async Task<IActionResult> Logout()
        {
            // Get token 
            var id = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var claim = User.FindFirst(JwtRegisteredClaimNames.Jti);

            // Set TTL for the cached token
            var options = new DistributedCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromMinutes(5),
                AbsoluteExpiration = DateTime.Now.AddDays(30)
            };

            // Store the updated list in Redis
            await _cache.SetAsync(claim.Value, Encoding.UTF8.GetBytes(claim.Value), options);
            _logger.LogInformation($"LOGOUT USER ID: {id}");
            return Ok(new { jti = claim.Value });
        }

        [HttpGet("tasks")]
        public ActionResult<IEnumerable<User>> GetUserTasks()
        {
            long userId;
            var claimValue = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            if (long.TryParse(claimValue, out userId))
            {
                return Ok(_context.TaskItem.Where(item => item.UserId == userId));
            }
            else
            {
                return BadRequest(new { message = "Invalid claim" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(long id)
        {
            var user = await _context.User.Include(u => u.TaskItems).FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchUser(long id, [FromBody] JsonPatchDocument<User> patchDoc)
        {
            if (patchDoc != null)
            {
                // Find user on which to apply the update
                var user = await _context.User.FindAsync(id);
                // Update only the fields provided in the patchDoc object
                if (user != null)
                {
                    patchDoc.ApplyTo(user, ModelState);

                    if (!ModelState.IsValid)
                        return BadRequest(ModelState);
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!Exists(id))
                            return NotFound();
                        else
                        {
                            _logger.LogInformation($"UNABLE TO PATCH USER WITH ID: {id}");
                            throw;
                        }
                    }
                }
                else
                {
                    return NotFound(new { message = $"Unable to find TodoItem with id {id}" });
                }
                return new ObjectResult(user);
            }
            else
                return BadRequest(ModelState);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<User>> PostUser(UserRequest userReq)
        {
            if (userReq.Password != userReq.PasswordConfirmation)
            {
                return BadRequest(new { message = "Passwords don't match" }); ;
            }
            else if (!ExistByEmail(userReq.Email))
            {
                var obj = GetPswdHash(userReq.Email, userReq.Password);
                User user = new User
                {
                    Username = userReq.Username,
                    FirstName = userReq.FirstName,
                    LastName = userReq.LastName,
                    Email = userReq.Email,
                    PhoneNumber = userReq.PhoneNumber,
                    Password = obj.password,
                    PasswordSalt = obj.salt
                };
                _context.User.Add(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"NEW USER: {user.Username}");
                return CreatedAtAction("PostUser", new { id = user.UserId }, user);
            }
            else
            {
                return Conflict(new { message = "User already exists" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(long id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"DELETE USER ID: {id}");
            return Ok(user);
        }


        #region ============================== HELPER FUNCTIONS ==============================

        private object GetJwtToken(AuthRequest req)
        {
            var user = _context.User.SingleOrDefault(u => u.Email == req.Email && u.Password == req.Password);
            if (user == null)
            {
                return null;
            }
            var token = GenerateJwtToken(user);
            _logger.LogInformation($"JWT GENERATED: ${user.Username}");
            return new { user, token };
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()), new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) }),
                Expires = DateTime.UtcNow.AddDays(_appSettings.ExpirationInDays),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public dynamic GetPswdHash(string email, string password)
        {
            if (email != null && password != null)
            {
                byte[] salt = Encoding.UTF8.GetBytes(email + PRESALT);
                byte[] pswdHash;
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 4000))
                    pswdHash = pbkdf2.GetBytes(HASH_LENGHT);
                return new { password = Convert.ToBase64String(pswdHash), salt = Convert.ToBase64String(salt) };
            }
            else
            {
                return null;
            }
        }

        private bool Exists(long id)
        {
            return _context.User.Any(e => e.UserId == id);
        }
        private bool ExistByEmail(string email)
        {
            return _context.User.Any(e => e.Email == email);
        }
        #endregion


    }
}
