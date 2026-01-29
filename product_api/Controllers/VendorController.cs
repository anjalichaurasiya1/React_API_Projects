using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace product_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendorController : ControllerBase
    {
        private readonly IConfiguration _config;

        public VendorController(IConfiguration config)
        {
            _config = config;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        }


    [HttpPost("savevendordetails")]
    public IActionResult SaveDetails([FromForm] VendorIdHolder model)
    {
        try{
            using SqlConnection con = GetConnection();  
            VendorModel vendorModel = new VendorModel();
       int vendoridfordetails=vendorModel.Id;
        using SqlCommand cmd = new SqlCommand("sp_InsertVendorDetails", con); 
        cmd.CommandType = CommandType.StoredProcedure; 
    
        cmd.Parameters.AddWithValue("@VendorId", vendoridfordetails); 
        cmd.Parameters.AddWithValue("@V_Name", model.VendorName ?? ""); 
        cmd.Parameters.AddWithValue("@New_Address", model.New_Address ?? ""); 
        cmd.Parameters.AddWithValue("@New_GSTNo", model.New_GSTNoGSTNo ?? ""); 
        cmd.Parameters.AddWithValue("@ApplicableFrom", model.ApplicableFrom ?? DateTime.Now); 
        cmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy ?? "");
         
             con.Open(); 
             cmd.ExecuteNonQuery();
              return Ok("Saved Successfully");
        }
        catch(Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
       
    }
[HttpPost("save")]
public IActionResult Save([FromForm] VendorModel model, IFormFile? file)
{
    try
    {
        using SqlConnection con = GetConnection();
        int vendorId;

        bool isUpdate = model.Id > 0;
        string spName = isUpdate ? "UPDATE_VENDOR" : "INSERT_VENDOR";

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
        cmd.Parameters.AddWithValue("@name", model.VendorName ?? "");
        cmd.Parameters.AddWithValue("@address", model.Address ?? "");
        cmd.Parameters.AddWithValue("@tel", model.Telephone ?? "");
        cmd.Parameters.AddWithValue("@VatTinno", model.VatTinNo ?? "");
        cmd.Parameters.AddWithValue("@ServiceTax", model.ServiceTax ?? "");
        cmd.Parameters.AddWithValue("@Panno", model.PanNo ?? "");
        cmd.Parameters.AddWithValue("@FSCCertificationNo", model.FscCertificationNo ?? "");
        cmd.Parameters.AddWithValue("@GSTNo", model.GSTNo ?? "");
        cmd.Parameters.AddWithValue("@EmailId", model.EmailId ?? "");
        cmd.Parameters.AddWithValue("@BankName", model.BankName ?? "");
        cmd.Parameters.AddWithValue("@AccountNo", model.AccountNo ?? "");
        cmd.Parameters.AddWithValue("@IfscCode", model.IfscCode ?? "");
        cmd.Parameters.AddWithValue("@Gst_Period", model.GstPeriod ?? "");
        cmd.Parameters.AddWithValue("@MSMERegistered", model.MSMERegistered ?? "");
        cmd.Parameters.AddWithValue("@MSMEFileName", fileName);
        cmd.Parameters.AddWithValue("@MSMEFilePath", filePath);

        // ================= ID HANDLING =================
        SqlParameter idParam = new SqlParameter("@id", SqlDbType.Int);
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


        // 🔹 GRID LIST
        [HttpGet("list")]
        public IActionResult GetVendors()
        {
            List<VendorModel> list = new List<VendorModel>();

            using SqlConnection con = GetConnection();
            using SqlCommand cmd = new SqlCommand("SP_Vendor_GetAll", con);
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();
            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                list.Add(new VendorModel
                {
                    Id = Convert.ToInt32(dr["id"]),
                    VendorName = dr["v_name"]?.ToString(),
                    Address = dr["address"]?.ToString(),
                    Telephone = dr["tel_no"]?.ToString(),
                    VatTinNo = dr["VatTinNo"]?.ToString(),
                    ServiceTax = dr["ServiceTax"]?.ToString(),
                    PanNo = dr["PanNo"]?.ToString(),
                    FscCertificationNo = dr["FscCertificationNo"]?.ToString(),
                    GSTNo = dr["GSTNo"]?.ToString(),
                    EmailId = dr["EmailId"]?.ToString(),
                    BankName = dr["BankName"]?.ToString(),
                    AccountNo = dr["AccountNo"]?.ToString(),
                    IfscCode = dr["IfscCode"]?.ToString(),
                    GstPeriod = dr["GST_Period"]?.ToString(),
                    MSMERegistered = dr["MSMERegistered"]?.ToString()
                });
            }

            return Ok(list);
        }

        // 🔹 DELETE
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            using SqlConnection con = GetConnection();
            using SqlCommand cmd = new SqlCommand("DELETE_VENDOR", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);

            con.Open();
            cmd.ExecuteNonQuery();

            return Ok("Deleted Successfully");
        }

       [HttpGet("Get/{id}")]
   public IActionResult GetDataById(int id, [FromQuery] string? name)
{
    VendorModel? vendor = null;

    using SqlConnection con = GetConnection();
    using SqlCommand cmd = new SqlCommand("SP_Vendor_GetById", con);
    cmd.CommandType = CommandType.StoredProcedure;

    cmd.Parameters.AddWithValue("@Id", id);
    cmd.Parameters.AddWithValue("@name", name ?? "");

    con.Open();

    using SqlDataReader dr = cmd.ExecuteReader();
    if (dr.Read())
    {
        vendor = new VendorModel
        {
            Id = Convert.ToInt32(dr["id"]),
            VendorName = dr["v_name"]?.ToString(),
            Address = dr["address"]?.ToString(),
            Telephone = dr["tel_no"]?.ToString(),
            VatTinNo = dr["VatTinNo"]?.ToString(),
            ServiceTax = dr["ServiceTax"]?.ToString(),
            PanNo = dr["PanNo"]?.ToString(),
            FscCertificationNo = dr["FscCertificationNo"]?.ToString(),
            GSTNo = dr["GSTNo"]?.ToString(),
            EmailId = dr["EmailId"]?.ToString(),
            BankName = dr["BankName"]?.ToString(),
            AccountNo = dr["AccountNo"]?.ToString(),
            IfscCode = dr["IfscCode"]?.ToString(),
            GstPeriod = dr["GST_Period"]?.ToString(),
            MSMERegistered = dr["MSMERegistered"]?.ToString()
        };
    }

    if (vendor == null)
        return NotFound("Vendor not found");

    return Ok(vendor);
}


    }

    // 🔹 MODEL
    public class VendorModel
    {
        public int Id { get; set; }
        public string? VendorName { get; set; }
        public string? Address { get; set; }
        public string? Telephone { get; set; }
        public string? VatTinNo { get; set; }
        public string? ServiceTax { get; set; }
        public string? PanNo { get; set; }
        public string? FscCertificationNo { get; set; }
        public string? GSTNo { get; set; }
        public string? EmailId { get; set; }
        public string? BankName { get; set; }
        public string? AccountNo { get; set; }
        public string? IfscCode { get; set; }
        public string? GstPeriod { get; set; }
        public string? MSMERegistered { get; set; }
        

    }
    public  class VendorIdHolder
    {
        public  int VendorId { get; set; }
        public string? VendorName { get; set; }
        public string? New_Address{get; set;}

        public string? New_GSTNoGSTNo { get; set; }
        public DateTime? ApplicableFrom{get; set;}
        public string? CreatedBy{get; set;}
    }
}
