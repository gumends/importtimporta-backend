using Domain.Entities;

namespace Domain.Models.UserMenu
{
    public class UserMenu
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public Roles Role { get; set; } = Roles.User;
    }
}
