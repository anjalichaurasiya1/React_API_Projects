using System;

namespace product_api.Models
{
    public class UnitmasterModel
    {
        private string? phone;
        private int unitid;
        private int jobid;
        private string? unit;
        private string? address;

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
        public string? Unit
        {
            get { return unit; }
            set { unit = value; }
        }
        public string? Address
        {
            get { return address; }
            set { address = value; }
        }
        public string? Phone
        {
            get { return phone; }
            set { phone = value; }
        }
    }
}