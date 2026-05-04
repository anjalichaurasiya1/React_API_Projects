using Dapper;
using product_api.Models;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace product_api.Service
{
    public class AttendenceService
    {
        private readonly string _connectionString;

        public AttendenceService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<AttendenceModel> GetEmployeeMonthlyAttendanceAsync(int empId, int month, int year)
{
    using var connection = new SqlConnection(_connectionString);

    var parameters = new DynamicParameters();
    parameters.Add("@EmpId", empId);
    parameters.Add("@Month", month);
    parameters.Add("@Year", year);

    await connection.OpenAsync();

    using var multi = await connection.QueryMultipleAsync(
        "sp_GetEmployeeMonthlyAttendance",
        parameters,
        commandType: CommandType.StoredProcedure
    );

    var summary = multi.ReadFirstOrDefault<AttendenceModel>();
    var attendance = multi.Read<AttendenceModel>().ToList();

    return new AttendenceModel
    {
        Summary = summary,
        Attendance = attendance
    };
}
    }
}