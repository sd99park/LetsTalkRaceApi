namespace LetsTalkRaceApi.Models.Helpers;

public static class SqlStrings
{
    #region Homework Controller
    
    public static string GET_ALL_HOMEWORKS =
        "SELECT json_agg(a) FROM ( SELECT * FROM \"Homeworks\") a;";
    
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
    
    #region Homework Controller

    public static string GET_ALL_PROFILES = 
        "SELECT json_agg(a) FROM ( SELECT * FROM \"Profiles\") a;";
    
    // TODO: This will need to change to join to all profile tables when more profile info gets added
    public static string GET_PROFILE = 
        "SELECT json_agg(a) FROM ( SELECT * FROM \"Profiles\" WHERE \"ProfileId\" = $1) a;";
    
    public static string INSERT_PROFILE = 
        "INSERT INTO \"Profiles\" (\"ProfileId\", \"FirstName\", \"LastName\", \"EmailAddress\"," +
        "\"UpdatedAt\", \"CreatedAt\") VALUES ($1, $2, $3, $4, now(), now());";
    
    public static string DELETE_PROFILE = "DELETE FROM \"Profiles\" WHERE \"ProfileId\" = $1;";

    #endregion
}