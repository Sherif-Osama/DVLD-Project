using DataAccessLayer;
using DTO;
using System;
using System.Data;
using System.Threading.Tasks;
namespace BusinessLayer
{
    // Represents a detained driving license record and related operations.
    public class ClsDetainedLicenses
    {
        public enum EnMode { AddNew = 1, Update = 2 };

        public EnMode Mode { private set; get; }

        public int DetainID { set; get; }

        public int LicenseID { set; get; }

        public DateTime DetainDate { set; get; }

        public float FineFees { set; get; }

        public int CreatedByUserID { set; get; }

        public bool IsReleased { set; get; }

        public DateTime ReleaseDate { set; get; }

        public int ReleasedByUserID { set; get; }

        public int ReleaseApplicationID { set; get; }

        // Default constructor for creating a new detained license (unsaved).
        public ClsDetainedLicenses()
        {
            this.DetainID = -1;
            this.LicenseID = -1;
            this.DetainDate = DateTime.Now;
            this.FineFees = 0;
            this.CreatedByUserID = -1;
            this.IsReleased = false;
            this.ReleaseDate = DateTime.MinValue;
            this.ReleasedByUserID = -1;
            this.ReleaseApplicationID = -1;

            // New instance should be added (not updated) by default.
            this.Mode = EnMode.AddNew;
        }

        // Construct from a DTO (used when loading from data store).
        public ClsDetainedLicenses(DetainedLicenseDTO dto)
        {
            this.DetainID = dto.DetainID;
            this.LicenseID = dto.LicenseID;
            this.DetainDate = dto.DetainDate;
            this.FineFees = dto.FineFees;
            this.CreatedByUserID = dto.CreatedByUserID;
            this.IsReleased = dto.IsReleased;
            this.ReleaseDate = dto.ReleaseDate;
            this.ReleasedByUserID = dto.ReleasedByUserID;
            this.ReleaseApplicationID = dto.ReleaseApplicationID;

            // Instance loaded from DB should be in update mode.
            this.Mode = EnMode.Update;
        }

        // Map the current instance to a DTO for persistence operations.
        private DetainedLicenseDTO MappingToDTO()
        {
            return new DetainedLicenseDTO()
            {
                DetainID = this.DetainID,
                LicenseID = this.LicenseID,
                DetainDate = this.DetainDate,
                FineFees = this.FineFees,
                CreatedByUserID = this.CreatedByUserID,
                IsReleased = this.IsReleased,
                ReleaseDate = this.ReleaseDate,
                ReleasedByUserID = this.ReleasedByUserID,
                ReleaseApplicationID = this.ReleaseApplicationID
            };
        }

        // Static helper to get all detained license records.
        public static Task<DataTable> GetAllDetainedLicensesAsync() => ClsDetainedLicenseData.GetAllDetainedLicensesAsync();

        // Find a detained license by its DetainID.
        public static async Task<ClsDetainedLicenses> FindAsync(int DetainID)
        {
            DetainedLicenseDTO DLDTO = await ClsDetainedLicenseData.FindAsync(DetainID);

            if (DLDTO == null) { return null; }

            return new ClsDetainedLicenses(DLDTO);
        }

        // Find a detained license record by the detained LicenseID (the license itself).
        public static async Task<ClsDetainedLicenses> FindByLicenseIDAsync(int LicenseID)
        {
            DetainedLicenseDTO DLDTO = await ClsDetainedLicenseData.FindByLicenseIDAsync(LicenseID);

            if (DLDTO == null) { return null; }

            return new ClsDetainedLicenses(DLDTO);
        }

        // Check if a given license is currently detained.
        public static Task<bool> IsLicenseDetainedAsync(int LicenseID) => ClsDetainedLicenseData.IsLicenseDetainedAsync(LicenseID);

        // Release the detained license: mark released fields and persist changes.
        public async Task<bool> ReleaseAsync(int ReleasedByUserID, int ApplicationID)
        {
            this.IsReleased = true;
            this.ReleaseDate = DateTime.Now;
            this.ReleasedByUserID = ReleasedByUserID;
            this.ReleaseApplicationID = ApplicationID;

            return await this.SaveAsync();
        }

        // Add a new detained license record to the data store.
        private async Task<bool> AddNewAsync()
        {
            this.DetainID = await ClsDetainedLicenseData.AddNewAsync(MappingToDTO());
            return (this.DetainID != -1);
        }

        // Update an existing detained license record in the data store.
        private Task<bool> UpdateAsync() => ClsDetainedLicenseData.UpdateAsync(MappingToDTO());

        private async Task<bool> BusinessRulesAsync()
        {
            if (Mode == EnMode.AddNew)
            { return !await IsLicenseDetainedAsync(this.LicenseID); }
            else if (Mode == EnMode.Update)
            { return await IsLicenseDetainedAsync(this.LicenseID); }

            return false;
        }

        // Save the current instance by respecting business rules and mode.
        public async Task<bool> SaveAsync()
        {
            if (await BusinessRulesAsync())
            {
                switch (Mode)
                {
                    case EnMode.AddNew:
                        if (await AddNewAsync()) { Mode = EnMode.Update; return true; }
                        return false;
                    case EnMode.Update:
                        return await UpdateAsync();
                    default:
                        return false;
                }
            }
            return false;
        }
    }
}