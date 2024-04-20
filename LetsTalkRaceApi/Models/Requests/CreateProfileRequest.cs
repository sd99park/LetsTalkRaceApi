namespace LetsTalkRaceApi.Models.Requests;

public class CreateProfileRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    // public bool ShowProfileInAbout { get; set; }
    // public AboutProfile AboutProfile { get; set; } = new ();
}