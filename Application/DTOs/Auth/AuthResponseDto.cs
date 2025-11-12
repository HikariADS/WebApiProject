using System.ComponentModel.DataAnnotations;

namespace WebApiProject.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}