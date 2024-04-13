
using System.Security.Claims;

namespace LetsTalkRaceApi.Models;

public class Identity
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string Role { get; set; }

    public Identity(ClaimsPrincipal user)
    {
        Id = user.FindFirstValue(ClaimTypes.NameIdentifier);
        Email = user.FindFirstValue(ClaimTypes.Email);
        UserName = user.FindFirstValue(ClaimTypes.Name);
        Role = user.FindFirstValue(ClaimTypes.Role);
    }
}