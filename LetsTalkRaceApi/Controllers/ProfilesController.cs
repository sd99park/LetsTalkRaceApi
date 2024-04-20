using System.Text.Json;
using LetsTalkRaceApi.Interfaces;
using LetsTalkRaceApi.Models;
using LetsTalkRaceApi.Models.Helpers;
using LetsTalkRaceApi.Models.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace LetsTalkRaceApi.Controllers;

[ApiController]
[Route("api/profiles/v1")]
public class ProfilesController : LtrControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IMailService _mailService;
    private readonly LoginController _loginController;
    
    public ProfilesController(IConfiguration config, SignInManager<IdentityUser> signInManager,
    UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IMailService mailService) : base(config)
    {
        _userManager = userManager;
        _mailService = mailService;
        _loginController = new LoginController(config, signInManager, _userManager, roleManager);
    }

    [HttpGet]
    [Route("/profile")]
    [ProducesResponseType(typeof(Profile), 201)]
    [ProducesResponseType(typeof(BadRequestResult), 400)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), 401)]
    public async Task<IActionResult> GetAllProfiles()
    {
        NpgsqlConnection conn = null;

        try
        {
            conn = OpenConnection();

            var cmd = PostgresCommandHelper.GetAllProfiles(conn);
            var strResponse = Convert.ToString(cmd.ExecuteScalar());
            var profiles = string.IsNullOrWhiteSpace(strResponse) 
                ? new List<Profile>() 
                : JsonSerializer.Deserialize<List<Profile>>(strResponse);
            
            return Ok(profiles);
        }
        finally
        {
            CloseConnection(conn);
        } 
    }
    
    [HttpGet]
    [Route("/profile/{profileId}")]
    [ProducesResponseType(typeof(Profile), 201)]
    [ProducesResponseType(typeof(BadRequestResult), 400)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), 401)]
    public async Task<IActionResult> GetProfile(string profileId)
    {
        NpgsqlConnection conn = null;

        try
        {
            conn = OpenConnection();

            var cmd = PostgresCommandHelper.GetProfileByProfileId(conn, profileId);
            var strResponse = Convert.ToString(cmd.ExecuteScalar());
            var profile = string.IsNullOrWhiteSpace(strResponse) 
                ? new List<Profile>() 
                : JsonSerializer.Deserialize<List<Profile>>(strResponse);
            
            return Ok(profile?.FirstOrDefault());
        }
        finally
        {
            CloseConnection(conn);
        } 
    }

    // TODO : Make this a re-send invite email endpoint
    [HttpPost]
    [Route("/sendEmail")]
    [ProducesResponseType(typeof(Profile), 201)]
    [ProducesResponseType(typeof(BadRequestResult), 400)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), 401)]
    public async Task<IActionResult> SendEmail([FromBody] string name)
    {
        var mailData = new MailData()
        {
            Body = "Testing body",
            Subject = "Testing Subject",
            Email = "sd99park@gmail.com",
            UserName = "Testing Name",
            Password = "SomePassword"
        };
        _mailService.SendMail(mailData);

        return Ok("Email sent");
    }

    [HttpPost]
    [Route("/profile")]
    [ProducesResponseType(typeof(Profile), 201)]
    [ProducesResponseType(typeof(BadRequestResult), 400)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), 401)]
    public async Task<IActionResult> CreateProfile([FromBody] CreateProfileRequest request)
    {
        NpgsqlConnection conn = null;
        NpgsqlTransaction npgsqlTrans = null;

        try
        { 
            conn = OpenConnection();
            npgsqlTrans = conn.BeginTransaction();
            
            conn = OpenConnection();
            
            // Create Identity with temp password

            var tempPassword = GenerateRandomPassword(_userManager.PasswordValidators.FirstOrDefault());
            // TODO: verify this worked
            await _loginController.Register(new RegisterRequest()
            {
                Email = request.Email,
                Password = tempPassword,
                Role = request.Role
            });

            var user = await _userManager.FindByEmailAsync(request.Email);

            // Create Profile
            var createdProfile = new Profile(request, user);
            var cmd = PostgresCommandHelper.InsertProfile(conn, createdProfile);
            cmd.Transaction = npgsqlTrans;
            var rowsAffected = cmd.ExecuteNonQuery();
            if (rowsAffected != 1)
            {
                throw new Exception($"Unexpected exception writing to Profiles table. Affected Rows: {rowsAffected}");
            }
            
            // Send Creation email
            var mailData = new MailData()
            {
                Body = "Some body",
                Subject = "Account Creation",
                Email = request.Email,
                UserName = $"{request.FirstName} {request.LastName}",
                Password = tempPassword
            };
            var mailSent = _mailService.SendMail(mailData);
            if (!mailSent)
            {
                throw new Exception("There was an error sending account creation email");
            } 
            
            return Ok(createdProfile);
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
    
    [HttpDelete]
    [Route("/profile/{profileId}")]
    [ProducesResponseType(typeof(Profile), 201)]
    [ProducesResponseType(typeof(BadRequestResult), 400)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), 401)]
    public async Task<IActionResult> DeleteProfile(string profileId)
    {
        NpgsqlConnection conn = null;
        NpgsqlTransaction npgsqlTrans = null;

        try
        { 
            conn = OpenConnection();
            npgsqlTrans = conn.BeginTransaction();
            
            conn = OpenConnection();

            // TODO: Verify this worked
            await _loginController.DeleteIdentity(profileId);

            var cmd = PostgresCommandHelper.DeleteProfile(conn, profileId);
            cmd.Transaction = npgsqlTrans;
            var rowsAffected = cmd.ExecuteNonQuery();
            if (rowsAffected != 1)
            {
                throw new Exception($"Unexpected exception deleting from Profiles table. Affected Rows: {rowsAffected}");
            }

            return Ok("Profile Successfully deleted");
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
    
    // TODO: Needs update name, update email endpoints
    
    private string GenerateRandomPassword(IPasswordValidator<IdentityUser>? passwordValidator)
    {
        // Generate a random password
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%&*?";
        var password = new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());

        // Validate the generated password
        var validationResult = passwordValidator.ValidateAsync(_userManager, null, password).Result;

        // If the generated password meets the validation criteria, return it, otherwise try again
        return validationResult.Succeeded ? password :
            GenerateRandomPassword(passwordValidator);
    }
}