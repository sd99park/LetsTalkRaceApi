namespace LetsTalkRaceApi.Models.Requests;

public class HomeworkRequest
{
    public string Link { get; set; }
    public string Description { get; set; }
    public string Section { get; set; }
    public string HomeworkPage { get; set; }
    public bool Required { get; set; }
}