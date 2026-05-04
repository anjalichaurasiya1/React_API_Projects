using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace product_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //  LOGIN API (WITH JWT)
        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginModel model)
        {
            try
            {
                string connStr = _configuration.GetConnectionString("DefaultConnection")
                    ?? throw new Exception("Connection string not found");

                using SqlConnection con = new SqlConnection(connStr);
                using SqlCommand cmd = new SqlCommand("SEARCH_LOGIN_React", con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@user", model.Username);
                cmd.Parameters.AddWithValue("@pwd", model.Password);
                cmd.Parameters.AddWithValue("@level", model.Level);
                

                con.Open();
                using SqlDataReader dr = cmd.ExecuteReader();

                if (!dr.Read())
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Invalid username or password"
                    });


                //  READ DATA FROM DB
                var userId = dr["EmpId"].ToString();
                var username = dr["Username"].ToString();
                var level = dr["Level"].ToString();
                var workPlace = dr["WorkPlace"].ToString();

                //  JWT CLAIMS
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username!),
                    new Claim("UserId", userId!),
                    new Claim(ClaimTypes.Role, level!),
                    new Claim("WorkPlace", workPlace!)
                };

                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
                );

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(
                        Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])
                    ),
                    signingCredentials: new SigningCredentials(
                        key, SecurityAlgorithms.HmacSha256
                    )
                );

                //  RETURN TOKEN + USER DATA
                return Ok(new
                {
                    message = "Login successful",
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    userId = userId,
                    username = username,
                    level = level,
                    workPlace = workPlace
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error",
                    error = ex.Message
                });

            }
        }

        //  PROTECTED API
        [Authorize]
        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            return Ok(new
            {
                message = "Authorized access",
                username = User.Identity?.Name,
                userId = User.FindFirst("UserId")?.Value,
                role = User.FindFirst(ClaimTypes.Role)?.Value,
                workPlace = User.FindFirst("WorkPlace")?.Value
            });
        }
    }

    //  SAME MODEL YOU USED INITIALLY
    public class LoginModel
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Level { get; set; }
       
    }
}
