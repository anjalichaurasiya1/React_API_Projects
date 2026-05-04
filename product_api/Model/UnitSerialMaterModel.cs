using System;

namespace product_api.Models
{
    public class UnitSerialMasterModel
    {
 private int unitid;
        private int jobid;
        private int srfrom;
        private int srto;
        private int srid;
        private int printed;
        private int balance;
        private string? unit;
        private int qty;


        public int UnitId
        {
            get { return unitid; }
            set { unitid = value; }
        }

        public int JobId
        {
            get { return jobid; }
            set { jobid = value; }
        }
        public int SrFrom
        {
            get { return srfrom; }
            set { srfrom = value; }
        }
        public int SrTo
        {
            get { return srto; }
            set { srto = value; }
        }
        public int SrId
        {
            get { return srid; }
            set { srid = value; }
        }

        public int Printed
        {
            get { return printed; }
            set { printed = value; }
        }
        public int Balance
        {
            get { return balance; }
            set { balance = value; }
        }
        public string? Unit
        {
            get { return unit; }
            set { unit = value; }
        }
        public int Qty
        {
            get { return qty; }
            set { qty = value; }
        }
    }
}