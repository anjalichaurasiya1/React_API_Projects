using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace product_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeDetailsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public EmployeeDetailsController(IConfiguration config)
        {
            _config = config;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        }

        [HttpPost("Insert_Salary_rate")]
        public IActionResult Insert_Salary_rate([FromBody] EmployeeDetailsModel model)
        {
            try
            {
                using SqlConnection con = GetConnection();
                using SqlCommand cmd = new SqlCommand("Insert_Salary_rate", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Employee_ID", model.EmployeeId);
                cmd.Parameters.AddWithValue("@Basic", model.Bassic);
                cmd.Parameters.AddWithValue("@HRA", model.HRA);
                cmd.Parameters.AddWithValue("@Medical", model.Medical);
                cmd.Parameters.AddWithValue("@Conveyance", model.Conveyavce);
                cmd.Parameters.AddWithValue("@CCA", model.CCA);
                cmd.Parameters.AddWithValue("@Other", model.Other);
                cmd.Parameters.AddWithValue("@LTA", model.LTA);
                cmd.Parameters.AddWithValue("@PL", model.PL);
                cmd.Parameters.AddWithValue("@SL", model.SL);
                cmd.Parameters.AddWithValue("@CL", model.CL);
                
               
                cmd.Parameters.AddWithValue("@Date", model.Date ?? "");
                cmd.Parameters.AddWithValue("@Username", model.Username ?? "");
                cmd.Parameters.AddWithValue("@DutyHours", model.Duty ?? "");
                cmd.Parameters.AddWithValue("@Minimum_DutyHours", model.Minimum_Duty ?? "");
                cmd.Parameters.AddWithValue("@LunchHours", model.LunchHours ?? "");
                cmd.Parameters.AddWithValue("@OT_Applicable", model.OT_Applicable ?? "");
                cmd.Parameters.AddWithValue("@IsShiftApplicable", model.IsShiftApplicable ?? "");
                cmd.Parameters.AddWithValue("@NPS", model.NpsDeduction ?? "");
                
                

                con.Open();
                cmd.ExecuteNonQuery();

                return Ok(new { message = "Salary Rate Inserted Successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("InsertEmployeeAssets")]
public IActionResult InsertEmployeeAssets([FromForm] EmployeeDetailsModel model)
{
    try
    {
        using SqlConnection con = GetConnection();

        // Decide SP
        string spName = model.Id > 0
            ? "Update_EmployeeAssets"
            : "Insert_EmployeeAssets";

        using SqlCommand cmd = new SqlCommand(spName, con);
        cmd.CommandType = CommandType.StoredProcedure;

        // ===== SAME PARAMETERS AS OLD CODE =====
        cmd.Parameters.AddWithValue("@EmpId", model.EmployeeId);
        cmd.Parameters.AddWithValue("@EmployeeName", model.Name ?? "");
        cmd.Parameters.AddWithValue("@Assets", model.AssetsName ?? "");
        cmd.Parameters.AddWithValue("@Received", model.Received ?? "");
        cmd.Parameters.AddWithValue("@AllocatedBy", model.AllocatedBy ?? "");

        // ===== ID PARAM =====
        SqlParameter idParam = new SqlParameter("@ID", SqlDbType.VarChar, 50);
        if (model.Id > 0)
        {
            idParam.Direction = ParameterDirection.Input;
            idParam.Value = model.Id.ToString();
        }
        else
        {
            idParam.Direction = ParameterDirection.Output;
        }
        cmd.Parameters.Add(idParam);

        // ===== OUTPUT PARAMS =====
        SqlParameter errorCode = new SqlParameter("@errorCode", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        cmd.Parameters.Add(errorCode);

        SqlParameter errorMsg = new SqlParameter("@errorMsg", SqlDbType.VarChar, 255)
        {
            Direction = ParameterDirection.Output
        };
        cmd.Parameters.Add(errorMsg);

        // ===== EXECUTE =====
        con.Open();
        cmd.ExecuteNonQuery();

        int errCode = Convert.ToInt32(errorCode.Value ?? 0);
        string msg = errorMsg.Value?.ToString() ?? "";

        if (errCode != 0)
            return BadRequest(msg);

        return Ok(msg == "" ? "Asset saved successfully" : msg);
    }
    catch (Exception ex)
    {
        return StatusCode(500, ex.Message);
    }
}

        [HttpGet("CheckEmployeeLeavingDate")]
        public IActionResult CheckEmployeeLeavingDate(int empId)
        {
            try
            {
                DataTable dt = new DataTable();

                using SqlConnection con = GetConnection();
                using SqlCommand cmd = new SqlCommand("Check_Employee_LeavingDate", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@EmpId", empId);

                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                    return NotFound("No leaving date found for this employee");

                return Ok(dt);   // returned as JSON
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        // [HttpGet("GetEmployeeAssetsByEmpId")]
        // public IActionResult GetEmployeeAssetsByEmpId(int empId)
        // {
        //     try
        //     {
        //         DataTable dt = new DataTable();

        //         using SqlConnection con = GetConnection();
        //         using SqlCommand cmd = new SqlCommand("Get_EmployeeAssetsByEmpId", con);
        //         cmd.CommandType = CommandType.StoredProcedure;

        //         cmd.Parameters.AddWithValue("@EmpId", empId);

        //         using SqlDataAdapter da = new SqlDataAdapter(cmd);
        //         da.Fill(dt);

        //         if (dt.Rows.Count == 0)
        //             return NotFound("No assets found for this employee");

        //         return Ok(dt);   // returned as JSON
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(500, ex.Message);
        //     }
        // }

        [HttpGet("GetEmployeeAssetsByEmpId")]
        public IActionResult GetEmployeeAssetsByEmpId(int empId)
        {
            try
            {
                DataTable dt = new DataTable();

                using SqlConnection con = GetConnection();
                using SqlCommand cmd = new SqlCommand("Get_EmployeeAssetsByEmpId", con);
                cmd.CommandType = CommandType.StoredProcedure;

                // SAME CONDITIONS AS ASP.NET CODE
                if (empId != 0)
                    cmd.Parameters.AddWithValue("@EmpId", empId);

                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                    return NotFound("No details found");

                 return Ok(DataTableToList(dt));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        // [HttpGet("Search_Edit_Employee_Details")]
        // public IActionResult Search_Edit_Employee_Details(int empId, int id, string? name, string? DateOfJoining,string? DateOfLeaving,string? Date_of_Birth)
        // {
        //     try
        //     {
        //         DataTable dt = new DataTable();
        //         using SqlConnection con = GetConnection();
        //         using SqlCommand cmd = new SqlCommand("Search_Edit_Employee_Details", con);
        //         cmd.CommandType = CommandType.StoredProcedure;
        //         cmd.Parameters.AddWithValue("@Id", id);
        //         cmd.Parameters.AddWithValue("@Employee_Name", name ?? "");

        //         cmd.Parameters.AddWithValue("@Employee_ID", empId);
        //         cmd.Parameters.AddWithValue("@Date_of_joining", DateOfJoining ?? "");
        //         cmd.Parameters.AddWithValue("@Date_of_leving", DateOfLeaving ?? "");
        //         cmd.Parameters.AddWithValue("@Date_of_Birth", Date_of_Birth ?? "");


        //         using SqlDataAdapter da = new SqlDataAdapter(cmd);
        //         da.Fill(dt);
        //         if (dt.Rows.Count == 0)
        //             return NotFound("No details found for this employee");
        //         return Ok(dt);   // returned as JSON
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(500, ex.Message);
        //     }
        // }



        

        [HttpGet("Search_Edit_Employee_Details")]
        public IActionResult Search_Edit_Employee_Details(int id = 0, int employeeId = 0, string? employeeName = null, string? Date_of_Joining = null, string? Date_of_leving = null, string? Date_of_Birth = null)
        {
            try
            {
                DataTable dt = new DataTable();

                using SqlConnection con = GetConnection();
                using SqlCommand cmd = new SqlCommand("Search_Edit_Employee_Details", con);
                cmd.CommandType = CommandType.StoredProcedure;

                // SAME CONDITIONS AS ASP.NET CODE
                if (id != 0)
                    cmd.Parameters.AddWithValue("@Id", id);

                if (employeeId != 0)
                    cmd.Parameters.AddWithValue("@Employee_ID", employeeId);

                if (!string.IsNullOrEmpty(employeeName))
                    cmd.Parameters.AddWithValue("@Employee_Name", employeeName);

                if (!string.IsNullOrEmpty(Date_of_Joining))
                    cmd.Parameters.AddWithValue("@Date_of_Joining", Date_of_Joining);

                if (!string.IsNullOrEmpty(Date_of_leving))
                    cmd.Parameters.AddWithValue("@Date_of_leving", Date_of_leving);

                if (!string.IsNullOrEmpty(Date_of_Birth))
                    cmd.Parameters.AddWithValue("@Date_of_Birth", Date_of_Birth);

                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                    return NotFound("No details found");

                 return Ok(DataTableToList(dt));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



        [HttpGet("GetSalaryRateDataByEmpId")]
        public IActionResult GetSalaryRateDataByEmpId(int empcode = 0)
        {
            try
            {
                DataTable dt = new DataTable();

                using SqlConnection con = GetConnection();
                using SqlCommand cmd = new SqlCommand("GET_SALARyRATE", con);
                cmd.CommandType = CommandType.StoredProcedure;

                // SAME CONDITIONS AS ASP.NET CODE
                if (empcode != 0)
                    cmd.Parameters.AddWithValue("@EmpCode", empcode);

                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                    return NotFound("No details found");

                 return Ok(DataTableToList(dt));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        private List<Dictionary<string, object?>> DataTableToList(DataTable dt)
{
    List<Dictionary<string, object?>> list = new();

    foreach (DataRow row in dt.Rows)
    {
        Dictionary<string, object?> dict = new();

        foreach (DataColumn col in dt.Columns)
        {
            dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
        }

        list.Add(dict);
    }

    return list;
}


        [HttpGet("Search_Employee_Details")]
        public IActionResult Search_Employee_Details(int id = 0, int employeeId = 0, string? employeeName = null, string? workPlace = null, string? category = null, string? level = null)
        {
            try
            {
                DataTable dt = new DataTable();

                using SqlConnection con = GetConnection();
                using SqlCommand cmd = new SqlCommand("Search_Employee_Details", con);
                cmd.CommandType = CommandType.StoredProcedure;

                // SAME CONDITIONS AS ASP.NET CODE
                if (id != 0)
                    cmd.Parameters.AddWithValue("@Id", id);

                if (employeeId != 0)
                    cmd.Parameters.AddWithValue("@Employee_ID", employeeId);

                if (!string.IsNullOrEmpty(employeeName))
                    cmd.Parameters.AddWithValue("@Employee_Name", employeeName);

                if (!string.IsNullOrEmpty(workPlace))
                    cmd.Parameters.AddWithValue("@WorkLocation", workPlace);

                if (!string.IsNullOrEmpty(category))
                    cmd.Parameters.AddWithValue("@Category", category);

                if (!string.IsNullOrEmpty(level))
                    cmd.Parameters.AddWithValue("@Level", level);

                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                    return NotFound("No details found");

                 return Ok(DataTableToList(dt));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("Load_Department")]
        public IActionResult Load_Department()
        {
            try
            {
                EmployeeDetailsModel obj = new EmployeeDetailsModel();
                DataTable dt = new DataTable();
                using SqlConnection con = GetConnection();
                using SqlCommand cmd = new SqlCommand("Get_Departments_Combo", con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", obj.Id);
                cmd.Parameters.AddWithValue("@Departments", obj.Department ?? "");

                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count == 0)
                    return NotFound("No departments found");
                return Ok(dt);   // returned as JSON
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
         [HttpDelete("delete/{id}")]
        public IActionResult Delete_Employee_Details(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid Employee ID");

                using SqlConnection con = GetConnection();
                using SqlCommand cmd = new SqlCommand("Delete_Employee_Details", con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", id);

                con.Open();
                cmd.ExecuteNonQuery();

                return Ok("Employee deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        // [HttpGet("Load_EmployeeName")]
        // public IActionResult Load_EmployeeName()
        // {
        //     try
        //     {
        //         DataTable dt = new DataTable();
        //         using SqlConnection con = GetConnection();
        //         using SqlCommand cmd = new SqlCommand("Load_EmployeeName", con);
        //         cmd.CommandType = CommandType.StoredProcedure;
        //         using SqlDataAdapter da = new SqlDataAdapter(cmd);
        //         da.Fill(dt);
        //         if (dt.Rows.Count == 0)
        //             return NotFound("No employee names found");
        //         return Ok(dt);   // returned as JSON
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(500, ex.Message);
        //     }
        // }
        [HttpPost("InsertEmployeeAssetsData")]
public IActionResult Insert_EmployeeAssetsData([FromBody] EmployeeAssetAssignModel model)
{
    if (string.IsNullOrWhiteSpace(model.EmpId) || model.Assets == null || !model.Assets.Any())
        return BadRequest(new { message = "Invalid data" });

       string allocatedBy = HttpContext.Session.GetString("UserName") ?? "SYSTEM";

    try
    {
        using SqlConnection con = GetConnection();
        con.Open();

        foreach (var asset in model.Assets)
        {
            using SqlCommand cmd = new SqlCommand("Insert_EmployeeAssets", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@EmpId", model.EmpId);
            cmd.Parameters.AddWithValue("@EmployeeName", model.EmployeeName ?? "");
            cmd.Parameters.AddWithValue("@Assets", asset);
            cmd.Parameters.AddWithValue("@Received", "NO");
            cmd.Parameters.AddWithValue("@AllocatedBy", allocatedBy);

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

            cmd.ExecuteNonQuery();

            int errorCode = Convert.ToInt32(errorCodeParam.Value ?? 0);
            if (errorCode != 0)
            {
                string errorMsg = errorMsgParam.Value?.ToString() ?? "Asset insert failed";
                return BadRequest(new { message = errorMsg });
            }
        }

        return Ok(new { message = "Assets assigned successfully" });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { message = ex.Message });
    }
}

object GetDbValue(string? value)
{
    return string.IsNullOrEmpty(value) ? DBNull.Value : value;
}


        // 🔹 SAVE (INSERT/UPDATE
      [HttpPost("save")]
public IActionResult Save([FromForm] EmployeeDetailsModel model, IFormFile? file)
{
    try
    {
        using SqlConnection con = GetConnection();
        using SqlCommand cmd = new SqlCommand(
            model.Id != 0 ? "Update_Employee_Details" : "Insert_Employee_Details",
            con);

        cmd.CommandType = CommandType.StoredProcedure;

        // ================= ID =================
        if (model.Id != 0)
            cmd.Parameters.AddWithValue("@Id", model.Id);
        else
            cmd.Parameters.Add("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;

        // ================= BASIC =================
        if (model.EmployeeId != 0)
            cmd.Parameters.AddWithValue("@Employee_ID", model.EmployeeId);

        if (model.Name != null)
            cmd.Parameters.AddWithValue("@Employee_Name", model.Name);

        if (model.Surname != null)
            cmd.Parameters.AddWithValue("@Sur_Name", model.Surname);

        if (model.FatherName != null)
            cmd.Parameters.AddWithValue("@Father_Name", model.FatherName);

        if (model.Address != null)
            cmd.Parameters.AddWithValue("@Address", model.Address);

        if (model.Designation != null)
            cmd.Parameters.AddWithValue("@Designation", model.Designation);

        // ================= DATE =================
        if (model.DateOfBirth != null)
            cmd.Parameters.AddWithValue("@Date_of_Birth", model.DateOfBirth);

        if (model.Date_of_Letter != null)
            cmd.Parameters.AddWithValue("@Date_of_Letter", model.Date_of_Letter);

        if (model.DateOfJoining != null)
            cmd.Parameters.AddWithValue("@Date_of_Joining", model.DateOfJoining);

        if (model.DateOfLeaving != null)
            cmd.Parameters.AddWithValue("@Date_of_leving", model.DateOfLeaving);

        // ================= IDS =================
        if (model.PF_No != null)
            cmd.Parameters.AddWithValue("@PF_no", model.PF_No);

        if (model.ESIC_no != null)
            cmd.Parameters.AddWithValue("@ESIC_no", model.ESIC_no);

        if (model.UAN_No != null)
            cmd.Parameters.AddWithValue("@UAN_no", model.UAN_No);

        if (model.PanNo != null)
            cmd.Parameters.AddWithValue("@PAN_no", model.PanNo);

        if (model.AadharNo != null)
            cmd.Parameters.AddWithValue("@Aadhar_Card_no", model.AadharNo);

        // ================= CONTACT =================
        if (model.PermanentAddress != null)
            cmd.Parameters.AddWithValue("@Pernament_address", model.PermanentAddress);

        if (model.MobileNo != null)
            cmd.Parameters.AddWithValue("@Mobile_no", model.MobileNo);

        if (model.Email != null)
            cmd.Parameters.AddWithValue("@email_id", model.Email);

        // ================= BANK =================
        if (model.BankName != null)
            cmd.Parameters.AddWithValue("@Bank_Name", model.BankName);

        if (model.BankAccount != null)
            cmd.Parameters.AddWithValue("@Bank_Acount_No", model.BankAccount);

        if (model.IFSCCode != null)
            cmd.Parameters.AddWithValue("@IFSC_code", model.IFSCCode);

        // ================= HR =================
        if (model.Status != null)
            cmd.Parameters.AddWithValue("@Status", model.Status);

        if (model.Gender != null)
            cmd.Parameters.AddWithValue("@Gender", model.Gender);

        if (model.WorkPlace != null)
            cmd.Parameters.AddWithValue("@WorkPlace", model.WorkPlace);

        if (model.ApplyPF != null)
            cmd.Parameters.AddWithValue("@ApplyPF", model.ApplyPF);

        if (model.ApplyESIC != null)
            cmd.Parameters.AddWithValue("@ApplyESIC", model.ApplyESIC);

        if (model.Category != null)
            cmd.Parameters.AddWithValue("@Category", model.Category);

        if (model.Department != null)
            cmd.Parameters.AddWithValue("@Department", model.Department);

        // ================= NUMERIC (FIXED) =================
        if (model.Duty != null)
            cmd.Parameters.AddWithValue("@DutyHours", model.Duty);

        if (model.Minimum_Duty != null)
            cmd.Parameters.AddWithValue("@Minimum_DutyHours", model.Minimum_Duty);

        if (model.LunchHours != null)
            cmd.Parameters.AddWithValue("@LunchHours", model.LunchHours);

        if (model.NpsDeduction != null)
            cmd.Parameters.AddWithValue("@NPS_Deduction", model.NpsDeduction);

        if (model.OT_Applicable != null)
            cmd.Parameters.AddWithValue("@OT_Applicable", model.OT_Applicable);

        if (model.IsShiftApplicable != null)
            cmd.Parameters.AddWithValue("@IsShiftApplicable", model.IsShiftApplicable);

        if (model.Username != null)
            cmd.Parameters.AddWithValue("@CreatedBy", model.Username);

        // ================= FILE =================
        if (file != null)
        {
            string folder = Path.Combine("wwwroot", "uploads");
            Directory.CreateDirectory(folder);

            string path = Path.Combine(folder, file.FileName);
            using var fs = new FileStream(path, FileMode.Create);
            file.CopyTo(fs);

            cmd.Parameters.AddWithValue("@attachment", file.FileName);
            cmd.Parameters.AddWithValue("@path", path);
        }

        // ================= OUTPUT =================
        SqlParameter errorCode = new SqlParameter("@errorCode", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        cmd.Parameters.Add(errorCode);

        SqlParameter errorMsg = new SqlParameter("@errorMsg", SqlDbType.VarChar, 255)
        {
            Direction = ParameterDirection.Output
        };
        cmd.Parameters.Add(errorMsg);

        con.Open();
        cmd.ExecuteNonQuery();

        if (Convert.ToInt32(errorCode.Value ?? 0) != 0)
            return BadRequest(errorMsg.Value?.ToString());

        return Ok("Employee saved successfully");
    }
    catch (Exception ex)
    {
        return StatusCode(500, ex.Message);
    }
}



        [HttpGet("GetAssetsName")]
        public IActionResult GetAssetsName()
        {
            List<EmployeeDetailsModel> list = new();

            using SqlConnection con = GetConnection();
            using SqlCommand cmd = new SqlCommand("GetAssetsDetails", con);
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())   //  LOOP instead of if
            {
                list.Add(new EmployeeDetailsModel
                {
                    Id = Convert.ToInt32(dr["ID"]),
                    AssetsName = dr["AssetName"]?.ToString()
                });
            }

            return Ok(list);   //  Return list
        }

        [HttpGet("GetCalculateLeaves")]
        public IActionResult GetCalculateLeaves(int? employeeId)
        {
            List<EmployeeDetailsModel> list = new();

            using SqlConnection con = GetConnection();
            using SqlCommand cmd = new SqlCommand("Get_Calculated_Leaves", con);
            cmd.CommandType = CommandType.StoredProcedure;

             cmd.Parameters.AddWithValue("@EmpId", employeeId);


            con.Open();

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())   //  LOOP instead of if
            {
                list.Add(new EmployeeDetailsModel
                {
                    PL = Convert.ToInt32(dr["PL"]),
                    SL = Convert.ToInt32(dr["SL"]),
                    CL = Convert.ToInt32(dr["Cl"]),
                });
            }

            return Ok(list);   //  Return list
        }

        [HttpGet("GetAssest/{id}")]
        public IActionResult GetDataById(int id)
        {
            EmployeeDetailsModel? model = null;

            using SqlConnection con = GetConnection();
            using SqlCommand cmd = new SqlCommand("Get_EmployeeAssetsByEmpId", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@EmpId", id);


            con.Open();

            using SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                model = new EmployeeDetailsModel
                {
                    Id = Convert.ToInt32(dr["id"]),
                    EmpId = Convert.ToInt32(dr["EmpId"]),
                    AssetsName = dr["AssetName"]?.ToString()
                    // IsActive = dr["IsActive"]?.ToString()
                };
            }

            if (model == null)
                return NotFound("Assests not found");

            return Ok(model);
        }






        [HttpGet("GetDepartmentById")]
        public IActionResult GetData(int? id, [FromQuery] string? department)
        {
            List<EmployeeDetailsModel> list = new();

            using SqlConnection con = GetConnection();
            using SqlCommand cmd = new SqlCommand("Get_Departments_Combo", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", id ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Departments", string.IsNullOrEmpty(department) ? (object)DBNull.Value : department);

            con.Open();

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())   //  LOOP instead of if
            {
                list.Add(new EmployeeDetailsModel
                {
                    Id = Convert.ToInt32(dr["id"]),
                    Department = dr["Department"]?.ToString()
                });
            }

            return Ok(list);   //  Return list
        }

    }
    public class EmployeeDetailsModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? FatherName { get; set; }
        public string? Designation { get; set; }
        
       public string? DateOfBirth { get; set; }
    public string? DateOfJoining { get; set; }
    public string? DateOfLeaving { get; set; }
        
         public string? Gender { get; set; }
    public string? Address { get; set; }
    public string? PermanentAddress { get; set; }
         public string? PanNo { get; set; }
    public string? AadharNo { get; set; }
    public string? MobileNo { get; set; }
    public string? Email { get; set; }

    public string? ApplyPF { get; set; }
    public string? ApplyESIC { get; set; }
    public string? NpsDeduction { get; set; }

    public string? WorkPlace { get; set; }
    public string? Category { get; set; }
    public string? Department { get; set; }
    public string? BankName { get; set; }
    public string? BankAccount { get; set; }
    public string? IFSCCode { get; set; }
        public string? PF_No { get; set; }
        public string? ESIC_no { get; set; }
        
       
        public string? UAN_No { get; set; }
       
        
        
       
        //public string? Bank_Account_No { get; set; }
        //public string? IFSC_Code { get; set; }
        public string? Status { get; set; }
        
        
       

       
       
        public string? Employee_picture { get; set; }
        
        //public string? NPS_DEDUCTION{ get; set; }
        public string? Date_of_Letter { get; set; }
       
        public DateTime? LetterDate { get; set; }
        public string? Level { get; set; }
        public int? PL { get; set; }
        public int? SL { get; set; }
        public int? CL { get; set; }
        public string? AssetsName { get; set; }
        public string? IsActive { get; set; }
        public int? EmpId { get; set; }
        public string? Received { get; set; }
        public string? AllocatedBy { get; set; }
         
        //public int Employee_ID { get; set; }
        public decimal? Bassic { get; set; }
        public decimal? HRA { get; set; }
        public decimal? Medical { get; set; }
        public decimal? Conveyavce { get; set; }
        public decimal? CCA { get; set; }
        public decimal? Other { get; set; }

        public decimal? LTA { get; set; }
        public string? Date { get; set; }
        public string? Username { get; set; }
         public int perquisites { get; set; }
         public decimal RentPaid { get; set; }
          public string? Duty { get; set; }
        public string? Minimum_Duty { get; set; }
        public string? LunchHours { get; set; }
         public string? OT_Applicable { get; set; }

        public string? IsShiftApplicable { get; set; }

        //public string? NPS_Deduction { get; set; }

    }

    public class EmployeeAssetAssignModel
{
    public string? EmpId { get; set; }
    public string? EmployeeName { get; set; }
    public List<string> Assets { get; set; } = new();   // checklist
    public string? AllocatedBy { get; set; }
}

}