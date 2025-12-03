namespace Api.Models.DTOs
{
    public class AuthTokensDTO
    {
        public required string RefreshToken { get; set; }
        public required string AccessToken { get; set; }
    }
}
