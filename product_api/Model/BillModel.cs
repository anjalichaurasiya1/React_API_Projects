using System;
using System.Collections.Generic;

namespace product_api.Models
{
    public class BillModel
    {
        public int Id { get; set; }
        public int BillNo { get; set; }
        public int BillItemId { get; set; }
        public int ClientId { get; set; }
        public string? JobName { get; set; }
        public string? Size { get; set; }
        public string? Pages { get; set; }
        public string? OdNo { get; set; }
        public DateTime Date { get; set; }
        public DateTime OdDate { get; set; }
        public decimal Rate { get; set; }
        public int Qty { get; set; }
        public decimal Tax { get; set; }
        public string? TaxType { get; set; }
        public string? RateType { get; set; }
        public string? Remark { get; set; }
        public decimal Amount { get; set; }
        public string? InvoiceNo { get; set; }
        public int JobId { get; set; }
        public string? Paper { get; set; }
        public string? Finish { get; set; }
        public string? Printing { get; set; }
        public string? Cover { get; set; }
        public string? FyrVar { get; set; }
        public string? QuotationFile { get; set; }
        public string? FileName { get; set; }
        public string? Category { get; set; }
        public int IsCompleted { get; set; }
        public decimal ExciseDuty { get; set; }
        public decimal EduCess { get; set; }
        public decimal ShEduCess { get; set; }
        public int IsExcisable { get; set; }
        public int CreditDays { get; set; }
        public int ExcludeMe { get; set; }
        public string? MarketingPerson { get; set; }
        public string? OtherJobIds { get; set; }
        public string? Type { get; set; }
        public string? ReferenceNo { get; set; }
        public int BillingClient { get; set; }
        public int JobSheetDetailId { get; set; }
        public int ShowDates { get; set; }
        public string? TaxSubType { get; set; }
        public int DisplayJobIdWithJobName { get; set; }
        public string? JobCategory { get; set; }
        public int HsnCode { get; set; }
        public string? BillingAddressType { get; set; }
        public string? Consignee { get; set; }
        public string? ConsigneeAddress { get; set; }
        public decimal DollarAmount { get; set; }
        public string? PrintingMachine { get; set; }
        public int IsKit { get; set; }
        public decimal AssesableValue { get; set; }
        public decimal TcsAmount { get; set; }
        public decimal TcsPer { get; set; }
        public int GenerateInvoiceNo { get; set; }
        public string? ChallanIdString { get; set; }
        public string? PinCode { get; set; }
        public string? State { get; set; }
        public string? StateCode { get; set; }
        public int ShowPendingeInvoice { get; set; }
        public string? BillType { get; set; }
        public int KitId { get; set; }
        public int ComplementryQty { get; set; }
        public int UnitId { get; set; }
        public int MergeWith { get; set; }
        public string? Username { get; set; }
    }

    public class BillItemModel
    {
        public int Id { get; set; }
        public int BillId { get; set; }
        public string? JobName { get; set; }
        public string? Size { get; set; }
        public string? Pages { get; set; }
        public int Qty { get; set; }
        public decimal Rate { get; set; }
        public decimal GrossAmount { get; set; }
        public string? RateType { get; set; }
        public string? TaxType { get; set; }
        public decimal Tax { get; set; }
        public string? Remark { get; set; }
        public int JobId { get; set; }
        public string? Paper { get; set; }
        public string? Finish { get; set; }
        public string? Printing { get; set; }
        public string? Cover { get; set; }
        public int IsCompleted { get; set; }
        public string? Fyr { get; set; }
        public string? QuotFile { get; set; }
        public string? FileName { get; set; }
        public decimal ExciseDuty { get; set; }
        public decimal EduCess { get; set; }
        public decimal ShEduCess { get; set; }
        public string? OtherJobIds { get; set; }
        public int ExcludeMe { get; set; }
        public int JobSheetDetailId { get; set; }
        public string? Category { get; set; }
        public int HsnCode { get; set; }
        public string? MachineName { get; set; }
        public int KitId { get; set; }
        public int ComplementryQty { get; set; }
        public int MergeWith { get; set; }
    }

    public class JobSheetModel
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string? SectionName { get; set; }
        public string? Pages { get; set; }
        public string? Gsm { get; set; }
        public string? Paper { get; set; }
        public string? Colour { get; set; }
        public string? Remark { get; set; }
        public int Qty { get; set; }
        public int BilledQty { get; set; }
        public decimal Rate { get; set; }
        public string? Size { get; set; }
        public string? Category { get; set; }
        public int HsnCode { get; set; }
        public decimal TaxRate { get; set; }
        public string? IsIndependent { get; set; }
        public int BillItemId { get; set; }
    }

    public class ClientModel
    {
        public int ClientId { get; set; }
        public string? name { get; set; }
        public string? VatTinNo { get; set; }
        public string? PanNo { get; set; }
        public int CreditPeriod { get; set; }
        public string? GstNo { get; set; }
    }

    public class ConsigneeModel
    {
        
        public string? Address { get; set; }
        public string? Consignee { get; set; }
    }

    public class JobModel
    {
        public int JobId { get; set; }
        public string? Name { get; set; }
        public string? Pages { get; set; }
        public string? Size { get; set; }
        public string? Paper { get; set; }
        public string? Finish { get; set; }
        public string? Printing { get; set; }
        public string? Colour { get; set; }
        public int Qty { get; set; }
        public string? Pono { get; set; }
        public string? FileName { get; set; }
        public string? DocPath { get; set; }
        public decimal ExciseRate { get; set; }
        public decimal EducationCess { get; set; }
        public decimal ShEduCess { get; set; }
        public string? CreatedBy { get; set; }
        public string? Remark { get; set; }
        public string? JobRate { get; set; }
        public string? Category { get; set; }
        public int HsnCode { get; set; }
        public decimal TaxRate { get; set; }
        public string? Rate { get; set; }
        public string? RateApproval { get; set; }
        public string? IsExcisable { get; set; }
        public int ClientId { get; set; }
    }

    public class ChallanModel
    {
        public int Id { get; set; }
        public string? ChallanId { get; set; }
        public int Qty { get; set; }
        public int TotalChallanQty { get; set; }
        public int AckChallanQty { get; set; }
        public string? Destination { get; set; }
        public string? EwayBillNo { get; set; }
        public decimal EwayBillAmount { get; set; }
        public int Cnt { get; set; }
    }

    public class AdvancePaymentModel
    {
        public int Id { get; set; }
        public string? Date { get; set; }
        public decimal Amount { get; set; }
        public string? Remark { get; set; }
    }

    public class DigitalPrintModel
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string? JobName { get; set; }
        public string? Specs { get; set; }
        public string? Machine { get; set; }
        public int TotalPrints { get; set; }
        public int Wastage { get; set; }
        public int AdditionalQty { get; set; }
        public decimal Amount { get; set; }
        public int BilledQty { get; set; }
        public int BalanceQty { get; set; }
    }

    public class TaxTypeModel
    {
        public string? Name { get; set; }
    }

    public class MarketingPersonModel
    {
        public string? Username { get; set; }
    }

    public class JobCategoryModel
    {
        public string? Category { get; set; }
        public int HsnCode { get; set; }
        public decimal Gstrate { get; set; }
    }

    public class UnitModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public class StateModel
    {
        public string? StateCode { get; set; }
        public string? Name { get; set; }
    }

    public class CheckQuantityResponse
    {
        public string? Message { get; set; }
    }

    public class SaveBillResponse
    {
        public int BillId { get; set; }
        public string? InvoiceNo { get; set; }
        public string? Message { get; set; }
    }

    public class EInvoiceResponse
    {
        public string? Status { get; set; }
        public string? AckNo { get; set; }
        public string? Irn { get; set; }
        public object? Data { get; set; }
        public string? ErrorDetails { get; set; }
        public object? DcryptedSignedInvoice { get; set; }
        public SellerDetails? Seller { get; set; }
        public BuyerDetails? Buyer { get; set; }
        public DespatchDetails? Despatch { get; set; }
        public ShipDetails? Ship { get; set; }
        public List<EInvoiceItem>? Items { get; set; }
        public decimal Amount { get; set; }
        public decimal GstAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class SellerDetails
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Gst { get; set; }
    }

    public class BuyerDetails
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Gst { get; set; }
    }

    public class DespatchDetails
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
    }

    public class ShipDetails
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
    }

    public class EInvoiceItem
    {
        public string? PrdDesc { get; set; }
        public string? HsnCd { get; set; }
        public decimal Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotAmt { get; set; }
        public decimal CgstAmt { get; set; }
        public decimal SgstAmt { get; set; }
        public decimal IgstAmt { get; set; }
    }
}