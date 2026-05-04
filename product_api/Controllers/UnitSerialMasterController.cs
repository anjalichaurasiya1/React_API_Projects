using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using product_api.Models;
using product_api.Service;

namespace product_api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class UnitSerialMasterController : ControllerBase
	{
		private readonly UnitSerialService _unitSerialService;
		public UnitSerialMasterController(UnitSerialService unitSerialService)
		{
			_unitSerialService = unitSerialService;
		}

        [HttpGet]
		public async Task<ActionResult<IEnumerable<UnitSerialMasterModel>>> GetAllUnits()
		{
			var units = await _unitSerialService.GetAllUnitsAsync();
			return Ok(units);
		}

       [HttpGet("GetUnitSerialById")]
        public async Task<ActionResult<List<UnitSerialMasterModel>>> GetUnitSerialById(int? UnitId = null, int? JobId = null, int? SrId = null, int? SrFrom = null, int? SrTo = null, string? Unit = null)
        {
             var data = await _unitSerialService.GetUnitSerialByIdAsync(UnitId, JobId, SrId, SrFrom, SrTo, Unit);

            if (data == null || data.Count == 0)
                return NotFound();

            return Ok(data);   // ✅ RETURN LIST
        }

        [HttpGet("Check_UnitSerial_Within_Serial")]
        public async Task<ActionResult<List<UnitSerialMasterModel>>> CheckUnitSerialWithinSerial(int? UnitId = null, int? JobId = null, int? SrId = null, int? SrFrom = null, int? SrTo = null)
        {
             var data = await _unitSerialService.Check_UnitSerial_Within_SerialAsync(UnitId, JobId, SrId, SrFrom, SrTo);

            if (data == null || data.Count == 0)
                return NotFound();

            return Ok(data);   // ✅ RETURN LIST
        }
    }
}