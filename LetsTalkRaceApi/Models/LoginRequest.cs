namespace LetsTalkRaceApi.Models;

public class LoginRequest
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public bool IsPersistent { get; set; }
}