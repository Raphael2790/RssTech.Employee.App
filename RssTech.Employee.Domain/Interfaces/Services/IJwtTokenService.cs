namespace RssTech.Employee.Domain.Interfaces.Services;

public interface IJwtTokenService
{
    string GenerateToken(Entities.Employee employee);
    string GenerateRefreshToken();
    DateTime GetTokenExpiration();
    bool ValidateToken(string token);
}