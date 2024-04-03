using Npgsql;
using LetsTalkRaceApi.Models;
using LetsTalkRaceApi.Models.Requests;
using NpgsqlTypes;

namespace LetsTalkRaceApi.Models.Helpers;

public class PostgresCommandHelper
{
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
}