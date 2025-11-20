namespace WebApiProject.Application.DTOs.Auth
{
    public class ChangeRoleDto
    {
        public string UserId { get; set; } = string.Empty;
        public string NewRole { get; set; } = string.Empty;
    }
}
