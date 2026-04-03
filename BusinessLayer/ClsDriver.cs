using DataAccessLayer;
using DTO;
using System;
using System.Data;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class ClsDriver
    {
        public enum EnMode { AddNew = 1, Update = 2 }

        public EnMode Mode { get; private set; }
        // Primary key for the driver record.
        public int DriverID { get; set; }
        public int PersonID { get; set; }
        public ClsPerson PersonInfo { get; private set; }
        public int CreatedByUserID { get; set; }
        public DateTime CreatedDate { get; set; }

        public ClsDriver()
        {
            DriverID = -1;
            PersonID = -1;
            CreatedByUserID = -1;
            CreatedDate = DateTime.Now;
            Mode = EnMode.AddNew; // New instances default to AddNew mode.
        }

        // Private ctor used when instantiating from a DTO (loaded from the data store).
        private ClsDriver(DriverDTO driverDTO)
        {
            DriverID = driverDTO.DriverID;
            PersonID = driverDTO.PersonID;
            CreatedByUserID = driverDTO.CreatedByUserID;
            CreatedDate = driverDTO.CreatedDate;
            Mode = EnMode.Update;
        }

        // Map this business object to a DTO used by the data layer.
        private DriverDTO MappingToDTO()
        {
            return new DriverDTO
            {
                DriverID = this.DriverID,
                PersonID = this.PersonID,
                CreatedByUserID = this.CreatedByUserID,
                CreatedDate = this.CreatedDate
            };
        }

        // Retrieve a DataTable with all drivers.
        public static Task<DataTable> GetAllDriversAsync() => ClsDriverData.GetAllDriversAsync();

        // Convenience helpers to get licenses for a driver.
        public static Task<DataTable> GetAllLocalLicensesAsync(int DriverID) => ClsLicenses.GetDriverLicensesAsync(DriverID);

        public static Task<DataTable> GetAllInternationalLicensesAsync(int DriverID) => ClsInternationalLicenses.GetDriverLicensesAsync(DriverID);

        #region Find Methods

        private async Task LoadRelatedDataAsync()
        {
            this.PersonInfo = await ClsPerson.FindAsync(this.PersonID);
        }

        // Find driver by the associated person ID. Returns null when not found.
        public static async Task<ClsDriver> FindByPersonIDAsync(int PersonID)
        {
            DriverDTO DriverDTO = await ClsDriverData.FindByPersonIDAsync(PersonID);
            if (DriverDTO == null) { return null; }

            ClsDriver DriverObj = new ClsDriver(DriverDTO);

            await DriverObj.LoadRelatedDataAsync();

            return DriverObj;
        }

        // Find driver by driver ID. Returns null when not found.
        public static async Task<ClsDriver> FindAsync(int DriverID)
        {
            DriverDTO DriverDTO = await ClsDriverData.FindAsync(DriverID);

            if (DriverDTO == null) { return null; }

            ClsDriver DriverObj = new ClsDriver(DriverDTO);

            await DriverObj.LoadRelatedDataAsync();

            return DriverObj;
        }
        #endregion

        // Insert a new driver record via the data layer. Sets DriverID on success.
        private async Task<bool> AddNewAsync()
        {
            this.DriverID = await ClsDriverData.AddNewAsync(MappingToDTO());
            return this.DriverID != -1;
        }

        // Update an existing driver record via the data layer.
        private Task<bool> UpdateAsync() => ClsDriverData.UpdateAsync(MappingToDTO());

        private async Task<bool> BusinessRulesAsync()
        {
            ClsDriver ExistingDriver = await ClsDriver.FindByPersonIDAsync(this.PersonID);
            if (Mode == EnMode.AddNew)
            {
                return (ExistingDriver == null);
            }
            else if (Mode == EnMode.Update)
            {
                return (ExistingDriver != null && ExistingDriver.DriverID == this.DriverID);
            }

            return false;
        }

        public async Task<bool> SaveAsync()
        {
            if (await BusinessRulesAsync())
            {
                switch (this.Mode)
                {
                    case EnMode.AddNew:
                        if (await AddNewAsync()) { this.Mode = EnMode.Update; return true; }
                        return false;
                    case EnMode.Update:
                        return await UpdateAsync();
                }
            }
            return false;
        }
    }
}