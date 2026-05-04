using Dapper;
using product_api.Models;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace product_api.Service
{
	public class UnitSerialService
	{
		private readonly string _connectionString;
		public UnitSerialService(string connectionString)
		{
			_connectionString = connectionString;
		}

        public async Task<IEnumerable<UnitSerialMasterModel>> GetAllUnitsAsync()
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				// Assumes SEARCH_UNIT without @unitid returns all items. Adjust if you have a dedicated proc.
				return await connection.QueryAsync<UnitSerialMasterModel>("Get_Unit",commandType: CommandType.StoredProcedure);
			}
		}

     public async Task<List<UnitSerialMasterModel>> GetUnitSerialByIdAsync( int? UnitId, int? JobId, int? SrId, int? SrFrom, int? SrTo, string? Unit)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();

                if (UnitId != null)
                     parameters.Add("@unitid", UnitId);

                if (JobId != null)
                     parameters.Add("@jobid", JobId);

                if (SrId != null)
                    parameters.Add("@srid", SrId);

                 if (SrFrom != null)
                     parameters.Add("@from", SrFrom);

                 if (SrTo != null)
                     parameters.Add("@to", SrTo);

                 if (!string.IsNullOrEmpty(Unit))
                     parameters.Add("@unit", Unit);

                var result = await connection.QueryAsync<UnitSerialMasterModel>("SEARCH_UNIT_SERIAL", parameters, commandType: CommandType.StoredProcedure );

                 return result.ToList();   // ✅ RETURN ALL RECORDS
             }
        }

         public async Task<List<UnitSerialMasterModel>> Check_UnitSerial_Within_SerialAsync( int? UnitId, int? JobId, int? SrId, int? SrFrom, int? SrTo)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();

                if (UnitId != null)
                     parameters.Add("@unitid", UnitId);

                if (JobId != null)
                     parameters.Add("@jobid", JobId);

                if (SrId != null)
                    parameters.Add("@srid", SrId);

                 if (SrFrom != null)
                     parameters.Add("@s", SrFrom);

                 if (SrTo != null)
                     parameters.Add("@t", SrTo);

                

                var result = await connection.QueryAsync<UnitSerialMasterModel>("CHECK_UNIT_SERIAL_WITHIN_SERIAL", parameters, commandType: CommandType.StoredProcedure );

                 return result.ToList();   // ✅ RETURN ALL RECORDS
             }
        }
    }
}