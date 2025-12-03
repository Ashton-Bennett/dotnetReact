namespace Api.Models.DTOs
{
    public class LoginResponse
    {
        public required string Email { get; set; }
        public string ?Username { get; set; }
        public required IEnumerable<string> Roles { get; set; }
        public required string AccessToken { get; set; }
        public required int Id { get; set; }
    }
}
