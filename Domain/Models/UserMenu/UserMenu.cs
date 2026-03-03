using Domain.Entities;
using Domain.Enuns;

namespace Domain.Models.UserMenu
{
    public class UserMenu
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public Roles Role { get; set; } = Roles.User;
    }
}
