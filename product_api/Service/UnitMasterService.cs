using Dapper;
using product_api.Models;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace product_api.Service
{
	public class UnitMasterService
	{
		private readonly string _connectionString;
		public UnitMasterService(string connectionString)
		{
			_connectionString = connectionString;
		}

		public async Task<UnitmasterModel?> GetUnitByIdAsync(int unitId)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var parameters = new DynamicParameters();
				parameters.Add("@unitid", unitId);

				var result = await connection.QueryAsync<UnitmasterModel>("SEARCH_UNIT",parameters,commandType: CommandType.StoredProcedure);

				return result.FirstOrDefault();
			}
		}

		public async Task<IEnumerable<UnitmasterModel>> GetAllUnitsAsync()
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				// Assumes SEARCH_UNIT without @unitid returns all items. Adjust if you have a dedicated proc.
				return await connection.QueryAsync<UnitmasterModel>("SEARCH_UNIT",commandType: CommandType.StoredProcedure);
			}
		}

		public async Task<string> CreateUnitAsync(UnitmasterModel unit)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var parameters = new DynamicParameters();

				parameters.Add("@unitId", dbType: DbType.Int32, direction: ParameterDirection.Output);
				if(unit.Unit != null)
					parameters.Add("@unit", unit.Unit);
				if(unit.JobId != 0)
					parameters.Add("@jobid", unit.JobId);
				if(unit.Address != null)
					parameters.Add("@add", unit.Address);
				if(unit.Phone != null)
					parameters.Add("@phone", unit.Phone);

				parameters.Add("@errorCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@errorMsg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

				await connection.ExecuteAsync(
					"INSERT_UNIT",
					parameters,
					commandType: CommandType.StoredProcedure);

				return parameters.Get<string>("@errorMsg");
			}
		}

		public async Task<string> UpdateUnitAsync(UnitmasterModel unit)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var parameters = new DynamicParameters();
				if(unit.UnitId != 0)
                 	parameters.Add("@unitId", unit.UnitId);
				if(unit.Unit != null)	
					parameters.Add("@unit", unit.Unit);
				if(unit.JobId != 0)	
					parameters.Add("@jobid", unit.JobId);
				if(unit.Address != null)	
					parameters.Add("@add", unit.Address);
				if(unit.Phone != null)	
					parameters.Add("@phone", unit.Phone);

				parameters.Add("@errorCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
				parameters.Add("@errorMsg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

				await connection.ExecuteAsync("UPDATE_UNIT",parameters,	commandType: CommandType.StoredProcedure);

				return parameters.Get<string>("@errorMsg");
			}
		}

		public async Task<string> DeleteUnitAsync(int unitId)
{
    using (var connection = new SqlConnection(_connectionString))
    {
        var parameters = new DynamicParameters();
		if(unitId != 0)
        	parameters.Add("@unitId", unitId);

        await connection.ExecuteAsync("DELETE_UNIT",parameters,commandType: CommandType.StoredProcedure);

        return "Deleted successfully";
    }
}

		public async Task<IEnumerable<UnitmasterModel>> LoadJobIdAsync()
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				return await connection.QueryAsync<UnitmasterModel>("LOAD_JOBID",commandType: CommandType.StoredProcedure);
			}
		}

		
	}
}