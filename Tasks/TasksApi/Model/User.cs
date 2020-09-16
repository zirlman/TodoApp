using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TasksApi.Model
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Email { get; set; }

        // Prevent password property from being serialized and returned in API response
        [JsonIgnore]
        public string Password { get; set; }

        public User()
        {
        }



        public override bool Equals(object obj)
        {
            return obj is User task &&
                   Id == task.Id &&
                   FirstName == task.FirstName &&
                   LastName == task.LastName &&
                   PhoneNumber == task.PhoneNumber &&
                   Email == task.Email &&
                   Password == task.Password;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, FirstName, LastName, PhoneNumber, Email, Password);
        }
    }
}
