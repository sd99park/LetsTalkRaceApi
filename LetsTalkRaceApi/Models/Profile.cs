using LetsTalkRaceApi.Models.Requests;
using Microsoft.AspNetCore.Identity;

namespace LetsTalkRaceApi.Models;

public class Profile
{
    public string ProfileId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // TODO : This stuff is for later implementations
    // public bool ShowProfileInAbout { get; set; }
    // public AboutProfile AboutProfile { get; set; }

    public Profile()
    {
    }
    
    public Profile(CreateProfileRequest req, IdentityUser user)
    {
        ProfileId = user.Id;
        FirstName = req.FirstName;
        LastName = req.LastName;
        EmailAddress = req.Email;
        // ShowProfileInAbout = req.ShowProfileInAbout;
        // AboutProfile = req.AboutProfile;
    }
}