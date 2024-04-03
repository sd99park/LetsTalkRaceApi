using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace LetsTalkRaceApi.Controllers;

public class LtrControllerBase : ControllerBase
{
    protected readonly IConfiguration _config;
    
    protected LtrControllerBase(IConfiguration config)
    {
        _config = config;
    }

    protected NpgsqlConnection OpenConnection()
    {
        var npgsqlConnection = new NpgsqlConnection(_config.GetConnectionString("PostgresConnection"));
        npgsqlConnection.Open();
        return npgsqlConnection;
    }

    protected void CloseConnection(NpgsqlConnection npgsqlConnection) => npgsqlConnection?.Close();
}