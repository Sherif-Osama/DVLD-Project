using DataAccessLayer;
using DTO;
using System;
using System.Data;
using System.Threading.Tasks;
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
        public static Task<DataTable> GetAllInternationalLicensesAsync() => ClsInternationalLicenseData.GetAllInternationalLicensesAsync();
        public static Task<DataTable> GetDriverLicensesAsync(int DriverID) => ClsInternationalLicenseData.GetDriverLicensesAsync(DriverID);

        #region Find Methods
        // Find by international license ID
        public new static async Task<ClsInternationalLicenses> FindAsync(int InternationalLicenseID)
        {
            InternationalLicenseDTO InternationalLicenseDTO = await ClsInternationalLicenseData.FindAsync(InternationalLicenseID);
            if (InternationalLicenseDTO == null) { return null; }

            return new ClsInternationalLicenses(InternationalLicenseDTO);
        }

        // Find by driver ID (returns the international license for a driver)
        public static async Task<ClsInternationalLicenses> FindByDriverIDAsync(int DriverID)
        {
            InternationalLicenseDTO InternationalLicenseDTO = await ClsInternationalLicenseData.FindByDriverIDAsync(DriverID);

            if (InternationalLicenseDTO == null) { return null; }

            return new ClsInternationalLicenses(InternationalLicenseDTO);
        }

        // Get the active international license for a specific driver
        public static async Task<ClsInternationalLicenses> GetActiveInternationalLicenseByDriverIDAsync(int DriverID)
        {
            InternationalLicenseDTO InternationalLicenseDTO = await ClsInternationalLicenseData.GetActiveInternationalLicenseByDriverIDAsync(DriverID);

            if (InternationalLicenseDTO == null) { return null; }

            return new ClsInternationalLicenses(InternationalLicenseDTO);
        }
        #endregion

        // Insert a new international license record via data layer
        private async Task<bool> AddNewAsync()
        {
            this.InternationalLicenseID = await ClsInternationalLicenseData.AddNewAsync(MappingToDTO());
            return (this.InternationalLicenseID != -1);
        }

        // Update an existing international license record via data layer
        private Task<bool> UpdateAsync() => ClsInternationalLicenseData.UpdateAsync(MappingToDTO());

        private async Task<bool> BusinessRulesAsync()
        {
            ClsInternationalLicenses License = await FindByDriverIDAsync(this.DriverID);
            if (Mode == EnMode.AddNew)
            {
                ClsLicenses LocalLicenses = await ClsLicenses.FindAsync(this.IssuedUsingLocalLicenseID);
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

        public override async Task<bool> SaveAsync()
        {
            if (await BusinessRulesAsync())
            {
                bool IsNew = (this.Mode == EnMode.AddNew);

                using (TransactionScope Scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // Persist shared application fields first (base class)
                    if (!await base.SaveAsync())
                        return false;

                    // Persist international-license-specific data
                    if (IsNew)
                    {
                        if (await AddNewAsync())
                        {
                            if (await this.SetCompleteAsync())
                            { this.Mode = EnMode.Update; Scope.Complete(); return true; }
                            return false;
                        }
                        return false;
                    }
                    else
                    {
                        if (await UpdateAsync()) { Scope.Complete(); return true; }
                        return false;
                    }
                }
            }
            return false;
        }
    }
}