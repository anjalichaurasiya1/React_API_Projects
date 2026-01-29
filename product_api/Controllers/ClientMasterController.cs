using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace product_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientMasterController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ClientMasterController(IConfiguration config)
        {
            _config = config;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        }


//     [HttpPost("savevendordetails")]
// public IActionResult SaveVendorDetails([FromBody] VendorDetailsModel model)
// {
//     try
//     {
//         using SqlConnection con = GetConnection();
//         using SqlCommand cmd = new SqlCommand("sp_InsertVendorDetails", con);
//         cmd.CommandType = CommandType.StoredProcedure;

//         cmd.Parameters.AddWithValue("@VendorId", model.VendorId);
//         cmd.Parameters.AddWithValue("@V_Name", model.VendorName ?? "");
//         cmd.Parameters.AddWithValue("@New_Address", model.New_Address ?? "");
//         cmd.Parameters.AddWithValue("@New_GSTNo", model.New_GSTNo ?? "");
//         cmd.Parameters.AddWithValue("@ApplicableFrom", model.ApplicableFrom);
//         cmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy ?? "");

//         con.Open();
//         cmd.ExecuteNonQuery();

//         return Ok(new { message = "Saved Successfully" });
//     }
//     catch (Exception ex)
//     {
//         return StatusCode(500, ex.Message);
//     }
// }
        // 🔹 SAVE (INSERT/UPDATE)
[HttpPost("save")]
public IActionResult Save([FromForm] ClientModel model, IFormFile? file)
{
    try
    {
        using SqlConnection con = GetConnection();
        int vendorId;

        bool isUpdate = model.Id > 0;
        string spName = isUpdate ? "UPDATE_CLIENT" : "INSERT_CLIENT";

        using SqlCommand cmd = new SqlCommand(spName, con);
        cmd.CommandType = CommandType.StoredProcedure;
        

        // ================= FILE =================
        string fileName = "";
        string filePath = "";

        if (file != null && file.Length > 0)
        {
            string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadDir))
                Directory.CreateDirectory(uploadDir);

            fileName = Path.GetFileName(file.FileName);
            filePath = Path.Combine(uploadDir, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(stream);
        }

        // ================= COMMON INPUT =================
        cmd.Parameters.AddWithValue("@name", model.ClientName ?? "");
        cmd.Parameters.AddWithValue("@address", model.Address ?? "");
        cmd.Parameters.AddWithValue("@city", model.Telephone ?? "");
        cmd.Parameters.AddWithValue("@VatTin", model.VatTinNo ?? "");
        cmd.Parameters.AddWithValue("@Servicetax", model.ServiceTax ?? "");
        cmd.Parameters.AddWithValue("@Panno", model.PanNo ?? "");
        cmd.Parameters.AddWithValue("@fileName", fileName);
        cmd.Parameters.AddWithValue("@FilePath", filePath);
        cmd.Parameters.AddWithValue("@GSTNo", model.GSTNo ?? "");
        cmd.Parameters.AddWithValue("@State", model.EmailId ?? "");
        cmd.Parameters.AddWithValue("@CreditPeriod", model.CreditDays ?? "");
        cmd.Parameters.AddWithValue("@PinNo", model.Pin ?? "");
        cmd.Parameters.AddWithValue("@Currency", model.Currency ?? "");
        cmd.Parameters.AddWithValue("@Type", model.Type ?? "");
        cmd.Parameters.AddWithValue("@ShowMIS", model.ShowMIS ?? "");
        
        

        // ================= ID HANDLING =================
        SqlParameter idParam = new SqlParameter("@clientId", SqlDbType.Int);
        if (isUpdate)
        {
            idParam.Direction = ParameterDirection.Input;
            idParam.Value = model.Id;
        }
        else
        {
            idParam.Direction = ParameterDirection.Output;
        }
        cmd.Parameters.Add(idParam);

        // ================= OUTPUT =================
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

        // ================= EXECUTE =================
        con.Open();
        cmd.ExecuteNonQuery();

         vendorId = isUpdate ? model.Id : Convert.ToInt32(idParam.Value);
        int errorCode = Convert.ToInt32(errorCodeParam.Value);
        string errorMsg = errorMsgParam.Value?.ToString() ?? "";

        if (errorCode != 0)
            return BadRequest(errorMsg);

        return Ok(new
        {
            VendorId = vendorId,
            Message = errorMsg
        });
    }
    catch (Exception ex)
    {
        return StatusCode(500, ex.Message);
    }
}


        

        // 🔹 DELETE
       [HttpDelete("delete/{id}")]
public IActionResult Delete(int id, [FromHeader] string department)
{
    try
    {
        using SqlConnection con = GetConnection();
        con.Open();

        // ================= CHECK DELETE ACCESS =================
        using (SqlCommand cmdAccess = new SqlCommand("DELETE_ACCESS", con))
        {
            cmdAccess.CommandType = CommandType.StoredProcedure;
            cmdAccess.Parameters.AddWithValue("@Department", department);
            cmdAccess.Parameters.AddWithValue("@Action", "DELETE");

            using SqlDataAdapter da = new SqlDataAdapter(cmdAccess);
            DataTable dtAccess = new DataTable();
            da.Fill(dtAccess);

            string res = dtAccess.Rows.Count > 0 ? dtAccess.Rows[0]["Res"]?.ToString()?.ToUpper() ?? "" : "";

        if (res != "YES")
        {
             return Unauthorized("You do not have permission to delete the client. Please contact admin.");
            }

        }

        // =================  CHECK CLIENT DEPENDENCY =================
        using (SqlCommand cmdCheck = new SqlCommand("CHECK_CLIENTID_BEFOREDELETE", con))
        {
            cmdCheck.CommandType = CommandType.StoredProcedure;
            cmdCheck.Parameters.AddWithValue("@ClientId", id);

            using SqlDataAdapter da = new SqlDataAdapter(cmdCheck);
            DataTable dtCheck = new DataTable();
            da.Fill(dtCheck);

            string checkResult = dtCheck.Rows.Count > 0 ? dtCheck.Rows[0][0]?.ToString()?.ToUpper() ?? "" : "";
            if (checkResult != "YES" && checkResult != "")
            {
                return BadRequest(checkResult);
            }
        }

        // ================= 3️⃣ DELETE CLIENT =================
        using (SqlCommand cmdDelete = new SqlCommand("DELETE_CLIENT", con))
        {
            cmdDelete.CommandType = CommandType.StoredProcedure;
            cmdDelete.Parameters.AddWithValue("@clientId", id);
            cmdDelete.ExecuteNonQuery();
        }

        return Ok("Client deleted successfully");
    }
    catch (SqlException ex)
    {
        return StatusCode(500, ex.Message);
    }
}


       [HttpGet("Get")]
public IActionResult GetData(int? clientId, int? id, string? name, string? city,string? pin,string? state)
{
    List<ClientModel> list = new();

    using SqlConnection con = GetConnection();
    using SqlCommand cmd = new SqlCommand("SEARCH_CLIENTDETAILS", con);
    cmd.CommandType = CommandType.StoredProcedure;

    if (clientId.HasValue)
        cmd.Parameters.AddWithValue("@ClientId", clientId.Value);
    else
        cmd.Parameters.AddWithValue("@ClientId", DBNull.Value);

    if (id.HasValue)
        cmd.Parameters.AddWithValue("@Id", id.Value);
    else
        cmd.Parameters.AddWithValue("@Id", DBNull.Value);

    if (!string.IsNullOrWhiteSpace(name))
        cmd.Parameters.AddWithValue("@ContactName", name);
    else
        cmd.Parameters.AddWithValue("@ContactName", DBNull.Value);

    if (!string.IsNullOrWhiteSpace(city))
        cmd.Parameters.AddWithValue("@City", city);
    else
        cmd.Parameters.AddWithValue("@City", DBNull.Value);

    if (!string.IsNullOrWhiteSpace(pin))
        cmd.Parameters.AddWithValue("@Pin", pin);
    else
        cmd.Parameters.AddWithValue("@Pin", DBNull.Value);

    if (!string.IsNullOrWhiteSpace(state))
        cmd.Parameters.AddWithValue("@State", state);
    else
        cmd.Parameters.AddWithValue("@State", DBNull.Value);

    con.Open();
    using SqlDataReader dr = cmd.ExecuteReader();

    while (dr.Read())
    {
        list.Add(new ClientModel
        {
            Id = Convert.ToInt32(dr["Id"]),
            ClientId = Convert.ToInt32(dr["ClientId"]),
            ContactName = dr["Contactname"]?.ToString(),
            Address = dr["Address"]?.ToString(),
            City = dr["City"]?.ToString(),
            Pin = dr["Pin"]?.ToString(),
            State = dr["State"]?.ToString(),
            Department = dr["Department"]?.ToString(),
            Telephone = dr["Telno"]?.ToString(),
            MobileNo = dr["MobileNo"]?.ToString(),
            EmailId = dr["EmailId"]?.ToString(),
            GSTNo = dr["GSTNO"]?.ToString(),
            ClientName = dr["ClientName"]?.ToString()
        });
    }

    return Ok(list);
}

  }

    // 🔹 MODEL
    public class ClientModel
    {
        public int ClientId { get; set; }
        public string? ClientName { get; set; }
        public string? ClientAddress { get; set; }
        public string? ClientGSTNo { get; set; }
        public string? ContactName { get; set; }
        public string? City { get; set; }
        public string? Pin { get; set; }
        public string? State { get; set; }
        public string? Type { get; set; }



        public int Id { get; set; }
        
        public string? VatTinNo { get; set; }
        public string? ServiceTax { get; set; }
        public string? PanNo { get; set; }
        public string? PanDoc { get; set; }
        public string? GSTNo { get; set; }
        public string? CreditDays { get; set; }
        public string? Currency { get; set; }

        public string? ShowMIS { get; set; }



        public string? Address { get; set; }
        public string? ContactPin { get; set; }
        public string? ContactCity { get; set; }
        public string? ContactState { get; set; }
        public string? Designation { get; set; }
        public string? Department { get; set; }
        public string? Telephone { get; set; }
        public string? MobileNo { get; set; }
        public string? EmailId { get; set; }
        public string? ContactGSTNo { get; set; }
    }
    
}

