using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace WebApiProject.Application.DTOs.Auth
{
    public class LoginDto
    {

        [Required]
        public string EmailorUserName { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}