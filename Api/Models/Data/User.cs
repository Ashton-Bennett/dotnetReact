using System.Text.Json.Serialization;

namespace Api.Models.Data
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public IEnumerable<string> Roles { get; set; } = Array.Empty<string>();

        public string Password { get; set; } = string.Empty;

        public required string Email { get; set; }

        public string DateOfBirth { get; set; } = string.Empty;

        [JsonIgnore]
        public string? RefreshToken { get; set; }   
    }
}
