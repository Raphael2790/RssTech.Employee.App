namespace RssTech.Employee.Application.UseCases.Auth.Login.Response;

public record struct LoginEmployeeResponse
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}
