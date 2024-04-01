namespace LetsTalkRaceApi.Models.Requests;

public class UpdatePasswordRequest
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
}