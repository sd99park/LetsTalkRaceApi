using LetsTalkRaceApi.Models;
using LetsTalkRaceApi.Models.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace LetsTalkRaceApi.Controllers;

[ApiController]
[Route("api/profiles/v1")]
public class ProfilesController : LtrControllerBase
{
    private IConfiguration _configuration;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    
    public ProfilesController(IConfiguration config, SignInManager<IdentityUser> signInManager,
    UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) : base(config)
    {
        _configuration = config;
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet]
    [Route("/profile/{profileId}")]
    [ProducesResponseType(typeof(Profile), 201)]
    [ProducesResponseType(typeof(BadRequestResult), 400)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), 401)]
    public async Task<IActionResult> GetProfile()
    {
        return Ok("");
    }

    [HttpPost]
    [Route("/profile/{profileId}")]
    [ProducesResponseType(typeof(Profile), 201)]
    [ProducesResponseType(typeof(BadRequestResult), 400)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), 401)]
    public async Task<IActionResult> CreateProfile(string profileId, [FromBody] Profile request)
    {
        NpgsqlConnection conn = null;
        NpgsqlTransaction npgsqlTrans = null;

        try
        { 
            // Create Identity with temp password
            var controller = new LoginController(_configuration, _signInManager, _userManager, _roleManager);
            var tempPassword = Guid.NewGuid().ToString().Substring(1, 8);
            var result = await controller.Register(new RegisterRequest()
            {
                Email = request.Email,
                Password = tempPassword
            }) as OkObjectResult;
            
            // TODO: This might not work
            var identity = result?.Value as IdentityUser;
            
            // Create Profile
            
            // Send Creation email
            
            return Ok(identity);
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