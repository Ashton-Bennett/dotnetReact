namespace Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public IEnumerable<string> Roles { get; set; } = [string.Empty];
        public string Password { get; set; } = string.Empty;
        public required string Email { get; set; }
        public string DateOfBirth { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }   
    }
}
