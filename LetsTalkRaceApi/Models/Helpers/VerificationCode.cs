namespace LetsTalkRaceApi.Models.Helpers;

public class VerificationCode
{
    public string GenerateVerificationCode()
    {
        var r = new Random();
        var randNum = r.Next(1000000);
        return randNum.ToString("D6");
    }
}