using DataAccessLayer;
using DTO;
using System;
using System.Data;
using System.Threading.Tasks;

namespace BusinessLayer
{
    // Business logic class for managing person-related operations
    public class ClsPerson
    {
        // Enum to track whether this object is for adding or updating a person
        public enum EnGender { Male = 0, Female = 1 }
        public enum EnMode { AddNew = 1, Update = 2 };
        public EnMode Mode { get; private set; }
        public int PersonID { get; private set; }
        public string NationalNo { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string ThirdName { get; set; }
        public string LastName { get; set; }
        public string FullName
        { get => $"{FirstName} {SecondName} {ThirdName} {LastName}"; }
        public DateTime DateOfBirth { get; set; }
        public EnGender Gender { set; get; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int NationalityCountryID { get; set; }
        public ClsCountry Country { get; private set; }
        public string ImagePath { get; set; }

        #region Constructors
        // Constructor for loading existing person from DB
        private ClsPerson(PersonDTO PersonInfoDTO)
        {
            this.PersonID = PersonInfoDTO.PersonID;
            this.NationalNo = PersonInfoDTO.NationalNo;
            this.FirstName = PersonInfoDTO.FirstName;
            this.SecondName = PersonInfoDTO.SecondName;
            this.ThirdName = PersonInfoDTO.ThirdName;
            this.LastName = PersonInfoDTO.LastName;
            this.DateOfBirth = PersonInfoDTO.DateOfBirth;
            this.Gender = (EnGender)PersonInfoDTO.Gender;
            this.Address = PersonInfoDTO.Address;
            this.Phone = PersonInfoDTO.Phone;
            this.Email = PersonInfoDTO.Email;
            this.NationalityCountryID = PersonInfoDTO.NationalityCountryID;
            this.ImagePath = PersonInfoDTO.ImagePath;
            Mode = EnMode.Update;
        }

        // Default constructor for creating a new person
        public ClsPerson()
        {
            PersonID = -1;
            NationalNo = string.Empty;
            FirstName = string.Empty;
            SecondName = string.Empty;
            ThirdName = string.Empty;
            LastName = string.Empty;
            DateOfBirth = DateTime.Now;
            Gender = EnGender.Male;
            Address = string.Empty;
            Phone = string.Empty;
            Email = string.Empty;
            NationalityCountryID = -1;
            ImagePath = string.Empty;
            Mode = EnMode.AddNew;
        }
        #endregion
        // Convert this object to DTO for DAL operations
        private PersonDTO MappingToDTO()
        {
            return new PersonDTO()
            {
                PersonID = this.PersonID,
                NationalNo = this.NationalNo,
                FirstName = this.FirstName,
                SecondName = this.SecondName,
                ThirdName = this.ThirdName,
                LastName = this.LastName,
                DateOfBirth = this.DateOfBirth,
                Gender = (int)this.Gender,
                Address = this.Address,
                Phone = this.Phone,
                Email = this.Email,
                NationalityCountryID = this.NationalityCountryID,
                ImagePath = this.ImagePath,
            };
        }

        // Get all people from DB
        static public Task<DataTable> GetAllPeopleAsync() => ClsPeopleData.GetAllPeopleAsync();

        #region Existence Check Methods
        // Check if a person exists by National Number
        static public Task<bool> PersonExistsAsync(string NationalNo) => ClsPeopleData.PersonExistsAsync(NationalNo);

        // Check if a person exists by ID
        static public Task<bool> PersonExistsAsync(int ID) => ClsPeopleData.PersonExistsAsync(ID);
        #endregion
        #region Find Methods

        private async Task LoadRelatedDataAsync()
        {
            this.Country = await ClsCountry.FindAsync(this.NationalityCountryID);
        }

        // Find person by ID
        static public async Task<ClsPerson> FindAsync(int PersonID)
        {
            PersonDTO PersonInfoDTO = await ClsPeopleData.FindAsync(PersonID);
            if (PersonInfoDTO == null) return null; // Return null if not found

            ClsPerson PersonObj = new ClsPerson(PersonInfoDTO);

            await PersonObj.LoadRelatedDataAsync();

            return PersonObj;
        }

        // Find person by National Number
        static public async Task<ClsPerson> FindAsync(string NationalNo)
        {
            PersonDTO PersonInfoDTO = await ClsPeopleData.FindAsync(NationalNo);
            if (PersonInfoDTO == null) return null; // Return null if not found

            ClsPerson PersonObj = new ClsPerson(PersonInfoDTO);

            await PersonObj.LoadRelatedDataAsync();

            return PersonObj;
        }
        #endregion
        #region Add/Update/Delete Methods

        private async Task<bool> BusinessRulesAsync()
        {
            if (this.Mode == EnMode.AddNew)
            {
                return !await PersonExistsAsync(this.NationalNo); // Ensure national number is unique
            }
            else if (this.Mode == EnMode.Update)
            {
                PersonDTO Person = await ClsPeopleData.FindAsync(this.NationalNo);
                return Person == null || this.PersonID == Person?.PersonID;
            }
            return true;
        }

        // Add a new person to the database
        private async Task<bool> AddNewPersonAsync()
        {
            PersonID = await ClsPeopleData.AddNewPersonAsync(MappingToDTO());

            return PersonID != -1; // Return true if added successfully
        }

        // Update existing person in the database
        private Task<bool> UpdatePersonAsync() => ClsPeopleData.UpdatePersonInfoAsync(MappingToDTO());

        // Save the person (insert if new, update if existing)
        public async Task<bool> SaveAsync()
        {
            if (await BusinessRulesAsync())
            {
                switch (Mode)
                {
                    case EnMode.AddNew:
                        if (await AddNewPersonAsync()) { Mode = EnMode.Update; return true; }
                        return false;

                    case EnMode.Update:
                        return await UpdatePersonAsync();
                }
            }
            return false;
        }
        public static Task<bool> DeleteAsync(int PersonID) => ClsPeopleData.DeleteAsync(PersonID);
    }
    #endregion
}