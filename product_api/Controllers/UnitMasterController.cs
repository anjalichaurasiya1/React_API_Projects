using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using product_api.Models;
using product_api.Service;

namespace product_api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class UnitMasterController : ControllerBase
	{
		private readonly UnitMasterService _unitMasterService;
		public UnitMasterController(UnitMasterService unitMasterService)
		{
			_unitMasterService = unitMasterService;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<UnitmasterModel>> GetUnitById(int id)
		{
			var unit = await _unitMasterService.GetUnitByIdAsync(id);
			if (unit == null) return NotFound();
			return Ok(unit);
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<UnitmasterModel>>> GetAllUnits()
		{
			var units = await _unitMasterService.GetAllUnitsAsync();
			return Ok(units);
		}

		[HttpPost("save")]
		public async Task<ActionResult<string>> CreateUnit([FromBody] UnitmasterModel unit)
		{
			 var result = await _unitMasterService.CreateUnitAsync(unit);
   			 return Ok(result);
		}

		[HttpPut("update")]
		public async Task<ActionResult<string>> UpdateUnit([FromBody] UnitmasterModel unit)
		{
			var result = await _unitMasterService.UpdateUnitAsync(unit);
			
			return Ok(result);
		}

		[HttpDelete("delete/{id}")]
		public async Task<ActionResult<string>> DeleteUnit(int id)
		{
			var result = await _unitMasterService.DeleteUnitAsync(id);
			
			return Ok(result);
		}
	}
}