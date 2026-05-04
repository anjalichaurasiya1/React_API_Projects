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
            var cs = _config.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(cs))
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
            return new SqlConnection(cs);
        }


        [HttpPost("Insert_Salary_rate")]
        public IActionResult Insert_Salary_rate([FromBody] EmployeeDetailsModel model)
        {
            try
            {
                using SqlConnection con = GetConnection();
                using SqlCommand cmd = new SqlCommand("Insert_Salary_rate", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Employee_ID", model.EmployeeId);
                cmd.Parameters.AddWithValue("@Basic", model.Bassic ?? 0);
                cmd.Parameters.AddWithValue("@HRA", model.HRA ?? 0);
                cmd.Parameters.AddWithValue("@Medical", model.Medical ?? 0);
                cmd.Parameters.AddWithValue("@Conveyance", model.Conveyavce ?? 0);
                cmd.Parameters.AddWithValue("@CCA", model.CCA ?? 0);
                cmd.Parameters.AddWithValue("@Other", model.Other ?? 0);
                cmd.Parameters.AddWithValue("@LTA", model.LTA ?? 0);
                cmd.Parameters.AddWithValue("@PL", model.PL ?? 0);
                cmd.Parameters.AddWithValue("@SL", model.SL ?? 0);
                cmd.Parameters.AddWithValue("@CL", model.CL ?? 0);

                cmd.Parameters.AddWithValue("@Date", model.Date ?? string.Empty);
                cmd.Parameters.AddWithValue("@Username", model.Username ?? string.Empty);
                cmd.Parameters.AddWithValue("@DutyHours", model.Duty ?? string.Empty);
                cmd.Parameters.AddWithValue("@Minimum_DutyHours", model.Minimum_Duty ?? string.Empty);
                cmd.Parameters.AddWithValue("@LunchHours", model.LunchHours ?? string.Empty);
                cmd.Parameters.AddWithValue("@OT_Applicable", model.OT_Applicable ?? string.Empty);
                cmd.Parameters.AddWithValue("@IsShiftApplicable", model.IsShiftApplicable ?? string.Empty);
                cmd.Parameters.AddWithValue("@NPS", model.NpsDeduction ?? string.Empty);

                con.Open();
                cmd.ExecuteNonQuery();

                return Ok(new { message = "Salary saved successfully" });
            }
            catch (Exception ex)
            {
                // In Production you might return a sanitized message; for debugging include exception details.
                return StatusCode(500, new { error = ex.Message, trace = ex.StackTrace });
            }
        }


        [HttpPost("InsertEmployeeAssets")]
        public IActionResult InsertEmployeeAssets([FromForm] EmployeeDetailsModel model)
        {
            try
            {
                using SqlConnection con = GetConnection();

                string spName = model.Id > 0 ? "Update_EmployeeAssets" : "Insert_EmployeeAssets";

                using SqlCommand cmd = new SqlCommand(spName, con) { CommandType = CommandType.StoredProcedure };

                // ===== SAME PARAMETERS AS OLD CODE =====
                cmd.Parameters.AddWithValue("@EmpId", model.EmployeeId);
                cmd.Parameters.AddWithValue("@EmployeeName", model.Name ?? string.Empty);
                cmd.Parameters.AddWithValue("@Assets", model.AssetsName ?? string.Empty);
                cmd.Parameters.AddWithValue("@Received", model.Received ?? string.Empty);
                cmd.Parameters.AddWithValue("@AllocatedBy", model.AllocatedBy ?? string.Empty);

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
                using SqlCommand cmd = new SqlCommand("Search_Edit_Employee_Details", con) { CommandType = CommandType.StoredProcedure };

                // Always supply parameters to the stored procedure; use DBNull for "not provided"
                cmd.Parameters.AddWithValue("@Id", id == 0 ? (object)DBNull.Value : id);
                cmd.Parameters.AddWithValue("@Employee_ID", employeeId == 0 ? (object)DBNull.Value : employeeId);
                cmd.Parameters.AddWithValue("@Employee_Name", GetDbValue(employeeName));
                cmd.Parameters.AddWithValue("@Date_of_Joining", GetDbValue(Date_of_Joining));
                cmd.Parameters.AddWithValue("@Date_of_leving", GetDbValue(Date_of_leving));
                cmd.Parameters.AddWithValue("@Date_of_Birth", GetDbValue(Date_of_Birth));

                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                con.Open();
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
                using SqlCommand cmd = new SqlCommand("Get_Departments_Combo", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Id", obj.Id == 0 ? (object)DBNull.Value : obj.Id);
                cmd.Parameters.AddWithValue("@Departments", string.IsNullOrEmpty(obj.Department) ? (object)DBNull.Value : obj.Department);

                con.Open();
                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                    return NotFound("No departments found");

                return Ok(DataTableToList(dt));
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

        int SafeInt(object val)
        {
            return val == DBNull.Value ? 0 : Convert.ToInt32(val);
        }


        // 🔹 SAVE (INSERT/UPDATE
      [HttpPost("save")]
public IActionResult Save([FromForm] EmployeeDetailsModel model)
{
    try
    {
        using SqlConnection con = GetConnection();

        string spName = model.Id > 0 ? "Update_Employee_Details" : "Insert_Employee_Details";

        using SqlCommand cmd = new SqlCommand(spName, con);
        cmd.CommandType = CommandType.StoredProcedure;

  
        if (model.Id > 0)
        {
            cmd.Parameters.Add("@Id", SqlDbType.Int).Value = model.Id;
        }
        else
        {
            cmd.Parameters.Add("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
        }

        

       cmd.Parameters.Add("@Employee_ID", SqlDbType.Int).Value = model.EmployeeId;

        if (!string.IsNullOrEmpty(model.Name))
            cmd.Parameters.Add("@Employee_Name", SqlDbType.VarChar, 100).Value = model.Name;

        if (!string.IsNullOrEmpty(model.Surname))
            cmd.Parameters.Add("@Sur_Name", SqlDbType.VarChar, 100).Value = model.Surname;

        if (!string.IsNullOrEmpty(model.FatherName))
            cmd.Parameters.Add("@Father_Name", SqlDbType.VarChar, 100).Value = model.FatherName;

        if (!string.IsNullOrEmpty(model.Address))
            cmd.Parameters.Add("@Address", SqlDbType.VarChar, -1).Value = model.Address;


             if (!string.IsNullOrEmpty(model.PermanentAddress))
            cmd.Parameters.Add("@Pernament_address", SqlDbType.VarChar, -1).Value = model.PermanentAddress;

        if (!string.IsNullOrEmpty(model.Designation))
            cmd.Parameters.Add("@Designation", SqlDbType.VarChar, 100).Value = model.Designation;

        
        if (model.DateOfJoining != null)
            cmd.Parameters.Add("@Date_of_joining", SqlDbType.VarChar, 10)
                .Value = model.DateOfJoining.Value.ToString("yyyy-MM-dd");

        if (model.DateOfBirth != null)
            cmd.Parameters.Add("@Date_of_Birth", SqlDbType.VarChar, 10)
                .Value = model.DateOfBirth.Value.ToString("yyyy-MM-dd");

        if (model.DateOfLeaving != null)
            cmd.Parameters.Add("@Date_of_leving", SqlDbType.VarChar, 10)
                .Value = model.DateOfLeaving.Value.ToString("yyyy-MM-dd");

        if (model.LetterDate != null)
            cmd.Parameters.Add("@Date_of_Letter", SqlDbType.VarChar, 10)
                .Value = model.LetterDate.Value.ToString("yyyy-MM-dd");

        if (!string.IsNullOrEmpty(model.MobileNo))
            cmd.Parameters.Add("@Mobile_no", SqlDbType.VarChar, 15).Value = model.MobileNo;

        if (!string.IsNullOrEmpty(model.Email))
            cmd.Parameters.Add("@email_id", SqlDbType.VarChar, 100).Value = model.Email;

        if (!string.IsNullOrEmpty(model.PF_No))
            cmd.Parameters.Add("@PF_no", SqlDbType.VarChar, 50).Value = model.PF_No;

        if (!string.IsNullOrEmpty(model.ESIC_no))
            cmd.Parameters.Add("@ESIC_no", SqlDbType.VarChar, 50).Value = model.ESIC_no;

        if (!string.IsNullOrEmpty(model.PanNo))
            cmd.Parameters.Add("@PAN_no", SqlDbType.VarChar, 50).Value = model.PanNo;

        if (!string.IsNullOrEmpty(model.AadharNo))
            cmd.Parameters.Add("@Aadhar_Card_no", SqlDbType.VarChar, 12).Value = model.AadharNo;

        if (!string.IsNullOrEmpty(model.UAN_No))
            cmd.Parameters.Add("@UAN_No", SqlDbType.VarChar, 12).Value = model.UAN_No;

        if (!string.IsNullOrEmpty(model.BankName))
            cmd.Parameters.Add("@Bank_Name", SqlDbType.VarChar, 100).Value = model.BankName;

        if (!string.IsNullOrEmpty(model.BankAccount))
            cmd.Parameters.Add("@Bank_Acount_No", SqlDbType.VarChar, 50).Value = model.BankAccount;

        if (!string.IsNullOrEmpty(model.IFSCCode))
            cmd.Parameters.Add("@IFSC_code", SqlDbType.VarChar, 50).Value = model.IFSCCode;

        if (!string.IsNullOrEmpty(model.Status))
            cmd.Parameters.Add("@Status", SqlDbType.VarChar, 50).Value = model.Status;

        if (!string.IsNullOrEmpty(model.Gender))
            cmd.Parameters.Add("@Gender", SqlDbType.VarChar, 20).Value = model.Gender;

        if (!string.IsNullOrEmpty(model.WorkPlace))
            cmd.Parameters.Add("@WorkPlace", SqlDbType.VarChar, 200).Value = model.WorkPlace;

        if (!string.IsNullOrEmpty(model.ApplyPF))
            cmd.Parameters.Add("@ApplyPF", SqlDbType.VarChar, 50).Value = model.ApplyPF;

        if (!string.IsNullOrEmpty(model.ApplyESIC))
            cmd.Parameters.Add("@ApplyESIC", SqlDbType.VarChar, 50).Value = model.ApplyESIC;

        if (!string.IsNullOrEmpty(model.Category))
            cmd.Parameters.Add("@Category", SqlDbType.VarChar, 50).Value = model.Category;

        if (!string.IsNullOrEmpty(model.Department))
            cmd.Parameters.Add("@Department", SqlDbType.VarChar, 100).Value = model.Department;

        if (!string.IsNullOrEmpty(model.NpsDeduction))
            cmd.Parameters.Add("@NPS_Deduction", SqlDbType.VarChar, 50).Value = model.NpsDeduction;

        if (!string.IsNullOrEmpty(model.Username))
            cmd.Parameters.Add("@CreatedBy", SqlDbType.VarChar, 100).Value = model.Username;

        
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

        return Ok(new
        {
            message = errorMsg.Value?.ToString(),
            id = cmd.Parameters["@Id"].Value
        });
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
                   Id = SafeInt(dr["ID"]),
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
                    PL = SafeInt(dr["PL"]),
                    SL = SafeInt(dr["SL"]),
                    CL = SafeInt(dr["Cl"]),
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
                    Id = SafeInt(dr["id"]),
                    EmpId = SafeInt(dr["EmpId"]),
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
                    Id = SafeInt(dr["id"]),
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

        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public DateTime? DateOfLeaving { get; set; }

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
        // public string? Date_of_Letter { get; set; }

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
        public int? perquisites { get; set; }
        public decimal? RentPaid { get; set; }
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