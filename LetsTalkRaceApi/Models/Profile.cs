namespace LetsTalkRaceApi.Models;

public class Profile
{
    public string ProfileId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public bool ShowProfileInAbout { get; set; }
    public AboutProfile AboutProfile { get; set; } = new ();
    
}