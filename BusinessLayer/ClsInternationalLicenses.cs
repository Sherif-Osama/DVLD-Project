using DataAccessLayer;
using DTO;
using System;
using System.Data;
using System.Transactions;

namespace BusinessLayer
{

    public class ClsInternationalLicenses : ClsApplications
    {

        public int InternationalLicenseID { set; get; }
        public int DriverID { set; get; }
        public int IssuedUsingLocalLicenseID { set; get; }
        public DateTime IssueDate { set; get; }
        public DateTime ExpirationDate { set; get; }
        public bool IsActive { set; get; }


        public ClsInternationalLicenses()
        {
            InternationalLicenseID = -1;
            DriverID = -1;
            IssuedUsingLocalLicenseID = -1;
            IssueDate = DateTime.Now;
            ExpirationDate = DateTime.Now.AddYears(1);
            IsActive = true;
            ApplicationType = EnApplicationType.NewInternationalLicense;
            ApplicationTypeID = (int)ApplicationType;
            ApplicationTypeInfo = ClsApplicationsTypes.Find(ApplicationTypeID);
        }

        // Private constructor to initialize object from a DTO (loaded from DB)
        private ClsInternationalLicenses(InternationalLicenseDTO ILDTO) : base(ILDTO)
        {
            // Map international-license-specific fields
            this.InternationalLicenseID = ILDTO.InternationalLicenseID;
            this.DriverID = ILDTO.DriverID;
            this.IssuedUsingLocalLicenseID = ILDTO.IssuedUsingLocalLicenseID;
            this.IssueDate = ILDTO.IssueDate;
            this.ExpirationDate = ILDTO.ExpirationDate;
            this.IsActive = ILDTO.IsActive;
        }

        // Map this business object to a DTO for persistence
        private InternationalLicenseDTO MappingToDTO()
        {
            return new InternationalLicenseDTO
            {
                InternationalLicenseID = this.InternationalLicenseID,
                ApplicationID = this.ApplicationID,
                DriverID = this.DriverID,
                IssuedUsingLocalLicenseID = this.IssuedUsingLocalLicenseID,
                IssueDate = this.IssueDate,
                ExpirationDate = this.ExpirationDate,
                IsActive = this.IsActive,
                CreatedByUserID = this.CreatedByUserID
            };
        }

        // Data retrieval helpers
        public static DataTable GetAllInternationalLicenses() => ClsInternationalLicenseData.GetAllInternationalLicenses();
        public static DataTable GetDriverLicenses(int DriverID) => ClsInternationalLicenseData.GetDriverLicenses(DriverID);

        #region Find Methods
        // Find by international license ID
        public new static ClsInternationalLicenses Find(int InternationalLicenseID)
        {
            InternationalLicenseDTO InternationalLicenseDTO = ClsInternationalLicenseData.Find(InternationalLicenseID);
            if (InternationalLicenseDTO == null) { return null; }
            return new ClsInternationalLicenses(InternationalLicenseDTO);
        }

        // Find by driver ID (returns the international license for a driver)
        public static ClsInternationalLicenses FindByDriverID(int DriverID)
        {
            InternationalLicenseDTO InternationalLicenseDTO = ClsInternationalLicenseData.FindByDriverID(DriverID);

            if (InternationalLicenseDTO == null) { return null; }

            return new ClsInternationalLicenses(InternationalLicenseDTO);
        }

        // Get the active international license for a specific driver
        public static ClsInternationalLicenses GetActiveInternationalLicenseByDriverID(int DriverID)
        {
            InternationalLicenseDTO InternationalLicenseDTO = ClsInternationalLicenseData.GetActiveInternationalLicenseByDriverID(DriverID);

            if (InternationalLicenseDTO == null) { return null; }

            return new ClsInternationalLicenses(InternationalLicenseDTO);
        }
        #endregion

        // Insert a new international license record via data layer
        private bool AddNew()
        {
            this.InternationalLicenseID = ClsInternationalLicenseData.AddNew(MappingToDTO());
            return (this.InternationalLicenseID != -1);
        }

        // Update an existing international license record via data layer
        private bool Update() => ClsInternationalLicenseData.Update(MappingToDTO());

        private bool BusinessRules()
        {
            ClsInternationalLicenses License = FindByDriverID(this.DriverID);

            if (Mode == EnMode.AddNew)
            {
                ClsLicenses LocalLicenses = ClsLicenses.Find(this.IssuedUsingLocalLicenseID);
                if (LocalLicenses != null)
                {
                    bool IsExpired = LocalLicenses.ExpirationDate < DateTime.Now;

                    return ((License == null || License?.ExpirationDate < DateTime.Now) && LocalLicenses.IsActive && LocalLicenses.LicenseClassID == 3 && !IsExpired);
                }
                else { return false; }
            }
            else if (Mode == EnMode.Update) { return (License != null); }

            return false;
        }

        public override bool Save()
        {
            if (BusinessRules())
            {
                bool IsNew = (this.Mode == EnMode.AddNew);

                using (TransactionScope Scope = new TransactionScope())
                {
                    // Persist shared application fields first (base class)
                    if (!base.Save())
                        return false;

                    // Persist international-license-specific data
                    if (IsNew)
                    {
                        if (AddNew())
                        {
                            if (this.SetComplete())
                            { this.Mode = EnMode.Update; Scope.Complete(); return true; }
                            return false;
                        }
                        return false;
                    }
                    else
                    {
                        if (Update()) { Scope.Complete(); return true; }
                        return false;
                    }
                }
            }
            return false;
        }
    }
}