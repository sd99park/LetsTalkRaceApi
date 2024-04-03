using System.Text.Json;
using LetsTalkRaceApi.Models;
using LetsTalkRaceApi.Models.Helpers;
using LetsTalkRaceApi.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace LetsTalkRaceApi.Controllers;

[ApiController]
[Route("api/homework/v1")]
public class HomeworkController : LtrControllerBase
{
    
    public HomeworkController(IConfiguration config) : base(config)
    {
    }

    [HttpGet]
    [Route("homework/{homeworkPage}")]
    [ProducesResponseType(typeof(List<Homework>), 201)]
    [ProducesResponseType(typeof(BadRequestResult), 400)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), 401)]
    public async Task<IActionResult> GetAllHomework(string homeworkPage)
    {
        NpgsqlConnection conn = null;

        try
        {
            conn = OpenConnection();

            var cmd = PostgresCommandHelper.GetHomeworkByHomeworkPage(conn, homeworkPage);
            var strResponse = Convert.ToString(cmd.ExecuteScalar());
            var hws = string.IsNullOrWhiteSpace(strResponse) 
                ? new List<Homework>() 
                : JsonSerializer.Deserialize<List<Homework>>(strResponse);
            
            return Ok(hws);
        }
        finally
        {
            CloseConnection(conn);
        } 
    }

    [HttpPost]
    [Route("addHomework")]
    [ProducesResponseType(typeof(Homework), 201)]
    [ProducesResponseType(typeof(BadRequestResult), 400)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), 401)]
    public async Task<IActionResult> AddHomework([FromBody] HomeworkRequest homework)
    {
        NpgsqlConnection conn = null;
        NpgsqlTransaction npgsqlTrans = null;

        try
        {
            conn = OpenConnection();
            npgsqlTrans = conn.BeginTransaction();
            
            conn = OpenConnection();

            var cmd = PostgresCommandHelper.InsertHomework(conn, Guid.NewGuid().ToString(), homework);
            cmd.Transaction = npgsqlTrans;

            var rowsAffected = cmd.ExecuteNonQuery();
            if (rowsAffected != 1)
            {
                throw new Exception($"Unexpected exception writing to Homework table. Affected Rows: {rowsAffected}");
            }
            
            return Ok(homework);
        }
        catch (Exception e)
        {
            npgsqlTrans?.Rollback();
            throw new Exception(e.Message);
        }
        finally
        {
            CloseConnection(conn);
        }
    }
    
    [HttpPost]
    [Route("updateHomework")]
    [ProducesResponseType(typeof(Homework), 201)]
    [ProducesResponseType(typeof(BadRequestResult), 400)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), 401)]
    public async Task<IActionResult> UpdateHomework([FromBody] Homework homework)
    {
        NpgsqlConnection conn = null;
        NpgsqlTransaction npgsqlTrans = null;

        try
        {
            conn = OpenConnection();
            npgsqlTrans = conn.BeginTransaction();
            
            conn = OpenConnection();

            var cmd = PostgresCommandHelper.UpdateHomework(conn, homework);
            cmd.Transaction = npgsqlTrans;

            var rowsAffected = cmd.ExecuteNonQuery();
            if (rowsAffected != 1)
            {
                throw new Exception($"Unexpected exception updating Homework table. Affected Rows: {rowsAffected}");
            }
            
            return Ok(homework);
        }
        catch (Exception e)
        {
            npgsqlTrans?.Rollback();
            throw new Exception(e.Message);
        }
        finally
        {
            CloseConnection(conn);
        }
    }
    
    [HttpPost]
    [Route("removeHomework")]
    [ProducesResponseType(typeof(string), 201)]
    [ProducesResponseType(typeof(BadRequestResult), 400)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), 401)]
    public async Task<IActionResult> RemoveHomework([FromBody] string homeworkId)
    {
        NpgsqlConnection conn = null;
        NpgsqlTransaction npgsqlTrans = null;

        try
        {
            conn = OpenConnection();
            npgsqlTrans = conn.BeginTransaction();
            
            conn = OpenConnection();

            var cmd = PostgresCommandHelper.DeleteHomework(conn, homeworkId);
            cmd.Transaction = npgsqlTrans;

            var rowsAffected = cmd.ExecuteNonQuery();
            if (rowsAffected != 1)
            {
                throw new Exception($"Unexpected exception deleting from Homework table. Affected Rows: {rowsAffected}");
            }
            
            return Ok("Successfully deleted homework");
        }
        catch (Exception e)
        {
            npgsqlTrans?.Rollback();
            throw new Exception(e.Message);
        }
        finally
        {
            CloseConnection(conn);
        }
    }
    
    // Not to be used on FE, use for mass upload on initial create
    [HttpPost]
    [Route("addMultipleHomework")]
    [ProducesResponseType(typeof(string), 201)]
    [ProducesResponseType(typeof(BadRequestResult), 400)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), 401)]
    public async Task<IActionResult> AddMultipleHomework([FromBody] List<HomeworkRequest> homeworks)
    {
        NpgsqlConnection conn = null;
        NpgsqlTransaction npgsqlTrans = null;

        try
        {
            conn = OpenConnection();
            npgsqlTrans = conn.BeginTransaction();
            
            conn = OpenConnection();

            homeworks.ForEach(h =>
            {
                var cmd = PostgresCommandHelper.InsertHomework(conn, Guid.NewGuid().ToString(), h);
                cmd.Transaction = npgsqlTrans;

                var rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected != 1)
                {
                    throw new Exception(
                        $"Unexpected exception writing to Homework table. Affected Rows: {rowsAffected}");
                }
            });
            
            return Ok(homeworks);
        }
        catch (Exception e)
        {
            npgsqlTrans?.Rollback();
            throw new Exception(e.Message);
        }
        finally
        {
            CloseConnection(conn);
        }
    }
}