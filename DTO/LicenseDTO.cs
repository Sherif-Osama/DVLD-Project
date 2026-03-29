using System;

namespace DTO
{
    public class LicenseDTO
    {
        public int LicenseID { set; get; }
        public int ApplicationID { set; get; }
        public int DriverID { set; get; }
        public int LicenseClassID { set; get; }
        public DateTime IssueDate { set; get; }
        public DateTime ExpirationDate { set; get; }
        public string Notes { set; get; }
        public float PaidFees { set; get; }
        public bool IsActive { set; get; }
        public byte IssueReason { set; get; }
        public int CreatedByUserID { get; set; }
    }
}
