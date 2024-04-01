namespace LetsTalkRaceApi.Models.Responses;

public class IdentityResponseDTO
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }

    public IdentityResponseDTO(ApplicationUser user)
    {
        Id = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        UserName = user.UserName;
        Email = user.Email;
        PhoneNumber = user.PhoneNumber;
    }
}