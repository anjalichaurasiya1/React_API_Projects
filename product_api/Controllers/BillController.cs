using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using product_api.Models;
using product_api.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace product_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillController : ControllerBase
    {
        private readonly BillService _billService;

        public BillController(BillService billService)
        {
            _billService = billService;
        }

        // ==================== GET ENDPOINTS ====================

        [HttpGet("GetClients")]
        public async Task<ActionResult<IEnumerable<ClientModel>>> GetClients()
        {
            try
            {
                var clients = await _billService.GetClientsAsync();
                return Ok(clients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetBillingClients")]
        public async Task<ActionResult<IEnumerable<ClientModel>>> GetBillingClients([FromQuery] int clientId, [FromQuery] bool showBranch = false)
        {
            try
            {
                var clients = await _billService.GetBillingClientsAsync(clientId, showBranch);
                return Ok(clients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetConsignees")]
        public async Task<ActionResult<IEnumerable<ClientModel>>> GetConsignees([FromQuery] int jobId)
        {
            try
            {
                var consignees = await _billService.GetConsigneesAsync(jobId);
                return Ok(consignees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("GetJobs")]
        public async Task<ActionResult<IEnumerable<JobModel>>> GetJobs(
            [FromQuery] int? clientId = null,
            [FromQuery] int? billingClientId = null,
            [FromQuery] int? jobId = null,
            [FromQuery] int? displayJobId = null)
        {
            try
            {
                var jobs = await _billService.GetJobsAsync(clientId, billingClientId, jobId, displayJobId);

                if (jobs == null || !jobs.Any())
                    return NotFound("No jobs found");

                return Ok(jobs);
            }
            catch (Exception ex)
            {
                // Better error response
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }




        [HttpGet("GetJobSheet")]



        public async Task<ActionResult<IEnumerable<JobModel>>> GetJobSheet(
           
            [FromQuery] int? Id = null,
            [FromQuery] int? jobId = null,
            [FromQuery] int? billid = null)
        {
            try
            {
                var jobs = await _billService.GetJobSheetAsync( Id, jobId, billid);

                if (jobs == null || !jobs.Any())
                    return NotFound("No jobs found");

                return Ok(jobs);
            }
            catch (Exception ex)
            {
                // Better error response
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }
       

        [HttpGet("GetBillItems")]
        public async Task<ActionResult<IEnumerable<BillItemModel>>> GetBillItems([FromQuery] int billId)
        {
            try
            {
                var billItems = await _billService.GetBillItemsAsync(billId);
                return Ok(billItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("Search")]
        public async Task<ActionResult<IEnumerable<BillItemModel>>> GetBill([FromQuery] int? BillNo, [FromQuery] int? clientId, [FromQuery] string? InvoiceNoTo, [FromQuery] string? invoiceNo, [FromQuery] string? fromDate, [FromQuery] string? toDate, [FromQuery] int? showPendingEinvoice)
        {
            try
            {
                var billItems = await _billService.GetBillAsync(BillNo, clientId, InvoiceNoTo, invoiceNo, fromDate, toDate, showPendingEinvoice);
                return Ok(billItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("GetAdvancePayments")]
        public async Task<ActionResult<IEnumerable<AdvancePaymentModel>>> GetAdvancePayments([FromQuery] int? clientId)
        {
            try
            {
                var advances = await _billService.GetAdvancePaymentsAsync(clientId);
                return Ok(advances);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetChallans")]
        public async Task<ActionResult<IEnumerable<ChallanModel>>> GetChallans(
            [FromQuery] int jobId,
            [FromQuery] int billId,
            [FromQuery] string filter = "",
            [FromQuery] int isKit = 0)
        {
            try
            {
                var challans = await _billService.GetChallansAsync(jobId, billId, filter, isKit);
                return Ok(challans);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetChallanFilter")]
        public async Task<ActionResult<IEnumerable<ChallanModel>>> GetChallanFilter(
            [FromQuery] int jobId,
            [FromQuery] int billId,
            [FromQuery] int isKit = 0)
        {
            try
            {
                var filters = await _billService.GetChallanFilterAsync(jobId, billId, isKit);
                return Ok(filters);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetDigitalPrintData")]
        public async Task<ActionResult<IEnumerable<DigitalPrintModel>>> GetDigitalPrintData([FromQuery] int jobId)
        {
            try
            {
                var digitalData = await _billService.GetDigitalPrintDataAsync(jobId);
                return Ok(digitalData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    

        [HttpGet("GetMarketingPersons")]
        public async Task<ActionResult<IEnumerable<MarketingPersonModel>>> GetMarketingPersons()
        {
            try
            {
                var persons = await _billService.GetMarketingPersonsAsync();
                return Ok(persons);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetJobCategories")]
        public async Task<ActionResult<IEnumerable<JobCategoryModel>>> GetJobCategories()
        {
            try
            {
                var categories = await _billService.GetJobCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetUnits")]
        public async Task<ActionResult<IEnumerable<UnitModel>>> GetUnits()
        {
            try
            {
                var units = await _billService.GetUnitsAsync();
                return Ok(units);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetStates")]
        public async Task<ActionResult<IEnumerable<StateModel>>> GetStates()
        {
            try
            {
                var states = await _billService.GetStatesAsync();
                return Ok(states);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetClientDetails")]
        public async Task<ActionResult<ClientModel>> GetClientDetails([FromQuery] int clientId)
        {
            try
            {
                var client = await _billService.GetClientDetailsAsync(clientId);
                if (client == null)
                    return NotFound();
                return Ok(client);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetJobDetails")]
        public async Task<ActionResult<JobModel>> GetJobDetails([FromQuery] int? jobId, [FromQuery] string? type )
        {
            try
            {
                var job = await _billService.GetJobDetailsAsync(jobId, type);
                if (job == null)
                    return NotFound();
                return Ok(job);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetOtherJobs")]
        public async Task<ActionResult<IEnumerable<JobModel>>> GetOtherJobs(
     [FromQuery] int? clientId,
     [FromQuery] int? jobId,
     [FromQuery] int? billingClientId,
     [FromQuery] int? showJobId)
        {
            try
            {
                // Match the service method signature: GetOtherJobsAsync(int clientId, int jobid, int billingClientId, bool showJobId)
                var jobs = await _billService.GetOtherJobsAsync(clientId, jobId, billingClientId, showJobId);
                return Ok(jobs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetKitJobs")]
        public async Task<ActionResult<IEnumerable<JobModel>>> GetKitJobs([FromQuery] int clientId)
        {
            try
            {
                var jobs = await _billService.GetKitJobsAsync(clientId);
                return Ok(jobs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("CheckJobQuantity")]
        public async Task<ActionResult<CheckQuantityResponse>> CheckJobQuantity(
            [FromQuery] int jobId,
            [FromQuery] int quantity,
            [FromQuery] int billedQty,
            [FromQuery] string isIndependent,
            [FromQuery] int jobSheetDetailId)
        {
            try
            {
                var result = await _billService.CheckJobQuantityAsync(jobId, quantity, billedQty, isIndependent, jobSheetDetailId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetChallanDetails")]
        public async Task<ActionResult<ChallanModel>> GetChallanDetails([FromQuery] int jobId)
        {
            try
            {
                var details = await _billService.GetChallanDetailsAsync(jobId);
                return Ok(details);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ==================== POST/PUT/DELETE ENDPOINTS ====================

        [HttpPost("Save")]
        public async Task<ActionResult<SaveBillResponse>> SaveBill([FromForm] BillModel bill)
        {
            try
            {
                // Extract username from header
                var username = Request.Headers["Username"].ToString();
                bill.Username = username;

                var result = await _billService.SaveBillAsync(bill);

                // Save bill items if any
                // This would be handled separately or in the same transaction

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("AddToBill")]
        public async Task<ActionResult<int>> AddToBill([FromBody] BillItemModel billItem)
        {
            try
            {
                var result = await _billService.SaveBillItemAsync(billItem);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("DeleteBillItem")]
        public async Task<ActionResult> DeleteBillItem([FromQuery] int billItemId, [FromQuery] int billId)
        {
            try
            {
                await _billService.DeleteBillItemAsync(billItemId, billId);
                return Ok(new { message = "Item deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("LinkChallans")]
        public async Task<ActionResult> LinkChallans([FromBody] LinkChallansRequest request)
        {
            try
            {
                await _billService.LinkChallansToBillAsync(
                    request.BillId,
                    request.ChallanIds,
                    request.ClientId,
                    request.PinCode,
                    request.State,
                    request.StateCode,
                    request.UnitId
                );
                return Ok(new { message = "Challans linked successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("UpdateKitStatus")]
        public async Task<ActionResult> UpdateKitStatus([FromQuery] int billId)
        {
            try
            {
                await _billService.UpdateKitStatusAsync(billId);
                return Ok(new { message = "Kit status updated" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("UpdateBillForDigital")]
        public async Task<ActionResult> UpdateBillForDigital([FromBody] UpdateDigitalRequest request)
        {
            try
            {
                await _billService.UpdateBillForDigitalAsync(request.BillId, request.DigitalIds);
                return Ok(new { message = "Digital jobs updated" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("GenerateEInvoice")]
        public async Task<ActionResult<EInvoiceResponse>> GenerateEInvoice([FromQuery] int billId)
        {
            try
            {
                // This would call your e-invoice service
                // For now, return a mock response
                var response = new EInvoiceResponse
                {
                    Status = "SUCCESS",
                    AckNo = "ACK123456",
                    Irn = "IRN987654321",
                    Seller = new SellerDetails { Name = "SAP Pvt. Ltd.", Address = "Mumbai", Gst = "27AABCS1234A1Z5" },
                    Buyer = new BuyerDetails { Name = "Client Name", Address = "Client Address", Gst = "27XXXXX1234A1Z5" },
                    Items = new List<EInvoiceItem>(),
                    Amount = 10000,
                    GstAmount = 1800,
                    TotalAmount = 11800
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "ERROR", errorDetails = ex.Message });
            }
        }

        // Change these methods from async to regular methods

        [HttpGet("PrintBill")]
        public IActionResult PrintBill(  // Remove async
            [FromQuery] int billId,
            [FromQuery] string printType,
            [FromQuery] bool isExcisable,
            [FromQuery] bool isKit,
            [FromQuery] bool isYCMU,
            [FromQuery] bool isLTBill)
        {
            try
            {
                byte[] pdfBytes = System.Text.Encoding.UTF8.GetBytes("PDF content would be here");
                return File(pdfBytes, "application/pdf", $"Bill_{billId}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("PrintBulkJobReport")]
        public IActionResult PrintBulkJobReport([FromQuery] int billId, [FromQuery] bool isExcisable)  // Remove async
        {
            try
            {
                byte[] pdfBytes = System.Text.Encoding.UTF8.GetBytes("Bulk job report PDF");
                return File(pdfBytes, "application/pdf", $"BulkJobReport_{billId}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("ExportChallansToExcel")]
        public IActionResult ExportChallansToExcel([FromQuery] int jobId)  // Remove async
        {
            try
            {
                byte[] excelBytes = System.Text.Encoding.UTF8.GetBytes("Excel file content");
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Challans_{jobId}.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetEInvoiceStatus")]
        public IActionResult GetEInvoiceStatus([FromQuery] int billId)  // Remove async
        {
            try
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "eInvoice", $"{billId}.txt");
                if (System.IO.File.Exists(filePath))
                {
                    return Ok(new { exists = true, filePath });
                }
                return Ok(new { exists = false });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetUnbilledJobs")]
        public async Task<ActionResult<IEnumerable<JobModel>>> GetUnbilledJobs([FromQuery] int clientId)
        {
            try
            {
                var jobs = await _billService.GetJobsAsync(clientId, 0, 0, 0);
                return Ok(jobs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

    // Request Models
    // In your BillController.cs, update the request models
    public class LinkChallansRequest
    {
        public int BillId { get; set; }
        public string? ChallanIds { get; set; }  // Made nullable
        public int ClientId { get; set; }
        public string? PinCode { get; set; }     // Made nullable
        public string? State { get; set; }       // Made nullable
        public string? StateCode { get; set; }   // Made nullable
        public int UnitId { get; set; }
    }

    public class UpdateDigitalRequest
    {
        public int BillId { get; set; }
        public string? DigitalIds { get; set; }  // Made nullable
    }
}