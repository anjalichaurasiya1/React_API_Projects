using Dapper;
using product_api.Models;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace product_api.Service
{
    public class BillService
    {
        private readonly string _connectionString;

        public BillService(string connectionString)
        {
            _connectionString = connectionString;
        }

        // ==================== GET METHODS ====================

        public async Task<IEnumerable<ClientModel>> GetClientsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<ClientModel>("GET_CLIENT_FOR_BILLADD", commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<ClientModel>> GetBillingClientsAsync(int clientId, bool showBranch)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@Clientid", clientId);
            // if (showBranch)
            //     parameters.Add("@Load_Branch_Details", "BRANCH");

            return await connection.QueryAsync<ClientModel>("SEARCH_CLIENT", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<ConsigneeModel>> GetConsigneesAsync(int jobId)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@jobid", jobId);

            return await connection.QueryAsync<ConsigneeModel>("GET_Consignee_For_Bill", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<JobModel>> GetJobsAsync(int? clientId, int? billingClientId, int? jobId, int? displayJobId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();

                parameters.Add("@Clientid", clientId);
                parameters.Add("@BillingClientid", billingClientId);
                parameters.Add("@jobid", jobId);
                parameters.Add("@DisplayJobId", displayJobId);

                var result = await connection.QueryAsync<JobModel>(
                    "GET_COMPLETEDJOB_FOR_BILL",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result.ToList();
            }
        }


        public async Task<IEnumerable<JobModel>> GetJobSheetAsync(int? Id, int? jobId, int? billId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();

               
                parameters.Add("@Id", Id);
                parameters.Add("@jobid", jobId);
                parameters.Add("@BillId", billId);

                var result = await connection.QueryAsync<JobModel>(
                    "SEARCH_JOBSHEETDETAILS",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result.ToList();
            }
        }


        

        public async Task<IEnumerable<BillItemModel>> GetBillItemsAsync(int billId)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@billid", billId);

            return await connection.QueryAsync<BillItemModel>("SEARCH_BILL_ITEMS", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<BillItemModel>> GetBillAsync(int? BillNo, int? clientId, string? InvoiceNoTo, string? invoiceNo, string? fromDate, string? toDate, int? showPendingEinvoice)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@clientid", clientId);
            parameters.Add("@date1", fromDate);
            parameters.Add("@date2", toDate);
            parameters.Add("@id", BillNo);
            parameters.Add("@Invoiceno", invoiceNo);
            parameters.Add("@InvoicenoTo", InvoiceNoTo);
            parameters.Add("@Show_PendingEinvoice", showPendingEinvoice);

            return await connection.QueryAsync<BillItemModel>("SEARCH_BILL", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<AdvancePaymentModel>> GetAdvancePaymentsAsync(int? clientId)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@clientid", clientId);

            return await connection.QueryAsync<AdvancePaymentModel>("GET_ADVANCE_FOR_CLIENT", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<ChallanModel>> GetChallansAsync(int jobId, int billId, string filter, int isKit)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@Jobid", jobId);
            parameters.Add("@Billid", billId);
            parameters.Add("@IsKit", isKit);
            if (!string.IsNullOrEmpty(filter) && filter != "0")
                parameters.Add("@Destination", filter);

            return await connection.QueryAsync<ChallanModel>("GET_CHALLAN_TO_BILL_NEXUS", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<ChallanModel>> GetChallanFilterAsync(int jobId, int billId, int isKit)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@Jobid", jobId);
            parameters.Add("@Billid", billId);
            parameters.Add("@IsKit", isKit);

            return await connection.QueryAsync<ChallanModel>("GET_CHALLAN_TO_BILL_NEXUS_Filter", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<DigitalPrintModel>> GetDigitalPrintDataAsync(int jobId)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@JobId", jobId);

            return await connection.QueryAsync<DigitalPrintModel>("Get_IncompletedDigitalJobs_TaskWise", parameters, commandType: CommandType.StoredProcedure);
        }

        

        public async Task<IEnumerable<MarketingPersonModel>> GetMarketingPersonsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<MarketingPersonModel>("EXEC GET_MARKETINGPERSON", commandType: CommandType.Text);
        }

        public async Task<IEnumerable<JobCategoryModel>> GetJobCategoriesAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<JobCategoryModel>("GET_JOBCATEGORY", commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<UnitModel>> GetUnitsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<UnitModel>("LOAD_SAPUNITS", commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<StateModel>> GetStatesAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<StateModel>("GET_STATE", commandType: CommandType.StoredProcedure);
        }

        public async Task<ClientModel> GetClientDetailsAsync(int clientId)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@ClientId", clientId);

            var result = await connection.QueryFirstOrDefaultAsync<ClientModel>("Get_Clientdetails", parameters, commandType: CommandType.StoredProcedure);
            return result!;
        }

        public async Task<IEnumerable<JobModel>> GetJobDetailsAsync(int? jobId, string? type)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@jobid", jobId);
            parameters.Add("@Type", type);
            //parameters.Add("@IsKit", isKit);

            return await connection.QueryAsync<JobModel>("SEARCH_JOB_BILLADD", parameters, commandType: CommandType.StoredProcedure);
        }

        

        // In BillService.cs, update the GetOtherJobsAsync method to match the controller call
        public async Task<IEnumerable<JobModel>> GetOtherJobsAsync(int? clientId, int? jobId, int? billingClientId, int? showJobId)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@Clientid", clientId);
            parameters.Add("@jobid", jobId);
            parameters.Add("@BillingClientid", billingClientId);
           
            parameters.Add("@DisplayJobId", showJobId);

            return await connection.QueryAsync<JobModel>("GET_COMPLETEDJOB_FOR_BILL", parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<JobModel>> GetKitJobsAsync(int clientId)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@clientid", clientId);

            return await connection.QueryAsync<JobModel>("Get_Kit_ForClient", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<CheckQuantityResponse> CheckJobQuantityAsync(int jobId, int quantity, int billedQty, string isIndependent, int jobSheetDetailId)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@jobid", jobId);
            parameters.Add("@iQty", quantity);
            parameters.Add("@jQty", billedQty);
            parameters.Add("@ISIndependent", isIndependent);
            parameters.Add("@JobSheetDetailsId", jobSheetDetailId);

            // This would need to be implemented in SQL as a function
            // For now, we'll do a simple check
            var jobDetails = await GetJobDetailsAsync(jobId, "JOB");
            var job = jobDetails?.FirstOrDefault();
            if (job != null)
            {
                int maxQty = job.Qty + billedQty;
                if (quantity > maxQty)
                {
                    return new CheckQuantityResponse
                    {
                        Message = $"Entered Qty is more than Balance Job Qty-{maxQty}. Do You want to continue?"
                    };
                }
            }

            return new CheckQuantityResponse { Message = "" };
        }

        public async Task<ChallanModel> GetChallanDetailsAsync(int jobId)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@jobid", jobId);

            var result = await connection.QueryFirstOrDefaultAsync<ChallanModel>("GET_CHALLANDETAILS_For_Job", parameters, commandType: CommandType.StoredProcedure);
            return result!;
        }

        // ==================== SAVE/UPDATE METHODS ====================

        public async Task<SaveBillResponse> SaveBillAsync(BillModel bill)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                // Generate FYR if needed
                if (string.IsNullOrEmpty(bill.FyrVar))
                {
                    bill.FyrVar = GetFyr(bill.Date);
                }

                // Generate invoice number if needed
                if (string.IsNullOrEmpty(bill.InvoiceNo))
                {
                    bill.InvoiceNo = GetFyr(bill.Date, 2);
                }

                var parameters = new DynamicParameters();
                parameters.Add("@odno", bill.OdNo);
                parameters.Add("@date", bill.Date.ToString("dd/MM/yyyy"));
                parameters.Add("@oddate", bill.OdDate.ToString("dd/MM/yyyy"));
                parameters.Add("@taxtype", bill.TaxType);
                parameters.Add("@tax", bill.Tax);
                parameters.Add("@amount", bill.Amount);
                parameters.Add("@clientid", bill.ClientId);
                parameters.Add("@Invoice", bill.InvoiceNo);
                parameters.Add("@fyr", bill.FyrVar);
                parameters.Add("@IsExcisable", bill.IsExcisable);
                parameters.Add("@Exciseduty", bill.ExciseDuty);
                parameters.Add("@EduCess", bill.EduCess);
                parameters.Add("@ShEduCess", bill.ShEduCess);
                parameters.Add("@CreditDays", bill.CreditDays);
                parameters.Add("@MarketingPerson", bill.MarketingPerson);
                parameters.Add("@BillingClient", bill.BillingClient);
                parameters.Add("@Type", bill.Type);
                parameters.Add("@Reference", bill.ReferenceNo);
                parameters.Add("@Showdates", bill.ShowDates);
                parameters.Add("@Tax_SubType", bill.TaxSubType);
                parameters.Add("@BillAddressType", bill.BillingAddressType);
                parameters.Add("@ConsigneeId", bill.Consignee);
                parameters.Add("@Consignee_Address", bill.ConsigneeAddress);
                parameters.Add("@DollarAmount", bill.DollarAmount);
                parameters.Add("@IsKit", bill.IsKit);
                parameters.Add("@AssesableValue", bill.AssesableValue);
                parameters.Add("@TcsAmount", bill.TcsAmount);
                parameters.Add("@TcsPer", bill.TcsPer);
                parameters.Add("@BillType", bill.BillType);
                parameters.Add("@GenerateInvoiceNo", bill.GenerateInvoiceNo);
                parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@errorCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@errorMsg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

                if (bill.Id > 0)
                {
                    parameters.Add("@id", bill.Id);
                    await connection.ExecuteAsync("UPDATE_BILL", parameters, transaction, commandType: CommandType.StoredProcedure);
                }
                else
                {
                    await connection.ExecuteAsync("INSERT_BILL", parameters, transaction, commandType: CommandType.StoredProcedure);
                }

                int billId = parameters.Get<int>("@id");

                transaction.Commit();

                return new SaveBillResponse
                {
                    BillId = billId,
                    InvoiceNo = bill.InvoiceNo,
                    Message = "Bill saved successfully"
                };
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<int> SaveBillItemAsync(BillItemModel billItem)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();

            parameters.Add("@billid", billItem.BillId);
            parameters.Add("@jobname", billItem.JobName);
            parameters.Add("@size", billItem.Size);
            parameters.Add("@pages", billItem.Pages);
            parameters.Add("@qty", billItem.Qty);
            parameters.Add("@rate", billItem.Rate);
            parameters.Add("@amount", billItem.GrossAmount);
            parameters.Add("@rate_type", billItem.RateType);
            parameters.Add("@taxtype", billItem.TaxType);
            parameters.Add("@tax", billItem.Tax);
            parameters.Add("@remark", billItem.Remark);
            parameters.Add("@jobid", billItem.JobId);
            parameters.Add("@Paper", billItem.Paper);
            parameters.Add("@Finish", billItem.Finish);
            parameters.Add("@Printing", billItem.Printing);
            parameters.Add("@Cover", billItem.Cover);
            parameters.Add("@isCompleted", billItem.IsCompleted);
            parameters.Add("@fyr", billItem.Fyr);
            parameters.Add("@QuotFile", billItem.QuotFile);
            parameters.Add("@FileName", billItem.FileName);
            parameters.Add("@Exciseduty", billItem.ExciseDuty);
            parameters.Add("@EduCess", billItem.EduCess);
            parameters.Add("@ShEduCess", billItem.ShEduCess);
            parameters.Add("@OtherJobIds", billItem.OtherJobIds);
            parameters.Add("@ExcludeMe", billItem.ExcludeMe);
            parameters.Add("@JobSheetDetailId", billItem.JobSheetDetailId);
            parameters.Add("@Category", billItem.Category);
            parameters.Add("@HsnCode", billItem.HsnCode);
            parameters.Add("@Machinename", billItem.MachineName);
            parameters.Add("@KitId", billItem.KitId);
            parameters.Add("@ComplementryQty", billItem.ComplementryQty);
            parameters.Add("@MergeWith", billItem.MergeWith);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            if (billItem.Id > 0)
            {
                parameters.Add("@id", billItem.Id);
                await connection.ExecuteAsync("UPDATE_BILL_ITEMS", parameters, commandType: CommandType.StoredProcedure);
            }
            else
            {
                await connection.ExecuteAsync("INSERT_BILL_ITEMS", parameters, commandType: CommandType.StoredProcedure);
            }

            return parameters.Get<int>("@id");
        }

        public async Task DeleteBillItemAsync(int billItemId, int billId)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@id", billItemId);
            parameters.Add("@billid", billId);

            await connection.ExecuteAsync("Delete_Bill_Items", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task LinkChallansToBillAsync(int billId, string? challanIds, int clientId, string? pinCode, string? state, string? stateCode, int unitId)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@BillId", billId);
            parameters.Add("@ChallanId", challanIds ?? string.Empty);  // Use empty string if null
            parameters.Add("@CLientId", clientId);
            parameters.Add("@PinCode", pinCode ?? string.Empty);      // Use empty string if null
            parameters.Add("@State", state ?? string.Empty);          // Use empty string if null
            parameters.Add("@StateCode", stateCode ?? string.Empty);  // Use empty string if null
            parameters.Add("@UnitId", unitId);

            await connection.ExecuteAsync("Update_ShippingAddress_For_eInvoice", parameters, commandType: CommandType.StoredProcedure);

            // Also link each challan
            if (!string.IsNullOrEmpty(challanIds))
            {
                var ids = challanIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var id in ids)
                {
                    var linkParams = new DynamicParameters();
                    linkParams.Add("@ChallanId", id);
                    linkParams.Add("@BillId", billId);
                    linkParams.Add("@IsDeleted", 0);
                    await connection.ExecuteAsync("INSERT_CHALLAN_BILL_LINK", linkParams, commandType: CommandType.StoredProcedure);
                }
            }
        }
        public async Task UpdateKitStatusAsync(int billId)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@BillId", billId);

            await connection.ExecuteAsync("UPDATE_KIT_STATUS", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateBillForDigitalAsync(int billId, string? digitalIds)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@Id", digitalIds ?? string.Empty);  // Use empty string if null
            parameters.Add("@BillId", billId);

            await connection.ExecuteAsync("Update_BillNo", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAdvancePaymentBillNoAsync(string advanceIds, int billId)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@AdvanceIds", advanceIds);
            parameters.Add("@BillId", billId);

            await connection.ExecuteAsync("UPDATE_BILLNO_FOR_ADVANCEPAYMENTS", parameters, commandType: CommandType.StoredProcedure);
        }

        // ==================== HELPER METHODS ====================

        private string GetFyr(DateTime date, int returnType = 0)
        {
            string fyr = "";
            int len, len1;

            if (date.Month > 3)
            {
                len = 0;
                len1 = 1;
            }
            else
            {
                len = 1;
                len1 = 0;
            }

            if (returnType == 0)
                fyr = (date.Year - len).ToString().Substring(2, 2) + "-" + (date.Year + len1).ToString().Substring(2, 2);
            else if (returnType == 1)
                fyr = (date.Year - len).ToString().Substring(0, 4) + "-" + (date.Year + len1).ToString().Substring(0, 4);
            else
                fyr = (date.Year - len).ToString().Substring(0, 4) + "-" + (date.Year + len1).ToString().Substring(2, 2);

            return fyr;
        }
    }
}