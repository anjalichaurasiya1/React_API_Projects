using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace product_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubContractorController : ControllerBase
    {
        private readonly IConfiguration _config;

        public SubContractorController(IConfiguration config)
        {
            _config = config;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        }

        [HttpPost("save")]
        public IActionResult Save(SubContractorModel model)
        {
            try
            {
                using SqlConnection con = GetConnection();

                string spName = model.Id > 0 ? "UPDATE_SUBCONTRACTOR" : "INSERT_SUBCONTRACTOR";

                using SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;

               
                cmd.Parameters.AddWithValue("@Id", model.Id);
                cmd.Parameters.AddWithValue("@name", model.Name ?? "");
                cmd.Parameters.AddWithValue("@address", model.Address ?? "");
                cmd.Parameters.AddWithValue("@contactno", model.ContactNo ?? "");
                cmd.Parameters.AddWithValue("@PanNo", model.PanNo ?? "");
                cmd.Parameters.AddWithValue("@VatTin", model.TinNo ?? "");
                cmd.Parameters.AddWithValue("@Remark", model.Remark ?? "");

               
                SqlParameter errorCodeParam = new SqlParameter("@errorCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(errorCodeParam);

                SqlParameter errorMsgParam = new SqlParameter("@errorMsg", SqlDbType.VarChar, 200)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(errorMsgParam);

                con.Open();
                cmd.ExecuteNonQuery();

                int errorCode = errorCodeParam.Value != DBNull.Value
                    ? Convert.ToInt32(errorCodeParam.Value)
                    : 0;

                string errorMsg = errorMsgParam.Value?.ToString() ?? "";

                if (errorCode != 0)
                {
                    return BadRequest(errorMsg);
                }

                return Ok(new
                {
                    message = string.IsNullOrEmpty(errorMsg)
                        ? (model.Id > 0 ? "Updated Successfully" : "Inserted Successfully")
                        : errorMsg
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("Get")]
public IActionResult GetData(int? id, [FromQuery] string? name)
{
    List<SubContractorModel> list = new();

    using SqlConnection con = GetConnection();
    using SqlCommand cmd = new SqlCommand("SEARCH_SUBCONTRACTOR", con);
    cmd.CommandType = CommandType.StoredProcedure;

    
    cmd.Parameters.AddWithValue("@Id", id ?? (object)DBNull.Value);
    cmd.Parameters.AddWithValue("@name", string.IsNullOrEmpty(name) ? (object)DBNull.Value : name);

    con.Open();

    using SqlDataReader dr = cmd.ExecuteReader();
    while (dr.Read())
    {
        list.Add(new SubContractorModel
        {
            Id = Convert.ToInt32(dr["Id"]),
            Name = dr["name"]?.ToString(),
            Address = dr["address"]?.ToString(),
            ContactNo = dr["contactNo"]?.ToString(),
            TinNo = dr["TinNo"]?.ToString(),
            PanNo = dr["PanNo"]?.ToString(),
            Remark = dr["Remark"]?.ToString()
        });
    }

    return Ok(list);   
}

        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            using SqlConnection con = GetConnection();
            using SqlCommand cmd = new SqlCommand("DELETE_SUBCONTRACTOR", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);

            con.Open();
            cmd.ExecuteNonQuery();

            return Ok("Deleted Successfully");
        }

    }
}

public class SubContractorModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? ContactNo { get; set; }
    public string? PanNo { get; set; }
    public string? TinNo { get; set; }
    public string? Remark { get; set; }
}
