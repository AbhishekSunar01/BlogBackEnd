using System.ComponentModel.DataAnnotations;

namespace BisleriumServer.Model.DTO
{
    public class RegisterModel
    {
        [Required]
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
    }
}
