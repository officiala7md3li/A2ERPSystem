namespace DomainDrivenERP.Web.Models.Auth;

public class LoginResponse
{
    public string Id { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public List<string> Errors { get; set; } = new();
}
