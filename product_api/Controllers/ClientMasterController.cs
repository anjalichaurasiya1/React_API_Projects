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



        [HttpGet("search-client")]
        public IActionResult SearchClient(long? clientId = null, string? name = null, string? city = null, string? branch = null)
        {
            try
            {
                List<ClientModel> list = new();

                using SqlConnection con = GetConnection();
                using SqlCommand cmd = new SqlCommand("SEARCH_CLIENT", con);
                cmd.CommandType = CommandType.StoredProcedure;

                // Parameters (same logic as your SP)
                if (clientId != null)
                    cmd.Parameters.AddWithValue("@clientId", clientId);

                if (!string.IsNullOrWhiteSpace(name))
                    cmd.Parameters.AddWithValue("@name", name);

                if (!string.IsNullOrWhiteSpace(city))
                    cmd.Parameters.AddWithValue("@City", city);

                if (!string.IsNullOrWhiteSpace(branch))
                    cmd.Parameters.AddWithValue("@Branch", branch);

                con.Open();
                using SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    ClientModel c = new();

                    // 🔹 When Branch NOT provided → Client Master
                    if (string.IsNullOrWhiteSpace(branch))
                    {
                        c.ClientId = Convert.ToInt32(dr["clientId"]);
                        c.ClientName = dr["Name"]?.ToString();
                        c.Address = dr["address"]?.ToString();
                        c.City = dr["city"]?.ToString();
                        c.VatTinNo = dr["VatTinNo"]?.ToString();
                        c.ServiceTax = dr["ServiceTax"]?.ToString();
                        c.PanNo = dr["Panno"]?.ToString();
                        c.PanDoc = dr["Filename"]?.ToString();
                        c.GSTNo = dr["GstNO"]?.ToString();
                        c.State = dr["State"]?.ToString();
                        c.CreditDays = dr["CreditPeriod"] != DBNull.Value ? Convert.ToInt32(dr["CreditPeriod"]) : (int?)null;
                        c.ShowMIS = dr["SHOW_MIS"]?.ToString();
                        c.Pin = dr["PinCode"]?.ToString();
                        c.Currency = dr["Currency"]?.ToString();
                        c.Type = dr["Type"]?.ToString();
                    }
                    // 🔹 When Branch provided → Contact Details
                    else
                    {
                        c.ClientId = Convert.ToInt32(dr["ClientId"]);
                        c.ContactName = dr["Name"]?.ToString();
                        c.ContactCity = dr["City"]?.ToString();
                    }

                    list.Add(c);
                }

                if (list.Count == 0)
                    return NotFound("No client found");

                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // 🔹 SAVE (INSERT/UPDATE)

       [HttpPost("save")]
public IActionResult Save([FromForm] ClientModel model, IFormFile? file)
{
    try
    {
        using SqlConnection con = GetConnection();
        int clientId;

        bool isUpdate = model.ClientId > 0;
        string spName = isUpdate ? "UPDATE_CLIENT" : "INSERT_CLIENT";

        using SqlCommand cmd = new SqlCommand(spName, con);
        cmd.CommandType = CommandType.StoredProcedure;

        // ================= FILE UPLOAD =================
        string fileName = "";
        string filePath = "";

        if (file != null && file.Length > 0)
        {
            string uploadDir = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "clients"
            );

            if (!Directory.Exists(uploadDir))
                Directory.CreateDirectory(uploadDir);

            fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            filePath = Path.Combine(uploadDir, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(stream);
        }

        // ================= COMMON INPUT =================
        cmd.Parameters.AddWithValue("@name", model.ClientName ?? "");
        cmd.Parameters.AddWithValue("@address", model.Address ?? "");
       cmd.Parameters.AddWithValue("@city", model.City ?? "");  
        cmd.Parameters.AddWithValue("@VatTin", model.VatTinNo ?? "");
        cmd.Parameters.AddWithValue("@Servicetax", model.ServiceTax ?? "");
        cmd.Parameters.AddWithValue("@Panno", model.PanNo ?? "");
        cmd.Parameters.AddWithValue("@FilePath", filePath);
        cmd.Parameters.AddWithValue("@fileName", fileName);
        cmd.Parameters.AddWithValue("@GSTNo", model.GSTNo ?? "");
       cmd.Parameters.AddWithValue("@State", model.State ?? "");
        cmd.Parameters.AddWithValue("@CreditPeriod", model.CreditDays ?? 0);
        cmd.Parameters.AddWithValue("@PinNo", model.Pin ?? "");
        cmd.Parameters.AddWithValue("@Currency", model.Currency ?? "");
        cmd.Parameters.AddWithValue("@Type", model.Type ?? "");
                if (!isUpdate)
                {
                    cmd.Parameters.AddWithValue("@ShowMIS", model.ShowMIS ?? "1");
                }

                // ================= ID HANDLING =================
                SqlParameter idParam = new SqlParameter("@clientId", SqlDbType.Int);

        if (isUpdate)
        {
            idParam.Direction = ParameterDirection.Input;
            idParam.Value = model.ClientId;
        }
        else
        {
            idParam.Direction = ParameterDirection.Output;
        }

        cmd.Parameters.Add(idParam);

        // ================= OUTPUT PARAMS =================
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

        clientId = isUpdate ? model.ClientId : Convert.ToInt32(idParam.Value);
        int errorCode = Convert.ToInt32(errorCodeParam.Value);
        string errorMsg = errorMsgParam.Value?.ToString() ?? "";

        if (errorCode != 0)
            return BadRequest(errorMsg);

        return Ok(new
        {
            ClientId = clientId,
            Message = errorMsg,
            FileName = fileName
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

         public int? CreditDays { get; set; }

        public int Id { get; set; }
        
        public string? VatTinNo { get; set; }
        public string? ServiceTax { get; set; }
        public string? PanNo { get; set; }
        public string? PanDoc { get; set; }
        public string? GSTNo { get; set; }
        
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

