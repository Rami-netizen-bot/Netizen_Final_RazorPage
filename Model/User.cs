namespace RazorDemo.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty; // Male, Female, Other
        public string Role { get; set; } = "User";         // Admin, User, Editor
        public string? Remark { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
    }
}