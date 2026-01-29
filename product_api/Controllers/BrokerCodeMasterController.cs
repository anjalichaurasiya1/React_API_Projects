using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace product_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrokerCodeMasterController : ControllerBase
    {
        private readonly IConfiguration _config;

        public BrokerCodeMasterController(IConfiguration config)
        {
            _config = config;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        }
        // 🔹 SAVE (INSERT/UPDATE
            [HttpPost("save")]
        public IActionResult Save(BrokerCodeMasterModel model)
        {
            try
            {
                using SqlConnection con = GetConnection();

                string spName = model.Id > 0 ? "UPDATE_BROKER_CODE" : "INSERT_BROKER_CODE";

                using SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;

               
                cmd.Parameters.AddWithValue("@id", model.Id);
                cmd.Parameters.AddWithValue("@name", model.Name ?? "");
                cmd.Parameters.AddWithValue("@code", model.Code ?? "");
                cmd.Parameters.AddWithValue("@clearing", model.ClearingNo ?? "");
                cmd.Parameters.AddWithValue("@tel", model.TelephoneNo ?? "");
                cmd.Parameters.AddWithValue("@fax", model.Fax ?? "");
                cmd.Parameters.AddWithValue("@status", model.Status ?? "");
                cmd.Parameters.AddWithValue("@add", model.Address ?? "");

               
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

                int errorCode = errorCodeParam.Value != DBNull.Value ? (int)errorCodeParam.Value : -1;
                string errorMsg = errorMsgParam.Value != DBNull.Value ? (string)errorMsgParam.Value : "Unknown error";

                if (errorCode != 0)
                {
                    return BadRequest(new { errorCode, errorMsg });
                }

                return Ok(new { message = "Saved Successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        // 🔹 GRID LIST 
         [HttpGet("Get")]
public IActionResult GetData(int? id, [FromQuery] string? name, [FromQuery] string? code)
{
    List<BrokerCodeMasterModel> list = new();

    using SqlConnection con = GetConnection();
    using SqlCommand cmd = new SqlCommand("SEARCH_BROKER_CODE", con);
    cmd.CommandType = CommandType.StoredProcedure;

    
    cmd.Parameters.AddWithValue("@id", id ?? (object)DBNull.Value);
    cmd.Parameters.AddWithValue("@name", string.IsNullOrEmpty(name) ? (object)DBNull.Value : name);
    cmd.Parameters.AddWithValue("@code", string.IsNullOrEmpty(code) ? (object)DBNull.Value : code);

    con.Open();

    using SqlDataReader dr = cmd.ExecuteReader();
    while (dr.Read())
    {
        list.Add(new BrokerCodeMasterModel
        {
            Id = Convert.ToInt32(dr["Id"]),
            Name = dr["B_Name"]?.ToString(),
            Code = dr["Code"]?.ToString(),
            ClearingNo = dr["clearing_no"]?.ToString(),
            TelephoneNo = dr["tel_no"]?.ToString(),
            Fax = dr["fax"]?.ToString(),
            Status = dr["status"]?.ToString(),
            Address = dr["Add"]?.ToString()
        });
    }

    return Ok(list);   
}

        // 🔹 DELETE
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                using SqlConnection con = GetConnection();
                using SqlCommand cmd = new SqlCommand("DELETE_BROKER_CODE", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);

                con.Open();
                cmd.ExecuteNonQuery();

                return Ok(new { message = "Deleted Successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

public class BrokerCodeMasterModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? ClearingNo { get; set; }
    public string? TelephoneNo { get; set; }
    public string? Fax { get; set; }
    public string? Status { get; set; }
    public string? Address { get; set; }
}