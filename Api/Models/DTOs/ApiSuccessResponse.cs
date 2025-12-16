namespace Api.Models.DTOs
{
    public class ApiSuccessResponse<T>
    {
        public bool Success { get; set; } = true;
        public T Data { get; set; }
        public string Message { get; set; } = "Success";
    }
}
