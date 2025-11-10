using System.ComponentModel.DataAnnotations;

namespace WebApiProject.Application.DTOs.User
{
    public class UserUpdateDto : UserCreateDto
    {
        [Required]
        public int Id { get; set; }
    }
}