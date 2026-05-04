using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace product_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComboloadingController : ControllerBase
    {
        private readonly string _connectionString;

        public ComboloadingController(IConfiguration configuration)
        {
            // Read the actual connection string from appsettings.json (ConnectionStrings:DefaultConnection)
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? configuration["DefaultConnection"];
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        [HttpGet("GetCombo")]
        public async Task<IActionResult> GetCombo(string column)
        {
            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();

                    var parameters = new DynamicParameters();
                    parameters.Add("@Column", column);

                    var result = await con.QueryAsync<dynamic>(
                        "COMBO_LOADING",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}