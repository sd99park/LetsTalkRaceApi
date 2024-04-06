using Microsoft.AspNetCore.Identity;

namespace LetsTalkRaceApi.Models.Responses;

public class IdentityResponseDTO
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }

    public IdentityResponseDTO(IdentityUser user)
    {
        Id = user.Id;
        UserName = user.UserName;
        Email = user.Email;
        PhoneNumber = user.PhoneNumber;
    }
}