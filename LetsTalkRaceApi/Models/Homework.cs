namespace LetsTalkRaceApi.Models;

public class Homework
{
    public string HomeworkId { get; set; }
    public string Link { get; set; }
    public string Description { get; set; }
    public string Section { get; set; } // Watch, Listen, Read, Take
    public string HomeworkPage { get; set; }
    public bool Required { get; set; }
}