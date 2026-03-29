using DataAccessLayer;
using DTO;
using System;
using System.Data;

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
            PersonInfo = ClsPerson.Find(PersonID);
            CreatedByUserID = driverDTO.CreatedByUserID;
            CreatedDate = driverDTO.CreatedDate;
            Mode = EnMode.Update;
        }

        // Retrieve a DataTable with all drivers.
        public static DataTable GetAllDrivers() => ClsDriverData.GetAllDrivers();

        // Convenience helpers to get licenses for a driver.
        public static DataTable GetAllLocalLicenses(int DriverID) => ClsLicenses.GetDriverLicenses(DriverID);
        public static DataTable GetAllInternationalLicenses(int DriverID) => ClsInternationalLicenses.GetDriverLicenses(DriverID);

        // Find driver by the associated person ID. Returns null when not found.
        public static ClsDriver FindByPersonID(int PersonID)
        {
            DriverDTO DriverDTO = ClsDriverData.FindByPersonID(PersonID);
            if (DriverDTO == null) { return null; }
            return new ClsDriver(DriverDTO);
        }

        // Find driver by driver ID. Returns null when not found.
        public static ClsDriver Find(int DriverID)
        {
            DriverDTO DriverDTO = ClsDriverData.Find(DriverID);

            if (DriverDTO == null) { return null; }

            return new ClsDriver(DriverDTO);
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

        // Insert a new driver record via the data layer. Sets DriverID on success.
        private bool AddNew()
        {
            this.DriverID = ClsDriverData.AddNew(MappingToDTO());
            return this.DriverID != -1;
        }

        // Update an existing driver record via the data layer.
        private bool Update() => ClsDriverData.Update(MappingToDTO());

        private bool BusinessRules()
        {
            ClsDriver ExistingDriver = ClsDriver.FindByPersonID(this.PersonID);
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

        public bool Save()
        {
            if (BusinessRules())
            {
                switch (this.Mode)
                {
                    case EnMode.AddNew:
                        if (AddNew()) { this.Mode = EnMode.Update; return true; }
                        return false;
                    case EnMode.Update:
                        return Update();
                }
            }
            return false;
        }
    }
}