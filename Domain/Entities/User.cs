using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiProject.Domain.Constant;
using WebApiProject.Domain.Enums;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace WebApiProject.Domain.Entities
{
    public class User : IdentityUser 
    {

        [Required]
        public string Role { get; set; } = string.Empty;
        [Required]
        [StringLength(36)]
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    }
}