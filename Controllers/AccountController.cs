using Cards.DTO;
using Cards.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Cards.Controllers
{
    [Route("api/[controller][action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<ApiUser> _userManager;


        public AccountController(
            ApplicationDbContext context, 
            IConfiguration configuration, 
            ILogger<AccountController> logger,
            UserManager<ApiUser> userManager)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginDTO input)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByEmailAsync(input.EmailAddress);
                    if (user == null
                        || !await _userManager.CheckPasswordAsync(
                                user, input.Password))
                        throw new Exception("Invalid login attempt.");
                    else
                    {
                        var signingCredentials = new SigningCredentials(
                            new SymmetricSecurityKey(
                                System.Text.Encoding.UTF8.GetBytes(
                                    _configuration["JWT:SigningKey"])),
                            SecurityAlgorithms.HmacSha256);

                        var claims = new List<Claim>();
                        claims.Add(new Claim(
                            ClaimTypes.Email, user.Email));
                        claims.Add(new Claim(
                           ClaimTypes.Name, user.UserName));
                        claims.Add(new Claim(
                            "userId", user.Id));
                        claims.AddRange(
                            (await _userManager.GetRolesAsync(user))
                                .Select(r => new Claim(ClaimTypes.Role, r)));

                        var jwtObject = new JwtSecurityToken(
                            issuer: _configuration["JWT:Issuer"],
                            audience: _configuration["JWT:Audience"],
                            claims: claims,
                            expires: DateTime.Now.AddSeconds(300),
                            signingCredentials: signingCredentials);

                        var jwtString = new JwtSecurityTokenHandler()
                            .WriteToken(jwtObject);

                        _logger.LogInformation("Successfully logged in user.");
                        return StatusCode(
                            StatusCodes.Status200OK,
                            jwtString);
                    }
                }
                else
                {
                    var details = new ValidationProblemDetails(ModelState);
                    details.Type =
                            "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                    details.Status = StatusCodes.Status400BadRequest;
                    return new BadRequestObjectResult(details);
                }
            }
            catch (Exception e)
            {
                var exceptionDetails = new ProblemDetails();
                exceptionDetails.Detail = e.Message;
                exceptionDetails.Status =
                    StatusCodes.Status401Unauthorized;
                exceptionDetails.Type =
                        "https://tools.ietf.org/html/rfc7231#section-6.6.1";
                return StatusCode(
                    StatusCodes.Status401Unauthorized,
                    exceptionDetails);
            }
        }
    }
}
