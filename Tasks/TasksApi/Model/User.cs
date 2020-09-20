using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TasksApi.Model
{
    public class User
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long UserId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        // Prevent password properties from being serialized and returned in API response
        [JsonIgnore]
        public string Password { get; set; }
        [JsonIgnore]
        public string PasswordSalt { get; set; }

        // TaskItems
        public ICollection<TaskItem> TaskItems { get; set; }
    }

    public class UserRequest
    {
        public long UserId { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string PasswordConfirmation { get; set; }
    }
}
