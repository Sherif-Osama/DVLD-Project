using DataAccessLayer;
using DTO;
using System.Data;

namespace BusinessLayer
{
    public class ClsApplicationsTypes
    {
        // Properties representing Application Type data
        public int ID { set; get; }
        public string Title { set; get; }
        public float Fees { set; get; }

        #region Constructors
        // Default constructor initializing default values
        public ClsApplicationsTypes()
        {
            this.ID = -1;
            this.Title = string.Empty;
            this.Fees = 0;
        }

        // Private constructor that initializes the object from a DTO
        private ClsApplicationsTypes(ApplicationTypesDTO TypesDTO)
        {
            this.ID = TypesDTO.ID;
            this.Title = TypesDTO.Title;
            this.Fees = TypesDTO.Fees;
        }
        #endregion
        // Converts current object to DTO for database operations
        private ApplicationTypesDTO MappingToDTO()
        {
            return new ApplicationTypesDTO
            {
                ID = this.ID,
                Title = this.Title,
                Fees = this.Fees,
            };
        }
        // Retrieves all application types from the database
        public static DataTable GetAllApplicationsTypes() => ClsApplicationsTypesData.GetAllApplicationsTypes();
        #region Find Methods
        // Finds an application type by ID
        public static ClsApplicationsTypes Find(int ApplicationTypeID)
        {
            ApplicationTypesDTO ApplicationData = ClsApplicationsTypesData.Find(ApplicationTypeID);

            if (ApplicationData == null) return null;

            return new ClsApplicationsTypes(ApplicationData);
        }

        // Finds an application type by title
        public static ClsApplicationsTypes Find(string ApplicationTitle)
        {
            ApplicationTypesDTO ApplicationData = ClsApplicationsTypesData.Find(ApplicationTitle);

            if (ApplicationData == null) return null;

            return new ClsApplicationsTypes(ApplicationData);
        }
        #endregion
        #region Update/Save Methods
        // Updates the current application type in the database
        public bool UpdateApplicationsTypes() => ClsApplicationsTypesData.UpdateApplicationsTypes(MappingToDTO());

        // Saves the current object (updates if exists, prevents duplicates)
        public bool Save()
        {
            ClsApplicationsTypes ApplicationData = Find(this.Title);

            if (ApplicationData == null || this.ID == ApplicationData.ID)
            {
                return this.UpdateApplicationsTypes();
            }

            // Return false if another record with the same title exists
            return false;
        }
        #endregion
    }
}
