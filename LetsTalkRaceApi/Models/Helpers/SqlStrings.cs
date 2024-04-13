namespace LetsTalkRaceApi.Models.Helpers;

public static class SqlStrings
{
    #region Homework Controller
    public static string GET_HOMEWORK_BY_HOMEWORK_PAGE =
        "SELECT json_agg(a) FROM ( SELECT * FROM \"Homeworks\" WHERE \"HomeworkPage\" = @homeworkPage) a;";

    public static string INSERT_HOMEWORK =
        "INSERT INTO \"Homeworks\" (\"HomeworkId\", \"Link\", \"Description\", \"Section\", \"HomeworkPage\", \"Required\")" +
        "VALUES (@homeworkId, @link, @description, @section, @homeworkPage, @required);";

    public static string UPDATE_HOMEWORK =
        "UPDATE \"Homeworks\" SET \"Link\" = @link, \"Description\" = @description, \"Section\" = @section, " +
        "\"HomeworkPage\" = @homeworkPage, \"Required\" = @required WHERE \"HomeworkId\" = @homeworkId;";

    public static string DELETE_HOMEWORK = "DELETE FROM \"Homeworks\" WHERE \"HomeworkId\" = @homeworkId;";
    #endregion
    
}