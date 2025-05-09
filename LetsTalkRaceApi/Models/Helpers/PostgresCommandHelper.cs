
using Npgsql;using LetsTalkRaceApi.Models.Requests;
using NpgsqlTypes;

namespace LetsTalkRaceApi.Models.Helpers;

public static class PostgresCommandHelper
{
    
    #region Homework Controller
    
    public static NpgsqlCommand GetAllHomeworks(NpgsqlConnection conn)
    {
        var cmd = new NpgsqlCommand(SqlStrings.GET_ALL_HOMEWORKS, conn);
        cmd.Prepare();
        return cmd;
    }
    
    public static NpgsqlCommand GetHomeworkByHomeworkPage(NpgsqlConnection conn, string homeworkPage)
    {
        var cmd = new NpgsqlCommand(SqlStrings.GET_HOMEWORK_BY_HOMEWORK_PAGE, conn);
        cmd.Parameters.AddWithValue(Constants.HOMEWORK_PAGE, NpgsqlDbType.Varchar, homeworkPage);
        cmd.Prepare();
        return cmd;
    }

    public static NpgsqlCommand InsertHomework(NpgsqlConnection conn, string homeworkId,  HomeworkRequest homework)
    {
        var cmd = new NpgsqlCommand(SqlStrings.INSERT_HOMEWORK, conn);
        cmd.Parameters.AddWithValue(Constants.HOMEWORK_ID, NpgsqlDbType.Varchar, homeworkId);
        cmd.Parameters.AddWithValue(Constants.LINK, NpgsqlDbType.Varchar, homework.Link);
        cmd.Parameters.AddWithValue(Constants.DESCRIPTION, NpgsqlDbType.Varchar, homework.Description);
        cmd.Parameters.AddWithValue(Constants.SECTION, NpgsqlDbType.Varchar, homework.Section);
        cmd.Parameters.AddWithValue(Constants.HOMEWORK_PAGE, NpgsqlDbType.Varchar, homework.HomeworkPage);
        cmd.Parameters.AddWithValue(Constants.REQUIRED, NpgsqlDbType.Boolean, homework.Required);
        cmd.Prepare();
        return cmd;
    }
    
    public static NpgsqlCommand UpdateHomework(NpgsqlConnection conn, Homework homework)
    {
        var cmd = new NpgsqlCommand(SqlStrings.UPDATE_HOMEWORK, conn);
        cmd.Parameters.AddWithValue(Constants.HOMEWORK_ID, NpgsqlDbType.Varchar, homework.HomeworkId);
        cmd.Parameters.AddWithValue(Constants.LINK, NpgsqlDbType.Varchar, homework.Link);
        cmd.Parameters.AddWithValue(Constants.DESCRIPTION, NpgsqlDbType.Varchar, homework.Description);
        cmd.Parameters.AddWithValue(Constants.SECTION, NpgsqlDbType.Varchar, homework.Section);
        cmd.Parameters.AddWithValue(Constants.HOMEWORK_PAGE, NpgsqlDbType.Varchar, homework.HomeworkPage);
        cmd.Parameters.AddWithValue(Constants.REQUIRED, NpgsqlDbType.Boolean, homework.Required);
        cmd.Prepare();
        return cmd;
    }

    public static NpgsqlCommand DeleteHomework(NpgsqlConnection conn, string homeworkId)
    {
        var cmd = new NpgsqlCommand(SqlStrings.DELETE_HOMEWORK, conn);
        cmd.Parameters.AddWithValue(Constants.HOMEWORK_ID, NpgsqlDbType.Varchar, homeworkId);
        cmd.Prepare();
        return cmd;
    }
    
    #endregion
    
    #region Homework Controller

    public static NpgsqlCommand GetAllProfiles(NpgsqlConnection conn)
    {
        var cmd = new NpgsqlCommand(SqlStrings.GET_ALL_PROFILES, conn);
        cmd.Prepare();
        return cmd;
    }
    
    public static NpgsqlCommand GetProfileByProfileId(NpgsqlConnection conn, string profileId)
    {
        var cmd = new NpgsqlCommand(SqlStrings.GET_PROFILE, conn);
        cmd.Parameters.AddWithValue(NpgsqlDbType.Varchar, profileId);
        cmd.Prepare();
        return cmd;
    }

    public static NpgsqlCommand InsertProfile(NpgsqlConnection conn, Profile profile)
    {
        var cmd = new NpgsqlCommand(SqlStrings.INSERT_PROFILE, conn);
        cmd.Parameters.AddWithValue(NpgsqlDbType.Varchar, profile.ProfileId );
        cmd.Parameters.AddWithValue(NpgsqlDbType.Varchar, profile.FirstName);
        cmd.Parameters.AddWithValue(NpgsqlDbType.Varchar, profile.LastName);
        cmd.Parameters.AddWithValue(NpgsqlDbType.Varchar, profile.EmailAddress);
        cmd.Prepare();
        return cmd;
    }

    public static NpgsqlCommand DeleteProfile(NpgsqlConnection conn, string profileId)
    {
        var cmd = new NpgsqlCommand(SqlStrings.DELETE_PROFILE, conn);
        cmd.Parameters.AddWithValue(NpgsqlDbType.Varchar, profileId);
        cmd.Prepare();
        return cmd;
    }
    
    #endregion
    
}